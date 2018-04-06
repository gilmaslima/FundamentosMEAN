using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;

namespace Rede.PN.Cancelamento.Agentes
{
    /// <summary>
    /// Classe agentes para o cancelamento
    /// </summary>
    public class CancelamentoAG : AgentesBase
    {
        #region [ Constantes ]

        /// <summary>
        /// Código de erro genérico do agentes
        /// </summary>
        public static Int32 CodigoErro { get { return 500; } }
        
        /// <summary>
        /// Fonte
        /// </summary>
        public static String Fonte { get { return "Rede.PN.Cancelamento.Agentes"; } }
        
        #endregion

        /// <summary>
        /// Busca Transações Canceladas
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="numeroEstabelecimento">Número do estabelecimento</param>
        /// <param name="indicadorPesquisa">Indicador do tipo de pesquisa</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="numeroAvisoCancelamento">Número do aviso do cancelamento</param>
        /// <param name="numeroNsu">Número de sequência único</param>
        /// <param name="tipoVenda">Tipo de venda</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="numeroAvisoRechamada">Número do aviso rechamada</param>
        /// <param name="numeroProgramaRechamada">Nome do programa rechamada</param>
        /// <param name="numeroNsuRechamada">Número de sequência único rechamada</param>
        /// <param name="dataTransacaoRechamada">Data da transação rechamada</param>
        /// <param name="timestampTransacaoRechamada">Timestamp de transação rachamada</param>
        /// <param name="timestampCancelamentoRechamada">Timestamp do cancelamento rechamada</param>
        /// <param name="totalRegistros">Total de registros</param>
        /// <returns>Lista de cancelamentos</returns>
        public List<PATransacao.TransacaoCanceladaServico> BuscarCancelamentos(String codigoUsuario,
                    Int32 numeroEstabelecimento,
                    String indicadorPesquisa,
                    DateTime dataInicial,
                    DateTime dataFinal,
                    Int64 numeroAvisoCancelamento,
                    Int64 numeroNsu,
                    Rede.PN.Cancelamento.Agentes.PATransacao.TipoDeVenda tipoVenda,
                    ref String indicadorRechamada,
                    ref Decimal numeroAvisoRechamada,
                    ref String numeroProgramaRechamada,
                    ref Decimal numeroNsuRechamada,
                    ref String dataTransacaoRechamada,
                    ref String timestampTransacaoRechamada,
                    ref String timestampCancelamentoRechamada,
                    out Int32 totalRegistros)
        {
            List<PATransacao.TransacaoCanceladaServico> retorno = new List<PATransacao.TransacaoCanceladaServico>();

            using (var log = Logger.IniciarLog("Buscar Cancelamentos - Agente"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
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
                    using (var contexto = new ContextoWCF<PATransacao.ServicoTransacaoClient>())
                    {
                        retorno = contexto.Cliente.ObterCancelamentos(codigoUsuario,
                            numeroEstabelecimento,
                            indicadorPesquisa,
                            dataInicial,
                            dataFinal,
                            numeroAvisoCancelamento,
                            numeroNsu,
                            tipoVenda,
                            ref indicadorRechamada,
                            ref numeroAvisoRechamada,
                            ref numeroProgramaRechamada,
                            ref numeroNsuRechamada,
                            ref dataTransacaoRechamada,
                            ref timestampTransacaoRechamada,
                            ref timestampCancelamentoRechamada,
                            out totalRegistros);
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

                log.GravarLog(EventoLog.FimAgente, new
                {
                    retorno,
                    indicadorRechamada,
                    numeroAvisoRechamada,
                    numeroProgramaRechamada,
                    numeroNsuRechamada,
                    dataTransacaoRechamada,
                    timestampTransacaoRechamada,
                    timestampCancelamentoRechamada,
                    totalRegistros
                });
            }

            return retorno;
        }

        /// <summary>
        /// Inclui solicitações de cancelamento
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="transacoes">Transações para cancelamento</param>
        /// <returns>Retorna uma lista de cancelamentos</returns>
        public List<PATransacao.TransacaoCanceladaServico> IncluirCancelamentos(String codigoUsuario, List<PATransacao.TransacaoCanceladaServico> transacoes)
        {
            List<PATransacao.TransacaoCanceladaServico> retorno = new List<PATransacao.TransacaoCanceladaServico>();

            using (var log = Logger.IniciarLog("Incluir Cancelamentos - Agente"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codigoUsuario,
                    transacoes
                });

                try
                {
                    using (var contexto = new ContextoWCF<PATransacao.ServicoTransacaoClient>())
                    {
                        retorno = contexto.Cliente.InclusaoCancelamentoTransacaoCredito(codigoUsuario, transacoes, null);
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

                log.GravarLog(EventoLog.FimAgente, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Valida solicitações de cancelamento
        /// </summary>
        /// <param name="tipoOperacao">Tipo de operação - 'B' = validações</param>
        /// <param name="transacoes">Trasações para validação</param>
        /// <returns>Retorna uma lista de retornos das validações</returns>
        public List<PATransacao.ValidadorTransacaoFEServico> ValidarParametrosBloqueio(Char tipoOperacao, List<PATransacao.TransacaoCanceladaServico> transacoes)
        {
            List<PATransacao.ValidadorTransacaoFEServico> retorno = new List<PATransacao.ValidadorTransacaoFEServico>();

            using (var log = Logger.IniciarLog("Validar Parâmetros Bloqueio - Agente"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    tipoOperacao,
                    transacoes
                });

                try
                {
                    using (var contexto = new ContextoWCF<PATransacao.ServicoTransacaoClient>())
                    {
                        retorno = contexto.Cliente.ValidarParametrosBloqueio(tipoOperacao, transacoes);
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

                log.GravarLog(EventoLog.FimAgente, new
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
        /// <param name="valorBrutoVenda">Valor bruto da venda</param>
        /// <param name="tipoDeVenda">Tipo de venda</param>
        /// <returns>Retorna dados detalhados da transação para cancelamento</returns>
        public PATransacao.TransacaoServico BuscarTransacaoParaCancelamento(Int32 numeroEstabelecimento,
                    DateTime dataInicial,
                    String numeroNSUouCartao,
                    String timestampTransacao,
                    Int16 numeroMes,
                    PATransacao.TipoDetalhamentoTransacao tipoTransacao,
                    PATransacao.TipoDeVenda tipoVenda)
        {
            PATransacao.TransacaoServico retorno = new PATransacao.TransacaoServico();

            using (var log = Logger.IniciarLog("Buscar Transação Para Cancelamento - Agente"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
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
                    using (var contexto = new ContextoWCF<PATransacao.ServicoTransacaoClient>())
                    {
                        retorno = contexto.Cliente.ObterTransacaoParaCancelamento(numeroEstabelecimento, dataInicial, numeroNSUouCartao, timestampTransacao, numeroMes, tipoTransacao, tipoVenda);
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

                log.GravarLog(EventoLog.FimAgente, new
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
        /// <returns>Retorna uma lista de transações com o mesmo ponto de venda, data da transação, tipo de venda e nsu/cartão</returns>
        public List<PATransacao.TransacaoServico> BuscarTransacaoDuplicadaParaCancelamento(Int32 numeroEstabelecimento, DateTime dataVenda, String numeroNSUouCartao, PATransacao.TipoDeVenda tipoVenda)
        {
            List<PATransacao.TransacaoServico> retorno = new List<PATransacao.TransacaoServico>();

            using (var log = Logger.IniciarLog("Buscar Transação Duplicada Para Cancelamento - Agente"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    numeroEstabelecimento,
                    dataVenda,
                    numeroNSUouCartao,
                    tipoVenda
                });

                try
                {
                    using (var contexto = new ContextoWCF<PATransacao.ServicoTransacaoClient>())
                    {
                        retorno = contexto.Cliente.ObterTransacoesDuplicadasParaCancelamento(numeroEstabelecimento, dataVenda, numeroNSUouCartao, tipoVenda);
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

                log.GravarLog(EventoLog.FimAgente, new
                {
                    retorno
                });
            }

            return retorno;
        }

        /// <summary>
        /// Retorna os detalhes de uma transação
        /// </summary>
        /// <param name="numeroEstabelecimento">Número do estabelecimento</param>
        /// <param name="timestampTransacao">Timestamp da transação</param>
        /// <param name="dataTransacao">Data da transação</param>
        /// <param name="numeroMes">Número do mês</param>
        /// <returns>Busca detalhes de uma transação de crédito</returns>
        public PATransacao.TransacaoServico BuscarDetalhesTransacaoCredito(Int32 numeroEstabelecimento, String timestampTransacao, DateTime dataTransacao, Int32 numeroMes)
        {
            PATransacao.TransacaoServico retorno = new PATransacao.TransacaoServico();

            using (var log = Logger.IniciarLog("Buscar Detalhes da Transação de Crédito - Agente"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    numeroEstabelecimento,
                    timestampTransacao,
                    dataTransacao,
                    numeroMes
                });

                try
                {
                    using (var contexto = new ContextoWCF<PATransacao.ServicoTransacaoClient>())
                    {
                        retorno = contexto.Cliente.ObterDetalhesDaTransacaoSelecionada(new PATransacao.TransacaoServico
                        {
                            NumeroEstabelecimento = numeroEstabelecimento,
                            TimestampTransacao = timestampTransacao,
                            DataTransacao = dataTransacao,
                            NumeroMes = numeroMes,
                            TipoDeVenda = PATransacao.TipoDeVenda.Credito,
                            TipoTransacao = PATransacao.TipoDetalhamentoTransacao.PAR
                        });
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

                log.GravarLog(EventoLog.FimAgente, new
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
        /// <param name="transacoes">Cancelamentos</param>
        /// <returns>Retorna uma lista de retorno dos cancelamentos anuladas</returns>
        public List<PATransacao.ValidadorTransacaoFEServico> AnularCancelamentos(String codigoUsuario, Int16 codigoCanal, List<PATransacao.TransacaoCanceladaServico> transacoes)
        {
            List<PATransacao.ValidadorTransacaoFEServico> retorno = new List<PATransacao.ValidadorTransacaoFEServico>();

            using (var log = Logger.IniciarLog("Anular Cancelamentos - Agente"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    codigoUsuario,
                    codigoCanal,
                    transacoes
                });

                try
                {
                    using (var contexto = new ContextoWCF<PATransacao.ServicoTransacaoClient>())
                    {
                        retorno = contexto.Cliente.AnularCancelamentos(codigoUsuario, codigoCanal, transacoes);
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

                log.GravarLog(EventoLog.FimAgente, new
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

            using (var log = Logger.IniciarLog("Verifica estabelecimento em matriz - Agente"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    numeroEstabelecimento,
                    numeroMatriz
                });

                try
                {
                    using (var contexto = new ContextoWCF<PATransacao.ServicoTransacaoClient>())
                    {
                        retorno = contexto.Cliente.VerificarEstabelecimentoEmMatriz(numeroEstabelecimento, numeroMatriz);
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

                log.GravarLog(EventoLog.FimAgente, new
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
        /// <param name="numeroAvisoCancelamento">Número do aviso do cancelamento</param>
        /// <param name="numeroCartao">Número do cartão</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <returns>Retorna uma lista de cancelamentos desfeitos pelo FMS</returns>
        public List<KCCancelamentos.Cancelamento> BuscaCancelamentosDesfeitosPorPeriodo(DateTime dataInicial, DateTime dataFinal, Decimal numeroAvisoCancelamento, String numeroCartao, Int32 numeroPontoVenda)
        {
            List<KCCancelamentos.Cancelamento> cancelamentos = new List<KCCancelamentos.Cancelamento>();

            using (var log = Logger.IniciarLog("Busca Cancelamentos Desfeitos por Período - Agentes"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    dataInicial,
                    dataFinal,
                    numeroAvisoCancelamento,
                    numeroCartao,
                    numeroPontoVenda
                });

                try
                {
                    KCCancelamentos.ParametrosBuscaCancelamentos parametrosBuscaCancelamentos = new KCCancelamentos.ParametrosBuscaCancelamentos();
                    parametrosBuscaCancelamentos.DataInicial = dataInicial;
                    parametrosBuscaCancelamentos.DataFinal = dataFinal;
                    parametrosBuscaCancelamentos.NumeroAvisoCancelamento = numeroAvisoCancelamento;
                    parametrosBuscaCancelamentos.NumeroCartao = numeroCartao;
                    parametrosBuscaCancelamentos.NumeroPontoVenda = numeroPontoVenda;

                    using (var contexto = new ContextoWCF<KCCancelamentos.ServicoKCCancelamentosClient>())
                    {
                        cancelamentos = contexto.Cliente.BuscaCancelamentosDesfeitosPorPeriodo(parametrosBuscaCancelamentos);
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

                log.GravarLog(EventoLog.FimAgente, new
                {
                    cancelamentos
                });
            }

            return cancelamentos;
        }

        /// <summary>
        /// Busca cancelamentos desfeitos por Ponto de Venda e Número de Aviso
        /// </summary>
        /// <param name="numeroAvisoCancelamento">Número do aviso do cancelamento</param>
        /// <param name="numeroCartao">Número do cartão</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <returns>Retorna uma lista de cancelamentos desfeitos pelo FMS</returns>
        public List<KCCancelamentos.Cancelamento> BuscaCancelamentosDesfeitosPorPontoVendaENumeroAviso(Decimal numeroAvisoCancelamento, String numeroCartao, Int32 numeroPontoVenda)
        {
            List<KCCancelamentos.Cancelamento> cancelamentos = new List<KCCancelamentos.Cancelamento>();

            using (var log = Logger.IniciarLog("Busca Cancelamentos Desfeitos por Ponto de Venda e Número do Aviso - Agentes"))
            {
                log.GravarLog(EventoLog.InicioAgente, new
                {
                    numeroAvisoCancelamento,
                    numeroCartao,
                    numeroPontoVenda
                });

                try
                {
                    KCCancelamentos.ParametrosBuscaCancelamentos parametrosBuscaCancelamentos = new KCCancelamentos.ParametrosBuscaCancelamentos();
                    parametrosBuscaCancelamentos.NumeroAvisoCancelamento = numeroAvisoCancelamento;
                    parametrosBuscaCancelamentos.NumeroCartao = numeroCartao;
                    parametrosBuscaCancelamentos.NumeroPontoVenda = numeroPontoVenda;

                    using (var contexto = new ContextoWCF<KCCancelamentos.ServicoKCCancelamentosClient>())
                    {
                        cancelamentos = contexto.Cliente.BuscaCancelamentosDesfeitosPorPontoVendaENumeroAviso(parametrosBuscaCancelamentos);
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

                log.GravarLog(EventoLog.FimAgente, new
                {
                    cancelamentos
                });
            }

            return cancelamentos;
        }

        #endregion
    }
}
