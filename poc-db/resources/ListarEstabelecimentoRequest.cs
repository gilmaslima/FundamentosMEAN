using System.Runtime.Serialization;

namespace Rede.PN.ApiLogin.Sharepoint.ISAPI.Login.Models.Request
{
    [DataContract]
    public class ListarEstabelecimentoRequest
    {
        /// <summary>
        /// Dados do Usuário (E-mail e Senha obrigatórios)
        /// </summary>
        [DataMember(IsRequired = true)]
        public Usuario Usuario { get; set; }
        
    }
}