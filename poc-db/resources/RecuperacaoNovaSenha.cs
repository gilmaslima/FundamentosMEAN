using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.RecuperacaoNovaSenha
{
    [ToolboxItemAttribute(false)]
    public class RecuperacaoNovaSenha : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DadosCadastrais.SharePoint.WPAberto/RecuperacaoNovaSenha/RecuperacaoNovaSenhaUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            if (control != null)
            {
                ((RecuperacaoNovaSenhaUserControl)control).WebPartRecuperacaoNovaSenha = this;
            }
            Controls.Add(control);
        }

        /// <summary>
        /// Atributo costumizado para verificar se a identificação será para recuperar o Usuário
        /// </summary>
        [WebBrowsable(true),
        Category("Custom Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("Recuperação Senha"),
        WebDescription("Informa se o fluxo coresponde a recuperação de senha")]
        public bool RecuperacaoSenha { get; set; }

        
    }
}
