using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;

namespace Redecard.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Redecard.PN.AutoCredenciamento
{
    public partial class HeaderAutoCredenciamento : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Form.DefaultFocus = this.UniqueID;
        }
    }
}
