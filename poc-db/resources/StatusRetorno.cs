using System;
using System.Runtime.Serialization;

namespace Rede.PN.ApiLogin.Models.Response
{
    /// <summary>
    /// Classe modelo de status da resposta
    /// </summary>
    [DataContract]
    public class StatusRetorno
    {
        /// <summary>
        /// Código de Retorno
        /// </summary>
        [DataMember(Name="codigo")]
        public Int32 Codigo { get; set; }

        /// <summary>
        /// Mensagem de retorno
        /// </summary>
        [DataMember(Name = "mensagem")]
        public String Mensagem { get; set; }
    }
}