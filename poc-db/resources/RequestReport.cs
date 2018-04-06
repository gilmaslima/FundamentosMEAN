using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.Request.SharePoint.WebParts.RequestReport
{
    [ToolboxItemAttribute(false)]
    public class RequestReport : WebPart
    {
        private const string ascxPath = @"~/_CONTROLTEMPLATES/Redecard.PN.Request.SharePoint.WebParts/RequestReport/RequestReportUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(ascxPath);
            Controls.Add(control);
        }
    }
}
