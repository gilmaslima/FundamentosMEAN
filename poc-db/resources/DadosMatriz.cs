using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class DadosMatriz
    {
        [DataMember]
        public PontoDeVenda PontoDeVenda { get; set; }

        [DataMember]
        public List<DomicilioBancarioMatriz> DomicilioBancarioMatriz { get; set; }

        [DataMember]
        public List<Proprietario> Proprietarios { get; set; }
    }
}
