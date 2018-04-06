using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios.RejeicaoAcesso
{
    [ToolboxItemAttribute(false)]
    public class RejeicaoAcesso : UsuariosWebPartBase
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios/RejeicaoAcesso/RejeicaoAcessoUserControl.ascx";

        protected override void CreateChildControls()
        {
            RejeicaoAcessoUserControl control = (RejeicaoAcessoUserControl)Page.LoadControl(_ascxPath);

            //Repassa a informação se a página é de edição ou criação de usuários
            control.Modo = this.Modo;

            Controls.Add(control);
        }
    }
}
