using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Tradutor;
using Redecard.PN.Extrato.Negocio;
using Redecard.PN.Extrato.Comum;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Dirf" in code, svc and config file together.
    public class Dirf : Redecard.PN.Comum.ServicoBase, IDirf
    {
        #region WACA075 - Dirf.
        /// <summary>
        /// WACA075 - Dirf.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public ConsultarExtratoDirfRetorno ConsultarExtratoDirf(out StatusRetorno statusRetorno, ConsultarExtratoDirfEnvio envio)
        {

            using (Logger Log = Logger.IniciarLog("Dirf [WACA075]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarExtratoDirfEnvioDTO envioDTO = TradutorConsultarExtratoDirf.TraduzirEnvioConsultarExtratoDirf(envio);

                    StatusRetornoDTO statusRetornoDTO;

                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO});
                    ConsultarExtratoDirfRetornoDTO dto = ConsultarExtratoDirfBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);
                    Log.GravarLog(EventoLog.RetornoAgente, new { dto, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);


                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { dto, statusRetorno });
                        return null;
                    }

                    ConsultarExtratoDirfRetorno result = TradutorConsultarExtratoDirf.TraduzirRetornoConsultarExtratoDirf(dto);

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

        #region WACA079 - Dirf.
        /// <summary>
        /// WACA079 - Dirf.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <returns></returns>
        public ConsultarAnosBaseDirfRetorno ConsultarAnosBaseDirf(out StatusRetorno statusRetorno)
        {

            using (Logger Log = Logger.IniciarLog("Dirf [WACA079]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico);

                    StatusRetornoDTO statusRetornoDTO;

                    Log.GravarLog(EventoLog.ChamadaAgente);

                    ConsultarAnosBaseDirfRetornoDTO dto = ConsultarAnosBaseDirfBLL.Instance.Pesquisar(out statusRetornoDTO);

                    Log.GravarLog(EventoLog.RetornoAgente, new { dto, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);


                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { dto, statusRetorno });
                        return null;
                    }

                    ConsultarAnosBaseDirfRetorno result = TradutorConsultarAnosBaseDirf.TraduzirRetornoConsultarAnosBaseDirf(dto);

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
