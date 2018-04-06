using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.SharePoint.PortalApi.Modelo
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class PermissaoRetorno
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "descricao")]
        public string Descricao { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "caminho")]
        public string Caminho { get; set; }
    }
}
