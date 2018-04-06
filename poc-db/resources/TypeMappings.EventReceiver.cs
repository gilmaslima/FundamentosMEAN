#region Used Namespaces
using System;
using System.Runtime.InteropServices;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Redecard.Portal.Fechado.Model;
using Redecard.Portal.Fechado.Model.Repository;
using Redecard.Portal.Fechado.Model.Repository.DTOs;
#endregion

namespace Redecard.Portal.Fechado.EventReceivers {
    /// <summary>
    ///   Esta classe trata os eventos disparados durante o ciclo de vida da feature.
    /// </summary>
    /// <remarks>
    ///   O GUID desta classe irá ser usado no pacote e não deve ser modificado.
    /// </remarks>
    [Guid("fb608c2d-3c37-4986-b882-7658f15b1b38")]
    public class TypeMappingsEventReceiver : SPFeatureReceiver {
        /// <summary>
        ///   Constante que define o numéro máximo de tentativas de salvar as configurações que a feature
        ///   realizará.
        /// </summary>
        private const int MaxConfigurationSaveRetryCount = 10;

        /// <summary>
        ///   Registra os repositórios no Service Locator.
        /// </summary>
        /// <param name = "properties">Propriedades da feature.</param>
        public override void FeatureActivated(SPFeatureReceiverProperties properties) {
            var savedConfig = false;
            var savedException = new Exception();

            for (var retryCnt = 0; retryCnt < MaxConfigurationSaveRetryCount && !savedConfig; retryCnt++) {
                //Se múltiplas tentativas de escrita ocorrerem concorrentes à property bag para um objeto no mesmo tempo,
                //então o SharePoint irá disparar uma exceção.
                //Por isto deve-se armazenar esta excessão e tentar salvar as configurações antes de disparar este erro.
                try {
                    AddMappings();
                    savedConfig = true;
                }
                catch (SPUpdatedConcurrencyException ex) {
                    savedException = ex;
                }
            }

            if (savedConfig == false) throw savedException;
        }

        /// <summary>
        ///   Remove os repositórios do Service Locator.
        /// </summary>
        /// <param name = "properties">Propriedades da feature.</param>
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties) {
            var savedConfig = false;
            var savedException = new Exception();

            for (var retryCnt = 0; retryCnt < MaxConfigurationSaveRetryCount && !savedConfig; retryCnt++) {
                //Se múltiplas tentativas de escrita ocorrerem concorrentes à property bag para um objeto no mesmo tempo,
                //então o SharePoint irá disparar uma exceção.
                //Por isto deve-se armazenar esta excessão e tentar salvar as configurações antes de disparar este erro.
                try {
                    RemoveMappings();
                    savedConfig = true;
                }
                catch (SPUpdatedConcurrencyException ex) {
                    savedException = ex;
                }
            }

            if (savedConfig == false) throw savedException;
        }

        /// <summary>
        ///   Registra os repositórios no Service Locator.
        /// </summary>
        private static void AddMappings() {
            SPSecurity.RunWithElevatedPrivileges(delegate {
                var serviceLocator = SharePointServiceLocator.GetCurrent();
                var typeMappings = serviceLocator.GetInstance<IServiceLocatorConfig>();

                typeMappings.RegisterTypeMapping<IRepository<DTOAcessoRapido, AcessoRápidoItem>, RepositoryAcessoRapido>();
                typeMappings.RegisterTypeMapping<IRepository<DTOAviso, AvisosItem>, RepositoryAvisos>();
                typeMappings.RegisterTypeMapping<IRepository<DTODicaUsoMaquininha, DicasDoUsoDaMaquininhaItem>, RepositoryDicasUsoMaquininha>();
                typeMappings.RegisterTypeMapping<IRepository<DTODuvidaFrequente, DúvidasFrequentesItem>, RepositoryDuvidasFrequentes>();
                typeMappings.RegisterTypeMapping<IRepository<DTOEnquetePergunta, EnquetePerguntasItem>, RepositoryEnquetePerguntas>();
                typeMappings.RegisterTypeMapping<IRepository<DTOEnqueteResposta, EnqueteRespostasItem>, RepositoryEnqueteRespostas>();
                typeMappings.RegisterTypeMapping<IRepository<DTOEnqueteResultado, EnqueteResultadosItem>, RepositoryEnqueteResultados>();
                typeMappings.RegisterTypeMapping<IRepository<DTOLinkFavorito, LinksFavoritosItem>, RepositoryLinksFavoritos>();
                typeMappings.RegisterTypeMapping<IRepository<DTOPerguntaFrequente, PerguntasFrequentesItem>, RepositoryPerguntasFrequentes>();

            });
        }

        /// <summary>
        ///   Remove os repositórios do Service Locator.
        /// </summary>
        private static void RemoveMappings() {
            SPSecurity.RunWithElevatedPrivileges(delegate {
                var serviceLocator = SharePointServiceLocator.GetCurrent();
                var typeMappings = serviceLocator.GetInstance<IServiceLocatorConfig>();

                typeMappings.RemoveTypeMapping<IRepository<DTOAcessoRapido, AcessoRápidoItem>>(null);
                typeMappings.RemoveTypeMapping<IRepository<DTOAviso, AvisosItem>>(null);
                typeMappings.RemoveTypeMapping<IRepository<DTODicaUsoMaquininha, DicasDoUsoDaMaquininhaItem>>(null);
                typeMappings.RemoveTypeMapping<IRepository<DTODuvidaFrequente, DúvidasFrequentesItem>>(null);
                typeMappings.RemoveTypeMapping<IRepository<DTOEnquetePergunta, EnquetePerguntasItem>>(null);
                typeMappings.RemoveTypeMapping<IRepository<DTOEnqueteResposta, EnqueteRespostasItem>>(null);
                typeMappings.RemoveTypeMapping<IRepository<DTOEnqueteResultado, EnqueteResultadosItem>>(null);
                typeMappings.RemoveTypeMapping<IRepository<DTOLinkFavorito, LinksFavoritosItem>>(null);
                typeMappings.RemoveTypeMapping<IRepository<DTOPerguntaFrequente, PerguntasFrequentesItem>>(null);

            });
        }

        #region Unusable Methods
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
        #endregion
    }
}