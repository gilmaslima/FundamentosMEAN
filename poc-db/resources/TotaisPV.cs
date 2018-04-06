using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Emissores.Modelos
{
    public class TotaisPV
    {
        public Int32 TotalDomiciliados { get; set; }
        public Int32 TotalNaoDomiciliados { get; set; }
        public Int32 TotalCancelados { get; set; }
    }
}
