using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.ValoresPagos;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using BLL = Redecard.PN.Extrato.Negocio.ValoresPagosBLL;
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
    public class HISServicoWA_Extrato_ValoresPagos : ServicoBaseExtrato, IHISServicoWA_Extrato_ValoresPagos
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
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        public void ConsultarRelatorioCredito(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out CreditoTotalizador totalizador,
            out List<Credito> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno _statusTotalizador = null, _statusRelatorio = null;            
            Int32 _quantidadeTotalRegistros = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<CreditoTotalizador> _funcaoTotalizador = () => {
                return ConsultarCreditoTotalizadores(
                    guidPesquisaTotalizador,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<Credito>> _funcaoRegistros = () => {
                return ConsultarCredito(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref _quantidadeTotalRegistros,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out _statusRelatorio); };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var _retorno = base.ConsultaMultiThread(_funcaoTotalizador, _funcaoRegistros);

            //Atribuição de dados de retorno
            registros = _retorno.Item2;
            totalizador = _retorno.Item1;
            statusRelatorio = _statusRelatorio;
            statusTotalizador = _statusTotalizador;
            quantidadeTotalRegistros = _quantidadeTotalRegistros;
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
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataRecebimento">Data de recebimento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        public void ConsultarRelatorioCreditoDetalhe(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            Int32 codigoBandeira,
            DateTime dataRecebimento,
            Int32 pv,
            Int32 numeroOcu,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out CreditoDetalheTotalizador totalizador,
            out List<CreditoDetalhe> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno _statusTotalizador = null, _statusRelatorio = null;            
            Int32 _quantidadeTotalRegistros = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<CreditoDetalheTotalizador> _funcaoTotalizador = () => {
                return ConsultarCreditoDetalheTotalizadores(
                    guidPesquisaTotalizador,
                    codigoBandeira,
                    dataRecebimento,
                    pv,
                    numeroOcu,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<CreditoDetalhe>> _funcaoRegistros = () => {
                return ConsultarCreditoDetalhe(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref _quantidadeTotalRegistros,
                    codigoBandeira,
                    dataRecebimento,
                    pv,
                    numeroOcu,
                    out _statusRelatorio); };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var _retorno = base.ConsultaMultiThread(_funcaoTotalizador, _funcaoRegistros);

            //Atribuição de dados de retorno
            registros = _retorno.Item2;
            totalizador = _retorno.Item1;
            statusRelatorio = _statusRelatorio;
            statusTotalizador = _statusTotalizador;
            quantidadeTotalRegistros = _quantidadeTotalRegistros;
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
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        public void ConsultarRelatorioDebito(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out DebitoTotalizador totalizador,
            out List<Debito> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno _statusTotalizador = null, _statusRelatorio = null;            
            Int32 _quantidadeTotalRegistros = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<DebitoTotalizador> _funcaoTotalizador = () => {
                return ConsultarDebitoTotalizadores(
                    guidPesquisaTotalizador,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<Debito>> _funcaoRegistros = () => {
                return ConsultarDebito(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref _quantidadeTotalRegistros,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out _statusRelatorio); };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var _retorno = base.ConsultaMultiThread(_funcaoTotalizador, _funcaoRegistros);

            //Atribuição de dados de retorno
            registros = _retorno.Item2;
            totalizador = _retorno.Item1;
            statusRelatorio = _statusRelatorio;
            statusTotalizador = _statusTotalizador;
            quantidadeTotalRegistros = _quantidadeTotalRegistros;
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
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="dataPagamento">Data de pagamento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        public void ConsultarRelatorioDebitoDetalhe(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            DateTime dataPagamento,
            Int32 pv,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out DebitoDetalheTotalizador totalizador,
            out List<DebitoDetalhe> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno _statusTotalizador = null, _statusRelatorio = null;            
            Int32 _quantidadeTotalRegistros = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<DebitoDetalheTotalizador> _funcaoTotalizador = () => {
                return ConsultarDebitoDetalheTotalizadores(
                    guidPesquisaTotalizador,
                    dataPagamento,
                    pv,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<DebitoDetalhe>> _funcaoRegistros = () => {
                return ConsultarDebitoDetalhe(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref _quantidadeTotalRegistros,
                    dataPagamento,
                    pv,
                    out _statusRelatorio); };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var _retorno = base.ConsultaMultiThread(_funcaoTotalizador, _funcaoRegistros);

            //Atribuição de dados de retorno
            registros = _retorno.Item2;
            totalizador = _retorno.Item1;
            statusRelatorio = _statusRelatorio;
            statusTotalizador = _statusTotalizador;
            quantidadeTotalRegistros = _quantidadeTotalRegistros;
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
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Crédito</returns>
        public CreditoTotalizador ConsultarCreditoTotalizadores(
            Guid guidPesquisa,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Crédito - Totalizadores - WACA1316"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigoBandeira, dataInicial, dataFinal, pvs });

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.CreditoTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarCreditoTotalizadores(
                            codigoBandeira,
                            pvs,
                            dataInicial,
                            dataFinal,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Conversão de DTO para modelo de serviço
                    var dados = CreditoTotalizador.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Valores Pagos - Crédito.</returns>
        public List<Credito> ConsultarCredito(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Crédito - WACA1317"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs
                });

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
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    Guid guid = Utils.GerarGuid(idCache.ToString());

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Credito>(Cache.Extrato,
                        guid, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarCredito(
                            codigoBandeira,
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            quantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guid, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Credito>(Cache.Extrato,
                        guid, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    var dados = Credito.FromDTO(dadosCache);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataRecebimento">Data de recebimento</param>        
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Crédito - Detalhe</returns>
        public CreditoDetalheTotalizador ConsultarCreditoDetalheTotalizadores(
            Guid guidPesquisa,
            Int32 codigoBandeira,
            DateTime dataRecebimento,
            Int32 pv,
            Int32 numeroOcu,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Crédito - Detalhe - Totalizadores - WACA1318"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigoBandeira, dataRecebimento, pv, numeroOcu });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataRecebimento.ToString("yyyy-MM-dd"));
                    idCache.Append(pv);
                    idCache.Append(numeroOcu);

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.CreditoDetalheTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarCreditoDetalheTotalizadores(
                            codigoBandeira,
                            pv,
                            dataRecebimento,
                            numeroOcu,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Conversão de DTO para modelo de serviço
                    var dados = CreditoDetalheTotalizador.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataRecebimento">Data de recebimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Valores Pagos - Crédito - Detalhe.</returns>
        public List<CreditoDetalhe> ConsultarCreditoDetalhe(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            DateTime dataRecebimento,
            Int32 pv,
            Int32 numeroOcu,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Crédito - Detalhe - WACA1319"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    codigoBandeira,
                    dataRecebimento,
                    pv,
                    numeroOcu
                });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataRecebimento.ToString("yyyy-MM-dd"));
                    idCache.Append(pv);
                    idCache.Append(numeroOcu);

                    Guid guid = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.CreditoDetalhe>(Cache.Extrato,
                        guid, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarCreditoDetalhe(
                            codigoBandeira,
                            pv,
                            dataRecebimento,
                            numeroOcu,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            quantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guid, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.CreditoDetalhe>(Cache.Extrato,
                        guid, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    var dados = CreditoDetalhe.FromDTO(dadosCache);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Débito</returns>
        public DebitoTotalizador ConsultarDebitoTotalizadores(
            Guid guidPesquisa,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Débito - Totalizadores - WACA1320"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigoBandeira, dataInicial, dataFinal, pvs });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
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
                            dataInicial,
                            dataFinal,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Conversão de DTO para modelo de serviço
                    var dados = DebitoTotalizador.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Valores Pagos - Débito.</returns>
        public List<Debito> ConsultarDebito(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Débito - WACA1321"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs
                });

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
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    Guid guid = Utils.GerarGuid(idCache.ToString());

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Debito>(Cache.Extrato,
                        guid, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDebito(
                            codigoBandeira,
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            quantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guid, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Debito>(Cache.Extrato,
                        guid, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    var dados = Debito.FromDTO(dadosCache);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataPagamento">Data de pagamento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Débito - Detalhe</returns>
        public DebitoDetalheTotalizador ConsultarDebitoDetalheTotalizadores(
            Guid guidPesquisa,
            DateTime dataPagamento,
            Int32 pv,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Débito - Detalhe - Totalizadores - WACA1322"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, dataPagamento, pv });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataPagamento.ToString("yyyy-MM-dd"));
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
                            dataPagamento,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Conversão de DTO para modelo de serviço
                    var dados = DebitoDetalheTotalizador.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataPagamento">Data de pagamento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Valores Pagos - Débito - Detalhe.</returns>
        public List<DebitoDetalhe> ConsultarDebitoDetalhe(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataPagamento,
            Int32 pv,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Valores Pagos - Débito - Detalhe - WACA1323"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    dataPagamento,
                    pv
                });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVALORESPAGOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataPagamento.ToString("yyyy-MM-dd"));
                    idCache.Append(pv);

                    Guid guid = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.DebitoDetalhe>(Cache.Extrato,
                        guid, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDebitoDetalhe(
                            pv,
                            dataPagamento,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            quantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guid, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.DebitoDetalhe>(Cache.Extrato,
                        guid, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    var dados = DebitoDetalhe.FromDTO(dadosCache);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #endregion
    }
}
