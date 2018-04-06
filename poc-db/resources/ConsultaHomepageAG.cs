using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Extrato.Agentes.ServicoConsultaHomepage;
using Redecard.PN.Extrato.Agentes.Tradutores;
using Redecard.PN.Extrato.Comum;
using Redecard.PN.Extrato.Comum.Helper;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Agentes
{
    public class ConsultaHomepageAG : AgentesBase
    {
        #region WACA1107 - Home - Últimas Vendas.
        /// <summary>
        /// WACA1107 - Home - Últimas Vendas.
        /// </summary>
        /// <param name="statusRetornoDTO"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarTransacoesCreditoDebitoRetornoDTO>ConsultarTransacoesCreditoDebito(out StatusRetornoDTO statusRetornoDTO, ConsultarTransacoesCreditoDebitoEnvioDTO envio)
        {
            using (Logger Log = Logger.IniciarLog("Home - Últimas Vendas [WACA1107]"))
            {
                Log.GravarLog(EventoLog.InicioAgente, new { envio });

                string FONTE_METODO = this.GetType().Name + ".ConsultarTransacoesCreditoDebito";
                
                try
                {
                    using (HomepagesClient cliente = new HomepagesClient())
                    {
                        string mensagemRetorno;
                        short quantidadeValores;
                        DadoValor[] valores;
                        string reservaDados = null;
                        string programaChamador = "WACA1107";
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

                        Log.GravarLog(EventoLog.ChamadaHIS, 
                            new { programaChamador, sistema, dataInicial, dataFinal, quantidadeEstabelecimentos, estabelecimentos });

                        if (!TesteHelper.IsAmbienteDesenvolvimento())
                        {                            
                            codigoRetorno = cliente.ConsultarTransacoesCreditoDebito(out mensagemRetorno, out quantidadeValores, out valores, out reservaDados, programaChamador, sistema, dataInicial, dataFinal, quantidadeEstabelecimentos, estabelecimentos);                            
                        }
                        else
                        {
                            codigoRetorno = 0;
                            mensagemRetorno = string.Empty;
                            valores = new DadoValor[6];
                            quantidadeValores = (Int16) valores.Length;
                            DateTime data;

                            for (Int32 iLanc = 1; iLanc <= quantidadeValores; iLanc++)
                            {
                                data = DateTime.Now.AddDays(iLanc - quantidadeValores);
                                valores[iLanc-1].Data = data.ToString("dd.MM.yyyy");
                                valores[iLanc-1].ValorCredito = (data.Day * estabelecimentos.FirstOrDefault()) / 100m;
                                valores[iLanc - 1].ValorDebito = (data.Day * estabelecimentos.FirstOrDefault()) / 1000m;
                            }
                        }
                        Log.GravarLog(EventoLog.RetornoHIS, new { codigoRetorno, mensagemRetorno, quantidadeValores, valores, reservaDados });

                        statusRetornoDTO = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, FONTE_METODO);

                        if (codigoRetorno != 0)
                        {
                            return null;
                        }

                        List<ConsultarTransacoesCreditoDebitoRetornoDTO> result = TradutorResultadoConsultaHomepage.TraduzirRetornoListaConsultarTransacoesCreditoDebito(valores, quantidadeValores);
                        
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
