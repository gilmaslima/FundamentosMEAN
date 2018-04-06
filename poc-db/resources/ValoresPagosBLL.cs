using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Modelo.ValoresPagos;
using AG = Redecard.PN.Extrato.Agentes.ValoresPagosAG;

namespace Redecard.PN.Extrato.Negocio
{
    public class ValoresPagosBLL : RegraDeNegocioBase
    {
        private static ValoresPagosBLL _Instancia;
        public static ValoresPagosBLL Instancia { get { return _Instancia ?? (_Instancia = new ValoresPagosBLL()); } }

        public CreditoTotalizador ConsultarCreditoTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Totalizadores - WACA1316"))
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

        public List<Credito> ConsultarCredito(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Crédito - WACA1317"))
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

        public CreditoDetalheTotalizador ConsultarCreditoDetalheTotalizadores(            
            Int32 codigoBandeira,
            Int32 pv,
            DateTime dataRecebimento,
            Int32 numeroOcu,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Detalhe - Totalizadores - WACA1318"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pv, dataRecebimento, numeroOcu });

                    var _retorno = AG.Instancia.ConsultarCreditoDetalheTotalizadores(                        
                        codigoBandeira,
                        pv,
                        dataRecebimento,
                        numeroOcu,
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

        public List<CreditoDetalhe> ConsultarCreditoDetalhe(            
            Int32 codigoBandeira,
            Int32 pv,
            DateTime dataRecebimento,
            Int32 numeroOcu,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Detalhe - WACA1319"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pv, dataRecebimento, numeroOcu, rechamada });

                    var _retorno = AG.Instancia.ConsultarCreditoDetalhe(                        
                        codigoBandeira,
                        pv,
                        dataRecebimento,
                        numeroOcu,
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

        public DebitoTotalizador ConsultarDebitoTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Débito - Totalizadores - WACA1320"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pvs, dataInicial, dataFinal });

                    var _retorno = AG.Instancia.ConsultarDebitoTotalizadores(                        
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

        public List<Debito> ConsultarDebito(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Débito - WACA1321"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pvs, dataInicial, dataFinal, rechamada });

                    var _retorno = AG.Instancia.ConsultarDebito(                        
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

        public DebitoDetalheTotalizador ConsultarDebitoDetalheTotalizadores(            
            Int32 pv,
            DateTime dataPagamento,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Débito - Detalhe - Totalizadores - WACA1322"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { pv, dataPagamento });

                    var _retorno = AG.Instancia.ConsultarDebitoDetalheTotalizadores(                        
                        pv,
                        dataPagamento,
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

        public List<DebitoDetalhe> ConsultarDebitoDetalhe(            
            Int32 pv,
            DateTime dataPagamento,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Débito - Detalhe - WACA1323"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { pv, dataPagamento });

                    var _retorno = AG.Instancia.ConsultarDebitoDetalhe(                        
                        pv,
                        dataPagamento,
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
    }
}
