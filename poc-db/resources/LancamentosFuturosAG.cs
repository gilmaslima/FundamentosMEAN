using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes.WAExtratoLancamentosFuturos;
using TR = Redecard.PN.Extrato.Agentes.Tradutores.LancamentosFuturosTR;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Modelo.Comum;
using Redecard.PN.Extrato.Modelo.LancamentosFuturos;
using System.Reflection;

namespace Redecard.PN.Extrato.Agentes
{
    public class LancamentosFuturosAG : AgentesBase
    {
        private static LancamentosFuturosAG _Instancia;
        public static LancamentosFuturosAG Instancia { get { return _Instancia ?? (_Instancia = new LancamentosFuturosAG()); } }

        public CreditoTotalizador ConsultarCreditoTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Totalizadores - WACA1324"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1324";
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

            using (Logger Log = Logger.IniciarLog("Consulta Crédito - WACA1325"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1325";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarCreditoEntrada(codigoBandeira, pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    Int32 _rechamadaNumPv = rechamada.GetValueOrDefault<Int32>("NumPv");
                    Int16 _rechamadaNumBlc = rechamada.GetValueOrDefault<Int16>("NumBlc");
                    String _rechamadaDtVct = rechamada.GetValueOrDefault<String>("DtVct");
                    Int16 _rechamadaCdBnd = rechamada.GetValueOrDefault<Int16>("CdBnd");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtVct, _rechamadaNumPv, _rechamadaCdBnd, 
                            _rechamadaNumBlc, _resto, _areaFixa });

                        client.ConsultarCredito(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref _rechamadaDtVct,
                            ref _rechamadaNumPv,
                            ref _rechamadaCdBnd,
                            ref _rechamadaNumBlc,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtVct, _rechamadaNumPv, _rechamadaCdBnd, 
                            _rechamadaNumBlc, _resto, _areaFixa });
                    }
                  
                    rechamada["NumPv"] = _rechamadaNumPv;
                    rechamada["NumBlc"] = _rechamadaNumBlc;
                    rechamada["DtVct"] = _rechamadaDtVct;
                    rechamada["CdBnd"] = _rechamadaCdBnd;
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

        public CreditoDetalheTotalizador ConsultarCreditoDetalheTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataVencimento,            
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Detalhe - Totalizadores - WACA1326"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1326";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataVencimento = dataVencimento.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = TR.ConsultarCreditoDetalheTotalizadoresEntrada(codigoBandeira, pvs);

                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataVencimento,
                            _codRetorno, _msgRetorno, _resto, _areaFixa });

                        client.ConsultarCreditoDetalheTotalizadores(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataVencimento,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataVencimento,
                            _codRetorno, _msgRetorno, _resto, _areaFixa });
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
            DateTime dataVencimento,
            Int32 codigoBandeira,
            List<Int32> pvs,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Detalhe - WACA1327"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1327";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataVencimento = dataVencimento.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarCreditoDetalheEntrada(codigoBandeira, pvs ?? new List<Int32>());

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    Int32 _rechamadaCdCtr = rechamada.GetValueOrDefault<Int32>("CdCtr");
                    String _rechamadaDtApr = rechamada.GetValueOrDefault<String>("DtApr");
                    Int32 _rechamadaNumPV = rechamada.GetValueOrDefault<Int32>("NumPV");
                    Int32 _rechamadaNumRV = rechamada.GetValueOrDefault<Int32>("NumRV");
                    Int16 _rechamadaNuBlc = rechamada.GetValueOrDefault<Int16>("NuBlc");
                    String _rechamadaTipCont = rechamada.GetValueOrDefault<String>("TipCont");
                    String _rechamadaTmsAjCont = rechamada.GetValueOrDefault<String>("TmsAjCont");
                    String _rechamadaTmsDtCont = rechamada.GetValueOrDefault<String>("TmsDtCont");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataVencimento, _codRetorno,
                            _msgRetorno, _rechamadaIndicador, _rechamadaCdCtr, _rechamadaDtApr, _rechamadaNumPV, _rechamadaNumRV,
                            _rechamadaNuBlc, _rechamadaTipCont, _rechamadaTmsAjCont, _rechamadaTmsDtCont, _resto, _areaFixa });

                        client.ConsultarCreditoDetalhe(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataVencimento,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref _rechamadaCdCtr,
                            ref _rechamadaDtApr,
                            ref _rechamadaNumPV,
                            ref _rechamadaNumRV,
                            ref _rechamadaNuBlc,
                            ref _rechamadaTipCont,
                            ref _rechamadaTmsAjCont,
                            ref _rechamadaTmsDtCont,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataVencimento, _codRetorno,
                            _msgRetorno, _rechamadaIndicador, _rechamadaCdCtr, _rechamadaDtApr, _rechamadaNumPV, _rechamadaNumRV,
                            _rechamadaNuBlc, _rechamadaTipCont, _rechamadaTmsAjCont,_rechamadaTmsDtCont, _resto, _areaFixa });
                    }

                    //Preparação dos dados de retorno do mainframe
                    rechamada["CdCtr"] = _rechamadaCdCtr;
                    rechamada["DtApr"] = _rechamadaDtApr;
                    rechamada["NumPV"] = _rechamadaNumPV;
                    rechamada["NumRV"] = _rechamadaNumRV;
                    rechamada["NuBlc"] = _rechamadaNuBlc;
                    rechamada["TipCont"] = _rechamadaTipCont;
                    rechamada["TmsAjCont"] = _rechamadaTmsAjCont;
                    rechamada["TmsDtCont"] = _rechamadaTmsDtCont;
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

            using (Logger Log = Logger.IniciarLog("Consulta Débito - Totalizadores - WACA1328"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1328";
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
            DateTime dataInicial,
            DateTime dataFinal,
            Int32 codigoBandeira,
            List<Int32> pvs,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Débito - WACA1329"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1329";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarCreditoDetalheEntrada(codigoBandeira, pvs ?? new List<Int32>());

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    String _rechamadaDtVct = rechamada.GetValueOrDefault<String>("DtVct");
                    Int32 _rechamadaNumPV = rechamada.GetValueOrDefault<Int32>("NumPV");
                    String _rechamadaTmsRv = rechamada.GetValueOrDefault<String>("TmsRv");
                    String _rechamadaTmsCv = rechamada.GetValueOrDefault<String>("TmsCv");
                    Int16 _rechamadaNuBlc = rechamada.GetValueOrDefault<Int16>("NuBlc");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtVct, _rechamadaNumPV, _rechamadaTmsRv,
                            _rechamadaTmsCv, _rechamadaNuBlc, _resto, _areaFixa });

                        client.ConsultarDebito(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataInicial,
                            ref _dataFinal,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref _rechamadaDtVct,
                            ref _rechamadaNumPV,
                            ref _rechamadaTmsRv,
                            ref _rechamadaTmsCv,
                            ref _rechamadaNuBlc, 
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtVct, _rechamadaNumPV, _rechamadaTmsRv,
                            _rechamadaTmsCv, _rechamadaNuBlc, _resto, _areaFixa });
                    }

                    //Preparação dos dados de retorno do mainframe                                    
                    rechamada["DtVct"] = _rechamadaDtVct;
                    rechamada["NumPV"] = _rechamadaNumPV;
                    rechamada["TmsRv"] = _rechamadaTmsRv;
                    rechamada["TmsCv"] = _rechamadaTmsCv;
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
    }
}
