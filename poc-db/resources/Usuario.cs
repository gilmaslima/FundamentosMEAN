using System;
using System.Runtime.Serialization;

namespace Rede.PN.ApiLogin.Models
{
    /// <summary>
    /// Classe Modelo de Usuário
    /// </summary>
    [DataContract]
    public class Usuario
    {
        /// <summary>
        /// E-mail do usuário
        /// </summary>
        [DataMember(Name="email")]
        public String Email { get; set; }

        /// <summary>
        /// Senha do usuário
        /// </summary>
        [DataMember(Name = "senha")]
        public String Senha { get; set; }

        /// <summary>
        /// Nome do usuário
        /// </summary>
        [DataMember(Name = "nome")]
        public String Nome { get; set; }
    }
}