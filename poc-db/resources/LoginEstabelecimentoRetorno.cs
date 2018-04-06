using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Redecard.PN.DadosCadastrais.SharePoint.PortalApi.Modelo
{
    /// <summary>
    /// Classe para serialização completa do retorno JSON da API
    /// </summary>
    [DataContract]
    public class LoginEstabelecimentoRetorno
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember (Name = "usuario")]
        public UsuarioRetorno Usuario { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "entidade")]
        public EstabelecimentoRetorno Entidade { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "autorizacao")]
        public AutorizacaoRetorno Autorizacao { get; set; }

        /// <summary>
        /// Token de Acesso para validação do OATH2 para chamadas na API
        /// </summary>
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Tipo do Token para validação do OATH2 para chamadas na API
        /// </summary>
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }
    }
}
