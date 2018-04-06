using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace Redecard.PN.DadosCadastrais.SharePoint.Features.ListasFidelidade
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("9f68ca63-afaa-45c2-a3ca-f185fe9947b6")]
    public class ListasFidelidadeEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        //public override void FeatureActivated(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            VerificarSeListaExiste(properties, "Ips Autorizados");
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

        #region .VerificarSeListaExiste.
        /// <summary>
        /// Verifica se a lista existe e apaga se encontrar.
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="nomeLista"></param>
        private void VerificarSeListaExiste(SPFeatureReceiverProperties properties, string nomeLista)
        {
            SPWeb web = properties.Feature.Parent as SPWeb;

            String[] listas = new[] { nomeLista };

            foreach (String lista in listas)
            {
                SPList spList = web.Lists.TryGetList(lista);
                if (spList != null)
                    spList.Delete();
            }
        }
        #endregion
    }
}
