using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.Boston.Sharepoint.WebParts.GerenciamentoMobile
{
    public partial class GerenciamentoMobileUserControl : UserControl
    {
        private GerenciamentoMobile WebPart
        {
            get
            {
                return (GerenciamentoMobile)this.Parent;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                hiddenUrl.Value = this.WebPart.URL;
            }
        }
    }
}
