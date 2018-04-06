/*
© Copyright 2017 Rede S.A.
Autor	: Francisco Mazali
Empresa	: Iteris Consultoria e Software LTDA
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.AntecipacaoRAV;
using Redecard.PN.Extrato.Servicos.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL = Redecard.PN.Extrato.Negocio.AntecipacaoRAVBLL;
using ContratoAntecipados = Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response;
using DTO = Redecard.PN.Extrato.Modelo.AntecipacaoRAV;

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
    public class HISServicoWAExtratoAntecipadosRest : ServicoBaseExtrato, IHISServicoWAExtratoAntecipadosRest
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
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>

        public RelatorioAntecipadosResponse ConsultarRelatorio(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros)
        {
            RelatorioAntecipadosResponse retorno = new RelatorioAntecipadosResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoAntecipados.ResponseBaseItem<RAVTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarTotalizadores(
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoAntecipados.ResponseBaseList<RAV>> funcaoRegistros = () =>
            {
                return Consultar(
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
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataAntecipacao">Data de antecipação</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        public RelatorioAntecipadosDetalheResponse ConsultarRelatorioDetalhe(
            Int32 codigoBandeira,
            String dataAntecipacao,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            RAVDetalheTipoRegistro tipoRegistro,
            Int32 quantidadeTotalRegistros)
        {
            RelatorioAntecipadosDetalheResponse retorno = new RelatorioAntecipadosDetalheResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoAntecipados.ResponseBaseItem<RAVDetalheTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarDetalheTotalizadores(
                    codigoBandeira,
                    dataAntecipacao,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoAntecipados.ResponseBaseList<RAVDetalhe>> funcaoRegistros = () =>
            {
                return ConsultarDetalhe(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataAntecipacao,
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
        /// Consulta de Relatório de Antecipações RAV - Detalhe - Ver Todos<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1332 / Programa WA1332 / TranID ISHX<br/>
        /// - Book WA1333 / Programa WA1333 / TranID ISHY</summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1332 / Programa WA1332 / TranID ISHX<br/>
        /// - Book WA1333 / Programa WA1333 / TranID ISHY
        /// </remarks>
        /// <param name="codigosBandeiras">Códigos das bandeiras</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        public RelatorioAntecipadosDetalheResponse ConsultarRelatorioDetalheTodos(
            List<Int32> codigosBandeiras,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            RAVDetalheTipoRegistro tipoRegistro,
            Int32 quantidadeTotalRegistros)
        {

            RelatorioAntecipadosDetalheResponse retorno = new RelatorioAntecipadosDetalheResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoAntecipados.ResponseBaseItem<RAVDetalheTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarDetalheTodosTotalizadores(
                    codigosBandeiras,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoAntecipados.ResponseBaseList<RAVDetalhe>> funcaoRegistros = () =>
            {
                return ConsultarDetalheTodos(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigosBandeiras,
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
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Antecipações RAV</returns>
        public ContratoAntecipados.ResponseBaseItem<RAVTotalizador> ConsultarTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Antecipação RAV - Totalizadores - WACA1330"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, dataInicial, dataFinal, pvs });

                ContratoAntecipados.ResponseBaseItem<RAVTotalizador> retorno = new ContratoAntecipados.ResponseBaseItem<RAVTotalizador>();

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEANTECIPADOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
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
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }


                    //Conversão de DTO para modelo de serviço
                    retorno.Item = RAVTotalizador.FromDTO(dadosConsulta);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
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
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Antecipações RAV</returns>
        public ContratoAntecipados.ResponseBaseList<RAV> Consultar(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Antecipação RAV - WACA1331"))
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

                ContratoAntecipados.ResponseBaseList<RAV> retorno = new ContratoAntecipados.ResponseBaseList<RAV>();

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEANTECIPADOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.RAV>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.Consultar(
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
                            retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            retorno.QuantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.RAV>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Converte DTO para modelo de serviço
                    retorno.Itens = RAV.FromDTO(dadosCache).ToArray();
                    retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
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
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataAntecipacao">Data de antecipação</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizador do Relatório de Antecipações RAV - Detalhe</returns>
        public ContratoAntecipados.ResponseBaseItem<RAVDetalheTotalizador> ConsultarDetalheTotalizadores(
            Int32 codigoBandeira,
            String dataAntecipacao,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Antecipação RAV - Detalhe - Totalizadores - WACA1332"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, dataAntecipacao, pvs });

                ContratoAntecipados.ResponseBaseItem<RAVDetalheTotalizador> retorno = new ContratoAntecipados.ResponseBaseItem<RAVDetalheTotalizador>();

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEANTECIPADOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataAntecipacao);
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
                            Utils.ParseDate(dataAntecipacao),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Conversão de DTO para modelo de serviço
                    retorno.Item = RAVDetalheTotalizador.FromDTO(dadosConsulta);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
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
        /// <param name="codigosBandeiras">Códigos das bandeiras</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizador do Relatório de Antecipações RAV - Detalhe</returns>
        public ContratoAntecipados.ResponseBaseItem<RAVDetalheTotalizador> ConsultarDetalheTodosTotalizadores(
            List<Int32> codigosBandeiras,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Antecipação RAV - Detalhe Todos - Totalizadores - WACA1332"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira = codigosBandeiras, dataInicial, dataFinal, pvs });

                ContratoAntecipados.ResponseBaseItem<RAVDetalheTotalizador> retorno = new ContratoAntecipados.ResponseBaseItem<RAVDetalheTotalizador>();

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEANTECIPADOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
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
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Conversão de DTO para modelo de serviço
                    retorno.Item = RAVDetalheTotalizador.FromDTO(dadosConsulta);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
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
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros existentes</param>
        /// <param name="tipoRegistro">Tipo de registro a ser retornado</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataAntecipacao">Data de antecipação</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Antecipações - RAV - Detalhe</returns>
        public ContratoAntecipados.ResponseBaseList<RAVDetalhe> ConsultarDetalhe(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            RAVDetalheTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            String dataAntecipacao,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Antecipação RAV - Detalhe - WACA1333"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataAntecipacao,
                    pvs
                });
                ContratoAntecipados.ResponseBaseList<RAVDetalhe> retorno = new ContratoAntecipados.ResponseBaseList<RAVDetalhe>();
                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEANTECIPADOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(tipoRegistro);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataAntecipacao);
                    idCache.Append(String.Join(",", pvs));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.RAVDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDetalhe(
                            codigoBandeira,
                            pvs,
                            Utils.ParseDate(dataAntecipacao),
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            retorno.QuantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Filtra de acordo com o tipo de registro
                        if (tipoRegistro == RAVDetalheTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.RAVDetalheDT).ToList();
                        else if (tipoRegistro == RAVDetalheTipoRegistro.AjusteComValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.RAVDetalheA1).ToList();
                        else if (tipoRegistro == RAVDetalheTipoRegistro.AjusteSemValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.RAVDetalheA2).ToList();

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.RAVDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Converte DTO para modelo de serviço
                    retorno.Itens = RAVDetalhe.FromDTO(dadosCache).ToArray();
                    retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
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
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros existentes</param>
        /// <param name="tipoRegistro">Tipo de registro a ser retornado</param>        
        /// <param name="codigosBandeiras">Código da bandeira (se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Antecipações - RAV - Detalhe</returns>
        public ContratoAntecipados.ResponseBaseList<RAVDetalhe> ConsultarDetalheTodos(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            RAVDetalheTipoRegistro tipoRegistro,
            List<Int32> codigosBandeiras,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Antecipação RAV - Detalhe Todos - WACA1333"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigosBandeiras,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                ContratoAntecipados.ResponseBaseList<RAVDetalhe> retorno = new ContratoAntecipados.ResponseBaseList<RAVDetalhe>();

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEANTECIPADOS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(tipoRegistro);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));
                    idCache.Append(String.Join(",", codigosBandeiras));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);
                    StatusRetorno status = new StatusRetorno();

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.RAVDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Data da pesquisa é por padrão a Data Inicial do período solicitado
                        DateTime dataAntecipacao = rechamada.GetValueOrDefault<DateTime>("DataAntecipacao", Utils.ParseDate(dataInicial));
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
                            retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
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
                            else if (Utils.ParseDate(dataAntecipacao) < Utils.ParseDate(dataFinal))
                            {
                                rechamada.Clear();
                                rechamada["Bandeira"] = codigosBandeiras.FirstOrDefault();
                                rechamada["DataAntecipacao"] = dataAntecipacao.AddDays(1);
                                indicadorRechamada = true;
                            }
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada, false);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.RAVDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    retorno.Itens = RAVDetalhe.FromDTO(dadosCache).ToArray();
                    retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { status, retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoAntecipados.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion
    }
}
