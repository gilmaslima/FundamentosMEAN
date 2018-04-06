using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.Net.Mail;
using Redecard.PNCadastrais.Core.Web.Controles.Portal;
using Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.ConclusaoCriacao
{
    public partial class ConclusaoCriacaoUserControl : UserControlBase
    {
        #region [ Propriedades ]

        /// <summary>
        /// Constante para controle do tempo mínimo para reenvio de e-mail
        /// </summary>
        private const Int32 TimeEnvioEmail = 20;

        /// <summary>
        /// Modelo complementar de UsuarioModel para reutilização dos dados do usuário passados na URL
        /// </summary>
        private UsuarioCriacaoModel usuarioModel;

        /// <summary>
        /// Modelo com os dados do usuário passados na URL
        /// </summary>
        public UsuarioCriacaoModel UsuarioModel
        {
            get
            {
                if (this.usuarioModel == null)
                {
                    if (Request.QueryString["dados"] == null)
                        return null;

                    QueryStringSegura q = new QueryStringSegura(Request.QueryString["dados"]);
                    if (q == null)
                        return null;

                    this.usuarioModel = UsuarioNegocio.ConvertFromQueryStringSegura(q);
                }

                return this.usuarioModel;
            }
        }

        /// <summary>
        /// Controla se deve permitir o reenvio de e-mail
        /// </summary>
        public bool PermiteReenvio
        {
            get
            {
                DateTime? ultimoEnvio = (DateTime?)Session["UltimoEnvioEmail"];
                if (!ultimoEnvio.HasValue)
                {
                    Session["UltimoEnvioEmail"] = DateTime.Now;
                    return true;
                }

                return (DateTime.Now - ultimoEnvio.Value).TotalMinutes >= TimeEnvioEmail;
            }
        }

        #endregion

        #region [Eventos da página]

        /// <summary>
        /// Inicialização da webpart de conclusão da cadastro de usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (Logger log = Logger.IniciarLog("Inicialização da webpart de conclusão da cadastro de usuário"))
                {
                    try
                    {
                        pnMensagem.Visible = false;
                        pnlAvisoConclusao.Visible = true;
                        pnlAvisoEmail.Visible = true;

                        // preenche literal com o e-mail do remetente padrão dos e-mails novo acesso
                        ltrRemetente.Text = EmailNovoAcesso.Remetente;

                        if (this.UsuarioModel != null)
                        {
                            if (this.UsuarioModel.EntidadePossuiMaster)
                            {
                                this.lblMensagemConclusao.Text = @"
Sua solicitação foi enviada com sucesso para os usuários Master do seu estabelecimento.
Assim que um Master aprovar sua solicitação, você receberá um e-mail com as informações para a conclusão de seu cadastro.";

                                pnlAvisoEmail.Visible = false;
                            }
                            else
                            {
                                this.lblMensagemConclusao.Text = @"
Dentro de instantes você receberá um e-mail de confirmação.
Acesse o link informado no e-mail em até 12h para concluir seu cadastro.";
                            }
                        }
                        else
                        {
                            pnlAvisoEmail.Visible = false;
                            pnlAvisoConclusao.Visible = false;
                            this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", base.RecuperarEnderecoPortal());
                        }
                    }
                    catch (PortalRedecardException ex)
                    {
                        log.GravarErro(ex);
                        this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                    }
                }
            }
        }

        /// <summary>
        /// Reenviar o e-mail de Confirmação para o usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkReenviarEmail_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Reenviar o e-mail de Confirmação para o usuário"))
            {
                try
                {
                    pnMensagem.Visible = false;
                    pnlAvisoConclusao.Visible = true;
                    pnlAvisoEmail.Visible = true;

                    if (this.UsuarioModel != null)
                    {
                        if (this.PermiteReenvio)
                        {
                            EmailNovoAcesso.EnviarEmailConfirmacaoCadastro12h(
                                this.UsuarioModel.Email,
                                this.UsuarioModel.IdUsuario,
                                this.UsuarioModel.HashEmail,
                                this.UsuarioModel.IdUsuario,
                                this.UsuarioModel.Email,
                                this.UsuarioModel.Nome,
                                this.UsuarioModel.TipoUsuario,
                                this.UsuarioModel.PvsSelecionados.Length > 0 ? this.UsuarioModel.PvsSelecionados[0] : 0,
                                String.Empty,
                                true,
                                this.UsuarioModel.CpfUsuario,
                                this.UsuarioModel.PvsSelecionados.Length > 0 ? this.UsuarioModel.PvsSelecionados : null);

                            this.ExibirModal(
                                "E-mail enviado com sucesso",
                                "Verifique a caixa de entrada do seu e-mail",
                                TipoModal.Success);
                        }
                        else
                        {
                            this.ExibirModal(
                                "Atenção",
                                String.Format("Já enviamos o seu e-mail. Se não receber em até {0} minutos, você pode tentar novamente.", TimeEnvioEmail),
                                TipoModal.Warning);
                        }
                    }
                    else
                    {
                        pnlAvisoEmail.Visible = false;
                        pnlAvisoConclusao.Visible = false;
                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", base.RecuperarEnderecoPortal());
                    }
                }
                catch (SmtpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                }
            }
        }

        #endregion

        #region [Métodos auxiliares]
        
        /// <summary>
        /// Exibe a mensagem de erro com o link de Voltar caso seja fornecida a URL
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <param name="titulo">Título para o quadro de aviso</param>
        /// <param name="urlVoltar">Url de retorno para a mensagem de erro</param>
        private void ExibirErro(String fonte, Int32 codigo, String titulo, String urlVoltar)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            pnMensagem.Titulo = titulo;
            pnMensagem.Mensagem = mensagem;
            pnMensagem.TipoMensagem = PainelMensagemIcon.Erro;

            if (!String.IsNullOrEmpty(urlVoltar))
            {
                pnlVoltar.Visible = true;
                btnVoltar.OnClientClick = String.Format("window.location.href='{0}'; return false;", urlVoltar);
            }

            pnMensagem.Visible = true;

            pnlAvisoConclusao.Visible = false;
            pnlAvisoEmail.Visible = false;
        }

        /// <summary>
        /// Exibe a modal de aviso via javascript
        /// </summary>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        /// <param name="tipoModal">Tipo relacionado ao ícone</param>
        private void ExibirModal(String titulo, String mensagem, TipoModal tipoModal)
        {
            String script = String.Format(@"ExibirAvisoLightbox('{0}', '{1}', {2});", titulo, mensagem, (Int32)tipoModal);
            Page.ClientScript.RegisterStartupScript(Page.GetType(), "ExibirAvisoLightBox", script, true);
        }

        #endregion
    }
}
