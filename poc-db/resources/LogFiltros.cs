using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Servicos
{
    [DataContract]
    public class LogFiltros
    {
        [DataMember]
        public Dictionary<Int32, String> Assemblies { get; set; }

        [DataMember]
        public Dictionary<Int32, String> Severidades { get; set; }

        [DataMember]
        public List<String> Classes { get; set; }

        [DataMember]
        public List<String> Metodos { get; set; }

        [DataMember]
        public List<String> Servidores { get; set; }
    }
}