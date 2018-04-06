
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.GEServicoInformacaoComercial;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.ZPServicoTerminalContratado;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rede.PN.HomePage.SharePoint
{
    /// <summary>
    /// 
    /// </summary>
    public partial class NovaHomeUserControl : Redecard.PN.Extrato.SharePoint.BaseUserControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void VoltarAnterior(object sender, EventArgs e) {
            Session["_show_new_home"] = false;

            Response.Redirect("/sites/fechado/Paginas/pn_home.aspx");
            Response.End();
        }
        /// <summary>
        /// Carrega as datas Atual e de Último Acesso
        /// </summary>
        private void CarregarDatasAcesso() {
            lblDataAtual.Text = DateTime.Today.ToString("dd/MM/yyyy");
            if (SessaoAtual != null)
                lblDataUltimoAcesso.Text = SessaoAtual.UltimoAcesso.ToString("dd/MM/yyyy 'às' HH'h'mm");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e) {
            if (this.SessaoAtual != null && Util.UsuarioLogadoFBA()) {
                ClientScriptManager scriptManager = Page.ClientScript;
                if (scriptManager != null) {
                    scriptManager.RegisterStartupScript(typeof(Page), "__dashboardScriptHomePage", this.GerarScriptsDashboard(), true);
                }
            }
            base.OnPreRender(e);
        }
        /// <summary>
        /// Não verificar permissões na página inicial do Portal Redecard Estabelecimento
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e) {
            base.ValidarPermissao = false;
            base.OnLoad(e);
        }
        /// <summary>
        /// 
        /// </summary>
        private Boolean AcessoLancamentosFuturos {
            get {
                //Código do serviço do Relatório de Lançamentos Futuros
                //Int32 codigoServico = 10071;
                //Boolean possuiAcesso = base.ValidarServico(codigoServico);
                //return possuiAcesso;
                return Sessao.Contem() &&
                    this.ValidarPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=4");
            }
        }
        /// <summary>
        /// Verifica se usuário possui permissão para Acesso ao Relatório de Valores Pagos
        /// </summary>
        private Boolean AcessoValoresPagos {
            get {
                //Código do serviço do Relatório de Valores Pagos
                //Int32 codigoServico = 10070;
                //Boolean possuiAcesso = base.ValidarServico(codigoServico);
                //return possuiAcesso;
                return Sessao.Contem() && this.ValidarPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=1");
            }
        }
        /// <summary>
        /// Verifica se usuário possui permissão para Acesso ao Relatório de Vendas
        /// </summary>
        private Boolean AcessoVendas {
            get {
                //Código do Serviço do Relatório de Vendas
                //Int32 codigoServico = 10069;
                //Boolean possuiAcesso = base.ValidarServico(codigoServico);
                //return possuiAcesso;
                return Sessao.Contem() &&
                    this.ValidarPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=0");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected Boolean EstabelecimentoVarejo() {
            if (Util.UsuarioLogadoFBA())
                return SessaoAtual.CodigoSegmento.ToString().ToLowerInvariant() == "v";
            else
                return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tipoRelatorio"></param>
        /// <param name="tipoVenda"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <param name="modalidade"></param>
        /// <param name="pesquisar"></param>
        /// <returns></returns>
        protected String ObterUrlLinkRelatorioModalidade(TipoRelatorio tipoRelatorio, TipoVenda tipoVenda,
            DateTime dataInicial, DateTime dataFinal, Int32 modalidade, Boolean pesquisar) {
            String webURL = String.Empty;
            try {
                String requestUrl = Request.Url.ToString();
                requestUrl = requestUrl.Substring(0, requestUrl.LastIndexOf("/"));

                using (SPSite site = new SPSite(requestUrl))
                using (SPWeb web = site.AllWebs["extrato"])
                    webURL = web.Url;

                QueryStringSegura qs = new QueryStringSegura();
                qs["DataInicial"] = dataInicial.ToString("dd/MM/yyyy");
                qs["DataFinal"] = dataFinal.ToString("dd/MM/yyyy");
                qs["TipoRelatorio"] = ((Int32)tipoRelatorio).ToString();
                qs["TipoVenda"] = ((Int32)tipoVenda).ToString();
                qs["Pesquisar"] = pesquisar ? Boolean.TrueString : Boolean.FalseString;
                qs["Modalidade"] = modalidade.ToString();


                return String.Format("{0}/Paginas/pn_Relatorios.aspx?dados={1}", webURL, qs.ToString());
            }
            catch (QueryStringExpiradaException e) {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterUrlLinkRelatorio");
                SharePointUlsLog.LogErro(e);
                throw e;
            }
            catch (QueryStringInvalidaException e) {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterUrlLinkRelatorio");
                SharePointUlsLog.LogErro(e);
                throw e;
            }
            catch (Exception e) {
                SharePointUlsLog.LogMensagem("BaseUserControl.ObterUrlLinkRelatorio");
                SharePointUlsLog.LogErro(e);
                throw e;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected String GerarScriptsDashboard()
        {
            DateTime atual = DateTime.Now;
            DateTime proximos5dias = atual.AddDays(5);
            DateTime ultimos5dias = atual.AddDays(-5);
            String script = String.Empty;

            if (EstabelecimentoVarejo())
                script += "__tipoEstabelecimento='varejo';";
            else
                script += "__tipoEstabelecimento='ibba';";

            // O que vendi
            script += String.Format("__linkCreditoDetalhes = '{0}';",
                base.ObterUrlLinkRelatorio(TipoRelatorio.Vendas, TipoVenda.Credito, ultimos5dias, atual, true));
            script += String.Format("__linkDebitoDetalhes = '{0}';",
                this.ObterUrlLinkRelatorioModalidade(TipoRelatorio.Vendas, TipoVenda.Debito, ultimos5dias, atual, 0, true));

            // O que receberei
            script += String.Format("__rlinkLancamentosFuturosDetalhes = '{0}';",
                base.ObterUrlLinkRelatorio(TipoRelatorio.LancamentosFuturos, TipoVenda.Credito, atual, proximos5dias, true));

            // O que recebi
            script += String.Format("__rlinkCreditoDetalhes = '{0}';",
                base.ObterUrlLinkRelatorio(TipoRelatorio.ValoresPagos, TipoVenda.Credito, ultimos5dias, atual, true));
            script += String.Format("__rlinkDebitoDetalhes = '{0}';",
                 this.ObterUrlLinkRelatorioModalidade(TipoRelatorio.Vendas, TipoVenda.Debito, ultimos5dias, atual, 0, true));
            script += String.Format("__rlinkDebitoPreDatadoDetalhes = '{0}';",
                this.ObterUrlLinkRelatorioModalidade(TipoRelatorio.ValoresPagos, TipoVenda.Debito, ultimos5dias, atual, 2, true));
            script += String.Format("__rlinkAntecipacoesDetalhes = '{0}';",
                base.ObterUrlLinkRelatorio(TipoRelatorio.AntecipacoesRAV, TipoVenda.Credito, ultimos5dias, atual, true));

            return script;
        }
        /// <summary>
        /// Load da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e) {
            try {
                if (!Page.IsPostBack)
                {
                    if (Sessao.Contem())
                    {
                        CarregarAvisoSenha();
                        ConfigurarVersaoExtratos();

                        if (!SessaoAtual.UsuarioAtendimento)
                        {
                            CarregarAceiteCondicoesComerciais();
                        }

                        CriacaoAcessoBloqueada();
                    }
                }
                
                CarregarDatasAcesso();
                CarregarHomePageSegmentada();

                if (((Boolean?)Session["DesbloqueioDiretoEntidadeSucesso"]).GetValueOrDefault(false))
                {
                    // exibe a confirmação ao usuário
                    String mensagem = "Desbloqueio efetuado com sucesso";
                    String script = String.Format("DesbloqueioEntidade.ExibirSucesso('{0}');", mensagem);
                    Page.ClientScript.RegisterStartupScript(Page.GetType(), "CriacaoAcessoBloqueada", script, true);
                    Session["DesbloqueioDiretoEntidadeSucesso"] = null;
                }

                // verificar se deve ser exibido o aviso da nova home
                pnlExibirAvisoHomeAntiga.Visible = this.WebPart.ExibirAvisoHomeAntiga;
                
                // verificar se deve ser exibido a modal da nova home
                if (this.WebPart.ExibirModalNovaHome && !Request.Cookies.AllKeys.Contains("__modal__nova__home__")) {
                    Response.Cookies.Add(new HttpCookie("__modal__nova__home__", "True"));

                    String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { homeOpenModal('[id$=lbxNovaHome]'); }, 'SP.UI.Dialog.js');";
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AbrirModalDialog", javaScript, true);
                }

                // popular classe do botão voltar para a home antiga
                if (EstabelecimentoVarejo()) {
                    linkVoltarAnterior.CssClass = "varejo-voltar-home-antiga";
                }
                else {
                    linkVoltarAnterior.CssClass = "ibba-voltar-home-antiga";
                }

                // Validar Permissões de Usuário
                if (AcessoVendas)
                    pnlAcessoVendas.Visible = true;
                else
                    pnlAcessoVendasNegado.Visible = true;

                if (AcessoValoresPagos) {
                    pnlAcessoValoresPagos.Visible = true;
                    pnlAcessoValoresPagosIbba.Visible = true;
                }
                else {
                    pnlAcessoValoresPagosNegado.Visible = true;
                    pnlAcessoValoresPagosIbbaNegado.Visible = true;
                }

                if (AcessoLancamentosFuturos) {
                    pnlAcessoLancamentosFuturos.Visible = true;
                    pnlAcessoLancamentosFuturosIbba.Visible = true;
                }
                else {
                    pnlAcessoLancamentosFuturosNegado.Visible = true;
                    pnlAcessoLancamentosFuturosIbbaNegado.Visible = true;
                }
            }
            catch (PortalRedecardException ex) {
                Logger.GravarErro("NOVAHOME", ex);
                SharePointUlsLog.LogErro(ex);
                SharePointUlsLog.LogMensagem("@@@NOVAHOME: " + ex.StackTrace);
                base.ExibirPainelExcecao(ex.Fonte, ex.Codigo);

            }
            catch (Exception ex) {
                Logger.GravarErro("NOVAHOME", ex);
                SharePointUlsLog.LogErro(ex);
                SharePointUlsLog.LogMensagem("@@@NOVAHOME: " + ex.StackTrace);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Home WebPart { get { return this.Parent as Home; } }
        /// <summary>
        /// ucVarejoAtalhos control.
        /// </summary>
        protected Atalhos UcVarejoAtalhos { get { return (Atalhos)ucVarejoAtalhos; } }
        /// <summary>
        /// ucEmpIbbaAtalhos control.
        /// </summary>
        protected Atalhos UcEmpIbbaAtalhos { get { return (Atalhos)ucEmpIbbaAtalhos; } }
        /// <summary>
        /// 
        /// </summary>
        private void ExibirHomePageVarejo() {
            UcVarejoAtalhos.ConfiguracaoAtalhos = this.WebPart.VarejoAtalhos;
            pnlVarejo.Visible = true;
        }
        /// <summary>
        /// 
        /// </summary>
        private void ExibirHomePageEmpIbba() {
            UcEmpIbbaAtalhos.ConfiguracaoAtalhos = this.WebPart.EmpIbbaAtalhos;
            pnlEmpIbba.Visible = true;
        }
        /// <summary>
        /// <para>Carrega a HomePage de acordo com o segmento do estabelecimento: </para>
        /// <para>    SEGMENTO           | CÓD. SEGMENTO |  TIPO HOMEPAGE         </para>
        /// <para>================================================================</para>
        /// <para>IBBA1                  |       I       |      CCG               </para>
        /// <para>IBBA2                  |       J       |      CCG               </para>
        /// <para>EMP1                   |       K       |      CCG               </para>
        /// <para>Grandes                |       G       |      CCG               </para>
        /// <para>EMP2                   |       L       |      CCG               </para>
        /// <para>Médios                 |       S       |      CCG               </para>
        /// <para>Vendas Centralizadas 1 |       M       |      CCG               </para>
        /// <para>Vendas Centralizadas 2 |       N       |      CCG               </para>
        /// <para>Top Varejo             |       E       |      CCG               </para>
        /// <para>----------------------------------------------------------------</para> 
        /// <para>Varejo                 |       V       |     Varejo             </para>
        /// </summary>
        private void CarregarHomePageSegmentada() {
            if (Sessao.Contem()) {
                Char codigoSegmento = SessaoAtual.CodigoSegmento;
#if DEBUG
                if (!String.IsNullOrEmpty(Request["Segmento"]))
                    codigoSegmento = Request["Segmento"][0];
#endif
                switch (codigoSegmento) {
                    case 'V':
                    case 'v':
                        this.ExibirHomePageVarejo();
                        break;
                    default:
                        this.ExibirHomePageEmpIbba();
                        break;
                }
            }
            else {
                this.ExibirHomePageVarejo();
                this.ExibirHomePageEmpIbba();
            }
        }

        #region [Métodos/Eventos - Condições Comerciais]

        /// <summary>
        /// Verifica e carrega informações para aceite de condições comerciais.
        /// </summary>
        private void CarregarAceiteCondicoesComerciais()
        {
            using (Logger log = Logger.IniciarLog("Carregando informações de aceite de condições comerciais."))
            {
                try
                {
                    using (var contextoGE = new ContextoWCF<ServicoInformacaoComercialClient>())
                    {
                        // Obtem informações de aceite
                        InformacaoComercialResponse aceite = contextoGE.Cliente.Consultar(new InformacaoComercialRequest() { NumeroPDV = SessaoAtual.CodigoEntidade });

                        if (aceite.NumeroPDV != null && aceite.NumeroPDV != default(Decimal?))
                        {
                            // Preenche informações no lightbox de aceite
                            ltrRazaoSocial.Text = aceite.RazaoSocial;
                            ltrRamoAtividade.Text = String.Format("{0}-{1}", aceite.CodigoRamoAtividade, aceite.DescricaoRamoAtividade);
                            ltrResponsavel.Text = aceite.Responsavel;

                            // Tratamento de preenchimento de telefones
                            String telefone1 = String.Empty;
                            String telefone2 = String.Empty;
                            if (aceite.Telefone1.GetValueOrDefault() > 0 && aceite.Telefone1.ToString().Length == 8)
                                telefone1 = String.Format("{0}-{1}", aceite.Telefone1.ToString().Substring(0, 4), aceite.Telefone1.ToString().Substring(4, 4));
                            else if (aceite.Telefone1.GetValueOrDefault() > 0 && aceite.Telefone1.ToString().Length == 9)
                                telefone1 = String.Format("{0}-{1}", aceite.Telefone1.ToString().Substring(0, 5), aceite.Telefone1.ToString().Substring(5, 4));
                            if (aceite.Telefone2.GetValueOrDefault() > 0 && aceite.Telefone2.ToString().Length == 8)
                                telefone2 = String.Format("{0}-{1}", aceite.Telefone2.ToString().Substring(0, 4), aceite.Telefone2.ToString().Substring(4, 4));
                            else if (aceite.Telefone2.GetValueOrDefault() > 0 && aceite.Telefone2.ToString().Length == 9)
                                telefone2 = String.Format("{0}-{1}", aceite.Telefone2.ToString().Substring(0, 5), aceite.Telefone2.ToString().Substring(5, 4));

                            StringBuilder telefones = new StringBuilder();
                            if (!String.IsNullOrWhiteSpace(telefone1))
                            {
                                telefones.Append(String.Format("({0}) {1}", aceite.DDD1, telefone1, aceite.Ramal1));
                                if (aceite.Ramal1.GetValueOrDefault() > 0)
                                    telefones.Append(String.Format(" R: {0}", aceite.Ramal1));
                            }
                            if (!String.IsNullOrWhiteSpace(telefone2))
                            {
                                telefones.Append(String.Format(" / ({0}) {1}", aceite.DDD2, telefone2));
                                if (aceite.Ramal2.GetValueOrDefault() > 0)
                                    telefones.Append(String.Format(" R: {0}", aceite.Ramal2));
                            }
                            ltrTelefones.Text = telefones.ToString();

                            grvSocios.DataSource = aceite.Socios;
                            grvSocios.DataBind();

                            rptEnderecos.DataSource = aceite.Enderecos;
                            rptEnderecos.DataBind();

                            rptDomicilioBancario.DataSource = aceite.DomiciliosBancarios;
                            rptDomicilioBancario.DataBind();

                            grvServicosContratados.DataSource = aceite.ServicosContratados;
                            grvServicosContratados.DataBind();

                            ltrValorTaxaAdesao.Text = String.Format("{0:C2}", aceite.ValorTaxaAdesao);

                            // Obtem informação complementar de aceite obtidos através do HIS
                            using (var contextoZP = new ContextoWCF<ServicoTerminalContratadoClient>())
                            {
                                ltrValorTaxaAdesaoMensal.Text = String.Format("{0:C2}", contextoZP.Cliente.ObterServico(new ValoresCobrancaServicosRequest() { CodigoServico = 302 }).ValorServico);

                                ListaTerminalContratadoResponse terminais = contextoZP.Cliente.ConsultarLista(new TerminalContratadoRequest()
                                {
                                    NumeroPDV = SessaoAtual.CodigoEntidade
                                });

                                // Preenche informações de terminais no lightbox de aceite
                                grvTerminaisContratados.DataSource = terminais.Itens;
                                grvTerminaisContratados.DataBind();

                                // Chamada ZPL84800 para obtenção e preenchimento de dados de Oferta Preço Único
                                DadosPrecoUnicoPvResponse dadosPrecoUnico = contextoZP.Cliente.ObterDadosPrecoUnicoPv(new DadosPrecoUnicoPvRequest()
                                {
                                    NumeroPDV = SessaoAtual.CodigoEntidade
                                });

                                if (dadosPrecoUnico.CodigoOferta == 0)
                                    ControleVisibilidadePaineisPrecoUnico(false, false);
                                else
                                {
                                    if (String.Compare(dadosPrecoUnico.Features.FirstOrDefault().IndicadorProdutoFlex, "S", true) == 0)
                                    {
                                        if (dadosPrecoUnico.Terminais != null && dadosPrecoUnico.Terminais.Count > 0)
                                        {
                                            ControleVisibilidadePaineisPrecoUnico(true, false);

                                            List<PrecoUnico> listaPrecoUnico = new List<PrecoUnico>();

                                            dadosPrecoUnico.Terminais.ForEach(t => listaPrecoUnico.Add(new PrecoUnico()
                                            {
                                                ValorFaturamentoContrato = dadosPrecoUnico.ValorFaturamentoContrato,
                                                ValorPrecoUnicoComFlex = dadosPrecoUnico.ValorPrecoUnicoComFlex,
                                                ValorPrecoUnicoSemFlex = dadosPrecoUnico.ValorPrecoUnicoSemFlex,
                                                QuantidadeEquipamento = t.QuantidadeEquipamento.ToString(),
                                                TipoEquipamento = t.TipoEquipamento
                                            }));

                                            grvCondicaoComercialTecnologiaFlex.DataSource = listaPrecoUnico;
                                            grvCondicaoComercialTecnologiaFlex.DataBind();

                                            this.PreencheCamposFlex(dadosPrecoUnico.Features.FirstOrDefault().PercentualTaxa1,
                                                                    dadosPrecoUnico.Features.FirstOrDefault().PercentualTaxa1,
                                                                    dadosPrecoUnico.Features.FirstOrDefault().PercentualTaxa2);
                                        }
                                    }
                                    else
                                    {
                                        if (dadosPrecoUnico.Terminais != null && dadosPrecoUnico.Terminais.Count > 0)
                                        {
                                            ControleVisibilidadePaineisPrecoUnico(false, true);

                                            List<PrecoUnico> listaPrecoUnico = new List<PrecoUnico>();

                                            dadosPrecoUnico.Terminais.ForEach(t => listaPrecoUnico.Add(new PrecoUnico()
                                            {
                                                ValorFaturamentoContrato = dadosPrecoUnico.ValorFaturamentoContrato,
                                                ValorPrecoUnicoComFlex = dadosPrecoUnico.ValorPrecoUnicoComFlex,
                                                ValorPrecoUnicoSemFlex = dadosPrecoUnico.ValorPrecoUnicoSemFlex,
                                                QuantidadeEquipamento = t.QuantidadeEquipamento.ToString(),
                                                TipoEquipamento = t.TipoEquipamento
                                            }));

                                            grvCondicaoComercialTecnologia.DataSource = listaPrecoUnico;
                                            grvCondicaoComercialTecnologia.DataBind();
                                        }
                                    }
        }
                            }

                            // verifica se deve exibir a div de confirmacao positiva
                            if (base.VerificarConfirmacaoPositia())
                            {
                                divConfirmacaoPositiva.Visible = true;
                            }

                            // Mostra lightbox ao usuário
                            String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { homeOpenModal('[id$=lbxCondicoesComerciais]'); }, 'SP.UI.Dialog.js');";
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AbrirModalDialog", javaScript, true);
                        }
    }
}
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    SharePointUlsLog.LogMensagem(String.Concat("@@@HOME: ", ex.StackTrace));
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Controle de visibilidade dos painéis de Condições Comerciais da Oferta
        /// </summary>
        /// <param name="flex">Indicador de controle sobre paineis Flex</param>
        /// <param name="nonFlex">Indicador de controle sobre painel não Flex</param>
        private void ControleVisibilidadePaineisPrecoUnico(Boolean flex, Boolean nonFlex)
        {
            phdCondicaoComercialFaturamentoFlex.Visible = flex;
            phdCondicaoComercialFlex.Visible = flex;
            phdCondicaoComercialFaturamento.Visible = nonFlex;
        }

        /// <summary>
        /// Preenche os campos de dados Flex
        /// </summary>
        /// <param name="vendaVista">Descrição de venda a vista</param>
        /// <param name="parcelaPrimeira">Descrição da primeira parcela</param>
        /// <param name="parcelaAdicional">Descrição das parcelas adicionais</param>
        private void PreencheCamposFlex(Decimal vendaVista, Decimal parcelaPrimeira, Decimal parcelaAdicional)
        {
            ltrVendaVista.Text = vendaVista > 0 ? (vendaVista / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : vendaVista.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrParcelaPrimeira.Text = parcelaPrimeira > 0 ? (parcelaPrimeira / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : parcelaPrimeira.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrParcelaAdicional.Text = parcelaAdicional > 0 ? (parcelaAdicional / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : parcelaAdicional.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
        }

        /// <summary>
        /// Preenche os campos de dados Flex sem valores
        /// </summary>
        private void PreencheCamposFlex()
        {
            ltrVendaVista.Text = default(Decimal).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrParcelaPrimeira.Text = default(Decimal).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrParcelaAdicional.Text = default(Decimal).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
        }

        /// <summary>
        /// Preenchimento de informações de Tecnlogias.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvCondicaoComercialTecnologia_DataBound(object sender, EventArgs e)
        {
            GridView condicaoComercial = sender as GridView;

            for (int i = condicaoComercial.Rows.Count - 1; i > 0; i--)
            {
                GridViewRow row = condicaoComercial.Rows[i];
                GridViewRow previousRow = condicaoComercial.Rows[i - 1];
                for (int j = 0; j < row.Cells.Count; j++)
                    if (row.Cells[j].Text == previousRow.Cells[j].Text)
                        if (previousRow.Cells[j].RowSpan == 0)
                        {
                            if (row.Cells[j].RowSpan == 0)
                                previousRow.Cells[j].RowSpan += 2;
                            else
                                previousRow.Cells[j].RowSpan = row.Cells[j].RowSpan + 1;
                            row.Cells[j].Visible = false;
                        }
            }
        }

        /// <summary>
        /// Baixar em PDF as condições comercias para aceite.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Clique para download em PDF de aceite de condições comerciais."))
            {
                try
                {
                    QueryStringSegura queryString = new QueryStringSegura();
                    queryString.Add("NUM_PDV", SessaoAtual.CodigoEntidade.ToString());
                    String url;

#if DEBUG
                    //Esta é a versão somente para testes locais.
                    using (SPSite site = new SPSite(SPContext.Current.Site.ID, SPUrlZone.Default))
                        url = site.MakeFullUrl(String.Format("/_layouts/Redecard.PN.Extrato.SharePoint/DownloadAceiteCondicoesComerciais.aspx?num_pdv={0}", queryString.ToString()));
# else
                    //Esta é a versão para ambientes Rede.
                    StringBuilder urlComposta = new StringBuilder();
                    urlComposta.Append("http://");
                    urlComposta.Append("localhost/");
                    urlComposta.Append("_layouts/Redecard.PN.Extrato.SharePoint/DownloadAceiteCondicoesComerciais.aspx?num_pdv=");
                    urlComposta.Append(queryString.ToString());
                    url = urlComposta.ToString();
#endif

                    log.GravarMensagem(url);

                    PDF pdf = new PDF();
                    byte[] pdfBytes = pdf.GerarPdfUrl(url);
                    log.GravarMensagem("GerarPdfUrl", pdfBytes);
                    Response.AddHeader("Content-Type", "application/pdf");
                    Response.AddHeader("Content-Disposition", String.Format("attachment; filename=Condicoes Comerciais {0}.pdf; size={1}", DateTime.Now.Ticks, pdfBytes.Length.ToString(CultureInfo.InvariantCulture)));
                    Response.BinaryWrite(pdfBytes);
                    Response.Flush();
                    Response.End();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    SharePointUlsLog.LogMensagem(String.Concat("@@@HOME: ", ex.StackTrace));
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Acessar o portal e aplicar condição de aceite.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAcessar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Clique para acesso."))
            {
                try
                {
                    if (cbxAceite.Checked)
                    {
                        using (var contextoGE = new ContextoWCF<ServicoInformacaoComercialClient>())
                        {
                            // Salva informações de aceite
                            var aceite = contextoGE.Cliente.Alterar(new InformacaoComercialRequest()
                            {
                                NumeroPDV = SessaoAtual.CodigoEntidade,
                                CodigoUsuario = SessaoAtual.CodigoIdUsuario,
                                DataUltimaAlteracao = DateTime.Now,
                                StatusAceite = "S",
                                DataStatusAceite = DateTime.Now
                            });
                            if (aceite.StatusRetorno != Redecard.PN.Extrato.SharePoint.GEServicoInformacaoComercial.StatusRetorno.OK && !String.IsNullOrWhiteSpace(aceite.Mensagem))
                                log.GravarMensagem(aceite.Mensagem);
                        }
                    }

                    lbxDesbloqueio.Visible = false;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    SharePointUlsLog.LogMensagem(String.Concat("@@@HOME: ", ex.StackTrace));
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento para carregar informação de tipo de Endereços no lightbox de Aceite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptEnderecos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var endereco = (EnderecoResponse)e.Item.DataItem as EnderecoResponse;
            var literalTipoEndereco = (Literal)e.Item.FindControl("ltrTipoEndereco");

            if (endereco.TipoEndereco.HasValue)
                literalTipoEndereco.Text = endereco.TipoEndereco == 1 ? "Endereço do Estabelecimento" : "Endereço de Correspondência";
            else
                literalTipoEndereco.Text = String.Empty;
        }

        /// <summary>
        /// Evento para carregar informações de Taxas de domicílios no lightbox de Aceite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptDomicilioBancario_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var domicilioBancario = (DomicilioBancarioResponse)e.Item.DataItem as DomicilioBancarioResponse;
            var grvTaxas = (GridView)e.Item.FindControl("grvTaxas");
            grvTaxas.DataSource = domicilioBancario.Taxas;
            grvTaxas.DataBind();
        }

        #endregion

        #region [Avisos de Senha/Novo Acesso]

        /// <summary>
        /// Configura a versão do Extrato que será utilizada:<br/>
        /// QueryString: .aspx?grandesconsultas=[v]
        /// </summary>
        private void ConfigurarVersaoExtratos()
        {
            ConfiguracaoVersao.VersaoGrandesConsultas(Request);
        }

        /// <summary>
        /// Verifica quando a senha do usuário irá expirar. Se a quantidade de dias for menor ou igual a 15, exibe o aviso
        /// </summary>
        private void CarregarAvisoSenha()
        {
            using (Logger log = Logger.IniciarLog("Início Verificação de quando a senha do usuário irá expirar."))
            {
                try
                {
                    if (Sessao.Contem() && SessaoAtual.PossuiKomerci
                        && !SessaoAtual.AcessoFilial && !SessaoAtual.UsuarioAtendimento)
                    {
                        using (var ctx = new ContextoWCF<Redecard.PN.Extrato.SharePoint.UsuarioServico.UsuarioServicoClient>())
                        {
                            Int32 codRetorno = 0;
                            Redecard.PN.Extrato.SharePoint.UsuarioServico.Usuario usuario = ctx.Cliente.ConsultarDadosUsuario(
                                out codRetorno, SessaoAtual.GrupoEntidade,
                                SessaoAtual.CodigoEntidade, SessaoAtual.LoginUsuario);

                            if (!object.ReferenceEquals(usuario, null) &&
                                !object.ReferenceEquals(usuario.DataExpiracaoSenha, null))
                            {
                                TimeSpan qtdDias = usuario.DataExpiracaoSenha - DateTime.Today;
                                Double diasExpiracao = Math.Truncate(qtdDias.TotalDays);

                                if (diasExpiracao <= 15)
                                {
                                    String link = String.Format("{0}/{1}", base.web.Site.ServerRelativeUrl,
                                        "minhaconta/Paginas/MeuUsuarioCadastroUsuario.aspx");

                                    log.GravarMensagem(link);

                                    String tipoHome = default(String);
                                    if (SessaoAtual.CodigoSegmento.Equals('V') || SessaoAtual.CodigoSegmento.Equals('v'))
                                        tipoHome = "varejo";
                                    else
                                        tipoHome = "empibba";

                                    String mensagem = default(String);
                                    if (diasExpiracao > 0)
                                        mensagem = String.Format(
                                            "Sua senha vai expirar dentro de <b>{0} dia{1}</b>. Para seu maior conforto, <a href='{2}' class='link-rede lnk-outros-home-{3}-gtm'>antecipe a troca</a>.",
                                            diasExpiracao, diasExpiracao > 1 ? "s" : "", link, tipoHome);
                                    else
                                        mensagem = String.Format(
                                            "Sua senha expirou. Por favor, <a href='{0}' class='link-rede lnk-outros-home-{1}-gtm'>efetue a troca</a>.", link, tipoHome);

                                    log.GravarMensagem(mensagem);

                                    qdAvisoSenhaHome.Mensagem = mensagem;
                                    qdAvisoSenhaHome.Visible = true;
                                }
                            }
                        }
                    }
                }
                catch (FaultException<Redecard.PN.Extrato.SharePoint.UsuarioServico.GeneralFault> ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    SharePointUlsLog.LogMensagem(String.Concat("@@@HOME: ", ex.StackTrace));
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    SharePointUlsLog.LogMensagem(String.Concat("@@@HOME: ", ex.StackTrace));
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Verificar se as solicitações de Acesso/Criação de Usuário para a Entidade estão bloqueadas no Portal Aberto
        /// </summary>
        /// <returns>
        /// <para>False - Não estão bloquadas</para>
        /// <para>True - Estão bloqueadas</para>
        /// </returns>
        private Boolean CriacaoAcessoBloqueada()
        {
            Boolean bloqueada = false;

            using (Logger log = Logger.IniciarLog("Verificar se as solicitações de Acesso/Criação de Usuário para a Entidade estão bloqueadas no Portal Aberto"))
            {
                try
                {
                    if (Sessao.Contem() && (SessaoAtual.UsuarioMaster() || SessaoAtual.UsuarioAtendimento) && SessaoAtual.GrupoEntidade == 1)
                    {
                        using (var contextoEntidade = new ContextoWCF<Redecard.PN.Comum.SharePoint.EntidadeServico.EntidadeServicoClient>())
                        {
                            Int32 codigoRetornoPn = 0;
                            Int32 codigoRetornoGe = 0;


                            var entidades = contextoEntidade.Cliente
                                            .Consultar(out codigoRetornoPn,
                                                       out codigoRetornoGe,
                                                       SessaoAtual.CodigoEntidade,
                                                       1); //SessaoAtual.GrupoEntidade - Grupo entidade fixo em 1
                            //pois quando Central de Atendimento retorna 14

                            if (entidades.Length > 0)
                            {
                                log.GravarMensagem("Status PN da Entiade", new { entidades[0].StatusPN, entidades[0].Codigo });

                                bloqueada = (entidades[0].StatusPN.Codigo ==
                                            (Int32)Redecard.PN.Comum.Enumerador.Status.EntidadeBloqueadaConfirmacaoPositiva);

                                if (bloqueada)
                                {
                                    ltrEmailBloqueio.Text = entidades[0].NomeResponsavel;

                                    String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { homeOpenModal('[id$=lbxDesbloqueio]'); }, 'SP.UI.Dialog.js');";
                                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AbrirModalDialog", javaScript, true);
                                }
                                else
                                {
                                    lbxDesbloqueio.Visible = false;
                                }
                            }
                        }
                    }
                }
                catch (FaultException<Redecard.PN.Comum.SharePoint.EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return bloqueada;
        }

        /// <summary>
        /// Redirecionar para o Desbloqueio do Formulário de Criação de Acesso Aberto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDesbloquearAgora_Click(Object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Redirecionar para o Desbloqueio do Formulário de Criação de Acesso Aberto"))
            {
                try
                {
                    QueryStringSegura qs = new QueryStringSegura();
                    qs["DesbloqueioDiretoEntidade"] = "true";
                    qs["DesbloqueioDiretoEntidadeUrlBack"] = Request.Url.AbsoluteUri;

                    String url = String.Format("/sites/fechado/minhaconta/Paginas/Usuarios.aspx?dados={0}", qs.ToString());

                    Response.Redirect(url, false);
                }
                catch (HttpException ex)
                {
                    Logger.GravarErro("Home", ex);
                    SharePointUlsLog.LogErro(ex);
                    SharePointUlsLog.LogMensagem(String.Concat("@@@HOME: ", ex.StackTrace));
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Home", ex);
                    SharePointUlsLog.LogErro(ex);
                    SharePointUlsLog.LogMensagem(String.Concat("@@@HOME: ", ex.StackTrace));
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }/// <summary>


        #endregion

        protected void lnkConfirmacaoPositiva_Click(object sender, EventArgs e)
        {
            try
            {
                base.RedirecionarConfirmacaoPositiva();
            }
            catch (HttpException ex)
            {
                Logger.GravarErro("Home", ex);
                SharePointUlsLog.LogErro(ex);
                SharePointUlsLog.LogMensagem(String.Concat("@@@HOME: ", ex.StackTrace));
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Home", ex);
                SharePointUlsLog.LogErro(ex);
                SharePointUlsLog.LogMensagem(String.Concat("@@@HOME: ", ex.StackTrace));
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }
    }
}