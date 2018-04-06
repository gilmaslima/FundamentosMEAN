using Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Request;
using Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Response;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Contratos
{
    [ServiceContract]
    public interface IConciliador
    {
        /// <summary>
        /// Consulta o ID do usuário baseado nos dados de entrada informado
        /// </summary>
        /// <param name="codGrupoEntidade"></param>
        /// <param name="codEntidade"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "consultausuario",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle=WebMessageBodyStyle.Wrapped)]
        UsuarioServico.Usuario ConsultarUsuarioEmail(string email);

        /// <summary>
        /// Retorna se o usuário atual do portal é um usuário do tipo atendimento.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "atendimento",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        bool VerificarUsuarioAtendimento();

        /// <summary>
        /// Consulta os serviços ativos do Conciliador.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "consultaservico",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ResponseBaseItem<ServicoAtivoResponse> ConsultarServicoAtivo(ServicoAtivoRequest request);

        /// <summary>
        /// Consulta os detalhes de regimes do serviço
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "consultaregime",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ResponseBaseList<ServicoRegimeResponse> ConsultarServicoRegime(ServicoRegimeRequest request);

        /// <summary>
        /// Serviço que contrata/cancela o serviço do Conciliador
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "contratarcancelar",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ResponseBase ContratarCancelarConciliador(ContratarCancelarRequest request);

        /// <summary>
        /// Serviço que envia o email de comprovante de contratação/cancelamento
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "comprovante",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        bool EnviarEmailComprovante(ComprovanteRequest request);

        /// <summary>
        /// Obtém o token da Login do Usuário
        /// </summary>
        /// <returns>JWT para o Login</returns>
        [OperationContract]
        [WebGet(UriTemplate = "singlesignon",
            ResponseFormat = WebMessageFormat.Json)]
        LoginResponse SingleSignOn();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract(Name = "SingleSignOnv2")]
        [WebGet(UriTemplate = "singlesignon/{codUsuario}/{atendimento}",
            ResponseFormat = WebMessageFormat.Json)]
        LoginResponse SingleSignOn(string codUsuario, string atendimento);
    }
}
