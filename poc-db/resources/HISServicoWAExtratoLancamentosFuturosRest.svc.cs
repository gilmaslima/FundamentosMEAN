/*
© Copyright 2017 Rede S.A.
Autor	: Francisco Mazali
Empresa	: Iteris Consultoria e Software LTDA
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.LancamentosFuturos;
using Redecard.PN.Extrato.Servicos.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL = Redecard.PN.Extrato.Negocio.LancamentosFuturosBLL;
using ContratoFuturos = Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response;
using DTO = Redecard.PN.Extrato.Modelo.LancamentosFuturos;

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
    public class HISServicoWAExtratoLancamentosFuturosRest : ServicoBaseExtrato, IHISServicoWAExtratoLancamentosFuturosRest
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
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        public RelatorioLancamentosFuturosCreditoResponse ConsultarRelatorioCredito(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros)
        {
            RelatorioLancamentosFuturosCreditoResponse retorno = new RelatorioLancamentosFuturosCreditoResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoFuturos.ResponseBaseItem<CreditoTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarCreditoTotalizadores(
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoFuturos.ResponseBaseList<Credito>> funcaoRegistros = () =>
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
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataVencimento">Data de vencimento</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        public RelatorioLancamentosFuturosCreditoDetalheResponse ConsultarRelatorioCreditoDetalhe(
            Int32 codigoBandeira,
            String dataVencimento,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,
            Int32 quantidadeTotalRegistros)
        {
            RelatorioLancamentosFuturosCreditoDetalheResponse retorno = new RelatorioLancamentosFuturosCreditoDetalheResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoFuturos.ResponseBaseItem<CreditoDetalheTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarCreditoDetalheTotalizadores(
                    codigoBandeira,
                    dataVencimento,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoFuturos.ResponseBaseList<CreditoDetalhe>> funcaoRegistros = () =>
            {
                return ConsultarCreditoDetalhe(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataVencimento,
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
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo de registro a ser retornado</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        public RelatorioLancamentosFuturosDebitoResponse ConsultarRelatorioDebito(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            DebitoTipoRegistro tipoRegistro,
            Int32 quantidadeTotalRegistros)
        {
            RelatorioLancamentosFuturosDebitoResponse retorno = new RelatorioLancamentosFuturosDebitoResponse();

            //Preparação da Func para consulta dos totalizadores
            Func<ContratoFuturos.ResponseBaseItem<DebitoTotalizador>> funcaoTotalizador = () =>
            {
                return ConsultarDebitoTotalizadores(
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs);
            };

            //Preparação da Func para consulta dos registros
            Func<ContratoFuturos.ResponseBaseList<Debito>> funcaoRegistros = () =>
            {
                return ConsultarDebito(
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeRegistros,
                    tipoRegistro,
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
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Crédito</returns>
        public ContratoFuturos.ResponseBaseItem<CreditoTotalizador> ConsultarCreditoTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Crédito - Totalizadores - WACA1324"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, dataInicial, dataFinal, pvs });

                ContratoFuturos.ResponseBaseItem<CreditoTotalizador> retorno = new ContratoFuturos.ResponseBaseItem<CreditoTotalizador>();

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHELANCAMENTOSFUTUROS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
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
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoFuturos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    retorno.Item = CreditoTotalizador.FromDTO(dadosConsulta);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
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
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Lançamentos Futuros - Crédito.</returns>
        public ContratoFuturos.ResponseBaseList<Credito> ConsultarCredito(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            ContratoFuturos.ResponseBaseList<Credito> retorno = new ContratoFuturos.ResponseBaseList<Credito>();

            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Crédito - WACA1325"))
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

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHELANCAMENTOSFUTUROS);
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

                            retorno.QuantidadeTotalRegistros = 0;
                            retorno.StatusRetorno = ContratoFuturos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Credito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    retorno.Itens = Credito.FromDTO(dadosCache).ToArray();
                    retorno.QuantidadeTotalRegistros = quantidadeRegistros;
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
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
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataVencimento">Data de vencimento</param>        
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe</returns>
        public ContratoFuturos.ResponseBaseItem<CreditoDetalheTotalizador> ConsultarCreditoDetalheTotalizadores(
            Int32 codigoBandeira,
            String dataVencimento,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Crédito - Detalhe - Totalizadores - WACA1326"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, dataVencimento, pvs });

                ContratoFuturos.ResponseBaseItem<CreditoDetalheTotalizador> retorno = new ContratoFuturos.ResponseBaseItem<CreditoDetalheTotalizador>();

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHELANCAMENTOSFUTUROS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataVencimento);
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
                            Utils.ParseDate(dataVencimento),
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoFuturos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    retorno.Item = CreditoDetalheTotalizador.FromDTO(dadosConsulta);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
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
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataVencimento">Data de vencimento</param>        
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Lançamentos Futuros - Crédito - Detalhe.</returns>
        public ContratoFuturos.ResponseBaseList<CreditoDetalhe> ConsultarCreditoDetalhe(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            String dataVencimento,
            List<Int32> pvs)
        {
            ContratoFuturos.ResponseBaseList<CreditoDetalhe> retorno = new ContratoFuturos.ResponseBaseList<CreditoDetalhe>();

            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Crédito - Detalhe - WACA1327"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
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
                    idCache.Append(dataVencimento);
                    idCache.Append(String.Join(",", pvs));

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
                            Utils.ParseDate(dataVencimento),
                            codigoBandeira,
                            pvs,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.StatusRetorno = ContratoFuturos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            quantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Filtra de acordo com o tipo de registro
                        if (tipoRegistro == CreditoDetalheTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheDT).ToList();
                        else if (tipoRegistro == CreditoDetalheTipoRegistro.AjusteComValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheA1).ToList();
                        else if (tipoRegistro == CreditoDetalheTipoRegistro.AjusteSemValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.CreditoDetalheA2).ToList();

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.CreditoDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);


                    retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;
                    retorno.Itens = CreditoDetalhe.FromDTO(dadosCache).ToArray();
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
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
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Débito</returns>
        public ContratoFuturos.ResponseBaseItem<DebitoTotalizador> ConsultarDebitoTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Débito - Totalizadores - WACA1328"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, dataInicial, dataFinal, pvs });

                ContratoFuturos.ResponseBaseItem<DebitoTotalizador> retorno = new ContratoFuturos.ResponseBaseItem<DebitoTotalizador>();

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHELANCAMENTOSFUTUROS);
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
                            retorno.StatusRetorno = ContratoFuturos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta });
                            return retorno;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Conversão de DTO para modelo de serviço
                    retorno.Item = DebitoTotalizador.FromDTO(dadosConsulta);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
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
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Lançamentos Futuros - Débito.</returns>
        public ContratoFuturos.ResponseBaseList<Debito> ConsultarDebito(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            DebitoTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("Lançamentos Futuros - Débito - WACA1329"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                ContratoFuturos.ResponseBaseList<Debito> retorno = new ContratoFuturos.ResponseBaseList<Debito>();

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHELANCAMENTOSFUTUROS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(tipoRegistro);
                    idCache.Append(dataInicial);
                    idCache.Append(dataFinal);
                    idCache.Append(String.Join(",", pvs));

                    Guid guidPesquisa = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Debito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDebito(
                            Utils.ParseDate(dataInicial),
                            Utils.ParseDate(dataFinal),
                            codigoBandeira,
                            pvs,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            retorno.QuantidadeTotalRegistros = 0;
                            retorno.StatusRetorno = ContratoFuturos.StatusRetorno.NaoEncontrado;
                            retorno.Mensagem = "Nenhum resultado para a pesquisa enviada.";
                            Log.GravarLog(EventoLog.FimServico, new { Status = StatusRetorno.FromDTO(statusDTO), dadosConsulta, quantidadeTotalRegistros });
                            return retorno;
                        }

                        //Filtra de acordo com o tipo de registro
                        if (tipoRegistro == DebitoTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro.TipoRegistro.Equals("DT", StringComparison.InvariantCultureIgnoreCase)).ToList();
                        else if (tipoRegistro == DebitoTipoRegistro.AjusteComValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro.TipoRegistro.Equals("A1", StringComparison.InvariantCultureIgnoreCase)).ToList();
                        else if (tipoRegistro == DebitoTipoRegistro.AjusteSemValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro.TipoRegistro.Equals("A2", StringComparison.InvariantCultureIgnoreCase)).ToList();

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Debito>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    retorno.QuantidadeTotalRegistros = quantidadeTotalRegistros;
                    retorno.Itens = Debito.FromDTO(dadosCache).ToArray();
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.OK;

                    Log.GravarLog(EventoLog.FimServico, new { Status = new StatusRetorno(0, String.Empty, FONTE), retorno });
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.ErroNegocio;
                    retorno.Mensagem = ex.Message;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    retorno.StatusRetorno = ContratoFuturos.StatusRetorno.ErroSistema;
                    retorno.Mensagem = ex.Message;
                }

                return retorno;
            }
        }

        #endregion
    }
}