/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes.WAExtratoVendas;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Modelo.Vendas;
using TR = Redecard.PN.Extrato.Agentes.Tradutores.VendasTR;
using Redecard.PN.Extrato.Modelo;
using System.Reflection;

namespace Redecard.PN.Extrato.Agentes
{
    /// <summary>
    /// Extrato - Relatório de Vendas
    /// </summary>
    public class VendasAG : AgentesBase
    {
        private static VendasAG _Instancia;
        public static VendasAG Instancia { get { return _Instancia ?? (_Instancia = new VendasAG()); } }

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
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Crédito - WACA1311"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1311";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);                    
                    String _resto = default(String);
                    String _areaFixa = TR.ConsultarCreditoEntrada(codigoBandeira, pvs ?? new List<Int32>());

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _rechamadaDtApr = rechamada.GetValueOrDefault<String>("DtApr");
                    String _rechamadaNumPV = rechamada.GetValueOrDefault<String>("NumPV");
                    Int32 _rechamadaNumRV = rechamada.GetValueOrDefault<Int32>("NumRV");
                    String _rechamadaDtVct = rechamada.GetValueOrDefault<String>("DtVct");
                    Int16 _rechamadaBloco = rechamada.GetValueOrDefault<Int16>("Bloco");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _sistema, _usuario, _dataInicial, _dataFinal, _codRetorno,
                            _msgRetorno, _rechamadaIndicador, _rechamadaDtApr, _rechamadaNumPV, _rechamadaNumRV, 
                            _rechamadaDtVct, _rechamadaBloco, _resto, _areaFixa });

                        client.ConsultarCredito(ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref _rechamadaDtApr,
                            ref _rechamadaNumPV,
                            ref _rechamadaNumRV,
                            ref _rechamadaDtVct,
                            ref _rechamadaBloco,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _sistema, _usuario, _dataInicial, _dataFinal, _codRetorno,
                            _msgRetorno, _rechamadaIndicador, _rechamadaDtApr, _rechamadaNumPV, _rechamadaNumRV, 
                            _rechamadaDtVct, _rechamadaBloco, _resto, _areaFixa });
                    }

                    //Preparação dos dados de retorno do mainframe
                    rechamada["DtApr"] = _rechamadaDtApr;
                    rechamada["NumPV"] = _rechamadaNumPV;
                    rechamada["NumRV"] = _rechamadaNumRV;
                    rechamada["DtVct"] = _rechamadaDtVct;
                    rechamada["Bloco"] = _rechamadaBloco;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarCreditoSaida(_areaFixa);                    
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
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Totalizadores - WACA1310"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1310";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = TR.ConsultarCreditoTotalizadoresEntrada(codigoBandeira, pvs ?? new List<Int32>());

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _resto, _areaFixa });

                        client.ConsultarCreditoTotalizadores(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _resto, _areaFixa });
                    }

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarCreditoTotalizadoresSaida(_areaFixa);                    
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consultar registros do Relatório de Vendas - Débito
        /// </summary>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial da busca</param>
        /// <param name="dataFinal">Data final da busca</param>
        /// <param name="modalidade">Modalidade</param>
        /// <param name="rechamada">Dados de rechamada</param>
        /// <param name="indicadorRechamada">Indicador de rechamada</param>
        /// <param name="status">Status retorno</param>
        /// <returns>Registros do Relatório de Vendas - Débito</returns>
        public List<Debito> ConsultarDebito(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            Modalidade modalidade,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String fonteMetodo = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger log = Logger.IniciarLog("Consulta Débito - WACA1313"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String nomePrograma = "WA1313";
                    String sistema = "IS";
                    String usuario = "xxx";
                    String dataInicio = dataInicial.ToString("dd/MM/yyyy");
                    String dataFim = dataFinal.ToString("dd/MM/yyyy");
                    Int16 codRetorno = default(Int16);
                    String msgRetorno = default(String);                    
                    String resto = default(String);
                    String areaFixa = TR.ConsultarDebitoEntrada(codigoBandeira, pvs ?? new List<Int32>());
                    Int16 tipoModalidade = (Int16)modalidade;

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String rechamadaDtRv = rechamada.GetValueOrDefault<String>("DtRv");
                    Int32 rechamadaNumPV = rechamada.GetValueOrDefault<Int32>("NumPV");
                    Int32 rechamadaNumRV = rechamada.GetValueOrDefault<Int32>("NumRV");
                    String rechamadaDtVct = rechamada.GetValueOrDefault<String>("DtVct");
                    Int16 rechamadaBloco = rechamada.GetValueOrDefault<Int16>("Bloco");
                    String rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    //Consulta serviço mainframe
                    using (var ctx = new ContextoWCF<COMTIWAClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { nomePrograma, sistema, usuario, dataInicio, dataFim,
                            modalidade, tipoDeVenda = tipoModalidade, codRetorno, msgRetorno, rechamadaIndicador, rechamadaDtRv, 
                            rechamadaDtVct, rechamadaNumPV, rechamadaNumRV, rechamadaBloco, resto, areaFixa });

                        ctx.Cliente.ConsultarDebitoV2(
                            ref nomePrograma,
                            ref sistema,
                            ref usuario,
                            ref dataInicio,
                            ref dataFim,
                            ref tipoModalidade,
                            ref codRetorno,
                            ref msgRetorno,
                            ref rechamadaIndicador,
                            ref rechamadaDtRv,
                            ref rechamadaDtVct,
                            ref rechamadaNumPV,
                            ref rechamadaNumRV,
                            ref rechamadaBloco,
                            ref resto,
                            ref areaFixa);

                        log.GravarLog(EventoLog.RetornoHIS, new { nomePrograma, sistema, usuario, dataInicio, dataFim,
                            modalidade, tipoDeVenda = tipoModalidade, codRetorno, msgRetorno, rechamadaIndicador, rechamadaDtRv, 
                            rechamadaDtVct, rechamadaNumPV, rechamadaNumRV, rechamadaBloco, resto, areaFixa });
                    }

                    //Preparação dos dados de retorno do mainframe
                    rechamada["DtRv"] = rechamadaDtRv;
                    rechamada["NumPV"] = rechamadaNumPV;
                    rechamada["NumRV"] = rechamadaNumRV;
                    rechamada["DtVct"] = rechamadaDtVct;
                    rechamada["Bloco"] = rechamadaBloco;
                    rechamada["Indicador"] = rechamadaIndicador;

                    status = new StatusRetornoDTO(codRetorno, msgRetorno, fonteMetodo);
                    indicadorRechamada = String.Compare("S", (rechamadaIndicador ?? String.Empty).Trim(), true) == 0;

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarDebitoSaida(areaFixa);                                        
                }
                catch(PortalRedecardException ex)
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
        /// Consultar totalizadores do Relatório de Vendas - Débito
        /// </summary>
        /// <param name="codigoBandeira">Código da bandeira</param>
        /// <param name="pvs">Lista de código de estabelecimentos</param>
        /// <param name="dataInicial">Data inicial da busca</param>
        /// <param name="dataFinal">Data final da busca</param>
        /// <param name="modalidade">Modalidade</param>
        /// <param name="status">Status de retorno da consulta</param>
        /// <returns>Totalizadores do Relatório de Vendas - Débito</returns>
        public DebitoTotalizador ConsultarDebitoTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            Modalidade modalidade,
            out StatusRetornoDTO status)
        {
            String fonteMetodo = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger log = Logger.IniciarLog("Consulta Débito - Totalizadores - WACA1312"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String nomePrograma = "WA1312";
                    String sistema = "IS";
                    String usuario = "xxx";
                    String dataInicio = dataInicial.ToString("dd/MM/yyyy");
                    String dataFim = dataFinal.ToString("dd/MM/yyyy");
                    Int16 codRetorno = default(Int16);
                    String msgRetorno = default(String);
                    String resto = default(String);
                    Int16 tipoModalidade = (Int16)modalidade;
                    String areaFixa = TR.ConsultarDebitoTotalizadoresEntrada(codigoBandeira, pvs ?? new List<Int32>());

                    //Consulta serviço mainframe
                    using (var ctx = new ContextoWCF<COMTIWAClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { nomePrograma, sistema, usuario, dataInicio, 
                            dataFim, tipoVenda = modalidade, tipoModalidade, codRetorno, msgRetorno, resto, areaFixa });

                        ctx.Cliente.ConsultarDebitoTotalizadoresV2(
                            ref nomePrograma,
                            ref sistema,
                            ref usuario,
                            ref dataInicio,
                            ref dataFim,
                            ref tipoModalidade,
                            ref codRetorno,
                            ref msgRetorno,
                            ref resto,
                            ref areaFixa);

                        log.GravarLog(EventoLog.RetornoHIS, new { nomePrograma, sistema, usuario, dataInicio, 
                            dataFim, modalidade, tipoModalidade, codRetorno, msgRetorno, resto, areaFixa });
                    }

                    status = new StatusRetornoDTO(codRetorno, msgRetorno, fonteMetodo);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarDebitoTotalizadoresSaida(areaFixa);                    
                }
                catch(PortalRedecardException ex)
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
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Construcard - WACA1315"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1315";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);                    
                    String _resto = default(String);
                    String _areaFixa = TR.ConsultarConstrucardEntrada(codigoBandeira, pvs ?? new List<Int32>());

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _rechamadaDtRv = rechamada.GetValueOrDefault<String>("DtRv");
                    Int32 _rechamadaNumPV = rechamada.GetValueOrDefault<Int32>("NumPV");
                    Int32 _rechamadaNumRV = rechamada.GetValueOrDefault<Int32>("NumRV");
                    String _rechamadaDtCre = rechamada.GetValueOrDefault<String>("DtCre");
                    Int16 _rechamadaBloco = rechamada.GetValueOrDefault<Int16>("Bloco");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtRv, _rechamadaNumPV, _rechamadaNumRV,
                            _rechamadaDtCre, _rechamadaBloco, _resto, _areaFixa });

                        client.ConsultarConstrucard(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref _rechamadaDtRv,
                            ref _rechamadaNumPV,
                            ref _rechamadaNumRV,
                            ref _rechamadaDtCre,
                            ref _rechamadaBloco,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtRv, _rechamadaNumPV, _rechamadaNumRV,
                            _rechamadaDtCre, _rechamadaBloco, _resto, _areaFixa });
                    }

                    //Preparação dos dados de retorno do mainframe
                    rechamada["DtRv"] = _rechamadaDtRv;
                    rechamada["NumPV"] = _rechamadaNumPV;
                    rechamada["NumRV"] = _rechamadaNumRV;
                    rechamada["DtCre"] = _rechamadaDtCre;
                    rechamada["Bloco"] = _rechamadaBloco;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarConstrucardSaida(_areaFixa);                                        
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
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Construcard - WACA1314"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1314";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = TR.ConsultarConstrucardTotalizadoresEntrada(codigoBandeira, pvs ?? new List<Int32>());

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _resto, _areaFixa });

                        client.ConsultarConstrucardTotalizadores(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _resto, _areaFixa });
                    }

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarConstrucardTotalizadoresSaida(_areaFixa);                    
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
        /// <remarks>
        /// Utiliza:<br/>
        /// - Book      BKWA2610<br/>
        /// - Programa  WAC261<br/>
        /// - TranID    WAAF
        /// </remarks>
        public RecargaCelularTotalizador ConsultarRecargaCelularPvFisicoTotalizadores(
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            String fonteMetodo = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (var log = Logger.IniciarLog("Relatório Recarga Celular PV Físico - Totalizadores - BKWA2610 / WAC261 / WAAF"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String nomePrograma = "WAC261";
                    Int32 dataInicio = dataInicial.ToString("yyyyMMdd").ToInt32();
                    Int32 dataFim = dataFinal.ToString("yyyyMMdd").ToInt32();
                    Int16 codigoRetorno = default(Int16);
                    Int16 codigoErro = default(Int16);
                    String mensagemRetorno = default(String);
                    String filler = default(String);
                    Decimal valorTotalRecarga = default(Decimal);
                    Decimal valorTotalComissao = default(Decimal);

                    WAC261E_FILLER[] listaPvs = default(WAC261E_FILLER[]);
                    Int32 quantidadePvs = default(Int32);
                    TR.ConsultarRecargaCelularPvFisicoTotalizadoresEntrada(pvs, out quantidadePvs, out listaPvs);

                    //Consulta serviço mainframe
                    using (var ctx = new ContextoWCF<COMTIWAClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { nomePrograma, dataInicio, dataFim,
                            quantidadePvs, listaPvs, codigoRetorno, codigoErro, mensagemRetorno,
                            valorTotalRecarga, valorTotalComissao, filler });

                        ctx.Cliente.ConsultarRecargaCelularPvFisicoTotalizadores(
                            ref nomePrograma,
                            ref dataInicio,
                            ref dataFim,
                            ref quantidadePvs,
                            ref listaPvs,
                            ref codigoRetorno,
                            ref codigoErro,
                            ref mensagemRetorno,
                            ref valorTotalRecarga,
                            ref valorTotalComissao,
                            ref filler);

                        log.GravarLog(EventoLog.RetornoHIS, new { nomePrograma, dataInicio, dataFim,
                            quantidadePvs, listaPvs, codigoRetorno, codigoErro, mensagemRetorno,
                            valorTotalRecarga, valorTotalComissao, filler });
    }

                    status = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, fonteMetodo);

                    //Se código for diferente de 0 => erro na consulta mainframe, não retorna registros
                    //00 - OK
                    //10 - PROGRAMA CHAMADOR NAO INFORMADO
                    //11 - ESTABELECIMENTO NAO INFORMADO / INVALIDO
                    //12 - DATA MOVTO VENDA INICIO NAO INFORMADA
                    //13 - DATA MOVTO VENDA INICIO INVALIDA
                    //14 - DATA MOVTO VENDA FINAL NAO INFORMADA
                    //15 - DATA MOVTO VENDA FINAL INVALIDA
                    //16 - DATA MOVTO VENDA INICIAL MAIOR FINAL
                    //17 - PERIODO DO MOVIMENTO SUPERIOR A 99 DIAS
                    //18 - ERRO NA CHAMADA DA MEC783
                    //60 - NENHUM ARGUMENTO DE PESQUISA ENCONTRADO
                    //99 - ERRO NO ACESSO AO DB2 (VIDE SQLCODE)
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarRecargaCelularPvFisicoTotalizadoresSaida(valorTotalRecarga, valorTotalComissao);
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
        /// <remarks>
        /// Utiliza:<br/>
        /// - Book      BKWA2620<br/>
        /// - Programa  WAC262<br/>
        /// - TranID    WAAG
        /// </remarks>
        public List<RecargaCelularPvFisico> ConsultarRecargaCelularPvFisico(
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String fonteMetodo = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger log = Logger.IniciarLog("Relatório Recarga Celular PV Físico - Registros - BKWA2620 / WAC262 / WAAG"))
			{
                try
                {
                    //Preparação dos dados para chamada HIS
                    String nomePrograma = "WAC262";
                    Int32 dataInicio = dataInicial.ToString("yyyyMMdd").ToInt32();
                    Int32 dataFim = dataFinal.ToString("yyyyMMdd").ToInt32();
                    Int16 codigoRetorno = default(Int16);
                    Int16 codigoErro = default(Int16);
                    String mensagemRetorno = default(String);
                    String areaFixa = TR.ConsultarRecargaCelularPvFisicoEntrada(pvs);

                    //Recuperação dos dados de rechamada
                    rechamada = rechamada ?? new Dictionary<String, Object>();
                    String rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");
                    Int32 rechamadaNumPv = rechamada.GetValueOrDefault<Int32>("NumPv");
                    String rechamadaDtApr = rechamada.GetValueOrDefault<String>("DtApr");
                    Decimal rechamadaNumNsu = rechamada.GetValueOrDefault<Decimal>("NumNsu");
                    String rechamadaDtVct = rechamada.GetValueOrDefault<String>("DtVct");
                    String rechamadaFiller = rechamada.GetValueOrDefault<String>("Filler");

                    //Chamada serviço mainframe
                    using (var ctx = new ContextoWCF<COMTIWAClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { nomePrograma, dataInicio, dataFim,
                            codigoRetorno, codigoErro, mensagemRetorno, rechamadaIndicador, rechamadaNumPv,
                            rechamadaDtApr, rechamadaNumNsu, rechamadaDtVct, rechamadaFiller, areaFixa });

                        ctx.Cliente.ConsultarRecargaCelularPvFisico(
                            ref nomePrograma,
                            ref dataInicio,
                            ref dataFim,
                            ref codigoRetorno,
                            ref codigoErro,
                            ref mensagemRetorno,
                            ref rechamadaIndicador,
                            ref rechamadaNumPv,
                            ref rechamadaDtApr,
                            ref rechamadaNumNsu,
                            ref rechamadaDtVct,
                            ref rechamadaFiller,
                            ref areaFixa);

                        log.GravarLog(EventoLog.ChamadaHIS, new { nomePrograma, dataInicio, dataFim,
                            codigoRetorno, codigoErro, mensagemRetorno, rechamadaIndicador, rechamadaNumPv,
                            rechamadaDtApr, rechamadaNumNsu, rechamadaDtVct, rechamadaFiller, areaFixa });
                    }

                    //Preparação dos dados de retorno do mainframe
                    rechamada["Indicador"] = rechamadaIndicador;
                    rechamada["NumPv"] = rechamadaNumPv;
                    rechamada["DtApr"] = rechamadaDtApr;
                    rechamada["NumNsu"] = rechamadaNumNsu;
                    rechamada["DtVct"] = rechamadaDtVct;
                    rechamada["Filler"] = rechamadaFiller;

                    status = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, fonteMetodo);
                    indicadorRechamada = String.Compare("S", rechamadaIndicador, true) == 0;

                    //Se código for diferente de 0 => erro/falha na consulta mainframe, não retorna registros
                    //00 - OK
                    //10 - PROGRAMA CHAMADOR NAO INFORMADO
                    //11 - ESTABELECIMENTO NAO INFORMADO / INVALIDO
                    //12 - DATA MOVTO VENDA INICIO NAO INFORMADA
                    //13 - DATA MOVTO VENDA INICIO INVALIDA
                    //14 - DATA MOVTO VENDA FINAL NAO INFORMADA
                    //15 - DATA MOVTO VENDA FINAL INVALIDA
                    //16 - DATA MOVTO VENDA INICIAL MAIOR FINAL
                    //17 - PERIODO DO MOVIMENTO SUPERIOR A 99 DIAS
                    //18 - ERRO NA CHAMADA DA MECXXX
                    //60 - NENHUM ARGUMENTO DE PESQUISA ENCONTRADO
                    //99 - ERRO NO ACESSO AO DB2 (VIDE SQLCODE)
                    if (status.CodigoRetorno != 0)
                        return null;                    

                    return TR.ConsultarRecargaCelularPvFisicoSaida(areaFixa);
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
        /// <remarks>
        /// Utiliza:<br/>
        /// - Book      BKWA2630<br/>
        /// - Programa  WAC263<br/>
        /// - TranID    WAAH
        /// </remarks>
        public RecargaCelularTotalizador ConsultarRecargaCelularPvLogicoTotalizadores(
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            String fonteMetodo = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (var log = Logger.IniciarLog("Relatório Recarga Celular PV Lógico - Totalizadores - BKWA2630 / WAC263 / WAAH"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String nomePrograma = "WAC263";
                    Int32 dataInicio = dataInicial.ToString("yyyyMMdd").ToInt32();
                    Int32 dataFim = dataFinal.ToString("yyyyMMdd").ToInt32();
                    Int16 codigoRetorno = default(Int16);
                    Int16 codigoErro = default(Int16);
                    String mensagemRetorno = default(String);
                    String filler = default(String);
                    Decimal valorTotalRecarga = default(Decimal);
                    Decimal valorTotalComissao = default(Decimal);

                    WAC263E_FILLER[] listaPvs = default(WAC263E_FILLER[]);
                    Int32 quantidadePvs = default(Int32);
                    TR.ConsultarRecargaCelularPvLogicoTotalizadoresEntrada(pvs, out quantidadePvs, out listaPvs);

                    //Consulta serviço mainframe
                    using (var ctx = new ContextoWCF<COMTIWAClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { nomePrograma, dataInicio, dataFim,
                            quantidadePvs, listaPvs, codigoRetorno, codigoErro, mensagemRetorno,
                            valorTotalRecarga, valorTotalComissao, filler });

                        ctx.Cliente.ConsultarRecargaCelularPvLogicoTotalizadores(
                            ref nomePrograma,
                            ref dataInicio,
                            ref dataFim,
                            ref quantidadePvs,
                            ref listaPvs,
                            ref codigoRetorno,
                            ref codigoErro,
                            ref mensagemRetorno,
                            ref valorTotalRecarga,
                            ref valorTotalComissao,
                            ref filler);

                        log.GravarLog(EventoLog.RetornoHIS, new { nomePrograma, dataInicio, dataFim,
                            quantidadePvs, listaPvs, codigoRetorno, codigoErro, mensagemRetorno,
                            valorTotalRecarga, valorTotalComissao, filler });
                    }

                    status = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, fonteMetodo);

                    //Se código for diferente de 0 => erro na consulta mainframe, não retorna registros
                    //00 - OK
                    //10 - PROGRAMA CHAMADOR NAO INFORMADO
                    //11 - ESTABELECIMENTO NAO INFORMADO / INVALIDO
                    //12 - DATA MOVTO VENDA INICIO NAO INFORMADA
                    //13 - DATA MOVTO VENDA INICIO INVALIDA
                    //14 - DATA MOVTO VENDA FINAL NAO INFORMADA
                    //15 - DATA MOVTO VENDA FINAL INVALIDA
                    //16 - DATA MOVTO VENDA INICIAL MAIOR FINAL
                    //17 - PERIODO DO MOVIMENTO SUPERIOR A 99 DIAS
                    //18 - ERRO NA CHAMADA DA MEC783
                    //60 - NENHUM ARGUMENTO DE PESQUISA ENCONTRADO
                    //99 - ERRO NO ACESSO AO DB2 (VIDE SQLCODE)
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarRecargaCelularPvLogicoTotalizadoresSaida(valorTotalRecarga, valorTotalComissao);
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
        /// <remarks>
        /// Utiliza:<br/>
        /// - Book      BKWA2640<br/>
        /// - Programa  WAC264<br/>
        /// - TranID    WAAI
        /// </remarks>
        public List<RecargaCelularPvLogico> ConsultarRecargaCelularPvLogico(
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String fonteMetodo = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger log = Logger.IniciarLog("Relatório Recarga Celular PV Lógico - Registros - BKWA2640 / WAC264 / WAAI"))
			{
                try
                {
                    //Preparação dos dados para chamada HIS
                    String nomePrograma = "WAC264";
                    Int32 dataInicio = dataInicial.ToString("yyyyMMdd").ToInt32();
                    Int32 dataFim = dataFinal.ToString("yyyyMMdd").ToInt32();
                    Int16 codigoRetorno = default(Int16);
                    Int16 codigoErro = default(Int16);
                    String mensagemRetorno = default(String);
                    String areaFixa = TR.ConsultarRecargaCelularPvLogicoEntrada(pvs);

                    //Recuperação dos dados de rechamada
                    rechamada = rechamada ?? new Dictionary<String, Object>();
                    String rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");
                    Int32 rechamadaNumPv = rechamada.GetValueOrDefault<Int32>("NumPv");
                    String rechamadaDtApr = rechamada.GetValueOrDefault<String>("DtApr");
                    Decimal rechamadaNumNsu = rechamada.GetValueOrDefault<Decimal>("NumNsu");
                    String rechamadaDtVct = rechamada.GetValueOrDefault<String>("DtVct");
                    String rechamadaFiller = rechamada.GetValueOrDefault<String>("Filler");

                    //Chamada serviço mainframe
                    using (var ctx = new ContextoWCF<COMTIWAClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaHIS, new { nomePrograma, dataInicio, dataFim,
                            codigoRetorno, codigoErro, mensagemRetorno, rechamadaIndicador, rechamadaNumPv,
                            rechamadaDtApr, rechamadaNumNsu, rechamadaDtVct, rechamadaFiller, areaFixa });

                        ctx.Cliente.ConsultarRecargaCelularPvLogico(
                            ref nomePrograma,
                            ref dataInicio,
                            ref dataFim,
                            ref codigoRetorno,
                            ref codigoErro,
                            ref mensagemRetorno,
                            ref rechamadaIndicador,
                            ref rechamadaNumPv,
                            ref rechamadaDtApr,
                            ref rechamadaNumNsu,
                            ref rechamadaDtVct,
                            ref rechamadaFiller,
                            ref areaFixa);

                        log.GravarLog(EventoLog.ChamadaHIS, new { nomePrograma, dataInicio, dataFim,
                            codigoRetorno, codigoErro, mensagemRetorno, rechamadaIndicador, rechamadaNumPv,
                            rechamadaDtApr, rechamadaNumNsu, rechamadaDtVct, rechamadaFiller, areaFixa });
                    }

                    //Preparação dos dados de retorno do mainframe
                    rechamada["Indicador"] = rechamadaIndicador;
                    rechamada["NumPv"] = rechamadaNumPv;
                    rechamada["DtApr"] = rechamadaDtApr;
                    rechamada["NumNsu"] = rechamadaNumNsu;
                    rechamada["DtVct"] = rechamadaDtVct;
                    rechamada["Filler"] = rechamadaFiller;

                    status = new StatusRetornoDTO(codigoRetorno, mensagemRetorno, fonteMetodo);
                    indicadorRechamada = String.Compare("S", rechamadaIndicador, true) == 0;

                    //Se código for diferente de 0 => erro/falha na consulta mainframe, não retorna registros
                    //00 - OK
                    //10 - PROGRAMA CHAMADOR NAO INFORMADO
                    //11 - ESTABELECIMENTO NAO INFORMADO / INVALIDO
                    //12 - DATA MOVTO VENDA INICIO NAO INFORMADA
                    //13 - DATA MOVTO VENDA INICIO INVALIDA
                    //14 - DATA MOVTO VENDA FINAL NAO INFORMADA
                    //15 - DATA MOVTO VENDA FINAL INVALIDA
                    //16 - DATA MOVTO VENDA INICIAL MAIOR FINAL
                    //17 - PERIODO DO MOVIMENTO SUPERIOR A 99 DIAS
                    //18 - ERRO NA CHAMADA DA MECXXX
                    //60 - NENHUM ARGUMENTO DE PESQUISA ENCONTRADO
                    //99 - ERRO NO ACESSO AO DB2 (VIDE SQLCODE)
                    if (status.CodigoRetorno != 0)
                        return null;                    

                    return TR.ConsultarRecargaCelularPvLogicoSaida(areaFixa);
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
