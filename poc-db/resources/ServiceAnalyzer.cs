/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using Microsoft.CSharp;
using Microsoft.SharePoint;
using Redecard.PN.Sustentacao.SharePoint.SustentacaoAdministracaoServico;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Threading;
using System.Xml;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe ServiceAnalyzer
    /// </summary>
	public class ServiceAnalyzer
    {
        #region [Attributes and Properties]

        /// <summary>
        /// defaultConfigName
        /// </summary>
        public const String defaultConfigName = "default.config";

        /// <summary>
        /// customBindingType
        /// </summary>
        private static Type customBindingType = typeof(CustomBindingElement);

        /// <summary>
        /// standardBindingType
        /// </summary>
        private static Type standardBindingType = typeof(StandardBindingElement);

        /// <summary>
        /// defaultSvcInvocationTimeout
        /// </summary>
        private static TimeSpan defaultSvcInvocationTimeout = new TimeSpan(0, 5, 0);        

        #endregion

        #region [Methods]

        /// <summary>
        /// Cria o proxy e prepara os objetos que serão utilizados para as chamadas do serviço, baseado no address.
        /// </summary>
        /// <param name="address">URL do serviço.</param>
        /// <param name="serviceSession">Objeto de sessão com informações sobre o serviço.</param>
        /// <returns>Objeto ServiceProject.</returns>
        public static ServiceProject AnalyzeService(String address, ServiceSession serviceSession)
        {
            ProxyInfo proxyInfo = serviceSession.Proxy;

            if (proxyInfo == null)
            {
                using (var client = new SustentacaoAdministracaoServicoClient())
                {
                    proxyInfo = client.GetProxy(address);
                    serviceSession.Proxy = proxyInfo;
                }
            }

            IDictionary<ChannelEndpointElement, ClientEndpointInfo> services = new Dictionary<ChannelEndpointElement, ClientEndpointInfo>();
            String errorMessage = String.Empty;
            String projectDirectory = serviceSession.ProjectDirectory;

            if (serviceSession.ProjectDirectory == null)
            {
                Guid guid = Guid.NewGuid();
                projectDirectory = Path.Combine(Path.GetTempPath(), String.Format("PN_WCFTestWeb\\{0}", guid));
                //Cria o objeto para a session.
                serviceSession.ProjectDirectory = projectDirectory;
            }

            String configPath = Path.Combine(projectDirectory, "WcfTestWeb.config");
            String assemblyFullPath = String.Empty;

            SPSecurity.RunWithElevatedPrivileges(() => {

                if (!Directory.Exists(projectDirectory))
                    Directory.CreateDirectory(projectDirectory);

            });

            ServiceModelSectionGroup config = ServiceAnalyzer.AnalyzeConfig(services, proxyInfo.Config, configPath);

            if (services.Count > 0)
            {
                AppDomain appDomain = ServiceAnalyzer.AnalyzeProxy(services, proxyInfo.Class, config, configPath, projectDirectory, out assemblyFullPath);
                return new ServiceProject(address, projectDirectory, configPath, assemblyFullPath, appDomain, services.Values);
            }

            return null;

        }

        /// <summary>
        /// Obtêm ou cria o config do serviço que será chamado e instância os objetos de ClientEndpoint basedo nesse config.
        /// </summary>
        /// <param name="services">Dictionary de ClienEndpoits que serão preenchido.</param>
        /// <param name="configXml">XML do config que será criado, caso não exista.</param>
        /// <param name="configPath">Caminho completo do arquivo de configuração para o serviço.</param>
        /// <param name="errorMessage">Parâmetro de saída para reportar erro.</param>
        /// <returns>Objeto ServiceModelSectionGroup</returns>
        public static ServiceModelSectionGroup AnalyzeConfig(IDictionary<ChannelEndpointElement, ClientEndpointInfo> services, String configXml, String configPath)
        {
            ServiceModelSectionGroup result = null;

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                if (!File.Exists(configPath))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;
                    var xmlDoc = new XmlDocument();

                    using (var writer = XmlWriter.Create(configPath, settings))
                    {
                        xmlDoc.LoadXml(configXml);
                        xmlDoc.Save(writer);
                        writer.Flush();
                        writer.Close();
                    }
                }

                ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();
                Configuration configuration = ConfigurationManager.OpenMachineConfiguration();
                exeConfigurationFileMap.MachineConfigFilename = configuration.FilePath;
                exeConfigurationFileMap.ExeConfigFilename = configPath;

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);

                ServiceModelSectionGroup sectionGroup = ServiceModelSectionGroup.GetSectionGroup(config);


                foreach (ChannelEndpointElement channelEndpointElement in sectionGroup.Client.Endpoints)
                {
                    services.Add(channelEndpointElement, new ClientEndpointInfo(channelEndpointElement.Contract));
                }

                result = sectionGroup;
            });

            return result;
        }

        /// <summary>
        /// Cria um AppDomain baseado na DLL proxy e no config gerado.
        /// </summary>
        /// <param name="services">Lista de ClientEnpoints do serviço em questão.</param>
        /// <param name="configObject">Objeto ServiceModelSectionGroup obtido do client config.</param>
        /// <param name="configPath">Caminho completo do client config.</param>
        /// <param name="assemblyFullPath">Caminho completo da DLL de proxy.</param>
        /// <returns>Objeto AppDomain criado.</returns>
        public static AppDomain AnalyzeProxy(
            IDictionary<ChannelEndpointElement, ClientEndpointInfo> services, ServiceModelSectionGroup configObject, String configPath, String assemblyFullPath)
        {
            AppDomain appDomain = ServiceAnalyzer.CreateAppDomain(configPath, assemblyFullPath);
            DataContractAnalyzer dataContractAnalyzer =
                (DataContractAnalyzer)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(DataContractAnalyzer).FullName);

            foreach (ChannelEndpointElement key in configObject.Client.Endpoints)
            {
                ClientEndpointInfo clientEndpointInfo = services[key];
                clientEndpointInfo = dataContractAnalyzer.AnalyzeDataContract(clientEndpointInfo, assemblyFullPath);
                if (clientEndpointInfo == null)
                {
                    services.Remove(key);
                }
                else
                {
                    services[key] = clientEndpointInfo;
                }
            }

            foreach (KeyValuePair<ChannelEndpointElement, ClientEndpointInfo> current in services)
            {
                current.Value.EndpointConfigurationName = current.Key.Name;
            }

            return appDomain;
        }

        /// <summary>
        /// Apaga o diretório informado.
        /// </summary>
        /// <param name="projectDirectory">Caminho completo do diretório a ser apagado.</param>
        public static void DeleteProjectFolder(String projectDirectory)
        {
            Directory.Delete(projectDirectory, true);
        }

        /// <summary>
        /// Altera a configuração de TimeOut dos bindings. de um config.
        /// </summary>
        /// <param name="configPath">Caminho completo do config.</param>
        private static void SetServiceInvocationTimeout(String configPath)
        {
            ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();

            Configuration configuration = ConfigurationManager.OpenMachineConfiguration();
            exeConfigurationFileMap.MachineConfigFilename = configuration.FilePath;
            exeConfigurationFileMap.ExeConfigFilename = configPath;

            Configuration configurationExe = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
            ServiceModelSectionGroup sectionGroup = ServiceModelSectionGroup.GetSectionGroup(configurationExe);

            foreach (BindingCollectionElement bindingCollectionElement in sectionGroup.Bindings.BindingCollections)
            {
                foreach (IBindingConfigurationElement configuredBindind in bindingCollectionElement.ConfiguredBindings)
                {
                    if (configuredBindind.SendTimeout.CompareTo(ServiceAnalyzer.defaultSvcInvocationTimeout) < 0)
                    {
                        Type type = configuredBindind.GetType();
                        if (ServiceAnalyzer.customBindingType.IsAssignableFrom(type))
                        {
                            (configuredBindind as CustomBindingElement).SendTimeout = ServiceAnalyzer.defaultSvcInvocationTimeout;
                        }
                        else
                        {
                            if (ServiceAnalyzer.standardBindingType.IsAssignableFrom(type))
                            {
                                (configuredBindind as StandardBindingElement).SendTimeout = ServiceAnalyzer.defaultSvcInvocationTimeout;
                            }
                        }
                    }
                }
            }

            configurationExe.Save();
        }

        /// <summary>
        /// Obtêm ou cria a DLL de proxy e retorna o AppDomain criado apartir dela.
        /// </summary>
        /// <param name="services">Lista de ClientEnpoints do serviço em questão.</param>
        /// <param name="sourceString">String com o código .cs do proxy retornado do serviço.</param>
        /// <param name="configObject">Objeto ServiceModelSectionGroup obtido do client config.</param>
        /// <param name="configPath">Caminho completo do client config.</param>
        /// <param name="projectDirectory">Diretório para gerar o proxy.</param>
        /// <param name="errorMessage">Variável para reporta erro.</param>
        /// <param name="assemblyFullPath">Caminho completo da DLL de proxy.</param>
        /// <returns>Objeto AppDomain criado.</returns>
        public static AppDomain AnalyzeProxy(IDictionary<ChannelEndpointElement, ClientEndpointInfo> services,
            String sourceString, ServiceModelSectionGroup configObject, String configPath, String projectDirectory, out String assemblyFullPath)
        {
            assemblyFullPath = null;
            CompilerResults compilerResults = null;
            String outputAssemblyPath = null;
            Boolean existsAssembly = false;

            if (services != null && services.Count > 0)
            {
                var cSharpCodeProvider = new CSharpCodeProvider();
                //String contractName = service;    
                assemblyFullPath = String.Format("{0}\\{1}.dll", projectDirectory, "WcfTestWeb");
                outputAssemblyPath = assemblyFullPath;

                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    //Se rodar essa verificação fora do privilégio elevado retorna que o arquivo não existe, por falta de permissão para ler o arquivo.
                    existsAssembly = File.Exists(outputAssemblyPath);
                });

                if (existsAssembly == false)
                {
                    SPSecurity.RunWithElevatedPrivileges(() =>
                    {
                        compilerResults = cSharpCodeProvider.CompileAssemblyFromSource(new CompilerParameters
                        {
                            OutputAssembly = outputAssemblyPath,
                            ReferencedAssemblies =
					            {
					                "System.dll",
					                typeof(DataSet).Assembly.Location,
					                typeof(TypedTableBaseExtensions).Assembly.Location,
					                typeof(XmlReader).Assembly.Location,
					                typeof(OperationDescription).Assembly.Location,
					                typeof(DataContractAttribute).Assembly.Location
				                },
                            GenerateExecutable = false
                        }, new String[] { sourceString });
                    });

                    if (compilerResults.Errors.Count == 0)
                        return ServiceAnalyzer.AnalyzeProxy(services, configObject, configPath, assemblyFullPath);

                    foreach (CompilerError compilerError in compilerResults.Errors)
                    {
                        throw new Exception(String.Format("Erro ao compilar o assmbly: {0} - Erro: {1}", assemblyFullPath, compilerError.ErrorText));
                    }
                }

                return ServiceAnalyzer.AnalyzeProxy(services, configObject, configPath, assemblyFullPath);
            }

            return null;
        }

        /// <summary>
        /// Cria um AppDomaim para a DLL e o Config informados.
        /// </summary>
        /// <param name="configPath">Caminho do config.</param>
        /// <param name="clientAssemblyPath">Caminho da DLL.</param>
        /// <returns>Objeto AppDomain criado.</returns>
        private static AppDomain CreateAppDomain(String configPath, String clientAssemblyPath)
        {
            AppDomain appDomain = null;

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                AppDomainSetup appDomainSetup = new AppDomainSetup();
                appDomainSetup.ConfigurationFile = configPath;
                appDomainSetup.ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                appDomain = AppDomain.CreateDomain(configPath, AppDomain.CurrentDomain.Evidence, appDomainSetup);
                appDomain.SetData("clientAssemblyPath", clientAssemblyPath);
                
            });

            return appDomain;
        }

        #endregion
        
	}
}
