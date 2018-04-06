using Rede.PN.ApiLogin.Sharepoint.ISAPI.Login.Models.Request;
using Rede.PN.ApiLogin.Sharepoint.ISAPI.Login.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Rede.PN.ApiLogin.Sharepoint.ISAPI.Login
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract]
    public interface ILoginServico
    {
        /// <summary>
        /// Validar o login do usuário
        /// </summary>
        /// <param name="modeloRequisicao">Objeto com as informações de login do usuário</param>
        /// <returns>Objeto com as informações de login</returns>
        [OperationContract]
        [WebInvoke(Method = "POST", 
                   UriTemplate = "login",
                   BodyStyle = WebMessageBodyStyle.Bare,
                   RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json)]
        ValidacaoLoginResponse ValidarLogin(LoginConciliadorRequest modeloRequisicao);

        /// <summary>
        /// Valida um token recebido
        /// </summary>
        /// <returns>Conteúdo do Token</returns>
        [OperationContract]
        [WebGet(UriTemplate = "login",
                BodyStyle = WebMessageBodyStyle.Bare,
                ResponseFormat = WebMessageFormat.Json)]
        ValidacaoLoginResponse ValidarToken();

        /// <summary>
        /// Obtém um token válido para o usuário com ID passado
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>JWT para o usuário</returns>
        [OperationContract]
        [WebGet(UriTemplate = "login/usuario/{id}",
                BodyStyle = WebMessageBodyStyle.Bare,               
                ResponseFormat = WebMessageFormat.Json)]
        String Token(String id);

        /// <summary>
        /// Obtém um token válido para o usuário com ID passado
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>JWT para o usuário</returns>
        [OperationContract(Name = "TokenAtendimento")]
        [WebGet(UriTemplate = "login/usuario/{id}/{atendimento}/{funcional}",
                BodyStyle = WebMessageBodyStyle.Bare,
                ResponseFormat = WebMessageFormat.Json)]
        String Token(String id, string atendimento, string funcional);
    }
}
