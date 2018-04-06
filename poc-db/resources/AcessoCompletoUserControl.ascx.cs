using Microsoft.SharePoint.Administration;
using System;
using System.ServiceModel;
using System.Web;
using System.Web.UI;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.NovoAcesso.AcessoCompleto
{
    /// <summary>
    /// WebPart de Liberação de Acesso Completo
    /// </summary>
    public partial class AcessoCompletoUserControl : UserControlBase
    {
        #region [Controles da WebPart]

        /// <summary>
        /// cpCriacaoAcesso control.
        /// </summary>
        protected ConfirmacaoPositivaNovoAcesso CpCriacaoAcesso { get { return (ConfirmacaoPositivaNovoAcesso)cpCriacaoAcesso; } }

        #endregion

        #region [Eventos da WebPart]

        /// <summary>
        /// Evento de carregamento do controle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.ValidarPermissao = false;
            base.OnLoad(e);
        }

        /// <summary>
        /// Inicialização da WebPart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Inicialização da WebPart"))
            {
                try
                {
                    // verifica se a página está sendo acessada por administrador para edição de conteúdo
                    #region edição de conteúdo
                    if (SPContext.Current.Site.RootWeb.CurrentUser != null
                        && SPContext.Current.Site.RootWeb.CurrentUser.IsSiteAdmin 
                        || SPContext.Current.FormContext.FormMode == SPControlMode.Edit)
                    {
                        pnlConfirmacaoPositivaAcesso.Visible = true;
                        pnlPermissoesAcesso.Visible = true;
                        return;
                    }
                    #endregion

                    if (!Page.IsPostBack)
                    {
                        Boolean possuiMaster = false;
                        Boolean possuiUsuario = false;
                        Boolean possuiSenhaTemporaria = false;

                        using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        {
                            contextoEntidade.Cliente.PossuiUsuario(out possuiUsuario, out possuiMaster, out possuiSenhaTemporaria, SessaoAtual.CodigoEntidade, 1);
                        }

                        if (!possuiMaster)
                        {
                            if (!Object.ReferenceEquals(Request.QueryString["dados"], null))
                            {
                                QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                                Boolean exibirInformacaoPermissao = queryString["ExibirInformacaoPermissao"].ToBoolNull().Value;

                                /* O BLOCO INFORMATIVO DE PERMISSÃO NÃO É MAIS EXIBIDO:
                                 * O PARÂMETRO "ExibirInformacaoPermissao" DEIXOU DE SER PASSADO COMO "true" NA URL
                                 * - SOURCE: Layouts\DadosCadastrais\Login.aspx.cs
                                 * - MÉTODO: void RedirecionarLiberacaoAcesso() */
                                if (exibirInformacaoPermissao)
                                {
                                    pnlPermissoesAcesso.Visible = true;
                                    pnlConfirmacaoPositivaAcesso.Visible = false;

                                    if (SessaoAtual.StatusPVCancelado())
                                    {
                                        pnlInformacaoEstabelecimentoCancelado.Visible = true;
                                        pnlInformacaoEstabelecimentoAtivo.Visible = false;
                                        btnCancelar.Visible = false;
                                        btnContinuar.Text = "Continuar";
                                    }
                                    else
                                    {
                                        pnlInformacaoEstabelecimentoAtivo.Visible = true;
                                        pnlInformacaoEstabelecimentoCancelado.Visible = false;
                                        btnCancelar.Visible = true;
                                    }
                                }
                                else
                                {
                                    pnlPermissoesAcesso.Visible = false;

                                    pnlConfirmacaoPositivaAcesso.Visible = true;
                                }
                            }
                            else
                            {
                                base.ExibirPainelExcecao("SharePoint.RequestQueryString", 1152);
                            }
                        }
                        else
                        {
                            base.ExibirPainelExcecao("EntidadeServico.PossuiUsuario", 1153);
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
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

#if DEBUG
            if (String.Compare(Request.QueryString["debug"], "true", true) == 0)
            {
                // exibe os controles dinamicamente na página
                String showControls = Request.QueryString["idcontrolspage"];
                if (!String.IsNullOrWhiteSpace(showControls))
                {
                    foreach (var controlId in showControls.Split(','))
                    {
                        var control = this.FindControl(controlId);
                        if (control != null)
                        {
                            control.Visible = true;
                        }
                    }
                }
            }
#endif
        }

        /// <summary>
        /// Processo de liberação de Acesso Completo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Continuar(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Continuando Processo de liberação de Acesso Completo"))
            {
                try
                {
                    if (SessaoAtual != null)
                    {
                        Guid hashEmail = Guid.Empty;
                        Int32 codigoRetorno = 0;
                        EntidadeServico.Entidade1[] entidades = null;
                        Boolean possuiKomerci = false;

                        //Recupera os PVs associados ao usuário (para verificar se alguma possui Komerci)
                        using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                            entidades = ctx.Cliente.ConsultarPorUsuario(out codigoRetorno, SessaoAtual.CodigoIdUsuario);
                        if (codigoRetorno > 0)
                        {
                            base.ExibirPainelExcecao("EntidadeServico.ConsultarPorUsuario", codigoRetorno);
                            return;
                        }

                        //Verifica se algum PV associado ao usuário possui Komerci
                        using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                            possuiKomerci = ctx.Cliente.PossuiKomerci(entidades.Select(entidade => entidade.Codigo).ToArray());

                        //Atualiza os dados do usuário para liberação de acesso completo
                        using (var contexto = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                            codigoRetorno = contexto.Cliente.Atualizar2(out hashEmail,
                                                                        possuiKomerci,
                                                                        SessaoAtual.GrupoEntidade,
                                                                        SessaoAtual.CodigoEntidade.ToString(),
                                                                        SessaoAtual.LoginUsuario,
                                                                        SessaoAtual.NomeUsuario,
                                                                        "M", //Liberação de Acesso completo = Usuário Master
                                                                        String.Empty,
                                                                        String.Empty,
                                                                        SessaoAtual.CodigoIdUsuario,
                                                                        SessaoAtual.Email,
                                                                        SessaoAtual.EmailSecundario,
                                                                        SessaoAtual.CPF,
                                                                        SessaoAtual.DDDCelular,
                                                                        SessaoAtual.Celular,
                                                                        UsuarioServico.Status1.UsuarioAtivo,
                                                                        2,
                                                                        DateTime.Now);
                             
                        if (codigoRetorno > 0)
                        {
                            base.ExibirPainelExcecao("UsuarioServico.Atualizar", codigoRetorno);
                            return;
                        }

                        // JFR - (comentado) a senha passou a ser informada no fluxo de confirmação positiva
                        // UsuarioServico.Usuario usuario = null;
                        // using (var contexto = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        //     usuario = contexto.Cliente.ConsultarPorID(out codigoRetorno, SessaoAtual.CodigoIdUsuario);

                        //Registra no histórico/log de atividades
                        Historico.LiberacaoAcessoCompleto(SessaoAtual);

                        //Persiste a informação de que o usuário já clicou no botão "Liberar Acesso Completo"
                        //e concluiu a confirmação positiva
                        ((ConfirmacaoPositivaNovoAcesso)cpCriacaoAcesso).AtualizarFlagExibicaoMensagemAcessoCompleto();

                        // JFR - obtém a senha informada durante a confirmação positiva
                        String senha = ((ConfirmacaoPositivaNovoAcesso)cpCriacaoAcesso).SenhaValidacao;

                        // JFR - a senha passou a ser informada no fluxo de confirmação positiva
                        base.LoginUsuario(SessaoAtual.CodigoEntidade.ToString(), SessaoAtual.Email, senha, "N", true);
                        // base.LoginUsuario(SessaoAtual.CodigoEntidade.ToString(), SessaoAtual.Email, usuario.Senha, "N");
                    }
                    else
                    {
                        base.ExibirPainelExcecao("SharePoint.ConfirmacaoPositiva", 1051);
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (HttpException ex)
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
        /// Habilita os campos de Confirmação Positiva Completa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkLiberarAcesso_Click(object sender, EventArgs e)
        {
            this.HabilitarCamposConfirmacaoPositiva();
        }

        /// <summary>
        /// Habilita os campos de Confirmação Positiva Completa ou Redireciona para a HomePage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Habilita os campos de Confirmação Positiva Completa ou Redireciona para a HomePage"))
            {
                try
                {
                    //Estabelecimento cancelado
                    if (pnlInformacaoEstabelecimentoCancelado.Visible)
                    {
                        //Persiste a informação de que o usuário já clicou no botão "Continuar"
                        ((ConfirmacaoPositivaNovoAcesso)cpCriacaoAcesso).AtualizarFlagExibicaoMensagemAcessoCompleto();

                        String url = base.RecuperarEnderecoPortalFechado();
                        Response.Redirect(url, false);
                    }
                    else
                    {
                        this.HabilitarCamposConfirmacaoPositiva();
                    }
                }
                catch (HttpException ex)
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
        /// Cancela a liberação de acesso completo e redireciona para a Home Page Fechada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Cancela a liberação de acesso completo e redireciona para a Home Page Fechada"))
            {
                try
                {
                    //Persiste a informação de que o usuário já clicou no botão "Continuar com Acesso Básico"
                    ((ConfirmacaoPositivaNovoAcesso)cpCriacaoAcesso).AtualizarFlagExibicaoMensagemAcessoCompleto();

                    String urlHomeFechada = base.RecuperarEnderecoPortalFechado();
                    Response.Redirect(urlHomeFechada, false);
                }
                catch (HttpException ex)
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
        #endregion

        #region [Métodos Auxiliares]

        /// <summary>
        /// Habilita os campos de Confirmação Positiva Completa
        /// </summary>
        private void HabilitarCamposConfirmacaoPositiva()
        {
            using (Logger log = Logger.IniciarLog("Habilita os campos de Confirmação Positiva Completa"))
            {
                try
                {
                    pnlConfirmacaoPositivaAcesso.Visible = true;
                    CpCriacaoAcesso.MontarConfirmacaoPositiva();
                    pnlPermissoesAcesso.Visible = false;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }


        #endregion
    }
}