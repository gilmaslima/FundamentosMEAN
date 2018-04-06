using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using Redecard.PN.Comum;
using Redecard.PN.Comum.TrataErroServico;
using Redecard.PN.Extrato.SharePoint.ModuloRAV;
using Redecard.PN.Extrato.SharePoint.Servico.Home;

namespace Redecard.PN.Extrato.SharePoint.Handlers
{
    public partial class HomePageVendedora : UserControlBase, IHttpHandler, IReadOnlySessionState
    {
        private const string FIDELIZADO = "FIDELIZADO";
        private const string ELEGIVEL = "ELEGIVEL";

        private static JavaScriptSerializer _jsSerializer;
        private static JavaScriptSerializer JsSerializer
        {
            get
            {
                if (_jsSerializer == null)
                    _jsSerializer = new JavaScriptSerializer();
                return _jsSerializer;
            }
        }

        private static Random _random;
        private static Random Random
        {
            get
            {
                if (_random == null) _random = new Random();
                return _random;
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                //Verifica se usuário está logado
                if (SessaoAtual == null)
                    return;

                //Obtém dados da requisição
                String strPVs = context.Request["pvs"];
                if (String.IsNullOrEmpty(strPVs))
                    return;

                //Descriptografa os PVs
                List<Int32> pvs = strPVs.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(pvCripto => Criptografia.DescriptografarPV(pvCripto, SessaoAtual.CodigoEntidade))
                    .Distinct().ToList();

                if (pvs.Count == 0)
                    return;

                //Obtém tipo de consulta solicitada
                String tipoConsulta = context.Request["consulta"];
                if (String.IsNullOrEmpty(tipoConsulta))
                    return;

                //Roteia requisição para o handler específico, de acordo com o tipo de consulta solicitada
                if (tipoConsulta == "ultimasVendas")
                    ProcessRequestUltimasVendas(context, pvs);
                else if (tipoConsulta == "lancamentosFuturos")
                    ProcessRequestLancamentosFuturos(context, pvs);
                else if (tipoConsulta == "valorDisponivel")
                    ProcessRequestValorDisponivel(context);
                else if (tipoConsulta == "imagemFidelidadePV")
                    ProcessRequestImagemFidelidadePV(context);
                else if (tipoConsulta == "AutoricaoUsuarioFielo")
                    ProcessRequestAutoricaoUsuarioFielo(context);
            }
            catch (PortalRedecardException exc)
            {
                SharePointUlsLog.LogErro(exc);
                ProcessRequestErro(context, exc.Fonte, exc.Codigo);
            }
            catch (FaultException<Servico.Home.GeneralFault> exc)
            {
                SharePointUlsLog.LogErro(exc);
                ProcessRequestErro(context, exc.Detail.Fonte, exc.Detail.Codigo);
            }
            catch (FaultException<ModuloRAV.ServicoRAVException> exc)
            {
                SharePointUlsLog.LogErro(exc);
                ProcessRequestErro(context, exc.Detail.Fonte, exc.Detail.Codigo);
            }
            catch (Exception exc)
            {
                SharePointUlsLog.LogErro(exc);
                ProcessRequestErro(context, FONTE, CODIGO_ERRO);
            }
        }

        #region [ Process Request ]

        private void ProcessRequestUltimasVendas(HttpContext context, List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("HomePage: Últimas Vendas - consulta assíncrona"))
            {
                try
                {
                    //Consulta de dados
                    ConsultarTransacoesCreditoDebitoRetorno[] ultimasVendas = ConsultarUltimasVendas(pvs, null, null);

                    //Formata dados para retorno
                    var retorno = ultimasVendas.Select(uv => new
                    {
                        Data = uv.Data.ToString("dd/MM/yyyy"),
                        ValorCredito = uv.ValorCredito.ToString("C"),
                        ValorDebito = uv.ValorDebito.ToString("C")
                    });

                    //Response contendo as últimas vendas
                    context.Response.Write(JsSerializer.Serialize(retorno));
                    context.Response.ContentType = "application/json";
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
            }
        }

