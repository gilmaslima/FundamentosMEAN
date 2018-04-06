using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.Extrato.SharePoint.WebParts.ConsultarVendas.ConsultarVendasFiltro
{
    [ToolboxItemAttribute(false)]
    public class ConsultarVendasFiltro : WebPart
    {
        private const string ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.Extrato/ConsultarVendasFiltroUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(ascxPath);
            Controls.Add(control);
        }
    }
}
