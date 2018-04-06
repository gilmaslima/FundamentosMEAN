using AutoMapper;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.DadosCadastrais.Modelo;
using System.Reflection;
using Redecard.PN.DadosCadastrais.Servicos.Modelos;
using Redecard.PN.DadosCadastrais.Servicos.Operacoes;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// <para>Serviço WCF para Consulta de dados da Entidade com retorno JSon</para>
    /// <para>para integração com a API de Login.</para>
    /// <para>Métodos de Serviço ConsultarDadosPV e ConsultarTecnologiaEstabelecimento replicados,</para> 
    /// <para>porém com alterações para tratar melhor os retornos de entrada e saída.</para>
    /// <para>Métodos da camada de Negócio e Dados permanecem os mesmos.</para>
    /// </summary>
    public class EntidadeServicoJson : EntidadeServico, IEntidadeServicoJson
    {
        /// <summary>
        /// Consultar dados do PV na base de dados do GE
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Modelo de Entidade preenchido com informações do Sybase GE, e objeto de Status de Retorno</returns>
        public Servicos.EntidadeResponse ConsultarDadosPV(String codigoEntidade)
        {
            Servicos.EntidadeResponse entidadeRetorno = new EntidadeResponse();

            using (Logger Log = Logger.IniciarLog("Consultar dados do PV na base de dados do GE"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                Int32 codigoRetorno = default(Int32);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });
                    var negocio = new Negocio.Entidade();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Modelo.Entidade entidade = negocio.ConsultarDadosPV(out codigoRetorno, Convert.ToInt32(codigoEntidade));

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, entidade });

                    var result = Mapper.Map<Servicos.Entidade>(entidade);

                    entidadeRetorno.Entidade = result;
                    entidadeRetorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = codigoRetorno,
                        FonteRetorno = String.Empty,
                        MensagemRetorno = String.Empty
                    };

                    Log.GravarLog(EventoLog.FimServico, new { entidadeRetorno });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    //throw new FaultException<GeneralFault>(
                    //    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                    entidadeRetorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    //throw new FaultException<GeneralFault>(
                    //    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                    entidadeRetorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }

                return entidadeRetorno;
            }
        }

        /// <summary>
        /// Consultar dados de todos os PVs na base de dados do GE e TG para réplica no PN
        /// </summary>
        /// <param name="codigoEntidade">Código da última Entidade em caso de rechamada</param>
        /// <returns>Modelo de Entidades preenchido com informações do Sybase GE, objeto de Status de Retorno e objeto de Rechamada</returns>
        public Servicos.ListaEntidadesResponse ConsultarEstabelecimentos(String codigoEntidade)
        {
            Servicos.ListaEntidadesResponse entidadesRetorno = new ListaEntidadesResponse();

            using (Logger Log = Logger.IniciarLog("Consultar dados do PV na base de dados do GE"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                Int32 codigoRetorno = default(Int32);
                Int32 registroFinal = default(Int32);
                Int32 totalRegistros = default(Int32);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade });
                    var negocio = new Negocio.Entidade();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();

                    List<Modelo.Entidade> entidades = negocio.ConsultarEstabelecimentos(Convert.ToInt32(codigoEntidade), out registroFinal, out totalRegistros, out codigoRetorno);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, entidades });

                    var result = Mapper.Map<List<Servicos.Entidade>>(entidades);

                    entidadesRetorno.Entidades = new List<Entidade>();
                    //entidadesRetorno.Entidades.Add(new Entidade());
                    entidadesRetorno.Entidades.AddRange(result);

                    entidadesRetorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = codigoRetorno,
                        FonteRetorno = String.Empty,
                        MensagemRetorno = String.Empty
                    };

                    entidadesRetorno.Rechamada = new RechamadaResponse()
                    {
                        RegistroFinal = registroFinal,
                        TotalRegistros = totalRegistros
                    };

                    Log.GravarLog(EventoLog.FimServico, new { entidadesRetorno });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    //throw new FaultException<GeneralFault>(
                    //    new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                    entidadesRetorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    //throw new FaultException<GeneralFault>(
                    //    new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                    entidadesRetorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }

                return entidadesRetorno;
            }
        }

        /// <summary>
        /// Consultar Tipo de Tecnologia do Estabelecimento.
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns> 
        /// <para>25 ou 26 ou 23 - Komerci</para>
        /// <para>20 - Normal</para>
        /// <para>0 - Não encontrou</para>
        /// </returns>
        public Servicos.TecnologiaResponse ConsultarTecnologiaEstabelecimento(String codigoEntidade)
        {
            Int32 codigoTecnologia = 0;
            Int32 codigoRetorno = default(Int32);

            TecnologiaResponse tecnologiaRetorno = new TecnologiaResponse();

            using (Logger Log = Logger.IniciarLog("Tipo de Tecnologia do Estabelecimento."))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    codigoTecnologia = new Negocio.DadosGerais().ConsultarTecnologiaEstabelecimento(codigoEntidade.ToInt32(), out codigoRetorno);

                    tecnologiaRetorno.CodigoTecnologia = codigoTecnologia;
                    tecnologiaRetorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = codigoRetorno,
                        FonteRetorno = String.Empty,
                        MensagemRetorno = String.Empty
                    };
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    tecnologiaRetorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        MensagemRetorno = String.Format("Fonte: {0} \n Exception:{1}", ex.Fonte, base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    tecnologiaRetorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        MensagemRetorno = String.Format("Fonte: {0} \n Exception:{1}", FONTE, base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { tecnologiaRetorno });
                return tecnologiaRetorno;

            }
        }

        /// <summary>
        /// Consultar filiais do estabelecimento a partir do código de associação
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <param name="tipoAssociacao"></param>
        /// <returns></returns>
        public List<Filial> ConsultarFiliais(String codigoEntidade, String tipoAssociacao)
        {
            int result = 0;
            return base.ConsultarFiliais(Int32.Parse(codigoEntidade), Int32.Parse(tipoAssociacao), out result);
        }

        /// <summary>
        /// Consultar filiais do estabelecimento informado
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <returns></returns>
        public List<Filial> ConsultarFiliaisEntidade(String codigoEntidade)
        {
            int result = 0;
            return base.ConsultarFiliaisEntidade(Int32.Parse(codigoEntidade), out result);
        }

        /// <summary>
        /// ValidarPossuiEAdquirencia
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <returns></returns>
        public bool ValidarPossuiEAdquirencia(String codigoEntidade)
        {
            int codigoRetorno = 0;
            return base.ValidarPossuiEAdquirencia(Int32.Parse(codigoEntidade), out codigoRetorno);
        }

        /// <summary>
        /// Consulta os PVs no PN, confere no GE e equaliza as bases GE-PN através do CPF/CNPJ.
        /// </summary>
        /// <param name="codigoTipoPessoa"></param>
        /// <param name="numeroCpfCnpj"></param>
        /// <returns>Lista de PVs</returns>
        public Servicos.ResponseBaseList<Modelo.EntidadeServicoModel> ConsultarEntidadeGeCriarNoPN(String codigoTipoPessoa, String numeroCpfCnpj)
        {
            Servicos.ResponseBaseList<Modelo.EntidadeServicoModel> retorno = new ResponseBaseList<Modelo.EntidadeServicoModel>();

            using (Logger Log = Logger.IniciarLog("Consulta os PVs no PN, confere no GE e equaliza as bases GE-PN através do CPF/CNPJ"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                Int32 codigoRetorno;
                try
                {
                    if (String.IsNullOrWhiteSpace(codigoTipoPessoa)
                     || (String.Compare(codigoTipoPessoa, "F", true) != 0
                      && String.Compare(codigoTipoPessoa, "J", true) != 0))
                        throw new PortalRedecardException(601, FONTE, "Código Tipo Pessoa Inválido.", new ArgumentNullException());

                    Int64 cpfCnpj = 0;
                    Int64.TryParse(numeroCpfCnpj, out cpfCnpj);

                    if (String.IsNullOrWhiteSpace(numeroCpfCnpj) || cpfCnpj == 0 || !numeroCpfCnpj.IsValidCPFCNPJ())
                        throw new PortalRedecardException(602, FONTE, "CPF/CNPJ Inválido.", new ArgumentNullException());

                    Int64? cpf = codigoTipoPessoa == "F" ? (Int64?)cpfCnpj : null;
                    Int64? cnpj = codigoTipoPessoa == "J" ? (Int64?)cpfCnpj : null;

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoTipoPessoa, numeroCpfCnpj });

                    Modelo.EntidadeServicoModel[] entidades = new Negocio.Entidade().ConsultarEntidadeGeCriarNoPN(out codigoRetorno, cpf, cnpj);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, entidades });

                    retorno.Itens = entidades;
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = codigoRetorno
                    };
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        /// <summary>
        /// Consulta os e-mails dos PVs informados
        /// </summary>
        /// <param name="pvs">Arrau de PVs para consulta à base de dados</param>
        /// <returns>Dicionário contendo o e-mail para cada PV consultado</returns>
        public Servicos.ResponseBaseItem<Dictionary<int, string>> ConsultarEmailPVs(int[] pvs)
        {
            Servicos.ResponseBaseItem<Dictionary<int, string>> retorno = new ResponseBaseItem<Dictionary<int, string>>();

            using (Logger Log = Logger.IniciarLog("Método ConsultarEmailPVs"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                Int32 codigoRetorno;
                try
                {
                    retorno.Item = new Negocio.Entidade().ConsultarEmailPVs(out codigoRetorno, pvs);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = codigoRetorno
                    };
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        /// <summary>
        /// ValidarConfirmacaoPositivaPrimeiroAcesso
        /// </summary>
        /// <param name="emailUsuarioAlteracao"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public Servicos.ConfirmacaoPositivaPrimeiroAcessoResponse ValidarConfirmacaoPositivaPrimeiroAcesso(
            String emailUsuarioAlteracao,
            Modelo.Mensagens.ConfirmacaoPositivaPrimeiroAcessoRequest request)
        {
            Servicos.ConfirmacaoPositivaPrimeiroAcessoResponse retorno = new Servicos.ConfirmacaoPositivaPrimeiroAcessoResponse();
            using (Logger Log = Logger.IniciarLog("Consultar a emtidade no PN"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                var entidadesPossuemUsuario = new Modelo.EntidadeServicoModel[0];
                var entidadesPossuemMaster = new Modelo.EntidadeServicoModel[0];

                try
                {
                    var retornoNegocio = new Negocio.Entidade().ValidarConfirmacaoPositivaPrimeiroAcesso(emailUsuarioAlteracao,
                                                                                    request,
                                                                                    out entidadesPossuemUsuario,
                                                                                    out entidadesPossuemMaster);
                    retorno.EntidadesPossuemMaster = entidadesPossuemMaster;
                    retorno.EntidadesPossuemUsuario = entidadesPossuemUsuario;
                    retorno.Retorno = retornoNegocio.Retorno;
                    retorno.TentativasRestantes = retornoNegocio.TentativasRestantes;
					retorno.PerguntasIncorretas = retornoNegocio.PerguntasIncorretas;
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = retornoNegocio.CodigoRetorno
                    };
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        /// <summary>
        /// Incrementa a quantidade de erros de Confirmação Positva das Entidades informadas
        /// </summary>
        /// <param name="codigoEntidades">Código das Entidades</param>
        /// <param name="emailUsuarioAlteracao">Email do usuário</param>
        /// <returns>Quantidade de Tentativas já realizadas por entidade</returns>
        public Servicos.ResponseBaseList<Modelo.Entidade> IncrementarQuantidadeConfirmacaoPositivaMultiplasEntidades(int[] codigoEntidades, String emailUsuarioAlteracao)
        {
            ResponseBaseList<Modelo.Entidade> retorno = new ResponseBaseList<Modelo.Entidade>();

            using (Logger Log = Logger.IniciarLog("Incrementa a quantidade de erros de Confirmação Positva da Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                Int32 codigoRetorno;
                try
                {
                    retorno.Itens = new Negocio.Entidade().IncrementarQuantidadeConfirmacaoPositivaMultiplasEntidades(out codigoRetorno, codigoEntidades, emailUsuarioAlteracao).ToArray();
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = codigoRetorno
                    };
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        /// <summary>
        /// Consulta os dados de IPs do Komerci
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="codigoGrupoEntidade">Código da Entidade</param>
        /// <returns>Informações do Komerci</returns>
        public Servicos.URLBackResponse ConsultarURLBack(String codigoEntidade, String codigoGrupoEntidade)
        {
            URLBackResponse retorno = new URLBackResponse();

            using (Logger Log = Logger.IniciarLog("Consulta os dados de IPs do Komerci"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Modelo.URLBack modeloURLBack = new Modelo.URLBack();
                    Mapper.CreateMap<Modelo.URLBack, Servicos.URLBack>();

                    Int32 codigoRetorno;
                    Modelo.Entidade modeloEntrada = new Modelo.Entidade()
                    {
                        Codigo = Convert.ToInt32(codigoEntidade),
                        GrupoEntidade = new Modelo.GrupoEntidade()
                        {
                            Codigo = Convert.ToInt32(codigoGrupoEntidade)
                        }
                    };
                    modeloURLBack = new Negocio.DadosGerais().ConsultarURLBack(modeloEntrada, out codigoRetorno);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, modeloURLBack });

                    if (codigoRetorno == 0)
                        retorno.URLBack = Mapper.Map<Modelo.URLBack, Servicos.URLBack>(modeloURLBack);

                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = codigoRetorno
                    };
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno });

                return retorno;
            }
        }

        /// <summary>
        /// Atualiza os IPs do Komerci
        /// </summary>
        /// <param name="codigoEntidade">Dados da Entidade do Komerci</param>
        /// <param name="dados">Dados gerais do URLBack para atualização</param>
        /// <param name="usuarioAlteracao">Usuário responsável pela atualização</param>
        /// <returns>Código de Erro da procedure</returns>
        public Servicos.URLBackResponse AtualizarURLBack(String codigoEntidade, Servicos.URLBack dados, String usuarioAlteracao)
        {
            URLBackResponse retorno = new URLBackResponse();

            using (Logger Log = Logger.IniciarLog("Atualiza os IPs do Komerci"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Modelo.URLBack modeloURLBack = null;

                    Negocio.DadosGerais negocioDadosGerais = new Negocio.DadosGerais();

                    Mapper.CreateMap<Servicos.URLBack, Modelo.URLBack>();
                    modeloURLBack = Mapper.Map<Servicos.URLBack, Modelo.URLBack>(dados);

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, modeloURLBack, usuarioAlteracao });

                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = negocioDadosGerais.AtualizarURLBack(Int32.Parse(codigoEntidade), modeloURLBack, usuarioAlteracao)
                    };
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno });

                return retorno;
            }
        }

        /// <summary>
        /// Verifica se os pvs relacionados ao CPF\CNPJ são apenas de filiais
        /// </summary>
        /// <param name="cnpj">CNPJ</param>
        /// <returns></returns>
        public Servicos.ResponseBaseItem<Boolean> PvsRelacionadosSaoFiliais(String cnpj)
        {
            ResponseBaseItem<Boolean> retorno = new ResponseBaseItem<Boolean>();

            using (Logger Log = Logger.IniciarLog("Consultar PV no GE por Cnpj/CPF PvsRelacionadosSaoFiliais"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                Int32 codigoRetorno;
                try
                {

                    retorno.Item = new Negocio.Entidade().PvsRelacionadosSaoFiliais(out codigoRetorno, Int64.Parse(cnpj));
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = codigoRetorno
                    };
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        /// <summary>
        /// Lista das alterações de Domicílio Bancário solicitadas pela Entidade em status Pendente
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Lista de Solicitações de alteração de Domicílio Bancário</returns>
        public Servicos.ResponseBaseList<Servicos.DadosDomiciolioBancario> ListarDomiciliosAlterados(String codigoEntidade)
        {
            ResponseBaseList<Servicos.DadosDomiciolioBancario> retorno = new ResponseBaseList<DadosDomiciolioBancario>();

            using (Logger Log = Logger.IniciarLog("Consulta as alterações de Domicílio Bancário solicitadas pela Entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Mapper.CreateMap<Modelo.DadosDomiciolioBancario, Servicos.DadosDomiciolioBancario>();
                    Mapper.CreateMap<Modelo.Bandeira, Servicos.Bandeira>();

                    var retornoNegocio = new Negocio.DadosBancarios().ListarDomiciliosAlterados(Int32.Parse(codigoEntidade));

                    retorno.Itens = Mapper.Map<List<Modelo.DadosDomiciolioBancario>, List<Servicos.DadosDomiciolioBancario>>(retornoNegocio).ToArray();
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        /// <summary>
        /// Consulta os usuários de um PV, filtrando-os pelo perfil
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="codigoGrupoEntidade">Código grupo entidade</param>
        /// <param name="tipoUsuario">
        /// <para>Tipo do Usuário:</para>
        /// <para> - "M": Master</para>
        /// <para> - "B": Básico</para>
        /// <para> - "P": Personalizado</para>
        /// </param>
        /// <returns>Usuários do PV com o Perfil solicitado</returns>
        public Servicos.ResponseBaseList<Servicos.Usuario> ConsultarUsuariosPorPerfil(
            String codigoEntidade, String codigoGrupoEntidade, String tipoUsuario)
        {
            ResponseBaseList<Servicos.Usuario> retorno = new ResponseBaseList<Servicos.Usuario>();
            Int32 codigoRetorno;
            using (Logger Log = Logger.IniciarLog("Consulta os usuário do Estabelecimento com determinado perfil"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoEntidade, codigoGrupoEntidade, tipoUsuario });
                try
                {
                    Mapper.CreateMap<Modelo.Usuario, Servicos.Usuario>();
                    Mapper.CreateMap<Modelo.Menu, Servicos.Menu>();
                    Mapper.CreateMap<Modelo.Pagina, Servicos.Pagina>();
                    Mapper.CreateMap<Modelo.PaginaMenu, Servicos.PaginaMenu>();
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.TipoEntidade, Servicos.TipoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();

                    //Gera filtro por Perfil
                    Func<Modelo.Usuario, Boolean> filtro =
                        (usuario) => String.Compare(usuario.TipoUsuario, tipoUsuario, true) == 0;
                    //Consulta os usuários pelo Perfil
                    List<Modelo.Usuario> retornoNegocio = new Negocio.Entidade().ConsultarUsuarios(
                        Int32.Parse(codigoEntidade), Int32.Parse(codigoGrupoEntidade), filtro, out codigoRetorno);
                    //Mapeamento do retorno
                    retorno.Itens = Mapper.Map<List<Modelo.Usuario>, List<Servicos.Usuario>>(retornoNegocio).ToArray();
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        /// <summary>
        /// Consulta os pvs por CPF com paginação no banco
        /// </summary>
        /// <param name="cpf">CPF</param>
        /// <param name="pagina">Pagina</param>
        /// <param name="qtdRegistros">Quantidade de registros por pagina</param>
        /// <param name="pvsSelecionados">Pvs selecionados</param>
        /// <param name="filtroGenerico">Filtro generico</param>
        /// <param name="retornarEmail"></param>
        /// <returns></returns>
        public ListaPaginadaEntidadesResponse ConsultarPvPorCpfComPaginacao(Int64 cpf, int pagina, int qtdRegistros, bool retornarEmail, string pvsSelecionados = null, string filtroGenerico = null)
        {
            using (Logger Log = Logger.IniciarLog("Consultar PV por CPF - REST"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                int codigoRetorno = default(int);
                int totalRows = default(int);
                int qtdEmailsPorCpf = default(int);

                ListaPaginadaEntidadesResponse retorno = new ListaPaginadaEntidadesResponse();
                Negocio.Entidade negocioEntidade = new Negocio.Entidade();

                try
                {
                    retorno.Itens = negocioEntidade.ConsultarPv(cpf, out codigoRetorno, out totalRows, out qtdEmailsPorCpf, retornarEmail, pagina, qtdRegistros, pvsSelecionados, filtroGenerico);
                    retorno.QuantidadeEmailsPorCpf = qtdEmailsPorCpf;
                    retorno.TotalRows = totalRows;

                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = codigoRetorno
                    };

                    return retorno; 
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }

                return retorno;
            }
        }

        /// <summary>
        /// Consulta os pvs por email
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="pvsSelecionados">Filtro pvs selecionados</param>
        /// <param name="filtroGenerico">Filtro generico</param>
        /// <returns></returns>
        public ListaPaginadaEntidadesResponse ConsultarPvPorEmail(string email, string pvsSelecionados = null, string filtroGenerico = null)
        {
            using (Logger Log = Logger.IniciarLog("Consultar PV por E-mail"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Negocio.Entidade negocioEntidade = new Negocio.Entidade();
                ListaPaginadaEntidadesResponse retorno = new ListaPaginadaEntidadesResponse();
                Int32 codigoRetorno = default(Int32);

                try
                {
                    retorno.Itens = negocioEntidade.ConsultarPv(email, out codigoRetorno, pvsSelecionados, filtroGenerico);
                    retorno.PVsSenhasIguais = negocioEntidade.PvSenhasIguais(email, out codigoRetorno);

                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = codigoRetorno
                    };

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }

                return retorno;
            }
        }


        #region Taxas

        /// <summary>
        /// Consulta os dados bancários de Crédito, Débito ou Voucher
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="tipoDado">Tipo de dados a ser consultado.
        ///                 C - Crédito
        ///                 D - Débito
        ///                 V - Voucher
        /// </param>
        /// <returns>Retorna a lista de Dados Bancários</returns>
        public DadosBancariosListRest ConsultarDadosBancarios(String codigoEntidade, String tipoDados)
        {
            List<Modelo.DadosBancarios> listaDados = null;
            List<Servicos.DadosBancarios> dados = new List<Servicos.DadosBancarios>();
            DadosBancariosListRest retorno;
            using (Logger Log = Logger.IniciarLog("Consulta os dados bancários de Crédito, Débito ou Voucher"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Int32 codigoRetorno;

                try
                {
                    Int32 intCodigoEntidade;
                    if (Int32.TryParse(codigoEntidade, out intCodigoEntidade))
                    {
                        Negocio.DadosBancarios negocioDados = new Negocio.DadosBancarios();
                        Mapper.CreateMap<Modelo.DadosBancarios, Servicos.DadosBancarios>();

                        Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, tipoDados });

                        listaDados = negocioDados.Consultar(intCodigoEntidade, tipoDados, out codigoRetorno);
                        Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, listaDados });


                        foreach (var dado in listaDados)
                        {
                            dados.Add(Mapper.Map<Modelo.DadosBancarios, Servicos.DadosBancarios>(dado));
                        }
                        Log.GravarLog(EventoLog.FimServico, new { listaDados });

                        retorno = new DadosBancariosListRest
                        {
                            StatusRetorno = new StatusRetorno
                            {
                                CodigoRetorno = codigoRetorno,
                                FonteRetorno = "",
                                MensagemRetorno = ""
                            },
                            Itens = dados.ToArray()
                        };
                    }
                    else
                    {
                        retorno = new DadosBancariosListRest
                        {
                            StatusRetorno = new StatusRetorno
                            {
                                CodigoRetorno = 304,
                                FonteRetorno = "ConsultarDadosBancarios",
                                MensagemRetorno = "Erro ao converter o Codigo da Entidade / PV"
                            }
                        };
                    }

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

                return retorno;
            }
        }

        /// <summary>
        /// Consulta os Produtos Flex cadastrados para um estabelecimento
        /// </summary>
        /// <param name="codigoCCA">Código da CCA</param>
        /// <param name="codigoFeature">Código da Feature</param>
        /// <param name="codigoEntidade">Código do estabelecimento</param>
        /// <param name="codigoRetorno">Código de retorno da procedure</param>
        /// <returns>Retorna a lista dos Produtos Flex de um estabelecimento</returns>        
        public ProdutoFlexListRest ConsultarProdutosFlex(String codigoEntidade, String codigoCCA, String codigoFeature)
        {
            List<Servicos.ProdutoFlex> dados = new List<Servicos.ProdutoFlex>();
            ProdutoFlexListRest retorno;
            Int32 codigoRetorno;

            using (Logger Log = Logger.IniciarLog("Consultar Produtos Flex"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoEntidade, codigoCCA, codigoFeature });

                try
                {
                    Int32 intCodigoEntidade;
                    if (Int32.TryParse(codigoEntidade, out intCodigoEntidade))
                    {
                        Int32 intCodigoCCA;
                        Int32 intCodigoFeature;

                        bool retornoCCA = Int32.TryParse(codigoCCA, out intCodigoCCA);
                        bool retornoFeature = Int32.TryParse(codigoFeature, out intCodigoFeature);

                        //Consulta os produtos flex
                        List<Modelo.ProdutoFlex> listaDados = new Negocio.DadosBancarios()
                            .ConsultarProdutosFlex(intCodigoEntidade,
                                                    (retornoCCA ? intCodigoCCA : (Int32?)null),
                                                    (retornoFeature ? intCodigoFeature : (Int32?)null),
                                                    out codigoRetorno);

                        //Mapeamento de saída
                        Mapper.CreateMap<Modelo.ProdutoFlex, Servicos.ProdutoFlex>();
                        dados = Mapper.Map<List<Servicos.ProdutoFlex>>(listaDados);

                        retorno = new ProdutoFlexListRest
                        {
                            StatusRetorno = new StatusRetorno
                            {
                                CodigoRetorno = codigoRetorno,
                                FonteRetorno = "",
                                MensagemRetorno = ""
                            },
                            Itens = dados.ToArray()
                        };

                    }
                    else
                    {
                        retorno = new ProdutoFlexListRest
                        {
                            StatusRetorno = new StatusRetorno
                            {
                                CodigoRetorno = 304,
                                FonteRetorno = "ConsultarDadosBancarios",
                                MensagemRetorno = "Erro ao converter o Codigo da Entidade / PV"
                            }
                        };
                    }
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

                Log.GravarLog(EventoLog.FimServico, new { retorno = dados });

                return retorno;
            }
        }

        /// <summary>
        /// Consulta os dados bancários de Crédito, Débito de forma resumida para bandeiras populares.
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Retorna a lista de Dados Bancários de forma resumida e para apenas algumas bandeiras.</returns>
        public ResumoListRest ResumoTaxas(String codigoEntidade)
        {
            List<Modelo.DadosBancarios> listaDadosCredito = null;
            List<Modelo.DadosBancarios> listaDadosDebito = null;
            List<Servicos.DadosBancarios> dadosCredito = new List<Servicos.DadosBancarios>();
            List<Servicos.DadosBancarios> dadosDebito = new List<Servicos.DadosBancarios>();
            ResumoListRest retorno = new ResumoListRest();
            using (Logger Log = Logger.IniciarLog("Consulta os dados bancários [Resumo]"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                Int32 codigoRetorno;

                try
                {
                    Int32 intCodigoEntidade;
                    if (Int32.TryParse(codigoEntidade, out intCodigoEntidade))
                    {
                        var negocioDados = new Negocio.DadosBancarios();
                        Mapper.CreateMap<Modelo.DadosBancarios, Servicos.DadosBancarios>();

                        #region Crédito
                        Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, tipoDados = "C" });

                        listaDadosCredito = negocioDados.Consultar(intCodigoEntidade, "C", out codigoRetorno);
                        Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, listaDadosCredito });


                        foreach (var dado in listaDadosCredito)
                        {
                            dadosCredito.Add(Mapper.Map<Modelo.DadosBancarios, Servicos.DadosBancarios>(dado));
                        }
                        #endregion

                        #region Débito
                        Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, tipoDados = "D" });

                        listaDadosDebito = negocioDados.Consultar(intCodigoEntidade, "D", out codigoRetorno);
                        Log.GravarLog(EventoLog.RetornoNegocio, new { codigoRetorno, listaDadosDebito });


                        foreach (var dado in listaDadosDebito)
                        {
                            dadosDebito.Add(Mapper.Map<Modelo.DadosBancarios, Servicos.DadosBancarios>(dado));
                        }
                        #endregion

                        retorno.StatusRetorno = new StatusRetorno
                        {
                            CodigoRetorno = codigoRetorno,
                            FonteRetorno = "",
                            MensagemRetorno = ""
                        };

                        retorno.Itens = OperacaoResumoTaxa.ObterResumoTaxas(dadosCredito, dadosDebito);

                        Log.GravarLog(EventoLog.FimServico, new { retorno.Itens });
                    }
                    else
                    {
                        retorno.StatusRetorno = new StatusRetorno
                        {
                            CodigoRetorno = 304,
                            FonteRetorno = "ConsultarDadosBancarios",
                            MensagemRetorno = "Erro ao converter o Codigo da Entidade / PV"
                        };
                    }
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

                return retorno;
            }
        }

        #endregion
		
        /// <summary>
        /// Consulta bancos cadastradados na base DR para confirmação positiva
        /// </summary>
        /// <returns>Lista de Bancos</returns>
        public Servicos.ResponseBaseList<Servicos.Banco> ConsultarBancosConfirmacaoPositivaRest()
        {
            ResponseBaseList<Servicos.Banco> retorno = new ResponseBaseList<Servicos.Banco>();
            using (Logger Log = Logger.IniciarLog("Consulta bancos cadastradados na base DR para confirmação positiva REST"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    var retornoNegocio = new Negocio.DadosBancarios().ConsultarBancosConfirmacaoPositiva();
                    //Mapeamento do retorno
                    Mapper.CreateMap<Modelo.Banco, Servicos.Banco>();
                    retorno.Itens = Mapper.Map<List<Modelo.Banco>, List<Servicos.Banco>>(retornoNegocio).ToArray();
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }
		
        /// <summary>
        /// Consulta as perguntas aleatórias disponíveis para o estabelecimento informado
        /// </summary>
        public ResponseBaseList<Servicos.Pergunta> ConsultarPerguntasAleatoriasRest(String numeroPV)
        {  
            ResponseBaseList<Servicos.Pergunta> retorno = new ResponseBaseList<Servicos.Pergunta>();
            using (Logger Log = Logger.IniciarLog("Consulta as perguntas aleatórias disponíveis para o estabelecimento informado"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    var retornoNegocio = new Negocio.Entidade().ConsultarPerguntasAleatorias(Convert.ToInt32(numeroPV));
                    //Mapeamento do retorno
                    Mapper.CreateMap<Modelo.Pergunta, Servicos.Pergunta>();
                    retorno.Itens = Mapper.Map<List<Modelo.Pergunta>, List<Servicos.Pergunta>>(retornoNegocio).ToArray();
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        /// <summary>
        /// Verifica, na listagem de PVs, se algum tem Komerci
        /// </summary>
        /// <param name="pvs">Lista de PVs</param>
        /// <returns>Se algum PV possui Komerci</returns>
        public ResponseBaseItem<Boolean> PossuiKomerciRest(List<Int32> pvs)
        {
            ResponseBaseItem<Boolean> retorno = new ResponseBaseItem<Boolean>();

            using (Logger Log = Logger.IniciarLog("Verifica se PVs possuem Komerci"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, pvs);

                    retorno.Item = new Negocio.Entidade().PossuiKomerci(pvs);

                    Log.GravarLog(EventoLog.FimServico, new { retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        /// <summary>
        /// Consulta entidade
        /// </summary>
        /// <param name="codigoEntidade">Id da entidade</param>
        /// <param name="codigoGrupoEntidade">Id do grupo da entidade</param>
        /// <returns>Entidade preenchida</returns>
        public Servicos.EntidadeConsultarResponse Consultar(String codigoEntidade, String codigoGrupoEntidade)
        {
            Servicos.EntidadeConsultarResponse retorno = new Servicos.EntidadeConsultarResponse();
            Int32 codigoRetornoIS = default(Int32);
            Int32 codigoRetornoGE = default(Int32);

            using (Logger Log = Logger.IniciarLog("Consulta entidade"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { codigoEntidade, codigoGrupoEntidade });

                    Int32? codEntidade = String.IsNullOrEmpty(codigoEntidade) ? (Int32?)null : Convert.ToInt32(codigoEntidade);
                    Int32? codGrupoEntidade = String.IsNullOrEmpty(codigoGrupoEntidade) ? (Int32?)null : Convert.ToInt32(codigoGrupoEntidade);

                    var retornoNegocio = new Negocio.Entidade().Consultar(codEntidade, codGrupoEntidade, out codigoRetornoIS, out codigoRetornoGE);

                    // Cria mapeamento, caso já exista utilizará o existente
                    Mapper.CreateMap<Modelo.Entidade, Servicos.Entidade>();
                    Mapper.CreateMap<Modelo.GrupoEntidade, Servicos.GrupoEntidade>();
                    Mapper.CreateMap<Modelo.Status, Servicos.Status>();
                    
                    retorno.Itens = Mapper.Map<List<Modelo.Entidade>, List<Servicos.Entidade>>(retornoNegocio).ToArray();
                    retorno.CodigoRetornoGE = codigoRetornoGE;
                    retorno.CodigoRetornoIS = codigoRetornoIS;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                Log.GravarLog(EventoLog.FimServico, new { retorno });
                return retorno;
            }
        }

        /// <summary>
        /// Atualiza os Dados Gerais na base do IS Sybase e IS SQL Server
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="dados">Modelo de Dados Gerais com informações atualizadas</param>
        /// <returns></returns>
        public ResponseBaseItem<Int32> AtualizarDadosGeraisRest(Int32 codigoEntidade, Servicos.DadosGerais dados)
        {
            ResponseBaseItem<Int32> retorno = new ResponseBaseItem<Int32>();

            using (Logger Log = Logger.IniciarLog("Atualiza os Dados Gerais na base do IS Sybase e IS SQL Server"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Mapper.CreateMap<Servicos.DadosGerais, Modelo.DadosGerais>();
                    var modeloDadosGerais = Mapper.Map<Servicos.DadosGerais, Modelo.DadosGerais>(dados);
                    retorno.Item = new Negocio.DadosGerais().Atualizar(codigoEntidade, modeloDadosGerais);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = ex.Codigo,
                        FonteRetorno = ex.Fonte,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = new StatusRetorno()
                    {
                        CodigoRetorno = CODIGO_ERRO,
                        FonteRetorno = FONTE,
                        MensagemRetorno = String.Format("Exception: {0}", base.RecuperarExcecao(ex))
                    };
                }

                Log.GravarLog(EventoLog.FimServico, new { retorno });

                return retorno;
            }
        }
    }
}
