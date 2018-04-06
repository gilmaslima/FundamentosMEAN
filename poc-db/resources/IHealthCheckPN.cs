#region Histórico do Arquivo
/*
(c) Copyright [2016] Redecard S.A.
Autor       : [Raphael Ivo]
Empresa     : [Iteris]
Histórico   :
- [19/09/2016] – [Raphael Ivo] – [Criação]
*/
#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Redecard.PN.Sustentacao.SharePoint
{
    [ServiceContract]
    interface IHealthCheckPN
    {
        [OperationContract]
        [WebGet(UriTemplate = "HealthCheckServicos/{servidor}", ResponseFormat = WebMessageFormat.Json)]
        List<Servico> HealthCheckServicos(string servidor);
    }
}
