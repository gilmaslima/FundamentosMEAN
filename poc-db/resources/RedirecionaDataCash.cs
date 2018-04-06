using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DataCash.SharePoint.WebParts.RedirecionaDataCash
{
    [ToolboxItemAttribute(false)]
    public class RedirecionaDataCash : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.DataCash.SharePoint.WebParts/RedirecionaDataCash/RedirecionaDataCashUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        /// <summary>
        /// Recupera o valor atribuído na WebPart
        /// </summary>
        /// <summary>
        [WebBrowsable(true)]
        [WebDisplayName("URL do DataCash")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [DefaultValue("")]
        public String URLDataCash
        {
            get;
            set;
        }


        /// <summary>
        /// 
        /// </summary>
        String largura = "100%";

        /// <summary>
        /// Largura do Iframe
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("Largura do Iframe")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [DefaultValue("100%")]
        public string Largura
        {
            get
            {
                return largura;
            }
            set
            {
                largura = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        String altura = "200px";

        /// <summary>
        /// Altura do Iframe
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("Altura do Iframe")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [DefaultValue("200px")]
        public string Altura
        {
            get
            {
                return altura;
            }
            set
            {
                altura = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        String scroll = "auto";

        /// <summary>
        /// Scroll do Iframe
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("Scroll do Iframe")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [DefaultValue("auto")]
        public String Scroll
        {
            get
            {
                return scroll;
            }
            set
            {
                scroll = value;
            }
        }
    }
}
