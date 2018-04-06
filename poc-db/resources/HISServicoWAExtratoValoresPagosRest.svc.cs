/*
© Copyright 2017 Rede S.A.
Autor	: Francisco Mazali
Empresa	: Iteris Consultoria e Software LTDA
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.ValoresPagos;
using System;
using System.Collections.Generic;
using System.Text;
using BLL = Redecard.PN.Extrato.Negocio.ValoresPagosBLL;
using ContratoValoresPagos = Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response;
using DTO = Redecard.PN.Extrato.Modelo.ValoresPagos;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Valores Pagos.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1316 / Programa WA1316 / TranID ISHG / Método ConsultarCreditoTotalizadores<br/>
    /// - Book WACA1317 / Programa WA1317 / TranID ISHH / Método ConsultarCredito<br/>
    /// - Book WACA1318 / Programa WA1318 / TranID ISHI / Método ConsultarCreditoDetalheTotalizadores<br/>
    /// - Book WACA1319 / Programa WA1319 / TranID ISHJ / Método ConsultarCreditoDetalhe<br/>
    /// - Book WACA1320 / Programa WA1320 / TranID ISHK / Método ConsultarDebitoTotalizadores<br/>
    /// - Book WACA1321 / Programa WA1321 / TranID ISHL / Método ConsultarDebito<br/>
    /// - Book WACA1322 / Programa WA1322 / TranID ISHM / Método ConsultarDebitoDetalheTotalizadores<br/>
    /// - Book WACA1323 / Programa WA1323 / TranID ISHN / Método ConsultarDebitoDetalhe
    /// </remarks>
    public class HISServicoWAExtratoValoresPagosRest : ServicoBaseExtrato, IHISServicoWAExtratoValoresPagosRest
    {
        #region [ RELATÓRIOS - Consultas Multi-Thread (Totalizadores e Registros) ]

        /// <summary>
        /// Consulta de Relatório de Valores Pagos - Crédito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1316 / Programa WA1316 / TranID ISHG<br/>
        /// - Book WACA1317 / Programa WA1317 / TranID ISHH
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1316 / Programa WA1316 / TranID ISHG<br/>
        /// - Book WACA1317 / Programa WA1317 / TranID ISHH
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        public RelatorioValoresPagosCreditoResponse ConsultarRelatorioCredito(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros)
        {
            RelatorioValoresPagosCreditoResponse retorno = new RelatorioValoresPagosCreditoResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoValoresPagos.ResponseBaseItem<CreditoTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarCreditoTotalizadores(
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoValoresPagos.ResponseBaseList<Credito>> funcaoRegistros = () =>
            {
                return ConsultarCredito(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var retornoFuncoes = base.ConsultaMultiThreadRest<ContratoValoresPagos.ResponseBaseItem<CreditoTotalizador>, ContratoValoresPagos.ResponseBaseList<ValoresPagos.Credito>>(funcaoTotalizador, funcaoRegistros);

            //atribuição de dados de retorno
            retorno.Registros = retornoFuncoes.Item2.Itens;
            retorno.Totalizador = retornoFuncoes.Item1.Item;
            retorno.StatusRelatorio = retornoFuncoes.Item2.StatusRetorno;
            retorno.StatusTotalizador = retornoFuncoes.Item1.StatusRetorno;
            retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

            return retorno;
        }

        /// <summary>
        /// Consulta de Relatório de Valores Pagos - Crédito - Detalhe<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1318 / Programa WA1318 / TranID ISHI<br/>
        /// - Book WA1319 / Programa WA1319 / TranID ISHJ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1318 / Programa WA1318 / TranID ISHI<br/>
        /// - Book WA1319 / Programa WA1319 / TranID ISHJ
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataRecebimento">Data de recebimento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        public RelatorioValoresPagosCreditoDetalheResponse ConsultarRelatorioCreditoDetalhe(
            Int32 codigoBandeira,
            String dataRecebimento,
            Int32 pv,
            Int32 numeroOcu,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros)
        {

            RelatorioValoresPagosCreditoDetalheResponse retorno = new RelatorioValoresPagosCreditoDetalheResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoValoresPagos.ResponseBaseItem<CreditoDetalheTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarCreditoDetalheTotalizadores(
                    codigoBandeira,
                    dataRecebimento,
                    pv,
                    numeroOcu);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoValoresPagos.ResponseBaseList<CreditoDetalhe>> funcaoRegistros = () =>
            {
                return ConsultarCreditoDetalhe(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    codigoBandeira,
                    dataRecebimento,
                    pv,
                    numeroOcu);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var _retorno = base.ConsultaMultiThreadRest(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            retorno.Registros = _retorno.Item2.Itens;
            retorno.Totalizador = _retorno.Item1.Item;
            retorno.StatusRelatorio = _retorno.Item2.StatusRetorno;
            retorno.StatusTotalizador = _retorno.Item1.StatusRetorno;
            retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

            return retorno;
        }

        /// <summary>
        /// Consulta de Relatório de Valores Pagos - Débito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1320 / Programa WA1320 / TranID ISHK<br/>
        /// - Book WACA1321 / Programa WA1321 / TranID ISHL
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1320 / Programa WA1320 / TranID ISHK<br/>
        /// - Book WACA1321 / Programa WA1321 / TranID ISHL
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        public RelatorioValoresPagosDebitoResponse ConsultarRelatorioDebito(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros)
        {
            RelatorioValoresPagosDebitoResponse retorno = new RelatorioValoresPagosDebitoResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoValoresPagos.ResponseBaseItem<DebitoTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarDebitoTotalizadores(
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoValoresPagos.ResponseBaseList<Debito>> funcaoRegistros = () =>
            {
                return ConsultarDebito(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var retornoFuncoes = base.ConsultaMultiThreadRest(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            retorno.Registros = retornoFuncoes.Item2.Itens;
            retorno.Totalizador = retornoFuncoes.Item1.Item;
            retorno.StatusRelatorio = retornoFuncoes.Item2.StatusRetorno;
            retorno.StatusTotalizador = retornoFuncoes.Item1.StatusRetorno;
            retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

            return retorno;
        }

        /// <summary>
        /// Consulta de Relatório de Valores Pagos - Débito - Detalhe<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1322 / Programa WA1322 / TranID ISHM<br/>
        /// - Book WA1323 / Programa WA1323 / TranID ISHN
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1322 / Programa WA1322 / TranID ISHM<br/>
        /// - Book WA1323 / Programa WA1323 / TranID ISHN
        /// </remarks>
        /// <param name="dataPagamento">Data de pagamento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        public RelatorioValoresPagosDebitoDetalheResponse ConsultarRelatorioDebitoDetalhe(
            String dataPagamento,
            Int32 pv,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros)
        {

            RelatorioValoresPagosDebitoDetalheResponse retorno = new RelatorioValoresPagosDebitoDetalheResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoValoresPagos.ResponseBaseItem<DebitoDetalheTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarDebitoDetalheTotalizadores(
                    dataPagamento,
                    pv);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoValoresPagos.ResponseBaseList<DebitoDetalhe>> funcaoRegistros = () =>
            {
                return ConsultarDebitoDetalhe(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    dataPagamento,
                    pv);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var retornoFuncoes = base.ConsultaMultiThreadRest(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            retorno.Registros = retornoFuncoes.Item2.Itens;
            retorno.Totalizador = retornoFuncoes.Item1.Item;
            retorno.StatusRelatorio = retornoFuncoes.Item2.StatusRetorno;
            retorno.StatusTotalizador = retornoFuncoes.Item1.StatusRetorno;
            retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

            return retorno;
        }

        #endregion [ RELATÓRIOS - Consultas Multi-Thread (Totalizadores e Registros) ]

        #region [ Valores Pagos - Crédito - Totalizadores - WACA1316 / WA1316 / ISHG ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Valores Pagos - Crédito.<br/>
        /// - Book WACA1316 / Programa WA1316 / TranID ISHG
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1316 / Programa WA1316 / TranID ISHG
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Crédito</returns>
        public ContratoValoresPagos.ResponseBaseItem<CreditoTotalizador> ConsultarCreditoTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Crédito - Totalizadores - WACA1316"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, dataInicial, dataFinal, pvs });
                ContratoValoresPagos.ResponseBaseItem<CreditoTotalizador> retorno = new ContratoValoresPagos.ResponseBaseItem<CreditoTotalizador>();
                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    StatusRetornoDTO statusDTO;

                    var dadosConsulta = CacheAdmin.Recuperar<DTO.CreditoTotalizador>(Cache.Extrato, idCache.ToString());

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarCreditoTotalizadores(
                            codigoBandeira,
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idCache.ToString(), dadosConsulta, new TimeSpan(12, 0, 0));
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.OK;

                    //Conversão de DTO para modelo de serviço
                    retorno.Item = CreditoTotalizador.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status = new StatusRetorno(0, String.Empty, FONTE), retorno });


                }
                catch (PortalRedecardException ex)
                {
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Valores Pagos - Crédito - Registros - WACA1317 / WA1317 / ISHH ]

        /// <summary>
        /// Consulta de registros do Relatório de Valores Pagos - Crédito.<br/>
        /// - Book WACA1317 / Programa WA1317 / TranID ISHH
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1317 / Programa WA1317 / TranID ISHH
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Valores Pagos - Crédito.</returns>
        public ContratoValoresPagos.ResponseBaseList<Credito> ConsultarCredito(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Crédito - WACA1317"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                ContratoValoresPagos.ResponseBaseList<Credito> retorno = new ContratoValoresPagos.ResponseBaseList<Credito>();

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Credito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarCredito(
                            codigoBandeira,
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            retorno.QuantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Credito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.OK;

                    //Converte DTO para modelo de serviço
                    retorno.Itens = Credito.FromDTO(dadosCache).ToArray();

                    retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Valores Pagos - Crédito - Detalhe - Totalizadores - WACA1318 / WA1318 / ISHI ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Valores Pagos - Crédito - Detalhe.<br/>
        /// - Book WACA1318 / Programa WA1318 / TranID ISHI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1318 / Programa WA1318 / TranID ISHI
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataRecebimento">Data de recebimento</param>        
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Crédito - Detalhe</returns>
        public ContratoValoresPagos.ResponseBaseItem<CreditoDetalheTotalizador> ConsultarCreditoDetalheTotalizadores(
            Int32 codigoBandeira,
            String dataRecebimento,
            Int32 pv,
            Int32 numeroOcu)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Crédito - Detalhe - Totalizadores - WACA1318"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, dataRecebimento, pv, numeroOcu });
                ContratoValoresPagos.ResponseBaseItem<CreditoDetalheTotalizador> retorno = new ContratoValoresPagos.ResponseBaseItem<CreditoDetalheTotalizador>();
                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataRecebimento);
                    idCache.Append(pv);
                    idCache.Append(numeroOcu);

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.CreditoDetalheTotalizador>(Cache.Extrato, idCache.ToString());

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarCreditoDetalheTotalizadores(
                            codigoBandeira,
                            pv,
                            Utils.ParseDate(dataRecebimento),
                            numeroOcu,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idCache.ToString(), dadosConsulta);
                    }

                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.OK;
                    //Conversão de DTO para modelo de serviço
                    retorno.Item = CreditoDetalheTotalizador.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;

                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = Contrato.ContratoDados.Response.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Valores Pagos - Crédito - Detalhe - Registros - WACA1319 / WA1319 / ISHJ ]

        /// <summary>
        /// Consulta de registros do Relatório de Valores Pagos - Crédito - Detalhe.<br/>
        /// - Book WACA1319 / Programa WA1319 / TranID ISHJ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1319 / Programa WA1319 / TranID ISHJ
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataRecebimento">Data de recebimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <returns>Registros do Relatório de Valores Pagos - Crédito - Detalhe.</returns>
        public ContratoValoresPagos.ResponseBaseList<CreditoDetalhe> ConsultarCreditoDetalhe(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            String dataRecebimento,
            Int32 pv,
            Int32 numeroOcu)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Crédito - Detalhe - WACA1319"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    codigoBandeira,
                    dataRecebimento,
                    pv,
                    numeroOcu
                });

                ContratoValoresPagos.ResponseBaseList<CreditoDetalhe> retorno = new ContratoValoresPagos.ResponseBaseList<CreditoDetalhe>();

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataRecebimento);
                    idCache.Append(pv);
                    idCache.Append(numeroOcu);

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.CreditoDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarCreditoDetalhe(
                            codigoBandeira,
                            pv,
                            Utils.ParseDate(dataRecebimento),
                            numeroOcu,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            retorno.QuantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.CreditoDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.OK;

                    //Converte DTO para modelo de serviço
                    retorno.Itens = CreditoDetalhe.FromDTO(dadosCache).ToArray();
                    retorno.QuantidadeTotalRegistros = quantidadeRegistros;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Valores Pagos - Débito - Totalizadores - WACA1320 / WA1320 / ISHK ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Valores Pagos - Débito.<br/>
        /// - Book WACA1320 / Programa WA1320 / TranID ISHK
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1320 / Programa WA1320 / TranID ISHK
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Débito</returns>
        public ContratoValoresPagos.ResponseBaseItem<DebitoTotalizador> ConsultarDebitoTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Débito - Totalizadores - WACA1320"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, dataInicial, dataFinal, pvs });
                ContratoValoresPagos.ResponseBaseItem<DebitoTotalizador> retorno = new ContratoValoresPagos.ResponseBaseItem<DebitoTotalizador>();
                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.DebitoTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarDebitoTotalizadores(
                            codigoBandeira,
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Conversão de DTO para modelo de serviço
                    retorno.Item = DebitoTotalizador.FromDTO(dadosConsulta);

                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Valores Pagos - Débito - Registros - WACA1321 / WA1321 / ISHL ]

        /// <summary>
        /// Consulta de registros do Relatório de Valores Pagos - Débito.<br/>
        /// - Book WACA1321 / Programa WA1321 / TranID ISHL
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1321 / Programa WA1321 / TranID ISHL
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Valores Pagos - Débito.</returns>
        public ContratoValoresPagos.ResponseBaseList<Debito> ConsultarDebito(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Débito - WACA1321"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                ContratoValoresPagos.ResponseBaseList<Debito> retorno = new ContratoValoresPagos.ResponseBaseList<Debito>();

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Debito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDebito(
                            codigoBandeira,
                            pvs,
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.QuantidadeTotalRegistros = 0;
                            retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Debito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.OK;

                    //Converte DTO para modelo de serviço
                    retorno.Itens = Debito.FromDTO(dadosCache).ToArray();

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Valores Pagos - Débito - Detalhe - Totalizadores - WACA1322 / WA1322 / ISHM ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Valores Pagos - Débito - Detalhe.<br/>
        /// - Book WACA1322 / Programa WA1322 / TranID ISHM
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1322 / Programa WA1322 / TranID ISHM
        /// </remarks>
        /// <param name="dataPagamento">Data de pagamento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Débito - Detalhe</returns>
        public ContratoValoresPagos.ResponseBaseItem<DebitoDetalheTotalizador> ConsultarDebitoDetalheTotalizadores(
            String dataPagamento,
            Int32 pv)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Débito - Detalhe - Totalizadores - WACA1322"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { dataPagamento, pv });

                ContratoValoresPagos.ResponseBaseItem<DebitoDetalheTotalizador> retorno = new ContratoValoresPagos.ResponseBaseItem<DebitoDetalheTotalizador>();

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataPagamento);
                    idCache.Append(pv);

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.DebitoDetalheTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarDebitoDetalheTotalizadores(
                            pv,
                            Utils.ParseDate(dataPagamento),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Conversão de DTO para modelo de serviço
                    retorno.Item = DebitoDetalheTotalizador.FromDTO(dadosConsulta);

                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion

        #region [ Valores Pagos - Débito - Detalhe - Registros - WACA1323 / WA1323 / ISHN ]

        /// <summary>
        /// Consulta de registros do Relatório de Valores Pagos - Débito - Detalhe.<br/>
        /// - Book WACA1323 / Programa WA1323 / TranID ISHN
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1323 / Programa WA1323 / TranID ISHN
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="dataPagamento">Data de pagamento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <returns>Registros doa Relatório de Valores Pagos - Débito - Detalhe.</returns>
        public ContratoValoresPagos.ResponseBaseList<DebitoDetalhe> ConsultarDebitoDetalhe(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            String dataPagamento,
            Int32 pv)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Débito - Detalhe - WACA1323"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    dataPagamento,
                    pv
                });

                ContratoValoresPagos.ResponseBaseList<DebitoDetalhe> retorno = new ContratoValoresPagos.ResponseBaseList<DebitoDetalhe>();

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataPagamento);
                    idCache.Append(pv);

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.DebitoDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDebitoDetalhe(
                            pv,
                            Utils.ParseDate(dataPagamento),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.QuantidadeTotalRegistros = 0;
                            retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.DebitoDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Converte DTO para modelo de serviço
                    retorno.Itens = DebitoDetalhe.FromDTO(dadosCache).ToArray();
                    retorno.QuantidadeTotalRegistros = quantidadeRegistros;
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoValoresPagos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion
    }
}
