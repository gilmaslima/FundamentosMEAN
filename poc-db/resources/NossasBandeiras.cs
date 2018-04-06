using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.Portal.Aberto.WebParts.NossasBandeiras {
    [ToolboxItemAttribute(false)]
    public class NossasBandeiras : WebPart {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/NossasBandeiras/NossasBandeirasUserControl.ascx";

        /// <summary>
        /// 
        /// </summary>
        private string sTitle = "Nossas Bandeiras";

        /// <summary>
        /// 
        /// </summary>
        private string sLink = "/pt-br/conhecaredecard/Paginas/nossasbandeiras.aspx";

        /// <summary>
        /// 
        /// </summary>
        private string sLinkDescription = "Conheça Todas";

        /// <summary>
        /// 
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Título da Web Part")]
        [Description("Define o título da web part.")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public string WpTitle {
            get { return sTitle; }
            set { sTitle = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Link")]
        [Description("Define o link do botão de acesso as bandeiras.")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public string Link {
            get { return sLink; }
            set { sLink = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Descrição do Link")]
        [Description("Define o a descrição do link de acesso as bandeiras.")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public string LinkDescription {
            get { return sLinkDescription; }
            set { sLinkDescription = value; }
        }

        protected override void CreateChildControls() {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
