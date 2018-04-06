/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Interface do Serviço de Ofertas do PN
    /// </summary>
    [ServiceContract]
    public interface IServicoOferta
    {

        /// <summary>
        /// Consultar Contratos/Ofertas através do UK
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Servicos.PlanoContas.Oferta> ConsultarOfertas(Int32 codigoEntidade);

        /// <summary>
        /// Verificar se algum dos PVs passados possui Oferta de Taxa
        /// </summary>
        /// <param name="pvs">Lista de PVs para verificar</param>
        /// <returns>
        /// <para>True - 1 ou mais dos PVs possui Oferta de Taxa</para>
        /// <para>False - nenhum dos PVs possui Oferta de Taxa</para>
        /// </returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Boolean PossuiOfertaTaxa(List<Int32> pvs);

        /// <summary>
        /// Consultar o Contrato da Oferta
        /// </summary>
        /// <param name="codigoOferta">Código da Oferta a consultar o Contrato</param>
        /// <param name="codigoProposta">Código de Proposta da Oferta</param>
        /// <param name="codigoEntidade">Código da Entidade</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ContratoOferta ConsultarContratoOferta(
            Int32 codigoOferta,
            Int64 codigoProposta,
            Int64 codigoEntidade);

        /// <summary>
        /// Obter listagem de Sazonalidades da Oferta no UK
        /// </summary>
        /// <param name="codigoOferta">Código da Oferta</param>
        /// <param name="codigoContrato">Código do Contrato</param>
        /// <param name="codigoEstruturaMeta">Código de Estrutura Meta</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<SazonalidadeOferta> ConsultarSazonalizades(
            Int32 codigoOferta,
            Int64 codigoContrato,
            Int32 codigoEstruturaMeta);

        /// <summary>
        /// Listar os ramos da Oferta do Estabelecimento
        /// </summary>
        /// <param name="codigoOferta"></param>
        /// <param name="codigoProposta"></param>
        /// <param name="numeroCnpj"></param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<RamosAtividadeOferta> ConsultarRamosOferta(
            Int64 codigoOferta,
            Int64 codigoProposta,
            Int64 numeroCnpj);

        /// <summary>
        /// Consultar as faixas de meta da Oferta
        /// </summary>
        /// <param name="contrato">informações do Contrato da Oferta</param>
        /// <param name="sazonalidade">Sazonalidade selecionadada da Oferta</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<FaixaMetaOferta> ConsultarFaixasMeta(
            ContratoOferta contrato,
            SazonalidadeOferta sazonalidade);

        /// <summary>
        /// Consultar as Taxas Débito da Meta de uma Oferta
        /// </summary>
        /// <param name="contrato">Contrato da Oferta</param>
        /// <param name="sazonalidade">Sazonalidade da Oferta</param>
        /// <param name="numeroEstabelecimento">Número do Estabelecimento</param>
        /// <param name="codigoRamo">Código do Ramo</param>
        /// <param name="codigoFaixa">Código da Faixa</param>
        /// <returns>List of Modelo.ProdutoBandeiraMeta</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<ProdutoBandeiraMeta> ConsultarTaxasDebito(
            ContratoOferta contrato,
            SazonalidadeOferta sazonalidade,
            Int64 numeroEstabelecimento,
            Int32? codigoRamo,
            Int32 codigoFaixa);

        /// <summary>
        /// Consultar as Taxas Débito da Meta de uma Oferta
        /// </summary>
        /// <param name="contrato">Contrato da Oferta</param>
        /// <param name="sazonalidade">Sazonalidade da Oferta</param>
        /// <param name="numeroEstabelecimento">Número do Estabelecimento</param>
        /// <param name="codigoRamo">Código do Ramo</param>
        /// <param name="codigoFaixa">Código da Faixa</param>
        /// <returns>List of Modelo.ProdutoBandeiraMeta</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<ProdutoBandeiraMeta> ConsultarTaxasCredito(
            ContratoOferta contrato,
            SazonalidadeOferta sazonalidade,
            Int64 numeroEstabelecimento,
            Int32? codigoRamo,
            Int32 codigoFaixa);

        /// <summary>
        /// Listar os históricos da Oferta
        /// <param name="contrato">Contrato com as informações de Código de Proposta e Código de Oferta</param>
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<HistoricoOferta> ConsultarHistoricoOferta(ContratoOferta contrato);

        /// <summary>
        /// Listar os estabelecimentos dos históricos da Oferta no UK
        /// </summary>
        /// <param name="historico">Informações do Histórico da Ofeta</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<EstabelecimentoHistoricoOferta> ConsultarEstabelecimentosOferta(HistoricoOferta historico);

        /// <summary>
        /// Consultar as Taxas Crédito da Meta do Histórico da Oferta no UK
        /// </summary>
        /// <param name="historico">Histórico</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<ProdutoBandeiraMeta> ConsultarTaxasCreditoHistorico(HistoricoOferta historico);

        /// <summary>
        /// Consultar as Taxas Débito da Meta do Histórico da Oferta no UK
        /// </summary>
        /// <param name="historico">Histórico</param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<ProdutoBandeiraMeta> ConsultarTaxasDebitoHistorico(HistoricoOferta historico);
    }
}
