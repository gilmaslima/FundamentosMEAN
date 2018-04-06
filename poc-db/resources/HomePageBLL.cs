/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Modelo.HomePage;
using AG = Redecard.PN.Extrato.Agentes.HomePageAG;

namespace Redecard.PN.Extrato.Negocio
{
    /// <summary>
    /// Classe de Negócio da HomePage
    /// </summary>
    public class HomePageBLL : RegraDeNegocioBase
    {
        /// <summary>
        /// Instância singleton
        /// </summary>
        private static HomePageBLL instancia;

        /// <summary>
        /// Instância singleton
        /// </summary>
        public static HomePageBLL Instancia { get { return instancia ?? (instancia = new HomePageBLL()); } }

        /// <summary>
        /// Consulta de Vendas a Crédito utilizado na HomePage, consolidados por Bandeira
        /// </summary>
        /// <remarks>
        /// <para>Books utilizados:</para>
        /// <para>- Book BKWA2470 / Programa WAC247 / Transação WAG4</para>
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="pvs">Lista de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Vendas a Crédito</returns>
        public VendasCredito ConsultarVendasCredito(
            Int16 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Vendas Crédito - BKWA2470"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pvs, dataInicial, dataFinal });

                    var retorno = AG.Instancia.ConsultarVendasCredito(
                        codigoBandeira, pvs, dataInicial, dataFinal, out status);

                    Log.GravarLog(EventoLog.RetornoAgente, new { retorno, status });

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

        /// <summary>
        /// Consulta de Vendas a Débito utilizado na HomePage, consolidados por Bandeira
        /// </summary>
        /// <remarks>
        /// <para>Books utilizados:</para>
        /// <para>- Book BKWA2480 / Programa WAC248 / Transação WAG5</para>
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="pvs">Lista de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Vendas a Débito</returns>
        public VendasDebito ConsultarVendasDebito(
           Int16 codigoBandeira,
           List<Int32> pvs,
           DateTime dataInicial,
           DateTime dataFinal,
           out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Vendas Débito - BKWA2480"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pvs, dataInicial, dataFinal });

                    var retorno = AG.Instancia.ConsultarVendasDebito(
                        codigoBandeira, pvs, dataInicial, dataFinal, out status);

                    Log.GravarLog(EventoLog.RetornoAgente, new { retorno, status });

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

        /// <summary>
        /// Consulta de Lançamentos Futuros utilizado na HomePage, consolidados por Data de Recebimento
        /// </summary>
        /// <remarks>
        /// <para>Books utilizados:</para>
        /// <para>- Book BKWA2490 / Programa WAC249 / Transação WAG6</para>
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="pvs">Lista de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Lançamentos Futuros</returns>
        public LancamentosFuturos ConsultarLancamentosFuturos(
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Lançamentos Futuros - BKWA2490"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pvs, dataInicial, dataFinal });

                    var retorno = AG.Instancia.ConsultarLancamentosFuturos(
                        codigoBandeira, pvs, dataInicial, dataFinal, out status);

                    Log.GravarLog(EventoLog.RetornoAgente, new { retorno, status });

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

        /// <summary>
        /// Consulta de Valores Pagos utilizado na HomePage, consolidados por Data de Recebimento
        /// </summary>
        /// <remarks>
        /// <para>Books utilizados:</para>
        /// <para>- Book BKWA2500 / Programa WAC250 / Transação WAG7</para>
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="pvs">Lista de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Valores Pagos</returns>
        public ValoresPagos ConsultarValoresPagos(
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Valores Pagos - BKWA2500"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pvs, dataInicial, dataFinal });

                    var retorno = AG.Instancia.ConsultarValoresPagos(
                        codigoBandeira, pvs, dataInicial, dataFinal, out status);

                    Log.GravarLog(EventoLog.RetornoAgente, new { retorno, status });

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