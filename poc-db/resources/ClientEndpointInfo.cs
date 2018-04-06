/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe ClientEndpointInfo
    /// </summary>
	[Serializable]
	public class ClientEndpointInfo
    {
        #region [Attributes and Properties]

        /// <summary>
        /// operationContractTypeName
        /// </summary>
        private readonly String operationContractTypeName;

        /// <summary>
        /// methods
        /// </summary>
        private List<ServiceMethodInfo> methods = new List<ServiceMethodInfo>();

        /// <summary>
        /// InvalidReason
        /// </summary>
        public String InvalidReason { get; set; }

        /// <summary>
        /// Valid
        /// </summary>
        public Boolean Valid { get; set; }

        /// <summary>
        /// ClientTypeName
        /// </summary>
        public String ClientTypeName { get; set; }

        /// <summary>
        /// EndpointConfigurationName
        /// </summary>
        public String EndpointConfigurationName { get; set; }

        /// <summary>
        /// Methods
        /// </summary>
        public List<ServiceMethodInfo> Methods
        {
            get { return this.methods; }
            set { this.methods = value; }       
        }

        /// <summary>
        /// OperationContractTypeName
        /// </summary>
        public String OperationContractTypeName 
        {
            get 
            {
                return this.operationContractTypeName;
            }
        }

        /// <summary>
        /// ProxyIdentifier
        /// </summary>
        public String ProxyIdentifier { get; set; }

        /// <summary>
        /// ServiceProject
        /// </summary>
        public ServiceProject ServiceProject { get; set; }

        #endregion

        #region [Constructors]
        /// <summary>
        /// Construtor ClientEndpointInfo
        /// </summary>
        /// <param name="operationContractTypeName">Nome do tipo do OperationContract.</param>
        public ClientEndpointInfo(String operationContractTypeName)
        {
            this.operationContractTypeName = operationContractTypeName;
        }
        #endregion

        #region [Methods]
        /// <summary>
        /// Retorna o nome do OperationContract formatado com o EndpointConfigurationName.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            if (this.EndpointConfigurationName == null)
            {
                return this.OperationContractTypeName;
            }

            return String.Format(CultureInfo.CurrentUICulture, "{0} ({1})", new Object[]
			{
				this.OperationContractTypeName,
				this.EndpointConfigurationName
			});
        }  
      
        #endregion
	}
}
