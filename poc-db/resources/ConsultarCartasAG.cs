using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.ServicoConsultaCartas;
using Redecard.PN.Extrato.Comum.Helper;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Comum;
using Redecard.PN.Extrato.Agentes.Tradutores;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Agentes
{
    public class ConsultarCartasAG : AgentesBase
    {
        #region WACA1116 - Consultar por transação - Carta.
        /// <summary>
        /// WACA1116 - Consultar por transação - Carta.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarCartasRetornoDTO> ConsultarCartas(out StatusRetornoDTO statusRetornoDTO, ConsultarCartasEnvioDTO envio)
        {
            string FONTE_METODO = this.GetType().Name + ".ConsultarCartas";
            using (Logger Log = Logger.IniciarLog("Consultar por transação - Carta [WACA1116]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });
                try
                {
                    using (ChgBcksClient cliente = new ChgBcksClient())
                    {
                        string mensagemRetorno;
                        string programaChamador = "WACA1116";
                        string sistema = "IS";
                        string usuario = "xxx";
                        decimal numeroProcesso = envio.NumeroProcesso;
                        string timestampTransacao = envio.TimestampTransacao;
                        short sistemaDados = envio.SistemaDados;
                        string reservaDados = string.Empty;
                        short quantidadeRegistros = 0;
                        DadoCartaChargeback[] dadosCartaChargeback = new DadoCartaChargeback[50];

                        short codigoRetorno;
                        
                        Log.GravarLog(EventoLog.ChamadaHIS, new { programaChamador, sistema, usuario, numeroProcesso, timestampTransacao, sistemaDados, reservaDados, quantidadeRegistros, dadosCartaChargeback});

                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {
                            codigoRetorno = (short)cliente.ConsultarCartas(out mensagemRetorno, programaChamador, sistema, usuario, numeroProcesso, timestampTransacao, sistemaDados, reservaDados, ref quantidadeRegistros, ref dadosCartaChargeback);
                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;
                            quantidadeRegistros = 3;

                            dadosCartaChargeback = new DadoCartaChargeback[3];

                            dadosCartaChargeback[0].CodigoMotivo = 1;
                            dadosCartaChargeback[0].DataCancelamento = "01.01.2012";
                            dadosCartaChargeback[0].DataVenda = "01.01.2012";
                            dadosCartaChargeback[0].DescricaoMotivo = "Descriçao 001";
                            dadosCartaChargeback[0].NumeroCartao = "1111111111111111111";
                            dadosCartaChargeback[0].NumeroEstabelecimento = 111111111;
                            dadosCartaChargeback[0].NumeroNsu = 111111111111;
                            dadosCartaChargeback[0].NumeroProcesso = 111111111111111111M;
                            dadosCartaChargeback[0].NumeroResumo = 111111111;
                            dadosCartaChargeback[0].ValorAjuste = 111111111111111.11M;
                            dadosCartaChargeback[0].ValorCancelamento = 111111111111111.50M;
                            dadosCartaChargeback[0].ValorDebito = 111111111111111.10M;
                            dadosCartaChargeback[0].ValorTransacao = 111111111111111.90M;

                            dadosCartaChargeback[1].CodigoMotivo = 2;
                            dadosCartaChargeback[1].DataCancelamento = "02.02.2022";
                            dadosCartaChargeback[1].DataVenda = "02.02.2022";
                            dadosCartaChargeback[1].DescricaoMotivo = "Descriçao 002";
                            dadosCartaChargeback[1].NumeroCartao = "2222222222222222222";
                            dadosCartaChargeback[1].NumeroEstabelecimento = 222222222;
                            dadosCartaChargeback[1].NumeroNsu = 222222222222;
                            dadosCartaChargeback[1].NumeroProcesso = 222222222222222222M;
                            dadosCartaChargeback[1].NumeroResumo = 222222222;
                            dadosCartaChargeback[1].ValorAjuste = 222222222222222.22M;
                            dadosCartaChargeback[1].ValorCancelamento = 222222222222222.50M;
                            dadosCartaChargeback[1].ValorDebito = 222222222222222.20M;
                            dadosCartaChargeback[1].ValorTransacao = 222222222222222.90M;

                            dadosCartaChargeback[2].CodigoMotivo = 3;
                            dadosCartaChargeback[2].DataCancelamento = "31.12.2012";
                            dadosCartaChargeback[2].DataVenda = "10.12.2012";
                            dadosCartaChargeback[2].DescricaoMotivo = "Descriçao 003";
                            dadosCartaChargeback[2].NumeroCartao = "9999999999999999999";
                            dadosCartaChargeback[2].NumeroEstabelecimento = 999999999;
                            dadosCartaChargeback[2].NumeroNsu = 999999999999;
                            dadosCartaChargeback[2].NumeroProcesso = 999999999999999999M;
                            dadosCartaChargeback[2].NumeroResumo = 999999999;
                            dadosCartaChargeback[2].ValorAjuste = 999999999999999.99M;
                            dadosCartaChargeback[2].ValorCancelamento = 999999999999999.50M;
                            dadosCartaChargeback[2].ValorDebito = 999999999999999.90M;
                            dadosCartaChargeback[2].ValorTransacao = 999999999999999.90M;
                        }

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);

                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno, quantidadeRegistros, dadosCartaChargeback});
                        if (codigoRetorno != 0)
                        {
                            return null;
                        }
                        List<ConsultarCartasRetornoDTO> result = TradutorResultadoConsultaCarta.TraduzirRetornoListaConsultarCartas(dadosCartaChargeback, quantidadeRegistros);

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
