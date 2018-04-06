using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using AutoMapper;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Comum;
using DTO = Redecard.PN.Extrato.Modelo.LancamentosFuturos;
using BLL = Redecard.PN.Extrato.Negocio.LancamentosFuturosBLL;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.LancamentosFuturos;
using System.Threading.Tasks;
using Redecard.PN.Extrato.Modelo.Comum;
using System.Collections.Concurrent;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Lançamentos Futuros.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1324 / Programa WA1324 / TranID ISHO / Método ConsultarCreditoTotalizadores<br/>
    /// - Book WACA1325 / Programa WA1325 / TranID ISHP / Método ConsultarCredito<br/>
    /// - Book WACA1326 / Programa WA1326 / TranID ISHQ / Método ConsultarCreditoDetalheTotalizadores<br/>
    /// - Book WACA1327 / Programa WA1327 / TranID ISHR / Método ConsultarCreditoDetalhe<br/>
    /// - Book WACA1328 / Programa WA1328 / TranID ISHS / Método ConsultarDebitoTotalizadores<br/>
    /// - Book WACA1329 / Programa WA1329 / TranID ISHT / Método ConsultarDebito
    /// </remarks>
    public class HISServicoWA_Extrato_LancamentosFuturos : ServicoBaseExtrato, IHISServicoWA_Extrato_LancamentosFuturos
    {
        #region [ RELATÓRIOS - Consultas Multi-Thread (Totalizadores e Registros) ]

        /// <summary>
        /// Consulta de Relatório de Lançamentos Futuros - Crédito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1324 / Programa WA1324 / TranID ISHO<br/>
        /// - Book WACA1325 / Programa WA1325 / TranID ISHP
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1324 / Programa WA1324 / TranID ISHO<br/>
        /// - Book WACA1325 / Programa WA1325 / TranID ISHP
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
        /// Consulta de Relatório de Lançamentos Futuros - Crédito - Detalhe<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1326 / Programa WA1326 / TranID ISHQ<br/>
        /// - Book WA1327 / Programa WA1327 / TranID ISHR
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1326 / Programa WA1326 / TranID ISHQ<br/>
        /// - Book WA1327 / Programa WA1327 / TranID ISHR
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataVencimento">Data de vencimento</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        public void ConsultarRelatorioCreditoDetalhe(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            Int32 codigoBandeira,
            DateTime dataVencimento,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,
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
                    dataVencimento,
                    pvs,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<CreditoDetalhe>> _funcaoRegistros = () => {
                return ConsultarCreditoDetalhe(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref _quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataVencimento,
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
        /// Consulta de Relatório de Lançamentos Futuros - Crédito - Detalhe - Ver Todos<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1326 / Programa WA1326 / TranID ISHQ<br/>
        /// - Book WA1327 / Programa WA1327 / TranID ISHR
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1326 / Programa WA1326 / TranID ISHQ<br/>
        /// - Book WA1327 / Programa WA1327 / TranID ISHR
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="codigosBandeiras">Códigos das bandeiras</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>        
        public void ConsultarRelatorioCreditoDetalheTodos(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            List<Int32> codigosBandeiras,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,
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
                return ConsultarCreditoDetalheTodosTotalizadores(
                    guidPesquisaTotalizador,
                    codigosBandeiras,
                    pvs,
                    dataInicial,
                    dataFinal,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<CreditoDetalhe>> _funcaoRegistros = () => {
                return ConsultarCreditoDetalheTodos(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref _quantidadeTotalRegistros,
                    tipoRegistro,
                    codigosBandeiras,
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
        /// Consulta de Relatório de Lançamentos Futuros - Débito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1328 / Programa WA1328 / TranID ISHS<br/>
        /// - Book WACA1329 / Programa WA1329 / TranID ISHT
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1328 / Programa WA1328 / TranID ISHS<br/>
        /// - Book WACA1329 / Programa WA1329 / TranID ISHT
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo de registro a ser retornado</param>
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
            DebitoTipoRegistro tipoRegistro,
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
                    tipoRegistro,
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

        #endregion

        #region [ Lançamentos Futuros - Crédito - Totalizadores - WACA1324 / WA1324 / ISHO ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Lançamentos Futuros - Crédito.<br/>
        /// - Book WACA1324 / Programa WA1324 / TranID ISHO
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1324 / Programa WA1324 / TranID ISHO
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Crédito</returns>
        public CreditoTotalizador ConsultarCreditoTotalizadores(
            Guid guidPesquisa,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Crédito - Totalizadores - WACA1324"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigoBandeira, dataInicial, dataFinal, pvs });

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHELANCAMENTOSFUTUROS);
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

        #region [ Lançamentos Futuros - Crédito - Registros - WACA1325 / WA1325 / ISHP ]

        /// <summary>
        /// Consulta de registros do Relatório de Lançamentos Futuros - Crédito.<br/>
        /// - Book WACA1325 / Programa WA1325 / TranID ISHP
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1325 / Programa WA1325 / TranID ISHP
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
        /// <returns>Registros do Relatório de Lançamentos Futuros - Crédito.</returns>
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
            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Crédito - WACA1325"))
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
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHELANCAMENTOSFUTUROS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    Guid guid = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

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

                    Log.GravarLog(EventoLog.FimServico, new { status, dados, quantidadeTotalRegistros });

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

        #region [ Lançamentos Futuros - Crédito - Detalhe - Totalizadores - WACA1326 / WA1326 / ISHQ ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe.<br/>
        /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataVencimento">Data de vencimento</param>        
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe</returns>
        public CreditoDetalheTotalizador ConsultarCreditoDetalheTotalizadores(
            Guid guidPesquisa,
            Int32 codigoBandeira,
            DateTime dataVencimento,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Crédito - Detalhe - Totalizadores - WACA1326"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigoBandeira, dataVencimento, pvs });

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHELANCAMENTOSFUTUROS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataVencimento.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));
                    
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = Utils.GerarGuid(idCache.ToString()).ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.CreditoDetalheTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarCreditoDetalheTotalizadores(
                            codigoBandeira,
                            pvs,
                            dataVencimento,
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

        /// <summary>
        /// Consulta de totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe - Ver Todos.<br/>
        /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigosBandeiras">Código da bandeira</param>        
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe - Ver Todos</returns>
        public CreditoDetalheTotalizador ConsultarCreditoDetalheTodosTotalizadores(
            Guid guidPesquisa,
            List<Int32> codigosBandeiras,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Crédito - Detalhe Todos - Totalizadores - WACA1326"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigoBandeira = codigosBandeiras, dataInicial, dataFinal, pvs });

                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = guidPesquisa.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.CreditoDetalheTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarCreditoDetalheTodosTotalizadores(
                            codigosBandeiras,
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

        #region [ Lançamentos Futuros - Crédito - Detalhe - Registros - WACA1327 / WA1327 / ISHR ]

        /// <summary>
        /// Consulta de registros do Relatório de Lançamentos Futuros - Crédito - Detalhe.<br/>
        /// - Book WACA1327 / Programa WA1327 / TranID ISHR
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1327 / Programa WA1327 / TranID ISHR
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataVencimento">Data de vencimento</param>        
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Lançamentos Futuros - Crédito - Detalhe.</returns>
        public List<CreditoDetalhe> ConsultarCreditoDetalhe(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            DateTime dataVencimento,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Crédito - Detalhe - WACA1327"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataVencimento,
                    pvs
                });

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHELANCAMENTOSFUTUROS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(tipoRegistro);
                    idCache.Append(dataVencimento.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

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
                            dataVencimento,
                            codigoBandeira,
                            pvs,
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

                        //Filtra de acordo com o tipo de registro
                        if (tipoRegistro == CreditoDetalheTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheDT).ToList();
                        else if (tipoRegistro == CreditoDetalheTipoRegistro.AjusteComValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheA1).ToList();
                        else if (tipoRegistro == CreditoDetalheTipoRegistro.AjusteSemValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheA2).ToList();

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

                    Log.GravarLog(EventoLog.FimServico, new { status, dados, quantidadeTotalRegistros });

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

        /// <summary>
        /// Consulta de registros do Relatório de Lançamentos Futuros - Crédito - Detalhe - Ver Todos.<br/>
        /// - Book WACA1327 / Programa WA1327 / TranID ISHR
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1327 / Programa WA1327 / TranID ISHR
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>    
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigosBandeiras">Códigos das bandeiras</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Lançamentos Futuros - Crédito - Detalhe - Ver Todos.</returns>
        public List<CreditoDetalhe> ConsultarCreditoDetalheTodos(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,
            List<Int32> codigosBandeiras,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Crédito - Detalhe Todos - WACA1327"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigosBandeiras,
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

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.CreditoDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Data da pesquisa é por padrão a Data Inicial do período solicitado
                        DateTime dataVencimento = rechamada.GetValueOrDefault<DateTime>("DataVencimento", dataInicial);
                        Int32 _codigoBandeira = rechamada.GetValueOrDefault<Int32>("Bandeira", codigosBandeiras.FirstOrDefault());

                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarCreditoDetalhe(
                            dataVencimento,
                            _codigoBandeira,
                            pvs,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            dadosConsulta = new List<DTO.CreditoDetalhe>();
                            indicadorRechamada = false;
                        }

                        //Filtra de acordo com o tipo de registro
                        if (tipoRegistro == CreditoDetalheTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheDT).ToList();
                        else if (tipoRegistro == CreditoDetalheTipoRegistro.AjusteComValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheA1).ToList();
                        else if (tipoRegistro == CreditoDetalheTipoRegistro.AjusteSemValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheA2).ToList();

                        //Verifica se deve pesquisar no próximo dia do período da pesquisa ou para a próxima bandeira
                        if (indicadorRechamada == false)
                        {
                            //Se tem próxima bandeira a ser percorrida, avança bandeira
                            Int32 indiceBandeira = codigosBandeiras.IndexOf(_codigoBandeira);
                            if (indiceBandeira < codigosBandeiras.Count - 1)
                            {
                                rechamada.Clear();
                                rechamada["Bandeira"] = codigosBandeiras[indiceBandeira + 1];
                                rechamada["DataVencimento"] = dataVencimento;
                                indicadorRechamada = true;
                            }
                            //caso contrário, avança a data
                            else if (dataVencimento < dataFinal)
                            {
                                rechamada.Clear();
                                rechamada["Bandeira"] = codigosBandeiras.FirstOrDefault();
                                rechamada["DataVencimento"] = dataVencimento.AddDays(1);
                                indicadorRechamada = true;
                            }
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada, false);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.CreditoDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    var dados = CreditoDetalhe.FromDTO(dadosCache);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados, quantidadeTotalRegistros });

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

        #region [ Lançamentos Futuros - Débito - Totalizadores - WACA1328 / WA1328 / ISHS ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Lançamentos Futuros - Débito.<br/>
        /// - Book WACA1328 / Programa WA1328 / TranID ISHS
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1328 / Programa WA1328 / TranID ISHS
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Débito</returns>
        public DebitoTotalizador ConsultarDebitoTotalizadores(
            Guid guidPesquisa,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Débito - Totalizadores - WACA1328"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigoBandeira, dataInicial, dataFinal, pvs });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHELANCAMENTOSFUTUROS);
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

        #region [ Lançamentos Futuros - Débito - Registros - WACA1329 / WA1329 / ISHT ]

        /// <summary>
        /// Consulta de registros do Relatório de Lançamentos Futuros - Débito.<br/>
        /// - Book WACA1329 / Programa WA1329 / TranID ISHT
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1329 / Programa WA1329 / TranID ISHT
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Lançamentos Futuros - Débito.</returns>
        public List<Debito> ConsultarDebito(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DebitoTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Débito - WACA1329"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHELANCAMENTOSFUTUROS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(tipoRegistro);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    Guid guid = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Debito>(Cache.Extrato,
                        guid, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDebito(
                            dataInicial,
                            dataFinal,
                            codigoBandeira,
                            pvs,
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

                        //Filtra de acordo com o tipo de registro
                        if (tipoRegistro == DebitoTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro.TipoRegistro.Equals("DT", StringComparison.InvariantCultureIgnoreCase)).ToList();
                        else if (tipoRegistro == DebitoTipoRegistro.AjusteComValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro.TipoRegistro.Equals("A1", StringComparison.InvariantCultureIgnoreCase)).ToList();
                        else if (tipoRegistro == DebitoTipoRegistro.AjusteSemValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro.TipoRegistro.Equals("A2", StringComparison.InvariantCultureIgnoreCase)).ToList();

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

                    Log.GravarLog(EventoLog.FimServico, new { status, dados, quantidadeTotalRegistros });

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