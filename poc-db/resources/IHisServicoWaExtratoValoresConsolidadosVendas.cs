/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Extrato.Servicos.Modelo.RelatorioValoresConsolidadosVendas;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Relatório de Valores Consolidados de Vendas
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book BKWA2510	/ Programa WAC251 / TranID ISGM / Método ConsultarTotalVendasCreditoPorPeriodo<br/>
    /// - Book BKWA2520 / Programa WAC252 / TranID ISGN / Método ConsultarTotalVendasDebitoPorPeriodo<br/>
    /// - Book BKWA2530 / Programa WAC253 / TranID ISGO / Método ConsultarTotalVendasCreditoPorPeriodoBandeira<br/>
    /// - Book BKWA2540 / Programa WAC254 / TranID ISGP / Método ConsultarVendasCreditoPorDiaPv<br/>
    /// - Book BKWA2550 / Programa WAC255 / TranID ISGQ / Método ConsultarTotalVendasDebitoPorPeriodoBandeira<br/>
    /// - Book BKWA2560 / Programa WAC256 / TranID ISGR / Método ConsultarVendasDebitoPorDiaPv<br/>
    /// - Book BKWA2570 / Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira<br/>
    /// - Book BKWA2580 / Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia<br/>
    /// - Book BKWA2590 / Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira<br/>
    /// - Book BKWA2600 / Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia<br/>
    /// </remarks>
    [ServiceContract]
    public interface IHisServicoWaExtratoValoresConsolidadosVendas
    { 
        #region [Métodos Crédito]

        /// <summary>
        /// Consulta de Relatório Consolidado de Vendas Crédito, por período.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2530	/ Programa WAC253 / TranID ISGO / Método ConsultarTotalVendasCreditoPorPeriodoBandeira<br/>
        /// - Book BKWA2540	/ Programa WAC254 / TranID ISGP / Método ConsultarVendasCreditoPorDiaPv
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2530	/ Programa WAC253 / TranID ISGO / Método ConsultarTotalVendasCreditoPorPeriodoBandeira<br/>
        /// - Book BKWA2540	/ Programa WAC254 / TranID ISGP / Método ConsultarVendasCreditoPorDiaPv
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void ConsultarRelatorioVendasCreditoPorPeriodo(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out TotalVendasCreditoPorPeriodoBandeira totalizador,
            out List<VendasCreditoPorDiaPv> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

        /// <summary>
        /// Consulta de Relatório  Consolidado de Vendas Crédito, por dia.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2570	/ Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira<br/>
        /// - Book BKWA2580	/ Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2570	/ Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira<br/>
        /// - Book BKWA2580	/ Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="dataVenda">Data da venda que se deseja consultar.</param>
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar.</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void ConsultarRelatorioVendasCreditoPorDia(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            DateTime dataVenda,
            Int32 numeroPv,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out TotalVendasCreditoPorDiaBandeira totalizador,
            out List<ResumoVendasCreditoPorDia> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);
  
        /// <summary>
        /// Consulta de Relatório  Consolidado de Vendas Crédito, por dia - Ver Todos<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2570	/ Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira<br/>
        /// - Book BKWA2580	/ Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2570	/ Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira<br/>
        /// - Book BKWA2580	/ Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="guidPesquisaPvDataVenda">Identificador dos registros de PV e Data Venda do relatório (utilizado para cache de dados)</param>
        /// <param name="dataInicio">Data inicial das vendas que se deseja consultar.</param>
        /// <param name="dataFim">Data final das vendas que se deseja consultar.</param>
        /// <param name="pvs">Lista de números dos Pontos de Vendas (Estabelecimento) que se deseja consultar.</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void ConsultarRelatorioVendasCreditoPorDiaTodos(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            Guid guidPesquisaPvDataVenda,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out TotalVendasCreditoPorDiaBandeira totalizador,
            out List<ResumoVendasCreditoPorDia> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

        /// <summary>
        /// Consulta o total de vendas realizadas a crédito no período informado.<br/>        
        /// - Book BKWA2510	/ Programa WAC251 / TranID ISGM / Método ConsultarTotalVendasCreditoPorPeriodo
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2510 / Programa WAC251 / TranID ISGM / Método ConsultarTotalVendasCreditoPorPeriodo
        /// </remarks>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito do período.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        TotalVendasCreditoPorPeriodo ConsultarTotalVendasCreditoPorPeriodo(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta o total de vendas realizadas á crédito no período informado e retorna separado por bandeira.<br/>        
        /// - Book BKWA2530	/ Programa WAC253 / TranID ISGO / Método ConsultarTotalVendasCreditoPorPeriodoBandeira
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2530 / Programa WAC253 / TranID ISGO / Método ConsultarTotalVendasCreditoPorPeriodoBandeira
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito do período por bandeira.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        TotalVendasCreditoPorPeriodoBandeira ConsultarTotalVendasCreditoPorPeriodoBandeira(
            Guid guidPesquisa,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta as vendas realizadas á crédito, no período informado, separadas por dia e por PV (Ponto de Venda).<br/>        
        /// - Book BKWA2540	/ Programa WAC254 / TranID ISGP / Método ConsultarVendasCreditoPorDiaPv
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2540 / Programa WAC254 / TranID ISGP / Método ConsultarVendasCreditoPorDiaPv
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito por dia e por PV.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<VendasCreditoPorDiaPv> ConsultarVendasCreditoPorDiaPv(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,  
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta o total de vendas realizadas á crédito na data informada por bandeira.<br/>        
        /// - Book BKWA2570	/ Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2570 / Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="dataVenda">Data da venda que se deseja consultar.</param>
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito do dia por bandeira.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        TotalVendasCreditoPorDiaBandeira ConsultarTotalVendasCreditoPorDiaBandeira(
            Guid guidPesquisa,
            DateTime dataVenda,
            Int32 numeroPv,
            out StatusRetorno status);

        /// <summary>
        /// Consulta o total de vendas realizadas á crédito na data informada por bandeira - Ver Todos.<br/>        
        /// - Book BKWA2570	/ Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2570 / Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="guidPesquisaPvDataVenda">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="dataInicio">Data inicial que se deseja consultar.</param>
        /// <param name="dataFim">Data final que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito do dia por bandeira.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        TotalVendasCreditoPorDiaBandeira ConsultarTotalVendasCreditoPorDiaBandeiraTodos(
            Guid guidPesquisa,
            Guid guidPesquisaPvDataVenda,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta o resumo das vendas realizadas á crédito na data informada por bandeira.<br/>        
        /// - Book BKWA2580	/ Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2580 / Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="dataVenda">Data da venda que se deseja consultar.</param>
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Resumo das vendas crédito do dia.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ResumoVendasCreditoPorDia> ConsultarResumoVendasCreditoPorDia(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,  
            DateTime dataVenda,
            Int32 numeroPv,
            out StatusRetorno status);

        /// <summary>
        /// Consulta o resumo das vendas realizadas á crédito na data informada por bandeira - Ver Todos.<br/>        
        /// - Book BKWA2580	/ Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2580 / Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// </remarks>        
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="guidPesquisaPvDataVenda">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="dataInicio">Data inicial que se deseja consultar.</param>
        /// <param name="dataFim">Data final que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Resumo das vendas crédito do período informado.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ResumoVendasCreditoPorDia> ConsultarResumoVendasCreditoPorDiaTodos(
            Guid guidPesquisa,
            Guid guidPesquisaPvDataVenda,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,            
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status);

        #endregion

        #region [Métodos Débito]

        /// <summary>
        /// Consulta de Relatório Consolidado de Vendas Crédito, por período.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2550	/ Programa WAC255 / TranID ISGQ / Método ConsultarTotalVendasDebitoPorPeriodoBandeira
        /// - Book BKWA2560	/ Programa WAC256 / TranID ISGR / Método ConsultarVendasDebitoPorDiaPv
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2550	/ Programa WAC255 / TranID ISGQ / Método ConsultarTotalVendasDebitoPorPeriodoBandeira
        /// - Book BKWA2560	/ Programa WAC256 / TranID ISGR / Método ConsultarVendasDebitoPorDiaPv
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void ConsultarRelatorioVendasDebitoPorPeriodo(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out TotalVendasDebitoPorPeriodoBandeira totalizador,
            out List<VendasDebitoPorDiaPv> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

        /// <summary>
        /// Consulta de Relatório Consolidado de Vendas Débito, por dia.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2590	/ Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira<br/>
        /// - Book BKWA2600	/ Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2590	/ Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira<br/>
        /// - Book BKWA2600	/ Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="dataVenda">Data da venda que se deseja consultar.</param>
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar.</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void ConsultarRelatorioVendasDebitoPorDia(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            DateTime dataVenda,
            Int32 numeroPv,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out TotalVendasDebitoPorDiaBandeira totalizador,
            out List<ResumoVendasDebitoPorDia> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

        /// <summary>
        /// Consulta de Relatório Consolidado de Vendas Débito, por dia. Ver Todos<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2590	/ Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira<br/>
        /// - Book BKWA2600	/ Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2590	/ Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira<br/>
        /// - Book BKWA2600	/ Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// </remarks>
        /// <param name="guidPesquisaTotalizador">Identificador do totalizador do relatório(utilizado para cache de dados)</param>
        /// <param name="guidPesquisaRelatorio">Identificador dos registros do relatório (utilizado para cache de dados)</param>
        /// <param name="guidPesquisaPvDataVenda">Identificador dos registros de PV e Data Venda do relatório (utilizado para cache de dados)</param>
        /// <param name="dataInicio">Data inicial das vendas que se deseja consultar.</param>
        /// <param name="dataFim">Data final das vendas que se deseja consultar.</param>
        /// <param name="pvs">Lista de números dos Pontos de Vendas (Estabelecimento) que se deseja consultar.</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="totalizador">Totalizadores do relatório</param>
        /// <param name="registros">Registros do relatório (apenas o intervalo de registros solicitado)</param>
        /// <param name="statusTotalizador">Status de retorno da consulta dos totalizadores do relatório</param>
        /// <param name="statusRelatorio">Status de retorno da consulta dos registros do relatório</param>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        void ConsultarRelatorioVendasDebitoPorDiaTodos(
            Guid guidPesquisaTotalizador,
            Guid guidPesquisaRelatorio,
            Guid guidPesquisaPvDataVenda,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            out TotalVendasDebitoPorDiaBandeira totalizador,
            out List<ResumoVendasDebitoPorDia> registros,
            out StatusRetorno statusTotalizador,
            out StatusRetorno statusRelatorio);

        /// <summary>
        /// Consulta o total de vendas realizadas á débito no período informado.<br/>        
        /// - Book BKWA2520	/ Programa WAC252 / TranID ISGN / Método ConsultarTotalVendasDebitoPorPeriodo
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2520 / Programa WAC252 / TranID ISGN / Método ConsultarTotalVendasDebitoPorPeriodo
        /// </remarks>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito do período.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        TotalVendasDebitoPorPeriodo ConsultarTotalVendasDebitoPorPeriodo(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta o total de vendas realizadas á débito no período informado e retorna separado por bandeira.<br/>        
        /// - Book BKWA2550	/ Programa WAC255 / TranID ISGQ / Método ConsultarTotalVendasDebitoPorPeriodoBandeira
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2550 / Programa WAC255 / TranID ISGQ / Método ConsultarTotalVendasDebitoPorPeriodoBandeira
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito do período por bandeira.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        TotalVendasDebitoPorPeriodoBandeira ConsultarTotalVendasDebitoPorPeriodoBandeira(
            Guid guidPesquisa,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta as vendas realizadas á débito, no período informado, separadas por dia e por PV (Ponto de Venda).<br/>        
        /// - Book BKWA2560	/ Programa WAC256 / TranID ISGR / Método ConsultarVendasDebitoPorDiaPv
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2560 / Programa WAC256 / TranID ISGR / Método ConsultarVendasDebitoPorDiaPv
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="temMaisRegistros">Flag indicando se ainda existem registros para serem retornados.</param>
        /// <param name="chaveRechamada">Chave utilizada para solicitar os próximos registos.</param>    
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito por dia e por PV</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<VendasDebitoPorDiaPv> ConsultarVendasDebitoPorDiaPv(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,  
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status);

        /// <summary>
        /// Consulta o total de vendas realizadas á débito na data informada por bandeira.<br/>        
        /// - Book BKWA2590	/ Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2590 / Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="dataVenda">Data da venda que se deseja consultar.</param>
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito do dia por bandeira.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        TotalVendasDebitoPorDiaBandeira ConsultarTotalVendasDebitoPorDiaBandeira(
            Guid guidPesquisa,
            DateTime dataVenda,
            Int32 numeroPv,
            out StatusRetorno status);

        /// <summary>
        /// Consulta o total de vendas realizadas á débito na data informada por bandeira - Ver Todos<br/>        
        /// - Book BKWA2590	/ Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2590 / Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>      
        /// <param name="guidPesquisaPvDataVenda">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="dataInicio">Data inicial que se deseja consultar.</param>
        /// <param name="dataFim">Data final que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito do dia por bandeira.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        TotalVendasDebitoPorDiaBandeira ConsultarTotalVendasDebitoPorDiaBandeiraTodos(
            Guid guidPesquisa,
            Guid guidPesquisaPvDataVenda,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status);


        /// <summary>
        /// Consulta o resumo das vendas realizadas á débito na data informada por bandeira.<br/>        
        /// - Book BKWA2600	/ Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2600 / Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="dataVenda">Data da venda que se deseja consultar.</param>
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar.</param>
        /// <param name="temMaisRegistros">Flag indicando se ainda existem registros para serem retornados.</param>
        /// <param name="chaveRechamada">Chave utilizada para solicitar os próximos registos.</param>    
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Resumo das vendas débito do dia.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ResumoVendasDebitoPorDia> ConsultarResumoVendasDebitoPorDia(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,  
            DateTime dataVenda,
            Int32 numeroPv,
            out StatusRetorno status);

        /// <summary>
        /// Consulta o resumo das vendas realizadas á débito na data informada por bandeira - Ver Todos.<br/>        
        /// - Book BKWA2600	/ Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2600 / Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="guidPesquisaPvDataVenda">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="dataInicio">Data inicial que se deseja consultar.</param>
        /// <param name="dataFim">Data final que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Resumo das vendas débito do dia.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        List<ResumoVendasDebitoPorDia> ConsultarResumoVendasDebitoPorDiaTodos(
            Guid guidPesquisa,
            Guid guidPesquisaPvDataVenda,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status);

        #endregion

        #region [Método Total Consolidado]

        /// <summary>
        /// Consulta o total de vendas realizadas á crédito e á débito no período informado.<br/>        
        /// - Book's BKWA2510 / Programa WAC251 / TranID ISGM e BKWA2520 / Programa WAC252 / TranID ISGN / Método ConsultarTotalVendasPorPeriodoConsolidado
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2510 / Programa WAC251 / TranID ISGM / Método ConsultarTotalVendasPorPeriodoConsolidado
        /// </remarks>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data inicio que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito e débito do período.</returns>
        [OperationContract]
        [FaultContract(typeof(GeneralFault))]
        TotalVendasPorPeriodoConsolidado ConsultarTotalVendasPorPeriodoConsolidado(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status);

        #endregion
    }
}
