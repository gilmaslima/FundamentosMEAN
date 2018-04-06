using Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Response;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Contratos
{
    [ServiceContract]
    public interface IEntidade
    {
        /// <summary>
        /// Serviço que recupera a lista de entidades
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "filiais",
            ResponseFormat = WebMessageFormat.Json)]
        ListaEntidadesResponse ConsultarFiliais();
    }
}