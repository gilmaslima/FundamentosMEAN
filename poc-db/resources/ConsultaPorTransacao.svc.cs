using System;
using System.Collections.Generic;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Negocio;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.Tradutor;

namespace Redecard.PN.Extrato.Servicos
{
    public class ConsultaPorTransacao : Redecard.PN.Comum.ServicoBase, IConsultaPorTransacao
    {
        #region WACA1116 - Consultar por transação - Carta.
        /// <summary>
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarCartasRetorno> ConsultarCartas(out StatusRetorno statusRetorno, ConsultarCartasEnvio envio)
        {

            using (Logger Log = Logger.IniciarLog("Consultar por transação - Carta [WACA1116]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });
                    ConsultarCartasEnvioDTO envioDTO = TradutorConsultarCartas.TraduzirEnvioConsultarCartas(envio);

                    StatusRetornoDTO statusRetornoDTO;

                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO });
                    List<ConsultarCartasRetornoDTO> registros = ConsultarCartasBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                    Log.GravarLog(EventoLog.RetornoAgente, new { registros, statusRetornoDTO });


                    if (statusRetorno.CodigoRetorno != 0)
                    {

                        return null;
                    }

                    List<ConsultarCartasRetorno> result = TradutorConsultarCartas.TraduzirRetornoListaConsultarCartas(registros);
                    Log.GravarLog(EventoLog.FimServico, new { result });

                    return result;
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
        #endregion
    }
}
