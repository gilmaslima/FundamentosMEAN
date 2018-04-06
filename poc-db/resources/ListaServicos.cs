using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Rede.PN.ZeroDolar.SharePoint.WebParts.ListaServicos
{
    [ToolboxItemAttribute(false)]
    public class ListaServicos : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string ascxPath = @"~/_CONTROLTEMPLATES/Rede.PN.ZeroDolar.Web/ListaServicos/ListaServicosUserControl.ascx";

        protected override void CreateChildControls()
        {
            ListaServicosUserControl objControl = Page.LoadControl(ascxPath) as ListaServicosUserControl;
            objControl.TermosCondicoesHtml = this.TermosCondicoesHtml;
            objControl.TermosCondicoesPdf = this.TermosCondicoesPdf;
            Controls.Add(objControl);
        }

        [Personalizable( PersonalizationScope.Shared),
        WebBrowsable(true),
        WebDisplayNameAttribute("Termos e Condições - Html"), 
        WebDescription("Caminho para arquivo html de termos e condições de serviços"), 
        CategoryAttribute("Termos e Condições")]
        public String TermosCondicoesHtml {
            get;
            set;
        }

        [Personalizable(PersonalizationScope.Shared), 
        WebBrowsable(true),
        WebDisplayNameAttribute("Termos e Condições - Pdf"),
        WebDescription("Caminho para arquivo pdf de termos e condições de serviços"),
        CategoryAttribute("Termos e Condições")]
        public String TermosCondicoesPdf {
            get;
            set;
        } 
    }
}
