using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes.WAExtratoRAV;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Modelo.AntecipacaoRAV;
using Redecard.PN.Extrato.Modelo.Comum;
using System.Reflection;
using TR = Redecard.PN.Extrato.Agentes.Tradutores.AntecipacaoRAVTR;

namespace Redecard.PN.Extrato.Agentes
{
    public class AntecipacaoRAVAG : AgentesBase
    {
        private static AntecipacaoRAVAG _Instancia;
        public static AntecipacaoRAVAG Instancia { get { return _Instancia ?? (_Instancia = new AntecipacaoRAVAG()); } }

        public RAVTotalizador ConsultarTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Totalizadores - WACA1330"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1330";
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

        public List<RAV> Consultar(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Dados - WACA1331"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1331";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataInicial = dataInicial.ToString("dd/MM/yyyy");
                    String _dataFinal = dataFinal.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarEntrada(codigoBandeira, pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    Int32 _rechamadaDtAnt = rechamada.GetValueOrDefault<Int32>("DtAnt");
                    Int32 _rechamadaNumPv = rechamada.GetValueOrDefault<Int32>("NumPv");
                    Int16 _rechamadaCdBnd = rechamada.GetValueOrDefault<Int16>("CdBnd");
                    Int16 _rechamadaNuBlc = rechamada.GetValueOrDefault<Int16>("NuBlc");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");

                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal, 
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtAnt, _rechamadaNumPv, _rechamadaCdBnd,
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
                            ref _rechamadaDtAnt,
                            ref _rechamadaNumPv,
                            ref _rechamadaCdBnd,
                            ref _rechamadaNuBlc,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataInicial, _dataFinal, 
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaDtAnt, _rechamadaNumPv, _rechamadaCdBnd,
                            _rechamadaNuBlc, _resto, _areaFixa });
                    }

                    rechamada["DtAnt"] = _rechamadaDtAnt;
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

        public RAVDetalheTotalizador ConsultarDetalheTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataAntecipacao,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Totalizadores - Detalhe - WACA1332"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1332";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataAntecipacao = dataAntecipacao.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);
                    String _areaFixa = TR.ConsultarDetalheTotalizadoresEntrada(codigoBandeira, pvs);

                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataAntecipacao, 
                            _codRetorno, _msgRetorno, _resto, _areaFixa });

                        client.ConsultarDetalheTotalizadores(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataAntecipacao,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataAntecipacao, 
                            _codRetorno, _msgRetorno, _resto, _areaFixa });
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

        public List<RAVDetalhe> ConsultarDetalhe(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataAntecipacao,            
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Dados - Detalhe - WACA1333"))
            {
                try
                {
                    //Preparação dos dados para chamada HIS
                    String _nomePrograma = "WA1333";
                    String _sistema = "IS";
                    String _usuario = "xxx";
                    String _dataAntecipacao = dataAntecipacao.ToString("dd/MM/yyyy");
                    Int16 _codRetorno = default(Int16);
                    String _msgRetorno = default(String);
                    String _resto = default(String);                    
                    String _areaFixa = TR.ConsultarDetalheEntrada(codigoBandeira, pvs);

                    rechamada = rechamada != null ? rechamada : new Dictionary<String, Object>();
                    Int32 _rechamadaCdCtr = rechamada.GetValueOrDefault<Int32>("CdCtr");
                    String _rechamadaDtApr = rechamada.GetValueOrDefault<String>("DtApr");
                    String _rechamadaDtVct = rechamada.GetValueOrDefault<String>("DtVct");
                    Int32 _rechamadaNumPv = rechamada.GetValueOrDefault<Int32>("NumPv");
                    Int32 _rechamadaNumRv = rechamada.GetValueOrDefault<Int32>("NumRv");
                    Int16 _rechamadaNuBlc = rechamada.GetValueOrDefault<Int16>("NuBlc");
                    String _rechamadaIndicador = rechamada.GetValueOrDefault<String>("Indicador");
                    String _rechamadaCodCont = rechamada.GetValueOrDefault<String>("CodCont");
                    String _rechamadaTmsAj = rechamada.GetValueOrDefault<String>("TmsAj");

                    using (COMTIWAClient client = new COMTIWAClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaHIS, new { _nomePrograma, _sistema, _usuario, _dataAntecipacao, 
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaCdCtr, _rechamadaDtApr, 
                            _rechamadaDtVct, _rechamadaNumPv, _rechamadaNumRv, _rechamadaNuBlc, _resto, _areaFixa });

                        client.ConsultarDetalhe(
                            ref _nomePrograma,
                            ref _sistema,
                            ref _usuario,
                            ref _dataAntecipacao,
                            ref _codRetorno,
                            ref _msgRetorno,
                            ref _rechamadaIndicador,
                            ref _rechamadaCdCtr,
                            ref _rechamadaDtApr,
                            ref _rechamadaDtVct,
                            ref _rechamadaNumPv,
                            ref _rechamadaNumRv,
                            ref _rechamadaNuBlc,
                            ref _rechamadaCodCont,
                            ref _rechamadaTmsAj,
                            ref _resto,
                            ref _areaFixa);

                        Log.GravarLog(EventoLog.RetornoHIS, new { _nomePrograma, _sistema, _usuario, _dataAntecipacao, 
                            _codRetorno, _msgRetorno, _rechamadaIndicador, _rechamadaCdCtr, _rechamadaDtApr, 
                            _rechamadaDtVct, _rechamadaNumPv, _rechamadaNumRv, _rechamadaNuBlc, _rechamadaCodCont, _rechamadaTmsAj, _resto, _areaFixa });
                    }

                    rechamada["CdCtr"] = _rechamadaCdCtr;
                    rechamada["DtApr"] = _rechamadaDtApr;
                    rechamada["DtVct"] = _rechamadaDtVct;
                    rechamada["NumPv"] = _rechamadaNumPv;
                    rechamada["NumRv"] = _rechamadaNumRv;
                    rechamada["NuBlc"] = _rechamadaNuBlc;
                    rechamada["Indicador"] = _rechamadaIndicador;
                    rechamada["CodCont"] = _rechamadaCodCont;
                    rechamada["TmsAj"] = _rechamadaTmsAj;

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