using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public enum TipoOperacao
    {
        [EnumMember]
        Comercial = 'C',

        [EnumMember]
        Credito = 1,

        [EnumMember]
        Debito = 3,

        [EnumMember]
        Construcard = 4,

        [EnumMember]
        Voucher = 5
    }
    
}
