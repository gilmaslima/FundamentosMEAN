/*
 * © Copyright 2014 Rede S.A.
 * Autor : Alexandre Shiroma
 * Empresa : Iteris Consultoria e Software
 * 
 * ********************************************************************
 * ALTERADO EM 2015-05-06
 * Editor: Dhouglas Lombello
 * Alterações: Inclusão de Lightbox para Aceite de Condições Comerciais
*/

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.HomePage;
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

namespace Redecard.PN.Extrato.SharePoint.WebParts.Home
{
    public partial class HomeUserControl : BaseUserControl
    {
        #region [ Controles ]

        /// <summary>
        /// qdAvisoSenha control.
        /// </summary>
        protected QuadroAvisoHome QdAvisoSenha { get { return (QuadroAvisoHome)qdAvisoSenha; } }

        /// <summary>
        /// ucVarejoVendas control.
        /// </summary>
        protected VarejoVendas UcVarejoVendas { get { return (VarejoVendas)ucVarejoVendas; } }

        /// <summary>
        /// ucVarejoRecebimentos control.
        /// </summary>
        protected VarejoRecebimentos UcVarejoRecebimentos { get { return (VarejoRecebimentos)ucVarejoRecebimentos; } }

        /// <summary>
        /// ucVarejoAtalhos control.
        /// </summary>
        protected Atalhos UcVarejoAtalhos { get { return (Atalhos)ucVarejoAtalhos; } }

        /// <summary>
        /// ucEmpIbbaAtalhos control.
        /// </summary>
        protected Atalhos UcEmpIbbaAtalhos { get { return (Atalhos)ucEmpIbbaAtalhos; } }

        /// <summary>
        /// lbxDesbloqueio control.
        /// </summary>
        protected LightBox LbxDesbloqueio { get { return (LightBox)lbxDesbloqueio; } }

        /// <summary>
        /// lbxMigracao control.
        /// </summary>
        protected LightBox LbxMigracao { get { return (LightBox)lbxMigracao; } }

        /// <summary>
        /// lbxConfirmacaoPendente control.
        /// </summary>
        protected LightBox LbxConfirmacaoPendente { get { return (LightBox)lbxConfirmacaoPendente; } }

        /// <summary>
        /// lbxCondicoesComerciais control.
        /// </summary>
        protected LightBox LbxCondicoesComerciais { get { return (LightBox)lbxCondicoesComerciais; } }

        #endregion

        #region [ SP Listas ]

        /// <summary>
        /// Lista Período Migração
        /// </summary>
        private SPList ListaPeriodoMigracao
        {
            get
            {
                //Recupera a lista de "Período Migração" em sites/fechado/minhaconta
                using (SPSite spSite = SPContext.Current.Site.WebApplication.Sites["sites/fechado"])
                using (SPWeb spWeb = spSite.AllWebs["minhaconta"])
                    return spWeb.Lists.TryGetList("Período Migração");
            }
        }

        #endregion

        #region [ Propriedades ]

        /// <summary>
        /// WebPart.
        /// </summary>
        public Home WebPart { get { return this.Parent as Home; } }

        #endregion

        #region [ Eventos ]

        /// <summary>
        /// Load da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    CarregarAvisoSenha();
                    ConfigurarVersaoExtratos();
                    if (!SessaoAtual.UsuarioAtendimento)
                        CarregarAceiteCondicoesComerciais();

                    if (!RedirecionarLiberacaoAcessoCompleto())
                        if (!CriacaoAcessoBloqueada())
                            if (!ConfirmacaoMigracaoPendente())
                                VerificarPeriodoMigracao();
                }

