/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo.RelatorioValoresConsolidadosVendas
{
    /// <summary>
    /// Turquia - Total de vendas crédito por período retornando as bandeiras.
    /// </summary>
    public class TotalVendasCreditoPorPeriodoBandeira : TotalVendasBase
    {
        /// <summary>
        /// Lista de totais de vendas por bandeira
        /// </summary>
        public List<TotalVendasPorBandeira> ListaTotalVendasCreditoPorBandeira { get; set; }
    }
}
