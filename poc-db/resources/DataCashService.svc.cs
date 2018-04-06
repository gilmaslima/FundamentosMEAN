using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.DataCash.Negocio;
using Redecard.PN.Comum;
using AutoMapper;

namespace Redecard.PN.DataCash.Servicos
{
    public class DataCashService : ServicoBase, IDataCashService
    {
        #region [ Usuários E-Commerce ]

        /// <summary>Efetua a troca da senha.</summary>
        /// <param name="mensagemErro">Mensagem de erro de retorno</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="senha">Nova Senha</param>
        /// <returns>Boolean</returns>
        public Boolean TrocarSenha(out Modelos.MensagemErro mensagemErro, Int32 codigoEntidade, String senha)
        {
            using (Logger Log = Logger.IniciarLog("Troca de Senha"))
            {
                Log.GravarLog(EventoLog.InicioServico, codigoEntidade);

                //Variável de retorno
                Boolean retorno = false;

                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.UsuariosEcommerce();

                    retorno = negocio.TrocarSenha(out mensagemErro, codigoEntidade, senha);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno, mensagemErro });

                return retorno;
            }
        }

        /// <summary>Consulta senha cadastrada</summary>
        /// <param name="mensagemErro">Mensagem de erro de retorno</param>
        /// <param name="codigoEntidade">Código entidade</param>
        /// <returns>Senha</returns>
        public String ConsultarSenha(out Modelos.MensagemErro mensagemErro, Int32 codigoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Consulta de Senha"))
            {
                Log.GravarLog(EventoLog.InicioServico, codigoEntidade);

                //Variável de retorno
                String retorno = default(String);

                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.UsuariosEcommerce();

                    retorno = negocio.ConsultarSenha(out mensagemErro, codigoEntidade);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { mensagemErro });

                return retorno;
            }
        }

        #endregion [ FIM: Usuários E-Commerce ]

        #region [ Gerenciamento IP & URLBack ]

        /// <summary>Manutenção de IPs e URLs</summary>
        /// <param name="gerenciamentoPv">Dados para manutenção de IPs e URLs</param>
        /// <param name="mensagemErro">Mensagem de erro de retorno</param>
        /// <returns>Boolean</returns>
        public Boolean ManutencaoGerenciamentoPV(out Modelos.MensagemErro mensagemErro, Modelos.GerenciamentoPV gerenciamentoPv)
        {
            Boolean retorno = false;

            using (Logger Log = Logger.IniciarLog("Manutenção Gerenciamento PV"))
            {
                Log.GravarLog(EventoLog.InicioServico, gerenciamentoPv);

                try
                {
                    //Classe de negócio
                    var negocio = new Negocio.Configuracao();

                    //Chamada de negócio
                    retorno = negocio.ManutencaoGerenciamentoPV(out mensagemErro, gerenciamentoPv);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno, mensagemErro });

                return retorno;
            }
        }

        /// <summary>Consulta de IPs e URLs</summary>
        /// <param name="pv">Código da entidade</param>
        /// <returns>Dados do PV</returns>
        public Modelos.GerenciamentoPV ConsultaGerencimentoPV(Int32 pv)
        {
            Modelos.GerenciamentoPV retorno = null;

            using (Logger Log = Logger.IniciarLog("Consulta Gerenciamento PV"))
            {
                Log.GravarLog(EventoLog.InicioServico, pv);

                try
                {
                    //Classe de negócio
                    var negocio = new Negocio.Configuracao();

                    //Chamada de negócio
                    retorno = negocio.ConsultaGerencimentoPV(pv);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno });

                return retorno;
            }
        }

        #endregion [ FIM: Gerenciamento IP & URLBack ]

        #region [ Performance Autorização ]

        public Modelos.TotalTransacoes GetTotalTransacoes(DateTime dataInicio, DateTime dataFim, Int32 pv)
        {
            Modelos.TotalTransacoes retorno;

            using (Logger Log = Logger.IniciarLog("Performance Autorização"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { dataInicio, dataFim, pv });

                try
                {
                    //Classe de negócio
                    var negocio = new Negocio.Gerenciamento();

                    //Chamada de negócio
                    retorno = negocio.GetTotalTransacoes(dataInicio, dataFim, pv);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno });

                return retorno;
            }
        }

        #endregion

        #region [ Distribuidores ]

        /// <summary>
        /// Consulta os distribuidores cadastrados
        /// </summary>
        /// <param name="pv">Número do PV</param>
        /// <param name="numeroPagina">Número da Página</param>
        /// <returns>RetornoDistribuidores</returns>
        public Modelos.RetornoDistribuidor ConsultarDistribuidores(int pv, int numeroPagina)
        {
            return new Distribuidores().ListarDistribuidor(pv, numeroPagina);
        }

        #endregion

        #region [ Configuração AVS ]

        public String ConsultarRegraAVS(Int32 pv, out Modelos.MensagemErro mensagemErro)
        {
            String retorno = String.Empty;

            using (Logger Log = Logger.IniciarLog("Consulta Regra AVS - Serviço"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { pv });

                try
                {
                    var negocio = new Negocio.Configuracao();

                    retorno = negocio.ConsultarRegraAVS(pv, out mensagemErro);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.RetornoNegocio, new { retorno, mensagemErro });
            }

            return retorno;
        }

        public Boolean GerenciarRegraAVS(Int32 pv, Char avs, out Modelos.MensagemErro mensagemErro)
        {
            Boolean retorno = false;

            using (Logger Log = Logger.IniciarLog("Gerenciar Regra AVS - Serviço"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { pv, avs });

                try
                {
                    var negocio = new Negocio.Configuracao();

                    retorno = negocio.GerenciaRegraAVS(pv, avs, out mensagemErro);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.RetornoNegocio, new { retorno, mensagemErro });
            }

            return retorno;
        }

        public List<GrupoAVS> ConsultarOpcoesAVS(Int32 pv)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Opções AVS"))
            {
                try
                {
                    //Classe de negócio
                    var negocio = new Negocio.Configuracao();

                    //Chamada de negócio
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { pv });
                    var modelo = negocio.ObterGrupoAVS(pv);

                    //Conversão para modelo de serviço
                    var retorno = ConverterOpcoesAVS(modelo);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { retorno, modelo });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        public List<GrupoAVS> AtualizarGrupoAVS(Int32 pv, List<GrupoAVS> lstGrupos)
        {
            using (Logger Log = Logger.IniciarLog("Atualizar Opções AVS"))
            {
                try
                {

                    var lstEnvio = new List<Modelo.Configuracao.GrupoAVS>();

                    lstGrupos.ForEach(item => lstEnvio.Add(new Modelo.Configuracao.GrupoAVS()
                    {
                        IDGrupo = item.IdGrupo,
                        Grupo = item.Grupo,
                        NotMatch = item.NotMatch
                    }));
                    //Classe de negócio
                    var negocio = new Negocio.Configuracao();

                    //Chamada de negócio
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { pv });
                    var lista = negocio.AtualizarGrupoAVS(pv, lstEnvio);

                    //Conversão para modelo de serviço
                    var retorno = ConverterOpcoesAVS(lista);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { retorno, lista });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        private List<GrupoAVS> ConverterOpcoesAVS(List<Modelo.Configuracao.GrupoAVS> itens)
        {
            List<GrupoAVS> lstRetorno = new List<GrupoAVS>();
            foreach (Modelo.Configuracao.GrupoAVS item in itens)
            {
                lstRetorno.Add(new GrupoAVS()
                {
                    IdGrupo = item.IDGrupo,
                    Grupo = item.Grupo,
                    NotMatch = item.NotMatch
                });
            }
            return lstRetorno.OrderBy("IdGrupo").ToList();
        }

        #endregion

        #region [ Configurações Boleto ]

        public Modelos.Boleto ConsultarBoleto(Int32 pv, out Modelos.MensagemErro mensagemErro)
        {
            Modelos.Boleto retorno = new Modelos.Boleto();

            using (Logger Log = Logger.IniciarLog("Consulta Boleto - Serviço"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { pv });

                try
                {
                    var negocio = new Negocio.Configuracao();

                    retorno = negocio.ConsultaBoleto(pv, out mensagemErro);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.RetornoNegocio, new { retorno, mensagemErro });
            }

            return retorno;
        }

        public Boolean GerenciarBoleto(Int32 pv, Modelos.Boleto boleto, out Modelos.MensagemErro mensagemErro)
        {
            Boolean retorno = false;

            using (Logger Log = Logger.IniciarLog("Gerenciar Boleto - Serviço"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { pv, boleto });

                try
                {
                    var negocio = new Negocio.Configuracao();

                    retorno = negocio.GerenciaBoleto(pv, boleto, out mensagemErro);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.RetornoNegocio, new { retorno, mensagemErro });
            }

            return retorno;
        }

        #endregion

        #region [ Perfil ]

        public Int32 ConsultaPerfilPV(Int32 pv, out Modelos.MensagemErro mensagemErro)
        {
            Int32 retorno = 0;

            using (Logger Log = Logger.IniciarLog("Perfil PV Forncecedor - Serviço"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { pv });

                try
                {
                    var negocio = new Negocio.Distribuidores();

                    retorno = negocio.ConsultaPerfilPV(pv, out mensagemErro);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.RetornoNegocio, new { retorno, mensagemErro });
            }

            return retorno;
        }

        public Boolean PerfilPVFornecedor(Int32 pv, out Modelos.MensagemErro mensagemErro)
        {
            Boolean retorno = false;
            Int32 perfil = 0;

            using (Logger Log = Logger.IniciarLog("Perfil PV Forncecedor - Serviço"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { pv });

                try
                {
                    var negocio = new Negocio.Distribuidores();

                    perfil = negocio.ConsultaPerfilPV(pv, out mensagemErro);

                    if(perfil == 1)
                        retorno = true;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.RetornoNegocio, new { retorno, perfil, mensagemErro });
            }

            return retorno;
        }

        public Boolean PerfilPVDistribuidor(Int32 pv, out Modelos.MensagemErro mensagemErro)
        {
            Boolean retorno = false;
            Int32 perfil = 0;

            using (Logger Log = Logger.IniciarLog("Perfil PV Distribuidor - Serviço"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { pv });

                try
                {
                    var negocio = new Negocio.Distribuidores();

                    perfil = negocio.ConsultaPerfilPV(pv, out mensagemErro);

                    if(perfil == 2)
                        retorno = true;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.RetornoNegocio, new { retorno, perfil, mensagemErro });
            }

            return retorno;
        }

        public Boolean PerfilPVComum(Int32 pv, out Modelos.MensagemErro mensagemErro)
        {
            Boolean retorno = false;
            Int32 perfil = 0;

            using (Logger Log = Logger.IniciarLog("Perfil PV Comum - Serviço"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { pv });

                try
                {
                    var negocio = new Negocio.Distribuidores();

                    perfil = negocio.ConsultaPerfilPV(pv, out mensagemErro);

                    if(perfil == 3)
                        retorno = true;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.RetornoNegocio, new { retorno, perfil, mensagemErro });
            }

            return retorno;
        }

        /// <summary>
        /// Consulta o número de parcelas que o PV pode parcelar na suas vendas
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <returns>Número máximo de parcelas</returns>
        public Int32 ConsultarNumeroParcelas(Int32 codigoEntidade)
        {
            Int32 numeroParcelas = 0;

            using (Logger Log = Logger.IniciarLog("Consultar Número de Parcelas - Serviço"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { codigoEntidade });
                
                try
                {
                    var negocio = new Negocio.Entidade();
                    numeroParcelas = negocio.ConsultarNumeroParcelas(codigoEntidade);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.RetornoNegocio, numeroParcelas.ToString());
            }

            return numeroParcelas;
        }

        #endregion

        #region [ Configurações Bandeiras Adicionais ]

        public Boolean GravarAtualizarBandeirasAdicionais(int pv, out Modelos.MensagemErro mensagemErro, string numeroAfiliacaoPdv, string chaveConfiguracaoPdv)
        {
            Boolean retorno = false;

            using (Logger Log = Logger.IniciarLog("Gravar Atualizar Bandeiras Adicionais"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { pv, numeroAfiliacaoPdv, chaveConfiguracaoPdv });

                try
                {
                    var negocio = new Negocio.Configuracao();
                    retorno = negocio.GravarAtualizarBandeirasAdicionais(pv, out mensagemErro, numeroAfiliacaoPdv, chaveConfiguracaoPdv);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno, mensagemErro });
            }

            return retorno;
        }

        public Modelos.RetornoBandeirasAdicionais ConsultaBandeirasAdicionais(int pv, out Modelos.MensagemErro mensagemErro)
        {
            var retorno = new Modelos.RetornoBandeirasAdicionais();

            using (var log = Logger.IniciarLog("Consulta Bandeiras Adicionais"))
            {
                log.GravarLog(EventoLog.InicioServico, pv);

                try
                {
                    var negocio = new Negocio.Configuracao();
                    retorno = negocio.ConsultaBandeirasAdicionais(pv, out mensagemErro);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    retorno,
                    mensagemErro
                });
            }

            return retorno;
        }

        public Modelos.RetornoServicoPV ListaServicoPV(Int32 pv)
        {
            var retorno = new Modelos.RetornoServicoPV();

            using (var log = Logger.IniciarLog("Lista Servico PV"))
            {
                log.GravarLog(EventoLog.InicioServico, pv);

                try
                {
                    var negocio = new Negocio.Configuracao();
                    retorno = negocio.ListaServicoPV(pv);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    retorno,
                });
            }

            return retorno;
        }

        #endregion
    }
}