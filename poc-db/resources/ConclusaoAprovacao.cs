using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios.ConclusaoAprovacao
{
    [ToolboxItemAttribute(false)]
    public class ConclusaoAprovacao : UsuariosWebPartBase
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios/ConclusaoAprovacao/ConclusaoAprovacaoUserControl.ascx";

        protected override void CreateChildControls()
        {
            ConclusaoAprovacaoUserControl control = (ConclusaoAprovacaoUserControl)Page.LoadControl(_ascxPath);
            control.Modo = this.Modo;

            Controls.Add(control);
        }
    }
}
