using Microsoft.SharePoint.Administration;
using Rede.PN.AtendimentoDigital.SharePoint.Core.OrdemServico;
using Rede.PN.AtendimentoDigital.SharePoint.MaximoServico;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace Rede.PN.AtendimentoDigital.SharePoint.Handlers
{
    public class Maximo : HandlerBase
    {
        /// <summary>
        /// Consulta de terminais POO e POS
        /// </summary>
        /// <returns>Retorna os terminais do usuário</returns>
        [HttpGet]
        public HandlerResponse ConsultaTerminais()
        {
            try
            {
                HandlerResponse response = new HandlerResponse();

                //Prepara parâmetros para consulta no Sistema Máximo
                FiltroTerminal filtroTerminal = new FiltroTerminal();
                filtroTerminal.PontoVenda = Sessao.CodigoEntidade.ToString();
                filtroTerminal.Situacao = TipoTerminalStatus.EMPRODUCAO;

                List<TerminalDetalhado> listResult = null;

                //Consulta Sistema Máximo
                using (var contexto = new ContextoWCF<MaximoServicoClient>())
                {
                    listResult = contexto.Cliente.ConsultarTerminalDetalhado(filtroTerminal).ToList();
                }

                //Mantem apenas os Terminais com "Tipo Equipamento" POO e POS
                if (listResult != null)
                {
                    response.Dados = listResult.Where(
                        terminal =>
                        {
                            return String.Compare(terminal.TipoEquipamento, "POO", true) == 0
                                || String.Compare(terminal.TipoEquipamento, "POS", true) == 0;
                        }).ToList();
                }


                if (listResult == null || listResult.Count == 0)
                {
                    response.Codigo = 0;
                    response.DetalhesErro = "Não há tecnologia cadastrada.";
                }

                return response;
            }
            catch (FaultException<GeneralFault> ex)
            {
                Logger.GravarErro("Erro ao consultar os terminais", ex);
                return new HandlerResponse(
                    301,
                    "Erro ao consultar os terminais",
                    new
                    {
                        Codigo = ex.Detail.Codigo,
                        CodeName = ex.Code != null ? ex.Code.Name : null,
                        CodeSubcode = ex.Code != null ? ex.Code.SubCode : null
                    }, null);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro ao consultar os terminais", ex);
                return new HandlerResponse(
                    HandlerBase.CodigoErro,
                    "Erro ao consultar os terminais");
            }
        }

        /// <summary>
        /// Consulta o endereço do usuário.
        /// </summary>
        /// <returns>Retorna dados tratado</returns>
        [HttpPost]
        public HandlerResponse CriarOrdemServico()
        {
            //out String protocoloOS, out Nullable<DateTime> dataProgramada
            using (Logger Log = Logger.IniciarLog("Criação de OS"))
            {
                Nullable<DateTime> dataProgramada = null;
                String strProtocoloOS = String.Empty;

                try
                {
                    Log.GravarMensagem("dados", base.Request["dados"]);

                    OrdemServico ordemServico = new JavaScriptSerializer().Deserialize<OrdemServico>(Convert.ToString(base.Request["dados"].Trim()));

                    using (var contexto = new ContextoWCF<MaximoServicoClient>())
                    {
                        //Monta objetos para criação de OS no Sistema Máximo
                        OSCriacao os = this.RecuperaDadosCriacaoOS(ordemServico);

                        //Valida se o terminal é do estabelecimento
                        if (!this.ValidarTerminal(os.NumeroLogico))
                        {
                            return new HandlerResponse(301, "Ocorreu um erro na criação da Ordem de Serviço: número do terminal inválido.");
                        }

                        //Efetua a criação da OS
                        strProtocoloOS = contexto.Cliente.CriarOS(os, out dataProgramada);
                        var comprovanteOS = new
                        {
                            strProtocoloOS = strProtocoloOS,
                            dtDataProgramada = dataProgramada.GetValueOrDefault(DateTime.MinValue).ToString("dd/MM/yyyy"),
                            dtSolicitacao = DateTime.Now.ToString("dd/MM/yyyy"),
                            hrSolicitacao = DateTime.Now.ToString("HH:mm"),
                            strEmailUsuario = os.Contato.Email
                        };

                        if (!String.IsNullOrEmpty(strProtocoloOS))
                        {
                            ordemServico.Contato.Celular = FormatarTelefone(ordemServico.Contato.Celular);
                            ordemServico.Contato.Telefone = FormatarTelefone(ordemServico.Contato.Telefone);

                            var email = new EmailHandler(base.CurrentSPContext, base.Sessao);
                            email.EnviarEmailSuporteMaquininha(base.CurrentSPContext.Web, ordemServico, dataProgramada.GetValueOrDefault(DateTime.MinValue).ToString("dd/MM/yyyy"), strProtocoloOS);

                            return new HandlerResponse(comprovanteOS);
                        }
                        else
                        {
                            return new HandlerResponse(302, "Ocorreu um erro na criação da Ordem de Serviço.");
                        }

                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    return new HandlerResponse(303, ex.Message, new
                    {
                        Codigo = ex.Detail != null ? ex.Detail.Codigo : 0,
                        CodeName = ex.Code != null ? ex.Code.Name : null,
                        CodeSubcode = ex.Code != null ? ex.Code.SubCode : null
                    }, null);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    return new HandlerResponse(304, ex.Message);
                }
            }
        }

        /// <summary>
        /// Recupera os dados apartir da requisição base
        /// </summary>
        /// <returns>
        /// retorna um objeto OSCriacao
        /// </returns>
        private OSCriacao RecuperaDadosCriacaoOS(OrdemServico ordemServico)
        {
            var os = new OSCriacao();
            var campo = string.Empty;
            try
            {
                if (ordemServico != null)
                {
                    //Preenche dados do endereço
                    if (ordemServico.EnderecoAtendimento != null)
                    {
                        os.EnderecoAtendimento = new Endereco
                        {
                            Bairro = ordemServico.EnderecoAtendimento.Bairro.RemoverEspacos(),
                            Cep = ordemServico.EnderecoAtendimento.Cep.RemoverLetras(),
                            Cidade = ordemServico.EnderecoAtendimento.Cidade.RemoverEspacos(),
                            Complemento = ordemServico.EnderecoAtendimento.Complemento.RemoverEspacos(),
                            Estado = (TipoUf)Enum.Parse(typeof(TipoUf), ordemServico.EnderecoAtendimento.Estado),
                            Logradouro = ordemServico.EnderecoAtendimento.Logradouro.RemoverEspacos(),
                            Numero = ordemServico.EnderecoAtendimento.Numero.RemoverLetras(),
                            PontoReferencia = ordemServico.EnderecoAtendimento.PontoReferencia.RemoverEspacos()
                        };
                    }

                    //Preenche dados de contato
                    if (ordemServico.Contato != null)
                    {
                        os.Contato = new Contato
                        {
                            Celular = FormatarTelefone(ordemServico.Contato.Celular),
                            Email = ordemServico.Contato.Email.RemoverEspacos(),
                            Nome = ordemServico.Contato.Nome.RemoverEspacos(),
                            Telefone = FormatarTelefone(ordemServico.Contato.Telefone)
                        };
                    }

                    //Recuperar lista de horarios de atendimento
                    if (ordemServico.HorarioAtendimento == null || !ordemServico.HorarioAtendimento.Any())
                        throw new Exception("não foi informado horário para atendimento");

                    if (ordemServico.HorarioAtendimento.Any(hor => (hor == null) || (hor.Dia < 0)))
                        throw new Exception("Dia de atendimento inválido");

                    if (ordemServico.HorarioAtendimento.Any(hor => ((hor.HoraAs != 0) && (hor.HoraDas > hor.HoraAs))))
                        throw new Exception("horário de atendimento inválido");

                    os.HorarioAtendimento = ordemServico.HorarioAtendimento.Select(hor => new Horario
                    {
                        Dia = (TipoDia)hor.Dia,
                        Inicio = new DateTime().AddHours(hor.HoraDas),
                        Termino = new DateTime().AddHours(hor.HoraAs)
                    }).ToList();

                    //
                    os.NumeroLogico = ordemServico.NumeroLogico;
                    os.TipoEquipamento = ordemServico.TipoEquipamento;
                    os.Observacao = String.Format("{0} - {1}", ordemServico.ProblemaEncontrado, "Criado via Portal.");
                }

                //Preenche dados do terminal e do PV
                os.PontoVenda = new PontoVenda
                {
                    Numero = Sessao.CodigoEntidade.ToString()
                };

                os.Classificacao = TipoClassificacao.SERVIÇOSDECAMPOTROCADEEQUIPAMENTO;
                os.Prioridade = TipoPrioridade.NORMAL;

                return os;
            }
            catch (NullReferenceException ex)
            {
                throw new Exception("Campo Nulo" + ex.Message, ex.InnerException);
            }
            catch (Exception ex)
            {
                throw new Exception("O campo '" + campo + "' não está preenchido.", ex.InnerException);
            }
        }

        /// <summary>
        /// Verifica se um número do terminal é de um estabelecimento.
        /// </summary>
        /// <param name="numeroLogico">Número lógico a ser verificado</param>
        /// <returns>Terminal válido (existe e pertence ao estabelecimento)</returns>
        private Boolean ValidarTerminal(String numeroLogico)
        {
            var terminalValido = default(Boolean);

            //Prepara parâmetros para consulta no Sistema Máximo
            FiltroTerminal filtroTerminal = new FiltroTerminal();
            filtroTerminal.PontoVenda = Sessao.CodigoEntidade.ToString();
            filtroTerminal.Situacao = TipoTerminalStatus.EMPRODUCAO;

            List<TerminalDetalhado> listResult = null;
            using (var contexto = new ContextoWCF<MaximoServicoClient>())
            {
                listResult = contexto.Cliente.ConsultarTerminalDetalhado(filtroTerminal).ToList();
            }

            if (listResult != null)
                terminalValido = listResult.Any(terminal => String.Compare(terminal.NumeroLogico, numeroLogico, true) == 0);

            return terminalValido;
        }

        /// <summary>
        /// Formata um número de celular/telefone para o valor esperado da OS
        /// </summary>
        /// <param name="telefone">Número do telefone/celular</param>
        /// <returns>Valor formatado</returns>
        private String FormatarTelefone(String telefone)
        {
            String numeros = telefone.RemoverLetras();

            return String.Format("{0} {1}",
                new String(numeros.Take(2).ToArray()),
                new String(numeros.Skip(2).ToArray()));
        }

        /// <summary>
        /// Valida se Para o Pv da Sessão Atual existem Terminais físicos que utilizam bobina.
        /// </summary>
        /// <returns>Existem Terminais físicos que utilizam bobina?</returns>
        [HttpGet]
        public HandlerResponse PossuiTerminalComBobina()
        {
            try
            {
                HandlerResponse response = new HandlerResponse();
                String pv = Sessao.CodigoEntidade.ToString();
                Boolean retorno;

                //Consulta Sistema Máximo
                using (var contexto = new ContextoWCF<MaximoServicoClient>())
                {
                    retorno = contexto.Cliente.PossuiTerminalComBobina(pv);
                }

                return new HandlerResponse(new { possuiTerminalComBobina = retorno });
            }
            catch (FaultException<GeneralFault> ex)
            {
                Logger.GravarErro("Erro ao validar terminais com bobina", ex);
                return new HandlerResponse(
                    301,
                    "Erro ao validar terminais com bobina",
                    new
                    {
                        Codigo = ex.Detail.Codigo,
                        CodeName = ex.Code != null ? ex.Code.Name : null,
                        CodeSubcode = ex.Code != null ? ex.Code.SubCode : null
                    }, null);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro ao validar terminais com bobina", ex);
                return new HandlerResponse(
                    HandlerBase.CodigoErro,
                    "Erro ao validar terminais com bobina");
            }
        }
    }
}