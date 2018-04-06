using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace Redecard.Portal.Aberto.WebControls {

    /// <summary>
    /// 
    /// </summary>
    public class LegadoLiteral : Literal {

        protected override void OnPreRender(System.EventArgs e) {
            base.Text = this.Text.Replace("[URL]", SPContext.Current.Web.Environment().LegadoUrl);
            base.OnPreRender(e);
        }
    }
}
