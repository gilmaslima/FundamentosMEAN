/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes.WAExtratoHomePage;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Modelo.HomePage;
using TR = Redecard.PN.Extrato.Agentes.Tradutores.HomePageTR;

namespace Redecard.PN.Extrato.Agentes
{
    /// <summary>
    /// Classe Agentes utilizado pela HomePage.
    /// </summary>
    public class HomePageAG : AgentesBase
    {
        /// <summary>
        /// Instância singleton
        /// </summary>
        private static HomePageAG instancia;

        /// <summary>
        /// Instância singleton
        /// </summary>
        public static HomePageAG Instancia { get { return instancia ?? (instancia = new HomePageAG()); } }

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
        public VendasCredito ConsultarVendasCredito(
            Int16 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Vendas Crédito - BKWA2470"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    var nomePrograma = "WAC247";
                    var dataInicio = dataInicial.ToString("yyyyMMdd").ToInt32();
                    var dataFim = dataFinal.ToString("yyyyMMdd").ToInt32();                    
                    var codigoErro = default(Int16);
                    var mensagemErro = default(String);
                    var filler = default(String);
                    var quantidadeBandeiras = default(Int16);
                    var descricaoEstabelecimento = default(String);
                    var totalApresentado = default(Decimal);
                    var totalLiquido = default(Decimal);
                    var totalDesconto = default(Decimal);
                    var totalTransacoes = default(Decimal);
                    var registros = default(List<WAC247S_OCOR>);
                    var fillerPvs = TR.ConsultarVendasCreditoEntrada(pvs);

                    //Consulta serviço mainframe
                    using (var ctx = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new {
                            nomePrograma, dataInicio, dataFim, pvs.Count, fillerPvs, codigoBandeira });
                        
#if !DEBUG
                        ctx.Cliente.ConsultarVendasCredito(
                            out codigoErro,
                            out mensagemErro,
                            out quantidadeBandeiras,
                            out descricaoEstabelecimento,
                            out totalApresentado,
                            out totalLiquido,
                            out totalDesconto,
                            out totalTransacoes,
                            out registros,
                            out filler,
                            nomePrograma,
                            dataInicio,
                            dataFim,
                            pvs.Count,
                            fillerPvs,
                            codigoBandeira);
#else
                        var r = new Random();
                        quantidadeBandeiras = (Int16) r.Next(5);
                        descricaoEstabelecimento = String.Empty;
                        totalApresentado = (Decimal) r.Next(Int16.MaxValue) / 100;
                        totalLiquido = (Decimal)r.Next(Int16.MaxValue) / 100;
                        totalDesconto = (Decimal)r.Next(Int16.MaxValue) / 100;
                        totalTransacoes = (Decimal)r.Next(Int16.MaxValue);
                        registros = new List<WAC247S_OCOR>();
                        for (Int32 iRegistro = 0; iRegistro < quantidadeBandeiras; iRegistro++)
                        {
                            registros.Add(new WAC247S_OCOR
                            {
                                WAC247S_DS_BNDR = new[] { "Visa", "Maestro", "Mastercard", "HiperCard" }[iRegistro],
                                WAC247S_CD_BNDR = (Int16) (iRegistro + 1),
                                WAC247S_QTD_TRAN = (Decimal)r.Next(1000),
                                WAC247S_VL_APRES = (Decimal)r.Next(Int16.MaxValue) / 100,
                                WAC247S_VL_DESC = (Decimal)r.Next(Int16.MaxValue) / 100,
                                WAC247S_VL_LIQ = (Decimal)r.Next(Int16.MaxValue) / 100
                            });
                        }
#endif

                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoErro, mensagemErro, quantidadeBandeiras,
                            descricaoEstabelecimento, totalApresentado, totalLiquido, totalDesconto,
                            totalTransacoes, registros, filler });
                    }

                    status = new StatusRetornoDTO(codigoErro, mensagemErro, FONTE_METODO);
                    
                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    //Converte saídas para modelo de negócio
                    return TR.ConsultarVendasCreditoSaida(totalApresentado, totalLiquido, 
                        totalDesconto, totalTransacoes, registros, quantidadeBandeiras);
                }                                         
                catch (Exception ex)                      
                {                                         
                    Log.GravarErro(ex);                   
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
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
        public VendasDebito ConsultarVendasDebito(
            Int16 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Vendas Débito - BKWA2480"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    var nomePrograma = "WAC248";
                    var dataInicio = dataInicial.ToString("yyyyMMdd").ToInt32();
                    var dataFim = dataFinal.ToString("yyyyMMdd").ToInt32();                    
                    var codigoErro = default(Int16);
                    var mensagemErro = default(String);
                    var filler = default(String);
                    var quantidadeBandeiras = default(Int16);
                    var descricaoEstabelecimento = default(String);
                    var totalApresentado = default(Decimal);
                    var totalLiquido = default(Decimal);
                    var totalDesconto = default(Decimal);
                    var totalTransacoes = default(Decimal);
                    var registros = default(List<WAC248S_OCOR>);
                    var fillerPvs = TR.ConsultarVendasDebitoEntrada(pvs);

                    //Consulta serviço mainframe
                    using (var ctx = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new {
                            nomePrograma, dataInicio, dataFim, pvs.Count, fillerPvs, codigoBandeira });
                        
#if !DEBUG
                        ctx.Cliente.ConsultarVendasDebito(
                            out codigoErro,
                            out mensagemErro,
                            out quantidadeBandeiras,
                            out descricaoEstabelecimento,
                            out totalApresentado,
                            out totalLiquido,
                            out totalDesconto,
                            out totalTransacoes,
                            out registros,
                            out filler,
                            nomePrograma,
                            dataInicio,
                            dataFim,
                            pvs.Count,
                            fillerPvs,
                            codigoBandeira);
#else
                        var r = new Random();
                        quantidadeBandeiras = (Int16)r.Next(5);
                        descricaoEstabelecimento = String.Empty;
                        totalApresentado = (Decimal)r.Next(Int16.MaxValue) / 100;
                        totalLiquido = (Decimal)r.Next(Int16.MaxValue) / 100;
                        totalDesconto = (Decimal)r.Next(Int16.MaxValue) / 100;
                        totalTransacoes = (Decimal)r.Next(Int16.MaxValue);
                        registros = new List<WAC248S_OCOR>();
                        for (Int32 iRegistro = 0; iRegistro < quantidadeBandeiras; iRegistro++)
                        {
                            registros.Add(new WAC248S_OCOR
                            {                           
                                WAC248S_DS_BNDR = new [] { "Diners", "Coopercred", "Avista", "Sicredi" }[iRegistro],
                                WAC248S_CD_BNDR = (Int16)(iRegistro + 1),
                                WAC248S_QTD_TRAN = (Decimal)r.Next(Int16.MaxValue),
                                WAC248S_VL_APRES = (Decimal)r.Next(Int16.MaxValue) / 100,
                                WAC248S_VL_DESC = (Decimal)r.Next(Int16.MaxValue) / 100,
                                WAC248S_VL_LIQ = (Decimal)r.Next(Int16.MaxValue) / 100
                            });
                        }
#endif

                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoErro, mensagemErro, quantidadeBandeiras,
                            descricaoEstabelecimento, totalApresentado, totalLiquido, totalDesconto,
                            totalTransacoes, registros, filler });
                    }

                    status = new StatusRetornoDTO(codigoErro, mensagemErro, FONTE_METODO);
                    
                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    //Converte saídas para modelo de negócio
                    return TR.ConsultarVendasDebitoSaida(totalApresentado, totalLiquido, 
                        totalDesconto, totalTransacoes, registros, quantidadeBandeiras);
                }                                         
                catch (Exception ex)                      
                {                                         
                    Log.GravarErro(ex);                   
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
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
        public LancamentosFuturos ConsultarLancamentosFuturos(
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Lançamentos Futuros - BKWA2490"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String nomePrograma = "WAC249";
                    Int32 dataInicio = dataInicial.ToString("yyyyMMdd").ToInt32();
                    Int32 dataFim = dataFinal.ToString("yyyyMMdd").ToInt32();
                    String areaFixa = TR.ConsultarLancamentosFuturosEntrada(pvs, codigoBandeira);
                    var codigoRetorno = default(Int16);
                    var codigoErro = default(Int16);
                    var mensagemErro = default(String);

                    //Consulta serviço mainframe
                    using (var ctx = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { 
                            nomePrograma, dataInicio, dataFim, areaFixa });

#if !DEBUG
                        ctx.Cliente.ConsultarLancamentosFuturos(
                            nomePrograma,
                            dataInicio,
                            dataFim,
                            ref areaFixa);
#else
                        var r = new Random();
                        Int32 qtdRegistros = r.Next(6);
                        codigoRetorno = 0;
                        codigoErro = 0;
                        mensagemErro = "PESQUISA REALIZADA COM SUCESSO";
                        areaFixa =
                            "00" +
                            "000" +
                            "                                                                      " +
                            qtdRegistros.ToString("D7") +
                            r.Next(1000000).ToString("D17") +
                            r.Next(1000000).ToString("D17");

                        for (Int32 iRegistro = 0; iRegistro < qtdRegistros; iRegistro++)
                        {
                            areaFixa += DateTime.Today.AddDays(iRegistro).ToString("yyyyMMdd");
                            areaFixa += r.Next(Int16.MaxValue).ToString("D17");
                            areaFixa += r.Next(Int16.MaxValue).ToString("D17");
                        }
#endif

                        Log.GravarLog(EventoLog.RetornoHIS, new { 
                            nomePrograma, dataInicio, dataFim, areaFixa });
                    }

                    //Converte saídas para modelo de negócio
                    LancamentosFuturos retorno = TR.ConsultarLancamentosFuturosSaida(
                        areaFixa, out codigoRetorno, out codigoErro, out mensagemErro);

                    status = new StatusRetornoDTO(codigoErro, mensagemErro, FONTE_METODO);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return retorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
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
        public ValoresPagos ConsultarValoresPagos(
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Valores Pagos - BKWA2500"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String nomePrograma = "WAC250";
                    Int32 dataInicio = dataInicial.ToString("yyyyMMdd").ToInt32();
                    Int32 dataFim = dataFinal.ToString("yyyyMMdd").ToInt32();
                    String areaFixa = TR.ConsultarValoresPagosEntrada(pvs, codigoBandeira);
                    var codigoRetorno = default(Int16);
                    var codigoErro = default(Int16);
                    var mensagemErro = default(String);

                    //Consulta serviço mainframe
                    using (var ctx = new ContextoWCF<COMTIWAClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, 
                            new { nomePrograma, dataInicio, dataFim, areaFixa });

#if !DEBUG
                        ctx.Cliente.ConsultarLancamentosFuturos(
                            nomePrograma,
                            dataInicio,
                            dataFim,
                            ref areaFixa);
#else
                        var r = new Random();
                        Int32 qtdRegistros = r.Next(6);
                        codigoRetorno = 0;
                        codigoErro = 0;
                        mensagemErro = "PESQUISA REALIZADA COM SUCESSO";
                        areaFixa =
                            "00" +
                            "000" +
                            "                                                                      " +
                            qtdRegistros.ToString("D7") +
                            r.Next(1000000).ToString("D17") +
                            r.Next(1000000).ToString("D17");

                        for (Int32 iRegistro = 0; iRegistro < qtdRegistros; iRegistro++)
                        {
                            areaFixa += DateTime.Today.AddDays(iRegistro).ToString("yyyyMMdd");
                            areaFixa += r.Next(Int16.MaxValue).ToString("D17");
                            areaFixa += r.Next(Int16.MaxValue).ToString("D17");
                        }
#endif

                        Log.GravarLog(EventoLog.RetornoHIS, 
                            new { nomePrograma, dataInicio, dataFim, areaFixa });
                    }

                    //Converte saídas para modelo de negócio
                    ValoresPagos retorno = TR.ConsultarValoresPagosSaida(
                        areaFixa, out codigoRetorno, out codigoErro, out mensagemErro);

                    status = new StatusRetornoDTO(codigoErro, mensagemErro, FONTE_METODO);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return retorno;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
    }
}