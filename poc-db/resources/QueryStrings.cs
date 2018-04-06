/*
© Copyright 2017 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.ComponentModel;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Enums
{
    /// <summary>
    /// Classe criada para armazenar as constantes dos identificadores
    /// das querystrings criptografadas
    /// </summary>
    public enum QueryStrings
    {
        [Description("")]
        Nenhum,

        /// <summary>
        /// Extrato - Lançamentos Futuros Crédito
        /// </summary>
        [Description("extrato.lancamentosFuturos.credito")]
        ExtratoLancamentosFuturosCredito,

        /// <summary>
        /// Extrato - Lançamentos Futuros Débito
        /// </summary>
        [Description("extrato.lancamentosFuturos.debito")]
        ExtratoLancamentosFuturosDebito,

        /// <summary>
        /// Extrato - Vendas Crédito
        /// </summary>
        [Description("extrato.vendas.credito")]
        ExtratoVendasCredito,

        /// <summary>
        /// Extrato - Vendas Débito
        /// </summary>
        [Description("extrato.vendas.debito")]
        ExtratoVendasDebito,

        /// <summary>
        /// Extrato - Valores Pagos Crédito
        /// </summary>
        [Description("extrato.valoresPagos.credito")]
        ExtratoValoresPagosCredito,

        /// <summary>
        /// Extrato - Valores Pagos Débito
        /// </summary>
        [Description("extrato.valoresPagos.debito")]
        ExtratoValoresPagosDebito,

        /// <summary>
        /// DIRF
        /// </summary>
        [Description("dirf")]
        Dirf,

        /// <summary>
        /// Consulta de Vendas
        /// </summary>
        [Description("consultaVendas")]
        ConsultaVendas
    }
}