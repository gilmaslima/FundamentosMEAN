using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.DadosIniciais
{
    [ToolboxItemAttribute(false)]
    public class DadosIniciais : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DadosCadastrais.SharePoint.WPAberto/DadosIniciais/DadosIniciaisUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);

            if (control!= null)
                ((DadosIniciaisUserControl)control).WebPartDadosIniciais = this;

            Controls.Add(control);
        }

        /// <summary>
        /// Atributo costumizado para verificar se a criação de acesso será para recuperar Usuário Legado
        /// </summary>
        [WebBrowsable(true),
        Category("Custom Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("Acesso Usuário Legado"),
        WebDescription("Indicador se a criação de acesso será para recuperar Usuário Legado. Caso não seja seguirá o processo de Criação de Novo Acesso")]
        public Boolean AcessoUsuarioLegado { get; set; }
    }
}
