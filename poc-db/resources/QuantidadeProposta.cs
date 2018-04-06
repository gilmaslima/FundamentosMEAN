using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class QuantidadeProposta
    {
        [DataMember]
        public int? QuantidadePVsAtivos { get; set; }

        [DataMember]
        public int? QuantidadePropostasPendentes { get; set; }

    }
}
