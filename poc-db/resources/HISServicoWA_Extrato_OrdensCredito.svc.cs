using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DTO = Redecard.PN.Extrato.Modelo.OrdensCredito;
using BLL = Redecard.PN.Extrato.Negocio.OrdensCreditoBLL;
using Redecard.PN.Extrato.Servicos.OrdensCredito;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Modelo;
using System.Threading.Tasks;
using Redecard.PN.Extrato.Modelo.Comum;
using System.Collections.Concurrent;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Ordens de Crédito.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1334	/ Programa WA1334 / TranID ISHZ / Método ConsultarTotalizadores<br/>
    /// - Book WACA1335	/ Programa WA1335 / TranID ISHW / Método Consultar<br/>
    /// - Book WACA1336	/ Programa WA1336 / TranID ISH0 / Método ConsultarDetalheTotalizadores<br/>
    /// - Book WACA1337	/ Programa WA1337 / TranID ISH1 / Método ConsultarDetalhe
    /// </remarks>
    public class HISServicoWA_Extrato_OrdensCredito : ServicoBaseExtrato, IHISServicoWA_Extrato_OrdensCredito
    {
        #region [ RELATÓRIOS - Consultas Multi-Thread (Totalizadores e Registros) ]

        /// <summary>
        /// Consulta de Relatório de Ordens de Crédito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1334 / Programa WA1334 / TranID ISHZ<br/>
        /// - Book WACA1335 / Programa WA1335 / TranID ISHW
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1334 / Programa WA1334 / TranID ISHZ<br/>
        /// - Book WACA1335 / Programa WA1335 / TranID ISHW
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
                return ConsultarTotalizadores(
                    guidPesquisaTotalizador,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<Credito>> _funcaoRegistros = () => {
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
        /// Consulta de Relatório de Ordens de Crédito - Detalhe<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1336 / Programa WA1336 / TranID ISH0<br/>
        /// - Book WA1337 / Programa WA1337 / TranID ISH1
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1336 / Programa WA1336 / TranID ISH0<br/>
        /// - Book WA1337 / Programa WA1337 / TranID ISH1
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataEmissao">Data de emissão</param>
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
            DateTime dataEmissao,            
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
                return ConsultarDetalheTotalizadores(
                    guidPesquisaTotalizador,
                    codigoBandeira,
                    dataEmissao,
                    pvs,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<CreditoDetalhe>> _funcaoRegistros = () => {
                return ConsultarDetalhe(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref _quantidadeTotalRegistros,
                    tipoRegistro,                                
                    codigoBandeira,
                    dataEmissao,
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
        /// Consulta de Relatório de Ordens de Crédito - Detalhe - Ver Todos<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1336 / Programa WA1336 / TranID ISH0<br/>
        /// - Book WA1337 / Programa WA1337 / TranID ISH1
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1336 / Programa WA1336 / TranID ISH0<br/>
        /// - Book WA1337 / Programa WA1337 / TranID ISH1
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
            CreditoDetalheTipoRegistro tipoRegistro,
            ref Int32 quantidadeTotalRegistros,
            out CreditoDetalheTotalizador totalizador,
            out List<CreditoDetalhe> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno _statusTotalizador = null, _statusRelatorio = null;
            CreditoDetalheTotalizador _totalizador = null;
            List<CreditoDetalhe> _registros = null;
            Int32 _quantidadeTotalRegistros = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<CreditoDetalheTotalizador> _funcaoTotalizador = () => {
                return ConsultarDetalheTodosTotalizadores(
                    guidPesquisaTotalizador,                                
                    codigosBandeiras,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<CreditoDetalhe>> _funcaoRegistros = () => {
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

        #endregion [ RELATÓRIOS - Consultas Multi-Thread (Totalizadores e Registros) ]
               
        #region [ Ordens de Crédito - Totalizadores - WACA1334 / WA1334 / ISHZ ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Ordens de Crédito.<br/>
        /// - Book WACA1334 / Programa WA1334 / TranID ISHZ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1334 / Programa WA1334 / TranID ISHZ
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Ordens de Crédito</returns>
        public CreditoTotalizador ConsultarTotalizadores(            
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Ordens de Crédito - Totalizadores - WACA1334"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {  guidPesquisa, codigoBandeira, dataInicial, dataFinal, pvs });

                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = guidPesquisa.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.CreditoTotalizador>(Cache.Extrato, idPesquisa);

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

        #region [ Ordens de Crédito - Registros - WACA1335 / WA1335 / ISHW ]

        /// <summary>
        /// Consulta de registros do Relatório de Ordens de Crédito.<br/>
        /// - Book WACA1335 / Programa WA1335 / TranID ISHW
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1335 / Programa WA1335 / TranID ISHW
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Ordens de Crédito.</returns>
        public List<Credito> Consultar(
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
            using (Logger Log = Logger.IniciarLog("Ordens de Crédito - WACA1335"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {  guidPesquisa, registroInicial, quantidadeRegistros, 
                    quantidadeTotalRegistros, codigoBandeira, dataInicial, dataFinal, pvs });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Credito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
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
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Credito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

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

        #region [ Ordens de Crédito - Detalhe - Totalizadores - WACA1336 / WA1336 / ISH0 ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Ordens de Crédito - Detalhe.<br/>
        /// - Book WACA1336 / Programa WA1336 / TranID ISH0
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1336 / Programa WA1336 / TranID ISH0
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataEmissao">Data da emissão</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Ordens de Crédito - Detalhe</returns>
        public CreditoDetalheTotalizador ConsultarDetalheTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataEmissao,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Ordens de Crédito - Detalhe - Totalizadores - WACA1336"))
            {
                 Log.GravarLog(EventoLog.InicioServico, new {  guidPesquisa, codigoBandeira, dataEmissao, pvs });

                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = guidPesquisa.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta =
                        CacheAdmin.Recuperar<DTO.CreditoDetalheTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarDetalheTotalizadores(                            
                            codigoBandeira,
                            pvs,
                            dataEmissao,
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
        /// Consulta de totalizadores do Relatório de Ordens de Crédito - Detalhe - Ver Todos.<br/>
        /// - Book WACA1336 / Programa WA1336 / TranID ISH0
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1336 / Programa WA1336 / TranID ISH0
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigosBandeiras">Códigos das bandeiras</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Ordens de Crédito - Detalhe - Ver Todos</returns>
        public CreditoDetalheTotalizador ConsultarDetalheTodosTotalizadores(
            Guid guidPesquisa,            
            List<Int32> codigosBandeiras,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Ordens de Crédito - Detalhe Todos - Totalizadores - WACA1336"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigosBandeiras, dataInicial, dataFinal, pvs });

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

        #region [ Ordens de Crédito - Detalhe - Registros - WACA1337 / WA1337 / ISH1 ]

        /// <summary>
        /// Consulta de registros do Relatório de Ordens de Crédito - Detalhe.<br/>
        /// - Book WACA1337 / Programa WA1337 / TranID ISH1
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1337 / Programa WA1337 / TranID ISH1
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param> 
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataEmissao">Data de emissão</param>        
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Ordens de Crédito - Detalhe.</returns>
        public List<CreditoDetalhe> ConsultarDetalhe(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,            
            Int32 codigoBandeira,
            DateTime dataEmissao,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Ordens de Crédito - Detalhe - WACA1337"))
            {
                Log.GravarLog(EventoLog.InicioServico, new {  guidPesquisa, registroInicial, quantidadeRegistros, 
                    quantidadeTotalRegistros, tipoRegistro, codigoBandeira, dataEmissao, pvs });

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
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDetalhe(                            
                            codigoBandeira,
                            pvs,
                            dataEmissao,
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
                        if (tipoRegistro == CreditoDetalheTipoRegistro.Ajustes)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheD1).ToList();
                        else if (tipoRegistro == CreditoDetalheTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheDT).ToList();

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.CreditoDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

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

        /// <summary>
        /// Consulta de registros do Relatório de Ordens de Crédito - Detalhe - Ver Todos.<br/>
        /// - Book WACA1337 / Programa WA1337 / TranID ISH1
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1337 / Programa WA1337 / TranID ISH1
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
        /// <returns>Registros do Relatório de Ordens de Crédito - Detalhe - Ver Todos.</returns>
        public List<CreditoDetalhe> ConsultarDetalheTodos(
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
            using (Logger Log = Logger.IniciarLog("Ordens de Crédito - Detalhe Todos - WACA1337"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros, 
                    quantidadeTotalRegistros, tipoRegistro, codigosBandeiras, dataInicial, dataFinal, pvs });

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
                        DateTime dataEmissao = rechamada.GetValueOrDefault<DateTime>("DataEmissao", dataInicial);
                        Int32 _codigoBandeira = rechamada.GetValueOrDefault<Int32>("Bandeira", codigosBandeiras.FirstOrDefault());

                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDetalhe(                            
                            _codigoBandeira,
                            pvs,
                            dataEmissao,
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
                        if (tipoRegistro == CreditoDetalheTipoRegistro.Ajustes)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheD1).ToList();
                        else if (tipoRegistro == CreditoDetalheTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheDT).ToList();

                        //Verifica se deve pesquisar no próximo dia do período da pesquisa ou para a próxima bandeira
                        if (indicadorRechamada == false)
                        {
                            //Se tem próxima bandeira a ser percorrida, avança bandeira
                            Int32 indiceBandeira = codigosBandeiras.IndexOf(_codigoBandeira);
                            if (indiceBandeira < codigosBandeiras.Count - 1)
                            {
                                rechamada.Clear();
                                rechamada["Bandeira"] = codigosBandeiras[indiceBandeira + 1];
                                rechamada["DataEmissao"] = dataEmissao;
                                indicadorRechamada = true;
                            }
                            //caso contrário, avança a data
                            else if (dataEmissao < dataFinal)
                            {
                                rechamada.Clear();
                                rechamada["Bandeira"] = codigosBandeiras.FirstOrDefault();
                                rechamada["DataEmissao"] = dataEmissao.AddDays(1);
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