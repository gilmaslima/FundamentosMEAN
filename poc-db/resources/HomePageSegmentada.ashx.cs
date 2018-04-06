/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using Redecard.PN.Comum;
using Redecard.PN.Comum.TrataErroServico;
using Redecard.PN.Extrato.SharePoint.ModuloRAV;

namespace Redecard.PN.Extrato.SharePoint.Handlers
{
    /// <summary>
    /// Handler utilizado nas requisições assíncronas da HomePage Segmentada (Varejo e EMP/IBBA)
    /// </summary>
    public partial class HomePageSegmentada : UserControlBase, IHttpHandler, IReadOnlySessionState
    {   
        /// <summary>
        /// IsReusable
        /// </summary>
        public Boolean IsReusable { get { return false; } }

        /// <summary>
        /// Culture pt-BR
        /// </summary>
        private static CultureInfo PtBr { get { return new CultureInfo("pt-BR"); } }

        /// <summary>
        /// Random
        /// </summary>
        private static Random Random { get { return new Random(); } }

        /// <summary>
        /// ProcessRequest
        /// </summary>
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                //Se não está autenticado, gera mensagem de erro
                if (!Sessao.Contem())
                {
                    ProcessarErro(context, String.Empty, null);
                    return;
                }

#if DEBUG
                System.Threading.Thread.Sleep(Random.Next(1000, 5000));
#endif

                //Recuperação do PV da sessão
                Int32 pv = SessaoAtual.CodigoEntidade;

                //Obtém o tipo de HomePage da origem da requisição ("empibba" ou "varejo")
                String tipoHomePage = context.Request["home"];
                
                //Obtém qual consulta deve ser realizada 
                // - V (vendas) 
                // - LF (lançamentos futuros)
                // - VP (valores pagos)
                // - RAV (antecipação)
                // - P (pendentes)
                // - FS (Situação Fidelidade)
                // - FAuth (Fidelidade Autorização)
                String consulta = context.Request["consulta"];

                //Obtém tipo da venda
                //C (crédito) ou D (débito)
                String tipoVenda = context.Request["tipoVenda"];

                //Obtém o período da consulta
                DateTime? dataInicial = context.Request["dataInicial"].ToDateTimeNull("dd/MM/yyyy");
                DateTime? dataFinal = context.Request["dataFinal"].ToDateTimeNull("dd/MM/yyyy");

