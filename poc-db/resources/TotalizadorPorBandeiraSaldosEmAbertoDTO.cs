using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    public class TotalizadorPorBandeiraSaldosEmAbertoDTO
    {
        public List<TotalPorBandeiraSaldosEmAbertoDTO> TotaisBandeira { get; set; }
        public decimal ValorLiquido { get; set; }
        public Int16 QuantidadeBandeiras { get; set; }
    }
}
