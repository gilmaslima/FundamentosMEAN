using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public enum TipoEndereco
    {
        [EnumMember]
        Comercial = 1,
        [EnumMember]
        Correspondência = 2,
        [EnumMember]
        Instalação = 4
    }
}
