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
    /// Turquia - Total de vendas por bandeira
    /// </summary>
    public class TotalVendasPorBandeira : TotalVendasBase
    {
        /// <summary>
        /// Código da bandeira
        /// </summary>
        public Int16 CodigoBandeira { get; set; }

        /// <summary>
        /// Descrição da bandeira
        /// </summary>
        public String DescricaoBandeira { get; set; }

     }
}
