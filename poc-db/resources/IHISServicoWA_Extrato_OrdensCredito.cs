using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.OrdensCredito;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Ordens de Crédito.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book WACA1334	/ Programa WA1334 / TranID ISHZ / Método ConsultarTotalizadores<br/>
    /// - Book WACA1335	/ Programa WA1335 / TranID ISHW / Método Consultar<br/>
    /// - Book WACA1336	/ Programa WA1336 / TranID ISH0 / Método ConsultarDetalheTotalizadores<br/>
    /// - Book WACA1337	/ Programa WA1337 / TranID ISH1 / Método ConsultarDetalhe
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
    [ServiceKnownType(typeof(CreditoDetalheDT))]
    [ServiceKnownType(typeof(CreditoDetalheD1))]
    public interface IHISServicoWA_Extrato_OrdensCredito
    {
        /// <summary>
        /// Consulta de Relatório de Ordens de Crédito.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book WACA1334 / Programa WA1334 / TranID ISHZ<br/>
        /// - Book WACA1335 / Programa WA1335 / TranID ISHW
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA1334 / Programa WA1334 / TranID ISHZ<br/>
        /// - Book WACA1335 / Programa WA1335 / TranID ISHW
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
            out CreditoTotalizador totalizador,
            out List<Credito> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

        /// <summary>
        /// Consulta de Relatório de Ordens de Crédito - Detalhe<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1336 / Programa WA1336 / TranID ISH0<br/>
        /// - Book WA1337 / Programa WA1337 / TranID ISH1
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1336 / Programa WA1336 / TranID ISH0<br/>
        /// - Book WA1337 / Programa WA1337 / TranID ISH1
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>        
        /// <param name="codigoBandeira">Código da bandeira (Se 0, pesquisa para todas as bandeiras)</param>
        /// <param name="dataEmissao">Data de emissão</param>
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
        void ConsultarRelatorioDetalhe(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,            
            Int32 codigoBandeira,
            DateTime dataEmissao,
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
        /// Consulta de Relatório de Ordens de Crédito - Detalhe - Ver Todos<br/>
        /// Efetua consulta paralela dos Totalizadores e dos Registros do relatório.<br/>
        /// - Book WA1336 / Programa WA1336 / TranID ISH0<br/>
        /// - Book WA1337 / Programa WA1337 / TranID ISH1
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WA1336 / Programa WA1336 / TranID ISH0<br/>
        /// - Book WA1337 / Programa WA1337 / TranID ISH1
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
        void ConsultarRelatorioDetalheTodos(
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
        /// Consulta de totalizadores do Relatório de Ordens de Crédito.<br/>
        /// - Book WACA1334 / Programa WA1334 / TranID ISHZ
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1334 / Programa WA1334 / TranID ISHZ
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Ordens de Crédito</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        CreditoTotalizador ConsultarTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Ordens de Crédito.<br/>
        /// - Book WACA1335 / Programa WA1335 / TranID ISHW
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1335 / Programa WA1335 / TranID ISHW
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na consulta</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Ordens de Crédito.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<Credito> Consultar(
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
        /// Consulta de totalizadores do Relatório de Ordens de Crédito - Detalhe.<br/>
        /// - Book WACA1336 / Programa WA1336 / TranID ISH0
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1336 / Programa WA1336 / TranID ISH0
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataEmissao">Data da emissão</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Ordens de Crédito - Detalhe</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        CreditoDetalheTotalizador ConsultarDetalheTotalizadores(
            Guid guidPesquisa,            
            Int32 codigoBandeira,
            DateTime dataEmissao,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de totalizadores do Relatório de Ordens de Crédito - Detalhe - Ver Todos.<br/>
        /// - Book WACA1336 / Programa WA1336 / TranID ISH0
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1336 / Programa WA1336 / TranID ISH0
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="codigosBandeiras">Códigos das bandeiras</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Ordens de Crédito - Detalhe - Ver Todos</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        CreditoDetalheTotalizador ConsultarDetalheTodosTotalizadores(
            Guid guidPesquisa,            
            List<Int32> codigosBandeiras,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Ordens de Crédito - Detalhe.<br/>
        /// - Book WACA1337 / Programa WA1337 / TranID ISH1
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1337 / Programa WA1337 / TranID ISH1
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param> 
        /// <param name="tipoRegistro">Tipo do registro a ser retornado</param>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="dataEmissao">Data de emissão</param>        
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Ordens de Crédito - Detalhe.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<CreditoDetalhe> ConsultarDetalhe(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            CreditoDetalheTipoRegistro tipoRegistro,            
            Int32 codigoBandeira,
            DateTime dataEmissao,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta de registros do Relatório de Ordens de Crédito - Detalhe - Ver Todos.<br/>
        /// - Book WACA1337 / Programa WA1337 / TranID ISH1
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book WACA1337 / Programa WA1337 / TranID ISH1
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
        /// <returns>Registros do Relatório de Ordens de Crédito - Detalhe - Ver Todos.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<CreditoDetalhe> ConsultarDetalheTodos(
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
    }
}
