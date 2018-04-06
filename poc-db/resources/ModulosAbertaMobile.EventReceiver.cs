using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using System.Collections.Generic;
using Microsoft.SharePoint.WebPartPages;
using Redecard.PN.Comum;
using System.Web.UI.WebControls.WebParts;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.Features.ModulosAbertaMobile
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("ae36d7ee-d811-4ffd-b1d4-4ad3ef99f361")]
    public class ModulosAbertaMobileEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var site = properties.Feature.Parent as SPWeb;
            Redecard.PN.Comum.Util.FixDuplicateWebParts(site, string.Empty);
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            Redecard.PN.Comum.Util.RemoverPaginasAssociadas(properties);
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
