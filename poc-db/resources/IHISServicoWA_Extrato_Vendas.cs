/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Vendas;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo.Vendas;
using System.ServiceModel.Web;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Vendas.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1310 / Programa WA1310 / TranID ISHA / Método ConsultarCreditoTotalizadores<br/>
    /// - Book WACA1311 / Programa WA1311 / TranID ISHB / Método ConsultarCredito<br/>
    /// - Book WACA1312 / Programa WA1312 / TranID ISHC / Método ConsultarDebitoTotalizadores<br/>
    /// - Book WACA1313 / Programa WA1313 / TranID ISHD / Método ConsultarDebito<br/>
    /// - Book WACA1314 / Programa WA1314 / TranID ISHE / Método ConsultarConstrucardTotalizadores<br/>
    /// - Book WACA1315 / Programa WA1315 / TranID ISHF / Método ConsultarConstrucard
    /// - Book BKWA2610	/ Programa WAC261 / TranID WAAF / Método ConsultarRecargaCelularPvFisicoTotalizadores<br/>
    /// - Book BKWA2620	/ Programa WAC262 / TranID WAAG / Método ConsultarRecargaCelularPvFisico<br/>
    /// - Book BKWA2630	/ Programa WAC263 / TranID WAAH / Método ConsultarRecargaCelularPvLogicoTotalizadores<br/>
    /// - Book BKWA2640	/ Programa WAC264 / TranID WAAI / Método ConsultarRecargaCelularPvLogico
    /// </remarks>
    [ServiceContract,
    ServiceKnownType(typeof(CreditoTotalizador)),
    ServiceKnownType(typeof(Credito)),
    ServiceKnownType(typeof(CreditoD)),
    ServiceKnownType(typeof(DebitoTotalizador)),
    ServiceKnownType(typeof(Debito)),
    ServiceKnownType(typeof(DebitoDT)),
    ServiceKnownType(typeof(DebitoA1)),
    ServiceKnownType(typeof(DebitoA2)),
    ServiceKnownType(typeof(ConstrucardTotalizador)),
    ServiceKnownType(typeof(Construcard)),
    ServiceKnownType(typeof(ConstrucardDT)),
    ServiceKnownType(typeof(ConstrucardA1)),
    ServiceKnownType(typeof(ConstrucardA2)),
    ServiceKnownType(typeof(RecargaCelularTotalizador)),
    ServiceKnownType(typeof(RecargaCelular)),
    ServiceKnownType(typeof(RecargaCelularPvFisico)),
    ServiceKnownType(typeof(RecargaCelularPvLogico))]   
    public interface IHISServicoWA_Extrato_Vendas
    {
        /// <summary>
        /// Consulta de Relatório de Vendas - Crédito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1310 / Programa WA1310 / TranID ISHA<br/>
        /// - Book WACA1311 / Programa WA1311 / TranID ISHB
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1310 / Programa WA1310 / TranID ISHA<br/>
        /// - Book WACA1311 / Programa WA1311 / TranID ISHB
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
        /// Consulta de Relatório de Vendas - Débito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1312 / Programa WA1312 / TranID ISHC<br/>
        /// - Book WACA1313 / Programa WA1313 / TranID ISHD
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1312 / Programa WA1312 / TranID ISHC<br/>
        /// - Book WACA1313 / Programa WA1313 / TranID ISHD
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="modalidade">Modalidade</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
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
            Modalidade modalidade,
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
        /// Consulta de Relatório de Vendas - Construcard.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1314 / Programa WA1314 / TranID ISHE<br/>
        /// - Book WACA1315 / Programa WA1315 / TranID ISHF
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1314 / Programa WA1314 / TranID ISHE<br/>
        /// - Book WACA1315 / Programa WA1315 / TranID ISHF
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RelatorioConstrucard")]
        void ConsultarRelatorioConstrucard(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ConstrucardTipoRegistro tipoRegistro,
            ref Int32 quantidadeTotalRegistros,
            out ConstrucardTotalizador totalizador,
            out List<Construcard> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

        /// <summary>
        /// Consulta de Relatório de Vendas - Recarga de Celular - PV Físico.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório (utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
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
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RelatorioRecargaFisico")]
        void ConsultarRelatorioRecargaCelularPvFisico(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out RecargaCelularTotalizador totalizador,
            out List<RecargaCelular> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);       

        /// <summary>
        /// Consulta de Relatório de Vendas - Recarga de Celular - PV Lógico.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório (utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
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
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RelatorioRecargaLogico")]
        void ConsultarRelatorioRecargaCelularPvLogico(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out RecargaCelularTotalizador totalizador,
            out List<RecargaCelular> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

         /// <summary>
        /// Consulta de Relatório de Vendas - Recarga de Celular - PV Lógico.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório (utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
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
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RelatorioRecarga")]
        void ConsultarRelatorioRecargaCelular(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            RecargaCelularTipoPv tipoPv,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out RecargaCelularTotalizador totalizador,
            out List<RecargaCelular> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Crédito.<br/>
        /// - Book WACA1310 / Programa WA1310 / TranID ISHA
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1310 / Programa WA1310 / TranID ISHA
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Vendas - Crédito</returns>
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
        /// Consulta de registros do Relatório de Vendas - Crédito.<br/>
        /// - Book WACA1311 / Programa WA1311 / TranID ISHB
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1311 / Programa WA1311 / TranID ISHB
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
        /// <returns>Registros do Relatório de Vendas - Crédito.</returns>
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
        /// Consulta de totalizadores do Relatório de Vendas - Débito.<br/>
        /// - Book WACA1312 / Programa WA1312 / TranID ISHC
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1312 / Programa WA1312 / TranID ISHC
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="modalidade">Modalidade</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Vendas - Débito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "TotalizadoresDebito")]
        DebitoTotalizador ConsultarDebitoTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            Modalidade modalidade,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Débito.<br/>
        /// - Book WACA1313 / Programa WA1313 / TranID ISHD
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1313 / Programa WA1313 / TranID ISHD
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="modalidade">Modalidade</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Vendas - Débito.</returns>
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
            Modalidade modalidade,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Construcard.<br/>
        /// - Book WACA1314 / Programa WA1314 / TranID ISHE
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1314 / Programa WA1314 / TranID ISHE
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Vendas - Construcard</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "TotalizadoresConstrucard")]
        ConstrucardTotalizador ConsultarConstrucardTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Construcard.<br/>
        /// - Book WACA1315 / Programa WA1315 / TranID ISHF
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1315 / Programa WA1315 / TranID ISHF
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>        
        /// <param name="tipoRegistro">Tipo dos registros a serem retornados</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Vendas - Construcard.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Construcard")]
        List<Construcard> ConsultarConstrucard(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            ConstrucardTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Recarga de Celular de PV Físico.<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Vendas - Recarga de Celular de PV Físico</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "TotalizadoresRecargaFisico")]
        RecargaCelularTotalizador ConsultarRecargaCelularPvFisicoTotalizadores(
            Guid guidPesquisa,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Recarga de Celular de PV Físico.<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>        
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Vendas - Recarga de Celular de PV Físico.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RecargaFisico")]
        List<RecargaCelularPvFisico> ConsultarRecargaCelularPvFisico(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Vendas - Recarga de Celular de PV Lógico.<br/>
        /// - Book BKWA2630 / Programa WAC263 / TranID WAAH
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2630 / Programa WAC263 / TranID WAAH
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Vendas - Recarga de Celular de PV Lógico</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "TotalizadoresRecargaLogico")]
        RecargaCelularTotalizador ConsultarRecargaCelularPvLogicoTotalizadores(
            Guid guidPesquisa,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Vendas - Recarga de Celular de PV Lógico.<br/>
        /// - Book BKWA2640 / Programa WAC264 / TranID WAAI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2640 / Programa WAC264 / TranID WAAI
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>        
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Vendas - Recarga de Celular de PV Lógico.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RecargaLogico")]
        List<RecargaCelularPvLogico> ConsultarRecargaCelularPvLogico(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);
    }
    }
