using System;
using System.Collections.Generic;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes.ServicoConsultaResumoVendas;
using Redecard.PN.Extrato.Agentes.Tradutores;
using Redecard.PN.Extrato.Comum.Helper;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes
{
    public class ConsultarResumoVendasAG : AgentesBase
    {             
        #region WACA617 - Resumo de vendas - Cartões de débito.
        /// <summary>
        /// WACA617 - Resumo de vendas - Cartões de débito.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarPreDatadosRetornoDTO> ConsultarPreDatados(out StatusRetornoDTO statusRetornoDTO, ConsultarPreDatadosEnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarPreDatados";
            using (Logger Log = Logger.IniciarLog("Resumo de vendas - Cartões de débito [WACA617]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });

                try
                {
                    using (RVClient cliente = new RVClient())
                    {
                        string canalChamador = "I";
                        int numeroEstabelecimento = envio.NumeroEstabelecimento;
                        int numeroResumoVenda = envio.NumeroResumoVenda;
                        string dataApresentacao = envio.DataApresentacao.ToString("ddMMyyyy");
                        string tipoResumoVenda = string.Empty;
                        string timestamp = string.Empty;
                        short tipoMovimento = 0;
                        short quantidadeOcorrencias = 0;
                        short transacaoRegistrada = 0;
                        short totalTransacoesRegistradas = 0;
                        short colunaInicial = 0;
                        string programa = string.Empty;
                        int numeroEstabelecimentoAnterior = 0;
                        int numeroResumoVendaAnterior = 0;
                        string dataApresentacaoAnterior = string.Empty;
                        string indicadorPreDatado = string.Empty;
                        string reservaDados = string.Empty;
                        short quantidadeOcorrenciasRetornadas = default(short);
                        string mensagemRetorno = default(string);
                        PreDatadoResumoVenda[] preDatados = null;

                        short codigoRetorno;
                        Log.GravarLog(EventoLog.ChamadaHIS, new { canalChamador, numeroEstabelecimento, numeroResumoVenda, dataApresentacao, tipoResumoVenda, timestamp, tipoMovimento, quantidadeOcorrencias, transacaoRegistrada, totalTransacoesRegistradas, colunaInicial, programa, numeroEstabelecimentoAnterior, numeroResumoVendaAnterior, dataApresentacaoAnterior, indicadorPreDatado, reservaDados });

                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {
                            codigoRetorno = (short)cliente.ConsultarPreDatados(canalChamador, numeroEstabelecimento, numeroResumoVenda, dataApresentacao, tipoResumoVenda, timestamp, tipoMovimento, ref quantidadeOcorrencias, ref transacaoRegistrada, ref totalTransacoesRegistradas, ref colunaInicial, ref programa, ref numeroEstabelecimentoAnterior, ref numeroResumoVendaAnterior, ref dataApresentacaoAnterior, ref indicadorPreDatado, ref reservaDados, out quantidadeOcorrenciasRetornadas, out mensagemRetorno, out preDatados);
                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;
                            quantidadeOcorrencias = 3;

                            preDatados = new PreDatadoResumoVenda[3];

                            preDatados[0].CodigoSubTransacao = 11;
                            preDatados[0].CodigoTransacao = 11;
                            preDatados[0].DataVencimento = "20010101";
                            preDatados[0].DescontoTaxaCredito = 1111111.11M;
                            preDatados[0].Descricao = "Descriçao descrita 000001";
                            preDatados[0].DescricaoBandeira = "Bandeira 001";
                            preDatados[0].NumeroPeca = 111;
                            preDatados[0].QuantidadeComprovantesVenda = 1111;
                            preDatados[0].QuantidadePecas = 111;
                            preDatados[0].ReservaDados = String.Empty;
                            preDatados[0].TipoVenda = "X";
                            preDatados[0].ValorApresentado = 11111111111.11M;
                            preDatados[0].ValorDesconto = 11111111111.11M; ;
                            preDatados[0].ValorLiquido = 11111111111.50M; ;
                            preDatados[0].ValorSaque = 1111111.11M;
                            preDatados[0].ValorTotalCpmf = 1111111.11M;
                            preDatados[0].ValorTotalIof = 1111111.11M;

                            preDatados[1].CodigoSubTransacao = 22;
                            preDatados[1].CodigoTransacao = 22;
                            preDatados[1].DataVencimento = "20020202";
                            preDatados[1].DescontoTaxaCredito = 2222222.22M;
                            preDatados[1].Descricao = "Descriçao descrita 000002";
                            preDatados[1].DescricaoBandeira = "Bandeira 002";
                            preDatados[1].NumeroPeca = 222;
                            preDatados[1].QuantidadeComprovantesVenda = 2222;
                            preDatados[1].QuantidadePecas = 222;
                            preDatados[1].ReservaDados = String.Empty;
                            preDatados[1].TipoVenda = "Y";
                            preDatados[1].ValorApresentado = 22222222222.22M;
                            preDatados[1].ValorDesconto = 22222222222.22M; ;
                            preDatados[1].ValorLiquido = 22222222222.50M; ;
                            preDatados[1].ValorSaque = 2222222.22M;
                            preDatados[1].ValorTotalCpmf = 2222222.22M;
                            preDatados[1].ValorTotalIof = 2222222.22M;


                            preDatados[2].CodigoSubTransacao = 99;
                            preDatados[2].CodigoTransacao = 99;
                            preDatados[2].DataVencimento = "20121231";
                            preDatados[2].DescontoTaxaCredito = 9999999.99M;
                            preDatados[2].Descricao = "Descriçao descrita 000003";
                            preDatados[2].DescricaoBandeira = "Bandeira 003";
                            preDatados[2].NumeroPeca = 999;
                            preDatados[2].QuantidadeComprovantesVenda = 9999;
                            preDatados[2].QuantidadePecas = 999;
                            preDatados[2].ReservaDados = String.Empty;
                            preDatados[2].TipoVenda = "Z";
                            preDatados[2].ValorApresentado = 99999999999.99M;
                            preDatados[2].ValorDesconto = 99999999999.99M; ;
                            preDatados[2].ValorLiquido = 99999999999.99M; ;
                            preDatados[2].ValorSaque = 9999999.99M;
                            preDatados[2].ValorTotalCpmf = 9999999.99M;
                            preDatados[2].ValorTotalIof = 9999999.99M;
                        }

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);
                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno, quantidadeOcorrencias, transacaoRegistrada, totalTransacoesRegistradas, colunaInicial, programa, numeroEstabelecimentoAnterior, numeroResumoVendaAnterior, dataApresentacaoAnterior, indicadorPreDatado, reservaDados, quantidadeOcorrenciasRetornadas, preDatados });

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        List<ConsultarPreDatadosRetornoDTO> result = TradutorResultadoResumoVendas.TraduzirRetornoListaConsultarPreDatados(preDatados, quantidadeOcorrencias);
                        Log.GravarLog(EventoLog.FimAgente, new { result });

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion

        #region WACA615 - Resumo de vendas - Cartões de débito - Vencimentos.
        /// <summary>
        /// WACA615 - Resumo de vendas - Cartões de débito - Vencimentos.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarVencimentosResumoVendaRetornoDTO> ConsultarVencimentosResumoVenda(out StatusRetornoDTO statusRetornoDTO, ConsultarVencimentosResumoVendaEnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarVencimentosResumoVenda";
            using (Logger Log = Logger.IniciarLog("Resumo de vendas - Cartões de débito - Vencimentos [WACA615]"))
            {


                Log.GravarLog(EventoLog.InicioAgente, new { envio });

                try
                {
                    List<ConsultarVencimentosResumoVendaRetornoDTO> result = new List<ConsultarVencimentosResumoVendaRetornoDTO>();

                    using (RVClient cliente = new RVClient())
                    {
                        string canalChamador = "I";
                        int numeroEstabelecimento = envio.NumeroEstabelecimento;
                        int numeroResumoVenda = envio.NumeroResumoVenda;
                        string dataApresentacao = envio.DataApresentacao.ToString("yyyyMMdd");
                        short tipoMovimento = 2;
                        string timestamp = string.Empty;
                        decimal valorApresentado = 0;
                        decimal valorLiquido = 0;
                        string flagContinua = string.Empty;
                        short quantidadeOcorrencias = 0;
                        short transacaoRegistrada = 0;
                        short totalTransacoesRegistradas = 0;
                        short colunaInicial = 0;
                        string programa = string.Empty;
                        string reservaDados = string.Empty;
                        string mensagemRetorno = default(string);
                        short quantidadeVencimentos = default(short);
                        VencimentoResumoVenda[] vencimentos = null;

                        do
                        {
                            short codigoRetorno;
                            Log.GravarLog(EventoLog.ChamadaHIS, new { canalChamador, numeroEstabelecimento, numeroResumoVenda, dataApresentacao, tipoMovimento, timestamp, valorApresentado, valorLiquido, flagContinua, quantidadeOcorrencias, transacaoRegistrada, totalTransacoesRegistradas, colunaInicial, programa, reservaDados });
                            if (!TesteHelper.IsAmbienteDesenvolvimento())
                            {
                                codigoRetorno = (short)cliente.ConsultarVencimentosResumoVenda(canalChamador, numeroEstabelecimento, numeroResumoVenda, dataApresentacao, tipoMovimento, ref timestamp, ref valorApresentado, ref valorLiquido, ref flagContinua, ref quantidadeOcorrencias, ref transacaoRegistrada, ref totalTransacoesRegistradas, ref colunaInicial, ref programa, ref reservaDados, out mensagemRetorno, out quantidadeVencimentos, out vencimentos);

                            }
                            else
                            {
                                codigoRetorno = 0;
                                mensagemRetorno = string.Empty;
                                quantidadeVencimentos = 3;

                                vencimentos = new VencimentoResumoVenda[3];

                                vencimentos[0].Agencia = 11111;
                                vencimentos[0].Banco = 11111;
                                vencimentos[0].Conta = "Conta no. 00001";
                                vencimentos[0].DataVencimento = "20010101";
                                vencimentos[0].Descricao = "Descriçao descrita 000001";
                                vencimentos[0].NumeroPeca = 111;
                                vencimentos[0].QuantidadeComprovanteVenda = 1111;
                                vencimentos[0].QuantidadePecas = 111;
                                vencimentos[0].ReservaDados = String.Empty;
                                vencimentos[0].ValorApresentado = 11111111111.11M;
                                vencimentos[0].ValorLiquido = 11111111111.50M;

                                vencimentos[1].Agencia = 22222;
                                vencimentos[1].Banco = 22222;
                                vencimentos[1].Conta = "Conta no. 00002";
                                vencimentos[1].DataVencimento = "20020202";
                                vencimentos[1].Descricao = "Descriçao descrita 000002";
                                vencimentos[1].NumeroPeca = 222;
                                vencimentos[1].QuantidadeComprovanteVenda = 2222;
                                vencimentos[1].QuantidadePecas = 222;
                                vencimentos[1].ReservaDados = String.Empty;
                                vencimentos[1].ValorApresentado = 22222222222.22M;
                                vencimentos[1].ValorLiquido = 22222222222.50M;

                                vencimentos[2].Agencia = 99999;
                                vencimentos[2].Banco = 99999;
                                vencimentos[2].Conta = "Conta no. 00003";
                                vencimentos[2].DataVencimento = "20121231";
                                vencimentos[2].Descricao = "Descriçao descrita 000003";
                                vencimentos[2].NumeroPeca = 999;
                                vencimentos[2].QuantidadeComprovanteVenda = 9999;
                                vencimentos[2].QuantidadePecas = 999;
                                vencimentos[2].ReservaDados = String.Empty;
                                vencimentos[2].ValorApresentado = 99999999999.99M;
                                vencimentos[2].ValorLiquido = 99999999999.99M;
                            }

                            statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);
                            Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno, timestamp, valorApresentado, valorLiquido, flagContinua, quantidadeOcorrencias, transacaoRegistrada, totalTransacoesRegistradas, colunaInicial, programa, reservaDados, quantidadeVencimentos, vencimentos });

                            if (codigoRetorno != 0)
                            {

                                return null;
                            }

                            result.AddRange(TradutorResultadoResumoVendas.TraduzirRetornoListaConsultarVencimentosResumoVenda(vencimentos, quantidadeVencimentos));

                        } while (flagContinua == "S");
                    }

                    Log.GravarLog(EventoLog.FimAgente, new { result });

                    return result;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion

        #region WACA700 - Resumo de vendas - Cartões de crédito.
        /// <summary>
        /// WACA700 - Resumo de vendas - Cartões de crédito.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarWACA700RetornoDTO> ConsultarWACA700(out StatusRetornoDTO statusRetornoDTO, ConsultarWACA700EnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarWACA700";

            using (Logger Log = Logger.IniciarLog("Resumo de vendas - Cartões de crédito [WACA700]"))
            {


                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {
                    using (RVClient cliente = new RVClient())
                    {
                        int numeroEstabelecimento = envio.NumeroEstabelecimento;
                        int numeroResumoVenda = envio.NumeroResumoVenda;
                        string data = envio.DataApresentacao.ToString("ddMMyyyy");
                        string chaveContinua = numeroEstabelecimento.ToString().PadLeft(9, '0') +
                                                numeroResumoVenda.ToString().PadLeft(9, '0') +
                                                data;

                        string programaRetorno2 = string.Empty;
                        short quantidadeMensagensErro = 0;
                        string mensagemRetorno;
                        string flagTemRegistro = string.Empty;
                        short quantidadeOcorrencias = 0;
                        string programa = string.Empty;
                        string programaRetorno = string.Empty;
                        string tipoResumoVenda = string.Empty;
                        short numeroMes = 0;
                        short contadorTela = 0;
                        short totalTransacoesRegistradas = 0;
                        short transacaoRegistrada = 0;
                        DetalheResumoVenda[] detalhes;

                        short codigoRetorno;
                        Log.GravarLog(EventoLog.ChamadaHIS, new { numeroEstabelecimento, numeroResumoVenda, data, chaveContinua, programaRetorno2, quantidadeMensagensErro, flagTemRegistro, quantidadeOcorrencias, programa, programaRetorno, tipoResumoVenda, numeroMes, contadorTela, totalTransacoesRegistradas, transacaoRegistrada });


                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {
                            codigoRetorno = cliente.ConsultarDetalhesResumoDeVenda(numeroEstabelecimento, numeroResumoVenda, data, ref chaveContinua, ref programaRetorno2, ref quantidadeMensagensErro, out mensagemRetorno, ref flagTemRegistro, ref quantidadeOcorrencias, ref programa, ref programaRetorno, ref tipoResumoVenda, ref numeroMes, ref contadorTela, ref totalTransacoesRegistradas, ref transacaoRegistrada, out detalhes);
                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;
                            quantidadeOcorrencias = 3;

                            detalhes = new DetalheResumoVenda[3];

                            detalhes[0].Detalhe = "Detalhe 1 Detalhe 1 Detalhe 1 Detalhe 1 Detalhe 1 Detalhe 1 Detalhe 1";
                            detalhes[0].NumeroMes = 01;
                            detalhes[0].Timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:ffffff"); ;
                            detalhes[0].TipoResumoVenda = "1";

                            detalhes[1].Detalhe = "Detalhe 2 Detalhe 2 Detalhe 2 Detalhe 2 Detalhe 2 Detalhe 2 Detalhe 2";
                            detalhes[1].NumeroMes = 02;
                            detalhes[1].Timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:ffffff"); ;
                            detalhes[1].TipoResumoVenda = "2";

                            detalhes[2].Detalhe = "Detalhe 3 Detalhe 3 Detalhe 3 Detalhe 3 Detalhe 3 Detalhe 3 Detalhe 3";
                            detalhes[2].NumeroMes = 03;
                            detalhes[2].Timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:ffffff"); ;
                            detalhes[2].TipoResumoVenda = "3";
                        }

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);
                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno, chaveContinua, programaRetorno2, quantidadeMensagensErro, flagTemRegistro, quantidadeOcorrencias, programa, programaRetorno, tipoResumoVenda, numeroMes, contadorTela, totalTransacoesRegistradas, transacaoRegistrada, detalhes });

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        List<ConsultarWACA700RetornoDTO> result = TradutorResultadoResumoVendas.TraduzirRetornoListaConsultarWACA700(detalhes, quantidadeOcorrencias);

                        Log.GravarLog(EventoLog.FimAgente, new { result });

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion

        #region WACA701 - Resumo de vendas - Cartões de crédito.
        /// <summary>
        /// WACA701 - Resumo de vendas - Cartões de crédito.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public ConsultarWACA701RetornoDTO ConsultarWACA701(out StatusRetornoDTO statusRetornoDTO, ConsultarWACA701EnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarWACA701";
            using (Logger Log = Logger.IniciarLog("Resumo de vendas - Cartões de crédito [WACA701]"))
            {


                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {
                    using (RVClient cliente = new RVClient())
                    {
                        short quantidadeMensagensErro = default(short);
                        string mensagemRetorno;
                        string programa = default(string);
                        string programaRetornado = default(string);
                        int resumoVenda;
                        decimal valorApresentado;
                        short quantidadeComprovantesVenda;
                        decimal valorApurado;
                        string dataApresentacaoRetornado;
                        decimal valorDesconto;
                        string dataProcessamento;
                        decimal valorGorjetaTaxaEmbarque;
                        string tipoResumo;
                        decimal valorCotacao;
                        string tipoMoeda;
                        string indicadorTaxaEmbarque;
                        string reservaDados;
                        int numeroResumoVenda = envio.NumeroResumoVenda;
                        int numeroEstabelecimento = envio.NumeroEstabelecimento;
                        string tipoResumoVenda = envio.TipoResumoVenda;
                        string timestamp = envio.Timestamp;
                        string dataApresentacao = envio.DataApresentacao.ToString("ddMMyyyy");

                        short codigoRetorno;
                        Log.GravarLog(EventoLog.ChamadaHIS, new { numeroResumoVenda, numeroEstabelecimento, tipoResumoVenda, timestamp, dataApresentacao });


                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {
                            codigoRetorno = cliente.ConsultarResumosDeVenda(out quantidadeMensagensErro, out mensagemRetorno, out programa, out programaRetornado, out resumoVenda, out valorApresentado, out quantidadeComprovantesVenda, out valorApurado, out dataApresentacaoRetornado, out valorDesconto, out dataProcessamento, out valorGorjetaTaxaEmbarque, out tipoResumo, out valorCotacao, out tipoMoeda, out indicadorTaxaEmbarque, out reservaDados, numeroResumoVenda, numeroEstabelecimento, tipoResumoVenda, timestamp, dataApresentacao);

                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;

                            resumoVenda = 0;
                            valorApresentado = 111111111111111.11M;
                            quantidadeComprovantesVenda = 111;
                            valorApurado = 999999999999999.99M;
                            dataApresentacaoRetornado = "01/01/2012";
                            valorDesconto = 333333333333333.33M;
                            dataProcessamento = "01/01/2012";
                            valorGorjetaTaxaEmbarque = 444444444444444.44M;
                            tipoResumo = "Bandeira 001";
                            valorCotacao = 55555.55555M; ;
                            tipoMoeda = "R";
                            indicadorTaxaEmbarque = "X";
                            reservaDados = string.Empty;
                            numeroResumoVenda = 111111111;

                        }

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);
                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno, quantidadeMensagensErro, programa, programaRetornado, resumoVenda, valorApresentado, quantidadeComprovantesVenda, valorApurado, dataApresentacaoRetornado, valorDesconto, dataProcessamento, valorGorjetaTaxaEmbarque, tipoResumo, valorCotacao, tipoMoeda, indicadorTaxaEmbarque, reservaDados });
                        if (codigoRetorno != 0)
                        {

                            return null;
                        }

                        ConsultarWACA701RetornoDTO result = TradutorResultadoResumoVendas.TraduzirRetornoConsultarWACA701(resumoVenda, valorApresentado, quantidadeComprovantesVenda, valorApurado, dataApresentacaoRetornado, valorDesconto, dataProcessamento, valorGorjetaTaxaEmbarque, tipoResumo, valorCotacao, tipoMoeda, indicadorTaxaEmbarque);

                        Log.GravarLog(EventoLog.FimAgente, new { result });

                        return result;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion

        #region WACA705 - Resumo de vendas - Cartões de crédito - CV's rejeitados.
        /// <summary>
        /// WACA705 - Resumo de vendas - Cartões de crédito - CV's rejeitados.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarRejeitadosRetornoDTO> ConsultarRejeitados(out StatusRetornoDTO statusRetornoDTO, ConsultarRejeitadosEnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarRejeitados";
            using (Logger Log = Logger.IniciarLog("Resumo de vendas - Cartões de crédito - CV's rejeitados [WACA705]"))
            {


                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {
                    List<ConsultarRejeitadosRetornoDTO> result = new List<ConsultarRejeitadosRetornoDTO>();

                    using (RVClient cliente = new RVClient())
                    {
                        string mensagemRetorno = string.Empty;
                        string timestamp = envio.Timestamp;
                        short tipoResumoVenda = envio.TipoResumoVenda;
                        int numeroEstabelecimento = envio.NumeroEstabelecimento;
                        int numeroResumoVenda = envio.NumeroResumoVenda;
                        string dataApresentacao = envio.DataApresentacao.ToString("yyyyMMdd");
                        string chaveContinua = string.Empty;
                        short numeroMes = (short)envio.DataApresentacao.Month;
                        string flagTemRegistro = string.Empty;
                        short quantidadeOcorrencias = 0;
                        int quantidadeResumosVenda = 0;
                        string programa = string.Empty;
                        string programaRetorno = string.Empty;
                        short transacaoRegistrada = 0;
                        short totalTransacoesRegistradas = 0;
                        short colunaInicial = 0;
                        short contadorTela = 0;
                        ResumoVendaRejeitado[] resumos;

                        do
                        {
                            short codigoRetorno;
                            Log.GravarLog(EventoLog.ChamadaHIS, new { timestamp, tipoResumoVenda, numeroEstabelecimento, numeroResumoVenda, dataApresentacao, chaveContinua, numeroMes, flagTemRegistro, quantidadeOcorrencias, quantidadeResumosVenda, programa, programaRetorno, transacaoRegistrada, totalTransacoesRegistradas, colunaInicial, contadorTela });


                            if (!TesteHelper.IsAmbienteDesenvolvimento())
                            {
                                cliente.ConsultarRejeitados(timestamp, tipoResumoVenda, numeroEstabelecimento, numeroResumoVenda, dataApresentacao, ref chaveContinua, out codigoRetorno, numeroMes, ref mensagemRetorno, ref flagTemRegistro, ref quantidadeOcorrencias, ref quantidadeResumosVenda, ref programa, ref programaRetorno, ref transacaoRegistrada, ref totalTransacoesRegistradas, ref colunaInicial, ref contadorTela, out resumos);
                            }
                            else
                            {
                                codigoRetorno = 0;
                                mensagemRetorno = string.Empty;
                                quantidadeOcorrencias = 3;

                                resumos = new ResumoVendaRejeitado[3];

                                resumos[0].Cartao = "1111111111111111111";
                                resumos[0].DataComprovanteVenda = "01.01.2001"; //01/01/2001;
                                resumos[0].Descricao = "Descricao 000000001";
                                resumos[0].NumeroEstabelecimento = 111111111;
                                resumos[0].Sequencia = 111;
                                resumos[0].Valor = 111111111111111.11M;
                                resumos[0].Autorizacao = 111111111;
                                resumos[0].WACA705_ID_TOK = "S";

                                resumos[1].Cartao = "2222222222222222222";
                                resumos[1].DataComprovanteVenda = "02.02.2002"; //02/02/2002;
                                resumos[1].Descricao = "Descricao 000000002";
                                resumos[1].NumeroEstabelecimento = 222222222;
                                resumos[1].Sequencia = 222;
                                resumos[1].Valor = 222222222222222.22M;
                                resumos[1].Autorizacao = 222222222;
                                resumos[1].WACA705_ID_TOK = "S";

                                resumos[2].Cartao = "9999999999999999999";
                                resumos[2].DataComprovanteVenda = "31.12.2012"; //31/12/2012;
                                resumos[2].Descricao = "Descricao 000000003";
                                resumos[2].NumeroEstabelecimento = 999999999;
                                resumos[2].Sequencia = 999;
                                resumos[2].Valor = 999999999999999.99M;
                                resumos[2].Autorizacao = 999999999;
                                resumos[2].WACA705_ID_TOK = "S";

                            }

                            statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);
                            Log.GravarLog(EventoLog.RetornoHIS, new { chaveContinua, mensagemRetorno, numeroMes, flagTemRegistro, quantidadeOcorrencias, quantidadeResumosVenda, programa, programaRetorno, transacaoRegistrada, totalTransacoesRegistradas, colunaInicial, contadorTela, resumos });

                            if (codigoRetorno != 0)
                            {
                                return null;
                            }

                            result.AddRange(TradutorResultadoResumoVendas.TraduzirRetornoListaConsultarRejeitados(resumos, quantidadeOcorrencias));

                        } while (chaveContinua == "S");
                    }

                    Log.GravarLog(EventoLog.FimAgente, new { result });

                    return result;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);

                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
        #endregion
    }
}
