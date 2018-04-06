/*
© Copyright 2015 Rede S.A.
Autor : Dhouglas Lombello
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response;
using Redecard.PN.Extrato.Servicos.Modelo;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using BLL = Redecard.PN.Extrato.Negocio.EstornosBLL;
using DTO = Redecard.PN.Extrato.Modelo.Estornos;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Estorno.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book BKWA2930 / Programa WAC293 / TranID WAAP / Método ConsultarEstornoTotalizadores<br/>
    /// - Book BKWA2940 / Programa WAC294 / TranID WAAQ / Método ConsultarEstorno<br/>
    /// </remarks>
    public class HISServicoWAExtratoEstornosRest : ServicoBaseExtrato, IHISServicoWAExtratoEstornosRest
    {
        #region [ RELATÓRIOS - Consultas Multi-Thread (Totalizadores e Registros) ]

        /// <summary>
        /// Consulta de Relatório de Estornos.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2930 / Programa WAC293 / TranID WAAP<br/>
        /// - Book BKWA2940 / Programa WAC294 / TranID WAAQ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2930 / Programa WAC293 / TranID WAAP<br/>
        /// - Book BKWA2940 / Programa WAC294 / TranID WAAQ
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="tipoModalidade">Tipo da Modalidade da pesquisa</param>
        /// <param name="codigoTipoVenda">Código do Tipo de Venda da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <returns>Objeto RelatorioEstornoResponse</returns>
        public ResponseBaseItem<RelatorioEstornoResponse> ConsultarRelatorioEstorno(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            DateTime dataInicial,
            DateTime dataFinal,
            Int16 tipoModalidade,
            Int16 codigoTipoVenda,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros)
        {
            ResponseBaseItem<RelatorioEstornoResponse> retorno = new ResponseBaseItem<RelatorioEstornoResponse>();
            Logger.GravarLog("ConsultarRelatorioEstorno() - Iniciando Serviço Rest");

            try
            {
                Modelo.StatusRetorno statusTotalizador = new Modelo.StatusRetorno(), statusRelatorio = new Modelo.StatusRetorno();
                //Variáveis auxiliares
                retorno.Item = new RelatorioEstornoResponse();
                retorno.Item.QuantidadeTotalRegistros = quantidadeTotalRegistros;

                Logger.GravarLog("ConsultarRelatorioEstorno() - Iniciando funcaoTotalizador");
                //Preparação da Func para consulta dos totalizadores
                Func<EstornoTotalizador> funcaoTotalizador = () =>
                {
                    return ConsultarEstornoTotalizadores(
                        guidPesquisaTotalizador,
                        dataInicial,
                        dataFinal,
                        tipoModalidade,
                        codigoTipoVenda,
                        pvs,
                        out statusTotalizador);
                };

                Logger.GravarLog("ConsultarRelatorioEstorno() - Iniciando funcaoRegistros");
                //Preparação da Func para consulta dos registros
                Func<List<EstornoD>> funcaoRegistros = () =>
                {
                    return ConsultarEstorno(
                        guidPesquisaRelatorio,
                        registroInicial,
                        quantidadeRegistros,
                        ref quantidadeTotalRegistros,
                        dataInicial,
                        dataFinal,
                        tipoModalidade,
                        codigoTipoVenda,
                        pvs,
                        out statusRelatorio);
                };

                Logger.GravarLog("ConsultarRelatorioEstorno() - Iniciando ConsultaMultiThread");
                //Consulta multi-thread dos totalizadores e registros do relatório
                var retornoConsulta = base.ConsultaMultiThread(funcaoTotalizador, funcaoRegistros);

                //Atribuição de dados de retorno
                retorno.Item.Totalizador = retornoConsulta.Item1;
                retorno.Item.Registros = retornoConsulta.Item2;
                retorno.Item.StatusRelatorio = statusRelatorio;
                retorno.Item.StatusTotalizador = statusTotalizador;
                retorno.Item.QuantidadeTotalRegistros = quantidadeTotalRegistros;
                retorno.StatusRetorno = Contrato.ContratoDados.Response.StatusRetorno.OK;
                //#endif

            }
            catch (PortalRedecardException ex)
            {
                retorno.StatusRetorno = Contrato.ContratoDados.Response.StatusRetorno.ErroNegocio;
                retorno.Mensagem = ex.Message;
            }
            catch (Exception ex)
            {
                retorno.StatusRetorno = Contrato.ContratoDados.Response.StatusRetorno.ErroSistema;
                retorno.Mensagem = ex.Message;
            }
            Logger.GravarLog("ConsultarRelatorioEstorno() - Finalizando Serviço", retorno);

            return retorno;
        }

        #endregion [ RELATÓRIOS - Consultas Multi-Thread (Totalizadores e Registros) ]

        #region [ Estorno - Totalizadores - BKWA2930 / WAC293 / WAAP ]

        /// <summary>
        /// Consulta de totalizadores do Relatório de Estorno.<br/>
        /// - Book BKWA2930 / Programa WAC293 / TranID WAAP
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2930 / Programa WAC293 / TranID WAAP
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>   
        /// <param name="codigoTipoVenda">Código do tipo de venda</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="tipoModalidade">Tipo da Modalidade da pesquisa</param>
        /// <param name="codigoTipoVenda">Código do Tipo da Venda da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Vendas - Crédito</returns>
        private EstornoTotalizador ConsultarEstornoTotalizadores(
            Guid guidPesquisa,
            DateTime dataInicial,
            DateTime dataFinal,
            Int16 tipoModalidade,
            Int16 codigoTipoVenda,
            List<Int32> pvs,
            out Modelo.StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Estorno - Totalizadores - BKWA2930"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    dataInicial,
                    dataFinal,
                    tipoModalidade,
                    codigoTipoVenda,
                    pvs
                });

                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = guidPesquisa.ToString();
                    StatusRetornoDTO statusDTO;
#if DEBUG
                    DTO.EstornoTotalizador dadosConsulta = null;
#else
                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.EstornoTotalizador>(Cache.Extrato, idPesquisa);
#endif
                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarTotalizadores(
                            pvs,
                            dataInicial,
                            dataFinal,
                            tipoModalidade,
                            codigoTipoVenda,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO != null && statusDTO.CodigoRetorno != 0 && statusDTO.CodigoRetorno != 60)
                        {
                            status = Modelo.StatusRetorno.FromDTO(statusDTO);
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                            return null;
                        }
#if !DEBUG
                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
#endif
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new Modelo.StatusRetorno(0, String.Empty, FONTE);

                    //Conversão de DTO para modelo de serviço
                    var dados = EstornoTotalizador.FromDTO(dadosConsulta);

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

        #region [ Estorno - registros - BKWA2940 / WAC294 / WAAQ ]

        /// <summary>
        /// Consulta de registros do Relatório de Estornos.<br/>
        /// - Book BKWA2940 / Programa WAC294 / TranID WAAQ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2940 / Programa WAC294 / TranID WAAQ
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="saidaQuantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="tipoModalidade">Tipo da Modalidade da pesquisa</param>
        /// <param name="codigoTipoVenda">Código do tipo de venda da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Vendas - Crédito.</returns>
        private List<EstornoD> ConsultarEstorno(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 saidaQuantidadeTotalRegistros,
            DateTime dataInicial,
            DateTime dataFinal,
            Int16 tipoModalidade,
            Int16 codigoTipoVenda,
            List<Int32> pvs,
            out Modelo.StatusRetorno status)
        {
            List<EstornoD> retorno = new List<EstornoD>();

            using (Logger Log = Logger.IniciarLog("Estorno - BKWA2940"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    saidaQuantidadeTotalRegistros,
                    dataInicial,
                    dataFinal,
                    tipoModalidade,
                    codigoTipoVenda,
                    pvs
                });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);
#if DEBUG
                    //Execução da pesquisa mainframe
                    var dadosConsulta = BLL.Instancia.Consultar(
                        codigoTipoVenda,
                        pvs,
                        dataInicial,
                        dataFinal,
                        tipoModalidade,
                        ref rechamada,
                        out indicadorRechamada,
                        out statusDTO);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                    {
                        status = Modelo.StatusRetorno.FromDTO(statusDTO);
                        saidaQuantidadeTotalRegistros = 0;
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta, saidaQuantidadeTotalRegistros });
                        return null;
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = dadosConsulta;
#else
                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Estorno>(Cache.Extrato,
                        guidPesquisa, registroInicial, saidaQuantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.Consultar(
                            codigoTipoVenda,
                            pvs,
                            dataInicial,
                            dataFinal,
                            tipoModalidade,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = Modelo.StatusRetorno.FromDTO(statusDTO);
                            saidaQuantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta, saidaQuantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }
                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Estorno>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out saidaQuantidadeTotalRegistros);

#endif
                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new Modelo.StatusRetorno(0, String.Empty, FONTE);

                    var dados = Estorno.FromDTO(dadosCache);

                    retorno = dados.ConvertAll(x => (EstornoD)x);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });
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
            return retorno;
        }
        #endregion
    }
}
