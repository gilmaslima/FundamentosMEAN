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
    public class ResumoVendas : Redecard.PN.Comum.ServicoBase, IResumoVendas
    {
        #region WACA617 - Resumo de vendas - Cartões de débito.
        /// <summary>
        /// WACA617 - Resumo de vendas - Cartões de débito.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarPreDatadosRetorno> ConsultarPreDatados(out StatusRetorno statusRetorno, ConsultarPreDatadosEnvio envio)
        {

            using (Logger Log = Logger.IniciarLog("Resumo de vendas - Cartões de débito [WACA617]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarPreDatadosEnvioDTO envioDTO = TradutorConsultarPreDatados.TraduzirEnvioConsultarPreDatados(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO});
                    List<ConsultarPreDatadosRetornoDTO> registros = ConsultarPreDatadosBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);
                    Log.GravarLog(EventoLog.RetornoAgente, new { registros, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { registros, statusRetorno });
                        return null;
                    }

                    List<ConsultarPreDatadosRetorno> result = TradutorConsultarPreDatados.TraduzirRetornoListaConsultarPreDatados(registros);


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

        #region WACA615 - Resumo de vendas - Cartões de débito - Vencimentos.
        /// <summary>
        /// WACA615 - Resumo de vendas - Cartões de débito - Vencimentos.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarVencimentosResumoVendaRetorno> ConsultarVencimentosResumoVenda(out StatusRetorno statusRetorno, ConsultarVencimentosResumoVendaEnvio envio)
        {

            using (Logger Log = Logger.IniciarLog("Resumo de vendas - Cartões de débito - Vencimentos [WACA615]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarVencimentosResumoVendaEnvioDTO envioDTO = TradutorConsultarVencimentosResumoVenda.TraduzirEnvioConsultarVencimentosResumoVenda(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO});

                    List<ConsultarVencimentosResumoVendaRetornoDTO> registros = ConsultarVencimentosResumoVendaBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);
                    Log.GravarLog(EventoLog.RetornoAgente, new { registros, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { registros, statusRetorno });

                        return null;
                    }

                    List<ConsultarVencimentosResumoVendaRetorno> result = TradutorConsultarVencimentosResumoVenda.TraduzirRetornoListaConsultarVencimentosResumoVenda(registros);

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

        #region WACA799 - Resumo de vendas - CDC.
        /// <summary>
        /// WACA799 - Resumo de vendas - CDC.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public ConsultarTransacaoDebitoRetorno ConsultarTransacaoDebito(out StatusRetorno statusRetorno, ConsultarTransacaoDebitoEnvio envio)
        {

            using (Logger Log = Logger.IniciarLog("Resumo de vendas - CDC [WACA799]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarTransacaoDebitoEnvioDTO envioDTO = TradutorConsultarTransacaoDebito.TraduzirEnvioConsultarTransacaoDebito(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO});
                    ConsultarTransacaoDebitoRetornoDTO registro = ConsultarTransacaoDebitoBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);
                    Log.GravarLog(EventoLog.RetornoAgente, new { registro, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);


                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { registro, statusRetorno });
                        return null;
                    }

                    ConsultarTransacaoDebitoRetorno result = TradutorConsultarTransacaoDebito.TraduzirRetornoConsultarTransacaoDebito(registro);

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

        #region WACA700/WACA701 - Resumo de vendas - Cartões de crédito.
        public List<ConsultarDetalhesResumoDeVendaRetorno> ConsultarDetalhesResumoDeVenda(out StatusRetorno statusRetorno, ConsultarDetalhesResumoDeVendaEnvio envio)
        {

            using (Logger Log = Logger.IniciarLog("Resumo de vendas - Cartões de crédito [WACA700/WACA701]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarDetalhesResumoDeVendaEnvioDTO envioDTO = TradutorConsultarDetalhesResumoDeVenda.TraduzirEnvioConsultarDetalhesResumoDeVenda(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO});
                    List<ConsultarDetalhesResumoDeVendaRetornoDTO> registros = ConsultarDetalhesResumoDeVendaBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);
                    Log.GravarLog(EventoLog.RetornoAgente, new { registros, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { registros, statusRetorno });

                        return null;
                    }

                    List<ConsultarDetalhesResumoDeVendaRetorno> result = TradutorConsultarDetalhesResumoDeVenda.TraduzirRetornoListaConsultarDetalhesResumoDeVenda(registros);

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

        #region WACA703 - Resumo de vendas - Cartões de crédito - Vencimentos.
        /// <summary>
        /// WACA703 - Resumo de vendas - Cartões de crédito - Vencimentos.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarDisponibilidadeVencimentosODCRetorno> ConsultarDisponibilidadeVencimentosODC(out StatusRetorno statusRetorno, ConsultarDisponibilidadeVencimentosODCEnvio envio)
        {

            using (Logger Log = Logger.IniciarLog("Resumo de vendas - Cartões de crédito - Vencimentos [WACA703]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarDisponibilidadeVencimentosODCEnvioDTO envioDTO = TradutorConsultarDisponibilidadeVencimentosODC.TraduzirEnvioConsultarDisponibilidadeVencimentosODC(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO});
                    List<ConsultarDisponibilidadeVencimentosODCRetornoDTO> registros = ConsultarDisponibilidadeVencimentosODCBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);
                    Log.GravarLog(EventoLog.RetornoAgente, new { registros, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);


                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { registros, statusRetorno });
                        return null;
                    }

                    List<ConsultarDisponibilidadeVencimentosODCRetorno> result = TradutorConsultarDisponibilidadeVencimentosODC.TraduzirRetornoListaConsultarDisponibilidadeVencimentosODC(registros);

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

        #region WACA705 - Resumo de vendas - Cartões de crédito - CV's rejeitados.
        /// <summary>
        /// WACA705 - Resumo de vendas - Cartões de crédito - CV's rejeitados.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <returns></returns>
        public List<ConsultarRejeitadosRetorno> ConsultarRejeitados(out StatusRetorno statusRetorno, ConsultarRejeitadosEnvio envio)
        {

            using (Logger Log = Logger.IniciarLog("Resumo de vendas - Cartões de crédito - CV's rejeitado [WACA705]"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarRejeitadosEnvioDTO envioDTO = TradutorConsultarRejeitados.TraduzirEnvioConsultarRejeitados(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO});
                    List<ConsultarRejeitadosRetornoDTO> registros = ConsultarRejeitadosBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO);
                    Log.GravarLog(EventoLog.RetornoAgente, new { registros, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);
                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { registros, statusRetorno });
                        return null;
                    }

                    List<ConsultarRejeitadosRetorno> result = TradutorConsultarRejeitados.TraduzirRetornoListaConsultarRejeitados(registros);


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
