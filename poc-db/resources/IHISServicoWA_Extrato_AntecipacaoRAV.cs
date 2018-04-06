using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.AntecipacaoRAV;
using Redecard.PN.Extrato.Servicos.Modelo;
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
    public interface IHISServicoWA_Extrato_AntecipacaoRAV
    {
        /// <summary>
        /// Consulta de Relatório de Antecipações RAV.<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WACA1330 / Programa WA1330 / TranID ISHU<br/>
        /// - Book WACA1331 / Programa WA1331 / TranID ISHV
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1330 / Programa WA1330 / TranID ISHU<br/>
        /// - Book WACA1331 / Programa WA1331 / TranID ISHV
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Relatorio")]
        void ConsultarRelatorio(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out RAVTotalizador totalizador,
            out List<RAV> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

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
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataAntecipacao">Data de antecipação</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RelatorioDetalhe")]
        void ConsultarRelatorioDetalhe(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,            
            Int32 codigoBandeira,
            DateTime dataAntecipacao,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            RAVDetalheTipoRegistro tipoRegistro,
            ref Int32 quantidadeTotalRegistros,
            out RAVDetalheTotalizador totalizador,
            out List<RAVDetalhe> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

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
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="codigosBandeiras">Códigos das bandeiras</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RelatorioDetalheTodos")]
        void ConsultarRelatorioDetalheTodos(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,            
            List<Int32> codigosBandeiras,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            RAVDetalheTipoRegistro tipoRegistro,
            ref Int32 quantidadeTotalRegistros,
            out RAVDetalheTotalizador totalizador,
            out List<RAVDetalhe> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Antecipações RAV.<br/>
        /// - Book WACA1330 / Programa WA1330 / TranID ISHU
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1330 / Programa WA1330 / TranID ISHU
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Antecipações RAV</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Totalizadores")]
        RAVTotalizador ConsultarTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Antecipações RAV.<br/>
        /// - Book WACA1331 / Programa WA1331 / TranID ISHV
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1331 / Programa WA1331 / TranID ISHV
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Antecipações RAV</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Consultar")]
        List<RAV> Consultar(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Antecipações RAV - Detalhe.<br/>
        /// - Book WACA1332 / Programa WA1332 / TranID ISHX
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1332 / Programa WA1332 / TranID ISHX
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataAntecipacao">Data de antecipação</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizador do Relatório de Antecipações RAV - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ConsultarDetalheTotalizadores")]
        RAVDetalheTotalizador ConsultarDetalheTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataAntecipacao,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Antecipações RAV - Detalhe, para um intervalo de datas.
        /// - Book WACA1332 / Programa WA1332 / TranID ISHX
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1332 / Programa WA1332 / TranID ISHX
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigosBandeiras">Códigos das bandeiras</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizador do Relatório de Antecipações RAV - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ConsultarDetalheTotalizadoresTodos")]
        RAVDetalheTotalizador ConsultarDetalheTodosTotalizadores(
            Guid guidPesquisa,            
            List<Int32> codigosBandeiras,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Antecipações RAV - Detalhe.<br/>
        /// - Book WACA1333 / Programa WA1333 / TranID ISHY
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1333 / Programa WA1333 / TranID ISHY
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros existentes</param>
        /// <param name="tipoRegistro">Tipo de registro a ser retornado</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataAntecipacao">Data de antecipação</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Antecipações - RAV - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ConsultarDetalhe")]
        List<RAVDetalhe> ConsultarDetalhe(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            RAVDetalheTipoRegistro tipoRegistro,            
            Int32 codigoBandeira,
            DateTime dataAntecipacao,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Antecipações RAV - Detalhe, para um intervalo de datas.
        /// Se código da bandeira informado for igual a 0, pesquisa para todas as bandeiras (1 a 12).
        /// - Book WACA1333 / Programa WA1333 / TranID ISHY
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1333 / Programa WA1333 / TranID ISHY
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros existentes</param>
        /// <param name="tipoRegistro">Tipo de registro a ser retornado</param>        
        /// <param name="codigosBandeiras">Código da bandeira (se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Antecipações - RAV - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "ConsultarDetalheTodos")]
        List<RAVDetalhe> ConsultarDetalheTodos(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            RAVDetalheTipoRegistro tipoRegistro,            
            List<Int32> codigosBandeiras,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);
    }
}
