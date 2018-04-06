using System.ComponentModel;

namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
	public enum ConsultaPvTipoAssociacao
    {
        [Description("Próprio")]
        Proprio = 0,
        [Description("Centralizados")]
        Centralizados = 1,
        [Description("Filiais")]
        Filiais = 2,
        [Description("Consignados")]
        Consignados = 3,
        [Description("Mesmo CNPJ")]
        MesmoCNPJ = 4
    }
}
