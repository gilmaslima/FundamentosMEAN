/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe ServiceProject
    /// </summary>
    [Serializable]
	public class ServiceProject
    {
        #region [Attributes and Properties]

        /// <summary>
        /// address
        /// </summary>
        private readonly String address;

        /// <summary>
        /// assemblyPath
        /// </summary>
        private readonly String assemblyPath;

        /// <summary>
        /// endpoints
        /// </summary>
        private readonly ICollection<ClientEndpointInfo> endpoints;

        /// <summary>
        /// foldName
        /// </summary>
        private String foldName;

        /// <summary>
        /// projectDirectory
        /// </summary>
        private String projectDirectory;

        /// <summary>
        /// Domain
        /// </summary>
        public AppDomain Domain { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public String Address
        {
            get
            {
                return this.address;
            }
        }

        /// <summary>
        /// Endpoints
        /// </summary>
        public ICollection<ClientEndpointInfo> Endpoints
        {
            get
            {
                return this.endpoints;
            }
        }

        #endregion

        #region [Constructors]

        /// <summary>
        /// Contrutor ServiceProject
        /// </summary>
        /// <param name="address">URL do endpoint</param>
        /// <param name="projectDirectory">Diretório raiz para geração do proxy e config</param>
        /// <param name="configPath">Caminho completo do arquivo de config</param>
        /// <param name="assemblyPath">Caminho completo do proxy.</param>
        /// <param name="domain">Client Domain</param>
        /// <param name="endpoints">Lista de ClientEndpointInfo que será preenchida.</param>
        public ServiceProject(String address, String projectDirectory, String configPath, String assemblyPath, AppDomain domain, ICollection<ClientEndpointInfo> endpoints)
        {
            this.address = address;
            this.projectDirectory = projectDirectory;
            this.endpoints = endpoints;
            this.assemblyPath = assemblyPath;
            this.foldName = Path.GetFileName(this.projectDirectory);
            this.UpdateAndValidateEndpointsInfo();
            this.Domain = domain;
        }
        
        #endregion

        #region [Methods]

        /// <summary>
        /// Método publico para apagar o Project Directory.
        /// </summary>
        public void Remove()
        {
            this.DeleteProjectFolder();
        }

        /// <summary>
        /// Método privado para apagar o Project Directory.
        /// </summary>
        private void DeleteProjectFolder()
        {
            Directory.Delete(this.projectDirectory, true);
        }

        /// <summary>
        /// Método que valida se o endpoint possui métodos e em caso positivo seta a propriedade ServiceProject
        /// </summary>
        private void UpdateAndValidateEndpointsInfo()
        {
            ClientEndpointInfo clientEndpointInfo = null;
            foreach (ClientEndpointInfo current in this.endpoints)
            {
                current.ProxyIdentifier = new StringBuilder().Append(this.foldName).Append(current.EndpointConfigurationName).ToString();
                if (current.Methods.Count < 1)
                {
                    current.InvalidReason = String.Format(CultureInfo.CurrentUICulture, "InvalidContract", new Object[]
					{
						current.OperationContractTypeName
					});
                    clientEndpointInfo = current;
                }
                else
                {
                    current.ServiceProject = this;
                }
            }
        }
        
        #endregion

	}
}
