using Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.ValoresPagos;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Valores Pagos.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1316 / Programa WA1316 / TranID ISHG / Método ConsultarCreditoTotalizadores<br/>
    /// - Book WACA1317 / Programa WA1317 / TranID ISHH / Método ConsultarCredito<br/>
    /// - Book WACA1318 / Programa WA1318 / TranID ISHI / Método ConsultarCreditoDetalheTotalizadores<br/>
    /// - Book WACA1319 / Programa WA1319 / TranID ISHJ / Método ConsultarCreditoDetalhe<br/>
    /// - Book WACA1320 / Programa WA1320 / TranID ISHK / Método ConsultarDebitoTotalizadores<br/>
    /// - Book WACA1321 / Programa WA1321 / TranID ISHL / Método ConsultarDebito<br/>
    /// - Book WACA1322 / Programa WA1322 / TranID ISHM / Método ConsultarDebitoDetalheTotalizadores<br/>
    /// - Book WACA1323 / Programa WA13 23 / TranID ISHN / Método ConsultarDebitoDetalhe
    /// </remarks>
    [ServiceContract]
    public interface IHISServicoWAExtratoValoresPagosRest
    {
        /// <summary>
        /// Consulta de Relatório de Valores Pagos - Crédito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1316 / Programa WA1316 / TranID ISHG<br/>
        /// - Book WACA1317 / Programa WA1317 / TranID ISHH
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1316 / Programa WA1316 / TranID ISHG<br/>
        /// - Book WACA1317 / Programa WA1317 / TranID ISHH
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "RelatorioCredito")]
        RelatorioValoresPagosCreditoResponse ConsultarRelatorioCredito(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de Relatório de Valores Pagos - Crédito - Detalhe<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1318 / Programa WA1318 / TranID ISHI<br/>
        /// - Book WA1319 / Programa WA1319 / TranID ISHJ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1318 / Programa WA1318 / TranID ISHI<br/>
        /// - Book WA1319 / Programa WA1319 / TranID ISHJ
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataRecebimento">Data de recebimento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "RelatorioCreditoDetalhe")]
        RelatorioValoresPagosCreditoDetalheResponse ConsultarRelatorioCreditoDetalhe(
            Int32 codigoBandeira,
            String dataRecebimento,
            Int32 pv,
            Int32 numeroOcu,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de Relatório de Valores Pagos - Débito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1320 / Programa WA1320 / TranID ISHK<br/>
        /// - Book WACA1321 / Programa WA1321 / TranID ISHL
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1320 / Programa WA1320 / TranID ISHK<br/>
        /// - Book WACA1321 / Programa WA1321 / TranID ISHL
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "RelatorioDebito")]
        RelatorioValoresPagosDebitoResponse ConsultarRelatorioDebito(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de Relatório de Valores Pagos - Débito - Detalhe<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1322 / Programa WA1322 / TranID ISHM<br/>
        /// - Book WA1323 / Programa WA1323 / TranID ISHN
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1322 / Programa WA1322 / TranID ISHM<br/>
        /// - Book WA1323 / Programa WA1323 / TranID ISHN
        /// </remarks>
        /// <param name="dataPagamento">Data de pagamento</param>
        /// <param name="pvs">Código do estabelecimento</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>

        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, 
            BodyStyle = WebMessageBodyStyle.WrappedRequest, 
            UriTemplate = "RelatorioDebitoDetalhe")]
        RelatorioValoresPagosDebitoDetalheResponse ConsultarRelatorioDebitoDetalhe(
            String dataPagamento,
            Int32 pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Valores Pagos - Crédito.<br/>
        /// - Book WACA1316 / Programa WA1316 / TranID ISHG
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1316 / Programa WA1316 / TranID ISHG
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Crédito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, 
            UriTemplate = "TotalizadoresCredito")]
        ResponseBaseItem<CreditoTotalizador> ConsultarCreditoTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de registros do Relatório de Valores Pagos - Crédito.<br/>
        /// - Book WACA1317 / Programa WA1317 / TranID ISHH
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1317 / Programa WA1317 / TranID ISHH
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Valores Pagos - Crédito.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, 
            UriTemplate = "Credito")]
        ResponseBaseList<Credito> ConsultarCredito(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Valores Pagos - Crédito - Detalhe.<br/>
        /// - Book WACA1318 / Programa WA1318 / TranID ISHI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1318 / Programa WA1318 / TranID ISHI
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataRecebimento">Data de recebimento</param>        
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Crédito - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json, 
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "TotalizadoresCreditoDetalhe")]
        ResponseBaseItem<CreditoDetalheTotalizador> ConsultarCreditoDetalheTotalizadores(
            Int32 codigoBandeira,
            String dataRecebimento,
            Int32 pv,
            Int32 numeroOcu);

        /// <summary>
        /// Consulta de registros do Relatório de Valores Pagos - Crédito - Detalhe.<br/>
        /// - Book WACA1319 / Programa WA1319 / TranID ISHJ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1319 / Programa WA1319 / TranID ISHJ
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataRecebimento">Data de recebimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <returns>Registros do Relatório de Valores Pagos - Crédito - Detalhe.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json, 
            RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest, 
            UriTemplate = "CreditoDetalhe")]
        ResponseBaseList<CreditoDetalhe> ConsultarCreditoDetalhe(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            String dataRecebimento,
            Int32 pv,
            Int32 numeroOcu);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Valores Pagos - Débito.<br/>
        /// - Book WACA1320 / Programa WA1320 / TranID ISHK
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1320 / Programa WA1320 / TranID ISHK
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json, 
            RequestFormat =  WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest, 
            UriTemplate = "TotalizadoresDebito")]
        ResponseBaseItem<DebitoTotalizador> ConsultarDebitoTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de registros do Relatório de Valores Pagos - Débito.<br/>
        /// - Book WACA1321 / Programa WA1321 / TranID ISHL
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1321 / Programa WA1321 / TranID ISHL
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Valores Pagos - Débito.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "Debito")]
        ResponseBaseList<Debito> ConsultarDebito(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Valores Pagos - Débito - Detalhe.<br/>
        /// - Book WACA1322 / Programa WA1322 / TranID ISHM
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1322 / Programa WA1322 / TranID ISHM
        /// </remarks>
        /// <param name="dataPagamento">Data de pagamento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Débito - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "TotalizadoresDebitoDetalhe")]
        ResponseBaseItem<DebitoDetalheTotalizador> ConsultarDebitoDetalheTotalizadores(
            String dataPagamento,
            Int32 pv);

        /// <summary>
        /// Consulta de registros do Relatório de Valores Pagos - Débito - Detalhe.<br/>
        /// - Book WACA1323 / Programa WA1323 / TranID ISHN
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1323 / Programa WA1323 / TranID ISHN
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="dataPagamento">Data de pagamento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <returns>Registros do Relatório de Valores Pagos - Débito - Detalhe.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "DebitoDetalhe")]
        ResponseBaseList<DebitoDetalhe> ConsultarDebitoDetalhe(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            String dataPagamento,
            Int32 pv);
    }
}
