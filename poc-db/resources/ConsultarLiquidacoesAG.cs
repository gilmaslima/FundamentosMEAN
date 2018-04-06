using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.ServicoConsultaLiquidacoes;
using Redecard.PN.Extrato.Agentes.Tradutores;
using Redecard.PN.Extrato.Comum;
using Redecard.PN.Extrato.Comum.Helper;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Agentes
{
    public class ConsultarLiquidacoesAG : AgentesBase
    {
        #region ISF3 - WACA152 - Relatório de débitos e desagendamentos - Motivo débito.
        /// <summary>
        /// WACA152 - Relatório de débitos e desagendamentos - Motivo débito.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envioDTO"></param>
        /// <returns></returns>
        public ConsultarMotivoDebitoRetornoDTO ConsultarMotivoDebito(out StatusRetornoDTO statusRetornoDTO, ConsultarMotivoDebitoEnvioDTO envioDTO)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarMotivoDebito";
            using (Logger Log = Logger.IniciarLog("Relatório de débitos e desagendamentos - Motivo débito [WACA152-ISF3]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envioDTO });

                try
                {
                    using (LiquidacoesClient cliente = new LiquidacoesClient())
                    {
                        string mensagemRetorno;
                        string dataInclusao = string.Empty;
                        int numeroEstabelecimentoOrigem = 0;
                        string motivoDebito = string.Empty;
                        string referenciaProcessamento = string.Empty;
                        string resumo = string.Empty;
                        string bandeiraResumoVendas = string.Empty;
                        string dataVenda = string.Empty;
                        string cartao = string.Empty;
                        string numeroComprovanteVenda = string.Empty;
                        string valorConta = string.Empty;
                        string parcela = string.Empty;
                        string flagRetorno = string.Empty;
                        short quantidadeRegistros;
                        ExtratoLiquidacao[] extratos = new ExtratoLiquidacao[240];
                        decimal totalDebito = default(decimal);
                        decimal totalComplementar = default(decimal);
                        decimal totalPendente = default(decimal);
                        string programaChamador = string.Empty;
                        int numeroEstabelecimento = envioDTO.NumeroEstabelecimento;
                        string dataPesquisa = envioDTO.DataPesquisa.ToString("yyyyMMdd");
                        string timestamp = envioDTO.Timestamp;
                        decimal numeroDebito = envioDTO.NumeroDebito;
                        string tipoPesquisa = envioDTO.TipoPesquisa;

                        short codigoRetorno;
                        Log.GravarLog(EventoLog.ChamadaHIS, new { programaChamador, numeroEstabelecimento, dataPesquisa, timestamp, numeroDebito, tipoPesquisa });

                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {

                            codigoRetorno = cliente
                                                .ConsultarResumoVendasDebitos2(out mensagemRetorno, 
                                                                               out dataInclusao, 
                                                                               out numeroEstabelecimentoOrigem, 
                                                                               out motivoDebito, 
                                                                               out referenciaProcessamento, 
                                                                               out resumo, 
                                                                               out bandeiraResumoVendas, 
                                                                               out dataVenda, 
                                                                               out cartao, 
                                                                               out numeroComprovanteVenda, 
                                                                               out valorConta, 
                                                                               out parcela, 
                                                                               out flagRetorno, 
                                                                               out quantidadeRegistros, 
                                                                               out extratos, 
                                                                               out totalDebito, 
                                                                               out totalComplementar, 
                                                                               out totalPendente, 
                                                                               programaChamador, 
                                                                               numeroEstabelecimento, 
                                                                               dataPesquisa, 
                                                                               timestamp, 
                                                                               numeroDebito, 
                                                                               tipoPesquisa);
                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;
                            quantidadeRegistros = 3;

                            extratos = new ExtratoLiquidacao[3];

                            extratos[0].BandeiraResumoVendasDebitado = "1";
                            extratos[0].DataPagamento = "20010101";
                            extratos[0].EstabelecimentoDebito = "000000001";
                            extratos[0].MeioPagamento = "Meio 01";
                            extratos[0].ResumoVendaDebito = "resumo 01";
                            extratos[0].SinalDebitoCredito = "C";
                            extratos[0].ValorComplementar = 11111111111.11M;
                            extratos[0].ValorDebito = 111111111111111.11M;
                            extratos[0].ValorPendente = 111111111111111.50M;

                            extratos[1].BandeiraResumoVendasDebitado = "2";
                            extratos[1].DataPagamento = "20020202";
                            extratos[1].EstabelecimentoDebito = "000000002";
                            extratos[1].MeioPagamento = "Meio 02";
                            extratos[1].ResumoVendaDebito = "resumo 02";
                            extratos[1].SinalDebitoCredito = "D";
                            extratos[1].ValorComplementar = 22222222222.22M;
                            extratos[1].ValorDebito = 222222222222222.22M;
                            extratos[1].ValorPendente = 222222222222222.50M;

                            extratos[2].BandeiraResumoVendasDebitado = "9";
                            extratos[2].DataPagamento = "20121231";
                            extratos[2].EstabelecimentoDebito = "000000003";
                            extratos[2].MeioPagamento = "Meio 03";
                            extratos[2].ResumoVendaDebito = "resumo 03";
                            extratos[2].SinalDebitoCredito = "D";
                            extratos[2].ValorComplementar = 99999999999.99M;
                            extratos[2].ValorDebito = 999999999999999.99M;
                            extratos[2].ValorPendente = 999999999999999.50M;
                        }

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);
                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno, dataInclusao, numeroEstabelecimentoOrigem, motivoDebito, referenciaProcessamento, resumo, bandeiraResumoVendas, dataVenda, cartao, numeroComprovanteVenda, valorConta, parcela, flagRetorno, quantidadeRegistros, extratos, totalDebito, totalComplementar, totalPendente, });

                        if (codigoRetorno != 0)
                        {

                            return null;
                        }

                        ConsultarMotivoDebitoRetornoDTO result = TradutorResultadoConsultaLiquidacoes.TraduzirRetornoConsultarMotivoDebito(dataInclusao, numeroEstabelecimentoOrigem, motivoDebito, referenciaProcessamento, resumo, bandeiraResumoVendas, dataVenda, cartao, numeroComprovanteVenda, valorConta, parcela, quantidadeRegistros, extratos);

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

        #region ISD3 - WACA152 - Relatório de débitos e desagendamentos - Motivo débito.
        /// <summary>
        /// WACA152 - Relatório de débitos e desagendamentos - Motivo débito.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envioDTO"></param>
        /// <returns></returns>
        public ConsultarMotivoDebitoRetornoDTO ConsultarMotivoDebitoISD3(out StatusRetornoDTO statusRetornoDTO, ConsultarMotivoDebitoEnvioDTO envioDTO)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarMotivoDebito";
            using (Logger Log = Logger.IniciarLog("Relatório de débitos e desagendamentos - Motivo débito [WACA152-ISD3]"))
            {


                Log.GravarLog(EventoLog.InicioAgente, new { envioDTO });

                try
                {
                    using (LiquidacoesClient cliente = new LiquidacoesClient())
                    {
                        string mensagemRetorno;
                        string dataInclusao = string.Empty;
                        int numeroEstabelecimentoOrigem = 0;
                        string motivoDebito = string.Empty;
                        string referenciaProcessamento = string.Empty;
                        string resumo = string.Empty;
                        string bandeiraResumoVendas = string.Empty;
                        string dataVenda = string.Empty;
                        string cartao = string.Empty;
                        string numeroComprovanteVenda = string.Empty;
                        string valorConta = string.Empty;
                        string parcela = string.Empty;
                        string flagRetorno = string.Empty;
                        short quantidadeRegistros;
                        ExtratoLiquidacao[] extratos = new ExtratoLiquidacao[270];
                        decimal totalDebito = default(decimal);
                        decimal totalComplementar = default(decimal);
                        decimal totalPendente = default(decimal);

                        String _totalDebito = String.Empty;
                        String _totalComplementar = String.Empty;
                        String _totalPendente = String.Empty;

                        string programaChamador = string.Empty;
                        int numeroEstabelecimento = envioDTO.NumeroEstabelecimento;
                        string dataPesquisa = envioDTO.DataPesquisa.ToString("yyyyMMdd");
                        string timestamp = envioDTO.Timestamp;
                        decimal numeroDebito = envioDTO.NumeroDebito;
                        string tipoPesquisa = envioDTO.TipoPesquisa;

                        short codigoRetorno;
                        Log.GravarLog(EventoLog.ChamadaHIS, new { programaChamador, numeroEstabelecimento, dataPesquisa, timestamp, numeroDebito, tipoPesquisa });

                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {
                            // Este código foi substituido pela versão anterior do book, a nova versão
                            // buscava dados da base online e não estava funcionando corretamente por divergências
                            // nos ambientes de Simulação/Produção da Redecard (Mainframe)

                            //codigoRetorno = cliente.ConsultarResumoVendasDebitos(out mensagemRetorno, out dataInclusao, out numeroEstabelecimentoOrigem, out motivoDebito, out referenciaProcessamento, out resumo, out bandeiraResumoVendas, out dataVenda, out cartao, out numeroComprovanteVenda, out valorConta, out parcela, out flagRetorno, out quantidadeRegistros, out extratos, out totalDebito, out totalComplementar, out totalPendente, programaChamador, numeroEstabelecimento, dataPesquisa, timestamp, numeroDebito, tipoPesquisa);
                            codigoRetorno = cliente.WACA152(out mensagemRetorno, out dataInclusao, out numeroEstabelecimentoOrigem, out motivoDebito, out referenciaProcessamento, out resumo, out bandeiraResumoVendas, out dataVenda, out cartao, out numeroComprovanteVenda, out valorConta, out parcela, out flagRetorno, out quantidadeRegistros, out extratos, out _totalDebito, out _totalComplementar, out _totalPendente, programaChamador, numeroEstabelecimento, dataPesquisa, timestamp, tipoPesquisa);
                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;
                            quantidadeRegistros = 3;

                            extratos = new ExtratoLiquidacao[3];

                            extratos[0].BandeiraResumoVendasDebitado = "1";
                            extratos[0].DataPagamento = "20010101";
                            extratos[0].EstabelecimentoDebito = "000000001";
                            extratos[0].MeioPagamento = "Meio 01";
                            extratos[0].ResumoVendaDebito = "resumo 01";
                            extratos[0].SinalDebitoCredito = "C";
                            extratos[0].ValorComplementar = 11111111111.11M;
                            extratos[0].ValorDebito = 111111111111111.11M;
                            extratos[0].ValorPendente = 111111111111111.50M;

                            extratos[1].BandeiraResumoVendasDebitado = "2";
                            extratos[1].DataPagamento = "20020202";
                            extratos[1].EstabelecimentoDebito = "000000002";
                            extratos[1].MeioPagamento = "Meio 02";
                            extratos[1].ResumoVendaDebito = "resumo 02";
                            extratos[1].SinalDebitoCredito = "D";
                            extratos[1].ValorComplementar = 22222222222.22M;
                            extratos[1].ValorDebito = 222222222222222.22M;
                            extratos[1].ValorPendente = 222222222222222.50M;

                            extratos[2].BandeiraResumoVendasDebitado = "9";
                            extratos[2].DataPagamento = "20121231";
                            extratos[2].EstabelecimentoDebito = "000000003";
                            extratos[2].MeioPagamento = "Meio 03";
                            extratos[2].ResumoVendaDebito = "resumo 03";
                            extratos[2].SinalDebitoCredito = "D";
                            extratos[2].ValorComplementar = 99999999999.99M;
                            extratos[2].ValorDebito = 999999999999999.99M;
                            extratos[2].ValorPendente = 999999999999999.50M;
                        }

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);
                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno, dataInclusao, numeroEstabelecimentoOrigem, motivoDebito, referenciaProcessamento, resumo, bandeiraResumoVendas, dataVenda, cartao, numeroComprovanteVenda, valorConta, parcela, flagRetorno, quantidadeRegistros, extratos, totalDebito, totalComplementar, totalPendente, });

                        if (codigoRetorno != 0)
                        {

                            return null;
                        }

                        ConsultarMotivoDebitoRetornoDTO result = TradutorResultadoConsultaLiquidacoes.TraduzirRetornoConsultarMotivoDebito(dataInclusao, numeroEstabelecimentoOrigem, motivoDebito, referenciaProcessamento, resumo, bandeiraResumoVendas, dataVenda, cartao, numeroComprovanteVenda, valorConta, parcela, quantidadeRegistros, extratos);

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
    }
}
