/*
© Copyright 2015 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

namespace Redecard.PN.Extrato.Modelo.ResumoVendas
{
    /// <summary>
    /// Tipo da Pesquisa dos totais do resumo de vendas de recarga de celular.
    /// Indica a origem da consulta.
    /// </summary>
    public enum RecargaCelularResumoOrigem : short
    {
        /// <summary>
        /// Origem é o Relatorio de Pagamentos Ajustados
        /// </summary>
        PagamentosAjustados = 1,

        /// <summary>
        /// Origem é o Resumo de Vendas de Recarga de Celular
        /// </summary>
        ResumoVendas = 2
    }
}