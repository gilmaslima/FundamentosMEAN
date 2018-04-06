using System;
using System.Runtime.Serialization;

namespace Rede.PN.ApiLogin.Sharepoint.ISAPI.Login.Models.Request
{
    /// <summary>
    /// Classe modelo de requisição de validação de um login
    /// </summary>
    [DataContract]
    public class LoginConciliadorRequest
    {
        /// <summary>
        /// Dados do Usuário (E-mail)
        /// </summary>
        [DataMember(IsRequired = true, Name="usuario")]
        public String Usuario { get; set; }

        /// <summary>
        /// Dados do Usuário (Senha)
        /// </summary>
        [DataMember(IsRequired = true, Name = "senha")]
        public String Senha { get; set; }
        
        /// <summary>
        /// Entidade/Estabelecimento do usuário (Obrigatório
        /// </summary>
        [DataMember(IsRequired = true, Name = "grupo_entidade")]
        public Int32 GrupoEntidade { get; set; }

        /// <summary>
        /// Chave de acesso do Cliente/Canal
        /// </summary>
        [DataMember(IsRequired = false, Name = "codigo_cliente")]
        public String CodigoCliente { get; set; }
    }
}