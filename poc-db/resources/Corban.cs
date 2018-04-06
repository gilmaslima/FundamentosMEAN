/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.OutrosServicos.Agentes.COMTIWAOutrosServicos;
using Redecard.PN.OutrosServicos.Agentes.Tradutores;
using Redecard.PN.OutrosServicos.Modelo;
using Redecard.PN.OutrosServicos.Modelo.Corban;
using System.Linq;

namespace Redecard.PN.OutrosServicos.Agentes
{
    /// <summary>
    /// Classe de Consulta de Transações Corban
    /// </summary>
    public class Corban : AgentesBase
    {

        /// <summary>
        /// <para>Consulta de Totalizador de transações Corban</para>
        /// <para>Book: BKWA2650; Programa: WAC265; TRAN-ID: WAAD</para>
        /// </summary>
        /// <param name="codigoRetorno">
        /// <para>Código de retorno do programa</para>
        /// </param>
        /// <param name="quantidadeTotal"></param>
        /// <param name="bandeirasTransacao">Totalizador de bandeiras</param>
        /// <param name="nomePrograma">Nome do programa chamador</param>
        /// <param name="dataInicio">Data de início para filtragem</param>
        /// <param name="dataFinal">Data de fim para filtragem</param>
        /// <param name="tipoConta">Tipo de Contas para filtragem</param>
        /// <param name="formaPagamento">Forma de Pagamento para filtragem</param>
        /// <param name="statusCorban">Status da transação para filtragem</param>
        /// <param name="pvs">Lista de PVs para filtragem</param>
        /// <param name="codigoServico">Código do serviço</param>
        /// <returns></returns>
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
                log.GravarLog(EventoLog.InicioAgente, new { 
                    dataInicio, dataFinal, tipoConta, formaPagamento, statusCorban, pvs, codigoServico });

                try
                {
                    //Declaração a atribuição inicial de variáveis auxiliares
                    codigoRetorno = default(Int16);
                    quantidadeTotal = default(Int32);
                    var codigoSqlCode = default(Int16);

                    //String msgRetorno = default(String); -> Na V2, a msg de retorno foi quebrada em 3 parâmetros
                    var programaRetorno = default(String);
                    var sequenciaRetorno = default(Int16);
                    var descricaoRetorno = default(String);

                    var quantidadeContas = default(Int32);
                    Int32 quantidadePvs = pvs.Length;
                    var totalContas = default(Decimal);
                    var reserva = default(String);

                    var qtdOcorrenciasBandeiras = default(Int16);
                    var bandeiras = new B265S_BANDEIRAS[99];
                    var fillerPvs = new B265E_TAB_PDV[3000];

                    var nomePrograma = "WAC265";

                    var codTipoConta = (Int16)tipoConta;
                    var codFormaPagamento = (Int16)formaPagamento;
                    var codStatusCorban = ((Char)statusCorban).ToString();

                    //Parâmetros que serão retornados
                    TransacaoCorban totalTransacoes = null;
                    bandeirasTransacao = new List<BandeiraTransacao>();

                    Int32 dataMovimentoInicio = dataInicio.ToString("yyyyMMdd").ToInt32();
                    Int32 dataMovimentoFim = dataFinal.ToString("yyyyMMdd").ToInt32();

                    for (Int16 indicePv = 0; indicePv < pvs.Length; indicePv++)
                        fillerPvs[indicePv].B265E_NUM_PV = pvs[indicePv];

                    //Chamando serviço mainframe
                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS,  new {
                            nomePrograma, dataMovimentoInicio, dataMovimentoFim, codTipoConta, codFormaPagamento,
                            codStatusCorban, codigoServico, quantidadePvs, fillerPvs });

#if !DEBUG
                        codigoRetorno = contexto.Cliente.ConsultarTotalizadorTransacoesCorban(
                            out codigoSqlCode, 
                            out programaRetorno,
                            out sequenciaRetorno,
                            out descricaoRetorno,
                            out quantidadeContas, 
                            out totalContas,
                            out qtdOcorrenciasBandeiras,
                            out bandeiras, 
                            out reserva, 
                            nomePrograma, 
                            dataMovimentoInicio,
                            dataMovimentoFim, 
                            codTipoConta,
                            codFormaPagamento,
                            codStatusCorban,
                            codigoServico,
                            quantidadePvs, 
                            fillerPvs);
#else
                        codigoRetorno = 0;
                        quantidadeContas = 50;
                        quantidadeTotal = quantidadeContas;
                        qtdOcorrenciasBandeiras = 2;
                        totalContas = 5679446.54M;
                        reserva = new String[2862].ToString();
                        bandeiras = new B265S_BANDEIRAS[2] {
                            new B265S_BANDEIRAS { B265S_DES_BAND = "JUPITER", B265S_TOT_BAND = 54752.14M },
                            new B265S_BANDEIRAS { B265S_DES_BAND = "TERRA", B265S_TOT_BAND = 54169.14M }
                        };
#endif

