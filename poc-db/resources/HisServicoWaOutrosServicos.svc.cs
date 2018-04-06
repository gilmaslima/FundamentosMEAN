/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.ServiceModel;
using AutoMapper;
using Redecard.PN.Comum;
using Redecard.PN.OutrosServicos.Servicos.Corban;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Serviço de acesso aos programas mainframe WA para o módulo Outros Serviços.
    /// </summary>
    public class HisServicoWaOutrosServicos : ServicoBase, IHisServicoWaOutrosServicos
    {
        /// <summary>
        /// <para>Consulta de Totalizador de transações Corban</para>
        /// <para>Book: BKWA2650; Programa: WAC265; TRAN-ID: WAAD</para>
        /// </summary>
        /// <param name="codigoRetorno">
        /// <para>Código de retorno do programa</para>
        /// </param>
        /// <param name="quantidadeTotal">Quantidade Total de Transações</param>
        /// <param name="bandeirasTransacao">Totalizador de bandeiras</param>
        /// <param name="dataInicio">Data de início para filtragem</param>
        /// <param name="dataFinal">Data de fim para filtragem</param>
        /// <param name="tipoConta">Tipo de Contas para filtragem</param>
        /// <param name="formaPagamento">Forma de Pagamento para filtragem</param>
        /// <param name="statusCorban">Status da transação para filtragem</param>
        /// <param name="pvs">Lista de PVs para filtragem</param>
        /// <param name="codigoServico">Código do serviço</param>
        /// <returns>Totalizador Transações Corban</returns>
        public TransacaoCorban ConsultarTotalizadorTransacoes(
            out Int16 codigoRetorno,
            out Int32 quantidadeTotal,
            out List<BandeiraTransacao> bandeirasTransacao,
            DateTime dataInicio,
            DateTime dataFinal,
            TipoConta tipoConta,
            FormaPagemento formaPagamento,
            StatusCorban statusCorban,
            Decimal codigoServico,
            Int32[] pvs)
        {
            using (Logger log = Logger.IniciarLog("Consulta de Totalizador de transações Corban. BKWA2650 / WAC265 / WAAD"))
            {
                log.GravarLog(EventoLog.InicioServico, new { dataInicio, dataFinal, tipoConta, formaPagamento, statusCorban, pvs });

                //Declaração de variável de retorno
                var retorno = default(TransacaoCorban);

                try
                {
                    //Instanciação da classe de negócio
                    var negocio = new Negocio.Corban();
                    
                    //Mapeamento entre classes Modelo de serviço e negócio
                    Mapper.CreateMap<Modelo.Corban.TipoConta, Corban.TipoConta>();
                    Mapper.CreateMap<Modelo.Corban.FormaPagemento, Corban.FormaPagemento>();
                    Mapper.CreateMap<Modelo.Corban.StatusCorban, Corban.StatusCorban>();
                    Mapper.CreateMap<Modelo.TransacaoCorban, TransacaoCorban>();
                    Mapper.CreateMap<Modelo.BandeiraTransacao, BandeiraTransacao>();

                    var bandeirasTrasacaoModelo = new List<Modelo.BandeiraTransacao>();
                    Modelo.Corban.TipoConta tipoContaModelo = Mapper.Map<Modelo.Corban.TipoConta>(tipoConta);
                    Modelo.Corban.FormaPagemento formaPagamentoModelo = Mapper.Map<Modelo.Corban.FormaPagemento>(formaPagamento);
                    Modelo.Corban.StatusCorban statusCorbanModelo = Mapper.Map<Modelo.Corban.StatusCorban>(statusCorban);

                    Modelo.TransacaoCorban retornoModelo = negocio.ConsultarTotalizadorTransacoes(
                        out codigoRetorno,
                        out quantidadeTotal,
                        out bandeirasTrasacaoModelo,
                        dataInicio,
                        dataFinal,
                        tipoContaModelo,
                        formaPagamentoModelo,
                        statusCorbanModelo,
                        codigoServico,
                        pvs);

                    bandeirasTransacao = Mapper.Map<List<BandeiraTransacao>>(bandeirasTrasacaoModelo);
                    retorno = Mapper.Map<TransacaoCorban>(retornoModelo);

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

                log.GravarLog(EventoLog.RetornoServico, new { retorno, codigoRetorno });

                return retorno;
            }
        }

        /// <summary>
        /// <para>Consulta as transações Corban</para>
        /// <para>Book: BKWA2660; Programa: WAC266; TRAN-ID: WAAE</para>
        /// </summary>
        /// <param name="codigoRetorno">
        /// <para>Código de retorno do programa</para>
        /// </param>
        /// <param name="guidPesquisa"></param>
        /// <param name="registroInicial"></param>
        /// <param name="quantidadeRegistros"></param>
        /// <param name="quantidadeTotalRegistros"></param>
        /// <param name="dataInicio">Data de início para filtragem</param>
        /// <param name="dataFinal">Data de fim para filtragem</param>
        /// <param name="tipoConta">Tipo de Contas para filtragem</param>
        /// <param name="formaPagamento">Forma de Pagamento para filtragem</param>
        /// <param name="statusCorban">Status da transação para filtragem</param>
        /// <param name="pvs">Lista de PVs para filtragem</param>
        /// <param name="codigoServico">Código do serviço</param>
        /// <returns>Transações Corban</returns>
        public List<TransacaoCorban> ConsultarTransacoes(
            out Int16 codigoRetorno,
            Guid guidPesquisa,
            Int32 registroInicial,
            Int32 quantidadeRegistros,
            ref Int32 quantidadeTotalRegistros,
            DateTime dataInicio,
            DateTime dataFinal,
            TipoConta tipoConta,
            FormaPagemento formaPagamento,
            StatusCorban statusCorban,
            Decimal codigoServico,
            Int32[] pvs)
        {
            using (Logger log = Logger.IniciarLog("Consulta as transações Corban. BKWA2660 / WAC266 / WAAE"))
            {
                log.GravarLog(EventoLog.InicioServico, new { guidPesquisa, registroInicial, quantidadeRegistros,
                    quantidadeTotalRegistros, dataInicio, dataFinal, tipoConta, formaPagamento, statusCorban, pvs,
                    codigoServico });

                //Declaração de variável de retorno
                var retorno = default(List<TransacaoCorban>);
                var retornoModelo = default(List<Modelo.TransacaoCorban>);

                var rechamada = new Dictionary<String, Object>();
                Boolean indicadorRechamada = default(Boolean);

                codigoRetorno = 0;

                try
                {
                    //Enquanto for necessário executar pesquisa
                    while (CacheAdmin.DeveExecutarPesquisa<Modelo.TransacaoCorban>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeTotalRegistros, out rechamada))
                    {
                        //Instanciação da classe de negócio
                        var negocio = new Negocio.Corban();

                        //Mapeamento entre classes Modelo de serviço e negócio
                        Mapper.CreateMap<Modelo.Corban.TipoConta, TipoConta>();
                        Mapper.CreateMap<Modelo.Corban.FormaPagemento, FormaPagemento>();
                        Mapper.CreateMap<Modelo.Corban.StatusCorban, StatusCorban>();
                        Mapper.CreateMap<Modelo.TransacaoCorban, TransacaoCorban>();

                        var bandeirasTrasacaoModelo = new List<Modelo.BandeiraTransacao>();
                        Modelo.Corban.TipoConta tipoContaModelo = Mapper.Map<Modelo.Corban.TipoConta>(tipoConta);
                        Modelo.Corban.FormaPagemento formaPagamentoModelo = Mapper.Map<Modelo.Corban.FormaPagemento>(formaPagamento);
                        Modelo.Corban.StatusCorban statusCorbanModelo = Mapper.Map<Modelo.Corban.StatusCorban>(statusCorban);

                        retornoModelo = negocio.ConsultarTransacoes(
                            out codigoRetorno,
                            dataInicio,
                            dataFinal,
                            tipoContaModelo,
                            formaPagamentoModelo,
                            statusCorbanModelo,
                            pvs,
                            codigoServico,
                            ref rechamada,
                            out indicadorRechamada);

                        if (retornoModelo == null || codigoRetorno != 0)
                        {
                            quantidadeTotalRegistros = 0;
                            log.GravarLog(EventoLog.FimServico, new { codigoRetorno, retornoModelo, quantidadeTotalRegistros });

                            return null;
                        }
                        else
                        {
                            log.GravarMensagem("Gravando no cache a busca", new { 
                                retornoModelo, guidPesquisa, indicadorRechamada, rechamada });

                            //Atualiza os dados da consulta e complementa com os dados (já ou não existentes) do cache
                            CacheAdmin.AtualizarPesquisa(Cache.Extrato, guidPesquisa, retornoModelo, indicadorRechamada, rechamada);
                        }
                    }

                    //Retorna o intervalo específico de registros da consulta
                    var dadosCache = CacheAdmin.RecuperarPesquisa<Modelo.TransacaoCorban>(Cache.Extrato,
                        guidPesquisa, registroInicial, quantidadeRegistros, out quantidadeTotalRegistros);
     
                    retorno = Mapper.Map<List<TransacaoCorban>>(dadosCache);

                    log.GravarLog(EventoLog.FimServico, new { retorno, codigoRetorno, quantidadeTotalRegistros });

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
                
                return retorno;
            }
        }
    }
}