                CarregarDatasAcesso();
                CarregarHomePageSegmentada();
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Home", ex);
                SharePointUlsLog.LogErro(ex);
                SharePointUlsLog.LogMensagem("@@@HOME: " + ex.StackTrace);
                base.ExibirPainelExcecao(ex.Fonte, ex.Codigo);

            }
            catch (Exception ex)
            {
                Logger.GravarErro("Home", ex);
                SharePointUlsLog.LogErro(ex);
                SharePointUlsLog.LogMensagem("@@@HOME: " + ex.StackTrace);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Não verificar permissões na página inicial do Portal Redecard Estabelecimento
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.ValidarPermissao = false;
            base.OnLoad(e);
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
                    qs["ExibirConfirmacaoDesbloqueio"] = "true";

                    String url = String.Format("/sites/fechado/minhaconta/Paginas/AprovacaoAcessos.aspx?dados={0}", qs.ToString());

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
        }

        /// <summary>
        /// Registrar que decidiu migrar seus dados depois
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAlterarDepois_Click(Object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Registrar que decidiu migrar seus dados depois"))
            {
                try
                {
                    SessaoAtual.MigrarDepois = true;
                    lbxMigracao.Visible = false;
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Home", ex);
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

        /// <summary>
        /// Redirecionar para o formulário de atulização de usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAlterarAgora_Click(Object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Redirecionar para o formulário de atulização de usuário"))
            {
                try
                {
                    String url = "/sites/fechado/minhaconta/Paginas/CadastroUsuarioMigracao.aspx";

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
        }

        /// <summary>
        /// Reenviar o e-mail de confirmação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkReenviar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Reenviar o e-mail de confirmação"))
            {
                try
                {
                    using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    {
                        Int32 codigoRetorno = 0;

                        UsuarioServico.UsuarioHash[] guidsEmail = contextoUsuario.Cliente.ConsultarHash(out codigoRetorno, SessaoAtual.CodigoIdUsuario, UsuarioServico.Status1.UsuarioAguardandoConfirmacaoAlteracaoEmail, null);

                        if (guidsEmail.Length > 0)
                        {
                            Guid guidEmailUsuario = guidsEmail[0].Hash;

                            EmailNovoAcesso.EnviarEmailConfirmacaoMigracao(SessaoAtual.EmailTemporario,
                                SessaoAtual.CodigoIdUsuario, guidEmailUsuario, SessaoAtual.NomeUsuario, SessaoAtual.TipoUsuario, SessaoAtual.CodigoEntidade, SessaoAtual.Funcional);

                            SessaoAtual.MigrarDepois = true;

                            Panel[] paineis = new Panel[0];

                            base.ExibirPainelConfirmacaoAcao("E-mail enviado com sucesso",
                                                             @"Dentro de instantes, você receberá o e-mail de confirmação. 
                                                               Ao acessar o link informado no e-mail, você será redirecionado para a conclusão de cadastro.",
                                                             this.PeriodoPosInicioMigracao() ?
                                                             base.RecuperarEnderecoPortal() : base.RecuperarEnderecoPortalFechado(),
                                                             paineis);
                        }
                    }
                }
                catch (FaultException<Comum.SharePoint.EntidadeServico.GeneralFault> ex)
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
        }

        /// <summary>
        /// Continuar para a home do Portal Aberto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Continuar para o Portal Aberto"))
            {
                try
                {
                    if (this.PeriodoPosInicioMigracao()) //Redirecionar para a tela de migração
                    {
                        Response.Redirect(base.RecuperarEnderecoPortal(), false);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
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
                            if (aceite.StatusRetorno != GEServicoInformacaoComercial.StatusRetorno.OK && !String.IsNullOrWhiteSpace(aceite.Mensagem))
                                log.GravarMensagem(aceite.Mensagem);
                        }
                    }

                    LbxDesbloqueio.Visible = false;
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

                    //Esta é a versão somente para testes locais.
                    using (SPSite site = new SPSite(SPContext.Current.Site.ID, SPUrlZone.Default))
                        url = site.MakeFullUrl(String.Format("/_layouts/Redecard.PN.Extrato.SharePoint/DownloadAceiteCondicoesComerciais.aspx?num_pdv={0}", queryString.ToString()));

                    //Esta é a versão para ambientes Rede.
                    //StringBuilder urlComposta = new StringBuilder();
                    //urlComposta.Append("http://");
                    //urlComposta.Append("localhost/");
                    //urlComposta.Append("_layouts/Redecard.PN.Extrato.SharePoint/DownloadAceiteCondicoesComerciais.aspx?num_pdv=");
                    //urlComposta.Append(queryString.ToString());
                    //url = urlComposta.ToString();

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
        #endregion

        #region [ Métodos Privados ]
        /// <summary>
        /// Verificar se a data atual é Pós ou Durante Migração
        /// </summary>
        /// <returns></returns>
        private Boolean PeriodoPosInicioMigracao()
        {
            Boolean periodoPosMigracao = false;

            using (Logger log = Logger.IniciarLog("Continuar para o Portal Aberto"))
            {
                try
                {
                    if (this.ListaPeriodoMigracao != null)
                    {
                        SPQuery query = new SPQuery();
                        query.Query = String.Concat(
                            "<Where>",
                                "<Eq>",
                                    "<FieldRef Name=\"Title\" />",
                                    "<Value Type=\"Text\">Período Primeira Fase</Value>",
                                "</Eq>",
                            "</Where>");

                        SPListItemCollection periodo = this.ListaPeriodoMigracao.GetItems(query);

                        if (periodo.Count > 0)
                        {
                            DateTime dataInicio;
                            DateTime dataFinal;

                            if (!DateTime.TryParse(periodo[0]["DataInicio"].ToString(), out dataInicio))
                            {
                                log.GravarMensagem("Erro ao converter a data de início");
                                periodoPosMigracao = false;
                            }

                            if (!DateTime.TryParse(periodo[0]["DataFinal"].ToString(), out dataFinal))
                            {
                                log.GravarMensagem("Erro ao converter a data de final");
                                periodoPosMigracao = false;
                            }

                            if (DateTime.Today >= dataInicio)
                            {
                                if (DateTime.Today >= dataFinal) //Período de Pós Migração
                                {
                                    periodoPosMigracao = true;
                                }
                            }
                        }
                    }

                }
                catch (SPException ex)
                {
                    log.GravarErro(ex);
                    periodoPosMigracao = false;
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    periodoPosMigracao = false;
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return periodoPosMigracao;
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
                        using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            Int32 codRetorno = 0;
                            UsuarioServico.Usuario usuario = ctx.Cliente.ConsultarDadosUsuario(
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
                                        "minhaconta/Paginas/MeuUsuarioAlteracaoSenha.aspx");
                                    log.GravarMensagem(link);

                                    String mensagem = default(String);
                                    if (diasExpiracao > 0)
                                        mensagem = String.Format(
                                            "Sua senha vai expirar dentro de <b>{0} dia{1}</b>. Para seu maior conforto, <a href='{2}'>antecipe a troca</a>.",
                                            diasExpiracao, diasExpiracao > 1 ? "s" : "", link);
                                    else
                                        mensagem = String.Format(
                                            "Sua senha expirou. Por favor, <a href='{0}'>efetue a troca</a>.", link);

                                    log.GravarMensagem(mensagem);
                                    QdAvisoSenha.CarregarMensagem("Atenção!", mensagem, QuadroAvisoHome.Icone.Aviso);
                                    QdAvisoSenha.Visible = true;
                                }
                            }
                        }
                    }
                }
                catch (FaultException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    SharePointUlsLog.LogMensagem(String.Concat("@@@HOME: ", ex.StackTrace));
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Message, ex.Code.ToString().ToInt32());
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
        /// Configura a versão do Extrato que será utilizada:<br/>
        /// QueryString: .aspx?grandesconsultas=[v]
        /// </summary>
        private void ConfigurarVersaoExtratos()
        {
            ConfiguracaoVersao.VersaoGrandesConsultas(Request);
        }

        /// <summary>
        /// Carrega as datas Atual e de Último Acesso
        /// </summary>
        private void CarregarDatasAcesso()
        {
            lblDataAtual.Text = DateTime.Today.ToString("dd/MM/yyyy");
            if (SessaoAtual != null)
                lblDataUltimoAcesso.Text = SessaoAtual.UltimoAcesso.ToString("dd/MM/yyyy 'às' HH'h'mm");
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
        private void CarregarHomePageSegmentada()
        {
            if (Sessao.Contem())
            {
                Char codigoSegmento = SessaoAtual.CodigoSegmento;
#if DEBUG
                if (!String.IsNullOrEmpty(Request["Segmento"]))
                    codigoSegmento = Request["Segmento"][0];
#endif
                switch (codigoSegmento)
                {
                    case 'V':
                    case 'v':
                        this.ExibirHomePageVarejo();
                        break;
                    default:
                        this.ExibirHomePageEmpIbba();
                        break;
                }
            }
            else
            {
                this.ExibirHomePageVarejo();
                this.ExibirHomePageEmpIbba();
            }
        }

        /// <summary>
        /// Carrega a HomePage Varejo
        /// </summary>
        private void ExibirHomePageVarejo()
        {
            UcVarejoRecebimentos.ConfiguracaoAtalhos = this.WebPart.VarejoRecebimentosAtalhos;
            UcVarejoVendas.ConfiguracaoAtalhos = this.WebPart.VarejoVendasAtalhos;
            UcVarejoAtalhos.ConfiguracaoAtalhos = this.WebPart.VarejoAtalhos;
            pnlVarejo.Visible = true;
        }

        /// <summary>
        /// Carrega a HomePage EMP/IBBA
        /// </summary>
        private void ExibirHomePageEmpIbba()
        {
            UcEmpIbbaAtalhos.ConfiguracaoAtalhos = this.WebPart.EmpIbbaAtalhos;
            pnlEmpIbba.Visible = true;
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
                        using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
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
                                            (Int32)Comum.Enumerador.Status.EntidadeBloqueadaConfirmacaoPositiva);

                                if (bloqueada)
                                {
                                    ltrEmailBloqueio.Text = entidades[0].NomeResponsavel;

                                    LbxDesbloqueio.Exibir();
                                }
                                else
                                {
                                    LbxDesbloqueio.Visible = false;
                                }
                            }
                        }
                    }
                }
                catch (FaultException<Comum.SharePoint.EntidadeServico.GeneralFault> ex)
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
        /// Verificar se já está no período de migração de usuários e exibir o painel para usuários legados
        /// </summary>
        private void VerificarPeriodoMigracao()
        {
            using (Logger log = Logger.IniciarLog("Verifica se já está no período de migração de usuários"))
            {
                if (Sessao.Contem())
                {
                    try
                    {
                        log.GravarMensagem("Dados para migração do usuário", new { SessaoAtual.MigrarDepois, SessaoAtual.Legado });

                        if (!SessaoAtual.MigrarDepois && SessaoAtual.Legado && SessaoAtual.GrupoEntidade == 1)
                        {
                            if (this.ListaPeriodoMigracao != null)
                            {
                                SPQuery query = new SPQuery();
                                query.Query = String.Concat(
                                    "<Where>",
                                        "<Eq>",
                                            "<FieldRef Name=\"Title\" />",
                                            "<Value Type=\"Text\">Período Primeira Fase</Value>",
                                        "</Eq>",
                                    "</Where>");

                                SPListItemCollection periodo = this.ListaPeriodoMigracao.GetItems(query);

                                if (periodo.Count > 0)
                                {
                                    DateTime dataInicio;
                                    DateTime dataFinal;

                                    if (!DateTime.TryParse(periodo[0]["DataInicio"].ToString(), out dataInicio))
                                    {
                                        log.GravarMensagem("Erro ao converter a data de início");
                                        return;
                                    }

                                    if (!DateTime.TryParse(periodo[0]["DataFinal"].ToString(), out dataFinal))
                                    {
                                        log.GravarMensagem("Erro ao converter a data de final");
                                        return;
                                    }

                                    if (DateTime.Today >= dataInicio)
                                    {
                                        if (DateTime.Today >= dataFinal) //Redirecionar para a tela de migração
                                        {
                                            String url = "/sites/fechado/minhaconta/Paginas/CadastroUsuarioMigracao.aspx";

                                            Response.Redirect(url, false);
                                        }

                                        LbxMigracao.Exibir();
                                        ltrPeriodoFinal.Text = dataFinal.ToShortDateString();
                                    }
                                }
                            }
                        }
                    }
                    catch (SPException ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }
        }

        /// <summary>
        /// Verificar se o usuário está pendente de confirmar e-mail de migração
        /// </summary>
        /// <returns></returns>
        private Boolean ConfirmacaoMigracaoPendente()
        {
            Boolean pendente = false;

            using (Logger log = Logger.IniciarLog("Verificar se o usuário está pendente de confirmar e-mail de migração"))
            {
                if (Sessao.Contem())
                {
                    try
                    {
                        pendente = SessaoAtual.Legado && (SessaoAtual.CodigoStatus.Equals(Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoAlteracaoEmail));

                        if (pendente && !SessaoAtual.MigrarDepois)
                        {
                            DateTime dataFinal = new DateTime(2014, 12, 17); //Data default

                            LbxConfirmacaoPendente.Exibir();

                            if (this.ListaPeriodoMigracao != null)
                            {
                                SPQuery query = new SPQuery();
                                query.Query = String.Concat(
                                    "<Where>",
                                        "<Eq>",
                                            "<FieldRef Name=\"Title\" />",
                                            "<Value Type=\"Text\">Período Primeira Fase</Value>",
                                        "</Eq>",
                                    "</Where>");

                                SPListItemCollection periodo = this.ListaPeriodoMigracao.GetItems(query);

                                if (periodo.Count > 0)
                                {
                                    if (!DateTime.TryParse(periodo[0]["DataFinal"].ToString(), out dataFinal))
                                    {
                                        log.GravarMensagem("Erro ao converter a data de final");
                                    }
                                }
                            }

                            ltrDataConfirmacaoMigracao.Text = dataFinal.ToShortDateString();

                            //TODO NOVO ACESSO: Verificar por que existe esta condição.
                            //if (this.PeriodoPosInicioMigracao())
                            lnkReenviar.OnClientClick = "lbxConfirmacaoPendente.hide(); return true;";
                            //else                                
                            //lnkReenviar.OnClientClick = "lbxConfirmacaoPendente.hide(); return false;";
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
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }

            return pendente;
        }

        /// <summary>
        /// Validar se o usuário
        /// </summary>
        /// <returns></returns>
        private Boolean RedirecionarLiberacaoAcessoCompleto()
        {
            Boolean redirecionar = false;

            if (Sessao.Contem() && SessaoAtual.LiberarAcessoCompleto)
            {
                QueryStringSegura qs = new QueryStringSegura();
                qs["ExibirInformacaoPermissao"] = "true";

                String url = String.Format("/sites/fechado/minhaconta/Paginas/LiberacaoAcessoCompleto.aspx?dados={0}", qs.ToString());

                Response.Redirect(url, false);

                redirecionar = true;

                base.SessaoAtual.LiberarAcessoCompleto = false;
            }

            return redirecionar;
        }

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

                            // Mostra lightbox ao usuário
                            LbxCondicoesComerciais.Exibir();
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
        /// Converte string para TitleCase
        /// </summary>
        /// <param name="texto">Texto a ser convertido</param>
        /// <returns></returns>
        private string ToTitleCase(string texto)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto.ToLower());
        }
        #endregion
    }
}