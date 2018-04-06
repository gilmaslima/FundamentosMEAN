using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1107 - Home - Últimas Vendas.
    /// </summary>
    public class ConsultarTransacoesCreditoDebitoRetornoDTO
    {
        public DateTime Data { get; set; }
        public decimal ValorCredito { get; set; }
        public decimal ValorDebito { get; set; }
    }
}
