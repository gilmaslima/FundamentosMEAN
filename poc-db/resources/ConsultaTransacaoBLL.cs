using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Modelo;
using AG = Redecard.PN.Extrato.Agentes.ConsultaTransacaoAG;
using Redecard.PN.Extrato.Modelo.ConsultaTransacao;
using System.Reflection;

namespace Redecard.PN.Extrato.Negocio
{
    public class ConsultaTransacaoBLL : RegraDeNegocioBase
    {
        private static ConsultaTransacaoBLL _Instancia;
        public static ConsultaTransacaoBLL Instancia { get { return _Instancia ?? (_Instancia = new ConsultaTransacaoBLL()); } }

        public List<Debito> ConsultarDebito(
            Int32 numeroPV,
            DateTime dataInicial,
            DateTime dataFinal,
            String numeroCartao,
            Int64 nsuAcquirer,
            ref Dictionary<String,Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Dados - Débito - MEC084CO"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { numeroPV, dataInicial, dataFinal, numeroCartao, nsuAcquirer, rechamada });

                    var _retorno = AG.Instancia.ConsultarDebito(
                        numeroPV,
                        dataInicial,
                        dataFinal,
                        numeroCartao,
                        nsuAcquirer,
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

        public List<Credito> ConsultarCredito(
            Int32 numeroPV,
            DateTime dataInicial,
            DateTime dataFinal,
            String numeroCartao,
            Int64 nsu,
            ref Dictionary<String,Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Dados - Crédito - MEC119CO"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { numeroPV, dataInicial, dataFinal, numeroCartao, nsu, rechamada });

                    var _retorno = AG.Instancia.ConsultarCredito(
                        numeroPV,
                        dataInicial,
                        dataFinal,
                        numeroCartao,
                        nsu,
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

        public CreditoTID ConsultarCreditoTID(String idDataCash, Int32 numeroPV, out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Dados - Crédito TID - MEC323CO"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { idDataCash, numeroPV });

                    var _retorno = AG.Instancia.ConsultarCreditoTID(
                        idDataCash,                        
                        out status);
                    
                    Log.GravarLog(EventoLog.RetornoAgente, new { _retorno, status });

                    //Regra: se o número do PV for diferente do número do PV da TID, 
                    //o serviço não deve retornar os dados da TID
                    if (_retorno != null && _retorno.NumeroPV != 0 && _retorno.NumeroPV != numeroPV)
                    {
                        Log.GravarMensagem("TID encontrada, porém não pertence ao PV informado");
                        status = new StatusRetornoDTO(20, "NUMERO ID DATACASH NAO INFORMADO OU INVALIDO", FONTE_METODO);
                        _retorno = null; //limpa o objeto
                    }

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

        public DebitoTID ConsultarDebitoTID(String idDataCash, Int32 numeroPV, out StatusRetornoDTO status)
        {
            String FONTE_METODO = String.Concat(this.GetType().Name, ".", MethodInfo.GetCurrentMethod().Name);

            using (Logger Log = Logger.IniciarLog("Consulta Dados - Débito TID - MEC324CO"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { idDataCash, numeroPV });

                    var _retorno = AG.Instancia.ConsultarDebitoTID(
                        idDataCash,                        
                        out status);

                    Log.GravarLog(EventoLog.RetornoAgente, new { _retorno, status });

                    //Regra: se o número do PV for diferente do número do PV da TID, 
                    //o serviço não deve retornar os dados da TID
                    if (_retorno != null && _retorno.NumeroPV != 0 && _retorno.NumeroPV != numeroPV)
                    {
                        Log.GravarMensagem("TID encontrada, porém não pertence ao PV informado");
                        status = new StatusRetornoDTO(20, "NUMERO ID DATACASH NAO INFORMADO OU INVALIDO", FONTE_METODO);
                        _retorno = null; //limpa o objeto
                    }

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
