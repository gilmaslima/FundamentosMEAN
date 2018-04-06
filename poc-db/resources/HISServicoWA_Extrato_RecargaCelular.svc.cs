/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.Recarga;
using BLL = Redecard.PN.Extrato.Negocio.RecargaCelularBLL;
using DTO = Redecard.PN.Extrato.Modelo.RecargaCelular;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Recarga
    /// de Celular - Detalhes.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book BKWA2420 / Programa WA242 / TranID ISIB / Método ConsultarRecargaCelularDetalhe
    /// </remarks>
    public class HISServicoWA_Extrato_RecargaCelular : ServicoBaseExtrato, IHISServicoWA_Extrato_RecargaCelular
    {
        #region [ Recarga de Celular - Detalhes - BKWA2420 / WA242 / ISIB ]

        /// <summary>
        /// Consulta utilizada no Relatório de Recarga de Celular - Detalhes,
        /// com pesquisa através do Resumo de Vendas.<br/>
        /// - Book BKWA2420 / Programa WA242 / TranID ISIB
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="rv">Número do resumo de venda</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Recarga de Celular - Detalhes.</returns>
        public List<RecargaCelularDetalhe> ConsultarRecargaCelularDetalhePorResumo(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicial,
            Int32 pv,
            Int32 rv,
            out StatusRetorno status)
        {
            //Chama a consulta genérica, passando os dados para pesquisa pelo Resumo de Vendas
            return this.ConsultarRecargaCelularDetalhe(
                guidPesquisa, registroInicial, quantidadeRegistros, ref quantidadeTotalRegistros,
                dataInicial, null, pv, rv, out status);
        }

        /// <summary>
        /// Consulta utilizada no Relatório de Recarga de Celular - Detalhes,
        /// com pesquisa através do Período.<br/>
        /// - Book BKWA2420 / Programa WA242 / TranID ISIB
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Recarga de Celular - Detalhes.</returns>
        public List<RecargaCelularDetalhe> ConsultarRecargaCelularDetalhePorPeriodo(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicial,
            DateTime dataFinal,
            Int32 pv,
            out StatusRetorno status)
        {
            //Chama a consulta genérica, passando os dados para pesquisa pelo Período
            return this.ConsultarRecargaCelularDetalhe(
                guidPesquisa, registroInicial, quantidadeRegistros, ref quantidadeTotalRegistros,
                dataInicial, dataFinal, pv, null, out status);
        }

        /// <summary>
        /// Consulta utilizada no Relatório de Recarga de Celular - Detalhes.<br/>
        /// - Book BKWA2420 / Programa WA242 / TranID ISIB
        /// </summary>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="rv">Número do resumo de venda</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Recarga de Celular - Detalhes.</returns>
        public List<RecargaCelularDetalhe> ConsultarRecargaCelularDetalhe(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicial,
            DateTime? dataFinal,
            Int32 pv,
            Int32? rv,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Recarga de Celular - Detalhe"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, 
                    quantidadeRegistros, dataInicial, dataFinal, pv, rv });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.RecargaCelularDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        var dadosConsulta = default(List<DTO.RecargaCelularDetalhe>);

                        //Execução da pesquisa mainframe
                        if (dataFinal.HasValue && !rv.HasValue)
                        {
                            dadosConsulta = BLL.Instancia.ConsultarRecargaCelularDetalhe(
                                pv,
                                dataInicial,
                                dataFinal.Value,
                                ref rechamada,
                                out indicadorRechamada,
                                out statusDTO);
                        }
                        else if (rv.HasValue && !dataFinal.HasValue)
                        {
                            dadosConsulta = BLL.Instancia.ConsultarRecargaCelularDetalhe(
                                pv,
                                dataInicial,
                                rv.Value,
                                ref rechamada,
                                out indicadorRechamada,
                                out statusDTO);
                        }
                        else if (rv.HasValue && dataFinal.HasValue)
                            dadosConsulta = BLL.Instancia.ConsultarRecargaCelularDetalhe(
                                pv,
                                rv.Value,
                                dataInicial,
                                dataFinal.Value,
                                1,
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
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.RecargaCelularDetalhe>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    var dados = RecargaCelularDetalhe.FromDTO(dadosCache);

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
