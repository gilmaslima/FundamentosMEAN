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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RelatorioCreditoSuspensosRetidosPenhorados" in code, svc and config file together.
    public class RelatorioCreditoSuspensosRetidosPenhorados : Redecard.PN.Comum.ServicoBase, IRelatorioCreditoSuspensosRetidosPenhorados
    {
        #region WACA1111 - Relatório de créditos suspensos, retidos e penhorados - Créditos/Débitos suspensos - Cartões de crédito.
        public ConsultarSuspensaoRetorno ConsultarSuspensaoPesquisaCredito(out StatusRetorno statusRetorno, ConsultarSuspensaoEnvio envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {

            using (Logger Log = Logger.IniciarLog("Relatório de créditos suspensos, retidos e penhorados - Créditos/Débitos suspensos - Cartões de crédito [WACA1111]"))
            {

                try
                {
                Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarSuspensaoEnvioDTO envioDTO = TradutorConsultarSuspensao.TraduzirEnvioConsultarSuspensao(envio);

                    StatusRetornoDTO statusRetornoDTO;

                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato });

                    RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarSuspensaoTotaisRetornoDTO> retornoPesquisa = ConsultarSuspensaoCreditoBLL.Instance.PesquisarDT(out statusRetornoDTO, envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato);
                    Log.GravarLog(EventoLog.RetornoAgente, new { retornoPesquisa, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);
                    
                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { retornoPesquisa, statusRetorno });

                        return null;
                    }

                    ConsultarSuspensaoRetorno result = TradutorConsultarSuspensao.TraduzirRetornoConsultarSuspensao(retornoPesquisa);

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

        #region WACA1111 - Relatório de créditos suspensos, retidos e penhorados - Créditos/Débitos suspensos - Cartões de débito.
        public ConsultarSuspensaoRetorno ConsultarSuspensaoPesquisaDebito(out StatusRetorno statusRetorno, ConsultarSuspensaoEnvio envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {

            using (Logger Log = Logger.IniciarLog("Relatório de créditos suspensos, retidos e penhorados - Créditos/Débitos suspensos - Cartões de débito [WACA1111]"))
            {


                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });
                    ConsultarSuspensaoEnvioDTO envioDTO = TradutorConsultarSuspensao.TraduzirEnvioConsultarSuspensao(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato });
                    RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarSuspensaoTotaisRetornoDTO> retornoPesquisa = ConsultarSuspensaoDebitoBLL.Instance.PesquisarDT(out statusRetornoDTO, envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato);
                    Log.GravarLog(EventoLog.RetornoAgente, new { retornoPesquisa, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);
                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { retornoPesquisa, statusRetorno });
                        return null;
                    }

                    ConsultarSuspensaoRetorno result = TradutorConsultarSuspensao.TraduzirRetornoConsultarSuspensao(retornoPesquisa);

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

        #region WACA1112 - Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados.
        /// <summary>
        /// WACA1112 - Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <param name="quantidadeRegistrosPorPagina"></param>
        /// <param name="guidPesquisa"></param>
        /// <param name="guidUsuarioCacheExtrato"></param>
        /// <returns></returns>
        public ConsultarPenhoraRetorno ConsultarPenhoraPesquisa(out StatusRetorno statusRetorno, ConsultarPenhoraEnvio envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {

            using (Logger Log = Logger.IniciarLog("Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados [WACA1112]"))
            {


                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarPenhoraEnvioDTO envioDTO = TradutorConsultarPenhora.TraduzirEnvioConsultarPenhora(envio);

                    StatusRetornoDTO statusRetornoDTO;

                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato });
                    RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarPenhoraTotaisRetornoDTO> retornoPesquisa = ConsultarPenhoraBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato);
                    Log.GravarLog(EventoLog.RetornoAgente, new { retornoPesquisa, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { retornoPesquisa, statusRetorno });
                        return null;
                    }

                    ConsultarPenhoraRetorno result = TradutorConsultarPenhora.TraduzirRetornoConsultarPenhora(retornoPesquisa);


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

        #region WACA1113 - Relatório de créditos suspensos, retidos e penhorados - Créditos retidos.
        /// <summary>
        /// WACA1113 - Relatório de créditos suspensos, retidos e penhorados - Créditos retidos.
        /// </summary>
        /// <param name="statusRetorno"></param>
        /// <param name="envio"></param>
        /// <param name="quantidadeRegistrosPorPagina"></param>
        /// <param name="guidPesquisa"></param>
        /// <param name="guidUsuarioCacheExtrato"></param>
        /// <returns></returns>
        public ConsultarRetencaoRetorno ConsultarRetencaoPesquisa(out StatusRetorno statusRetorno, ConsultarRetencaoEnvio envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {

            using (Logger Log = Logger.IniciarLog("Relatório de créditos suspensos, retidos e penhorados - Créditos retidos [WACA1113]"))
            {
                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    ConsultarRetencaoEnvioDTO envioDTO = TradutorConsultarRetencao.TraduzirEnvioConsultarRetencao(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato });
                    RetornoPesquisaComTotalizadorDTO<BasicDTO, ConsultarRetencaoTotaisRetornoDTO> retornoPesquisa = ConsultarRetencaoBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato);
                    Log.GravarLog(EventoLog.RetornoAgente, new { retornoPesquisa, statusRetornoDTO });

                    statusRetorno = new StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                    if (statusRetorno.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { retornoPesquisa, statusRetorno });
                        return null;
                    }

                    ConsultarRetencaoRetorno result = TradutorConsultarRetencao.TraduzirRetornoConsultarRetencao(retornoPesquisa);


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
