using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.Passos
{
    [ToolboxItemAttribute(false)]
    public class Passos : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DadosCadastrais.SharePoint.WPAberto/Passos/PassosUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            if (control != null)
            {
                ((PassosUserControl)control).WebPartPassos = this;
            }
            Controls.Add(control);
        }

        [WebBrowsable(true),
        Category("Custom Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("Passos"),
        WebDescription("String com a descrição dos passos separados por ';'")]
        public String PassosDescricao { get; set; }

        [WebBrowsable(true),
        Category("Custom Properties"),
        Personalizable(PersonalizationScope.Shared),
        WebDisplayName("Passo Ativo"),
        WebDescription("Número do passo ativo"),
        DefaultValue(0)]
        public String PassoAtivo { get; set; }

    }
}
