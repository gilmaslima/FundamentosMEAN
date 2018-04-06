using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Modelo;
using System.ServiceModel.Web;

namespace Redecard.PN.Extrato.Servicos
{
    [ServiceContract]
    public interface IDirfRest
    {
        #region WACA075 - Dirf.

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarExtratoDirf")]
        ConsultarExtratoDirfRetornoRest ConsultarExtratoDirf(ConsultarExtratoDirfEnvio envio);
        #endregion

        #region WACA079 - Dirf.
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebGet(
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarAnosBaseDirf")]
        ConsultarAnosBaseDirfRetornoRest ConsultarAnosBaseDirf();
        #endregion

        #region Resumo
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "Resumo")]
        ResumoRetornoListRest Resumo(ConsultarExtratoDirfResumoEnvio envio);
        #endregion
    }
}