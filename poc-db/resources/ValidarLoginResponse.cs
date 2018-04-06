using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Rede.PN.ApiLogin.Models.Response
{
    [DataContract]
    public class ValidacaoLoginResponse
    {
        /// <summary>
        /// Status de Retorno
        /// </summary>
        [DataMember(Name = "status_retorno")]
        public StatusRetorno StatusRetorno { get; set; }

        /// <summary>
        /// Lista de Entidades relacionadas ao login
        /// </summary>
        [DataMember(Name = "entidades")]
        public List<Entidade> Entidades { get; set; }

        /// <summary>
        /// Access Token do Login
        /// </summary>
        [DataMember(Name = "access_token")]
        public String AccessToken { get; set; }

        /// <summary>
        /// Tipo do Token
        /// </summary>
        [DataMember(Name = "token_type")]
        public String TokenType { get; set; }
    }
}