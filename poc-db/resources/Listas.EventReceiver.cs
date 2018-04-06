/*
© Copyright 2014 Rede S.A.
Autor : Evandro Coutinho
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using System.IO;
using System.Xml.Serialization;
using Rede.PN.AtendimentoDigital.SharePoint.Core.ListSchema;
using System.Xml;

namespace Rede.PN.AtendimentoDigital.SharePoint.Features.Listas
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("bba7584f-af2b-4e20-8fd5-f5ac2797e622")]
    public class ListasEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            if (properties != null)
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    try
                    {
                        SPWeb currentWeb = properties.Feature.Parent as SPWeb;
                        if (currentWeb != null)
                        {
                            // Varrendo as propriedades da Feature 
                            foreach (SPFeatureProperty prop in properties.Feature.Properties)
                            {
                                // Retornado a lista do SharePoint
                                SPList list = currentWeb.Lists.TryGetList(prop.Name);

                                if (list != null)
                                {
                                    // Concatenando o caminho onde está o xml
                                    string filePath = Path.Combine(properties.Definition.RootDirectory, prop.Value);

                                    // Realizand o streaming do aquivo
                                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                                    {
                                        // criando um objeto serializado da classe List
                                        XmlSerializer ser = new XmlSerializer(typeof(List));

                                        List arrList = (List)ser.Deserialize(fs);

                                        if (arrList != null)
                                        {
                                            // varrendo todas as colunas do lista serializada
                                            foreach (var item in arrList.MetaData.Fields)
                                            {
                                                // Verificando se a coluna já existe
                                                if (list.Fields.ContainsField(item.DisplayName))
                                                    continue;

                                                using (StringWriter stringwriter = new System.IO.StringWriter())
                                                {
                                                    // serializando a classe list.Fields.Field para o xml
                                                    var serializer = new XmlSerializer(item.GetType());
                                                    serializer.Serialize(stringwriter, item);
                                                    XmlDocument xmlDoc = new XmlDocument();
                                                    xmlDoc.LoadXml(stringwriter.ToString());

                                                    //Adicionando o campo Field a partir do xml
                                                    list.Fields.AddFieldAsXml(xmlDoc.OuterXml, false, SPAddFieldOptions.AddFieldInternalNameHint);
                                                    list.Update();
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                    }
                    catch (FileNotFoundException ex)
                    {
                        throw new FileNotFoundException("Arquivo ou lista não encontrado.", ex);
                    }
                    catch (SPException)
                    {

                        throw;
                    }
                });
            }

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
