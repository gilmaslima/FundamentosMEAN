using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.RAV.Servicos
{
    [DataContract]
    public class ModRAVAutomaticoBandeira
    {
        [DataMember]
        public string IndSel { get; set; }

        [DataMember]
        public short CodBandeira { get; set; }

        [DataMember]
        public string DscBandeira { get; set; }
    }
}