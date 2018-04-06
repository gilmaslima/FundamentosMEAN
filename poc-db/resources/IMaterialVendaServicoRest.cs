/*
© Copyright 2017 Rede S.A.
Autor : Mário Neto
Empresa : Iteris Software e Consultoria.
*/

using Redecard.PN.OutrosServicos.ContratoDados;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Contrato do serviço de Material de Venda
    /// </summary>
    [ServiceContract]
    public interface IMaterialVendaServicoRest
    {
        /// <summary>
        /// Lista as últimas remessas enviadas para o estabelecimento
        /// </summary>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarUltimasRemessas/{codigoPV}")]
        ResponseBaseList<Remessa> ConsultarUltimasRemessas(String codigoPV);

        /// <summary>
        /// Lista as próximas remessas enviadas para o estabelecimento
        /// </summary>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarProximasRemessas/{codigoPV}")]
        ResponseBaseList<Remessa> ConsultarProximasRemessas(String codigoPV);

        /// <summary>
        /// Consultar a composição de um Kit para o estabelecimento
        /// </summary>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarComposicaoKit/{codigoKit}")]
        ResponseBaseList<Material> ConsultarComposicaoKit(String codigoKit);

        /// <summary>
        /// Lista os motivos das remessas de Material de venda
        /// </summary>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarMotivos")]
        ResponseBaseList<Motivo> ConsultarMotivos();

        /// <summary>
        /// Método consultar kits de material de vendas para o estabelecimento
        /// </summary>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarKitsVendas/{codigoPV}")]
        ResponseBaseList<Kit> ConsultarKitsVendas(String codigoPV);

        /// <summary>
        /// Método consultar kits de material de sinalização para o estabelecimento
        /// </summary>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarKitsSinalizacao/{codigoPV}")]
        ResponseBaseList<Kit> ConsultarKitsSinalizacao(String codigoPV);

        /// <summary>
        /// Método consultar kits de material de tecnologia para o estabelecimento
        /// </summary>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarKitsTecnologia/{codigoPV}")]
        ResponseBaseList<Kit> ConsultarKitsTecnologia(String codigoPV);

        /// <summary>
        /// Método consultar kits de material de apoio para o estabelecimento
        /// </summary>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarKitsApoio/{codigoPV}")]
        ResponseBaseList<Kit> ConsultarKitsApoio(String codigoPV);

        /// <summary>
        /// Consulta a quantidade máxima de Kits de Sinalização que podem ser solicitadas
        /// </summary>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarQuantidadeMaximaKitsSinalizacao")]
        ResponseBaseItem<String> ConsultarQuantidadeMaximaKitsSinalizacao();

        /// <summary>
        /// Consulta a quantidade máxima de Kits de Apoio que podem ser solicitadas
        /// </summary>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarQuantidadeMaximaKitsApoio")]
        ResponseBaseItem<String> ConsultarQuantidadeMaximaKitsApoio();

        /// <summary>
        /// Inclui uma nova solicitação de material de venda/tecnologia/apoio/sinalização do
        /// estabelecimento especificado.
        /// </summary>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
           Method = "POST",
           BodyStyle = WebMessageBodyStyle.Bare,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
          UriTemplate = "IncluirKit")]
        ResponseBaseItem<Int32> IncluirKit(IncluirKitRequest request);

        /// <summary>
        /// Método que consulta todas as informações pertinente à rotina de solicitaçãoMateriais de maneira mresumida.
        /// </summary>
        [OperationContract, FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "GET",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
           UriTemplate = "ConsultarInfoMateriais/{codigoPV}")]
        ResponseBaseItem<InfoMateriaisResponse> ConsultarInfoMateriais(String codigoPV);
    }
}