using Microsoft.SharePoint;

namespace Redecard.Portal.Helper {
    
    /// <summary>
    /// 
    /// </summary>
    public class FactoryEnvironment {

        /// <summary>
        /// 
        /// </summary>
        public static IPortalEnvironment Current {
            get {
                if (SPContext.Current.Web.Url.ToLowerInvariant().Contains("portal.redecard.com.br") ||
                    SPContext.Current.Web.Url.ToLowerInvariant().Contains("www.redecard.com.br") ||
                    SPContext.Current.Web.Url.ToLowerInvariant().Contains("redecard.com.br"))
                    return new ProdEnvironment();
                return new SimulEnvironment();
            }
        }
    }
}