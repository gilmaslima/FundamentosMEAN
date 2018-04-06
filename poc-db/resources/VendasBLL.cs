/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Modelo.Vendas;
using AG = Redecard.PN.Extrato.Agentes.VendasAG;

namespace Redecard.PN.Extrato.Negocio
{
    /// <summary>
    /// Extrato - Relatório de Vendas
    /// </summary>
    public class VendasBLL : RegraDeNegocioBase
    {
        private static VendasBLL _Instancia;
        public static VendasBLL Instancia { get { return _Instancia ?? (_Instancia = new VendasBLL()); } }

        #region [ Relatório de Vendas - Crédito ]

        public List<Credito> ConsultarCredito(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Crédito - WACA1311"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pvs, dataInicial, dataFinal, rechamada });

                    var _retorno = AG.Instancia.ConsultarCredito(                        
                        codigoBandeira,
                        pvs,
                        dataInicial,
                        dataFinal,
                        ref rechamada,
                        out indicadorRechamada,
                        out status);

                    //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                    if (_retorno == null || _retorno.Count == 0)
                        indicadorRechamada = false;

                    Log.GravarLog(EventoLog.RetornoAgente, new { _retorno, rechamada, indicadorRechamada, status });

                    return _retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public CreditoTotalizador ConsultarCreditoTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Totalizadores - WACA1310"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pvs, dataInicial, dataFinal });

                    var _retorno = AG.Instancia.ConsultarCreditoTotalizadores(                        
                        codigoBandeira,
                        pvs,
                        dataInicial,
                        dataFinal,
                        out status);

                    Log.GravarLog(EventoLog.RetornoAgente, new { _retorno, status });

                    return _retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consultar os registros do Relatório de Vendas - Débito
        /// </summary>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial</param>
        /// <param name="dataFinal">Data final</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="tipoVenda">Tipo de venda</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="status">Status de retorno</param>
        /// <returns>Registros do Relatório de Vendas Débito</returns>
        public List<Debito> ConsultarDebito(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            Modalidade tipoVenda,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Consulta Débito - WACA1313"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, 
                        new { codigoBandeira, pvs, dataInicial, dataFinal, tipoVenda, rechamada });

                    var retorno = AG.Instancia.ConsultarDebito(                        
                        codigoBandeira,
                        pvs,
                        dataInicial,
                        dataFinal,
                        tipoVenda,
                        ref rechamada,
                        out indicadorRechamada,
                        out status);

                    //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                    if (retorno == null || retorno.Count == 0)
                        indicadorRechamada = false;

                    log.GravarLog(EventoLog.RetornoAgente, new { retorno, rechamada, indicadorRechamada, status });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta os totalizadores do Relatórios de Vendas - Débito
        /// </summary>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial da pesquisa</param>
        /// <param name="dataFinal">Data final da pesquisa</param>
        /// <param name="tipoVenda">Tipo de Venda</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Totalizador do Relatório de Vendas - Débito</returns>
        public DebitoTotalizador ConsultarDebitoTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            Modalidade tipoVenda,
            out StatusRetornoDTO status)
        {
            using (Logger log = Logger.IniciarLog("Consulta Débito - Totalizadores - WACA1312"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, 
                        new { codigoBandeira, pvs, dataInicial, dataFinal, tipoVenda });

                    var retorno = AG.Instancia.ConsultarDebitoTotalizadores(                        
                        codigoBandeira,
                        pvs,
                        dataInicial,
                        dataFinal,
                        tipoVenda,
                        out status);

                    log.GravarLog(EventoLog.RetornoAgente, new { retorno, status });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #endregion

        #region [ Relatório de Vendas - Construcard ]

        public List<Construcard> ConsultarConstrucard(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Construcard - WACA1315"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pvs, dataInicial, dataFinal, rechamada });

                    var _retorno = AG.Instancia.ConsultarConstrucard(                        
                        codigoBandeira,
                        pvs,
                        dataInicial,
                        dataFinal,
                        ref rechamada,
                        out indicadorRechamada,
                        out status);

                    //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                    if (_retorno == null || _retorno.Count == 0)
                        indicadorRechamada = false;

                    Log.GravarLog(EventoLog.RetornoAgente, new { _retorno, rechamada, indicadorRechamada, status });

                    return _retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public ConstrucardTotalizador ConsultarConstrucardTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Construcard - WACA1314"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pvs, dataInicial, dataFinal });

                    var _retorno = AG.Instancia.ConsultarConstrucardTotalizadores(                        
                        codigoBandeira,
                        pvs,
                        dataInicial,
                        dataFinal,
                        out status);

                    Log.GravarLog(EventoLog.RetornoAgente, new { _retorno, status });

                    return _retorno;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #endregion

        #region [ Relatório de Vendas - Recarga de Celular - PV Físico ]

         /// <summary>
        /// Consulta dos totalizadores do Relatório de Vendas de Recarga de Celular - PV Físico
        /// </summary>
        /// <param name="pvs">Lista de PVs</param>
        /// <param name="dataInicial">Data inicial da pesquisa</param>
        /// <param name="dataFinal">Data final da pesquisa</param>
        /// <param name="status">Status de retorno da pesquisa</param>
        /// <returns>Totalizadores do relatório</returns>
        public RecargaCelularTotalizador ConsultarRecargaCelularPvFisicoTotalizadores(
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (var log = Logger.IniciarLog("Relatório Recarga Celular PV Físico - Totalizadores - BKWA2610 / WAC261 / WAAF"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, new { pvs, dataInicial, dataFinal });

                    var retorno = AG.Instancia.ConsultarRecargaCelularPvFisicoTotalizadores(
                        pvs,
                        dataInicial,
                        dataFinal,
                        out status);

                    log.GravarLog(EventoLog.RetornoAgente, new { retorno, status });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
    }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

         /// <summary>
        /// Consulta dos registro do Relatório de Vendas de Recarga de Celular - PV Físico
        /// </summary>
        /// <param name="pvs">Lista de PVs</param>
        /// <param name="dataInicial">Data inicial da pesquisa</param>
        /// <param name="dataFinal">Data final da pesquisa</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Registros do relatório</returns>
        public List<RecargaCelularPvFisico> ConsultarRecargaCelularPvFisico(
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (var log = Logger.IniciarLog("Relatório Recarga Celular PV Físico - Registros - BKWA2620 / WAC262 / WAAG"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, new { pvs, dataInicial, dataFinal, rechamada });

                    var retorno = AG.Instancia.ConsultarRecargaCelularPvFisico(
                        pvs,
                        dataInicial,
                        dataFinal,
                        ref rechamada,
                        out indicadorRechamada,
                        out status);

                    //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                    if (retorno == null || retorno.Count == 0)
                        indicadorRechamada = false;

                    log.GravarLog(EventoLog.RetornoAgente, new { retorno, rechamada, indicadorRechamada, status });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #endregion

        #region [ Relatório de Vendas - Recarga de Celular - PV Lógico ]

        /// <summary>
        /// Consulta dos totalizadores do Relatório de Vendas de Recarga de Celular - PV Lógico
        /// </summary>
        /// <param name="pvs">Lista de PVs</param>
        /// <param name="dataInicial">Data inicial da pesquisa</param>
        /// <param name="dataFinal">Data final da pesquisa</param>
        /// <param name="status">Status de retorno da pesquisa</param>
        /// <returns>Totalizadores do relatório</returns>
        public RecargaCelularTotalizador ConsultarRecargaCelularPvLogicoTotalizadores(
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (var log = Logger.IniciarLog("Relatório Recarga Celular PV Lógico - Totalizadores - BKWA2630 / WAC263 / WAAH"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, new { pvs, dataInicial, dataFinal });

                    var retorno = AG.Instancia.ConsultarRecargaCelularPvLogicoTotalizadores(
                        pvs,
                        dataInicial,
                        dataFinal,
                        out status);

                    log.GravarLog(EventoLog.RetornoAgente, new { retorno, status });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta dos registro do Relatório de Vendas de Recarga de Celular - PV Lógico
        /// </summary>
        /// <param name="pvs">Lista de PVs</param>
        /// <param name="dataInicial">Data inicial da pesquisa</param>
        /// <param name="dataFinal">Data final da pesquisa</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Registros do relatório</returns>
        public List<RecargaCelularPvLogico> ConsultarRecargaCelularPvLogico(
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (var log = Logger.IniciarLog("Relatório Recarga Celular PV Lógico - Registros - BKWA2640 / WAC264 / WAAI"))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaAgente, new { pvs, dataInicial, dataFinal, rechamada });

                    var retorno = AG.Instancia.ConsultarRecargaCelularPvLogico(
                        pvs,
                        dataInicial,
                        dataFinal,
                        ref rechamada,
                        out indicadorRechamada,
                        out status);

                    //Se não retornou nenhum registro, corrige valor da flag indicadorRechamada para false
                    if (retorno == null || retorno.Count == 0)
                        indicadorRechamada = false;

                    log.GravarLog(EventoLog.RetornoAgente, new { retorno, rechamada, indicadorRechamada, status });

                    return retorno;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #endregion
    }
    }
