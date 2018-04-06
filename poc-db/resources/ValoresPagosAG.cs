using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo.ValoresPagos;
using Redecard.PN.Extrato.Agentes.WAExtratoValoresPagos;
using Redecard.PN.Extrato.Modelo;
using TR = Redecard.PN.Extrato.Agentes.Tradutores.ValoresPagosTR;
using System.Reflection;

namespace Redecard.PN.Extrato.Agentes
{
    public class ValoresPagosAG : AgentesBase
    {
        private static ValoresPagosAG _Instancia;
        public static ValoresPagosAG Instancia { get { return _Instancia ?? (_Instancia = new ValoresPagosAG()); } }

        public CreditoTotalizador ConsultarCreditoTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Totalizadores - WACA1316"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1316";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = TR.ConsultarCreditoTotalizadoresEntrada(codigoBandeira, pvs);

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

            using (Logger Log = Logger.IniciarLog("Consulta Crédito - WACA1317"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1317";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarCreditoEntrada(codigoBandeira, pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _rechamadaDtBxa = rechamada.GetValueOrDefault<String>("DtBxa");
                    Int32 _rechamadaNumPv = rechamada.GetValueOrDefault<Int32>("NumPv");
                    Int32 _rechamadaNumOcu = rechamada.GetValueOrDefault<Int32>("NumOcu");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");
                    Int16 _rechamadaNuBlc = rechamada.GetValueOrDefault<Int16>("NuBlc");

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtBxa, _rechamadaNumPv, _rechamadaNumOcu,
                            _rechamadaNuBlc, _resto, _areaFixa });