                        log.GravarLog(EventoLog.RetornoHIS, new { 
                            codigoRetorno, codigoSqlCode, programaRetorno, sequenciaRetorno, descricaoRetorno,
                            quantidadeContas, totalContas, qtdOcorrenciasBandeiras, bandeiras, reserva });
                    }

                    //00 - ok
                    //10 - programa chamador nao informado
                    //11 - estabelecimento nao informado / invalido
                    //12 - data movto pgto  inicio nao informada
                    //13 - data movto pgto  inicio invalida
                    //14 - data movto pgto  final nao informada
                    //15 - data movto pgto  final invalida
                    //16 - data movto pgto  inicial maior final
                    //17 - periodo do movimento superior a 99 dias
                    //18 - tipo de conta difernete de 00, 01, 02 e ou 03
                    //19 - forma de pgto difernete de 00, 01, 02 e ou 03
                    //20 - status de pgto difernete de 00, 13, 14 e ou 15
                    //21 - erro na chamada da qexxxx
                    //60 - nenhum argumento de pesquisa encontrado
                    //99 - erro no acesso ao db2 (vide sqlcode)
                    if (codigoRetorno == 0)
                    {
                        if (!Object.ReferenceEquals(bandeiras, null))
                        {
                            totalTransacoes = new TransacaoCorban();
                            totalTransacoes.ValorBrutoPagamento = totalContas;
                            quantidadeTotal = quantidadeContas;
                            bandeirasTransacao = bandeiras
                                .Take(qtdOcorrenciasBandeiras)
                                .Select(bandeira => new BandeiraTransacao
                                {
                                    Descricao = bandeira.B265S_DES_BAND,
                                    Valor = bandeira.B265S_TOT_BAND
                                }).ToList();
                        }
                    }

                    log.GravarLog(EventoLog.RetornoAgente, new { 
                        codigoRetorno, bandeirasTransacao, totalTransacoes, quantidadeTotal });

