using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Emissores.Sharepoint
{
    public class DetalheTrava
    {
        public Int32 NumeroPV { get; set; }
        public String CpfCnpj { get; set; }
        public Int32 Agencia { get; set; }
        public String Conta { get; set; }
        public String DiasTrava { get; set; }
        public String PeriodoTrava { get; set; }
        public Decimal ValorLiquido { get; set; }
        public Decimal ValorCobranca { get; set; }
        public String TipoConta { get; set; }

    }
}
