using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace Redecard.Portal.Fechado.WebParts.RedecardBanner
{
    [ToolboxItemAttribute(false)]
    public class RedecardBanner : System.Web.UI.WebControls.WebParts.WebPart
    {
        #region Variáveis_____________________

        private string nomeBiblioteca = string.Empty;

        #endregion

        #region Constantes____________________

        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Fechado.WebParts/RedecardBanner/RedecardBannerUserControl.ascx";

        #endregion

        #region Propriedades__________________

        /// <summary>
        /// Biblioteca de exibição
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Nome da biblioteca de banners:")]
        [Description("Define o nome da bibiloteca de banners cujos itens serão listados")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Nome da biblioteca de Banners:")]
        public string NomeBiblioteca
        {
            get { return this.nomeBiblioteca; }
            set { this.nomeBiblioteca = value; }
        }

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
