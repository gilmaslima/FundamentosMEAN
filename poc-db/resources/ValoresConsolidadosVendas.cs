/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Modelo.RelatorioValoresConsolidadosVendas;
using Redecard.PN.Comum;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Redecard.PN.Extrato.Negocio
{
    /// <summary>
    /// Turquia - Valores Consolidados de Vendas
    /// </summary>
    public class ValoresConsolidadosVendas : RegraDeNegocioBase
    {
        /// <summary>
        /// Instacia atual da classe. (Singleton)
        /// </summary>
        private static ValoresConsolidadosVendas instancia;

        /// <summary>
        /// Retorna a instacia atual da classe ou cria uma nova. (Singleton)
        /// </summary>
        public static ValoresConsolidadosVendas Instancia { get { return instancia ?? (instancia = new ValoresConsolidadosVendas()); } }

        /// <summary>
        /// Consulta o total de vendas realizadas á crédito e á débito no período informado.<br/>        
        /// - Book's BKWA2510 / Programa WAC251 / TranID ISGM e BKWA2520 / Programa WAC252 / TranID ISGN / Método ConsultarTotalVendasPorPeriodoConsolidado
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2510 / Programa WAC251 / TranID ISGM / Método ConsultarTotalVendasPorPeriodoConsolidado
        /// </remarks>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito e débito do período.</returns>
        public TotalVendasPorPeriodoConsolidado ConsultarTotalVendasPorPeriodoConsolidado(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas por período consolidado - BKWA2520 / WAC252 / ISGN e BKWA2520 / Programa WAC252 / TranID ISGN"))
            {
                try
                {
                    //Consulta dados crédito
                    TotalVendasCreditoPorPeriodo totalVendasCreditoPorPeriodo =
                        Agentes.ValoresConsolidadosVendas.Instancia.ConsultarTotalVendasCreditoPorPeriodo(dataInicio, dataFim, pvs, out status);

                    //Consulta dados débito
                    TotalVendasDebitoPorPeriodo totalVendasDebitoPorPeriodo =
                         Agentes.ValoresConsolidadosVendas.Instancia.ConsultarTotalVendasDebitoPorPeriodo(dataInicio, dataFim, pvs, out status);

                    Decimal totalBrutoVendasCredito = totalVendasCreditoPorPeriodo != null ? totalVendasCreditoPorPeriodo.TotalBruto : 0;
                    Decimal totaBrutoVendasDebito = totalVendasDebitoPorPeriodo != null ? totalVendasDebitoPorPeriodo.TotalBruto : 0;
                    Decimal totalBrutoPeriodo = totalBrutoVendasCredito + totaBrutoVendasDebito;

                    TotalVendasPorPeriodoConsolidado retorno = new TotalVendasPorPeriodoConsolidado {
                        TotalVendasCredito = totalBrutoVendasCredito,
                        TotalVendasDebito = totaBrutoVendasDebito,
                        TotalVendasPeriodo =  totalBrutoPeriodo };                    

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #region [Métodos Crédito]

        /// <summary>
        /// Consulta o total de vendas realizadas a crédito no período informado.<br/>        
        /// - Book BKWA2510	/ Programa WAC251 / TranID ISGM / Método ConsultarTotalVendasCreditoPorPeriodo
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2510 / Programa WAC251 / TranID ISGM / Método ConsultarTotalVendasCreditoPorPeriodo
        /// </remarks>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito do período.</returns>
        public TotalVendasCreditoPorPeriodo ConsultarTotalVendasCreditoPorPeriodo(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas crédito por período - BKWA2510 / WAC251 / ISGM"))
            {
                try
                {
                    //Consulta dados
                    TotalVendasCreditoPorPeriodo retorno =
                        Agentes.ValoresConsolidadosVendas.Instancia.ConsultarTotalVendasCreditoPorPeriodo(dataInicio, dataFim, pvs, out status);

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta o total de vendas realizadas á crédito no período informado e retorna separado por bandeira.<br/>        
        /// - Book BKWA2530	/ Programa WAC253 / TranID ISGO / Método ConsultarTotalVendasCreditoPorPeriodoBandeira
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2530 / Programa WAC253 / TranID ISGO / Método ConsultarTotalVendasCreditoPorPeriodoBandeira
        /// </remarks>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito do período por bandeira.</returns>
        public TotalVendasCreditoPorPeriodoBandeira ConsultarTotalVendasCreditoPorPeriodoBandeira(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas crédito por bandeira - BKWA2530 / WAC253 / ISGO"))
            {
                try
                {
                    //Consulta dados
                    TotalVendasCreditoPorPeriodoBandeira retorno =
                        Agentes.ValoresConsolidadosVendas.Instancia.ConsultarTotalVendasCreditoPorPeriodoBandeira(dataInicio, dataFim, pvs, out status);

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta as vendas realizadas á crédito, no período informado, separadas por dia e por PV (Ponto de Venda).<br/>        
        /// - Book BKWA2540	/ Programa WAC254 / TranID ISGP / Método ConsultarVendasCreditoPorDiaPv
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2540 / Programa WAC254 / TranID ISGP / Método ConsultarVendasCreditoPorDiaPv
        /// </remarks>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="indicadorRechamada">Flag indicando se ainda existem registros para serem retornados.</param>
        /// <param name="rechamada">Dicionário com a(s) chave(s) utilizada(s) para solicitar os próximos registos.</param>    
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito por dia e por PV.</returns>
        public List<VendasCreditoPorDiaPv> ConsultarVendasCreditoPorDiaPv(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out Boolean indicadorRechamada,
            ref Dictionary<String, Object> rechamada, 
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Vendas crédito por dia por PV - BKWA2540 / WAC254 / ISGP"))
            {
                try
                {
                    //Consulta dados
                    List<VendasCreditoPorDiaPv> retorno =
                        Agentes.ValoresConsolidadosVendas.Instancia.ConsultarVendasCreditoPorDiaPv(dataInicio, dataFim, pvs,out indicadorRechamada, ref rechamada, out status);

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta o total de vendas realizadas á crédito na data informada por bandeira.<br/>        
        /// - Book BKWA2570	/ Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2570 / Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira
        /// </remarks>
        /// <param name="dataVenda">Data da venda que se deseja consultar.</param>
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito do dia por bandeira.</returns>
        public TotalVendasCreditoPorDiaBandeira ConsultarTotalVendasCreditoPorDiaBandeira(
            DateTime dataVenda,
            Int32 numeroPv,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas crédito por dia e por bandeira - BKWA2570 / WAC257 / ISGS"))
            {
                try
                {
                    //Consulta dados
                    TotalVendasCreditoPorDiaBandeira retorno =
                        Agentes.ValoresConsolidadosVendas.Instancia.ConsultarTotalVendasCreditoPorDiaBandeira(dataVenda,numeroPv, out status);

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta o total de vendas realizadas á crédito na data informada por bandeira - Ver Todos<br/>        
        /// - Book BKWA2570	/ Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2570 / Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira
        /// </remarks>
        /// <param name="listaPvsDatasVendas">Lista de pares (Numero Ponto de Venda e Data Venda) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito do dia por bandeira.</returns>
        public TotalVendasCreditoPorDiaBandeira ConsultarTotalVendasCreditoPorDiaBandeiraTodos(
            List<Tuple<Int32, DateTime>> listaPvsDatasVendas,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas crédito por dia e por bandeira - BKWA2570 / WAC257 / ISGS - Ver Todos"))
            {
                //Variável auxiliar
                var statusAux = new StatusRetornoDTO(0, String.Empty, FONTE);

                //Prepara os parâmetros de chamada (por Bandeira, por dia)
                var parametrosChamada = new List<Tuple<Int32, DateTime>>();
                parametrosChamada.AddRange(listaPvsDatasVendas);              

                //Coleção thread-safe para armazenamento dos resultados e exceções das threads
                var dados = new ConcurrentQueue<TotalVendasCreditoPorDiaBandeira>();
                var excecoes = new ConcurrentQueue<Exception>();

                //Inicia a consulta multi-thread para os parâmetros de chamada preparados anteriormente
                Parallel.ForEach(parametrosChamada, new ParallelOptions { MaxDegreeOfParallelism = 30 }, parametroChamada =>
                {
                    try
                    {
                        //Variáveis auxiliares para a chamada atual
                        StatusRetornoDTO statusTotalizadorDto;
                        Int32 numeroPv = parametroChamada.Item1;
                        DateTime dataVenda = parametroChamada.Item2;

                        //Consulta dados
                        TotalVendasCreditoPorDiaBandeira dadosConsulta =
                            Agentes.ValoresConsolidadosVendas.Instancia.ConsultarTotalVendasCreditoPorDiaBandeira(dataVenda, numeroPv, out statusTotalizadorDto);

                        //Se consulta foi realizada com sucesso, adiciona resultado na lista thread-safe
                        if (statusTotalizadorDto.CodigoRetorno == 0)
                            dados.Enqueue(dadosConsulta);
                        //Se ocorreu um erro na consulta, armazena status retornado
                        else if (statusAux == null)
                            statusAux = statusTotalizadorDto;
                    }
                    catch (PortalRedecardException ex)
                    {
                        excecoes.Enqueue(ex);
                    }
                    catch (Exception ex)
                    {
                         excecoes.Enqueue(ex);
                    }

                });

                //Caso tenha ocorrido alguma exceção, lança exceções geradas
                if (excecoes.Count > 0) throw new PortalRedecardException(CODIGO_ERRO, FONTE, new AggregateException(excecoes));

                //Consolida os dados dos totalizadores por dia, em um totalizador único para o período
                TotalVendasCreditoPorDiaBandeira[] totalizadoresDia = dados.Where(result => result != null).ToArray();

                //Instanciação das variáveis de retorno
                var totalizador = new TotalVendasCreditoPorDiaBandeira();
                status = statusAux;

                //Totaliza o Valor Bruto Venda
                totalizador.TotalBruto = totalizadoresDia
                    .Sum(totalizadorDia => totalizadorDia.TotalBruto);

                //Totaliza o Valor Liquido
                totalizador.TotalLiquido = totalizadoresDia
                    .Sum(totalizadorDia => totalizadorDia.TotalLiquido);

                //Totaliza por Bandeira
                totalizador.ListaTotalVendasCreditoPorBandeira = totalizadoresDia
                    .SelectMany(totalizadorDia => totalizadorDia.ListaTotalVendasCreditoPorBandeira)
                    .GroupBy(totalizadorDia => new { codigoBandeira = totalizadorDia.CodigoBandeira, descricaoBandeira = totalizadorDia.DescricaoBandeira })
                    .Select(grupoTotalizador => new TotalVendasPorBandeira
                    {
                        CodigoBandeira  = grupoTotalizador.Key.codigoBandeira,
                        DescricaoBandeira = grupoTotalizador.Key.descricaoBandeira,
                        TotalBruto = grupoTotalizador.Sum(totalizadorBandeiraDia => totalizadorBandeiraDia.TotalBruto),
                        TotalLiquido = grupoTotalizador.Sum(totalizadorBandeiraDia => totalizadorBandeiraDia.TotalLiquido)
                    }).ToList();

                return totalizador;
            }
        }

        /// <summary>
        /// Consulta o resumo das vendas realizadas á crédito na data informada por bandeira.<br/>        
        /// - Book BKWA2580	/ Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2580 / Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// </remarks>
        /// <param name="dataVenda">Data da venda que se deseja consultar.</param>
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar.</param>
        /// <param name="indicadorRechamada">Flag indicando se ainda existem registros para serem retornados.</param>
        /// <param name="rechamada">Dicionário com a(s) chave(s) utilizada(s) para solicitar os próximos registos.</param>    
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Resumo das vendas crédito do dia.</returns>
        public List<ResumoVendasCreditoPorDia> ConsultarResumoVendasCreditoPorDia(
            DateTime dataVenda,
            Int32 numeroPv,
            out Boolean indicadorRechamada,
            ref Dictionary<String, Object> rechamada,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Resumo de vendas crédito por dia - BKWA2580 / WAC258 / ISGT"))
            {
                try
                {
                    //Consulta dados
                    List<ResumoVendasCreditoPorDia> retorno =
                        Agentes.ValoresConsolidadosVendas.Instancia.ConsultarResumoVendasCreditoPorDia(dataVenda, numeroPv, out indicadorRechamada, ref rechamada, out status);

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #endregion

        #region [Métodos Débito]

        /// <summary>
        /// Consulta o total de vendas realizadas á débito no período informado.<br/>        
        /// - Book BKWA2520	/ Programa WAC252 / TranID ISGN / Método ConsultarTotalVendasDebitoPorPeriodo
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2520 / Programa WAC252 / TranID ISGN / Método ConsultarTotalVendasDebitoPorPeriodo
        /// </remarks>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito do período.</returns>
        public TotalVendasDebitoPorPeriodo ConsultarTotalVendasDebitoPorPeriodo(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas débito por período - BKWA2520 / WAC252 / ISGN"))
            {
                try
                {
                    //Consulta dados
                    TotalVendasDebitoPorPeriodo retorno =
                        Agentes.ValoresConsolidadosVendas.Instancia.ConsultarTotalVendasDebitoPorPeriodo(dataInicio, dataFim, pvs, out status);

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta o total de vendas realizadas á débito no período informado e retorna separado por bandeira.<br/>        
        /// - Book BKWA2550	/ Programa WAC255 / TranID ISGQ / Método ConsultarTotalVendasDebitoPorPeriodoBandeira
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2550 / Programa WAC255 / TranID ISGQ / Método ConsultarTotalVendasDebitoPorPeriodoBandeira
        /// </remarks>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito do período por bandeira.</returns>
        public TotalVendasDebitoPorPeriodoBandeira ConsultarTotalVendasDebitoPorPeriodoBandeira(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas débito por bandeira - BKWA2550 / WAC255 / ISGQ"))
            {
                try
                {
                    //Consulta dados
                    TotalVendasDebitoPorPeriodoBandeira retorno =
                        Agentes.ValoresConsolidadosVendas.Instancia.ConsultarTotalVendasDebitoPorPeriodoBandeira(dataInicio, dataFim, pvs, out status);

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta as vendas realizadas á débito, no período informado, separadas por dia e por PV (Ponto de Venda).<br/>        
        /// - Book BKWA2560	/ Programa WAC256 / TranID ISGR / Método ConsultarVendasDebitoPorDiaPv
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2560 / Programa WAC256 / TranID ISGR / Método ConsultarVendasDebitoPorDiaPv
        /// </remarks>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="indicadorRechamada">Flag indicando se ainda existem registros para serem retornados.</param>
        /// <param name="rechamada">Dicionário com a(s) chave(s) utilizada(s) para solicitar os próximos registos.</param>    
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito por dia e por PV</returns>
        public List<VendasDebitoPorDiaPv> ConsultarVendasDebitoPorDiaPv(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out Boolean indicadorRechamada,
            ref Dictionary<String, Object> rechamada,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Vendas débito por dia por PV - BKWA2560 / WAC256 / ISGR"))
            {
                try
                {
                    //Consulta dados
                    List<VendasDebitoPorDiaPv> retorno =
                        Agentes.ValoresConsolidadosVendas.Instancia.ConsultarVendasDebitoPorDiaPv(dataInicio, dataFim, pvs, out indicadorRechamada, ref rechamada, out status);

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta o total de vendas realizadas á débito na data informada por bandeira.<br/>        
        /// - Book BKWA2590	/ Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2590 / Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira
        /// </remarks>
        /// <param name="dataVenda">Data da venda que se deseja consultar.</param>
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito do dia por bandeira.</returns>
        public TotalVendasDebitoPorDiaBandeira ConsultarTotalVendasDebitoPorDiaBandeira(
            DateTime dataVenda,
            Int32 numeroPv,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas débito por dia e por bandeira - BKWA2590 / WAC259 / ISGU"))
            {
                try
                {
                    //Consulta dados
                    TotalVendasDebitoPorDiaBandeira retorno =
                        Agentes.ValoresConsolidadosVendas.Instancia.ConsultarTotalVendasDebitoPorDiaBandeira(dataVenda, numeroPv, out status);

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta o total de vendas realizadas á débito na data informada por bandeira - Ver Todos.<br/>        
        /// - Book BKWA2590	/ Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2590 / Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira
        /// </remarks>
        /// <param name="listaPvsDatasVendas">Lista de pares (Numero Ponto de Venda e Data Venda) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito do dia por bandeira.</returns>
        public TotalVendasDebitoPorDiaBandeira ConsultarTotalVendasDebitoPorDiaBandeiraTodos(
            List<Tuple<Int32, DateTime>> listaPvsDatasVendas,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas débito por dia e por bandeira - BKWA2590 / WAC259 / ISGU - Ver Todos."))
            {
                //Variável auxiliar
                var statusAux = new StatusRetornoDTO(0, String.Empty, FONTE);

                //Prepara os parâmetros de chamada (por Bandeira, por dia)
                var parametrosChamada = new List<Tuple<Int32, DateTime>>();
                parametrosChamada.AddRange(listaPvsDatasVendas);

                //Coleção thread-safe para armazenamento dos resultados e exceções das threads
                var dados = new ConcurrentQueue<TotalVendasDebitoPorDiaBandeira>();
                var excecoes = new ConcurrentQueue<Exception>();

                //Inicia a consulta multi-thread para os parâmetros de chamada preparados anteriormente
                Parallel.ForEach(parametrosChamada, new ParallelOptions { MaxDegreeOfParallelism = 30 }, parametroChamada =>
                {
                    try
                    {
                        //Variáveis auxiliares para a chamada atual
                        StatusRetornoDTO statusTotalizadorDto;
                        Int32 numeroPv = parametroChamada.Item1;
                        DateTime dataVenda = parametroChamada.Item2;

                        //Consulta dados
                        TotalVendasDebitoPorDiaBandeira dadosConsulta =
                            Agentes.ValoresConsolidadosVendas.Instancia.ConsultarTotalVendasDebitoPorDiaBandeira(dataVenda, numeroPv, out statusTotalizadorDto);

                        //Se consulta foi realizada com sucesso, adiciona resultado na lista thread-safe
                        if (statusTotalizadorDto.CodigoRetorno == 0)
                            dados.Enqueue(dadosConsulta);
                        //Se ocorreu um erro na consulta, armazena status retornado
                        else if (statusAux == null)
                            statusAux = statusTotalizadorDto;
                    }
                    catch (PortalRedecardException ex)
                    {
                        excecoes.Enqueue(ex);
                    }
                    catch (Exception ex)
                    {
                        excecoes.Enqueue(ex);
                    }

                });

                //Caso tenha ocorrido alguma exceção, lança exceções geradas
                if (excecoes.Count > 0) throw new PortalRedecardException(CODIGO_ERRO, FONTE, new AggregateException(excecoes));

                //Consolida os dados dos totalizadores por dia, em um totalizador único para o período
                TotalVendasDebitoPorDiaBandeira[] totalizadoresDia = dados.Where(result => result != null).ToArray();

                //Instanciação das variáveis de retorno
                var totalizador = new TotalVendasDebitoPorDiaBandeira();
                status = statusAux;

                //Totaliza o Valor Bruto Venda
                totalizador.TotalBruto = totalizadoresDia
                    .Sum(totalizadorDia => totalizadorDia.TotalBruto);

                //Totaliza o Valor Liquido
                totalizador.TotalLiquido = totalizadoresDia
                    .Sum(totalizadorDia => totalizadorDia.TotalLiquido);

                //Totaliza por Bandeira
                totalizador.ListaTotalVendasDebitoPorBandeira = totalizadoresDia
                    .SelectMany(totalizadorDia => totalizadorDia.ListaTotalVendasDebitoPorBandeira)
                    .GroupBy(totalizadorDia => new { codigoBandeira = totalizadorDia.CodigoBandeira, descricaoBandeira = totalizadorDia.DescricaoBandeira })
                    .Select(grupoTotalizador => new TotalVendasPorBandeira
                    {
                        CodigoBandeira = grupoTotalizador.Key.codigoBandeira,
                        DescricaoBandeira = grupoTotalizador.Key.descricaoBandeira,
                        TotalBruto = grupoTotalizador.Sum(totalizadorBandeiraDia => totalizadorBandeiraDia.TotalBruto),
                        TotalLiquido = grupoTotalizador.Sum(totalizadorBandeiraDia => totalizadorBandeiraDia.TotalLiquido)
                    }).ToList();

                return totalizador;
            }
        }

        /// <summary>
        /// Consulta o resumo das vendas realizadas á débito na data informada por bandeira.<br/>        
        /// - Book BKWA2600	/ Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2600 / Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// </remarks>
        /// <param name="dataVenda">Data da venda que se deseja consultar.</param>
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar.</param>
        /// <param name="indicadorRechamada">Flag indicando se ainda existem registros para serem retornados.</param>
        /// <param name="rechamada">Dicionário com a(s) chave(s) utilizada(s) para solicitar os próximos registos.</param>    
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Resumo das vendas débito do dia.</returns>
        public List<ResumoVendasDebitoPorDia> ConsultarResumoVendasDebitoPorDia(
            DateTime dataVenda,
            Int32 numeroPv,
            out Boolean indicadorRechamada,
            ref Dictionary<String, Object> rechamada,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Resumo de vendas débito por dia - BKWA2600 / WAC260 / ISGV"))
            {
                try
                {
                    //Consulta dados
                    List<ResumoVendasDebitoPorDia> retorno =
                        Agentes.ValoresConsolidadosVendas.Instancia.ConsultarResumoVendasDebitoPorDia(dataVenda, numeroPv, out indicadorRechamada, ref rechamada, out status);

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        
        #endregion
    }
}