        private void ProcessRequestLancamentosFuturos(HttpContext context, List<Int32> pvs)
        {
            using (Logger Log = Logger.IniciarLog("HomePage: Lançamentos Futuros - consulta assíncrona"))
            {
                try
                {
                    //Consulta de dados
                    ConsultarCreditoDebitoRetorno[] lancamentosFuturos = ConsultarLancamentosFuturos(pvs, null, null);

                    //Formata dados para retorno
                    var retorno = lancamentosFuturos.Select(lf => new
                    {
                        Data = lf.Data.ToString("dd/MM/yyyy"),
                        ValorCredito = lf.ValorCredito.ToString("C"),
                        ValorDebito = lf.ValorDebito.ToString("C")
                    });

                    //Response contendo as últimas vendas
                    context.Response.Write(JsSerializer.Serialize(retorno));
                    context.Response.ContentType = "application/json";
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
            }
        }

        private void ProcessRequestValorDisponivel(HttpContext context)
        {
            using (Logger Log = Logger.IniciarLog("HomePage: Valor disponível - consulta assíncrona"))
            {
                try
                {
                    //Consulta de dados
                    ModRAVAvulsoSaida valorDisponivel = ConsultarValorDisponivel();
                    String valorRav = String.Empty;

                    if (valorDisponivel == null || valorDisponivel.ValorDisponivel < 50m || this.SessaoAtual.StatusPVCancelado())
                        valorRav = String.Empty;
                    else
                        valorRav = valorDisponivel.ValorDisponivel.ToString("C") + "*";

                    //Response contendo as últimas vendas
                    context.Response.Write(JsSerializer.Serialize(valorRav));
                    context.Response.ContentType = "application/json";
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw ex;
                }
            }
        }

        private void ProcessRequestImagemFidelidadePV(HttpContext context)
        {
            using (Logger log = Logger.IniciarLog("HomePage: Imagem fidelidade PV - consulta assíncrona"))
            {
                try
                {
                    var retorno = new
                    {
                        UrlImage = ConsultaPVFidelidade()
                    };

                    context.Response.Write(JsSerializer.Serialize(retorno));
                    context.Response.ContentType = "application/json";
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
            }
        }

        private void ProcessRequestAutoricaoUsuarioFielo(HttpContext context)
        {
            using (Logger log = Logger.IniciarLog("HomePage: Autorizacao usuario Fielo - consulta assíncrona"))
            {
                try
                {
                    var pv = SessaoAtual.CodigoEntidade.ToString();
                    var hash = Guid.NewGuid().ToString();


                    Dictionary<string, string> parametros = new Dictionary<string, string>();
                    parametros.Add("pv", pv);
                    parametros.Add("hash", hash);

                    var chave = String.Format("FD_{0}", DateTime.Now.ToString("ddMMyyHHmmss"));

                    CacheAdmin.Adicionar(Redecard.PN.Comum.Cache.Fidelidade, chave, parametros.ToList());

                    var retorno = new
                    {
                        Pv = pv,
                        Hash = hash,
                        AppFabric = chave
                    };

                    context.Response.Write(JsSerializer.Serialize(retorno));
                    context.Response.ContentType = "application/json";
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw ex;
                }
            }
        }

        private void ProcessRequestErro(HttpContext context, String fonte, Int32 codigo)
        {
            String mensagemRetorno = "Sistema indisponível";
            Int32 codigoRetorno = -1;

            try
            {
                using (var contexto = new ContextoWCF<TrataErroServicoClient>())
                {
                    TrataErro erro = contexto.Cliente.Consultar(fonte, codigo);
                    if (erro != null && erro.Codigo != 0)
                    {
                        mensagemRetorno = erro.Fonte;
                        codigoRetorno = erro.Codigo;
                    }
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Erro durante exibição do erro", ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante exibição do erro", ex);
            }

            var retornoErro = new { Mensagem = mensagemRetorno, Codigo = codigoRetorno };
            //Response contendo o erro
            context.Response.Write(JsSerializer.Serialize(retornoErro));
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }

        #endregion

        #region [ Consultas ]

        /// <summary>Consulta as Últimas vendas de crédito e débito</summary>
        private ConsultarTransacoesCreditoDebitoRetorno[] ConsultarUltimasVendas(List<Int32> pvs, DateTime? dataInicial, DateTime? dataFinal)
        {
            StatusRetorno objStatusRetorno;

            var objEnvio = new ConsultarTransacoesCreditoDebitoEnvio();
            objEnvio.DataInicial = DateTime.Now.AddDays(-6);
            objEnvio.DataFinal = DateTime.Now.AddDays(-1);

            //Obtém e valida número do estabelecimento                        
            if (pvs == null || pvs.Count == 0)
                objEnvio.Estabelecimentos = new Int32[1] { base.SessaoAtual.CodigoEntidade };
            else
                objEnvio.Estabelecimentos = pvs.Distinct().ToArray();

            //recupera data da querystring
            if (dataInicial.HasValue)
                objEnvio.DataInicial = dataInicial.Value;

            //recupera data da querystring
            if (dataFinal.HasValue)
                objEnvio.DataFinal = dataFinal.Value;

            ConsultarTransacoesCreditoDebitoRetorno[] objRetorno = null;
            using (var contexto = new ContextoWCF<HomeClient>())
                objRetorno = contexto.Cliente.ConsultarTransacoesCreditoDebito(out objStatusRetorno, objEnvio);

            //trata retorno do serviço
            Boolean retornoSemDados = objStatusRetorno.CodigoRetorno == 10;

            if (objStatusRetorno.CodigoRetorno != 0)
            {
                if (!retornoSemDados)
                    throw new PortalRedecardException(objStatusRetorno.CodigoRetorno, objStatusRetorno.Fonte);
                return null;
            }

            return objRetorno;
        }

        /// <summary>Consulta os Lançamentos Futuros de crédito e débito</summary>
        private ConsultarCreditoDebitoRetorno[] ConsultarLancamentosFuturos(List<Int32> pvs, DateTime? dataInicial, DateTime? dataFinal)
        {
            StatusRetorno objStatusRetorno;

            var objEnvio = new Servico.Home.ConsultarCreditoDebitoEnvio();
            objEnvio.DataInicial = DateTime.Now.AddDays(1);
            objEnvio.DataFinal = DateTime.Now.AddDays(6);

            //Obtém e valida número do estabelecimento            
            if (pvs == null || pvs.Count == 0)
                objEnvio.Estabelecimentos = new Int32[1] { base.SessaoAtual.CodigoEntidade };
            else
                objEnvio.Estabelecimentos = pvs.Distinct().ToArray();

            //recupera data da querystring
            if (dataInicial.HasValue)
                objEnvio.DataInicial = dataInicial.Value;

            //recupera data da querystring
            if (dataFinal.HasValue)
                objEnvio.DataFinal = dataFinal.Value;

            ConsultarCreditoDebitoRetorno[] objRetorno;
            using (var contexto = new ContextoWCF<HomeClient>())
                objRetorno = contexto.Cliente.ConsultarCreditoDebito(out objStatusRetorno, objEnvio);

            //trata retorno do serviço
            bool retornoSemDados = objStatusRetorno.CodigoRetorno == 10;

            if (objStatusRetorno.CodigoRetorno != 0)
            {
                if (!retornoSemDados)
                    throw new PortalRedecardException(objStatusRetorno.CodigoRetorno, objStatusRetorno.Fonte);
                return null;
            }

            return objRetorno;
        }

        /// <summary>Recupera o Valor disponível para o RAV</summary>
        private ModRAVAvulsoSaida ConsultarValorDisponivel()
        {
            ModRAVAvulsoSaida objRetorno = null;

            using (var contexto = new ContextoWCF<ModuloRAV.ServicoPortalRAVClient>())
            {
                objRetorno = contexto.Cliente.VerificarRAVDisponivel_Cache(base.SessaoAtual.CodigoEntidade);
            }

            return objRetorno;
        }

        /// <summary>
        /// Consulta a situação do PV no programa de fidelidade.
        /// </summary>
        /// <returns>URL da imagem que irá ser mostrada no banner</returns>
        private String ConsultaPVFidelidade()
        {
            using (Logger log = Logger.IniciarLog("HomePageVendedora - ConsultaPVFidelidade"))
            {
                log.GravarLog(EventoLog.ChamadaServico);

                try
                {
                    String retornoServico = String.Empty;

                    using (var context = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        retornoServico = context.Cliente.ConsultarPVFidelidade(SessaoAtual.CodigoEntidade);
                    }

                    if (String.Compare(retornoServico.ToUpper(), FIDELIZADO) == 0 && String.Compare(SessaoAtual.TipoUsuario, "M", true) == 0)
                    {
                        return "~/_layouts/Redecard.PN.Extrato.SharePoint/Styles/Fidelidade.png";
                    }
                    else if (String.Compare(retornoServico.ToUpper(), ELEGIVEL) == 0 && String.Compare(SessaoAtual.TipoUsuario, "M", true) == 0)
                    {
                        return "~/_layouts/Redecard.PN.Extrato.SharePoint/Styles/Elegivel.png";
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    throw;
                }
            }
        }

        #endregion
    }
}