using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{

    [DataContract]
    [Serializable]
    public enum LocalPagamento
    {
        [EnumMember]
        ESTABELECIMENTO = 1,
        [EnumMember]
        CENTRALIZADO = 2
    }

}
