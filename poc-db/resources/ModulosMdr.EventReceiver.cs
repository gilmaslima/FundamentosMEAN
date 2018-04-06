using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using System.Collections.Generic;

namespace Redecard.PN.OutrosServicos.SharePoint.Features.ModulosMdr
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("a4fb3352-bf5f-49ad-bba1-9d79a404728e")]
    public class ModulosMdrEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        //public override void FeatureActivated(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //    SPWeb web = properties.Feature.Parent as SPWeb;

        //    List<String> paginas = new List<String>() { 
        //    };

        //    String queryPaginas = "<Where><Or>";

        //    foreach (String pagina in paginas)
        //    {
        //        queryPaginas = String.Concat(queryPaginas,
        //                                        "<Eq>",
        //                                        "<FieldRef Name='Title'/>",
        //                                        "<Value Type='Text'>", pagina, "</Value>",
        //                                        "</Eq>");
        //    }

        //    queryPaginas = String.Concat(queryPaginas, "</Or></Where>");

        //    SPQuery query = new SPQuery();
        //    query.Query = queryPaginas;

        //    SPList listaPaginas = web.Lists.TryGetList("Páginas");

        //    if (listaPaginas != null)
        //    {
        //        var paginasModulo = listaPaginas.GetItems();
        //        var paginasDeletar = new List<Int32>();

        //        if (!Object.ReferenceEquals(paginasModulo, null))
        //        {
        //            foreach (SPListItem item in paginasModulo)
        //            {
        //                if (paginas.Contains(item.Name))
        //                {
        //                    paginasDeletar.Add(item.ID);
        //                }
        //            }
        //        }

        //        if (paginasDeletar.Count > 0)
        //        {
        //            paginasDeletar.ForEach(item =>
        //            {
        //                var itemDeletar = paginasModulo.GetItemById(item);
        //                itemDeletar.Delete();
        //            });
        //        }

        //        web.Dispose();
        //    }
        //}


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
