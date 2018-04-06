using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Agentes;
using Redecard.PN.Extrato.Modelo.Comum;
using Redecard.PN.Extrato.Modelo;
using Redecard.PN.Extrato.Modelo.LancamentosFuturos;
using AG = Redecard.PN.Extrato.Agentes.LancamentosFuturosAG;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Redecard.PN.Extrato.Negocio
{
    public class LancamentosFuturosBLL : RegraDeNegocioBase
    {
        private static LancamentosFuturosBLL _Instancia;
        public static LancamentosFuturosBLL Instancia { get { return _Instancia ?? (_Instancia = new LancamentosFuturosBLL()); } }

        public CreditoTotalizador ConsultarCreditoTotalizadores(            
            Int32 codigoBandeira,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Totalizadores - WACA1324"))
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

                    Log.GravarLog(EventoLog.RetornoAgente, new { retorno = _retorno, status });

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
            using (Logger Log = Logger.IniciarLog("Consulta Crédito - WACA1325"))
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
            List<Int32> pvs,
            DateTime dataVencimento,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Detalhe - Totalizadores - WACA1326"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { codigoBandeira, pvs, dataVencimento });

                    var _retorno = AG.Instancia.ConsultarCreditoDetalheTotalizadores(                        
                        codigoBandeira,
                        pvs,
                        dataVencimento,
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

        public CreditoDetalheTotalizador ConsultarCreditoDetalheTodosTotalizadores(                       
            List<Int32> codigosBandeiras,
            List<Int32> pvs,
            DateTime dataInicial,
            DateTime dataFinal,            
            out StatusRetornoDTO status)
        {
            //Variável auxiliar
            var _status = new StatusRetornoDTO(0, String.Empty, FONTE);
            
            //Prepara os parâmetros de chamada (por Bandeira, por dia)
            var _parametrosChamada = new List<Tuple<Int32, DateTime>>();

            //Percorre por bandeira da lista de bandeiras considerada
            foreach (Int32 _codigoBandeira in codigosBandeiras)
                //Percorre a consulta para cada dia do período
                for (DateTime _dataCorrente = dataInicial; _dataCorrente <= dataFinal; _dataCorrente = _dataCorrente.AddDays(1))
                    _parametrosChamada.Add(Tuple.Create(_codigoBandeira, _dataCorrente));

            //Coleção thread-safe para armazenamento dos resultados e exceções das threads
            var _dados = new ConcurrentQueue<CreditoDetalheTotalizador>();
            var _excecoes = new ConcurrentQueue<Exception>();

            //Inicia a consulta multi-thread para os parâmetros de chamada preparados anteriormente
            Parallel.ForEach(_parametrosChamada, new ParallelOptions { MaxDegreeOfParallelism = 30 }, parametroChamada =>
            {
                try
                {
                    //Variáveis auxiliares para a chamada atual
                    StatusRetornoDTO _statusTotalizadorDTO;
                    Int32 _codigoBandeira = parametroChamada.Item1;
                    DateTime _dataVencimento = parametroChamada.Item2;

                    //Realiza pesquisa para o período desejado
                    var dadosConsulta = ConsultarCreditoDetalheTotalizadores(                        
                        _codigoBandeira,
                        pvs,
                        _dataVencimento,
                        out _statusTotalizadorDTO);

                    //Se consulta foi realizada com sucesso, adiciona resultado na lista thread-safe
                    if (_statusTotalizadorDTO.CodigoRetorno == 0)
                        _dados.Enqueue(dadosConsulta);
                    //Se ocorreu um erro na consulta, armazena status retornado
                    else if (_status == null)
                        _status = _statusTotalizadorDTO;
                }
                catch (Exception ex)
                {
                    _excecoes.Enqueue(ex);
                }
            });         

            //Caso tenha ocorrido alguma exceção, lança exceções geradas
            if(_excecoes.Count > 0) throw new PortalRedecardException(CODIGO_ERRO, FONTE, new AggregateException(_excecoes));

            //Consolida os dados dos totalizadores por dia, em um totalizador único para o período
            var _totalizadoresDia = _dados.Where(result => result != null).ToArray();

            //Instanciação das variáveis de retorno
            var _totalizador = new CreditoDetalheTotalizador();
            status = _status;

            //Totaliza o Valor Bruto Venda
            _totalizador.Totais.TotalValorBrutoVenda = _totalizadoresDia
                .Sum(totalizadorDia => totalizadorDia.Totais.TotalValorBrutoVenda);

            //Totaliza o Valor Descontado
            _totalizador.Totais.TotalValorDescontado = _totalizadoresDia
                .Sum(totalizadorDia => totalizadorDia.Totais.TotalValorDescontado);
            
            //Totaliza o Valor Líquido
            _totalizador.Totais.TotalValorLiquido = _totalizadoresDia
                .Sum(totalizadorDia => totalizadorDia.Totais.TotalValorLiquido);

            //Totaliza por Bandeira
            _totalizador.Valores = _totalizadoresDia
                .SelectMany(totalizadorDia => totalizadorDia.Valores)
                .GroupBy(totalizadorDia => new { Bandeira = totalizadorDia.TipoBandeira })
                .Select(grupoTotalizador => new CreditoDetalheTotalizadorValor
                {
                    TipoBandeira = grupoTotalizador.Key.Bandeira,
                    ValorBruto = grupoTotalizador.Sum(totalizadorBandeiraDia => totalizadorBandeiraDia.ValorBruto),
                    ValorLiquido = grupoTotalizador.Sum(totalizadorBandeiraDia => totalizadorBandeiraDia.ValorLiquido)
                }).ToList();

            return _totalizador;
        }

        public List<CreditoDetalhe> ConsultarCreditoDetalhe(            
            DateTime dataVencimento,
            Int32 codigoBandeira,
            List<Int32> pvs,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Crédito - Detalhe - WACA1327"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { dataVencimento, codigoBandeira, pvs, rechamada });

                    var _retorno = AG.Instancia.ConsultarCreditoDetalhe(                        
                        dataVencimento,
                        codigoBandeira,
                        pvs,
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
            using (Logger Log = Logger.IniciarLog("Consulta Débito - Totalizadores - WACA1328"))
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
            DateTime dataInicial,
            DateTime dataFinal,
            Int32 codigoBandeira,
            List<Int32> pvs,
            ref Dictionary<String, Object> rechamada,
            out Boolean indicadorRechamada,
            out StatusRetornoDTO status)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Débito - WACA1329"))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaAgente, new { dataInicial, dataFinal, codigoBandeira, pvs, rechamada });

                    var _retorno = AG.Instancia.ConsultarDebito(                        
                        dataInicial,
                        dataFinal,
                        codigoBandeira,
                        pvs,
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
    }
}