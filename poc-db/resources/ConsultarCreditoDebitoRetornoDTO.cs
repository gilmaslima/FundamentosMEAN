using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA1108 - Home - Lançamentos futuros.
    /// </summary>
    public class ConsultarCreditoDebitoRetornoDTO
    {
        public DateTime Data { get; set; }
        public decimal ValorCredito { get; set; }
        public decimal ValorDebito { get; set; }
    }
}
