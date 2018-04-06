using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class CodigoDescricaoPvsDuplicados
    {
        [DataMember]
        public int? CodErro { get; set; }

        [DataMember]
        public string DescricaoErro { get; set; }

        [DataMember]
        public string PvsDuplicados { get; set; }
    }
}
