/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.MeuUsuario.AlteracaoEmail
{
    [ToolboxItemAttribute(false)]
    public class AlteracaoEmail : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DadosCadastrais.SharePoint.WebParts.MeuUsuario/AlteracaoEmail/AlteracaoEmailUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
