using System;
using System.Collections.Generic;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes.ServicoConsultaDebito;
using Redecard.PN.Extrato.Agentes.Tradutores;
using Redecard.PN.Extrato.Comum.Helper;
using Redecard.PN.Extrato.Modelo;

namespace Redecard.PN.Extrato.Agentes
{
    public class ConsultarDebitoAG : AgentesBase
    {                    
        #region WACA799 - Resumo de vendas - CDC.
        /// <summary>
        /// WACA799 - Resumo de vendas - CDC.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public ConsultarTransacaoDebitoRetornoDTO ConsultarTransacaoDebito(out StatusRetornoDTO statusRetornoDTO, ConsultarTransacaoDebitoEnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarTransacaoDebito";
            using (Logger Log = Logger.IniciarLog("Resumo de vendas - CDC [WACA799]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });

                try
                {
                    using (DebitosClient cliente = new DebitosClient())
                    {
                        string programaRetornado = string.Empty;
                        string programa = string.Empty;
                        string codigoErroSql = string.Empty;
                        string mensagemRetorno;
                        int numeroEstabelecimentoAnterior;
                        int numeroResumoVendaAnterior;
                        string dataApresentacaoAnterior;
                        short quantidadeOcorrencias;
                        int quantidadeComprovanteVenda;
                        decimal valorApresentado;
                        decimal valorLiquido;
                        decimal valorDesconto;
                        string dataVencimento;
                        string indicadorPreDatado;
                        short codigoTransacao;
                        short subCodigoTransacao;
                        string descricaoTransacao;
                        decimal valorDescontoTaxaCrediario;
                        decimal valorSaque;
                        decimal valorTotalCpmf;
                        decimal valorIof;
                        string tipoResposta;
                        string codigoBandeira;
                        string programaChamador = "WACA799";
                        int numeroEstabelecimento = envio.NumeroEstabelecimento;
                        int numeroResumoVenda = envio.NumeroResumoVenda;
                        string dataApresentacao = envio.DataApresentacao.ToString("yyyyMMdd");

                        short codigoRetorno;
                        Log.GravarLog(EventoLog.ChamadaHIS, new { programaChamador, numeroEstabelecimento, numeroResumoVenda, dataApresentacao });

                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {
                            codigoRetorno = cliente.ConsultarTransacaoDebito(out programaRetornado, out programa, out codigoErroSql, out mensagemRetorno, out numeroEstabelecimentoAnterior, out numeroResumoVendaAnterior, out dataApresentacaoAnterior, out quantidadeOcorrencias, out quantidadeComprovanteVenda, out valorApresentado, out valorLiquido, out valorDesconto, out dataVencimento, out indicadorPreDatado, out codigoTransacao, out subCodigoTransacao, out descricaoTransacao, out valorDescontoTaxaCrediario, out valorSaque, out valorTotalCpmf, out valorIof, out tipoResposta, out codigoBandeira, programaChamador, numeroEstabelecimento, numeroResumoVenda, dataApresentacao);

                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;

                            numeroEstabelecimentoAnterior = 111111111;
                            numeroResumoVendaAnterior = 222222222;
                            dataApresentacaoAnterior = "20010101";
                            quantidadeOcorrencias = 111;
                            quantidadeComprovanteVenda = 22222;
                            valorApresentado = 11111111111111111;
                            valorLiquido = 22222222222222222;
                            valorDesconto = 33333333333333333;
                            dataVencimento = "20121231";
                            indicadorPreDatado = "P";
                            codigoTransacao = 64;
                            subCodigoTransacao = 4;
                            descricaoTransacao = "Deveria ser descrição, não desconto";
                            valorDescontoTaxaCrediario = 44444444444;
                            valorSaque = 55555555555;
                            valorTotalCpmf = 66666666666;
                            valorIof = 77777777777;
                            tipoResposta = "X";
                            codigoBandeira = "Bandeira 001";
                        }
                        Log.GravarLog(EventoLog.RetornoHIS, new { programaRetornado, programa, codigoErroSql, mensagemRetorno, numeroEstabelecimentoAnterior, numeroResumoVendaAnterior, dataApresentacaoAnterior, quantidadeOcorrencias, quantidadeComprovanteVenda, valorApresentado, valorLiquido, valorDesconto, dataVencimento, indicadorPreDatado, codigoTransacao, subCodigoTransacao, descricaoTransacao, valorDescontoTaxaCrediario, valorSaque, valorTotalCpmf, valorIof, tipoResposta, codigoBandeira });
                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        Log.GravarLog(EventoLog.FimAgente, new { statusRetornoDTO });

                        return TradutorResultadoConsultaDebito.TraduzirRetornoConsultarTransacaoDebito(numeroEstabelecimentoAnterior, numeroResumoVendaAnterior, dataApresentacaoAnterior, quantidadeOcorrencias, quantidadeComprovanteVenda, valorApresentado, valorLiquido, valorDesconto, dataVencimento, indicadorPreDatado, codigoTransacao, subCodigoTransacao, descricaoTransacao, valorDescontoTaxaCrediario, valorSaque, valorTotalCpmf, valorIof, tipoResposta, codigoBandeira);
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
