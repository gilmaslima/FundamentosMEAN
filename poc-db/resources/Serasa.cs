using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public class Serasa
    {
        [DataMember]
        public TipoPessoa TipoPessoa { get; set; }

        [DataMember]
        public Int64 NumeroCnpjCpf { get; set; }
        
        [DataMember]
        public RetornoSerasa RetornoSerasa { get; set; }

        [DataMember]
        public Empresa Empresa { get; set; }

        [DataMember]
        public Endereco Endereco { get; set; }

        [DataMember]
        public ICollection<Proprietario> Proprietarios { get; set; }
        
        [DataMember]
        public Cnae Cnae { get; set; }

    }
}
