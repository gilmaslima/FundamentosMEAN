/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Redecard.PN.Sustentacao.SharePoint.Helpers;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe ServiceExecutor
    /// </summary>
    [Serializable]
    public class ServiceExecutor : MarshalByRefObject
    {
        #region [Attributes and Properties]

        /// <summary>
        /// closeClientCallBack
        /// </summary>
        private static AsyncCallback closeClientCallBack = new AsyncCallback(ServiceExecutor.CloseClientCallBack);

        /// <summary>
        /// cachedProxies
        /// </summary>
        private static IDictionary<String, Object> cachedProxies = new Dictionary<String, Object>();

        /// <summary>
        /// lockObject
        /// </summary>
        private static Object lockObject = new Object();

        #endregion

        #region [Methods]

        /// <summary>
        /// Executa a chamada do serviço utilizando um client domain.
        /// </summary>
        /// <param name="inputs">Parâmetros de entrega do serviço.</param>
        /// <returns>Objeto de retorno do serviço.</returns>
        public static ServiceInvocationOutputs ExecuteInClientDomain(ServiceInvocationInputs inputs)
        {

            ServiceInvocationOutputs outPuts = null;
            
            ServiceExecutor serviceExecutor =
                (ServiceExecutor)inputs.Domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ServiceExecutor).FullName);

            outPuts = serviceExecutor.Execute(inputs);

            return outPuts;
        }

        /// <summary>
        /// Valida se o campo é do tipo Dictionary, utilizando o client domain pra criara o objeto.
        /// </summary>
        /// <param name="field">Campo que será verificado</param>
        /// <param name="domain">Objeto AppDomain</param>
        /// <returns>Retorna uma lista dos index's que geraram erro ao tentar criar o item.</returns>
        public static IList<Int32> ValidateDictionary(DictionaryField field, AppDomain domain)
        {
            ServiceExecutor serviceExecutor =
                (ServiceExecutor)domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(ServiceExecutor).FullName);

            return serviceExecutor.ValidateDictionary(field);
        }

        /// <summary>
        /// Cria o proxy e adiciona para o "cache" (um dictionary em memória.) 
        /// </summary>
        /// <param name="proxyIdentifier">Nome do proxy.</param>
        /// <param name="clientTypeName">Tipo do contrato do proxy.</param>
        /// <param name="endpointConfigurationName">Nome do endpoint.</param>
        public void ConstructClientToCache(String proxyIdentifier, String clientTypeName, String endpointConfigurationName)
        {
            Object value = ServiceExecutor.ConstructClient(clientTypeName, endpointConfigurationName);
            ServiceExecutor.cachedProxies.Add(proxyIdentifier, value);
        }

        /// <summary>
        /// Apaga o proxy especificado do "cache" (um dictionary em memória.) 
        /// </summary>
        /// <param name="proxyIdentifier">Nome do proxy.</param>
        public void DeleteClient(String proxyIdentifier)
        {
            if (ServiceExecutor.cachedProxies.ContainsKey(proxyIdentifier))
            {
                ServiceExecutor.CloseClient((ICommunicationObject)ServiceExecutor.cachedProxies[proxyIdentifier]);
                ServiceExecutor.cachedProxies.Remove(proxyIdentifier);
            }
        }

        /// <summary>
        /// Convert os campos de entrada em um Dictionary de parâmetros.
        /// </summary>
        /// <param name="inputs">Array de fields.</param>
        /// <returns>Dictionary de parâmetros.</returns>
        private static IDictionary<String, Object> BuildParameters(Field[] inputs)
        {
            IDictionary<String, Object> parameters = new Dictionary<String, Object>();

            for (Int32 i = 0; i < inputs.Length; i++)
            {
                Field field = inputs[i];
                Object value = field.CreateObject();
                parameters.Add(field.Name, value);
            }

            return parameters;
        }

        /// <summary>
        /// Encerra o processo de client callback, para chamada assincronas.
        /// </summary>
        /// <param name="result">Objeto IAsyncResult.</param>
        private static void CloseClientCallBack(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                ICommunicationObject client = (ICommunicationObject)result.AsyncState;
                ServiceExecutor.ProcessClientClose(client, result);
            }
        }

        /// <summary>
        /// Fecha o canal de comunicação do client com o serviço.
        /// </summary>
        /// <param name="client">Objeto ICommunicationObject.</param>
        /// <param name="result">Objeto IAsyncResult</param>
        private static void ProcessClientClose(ICommunicationObject client, IAsyncResult result)
        {
            try
            {
                client.EndClose(result);
            }
            catch (TimeoutException)
            {
                client.Abort();
            }
            catch (CommunicationException)
            {
                client.Abort();
            }
        }

        /// <summary>
        /// Fecha o canal de comunicação do client com o serviço.
        /// </summary>
        /// <param name="client">Objeto ICommunicationObject.</param>
        private static void CloseClient(ICommunicationObject client)
        {
            try
            {
                IAsyncResult asyncResult = client.BeginClose(ServiceExecutor.closeClientCallBack, client);
                if (asyncResult.CompletedSynchronously)
                {
                    ServiceExecutor.ProcessClientClose(client, asyncResult);
                }
            }
            catch (TimeoutException)
            {
                client.Abort();
            }
            catch (CommunicationException)
            {
                client.Abort();
            }
        }

        /// <summary>
        /// Cria o client proxy para conexão com o serviço.
        /// </summary>
        /// <param name="clientTypeName">Nome do tipo do client.</param>
        /// <param name="endpointConfigurationName">Nome do endpoint.</param>
        /// <param name="client">Objeto de saída do client criado.</param>
        /// <returns></returns>
        private static Object ConstructClient(String clientTypeName, String endpointConfigurationName)
        {
            Type type = ClientSettings.ClientAssembly.GetType(clientTypeName);
            Object client = null;

            if (endpointConfigurationName == null)
            {
                client = type.GetConstructor(Type.EmptyTypes).Invoke(null);
            }
            else
            {
                client = type.GetConstructor(
                    new Type[] { typeof(String) }
                    ).Invoke(new Object[] { 
                            endpointConfigurationName 
                        });
            }

            return client;
        }

        /// <summary>
        /// Convert um array de Field em um array de ParameterInfo que será utilziado para a chamada do serviço.
        /// </summary>
        /// <param name="methodName">Nome do método que será chamado.</param>
        /// <param name="inputs">Array de Field com os parâmetros de entrada.</param>
        /// <param name="contractType">Tipo do contrato que será utilizado.</param>
        /// <param name="method">Obejto MethodInfo com as informações sobre o método que será chamado.</param>
        /// <param name="parameters">Array de ParameterInfo que será preenchido.</param>
        /// <param name="parameterArray">Array de dictionary com o nome do parameter e o valor que será passado na chamada do método.</param>
        private static void PopulateInputParameters(String methodName, Field[] inputs,
            Type contractType, out MethodInfo method, out ParameterInfo[] parameters, out Object[] parameterArray)
        {
            method = contractType.GetMethod(methodName);
            parameters = method.GetParameters();
            parameterArray = new Object[parameters.Length];
            IDictionary<String, Object> dictionary = ServiceExecutor.BuildParameters(inputs);

            for (Int32 i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameterInfo = parameters[i];
                if (parameterInfo.IsIn || !parameterInfo.IsOut)
                {
                    parameterArray[i] = dictionary[parameterInfo.Name];
                }
            }
        }

        /// <summary>
        /// Executa a chamada do serviço utilizando os valores de entrada e retorna um objeto com os valores de saída.
        /// </summary>
        /// <param name="inputValues">Objeto com os valores de entrada.</param>
        /// <returns>Obejtos com os valores de saída do serviço.</returns>
        private ServiceInvocationOutputs Execute(ServiceInvocationInputs inputValues)
        {
            ServiceInvocationOutputs result;

            lock (ServiceExecutor.lockObject)
            {
                String clientTypeName = inputValues.ClientTypeName;
                String contractTypeName = inputValues.ContractTypeName;
                String endpointConfigurationName = inputValues.EndpointConfigurationName;
                String methodName = inputValues.MethodName;
                Field[] inputs = inputValues.Inputs;
                Type type = ClientSettings.ClientAssembly.GetType(contractTypeName);
                MethodInfo methodInfo = null;
                ParameterInfo[] parametersInfo = null;
                Object[] parameters = null;

                ServiceExecutor.PopulateInputParameters(methodName, inputs, type, out methodInfo, out parametersInfo, out parameters);

                if (inputValues.StartNewClient || !ServiceExecutor.cachedProxies.ContainsKey(inputValues.ProxyIdentifier))
                {
                    this.ConstructClientToCache(inputValues.ProxyIdentifier, clientTypeName, endpointConfigurationName);
                }

                Object proxy = ServiceExecutor.cachedProxies[inputValues.ProxyIdentifier];
                Type baseType = proxy.GetType().BaseType;

                PropertyInfo property = baseType.GetProperty("Endpoint");
                ServiceEndpoint serviceEndpoint = (ServiceEndpoint)property.GetValue(proxy, null);

                Object returnMethod = null;

                try
                {
                    returnMethod = methodInfo.Invoke(proxy, parameters);
                }
                catch (ArgumentNullException argNullex)
                {
                    result = new ServiceInvocationOutputs();
                    result.HasError = true;
                    result.ErrorMessage = Util.FormatExceptionMessage(argNullex);
                    return result;

                }
                catch (Exception ex)
                {
                    result = new ServiceInvocationOutputs();
                    result.HasError = true;
                    result.ErrorMessage = Util.FormatExceptionMessage(ex);
                    return result;
                }

                IDictionary<String, Object> dictionary = new Dictionary<String, Object>();

                for (Int32 i = 0; i < parametersInfo.Length; i++)
                {
                    ParameterInfo parameterInfo = parametersInfo[i];
                    if (parameterInfo.ParameterType.IsByRef)
                    {
                        Object parameterByRef = parameters[i];
                        dictionary.Add(parameterInfo.Name, parameterByRef);
                    }
                }

                result = new ServiceInvocationOutputs(DataContractAnalyzer.BuildFieldInfos(returnMethod, dictionary));
            }

            return result;
        }
        
        /// <summary>
        /// Valida se é possivel criar um Dictionary com os ChildField do objeto field.
        /// </summary>
        /// <param name="field">Objeto a ser validado.</param>
        /// <returns>Lista dos campos que não foram válidos para o tipo de Dictionary.</returns>
        private IList<Int32> ValidateDictionary(DictionaryField field)
        {
            return field.ValidateDictionary();
        }

        #endregion
    }
}
