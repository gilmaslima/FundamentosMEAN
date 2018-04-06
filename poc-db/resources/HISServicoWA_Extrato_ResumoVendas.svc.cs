using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Comum;
using DTO = Redecard.PN.Extrato.Modelo.ResumoVendas;
using BLL = Redecard.PN.Extrato.Negocio.ResumoVendasBLL;
using Redecard.PN.Extrato.Servicos.ResumoVenda;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Extrato.Modelo;
using System.Threading.Tasks;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>
    /// Serviço para acesso ao componente WA utilizado no módulo Extrato - Resumo de Vendas.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// WACA668	WA668	IS10	ConsultarDebitoCVsAceitos
    /// - Book WACA704 / Programa WA704 / TranID IS14 / Método ConsultarCreditoAjustes<br/>
    /// - Book WACA706 / Programa WA706 / TranID IS16 / Método ConsultarCreditoCVsAceitos<br/>
    /// - Book WACA748 / Programa WA748 / TranID ISD4 / Método ConsultarDebitoConstrucardAjustes<br/>
    /// - Book WACA797 / Programa WA797 / TranID IS35 / Método ConsultarConstrucardCVsAceitos<br/>
    /// - Book BKWA2430 / Programa WA243 / TranID ISIC / Método ConsultarRecargaCelularResumo<br/>
    /// - Book BKWA2440 / Programa WA244 / TranID ISID / Método ConsultarRecargaCelularVencimentos<br/>
    /// - Book BKWA2450 / Programa WA245 / TranID ISIE / Método ConsultarRecargaCelularAjustesCredito<br/>
    /// - Book BKWA2450 / Programa WA245 / TranID ISIE / Método ConsultarRecargaCelularAjustesDebito<br/>
    /// - Book BKWA2460 / Programa WA246 / TranID ISIF / Método ConsultarRecargaCelularComprovantes
    /// </remarks>
    public class HISServicoWA_Extrato_ResumoVendas : ServicoBase, IHISServicoWA_Extrato_ResumoVendas
    {
        #region [ Resumo de Vendas - Débito - CVs Aceitos - WACA668 / WA668 / IS10 ]

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Débito.<br/>
        /// Efetua a consulta dos CVs Aceitos.<br/>
        /// - Book WACA668 / Programa WA668 / TranID IS10
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA668 / Programa WA668 / TranID IS10
        /// </remarks>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroRV">Número do Resumo de Vendas</param>
        /// <param name="dataApresentacao">Data de apresentação do Resumo de Vendas</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>CVs de Débito aceitos</returns>
        public List<DebitoCVsAceitos> ConsultarDebitoCVsAceitos(
            Int32 pv,
            Int32 numeroRV,
            DateTime dataApresentacao,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Débito - CVs Aceitos - WACA668"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { pv, numeroRV, dataApresentacao });
                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    StatusRetornoDTO statusDTO;

                    //Executa consulta no mainframe
                    var dadosConsulta = BLL.Instancia.ConsultarDebitoCVsAceitos(
                        pv,
                        numeroRV,
                        dataApresentacao,
                        out statusDTO);

                    status = StatusRetorno.FromDTO(statusDTO);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                    {                        
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return null;
                    }

                    //Conversão de DTO para modelo de serviço
                    var dados = DebitoCVsAceitos.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #region [ Resumo de Vendas - Débito/Construcard - WACA748 / WA748 / ISD4 ]

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Débito/Construcard.<br/>
        /// Efetua a consulta dos Ajustes.<br/>
        /// - Book WACA748 / Programa WA748 / TranID ISD4
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA748 / Programa WA748 / TranID ISD4
        /// </remarks>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="dataApresentacao">Data de apresentação do Resumo de Vendas</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Ajustes Débito/Construcard</returns>
        public List<DebitoCDCAjuste> ConsultarDebitoConstrucardAjustes(
            Int32 pv,
            Int32 resumo,
            DateTime dataApresentacao,
            String tipoResumo,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Débito/Construcard Ajustes - WACA748"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { pv, resumo, dataApresentacao, tipoResumo });
                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    StatusRetornoDTO statusDTO;

                    //Executa consulta no mainframe
                    var dadosConsulta = BLL.Instancia.ConsultarDebitoConstrucardAjustes(
                        pv,
                        resumo,
                        dataApresentacao,
                        tipoResumo,                       
                        out statusDTO);

                    status = StatusRetorno.FromDTO(statusDTO);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return null;
                    }

                    //Conversão de DTO para modelo de serviço
                    var dados = DebitoCDCAjuste.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #region [ Resumo de Vendas - Crédito - CVs Aceitos - WACA706 / WA706 / IS16 ]

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Crédito.<br/>
        /// Efetua a consulta dos CVs Aceitos.<br/>
        /// - Book WACA706 / Programa WA706 / TranID IS16
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA706 / Programa WA706 / TranID IS16
        /// </remarks>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroRV">Número do Resumo de Vendas</param>
        /// <param name="numeroMes">Número do mês</param>
        /// <param name="dataApresentacao">Data de apresentação do Resumo de Vendas</param>
        /// <param name="timestamp">Timestamps do Resumo de Vendas</param>
        /// <param name="tipoResumoVenda">Tipo do Resumo de Vendas</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>CVs de Crédito aceitos</returns>
        public List<CreditoCVsAceitos> ConsultarCreditoCVsAceitos(
            Int32 pv,
            Int32 numeroRV,
            Int16 numeroMes,
            DateTime dataApresentacao,
            String timestamp,
            String tipoResumoVenda,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Crédito - CVs Aceitos - WACA706"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { pv, numeroRV, numeroMes, dataApresentacao, timestamp, tipoResumoVenda });
                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    StatusRetornoDTO statusDTO;

                    //Executa consulta no mainframe
                    var dadosConsulta = BLL.Instancia.ConsultarCreditoCVsAceitos(
                        pv,
                        numeroRV,
                        numeroMes,
                        dataApresentacao,
                        timestamp,
                        tipoResumoVenda,
                        out statusDTO);

                    status = StatusRetorno.FromDTO(statusDTO);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return null;
                    }

                    //Conversão de DTO para modelo de serviço
                    var dados = CreditoCVsAceitos.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #region [ Resumo de Vendas - Crédito - Ajustes - WACA704 / WA704 / IS14 ]

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Crédito.<br/>
        /// Efetua a consulta dos Ajustes<br/>
        /// - Book WACA704 / Programa WA704 / TranID IS14
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA704 / Programa WA704 / TranID IS14
        /// </remarks>
        /// <param name="timestamp">Timestamps do Resumo de Vendas</param>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroRV">Número do Resumo de Vendas</param>
        /// <param name="dataApresentacao">Data de apresentação do Resumo de Vendas</param>        
        /// <param name="tipoRV">Tipo do Resumo de Vendas</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Ajustes Crédito</returns>
        public List<CreditoAjustes> ConsultarCreditoAjustes(
            String timestamp,
            Int32 pv,
            Int16 tipoRV,
            Int32 numeroRV,
            DateTime dataApresentacao,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Crédito - Ajustes - WACA704"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { timestamp, pv, tipoRV, numeroRV, dataApresentacao });
                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    StatusRetornoDTO statusDTO;

                    //Executa consulta no mainframe
                    var dadosConsulta = BLL.Instancia.ConsultarCreditoAjustes(
                        timestamp,
                        pv,
                        tipoRV,
                        numeroRV,
                        dataApresentacao,                        
                        out statusDTO);

                    status = StatusRetorno.FromDTO(statusDTO);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return null;
                    }

                    //Conversão de DTO para modelo de serviço
                    var dados = CreditoAjustes.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #region [ Resumo de Vendas - Construcard - WACA797 / WA797 / IS35 ]

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Construcard.<br/>
        /// Efetua a consulta dos CVs Aceitos.<br/>
        /// - Book WACA797 / Programa WA797 / TranID IS35
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através dos seguintes Books:<br/>
        /// - Book WACA797 / Programa WA797 / TranID IS35
        /// </remarks>
        /// <param name="pv">Código do estabelecimento</param>
        /// <param name="numeroRV">Número do Resumo de Vendas</param>        
        /// <param name="dataApresentacao">Data de apresentação do Resumo de Vendas</param>        
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>CVs de Construcard aceitos</returns>
        public List<ConstrucardCVsAceitos> ConsultarConstrucardCVsAceitos(
            Int32 pv,
            Int32 numeroRV,
            DateTime dataApresentacao,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Construcard - CVs Aceitos - WACA797"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { pv, numeroRV, dataApresentacao });
                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    StatusRetornoDTO statusDTO;

                    //Executa consulta no mainframe
                    var dadosConsulta = BLL.Instancia.ConsultarConstrucardCVsAceitos(
                        pv,
                        numeroRV,
                        dataApresentacao,
                        out statusDTO);

                    status = StatusRetorno.FromDTO(statusDTO);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return null;
                    }

                    //Conversão de DTO para modelo de serviço
                    var dados = ConstrucardCVsAceitos.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #region [ Resumo de Vendas - Recarga de Celular - Resumo - BKWA2430 / WA243 / ISIC ]

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Recarga de Celular.<br/>
        /// Efetua a consulta do Resumo.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do Book:<br/>
        /// - Book BKWA2430 / Programa WA243 / TranID ISIC
        /// </remarks>
        /// <param name="numeroPv">Número do estabelecimento</param>
        /// <param name="numeroRv">Número do resumo de vendas</param>
        /// <param name="dataPagamento">Data do pagamento</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <param name="origemPesquisa">Origem/Tipo da pesquisa</param>
        /// <returns>Resumo do Resumo de Vendas</returns>
        public RecargaCelularResumo ConsultarRecargaCelularResumo(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            RecargaCelularResumoOrigem origemPesquisa,
            out StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("Resumo de Vendas - Recarga Celular - Resumo - BKWA2430"))
            {
                log.GravarLog(EventoLog.InicioServico, new { numeroPv, numeroRv, dataPagamento, origemPesquisa });

                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    StatusRetornoDTO statusDTO;
                    var origemPesquisaRecarga = (DTO.RecargaCelularResumoOrigem)origemPesquisa;

                    //Executa consulta no mainframe
                    var dadosConsulta = BLL.Instancia.ConsultarRecargaCelularResumo(
                        numeroPv,
                        numeroRv,
                        dataPagamento,
                        origemPesquisaRecarga,
                        out statusDTO);

                    status = StatusRetorno.FromDTO(statusDTO);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                    {
                        log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return null;
                    }

                    //Conversão de DTO para modelo de serviço
                    var dados = RecargaCelularResumo.FromDTO(dadosConsulta);

                    log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(ex.Codigo, ex.Fonte), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }
        }

        #endregion

        #region [ Resumo de Vendas - Recarga de Celular - Vencimentos - BKWA2440 / WA244 / ISID ]

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Recarga de Celular.<br/>
        /// Efetua a consulta do Vencimento.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do Book:<br/>
        /// - Book BKWA2440 / Programa WA244 / TranID ISID
        /// </remarks>
        /// <param name="numeroPv">Número do estabelecimento</param>
        /// <param name="numeroRv">Número do resumo de vendas</param>
        /// <param name="dataPagamento">Data do pagamento</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Vencimento</returns>
        public RecargaCelularVencimento ConsultarRecargaCelularVencimentos(
           Int32 numeroPv,
           Int32 numeroRv,
           DateTime dataPagamento,
           out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Recarga Celular - Vencimentos - BKWA2440"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPv, numeroRv, dataPagamento });
                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    StatusRetornoDTO statusDTO;

                    //Executa consulta no mainframe
                    var dadosConsulta = BLL.Instancia.ConsultarRecargaCelularVencimentos(
                        numeroPv,
                        numeroRv,
                        dataPagamento,
                        out statusDTO);

                    status = StatusRetorno.FromDTO(statusDTO);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return null;
                    }

                    //Conversão de DTO para modelo de serviço
                    var dados = RecargaCelularVencimento.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #region [ Resumo de Vendas - Recarga de Celular - Ajustes - BKWA2450 / WA245 / ISIE ]

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Recarga de Celular.<br/>
        /// Efetua a consulta dos Ajustes Crédito.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do Book:<br/>
        /// - Book BKWA2450 / Programa WA245 / TranID ISIE
        /// </remarks>
        /// <param name="numeroPv">Número do estabelecimento</param>
        /// <param name="numeroRv">Número do resumo de vendas</param>
        /// <param name="dataPagamento">Data do pagamento</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Ajustes Crédito</returns>
        public List<RecargaCelularAjusteCredito> ConsultarRecargaCelularAjustesCredito(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Recarga Celular - Ajustes Crédito - BKWA2450"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPv, numeroRv, dataPagamento });
                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    StatusRetornoDTO statusDTO;

                    //Executa consulta no mainframe
                    var dadosConsulta = BLL.Instancia.ConsultarRecargaCelularAjustesCredito(
                        numeroPv,
                        numeroRv,
                        dataPagamento,
                        out statusDTO);

                    status = StatusRetorno.FromDTO(statusDTO);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return null;
                    }

                    //Conversão de DTO para modelo de serviço
                    var dados = RecargaCelularAjusteCredito.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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
        /// Consulta utilizada em Resumo de Vendas - Recarga de Celular.<br/>
        /// Efetua a consulta dos Ajustes Débito.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do Book:<br/>
        /// - Book BKWA2450 / Programa WA245 / TranID ISIE
        /// </remarks>
        /// <param name="numeroPv">Número do estabelecimento</param>
        /// <param name="numeroRv">Número do resumo de vendas</param>
        /// <param name="dataPagamento">Data do pagamento</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Ajustes Débito</returns>
        public List<RecargaCelularAjusteDebito> ConsultarRecargaCelularAjustesDebito(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Recarga Celular - Ajustes Débito - BKWA2450"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPv, numeroRv, dataPagamento });
                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    StatusRetornoDTO statusDTO;

                    //Executa consulta no mainframe
                    var dadosConsulta = BLL.Instancia.ConsultarRecargaCelularAjustesDebito(
                        numeroPv,
                        numeroRv,
                        dataPagamento,
                        out statusDTO);

                    status = StatusRetorno.FromDTO(statusDTO);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return null;
                    }

                    //Conversão de DTO para modelo de serviço
                    var dados = RecargaCelularAjusteDebito.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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

        #region [ Resumo de Vendas - Recarga de Celular - Comprovantes Realizados - BKWA2460 / WA246 / ISIF ]

        /// <summary>
        /// Consulta utilizada em Resumo de Vendas - Recarga de Celular.<br/>
        /// Efetua a consulta dos Comprovantes Realizados.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do Book:<br/>
        /// - Book BKWA2460 / Programa WA246 / TranID ISIF
        /// </remarks>
        /// <param name="numeroPv">Número do estabelecimento</param>
        /// <param name="numeroRv">Número do resumo de vendas</param>
        /// <param name="dataPagamento">Data do pagamento</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Comprovantes Realizados</returns>
        public List<RecargaCelularComprovante> ConsultarRecargaCelularComprovantes(
            Int32 numeroPv,
            Int32 numeroRv,
            DateTime dataPagamento,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de Vendas - Recarga Celular - Comprovantes - BKWA2460"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPv, numeroRv, dataPagamento });
                try
                {
                    //Instancia variáveis auxiliares para execução da pesquisa e verificação de cache
                    StatusRetornoDTO statusDTO;

                    //Executa consulta no mainframe
                    var dadosConsulta = BLL.Instancia.ConsultarRecargaCelularComprovantes(
                        numeroPv,
                        numeroRv,
                        dataPagamento,
                        out statusDTO);

                    status = StatusRetorno.FromDTO(statusDTO);

                    //Em caso de código de retorno != 0, sai do método, sem sucesso
                    if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                    {
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return null;
                    }

                    //Conversão de DTO para modelo de serviço
                    var dados = RecargaCelularComprovante.FromDTO(dadosConsulta);

                    Log.GravarLog(EventoLog.FimServico, new { status, dados });

                    return dados;
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
