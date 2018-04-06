using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarExtratoDirfResumoEnvio
    {
        [DataMember]
        public int NumeroEstabelecimento { get; set; }
        [DataMember]
        public string CnpjEstabelecimento { get; set; }
    }
}