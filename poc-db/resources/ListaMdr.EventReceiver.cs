using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;

namespace Redecard.PN.Extrato.SharePoint.Features.ListaMdr
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("799364e7-06f0-49cf-b956-0fd745127913")]
    public class ListaMdrEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        //public override void FeatureActivated(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        /// <summary>
        /// Ao desativar a feature no site
        /// </summary>
        /// <param name="properties"></param>
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWeb web = properties.Feature.Parent as SPWeb;

            String[] listas = new[] { "Mensagem Oferta de Taxa" };

            foreach (String lista in listas)
            {
                SPList spList = web.Lists.TryGetList(lista);
                if (spList != null)
                    spList.Delete();
            }
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
