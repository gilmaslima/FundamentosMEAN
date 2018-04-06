using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes.WAExtratoOrdensCredito;
using TR = Redecard.PN.Extrato.Agentes.Tradutores.OrdensCreditoTR;
using Redecard.PN.Extrato.Modelo.OrdensCredito;
using Redecard.PN.Extrato.Modelo.Comum;
using Redecard.PN.Extrato.Modelo;
using System.Reflection;

namespace Redecard.PN.Extrato.Agentes
{
    public class OrdensCreditoAG : AgentesBase
    {
        private static OrdensCreditoAG _Instancia;
        public static OrdensCreditoAG Instancia { get { return _Instancia ?? (_Instancia = new OrdensCreditoAG()); } }

        public CreditoTotalizador ConsultarTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,            
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consultar Totalizadores - WACA1334"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1334";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = TR.ConsultarTotalizadoresEntrada(codigoBandeira, pvs);

                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _resto, _areaFixa });

                        client.ConsultarTotalizadores(
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

                    return TR.ConsultarTotalizadoresSaida(_areaFixa);                    
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public List<Credito> Consultar(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consultar - WACA1335"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1335";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarEntrada(codigoBandeira, pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    Int32 _rechamadaDtVct = rechamada.GetValueOrDefault<Int32>("DtVct");
                    Int32 _rechamadaNumPv = rechamada.GetValueOrDefault<Int32>("NumPv");
                    Int16 _rechamadaCdBnd = rechamada.GetValueOrDefault<Int16>("CdBnd");
                    Int16 _rechamadaNuBlc = rechamada.GetValueOrDefault<Int16>("NuBlc");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtVct, _rechamadaNumPv, _rechamadaCdBnd,
                            _rechamadaNuBlc, _resto, _areaFixa });

                        client.Consultar(
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
                            ref _rechamadaNuBlc,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal,
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtVct, _rechamadaNumPv, _rechamadaCdBnd,
                            _rechamadaNuBlc, _resto, _areaFixa });
                    }

                    rechamada["DtVct"] = _rechamadaDtVct;
                    rechamada["NumPv"] = _rechamadaNumPv;
                    rechamada["CdBnd"] = _rechamadaCdBnd;
                    rechamada["NuBlc"] = _rechamadaNuBlc;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarSaida(_areaFixa);                    
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        public CreditoDetalheTotalizador ConsultarDetalheTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataEmissao,            
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consultar - Detalhe - Totalizadores - WACA1336"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1336";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataEmissao = dataEmissao.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = TR.ConsultarDetalheTotalizadoresEntrada(codigoBandeira, pvs);

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataEmissao, _codRetorno,
                            _msgRetorno, _resto, _areaFixa });

                        client.ConsultarDetalheTotalizadores(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataEmissao,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataEmissao, _codRetorno,
                            _msgRetorno, _resto, _areaFixa });
                    }

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarDetalheTotalizadoresSaida(_areaFixa);                    
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }       

        public List<CreditoDetalhe> ConsultarDetalhe(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataEmissao,            
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consultar - Detalhe - WACA1337"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1337";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataEmissao = dataEmissao.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarDetalheEntrada(codigoBandeira, pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    Int32 _rechamadaCdCtr = rechamada.GetValueOrDefault<Int32>("CdCtr");
                    String _rechamadaDtApr = rechamada.GetValueOrDefault<String>("DtApr");
                    Int32 _rechamadaNumPv = rechamada.GetValueOrDefault<Int32>("NumPv");
                    Int32 _rechamadaNumRv = rechamada.GetValueOrDefault<Int32>("NumRv");
                    Int16 _rechamadaNuBlc = rechamada.GetValueOrDefault<Int16>("NuBlc");
                    Decimal _rechamadaNumOc = rechamada.GetValueOrDefault<Decimal>("NumOc");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    //Consulta serviço mainframe
                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataEmissao, _codRetorno,
                            _msgRetorno, _rechamadaIndicador, _rechamadaCdCtr, _rechamadaDtApr, _rechamadaNumPv,
                            _rechamadaNumRv, _rechamadaNuBlc,_rechamadaNumOc, _resto, _areaFixa });

                        client.ConsultarDetalhe(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataEmissao,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref _rechamadaCdCtr,
                            ref _rechamadaDtApr,
                            ref _rechamadaNumPv,
                            ref _rechamadaNumRv,
                            ref _rechamadaNuBlc,
                            ref _rechamadaNumOc,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataEmissao, _codRetorno,
                            _msgRetorno, _rechamadaIndicador,  _rechamadaCdCtr, _rechamadaDtApr, _rechamadaNumPv,
                            _rechamadaNumRv, _rechamadaNuBlc,_rechamadaNumOc, _resto, _areaFixa });
                    }

                    rechamada["CdCtr"] = _rechamadaCdCtr;
                    rechamada["DtApr"] = _rechamadaDtApr;
                    rechamada["NumPv"] = _rechamadaNumPv;
                    rechamada["NumRv"] = _rechamadaNumRv;
                    rechamada["NuBlc"] = _rechamadaNuBlc;
                    rechamada["NumOc"] = _rechamadaNumOc;
                    rechamada["Indicador"] = _rechamadaIndicador;

                    status = new StatusRetornoDTO(_codRetorno, _msgRetorno, FONTE_METODO);
                    indicadorRechamada = "S".Equals((_rechamadaIndicador ?? "").Trim(), StringComparison.InvariantCultureIgnoreCase);

                    //Se código for diferente de 0 => erro na consulta mainframe
                    if (status.CodigoRetorno != 0)
                        return null;

                    return TR.ConsultarDetalheSaida(_areaFixa);                    
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