                        client.ConsultarCredito(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref _rechamadaDtBxa,
                            ref _rechamadaNumPv,
                            ref _rechamadaNumOcu,
                            ref _rechamadaNuBlc,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtBxa, _rechamadaNumPv, _rechamadaNumOcu,
                            _rechamadaNuBlc, _resto, _areaFixa });
                    }

                    rechamada["DtBxa"] = _rechamadaDtBxa;
                    rechamada["NumPv"] = _rechamadaNumPv;
                    rechamada["NumOcu"] = _rechamadaNumOcu;
                    rechamada["Indicador"] = _rechamadaIndicador;
                    rechamada["NuBlc"] = _rechamadaNuBlc;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? String.Empty).Trim(), StringComparison.InvariantCultureIgnoreCase);

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

        public CreditoDetalheTotalizador ConsultarCreditoDetalheTotalizadores(            
            Int32 codigoBandeira,
            Int32 pv,
            DateTime dataRecebimento,
            Int32 numeroOcu,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Detalhe - Totalizadores - WACA1318"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1318";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataRecebimento = dataRecebimento.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    Int16 _codigoBandeira = Convert.ToInt16(codigoBandeira);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = default(String);

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, pv, _dataRecebimento,
                            numeroOcu, _codigoBandeira, _codRetorno, _msgRetorno, _resto, _areaFixa });

                        client.ConsultarCreditoDetalheTotalizadores(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref pv,
                            ref _dataRecebimento,
                            ref numeroOcu,
                            ref _codigoBandeira,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, pv, _dataRecebimento,
                            numeroOcu, _codigoBandeira, _codRetorno, _msgRetorno, _resto, _areaFixa });
                    }

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarCreditoDetalheTotalizadoresSaida(_areaFixa);                    
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<CreditoDetalhe> ConsultarCreditoDetalhe(            
            Int32 codigoBandeira,
            Int32 pv,
            DateTime dataRecebimento,
            Int32 numeroOcu,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Detalhe - WACA1319"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1319";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataRecebimento = dataRecebimento.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    Int16 _codigoBandeira = Convert.ToInt16(codigoBandeira);                    
                    String _areaFixa = default(String);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _rechamadaTmsOc = rechamada.GetValueOrDefault<String>("TmsOc");
                    String _rechamadaDatApr = rechamada.GetValueOrDefault<String>("DatApr");
                    Int32 _rechamadaNumPV = rechamada.GetValueOrDefault<Int32>("NumPV");
                    Int32 _rechamadaNumRV = rechamada.GetValueOrDefault<Int32>("NumRV");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, pv, _dataRecebimento,
                            numeroOcu, _codigoBandeira, _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDatApr,
                            _rechamadaNumPV, _rechamadaNumRV, _rechamadaTmsOc, _resto, _areaFixa });

                        client.ConsultarCreditoDetalhe(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref pv,
                            ref _dataRecebimento,
                            ref numeroOcu,
                            ref _codigoBandeira,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref _rechamadaDatApr,
                            ref _rechamadaNumPV,
                            ref _rechamadaNumRV,
                            ref _rechamadaTmsOc,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, pv, _dataRecebimento,
                            numeroOcu, _codigoBandeira, _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDatApr,
                            _rechamadaNumPV, _rechamadaNumRV, _rechamadaTmsOc, _resto, _areaFixa });
                    }

                    rechamada["TmsOc"] = _rechamadaTmsOc;
                    rechamada["NumPV"] = _rechamadaNumPV;
                    rechamada["NumRV"] = _rechamadaNumRV;
                    rechamada["DatApr"] = _rechamadaDatApr;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarCreditoDetalheSaida(_areaFixa);                    
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public DebitoTotalizador ConsultarDebitoTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Débito - Totalizadores - WACA1320"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1320";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = TR.ConsultarDebitoTotalizadoresEntrada(codigoBandeira, pvs);

                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _resto, _areaFixa });

                        client.ConsultarDebitoTotalizadores(
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

                    return TR.ConsultarDebitoTotalizadoresSaida(_areaFixa);                    
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<Debito> ConsultarDebito(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Débito - WACA1321"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1321";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarDebitoEntrada(codigoBandeira, pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _rechamadaDtPgto = rechamada.GetValueOrDefault<String>("DtPgto");
                    Int32 _rechamadaNumPv = rechamada.GetValueOrDefault<Int32>("NumPv");
                    Int16 _rechamadaCdBnd = rechamada.GetValueOrDefault<Int16>("CdBnd");
                    Int16 _rechamadaNuBlc = rechamada.GetValueOrDefault<Int16>("NuBlc");
                    String _rechamadaTms = rechamada.GetValueOrDefault<String>("Tms");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtPgto, _rechamadaNumPv, _rechamadaCdBnd,
                            _rechamadaTms, _rechamadaNuBlc, _resto, _areaFixa });

                        client.ConsultarDebito(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref _rechamadaDtPgto,
                            ref _rechamadaNumPv,
                            ref _rechamadaTms,
                            ref _rechamadaNuBlc,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtPgto, _rechamadaNumPv, _rechamadaCdBnd,
                            _rechamadaTms, _rechamadaNuBlc, _resto, _areaFixa });
                    }

                    rechamada["DtPgto"] = _rechamadaDtPgto;
                    rechamada["NumPv"] = _rechamadaNumPv;
                    rechamada["CdBnd"] = _rechamadaCdBnd;
                    rechamada["Tms"] = _rechamadaTms;
                    rechamada["NuBlc"] = _rechamadaNuBlc;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarDebitoSaida(_areaFixa);                    
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public DebitoDetalheTotalizador ConsultarDebitoDetalheTotalizadores(            
            Int32 pv,
            DateTime dataPagamento,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Débito - Detalhe - Totalizadores - WACA1322"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1322";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataPagamento = dataPagamento.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = default(String);

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, pv, 
                            _dataPagamento, _codRetorno, _msgRetorno, _resto, _areaFixa });

                        client.ConsultarDebitoDetalheTotalizadores(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataPagamento,
                            ref pv,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, pv,
                            _dataPagamento, _codRetorno, _msgRetorno, _resto, _areaFixa });
                    }

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarDebitoDetalheTotalizadoresSaida(_areaFixa);                    
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<DebitoDetalhe> ConsultarDebitoDetalhe(
            Int32 pv,
            DateTime dataPagamento,            
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Débito - Detalhe - WACA1323"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1323";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataPagamento = dataPagamento.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = default(String);

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, pv,
                            _dataPagamento, _codRetorno, _msgRetorno, _resto, _areaFixa });

                        client.ConsultarDebitoDetalhe(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataPagamento,
                            ref pv,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, pv,
                            _dataPagamento, _codRetorno, _msgRetorno, _resto, _areaFixa });
                    }

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    
                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarDebitoDetalheSaida(_areaFixa);                   
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }
    }
}
