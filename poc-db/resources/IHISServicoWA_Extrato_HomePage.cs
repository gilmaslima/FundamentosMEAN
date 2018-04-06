/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Interface do Serviço utilizado pela HomePage
    /// </summary>
    /// <remarks>
    /// <para>Books utilizados:</para>
    /// <para>- Book BKWA2470 / Programa WAC247 / Transação WAG4</para>
    /// <para>- Book BKWA2480 / Programa WAC248 / Transação WAG5</para>
    /// <para>- Book BKWA2490 / Programa WAC249 / Transação WAG6</para>
    /// <para>- Book BKWA2500 / Programa WAC250 / Transação WAG7</para>
    /// </remarks>
    [ServiceContract]
    public interface IHISServicoWA_Extrato_HomePage
    {
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
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.HomePage.VendasCredito ConsultarVendasCredito(
            Int16 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetorno status);

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
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.HomePage.VendasDebito ConsultarVendasDebito(
            Int16 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetorno status);

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
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.HomePage.LancamentosFuturos ConsultarLancamentosFuturos(
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetorno status);

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
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        Modelo.HomePage.ValoresPagos ConsultarValoresPagos(
           Int32 codigoBandeira,
           List<Int32> pvs,
           DateTime dataInicial,
           DateTime dataFinal,
           out StatusRetorno status);
    }
}