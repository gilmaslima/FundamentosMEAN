using System;
using System.ComponentModel;

namespace Rede.PN.MultivanAlelo.Core.Web.Controles.Portal
{
	[Flags]
    public enum ConsultaPvTipoEntidade
    {
        [Description("0")]
        Proprio = 1,
        [Description("1")]
        Centralizados = 2,
        [Description("2")]
        Filiais = 4,
        [Description("3")]
        Consignados = 8,
        [Description("4")]
        MesmoCNPJ = 16
    }
}
