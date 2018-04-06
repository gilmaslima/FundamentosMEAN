using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.Portal.Aberto.WebParts.RedecardIndicadorFinanceiro
{
    [ToolboxItemAttribute(false)]
    public class RedecardIndicadorFinanceiro : WebPart
    {

        #region Enumeração____________________

        #endregion

        #region Constantes____________________

        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/RedecardIndicadorFinanceiro/RedecardIndicadorFinanceiroUserControl.ascx";

        #endregion

        #region Propriedades__________________

        /// <summary>
        /// URL do XML
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("URL do XML")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configuracoes")]
        [DefaultValue("http://win-4ldjrn7nnv0/_layouts/Redecard.Portal.Aberto.WebParts/ProcessarXmlExterno.aspx")]
        public string urlXml
        {
            get;
            set;
        }

        #endregion

        #region Métodos_______________________

        #endregion

        #region Eventos_______________________

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        #endregion

    }
}
