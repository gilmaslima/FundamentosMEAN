using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.LancamentosFuturos;
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
    public interface IHISServicoWA_Extrato_LancamentosFuturos
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
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RelatorioCredito")]
        void ConsultarRelatorioCredito(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out CreditoTotalizador totalizador,
            out List<Credito> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

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
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataVencimento">Data de vencimento</param>
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
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RelatorioCreditoDetalhe")]
        void ConsultarRelatorioCreditoDetalhe(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,            
            Int32 codigoBandeira,
            DateTime dataVencimento,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,
            ref Int32 quantidadeTotalRegistros,
            out CreditoDetalheTotalizador totalizador,
            out List<CreditoDetalhe> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

        /// <summary>
        /// Consulta de Relatório de Lançamentos Futuros - Crédito - Detalhe - Ver Todos<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1326 / Programa WA1326 / TranID ISHQ<br/>
        /// - Book WA1327 / Programa WA1327 / TranID ISHR
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1326 / Programa WA1326 / TranID ISHQ<br/>
        /// - Book WA1327 / Programa WA1327 / TranID ISHR
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
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RelatorioCreditoDetalheTodos")]
        void ConsultarRelatorioCreditoDetalheTodos(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,            
            List<Int32> codigosBandeiras,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,
            ref Int32 quantidadeTotalRegistros,
            out CreditoDetalheTotalizador totalizador,
            out List<CreditoDetalhe> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

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
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="tipoRegistro">Tipo de registro a ser retornado</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RelatorioDebito")]
        void ConsultarRelatorioDebito(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            DebitoTipoRegistro tipoRegistro,
            ref Int32 quantidadeTotalRegistros,
            out DebitoTotalizador totalizador,
            out List<Debito> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Lançamentos Futuros - Crédito.<br/>
        /// - Book WACA1324 / Programa WA1324 / TranID ISHO
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1324 / Programa WA1324 / TranID ISHO
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Crédito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "TotalizadoresCredito")]
        CreditoTotalizador ConsultarCreditoTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Lançamentos Futuros - Crédito.<br/>
        /// - Book WACA1325 / Programa WA1325 / TranID ISHP
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1325 / Programa WA1325 / TranID ISHP
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
        /// <returns>Registros do Relatório de Lançamentos Futuros - Crédito.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Credito")]
        List<Credito> ConsultarCredito(
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
        /// Consulta de totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe.<br/>
        /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataVencimento">Data de vencimento</param>        
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "TotalizadoresCreditoDetalhe")]
        CreditoDetalheTotalizador ConsultarCreditoDetalheTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataVencimento,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe - Ver Todos.<br/>
        /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1326 / Programa WA1326 / TranID ISHQ
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigosBandeiras">Código da bandeira</param>        
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Crédito - Detalhe - Ver Todos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "TotalizadoresCreditoTodos")]
        CreditoDetalheTotalizador ConsultarCreditoDetalheTodosTotalizadores(
            Guid guidPesquisa,            
            List<Int32> codigosBandeiras,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Lançamentos Futuros - Crédito - Detalhe.<br/>
        /// - Book WACA1327 / Programa WA1327 / TranID ISHR
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1327 / Programa WA1327 / TranID ISHR
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataVencimento">Data de vencimento</param>        
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Lançamentos Futuros - Crédito - Detalhe.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "CreditoDetalhe")]
        List<CreditoDetalhe> ConsultarCreditoDetalhe(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,            
            Int32 codigoBandeira,
            DateTime dataVencimento,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Lançamentos Futuros - Crédito - Detalhe - Ver Todos.<br/>
        /// - Book WACA1327 / Programa WA1327 / TranID ISHR
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1327 / Programa WA1327 / TranID ISHR
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>    
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigosBandeiras">Códigos das bandeiras</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Lançamentos Futuros - Crédito - Detalhe - Ver Todos.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "CreditoDetalheTodos")]
        List<CreditoDetalhe> ConsultarCreditoDetalheTodos(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,            
            List<Int32> codigosBandeiras,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Lançamentos Futuros - Débito.<br/>
        /// - Book WACA1328 / Programa WA1328 / TranID ISHS
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1328 / Programa WA1328 / TranID ISHS
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Lançamentos Futuros - Débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "TotalizadoresDebito")]
        DebitoTotalizador ConsultarDebitoTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Lançamentos Futuros - Débito.<br/>
        /// - Book WACA1329 / Programa WA1329 / TranID ISHT
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1329 / Programa WA1329 / TranID ISHT
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Lançamentos Futuros - Débito.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Debito")]
        List<Debito> ConsultarDebito(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DebitoTipoRegistro tipoRegistro,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);
    }
}