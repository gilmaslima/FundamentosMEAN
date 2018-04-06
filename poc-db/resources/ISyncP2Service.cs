#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Rentes]
Empresa     : [Iteris]
Histórico   :
- [24/04/2012] – [André Rentes] – [Criação]
*/
#endregion
using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Redecard.PN.SyncPass
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceContract]
    public interface ISyncP2Service
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nomeArquivoSenha"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(Exception))]
        [WebGet(BodyStyle = WebMessageBodyStyle.Bare,
                RequestFormat = WebMessageFormat.Json,
                ResponseFormat = WebMessageFormat.Json,
                UriTemplate = "retornar/{nomeArquivoSenha}")]
        String RetornaSenha(String nomeArquivoSenha);
    }
}
