using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Redecard.PN.Comum;

namespace Rede.PN.Eadquirencia.Sharepoint.Features.Rede.PN.Eadquirencia.Modulos
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("196e8a57-a5aa-4903-b398-c03eb501e806")]
    public class RedePNEadquirenciaEventReceiver : SPFeatureReceiver
    {
        #region [Eventos]
        /// <summary>
        /// Ativação da feature
        /// </summary>
        /// <param name="properties"></param>
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var pagePrefix = "Pn";
            var site = properties.Feature.Parent as SPWeb;
            Util.FixDuplicateWebParts(site, pagePrefix);
        }

        /// <summary>
        /// Desativação da feature
        /// </summary>
        /// <param name="properties"></param>
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            this.RemoverPaginasAssociadas(properties);
        }

        #endregion [Fim - Eventos]

        #region [Métodos]

        public void RemoverPaginasAssociadas(SPFeatureReceiverProperties properties)
        {
            String[] paginas = new String[]
            {
                "pn_gerartoken.aspx",
                "pn_cadastrarDba.aspx",
                "pn_relatorioVendas.aspx",
                "pn_capturartransacao.aspx",
                "pn_estornartransacao.aspx",
                "pn_facasuavenda.aspx",
                "pn_gerenciamentovendas.aspx",
                "pn_homologarPV.aspx",
				"pn_configurarcallback.aspx"
            };

            using (SPWeb web = (SPWeb)properties.Feature.Parent)
            {
                if (web != null)
                {
                    foreach (SPFolder folder in ((SPWeb)properties.Feature.Parent).Folders)
                    {
                        if (folder.Name.Equals("Paginas", StringComparison.InvariantCultureIgnoreCase) || folder.Name.Equals("Páginas", StringComparison.InvariantCultureIgnoreCase))
                        {
                            for (int i = folder.Files.Count - 1; i >= 0; i--)
                            {
                                try
                                {
                                    if (Array.Exists(paginas, p => p.Equals(folder.Files[i].Name, StringComparison.InvariantCultureIgnoreCase)))
                                    {
                                        folder.Files[i].Delete();
                                    }
                                }
                                catch (NullReferenceException e)
                                {
                                    Logger.GravarErro(e.Message, e);
                                }
                                catch (Exception e)
                                {
                                    Logger.GravarLog(e.Message, System.Diagnostics.TraceEventType.Error);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion [Fim - Métodos]
    }
}
