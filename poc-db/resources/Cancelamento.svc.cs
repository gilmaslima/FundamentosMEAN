using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;

namespace Rede.PN.Cancelamento.Servicos
{
    /// <summary>
    /// Classe de serviços do cancelamento
    /// </summary>
    public class Cancelamento : ServicoBase, ICancelamento
    {

        #region [ Constantes ]

        /// <summary>
        /// Código de erro genérico de serviço
        /// </summary>
        public static Int32 CodigoErro { get { return 600; } }

        /// <summary>
        /// Fonte
        /// </summary>
        public static String Fonte { get { return "Rede.PN.Cancelamento.Servicos"; } }

        #endregion

        /// <summary>
        /// Busca Cancelamentos
        /// </summary>
        /// <param name="codigoUsuario">Código do usúario</param>
        /// <param name="numeroEstabelecimento">Número do estabelecimento</param>
        /// <param name="indicadorPesquisa">Indicador do tipo de pesquisa</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="numeroAvisoCancelamento">Número do aviso de cancelamento</param>
        /// <param name="numeroNsu">Número de sequência único</param>
        /// <param name="tipoVenda">Tipo de venda</param>
        /// <returns>Retorna uma lista de cancelamentos</returns>
        public List<Modelo.SolicitacaoCancelamento> BuscarCancelamentos(String codigoUsuario,
            Int32 numeroEstabelecimento,
            String indicadorPesquisa, DateTime dataInicial,
            DateTime dataFinal,
            Int64 numeroAvisoCancelamento,
            Int64 numeroNsu,
            Modelo.TipoVenda tipoVenda)
        {
            List<Modelo.SolicitacaoCancelamento> retorno = new List<Modelo.SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Buscar Cancelamentos - Serviço"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    codigoUsuario,
                    numeroEstabelecimento,
                    indicadorPesquisa,
                    dataInicial,
                    dataFinal,
                    numeroAvisoCancelamento,
                    numeroNsu,
                    tipoVenda
                });

