using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.Portal.Aberto.WebParts.RedecardMenuMiniSite
{
    [ToolboxItemAttribute(false)]
    public class RedecardMenuMiniSite : WebPart
    {

        #region Enumeração____________________

        #endregion

        #region Constantes____________________

        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/RedecardMenuMiniSite/RedecardMenuMiniSiteUserControl.ascx";

        #endregion

        #region Propriedades__________________

        /// <summary>
        /// URL da Imagem
        /// </summary>
        [WebBrowsable(true)]
        [WebDisplayName("URL da imagem")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configuracoes")]
        [DefaultValue(4)]
        public string urlImagem
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