                //Roteia para handlers de consultas da HomePage Segmentada - Varejo
                if (String.Compare(tipoHomePage, "varejo", true) == 0)
                {
                    if (String.Compare(consulta, "RAV", true) == 0)
                    {
                        //Trata consulta RAV
                        ProcessarValorRAV(context, pv, this.SessaoAtual.StatusPVCancelado());
                    }
                    else if (String.Compare(consulta, "FS", true) == 0)
                    {
                        //Trata consulta de Status Fidelidade do Pv
                        ProcessarStatusFidelidadePv(context, pv, this.SessaoAtual.TipoUsuario);
                    }
                    else if (String.Compare(consulta, "FAuth", true) == 0)
                    {
                        //Trata solicitação de criação de autorização para usuário Fielo
                        ProcessarAutoricaoUsuarioFielo(context, this.SessaoAtual.TipoUsuario, pv, this.SessaoAtual.CNPJEntidade);
                    }
                    else if (String.Compare(consulta, "VP", true) == 0)
                    {
                        //Trata consulta Valores Pagos Crédito
                        ProcessarVarejoValoresPagos(context, pv, dataInicial, dataFinal);
                    }
                    else if (String.Compare(consulta, "LF", true) == 0)
                    {
                        //Trata consulta Lançamentos Futuros Crédito
                        ProcessarVarejoLancamentosFuturos(context, pv, dataInicial, dataFinal);
                    }
                    else if (String.Compare(consulta, "V", true) == 0)
                    {
                        //Trata consulta Vendas Crédito
                        if (String.Compare(tipoVenda, "C", true) == 0)
                            ProcessarVarejoVendasCredito(context, pv, dataInicial, dataFinal);
                        //Trata consulta Vendas Débito
                        else if (String.Compare(tipoVenda, "D", true) == 0)
                            ProcessarVarejoVendasDebito(context, pv, dataInicial, dataFinal);
                    }
                }
                //Roteia para handlers de consultas da HomePage Segmentada - EMP/IBBA
                else if (String.Compare(tipoHomePage, "empibba", true) == 0)
                {                    
                    if (String.Compare(consulta, "RAV", true) == 0)
                    {
                        //Trata consulta RAV
                        ProcessarValorRAV(context, pv, this.SessaoAtual.StatusPVCancelado());
                    }
                    else if (String.Compare(consulta, "FS", true) == 0)
                    {
                        //Trata consulta de Status Fidelidade do Pv
                        ProcessarStatusFidelidadePv(context, pv, this.SessaoAtual.TipoUsuario);
                    }
                    else if (String.Compare(consulta, "FAuth", true) == 0)
                    {
                        //Trata solicitação de criação de autorização para usuário Fielo
                        ProcessarAutoricaoUsuarioFielo(context, this.SessaoAtual.TipoUsuario, pv, this.SessaoAtual.CNPJEntidade);
                    }
                    else if (String.Compare(consulta, "P", true) == 0)
                    {
                        //Trata consulta Pendentes
                        ProcessarEmpIbbaVendasPendentes(context, pv);
                    }
                    else if (String.Compare(consulta, "V", true) == 0)
                    {
                        //Trata consulta Transação Crédito
                        if (String.Compare(tipoVenda, "C", true) == 0)
                        {
                            Boolean nsuCv = String.Compare(context.Request["nsuCv"], "true", true) == 0;
                            Boolean tid = String.Compare(context.Request["tid"], "true", true) == 0;
                            Boolean cartao = String.Compare(context.Request["cartao"], "true", true) == 0;
                            String numero = context.Request["numero"];
                            ProcessarEmpIbbaTransacaoCredito(context, pv, dataInicial, dataFinal, nsuCv, tid, cartao, numero);
                        }
                        //Trata consulta Transação Débito
                        else if (String.Compare(tipoVenda, "D", true) == 0)
                        {
                            Boolean nsuCv = String.Compare(context.Request["nsuCv"], "true", true) == 0;
                            Boolean tid = String.Compare(context.Request["tid"], "true", true) == 0;
                            Boolean cartao = String.Compare(context.Request["cartao"], "true", true) == 0;
                            String numero = context.Request["numero"];
                            ProcessarEmpIbbaTransacaoDebito(context, pv, dataInicial, dataFinal, nsuCv, tid, cartao, numero);
                        }
                    }
                }
            }
            catch (PortalRedecardException exc)
            {
                SharePointUlsLog.LogErro(exc);
                ProcessarErro(context, exc.Fonte, exc.Codigo);
            }
            catch (Exception exc)
            {
                SharePointUlsLog.LogErro(exc);
                ProcessarErro(context, FONTE, CODIGO_ERRO);
            }
            finally
            {
                context.Response.ContentType = "application/json";
                context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            }
        }

        /// <summary>
        /// Serialização Javascript
        /// </summary>
        private static String Serializar(Object obj)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        #region [ Processamento HomePage Segmentada - Varejo ]
        
        /// <summary>
        /// Processamento da requisição da HomePage Segmentada - Varejo - Box Vendas Crédito
        /// </summary>
        private static void ProcessarVarejoVendasCredito(HttpContext context, Int32? pv, DateTime? dataInicial, DateTime? dataFinal)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada Varejo: Vendas Crédito - Consulta assíncrona"))
            {
                var status = default(WAExtratoHomePageServico.StatusRetorno);
                var vendas = new List<Object>();
                var footer = default(Object);

                if (pv.HasValue)
                {
                    //Consulta de dados
                    WAExtratoHomePageServico.VendasCredito vendasCredito = 
                        ConsultarVendasCredito(pv.Value, dataInicial.Value, dataFinal.Value, out status);
                        
                    //Prepara registros
                    vendas.AddRange(vendasCredito.Vendas.Select(v => new 
                        {
                            ExibirBandeira = true,
                            Bandeira = v.DescricaoBandeira,
                            CodigoBandeira = v.CodigoBandeira,
                            QuantidadeVendas = v.QuantidadeTransacoes.ToString(),
                            ValorTotal = v.ValorApresentado.ToString("N2", PtBr)
                        }).ToArray());

                    if (vendasCredito.OutrasBandeiras != null)
                    {
                        vendas.Add(new
                        {
                            ExibirBandeira = false,
                            Bandeira = "Outras",
                            CodigoBandeira = 0,
                            QuantidadeVendas = vendasCredito.OutrasBandeiras.QuantidadeTransacoes.ToString(),
                            ValorTotal = vendasCredito.OutrasBandeiras.ValorApresentado.ToString("N2", PtBr)
                        });
                    }

                    //Prepara footer/totalizadores
                    footer = new[] {
                        "Total", 
                        vendasCredito.Totalizador.QuantidadeTransacoes.ToString(), 
                        vendasCredito.Totalizador.ValorApresentado.ToString("N2", PtBr)
                    };
                }

                var retorno = new
                {
                    Vendas = vendas,
                    Totalizador = footer
                };

                //Response contendo as últimas vendas
                context.Response.Write(Serializar(retorno));
            }
        }

        /// <summary>
        /// Processamento da requisição da HomePage Segmentada - Varejo - Box Vendas Débito
        /// </summary>
        private static void ProcessarVarejoVendasDebito(HttpContext context, Int32? pv, DateTime? dataInicial, DateTime? dataFinal)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada Varejo: Vendas Débito - Consulta assíncrona"))
            {
                var status = default(WAExtratoHomePageServico.StatusRetorno);
                var vendas = new List<Object>();
                var footer = default(Object);

                if (pv.HasValue)
                {
                    //Consulta de dados
                    WAExtratoHomePageServico.VendasDebito vendasDebito =
                        ConsultarVendasDebito(pv.Value, dataInicial.Value, dataFinal.Value, out status);

                    //Prepara registros
                    vendas.AddRange(vendasDebito.Vendas.Select(v => new
                    {
                        ExibirBandeira = true,
                        Bandeira = v.DescricaoBandeira,
                        CodigoBandeira = v.CodigoBandeira,
                        QuantidadeVendas = v.QuantidadeTransacoes.ToString(),
                        ValorTotal = v.ValorApresentado.ToString("N2", PtBr)
                    }).ToArray());

                    if (vendasDebito.OutrasBandeiras != null)
                    {
                        vendas.Add(new
                        {
                            ExibirBandeira = false,
                            Bandeira = "Outras",
                            CodigoBandeira = 0,
                            QuantidadeVendas = vendasDebito.OutrasBandeiras.QuantidadeTransacoes.ToString(),
                            ValorTotal = vendasDebito.OutrasBandeiras.ValorApresentado.ToString("N2", PtBr)
                        });
                    }

                    //Prepara footer/totalizadores
                    footer = new[] {
                        "Total", 
                        vendasDebito.Totalizador.QuantidadeTransacoes.ToString(), 
                        vendasDebito.Totalizador.ValorApresentado.ToString("N2", PtBr)
                    };
                }

                var retorno = new
                {
                    Vendas = vendas,
                    Totalizador = footer
                };

                //Response contendo as últimas vendas
                context.Response.Write(Serializar(retorno));
            }
        }

        /// <summary>
        /// Processamento da requisição da HomePage Segmentada - Varejo - Box Lançamentos Futuros
        /// </summary>
        private static void ProcessarVarejoLancamentosFuturos(HttpContext context, Int32? pv, DateTime? dataInicial, DateTime? dataFinal)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada Varejo: Lançamentos Futuros - Consulta assíncrona"))
            {
                var status = default(WAExtratoHomePageServico.StatusRetorno);
                var retorno = default(Object);

                if (pv.HasValue)
                {
                    //Consulta de dados
                    WAExtratoHomePageServico.LancamentosFuturos registros = 
                        ConsultarLancamentosFuturos(pv.Value, dataInicial.Value, dataFinal.Value, out status);

                    retorno = registros.Resumos.Select(rv =>
                        new 
                        { 
                            DataRecebimento = rv.DataRecebimento.ToString("dd/MM/yyyy"),
                            ValorBruto = rv.ValorBruto.ToString("N2", PtBr),
                            ValorLiquido = rv.ValorLiquido.ToString("N2", PtBr)
                        });
                }

                //Response contendo as últimas vendas
                context.Response.Write(Serializar(retorno));
            }
        }

        /// <summary>
        /// Processamento da requisição da HomePage Segmentada - Varejo - Box Valores Pagos
        /// </summary>
        private static void ProcessarVarejoValoresPagos(HttpContext context, Int32? pv, DateTime? dataInicial, DateTime? dataFinal)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada Varejo: Valores Pagos - Consulta assíncrona"))
            {
                var status = default(WAExtratoHomePageServico.StatusRetorno);
                var retorno = default(Object);

                if (pv.HasValue)
                {
                    //Consulta de dados
                    WAExtratoHomePageServico.ValoresPagos registros =
                        ConsultarValoresPagos(pv.Value, dataInicial.Value, dataFinal.Value, out status);

                    retorno = registros.Resumos.Select(rv =>
                        new
                        {
                            DataRecebimento = rv.DataRecebimento.ToString("dd/MM/yyyy"),
                            ValorBruto = rv.ValorBruto.ToString("N2", PtBr),
                            ValorLiquido = rv.ValorLiquido.ToString("N2", PtBr)
                        });
                }

                //Response contendo as últimas vendas
                context.Response.Write(Serializar(retorno));
            }
        }

        #endregion

        #region [ Processamento HomePage Segmentada - EMP/IBBA ]

        /// <summary>
        /// Processamento da requisição da HomePage Segmentada - EMP/IBBA - Box Vendas Pendentes
        /// </summary>
        private static void ProcessarEmpIbbaVendasPendentes(HttpContext context, Int32? pv)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada EMP/IBBA: Vendas - Consulta assíncrona"))
            {
                var retorno = default(Int32);

                if (pv.HasValue)
                    retorno = ConsultarTotalVendasPendentes(pv.Value);

                //Response contendo as últimas vendas
                context.Response.Write(Serializar(retorno));
            }   
        }

        /// <summary>
        /// Processamento da requisição da HomePage Segmentada - EMP/IBBA - Consulta Transação Crédito
        /// </summary>
        private static void ProcessarEmpIbbaTransacaoCredito(HttpContext context, Int32? pv, DateTime? dataInicial, 
            DateTime? dataFinal, Boolean flagNsuCv, Boolean flagTid, Boolean flagCartao, String numero)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada EMP/IBBA: Transação Crédito - Consulta assíncrona"))
            {
                var status = default(MEExtratoConsultaTransacaoServico.StatusRetorno);
                var quantidadeTransacoes = default(Int32);
                var dataTransacao = default(DateTime);
                var valorTransacao = default(Decimal);
                var statusTransacao = default(String);
                var bandeiraTransacao = default(String);
                var autorizacaoTransacao = default(String);
                var urlDetalhamento = default(String);

                if (flagTid)
                {
                    MEExtratoConsultaTransacaoServico.CreditoTID transacaoCreditoTid = 
                        ConsultarPorTransacaoCreditoTID(pv.Value, dataInicial.Value, 
                        dataFinal.Value, numero, out status);

                    if (status.CodigoRetorno == 0)
                    {
                        quantidadeTransacoes = 1;
                        dataTransacao = transacaoCreditoTid.DataTransacao;
                        valorTransacao = transacaoCreditoTid.ValorTransacao;
                        statusTransacao = transacaoCreditoTid.QuantidadeCancelamento > 0 ? "Cancelada" : "Ativa";
                        bandeiraTransacao = transacaoCreditoTid.Bandeira;
                        autorizacaoTransacao = transacaoCreditoTid.AutorizacaoVenda;
                    }
                }
                else
                {
                    MEExtratoConsultaTransacaoServico.Credito[] transacoesCredito = 
                        ConsultarPorTransacaoCredito(pv.Value, dataInicial.Value, 
                        dataFinal.Value, flagNsuCv, flagCartao, numero, out status);

                    if (status.CodigoRetorno == 0)
                    {
                        quantidadeTransacoes = transacoesCredito.Length;
                        if (quantidadeTransacoes == 1)
                        {
                            var transacaoCredito = transacoesCredito.First();
                            dataTransacao = transacaoCredito.DataTransacao;
                            valorTransacao = transacaoCredito.ValorTransacao;
                            statusTransacao = transacaoCredito.QuantidadeCancelamentos > 0 ? "Cancelada" : "Ativa";
                            bandeiraTransacao = transacaoCredito.DescricaoBandeira;
                            autorizacaoTransacao = transacaoCredito.AutorizacaoVenda;
                        }
                        else if (quantidadeTransacoes > 1)
                        {
                            urlDetalhamento = GerarUrlConsultaPorTransacao('C',
                                dataInicial.Value, dataFinal.Value, numero, pv.Value, flagNsuCv, flagCartao, flagTid);
                        }
                    }
                }

                var retorno = new
                {
                    Quantidade = quantidadeTransacoes.ToString(),
                    TipoVenda = "Crédito",
                    Data = dataTransacao.ToString("dd/MM/yyyy"),
                    NsuCv = numero,
                    Status = statusTransacao,
                    Valor = valorTransacao.ToString("C2", PtBr),
                    Bandeira = bandeiraTransacao,
                    Autorizacao = autorizacaoTransacao,
                    Url = urlDetalhamento
                };

                //Response contendo as últimas vendas
                context.Response.Write(Serializar(retorno));
            } 
        }

        /// <summary>
        /// Processamento da requisição da HomePage Segmentada - EMP/IBBA - Consulta Transação Débito
        /// </summary>
        private static void ProcessarEmpIbbaTransacaoDebito(HttpContext context, Int32? pv, DateTime? dataInicial, 
            DateTime? dataFinal, Boolean flagNsuCv, Boolean flagTid, Boolean flagCartao, String numero)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada EMP/IBBA: Transação Débito - Consulta assíncrona"))
            {
                var status = default(MEExtratoConsultaTransacaoServico.StatusRetorno);
                var quantidadeTransacoes = default(Int32);
                var dataTransacao = default(DateTime);
                var valorTransacao = default(Decimal);
                var statusTransacao = default(String);
                var bandeiraTransacao = default(String);
                var autorizacaoTransacao = default(String);
                var urlDetalhamento = default(String);

                if (flagTid)
                {
                    MEExtratoConsultaTransacaoServico.DebitoTID transacaoDebitoTid = 
                        ConsultarPorTransacaoDebitoTID(pv.Value, dataInicial.Value, 
                        dataFinal.Value, numero, out status);
                        
                    if (status.CodigoRetorno == 0)
                    {
                        quantidadeTransacoes = 1;
                        dataTransacao = transacaoDebitoTid.DataTransacao;
                        valorTransacao = transacaoDebitoTid.ValorTransacao;
                        statusTransacao = transacaoDebitoTid.QuantidadeCancelamento > 0 ? "Cancelada" : "Ativa";
                        bandeiraTransacao = transacaoDebitoTid.Bandeira;
                        autorizacaoTransacao = transacaoDebitoTid.NumeroAutorizacaoBanco;
                    }
                }
                else
                {
                    MEExtratoConsultaTransacaoServico.Debito[] transacoesDebito = 
                        ConsultarPorTransacaoDebito(pv.Value, dataInicial.Value, 
                        dataFinal.Value, flagNsuCv, flagCartao, numero, out status);

                    if (status.CodigoRetorno == 0)
                    {
                        quantidadeTransacoes = transacoesDebito.Length;
                        if (quantidadeTransacoes == 1)
                        {
                            var transacaoDebito = transacoesDebito.First();
                            dataTransacao = transacaoDebito.DataTransacao;
                            valorTransacao = transacaoDebito.ValorTransacao;
                            statusTransacao = transacaoDebito.QuantidadeCancelamento > 0 ? "Cancelada" : "Ativa";
                            bandeiraTransacao = transacaoDebito.DescricaoBandeira;
                            autorizacaoTransacao = transacaoDebito.NumeroAutorizacaoBanco;
                        }
                        else if (quantidadeTransacoes > 1)
                        {
                            urlDetalhamento = GerarUrlConsultaPorTransacao('D',
                                dataInicial.Value, dataFinal.Value, numero, pv.Value, flagNsuCv, flagCartao, flagTid);
                        }
                    }
                }

                var retorno = new
                {
                    Quantidade = quantidadeTransacoes.ToString(),
                    TipoVenda = "Débito",
                    Data = dataTransacao.ToString("dd/MM/yyyy"),
                    NsuCv = numero,
                    Status = statusTransacao,
                    Valor = valorTransacao.ToString("C2", PtBr),
                    Bandeira = bandeiraTransacao,
                    Autorizacao = autorizacaoTransacao,
                    Url = urlDetalhamento
                };

                //Response contendo as últimas vendas
                context.Response.Write(Serializar(retorno));
            }
        }

        #endregion

        #region [ Processamento Fidelidade ]

        /// <summary>
        /// Processamento da requisição da HomePage Segmentada - Status da Fidelidade do PV
        /// </summary>
        private static void ProcessarStatusFidelidadePv(HttpContext context, Int32? pv, String tipoUsuario)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada: Status Fidelidade PV - Consulta assíncrona"))
            {
                var statusFidelidade = "NAOATENDE";
                
                //Apenas usuário Master possui acesso ao Programa Fidelidade
                if (String.Compare(tipoUsuario, "M", true) == 0)
                {
                    try
                    {
                        statusFidelidade = ConsultarFidelidadePv(pv.Value);
                    }
                    catch (PortalRedecardException ex)
                    {
                        log.GravarErro(ex);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                    }
                }

                //Response contendo a situação do PV
                context.Response.Write(Serializar(statusFidelidade.ToString()));
            }
        }

        /// <summary>
        /// Processamento da requisição da HomePage Segmentada - Criação da autorização de Usuário Fielo
        /// </summary>
        private static void ProcessarAutoricaoUsuarioFielo(HttpContext context, String tipoUsuario, Int32? pv, String cnpj)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada: Autorização usuário Fielo - Consulta assíncrona"))
            {
                //Apenas se for usuário Master possui acesso ao Programa Fidelidade
                if (String.Compare(tipoUsuario, "M", true) == 0)
                {
                    var hash = Guid.NewGuid().ToString();

                    var parametros = new Dictionary<String, String>();
                    parametros.Add("cnpj", cnpj);
                    parametros.Add("pv", pv.Value.ToString());
                    parametros.Add("hash", hash);

                    var chave = String.Format("FD_{0}", DateTime.Now.ToString("ddMMyyHHmmss"));

                    CacheAdmin.Adicionar(Redecard.PN.Comum.Cache.Fidelidade, chave, parametros.ToList());

                    var retorno = new
                    {
                        Cnpj = cnpj,
                        Pv = pv,
                        Hash = hash,
                        AppFabric = chave,
                        Url = ConfigurationManager.AppSettings["UrlPostFielo"]
                    };

                    //Response contendo os dados da consulta
                    context.Response.Write(Serializar(retorno));
                }
            }
        }

        #endregion

        #region [ Processamento Comum ]

        /// <summary>
        /// Processamento genérico de Erro
        /// </summary>
        private static void ProcessarErro(HttpContext context, String fonte, Int32? codigo)
        {
            String mensagemRetorno = "Sistema indisponível";
            Int32 codigoRetorno = -1;
            if (!String.IsNullOrEmpty(fonte) && codigo.HasValue)
            {
                try
                {
                    using (var ctx = new ContextoWCF<TrataErroServicoClient>())
                    {
                        TrataErro erro = ctx.Cliente.Consultar(fonte, codigo.Value);
                        if (erro != null && erro.Codigo != 0)
                        {
                            mensagemRetorno = erro.Fonte;
                            codigoRetorno = erro.Codigo;
                        }
                    }
                }
                catch (FaultException ex)
                {
                    Logger.GravarErro("Erro durante exibição de erro", ex);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Erro durante exibição de erro", ex);
                }
            }

            var retornoErro = new { Mensagem = mensagemRetorno, Codigo = codigoRetorno };

            //Response contendo o erro
            context.Response.Write(Serializar(retornoErro));
        }

        /// <summary>
        /// Processamento da requisição da HomePage Segmentada - Varejo ou EMP/IBBA - Box RAV.
        /// Deve ser PV Ativo e possuir Valor Disponível maior ou igual a R$50,00.
        /// </summary>
        private static void ProcessarValorRAV(HttpContext context, Int32? pv, Boolean statusPvCancelado)
        {
            String valorRav = String.Empty;

            if (pv.HasValue)
            {
                ModRAVAvulsoSaida dadosRav = ConsultarValorRAV(pv.Value);

                if (dadosRav != null && dadosRav.ValorDisponivel >= 50m && !statusPvCancelado)
                    valorRav = String.Format("{0}*", dadosRav.ValorDisponivel.ToString("C2", PtBr), "*");
            }

            //Response contendo as últimas vendas
            context.Response.Write(Serializar(valorRav));
        }

        #endregion

        #region [ Consultas HomePage Segmentada ]

        /// <summary>
        /// Consulta do Valor RAV disponível
        /// </summary>
        private static ModRAVAvulsoSaida ConsultarValorRAV(Int32 numeroPv)
        {
            //Váriável de retorno
            var objRetorno = default(ModRAVAvulsoSaida);

            using (Logger log = Logger.IniciarLog("HomePage Segmentada: Total de Vendas Pendentes"))
            {
                try
                {
#if DEBUG
                    objRetorno = new ModRAVAvulsoSaida { ValorDisponivel = (Decimal) new Random().NextDouble() * 1000 };
#else
                    using (var ctx = new ContextoWCF<ModuloRAV.ServicoPortalRAVClient>())
                        objRetorno = ctx.Cliente.VerificarRAVDisponivel_Cache(numeroPv);
#endif
                }
                catch (FaultException ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }

            return objRetorno;
        }

        /// <summary>
        /// Consulta do total de vendas pendentes (crédito e débito)
        /// </summary>
        private static Int32 ConsultarTotalVendasPendentes(Int32 numeroPv)
        {
            //Variável de retorno
            var totalPendentes = default(Int32);

            using (Logger log = Logger.IniciarLog("HomePage Segmentada - EMP/IBBA: Total de Vendas Pendentes"))
            {
                try
                {
                    Int16 intCredito = 1;
                    Int16 intDebito = 2;

                    var pendentesCredito = default(Int32);
                    var pendentesDebito = default(Int32);

                    log.GravarMensagem("Inicio Total de Vendas Pendentes Credito", new { numeroPv, intCredito });

                    log.GravarLog(EventoLog.ChamadaServico, numeroPv);
                    using (var ctx = new ContextoWCF<XBChargebackServico.HISServicoXBChargebackClient>())
                        pendentesCredito = ctx.Cliente.ConsultarTotalPendentes(numeroPv, intCredito);
                    log.GravarLog(EventoLog.RetornoServico, pendentesCredito);

                    log.GravarMensagem("Fim Total de Vendas Pendentes Credito", new { pendentesCredito });

                    log.GravarMensagem("Total de Vendas Pendentes Débito", new { numeroPv, intDebito });

                    log.GravarLog(EventoLog.ChamadaServico, numeroPv);
                    using (var ctx = new ContextoWCF<XBChargebackServico.HISServicoXBChargebackClient>())
                        pendentesDebito = ctx.Cliente.ConsultarTotalPendentes(numeroPv, intDebito);
                    log.GravarLog(EventoLog.RetornoServico, pendentesDebito);

                    log.GravarMensagem("Fim Total de Vendas Pendentes Débito", new { pendentesDebito });

                    //Contabiliza o total de vendas pendentes
                    totalPendentes = pendentesCredito + pendentesDebito;

                    log.GravarMensagem("Total de Vendas Pendentes", totalPendentes);
                }
                catch (FaultException<XBChargebackServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(ex.Detail.Codigo, ex.Detail.Fonte, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }

            return totalPendentes;
        }

        /// <summary>
        /// Consulta Vendas a Crédito
        /// </summary>
        private static WAExtratoHomePageServico.VendasCredito ConsultarVendasCredito(
            Int32 numeroPv, DateTime dataInicial, DateTime dataFinal, out WAExtratoHomePageServico.StatusRetorno status)
        {
            //Variável de retorno
            var retorno = default(WAExtratoHomePageServico.VendasCredito);

            using (Logger log = Logger.IniciarLog("HomePage Segmentada - Varejo: Vendas a Crédito"))
            {
                try
                {                    
                    Int32 codigoBandeira = 0;
                    List<Int32> pvs = new List<Int32>(new [] { numeroPv });

                    log.GravarLog(EventoLog.ChamadaServico, new { pvs, codigoBandeira, dataInicial, dataFinal });
                    using (var ctx = new ContextoWCF<WAExtratoHomePageServico.HISServicoWA_Extrato_HomePageClient>())
                        retorno = ctx.Cliente.ConsultarVendasCredito(
                            out status, 0, pvs, dataInicial, dataFinal);
                    log.GravarLog(EventoLog.RetornoServico, new { retorno, status });
                }
                catch (FaultException<WAExtratoHomePageServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(ex.Detail.Codigo, ex.Detail.Fonte, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Consulta Vendas a Débito
        /// </summary>
        private static WAExtratoHomePageServico.VendasDebito ConsultarVendasDebito(
            Int32 numeroPv, DateTime dataInicial, DateTime dataFinal, out WAExtratoHomePageServico.StatusRetorno status)
        {
            //Variável de retorno
            var retorno = default(WAExtratoHomePageServico.VendasDebito);

            using (Logger log = Logger.IniciarLog("HomePage Segmentada - Varejo: Vendas a Débito"))
            {
                try
                {
                    Int32 codigoBandeira = 0;
                    List<Int32> pvs = new List<Int32>(new[] { numeroPv });

                    log.GravarLog(EventoLog.ChamadaServico, new { pvs, codigoBandeira, dataInicial, dataFinal });
                    using (var ctx = new ContextoWCF<WAExtratoHomePageServico.HISServicoWA_Extrato_HomePageClient>())
                        retorno = ctx.Cliente.ConsultarVendasDebito(
                            out status, 0, pvs, dataInicial, dataFinal);
                    log.GravarLog(EventoLog.RetornoServico, new { retorno, status });
                }
                catch (FaultException<WAExtratoHomePageServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(ex.Detail.Codigo, ex.Detail.Fonte, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Consulta Lançamentos Futuros
        /// </summary>
        private static WAExtratoHomePageServico.LancamentosFuturos ConsultarLancamentosFuturos(
            Int32 numeroPv, DateTime dataInicial, DateTime dataFinal, out WAExtratoHomePageServico.StatusRetorno status)
        {
            //Variável de retorno
            var retorno = default(WAExtratoHomePageServico.LancamentosFuturos);

            using (Logger log = Logger.IniciarLog("HomePage Segmentada - Varejo: Lançamentos Futuros"))
            {
                try
                {
                    Int32 codigoBandeira = 0;
                    List<Int32> pvs = new List<Int32>(new[] { numeroPv });

                    log.GravarLog(EventoLog.ChamadaServico, new { pvs, codigoBandeira, dataInicial, dataFinal });
                    using (var ctx = new ContextoWCF<WAExtratoHomePageServico.HISServicoWA_Extrato_HomePageClient>())
                        retorno = ctx.Cliente.ConsultarLancamentosFuturos(
                            out status, 0, pvs, dataInicial, dataFinal);
                    log.GravarLog(EventoLog.RetornoServico, new { retorno, status });
                }
                catch (FaultException<WAExtratoHomePageServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(ex.Detail.Codigo, ex.Detail.Fonte, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Consulta Valores Pagos
        /// </summary>
        private static WAExtratoHomePageServico.ValoresPagos ConsultarValoresPagos(
            Int32 numeroPv, DateTime dataInicial, DateTime dataFinal, out WAExtratoHomePageServico.StatusRetorno status)
        {
            //Variável de retorno
            var retorno = default(WAExtratoHomePageServico.ValoresPagos);

            using (Logger log = Logger.IniciarLog("HomePage Segmentada - Varejo: Valores Pagos"))
            {
                try
                {
                    Int32 codigoBandeira = 0;
                    List<Int32> pvs = new List<Int32>(new[] { numeroPv });

                    log.GravarLog(EventoLog.ChamadaServico, new { pvs, codigoBandeira, dataInicial, dataFinal });
                    using (var ctx = new ContextoWCF<WAExtratoHomePageServico.HISServicoWA_Extrato_HomePageClient>())
                        retorno = ctx.Cliente.ConsultarValoresPagos(
                            out status, 0, pvs, dataInicial, dataFinal);
                    log.GravarLog(EventoLog.RetornoServico, new { retorno, status });
                }
                catch (FaultException<WAExtratoHomePageServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(ex.Detail.Codigo, ex.Detail.Fonte, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Consulta por Transação - Crédito TID
        /// </summary>
        private static MEExtratoConsultaTransacaoServico.CreditoTID ConsultarPorTransacaoCreditoTID(
            Int32 pv, DateTime dataInicial, DateTime dataFinal, String tid,
            out MEExtratoConsultaTransacaoServico.StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada - EMP/IBBA: Consulta por Transação - Crédito TID"))
            {
                try
                {
                    var transacao = default(MEExtratoConsultaTransacaoServico.CreditoTID);

                    log.GravarLog(EventoLog.ChamadaServico, new { pv, dataInicial, dataFinal, tid });
                    using (var ctx = new ContextoWCF<MEExtratoConsultaTransacaoServico.HISServicoME_Extrato_ConsultaTransacaoClient>())
                        transacao = ctx.Cliente.ConsultarCreditoTID(out status, tid, pv);
                    log.GravarLog(EventoLog.RetornoServico, new { transacao, status });

                    return transacao;
                }
                catch (FaultException<MEExtratoConsultaTransacaoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(ex.Detail.Codigo, ex.Detail.Fonte, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta por Transação - Débito TID
        /// </summary>
        private static MEExtratoConsultaTransacaoServico.DebitoTID ConsultarPorTransacaoDebitoTID(
            Int32 pv, DateTime dataInicial, DateTime dataFinal, String tid,
            out MEExtratoConsultaTransacaoServico.StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada - EMP/IBBA: Consulta por Transação - Débito TID"))
            {
                try
                {
                    var transacao = default(MEExtratoConsultaTransacaoServico.DebitoTID);

                    log.GravarLog(EventoLog.ChamadaServico, new { pv, dataInicial, dataFinal, tid });
                    using (var ctx = new ContextoWCF<MEExtratoConsultaTransacaoServico.HISServicoME_Extrato_ConsultaTransacaoClient>())
                        transacao = ctx.Cliente.ConsultarDebitoTID(out status, tid, pv);
                    log.GravarLog(EventoLog.RetornoServico, new { transacao, status });

                    return transacao;
                }
                catch (FaultException<MEExtratoConsultaTransacaoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(ex.Detail.Codigo, ex.Detail.Fonte, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta por Transação - Crédito - NSU/Cartão
        /// </summary>
        private static MEExtratoConsultaTransacaoServico.Credito[] ConsultarPorTransacaoCredito(
            Int32 pv, DateTime dataInicial, DateTime dataFinal, Boolean flagNsuCv, Boolean flagCartao, String numero,            
            out MEExtratoConsultaTransacaoServico.StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada - EMP/IBBA: Consulta por Transação - Crédito NSU/Cartão"))
            {
                try
                {
                    var transacoes = default(MEExtratoConsultaTransacaoServico.Credito[]);

                    log.GravarLog(EventoLog.ChamadaServico, new { pv, dataInicial, dataFinal, flagNsuCv, flagCartao, numero });

                    String numeroCartao = flagCartao ? numero : default(String);
                    Int64 numeroNsuCv = flagNsuCv ? numero.ToInt64(0) : default(Int64);
                    using (var ctx = new ContextoWCF<MEExtratoConsultaTransacaoServico.HISServicoME_Extrato_ConsultaTransacaoClient>())
                        transacoes = ctx.Cliente.ConsultarCredito(out status, pv, dataInicial, dataFinal, numeroCartao, numeroNsuCv);

                    log.GravarLog(EventoLog.RetornoServico, new { transacoes, status });

                    return transacoes;
                }
                catch (FaultException<MEExtratoConsultaTransacaoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(ex.Detail.Codigo, ex.Detail.Fonte, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta por Transação - Crédito - NSU/Cartão
        /// </summary>
        private static MEExtratoConsultaTransacaoServico.Debito[] ConsultarPorTransacaoDebito(
            Int32 pv, DateTime dataInicial, DateTime dataFinal, Boolean flagNsuCv, Boolean flagCartao, String numero,
            out MEExtratoConsultaTransacaoServico.StatusRetorno status)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada - EMP/IBBA: Consulta por Transação - Débito NSU/Cartão"))
            {
                try
                {
                    var transacoes = default(MEExtratoConsultaTransacaoServico.Debito[]);

                    log.GravarLog(EventoLog.ChamadaServico, new { pv, dataInicial, dataFinal, flagNsuCv, flagCartao, numero });

                    String numeroCartao = flagCartao ? numero : default(String);
                    Int64 numeroNsuCv = flagNsuCv ? numero.ToInt64(0) : default(Int64);
                    using (var ctx = new ContextoWCF<MEExtratoConsultaTransacaoServico.HISServicoME_Extrato_ConsultaTransacaoClient>())
                        transacoes = ctx.Cliente.ConsultarDebito(out status, pv, dataInicial, dataFinal, numeroCartao, numeroNsuCv);

                    log.GravarLog(EventoLog.RetornoServico, new { transacoes, status });

                    return transacoes;
                }
                catch (FaultException<MEExtratoConsultaTransacaoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(ex.Detail.Codigo, ex.Detail.Fonte, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        /// <summary>
        /// Consulta a situação do PV no programa de fidelidade.
        /// </summary>
        /// <returns>URL da imagem que irá ser mostrada no banner</returns>
        private static String ConsultarFidelidadePv(Int32 pv)
        {
            using (Logger log = Logger.IniciarLog("HomePage Segmentada - Consultar Status Fidelidade PV"))
            {
                try
                {
                    var status = default(String);

#if DEBUG
                    status = new[] { "ELEGIVEL", "FIDELIZADO", "NAOATENDE" } [new Random().Next(0, 3)];
#else
                    using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        status = ctx.Cliente.ConsultarPVFidelidade(pv);
#endif

                    return status;
                }
                catch (FaultException<Comum.SharePoint.EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(ex.Detail.Codigo, ex.Detail.Fonte, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw new PortalRedecardException(CODIGO_ERRO, FONTE, ex);
                }
            }
        }

        #endregion

        #region [ Métodos auxiliares ]

        /// <summary>
        /// Gera url para redirecionamento para a tela de Consulta por Transações
        /// </summary>
        private static String GerarUrlConsultaPorTransacao(
            Char tipoVenda, DateTime dataInicial, DateTime dataFinal,
            String numero, Int32 numeroPv, Boolean flagNsuCv, Boolean flagCartao, Boolean flagTid)
        {
            var qs = new QueryStringSegura();
            qs["TipoVenda"] = tipoVenda.ToString(); //"C" (crédito) ou "D" (débito)
            qs["DataInicial"] = dataInicial.ToString("dd/MM/yyyy");
            qs["DataFinal"] = dataFinal.ToString("dd/MM/yyyy");
            qs["Numero"] = numero;
            qs["NumeroPv"] = numeroPv.ToString();
            if (flagCartao)
                qs["TipoConsulta"] = "Cartao";
            else if (flagNsuCv)
                qs["TipoConsulta"] = "NsuCv";
            else if (flagTid)
                qs["TipoConsulta"] = "TID";

            String urlConsultaTransacao = 
                BaseUserControl.ProcurarUrlPaginaPN(Sessao.Obtem(), "extrato", "pn_ConsultaTransacao.aspx").FirstOrDefault();

            if (!String.IsNullOrEmpty(urlConsultaTransacao))
                return String.Format("{0}?dados={1}", urlConsultaTransacao, qs.ToString());
            else
                return null;
        }

        #endregion
    }
}