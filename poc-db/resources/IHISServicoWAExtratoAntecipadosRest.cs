/*
© Copyright 2017 Rede S.A.
Autor	: Francisco Mazali
Empresa	: Iteris Consultoria e Software LTDA
*/

using Redecard.PN.Extrato.Servicos.AntecipacaoRAV;
using Redecard.PN.Extrato.Servicos.Contrato.ContratoDados.Response;
using Redecard.PN.Extrato.Servicos.Modelo;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Antecipações RAV.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1330	/ Programa WA1330 / TranID ISHU / Método ConsultarTotalizadores<br/>
    /// - Book WACA1331	/ Programa WA1331 / TranID ISHV / Método Consultar<br/>
    /// - Book WACA1332	/ Programa WA1332 / TranID ISHX / Método ConsultarDetalheTotalizadores<br/>
    /// - Book WACA1333	/ Programa WA1333 / TranID ISHY / Método ConsultarDetalhe
    /// </remarks>
    [ServiceContract]
    [ServiceKnownType(typeof(RAVTotalizador))]
    [ServiceKnownType(typeof(RAVTotalizadorTotais))]
    [ServiceKnownType(typeof(RAVTotalizadorValor))]
    [ServiceKnownType(typeof(RAV))]
    [ServiceKnownType(typeof(RAVDetalheTotalizador))]
    [ServiceKnownType(typeof(RAVDetalheTotalizadorTotais))]
    [ServiceKnownType(typeof(RAVDetalheTotalizadorValor))]
    [ServiceKnownType(typeof(RAVDetalhe))]
    [ServiceKnownType(typeof(RAVDetalheA1))]
    [ServiceKnownType(typeof(RAVDetalheA2))]
    [ServiceKnownType(typeof(RAVDetalheDT))]
    public interface IHISServicoWAExtratoAntecipadosRest
    {
        /// <summary>
        /// Consulta de Relatório de Antecipações<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WACA1330 / Programa WA1330 / TranID ISHU<br/>
        /// - Book WACA1331 / Programa WA1331 / TranID ISHV
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1330 / Programa WA1330 / TranID ISHU<br/>
        /// - Book WACA1331 / Programa WA1331 / TranID ISHV
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
            UriTemplate = "Relatorio")]
        RelatorioAntecipadosResponse ConsultarRelatorio(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de Relatório de Antecipações RAV - Detalhe<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1332 / Programa WA1332 / TranID ISHX<br/>
        /// - Book WA1333 / Programa WA1333 / TranID ISHY
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1332 / Programa WA1332 / TranID ISHX<br/>
        /// - Book WA1333 / Programa WA1333 / TranID ISHY
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataAntecipacao">Data de antecipação</param>
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
            UriTemplate = "RelatorioDetalhe")]
        RelatorioAntecipadosDetalheResponse ConsultarRelatorioDetalhe(
            Int32 codigoBandeira,
            String dataAntecipacao,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            RAVDetalheTipoRegistro tipoRegistro,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de Relatório de Antecipações RAV - Detalhe - Ver Todos<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1332 / Programa WA1332 / TranID ISHX<br/>
        /// - Book WA1333 / Programa WA1333 / TranID ISHY</summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1332 / Programa WA1332 / TranID ISHX<br/>
        /// - Book WA1333 / Programa WA1333 / TranID ISHY
        /// </remarks>
        /// <param name="codigosBandeiras">Códigos das bandeiras</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
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
            UriTemplate = "RelatorioDetalheTodos")]
        RelatorioAntecipadosDetalheResponse ConsultarRelatorioDetalheTodos(
            List<Int32> codigosBandeiras,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            RAVDetalheTipoRegistro tipoRegistro,
            Int32 quantidadeTotalRegistros);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Antecipações RAV.<br/>
        /// - Book WACA1330 / Programa WA1330 / TranID ISHU
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1330 / Programa WA1330 / TranID ISHU
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizadores do Relatório de Antecipações RAV</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "Totalizadores")]
        ResponseBaseItem<RAVTotalizador> ConsultarTotalizadores(
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de registros do Relatório de Antecipações RAV.<br/>
        /// - Book WACA1331 / Programa WA1331 / TranID ISHV
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1331 / Programa WA1331 / TranID ISHV
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Antecipações RAV</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "Consultar")]
        ResponseBaseList<RAV> Consultar(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Antecipações RAV - Detalhe.<br/>
        /// - Book WACA1332 / Programa WA1332 / TranID ISHX
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1332 / Programa WA1332 / TranID ISHX
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataAntecipacao">Data de antecipação</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizador do Relatório de Antecipações RAV - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarDetalheTotalizadores")]
        ResponseBaseItem<RAVDetalheTotalizador> ConsultarDetalheTotalizadores(
            Int32 codigoBandeira,
            String dataAntecipacao,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Antecipações RAV - Detalhe, para um intervalo de datas.
        /// - Book WACA1332 / Programa WA1332 / TranID ISHX
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1332 / Programa WA1332 / TranID ISHX
        /// </remarks>
        /// <param name="codigosBandeiras">Códigos das bandeiras</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Totalizador do Relatório de Antecipações RAV - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarDetalheTotalizadoresTodos")]
        ResponseBaseItem<RAVDetalheTotalizador> ConsultarDetalheTodosTotalizadores(
            List<Int32> codigosBandeiras,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de registros do Relatório de Antecipações RAV - Detalhe.<br/>
        /// - Book WACA1333 / Programa WA1333 / TranID ISHY
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1333 / Programa WA1333 / TranID ISHY
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros existentes</param>
        /// <param name="tipoRegistro">Tipo de registro a ser retornado</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataAntecipacao">Data de antecipação</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Antecipações - RAV - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "ConsultarDetalhe")]
        ResponseBaseList<RAVDetalhe> ConsultarDetalhe(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            RAVDetalheTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            String dataAntecipacao,
            List<Int32> pvs);

        /// <summary>
        /// Consulta de registros do Relatório de Antecipações RAV - Detalhe, para um intervalo de datas.
        /// Se código da bandeira informado for igual a 0, pesquisa para todas as bandeiras (1 a 12).
        /// - Book WACA1333 / Programa WA1333 / TranID ISHY
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1333 / Programa WA1333 / TranID ISHY
        /// </remarks>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros existentes</param>
        /// <param name="tipoRegistro">Tipo de registro a ser retornado</param>        
        /// <param name="codigosBandeiras">Código da bandeira (se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <returns>Registros do Relatório de Antecipações - RAV - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedRequest,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ConsultarDetalheTodos")]
        ResponseBaseList<RAVDetalhe> ConsultarDetalheTodos(
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            Int32 quantidadeTotalRegistros,
            RAVDetalheTipoRegistro tipoRegistro,
            List<Int32> codigosBandeiras,
            String dataInicial,
            String dataFinal,
            List<Int32> pvs);
    }
}
