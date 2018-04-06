using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using DTO = Redecard.PN.Extrato.Modelo.AntecipacaoRAV;
using BLL = Redecard.PN.Extrato.Negocio.AntecipacaoRAVBLL;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.AntecipacaoRAV;
using System.Threading.Tasks;
using Redecard.PN.Extrato.Modelo.Comum;
using System.Collections.Concurrent;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Antecipações RAV.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1330	/ Programa WA1330 / TranID ISHU / Método ConsultarTotalizadores<br/>
    /// - Book WACA1331	/ Programa WA1331 / TranID ISHV / Método Consultar<br/>
    /// - Book WACA1332	/ Programa WA1332 / TranID ISHX / Método ConsultarDetalheTotalizadores<br/>
    /// - Book WACA1333	/ Programa WA1333 / TranID ISHY / Método ConsultarDetalhe
    /// </remarks>
    public class HISServicoWA_Extrato_AntecipacaoRAV : ServicoBaseExtrato, IHISServicoWA_Extrato_AntecipacaoRAV
    {
        #region [ RELATÓRIOS - Consultas Multi-Thread (Totalizadores e Registros) ]

        /// <summary>
        /// Consulta de Relatório de Antecipações RAV.<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WACA1330 / Programa WA1330 / TranID ISHU<br/>
        /// - Book WACA1331 / Programa WA1331 / TranID ISHV
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1330 / Programa WA1330 / TranID ISHU<br/>
        /// - Book WACA1331 / Programa WA1331 / TranID ISHV
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
        public void ConsultarRelatorio(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out RAVTotalizador totalizador,
            out List<RAV> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno _statusTotalizador = null, _statusRelatorio = null;
            Int32 _quantidadeTotalRegistros = quantidadeTotalRegistros;
            
            //Preparação da Func para consulta dos totalizadores
            Func<RAVTotalizador> _funcaoTotalizador = () => {
                return ConsultarTotalizadores(
                    guidPesquisaTotalizador,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<RAV>> _funcaoRegistros = () => {
                return Consultar(
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
        /// Consulta de Relatório de Antecipações RAV - Detalhe<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1332 / Programa WA1332 / TranID ISHX<br/>
        /// - Book WA1333 / Programa WA1333 / TranID ISHY
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1332 / Programa WA1332 / TranID ISHX<br/>
        /// - Book WA1333 / Programa WA1333 / TranID ISHY
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataAntecipacao">Data de antecipação</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        public void ConsultarRelatorioDetalhe(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,            
            Int32 codigoBandeira,
            DateTime dataAntecipacao,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            RAVDetalheTipoRegistro tipoRegistro,
            ref Int32 quantidadeTotalRegistros,
            out RAVDetalheTotalizador totalizador,
            out List<RAVDetalhe> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno _statusTotalizador = null, _statusRelatorio = null;            
            Int32 _quantidadeTotalRegistros = quantidadeTotalRegistros;
            
            //Preparação da Func para consulta dos totalizadores
            Func<RAVDetalheTotalizador> _funcaoTotalizador = () => {
                return ConsultarDetalheTotalizadores(
                    guidPesquisaTotalizador,
                    codigoBandeira,
                    dataAntecipacao,
                    pvs,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<RAVDetalhe>> _funcaoRegistros = () => {
                return ConsultarDetalhe(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref _quantidadeTotalRegistros,
                    tipoRegistro,                                
                    codigoBandeira,
                    dataAntecipacao,
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
        /// Consulta de Relatório de Antecipações RAV - Detalhe - Ver Todos<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1332 / Programa WA1332 / TranID ISHX<br/>
        /// - Book WA1333 / Programa WA1333 / TranID ISHY</summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1332 / Programa WA1332 / TranID ISHX<br/>
        /// - Book WA1333 / Programa WA1333 / TranID ISHY
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
        public void ConsultarRelatorioDetalheTodos(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,            
            List<Int32> codigosBandeiras,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            RAVDetalheTipoRegistro tipoRegistro,
            ref Int32 quantidadeTotalRegistros,
            out RAVDetalheTotalizador totalizador,
            out List<RAVDetalhe> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno _statusTotalizador = null, _statusRelatorio = null;             
            Int32 _quantidadeTotalRegistros = quantidadeTotalRegistros;            

            //Preparação da Func para consulta dos totalizadores
            Func<RAVDetalheTotalizador> _funcaoTotalizador = () => {
                return ConsultarDetalheTodosTotalizadores(
                    guidPesquisaTotalizador,                               
                    codigosBandeiras,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out _statusTotalizador); };
            
            //Preparação da Func para consulta dos registros
            Func<List<RAVDetalhe>> _funcaoRegistros = () =>
            {
                return ConsultarDetalheTodos(
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

        #endregion

        #region [ Antecipações RAV - Totalizadores - WACA1330 / WA1330 / ISHU ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Antecipações RAV.<br/>
        /// - Book WACA1330 / Programa WA1330 / TranID ISHU
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1330 / Programa WA1330 / TranID ISHU
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Antecipações RAV</returns>
        public RAVTotalizador ConsultarTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Antecipação RAV - Totalizadores - WACA1330"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigoBandeira, dataInicial, dataFinal, pvs });

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEANTECIPADOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.RAVTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarTotalizadores(                            
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
                    var dados = RAVTotalizador.FromDTO(dadosConsulta);

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

        #region [ Antecipações RAV - Registros - WACA1331 / WA1331 / ISHV ]

        /// <summary>
        /// Consulta de registros do Relatório de Antecipações RAV.<br/>
        /// - Book WACA1331 / Programa WA1331 / TranID ISHV
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1331 / Programa WA1331 / TranID ISHV
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
        /// <returns>Registros do Relatório de Antecipações RAV</returns>
        public List<RAV> Consultar(
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
            using (Logger Log = Logger.IniciarLog("Antecipação RAV - WACA1331"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros,
                    quantidadeTotalRegistros, codigoBandeira, dataInicial, dataFinal, pvs });

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEANTECIPADOS);
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
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.RAV>(Cache.Extrato,
                        guid, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.Consultar(                            
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
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.RAV>(Cache.Extrato,
                        guid, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    var dados = RAV.FromDTO(dadosCache);

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

        #region [ Antecipações RAV - Detalhe - Totalizadores - WACA1332 / WA1332 / ISHX ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Antecipações RAV - Detalhe.<br/>
        /// - Book WACA1332 / Programa WA1332 / TranID ISHX
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1332 / Programa WA1332 / TranID ISHX
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataAntecipacao">Data de antecipação</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizador do Relatório de Antecipações RAV - Detalhe</returns>
        public RAVDetalheTotalizador ConsultarDetalheTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataAntecipacao,            
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Antecipação RAV - Detalhe - Totalizadores - WACA1332"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigoBandeira, dataAntecipacao, pvs });

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEANTECIPADOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataAntecipacao.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.RAVDetalheTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarDetalheTotalizadores(                            
                            codigoBandeira,
                            pvs,
                            dataAntecipacao,
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
                    var dados = RAVDetalheTotalizador.FromDTO(dadosConsulta);

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
        /// Consulta de totalizadores do Relatório de Antecipações RAV - Detalhe, para um intervalo de datas.
        /// - Book WACA1332 / Programa WA1332 / TranID ISHX
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1332 / Programa WA1332 / TranID ISHX
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigosBandeiras">Códigos das bandeiras</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizador do Relatório de Antecipações RAV - Detalhe</returns>
        public RAVDetalheTotalizador ConsultarDetalheTodosTotalizadores(
            Guid guidPesquisa,            
            List<Int32> codigosBandeiras,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Antecipação RAV - Detalhe Todos - Totalizadores - WACA1332"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigoBandeira = codigosBandeiras, dataInicial, dataFinal, pvs });

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEANTECIPADOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", codigosBandeiras));
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.RAVDetalheTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarDetalheTodosTotalizadores(                            
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
                    var dados = RAVDetalheTotalizador.FromDTO(dadosConsulta);

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

        #region [ Antecipações RAV - Detalhe - Registros - WACA1333 / WA1333 / ISHY ]

        /// <summary>
        /// Consulta de registros do Relatório de Antecipações RAV - Detalhe.<br/>
        /// - Book WACA1333 / Programa WA1333 / TranID ISHY
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1333 / Programa WA1333 / TranID ISHY
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros existentes</param>
        /// <param name="tipoRegistro">Tipo de registro a ser retornado</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataAntecipacao">Data de antecipação</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Antecipações - RAV - Detalhe</returns>
        public List<RAVDetalhe> ConsultarDetalhe(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            RAVDetalheTipoRegistro tipoRegistro,            
            Int32 codigoBandeira,
            DateTime dataAntecipacao,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Antecipação RAV - Detalhe - WACA1333"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros, 
                    quantidadeTotalRegistros, tipoRegistro, codigoBandeira, dataAntecipacao, pvs});

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEANTECIPADOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(tipoRegistro);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataAntecipacao.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    Guid guid = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.RAVDetalhe>(Cache.Extrato,
                        guid, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDetalhe(                            
                            codigoBandeira,
                            pvs,
                            dataAntecipacao,
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
                        if (tipoRegistro == RAVDetalheTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.RAVDetalheDT).ToList();
                        else if (tipoRegistro == RAVDetalheTipoRegistro.AjusteComValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.RAVDetalheA1).ToList();
                        else if (tipoRegistro == RAVDetalheTipoRegistro.AjusteSemValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.RAVDetalheA2).ToList();

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guid, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.RAVDetalhe>(Cache.Extrato,
                        guid, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    var dados = RAVDetalhe.FromDTO(dadosCache);

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
        /// Consulta de registros do Relatório de Antecipações RAV - Detalhe, para um intervalo de datas.
        /// Se código da bandeira informado for igual a 0, pesquisa para todas as bandeiras (1 a 12).
        /// - Book WACA1333 / Programa WA1333 / TranID ISHY
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1333 / Programa WA1333 / TranID ISHY
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros existentes</param>
        /// <param name="tipoRegistro">Tipo de registro a ser retornado</param>        
        /// <param name="codigosBandeiras">Código da bandeira (se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Antecipações - RAV - Detalhe</returns>
        public List<RAVDetalhe> ConsultarDetalheTodos(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            RAVDetalheTipoRegistro tipoRegistro,            
            List<Int32> codigosBandeiras,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Antecipação RAV - Detalhe Todos - WACA1333"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros, 
                    quantidadeTotalRegistros, tipoRegistro, codigosBandeiras, dataInicial, dataFinal, pvs });

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEANTECIPADOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(tipoRegistro);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));
                    idCache.Append(String.Join(",", codigosBandeiras));

                    Guid guid = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.RAVDetalhe>(Cache.Extrato,
                        guid, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Data da pesquisa é por padrão a Data Inicial do período solicitado
                        DateTime dataAntecipacao = rechamada.GetValueOrDefault<DateTime>("DataAntecipacao", dataInicial);
                        Int32 _codigoBandeira = rechamada.GetValueOrDefault<Int32>("Bandeira", codigosBandeiras.FirstOrDefault());

                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDetalhe(                            
                            _codigoBandeira,
                            pvs,
                            dataAntecipacao,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            dadosConsulta = new List<DTO.RAVDetalhe>();                            
                            indicadorRechamada = false;                            
                        }

                        //Filtra de acordo com o tipo de registro
                        if (tipoRegistro == RAVDetalheTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.RAVDetalheDT).ToList();
                        else if (tipoRegistro == RAVDetalheTipoRegistro.AjusteComValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.RAVDetalheA1).ToList();
                        else if (tipoRegistro == RAVDetalheTipoRegistro.AjusteSemValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.RAVDetalheA2).ToList();

                        //Verifica se deve pesquisar no próximo dia do período da pesquisa ou para a próxima bandeira
                        if (indicadorRechamada == false)
                        {
                            //Se tem próxima bandeira a ser percorrida, avança bandeira
                            Int32 indiceBandeira = codigosBandeiras.IndexOf(_codigoBandeira);
                            if (indiceBandeira < codigosBandeiras.Count - 1)
                            {
                                rechamada.Clear();
                                rechamada["Bandeira"] = codigosBandeiras[indiceBandeira + 1];
                                rechamada["DataAntecipacao"] = dataAntecipacao;
                                indicadorRechamada = true;
                            }
                            //caso contrário, avança a data
                            else if (dataAntecipacao < dataFinal)
                            {
                                rechamada.Clear();
                                rechamada["Bandeira"] = codigosBandeiras.FirstOrDefault();
                                rechamada["DataAntecipacao"] = dataAntecipacao.AddDays(1);
                                indicadorRechamada = true;
                            }
                        }
                     
                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guid, dadosConsulta, indicadorRechamada, rechamada, false);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.RAVDetalhe>(Cache.Extrato,
                        guid, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    var dados = RAVDetalhe.FromDTO(dadosCache);

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