                    return totalTransacoes;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// <para>Consulta as transações Corban</para>
        /// <para>Book: BKWA2660; Programa: WAC266; TRAN-ID: WAAE</para>
        /// </summary>
        /// <param name="codigoRetorno">
        /// <para>Código de retorno do programa</para>
        /// </param>
        /// <param name="dataInicio">Data de início para filtragem</param>
        /// <param name="dataFinal">Data de fim para filtragem</param>
        /// <param name="tipoConta">Tipo de Contas para filtragem</param>
        /// <param name="formaPagamento">Forma de Pagamento para filtragem</param>
        /// <param name="statusCorban">Status da transação para filtragem</param>
        /// <param name="pvs">Lista de PVs para filtragem</param>
        /// <param name="indicadorRechamada"><para>'True' - Há rechamada</para><para>'False' - Não há rechamada</para></param>
        /// <param name="rechamada">Parâmetros de rechamada</param>
        /// <param name="codigoServico">Código do serviço</param>
        /// <returns>Transações Corban</returns>
        public List<TransacaoCorban> ConsultarTransacoes(            
            DateTime dataInicio,
            DateTime dataFinal,
            TipoConta tipoConta,
            FormaPagemento formaPagamento,
            StatusCorban statusCorban,
            Int32[] pvs,
            Decimal codigoServico,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out Int16 codigoRetorno)
        {
            using (Logger log = Logger.IniciarLog("Consulta as transações Corban. BKWA2660 / WAC266 / WAAE"))
            {
                log.GravarLog(EventoLog.InicioAgente, new { 
                    dataInicio, dataFinal, tipoConta, formaPagamento, statusCorban, pvs, codigoServico });

                try
                {
                    //Parâmetros que serão retornados
                    List<TransacaoCorban> transacoesCorban = new List<TransacaoCorban>();

                    //Declaração a atribuição inicial de variáveis auxiliares
                    codigoRetorno = default(Int16);
                    var codigoSqlCode = default(Int16);
                    var fillerEntrada = default(String);
                    var fillerSaida = default(String);

                    var quantidadeTransacoes = default(Int32);
                    var tipoRegistro = default(String);

                    String nomePrograma = "WAC266";

                    //Mensagem de retorno
                    var programaRetorno = default(String);
                    var sequenciaRetorno = default(Int16);
                    var descricaoRetorno = default(String);

                    Int32 dataMovimentoInicio = dataInicio.ToString("yyyyMMdd").ToInt32();
                    Int32 dataMovimentoFim = dataFinal.ToString("yyyyMMdd").ToInt32();

                    var codTipoConta = (Int16)tipoConta;
                    var codFormaPagamento = (Int16)formaPagamento;
                    var codStatusCorban = ((Char)statusCorban).ToString();

                    //Parâmetros de rechamada
                    rechamada = rechamada ?? new Dictionary<String, Object>();
                    Int32 numeroBlocoRechamada = rechamada.GetValueOrDefault<Int32>("NumeroBloco");
                    Int16 formaPagamentoRechamada = rechamada.GetValueOrDefault<Int16>("FormaPagamento");
                    Int32 dataTransacaoRechamada = rechamada.GetValueOrDefault<Int32>("DataTransacao");
                    Decimal codigoServicoRechamada = rechamada.GetValueOrDefault<Decimal>("CodigoServico");
                    String indicarRechamada = rechamada.GetValueOrDefault<String>("Indicador");
                    
                    //Dados dos PVs
                    String areaFixaRedefinicao = TradutorCorban.ConsultarTransacoesEntrada(pvs);
                    
                    //Chamando serviço mainframe
                    using (var contexto = new ContextoWCF<COMTIWAClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { 
                            nomePrograma, dataMovimentoInicio, dataMovimentoFim, codTipoConta, codFormaPagamento,
                            codStatusCorban, codigoServico, indicarRechamada, codigoServicoRechamada, dataTransacaoRechamada, 
                            formaPagamentoRechamada, numeroBlocoRechamada, fillerEntrada, fillerSaida, areaFixaRedefinicao });

#if !DEBUG
                        contexto.Cliente.ConsultaTransacoes(
                            nomePrograma,
                            dataMovimentoInicio,
                            dataMovimentoFim,
                            codTipoConta,
                            codFormaPagamento,
                            codStatusCorban,
                            codigoServico, 
                            ref indicarRechamada, 
                            ref codigoServicoRechamada, 
                            ref dataTransacaoRechamada, 
                            ref formaPagamentoRechamada, 
                            ref numeroBlocoRechamada,
                            out codigoRetorno,
                            out codigoSqlCode, 
                            out programaRetorno,
                            out sequenciaRetorno,
                            out descricaoRetorno,
                            fillerEntrada,
                            fillerSaida,
                            ref areaFixaRedefinicao);
#else
                        codigoRetorno = 0;
                        indicarRechamada = "N";

                        List<TransacaoCorban> retorno = new List<TransacaoCorban>();
            
                        tipoRegistro = (new Random().Next(2) == 1) ? "T" : "R";
                        quantidadeTransacoes = new Random().Next(180);

                        Random rndValores = new Random();

                        for (Int16 index = 0; index < quantidadeTransacoes; index++)
                        {
                            transacoesCorban.Add(new TransacaoCorban
                            {
                                NumeroEstabelecimento = rndValores.Next(Int32.MaxValue),
                                DataPagamento = DateTime.Today.AddDays(rndValores.Next(90)),
                                HoraPagamento = DateTime.Now.AddMinutes(index).AddHours(-index),
                                CodigoServico = rndValores.Next(Int16.MaxValue),
                                DescricaoTipoConta = String.Concat("Conta XXX-", rndValores.Next(Int16.MaxValue).ToString()),
                                DescricaoFormaPagamento = String.Concat("Pagamento XXX-", rndValores.Next(Int16.MaxValue).ToString()),
                                DescricaoBandeira = String.Concat("Bandeira XXX-", rndValores.Next(Int16.MaxValue).ToString()),
                                CodigoBarras = String.Concat(rndValores.Next(Int32.MaxValue), " ").PadLeft(28, 'X'),
                                NomeOperadora = String.Concat("Operadora XXX-", rndValores.Next(Int16.MaxValue).ToString()),
                                ValorBrutoPagamento = rndValores.Next(Int16.MaxValue),
                                StatusConta = String.Concat("Status XXX-", rndValores.Next(Int16.MaxValue).ToString())
                            });
                        }
#endif

                        log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, codigoSqlCode, programaRetorno,
                                sequenciaRetorno, descricaoRetorno, indicarRechamada, codigoServicoRechamada,
                                dataTransacaoRechamada, formaPagamentoRechamada, numeroBlocoRechamada, areaFixaRedefinicao });
                    }

