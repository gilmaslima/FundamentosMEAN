using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace Redecard.Portal.Aberto.WebControls {
    
    /// <summary>
    /// 
    /// </summary>
    public class LegadoLink : HyperLink {

        protected override void OnPreRender(System.EventArgs e) {
            // Alterar o NavigateUrl
            base.NavigateUrl = SPContext.Current.Web.Environment().LegadoUrl + base.NavigateUrl;
            base.OnPreRender(e);
        }
    }
}