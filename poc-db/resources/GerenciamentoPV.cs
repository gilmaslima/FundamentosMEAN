using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.DataCash.Servicos
{
    [DataContract]
    public class GerenciamentoPV : MensagemErro
    {
        [DataMember]
        public Int32 PV { get; set; }

        [DataMember]
        public List<String> Ips { get; set; }

        [DataMember]
        public String UrlExpirado { get; set; }

        [DataMember]
        public String UrlHps { get; set; }

        [DataMember]
        public String UrlOffLine { get; set; }
    }
}