using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.ServicoConsultaLancamentosFuturos;
using Redecard.PN.Extrato.Agentes.Tradutores;
using Redecard.PN.Extrato.Comum;
using Redecard.PN.Extrato.Comum.Helper;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Agentes
{
    public class ConsultarLancamentosFuturosAG : AgentesBase
    {
        #region WACA1108 - Home - Lançamentos futuros.
        /// <summary>
        /// WACA1108 - Home - Lançamentos futuros.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarCreditoDebitoRetornoDTO>ConsultarCreditoDebito(out StatusRetornoDTO statusRetornoDTO, ConsultarCreditoDebitoEnvioDTO envio)
        {
            using (Logger Log = Logger.IniciarLog("Home - Lançamentos Futuros [WACA1108]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });

                string FONTE_METODO = this.GetType().Name + ".ConsultarCreditoDebito";
                
                try
                {
                    using (LancFuturosClient cliente = new LancFuturosClient())
                    {
                        string mensagemRetorno;
                        short quantidadeValores;
                        LancamentoFuturo[] lancamentos;
                        string reservaDados = null;
                        string programa = "WACA1108";
                        string sistema = "IS";
                        string dataInicial = envio.DataInicial.ToString("dd/MM/yyyy");
                        string dataFinal = envio.DataFinal.ToString("dd/MM/yyyy");
                        short quantidadeEstabelecimentos = (short)envio.Estabelecimentos.Count;
                        int[] estabelecimentos = new int[3000];

                        for (int i = 0; i < envio.Estabelecimentos.Count; i++)
                        {
                            estabelecimentos[i] = envio.Estabelecimentos[i];
                        }

                        short codigoRetorno;

                        Log.GravarLog(EventoLog.ChamadaHIS, new { programa, sistema, dataInicial, dataFinal, quantidadeEstabelecimentos, estabelecimentos });
                        
                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {
                            codigoRetorno = cliente.ConsultarCreditoDebito(out mensagemRetorno, out quantidadeValores, out lancamentos, out reservaDados, programa, sistema, dataInicial, dataFinal, quantidadeEstabelecimentos, estabelecimentos);
                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;
                            lancamentos = new LancamentoFuturo[6];
                            quantidadeValores = (Int16) lancamentos.Length;
                            DateTime data;

                            for (Int32 iLanc = 1; iLanc <= lancamentos.Length; iLanc++)
                            {
                                data = DateTime.Now.AddDays(iLanc);                                    
                                lancamentos[iLanc-1].Data = data.ToString("dd.MM.yyyy");
                                lancamentos[iLanc-1].ValorCredito = (data.Day * estabelecimentos.FirstOrDefault()) / 100m;
                                lancamentos[iLanc-1].ValorDebito = 0;
                            }
                        }
                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno, quantidadeValores, lancamentos, reservaDados });

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        List<ConsultarCreditoDebitoRetornoDTO> result = TradutorResultadoLancamentosFuturos.TraduzirRetornoListaConsultarCreditoDebito(lancamentos, quantidadeValores);
                        
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
