using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.RAV.Sharepoint.WebParts.AcessoSenha
{
    [ToolboxItemAttribute(false)]
    public class AcessoSenha : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.RAV.Sharepoint.WebParts/AcessoSenha/AcessoSenhaUserControl.ascx";
        
        /// <summary>
        /// Ignora qualquer senha informada na tela
        /// </summary>
        private Boolean ignoraSenha;

        /// <summary>
        /// Redireciona automaticamente para páginas logadas
        /// </summary>
        private Boolean redirecionaAutomatico;

        /// <summary>
        /// Ignora qualquer senha informada na tela (valor informado pelo Sharepoint)
        /// </summary>
        [Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Ignorar senha"),
        WebDescription("Ignora a senha informada não submetendo para validação"),
        Category("Configurações RAV"),
        DefaultValue(false)]
        public Boolean IgnoraSenha
        {
            get
            {
                return ignoraSenha;
            }
            set
            {
                ignoraSenha = value;
            }
        }

        /// <summary>
        /// Redireciona automaticamente para páginas logadas (valor informado pelo Sharepoint)
        /// </summary>
        [Personalizable(PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayName("Redirecionar sem validar senha"),
        WebDescription("Redireciona a tela sem submeter a senha para validação"),
        Category("Configurações RAV"),
        DefaultValue(false)]
        public Boolean RedirecionaAutomatico
        {
            get
            {
                return redirecionaAutomatico;
            }
            set
            {
                redirecionaAutomatico = value;
            }
        }

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(ascxPath);
            Controls.Add(control);
        }
    }
}
