using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Redecard.PN.Comum;
using Redecard.PN.DataCash.Modelos;

namespace Redecard.PN.DataCash.Agentes
{
    /// <summary>
    /// Classe Agente para as funcionalidades do sub-módulo Gerenciamento de Vendas do DataCash.
    /// </summary>
    public class Gerenciamento : AgentesBase
    {
        #region [ Pré-Autorização (Cancelamento/Estorno/Confirmação) | Estorno de Vendas ]

        /// <summary>
        /// Estorno de Vendas<br/>
        /// Cancelamento de Pré-Autorização<br/>
        /// Estorno de Pré-Autorização<br/>
        /// Confirmação de Pré-Autorização
        /// </summary>
        /// <param name="tid">Número TID</param>
        /// <param name="pv">Número do Estabelecimento</param>
        /// <returns>Dados da transação</returns>
        public DadosTransacao GetDadosTransacao(String tid, Int32 pv)
        {
            using (Logger Log = Logger.IniciarLog("Pré-Autorização (Cancelamento/Estorno/Confirmação) / Estorno de Vendas"))
            {
                try
                {
                    //Variável de retorno
                    DadosTransacao retorno;

                    Log.GravarLog(EventoLog.InicioAgente, new { tid, pv });

#if !DEBUG
                    using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
                        retorno = contexto.Cliente.GetDadosTransacao(tid, pv);
#else
                    retorno = new DadosTransacao
                    {
                        CodigoRetorno = 1,
                        AuthCode = "54648756434273",
                        DataConfimacao = DateTime.Now,
                        DataExecucao = DateTime.Now.ToString("dd/MM/yyyy"),
                        DataPreAutorizacao = DateTime.Now.AddDays(1),
                        HoraConfimacao = DateTime.Now.ToString("HH:mm:ss"),
                        HoraPreAutorizacao = DateTime.Now.AddDays(1).ToString("HH:mm:ss"),
                        NSU = "5544363564634534",
                        NumeroCartao = 998877665,
                        TipoTransacao = "Crédito",
                        ValidadePreAutorizacao = DateTime.Now.AddDays(30),
                        Valor = 1000000.25m,
                        ValorPreAutorizacao = 7890123
                    };
#endif

                    Log.GravarLog(EventoLog.FimAgente, retorno);

                    return retorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #endregion [ Fim: Pré-Autorização (Cancelamento/Estorno/Confirmação) | Estorno de Vendas ]

        #region [ Relatório Transações Recorrentes - Agendado ]

        /// <summary>
        /// Consulta dados para montagem do Relatório de Transações Recorrentes - Agendado.
        /// </summary>
        /// <param name="parametros">Parâmetros para busca do relatório de Transações Recorrentes - Agendado</param>
        /// <returns>Dados do Relatório de Transações Recorrentes - Agendado</returns>
        public RetornoRelatorioFireAndForget GetRelatorioFireAndForget(ParametrosRelatorioFireAndForget parametros)
        {
            using (Logger Log = Logger.IniciarLog("Relatório de Transações Recorrentes - Agendado"))
            {
                try
                {
                    //Variável de retorno
                    RetornoRelatorioFireAndForget retorno;

                    Log.GravarLog(EventoLog.InicioAgente, parametros);

#if !DEBUG
                    using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
                        retorno = contexto.Cliente.GetRelatorioFireAndForget(parametros);
#else
                    retorno = new RetornoRelatorioFireAndForget
                    {
                        CodigoRetorno = 1,
                        QuantidadePaginas = 15,
                        Transacoes = new List<RegistroTransacaoFireAndForget>()
                    };

                    for (Int32 i = parametros.numerodapagina * parametros.linhasporpagina;
                        i < (parametros.numerodapagina + 1) * parametros.linhasporpagina; i++)
                    {
                        retorno.Transacoes.Add(new RegistroTransacaoFireAndForget
                        {
                            DataInicio = DateTime.Now.Subtract(TimeSpan.FromDays(30)),
                            DataPrimeiraCobranca = DateTime.Now.Subtract(TimeSpan.FromDays(15)),
                            DataUltimaCobranca = DateTime.Now.Add(TimeSpan.FromDays(30)),
                            DataVenda = DateTime.Now,
                            Bandeira = "Bandeira " + i,
                            Frequencia = "Freq" + i,
                            NumeroCartao = i.ToString("0000"),
                            NumeroPedido = i.ToString("000000000"),
                            QuantidadeRecorrencias = i + 1,
                            RecorrenciasRestantes = 30 - i,
                            TID = i.ToString(),
                            ValorPrimeiraCobranca = 10000 + i,
                            ValorUltimaCobranca = 20000 + i,
                            ValorRecorente = 30000 + i,
                            Status = new[] { "Ativa", "Cancelada" }[i % 4 != 0 ? 0 : 1]
                        });
                    }
#endif

                    Log.GravarLog(EventoLog.FimAgente, retorno);

                    return retorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #endregion [ Fim: Relatório Transações Recorrentes - Agendado ]

        #region [ Relatório Transações Recorrentes - Histórico ]

        /// <summary>
        /// Consulta dados para montagem do Relatório de Transações Recorrentes - Histórico.
        /// </summary>
        /// <param name="parametros">Parâmetros para busca do relatório de Transações Recorrentes - Histórico</param>
        /// <returns>Dados do Relatório de Transações Recorrentes - Histórico</returns>
        public RetornoRelatorioHistoricRecurring GetRelatorioHistoricRecurring(ParametrosRelatorioHistoricRecurring parametros)
        {
            using (Logger Log = Logger.IniciarLog("Relatório de Transações Recorrentes - Histórico"))
            {
                try
                {
                    //Variável de retorno
                    RetornoRelatorioHistoricRecurring retorno;

                    Log.GravarLog(EventoLog.InicioAgente, parametros);

#if !DEBUG
                    using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
                        retorno = contexto.Cliente.GetRelatorioHistoricRecurring(parametros);
#else
                    retorno = new RetornoRelatorioHistoricRecurring
                    {
                        CodigoRetorno = 1,
                        QuantidadePaginas = 15,
                        Transacoes = new List<RegistroTransacaoHistoricRecurring>()
                    };

                    for (Int32 i = parametros.numerodapagina * parametros.linhasporpagina;
                        i < (parametros.numerodapagina + 1) * parametros.linhasporpagina; i++)
                    {
                        retorno.Transacoes.Add(new RegistroTransacaoHistoricRecurring
                        {
                            DataVenda = DateTime.Now,
                            Bandeira = "Bandeira " + i,
                            NumeroCartao = i.ToString("0000"),
                            NumeroPedido = i.ToString("000000000"),
                            TID = i.ToString(),
                            ValorVenda = 10000 + i,
                            NumeroConta = new Random(i).Next(10000, 99999).ToString(),
                            Status = new[] { "Ativa", "Cancelada" }[i % 4 != 0 ? 0 : 1]
                        });
                    }
#endif

                    Log.GravarLog(EventoLog.FimAgente, retorno);

                    return retorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #endregion [ Fim: Relatório Transações Recorrentes - Histórico ]

        #region [ Relatório de Transações ]

        /// <summary>
        /// Consulta dados para montagem do Relatório de Transações.
        /// </summary>
        /// <param name="parametros">Parâmetros para busca do relatório</param>
        /// <returns>Dados do Relatório de Transações</returns>
        public RetornoRelatorioTransacoes GetRelatorioTransacoes(ParametrosRelatorioTransacoes parametros)
        {
            using (Logger Log = Logger.IniciarLog("Relatório de Transações"))
            {
                try
                {
                    //Variável de retorno
                    RetornoRelatorioTransacoes retorno;

                    Log.GravarLog(EventoLog.InicioAgente, parametros);

                    using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
                        retorno = contexto.Cliente.GetRelatorioTransacoes(parametros);

                    Log.GravarLog(EventoLog.FimAgente, retorno);

                    return retorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #endregion [ FIM: Relatório de Transações ]

        #region [ Performance de Autorização ]

        /// <summary>
        /// Performance de Autorização:<br/>
        /// O total é a soma das propriedades TotalTransacoesAprovadas + TotalTransacoesReprovadas
        /// </summary>
        /// <param name="dataInicio">Data de início</param>
        /// <param name="dataFim">Data de término</param>
        /// <param name="pv">Número do estabelecimento</param>
        /// <returns>Total Transações</returns>
        public TotalTransacoes GetTotalTransacoes(DateTime dataInicio, DateTime dataFim, Int32 pv)
        {
            using (Logger Log = Logger.IniciarLog("Performance de Autorização"))
            {
                try
                {
                    //Variável de retorno
                    TotalTransacoes retorno;

                    Log.GravarLog(EventoLog.InicioAgente, new { dataInicio, dataFim, pv });

#if !DEBUG
                    using (var contexto = new ContextoWCF<DataCashConsultaService.ConsultaTransactionClient>())
                        retorno = contexto.Cliente.GetTotalTransacoes(dataInicio, dataFim, pv);
#else
                    var r = new Random();
                    retorno = new TotalTransacoes
                    {
                        CodigoRetorno = 1,
                        TotalTransacoesAprovadas = r.Next(1000, 1000000),
                        TotalTransacoesReprovadas = r.Next(1000, 1000000)
                    };
#endif

                    Log.GravarLog(EventoLog.FimAgente, retorno);

                    return retorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #endregion [ Fim: Performance de Autorização ]

        
        #region [ TODO: Comprovação de Vendas ]
        #endregion [ Fim: Comprovação de Vendas ]
    }
}