/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo.Vendas;
using Redecard.PN.Extrato.Servicos.Vendas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BLL = Redecard.PN.Extrato.Negocio.VendasBLL;
using DTO = Redecard.PN.Extrato.Modelo.Vendas;

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
    public class HISServicoWA_Extrato_Vendas : ServicoBaseExtrato, IHISServicoWA_Extrato_Vendas
    {
        #region [ RELATÓRIOS - Consultas Multi-Thread (Totalizadores e Registros) ]

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
        public void ConsultarRelatorioCredito(
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
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno _statusTotalizador = null, _statusRelatorio = null;            
            Int32 _quantidadeTotalRegistros = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<CreditoTotalizador> _funcaoTotalizador = () => {
                return ConsultarCreditoTotalizadores(
                    guidPesquisaTotalizador,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<Credito>> _funcaoRegistros = () => {
                return ConsultarCredito(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref _quantidadeTotalRegistros,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out _statusRelatorio); };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var _retorno = base.ConsultaMultiThread(_funcaoTotalizador, _funcaoRegistros);

            //Atribuição de dados de retorno
            registros = _retorno.Item2;
            totalizador = _retorno.Item1;
            statusRelatorio = _statusRelatorio;
            statusTotalizador = _statusTotalizador;
            quantidadeTotalRegistros = _quantidadeTotalRegistros;
        }

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
        public void ConsultarRelatorioDebito(
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
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno _statusTotalizador = null, _statusRelatorio = null;            
            Int32 _quantidadeTotalRegistros = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<DebitoTotalizador> _funcaoTotalizador = () => {
                return ConsultarDebitoTotalizadores(
                    guidPesquisaTotalizador,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    modalidade,
                    pvs,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<Debito>> _funcaoRegistros = () => {
                return ConsultarDebito(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref _quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    modalidade,
                    pvs,
                    out _statusRelatorio); };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var _retorno = base.ConsultaMultiThread(_funcaoTotalizador, _funcaoRegistros);

            //Atribuição de dados de retorno
            registros = _retorno.Item2;
            totalizador = _retorno.Item1;
            statusRelatorio = _statusRelatorio;
            statusTotalizador = _statusTotalizador;
            quantidadeTotalRegistros = _quantidadeTotalRegistros;
        }

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
        public void ConsultarRelatorioConstrucard(
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
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno _statusTotalizador = null, _statusRelatorio = null;            
            Int32 _quantidadeTotalRegistros = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<ConstrucardTotalizador> _funcaoTotalizador = () => {
                return ConsultarConstrucardTotalizadores(
                    guidPesquisaTotalizador,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out _statusTotalizador); };

            //Preparação da Func para consulta dos registros
            Func<List<Construcard>> _funcaoRegistros = () => {
                return ConsultarConstrucard(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref _quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out _statusRelatorio); };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var _retorno = base.ConsultaMultiThread(_funcaoTotalizador, _funcaoRegistros);

            //Atribuição de dados de retorno
            registros = _retorno.Item2;
            totalizador = _retorno.Item1;
            statusRelatorio = _statusRelatorio;
            statusTotalizador = _statusTotalizador;
            quantidadeTotalRegistros = _quantidadeTotalRegistros;
        }

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
        public void ConsultarRelatorioRecargaCelularPvFisico(
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
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno stsTotalizador = null, stsRelatorio = null;
            Int32 qtdTotalRegistros = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<RecargaCelularTotalizador> funcaoTotalizador = () =>
            {
                return ConsultarRecargaCelularPvFisicoTotalizadores(
                    guidPesquisaTotalizador,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out stsTotalizador);
            };

            //Preparação da Func para consulta dos registros
            Func<List<RecargaCelularPvFisico>> funcaoRegistros = () =>
            {
                return ConsultarRecargaCelularPvFisico(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref qtdTotalRegistros,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out stsRelatorio);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var retorno = base.ConsultaMultiThread(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            registros = retorno.Item2 != null ? retorno.Item2.Cast<RecargaCelular>().ToList() : null;
            totalizador = retorno.Item1;
            statusRelatorio = stsRelatorio;
            statusTotalizador = stsTotalizador;
            quantidadeTotalRegistros = qtdTotalRegistros;
        }

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
        public void ConsultarRelatorioRecargaCelularPvLogico(
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
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno stsTotalizador = null, stsRelatorio = null;
            Int32 qtdTotalRegistros = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<RecargaCelularTotalizador> funcaoTotalizador = () =>
            {
                return ConsultarRecargaCelularPvLogicoTotalizadores(
                    guidPesquisaTotalizador,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out stsTotalizador);
            };

            //Preparação da Func para consulta dos registros
            Func<List<RecargaCelularPvLogico>> funcaoRegistros = () =>
            {
                return ConsultarRecargaCelularPvLogico(
                    guidPesquisaRelatorio,
                    registroInicial,
                    quantidadeRegistros,
                    ref qtdTotalRegistros,
                    dataInicial,
                    dataFinal,
                    pvs,
                    out stsRelatorio);
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var retorno = base.ConsultaMultiThread(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            registros = retorno.Item2 != null ? retorno.Item2.Cast<RecargaCelular>().ToList() : null;
            totalizador = retorno.Item1;
            statusRelatorio = stsRelatorio;
            statusTotalizador = stsTotalizador;
            quantidadeTotalRegistros = qtdTotalRegistros;
        }

        /// <summary>
        /// Consulta de Relatório de Vendas - Recarga de Celular - PV Lógico ou PV Físico.<br/>
        /// Efetua consulta paralela dos Totalizadores e Registros do relatório.<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
        /// - Book BKWA2610 / Programa WAC263 / TranID WAAH<br/>
        /// - Book BKWA2620 / Programa WAC264 / TranID WAAI
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book BKWA2610 / Programa WAC261 / TranID WAAF<br/>
        /// - Book BKWA2620 / Programa WAC262 / TranID WAAG
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
        /// <param name="tipoPv">Tipo do PV (Físico ou Lógico)</param>
        public void ConsultarRelatorioRecargaCelular(
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
            out StatusRetorno statusRelatorio)
        {
            //Variáveis auxiliares
            StatusRetorno stsTotalizador = null, stsRelatorio = null;
            Int32 qtdTotalRegistros = quantidadeTotalRegistros;

            //Preparação da Func para consulta dos totalizadores
            Func<RecargaCelularTotalizador> funcaoTotalizador = () =>
            {
                if (tipoPv == RecargaCelularTipoPv.PvFisico)
                {
                    return ConsultarRecargaCelularPvFisicoTotalizadores(
                        guidPesquisaTotalizador,
                        dataInicial,
                        dataFinal,
                        pvs,
                        out stsTotalizador);
                }
                else if (tipoPv == RecargaCelularTipoPv.PvLogico)
                {
                    return ConsultarRecargaCelularPvLogicoTotalizadores(
                       guidPesquisaTotalizador,
                       dataInicial,
                       dataFinal,
                       pvs,
                       out stsTotalizador);
                }
                else
                {
                    qtdTotalRegistros = 0;
                    return new RecargaCelularTotalizador();
                }
            };

            //Preparação da Func para consulta dos registros
            Func<List<RecargaCelular>> funcaoRegistros = () =>
            {
                var registrosRecarga = default(List<RecargaCelular>);

                if (tipoPv == RecargaCelularTipoPv.PvFisico)
                {
                    List<RecargaCelularPvFisico> registrosPvFisico = ConsultarRecargaCelularPvFisico(
                        guidPesquisaRelatorio,
                        registroInicial,
                        quantidadeRegistros,
                        ref qtdTotalRegistros,
                        dataInicial,
                        dataFinal,
                        pvs,
                        out stsRelatorio);

                    if (registrosPvFisico != null)
                        registrosRecarga = registrosPvFisico.Cast<RecargaCelular>().ToList();
                }
                else if (tipoPv == RecargaCelularTipoPv.PvLogico)
                {
                    List<RecargaCelularPvLogico> registrosPvLogico = ConsultarRecargaCelularPvLogico(
                        guidPesquisaRelatorio,
                        registroInicial,
                        quantidadeRegistros,
                        ref qtdTotalRegistros,
                        dataInicial,
                        dataFinal,
                        pvs,
                        out stsRelatorio);

                    if (registrosPvLogico != null)
                        registrosRecarga = registrosPvLogico.Cast<RecargaCelular>().ToList();
                }
                else
                {
                    qtdTotalRegistros = 0;
                    registrosRecarga = new List<RecargaCelular>();
                }

                return registrosRecarga;
            };

            //Consulta multi-thread dos totalizadores e registros do relatório
            var retorno = base.ConsultaMultiThread(funcaoTotalizador, funcaoRegistros);

            //Atribuição de dados de retorno
            registros = retorno.Item2;
            totalizador = retorno.Item1;
            statusRelatorio = stsRelatorio;
            statusTotalizador = stsTotalizador;
            quantidadeTotalRegistros = qtdTotalRegistros;
        }

        #endregion [ RELATÓRIOS - Consultas Multi-Thread (Totalizadores e Registros) ]

        #region [ Vendas - Crédito - Totalizadores - WACA1310 / WA1310 / ISHA ]

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
        public CreditoTotalizador ConsultarCreditoTotalizadores(
            Guid guidPesquisa,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Vendas - Crédito - Totalizadores - WACA1310"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigoBandeira, dataInicial, dataFinal, pvs });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.CreditoTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarCreditoTotalizadores(
                            codigoBandeira,
                            pvs,
                            dataInicial,
                            dataFinal,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Conversão de DTO para modelo de serviço
                    var dados = CreditoTotalizador.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region [ Vendas - Crédito - Registros - WACA1311 / WA1311 / ISHB ]

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
        public List<Credito> ConsultarCredito(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Vendas - Crédito - WACA1311"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    Guid guid = Utils.GerarGuid(idCache.ToString());

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Credito>(Cache.Extrato,
                        guid, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarCredito(
                            codigoBandeira,
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            quantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guid, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Credito>(Cache.Extrato,
                        guid, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    var dados = Credito.FromDTO(dadosCache);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region [ Vendas - Débito - Totalizadores - WACA1312 / WA1312 / ISHC ]

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
        /// <param name="tipoVenda">Tipo de Venda</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Totalizadores do Relatório de Vendas - Débito</returns>
        public DebitoTotalizador ConsultarDebitoTotalizadores(
            Guid guidPesquisa,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            Modalidade tipoVenda,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas - Débito - Totalizadores - WACA1312"))
            {
                log.GravarLog(EventoLog.InicioServico,
                    new { guidPesquisa, codigoBandeira, dataInicial, dataFinal, tipoVenda, pvs });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.DebitoTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarDebitoTotalizadores(
                            codigoBandeira,
                            pvs,
                            dataInicial,
                            dataFinal,
                            (DTO.Modalidade)tipoVenda,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Conversão de DTO para modelo de serviço
                    var dados = DebitoTotalizador.FromDTO(dadosConsulta);

                    log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #region [ Vendas - Débito - Registros - WACA1313 / WA1313 / ISHD ]

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
        /// <param name="tipoVenda">Tipo de Venda</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="status">Saída: código de retorno e mensagem da consulta realizada</param>
        /// <returns>Registros do Relatório de Vendas - Débito.</returns>
        public List<Debito> ConsultarDebito(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DebitoTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            Modalidade tipoVenda,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas - Débito - WACA1313"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    tipoVenda,
                    pvs
                });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(tipoRegistro);
                    idCache.Append(tipoVenda);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    Guid guid = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Debito>(Cache.Extrato,
                        guid, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarDebito(
                            codigoBandeira,
                            pvs,
                            dataInicial,
                            dataFinal,
                            (DTO.Modalidade)tipoVenda,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            quantidadeTotalRegistros = 0;
                            log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta, quantidadeTotalRegistros });
                            return null;
                        }

                        //Filtra de acordo com o tipo de registro
                        if (tipoRegistro == DebitoTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.DebitoDT).ToList();
                        else if (tipoRegistro == DebitoTipoRegistro.AjusteComValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.DebitoA1).ToList();
                        else if (tipoRegistro == DebitoTipoRegistro.AjusteSemValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.DebitoA2).ToList();

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guid, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Debito>(Cache.Extrato,
                        guid, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    var dados = Debito.FromDTO(dadosCache);

                    log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #region [ Vendas - Construcard - Totalizadores - WACA1314 / WA1314 / ISHE ]

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
        public ConstrucardTotalizador ConsultarConstrucardTotalizadores(
            Guid guidPesquisa,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Vendas - Construcard - Totalizadores - WACA1314"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, codigoBandeira, dataInicial, dataFinal, pvs });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = idCache.ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    var dadosConsulta = CacheAdmin.Recuperar<DTO.ConstrucardTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarConstrucardTotalizadores(
                            codigoBandeira,
                            pvs,
                            dataInicial,
                            dataFinal,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Conversão de DTO para modelo de serviço
                    var dados = ConstrucardTotalizador.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region [ Vendas - Construcard - Registros - WACA1315 / WA1315 / ISHF ]

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
        public List<Construcard> ConsultarConstrucard(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            ConstrucardTipoRegistro tipoRegistro,
            Int32 codigoBandeira,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Vendas - Construcard - WACA1315"))
            {
                Log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    tipoRegistro,
                    codigoBandeira,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(codigoBandeira);
                    idCache.Append(tipoRegistro);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    Guid guid = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.Construcard>(Cache.Extrato,
                        guid, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        var dadosConsulta = BLL.Instancia.ConsultarConstrucard(
                            codigoBandeira,
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            quantidadeTotalRegistros = 0;
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta, quantidadeTotalRegistros });
                            return null;
                        }

                        //Filtra de acordo com o tipo de registro
                        if (tipoRegistro == ConstrucardTipoRegistro.Detalhe)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.ConstrucardDT).ToList();
                        else if (tipoRegistro == ConstrucardTipoRegistro.AjusteComValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.ConstrucardA1).ToList();
                        else if (tipoRegistro == ConstrucardTipoRegistro.AjusteSemValor)
                            dadosConsulta = dadosConsulta.Where(registro => registro is DTO.ConstrucardA2).ToList();

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guid, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<DTO.Construcard>(Cache.Extrato,
                        guid, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    var dados = Construcard.FromDTO(dadosCache);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region [ Recarga de Celular - PV Físico - Totalizadores - BKWA2610 / WAC261 / WAAF ]

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
        public RecargaCelularTotalizador ConsultarRecargaCelularPvFisicoTotalizadores(
            Guid guidPesquisa,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas - Recarga de Celular PV Físico - Totalizadores - BKWA2610"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, dataInicial, dataFinal, pvs });

                try
                {
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = Utils.GerarGuid(idCache.ToString()).ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    DTO.RecargaCelularTotalizador dadosConsulta =
                        CacheAdmin.Recuperar<DTO.RecargaCelularTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarRecargaCelularPvFisicoTotalizadores(
                            pvs,
                            dataInicial,
                            dataFinal,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Conversão de DTO para modelo de serviço
                    var dados = RecargaCelularTotalizador.FromDTO(dadosConsulta);

                    log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #region [ Recarga de Celular - PV Físico - Registros - BKWA2620 / WAC262 / WAAG ]

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
        public List<RecargaCelularPvFisico> ConsultarRecargaCelularPvFisico(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas - Recarga de Celular de PV Físico - BKWA2620"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));

                    Guid guid = Utils.GerarGuid(idCache.ToString());

                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.RecargaCelularPvFisico>(Cache.Extrato,
                        guid, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        List<DTO.RecargaCelularPvFisico> dadosConsulta = BLL.Instancia.ConsultarRecargaCelularPvFisico(
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            quantidadeTotalRegistros = 0;
                            log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guid, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    List<DTO.RecargaCelularPvFisico> dadosCache =
                        CacheAdmin.RecuperarPesquisa<DTO.RecargaCelularPvFisico>(Cache.Extrato,
                        guid, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    List<RecargaCelularPvFisico> dados = RecargaCelular.FromDTO(dadosCache);

                    log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #region [ Recarga de Celular - PV Lógico - Totalizadores - BKWA2630 / WAC263 / WAAH ]

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
        public RecargaCelularTotalizador ConsultarRecargaCelularPvLogicoTotalizadores(
            Guid guidPesquisa,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas - Recarga de Celular PV Lógico - Totalizadores - BKWA2630"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, dataInicial, dataFinal, pvs });

                try
                {
                    //Variável de cache.
                    StringBuilder idCache = new StringBuilder();
                    idCache.Append(CACHEVENDAS);
                    idCache.Append(System.Reflection.MethodBase.GetCurrentMethod().Name);
                    idCache.Append(dataInicial.ToString("yyyy-MM-dd"));
                    idCache.Append(dataFinal.ToString("yyyy-MM-dd"));
                    idCache.Append(String.Join(",", pvs));
                    
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    String idPesquisa = Utils.GerarGuid(idCache.ToString()).ToString();
                    StatusRetornoDTO statusDTO;

                    //Retorna os dados da pesquisa do cache
                    DTO.RecargaCelularTotalizador dadosConsulta =
                        CacheAdmin.Recuperar<DTO.RecargaCelularTotalizador>(Cache.Extrato, idPesquisa);

                    //Se dados não existem, efetua consulta, armazenando resultado no cache
                    if (dadosConsulta == null)
                    {
                        //Executa pesquisa
                        dadosConsulta = BLL.Instancia.ConsultarRecargaCelularPvLogicoTotalizadores(
                            pvs,
                            dataInicial,
                            dataFinal,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                            return null;
                        }

                        //Adiciona os dados retornados da consulta em cache
                        CacheAdmin.Adicionar(Cache.Extrato, idPesquisa, dadosConsulta);
                    }

                    //Gera status fake, pois se chegou até aqui, consulta foi realizada com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Conversão de DTO para modelo de serviço
                    var dados = RecargaCelularTotalizador.FromDTO(dadosConsulta);

                    log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #region [ Recarga de Celular - PV Lógico - Registros - BKWA2640 / WAC264 / WAAI ]

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
        public List<RecargaCelularPvLogico> ConsultarRecargaCelularPvLogico(
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicial,
            DateTime dataFinal,
            List<Int32> pvs,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Vendas - Recarga de Celular de PV Lógico - BKWA2640"))
            {
                log.GravarLog(EventoLog.InicioServico, new
                {
                    guidPesquisa,
                    registroInicial,
                    quantidadeRegistros,
                    quantidadeTotalRegistros,
                    dataInicial,
                    dataFinal,
                    pvs
                });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa e verificação de cache
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = default(Boolean);
                    var statusDTO = default(StatusRetornoDTO);

                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<DTO.RecargaCelularPvLogico>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Execução da pesquisa mainframe
                        List<DTO.RecargaCelularPvLogico> dadosConsulta = BLL.Instancia.ConsultarRecargaCelularPvLogico(
                            pvs,
                            dataInicial,
                            dataFinal,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        //Em caso de código de retorno != 0, sai do método, sem sucesso
                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            quantidadeTotalRegistros = 0;
                            log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta, quantidadeTotalRegistros });
                            return null;
                        }

                        //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                        CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, dadosConsulta, indicadorRechamada, rechamada);
                    }

                    //Retorna o intervalo específico de registros da consulta
                    List<DTO.RecargaCelularPvLogico> dadosCache =
                        CacheAdmin.RecuperarPesquisa<DTO.RecargaCelularPvLogico>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);

                    //Status "fake": se está em cache, significa que as consultas foram realizadas com sucesso
                    status = new StatusRetorno(0, String.Empty, FONTE);

                    //Converte DTO para modelo de serviço
                    List<RecargaCelularPvLogico> dados = RecargaCelular.FromDTO(dadosCache);

                    log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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
