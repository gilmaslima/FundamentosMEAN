/*
© Copyright 2017 Rede S.A.
Autor	: Francisco Mazali
Empresa	: Iteris Consultoria e Software LTDA
*/

using Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response;
using Redecard.PN.Extrato.Servicos.LancamentosFuturos;
using Redecard.PN.Extrato.Servicos.Modelo;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Lançamentos Futuros.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1324 / Programa WA1324 / TranID ISHO / Método ConsultarCreditoTotalizadores<br/>
    /// - Book WACA1325 / Programa WA1325 / TranID ISHP / Método ConsultarCredito<br/>
    /// - Book WACA1326 / Programa WA1326 / TranID ISHQ / Método ConsultarCreditoDetalheTotalizadores<br/>
    /// - Book WACA1327 / Programa WA1327 / TranID ISHR / Método ConsultarCreditoDetalhe<br/>
    /// - Book WACA1328 / Programa WA1328 / TranID ISHS / Método ConsultarDebitoTotalizadores<br/>
    /// - Book WACA1329 / Programa WA1329 / TranID ISHT / Método ConsultarDebito
    /// </remarks>
    [ServiceContract]
    [ServiceKnownType(typeof(CreditoTotalizador))]
    [ServiceKnownType(typeof(CreditoTotalizadorTotais))]
    [ServiceKnownType(typeof(CreditoTotalizadorValor))]
    [ServiceKnownType(typeof(Credito))]
    [ServiceKnownType(typeof(CreditoDetalheTotalizador))]
    [ServiceKnownType(typeof(CreditoDetalheTotalizadorTotais))]
    [ServiceKnownType(typeof(CreditoDetalheTotalizadorValor))]
    [ServiceKnownType(typeof(CreditoDetalhe))]
    [ServiceKnownType(typeof(CreditoDetalheA1))]
    [ServiceKnownType(typeof(CreditoDetalheA2))]
    [ServiceKnownType(typeof(CreditoDetalheDT))]
    [ServiceKnownType(typeof(DebitoTotalizador))]
    [ServiceKnownType(typeof(DebitoTotalizadorTotais))]
    [ServiceKnownType(typeof(DebitoTotalizadorValor))]
    [ServiceKnownType(typeof(Debito))]
    [ServiceKnownType(typeof(DebitoA1))]
    [ServiceKnownType(typeof(DebitoA2))]
    [ServiceKnownType(typeof(DebitoDT))]
    public interface IHISServicoWAExtratoLancamentosFuturosRest
    {
        /// <summary>
        /// Consulta de Relatório de Lançamentos Futuros - Crédito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1324 / Programa WA1324 / TranID ISHO<br/>
        /// - Book WACA1325 / Programa WA1325 / TranID ISHP
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1324 / Programa WA1324 / TranID ISHO<br/>
        /// - Book WACA1325 / Programa WA1325 / TranID ISHP
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
        RelatorioLancamentosFuturosCreditoResponse ConsultarRelatorioCredito(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de Relatório de Lançamentos Futuros - Crédito - Detalhe<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1326 / Programa WA1326 / TranID ISHQ<br/>
        /// - Book WA1327 / Programa WA1327 / TranID ISHR
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1326 / Programa WA1326 / TranID ISHQ<br/>
        /// - Book WA1327 / Programa WA1327 / TranID ISHR
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataVencimento">Data de vencimento</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "RelatorioCreditoDetalhe")]
        RelatorioLancamentosFuturosCreditoDetalheResponse ConsultarRelatorioCreditoDetalhe(
            Int32 codigoBandeira,
            String dataVencimento,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,
            Int32 quantidadeTotalRegistros);
        
        /// <summary>
        /// Consulta de Relatório de Lançamentos Futuros - Débito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1328 / Programa WA1328 / TranID ISHS<br/>
        /// - Book WACA1329 / Programa WA1329 / TranID ISHT
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1328 / Programa WA1328 / TranID ISHS<br/>
        /// - Book WACA1329 / Programa WA1329 / TranID ISHT
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo de registro a ser retornado</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "RelatorioDebito")]
        RelatorioLancamentosFuturosDebitoResponse ConsultarRelatorioDebito(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            DebitoTipoRegistro tipoRegistro,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Lançamentos Futuros - Crédito.<br/>
        /// - Book WACA1324 / Programa WA1324 / TranID ISHO
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1324 / Programa WA1324 / TranID ISHO
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Crédito</returns>
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
        /// Consulta de registros do Relatório de Lançamentos Futuros - Crédito.<br/>
        /// - Book WACA1325 / Programa WA1325 / TranID ISHP
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1325 / Programa WA1325 / TranID ISHP
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Lançamentos Futuros - Crédito.</returns>
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
        /// Consulta de totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe.<br/>
        /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataVencimento">Data de vencimento</param>        
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "TotalizadoresCreditoDetalhe")]
        ResponseBaseItem<CreditoDetalheTotalizador> ConsultarCreditoDetalheTotalizadores(
            Int32 codigoBandeira,
            String dataVencimento,
            List<Int32> pvs);
        
        /// <summary>
        /// Consulta de registros do Relatório de Lançamentos Futuros - Crédito - Detalhe.<br/>
        /// - Book WACA1327 / Programa WA1327 / TranID ISHR
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1327 / Programa WA1327 / TranID ISHR
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataVencimento">Data de vencimento</param>        
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Lançamentos Futuros - Crédito - Detalhe.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "CreditoDetalhe")]
        ResponseBaseList<CreditoDetalhe> ConsultarCreditoDetalhe(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            String dataVencimento,
            List<Int32> pvs);
        
        /// <summary>
        /// Consulta de totalizadores do Relatório de Lançamentos Futuros - Débito.<br/>
        /// - Book WACA1328 / Programa WA1328 / TranID ISHS
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1328 / Programa WA1328 / TranID ISHS
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "TotalizadoresDebito")]
        ResponseBaseItem<DebitoTotalizador> ConsultarDebitoTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de registros do Relatório de Lançamentos Futuros - Débito.<br/>
        /// - Book WACA1329 / Programa WA1329 / TranID ISHT
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1329 / Programa WA1329 / TranID ISHT
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Lançamentos Futuros - Débito.</returns>
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
            DebitoTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);
    }
}