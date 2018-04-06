using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using System.Runtime.Serialization;

namespace Rede.PN.Cancelamento.Negocio
{
    /// <summary>
    /// Classe de négocio do cancelamento
    /// </summary>
    public class CancelamentoBLL : RegraDeNegocioBase
    {
        #region [ Constantes ]

        /// <summary>
        /// Código de erro genérico de negócio
        /// </summary>
        public static Int32 CodigoErro { get { return 400; } }

        /// <summary>
        /// Fonte
        /// </summary>
        public static String Fonte { get { return "Rede.PN.Cancelamento.Negocio"; } }

        #endregion

        #region [ Métodos Publicos ]

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
                    String indicadorPesquisa,
                    DateTime dataInicial,
                    DateTime dataFinal,
                    Int64 numeroAvisoCancelamento,
                    Int64 numeroNsu,
                    Modelo.TipoVenda tipoVenda)
        {

            List<Modelo.SolicitacaoCancelamento> retorno = new List<Modelo.SolicitacaoCancelamento>();
            Agentes.PATransacao.TipoDeVenda tipoDeVenda = new Agentes.PATransacao.TipoDeVenda();
            String indicadorRechamada = String.Empty;
            Decimal numeroAvisoRechamada = default(Decimal);
            String numeroProgramaRechamada = String.Empty;
            Decimal numeroNsuRechamada = default(Decimal);
            String dataTransacaoRechamada = String.Empty;
            String timestampTransacaoRechamada = String.Empty;
            String timestampCancelamentoRechamada = String.Empty;
            Int32 totalRegistros = default(Int32);

            using (var log = Logger.IniciarLog("Buscar Cancelamentos - Negócio"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new
                {
                    codigoUsuario,
                    numeroEstabelecimento,
                    indicadorPesquisa,
                    dataInicial,
                    dataFinal,
                    numeroAvisoCancelamento,
                    numeroNsu,
                    tipoVenda,
                    indicadorRechamada,
                    numeroAvisoRechamada,
                    numeroProgramaRechamada,
                    numeroNsuRechamada,
                    dataTransacaoRechamada,
                    timestampTransacaoRechamada,
                    timestampCancelamentoRechamada
                });

                try
                {
                    tipoDeVenda = MapTipoVenda1(tipoVenda);
                    if (codigoUsuario.Length > 8)
                        codigoUsuario = new String(codigoUsuario.Take(8).ToArray());

                    do
                    {
                        var cancelamentoAG = new Agentes.CancelamentoAG();
                        var cancelamentosPA = cancelamentoAG.BuscarCancelamentos(
                            codigoUsuario,
                            numeroEstabelecimento,
                            indicadorPesquisa,
                            dataInicial,
                            dataFinal,
                            numeroAvisoCancelamento,
                            numeroNsu,
                            tipoDeVenda,
                            ref indicadorRechamada,
                            ref numeroAvisoRechamada,
                            ref numeroProgramaRechamada,
                            ref numeroNsuRechamada,
                            ref dataTransacaoRechamada,
                            ref timestampTransacaoRechamada,
                            ref timestampCancelamentoRechamada,
                            out totalRegistros);

                        var cancelamentosPN = MapCancelamentos(cancelamentosPA);
                        retorno.AddRange(cancelamentosPN);

                    } while (indicadorRechamada.Equals("S"));
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CodigoErro, Fonte, ex);
                }

                log.GravarLog(EventoLog.FimNegocio, new
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

            using (var log = Logger.IniciarLog("Incluir Cancelamentos - Negócio"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new
                {
                    codigoUsuario,
                    cancelamentos
                });

                try
                {
                    var cancelamentosPAEnvio = MapCancelamentos(cancelamentos);

                    if (codigoUsuario.Length > 8)
                        codigoUsuario = new String(codigoUsuario.Take(8).ToArray());

                    var cancelamentoAG = new Agentes.CancelamentoAG();
                    var cancelamentosPARetorno = cancelamentoAG.IncluirCancelamentos(codigoUsuario, cancelamentosPAEnvio);

                    retorno = MapCancelamentos(cancelamentosPARetorno);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CodigoErro, Fonte, ex);
                }

                log.GravarLog(EventoLog.FimNegocio, new
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

            using (var log = Logger.IniciarLog("Validar Parâmetros Bloqueio - Negócio"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new
                {
                    tipoOperacao,
                    cancelamentos
                });

                try
                {
                    var cancelamentosPAEnvio = MapCancelamentos(cancelamentos);

                    var cancelamentoAG = new Agentes.CancelamentoAG();
                    var validacoesPA = cancelamentoAG.ValidarParametrosBloqueio(tipoOperacao, cancelamentosPAEnvio);

                    retorno = MapValidacoes(validacoesPA);

                    for (int i = 0; i < cancelamentos.Count; i++)
                    {
                        retorno[i].Linha = cancelamentos[i].Linha;
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CodigoErro, Fonte, ex);
                }

                log.GravarLog(EventoLog.FimNegocio, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Busca dados da transação para efetuar o cancelamento
        /// </summary>
        /// <param name="numeroEstabelecimento">Número do estabelecimento</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="numeroNSUouCartao">Número de sequência único ou número do cartão</param>
        /// <param name="timestampTransacao">Timestamp da transação</param>
        /// <param name="numeroMes">Número do mês</param>
        /// <param name="tipoTransacao">Tipo de transação</param>
        /// <param name="tipoVenda">Tipo de venda</param>
        /// <returns>Retorna um cancelamento</returns>
        public Modelo.SolicitacaoCancelamento BuscarTransacaoParaCancelamento(Int32 numeroEstabelecimento,
                    DateTime dataInicial,
                    String numeroNSUouCartao,
                    String timestampTransacao,
                    Int16 numeroMes,
                    Modelo.TipoTransacao tipoTransacao,
                    Modelo.TipoVenda tipoVenda)
        {
            Modelo.SolicitacaoCancelamento retorno = new Modelo.SolicitacaoCancelamento();

            using (var log = Logger.IniciarLog("Buscar Transação Para Cancelamento - Negócio"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new
                {
                    numeroEstabelecimento,
                    dataInicial,
                    numeroNSUouCartao,
                    timestampTransacao,
                    numeroMes,
                    tipoTransacao,
                    tipoVenda
                });

                try
                {
                    var tipoVendaPA = MapTipoVenda(tipoVenda);
                    var tipoTransacaoPA = MapTipoTransacao(tipoTransacao);

                    var cancelamentoAG = new Agentes.CancelamentoAG();
                    var cancelamentoPA = cancelamentoAG.BuscarTransacaoParaCancelamento(numeroEstabelecimento, dataInicial,
                        numeroNSUouCartao, timestampTransacao, numeroMes, tipoTransacaoPA, tipoVendaPA);

                    retorno = MapCancelamento(cancelamentoPA);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CodigoErro, Fonte, ex);
                }

                log.GravarLog(EventoLog.FimNegocio, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Busca transações duplicadas para cancelamento
        /// </summary>
        /// <param name="numeroEstabelecimento">Número do estabelecimento</param>
        /// <param name="dataVenda">Data da venda</param>
        /// <param name="numeroNSUouCartao">Número de sequência único ou número do cartão</param>
        /// <param name="tipoVenda">Tipo de venda</param>
        /// <returns>Retorna uma lista de cancelamentos com o mesmo ponto de venda, data da transação, tipo da transação e nsu/cartão</returns>
        public List<Modelo.SolicitacaoCancelamento> BuscarTransacaoDuplicadaParaCancelamento(Int32 numeroEstabelecimento, DateTime dataVenda, String numeroNSUouCartao, Modelo.TipoVenda tipoVenda)
        {
            List<Modelo.SolicitacaoCancelamento> retorno = new List<Modelo.SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Buscar Transação Para Cancelamento - Negócio"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new
                {
                    numeroEstabelecimento,
                    dataVenda,
                    numeroNSUouCartao,
                    tipoVenda
                });

                try
                {
                    var tipoVendaPA = MapTipoVenda(tipoVenda);

                    var cancelamentoAG = new Agentes.CancelamentoAG();
                    var cancelamentosPA = cancelamentoAG.BuscarTransacaoDuplicadaParaCancelamento(numeroEstabelecimento, dataVenda,
                        numeroNSUouCartao, tipoVendaPA);

                    retorno = MapCancelamentos(cancelamentosPA);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CodigoErro, Fonte, ex);
                }

                log.GravarLog(EventoLog.FimNegocio, new
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
        public List<Modelo.Validacao> AnularCancelamentos(String codigoUsuario, Int16 codigoCanal, List<Modelo.SolicitacaoCancelamento> cancelamentos)
        {
            List<Modelo.Validacao> retorno = new List<Modelo.Validacao>();

            using (var log = Logger.IniciarLog("Anular Cancelamentos - Negócio"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new
                {
                    codigoUsuario,
                    codigoCanal,
                    cancelamentos
                });

                try
                {
                    var cancelamentosPAEnvio = MapCancelamentos(cancelamentos);

                    if (codigoUsuario.Length > 8)
                        codigoUsuario = new String(codigoUsuario.Take(8).ToArray());

                    var cancelamentoAG = new Agentes.CancelamentoAG();
                    var validacoesPA = cancelamentoAG.AnularCancelamentos(codigoUsuario, codigoCanal, cancelamentosPAEnvio);

                    retorno = MapValidacoes(validacoesPA);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CodigoErro, Fonte, ex);
                }

                log.GravarLog(EventoLog.FimNegocio, new
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

            using (var log = Logger.IniciarLog("Verifica estabelecimento em matriz - Negócio"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new
                {
                    numeroEstabelecimento,
                    numeroMatriz
                });

                var cancelamentoAG = new Agentes.CancelamentoAG();

                try
                {
                    retorno = cancelamentoAG.VerificarEstabelecimentoEmMatriz(numeroEstabelecimento, numeroMatriz);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CodigoErro, Fonte, ex);
                }

                log.GravarLog(EventoLog.FimNegocio, new
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
        /// <returns>Retorna uma lista de cancelamentos desfeitos pelo FMS</returns>
        public List<Modelo.SolicitacaoCancelamento> BuscaCancelamentosDesfeitosPorPeriodo(DateTime dataInicial, DateTime dataFinal, Decimal numeroAvisoCancelamento, String numeroCartao, Int32 numeroPontoVenda)
        {
            List<Modelo.SolicitacaoCancelamento> cancelamentos = new List<Modelo.SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Busca Cancelamentos Desfeitos por Período - Negócio"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new
                {
                    dataInicial,
                    dataFinal,
                    numeroAvisoCancelamento,
                    numeroCartao,
                    numeroPontoVenda
                });

                try
                {
                    var cancelamentoAG = new Agentes.CancelamentoAG();
                    var cancelamentosRetornados = cancelamentoAG.BuscaCancelamentosDesfeitosPorPeriodo(dataInicial, dataFinal, numeroAvisoCancelamento, numeroCartao, numeroPontoVenda);

                    cancelamentos = MapCancelamentos(cancelamentosRetornados);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CodigoErro, Fonte, ex);
                }

                log.GravarLog(EventoLog.FimNegocio, new
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
        /// <returns>Retorna uma lista de cancelamentos desfeitos pelo FMS</returns>
        public List<Modelo.SolicitacaoCancelamento> BuscaCancelamentosDesfeitosPorPontoVendaENumeroAviso(Decimal numeroAvisoCancelamento, String numeroCartao, Int32 numeroPontoVenda)
        {
            List<Modelo.SolicitacaoCancelamento> cancelamentos = new List<Modelo.SolicitacaoCancelamento>();

            using (var log = Logger.IniciarLog("Busca Cancelamentos Desfeitos por Ponto de Venda e Número do Aviso - Negócio"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new
                {
                    numeroAvisoCancelamento,
                    numeroCartao,
                    numeroPontoVenda
                });

                try
                {
                    var cancelamentoAG = new Agentes.CancelamentoAG();
                    var cancelamentosRetornados = cancelamentoAG.BuscaCancelamentosDesfeitosPorPontoVendaENumeroAviso(numeroAvisoCancelamento, numeroCartao, numeroPontoVenda);

                    cancelamentos = MapCancelamentos(cancelamentosRetornados);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CodigoErro, Fonte, ex);
                }

                log.GravarLog(EventoLog.FimNegocio, new
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
            using (var log = Logger.IniciarLog("Incluir Cancelamentos - Negócio"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new
                {
                    lstSolicitacaoCancelamento
                });

                try
                {

                    var cancelamentoDados = new Dados.CancelamentoDados();

                    cancelamentoDados.IncluirCancelamentosPn(lstSolicitacaoCancelamento, ip, usuario);

                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CodigoErro, Fonte, ex);
                }

                log.GravarLog(EventoLog.FimNegocio, new
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

            using (var log = Logger.IniciarLog("Consulta Cancelamentos PN - Negócio"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new
                {
                    tipoTransacao,
                    numeroPontoVenda,
                    dataInicial,
                    dataFinal,
                    ip
                });

                try
                {
                    var cancelamentoDados = new Dados.CancelamentoDados();
                    cancelamentos = cancelamentoDados.ConsultarCancelamentosPn(tipoTransacao, numeroPontoVenda, dataInicial, dataFinal, ip);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CodigoErro, Fonte, ex);
                }

                log.GravarLog(EventoLog.FimNegocio, new
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
            using (var log = Logger.IniciarLog("Anular Cancelamentos PN - Negócio"))
            {
                log.GravarLog(EventoLog.InicioNegocio, new
                {
                    listaNumerosAvisos,
                    numeroPontoVenda,
                    usuario,
                    ip
                });

                try
                {
                    var cancelamentoDados = new Dados.CancelamentoDados();
                    cancelamentoDados.AnularCancelamentosPn(listaNumerosAvisos, numeroPontoVenda, usuario, ip);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CodigoErro, Fonte, ex);
                }

                log.GravarLog(EventoLog.FimNegocio, new
                {

                });
            }
        }

        #endregion

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Mapeia a entidade TipoDetalhamentoTransacao do PA para a entidade TipoTransacao do PN
        /// </summary>
        /// <param name="tipoTransacao">Tipo de transação</param>
        /// <returns>Retorna o tipo de transação</returns>
        private Modelo.TipoTransacao MapTipoTransacao(Agentes.PATransacao.TipoDetalhamentoTransacao tipoTransacao)
        {
            switch (tipoTransacao)
            {
                case Agentes.PATransacao.TipoDetalhamentoTransacao.DEB: return Modelo.TipoTransacao.Deb;
                case Agentes.PATransacao.TipoDetalhamentoTransacao.PAR: return Modelo.TipoTransacao.Par;
                case Agentes.PATransacao.TipoDetalhamentoTransacao.PRE: return Modelo.TipoTransacao.Pre;
                case Agentes.PATransacao.TipoDetalhamentoTransacao.REC: return Modelo.TipoTransacao.Rec;
                case Agentes.PATransacao.TipoDetalhamentoTransacao.ROT: return Modelo.TipoTransacao.Rot;
                default: return Modelo.TipoTransacao.Rot;
            }
        }

        /// <summary>
        /// Mapeia a entidade TipoTransacao do PN para a entidade TipoDetalhamentoTransacao do PA
        /// </summary>
        /// <param name="tipoTransacao">Tipo de transação</param>
        /// <returns>Retorna o tipo de transação</returns>
        private Agentes.PATransacao.TipoDetalhamentoTransacao MapTipoTransacao(Modelo.TipoTransacao tipoTransacao)
        {
            switch (tipoTransacao)
            {
                case Modelo.TipoTransacao.Deb: return Agentes.PATransacao.TipoDetalhamentoTransacao.DEB;
                case Modelo.TipoTransacao.Par: return Agentes.PATransacao.TipoDetalhamentoTransacao.PAR;
                case Modelo.TipoTransacao.Pre: return Agentes.PATransacao.TipoDetalhamentoTransacao.PRE;
                case Modelo.TipoTransacao.Rec: return Agentes.PATransacao.TipoDetalhamentoTransacao.REC;
                case Modelo.TipoTransacao.Rot: return Agentes.PATransacao.TipoDetalhamentoTransacao.ROT;
                default: return Agentes.PATransacao.TipoDetalhamentoTransacao.ROT;
            }
        }

        /// <summary>
        /// Mapeia a entidade TransacaoServico do PA para a entidade SolicitacaoCancelamento do PN
        /// </summary>
        /// <param name="cancelamentoPA">Cancelamento</param>
        /// <returns>Retorna o cancelamento</returns>
        private Modelo.SolicitacaoCancelamento MapCancelamento(Agentes.PATransacao.TransacaoServico cancelamentoPA)
        {
            var cancelamento = new Modelo.SolicitacaoCancelamento();
            cancelamento.NumeroEstabelecimentoVenda = cancelamentoPA.NumeroEstabelecimento;
            cancelamento.TipoVenda = MapTipoVenda(cancelamentoPA.TipoDeVenda);
            cancelamento.DataVenda = cancelamentoPA.DataTransacao;
            cancelamento.DataVendaTexto = cancelamentoPA.DataTransacao.ToString("dd/MM/yyyy");
            cancelamento.NSU = cancelamentoPA.NumeroNsu != 0 ? cancelamento.NSU = cancelamentoPA.NumeroNsu.ToString() : cancelamento.NSU = cancelamentoPA.NumeroCartao;
            cancelamento.ValorBruto = cancelamentoPA.ValorBruto;
            cancelamento.SaldoDisponivel = cancelamentoPA.ValorSaldo;
            cancelamento.NumeroAutorizacao = cancelamentoPA.CodigoAutorizacaoEmissor;
            cancelamento.NumeroMes = (Int16)cancelamentoPA.NumeroMes;
            cancelamento.TimestampTransacao = cancelamentoPA.TimestampTransacao;
            cancelamento.TipoTransacao = MapTipoTransacao(cancelamentoPA.TipoTransacao);
            cancelamento.TipoVendaDetalhado = MapTipoVendaDetalhado(cancelamentoPA);

            return cancelamento;
        }

        /// <summary>
        /// Mapeia a entidade Cancelamento do KC para a entidade SolicitacaoCancelamento do PN
        /// </summary>
        /// <param name="cancelamentoRetornado">Cancelamento</param>
        /// <returns>Retorna o cancelamento</returns>
        private Modelo.SolicitacaoCancelamento MapCancelamento(Agentes.KCCancelamentos.Cancelamento cancelamentoRetornado)
        {
            var cancelamento = new Modelo.SolicitacaoCancelamento();
            cancelamento.NumeroEstabelecimentoVenda = cancelamentoRetornado.NumeroPontoVenda;
            cancelamento.TipoVenda = MapTipoVenda(cancelamentoRetornado.TipoVenda);
            cancelamento.TipoVendaDetalhado = GetTipoVendaDetalhado(cancelamentoRetornado.TipoVenda, cancelamentoRetornado.TipoTransacao);
            cancelamento.DataVenda = cancelamentoRetornado.DataTransacao;
            cancelamento.DataVendaTexto = cancelamentoRetornado.DataTransacao.ToString("dd/MM/yyyy");
            cancelamento.NSU = cancelamentoRetornado.NSU.ToString();
            cancelamento.ValorBruto = cancelamentoRetornado.ValorTransacao;
            cancelamento.SaldoDisponivel = cancelamentoRetornado.ValorSaldo;
            cancelamento.ValorCancelamento = cancelamentoRetornado.ValorCancelamento;
            cancelamento.NumeroAvisoCancelamento = cancelamentoRetornado.NumeroAvisoCancelamento;
            cancelamento.TipoCancelamento = MapTipoCancelamento(cancelamentoRetornado.TipoCancelamento);
            cancelamento.Status = TratarPalavrasMensagem(cancelamentoRetornado.Status);
            cancelamento.Origem = cancelamentoRetornado.Origem;
            cancelamento.DataCancelamento = cancelamentoRetornado.DataCancelamento;

            return cancelamento;
        }

        /// <summary>
        /// Tratar palavras do texto para tratar letra "Upper e Lower Case"
        /// </summary>
        /// <param name="strMensagem"></param>
        /// <returns></returns>
        private static string TratarPalavrasMensagem(string strMensagem)
        {
            string strMensagemTratada = string.Empty;

            if (!string.IsNullOrEmpty(strMensagem))
            {
                string[] vetPalavras = strMensagem.Split(' ');


                for (int i = 0; i < vetPalavras.Count(); i++)
                {
                    if (!string.IsNullOrEmpty(vetPalavras[i]))
                    {
                        //Primeira letra da palavra maiúscula, restantes minúsculas
                        //strMensagemTratada = vetPalavras[i].First().ToString().ToUpper() + String.Join("", vetPalavras[i].ToLower().Skip(1));

                        strMensagemTratada = String.Format("{0} {1}{2}", strMensagemTratada, vetPalavras[i].First().ToString().ToUpper(), vetPalavras[i].ToLower().Substring(1));
                    }
                }

                //Tratamento palavras acentuadas
                strMensagemTratada = strMensagemTratada.Replace("Nao", "Não");
            }

            return strMensagemTratada;
        }

        /// <summary>
        /// Monta o tipo de venda de detalhado da transação
        /// </summary>
        /// <param name="tipoVenda">Tipo de venda</param>
        /// <param name="tipoTransacao">Tipo de transação</param>
        /// <returns>Retorna uma string com o tipo de venda detalhado</returns>
        private String GetTipoVendaDetalhado(Agentes.KCCancelamentos.TipoVenda tipoVenda, Int32 tipoTransacao)
        {
            var retorno = String.Empty;

            if (tipoVenda == Agentes.KCCancelamentos.TipoVenda.Credito)
            {
                switch (tipoTransacao)
                {
                    case 1: retorno = String.Format(@"Crédito (À vista)");
                        break;
                    case 2: retorno = String.Format(@"Crédito (Parcelado s/ juros)");
                        break;
                    case 3: retorno = String.Format(@"Crédito (Parcelado c/ juros)");
                        break;
                    default: retorno = String.Format(@"Crédito (À vista)");
                        break;
                }
            }
            else if (tipoVenda == Agentes.KCCancelamentos.TipoVenda.Debito)
            {
                retorno = String.Format(@"Débito (À vista)");
            }

            return retorno;
        }

        /// <summary>
        /// Mapeia uma lista da entidade TransacaoServico do PA para uma lista da entidade SolicitacaoCancelamento do PN
        /// </summary>
        /// <param name="cancelamentosPA">Lista de cancelamentos</param>
        /// <returns>Retorna uma lista de cancelamentos</returns>
        private List<Modelo.SolicitacaoCancelamento> MapCancelamentos(List<Agentes.PATransacao.TransacaoServico> cancelamentosPA)
        {
            List<Modelo.SolicitacaoCancelamento> retorno = new List<Modelo.SolicitacaoCancelamento>();

            foreach (var c in cancelamentosPA)
            {
                retorno.Add(MapCancelamento(c));
            }

            return retorno;
        }

        /// <summary>
        /// Mapeia a entidade SolicitacaoCancelamento do PN para a entidade TransacaoCanceladaServico do PA
        /// </summary>
        /// <param name="cancelamentos">Lista de cancelamentos</param>
        /// <returns>Retorna uma lista da cancelamentos</returns>
        private List<Agentes.PATransacao.TransacaoCanceladaServico> MapCancelamentos(List<Modelo.SolicitacaoCancelamento> cancelamentos)
        {
            List<Agentes.PATransacao.TransacaoCanceladaServico> retorno = new List<Agentes.PATransacao.TransacaoCanceladaServico>();

            foreach (var c in cancelamentos)
            {
                var cancelamento = new Agentes.PATransacao.TransacaoCanceladaServico();
                cancelamento.NumeroEstabelecimento = c.NumeroEstabelecimentoVenda;
                cancelamento.NumeroReferenciaEstabelecimento = c.NumeroEstabelecimentoVenda.ToString();
                cancelamento.NumeroAvisoCancelamento = c.NumeroAvisoCancelamento.ToInt64();
                cancelamento.TipoDeVenda = MapTipoVenda(c.TipoVenda);
                cancelamento.DataTransacao = c.DataVenda;
                cancelamento.DataCancelamento = DateTime.Now;
                //cancelamento.NumeroNsu = c.NSU.ToInt64();
                //cancelamento.NumeroCartao = c.NSU;
                cancelamento.ValorBruto = c.ValorBruto;
                cancelamento.ValorSaldo = c.SaldoDisponivel;
                cancelamento.TipoCancelamento = MapTipoCancelamento(c.TipoCancelamento);
                cancelamento.ValorCancelamento = c.ValorCancelamento;
                cancelamento.TipoMoeda = Agentes.PATransacao.TipoDeMoeda.Real;
                cancelamento.CodigoCanal = 3;
                cancelamento.StatusCancelamento = Agentes.PATransacao.StatusDoCancelamento.NaoCancelada;
                cancelamento.TimestampTransacao = c.TimestampTransacao;
                cancelamento.NumeroMes = c.NumeroMes;
                cancelamento.TipoTransacao = MapTipoTransacao(c.TipoTransacao);
                cancelamento.CodigoRamoAtividadeDoEstabelecimento = c.CodigoRamoAtividade;
                cancelamento.DataCarta = c.DataVenda;

                if (c.NSU != null)
                {
                    cancelamento.NumeroCartao = c.NSU.Length > 12 ? c.NSU : null;
                    cancelamento.NumeroNsu = c.NSU.Length <= 12 ? c.NSU.ToInt64() : 0;
                }
                else
                {
                    cancelamento.NumeroCartao = c.NSU;
                    cancelamento.NumeroNsu = c.NSU.ToInt64();
                }
               
                //cancelamento.NumeroCartao = c.NSU != null ? (c.NSU.Length > 12 ? c.NSU : null) : c.NSU;
                //cancelamento.NumeroNsu = c.NSU != null ? (c.NSU.Length <= 12 ? c.NSU.ToInt64() : 0) : c.NSU.ToInt64();

                retorno.Add(cancelamento);
            }

            return retorno;
        }

        /// <summary>
        /// Mapeia a entidade Cancelamento do KC para a entidade SolicitacaoCancelamento do PN
        /// </summary>
        /// <param name="cancelamentosRetornados">Cancelamentos</param>
        /// <returns>Retorna uma lista de cancelamentos</returns>
        private List<Modelo.SolicitacaoCancelamento> MapCancelamentos(List<Agentes.KCCancelamentos.Cancelamento> cancelamentosRetornados)
        {
            List<Modelo.SolicitacaoCancelamento> retorno = new List<Modelo.SolicitacaoCancelamento>();

            foreach (var cancelamentoRetornado in cancelamentosRetornados)
            {
                retorno.Add(MapCancelamento(cancelamentoRetornado));
            }

            return retorno;
        }

        /// <summary>
        /// Mapeia a entidade TransacaoCanceladaServico do PA para a entidade SolicitacaoCancelamento do PN
        /// </summary>
        /// <param name="cancelamentosPA">Cancelamentos</param>
        /// <returns>Retorna uma lista de cancelamentos</returns>
        private List<Modelo.SolicitacaoCancelamento> MapCancelamentos(List<Agentes.PATransacao.TransacaoCanceladaServico> cancelamentosPA)
        {
            List<Modelo.SolicitacaoCancelamento> cancelamentos = new List<Modelo.SolicitacaoCancelamento>();

            foreach (var c in cancelamentosPA)
            {
                var cancelamento = new Modelo.SolicitacaoCancelamento();
                cancelamento.NumeroEstabelecimentoCancelamento = c.NumeroEstabelecimentoCancelamento;
                cancelamento.DataCancelamento = c.DataCancelamento;
                cancelamento.NumeroEstabelecimentoVenda = c.NumeroEstabelecimento;
                cancelamento.TipoVenda = MapTipoVenda(c.TipoDeVenda);
                cancelamento.TipoVendaDetalhado = MapTipoVendaDetalhado(c);
                cancelamento.DataVenda = c.DataTransacao;
                cancelamento.NSU = c.NumeroNsu == null ? "" : c.NumeroNsu.ToString();
                cancelamento.ValorBruto = c.ValorBruto;
                cancelamento.SaldoDisponivel = c.ValorSaldo;
                cancelamento.TipoCancelamento = MapTipoCancelamento(c.TipoCancelamento);
                cancelamento.TipoCancelamento = c.ValorBruto == c.ValorCancelamento ? Modelo.TipoCancelamento.Total : Modelo.TipoCancelamento.Parcial;
                cancelamento.NumeroAvisoCancelamento = c.NumeroAvisoCancelamento.ToString();
                cancelamento.Origem = c.DescricaoCanalCancelamento;
                cancelamento.ValorCancelamento = c.ValorCancelamento;
                cancelamento.Status = "Efetivado";

                cancelamentos.Add(cancelamento);
            }

            return cancelamentos;
        }

        /// <summary>
        /// Retorna o tipo de venda detalhado
        /// </summary>
        /// <param name="transacao">Transação</param>
        /// <returns>Retorna o tipo de transação detalhado em string</returns>
        private String MapTipoVendaDetalhado(Agentes.PATransacao.TransacaoCanceladaServico transacao)
        {
            var retorno = String.Empty;

            switch (transacao.TipoDeVenda)
            {
                case Agentes.PATransacao.TipoDeVenda.Credito: retorno = "Crédito";
                    break;
                case Agentes.PATransacao.TipoDeVenda.Debito: retorno = "Débito";
                    break;
                default: break;
            }

            switch (transacao.TipoTransacao)
            {
                case Agentes.PATransacao.TipoDetalhamentoTransacao.ROT: retorno = String.Format("{0} (À Vista)", retorno);
                    break;
                case Agentes.PATransacao.TipoDetalhamentoTransacao.PAR:
                    var detalhes = new Agentes.CancelamentoAG().BuscarDetalhesTransacaoCredito(transacao.NumeroEstabelecimento, transacao.TimestampTransacao, transacao.DataTransacao, transacao.NumeroMes);
                    retorno = String.Format("{0} ({1})", retorno, detalhes.DescricaoTipoDetalheTransacao);
                    break;
                case Agentes.PATransacao.TipoDetalhamentoTransacao.REC: retorno = String.Format("{0}", retorno);
                    break;
                case Agentes.PATransacao.TipoDetalhamentoTransacao.DEB: retorno = String.Format("{0} (À Vista)", retorno);
                    break;
                case Agentes.PATransacao.TipoDetalhamentoTransacao.PRE: retorno = String.Format("{0} (Pré data.)", retorno);
                    break;
                default: break;
            }

            return retorno;
        }

        /// <summary>
        /// Retorna o tipo de venda detalhado
        /// </summary>
        /// <param name="transacao">Transação</param>
        /// <returns>Retorna o tipo de transação detalhado em string</returns>
        private String MapTipoVendaDetalhado(Agentes.PATransacao.TransacaoServico transacao)
        {
            var retorno = String.Empty;

            switch (transacao.TipoDeVenda)
            {
                case Agentes.PATransacao.TipoDeVenda.Credito: retorno = "Crédito";
                    break;
                case Agentes.PATransacao.TipoDeVenda.Debito: retorno = "Débito";
                    break;
                default: break;
            }

            switch (transacao.TipoTransacao)
            {
                case Agentes.PATransacao.TipoDetalhamentoTransacao.ROT: retorno = String.Format("{0} (À Vista)", retorno);
                    break;
                case Agentes.PATransacao.TipoDetalhamentoTransacao.PAR:
                    var detalhes = new Agentes.CancelamentoAG().BuscarDetalhesTransacaoCredito(transacao.NumeroEstabelecimento, transacao.TimestampTransacao, transacao.DataTransacao, transacao.NumeroMes);
                    retorno = String.Format("{0} ({1})", retorno, detalhes.DescricaoTipoDetalheTransacao);
                    break;
                case Agentes.PATransacao.TipoDetalhamentoTransacao.REC: retorno = String.Format("{0}", retorno);
                    break;
                case Agentes.PATransacao.TipoDetalhamentoTransacao.DEB: retorno = String.Format("{0} (À Vista)", retorno);
                    break;
                case Agentes.PATransacao.TipoDetalhamentoTransacao.PRE: retorno = String.Format("{0} (Pré data.)", retorno);
                    break;
                default: break;
            }

            return retorno;
        }

        /// <summary>
        /// Mapeia a enum TipoVenda do PN para a enum TipoDeVenda1 do PA
        /// </summary>
        /// <param name="tipoVenda">Tipo de venda</param>
        /// <returns>Retorna o tipo de venda</returns>
        private Agentes.PATransacao.TipoDeVenda MapTipoVenda1(Modelo.TipoVenda tipoVenda)
        {
            switch (tipoVenda)
            {
                case Modelo.TipoVenda.Credito:
                    return Agentes.PATransacao.TipoDeVenda.Credito;

                case Modelo.TipoVenda.Debito:
                    return Agentes.PATransacao.TipoDeVenda.Debito;

                default:
                    return Agentes.PATransacao.TipoDeVenda.Credito;
            }
        }

        /// <summary>
        /// Mapeia a enum TipoVenda do PN para a enum TipoDeVenda do PA
        /// </summary>
        /// <param name="tipoVenda">Tipo de venda</param>
        /// <returns>Retorna o tipo de venda</returns>
        private Agentes.PATransacao.TipoDeVenda MapTipoVenda(Modelo.TipoVenda tipoVenda)
        {
            switch (tipoVenda)
            {
                case Modelo.TipoVenda.Credito:
                    return Agentes.PATransacao.TipoDeVenda.Credito;

                case Modelo.TipoVenda.Debito:
                    return Agentes.PATransacao.TipoDeVenda.Debito;

                default:
                    return Agentes.PATransacao.TipoDeVenda.Credito;
            }
        }

        /// <summary>
        /// Mapeia a enum TipoDeVenda do PA para a enum TipoVenda do PN
        /// </summary>
        /// <param name="tipoDeVenda">Tipo de venda</param>
        /// <returns>Retorna o tipo de venda</returns>
        private Modelo.TipoVenda MapTipoVenda(Agentes.PATransacao.TipoDeVenda tipoDeVenda)
        {
            switch (tipoDeVenda)
            {
                case Agentes.PATransacao.TipoDeVenda.Credito:
                    return Modelo.TipoVenda.Credito;

                case Agentes.PATransacao.TipoDeVenda.Debito:
                    return Modelo.TipoVenda.Debito;

                default:
                    return Modelo.TipoVenda.Credito;
            }
        }

        /// <summary>
        /// Mapeia a enum TipoVenda do KC para a enum TipoVenda do PN
        /// </summary>
        /// <param name="tipoVenda">Tipo de venda</param>
        /// <returns>Retorna o tipo de venda</returns>
        private Modelo.TipoVenda MapTipoVenda(Agentes.KCCancelamentos.TipoVenda tipoVenda)
        {
            switch (tipoVenda)
            {
                case Agentes.KCCancelamentos.TipoVenda.Credito:
                    return Modelo.TipoVenda.Credito;

                case Agentes.KCCancelamentos.TipoVenda.Debito:
                    return Modelo.TipoVenda.Debito;

                default:
                    return Modelo.TipoVenda.Credito;
            }
        }

        /// <summary>
        /// Mapeia o tipoCancelamento do PA para a enum TipoCancelamento do PN
        /// </summary>
        /// <param name="tipoCancelamento">Tipo de cancelamento</param>
        /// <returns>Retorna o tipo de cancelamento</returns>
        private Modelo.TipoCancelamento MapTipoCancelamento(Char tipoCancelamento)
        {
            switch (tipoCancelamento)
            {
                case 'T': return Modelo.TipoCancelamento.Total;
                case 'P': return Modelo.TipoCancelamento.Parcial;
                default: return Modelo.TipoCancelamento.Total;
            }
        }

        /// <summary>
        /// Mapeia a enum TipoCancelamento do PN para o tipoCancelamento do PA
        /// </summary>
        /// <param name="tipoCancelamento">Tipo de cancelamento</param>
        /// <returns>Retorna o tipo de cancelamento</returns>
        private Char MapTipoCancelamento(Modelo.TipoCancelamento tipoCancelamento)
        {
            switch (tipoCancelamento)
            {
                case Modelo.TipoCancelamento.Total: return 'T';
                case Modelo.TipoCancelamento.Parcial: return 'P';
                default: return 'T';
            }
        }


        /// <summary>
        /// Mapeia a enum TipoCancelamento do KC para a enum TipoCancelamento do PN
        /// </summary>
        /// <param name="tipoCancelamento">Tipo de cancelamento</param>
        /// <returns>Retorna o tipo de cancelamento</returns>
        private Modelo.TipoCancelamento MapTipoCancelamento(Agentes.KCCancelamentos.TipoCancelamento tipoCancelamento)
        {
            switch (tipoCancelamento)
            {
                case Agentes.KCCancelamentos.TipoCancelamento.Total: return Modelo.TipoCancelamento.Total;
                case Agentes.KCCancelamentos.TipoCancelamento.Parcial: return Modelo.TipoCancelamento.Parcial;
                default: return Modelo.TipoCancelamento.Total;
            }
        }

        /// <summary>
        /// Mapeia a entidade ValidadorTransacaoFEServico do PA para a entidade Validacao do PN
        /// </summary>
        /// <param name="validacaoPA">Lista de validações</param>
        /// <returns>Retorna uma lista de validações</returns>
        private List<Modelo.Validacao> MapValidacoes(List<Agentes.PATransacao.ValidadorTransacaoFEServico> validacoesPA)
        {
            List<Modelo.Validacao> retorno = new List<Modelo.Validacao>();

            foreach (var validacao in validacoesPA)
            {
                retorno.Add(new Modelo.Validacao
                {
                    CodigoRetorno = validacao.CodigoRetorno,
                    Descricao = validacao.Descricao,
                    Status = validacao.Status
                });
            }

            return retorno;
        }

        #endregion
    }
}
