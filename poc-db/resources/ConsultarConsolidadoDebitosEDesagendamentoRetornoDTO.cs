using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class ConsultarConsolidadoDebitosEDesagendamentoRetornoDTO
    {
        public string ChavePesquisa { get; set; }
        public decimal ValorPendenteDebito { get; set; }
        public decimal ValorPendenteLiquido { get; set; }
        public decimal ValorPendente { get; set; }
        public decimal ValorLiquidadoDebito { get; set; }
        public decimal ValorLiquidadoLiquido { get; set; }
    }
}
