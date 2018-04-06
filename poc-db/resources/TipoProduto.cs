using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;


namespace Rede.PN.Credenciamento.Modelo
{
    [DataContract]
    [Serializable]
    public enum TipoProduto
    {
        [Description("Desconhecido")]
        [EnumMember]
        Desconhecido = 0,
        [Description("Crédito")]
        [EnumMember]
        Credito = 1,
        [Description("Débito")]
        [EnumMember]
        Debito = 2,
        [Description("Construcard")]
        [EnumMember]
        Construcard = 3,
        [Description("Voucher")]
        [EnumMember]
        Voucher = 4

    }
}
