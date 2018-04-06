using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Redecard.PN.Extrato.Negocio;
using AutoMapper;
using Redecard.PN.Extrato.Servicos.ConsultaTransacao;
using DTO = Redecard.PN.Extrato.Modelo.ConsultaTransacao;
using BLL = Redecard.PN.Extrato.Negocio.ConsultaTransacaoBLL;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Servicos.Modelo;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.Servicos
{
    /// <summary>    
    /// Serviço para acesso ao componente ME utilizado no módulo Extrato - Consulta por Transação.
    /// </summary>
    /// <remarks>
    /// Books utilizados no serviço:<br/>
    /// - Book MEC084CO	/ Programa MEC084 / TranID IS89 / Método ConsultarDebito<br/>
    /// - Book MEC119CO	/ Programa MEC119 / TranID IS96 / Método ConsultarCredito<br/>
    /// - Book MEC323CO	/ Programa MEC323 / TranID IS99 / Método ConsultarCreditoTID<br/>
    /// - Book MEC324CO	/ Programa MEC324 / TranID IS88 / Método ConsultarDebitoTID
    /// </remarks>
    public class HISServicoME_Extrato_ConsultaTransacao : ServicoBase, IHISServicoME_Extrato_ConsultaTransacao
    {
        #region [ Consulta por Transação Débito por Cartão/NSU - MEC084CO / MEC084 / IS89 ]

        /// <summary>
        /// Consulta de Transação de Débito, através do Número do Cartão ou NSU.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book: <br/>
        /// - Book MEC084CO / Programa MEC084 / TranID IS89
        /// </remarks>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="numeroCartao">Número do cartão</param>
        /// <param name="nsuAcquirer">Número do NSU</param>
        /// <param name="status">Saída: código de retorno/mensagem da consulta efetuada</param>
        /// <returns>Lista contendo as transações de débito encontradas</returns>
        public List<Debito> ConsultarDebito(
            Int32 numeroPV,
            DateTime dataInicial,
            DateTime dataFinal,
            String numeroCartao,
            Int64 nsuAcquirer,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Por Transação - Débito - MEC084CO"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPV, dataInicial, dataFinal, numeroCartao, nsuAcquirer });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa
                    var _rechamada = new Dictionary<String, Object>();
                    var _indicadorRechamada = true;
                    var _statusDTO = default(StatusRetornoDTO);
                    var _retorno = new List<DTO.Debito>();

                    //Enquanto for necessário executar pesquisa
                    while (_indicadorRechamada)
                    {
                        var dadosConsulta = BLL.Instancia.ConsultarDebito(
                            numeroPV,
                            dataInicial,
                            dataFinal,
                            numeroCartao,
                            nsuAcquirer,
                            ref _rechamada,
                            out _indicadorRechamada,
                            out _statusDTO);

                        if (_statusDTO == null || _statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(_statusDTO);
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                            return null;
                        }

                        _retorno.AddRange(dadosConsulta);
                    }

                    //Preparação dos dados de retorno
                    var dados = Debito.FromDTO(_retorno);
                    status = new StatusRetorno();

                    Log.GravarLog(EventoLog.FimServico, new { dados, status });

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

        #region [ Consulta por Transação Crédito por Cartão/NSU - MEC119CO / MEC119 / IS96 ]

        /// <summary>
        /// Consulta de Transação de Crédito, através do Número do Cartão ou NSU.
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book MEC119CO / Programa MEC119 / TranID IS96
        /// </remarks>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="dataInicial">Data inicial do período da pesquisa</param>
        /// <param name="dataFinal">Data final do período da pesquisa</param>
        /// <param name="numeroCartao">Número do cartão</param>
        /// <param name="nsu">Número do NSU</param>
        /// <param name="status">Saída: código de retorno/mensagem da consulta efetuada</param>
        /// <returns>Lista contendo as transações de crédito encontradas</returns>
        public List<Credito> ConsultarCredito(      
            Int32 numeroPV,
            DateTime dataInicial,
            DateTime dataFinal,
            String numeroCartao,
            Int64 nsu,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Por Transação - Crédito - MEC119CO"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { numeroPV, dataInicial, dataFinal, numeroCartao, nsu });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa
                    var rechamada = new Dictionary<String, Object>();
                    var indicadorRechamada = true;
                    var statusDTO = default(StatusRetornoDTO);
                    var retorno = new List<DTO.Credito>();

                    //Enquanto for necessário executar pesquisa
                    while (indicadorRechamada)
                    {
                        var dadosConsulta = BLL.Instancia.ConsultarCredito(
                            numeroPV,
                            dataInicial,
                            dataFinal,
                            numeroCartao,
                            nsu,
                            ref rechamada,
                            out indicadorRechamada,
                            out statusDTO);

                        if (statusDTO == null || statusDTO.CodigoRetorno != 0)
                        {
                            status = StatusRetorno.FromDTO(statusDTO);
                            Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                            return null;
                        }

                        retorno.AddRange(dadosConsulta);
                    }

                    //Preparação dos dados de retorno
                    var dados = Credito.FromDTO(retorno);
                    status = new StatusRetorno();

                    Log.GravarLog(EventoLog.FimServico, new { dados, status });

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

        #region [ Consulta por Transação Crédito por TID - MEC323CO / MEC323 / TranID IS99 ]

        /// <summary>
        /// Consulta de Transação de Crédito, através do Número de Identificação DataCash (TID).
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book MEC323CO / Programa MEC323 / TranID IS99
        /// </remarks>
        /// <param name="idDataCash">ID do DataCash</param>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="status">Saída: código de retorno/mensagem da consulta efetuada</param>
        /// <returns>Transação de Crédito encontrada</returns>
        public CreditoTID ConsultarCreditoTID(
            String idDataCash,
            Int32 numeroPV,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Por Transação - Crédito TID - MEC323CO"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { idDataCash, numeroPV });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa
                    StatusRetornoDTO statusRetornoDTO;

                    //Execução da pesquisa
                    var dadosConsulta = BLL.Instancia.ConsultarCreditoTID(
                        idDataCash, 
                        numeroPV, 
                        out statusRetornoDTO);

                    if (statusRetornoDTO == null || statusRetornoDTO.CodigoRetorno != 0)
                    {
                        status = StatusRetorno.FromDTO(statusRetornoDTO);
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return null;
                    }

                    //Preparação dos dados de retorno
                    var dados = CreditoTID.FromDTO(dadosConsulta);
                    status = new StatusRetorno();

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

        #region [ Consulta por Transação Débito por TID - MEC324CO / MEC324 / IS88 ]

        /// <summary>
        /// Consulta de Transação de Débito, através do Número de Identificação DataCash (TID).
        /// </summary>
        /// <remarks>
        /// Realiza a comunicação com o mainframe através do seguinte Book:<br/>
        /// - Book MEC324CO / Programa MEC324 / TranID IS88
        /// </remarks>
        /// <param name="idDataCash">ID do DataCash</param>
        /// <param name="numeroPV">Código do estabelecimento</param>
        /// <param name="status">Saída: código de retorno/mensagem da consulta efetuada</param>
        /// <returns>Transação de Débito encontrada</returns>
        public DebitoTID ConsultarDebitoTID(
            String idDataCash,
            Int32 numeroPV,
            out StatusRetorno status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Por Transação - Débito TID - MEC324CO"))
            {
                Log.GravarLog(EventoLog.InicioServico, new { idDataCash, numeroPV });

                try
                {
                    //Instancia variáveis auxiliares para execução de pesquisa
                    StatusRetornoDTO statusRetornoDTO;

                    //Execução da pesquisa
                    var dadosConsulta = BLL.Instancia.ConsultarDebitoTID(
                        idDataCash, 
                        numeroPV,
                        out statusRetornoDTO);

                    if (statusRetornoDTO == null || statusRetornoDTO.CodigoRetorno != 0)
                    {
                        status = StatusRetorno.FromDTO(statusRetornoDTO);
                        Log.GravarLog(EventoLog.FimServico, new { status, dadosConsulta });
                        return null;
                    }

                    //Preparação dos dados de retorno
                    var dados = DebitoTID.FromDTO(dadosConsulta);
                    status = new StatusRetorno();

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
