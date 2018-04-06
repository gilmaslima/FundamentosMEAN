using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.ConsultarOfertas
{
    [ToolboxItemAttribute(false)]
    public class ConsultarOfertas : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/PN.OutrosServicos.WP/ConsultarOfertas/ConsultarOfertasUserControl.ascx";

        /// <summary>
        /// Modo de funcionamento da WebPart - Tipo da Oferta
        /// </summary>
        [WebBrowsable(true),
        Category("Custom Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("TipoOferta"),
        WebDescription("Tipo da Oferta ([vazio], Conta Certa, Bônus Rede")]
        public String TipoOferta { get; set; }

        protected override void CreateChildControls()
        {
            var control = (ConsultarOfertasUserControl)Page.LoadControl(_ascxPath);
            control.TipoOferta = this.TipoOferta;
            Controls.Add(control);
        }
    }
}
