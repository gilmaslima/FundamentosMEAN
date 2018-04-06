using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class TotaisPorBandeiraSaldosEmAberto
    {
        [DataMember]
        public Decimal TotalLiquido { get; set; }
        [DataMember]
        public Int32 QuantidadeTotalBandeiras { get; set; }
        [DataMember]
        public List<BandeiraTotalSaldosEmAberto> TotaisBandeiras { get; set; }
    }
}