using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.Portal.Fechado.WebParts.RedecardDuvidasMaquininha
{
    public partial class RedecardDuvidasMaquininhaUserControl : UserControl
    {

        #region Propriedades__________________

        private RedecardDuvidasMaquininha WebPart
        {
            get
            {
                return (RedecardDuvidasMaquininha)this.Parent;
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
