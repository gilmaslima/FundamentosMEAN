using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Sustentacao.Modelo
{
    /// <summary>
    /// Classe ProxyInfo
    /// </summary>
    [DataContract]
    public class ProxyInfo
    {
        /// <summary>
        /// Código CS para criação do client proxy.
        /// </summary>
        [DataMember]
        public String Class { get; set; }

        /// <summary>
        /// XML para a criação do web.config do client proxy.
        /// </summary>
        [DataMember]
        public String Config { get; set; } 
    }
}