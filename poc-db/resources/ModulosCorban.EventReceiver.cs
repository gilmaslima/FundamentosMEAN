using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Publishing;
using System.Collections.Generic;

namespace Redecard.PN.OutrosServicos.SharePoint.Features.ModulosCorban
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("6a57138c-c7e9-4750-9009-90fcafe40de3")]
    public class ModulosCorbanEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        //public override void FeatureActivated(SPFeatureReceiverProperties properties)
        //{
        //}
        
        /// <summary>
        /// EventReceiver para desativação da Feature
        /// </summary>
        /// <param name="properties"></param>
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //SPWeb web = properties.Feature.Parent as SPWeb;
            
            //List<String> paginas = new List<String>() { "HistoricoTransacoesCorban" };

            ////String queryPaginas = "<Where><Or>";

            ////foreach (String pagina in paginas)
            ////{
            ////    queryPaginas = String.Concat(queryPaginas, 
            ////                                    "<Eq>",
            ////                                    "<FieldRef Name='Title'/>",
            ////                                    "<Value Type='Text'>", pagina, "</Value>",
            ////                                    "</Eq>");
            ////}

            ////queryPaginas = String.Concat(queryPaginas, "</Or></Where>");

            ////SPQuery query = new SPQuery();
            ////query.Query = queryPaginas;
            
            //SPList listaPaginas = web.Lists.TryGetList("Páginas");

            //if (listaPaginas != null)
            //{
            //    var paginasModulo = listaPaginas.GetItems();

            //    if (!Object.ReferenceEquals(paginasModulo, null))
            //    {
            //      foreach (SPListItem item in paginasModulo)
            //      {
            //          if (paginas.Contains(item.Name))
            //              item.Delete();
            //}
            //      }

            //      web.Dispose();
            //    }
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
