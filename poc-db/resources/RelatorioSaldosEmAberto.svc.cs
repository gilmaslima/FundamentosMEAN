using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Tradutor;
using Redecard.PN.Extrato.Negocio.RelatorioSaldosEmAberto;
using Redecard.PN.Extrato.Servicos.Modelo;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço de consulta de saldos em aberto
    /// </summary>
    public class RelatorioSaldosEmAberto : Redecard.PN.Comum.ServicoBase, IRelatorioSaldosEmAberto
    {
       /// <summary>
       /// Consulta os períodos solicitados disponíveis para consulta
       /// </summary>
       /// <param name="statusRetorno">Classe status retorno que retorna o código e mensagem de retorno da consulta</param>
       /// <param name="envio">Dados para a consulta</param>
       /// <param name="numeroPagina">Número da página consultada</param>
       /// <param name="quantidadeRegistrosPorPagina">Quantidade de registros por página</param>
       /// <param name="guidPesquisa">GUID da pesquisa</param>
       /// <param name="guidUsuarioCacheExtrato">GUID do cache do usuário</param>
       /// <returns>Lista de dperíodos disponíveis</returns>
        public List<Modelo.PeriodoDisponivel> ConsultarPeriodosDisponiveis(out Modelo.StatusRetorno statusRetorno, Modelo.DadosConsultaSaldosEmAberto envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Saldos em aberto"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    DadosConsultaSaldosEmAbertoDTO envioDTO = TradutorSaldosEmAberto.TraduzirEnvioConsultarSaldosEmAberto(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato });

                    RetornoPesquisaSemTotalizadorDTO<PeriodoDisponivelDTO> retornoPesquisa = ConsultarPeriodosDisponiveisBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato);


                    statusRetorno = new Modelo.StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                    if (statusRetorno.CodigoRetorno != 0)
                    {

                        Log.GravarLog(EventoLog.FimServico, new { retornoPesquisa, statusRetorno });
                        return null;
                    }

                    Log.GravarLog(EventoLog.RetornoAgente, new { retornoPesquisa, statusRetornoDTO });

                    List<Modelo.PeriodoDisponivel> result = TradutorSaldosEmAberto.TraduzirRetornoConsultarPeriodosDisponiveis(retornoPesquisa);

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

        /// <summary>
        /// Consulta o saldos em aberto online- para período de até 1(hum) ano
        /// </summary>
        /// <param name="statusRetorno">Classe status retorno que retorna o código e mensagem de retorno da consulta</param>
        /// <param name="envio">Dados para a consulta</param>
        /// <param name="numeroPagina">Número da página consultada</param>
        /// <param name="quantidadeRegistrosPorPagina">Quantidade de registros por página</param>
        /// <param name="guidPesquisa">GUID da pesquisa</param>
        /// <param name="guidUsuarioCacheExtrato">GUID do cache do usuário</param>
        /// <returns>Lista de saldos em aberto</returns>
        public RetornoConsultaSaldosEmAberto ConsultarSaldosEmAbertoOnline(out Modelo.StatusRetorno statusRetorno, Modelo.DadosConsultaSaldosEmAberto envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Saldos em aberto"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    DadosConsultaSaldosEmAbertoDTO envioDTO = TradutorSaldosEmAberto.TraduzirEnvioConsultarSaldosEmAberto(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato });

                    RetornoPesquisaComTotalizadorDTO<BasicDTO, TotalizadorPorBandeiraSaldosEmAbertoDTO> retornoPesquisa = ConsultarSaldosemAbertoOnlineBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato);


                    statusRetorno = new Modelo.StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                    if (statusRetorno.CodigoRetorno != 0)
                    {

                        Log.GravarLog(EventoLog.FimServico, new { retornoPesquisa, statusRetorno });
                        return null;
                    }

                    Log.GravarLog(EventoLog.RetornoAgente, new { retornoPesquisa, statusRetornoDTO });
                    RetornoConsultaSaldosEmAberto retornoConsulta = TradutorSaldosEmAberto.TraduzirRetornoConsultaSaldosEmAberto(retornoPesquisa);

                    Log.GravarLog(EventoLog.FimServico, new { retornoConsulta });

                    return retornoConsulta;
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
        /// <summary>
        /// Consulta de saldos em aberto VSAM - para períodos disponíveis e maiores que 1(hum) ano
        /// </summary>
        /// <param name="statusRetorno">Classe status retorno que retorna o código e mensagem de retorno da consulta</param>
        /// <param name="envio">Dados para a consulta</param>
        /// <param name="numeroPagina">Número da página consultada</param>
        /// <param name="quantidadeRegistrosPorPagina">Quantidade de registros por página</param>
        /// <param name="guidPesquisa">GUID da pesquisa</param>
        /// <param name="guidUsuarioCacheExtrato">GUID do cache do usuário</param>
        /// <returns>Lista de saldos em aberto</returns>
        public RetornoConsultaSaldosEmAberto ConsultarSaldosEmAbertoVSAM(out Modelo.StatusRetorno statusRetorno, Modelo.DadosConsultaSaldosEmAberto envio, int numeroPagina, int quantidadeRegistrosPorPagina, Guid guidPesquisa, Guid guidUsuarioCacheExtrato)
        {
            using (Logger Log = Logger.IniciarLog("Consultar Saldos em aberto VSAM"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    DadosConsultaSaldosEmAbertoDTO envioDTO = TradutorSaldosEmAberto.TraduzirEnvioConsultarSaldosEmAberto(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato });

                    RetornoPesquisaComTotalizadorDTO<BasicDTO, TotalizadorPorBandeiraSaldosEmAbertoDTO> retornoPesquisa = ConsultarSaldosemAbertoVSAMBLL.Instance.Pesquisar(out statusRetornoDTO, envioDTO, numeroPagina, quantidadeRegistrosPorPagina, guidPesquisa, guidUsuarioCacheExtrato);


                    statusRetorno = new Modelo.StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                    if (statusRetorno.CodigoRetorno != 0)
                    {

                        Log.GravarLog(EventoLog.FimServico, new { retornoPesquisa, statusRetorno });
                        return null;
                    }

                    Log.GravarLog(EventoLog.RetornoAgente, new { retornoPesquisa, statusRetornoDTO });

                    RetornoConsultaSaldosEmAberto retornoConsulta = TradutorSaldosEmAberto.TraduzirRetornoConsultaSaldosEmAberto(retornoPesquisa);

                    Log.GravarLog(EventoLog.FimServico, new { retornoConsulta });

                    return retornoConsulta;
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

        /// <summary>
        /// Inclui a solicitação de consulta de períodos maiores que 1(gum) ano
        /// </summary>
        /// <param name="statusRetorno">Classe status retorno que retorna o código e mensagem de retorno da consulta</param>
        /// <param name="envio">Dados para a consulta</param>
        /// <returns>código de retorno da consulta</returns>
        public Int16 IncluirSolicitacao(out Modelo.StatusRetorno statusRetorno, Modelo.DadosConsultaSaldosEmAberto envio)
        {
            using (Logger Log = Logger.IniciarLog("Incluir Solictação"))
            {

                try
                {
                    Log.GravarLog(EventoLog.InicioServico, new { envio });

                    DadosConsultaSaldosEmAbertoDTO envioDTO = TradutorSaldosEmAberto.TraduzirEnvioConsultarSaldosEmAberto(envio);

                    StatusRetornoDTO statusRetornoDTO;
                    Log.GravarLog(EventoLog.ChamadaAgente, new { envioDTO});

                    Int16 codigoRetorno = InserirSolicitacaoBLL.InserirSolicitacao(out statusRetornoDTO, envioDTO);

                    Log.GravarLog(EventoLog.RetornoAgente, new { codigoRetorno, statusRetornoDTO });

                    statusRetorno = new Modelo.StatusRetorno(statusRetornoDTO.CodigoRetorno, statusRetornoDTO.MensagemRetorno, statusRetornoDTO.Fonte);

                    if (statusRetorno.CodigoRetorno != 0)
                    {

                        Log.GravarLog(EventoLog.FimServico, new { codigoRetorno, statusRetorno });
                        return codigoRetorno;
                    }

                    Log.GravarLog(EventoLog.FimServico, new { codigoRetorno });

                    return codigoRetorno;
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
    }
}
