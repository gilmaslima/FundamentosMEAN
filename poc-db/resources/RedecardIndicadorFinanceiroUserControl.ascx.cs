using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Net;
using System.IO;

namespace Redecard.Portal.Aberto.WebParts.RedecardIndicadorFinanceiro
{
    public partial class RedecardIndicadorFinanceiroUserControl : UserControl
    {
        #region Propriedades__________________

        private RedecardIndicadorFinanceiro WebPart
        {
            get
            {
                return (RedecardIndicadorFinanceiro)this.Parent;
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
            this.urlXml.Text = String.Format("'{0}'", this.WebPart.urlXml); 
        }

        #endregion

    }
}
