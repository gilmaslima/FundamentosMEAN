using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.SolicitarMateriais
{
    [ToolboxItemAttribute(false)]
    public class SolicitarMateriais : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string ascxPath = @"~/_CONTROLTEMPLATES/PN.OutrosServicos.WP/SolicitarMateriais/SolicitarMateriaisUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(ascxPath);
            Controls.Add(control);
        }
    }
}
