
using Microsoft.SharePoint;
namespace Redecard.Portal.Helper {

    /// <summary>
    /// 
    /// </summary>
    public class ProdEnvironment : IPortalEnvironment {

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
                return "https://services.redecard.com.br/";
            }
        }
    }
}
