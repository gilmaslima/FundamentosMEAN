/*
© Copyright 2016 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software.
*/

using Redecard.PN.Sustentacao.SharePoint.SustentacaoAdministracaoServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Redecard.PN.Sustentacao.AdministracaoDados
{
    /// <summary>
    /// Classe ServiceSession
    /// </summary>
    [Serializable]
    public class ServiceSession
    {
        /// <summary>
        /// EndpointAddress
        /// </summary>
        public String EndpointAddress { get; set; }

        /// <summary>
        /// Proxy
        /// </summary>
        public ProxyInfo Proxy { get; set; }

        /// <summary>
        /// ProjectDirectory
        /// </summary>
        public String ProjectDirectory { get; set; }
    }
}