#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [Agnaldo Costa]
Empresa     : [Iteris]
Histórico   : Criação da Classe
- [26/07/2012] – [Agnaldo Costa] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using AutoMapper;
using Redecard.PN.Comum;
using System.ServiceModel;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Classe de Solicitações
    /// </summary>
    public class SolicitacaoServico : ServicoBase, ISolicitacaoServico
    {
        /// <summary>
        /// Consulta lista de ocorrências para solicitação
        /// </summary>
        /// <returns>Lista de ocorrências de solicitação</returns>
        public List<Ocorrencia> ConsultarOcorrencias()
        {
            using (Logger Log = Logger.IniciarLog("Consulta lista de ocorrências para solicitação"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Mapper.CreateMap<Modelo.Ocorrencia, Servicos.Ocorrencia>();
                    List<Modelo.Ocorrencia> listaOcorrenciasModelo = null;
                    List<Ocorrencia> listaOcorrenciasServico = new List<Ocorrencia>();

                    var solicitacaoNegocio = new Negocio.Solicitacao();
                    Log.GravarLog(EventoLog.ChamadaNegocio);

                    listaOcorrenciasModelo = solicitacaoNegocio.ConsultarOcorrencias();
                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaOcorrenciasModelo });

                    foreach (Modelo.Ocorrencia ocorrencia in listaOcorrenciasModelo)
                    {
                        listaOcorrenciasServico.Add(Mapper.Map<Modelo.Ocorrencia, Servicos.Ocorrencia>(ocorrencia));
                    }
                    Log.GravarLog(EventoLog.FimServico, new { listaOcorrenciasServico });

                    return listaOcorrenciasServico;
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

        /// <summary>
        /// Consulta as solicitações abertas de acordo com os parâmetros
        /// </summary>
        /// <param name="numeroSolicitacao">Número da solicitação</param>
        /// <param name="periodoInicio">Data inicial do período de busca</param>
        /// <param name="periodoFim">Data final do período de busca</param>
        /// <param name="tipoOcorrencia">Tipo de Ocorrência de solicitação</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns>Lista de Solicitações abertas</returns>
        public List<Solicitacao> Consultar(Int32 numeroSolicitacao, DateTime periodoInicio, DateTime periodoFim,
                        String tipoOcorrencia, Int32 codigoEntidade)
        {
            using (Logger Log = Logger.IniciarLog("Consulta as solicitações abertas de acordo com os parâmetros"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Mapper.CreateMap<Modelo.Solicitacao, Servicos.Solicitacao>();
                    List<Modelo.Solicitacao> listaSolicitacoesModelo = null;
                    List<Solicitacao> listaSolicitacoesServico = new List<Solicitacao>();

                    var solicitacaoNegocio = new Negocio.Solicitacao();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new
                    {
                        numeroSolicitacao,
                        periodoInicio,
                        periodoFim,
                        tipoOcorrencia,
                        codigoEntidade
                    });

                    listaSolicitacoesModelo = solicitacaoNegocio.Consultar(numeroSolicitacao, periodoInicio, periodoFim,
                                                        tipoOcorrencia, codigoEntidade);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaSolicitacoesModelo });

                    foreach (Modelo.Solicitacao solicitacao in listaSolicitacoesModelo)
                    {
                        listaSolicitacoesServico.Add(Mapper.Map<Modelo.Solicitacao, Servicos.Solicitacao>(solicitacao));
                    }
                    Log.GravarLog(EventoLog.FimServico, new { listaSolicitacoesServico });

                    return listaSolicitacoesServico;
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

        /// <summary>
        /// Consulta lista de motivos para o ocorrência
        /// </summary>
        /// <returns>Lista de motivos de solicitação</returns>
        public List<Motivo> ConsultarMotivos(String ocorrencia)
        {
            using (Logger Log = Logger.IniciarLog("Consulta lista de motivos para o ocorrência"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Mapper.CreateMap<Modelo.Motivo, Servicos.Motivo>();
                    List<Modelo.Motivo> listaMotivosModelo = null;
                    List<Motivo> listaMotivosServico = new List<Motivo>();

                    var solicitacaoNegocio = new Negocio.Solicitacao();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { ocorrencia });

                    listaMotivosModelo = solicitacaoNegocio.ConsultarMotivos(ocorrencia);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaMotivosModelo });
                    foreach (Modelo.Motivo motivo in listaMotivosModelo)
                    {
                        listaMotivosServico.Add(Mapper.Map<Modelo.Motivo, Servicos.Motivo>(motivo));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { listaMotivosServico });

                    return listaMotivosServico;
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
        /// <summary>
        /// Consulta lista de propriedades para o ocorrência
        /// </summary>
        public List<Propriedade> ConsultarPropriedades(String ocorrencia)
        {
            using (Logger Log = Logger.IniciarLog("Consulta lista de propriedades para o ocorrência"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Mapper.CreateMap<Modelo.Propriedade, Servicos.Propriedade>();
                    List<Modelo.Propriedade> listaPropriedadesModelo = null;
                    List<Propriedade> listaPropriedadesServico = new List<Propriedade>();

                    var solicitacaoNegocio = new Negocio.Solicitacao();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { ocorrencia });

                    listaPropriedadesModelo = solicitacaoNegocio.ConsultarPropriedades(ocorrencia);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaPropriedadesModelo });

                    foreach (Modelo.Propriedade propriedade in listaPropriedadesModelo)
                    {
                        listaPropriedadesServico.Add(Mapper.Map<Modelo.Propriedade, Servicos.Propriedade>(propriedade));
                    }
                    Log.GravarLog(EventoLog.FimServico, new { listaPropriedadesServico });

                    return listaPropriedadesServico;
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

        /// <summary>
        /// Consulta a descrição de caso da Solicitação
        /// </summary>
        /// <param name="numeroSolicitacao">Número da solicitação</param>
        /// <param name="codigoCaso">Código do caso</param>
        /// <returns>Descrição de Caso da Solicitação</returns>
        public String ConsultarDescricaoCaso(Int32 numeroSolicitacao, Int32 codigoCaso)
        {
            using (Logger Log = Logger.IniciarLog("Consulta a descrição de caso da Solicitação"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    var solicitacaoNegocio = new Negocio.Solicitacao();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroSolicitacao, codigoCaso });
                    var result = solicitacaoNegocio.ConsultarDescricaoCaso(numeroSolicitacao, codigoCaso);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico, new { result });
                    return result;
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
        /// <summary>
        /// Consultar modo de envio de resposta para o ocorrência
        /// </summary>
        public String ConsultarModoEnvio(String ocorrencia)
        {
            using (Logger Log = Logger.IniciarLog("Consultar modo de envio de resposta para o ocorrência"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    var solicitacaoNegocio = new Negocio.Solicitacao();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { ocorrencia });
                    var result = solicitacaoNegocio.ConsultarModoEnvio(ocorrencia);
                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
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
        /// <summary>
        /// Consulta as ocorrências da Solicitação
        /// </summary>
        /// <param name="numeroSolicitacao">Número da solicitação</param>
        /// <param name="codigoCaso">Código do caso de solicitação</param>
        /// <returns>Lista de ocorrências da Solicitação</returns>
        public List<Solicitacao> ConsultarOcorrenciasSolicitacao(Int32 numeroSolicitacao, Int32 codigoCaso)
        {
            using (Logger Log = Logger.IniciarLog("Consulta as ocorrências da Solicitação"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Mapper.CreateMap<Modelo.Solicitacao, Servicos.Solicitacao>();
                    List<Modelo.Solicitacao> listaSolicitacaoModelo = new List<Modelo.Solicitacao>();
                    List<Solicitacao> listaSolicitacaoServico = new List<Solicitacao>();

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroSolicitacao, codigoCaso });
                    var solicitacaoNegocio = new Negocio.Solicitacao();
                    listaSolicitacaoModelo = solicitacaoNegocio.ConsultarOcorrenciasSolicitacao(numeroSolicitacao, codigoCaso);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaSolicitacaoModelo });
                    foreach (Modelo.Solicitacao solicitacao in listaSolicitacaoModelo)
                    {
                        listaSolicitacaoServico.Add(Mapper.Map<Modelo.Solicitacao, Solicitacao>(solicitacao));
                    }


                    Log.GravarLog(EventoLog.FimServico, new { listaSolicitacaoServico });

                    return listaSolicitacaoServico;
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

        /// <summary>
        /// Inclui uma nova solicitação para o numero do PV
        /// </summary>
        public Int32 IncluirSolicitacao(String ocorrencia, Int32 numeroPV, String motivo, String descricaoCaso, List<KeyValuePair<String, String>> vencimentos, List<KeyValuePair<String, String>> formaEnvio)
        {
            using (Logger Log = Logger.IniciarLog("Inclui uma nova solicitação para o numero do PV"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { ocorrencia, numeroPV, motivo, descricaoCaso, vencimentos, formaEnvio });

                    var solicitacaoNegocio = new Negocio.Solicitacao();
                    var result = solicitacaoNegocio.IncluirSolicitacao(ocorrencia, numeroPV, motivo, descricaoCaso, vencimentos, formaEnvio);

                    Log.GravarLog(EventoLog.RetornoNegocio);

                    Log.GravarLog(EventoLog.FimServico, new { result });
                    return result;
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

        /// <summary>
        /// Consulta os pré-requisitos de uma Solicitação cadastrada
        /// </summary>
        /// <param name="numeroSolicitacao">Número da solicitação</param>
        /// <param name="codigoCaso">Código do caso de solicitação</param>
        /// <returns>Lista de pré-requisitos da Solicitação</returns>
        public List<PreRequisitoSolicitacao> ConsultarPreRequisitosSolicitacao(Int32 numeroSolicitacao, Int32 codigoCaso)
        {
            using (Logger Log = Logger.IniciarLog("Consulta os pré-requisitos de uma Solicitação cadastrada"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Mapper.CreateMap<Modelo.PreRequisitoSolicitacao, Servicos.PreRequisitoSolicitacao>();
                    List<Modelo.PreRequisitoSolicitacao> listaPreRequisitosModelo = new List<Modelo.PreRequisitoSolicitacao>();
                    List<PreRequisitoSolicitacao> listaPreRequitosServico = new List<PreRequisitoSolicitacao>();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroSolicitacao, codigoCaso });

                    var solicitacaoNegocio = new Negocio.Solicitacao();
                    listaPreRequisitosModelo = solicitacaoNegocio.ConsultarPreRequisitosSolicitacao(numeroSolicitacao, codigoCaso);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaPreRequisitosModelo });
                    foreach (Modelo.PreRequisitoSolicitacao solicitacao in listaPreRequisitosModelo)
                    {
                        listaPreRequitosServico.Add(Mapper.Map<Modelo.PreRequisitoSolicitacao, PreRequisitoSolicitacao>(solicitacao));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { listaPreRequitosServico });

                    return listaPreRequitosServico;
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

        /// <summary>
        /// Consultar o status da carta
        /// </summary>
        /// <param name="numeroSolicitacao">Número da solicitação</param>
        /// <param name="codigoCaso">Código do caso de solicitação</param>
        /// <returns>Modelo de carta com informações de status</returns>
        public CartaSolicitacao ConsultarStatusCarta(Int32 numeroSolicitacao, Int32 codigoCaso)
        {
            using (Logger Log = Logger.IniciarLog("Consultar o status da carta"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                try
                {
                    Mapper.CreateMap<Modelo.CartaSolicitacao, CartaSolicitacao>();
                    Modelo.CartaSolicitacao cartaSolicitacaoModelo = null;
                    CartaSolicitacao cartaSolicitacaoServico = null;

                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroSolicitacao, codigoCaso });
                    var solicitacaoNegocio = new Negocio.Solicitacao();
                    cartaSolicitacaoModelo = solicitacaoNegocio.ConsultarStatusCarta(numeroSolicitacao, codigoCaso);
                    Log.GravarLog(EventoLog.RetornoNegocio, new { cartaSolicitacaoModelo });


                    if (!object.ReferenceEquals(cartaSolicitacaoModelo, null))
                    {
                        cartaSolicitacaoServico = Mapper.Map<Modelo.CartaSolicitacao, CartaSolicitacao>(cartaSolicitacaoModelo);
                    }
                    Log.GravarLog(EventoLog.FimServico, new { cartaSolicitacaoServico });


                    return cartaSolicitacaoServico;
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

        /// <summary>
        /// Consulta os destinários e formas de resposta da carta de solicitação
        /// </summary>
        /// <param name="numeroSolicitacao"></param>
        /// <returns>Lista de formas de envio</returns>
        public List<CartaSolicitacao> ConsultarFormaRespostaCarta(Int32 numeroSolicitacao)
        {
            using (Logger Log = Logger.IniciarLog("Consulta os destinários e formas de resposta da carta de solicitação"))
            {
                Log.GravarLog(EventoLog.InicioServico);
                try
                {
                    Mapper.CreateMap<Modelo.CartaSolicitacao, CartaSolicitacao>();
                    List<Modelo.CartaSolicitacao> listaCartaSolicitacaoModelo = new List<Modelo.CartaSolicitacao>();
                    List<CartaSolicitacao> listaCartaSolicitacaoServico = new List<CartaSolicitacao>();
                    Log.GravarLog(EventoLog.ChamadaNegocio, new { numeroSolicitacao });

                    var solicitacaoNegocio = new Negocio.Solicitacao();
                    listaCartaSolicitacaoModelo = solicitacaoNegocio.ConsultarFormaRespostaCarta(numeroSolicitacao);

                    Log.GravarLog(EventoLog.RetornoNegocio, new { listaCartaSolicitacaoModelo });

                    foreach (Modelo.CartaSolicitacao carta in listaCartaSolicitacaoModelo)
                    {
                        listaCartaSolicitacaoServico.Add(Mapper.Map<Modelo.CartaSolicitacao, CartaSolicitacao>(carta));
                    }

                    Log.GravarLog(EventoLog.FimServico, new { listaCartaSolicitacaoServico });

                    return listaCartaSolicitacaoServico;
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
    }
}
