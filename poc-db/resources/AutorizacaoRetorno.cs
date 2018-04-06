using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Redecard.PN.DadosCadastrais.SharePoint.PortalApi.Modelo
{
    /// <summary>
    /// Classe modelo de dados da Autorização do Usuário
    /// </summary>
    [DataContract]
    public class AutorizacaoRetorno
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "permissoes")]
        public List<PermissaoRetorno> Pemissoes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "menu")]
        public List<MenuRetorno> Menu { get; set; }
    }
}
