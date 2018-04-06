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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RelatorioDebitosDesagendamentos" in code, svc and config file together.
    public class RelatorioDebitosDesagendamentos : Redecard.PN.Comum.ServicoBase, IRelatorioDebitosDesagendamentos
    {
        #region WACA150 - Relatório de débitos e desagendamentos - Consolidado.
        /// <summary>
        /// WACA150 - Relatório de débitos e desagendamentos - Consolidado.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <param name="numeroPagina"></param>
        /// <param name="quantidadeRegistrosPorPagina"></param>
        /// <param name="guidPesquisa"></param>
        /// <param name="guidUsuarioCacheExtrato"></param>
        /// <returns></returns>
        public ConsultarConsolidadoDebitosEDesagendamentoRetorno ConsultarConsolidadoDebitosEDesagendamentoPesquisa(out StatusRetorno statusRetorno, ConsultarConsolidadoDebitosEDesagendamentoEnvio envio)
        {
            using (Logger Log = Logger.IniciarLog("Relatório de débitos e desagendamentos - Consolidado [WACA150]"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });
                    ConsultarConsolidadoDebitosEDesagendamentoEnvioDTO envioDTO = TradutorConsultarConsolidadoDebitosEDesagendamento.TraduzirEnvioConsultarConsolidadoDebitosEDesagendamento(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO});
                    ConsultarConsolidadoDebitosEDesagendamentoRetornoDTO retornoDTO = ConsultarConsolidadoDebitosEDesagendamentoBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);
                    Log.GravarLog(EventoLog.RetornoAgente, new { retornoDTO, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);
                    Log.GravarLog(EventoLog.FimServico, new { retornoDTO, statusRetorno });


                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        return null;
                    }
                    ConsultarConsolidadoDebitosEDesagendamentoRetorno result = TradutorConsultarConsolidadoDebitosEDesagendamento.TraduzirRetornoConsultarConsolidadoDebitosEDesagendamento(retornoDTO);

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

        #region WACA151 - Relatório de débitos e desagendamentos - Detalhe.
        /// <summary>
        /// WACA151 - Relatório de débitos e desagendamentos - Detalhe.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <param name="numeroPagina"></param>
        /// <param name="quantidadeRegistrosPorPagina"></param>
        /// <param name="guidPesquisa"></param>
        /// <param name="guidUsuarioCacheExtrato"></param>
        /// <returns></returns>
        public ConsultarDetalhamentoDebitosRetorno ConsultarDetalhamentoDebitosPesquisa(out StatusRetorno statusRetorno, ConsultarDetalhamentoDebitosEnvio envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {

            using (Logger Log = Logger.IniciarLog("Relatório de débitos e desagendamentos - Detalhe [WACA151]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarDetalhamentoDebitosEnvioDTO envioDTO = TradutorConsultarDetalhamentoDebitos.TraduzirEnvioConsultarDetalhamentoDebitos(envio);

                    StatusRetornoDTO statusRetornoDTO;

                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato });
                    RetornoPesquisaComTotalizadorDTO<ConsultarDetalhamentoDebitosDetalheRetornoDTO, ConsultarDetalhamentoDebitosTotaisRetornoDTO> retornoPesquisa = ConsultarDetalhamentoDebitosBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato);
                    Log.GravarLog(EventoLog.RetornoAgente, new { retornoPesquisa, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { retornoPesquisa, statusRetorno });
                        return null;
                    }

                    ConsultarDetalhamentoDebitosRetorno result = TradutorConsultarDetalhamentoDebitos.TraduzirRetornoConsultarDetalhamentoDebitos(retornoPesquisa);


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

        #region WACA152 - Relatório de débitos e desagendamentos - Motivo débito.
        public ConsultarMotivoDebitoRetorno ConsultarMotivoDebito(out StatusRetorno statusRetorno, ConsultarMotivoDebitoEnvio envio)
        {
            using (Logger Log = Logger.IniciarLog("Relatório de débitos e desagendamentos - Motivo débito [WACA152]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarMotivoDebitoEnvioDTO envioDTO = TradutorConsultarMotivoDebito.TraduzirEnvioConsultarMotivoDebito(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO});

                    ConsultarMotivoDebitoRetornoDTO dto = ConsultarMotivoDebitoBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);
                    Log.GravarLog(EventoLog.RetornoAgente, new { dto , statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);


                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { dto, statusRetorno });
                        return null;
                    }

                    ConsultarMotivoDebitoRetorno result = TradutorConsultarMotivoDebito.TraduzirRetornoConsultarMotivoDebito(dto);

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
