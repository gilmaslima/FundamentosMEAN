/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;
using BLL = Redecard.PN.Extrato.Negocio.HomePageBLL;
using DTO = Redecard.PN.Extrato.Modelo.HomePage;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço utilizado pela HomePage
    /// </summary>
    /// <remarks>
    /// <para>Books utilizados:</para>
    /// <para>- Book BKWA2470 / Programa WAC247 / Transação WAG4</para>
    /// <para>- Book BKWA2480 / Programa WAC248 / Transação WAG5</para>
    /// <para>- Book BKWA2490 / Programa WAC249 / Transação WAG6</para>
    /// <para>- Book BKWA2500 / Programa WAC250 / Transação WAG7</para>
    /// </remarks>
    public class HISServicoWA_Extrato_HomePage : ServicoBase, IHISServicoWA_Extrato_HomePage
    {
        /// <summary>
        /// Consulta de Vendas a Crédito utilizado na HomePage, consolidados por Bandeira
        /// </summary>
        /// <remarks>
        /// <para>Books utilizados:</para>
        /// <para>- Book BKWA2470 / Programa WAC247 / Transação WAG4</para>
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="pvs">Lista de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Vendas a Crédito</returns>
        public Modelo.HomePage.VendasCredito ConsultarVendasCredito(
            Int16 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Vendas a Crédito - BKWA2470"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, pvs, dataInicial, dataFinal });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa
                    StatusRetornoDTO statusRetornoDTO;

                    //Execução da pesquisa
                    DTO.VendasCredito dadosConsulta = BLL.Instancia.ConsultarVendasCredito(
                        codigoBandeira,
                        pvs,
                        dataInicial,
                        dataFinal,
                        out statusRetornoDTO);

                    if (statusRetornoDTO == null || statusRetornoDTO.CodigoRetorno != 0)
                    {
                        status = StatusRetorno.FromDTO(statusRetornoDTO);
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return new Modelo.HomePage.VendasCredito();
                    }

                    //Preparação dos dados de retorno
                    var dados = Modelo.HomePage.VendasCredito.FromDTO(dadosConsulta);
                    status = new StatusRetorno();

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

        /// <summary>
        /// Consulta de Vendas a Débito utilizado na HomePage, consolidados por Bandeira
        /// </summary>
        /// <remarks>
        /// <para>Books utilizados:</para>
        /// <para>- Book BKWA2480 / Programa WAC248 / Transação WAG5</para>
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="pvs">Lista de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Vendas a Débito</returns>
        public Modelo.HomePage.VendasDebito ConsultarVendasDebito(
            Int16 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Vendas a Débito - BKWA2480"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, pvs, dataInicial, dataFinal });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa
                    StatusRetornoDTO statusRetornoDTO;

                    //Execução da pesquisa
                    DTO.VendasDebito dadosConsulta = BLL.Instancia.ConsultarVendasDebito(
                        codigoBandeira,
                        pvs,
                        dataInicial,
                        dataFinal,
                        out statusRetornoDTO);

                    if (statusRetornoDTO == null || statusRetornoDTO.CodigoRetorno != 0)
                    {
                        status = StatusRetorno.FromDTO(statusRetornoDTO);
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return new Modelo.HomePage.VendasDebito();
                    }

                    //Preparação dos dados de retorno
                    var dados = Modelo.HomePage.VendasDebito.FromDTO(dadosConsulta);
                    status = new StatusRetorno();

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

        /// <summary>
        /// Consulta de Lançamentos Futuros utilizado na HomePage, consolidados por Data de Recebimento
        /// </summary>
        /// <remarks>
        /// <para>Books utilizados:</para>
        /// <para>- Book BKWA2490 / Programa WAC249 / Transação WAG6</para>
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="pvs">Lista de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Lançamentos Futuros</returns>
        public Modelo.HomePage.LancamentosFuturos ConsultarLancamentosFuturos(
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Lançamentos Futuros - BKWA2490"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, pvs, dataInicial, dataFinal });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa
                    StatusRetornoDTO statusRetornoDTO;

                    //Execução da pesquisa
                    DTO.LancamentosFuturos dadosConsulta = BLL.Instancia.ConsultarLancamentosFuturos(
                        codigoBandeira,
                        pvs,
                        dataInicial,
                        dataFinal,
                        out statusRetornoDTO);

                    if (statusRetornoDTO == null || statusRetornoDTO.CodigoRetorno != 0)
                    {
                        status = StatusRetorno.FromDTO(statusRetornoDTO);
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return new Modelo.HomePage.LancamentosFuturos();
                    }

                    //Preparação dos dados de retorno
                    var dados = Modelo.HomePage.LancamentosFuturos.FromDTO(dadosConsulta);
                    status = new StatusRetorno();

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

        /// <summary>
        /// Consulta de Valores Pagos utilizado na HomePage, consolidados por Data de Recebimento
        /// </summary>
        /// <remarks>
        /// <para>Books utilizados:</para>
        /// <para>- Book BKWA2500 / Programa WAC250 / Transação WAG7</para>
        /// </remarks>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="pvs">Lista de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="status">Status da consulta</param>
        /// <returns>Valores Pagos</returns>
        public Modelo.HomePage.ValoresPagos ConsultarValoresPagos(
           Int32 codigoBandeira,
           List<Int32> pvs,
           DateTime dataInicial,
           DateTime dataFinal,
           out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Valores Pagos - BKWA2500"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { codigoBandeira, pvs, dataInicial, dataFinal });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa
                    StatusRetornoDTO statusRetornoDTO;

                    //Execução da pesquisa
                    DTO.ValoresPagos dadosConsulta = BLL.Instancia.ConsultarValoresPagos(
                        codigoBandeira,
                        pvs,
                        dataInicial,
                        dataFinal,
                        out statusRetornoDTO);

                    if (statusRetornoDTO == null || statusRetornoDTO.CodigoRetorno != 0)
                    {
                        status = StatusRetorno.FromDTO(statusRetornoDTO);
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return new Modelo.HomePage.ValoresPagos();
                    }

                    //Preparação dos dados de retorno
                    var dados = Modelo.HomePage.ValoresPagos.FromDTO(dadosConsulta);
                    status = new StatusRetorno();

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
    }
}