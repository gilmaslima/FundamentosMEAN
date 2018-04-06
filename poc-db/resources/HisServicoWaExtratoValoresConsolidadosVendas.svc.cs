/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Extrato.Servicos.Modelo.RelatorioValoresConsolidadosVendas;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Servicos.Modelo;
using DTO = Redecard.PN.Extrato.Modelo.RelatorioValoresConsolidadosVendas;
using BLL = Redecard.PN.Extrato.Negocio.ValoresConsolidadosVendas;

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
    public class HisServicoWaExtratoValoresConsolidadosVendas : ServicoBaseExtrato, IHisServicoWaExtratoValoresConsolidadosVendas
    {
        #region [ Relatórios - Consultas Multi-Thread (Totalizadores e Registros) ]

        #region [Relatórios Crédito]

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
        public void ConsultarRelatorioVendasCreditoPorPeriodo(
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
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno statusTotalizadorAux = null, statusRelatorioAux = null;
            Int32 quantidadeTotalRegistrosAux = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<TotalVendasCreditoPorPeriodoBandeira> funcaoTotalizador = () =>
            {
                return ConsultarTotalVendasCreditoPorPeriodoBandeira(
                    guidPesquisaTotalizador,
                    dataInicio,
                    dataFim,
                    pvs,
                    out statusTotalizadorAux);
            };

            //Preparação da Func para consulta dos registros
            Func<List<VendasCreditoPorDiaPv>> funcaoRegistros = () =>
            {
                return ConsultarVendasCreditoPorDiaPv(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref quantidadeTotalRegistrosAux,
                    dataInicio,
                    dataFim,
                    pvs,
                    out statusRelatorioAux);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            Tuple<TotalVendasCreditoPorPeriodoBandeira, List<VendasCreditoPorDiaPv>> retorno = base.ConsultaMultiThread(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            registros = retorno.Item2;
            totalizador = retorno.Item1;
            statusRelatorio = statusRelatorioAux;
            statusTotalizador = statusTotalizadorAux;
            quantidadeTotalRegistros = quantidadeTotalRegistrosAux;
        }

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
        public void ConsultarRelatorioVendasCreditoPorDia(            
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
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno statusTotalizadorAux = null, statusRelatorioAux = null;
            Int32 quantidadeTotalRegistrosAux = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<TotalVendasCreditoPorDiaBandeira> funcaoTotalizador = () =>
            {
                return ConsultarTotalVendasCreditoPorDiaBandeira(
                    guidPesquisaTotalizador,
                    dataVenda,
                    numeroPv,
                    out statusTotalizadorAux);
            };

            //Preparação da Func para consulta dos registros
            Func<List<ResumoVendasCreditoPorDia>> funcaoRegistros = () =>
            {
                return ConsultarResumoVendasCreditoPorDia(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref quantidadeTotalRegistrosAux,
                    dataVenda,
                    numeroPv,
                    out statusRelatorioAux);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            Tuple<TotalVendasCreditoPorDiaBandeira, List<ResumoVendasCreditoPorDia>> retorno = base.ConsultaMultiThread(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            registros = retorno.Item2;
            totalizador = retorno.Item1;
            statusRelatorio = statusRelatorioAux;
            statusTotalizador = statusTotalizadorAux;
            quantidadeTotalRegistros = quantidadeTotalRegistrosAux;
        }


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
        public void ConsultarRelatorioVendasCreditoPorDiaTodos(
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
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno statusTotalizadorAux = null, statusRelatorioAux = null;
            Int32 quantidadeTotalRegistrosAux = quantidadeTotalRegistros;
            var listaPvsDatasVendas = default(List<Tuple<Int32, DateTime>>);

            listaPvsDatasVendas = this.ConsultarPvDataVendaCreditoPorPeriodo(guidPesquisaPvDataVenda, dataInicio, dataFim, pvs, out statusRelatorio); 

            //Preparação da Func para consulta dos totalizadores
            Func<TotalVendasCreditoPorDiaBandeira> funcaoTotalizador = () =>
            {
                return ConsultarTotalVendasCreditoPorDiaBandeiraTodos(
                    guidPesquisaTotalizador,                    
                    listaPvsDatasVendas,
                    out statusTotalizadorAux);
            };

            //Preparação da Func para consulta dos registros
            Func<List<ResumoVendasCreditoPorDia>> funcaoRegistros = () =>
            {
                return ConsultarResumoVendasCreditoPorDiaTodos(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref quantidadeTotalRegistrosAux,      
                    listaPvsDatasVendas,
                    out statusRelatorioAux);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            Tuple<TotalVendasCreditoPorDiaBandeira, List<ResumoVendasCreditoPorDia>> retorno = base.ConsultaMultiThread(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            registros = retorno.Item2;
            totalizador = retorno.Item1;
            statusRelatorio = statusRelatorioAux;
            statusTotalizador = statusTotalizadorAux;
            quantidadeTotalRegistros = quantidadeTotalRegistrosAux;
        }

        #endregion

        #region [Relatórios Débito]

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
        public void ConsultarRelatorioVendasDebitoPorPeriodo(
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
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno statusTotalizadorAux = null, statusRelatorioAux = null;
            Int32 quantidadeTotalRegistrosAux = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<TotalVendasDebitoPorPeriodoBandeira> funcaoTotalizador = () =>
            {
                return ConsultarTotalVendasDebitoPorPeriodoBandeira(
                    guidPesquisaTotalizador,
                    dataInicio,
                    dataFim,
                    pvs,
                    out statusTotalizadorAux);
            };

            //Preparação da Func para consulta dos registros
            Func<List<VendasDebitoPorDiaPv>> funcaoRegistros = () =>
            {
                return ConsultarVendasDebitoPorDiaPv(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref quantidadeTotalRegistrosAux,
                    dataInicio,
                    dataFim,
                    pvs,
                    out statusRelatorioAux);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            Tuple<TotalVendasDebitoPorPeriodoBandeira, List<VendasDebitoPorDiaPv>> retorno = base.ConsultaMultiThread(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            registros = retorno.Item2;
            totalizador = retorno.Item1;
            statusRelatorio = statusRelatorioAux;
            statusTotalizador = statusTotalizadorAux;
            quantidadeTotalRegistros = quantidadeTotalRegistrosAux;
        }

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
        public void ConsultarRelatorioVendasDebitoPorDia(
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
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno statusTotalizadorAux = null, statusRelatorioAux = null;
            Int32 quantidadeTotalRegistrosAux = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<TotalVendasDebitoPorDiaBandeira> funcaoTotalizador = () =>
            {
                return ConsultarTotalVendasDebitoPorDiaBandeira(
                    guidPesquisaTotalizador,
                    dataVenda,
                    numeroPv,
                    out statusTotalizadorAux);
            };

            //Preparação da Func para consulta dos registros
            Func<List<ResumoVendasDebitoPorDia>> funcaoRegistros = () =>
            {
                return ConsultarResumoVendasDebitoPorDia(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref quantidadeTotalRegistrosAux,
                    dataVenda,
                    numeroPv,
                    out statusRelatorioAux);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            Tuple<TotalVendasDebitoPorDiaBandeira, List<ResumoVendasDebitoPorDia>> retorno = base.ConsultaMultiThread(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            registros = retorno.Item2;
            totalizador = retorno.Item1;
            statusRelatorio = statusRelatorioAux;
            statusTotalizador = statusTotalizadorAux;
            quantidadeTotalRegistros = quantidadeTotalRegistrosAux;
        }


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
         public void ConsultarRelatorioVendasDebitoPorDiaTodos(
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
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno statusTotalizadorAux = null, statusRelatorioAux = null;
            Int32 quantidadeTotalRegistrosAux = quantidadeTotalRegistros;

            var listaPvsDatasVendas = default(List<Tuple<Int32, DateTime>>);
            listaPvsDatasVendas = this.ConsultarPvDataVendaDebitoPorPeriodo(guidPesquisaPvDataVenda, dataInicio, dataFim, pvs, out statusRelatorio);

            //Preparação da Func para consulta dos totalizadores
            Func<TotalVendasDebitoPorDiaBandeira> funcaoTotalizador = () =>
            {
                return ConsultarTotalVendasDebitoPorDiaBandeiraTodos(
                    guidPesquisaTotalizador,
                    listaPvsDatasVendas,
                    out statusTotalizadorAux);
            };

            //Preparação da Func para consulta dos registros
            Func<List<ResumoVendasDebitoPorDia>> funcaoRegistros = () =>
            {
                return ConsultarResumoVendasDebitoPorDiaTodos(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref quantidadeTotalRegistrosAux,                    
                    listaPvsDatasVendas,
                    out statusRelatorioAux);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            Tuple<TotalVendasDebitoPorDiaBandeira, List<ResumoVendasDebitoPorDia>> retorno = base.ConsultaMultiThread(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            registros = retorno.Item2;
            totalizador = retorno.Item1;
            statusRelatorio = statusRelatorioAux;
            statusTotalizador = statusTotalizadorAux;
            quantidadeTotalRegistros = quantidadeTotalRegistrosAux;
        }

        #endregion

        #endregion

        #region [Métodos Crédito]

        #region [ Vendas - Crédito - Totalizador por período - BKWA2510 / WAC251 / ISGM ]

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
        public TotalVendasCreditoPorPeriodo ConsultarTotalVendasCreditoPorPeriodo(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas crédito por período - BKWA2510 / WAC251 / ISGM"))
            {
                log.GravarLog(EventoLog.InicioServico, new { dataInicio = dataInicio.ToString("yyyyMMdd").ToInt32(), dataFim = dataFim.ToString("yyyyMMdd").ToInt32(),
                    pvs = String.Join(",", pvs) });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var statusDto = default(StatusRetornoDTO);

                    //Execução da pesquisa mainframe
                    DTO.TotalVendasCreditoPorPeriodo totalVendasCreditoPorPeriodo =
                        BLL.Instancia.ConsultarTotalVendasCreditoPorPeriodo(
                            dataInicio,
                            dataFim,
                            pvs,
                            out statusDto);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDto == null || statusDto.CodigoRetorno != 0)
                    {
                        status = StatusRetorno.FromDTO(statusDto);
                        log.GravarLog(EventoLog.FimServico, new { status, totalVendasCreditoPorPeriodo });
                        return null;
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    TotalVendasCreditoPorPeriodo retorno = TotalVendasCreditoPorPeriodo.FromDto(totalVendasCreditoPorPeriodo);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region [ Vendas - Crédito - Totalizador por período e por bandeira - BKWA2530 / WAC253 / ISGO ]

        /// <summary>
        /// Consulta o total de vendas realizadas á crédito no período informado e retorna separado por bandeira.<br/>        
        /// - Book BKWA2530	/ Programa WAC253 / TranID ISGO / Método ConsultarTotalVendasCreditoPorPeriodoBandeira
        /// </summary>
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
        public TotalVendasCreditoPorPeriodoBandeira ConsultarTotalVendasCreditoPorPeriodoBandeira(
            Guid guidPesquisa,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas crédito por bandeira - BKWA2530 / WAC253 / ISGO"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, dataInicio = dataInicio.ToString("yyyyMMdd").ToInt32(), dataFim = dataFim.ToString("yyyyMMdd").ToInt32(),
                    pvs = String.Join(",", pvs) });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    String idPesquisa = guidPesquisa.ToString();
                    var statusDto = default(StatusRetornoDTO);

                    //Retorna os dados da pesquisa do cache
                    DTO.TotalVendasCreditoPorPeriodoBandeira totalVendasCreditoPorPeriodoBandeira =
                        CacheAdmin.Recuperar<DTO.TotalVendasCreditoPorPeriodoBandeira>(Cache.Extrato, idPesquisa);

                    if (totalVendasCreditoPorPeriodoBandeira == null)
                    {
                        //Execução da pesquisa mainframe
                        totalVendasCreditoPorPeriodoBandeira = 
                            BLL.Instancia.ConsultarTotalVendasCreditoPorPeriodoBandeira(
                                dataInicio, 
                                dataFim, 
                                pvs, 
                                out statusDto);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDto == null || statusDto.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDto);
                            log.GravarLog(EventoLog.FimServico, new { status, totalVendasCreditoPorPeriodoBandeira });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, totalVendasCreditoPorPeriodoBandeira);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    TotalVendasCreditoPorPeriodoBandeira retorno = TotalVendasCreditoPorPeriodoBandeira.FromDto(totalVendasCreditoPorPeriodoBandeira);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region [ Vendas - Crédito - Totalizador por dia e por bandeira - BKWA2570 / WAC257 / ISGS ]

        /// <summary>
        /// Consulta o total de vendas realizadas á crédito na data informada por bandeira.<br/>        
        /// - Book BKWA2570	/ Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2570 / Programa WAC257 / TranID ISGS / Método ConsultarTotalVendasCreditoPorDiaBandeira
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="dataVenda">Data da venda que se deseja consultar.</param>
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito do dia por bandeira.</returns>
        public TotalVendasCreditoPorDiaBandeira ConsultarTotalVendasCreditoPorDiaBandeira(
            Guid guidPesquisa,
            DateTime dataVenda,
            Int32 numeroPv,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas crédito por dia e por bandeira - BKWA2570 / WAC257 / ISGS"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, dataInicio = dataVenda.ToString("yyyyMMdd").ToInt32(), numeroPv });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    String idPesquisa = guidPesquisa.ToString();
                    var statusDto = default(StatusRetornoDTO);

                    //Retorna os dados da pesquisa do cache
                    DTO.TotalVendasCreditoPorDiaBandeira totalVendasCreditoPorDiaBandeira =
                        CacheAdmin.Recuperar<DTO.TotalVendasCreditoPorDiaBandeira>(Cache.Extrato, idPesquisa);

                    if (totalVendasCreditoPorDiaBandeira == null)
                    {
                        //Execução da pesquisa mainframe
                        totalVendasCreditoPorDiaBandeira =
                            BLL.Instancia.ConsultarTotalVendasCreditoPorDiaBandeira(
                                dataVenda,
                                numeroPv, 
                                out statusDto);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDto == null || statusDto.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDto);
                            log.GravarLog(EventoLog.FimServico, new { status, totalVendasCreditoPorDiaBandeira });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, totalVendasCreditoPorDiaBandeira);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    TotalVendasCreditoPorDiaBandeira retorno = TotalVendasCreditoPorDiaBandeira.FromDto(totalVendasCreditoPorDiaBandeira);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

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
        public TotalVendasCreditoPorDiaBandeira ConsultarTotalVendasCreditoPorDiaBandeiraTodos(
            Guid guidPesquisa,
            Guid guidPesquisaPvDataVenda,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas crédito por dia e por bandeira - BKWA2570 / WAC257 / ISGS - Ver Todos"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, guidPesquisaPvDataVenda, dataInicio, dataFim, pvs });                                

                try
                {
                    //Monta Tuple para chamar o método local pois essa chamada é do Front, que é framework .NET 3.5, e não conhece a classe Tuple.
                    var listaPvsDatasVendas = default(List<Tuple<Int32, DateTime>>);
                    listaPvsDatasVendas = this.ConsultarPvDataVendaCreditoPorPeriodo(guidPesquisaPvDataVenda, dataInicio, dataFim, pvs, out status);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (status == null || status.CodigoRetorno != 0)
                    {
                        log.GravarLog(EventoLog.FimServico, new { status, listaPvsDatasVendas });
                        return null;
                    }

                    TotalVendasCreditoPorDiaBandeira retorno = ConsultarTotalVendasCreditoPorDiaBandeiraTodos(guidPesquisa, listaPvsDatasVendas, out status);
                    
                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

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
        /// <param name="listaPvsDatasVendas">Lista de pares (Número do estabelecimento e Data Venda) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas crédito do dia por bandeira.</returns>
        private TotalVendasCreditoPorDiaBandeira ConsultarTotalVendasCreditoPorDiaBandeiraTodos(
            Guid guidPesquisa,
            List<Tuple<Int32, DateTime>> listaPvsDatasVendas,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas crédito por dia e por bandeira - BKWA2570 / WAC257 / ISGS - Ver Todos"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, listaPvDataVenda = listaPvsDatasVendas });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    String idPesquisa = guidPesquisa.ToString();
                    var statusDto = default(StatusRetornoDTO);

                    //Retorna os dados da pesquisa do cache
                    DTO.TotalVendasCreditoPorDiaBandeira totalVendasCreditoPorDiaBandeira =
                        CacheAdmin.Recuperar<DTO.TotalVendasCreditoPorDiaBandeira>(Cache.Extrato, idPesquisa);

                    if (totalVendasCreditoPorDiaBandeira == null)
                    {
                        //Execução da pesquisa mainframe
                        totalVendasCreditoPorDiaBandeira =
                            BLL.Instancia.ConsultarTotalVendasCreditoPorDiaBandeiraTodos(
                                listaPvsDatasVendas,
                                out statusDto);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDto == null || statusDto.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDto);
                            log.GravarLog(EventoLog.FimServico, new { status, totalVendasCreditoPorDiaBandeira });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, totalVendasCreditoPorDiaBandeira);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    TotalVendasCreditoPorDiaBandeira retorno = TotalVendasCreditoPorDiaBandeira.FromDto(totalVendasCreditoPorDiaBandeira);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region [ Vendas - Crédito - Registros por periodo - BKWA2540 / WAC254 / ISGP ]

        /// <summary>
        /// Consulta as vendas realizadas á crédito, no período informado, separadas por dia e por PV (Ponto de Venda).<br/>        
        /// - Book BKWA2540	/ Programa WAC254 / TranID ISGP / Método ConsultarVendasCreditoPorDiaPv
        /// </summary>
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
        public List<VendasCreditoPorDiaPv> ConsultarVendasCreditoPorDiaPv(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas crédito por dia por PV - BKWA2540 / WAC254 / ISGP"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros, quantidadeTotalRegistros,
                    dataInicio = dataInicio.ToString("yyyyMMdd").ToInt32(), dataFim = dataFim.ToString("yyyyMMdd").ToInt32(), pvs });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDto = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.VendasCreditoPorDiaPv>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        List<DTO.VendasCreditoPorDiaPv> listaVendasCreditoPorDiaPv =
                            BLL.Instancia.ConsultarVendasCreditoPorDiaPv(
                                dataInicio,
                                dataFim,
                                pvs,
                                out indicadorRechamada,
                                ref rechamada,
                                out statusDto);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDto == null || statusDto.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDto);
                            log.GravarLog(EventoLog.FimServico, new { status, listaVendasCreditoPorDiaPv, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, listaVendasCreditoPorDiaPv, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.VendasCreditoPorDiaPv>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    List<VendasCreditoPorDiaPv> retorno = VendasCreditoPorDiaPv.FromDto(dadosCache);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta as lista de PV's de datas das vendas realizadas á crédito, no período informado, separadas por dia e por PV (Ponto de Venda). - Ver Todos.<br/>        
        /// - Book BKWA2540	/ Programa WAC254 / TranID ISGP / Método ConsultarVendasCreditoPorDiaPv
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2540 / Programa WAC254 / TranID ISGP / Método ConsultarVendasCreditoPorDiaPv 
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="dataInicio">Data inicio que se deseja consultar.</param>
        /// <param name="dataFim">Data fim que se deseja consultar.</param>
        /// <param name="pvs">Lista dos Pontos de Venda (Estabelecimentos) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Lista de pares (Numero de ponto de venda e Data Venda) por período e por PV.</returns>
        private List<Tuple<Int32, DateTime>> ConsultarPvDataVendaCreditoPorPeriodo(
            Guid guidPesquisa,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas crédito por dia por PV - BKWA2540 / WAC254 / ISGP - Ver Todos."))
            {
                log.GravarLog(EventoLog.InicioServico, new { dataInicio = dataInicio.ToString("yyyyMMdd").ToInt32(), 
                    dataFim = dataFim.ToString("yyyyMMdd").ToInt32(), pvs });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    Boolean indicadorRechamada = true;
                    var statusDto = default(StatusRetornoDTO);
                    var listaVendasCreditoPorDiaPv = default(List<DTO.VendasCreditoPorDiaPv>);
                    String idPesquisa = guidPesquisa.ToString();

                    //Retorna os dados da pesquisa do cache
                    listaVendasCreditoPorDiaPv =
                        CacheAdmin.Recuperar<List<DTO.VendasCreditoPorDiaPv>>(Cache.Extrato, idPesquisa);

                    if (listaVendasCreditoPorDiaPv == null)
                    {
                        listaVendasCreditoPorDiaPv = new List<DTO.VendasCreditoPorDiaPv>();

                        //Enquanto houver registro é necessário executar a pesquisa
                        while (indicadorRechamada == true)
                        {
                            //Execução da pesquisa mainframe
                            List<DTO.VendasCreditoPorDiaPv> listaVendasCreditoAux =
                                BLL.Instancia.ConsultarVendasCreditoPorDiaPv(
                                    dataInicio,
                                    dataFim,
                                    pvs,
                                    out indicadorRechamada,
                                    ref rechamada,
                                    out statusDto);

                            //Em caso de código de retorno != 0, sai do método, sem sucesso
                            if (statusDto == null || statusDto.CodigoRetorno != 0)
                            {
                                status = StatusRetorno.FromDTO(statusDto);
                                log.GravarLog(EventoLog.FimServico, new { status, listaVendasCreditoPorDiaPv });
                                return null;
                            }

                            listaVendasCreditoPorDiaPv.AddRange(listaVendasCreditoAux);
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, listaVendasCreditoPorDiaPv);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    var retorno = new List<Tuple<Int32, DateTime>>();

                    //Gera uma lista somente com as informações de PV e Data Venda.
                    if (listaVendasCreditoPorDiaPv.Count > 0)
                        listaVendasCreditoPorDiaPv.ForEach(item => retorno.Add(
                            Tuple.Create(item.NumeroEstabelecimanto, item.DataVenda.HasValue ? item.DataVenda.Value : DateTime.MinValue)));

                    log.GravarLog(EventoLog.FimServico, new { status, dados = listaVendasCreditoPorDiaPv });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }        

        #endregion

        #region [ Vendas - Crédito - Registros por dia - BKWA2580 / WAC258 / ISGT ]

        /// <summary>
        /// Consulta o resumo das vendas realizadas á crédito na data informada por bandeira.<br/>        
        /// - Book BKWA2580	/ Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// </summary>
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
        public List<ResumoVendasCreditoPorDia> ConsultarResumoVendasCreditoPorDia(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataVenda,
            Int32 numeroPv,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas crédito por dia por PV - BKWA2580 / WAC258 / ISGT"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros, quantidadeTotalRegistros,
                    dataVenda = dataVenda.ToString("yyyyMMdd").ToInt32(), numeroPv });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDto = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.ResumoVendasCreditoPorDia>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        List<DTO.ResumoVendasCreditoPorDia> listaResumoVendasCreditoPorDia =
                            BLL.Instancia.ConsultarResumoVendasCreditoPorDia(
                                dataVenda,
                                numeroPv,
                                out indicadorRechamada,
                                ref rechamada,
                                out statusDto);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDto == null || statusDto.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDto);
                            log.GravarLog(EventoLog.FimServico, new { status, listaResumoVendasCreditoPorDia, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, listaResumoVendasCreditoPorDia, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.ResumoVendasCreditoPorDia>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    List<ResumoVendasCreditoPorDia> retorno = ResumoVendasCreditoPorDia.FromDto(dadosCache);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

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
        public List<ResumoVendasCreditoPorDia> ConsultarResumoVendasCreditoPorDiaTodos(
            Guid guidPesquisa,
            Guid guidPesquisaPvDataVenda,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,            
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas crédito por dia por PV - BKWA2580 / WAC258 / ISGT - Ver Todos"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros, quantidadeTotalRegistros,
                    guidPesquisaPvDataVenda, dataInicio, dataFim, pvs });

                try
                {
                    //Monta Tuple para chamar o método local pois essa chamada é do Front, que é framework .NET 3.5, e não conhece a classe Tuple.
                    var listaPvsDatasVendas = default(List<Tuple<Int32, DateTime>>);
                    listaPvsDatasVendas = this.ConsultarPvDataVendaCreditoPorPeriodo(guidPesquisaPvDataVenda, dataInicio, dataFim, pvs, out status);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (status == null || status.CodigoRetorno != 0)
                    {
                        log.GravarLog(EventoLog.FimServico, new { status, listaPvsDatasVendas, quantidadeTotalRegistros });
                        return null;
                    }

                    List<ResumoVendasCreditoPorDia> retorno = ConsultarResumoVendasCreditoPorDiaTodos(
                        guidPesquisa,
                        registroInicial,
                        quantidadeRegistros,
                        ref quantidadeTotalRegistros,
                        listaPvsDatasVendas,
                        out status);

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }


        /// <summary>
        /// Consulta o resumo das vendas realizadas á crédito na data informada por bandeira - Ver Todos.<br/>        
        /// - Book BKWA2580	/ Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2580 / Programa WAC258 / TranID ISGT / Método ConsultarResumoVendasCreditoPorDia
        /// </remarks>        
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="listaPvsDatasVendas">Lista de pares (Número do estabelecimento e Data Venda) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Resumo das vendas crédito do período informado.</returns>
        private List<ResumoVendasCreditoPorDia> ConsultarResumoVendasCreditoPorDiaTodos(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            List<Tuple<Int32, DateTime>> listaPvsDatasVendas,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas crédito por dia por PV - BKWA2580 / WAC258 / ISGT - Ver Todos"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros, quantidadeTotalRegistros, listaPvsDatasVendas });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDto = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.ResumoVendasCreditoPorDia>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        Int32 indexPvDataVenda = rechamada.GetValueOrDefault<Int32>("indexPvDataVenda", 0);
                        Tuple<Int32, DateTime> itemPvDataVenda = listaPvsDatasVendas[indexPvDataVenda];

                        //Execução da pesquisa mainframe
                        List<DTO.ResumoVendasCreditoPorDia> listaResumoVendasCreditoPorDia =
                            BLL.Instancia.ConsultarResumoVendasCreditoPorDia(
                                itemPvDataVenda.Item2,
                                itemPvDataVenda.Item1,
                                out indicadorRechamada,
                                ref rechamada,
                                out statusDto);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDto == null || statusDto.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDto);
                            log.GravarLog(EventoLog.FimServico, new { status, listaResumoVendasCreditoPorDia, quantidadeTotalRegistros });
                            return null;
                        }

                        //Verifica se deve pesquisar o próximo registro.
                        if (indicadorRechamada == false)
                        {
                            //Se tem próximo registro a ser percorrido, avança o index                           
                            if (indexPvDataVenda < listaPvsDatasVendas.Count - 1)
                            {
                                rechamada.Clear();
                                rechamada["indexPvDataVenda"] = indexPvDataVenda + 1;
                                indicadorRechamada = true;
                            }
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, listaResumoVendasCreditoPorDia, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.ResumoVendasCreditoPorDia>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    List<ResumoVendasCreditoPorDia> retorno = ResumoVendasCreditoPorDia.FromDto(dadosCache);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #endregion

        #region [Métodos Débito]

        #region [ Vendas - Débito - Totalizador por período - BKWA2520 / WAC252 / ISGN ]

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
        public TotalVendasDebitoPorPeriodo ConsultarTotalVendasDebitoPorPeriodo(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas débito por período - BKWA2520 / WAC252 / ISGN"))
            {
                log.GravarLog(EventoLog.InicioServico, new { dataInicio = dataInicio.ToString("yyyyMMdd").ToInt32(), dataFim = dataFim.ToString("yyyyMMdd").ToInt32(),
                    pvs = String.Join(",", pvs) });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var statusDto = default(StatusRetornoDTO);

                    //Execução da pesquisa mainframe
                    DTO.TotalVendasDebitoPorPeriodo totalVendasDebitoPorPeriodo =
                        BLL.Instancia.ConsultarTotalVendasDebitoPorPeriodo(
                            dataInicio,
                            dataFim,
                            pvs,
                            out statusDto);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDto == null || statusDto.CodigoRetorno != 0)
                    {
                        status = StatusRetorno.FromDTO(statusDto);
                        log.GravarLog(EventoLog.FimServico, new { status, totalVendasDebitoPorPeriodo });
                        return null;
                    }

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    TotalVendasDebitoPorPeriodo retorno = TotalVendasDebitoPorPeriodo.FromDto(totalVendasDebitoPorPeriodo);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region [ Vendas - Débito - Totalizadores por período por bandeira - BKWA2550 / WAC255 / ISGQ ]

        /// <summary>
        /// Consulta o total de vendas realizadas á débito no período informado e retorna separado por bandeira.<br/>        
        /// - Book BKWA2550	/ Programa WAC255 / TranID ISGQ / Método ConsultarTotalVendasDebitoPorPeriodoBandeira
        /// </summary>
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
        public TotalVendasDebitoPorPeriodoBandeira ConsultarTotalVendasDebitoPorPeriodoBandeira(
            Guid guidPesquisa,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas débito por bandeira - BKWA2550 / WAC255 / ISGQ"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, dataInicio = dataInicio.ToString("yyyyMMdd").ToInt32(), 
                    dataFim = dataFim.ToString("yyyyMMdd").ToInt32(), pvs = String.Join(",", pvs) });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    String idPesquisa = guidPesquisa.ToString();
                    var statusDto = default(StatusRetornoDTO);

                    //Retorna os dados da pesquisa do cache
                    DTO.TotalVendasDebitoPorPeriodoBandeira totalVendasDebitoPorPeriodoBandeira =
                        CacheAdmin.Recuperar<DTO.TotalVendasDebitoPorPeriodoBandeira>(Cache.Extrato, idPesquisa);

                    if (totalVendasDebitoPorPeriodoBandeira == null)
                    {
                        //Execução da pesquisa mainframe
                        totalVendasDebitoPorPeriodoBandeira =
                            BLL.Instancia.ConsultarTotalVendasDebitoPorPeriodoBandeira(
                                dataInicio,
                                dataFim,
                                pvs,
                                out statusDto);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDto == null || statusDto.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDto);
                            log.GravarLog(EventoLog.FimServico, new { status, totalVendasDebitoPorPeriodoBandeira });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, totalVendasDebitoPorPeriodoBandeira);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    TotalVendasDebitoPorPeriodoBandeira retorno = TotalVendasDebitoPorPeriodoBandeira.FromDto(totalVendasDebitoPorPeriodoBandeira);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region [ Vendas - Débito - Totalizadores por dia por bandeira - BKWA2590 / WAC259 / ISGU ]

        /// <summary>
        /// Consulta o total de vendas realizadas á débito na data informada por bandeira.<br/>        
        /// - Book BKWA2590	/ Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2590 / Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>        
        /// <param name="dataVenda">Data da venda que se deseja consultar.</param>
        /// <param name="numeroPv">Número do Ponto de Venda (Estabelecimento) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito do dia por bandeira.</returns>
        public TotalVendasDebitoPorDiaBandeira ConsultarTotalVendasDebitoPorDiaBandeira(
            Guid guidPesquisa,
            DateTime dataVenda,
            Int32 numeroPv,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas débito por dia e por bandeira - BKWA2590 / WAC259 / ISGU"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, dataInicio = dataVenda.ToString("yyyyMMdd").ToInt32(), numeroPv });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    String idPesquisa = guidPesquisa.ToString();
                    var statusDto = default(StatusRetornoDTO);

                    //Retorna os dados da pesquisa do cache
                    DTO.TotalVendasDebitoPorDiaBandeira totalVendasDebitoPorDiaBandeira =
                        CacheAdmin.Recuperar<DTO.TotalVendasDebitoPorDiaBandeira>(Cache.Extrato, idPesquisa);

                    if (totalVendasDebitoPorDiaBandeira == null)
                    {
                        //Execução da pesquisa mainframe
                        totalVendasDebitoPorDiaBandeira =
                            BLL.Instancia.ConsultarTotalVendasDebitoPorDiaBandeira(
                                dataVenda,
                                numeroPv,
                                out statusDto);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDto == null || statusDto.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDto);
                            log.GravarLog(EventoLog.FimServico, new { status, totalVendasDebitoPorDiaBandeira });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, totalVendasDebitoPorDiaBandeira);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    TotalVendasDebitoPorDiaBandeira retorno = TotalVendasDebitoPorDiaBandeira.FromDto(totalVendasDebitoPorDiaBandeira);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

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
        public TotalVendasDebitoPorDiaBandeira ConsultarTotalVendasDebitoPorDiaBandeiraTodos(
            Guid guidPesquisa,
            Guid guidPesquisaPvDataVenda,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas débito por dia e por bandeira - BKWA2590 / WAC259 / ISGU - Ver Todos"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, guidPesquisaPvDataVenda, dataInicio, dataFim });

                try
                {
                    //Monta Tuple para chamar o método local pois essa chamada é do Front, que é framework .NET 3.5, e não conhece a classe Tuple.
                    var listaPvsDatasVendas = default(List<Tuple<Int32, DateTime>>);
                    listaPvsDatasVendas = this.ConsultarPvDataVendaDebitoPorPeriodo(guidPesquisaPvDataVenda, dataInicio, dataFim, pvs, out status);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (status == null || status.CodigoRetorno != 0)
                    {
                        log.GravarLog(EventoLog.FimServico, new { status, listaPvsDatasVendas });
                        return null;
                    }

                    TotalVendasDebitoPorDiaBandeira retorno = ConsultarTotalVendasDebitoPorDiaBandeiraTodos(guidPesquisa, listaPvsDatasVendas, out status);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta o total de vendas realizadas á débito na data informada por bandeira - Ver Todos<br/>        
        /// - Book BKWA2590	/ Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2590 / Programa WAC259 / TranID ISGU / Método ConsultarTotalVendasDebitoPorDiaBandeira
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>      
        /// <param name="listaPvsDatasVendas">Lista de pares (Número do estabelecimento e Data Venda) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito do dia por bandeira.</returns>
        private TotalVendasDebitoPorDiaBandeira ConsultarTotalVendasDebitoPorDiaBandeiraTodos(
            Guid guidPesquisa,
            List<Tuple<Int32, DateTime>> listaPvsDatasVendas,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas débito por dia e por bandeira - BKWA2590 / WAC259 / ISGU - Ver Todos"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, listaPvsDatasVendas });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    String idPesquisa = guidPesquisa.ToString();
                    var statusDto = default(StatusRetornoDTO);

                    //Retorna os dados da pesquisa do cache
                    DTO.TotalVendasDebitoPorDiaBandeira totalVendasDebitoPorDiaBandeira =
                        CacheAdmin.Recuperar<DTO.TotalVendasDebitoPorDiaBandeira>(Cache.Extrato, idPesquisa);

                    if (totalVendasDebitoPorDiaBandeira == null)
                    {
                        //Execução da pesquisa mainframe
                        totalVendasDebitoPorDiaBandeira =
                            BLL.Instancia.ConsultarTotalVendasDebitoPorDiaBandeiraTodos(
                                listaPvsDatasVendas,
                                out statusDto);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDto == null || statusDto.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDto);
                            log.GravarLog(EventoLog.FimServico, new { status, totalVendasCreditoPorDiaBandeira = totalVendasDebitoPorDiaBandeira });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, totalVendasDebitoPorDiaBandeira);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    TotalVendasDebitoPorDiaBandeira retorno = TotalVendasDebitoPorDiaBandeira.FromDto(totalVendasDebitoPorDiaBandeira);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region [ Vendas - Débito - Registros por período - BKWA2560 / WAC256 / ISGR ]

        /// <summary>
        /// Consulta as vendas realizadas á débito, no período informado, separadas por dia e por PV (Ponto de Venda).<br/>        
        /// - Book BKWA2560	/ Programa WAC256 / TranID ISGR / Método ConsultarVendasDebitoPorDiaPv
        /// </summary>
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
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Total vendas débito por dia e por PV</returns>
        public List<VendasDebitoPorDiaPv> ConsultarVendasDebitoPorDiaPv(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas débito por dia por PV - BKWA2560 / WAC256 / ISGR"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros, quantidadeTotalRegistros,
                    dataInicio = dataInicio.ToString("yyyyMMdd").ToInt32(), dataFim = dataFim.ToString("yyyyMMdd").ToInt32(), pvs });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDto = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.VendasDebitoPorDiaPv>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        List<DTO.VendasDebitoPorDiaPv> listaVendasDebitoPorDiaPv =
                            BLL.Instancia.ConsultarVendasDebitoPorDiaPv(
                                dataInicio,
                                dataFim,
                                pvs,
                                out indicadorRechamada,
                                ref rechamada,
                                out statusDto);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDto == null || statusDto.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDto);
                            log.GravarLog(EventoLog.FimServico, new { status, listaVendasDebitoPorDiaPv, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, listaVendasDebitoPorDiaPv, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.VendasDebitoPorDiaPv>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    List<VendasDebitoPorDiaPv> retorno = VendasDebitoPorDiaPv.FromDto(dadosCache);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta as vendas realizadas á débito, no período informado, separadas por dia e por PV (Ponto de Venda) - Ver Todos.<br/>        
        /// - Book BKWA2560	/ Programa WAC256 / TranID ISGR / Método ConsultarVendasDebitoPorDiaPv
        /// </summary>
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
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Lista de pares (Numero de ponto de venda e Data Venda) por período e por PV.</returns>
        private List<Tuple<Int32, DateTime>> ConsultarPvDataVendaDebitoPorPeriodo(
            Guid guidPesquisa,
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas débito por dia por PV - BKWA2560 / WAC256 / ISGR - Ver Todos."))
            {
                log.GravarLog(EventoLog.InicioServico, new { dataInicio = dataInicio.ToString("yyyyMMdd").ToInt32(), 
                    dataFim = dataFim.ToString("yyyyMMdd").ToInt32(), pvs });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    Boolean indicadorRechamada = true;
                    var statusDto = default(StatusRetornoDTO);
                    var listaVendasDebitoPorDiaPv = default(List<DTO.VendasDebitoPorDiaPv>);
                    String idPesquisa = guidPesquisa.ToString();

                    //Retorna os dados da pesquisa do cache
                    listaVendasDebitoPorDiaPv =
                        CacheAdmin.Recuperar<List<DTO.VendasDebitoPorDiaPv>>(Cache.Extrato, idPesquisa);

                    if (listaVendasDebitoPorDiaPv == null)
                    {
                        listaVendasDebitoPorDiaPv = new List<DTO.VendasDebitoPorDiaPv>();

                        //Enquanto houver registro é necessário executar a pesquisa
                        while (indicadorRechamada == true)
                        {
                            //Execução da pesquisa mainframe
                            List<DTO.VendasDebitoPorDiaPv> listaVendasCreditoAux =
                                BLL.Instancia.ConsultarVendasDebitoPorDiaPv(
                                    dataInicio,
                                    dataFim,
                                    pvs,
                                    out indicadorRechamada,
                                    ref rechamada,
                                    out statusDto);

                            //Em caso de código de retorno != 0, sai do método, sem sucesso
                            if (statusDto == null || statusDto.CodigoRetorno != 0)
                            {
                                status = StatusRetorno.FromDTO(statusDto);
                                log.GravarLog(EventoLog.FimServico, new { status, listaVendasCreditoPorDiaPv = listaVendasDebitoPorDiaPv });
                                return null;
                            }

                            listaVendasDebitoPorDiaPv.AddRange(listaVendasCreditoAux);
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, listaVendasDebitoPorDiaPv);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    var retorno = new List<Tuple<Int32, DateTime>>();

                    //Gera uma lista somente com as informações de PV e Data Venda.
                    if (listaVendasDebitoPorDiaPv.Count > 0)
                        listaVendasDebitoPorDiaPv.ForEach(item => retorno.Add(
                            Tuple.Create(item.NumeroEstabelecimanto, item.DataVenda.HasValue ? item.DataVenda.Value : DateTime.MinValue)));

                    log.GravarLog(EventoLog.FimServico, new { status, dados = listaVendasDebitoPorDiaPv });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }        

        #endregion

        #region [ Vendas - Débito - Registro por dia - BKWA2600 / WAC260 / ISGV ]

        /// <summary>
        /// Consulta o resumo das vendas realizadas á débito na data informada por bandeira.<br/>        
        /// - Book BKWA2600	/ Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// </summary>
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
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Resumo das vendas débito do dia.</returns>
        public List<ResumoVendasDebitoPorDia> ConsultarResumoVendasDebitoPorDia(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataVenda,
            Int32 numeroPv,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Resumo de vendas débito por dia - BKWA2600 / WAC260 / ISGV"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros, quantidadeTotalRegistros,
                    dataVenda = dataVenda.ToString("yyyyMMdd").ToInt32(), numeroPv });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDto = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.ResumoVendasDebitoPorDia>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        List<DTO.ResumoVendasDebitoPorDia> listaResumoVendasDebitoPorDia =
                            BLL.Instancia.ConsultarResumoVendasDebitoPorDia(
                                dataVenda,
                                numeroPv,
                                out indicadorRechamada,
                                ref rechamada,
                                out statusDto);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDto == null || statusDto.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDto);
                            log.GravarLog(EventoLog.FimServico, new { status, listaResumoVendasDebitoPorDia, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, listaResumoVendasDebitoPorDia, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.ResumoVendasDebitoPorDia>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    List<ResumoVendasDebitoPorDia> retorno = ResumoVendasDebitoPorDia.FromDto(dadosCache);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

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
        public List<ResumoVendasDebitoPorDia> ConsultarResumoVendasDebitoPorDiaTodos(
            Guid guidPesquisa,
            Guid guidPesquisaPvDataVenda,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,            
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Resumo de vendas débito por dia - BKWA2600 / WAC260 / ISGV - Ver Todos."))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros, quantidadeTotalRegistros,
                    guidPesquisaPvDataVenda, dataInicio, dataFim, pvs });

                try
                {
                    //Monta Tuple para chamar o método local pois essa chamada é do Front, que é framework .NET 3.5, e não conhece a classe Tuple.
                    var listaPvsDatasVendas = default(List<Tuple<Int32, DateTime>>);
                    listaPvsDatasVendas = this.ConsultarPvDataVendaDebitoPorPeriodo(guidPesquisaPvDataVenda, dataInicio, dataFim, pvs, out status);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (status == null || status.CodigoRetorno != 0)
                    {                        
                        log.GravarLog(EventoLog.FimServico, new { status, listaPvsDatasVendas, quantidadeTotalRegistros });
                        return null;
                    }

                    List<ResumoVendasDebitoPorDia> retorno = ConsultarResumoVendasDebitoPorDiaTodos(
                        guidPesquisa,
                        registroInicial,
                        quantidadeRegistros,
                        ref quantidadeTotalRegistros,
                        listaPvsDatasVendas,
                        out status);

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        /// <summary>
        /// Consulta o resumo das vendas realizadas á débito na data informada por bandeira - Ver Todos.<br/>        
        /// - Book BKWA2600	/ Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book BKWA2600 / Programa WAC260 / TranID ISGV / Método ConsultarResumoVendasDebitoPorDia
        /// </remarks>
        /// <param name="guidPesquisa">Identificador da pesquisa (utilizado para cache de dados do relatório)</param>
        /// <param name="registroInicial">Índice do registro inicial a ser retornado</param>
        /// <param name="quantidadeRegistros">Quantidade de registros a serem retornados</param>
        /// <param name="quantidadeTotalRegistros">Quantidade total de registros na pesquisa</param>
        /// <param name="listaPvsDatasVendas">Lista de pares (Número do estabelecimento e Data Venda) que se deseja consultar.</param>
        /// <param name="status">Objeto com as informações retorno da execução do método no Mainframe.</param>
        /// <returns>Resumo das vendas débito do dia.</returns>
        private List<ResumoVendasDebitoPorDia> ConsultarResumoVendasDebitoPorDiaTodos(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            List<Tuple<Int32, DateTime>> listaPvsDatasVendas,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Resumo de vendas débito por dia - BKWA2600 / WAC260 / ISGV - Ver Todos."))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros, quantidadeTotalRegistros, listaPvsDatasVendas});

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDto = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.ResumoVendasDebitoPorDia>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        Int32 indexPvDataVenda = rechamada.GetValueOrDefault<Int32>("indexPvDataVenda", 0);
                        Tuple<Int32, DateTime> itemPvDataVenda = listaPvsDatasVendas[indexPvDataVenda];

                        //Execução da pesquisa mainframe
                        List<DTO.ResumoVendasDebitoPorDia> listaResumoVendasDebitoPorDia =
                            BLL.Instancia.ConsultarResumoVendasDebitoPorDia(
                                itemPvDataVenda.Item2,
                                itemPvDataVenda.Item1,
                                out indicadorRechamada,
                                ref rechamada,
                                out statusDto);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDto == null || statusDto.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDto);
                            log.GravarLog(EventoLog.FimServico, new { status, listaResumoVendasDebitoPorDia, quantidadeTotalRegistros });
                            return null;
                        }

                        //Verifica se deve pesquisar o próximo registro.
                        if (indicadorRechamada == false)
                        {
                            //Se tem próximo registro a ser percorrido, avança o index                           
                            if (indexPvDataVenda < listaPvsDatasVendas.Count - 1)
                            {
                                rechamada.Clear();
                                rechamada["indexPvDataVenda"] = indexPvDataVenda + 1;
                                indicadorRechamada = true;
                            }
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, listaResumoVendasDebitoPorDia, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.ResumoVendasDebitoPorDia>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    List<ResumoVendasDebitoPorDia> retorno = ResumoVendasDebitoPorDia.FromDto(dadosCache);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

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
        public TotalVendasPorPeriodoConsolidado ConsultarTotalVendasPorPeriodoConsolidado(
            DateTime dataInicio,
            DateTime dataFim,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Total de vendas por período consolidado - BKWA2520 / WAC252 / ISGN e BKWA2520 / Programa WAC252 / TranID ISGN"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    dataInicio = dataInicio.ToString("yyyyMMdd").ToInt32(),
                    dataFim = dataFim.ToString("yyyyMMdd").ToInt32(),
                    pvs = String.Join(",", pvs)
                });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var statusDto = default(StatusRetornoDTO);

                    //Execução da pesquisa mainframe
                    DTO.TotalVendasPorPeriodoConsolidado totalVendasPorPeriodoConsolidado =
                        BLL.Instancia.ConsultarTotalVendasPorPeriodoConsolidado(
                            dataInicio,
                            dataFim,
                            pvs,
                            out statusDto);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDto == null || statusDto.CodigoRetorno != 0)
                    {
                        status = StatusRetorno.FromDTO(statusDto);
                        log.GravarLog(EventoLog.FimServico, new { status, totalVendasPorPeriodoConsolidado });
                        return null;
                    }

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    TotalVendasPorPeriodoConsolidado retorno = TotalVendasPorPeriodoConsolidado.FromDto(totalVendasPorPeriodoConsolidado);

                    log.GravarLog(EventoLog.FimServico, new { status, dados = retorno });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion
    }
}
