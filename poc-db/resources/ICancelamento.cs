using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Rede.PN.Cancelamento.Servicos
{
    /// <summary>
    /// Interface dos serviços do cancelamento
    /// </summary>
    [ServiceContract]
    public interface ICancelamento
    {
        /// <summary>
        /// Expõe método de busca de cancelamentos no serviço
        /// </summary>
        /// <param name="codigoUsuario">Código do usúario</param>
        /// <param name="numeroEstabelecimento">Número do estabelecimento</param>
        /// <param name="indicadorPesquisa">Indicador do tipo de pesquisa</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="numeroAvisoCancelamento">Número do aviso de cancelamento</param>
        /// <param name="numeroNsu">Número de sequência único</param>
        /// <param name="tipoVenda">Tipo de venda</param>
        /// <returns>Retorna uma lista de cancelamentos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method ="POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Modelo.SolicitacaoCancelamento> BuscarCancelamentos(String codigoUsuario,
                    Int32 numeroEstabelecimento,
                    String indicadorPesquisa,
                    DateTime dataInicial,
                    DateTime dataFinal,
                    Int64 numeroAvisoCancelamento,
                    Int64 numeroNsu,
                    Modelo.TipoVenda tipoVenda);

        /// <summary>
        /// Expõe método de inclusão de cancelamentos
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="cancelamentos">Cancelamentos para inclusão</param>
        /// <returns>Retorna lista de cancelamentos incluídos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Modelo.SolicitacaoCancelamento> IncluirCancelamentos(String codigoUsuario, List<Modelo.SolicitacaoCancelamento> cancelamentos);

        /// <summary>
        /// Expõe método que valida se os cancelamentos podem ser efetuados
        /// </summary>
        /// <param name="tipoOperacao">Tipo de operação - 'B' = validação</param>
        /// <param name="cancelamentos">Cancelamentos para validação</param>
        /// <returns>Retorna uma lista de resultados da validação</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Modelo.Validacao> ValidarParametrosBloqueio(Char tipoOperacao, List<Modelo.SolicitacaoCancelamento> cancelamentos);

        /// <summary>
        /// Expõe método de busca dos detalhes de uma transação para cancelamento
        /// </summary>
        /// <param name="cancelamento">Cancelamento</param>
        /// <returns>Retorna o cancelamento buscado com mais informações</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Modelo.SolicitacaoCancelamento BuscarTransacaoParaCancelamento(Modelo.SolicitacaoCancelamento cancelamento);

        /// <summary>
        /// Expõe método de busca de transações duplicadas para cancelamento
        /// </summary>
        /// <param name="cancelamento">Cancelamento</param>
        /// <returns>Retorna lista de cancelamentos do mesmo ponto de venda, data de transação, tipo de transação e nsu/cartão</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Modelo.SolicitacaoCancelamento> BuscarTransacaoDuplicadaParaCancelamento(Modelo.SolicitacaoCancelamento cancelamento);

        /// <summary>
        /// Expõe método de anular cancelamentos
        /// </summary>
        /// <param name="codigoUsuario">Código do usuário</param>
        /// <param name="cancelamentos">Cancelamentos para anulação</param>
        /// <returns>Retorna uma lista de resultados das anulações</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Modelo.Validacao> AnularCancelamento(String codigoUsuario, Int16 codigoCanal, List<Modelo.SolicitacaoCancelamento> cancelamentos);

        /// <summary>
        /// Expõe método que verifica se um ponto de venda é filial de uma dada matriz
        /// </summary>
        /// <param name="numeroEstabelecimento">Número do estabelecimento</param>
        /// <param name="numeroMatriz">Número da matriz</param>
        /// <returns>Retorna verdadeiro quando o número do estabelecimento é filial da matriz informada</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Boolean VerificarEstabelecimentoEmMatriz(Int32 numeroEstabelecimento, Int32 numeroMatriz);

        /// <summary>
        /// Expõe método de busca de cancelamentos desfeitos por período
        /// </summary>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="numeroAvisoCancelamento">Número do aviso de cancelamento</param>
        /// <param name="numeroCartao">Número do cartão</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <returns>Retorna lista de cancelamentos defeitos pelo FMS</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Modelo.SolicitacaoCancelamento> BuscaCancelamentosDesfeitosPorPeriodo(DateTime dataInicial, DateTime dataFinal, Decimal numeroAvisoCancelamento, String numeroCartao, Int32 numeroPontoVenda);

        /// <summary>
        /// Expõe método de busca de cancelamentos desfeitos por ponto de venda e número do aviso
        /// </summary>
        /// <param name="numeroAvisoCancelamento">Número do aviso de cancelamento</param>
        /// <param name="numeroCartao">Número do cartão</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <returns>Retorna lista de cancelamentos defeitos pelo FMS</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Modelo.SolicitacaoCancelamento> BuscaCancelamentosDesfeitosPorPontoVendaENumeroAviso(Decimal numeroAvisoCancelamento, String numeroCartao, Int32 numeroPontoVenda);

        /// <summary>
        /// Incluir Lista de Solicitação de Cancelamento no Banco de dados PN
        /// </summary>
        /// <param name="lstSolicitacaoCancelamento">Lista de Solicitação de Cancelamento</param>
        /// <param name="ip">valor Ip da máquina que solicitou o cancelamento</param>
        /// <param name="usuario">usuário que solicitou o cancelamento</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        void IncluirCancelamentosPn(List<Modelo.SolicitacaoCancelamento> lstSolicitacaoCancelamento, String ip, String usuario);

        /// <summary>
        /// Consulta Cancelamentos PN
        /// </summary>
        /// <param name="tipoTransacao">Tipo de transação</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data Final</param>
        /// <param name="ip">ip</param>
        /// <returns>Retorna lista de cancelamentos PN</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        List<Modelo.CancelamentoPn> ConsultarCancelamentosPn(String tipoTransacao, Int32 numeroPontoVenda, DateTime dataInicial, DateTime dataFinal, String ip);

        /// <summary>
        /// Anula cancelamentos PN
        /// </summary>
        /// <param name="listaNumerosAvisos">Lista de números de avisos</param>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        /// <param name="usuario">Usuário</param>
        /// <param name="ip">Ip</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        void AnularCancelamentosPn(List<Int64> listaNumerosAvisos, Int32 numeroPontoVenda, String usuario, String ip);
    }
}
