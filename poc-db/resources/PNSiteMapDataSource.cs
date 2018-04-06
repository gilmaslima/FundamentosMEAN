using System;
using System.Web.UI.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.Providers.SiteMap
{
    /// <summary>
    /// DataSource do SiteMap construído para o Portal
    /// </summary>
    public class PNSiteMapDataSource : SiteMapDataSource
    {
        /// <summary>
        /// Método de inicio do DataSource
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.Provider = new PNSiteMapProvider();
            base.OnInit(e);
        }
    }
}
