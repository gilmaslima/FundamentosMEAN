/*
© Copyright 2015 Rede S.A.
Autor : Felipe Siatiquosque
Empresa : Rede
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Rede.PN.DadosCadastraisMobile.SharePoint.CONTROLTEMPLATES.DadosCadastraisMobile;
using Rede.PN.DadosCadastraisMobile.SharePoint.Util;
using System.Web;
using System.Net.Mail;
using Redecard.PN.Comum;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.ConclusaoCriacaoMob
{
    public partial class ConclusaoCriacaoMobUserControl : UserControlBase
    {
        #region [Controles WebPart]
        /// <summary>
        /// qdErro control.
        /// </summary>
        protected QuadroAvisosResponsivo QdErro { get { return (QuadroAvisosResponsivo)qdErro; } }

        /// <summary>
        /// qdAvisoConclusao control.
        /// </summary>
        protected QuadroAvisosResponsivo QdAvisoConclusao { get { return (QuadroAvisosResponsivo)qdAvisoConclusao; } }

        #endregion

        #region [Eventos da página]
        /// <summary>
        /// Inicialização da webpart de conclusão da cadastro de usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Inicialização da webpart de conclusão da cadastro de usuário"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        qdErro.Visible = false;

                        //Preenche literal com o e-mail do remetente padrão dos e-mails novo acesso
                        ltrRemetente.Text = EmailNovoAcesso.Remetente;
                        if (InformacaoUsuario.Existe())
                        {
                            InformacaoUsuario info = InformacaoUsuario.Recuperar();

                            if (info.EntidadePossuiMaster && !info.CriacaoAcessoLegado)
                            {
                                pnlAvisoEmail.Visible = false;
                                //btnPaginaPrincipal.Visible = false;

                                pnlAvisoConclusao.Visible = true;
                                QdAvisoConclusao.CarregarMensagem("Seu cadastro foi finalizado com sucesso.",
                                                                  @"Sua solicitação foi enviada com sucesso para os usuários Master do seu estabelecimento.<br />
                                                                    Assim que um Master aprovar sua solicitação, você receberá um e-mail com as informações para a conclusão de seu cadastro.",
                                                                  this.RecuperarEnderecoPortal(),
                                                                  QuadroAvisosResponsivo.IconeMensagem.Confirmacao);
                            }
                            else
                            {
                                pnlAvisoEmail.Visible = true;
                                pnlAvisoConclusao.Visible = true;
                                /* A pedido da Giovanna, no responsivo não aparece parar ir pra página principal
                                 * pnlIrPaginaPrincipal.Visible = true;
                                 */ 

                                QdAvisoConclusao.CarregarMensagem("Seu cadastro foi finalizado com sucesso.",
                                                                  @"Dentro de instantes você receberá um e-mail de confirmação.<br />
                                                                    Acesse o link informado no e-mail em até 12h para concluir seu cadastro.",
                                                                  QuadroAvisosResponsivo.IconeMensagem.Confirmacao);
                            }
                            InformacaoUsuario.Limpar();
                        }
                        else
                        {
                            pnlAvisoEmail.Visible = false;
                            //QdAvisoConclusao.Visible = false;
                            pnlAvisoConclusao.Visible = false;
                            this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", base.RecuperarEnderecoPortal());
                        }
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
                    qdErro.Visible = false;

                    if (InformacaoUsuario.Existe())
                    {
                        InformacaoUsuario info = InformacaoUsuario.Recuperar();
                        EmailNovoAcesso.EnviarEmailConfirmacaoCadastro48h(info.EmailUsuario, info.IdUsuario,
                            info.HashEmail, info.IdUsuario, info.EmailUsuario, info.NomeCompleto,
                            info.TipoUsuario, info.NumeroPV, null);
                    }
                    else
                    {
                        pnlAvisoEmail.Visible = false;
                        //qdAvisoConclusao.Visible = false;
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

        /// <summary>
        /// Redirecionar o usuário para a página inicial do Portal  Institucional
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnPaginaPrincipal_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Redirecionar o usuário para a página inicial do Portal  Institucional"))
            {
                try
                {
                    InformacaoUsuario.Limpar();
                    Response.Redirect(base.RecuperarEnderecoPortal(), false);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    //this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    //this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
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

            if (!String.IsNullOrEmpty(urlVoltar))
                QdErro.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Erro);
            else
                QdErro.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Erro);

            qdErro.Visible = true;
        }
        #endregion
    }
}
