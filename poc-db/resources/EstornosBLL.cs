/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Linq;
using System.Collections.Generic;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Modelo.Estornos;
using AG = Redecard.PN.Extrato.Agentes.EstornoAG;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Redecard.PN.Extrato.Negocio
{
    /// <summary>
    /// Extrato - Relatório de Estorno
    /// </summary>
    public class EstornosBLL : RegraDeNegocioBase
    {
        private static EstornosBLL instancia;

        public static EstornosBLL Instancia { get { return instancia ?? (instancia = new EstornosBLL()); } }

        /// <summary>
        /// Consultar
        /// </summary>
        /// <param name="tipoVenda"></param>
        /// <param name="pvs"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="tipoModalidade">Tipo da Modalidade da pesquisa</param>
        /// <param name="rechamada"></param>
        /// <param name="indicadorRechamada"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<Estorno> Consultar(
            Int16 tipoVenda,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            Int16 tipoModalidade,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Estorno - WAC294"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { tipoVenda, pvs, dataInicial, dataFinal, tipoModalidade, rechamada });

                    var retorno = AG.Instancia.Consultar(
                        tipoVenda,
                        pvs,
                        dataInicial,
                        dataFinal,
                        tipoModalidade,
                        ref rechamada,
                        out indicadorRechamada,
                        out status);

                    //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                    if (retorno == null || retorno.Count == 0)
                        indicadorRechamada = false;

                    Log.GravarLog(EventoLog.RetornoAgente, new { retorno, rechamada, indicadorRechamada, status });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// ConsultarTotalizadores
        /// </summary>
        /// <param name="pvs"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="tipoModalidade">Tipo da Modalidade da pesquisa</param>
        /// <param name="codigoTipoVenda"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public EstornoTotalizador ConsultarTotalizadores(
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            Int16 tipoModalidade,
            Int16 codigoTipoVenda,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Estorno - Totalizadores - WAC293"))
            {
                Log.GravarLog(EventoLog.ChamadaAgente, new { pvs, dataInicial, dataFinal, tipoModalidade, codigoTipoVenda });

                try
                {
                    //Realiza pesquisa para o período desejado
                    EstornoTotalizador retorno = AG.Instancia.ConsultarTotalizadores(
                        codigoTipoVenda,
                        pvs,
                        dataInicial,
                        dataFinal,
                        tipoModalidade,
                        out status);

                    //Se consulta foi realizada com sucesso, adiciona resultado na lista thread-safe
                    if (status.CodigoRetorno != 0 && status.CodigoRetorno != 60)
                        return new EstornoTotalizador();

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw;
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