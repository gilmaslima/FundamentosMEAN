using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.QuadrosMenu
{
    [ToolboxItemAttribute(false)]
    public class QuadrosMenu : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/PN.DadosCadastrais.WP/QuadrosMenu/QuadrosMenuUserControl.ascx";

        protected override void CreateChildControls()
        {
            QuadrosMenuUserControl control = Page.LoadControl(_ascxPath) as QuadrosMenuUserControl;
            control.ValidarPermissao = false;
            Controls.Add(control);
        }
    }
}
