using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Redecard.PN.Comum;
using System.ServiceModel;

namespace Redecard.PN.Request.SharePoint.WebParts.RequestOutrasFormas
{
    public partial class RequestOutrasFormasUserControl : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lnkCarta.NavigateUrl = "";
                lnkEnvelopeResposta.NavigateUrl = "";
                lnkCartaEnvelope.NavigateUrl = "";
            }
        }
    }
}
