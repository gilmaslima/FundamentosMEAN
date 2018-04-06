/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.MeuUsuario.MeuUsuario
{
    [ToolboxItemAttribute(false)]
    public class MeuUsuario : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DadosCadastrais.SharePoint.WebParts.MeuUsuario/MeuUsuario/MeuUsuarioUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
