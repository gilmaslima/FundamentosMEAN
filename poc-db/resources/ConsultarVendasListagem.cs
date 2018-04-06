using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.Extrato.SharePoint.WebParts.ConsultarVendas.ConsultarVendasListagem
{
    [ToolboxItemAttribute(false)]
    public class ConsultarVendasListagem : WebPart
    {
        private const string ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.Extrato/ConsultarVendasListagemUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(ascxPath);
            Controls.Add(control);
        }
    }
}
