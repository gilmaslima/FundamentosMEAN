using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Negocio;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Servicos.Tradutor;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Servicos
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RelatorioPagamentosAjustados" in code, svc and config file together.
    public class RelatorioPagamentosAjustados : Redecard.PN.Comum.ServicoBase, IRelatorioPagamentosAjustados
    {
        #region WACA1106 - Relatório de pagamentos ajustados.
        /// <summary>
        /// WACA1106 - Relatório de pagamentos ajustados.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <param name="quantidadeRegistrosPorPagina"></param>
        /// <param name="guidPesquisa"></param>
        /// <param name="guidUsuarioCacheExtrato"></param>
        /// <returns></returns>
        public ConsultarOrdensCreditoEnviadosAoBancoRetorno ConsultarOrdensCreditoEnviadosAoBancoPesquisa(out StatusRetorno statusRetorno, ConsultarOrdensCreditoEnviadosAoBancoEnvio envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {

            using (Logger Log = Logger.IniciarLog("Relatório de pagamentos ajustados [WACA1106]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarOrdensCreditoEnviadosAoBancoEnvioDTO envioDTO = TradutorConsultarOrdensCreditoEnviadosAoBanco.TraduzirEnvioConsultarOrdensCreditoEnviadosAoBanco(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato });
                    RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarOrdensCreditoEnviadosAoBancoTotaisRetornoDTO> retornoPesquisa = ConsultarOrdensCreditoEnviadosAoBancoBLL.Instance.PesquisarDT(out statusRetornoDTO, envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato);
                    Log.GravarLog(EventoLog.RetornoAgente, new { retornoPesquisa, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { retornoPesquisa, statusRetorno });
                        return null;
                    }

                    ConsultarOrdensCreditoEnviadosAoBancoRetorno result = TradutorConsultarOrdensCreditoEnviadosAoBanco.TraduzirRetornoConsultarOrdensCreditoEnviadosAoBanco(retornoPesquisa);


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