                    rechamada["FormaPagamento"] = formaPagamentoRechamada;
                    rechamada["DataTransacao"] = dataTransacaoRechamada;
                    rechamada["CodigoServico"] = codigoServicoRechamada;
                    rechamada["NumeroBloco"] = numeroBlocoRechamada;
                    rechamada["Indicador"] = indicarRechamada;

                    indicadorRechamada = String.Compare("S", indicarRechamada, true) == 0;

                    //00 - ok
                    //10 - programa chamador nao informado
                    //11 - estabelecimento nao informado / invalido
                    //12 - data movto pgto  inicio nao informada
                    //13 - data movto pgto  inicio invalida
                    //14 - data movto pgto  final nao informada
                    //15 - data movto pgto  final invalida
                    //16 - data movto pgto  inicial maior final
                    //17 - periodo do movimento superior a 99 dias
                    //18 - tipo de conta difernete de 00, 01, 02 e ou 03
                    //19 - forma de pgto difernete de 00, 01, 02 e ou 03
                    //20 - status de pgto difernete de 00, 13, 14 e ou 15
                    //21 - erro na chamada da qexxxx
                    //60 - nenhum argumento de pesquisa encontrado
                    //99 - erro no acesso ao db2 (vide sqlcode)
                    if (codigoRetorno == 0)
                    {
#if !DEBUG
                        transacoesCorban = TradutorCorban.ConsultarTransacoesSaida(
                            areaFixaRedefinicao, 
                            out tipoRegistro, 
                            out quantidadeTransacoes);
#endif
                    }

                    log.GravarLog(EventoLog.RetornoAgente, new { codigoRetorno, transacoesCorban, 
                        tipoRegistro, quantidadeTransacoes, indicadorRechamada, codigoServicoRechamada, 
                        dataTransacaoRechamada, formaPagamentoRechamada, numeroBlocoRechamada });

                    return transacoesCorban;
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
    }
}