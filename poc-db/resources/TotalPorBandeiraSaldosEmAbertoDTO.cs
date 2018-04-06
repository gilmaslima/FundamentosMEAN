using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redecard.PN.Extrato.Modelo
{
    public class TotalPorBandeiraSaldosEmAbertoDTO
    {
        public Int16 CodigoBandeira { get; set; }
        public String DescricaoBandeira { get; set; }
        public Decimal TotalBandeira { get; set; }
    }
}