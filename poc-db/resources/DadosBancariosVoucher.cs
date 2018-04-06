using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.DadosBancariosVoucher
{
    [ToolboxItemAttribute(false)]
    public class DadosBancariosVoucher : WebPart
    {
        private const string ascxPath = @"~/_CONTROLTEMPLATES/PN.DadosCadastrais.WP/DadosBancariosVoucher/DadosBancariosVoucherUserControl.ascx";
        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(ascxPath);
            Controls.Add(control);
        }
    }
}
