
using Microsoft.SharePoint;

namespace Redecard.Portal.Helper {
    
    /// <summary>
    /// 
    /// </summary>
    public class SimulEnvironment : IPortalEnvironment {

        /// <summary>
        /// 
        /// </summary>
        public string Url {
            get {
                return SPContext.Current.Web.Url;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string LegadoUrl {
            get { 
                return "https://simul.redecard.com.br/";
            }
        }
    }
}
