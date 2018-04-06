using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario;
using Redecard.PNCadastrais.Core.Web.Controles.Portal;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using System.Web;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.FormaEnvioRecuperacao
{
    /// <summary>
    /// WebPart de forma de envio da recuperação do Usuário
    /// </summary>
    public partial class FormaEnvioRecuperacaoUserControl : UserControlBase
    {
        /// <summary>
        /// Controle PainelMensagem na página
        /// </summary>
        public PainelMensagem PainelMensagem
        {
            get
            {
                return (PainelMensagem)this.pnMensagem;
            }
        }

        #region [Propriedades da Página]

        /// <summary>
        /// Constante para controle do tempo mínimo para reenvio de e-mail
        /// </summary>
        private const Int32 TimeEnvioEmail = 20;

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

        /// <summary>
        /// Info do usuário
        /// </summary>
        private InformacaoUsuario _infoUsuario
        {
            get
            {
                if (InformacaoUsuario.Existe())
                {
                    return InformacaoUsuario.Recuperar();
                }

                return null;

            }
        }

        /// <summary>
        /// Usuario de autentificação para envio de SMS
        /// </summary>
        private String UsuarioSms
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["usuarioSMS"]);
            }
        }

        /// <summary>
        /// Senha de autentificação para envio de SMS
        /// </summary>
        private String SenhaSms
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["senhaSMS"]);
            }
        }

        #endregion

        #region [ Eventos da Página ]
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
                    if (!IsPostBack)
                    {
                        // preenche literal com o e-mail do remetente padrão dos e-mails novo acesso
                        ltrRemetente.Text = EmailNovoAcesso.Remetente;

                        if (Request["rbEmailPrincipal"] != null &&
                            Request["rbEmailSecundario"] != null &&
                            Request["rbSMS"] != null)
                        {
                            this.EnviarClick(Convert.ToBoolean(Request["rbEmailPrincipal"]), Convert.ToBoolean(Request["rbEmailSecundario"]), Convert.ToBoolean(Request["rbSMS"]));
                        }
                        else
                        {
                            this.ExibirErro("SharePoint.SessaoUsuario", 1151, base.RecuperarEnderecoPortal());
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, this.RecuperarEnderecoPortal());
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, base.RecuperarEnderecoPortal());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, base.RecuperarEnderecoPortal());
                }
            }
        }

        /// <summary>
        /// Enviar o e-mail/SMS de recuperação de senha 
        /// </summary>
        public void EnviarClick(Boolean rbEmailPrincipalChecked, Boolean rbEmailSecundarioChecked, Boolean rbSMSChecked)
        {
            using (Logger log = Logger.IniciarLog("Enviar o e-mail/SMS de recuperação de senha"))
            {
                try
                {
                    if (InformacaoUsuario.Existe())
                    {
                        Guid hashEmail = Guid.Empty;
                        Int32 codigoRetorno = 0;

                        InformacaoUsuario usuario = InformacaoUsuario.Recuperar();

                        if (rbEmailPrincipalChecked || rbEmailSecundarioChecked)
                        {
                            log.GravarMensagem("Atualizando status e criando hash de e-mail");

                            using (var ctxUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                            {
                                hashEmail = ctxUsuario.Cliente.AtualizarStatusParaAguardandoConfirRecSenha(usuario.IdUsuario,
                                                                                                           GetEmail(),
                                                                                                           0.5,
                                                                                                           DateTime.Now,
                                                                                                           usuario.PvsSelecionados);

                                log.GravarMensagem("Hash criada", new { hashEmail = hashEmail });
                            }
                        }

                        if (codigoRetorno == 0)
                        {
                            log.GravarMensagem("Código de retorno", new { codigoRetorno = codigoRetorno });

                            usuario.HashEmail = hashEmail;
                            InformacaoUsuario.Salvar(usuario);

                            if (rbEmailPrincipalChecked)
                            {
                                log.GravarMensagem("Recuperação de senha sendo enviada para email principal", new { checkedEmailPrincipal = rbEmailPrincipalChecked });

                                usuario.FormaRecuperacaoSenha = FormaRecuperacao.EmailPrincipal;
                                this.EnviarParaEmailPrincipal(usuario);
                            }
                            else if (rbEmailSecundarioChecked)
                            {
                                log.GravarMensagem("Recuperação de senha sendo enviada para email secundario", new { checkedEmailSecundario = rbEmailSecundarioChecked });

                                usuario.FormaRecuperacaoSenha = FormaRecuperacao.EmailSecundario;
                                this.EnviarParaEmailSecundario(usuario);
                            }
                            else if (rbSMSChecked)
                            {
                                log.GravarMensagem("Recuperação de senha sendo enviada via SMS", new { checkedSms = rbSMSChecked });

                                usuario.FormaRecuperacaoSenha = FormaRecuperacao.Sms;
                                this.EnviarSMS(usuario);
                            }

                            InformacaoUsuario.Salvar(usuario);
                        }
                        else
                        {
                            log.GravarMensagem("Retorno indevido", new { codigoRetorno = codigoRetorno });
                            this.ExibirErro("UsuarioServico.AtualizarStatus", 1151, Request.Url.AbsoluteUri);
                        }
                    }
                    else
                    {
                        log.GravarMensagem("Informações inexistentes no cache", new { InformacoesExistentesCache = InformacaoUsuario.Existe() });
                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, base.RecuperarEnderecoPortal());
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, base.RecuperarEnderecoPortal());
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, base.RecuperarEnderecoPortal());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, base.RecuperarEnderecoPortal());
                }
            }
        }

        /// <summary>
        /// Reenviar o e-mail de confirmação de cadastro de nova senha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkReenviarEmail_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Reenviar o e-mail de confirmação de cadastro de nova senha"))
            {
                try
                {
                    if (InformacaoUsuario.Existe())
                    {
                        if (this.PermiteReenvio)
                        {
                            InformacaoUsuario info = InformacaoUsuario.Recuperar();
                            String emailUsuario = String.Empty;

                            if (info.FormaRecuperacaoSenha == FormaRecuperacao.EmailPrincipal)
                            {
                                emailUsuario = info.EmailUsuario;
                            }
                            else
                            {
                                emailUsuario = info.EmailSecundario;
                            }

                            EmailNovoAcesso.EnviarEmailRecuperacaoSenha(emailUsuario,
                                info.IdUsuario, info.HashEmail, info.FormaRecuperacaoSenha,
                                info.IdUsuario, info.EmailUsuario, info.NomeCompleto, info.TipoUsuario, info.NumeroPV, null);

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
                        log.GravarMensagem("Dados inexistentes no cache", new { PossuiDadosCache = InformacaoUsuario.Existe() });

                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, base.RecuperarEnderecoPortal());
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, this.RecuperarEnderecoPortal());
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, base.RecuperarEnderecoPortal());
                }
                catch (SmtpException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, base.RecuperarEnderecoPortal());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, base.RecuperarEnderecoPortal());
                }
            }
        }

        /// <summary>
        /// Solicitar outro código de recuperção por SMS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSolicitarOutroCodigo_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Solicitar outro código de recuperção por SMS"))
            {
                try
                {
                    if (InformacaoUsuario.Existe())
                    {
                        this.EnviarSMS(InformacaoUsuario.Recuperar());
                        this.ExibirMensagemReenvioCodigo();
                    }
                    else
                    {
                        log.GravarMensagem("Dados inexistentes no cache", new { PossuiDadosCache = InformacaoUsuario.Existe() });
                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, base.RecuperarEnderecoPortal());
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, Request.Url.AbsoluteUri);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, Request.Url.AbsoluteUri);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, Request.Url.AbsoluteUri);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, Request.Url.AbsoluteUri);
                }
            }
        }

        /// <summary>
        /// Validar se o código inserido está correto e redirecionar para o próximo passo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviarCodigo_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Validar se o código inserido está correto e redirecionar para o próximo passo"))
            {
                try
                {
                    if (!Page.IsValid)
                    {
                        log.GravarMensagem("Dados inválidos");
                        return;
                    }

                    if (InformacaoUsuario.Existe())
                    {
                        InformacaoUsuario infoUsuario = InformacaoUsuario.Recuperar();

                        String codigo = txtCodigo.Text;

                        if (infoUsuario.CodigoRecuperacaoSMS.Equals(codigo))
                        {
                            log.GravarMensagem("Código que o usuário digitou confere");

                            String urlRedirecionar = String.Empty;
                            infoUsuario.PodeRecuperarCriarAcesso = true;
                            InformacaoUsuario.Salvar(infoUsuario);

                            urlRedirecionar = String.Format("{0}/Paginas/RecNovaSenhaBasico.aspx", base.web.ServerRelativeUrl);

                            log.GravarMensagem("Iniciando redirect", new { Pagina = urlRedirecionar });

                            Response.Redirect(urlRedirecionar, false);
                        }
                        else
                        {
                            log.GravarMensagem("Código que o usuário digitou não confere");

                            txtCodigo.Text = String.Empty;
                            crvCodigo.IsValid = false;
                            crvCodigo.ErrorMessage = "Código inválido. Digite o código novamente ou solicite um novo";
                        }
                    }
                    else
                    {
                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, base.RecuperarEnderecoPortal());
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, Request.Url.AbsoluteUri);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, Request.Url.AbsoluteUri);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, Request.Url.AbsoluteUri);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, Request.Url.AbsoluteUri);
                }
            }
        }

        #endregion

        #region [ Métodos auxiliares ]

        /// <summary>
        /// Envia o email de recuperação para o E-mail principal
        /// </summary>
        private void EnviarParaEmailPrincipal(InformacaoUsuario usuario)
        {
            string pvsSelecinados = usuario.GetPvsSelecionados();
            int numeroPv =
                usuario.PvsSelecionados != null && usuario.PvsSelecionados.Length > 0 ?
                usuario.PvsSelecionados.FirstOrDefault() :
                usuario.NumeroPV;

            EmailNovoAcesso.EnviarEmailRecuperacaoSenha(usuario.EmailUsuario,
                                                    usuario.IdUsuario,
                                                    usuario.HashEmail,
                                                    usuario.FormaRecuperacaoSenha,
                                                    usuario.IdUsuario,
                                                    usuario.EmailUsuario,
                                                    usuario.NomeCompleto,
                                                    usuario.TipoUsuario,
                                                    numeroPv,
                                                    null,
                                                    pvsSelecinados);

            if (!String.IsNullOrEmpty(usuario.EmailSecundario))
            {
                try
                {

                    EmailNovoAcesso.EnviarEmailInformativoRecuperacaoSenha(usuario.EmailSecundario,
                                                                           FormaRecuperacao.EmailPrincipal,
                                                                           usuario.IdUsuario,
                                                                           usuario.NomeCompleto,
                                                                           usuario.TipoUsuario,
                                                                           numeroPv,
                                                                           null, pvsSelecinados);
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    Logger.GravarLog("Erro ao enviar email informativo", ex);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Logger.GravarLog("Erro ao enviar email informativo", ex);
                    SharePointUlsLog.LogErro(ex);
                }
            }
            else if (usuario.DddCelularUsuario > 0 && usuario.CelularUsuario > 0)
            {
                String mensagem = "Portal Rede: Voce solicitou uma nova senha. Caso nao tenha sido voce, ligue para 4001-4433 ou 0800 784433";
                String numeroCelular = String.Concat("55", usuario.DddCelularUsuario.ToString(), usuario.CelularUsuario.ToString());

                try
                {
                    this.EnviarSMS(mensagem, numeroCelular);
                }
                catch (ServiceActivationException ex)
                {
                    //Grava no Log, mas não exibe mensagem de erro ao usuários
                    Logger.GravarErro("Erro ao enviar SMS Informativo de Recuperação de Senha", ex);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    //Grava no Log, mas não exibe mensagem de erro ao usuários
                    Logger.GravarErro("Erro ao enviar SMS Informativo de Recuperação de Senha", ex);
                    SharePointUlsLog.LogErro(ex);
                }
            }

            pnlAvisoEmail.Visible = true;

            pnlAviso.Visible = false;
            pnlCodigoRecuperacao.Visible = false;
        }

        /// <summary>
        /// Envia o email de recuperação para o E-mail secundário
        /// </summary>
        private void EnviarParaEmailSecundario(InformacaoUsuario usuario)
        {
            string pvsSelecinados = usuario.GetPvsSelecionados();
            int numeroPv =
                usuario.PvsSelecionados != null && usuario.PvsSelecionados.Length > 0 ?
                usuario.PvsSelecionados.FirstOrDefault() :
                usuario.NumeroPV;

            EmailNovoAcesso.EnviarEmailRecuperacaoSenha(usuario.EmailSecundario,
                                                        usuario.IdUsuario,
                                                        usuario.HashEmail,
                                                        usuario.FormaRecuperacaoSenha,
                                                        usuario.IdUsuario,
                                                        usuario.EmailUsuario,
                                                        usuario.NomeCompleto,
                                                        usuario.TipoUsuario,
                                                        numeroPv,
                                                        null,
                                                        pvsSelecinados);

            if (usuario.DddCelularUsuario > 0 && usuario.CelularUsuario > 0)
            {
                String numeroCelular = String.Concat("55", usuario.DddCelularUsuario.ToString(), usuario.CelularUsuario.ToString());
                String mensagem = "Portal Rede: Voce solicitou uma nova senha. Caso nao tenha sido voce, ligue para 4001-4433 ou 0800 784433";

                try
                {
                    this.EnviarSMS(mensagem, numeroCelular);
                }
                catch (ServiceActivationException ex)
                {
                    //Grava no Log, mas não exibe mensagem de erro ao usuários
                    Logger.GravarErro("Erro ao enviar SMS Informativo de Recuperação de Senha", ex);
                }
                catch (Exception ex)
                {
                    //Grava no Log, mas não exibe mensagem de erro ao usuários
                    Logger.GravarErro("Erro ao enviar SMS Informativo de Recuperação de Senha", ex);
                }
            }
            else if (!String.IsNullOrEmpty(usuario.EmailUsuario))
            {
                try
                {
                    EmailNovoAcesso.EnviarEmailInformativoRecuperacaoSenha(usuario.EmailUsuario, FormaRecuperacao.EmailSecundario,
                        usuario.IdUsuario, usuario.NomeCompleto, usuario.TipoUsuario, numeroPv, null, pvsSelecinados);
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    Logger.GravarLog("Erro ao enviar email informativo", ex);
                }
                catch (Exception ex)
                {
                    Logger.GravarLog("Erro ao enviar email informativo", ex);
                }
            }

            pnlAvisoEmail.Visible = true;

            pnlAviso.Visible = false;
            pnlCodigoRecuperacao.Visible = false;
        }

        /// <summary>
        /// Envia o SMS de recuperação para o Celular do usuário
        /// </summary>
        private void EnviarSMS(InformacaoUsuario usuario)
        {
            String numeroCelular = String.Format("55{0}{1}",
                                                 usuario.DddCelularUsuario.ToString(),
                                                 usuario.CelularUsuario.ToString());
#if DEBUG
            String codigo = "123456";

            String mensagem = String.Format(
                            "Portal Rede: Seu codigo de recuperacao de senha e {0}.",
                            codigo);

            string retornoEnvio = "OK";

            if (!String.IsNullOrEmpty(Request.QueryString["retornoEnvio"]))
                retornoEnvio = Request.QueryString["retornoEnvio"];

#else
                        String codigo = this.GerarCodigoSMS();

                        String mensagem = String.Format(
                                        "Portal Rede: Seu codigo de recuperacao de senha e {0}.",
                                        codigo);

                        String retornoEnvio = this.EnviarSMSRecuperacao(mensagem, numeroCelular, usuario.IdUsuario);
#endif


            if (String.Compare(retornoEnvio, "OK", true) == 0)
            {
                usuario.CodigoRecuperacaoSMS = codigo;
                InformacaoUsuario.Salvar(usuario);
                lnkOutraFormaRecuperacao.NavigateUrl = String.Format("{0}/Paginas/RecuperacaoSenhaIdentificacao.aspx?outraForma=true", SPContext.Current.Web.Url);
                pnlCodigoRecuperacao.Visible = true;

                pnlAvisoEmail.Visible = false;
                pnlAviso.Visible = false;
            }
            else if (String.Compare(retornoEnvio, "NOK2", true) == 0)
            {
                this.ExibirAviso("Sua última solicitação foi feita há menos de 20 minutos. Por favor, aguarde.", true);
                return;
            }
            else //Algum erro
            {
                this.ExibirErro("Ocorreu um erro ao enviar o SMS. Tente novamente.", true);
                return;
            }
            try
            {
                if (!String.IsNullOrEmpty(usuario.EmailUsuario))
                {
                    EmailNovoAcesso.EnviarEmailInformativoRecuperacaoSenha(usuario.EmailUsuario, FormaRecuperacao.Sms,
                        usuario.IdUsuario, usuario.NomeCompleto, usuario.TipoUsuario, usuario.NumeroPV, null);
                }
                else if (!String.IsNullOrEmpty(usuario.EmailSecundario))
                {
                    EmailNovoAcesso.EnviarEmailInformativoRecuperacaoSenha(usuario.EmailSecundario, FormaRecuperacao.Sms,
                        usuario.IdUsuario, usuario.NomeCompleto, usuario.TipoUsuario, usuario.NumeroPV, null);
                }
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                Logger.GravarLog("Erro ao enviar email informativo", ex);
                SharePointUlsLog.LogErro(ex);
            }
            catch (Exception ex)
            {
                Logger.GravarLog("Erro ao enviar email informativo", ex);
                SharePointUlsLog.LogErro(ex);
            }
        }

        /// <summary>
        /// Envia o SMS de recuperação para o Celular do usuário
        /// </summary>
        private String EnviarSMSRecuperacao(String mensagem, String numeroCelular, Int32 codigoIdUsuario)
        {
            return Comum.SMS
                .EnviaSMSRecuperacaoSenha(
                          this.UsuarioSms,
                          this.SenhaSms,
                          codigoIdUsuario,
                          numeroCelular,
                          mensagem);
        }

        /// <summary>
        /// Envia o SMS informativo de recuperação para o Celular do usuário
        /// </summary>
        private String EnviarSMS(String mensagem, String numeroCelular)
        {
            return Comum.SMS
                .EnviaSMS(
                          this.UsuarioSms,
                          this.SenhaSms,
                          numeroCelular,
                          mensagem);
        }

        /// <summary>
        /// Retorna um código aleatório para confirmação do código de Recuperação via SMS
        /// </summary>
        /// <returns></returns>
        private String GerarCodigoSMS()
        {
            String codigoRecuperacao = String.Empty;
            Int16 TAMANHO_CODIGO = 6;

            Int32 seedRandom = DateTime.Now.Day
                               + DateTime.Now.Month
                               + DateTime.Now.Year
                               + DateTime.Now.Hour
                               + DateTime.Now.Minute
                               + DateTime.Now.Second;

            Random rndCodigo = new Random(seedRandom);

            for (int i = 1; i <= TAMANHO_CODIGO; i++)
            {
                codigoRecuperacao = String.Concat(codigoRecuperacao, rndCodigo.Next(0, 9).ToString());
            }

            return codigoRecuperacao;
        }

        /// <summary>
        /// Exibe uma mensagem de confirmação de reenvio do código
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        private void ExibirMensagemReenvioCodigo()
        {
            String mensagem = "Código enviado com sucesso";

            Page.ClientScript.RegisterClientScriptBlock(
                Page.GetType(),
                "jsExibirMensagemReenvio",
                String.Format("$(function() {{ FormaEnvioNotify.Sucesso('<span class=\"bold\">{0}</span>', ' '); }});", mensagem), 
                true);
        }

        /// <summary>
        /// Exibe uma mensagem de aviso
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="voltarPagina">Voltar para o estado anterior da página</param>
        /// 
        private void ExibirAviso(String mensagem, Boolean voltarPagina)
        {
            PainelMensagem.Titulo = "Atenção";
            PainelMensagem.Mensagem = mensagem;
            PainelMensagem.TipoMensagem = PainelMensagemIcon.Aviso;

            PainelMensagem.HideBotoesCustom = !voltarPagina;

            pnlAviso.Visible = true;
            pnlAvisoEmail.Visible = false;
            pnlCodigoRecuperacao.Visible = false;
        }

        /// <summary>
        /// Exibe uma mensagem de erro
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="voltarPaginar">Indica se deve voltar para o estado anterior da página</param>
        /// 
        private void ExibirErro(String mensagem, Boolean voltarPaginar)
        {
            PainelMensagem.Titulo = "Atenção";
            PainelMensagem.Mensagem = mensagem;
            PainelMensagem.TipoMensagem = PainelMensagemIcon.Erro;

            PainelMensagem.HideBotoesCustom = !voltarPaginar;

            pnlAviso.Visible = true;
            pnlAvisoEmail.Visible = false;
            pnlCodigoRecuperacao.Visible = false;
        }

        /// <summary>
        /// Exibe a mensagem de erro com o link de retorno
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <param name="urlVoltar">Url de retorno após o erro</param>
        /// 
        private void ExibirErro(String fonte, Int32 codigo, String urlVoltar)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);

            if (!String.IsNullOrEmpty(urlVoltar))
            {
                PainelMensagem.HideBotoesCustom = false;
                btnVoltar.OnClientClick = String.Format("window.location.href='{0}'; return false;", urlVoltar);
            }

            PainelMensagem.Titulo = "Atenção";
            PainelMensagem.Mensagem = mensagem;
            PainelMensagem.TipoMensagem = PainelMensagemIcon.Erro;

            pnlAviso.Visible = true;
            pnlAvisoEmail.Visible = false;
            pnlCodigoRecuperacao.Visible = false;
        }

        /// <summary>
        /// Exibe a modal de aviso via javascript
        /// </summary>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="mensagem">Conteúdo da mensagem</param>
        /// <param name="tipoModal">Tipo relacionado ao ícone</param>
        private void ExibirModal(String titulo, String mensagem, TipoModal tipoModal)
        {
            String script = String.Format(@"FormaEnvioNotify.ShowCustom('{0}', '{1}', {2});", mensagem, titulo, (Int32)tipoModal);
            Page.ClientScript.RegisterStartupScript(Page.GetType(), "FormaEnvioNotify", script, true);
        }

        #endregion

        #region Seleção PVs
        
        /// <summary>
        /// Obtem os pvs selecionados
        /// </summary>
        /// <returns></returns>
        public string GetPvsSelecionados()
        {
            if (InformacaoUsuario.Existe())
            {
                return _infoUsuario.GetPvsSelecionados();
            }

            return null;
        }

        /// <summary>
        /// Retorna E-mail preenxido na tela
        /// </summary>
        /// <returns></returns>
        public string GetEmail()
        {
            string retorno = _infoUsuario.EmailUsuario;

            if (string.IsNullOrEmpty(retorno))
            {
                return string.Empty;
            }
            else
            {
                return retorno.ToLower();
            }

        }

        #endregion
    }
}