using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redecard.PN.Extrato.Modelo
{
    public class TotalMesSaldosEmAbertoDTO : BasicDTO
    {
        public Decimal ValorLiquido { get; set; }
        public DateTime DataReferencia { get; set; }
    }
}