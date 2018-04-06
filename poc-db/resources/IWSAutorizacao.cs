using System.ServiceModel;

namespace Redecard.PN.DadosCadastrais.SharePoint.ISAPI.WSAutorizacao
{
    /// <summary>
    /// Interface de implementação do Serviço de autorização de usuários
    /// </summary>
    [ServiceContract]
    public interface IWSAutorizacao
    {
        /// <summary>
        /// Interface do método que autoriza usuários a acessarem o portal da Fielo
        /// </summary>
        /// <param name="cnpj"></param>
        /// <param name="pv"></param>
        /// <param name="hash"></param>
        /// <param name="appFabric"></param>
        /// <returns></returns>
        [OperationContract]
        bool AutorizarAcessoUsuario(string pv, string hash, string appFabric);
    }
}
