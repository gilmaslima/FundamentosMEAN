using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Emissores.Sharepoint
{
    public class DetalheFilho
    {
        public String CpfCNPJ { get; set; }
        public Int32 NumeroPV { get; set; }
        public Int32 Agencia { get; set; }
        public Int32 ContaCorrente { get; set; }
        public String PeriodoTrava { get; set; }
        public decimal ValorLiquido { get; set; }
        public decimal ValorFinal { get; set; }
    }
}
