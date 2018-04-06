using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{

    [DataContract]
    [Serializable]
    public enum TipoPessoa
    {
        [EnumMember]
        Desconhecido = 0,
        [EnumMember]
        Fisica = 'F',
        [EnumMember]
        Juridica = 'J'
    }

}
