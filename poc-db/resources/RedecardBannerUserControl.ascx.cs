using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.Portal.Fechado.WebParts.RedecardBanner
{
    public partial class RedecardBannerUserControl : UserControl
    {

        #region Propriedades__________________

        private RedecardBanner WebPart
        {
            get
            {
                return (RedecardBanner)this.Parent;
            }
        }

        #endregion

        #region Constantes____________________

        #endregion

        #region Métodos_______________________

        #endregion

        #region Eventos_______________________

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        #endregion

    }
}
