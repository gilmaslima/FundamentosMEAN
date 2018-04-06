/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe ServiceInvocationInputs
    /// </summary>
	[Serializable]
	public class ServiceInvocationInputs
    {
        #region [Attributes and Properties]

        /// <summary>
        /// inputs
        /// </summary>
        private readonly Field[] inputs;

        /// <summary>
        /// clientTypeName
        /// </summary>
        private readonly String clientTypeName;

        /// <summary>
        /// contractTypeName
        /// </summary>
        private readonly String contractTypeName;

        /// <summary>
        /// domain
        /// </summary>
        private readonly AppDomain domain;

        /// <summary>
        /// endpointConfigurationName
        /// </summary>
        private readonly String endpointConfigurationName;

        /// <summary>
        /// methodName
        /// </summary>
        private readonly String methodName;

        /// <summary>
        /// proxyIdentifier
        /// </summary>
        private readonly String proxyIdentifier;

        /// <summary>
        /// startNewClient
        /// </summary>
        private readonly Boolean startNewClient;

        /// <summary>
        /// Inputs -- readonly
        /// </summary>
        public Field[] Inputs
        {
            get
            {
                return this.inputs;
            }
        }

        /// <summary>
        /// ClientTypeName -- readonly
        /// </summary>
        public String ClientTypeName
        {
            get
            {
                return this.clientTypeName;
            }
        }

        /// <summary>
        /// ContractTypeName -- readonly
        /// </summary>
        public String ContractTypeName
        {
            get
            {
                return this.contractTypeName;
            }
        }

        /// <summary>
        /// Domain -- readonly
        /// </summary>
        public AppDomain Domain
        {
            get
            {
                return this.domain;
            }
        }

        /// <summary>
        /// EndpointConfigurationName -- readonly
        /// </summary>
        public String EndpointConfigurationName
        {
            get
            {
                return this.endpointConfigurationName;
            }
        }

        /// <summary>
        /// MethodName -- readonly
        /// </summary>
        public String MethodName
        {
            get
            {
                return this.methodName;
            }
        }

        /// <summary>
        /// ProxyIdentifier -- readonly
        /// </summary>
        public String ProxyIdentifier
        {
            get
            {
                return this.proxyIdentifier;
            }
        }

        /// <summary>
        /// StartNewClient -- readonly
        /// </summary>
        public Boolean StartNewClient
        {
            get
            {
                return this.startNewClient;
            }
        }

        #endregion

        #region [Constructors]

        /// <summary>
        /// Construtor ServiceInvocationInputs
        /// </summary>
        /// <param name="inputs">Array de field com os valores de entrada.</param>
        /// <param name="method">Nome do método que será chamado com o valores de entrada.</param>
        /// <param name="appDomain">Client Domain</param>
        /// <param name="startNewClient">Flag que indica de deve instanciar um novo proxy.</param>
        public ServiceInvocationInputs(Field[] inputs, ServiceMethodInfo method, AppDomain appDomain, Boolean startNewClient)
        {
            ClientEndpointInfo endpoint = method.Endpoint;
            this.clientTypeName = endpoint.ClientTypeName;
            this.contractTypeName = endpoint.OperationContractTypeName;
            this.endpointConfigurationName = endpoint.EndpointConfigurationName;
            this.proxyIdentifier = endpoint.ProxyIdentifier;
            this.methodName = method.MethodName;
            this.inputs = inputs;
            this.startNewClient = startNewClient;
            this.domain = appDomain;
        }

        #endregion
	}
}
