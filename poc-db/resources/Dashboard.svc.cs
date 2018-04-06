using Redecard.PN.Comum;
using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;
using WAExtratoVendasServico = Redecard.PN.Extrato.SharePoint.WAExtratoVendasServico;
using WAExtratoValoresPagosServico = Redecard.PN.Extrato.SharePoint.WAExtratoValoresPagosServico;
using WAExtratoAntecipacaoRAVServico = Redecard.PN.Extrato.SharePoint.WAExtratoAntecipacaoRAVServico;
using WAExtratoLancamentosFuturosServico = Redecard.PN.Extrato.SharePoint.WAExtratoLancamentosFuturosServico;
using System.Web.Script.Serialization;

namespace Rede.PN.HomePage.SharePoint {
    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class DashboardWCF : IDashboard {
        /// <summary>
        /// 
        /// </summary>
        public String NomeOperacao { get { return "Relatório - Consultar Dashboard"; } }
        /// <summary>
        /// 
        /// </summary>
        public object lockObject = new object();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Guid IdentificadorPesquisa(Int32 estabelecimento, string identCache) {
            Guid identificador = Guid.Empty;
            lock (lockObject) {
                var estabelecimentoCacheFormat = String.Format("{0}{1}", identCache, estabelecimento);
                var estabelecimentoCacheGuidFormat = estabelecimentoCacheFormat.PadLeft(32, '0');
                identificador = new Guid(estabelecimentoCacheGuidFormat);
            }
            return identificador;
        }
        /// <summary>
        /// 
        /// </summary>
        public Int32 Estabelecimento {
            get {
                Sessao sessaoUsuario = System.Web.HttpContext.Current.Session[Sessao.ChaveSessao] as Sessao;
                if (sessaoUsuario != null) {
                    return sessaoUsuario.CodigoEntidade;
                }
                throw new Exception("Número do estabelecimento inválido");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Valor> ConsultarTotalizadoresVendasCreditoAsync(Int32 estabelecimento) {
            return await Task.Run(() => {
                using (Logger Log = Logger.IniciarLog("ConsultarTotalizadoresVendasCreditoAsync")) {

                    Log.GravarLog(EventoLog.ChamadaServico);
                    DateTime atual = DateTime.Now;
                    DateTime ultimos5dias = atual.AddDays(-5);

                    var statusRetorno = new WAExtratoVendasServico.StatusRetorno();
                    Valor vendasObject = null;

                    using (var contexto = new ContextoWCF<WAExtratoVendasServico.HISServicoWA_Extrato_VendasClient>()) {
                        var totalizadores = contexto.Cliente.ConsultarCreditoTotalizadores(out statusRetorno, this.IdentificadorPesquisa(estabelecimento, "100"), 0, ultimos5dias, atual, new Int32[] { estabelecimento });
                        if (statusRetorno.CodigoRetorno == 0) {
                            vendasObject = new Valor() {
                                ValorBruto = (decimal)totalizadores.ValorApresentado,
                                ValorLiquido = (decimal)totalizadores.ValorLiquido
                            };
                        } else {
                            // retornou um código diferente de 0, não necessariamente um problema, pode acontecer de
                            // o estabelecimento não ter dados disponíveis
                            vendasObject = new Valor() {
                                ValorBruto = 0,
                                ValorLiquido = 0
                            };
                        }
                    }
                    Log.GravarLog(EventoLog.FimServico);
                    return vendasObject;
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Valor> ConsultarTotalizadoresVendasDebitoAsync(Int32 estabelecimento) {
            return await Task.Run(() => {
                using (Logger Log = Logger.IniciarLog("ConsultarTotalizadoresVendasDebitoAsync")) {

                    Log.GravarLog(EventoLog.ChamadaServico);
                    DateTime atual = DateTime.Now;
                    DateTime ultimos5dias = atual.AddDays(-5);

                    var statusRetorno = new WAExtratoVendasServico.StatusRetorno();
                    var modalidade = WAExtratoVendasServico.Modalidade.Todos;
                    Valor vendasObject = null;

                    using (var contexto = new ContextoWCF<WAExtratoVendasServico.HISServicoWA_Extrato_VendasClient>()) {
                        var totalizadores = contexto.Cliente.ConsultarDebitoTotalizadores(out statusRetorno, this.IdentificadorPesquisa(estabelecimento, "101"), 0, ultimos5dias, atual, modalidade, new Int32[] { estabelecimento });
                        if (statusRetorno.CodigoRetorno == 0) {
                            vendasObject = new Valor() {
                                ValorBruto = (decimal)totalizadores.ValorApresentado,
                                ValorLiquido = (decimal)totalizadores.ValorLiquido
                            };
                        } else {
                            // retornou um código diferente de 0, não necessariamente um problema, pode acontecer de
                            // o estabelecimento não ter dados disponíveis
                            vendasObject = new Valor() {
                                ValorBruto = 0,
                                ValorLiquido = 0
                            };
                        }
                    }
                    Log.GravarLog(EventoLog.FimServico);
                    return vendasObject;
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Valor> ConsultarTotalizadoresValoresPagosCreditoAsync(Int32 estabelecimento) {
            return await Task.Run(() => {
                using (Logger Log = Logger.IniciarLog("ConsultarTotalizadoresValoresPagosCreditoAsync")) {

                    Log.GravarLog(EventoLog.ChamadaServico);
                    DateTime atual = DateTime.Now;
                    DateTime ultimos5dias = atual.AddDays(-5);

                    var statusRetorno = new WAExtratoValoresPagosServico.StatusRetorno();
                    Valor valorObject = null;

                    using (var contexto = new ContextoWCF<WAExtratoValoresPagosServico.HISServicoWA_Extrato_ValoresPagosClient>()) {
                        var totalizadores = contexto.Cliente.ConsultarCreditoTotalizadores(out statusRetorno, this.IdentificadorPesquisa(estabelecimento, "110"), 0, ultimos5dias, atual, new Int32[] { estabelecimento });
                        if (statusRetorno.CodigoRetorno == 0) {
                            valorObject = new Valor() {
                                ValorLiquido = (decimal)totalizadores.TotalValorLiquido,
                                ValorBruto = 0
                            };
                        } else {
                            // retornou um código diferente de 0, não necessariamente um problema, pode acontecer de
                            // o estabelecimento não ter dados disponíveis
                            valorObject = new Valor() {
                                ValorBruto = 0,
                                ValorLiquido = 0
                            };
                        }
                    }
                    Log.GravarLog(EventoLog.FimServico);
                    return valorObject;
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Valor> ConsultarTotalizadoresValoresPagosDebitoPreDatadoAsync(Int32 estabelecimento) {
            return await Task.Run(() => {
                using (Logger Log = Logger.IniciarLog("ConsultarTotalizadoresValoresPagosDebitoPreDatadoAsync")) {

                    Log.GravarLog(EventoLog.ChamadaServico);
                    DateTime atual = DateTime.Now;
                    DateTime ultimos5dias = atual.AddDays(-5);

                    var statusRetorno = new WAExtratoValoresPagosServico.StatusRetorno();
                    Valor valorObject = null;

                    using (var contexto = new ContextoWCF<WAExtratoValoresPagosServico.HISServicoWA_Extrato_ValoresPagosClient>()) {
                        var totalizadores = contexto.Cliente.ConsultarDebitoTotalizadores(out statusRetorno, this.IdentificadorPesquisa(estabelecimento, "111"), 0, ultimos5dias, atual, new Int32[] { estabelecimento });
                        if (statusRetorno.CodigoRetorno == 0) {
                            valorObject = new Valor() {
                                ValorLiquido = (decimal)totalizadores.Totais.TotalValorLiquido,
                                ValorBruto = 0
                            };
                        } else {
                            // retornou um código diferente de 0, não necessariamente um problema, pode acontecer de
                            // o estabelecimento não ter dados disponíveis
                            valorObject = new Valor() {
                                ValorBruto = 0,
                                ValorLiquido = 0
                            };
                        }
                    }
                    Log.GravarLog(EventoLog.FimServico);
                    return valorObject;
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Valor> ConsultarTotalizadoresAntecipacoesAsync(Int32 estabelecimento) {
            return await Task.Run(() => {
                using (Logger Log = Logger.IniciarLog("ConsultarTotalizadoresAntecipacoesAsync")) {

                    Log.GravarLog(EventoLog.ChamadaServico);
                    DateTime atual = DateTime.Now;
                    DateTime ultimos5dias = atual.AddDays(-5);

                    var statusRetorno = new WAExtratoAntecipacaoRAVServico.StatusRetorno();
                    Valor valorObject = null;

                    using (var contexto = new ContextoWCF<WAExtratoAntecipacaoRAVServico.HISServicoWA_Extrato_AntecipacaoRAVClient>()) {
                        var totalizadores = contexto.Cliente.ConsultarTotalizadores(out statusRetorno, this.IdentificadorPesquisa(estabelecimento, "001"), 0, ultimos5dias, atual, new Int32[] { estabelecimento });
                        if (statusRetorno.CodigoRetorno == 0) {
                            valorObject = new Valor() {
                                ValorLiquido = (decimal)totalizadores.Totais.TotalValorLiquido,
                                ValorBruto = 0
                            };
                        } else {
                            // retornou um código diferente de 0, não necessariamente um problema, pode acontecer de
                            // o estabelecimento não ter dados disponíveis
                            valorObject = new Valor() {
                                ValorBruto = 0,
                                ValorLiquido = 0
                            };
                        }
                    }
                    Log.GravarLog(EventoLog.FimServico);
                    return valorObject;
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Valor> ConsultarTotalizadoresLancamentosFuturosAsync(Int32 estabelecimento) {
            return await Task.Run(() => {
                using (Logger Log = Logger.IniciarLog("ConsultarTotalizadoresLancamentosFuturosAsync")) {

                    Log.GravarLog(EventoLog.ChamadaServico);
                    DateTime atual = DateTime.Now;
                    DateTime proximos5dias = atual.AddDays(5);

                    var statusRetorno = new WAExtratoLancamentosFuturosServico.StatusRetorno();
                    Valor valorObject = null;

                    using (var contexto = new ContextoWCF<WAExtratoLancamentosFuturosServico.HISServicoWA_Extrato_LancamentosFuturosClient>()) {
                        var totalizadores = contexto.Cliente.ConsultarCreditoTotalizadores(out statusRetorno, this.IdentificadorPesquisa(estabelecimento, "010"), 0, atual, proximos5dias, new Int32[] { estabelecimento });
                        if (statusRetorno.CodigoRetorno == 0) {
                            valorObject = new Valor() {
                                ValorLiquido = (decimal)totalizadores.Totais.ValorLiquido,
                                ValorBruto = 0
                            };
                        }
                        else {
                            // retornou um código diferente de 0, não necessariamente um problema, pode acontecer de
                            // o estabelecimento não ter dados disponíveis
                            valorObject = new Valor() {
                                ValorBruto = 0,
                                ValorLiquido = 0
                            };
                        }
                    }
                    Log.GravarLog(EventoLog.FimServico);
                    return valorObject;
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<Dashboard> Summary() {
            using (Logger Log = Logger.IniciarLog(NomeOperacao)) {
                try {
                    Log.GravarMensagem("Início da consulta dos Extratos da Dashboard");

                    Int32 estabelecimento = this.Estabelecimento;

                    var totalizadoresVendasCredito = ConsultarTotalizadoresVendasCreditoAsync(estabelecimento);
                    var totalizadoresVendasDebito = ConsultarTotalizadoresVendasDebitoAsync(estabelecimento);
                    var totalizadoresValoresPagosCredito = ConsultarTotalizadoresValoresPagosCreditoAsync(estabelecimento);
                    var totalizadoresValoresPagosDebitoPreDatado = ConsultarTotalizadoresValoresPagosDebitoPreDatadoAsync(estabelecimento);
                    var totalizadoresAntecipacoes = ConsultarTotalizadoresAntecipacoesAsync(estabelecimento);
                    var totalizadoresLancamentosFuturosCredito = ConsultarTotalizadoresLancamentosFuturosAsync(estabelecimento);

                    // tratamento de erro: https://msdn.microsoft.com/pt-br/library/hh160374(v=vs.110).aspx
                    return await Task.WhenAll(totalizadoresVendasCredito, totalizadoresVendasDebito, totalizadoresValoresPagosCredito,
                        totalizadoresValoresPagosDebitoPreDatado, totalizadoresAntecipacoes, totalizadoresLancamentosFuturosCredito).ContinueWith(state => {
                            var response = new Dashboard() {
                                Vendas = new Vendas {
                                    Credito = totalizadoresVendasCredito.Result,
                                    Debito = totalizadoresVendasDebito.Result
                                },
                                LancamentosFuturos = new LancamentosFuturos() {
                                    Credito = totalizadoresLancamentosFuturosCredito.Result
                                },
                                ValoresPagos = new ValoresPagos() {
                                    Antecipacoes = totalizadoresAntecipacoes.Result,
                                    Credito = totalizadoresValoresPagosCredito.Result,
                                    DebitoPreDatado = totalizadoresValoresPagosDebitoPreDatado.Result,
                                    //Apesar de Valores Pagos, essa propriedade é obtida no serviço de Vendas e é igual ao valor do Vendas (O que vendi) Débito Líquido
                                    Debito = totalizadoresVendasDebito.Result
                                }
                            };
                            return response;
                        });
                }
                catch (FaultException e) {
                    Log.GravarErro(e);
                    throw new Exception(String.Format("Ocorreu um erro ao processar algum serviço do resumo do Extrato. Detail: {0}. Stack Trace: {1}", e.Message, e.StackTrace), e);
                }
                catch (ArgumentNullException e) {
                    Log.GravarErro(e);
                    throw new Exception(String.Format("A lista de tarefas para o resumo do Extrato é nulo. Detail: {0}. Stack Trace: {1}", e.Message, e.StackTrace), e);
                }
                catch (ArgumentException e) {
                    Log.GravarErro(e);
                    throw new Exception(String.Format("Algum item da lista de tarefas para o resumo do Extrato é nulo. Detail: {0}. Stack Trace: {1}", e.Message, e.StackTrace), e);
                }
                catch (Exception e) {
                    Log.GravarErro(e);
                    throw new Exception(String.Format("Ocorreu um erro ao processar o resumo do Extrato. Detail: {0}. Stack Trace: {1}", e.Message, e.StackTrace), e);
                }
            }
        }
    }
}