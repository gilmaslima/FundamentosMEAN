using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.PassosNovo
{
    [ToolboxItemAttribute(false)]
    public class PassosNovo : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DadosCadastrais.SharePoint.WPAberto/PassosNovo/PassosNovoUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(ascxPath);
            if (control != null)
            {
                ((PassosNovoUserControl)control).WebPartPassos = this;
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
