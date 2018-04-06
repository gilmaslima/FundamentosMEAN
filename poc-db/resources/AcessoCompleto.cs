using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.NovoAcesso.AcessoCompleto
{
    [ToolboxItemAttribute(false)]
    public class AcessoCompleto : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DadosCadastrais.SharePoint.WebParts.NovoAcesso/AcessoCompleto/AcessoCompletoUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            ((AcessoCompletoUserControl)control).ValidarPermissao = false;

            Controls.Add(control);
        }
    }
}