                try
                {
                    Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();
                    retorno.AddRange(cancelamentoBll.BuscarCancelamentos(codigoUsuario,
                            numeroEstabelecimento,
                            indicadorPesquisa,
                            dataInicial,
                            dataFinal,
                            numeroAvisoCancelamento,
                            numeroNsu,
                            tipoVenda));
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    throw new FaultException<GeneralFault>(
                                new GeneralFault(CodigoErro, Fonte), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Inclui solicitações de cancelamento
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="cancelamentos">Cancelamentos para inclusão</param>
        /// <returns>Retorna lista de cancelamentos incluídos</returns>
        public List<Modelo.SolicitacaoCancelamento> IncluirCancelamentos(String codigoUsuario, List<Modelo.SolicitacaoCancelamento> cancelamentos)
        {
            List<Modelo.SolicitacaoCancelamento> retorno = new List<Modelo.SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Incluir Cancelamentos - Serviço"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    codigoUsuario,
                    cancelamentos
                });

                try
                {
                    Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();
                    retorno = cancelamentoBll.IncluirCancelamentos(codigoUsuario, cancelamentos);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                                new GeneralFault(CodigoErro, Fonte), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Valida solicitações de cancelamento
        /// </summary>
        /// <param name="tipoOperacao">Tipo de operação - 'B' = validação</param>
        /// <param name="cancelamentos">Cancelamentos para validação</param>
        /// <returns>Retorna uma lista de resultados da validação</returns>
        public List<Modelo.Validacao> ValidarParametrosBloqueio(Char tipoOperacao, List<Modelo.SolicitacaoCancelamento> cancelamentos)
        {
            List<Modelo.Validacao> retorno = new List<Modelo.Validacao>();

            using (var log = Logger.IniciarLog("Validar Parâmetros Bloqueio - Serviço"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    tipoOperacao,
                    cancelamentos
                });

                try
                {
                    //Inicio de change - Contorno de problema no serviço HIS que causa problemas quando envia uma lista com mais de um item por vez
                    Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();
                    List<Modelo.Validacao> retornoUmItem = new List<Modelo.Validacao>();

                    foreach (Modelo.SolicitacaoCancelamento objCancelamento in cancelamentos)
                    {
                        List<Modelo.SolicitacaoCancelamento> lstUmItemCancelamento = new List<Modelo.SolicitacaoCancelamento>();

                        //Change verifica se saldo disponível == 0, se for enviar Valor do cancelamento != 0 para que a validação do HIS ocorra (motivo: cliente não quer modificar programa do main frame)
                        if (Decimal.Equals(objCancelamento.SaldoDisponivel, 0))
                            if (objCancelamento.TipoCancelamento == Modelo.TipoCancelamento.Total)
                                objCancelamento.ValorCancelamento = objCancelamento.ValorBruto;
                            else
                                objCancelamento.ValorCancelamento = 0.01M;

                        lstUmItemCancelamento.Add(objCancelamento);
                        retornoUmItem = cancelamentoBll.ValidarParametrosBloqueio(tipoOperacao, lstUmItemCancelamento);
                        retorno.Add(retornoUmItem[0]);
                    }
                    //Fim de change

                    //Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();
                    //retorno = cancelamentoBll.ValidarParametrosBloqueio(tipoOperacao, cancelamentos);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                                new GeneralFault(CodigoErro, Fonte), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Busca dados da transação para efetuar o cancelamento
        /// </summary>
        /// <param name="cancelamento">Cancelamento</param>
        /// <returns>Retorna o cancelamento buscado com mais informações</returns>
        public Modelo.SolicitacaoCancelamento BuscarTransacaoParaCancelamento(Modelo.SolicitacaoCancelamento cancelamento)
        {
            Modelo.SolicitacaoCancelamento retorno = new Modelo.SolicitacaoCancelamento();

            using (var log = Logger.IniciarLog("Buscar Transação Para Cancelamento - Serviço"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    cancelamento
                });

                try
                {
                    Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();
                    retorno = cancelamentoBll.BuscarTransacaoParaCancelamento(cancelamento.NumeroEstabelecimentoVenda,
                        cancelamento.DataVenda,
                        cancelamento.NSU,
                        cancelamento.TimestampTransacao,
                        cancelamento.NumeroMes,
                        cancelamento.TipoTransacao,
                        cancelamento.TipoVenda);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                                new GeneralFault(CodigoErro, Fonte), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Busca transações duplicadas para cancelamento
        /// </summary>
        /// <param name="cancelamento">Cancelamento</param>
        /// <returns>Retorna lista de cancelamentos do mesmo ponto de venda, data de transação, tipo de transação e nsu/cartão</returns>
        public List<Modelo.SolicitacaoCancelamento> BuscarTransacaoDuplicadaParaCancelamento(Modelo.SolicitacaoCancelamento cancelamento)
        {
            List<Modelo.SolicitacaoCancelamento> retorno = new List<Modelo.SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Buscar Transação Duplicada Para Cancelamento - Serviço"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    cancelamento
                });

                try
                {
                    Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();
                    retorno = cancelamentoBll.BuscarTransacaoDuplicadaParaCancelamento(cancelamento.NumeroEstabelecimentoVenda,
                        cancelamento.DataVenda,
                        cancelamento.NSU,
                        cancelamento.TipoVenda);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                                new GeneralFault(CodigoErro, Fonte), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Anula cancelamentos
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="cancelamentos">Cancelamentos para anulação</param>
        /// <returns>Retorna uma lista de resultados das anulações</returns>
        public List<Modelo.Validacao> AnularCancelamento(String codigoUsuario, Int16 codigoCanal, List<Modelo.SolicitacaoCancelamento> cancelamentos)
        {
            List<Modelo.Validacao> retorno = new List<Modelo.Validacao>();

            using (var log = Logger.IniciarLog("Anular Cancelamentos - Serviço"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    codigoUsuario,
                    codigoCanal,
                    cancelamentos
                });

                try
                {
                    Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();
                    retorno = cancelamentoBll.AnularCancelamentos(codigoUsuario, codigoCanal, cancelamentos);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                                new GeneralFault(CodigoErro, Fonte), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Verifica se um ponto de venda é filial de uma dada matriz
        /// </summary>
        /// <param name="numeroEstabelecimento">Número do estabelecimento</param>
        /// <param name="numeroMatriz">Número da matriz</param>
        /// <returns>Retorna verdadeiro quando o número do estabelecimento é filial da matriz informada</returns>
        public Boolean VerificarEstabelecimentoEmMatriz(Int32 numeroEstabelecimento, Int32 numeroMatriz)
        {
            Boolean retorno = false;

            using (var log = Logger.IniciarLog("Verifica estabelecimento em matriz - Serviço"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    numeroEstabelecimento,
                    numeroMatriz
                });

                Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();

                try
                {
                    retorno = cancelamentoBll.VerificarEstabelecimentoEmMatriz(numeroEstabelecimento, numeroMatriz);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CodigoErro, Fonte), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    retorno
                });
            }

            return retorno;
        }

        #region [ Cancelamentos Desfeitos ]

        /// <summary>
        /// Busca cancelamentos desfeitos por período
        /// </summary>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="numeroAvisoCancelamento">Número do aviso de cancelamento</param>
        /// <param name="numeroCartao">Número do cartão</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <returns>Retorna lista de cancelamentos defeitos pelo FMS</returns>
        public List<Modelo.SolicitacaoCancelamento> BuscaCancelamentosDesfeitosPorPeriodo(DateTime dataInicial, DateTime dataFinal, Decimal numeroAvisoCancelamento, String numeroCartao, Int32 numeroPontoVenda)
        {
            List<Modelo.SolicitacaoCancelamento> cancelamentos = new List<Modelo.SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Busca Cancelamentos Desfeitos por Período - Serviço"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    dataInicial,
                    dataFinal,
                    numeroAvisoCancelamento,
                    numeroCartao,
                    numeroPontoVenda
                });

                try
                {
                    Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();
                    cancelamentos = cancelamentoBll.BuscaCancelamentosDesfeitosPorPeriodo(dataInicial, dataFinal, numeroAvisoCancelamento, numeroCartao, numeroPontoVenda);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CodigoErro, Fonte), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    cancelamentos
                });
            }

            return cancelamentos;
        }

