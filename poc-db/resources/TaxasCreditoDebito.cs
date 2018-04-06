using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Rede.PN.CondicaoComercial.SharePoint.WebParts.TaxasCreditoDebito
{
    [ToolboxItemAttribute(false)]
    public class TaxasCreditoDebito : WebPart
    {
        private const string ascxPath = @"~/_CONTROLTEMPLATES/Rede.PN.CondicaoComercial/TaxasCreditoDebito/TaxasCreditoDebitoUserControl.ascx";
        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(ascxPath);
            Controls.Add(control);
        }
    }
}
