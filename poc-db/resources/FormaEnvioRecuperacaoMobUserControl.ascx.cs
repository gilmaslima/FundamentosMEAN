/*
© Copyright 2015 Rede S.A.
Autor : William Santos
Empresa : Rede
*/
using System;
using System.Configuration;
using System.ServiceModel;
using System.Net.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Rede.PN.DadosCadastraisMobile.SharePoint.CONTROLTEMPLATES.DadosCadastraisMobile;
using Rede.PN.DadosCadastraisMobile.SharePoint.Util;
using System.Linq;
using System.Collections.Generic;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.FormaEnvioRecuperacaoMob
{
    public partial class FormaEnvioRecuperacaoMobUserControl : UserControlBase
    {
        #region [ Controles WebPart ]

        /// <summary>
        /// qdAviso control.
        /// </summary>
        protected QuadroAvisosResponsivo QdAviso { get { return (QuadroAvisosResponsivo)qdAviso; } }

        #endregion

        #region [Propriedades da Página]
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

        /// <summary>
        /// Verifica os pvs selecinados;
        /// </summary>
        /// <param name="numPdv"></param>
        /// <returns></returns>
        public Boolean VerificarPvSelecionado(int numPdv)
        {
            if (InformacaoUsuario.Existe())
            {
                InformacaoUsuario infoUsuario = InformacaoUsuario.Recuperar();
                return infoUsuario.PvsSelecionados.Contains(numPdv);
            }
            return false;
        }


        #region [ Eventos da Página ]
        /// <summary>
        /// Inicialização da WebPart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // define o remetente da mensagem
            lblEmailRemetente.Text = EmailNovoAcesso.Remetente;

            HashSet<string> emailDistinct;
            HashSet<int?> celularDistinct;

            using (Logger log = Logger.IniciarLog("Inicialização da WebPart"))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        if (InformacaoUsuario.Existe())
                        {
                            log.GravarMensagem("Dados existentes no cache");

                            pnlAviso.Visible = false;
                            pnlEnvioCodigo.Visible = true;
                            pnlAvisoEmail.Visible = false;
                            pnlCodigoRecuperacao.Visible = false;

                            InformacaoUsuario infoUsuario = InformacaoUsuario.Recuperar();

                            EntidadeServico.EntidadeServicoModel[] arrayPvs = infoUsuario.EstabelecimentosRelacinados;

                            // verifica se algum PV foi selecionado
                            if (infoUsuario.PvsSelecionados != null && infoUsuario.PvsSelecionados.Count() > 1)
                            {
                                //HashSet irá realizar um distinct na nos emails
                                emailDistinct = new HashSet<string>(arrayPvs.Select(x => x.EmailSecundario));
                                celularDistinct = new HashSet<int?>(arrayPvs.Select(x => x.Celular));

                                spanOpEmailSecundario.Visible = true;
                                spanOpSMS.Visible = true;

                                //Caso alguem não tenha email secundario não mosta a opção
                                if (arrayPvs.Any(x => string.IsNullOrEmpty(x.EmailSecundario)))
                                {
                                    spanOpEmailSecundario.Visible = false;
                                }

                                //Caso alguem possua um email secundario diferente  não mostra a opção
                                if (emailDistinct != null && emailDistinct.Count > 1)
                                {
                                    spanOpEmailSecundario.Visible = false;
                                }

                                //Caso alguem não possuam celular não mostra a opção
                                if (arrayPvs.Any(x => x.Celular == null || x.Celular == 0))
                                {
                                    spanOpSMS.Visible = false;
                                }

                                //Caso alguem possua um celelar diferente  não mostra a opção
                                if (celularDistinct != null && celularDistinct.Count > 1)
                                {
                                    spanOpSMS.Visible = false;
                                }

                            }
                            //Caso seja apenas um PV selecionado abilita o check caso as informações não sejam nulas
                            else
                            {
                                log.GravarMensagem("Apenas um pv selecionado");

                                var selecionado = arrayPvs.FirstOrDefault(x => infoUsuario.PvsSelecionados.Contains(x.NumeroPV));

                                if (selecionado != null)
                                {
                                    spanOpEmailSecundario.Visible = !String.IsNullOrEmpty(selecionado.EmailSecundario);

                                    //TODO: AAL - Quando o envio de SMS estiver disponível nos ambientes, deve voltar a verificação de Celular
                                    spanOpSMS.Visible = (selecionado.DDDCelular > 0) && (selecionado.Celular > 0);
                                }
                            }

                            log.GravarMensagem("Opções que estão sendo exibidas", new { OpcaoSmsVisible = spanOpSMS.Visible, OpcaoEmailSecundarioVisible = spanOpEmailSecundario.Visible });

                            //Caso apenas a opção Email Pricipal esteja visivel proceguir para o passo 3
                            if (rbEmailPrincipal.Visible == true && spanOpEmailSecundario.Visible == false && spanOpSMS.Visible == false)
                            {
                                this.EnviarClick();
                                return;
                            }

                        }
                        else
                        {
                            this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", base.RecuperarEnderecoPortal());
                            return;
                        }
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                    return;
                }
            }
        }

        private void EnviarClick()
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

                        if (rbEmailPrincipal.Checked || rbEmailSecundario.Checked)
                        {
                            log.GravarMensagem("Atualisando o status do usuário e criando hash de email");

                            using (var ctxUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                            {
                                hashEmail = ctxUsuario.Cliente.AtualizarStatusParaAguardandoConfirRecSenha(usuario.IdUsuario,
                                                                                                           usuario.EmailUsuario,
                                                                                                           0.5,
                                                                                                           DateTime.Now,
                                                                                                           usuario.PvsSelecionados);
                                log.GravarMensagem("Hash de email criada", new { hashEmail });

                            }
                        }

                        if (codigoRetorno == 0)
                        {
                            log.GravarMensagem("Atualisação realisada com sucesso");

                            usuario.HashEmail = hashEmail;
                            InformacaoUsuario.Salvar(usuario);

                            if (rbEmailPrincipal.Checked)
                            {
                                log.GravarMensagem("Enviando email para email principal");

                                usuario.FormaRecuperacaoSenha = FormaRecuperacao.EmailPrincipal;
                                this.EnviarParaEmailPrincipal(usuario);
                            }
                            else if (rbEmailSecundario.Checked)
                            {
                                log.GravarMensagem("Enviando email para email secundario");

                                usuario.FormaRecuperacaoSenha = FormaRecuperacao.EmailSecundario;
                                this.EnviarParaEmailSecundario(usuario);
                            }
                            else if (rbSMS.Checked)
                            {
                                log.GravarMensagem("Enviando SMS");

                                usuario.FormaRecuperacaoSenha = FormaRecuperacao.Sms;
                                this.EnviarSMS(usuario);
                            }


                            log.GravarMensagem("Processo realizado com sucesso");

                            InformacaoUsuario.Salvar(usuario);
                        }
                        else
                        {
                            log.GravarMensagem("Retorno inesperado", new { codigoRetorno });

                            this.ExibirErro("UsuarioServico.AtualizarStatus", 1151, "Atenção", Request.Url.AbsoluteUri);
                            return;
                        }
                    }
                    else
                    {
                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", base.RecuperarEnderecoPortal());
                        return;
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", base.RecuperarEnderecoPortal());
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                    return;
                }
            }
        }

        /// <summary>
        /// Enviar o e-mail/SMS de recuperação de senha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            EnviarClick();
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
                        InformacaoUsuario info = InformacaoUsuario.Recuperar();
                        EmailNovoAcesso.EnviarEmailRecuperacaoSenha(info.EmailUsuario,
                            info.IdUsuario, info.HashEmail, info.FormaRecuperacaoSenha,
                            info.IdUsuario, info.EmailUsuario, info.NomeCompleto, info.TipoUsuario, info.NumeroPV, null);
                    }
                    else
                    {
                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", base.RecuperarEnderecoPortal());
                        return;
                    }
                }
                catch (SmtpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                    return;
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

                        if (!pnlAviso.Visible)
                        {
                            this.ExibirMensagemReenvio("", "Código enviado com sucesso!");
                            return;
                        }
                    }
                    else
                    {
                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", base.RecuperarEnderecoPortal());
                        return;
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", Request.Url.AbsoluteUri);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", Request.Url.AbsoluteUri);
                    return;
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
                    if (InformacaoUsuario.Existe())
                    {
                        InformacaoUsuario infoUsuario = InformacaoUsuario.Recuperar();

                        String codigo = pnlInserirCodigoRecuperacao.Visible ? txtCodigo.Text : txtCodigoNovamente.Text;
                        if (infoUsuario.CodigoRecuperacaoSMS.Equals(codigo))
                        {
                            String urlRedirecionar = String.Empty;
                            infoUsuario.PodeRecuperarCriarAcesso = true;
                            InformacaoUsuario.Salvar(infoUsuario);

                            urlRedirecionar = String.Format("{0}/Paginas/Mobile/RecNovaSenhaBasico.aspx", base.web.ServerRelativeUrl);

                            Response.Redirect(urlRedirecionar, false);
                        }
                        else
                        {
                            txtCodigoNovamente.Text = String.Empty;
                            txtCodigo.Text = String.Empty;

                            pnlInserirCodigoRecuperacao.Visible = false;
                            pnlInserirNovamenteCodigoRecuperacao.Visible = true;
                        }
                    }
                    else
                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", base.RecuperarEnderecoPortal());
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", Request.Url.AbsoluteUri);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", Request.Url.AbsoluteUri);
                    return;
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
            int numeroPv = 
                usuario.PvsSelecionados != null && usuario.PvsSelecionados.Length > 0 ?
                usuario.PvsSelecionados.FirstOrDefault() : 
                usuario.NumeroPV;

            using (Logger log = Logger.IniciarLog("Envio de emails para usuários masters"))
            {
                log.GravarMensagem("Enviando email recuperação de senha para email principal");

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
                                                            usuario.ObterPvsSelecionadosToString());

                log.GravarMensagem("Email enviado com sucesso");

                if (!String.IsNullOrEmpty(usuario.EmailSecundario))
                {
                    log.GravarMensagem("Enviando emial informativo para o e-mail secundario");

                    EmailNovoAcesso.EnviarEmailInformativoRecuperacaoSenha(usuario.EmailSecundario, FormaRecuperacao.EmailPrincipal,
                        usuario.IdUsuario, usuario.NomeCompleto, usuario.TipoUsuario, numeroPv, null, usuario.ObterPvsSelecionadosToString());

                    log.GravarMensagem("Email secundario enviado com sucesso");
                }
                else if (usuario.DddCelularUsuario > 0 && usuario.CelularUsuario > 0)
                {
                    log.GravarMensagem("Enviando SMS informativo");

                    String numeroCelular = String.Concat(usuario.DddCelularUsuario.ToString(), usuario.CelularUsuario.ToString());
                    String mensagem = "Portal Rede: Voce solicitou uma nova senha. Caso nao tenha sido voce, ligue para 4001-4433 ou 0800 784433";

                    try
                    {
                        this.EnviarSMS(mensagem, numeroCelular);
                        log.GravarMensagem("SMS enviado com sucesso");
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

                lblNomeEmail.Text = usuario.EmailUsuario;
                pnlAvisoEmail.Visible = true;
                
                pnlAviso.Visible = false;
                pnlEnvioCodigo.Visible = false;
                pnlCodigoRecuperacao.Visible = false;
            }
        }

        /// <summary>
        /// Envia o email de recuperação para o E-mail secundário
        /// </summary>
        private void EnviarParaEmailSecundario(InformacaoUsuario usuario)
        {
            int numeroPv =
                usuario.PvsSelecionados != null && usuario.PvsSelecionados.Length > 0 ?
                usuario.PvsSelecionados.FirstOrDefault() :
                usuario.NumeroPV;

            EmailNovoAcesso.EnviarEmailRecuperacaoSenha(usuario.EmailSecundario, usuario.IdUsuario, usuario.HashEmail,
                usuario.FormaRecuperacaoSenha, usuario.IdUsuario, usuario.EmailUsuario, usuario.NomeCompleto,
                usuario.TipoUsuario, numeroPv, null, usuario.ObterPvsSelecionadosToString());

            if (usuario.DddCelularUsuario > 0 && usuario.CelularUsuario > 0)
            {
                String numeroCelular = String.Concat(usuario.DddCelularUsuario.ToString(), usuario.CelularUsuario.ToString());
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
                EmailNovoAcesso.EnviarEmailInformativoRecuperacaoSenha(usuario.EmailUsuario, FormaRecuperacao.EmailSecundario,
                    usuario.IdUsuario, usuario.NomeCompleto, usuario.TipoUsuario, numeroPv, null, usuario.ObterPvsSelecionadosToString());
            }

            pnlAvisoEmail.Visible = true;
            //QdAvisoConclusao.CarregarMensagem();

            pnlAviso.Visible = false;
            pnlEnvioCodigo.Visible = false;
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
            String codigo = this.GerarCodigoSMS();

            String mensagem = String.Format(
                            "Portal Rede: Seu codigo de recuperacao de senha e {0}.",
                            codigo);

            String retornoEnvio = this.EnviarSMSRecuperacao(mensagem, numeroCelular, usuario.IdUsuario);
            Logger.GravarLog("Retorno envio SMS", new { retornoEnvio });

            if (retornoEnvio == "OK")
            {
                usuario.CodigoRecuperacaoSMS = codigo;
                InformacaoUsuario.Salvar(usuario);

                //lnkOutraFormaRecuperacao.NavigateUrl = Request.Url.AbsoluteUri;
                pnlCodigoRecuperacao.Visible = true;
                pnlInserirCodigoRecuperacao.Visible = true;

                pnlEnvioCodigo.Visible = false;
                pnlAvisoEmail.Visible = false;
                pnlAviso.Visible = false;
            }
            else if (retornoEnvio == "NOK2")
            {
                this.ExibirAviso("Sua última solicitação foi feita há menos de 20 minutos. Por favor, aguarde.", "Atenção", true);
                return;
            }
            else //Algum erro
            {
                this.ExibirErro("Ocorreu um erro ao enviar o SMS. Tente novamente.", "Atenção", true);
                return;
            }

            if (!String.IsNullOrEmpty(usuario.EmailSecundario))
            {
                EmailNovoAcesso.EnviarEmailInformativoRecuperacaoSenha(usuario.EmailSecundario, FormaRecuperacao.Sms,
                    usuario.IdUsuario, usuario.NomeCompleto, usuario.TipoUsuario, usuario.NumeroPV, null);
            }
            else if (!String.IsNullOrEmpty(usuario.EmailUsuario))
            {
                EmailNovoAcesso.EnviarEmailInformativoRecuperacaoSenha(usuario.EmailUsuario, FormaRecuperacao.Sms,
                    usuario.IdUsuario, usuario.NomeCompleto, usuario.TipoUsuario, usuario.NumeroPV, null);
            }
        }

        /// <summary>
        /// Envia o SMS de recuperação para o Celular do usuário
        /// </summary>
        private String EnviarSMSRecuperacao(String mensagem, String numeroCelular, Int32 codigoIdUsuario)
        {
            return Redecard.PN.Comum.SMS
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
            return Redecard.PN.Comum.SMS
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
        /// Exibe uma mensagem de aviso
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        private void ExibirAviso(String mensagem, String titulo, String urlVoltar)
        {
            if (String.IsNullOrEmpty(urlVoltar))
                QdAviso.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Aviso);
            else
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Aviso);

            pnlAviso.Visible = true;
            pnlEnvioCodigo.Visible = false;
            pnlAvisoEmail.Visible = false;
            pnlCodigoRecuperacao.Visible = false;
        }

        /// <summary>
        /// Exibe uma mensagem de confirmação de reenvio do código
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        private void ExibirMensagemReenvio(String mensagem, String titulo)
        {
            QdAviso.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Confirmacao);
            //pnlReenvioSucesso.Visible = true;

            pnlAviso.Visible = true;

            pnlEnvioCodigo.Visible = false;
            pnlAvisoEmail.Visible = false;
            pnlCodigoRecuperacao.Visible = false;
        }

        /// <summary>
        /// Exibe uma mensagem de aviso
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="voltarPagina">Voltar para o estado anterior da página</param>
        private void ExibirAviso(String mensagem, String titulo, Boolean voltarPagina)
        {
            QdAviso.CarregarMensagem(titulo, mensagem, voltarPagina, QuadroAvisosResponsivo.IconeMensagem.Aviso);

            pnlAviso.Visible = true;
            pnlEnvioCodigo.Visible = false;
            pnlAvisoEmail.Visible = false;
            pnlCodigoRecuperacao.Visible = false;
        }

        /// <summary>
        /// Exibe uma mensagem de erro
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        private void ExibirErro(String mensagem, String titulo, String urlVoltar)
        {
            if (String.IsNullOrEmpty(urlVoltar))
                QdAviso.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Erro);
            else
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Erro);

            pnlAviso.Visible = true;
            pnlEnvioCodigo.Visible = false;
            pnlAvisoEmail.Visible = false;
            pnlCodigoRecuperacao.Visible = false;
        }

        /// <summary>
        /// Exibe uma mensagem de erro
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="voltarPaginar">Indica se deve voltar para o estado anterior da página</param>
        private void ExibirErro(String mensagem, String titulo, Boolean voltarPaginar)
        {
            QdAviso.CarregarMensagem(titulo, mensagem, voltarPaginar, QuadroAvisosResponsivo.IconeMensagem.Erro);

            pnlAviso.Visible = true;
            pnlEnvioCodigo.Visible = false;
            pnlAvisoEmail.Visible = false;
            pnlCodigoRecuperacao.Visible = false;
        }

        /// <summary>
        /// Exibe a mensagem de erro com o link de retorno
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <param name="titulo">Título para o quadro de aviso</param>
        /// <param name="urlVoltar">Url de retorno após o erro</param>
        private void ExibirErro(String fonte, Int32 codigo, String titulo, String urlVoltar)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);

            if (String.IsNullOrEmpty(urlVoltar))
                QdAviso.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Erro);
            else
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Erro);

            pnlAviso.Visible = true;
            pnlEnvioCodigo.Visible = false;
            pnlAvisoEmail.Visible = false;
            pnlCodigoRecuperacao.Visible = false;
        }

        #endregion
    }
}
