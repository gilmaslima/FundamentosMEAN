using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class BandeiraTotalSaldosEmAberto
    {
        [DataMember]
        public Int16 CodigoBandeira { get; set; }
        [DataMember]
        public String DescricaoBandeira { get; set; }
        [DataMember]
        public Decimal TotalBandeira { get; set; }
    }
}