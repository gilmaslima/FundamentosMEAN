using System.ComponentModel;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios.Estabelecimentos
{
    [DataContract]
    public enum TipoEstabelecimento
    {
        [EnumMemberAttribute,
        Description("Matriz")]
        Proprio = 0,

        [EnumMemberAttribute,
        Description("Centralizado")]
        Centralizados = 1,

        [EnumMemberAttribute,
        Description("Filial")]
        Filiais = 2,

        [EnumMemberAttribute,
        Description("Consignado")]
        Consignados = 3,

        [EnumMemberAttribute,
        Description("Mesmo CNPJ")]
        MesmoCnpj = 4
    }
}
