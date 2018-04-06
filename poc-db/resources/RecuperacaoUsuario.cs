using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.RecuperacaoUsuario
{
    [ToolboxItemAttribute(false)]
    public class RecuperacaoUsuario : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DadosCadastrais.SharePoint.WPAberto/RecuperacaoUsuario/RecuperacaoUsuarioUserControl.ascx";

        /// <summary>
        /// 
        /// </summary>
        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(ascxPath);
            Controls.Add(control);
        }
    }
}
