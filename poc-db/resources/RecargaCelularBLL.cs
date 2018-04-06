/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using AG = Redecard.PN.Extrato.Agentes.RecargaCelularAG;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Modelo.RecargaCelular;

namespace Redecard.PN.Extrato.Negocio
{
    /// <summary>
    /// Classe de negócio para consultas WA do Módulo Extrato - Relatório de Recarga de Celular - Detalhe
    /// </summary>
    /// <remarks>
    /// Books utilizados pelos métodos da classe:<br/>
    /// - Book BKWA2420 / Programa WA242 / TranID ISIB / Método ConsultarRecargaCelularDetalhe
    /// </remarks>
    public class RecargaCelularBLL : RegraDeNegocioBase
    {        
        /// <summary>
        /// Instância singleton da classe
        /// </summary>
        private static RecargaCelularBLL _Instancia;
        /// <summary>
        /// Instância singleton da classe
        /// </summary>
        public static RecargaCelularBLL Instancia { get { return _Instancia ?? (_Instancia = new RecargaCelularBLL()); } }

        /// <summary>
        /// Consulta para o Relatório de Detalhes de Recarga de Celular,
        /// através de pesquisa pelo PV / RV e Data Inicial
        /// </summary>
        /// <param name="pv">Número do PV</param>
        /// <param name="rv">Número do Resumo de Vendas</param>
        /// <param name="dataInicial">Data inicial da pesquisa</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Lista de registros da consulta</returns>
        public List<RecargaCelularDetalhe> ConsultarRecargaCelularDetalhe(
            Int32 pv,            
            DateTime dataInicial,
            Int32 rv,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            return this.ConsultarRecargaCelularDetalhe(
                pv, rv, dataInicial, default(DateTime), 1, 
                ref rechamada, out indicadorRechamada, out status);
        }

        /// <summary>
        /// Consulta para o Relatório de Detalhes de Recarga de Celular,
        /// através de pesquisa pelo PV, Data Inicial e Data Final
        /// </summary>
        /// <param name="pv">Número do PV</param>
        /// <param name="dataInicial">Data inicial da pesquisa</param>
        /// <param name="dataFinal">Data final da pesquisa</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Lista de registros da consulta</returns>
        public List<RecargaCelularDetalhe> ConsultarRecargaCelularDetalhe(
            Int32 pv,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            return this.ConsultarRecargaCelularDetalhe(
                pv, default(Int32), dataInicial, dataFinal, 2,
                ref rechamada, out indicadorRechamada, out status);
        }

        /// <summary>
        /// Consulta para o Relatório de Detalhes de Recarga de Celular
        /// </summary>
        /// <param name="pv">Número do PV</param>
        /// <param name="rv">Número do Resumo de Vendas</param>
        /// <param name="dataInicial">Data inicial da pesquisa</param>
        /// <param name="dataFinal">Data final da pesquisa</param>
        /// <param name="opcaoPesquisa">
        /// Opção de pesquisa:<br/>
        ///     - 1: PV / RV / Data Inicial<br/>
        ///     - 2: PV / Data Inicial / Data Final
        /// </param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Lista de registros da consulta</returns>
        public List<RecargaCelularDetalhe> ConsultarRecargaCelularDetalhe(
            Int32 pv,
            Int32 rv,
            DateTime dataInicial,
            DateTime dataFinal,
            Int16 opcaoPesquisa,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Recarga Celular - Detalhe"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, 
                        new { pv, rv, dataInicial, dataFinal, opcaoPesquisa, rechamada });

                    List<RecargaCelularDetalhe> retorno = AG.Instancia.ConsultarRecargaCelularDetalhe(
                        pv,
                        rv,
                        dataInicial,
                        dataFinal,
                        opcaoPesquisa,
                        ref rechamada,
                        out indicadorRechamada,
                        out status);

                    //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                    if (retorno == null || retorno.Count == 0)
                        indicadorRechamada = false;
                    
#if DEBUG
                    for (Int32 i = 0; i < new Random().Next(100); i++)
                    {
                        retorno.Add(retorno[0].ObterCopia());
                        if(retorno.Last().DataHoraTransacao.HasValue)
                            retorno.Last().DataHoraTransacao = retorno.Last().DataHoraTransacao.Value.AddMinutes(1);
                    }
#endif

                    Log.GravarLog(EventoLog.RetornoAgente, new { retorno, rechamada, indicadorRechamada, status });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }                
            }
        }
    }
}
