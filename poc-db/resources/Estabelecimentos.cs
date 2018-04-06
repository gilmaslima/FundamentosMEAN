/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.ComponentModel;
using System.Web.UI;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios.Estabelecimentos
{
    [ToolboxItemAttribute(false)]
    public class Estabelecimentos : UsuariosWebPartBase
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios/Estabelecimentos/EstabelecimentosUserControl.ascx";

        protected override void CreateChildControls()
        {
            EstabelecimentosUserControl control = (EstabelecimentosUserControl) Page.LoadControl(_ascxPath);

            //Repassa a informação se a página é de edição ou criação de usuários
            control.Modo = this.Modo;

            Controls.Add(control);
        }
    }
}