        /// <summary>
        /// Busca cancelamentos desfeitos por Ponto de Venda e Número de Aviso
        /// </summary>
        /// <param name="numeroAvisoCancelamento">Número do aviso de cancelamento</param>
        /// <param name="numeroCartao">Número do cartão</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <returns>Retorna lista de cancelamentos defeitos pelo FMS</returns>
        public List<Modelo.SolicitacaoCancelamento> BuscaCancelamentosDesfeitosPorPontoVendaENumeroAviso(Decimal numeroAvisoCancelamento, String numeroCartao, Int32 numeroPontoVenda)
        {
            List<Modelo.SolicitacaoCancelamento> cancelamentos = new List<Modelo.SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Busca Cancelamentos Desfeitos por Ponto de Venda e Número do Aviso - Serviço"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    numeroAvisoCancelamento,
                    numeroCartao,
                    numeroPontoVenda
                });

                try
                {
                    Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();
                    cancelamentos = cancelamentoBll.BuscaCancelamentosDesfeitosPorPontoVendaENumeroAviso(numeroAvisoCancelamento, numeroCartao, numeroPontoVenda);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CodigoErro, Fonte), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    cancelamentos
                });
            }

            return cancelamentos;
        }

        #endregion

        #region [ Cancelamentos PN ]

        /// <summary>
        /// Incluir Lista de Solicitação de Cancelamento no Banco de dados PN
        /// </summary>
        /// <param name="lstSolicitacaoCancelamento">Lista de Solicitação de Cancelamento</param>
        /// <param name="ip">valor Ip da máquina que solicitou o cancelamento</param>
        /// <param name="usuario">usuário que solicitou o cancelamento</param>
        public void IncluirCancelamentosPn(List<Modelo.SolicitacaoCancelamento> lstSolicitacaoCancelamento, string ip, string usuario)
        {

            using (var log = Logger.IniciarLog("Incluir Cancelamentos - Serviço"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    lstSolicitacaoCancelamento
                });

                try
                {
                    Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();
                    cancelamentoBll.IncluirCancelamentosPn(lstSolicitacaoCancelamento, ip, usuario);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                                new GeneralFault(CodigoErro, Fonte), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {

                });
            }
        }

        /// <summary>
        /// Consulta Cancelamentos PN
        /// </summary>
        /// <param name="tipoTransacao">Tipo de transação</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data Final</param>
        /// <param name="ip">ip</param>
        /// <returns>Retorna lista de cancelamentos PN</returns>
        public List<Modelo.CancelamentoPn> ConsultarCancelamentosPn(String tipoTransacao, Int32 numeroPontoVenda, DateTime dataInicial, DateTime dataFinal, String ip)
        {
            List<Modelo.CancelamentoPn> cancelamentos = new List<Modelo.CancelamentoPn>();

            using (var log = Logger.IniciarLog("Consulta Cancelamentos PN - Serviço"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    tipoTransacao,
                    numeroPontoVenda,
                    dataInicial,
                    dataFinal,
                    ip
                });

                try
                {
                    Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();
                    cancelamentos = cancelamentoBll.ConsultarCancelamentosPn(tipoTransacao, numeroPontoVenda, dataInicial, dataFinal, ip);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CodigoErro, Fonte), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {
                    cancelamentos
                });
            }

            return cancelamentos;
        }

        /// <summary>
        /// Anula cancelamentos PN
        /// </summary>
        /// <param name="listaNumerosAvisos">Lista de números de avisos</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <param name="usuario">Usuário</param>
        /// <param name="ip">Ip</param>
        public void AnularCancelamentosPn(List<Int64> listaNumerosAvisos, Int32 numeroPontoVenda, String usuario, String ip)
        {
            using (var log = Logger.IniciarLog("Anular Cancelamentos PN - Serviço"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    listaNumerosAvisos,
                    numeroPontoVenda,
                    usuario,
                    ip
                });

                try
                {
                    Negocio.CancelamentoBLL cancelamentoBll = new Negocio.CancelamentoBLL();
                    cancelamentoBll.AnularCancelamentosPn(listaNumerosAvisos, numeroPontoVenda, usuario, ip);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CodigoErro, Fonte), base.RecuperarExcecao(ex));
                }

                log.GravarLog(EventoLog.FimServico, new
                {

                });
            }
        }

        #endregion
    }
}
