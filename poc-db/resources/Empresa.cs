using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class Empresa
    {
        [DataMember]
        public string RazaoSocial { get; set; }

        [DataMember]
        public DateTime DataFundacao { get; set; }

        [DataMember]
        public Int32 NumeroDdd { get; set; }

        [DataMember]
        public Int32 NumeroTelefone { get; set; }

    }
}
