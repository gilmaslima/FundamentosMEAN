using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class ContaCorrente
    {
        [DataMember]
        public Modelo.TipoDomicilioBancario TipoDomicilioBancario { get; set; }

        [DataMember]
        public Int64 CodigoContaCorrente { get; set; }

        [DataMember]
        public Boolean ContaCorrenteValida { get; set; }

    }
}
