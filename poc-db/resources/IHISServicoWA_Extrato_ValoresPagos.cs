using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.ValoresPagos;
using Redecard.PN.Extrato.Servicos.Modelo;
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
    /// - Book WACA1323 / Programa WA1323 / TranID ISHN / Método ConsultarDebitoDetalhe
    /// </remarks>
    [ServiceContract]
    public interface IHISServicoWA_Extrato_ValoresPagos
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
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataRecebimento">Data de recebimento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
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
            DateTime dataRecebimento,
            Int32 pv,
            Int32 numeroOcu,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out CreditoDetalheTotalizador totalizador,
            out List<CreditoDetalhe> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

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
            ref Int32 quantidadeTotalRegistros,
            out DebitoTotalizador totalizador,
            out List<Debito> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

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
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="dataPagamento">Data de pagamento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>        
        /// <param name="quantidadeTotalRegistros">Quantida total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "RelatorioDebitoDetalhe")]
        void ConsultarRelatorioDebitoDetalhe(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,                
            DateTime dataPagamento,
            Int32 pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out DebitoDetalheTotalizador totalizador,
            out List<DebitoDetalhe> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Valores Pagos - Crédito.<br/>
        /// - Book WACA1316 / Programa WA1316 / TranID ISHG
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1316 / Programa WA1316 / TranID ISHG
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Crédito</returns>
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
        /// Consulta de registros do Relatório de Valores Pagos - Crédito.<br/>
        /// - Book WACA1317 / Programa WA1317 / TranID ISHH
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1317 / Programa WA1317 / TranID ISHH
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
        /// <returns>Registros do Relatório de Valores Pagos - Crédito.</returns>
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
        /// Consulta de totalizadores do Relatório de Valores Pagos - Crédito - Detalhe.<br/>
        /// - Book WACA1318 / Programa WA1318 / TranID ISHI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1318 / Programa WA1318 / TranID ISHI
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataRecebimento">Data de recebimento</param>        
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Crédito - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "TotalizadoresCreditoDetalhe")]
        CreditoDetalheTotalizador ConsultarCreditoDetalheTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataRecebimento,
            Int32 pv,
            Int32 numeroOcu,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Valores Pagos - Crédito - Detalhe.<br/>
        /// - Book WACA1319 / Programa WA1319 / TranID ISHJ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1319 / Programa WA1319 / TranID ISHJ
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataRecebimento">Data de recebimento</param>
        /// <param name="numeroOcu">Número da Ocu</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Valores Pagos - Crédito - Detalhe.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "CreditoDetalhe")]
        List<CreditoDetalhe> ConsultarCreditoDetalhe(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,            
            Int32 codigoBandeira,
            DateTime dataRecebimento,
            Int32 pv,
            Int32 numeroOcu,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Valores Pagos - Débito.<br/>
        /// - Book WACA1320 / Programa WA1320 / TranID ISHK
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1320 / Programa WA1320 / TranID ISHK
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Débito</returns>
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
        /// Consulta de registros do Relatório de Valores Pagos - Débito.<br/>
        /// - Book WACA1321 / Programa WA1321 / TranID ISHL
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1321 / Programa WA1321 / TranID ISHL
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
        /// <returns>Registros do Relatório de Valores Pagos - Débito.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "Debito")]
        List<Debito> ConsultarDebito(
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
        /// Consulta de totalizadores do Relatório de Valores Pagos - Débito - Detalhe.<br/>
        /// - Book WACA1322 / Programa WA1322 / TranID ISHM
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1322 / Programa WA1322 / TranID ISHM
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataPagamento">Data de pagamento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Valores Pagos - Débito - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "TotalizadoresDebitoDetalhe")]
        DebitoDetalheTotalizador ConsultarDebitoDetalheTotalizadores(
            Guid guidPesquisa,             
            DateTime dataPagamento,
            Int32 pv,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Valores Pagos - Débito - Detalhe.<br/>
        /// - Book WACA1323 / Programa WA1323 / TranID ISHN
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1323 / Programa WA1323 / TranID ISHN
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataPagamento">Data de pagamento</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Valores Pagos - Débito - Detalhe.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "DebitoDetalhe")]
        List<DebitoDetalhe> ConsultarDebitoDetalhe(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataPagamento,
            Int32 pv,
            out StatusRetorno status);
    }
}
