using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace Redecard.Portal.Aberto.WebParts.FormularioImprensa
{
    [ToolboxItemAttribute(false)]
    public class FormularioImprensa : System.Web.UI.WebControls.WebParts.WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/FormularioImprensa/FormularioImprensaUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        private String de = "faleconosco@redecard.com.br";
        private String para = "imprensa@itau-unibanco.com.br";
        private String assunto = "imprensa@itau-unibanco.com.br";

        /// <summary>
        /// Dicas por página
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("De")]
        [Description("Emissor do formulário")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Emissor do formulário")]
        public String De
        {
            get { return this.de; }
            set { this.de = value; }
        }

        /// <summary>
        /// Dicas por página
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Para")]
        [Description("Destinatário do formulário")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Destinatário do formulário")]
        public String Para
        {
            get { return this.para; }
            set { this.para = value; }
        }

        /// <summary>
        /// Dicas por página
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Assunto")]
        [Description("Assunto do e-mail")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Assunto do e-mail")]
        public String Assunto
        {
            get { return this.assunto; }
            set { this.assunto = value; }
        }
    }
}
