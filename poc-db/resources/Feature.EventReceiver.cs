using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

namespace Redecard.Portal.Aberto.Variations
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    internal class VariationLabel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string FlagControlDisplayName { get; set; }
        ///
        /// eg. en-US
        /// 
        public string Language { get; set; }
        ///
        /// eg. 1033
        /// 
        public uint Locale { get; set; }
        public string HierarchyCreationMode { get; set; }
        public bool IsSource { get; set; }
        public string SourceVarRootWebTemplate { get; set; }
    }

    internal static class CreationMode
    {
        public const string PublishingSitesAndAllPages = "Publishing Sites and All Pages";
        public const string PublishingSitesOnly = "Publishing Sites Only";
        public const string RootSitesOnly = "Root Sites Only";
    }

    [Guid("36313282-a152-42b6-a84e-110b50cb23af")]
    public class VariationEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        private static VariationLabel[] labels = { 
                new VariationLabel 
                { 
                    Title = "pt-BR", 
                    FlagControlDisplayName = "pt-BR", 
                    Language = "pt-BR", 
                    Locale = 1046, 
                    HierarchyCreationMode = CreationMode.PublishingSitesAndAllPages, 
                    IsSource = true 
                }//, 
                
                //new VariationLabel 
                //{ 
                //    Title = "en-US", 
                //    FlagControlDisplayName = "en-US", 
                //    Language = "en-US", 
                //    Locale = 1033, 
                //    HierarchyCreationMode = CreationMode.PublishingSitesAndAllPages 
                //} 
        }; 

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            ConfigureVariationsSettings(((SPSite)properties.Feature.Parent).RootWeb);
            CreateVariations(((SPSite)properties.Feature.Parent).RootWeb);
            CreateHierarchies((SPSite)properties.Feature.Parent, ((SPSite)properties.Feature.Parent).RootWeb);
        }

        private static void ConfigureVariationsSettings(SPWeb rootWeb)
        {
            Guid varRelationshipsListId = new Guid(rootWeb.AllProperties["_VarRelationshipsListId"] as string);

            SPList varRelationshipsList = rootWeb.Lists[varRelationshipsListId];
            SPFolder rootFolder = varRelationshipsList.RootFolder;
            // Automatic creation     
            rootFolder.Properties["EnableAutoSpawnPropertyName"] = "true";
            // Recreate Deleted Target Page; set to false to enable recreation     
            rootFolder.Properties["AutoSpawnStopAfterDeletePropertyName"] = "true";
            // Update Target Page Web Parts     
            rootFolder.Properties["UpdateWebPartsPropertyName"] = "true";
            // Resources     
            rootFolder.Properties["CopyResourcesPropertyName"] = "true";
            // Notification     
            rootFolder.Properties["SendNotificationEmailPropertyName"] = "false";
            rootFolder.Properties["SourceVarRootWebTemplatePropertyName"] = "PortalRedecard#0";
            rootFolder.Update();

            SPListItem item = null;

            if (varRelationshipsList.Items.Count > 0)
            {
                item = varRelationshipsList.Items[0];
            }
            else
            {
                item = varRelationshipsList.Items.Add();
                item["GroupGuid"] = new Guid("7D5469DD-4DC0-4DD9-B384-9ED714C78A57");
            }

            item["Deleted"] = false;
            item["ObjectID"] = rootWeb.ServerRelativeUrl;
            item["ParentAreaID"] = String.Empty;
            item.Update();
        }

        private static void CreateVariations(SPWeb rootWeb)
        {
            Guid varListId = new Guid(rootWeb.AllProperties["_VarLabelsListId"] as string);

            SPList varList = rootWeb.Lists[varListId];

            foreach (VariationLabel label in labels)
            {
                SPListItem item = varList.Items.Add();
                item[SPBuiltInFieldId.Title] = label.Title;
                item["Description"] = label.Description;
                item["Flag Control Display Name"] = label.FlagControlDisplayName;
                item["Language"] = label.Language;
                item["Locale"] = label.Locale.ToString();
                item["Hierarchy Creation Mode"] = label.HierarchyCreationMode;
                item["Is Source"] = label.IsSource.ToString();
                item["Hierarchy Is Created"] = false;
                item.Update();
            }
        }

        private static void CreateHierarchies(SPSite site, SPWeb rootWeb)
        {
            
                site.AddWorkItem(Guid.Empty,
                    DateTime.Now.ToUniversalTime(),
                    new Guid("C00C1393-5647-4EED-ACD0-8BBA25C89EB4"),
                    rootWeb.ID,
                    site.ID,
                    1,
                    false,
                    Guid.Empty,
                    Guid.Empty,
                    rootWeb.CurrentUser.ID,
                    null,
                    String.Empty,
                    Guid.Empty,
                    false);

                //SPWebApplication webApplication = site.WebApplication;
                //SPJobDefinition variationsJob = (from SPJobDefinition job in webApplication.JobDefinitions where job.Name == "VariationsCreateHierarchies" select job).FirstOrDefault();
                //if (variationsJob != null)
                //{
                //    DateTime startTime = DateTime.Now.ToUniversalTime();
                //    variationsJob.RunNow();           // wait until the job is finished         

                //    while ((from SPJobHistory j in webApplication.JobHistoryEntries where j.JobDefinitionId == variationsJob.Id && j.StartTime > startTime select j).Any() == false)
                //    {
                //        Thread.Sleep(100);
                //    }
                //}
           
        }

        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
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
