using Rede.PN.SessaoPortal.SharePoint.SessaoPortal.Modelos;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Rede.PN.SessaoPortal.SharePoint.SessaoPortal
{
    [ServiceContract]
    public interface ISessaoServico
    {
        /// <summary>
        /// Renova a sessão Sharepoint do usuário
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "renovar",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ResponseBase RenovarSessao();

        /// <summary>
        /// Recupera os dados da sessão ativa do Sharepoint
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "consultar",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ResponseBaseItem<SessaoResponse> ConsultarSessao();

        /// <summary>
        /// Criptografa chave/valor recebido
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            Method = "POST",
            UriTemplate = "gerarqssegura",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ResponseBaseItem<String> GerarQsSegura(Dictionary<String, String> keys);

        /// <summary>
        /// Obtém as permissões de relatórios do módulo de Extrato
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(
            UriTemplate = "permissoesextrato",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ResponseBaseItem<Dictionary<String, Int32>> ObterPermissoesExtrato();

        [OperationContract]
        [WebInvoke(
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            Method = "OPTIONS",
            UriTemplate = "*",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        void Options();
    }
}