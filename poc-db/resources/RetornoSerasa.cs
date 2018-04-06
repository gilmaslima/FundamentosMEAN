using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public enum RetornoSerasa
    {
        [EnumMember]
        Sim,
        [EnumMember]
        Nao,
    }
}
