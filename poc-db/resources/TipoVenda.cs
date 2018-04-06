using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Rede.PN.Cancelamento.Sharepoint.Modelos
{
    /// <summary>
    /// Tipo de Venda
    /// </summary>
    [Serializable]
    public enum TipoVenda
    {
        [Description("Crédito")]
        Credito,
        [Description("Débito")]
        Debito
        //[Description("Crédito (À vista)")]
        //CreditoAVista,
        //[Description("Crédito (Parc Estab)")]
        //CreditoParceladoEstabelecimento,
        //[Description("Crédito (Parc Emissor)")]
        //CreditoParceladoEmissor,
        //[Description("Débito (À vista)")]
        //DebitoAVista,
        //[Description("Débito (Pré data)")]
        //DebitoPreDatado
    }
}
