/*
© Copyright 2015 Rede S.A.
Autor : Felipe Siatiquosque
Empresa : Rede
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Rede.PN.DadosCadastraisMobile.SharePoint.CONTROLTEMPLATES.DadosCadastraisMobile;
using System.ServiceModel;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.ConfirmacaoPositivaRecuperacaoMob
{
    public partial class ConfirmacaoPositivaRecuperacaoMobUserControl : UserControlBase
    {
        #region [Atributos da WebPart]

        /// <summary>
        /// Passos da Webpart
        /// </summary>
        public ConfirmacaoPositivaRecuperacaoMob WebPartRecuperacao { get; set; }

        #endregion

        #region [Controles WebPart]

        /// <summary>
        /// qdAvisoConfirmacaoPositiva control.
        /// </summary>
        protected QuadroAvisosResponsivo QdAvisoConfirmacaoPositiva { get { return (QuadroAvisosResponsivo)qdAvisoConfirmacaoPositiva; } }

        /// <summary>
        /// cpRecuperacaoAcesso control.
        /// </summary>
        protected ConfirmacaoPositivaResponsivo CpRecuperacaoAcesso { get { return (ConfirmacaoPositivaResponsivo)cpRecuperacaoAcesso; } }

        #endregion

        #region [Eventos da webpart]
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
                        if (!this.WebPartRecuperacao.RecuperacaoUsuario)
                        {
                            lblTitulo.Text = "Senha";
                            passosSenha.Visible = true;
                            passosUsuario.Visible = false;
                        }

                        this.VerificarInformacaoUsuario();

                        if (InformacaoUsuario.Existe())
                        {
                            InformacaoUsuario usuario = InformacaoUsuario.Recuperar();

                            if (usuario.TipoUsuario.Equals("P") || usuario.TipoUsuario.Equals("M"))
                                CpRecuperacaoAcesso.ConfirmacaoCompleta = true;
                            else
                                CpRecuperacaoAcesso.ConfirmacaoCompleta = false;

                            CpRecuperacaoAcesso.RecuperacaoUsuario = this.WebPartRecuperacao.RecuperacaoUsuario;
                        }
                        else
                        {
                            this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", base.RecuperarEnderecoPortal());
                        }
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", base.RecuperarEnderecoPortal());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                }
            }
        }

        /// <summary>
        /// Continuar para o próximo passo da Recuperação de Usuário/Senha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ContinuarRecuperacao(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Continuar para o próximo passo da Recuperação de Usuário/Senha"))
            {
                try
                {
                    if (InformacaoUsuario.Existe())
                    {
                        String urlConclusao = String.Empty;

                        InformacaoUsuario usuario = InformacaoUsuario.Recuperar();

                        if (this.WebPartRecuperacao.RecuperacaoUsuario)
                            urlConclusao = String.Format("{0}/Paginas/Mobile/RecuperacaoUsuarioConclusao.aspx", base.web.ServerRelativeUrl);
                        else
                        {
                            if (usuario.SenhaExpirada)
                                urlConclusao = String.Format("{0}/Paginas/Mobile/RecNovaSenhaExpirada.aspx", base.web.ServerRelativeUrl);
                            else
                                urlConclusao = String.Format("{0}/Paginas/Mobile/RecuperacaoNovaSenha.aspx", base.web.ServerRelativeUrl);
                        }

                        usuario.PodeRecuperarCriarAcesso = true;

                        Response.Redirect(urlConclusao, false);
                    }
                    else
                    {
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

        /// <summary>
        /// Voltar para o passo anterior. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void btnVoltar_Click(object sender, EventArgs e)
        //{
        //    //TODO: Verificar a volta
        //}
        #endregion

        #region [Métodos Privados]

        /// <summary>
        /// Verificar se há informações do usuário na URL para criação dos dados na sessão
        /// </summary>
        private void VerificarInformacaoUsuario()
        {
            UsuarioServico.Usuario usuario;

            Int32 codigoRetorno = 0;

            if (Request.QueryString["dados"] != null)
            {
                //Assegura de limpar os dados de usuário que possam estar na sessão
                InformacaoUsuario.Limpar();

                QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);

                Int32 codigoIdUsuario = queryString["CodigoIdUsuario"].ToInt32();
                Guid hashEmail = new Guid(queryString["Hash"].ToString());
                FormaRecuperacao formaRecuperacao = (FormaRecuperacao)queryString["FormaRecuperacao"].ToInt32();

                using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    usuario = contextoUsuario.Cliente.ConsultarPorID(out codigoRetorno, codigoIdUsuario);

                if (codigoRetorno == 0 && usuario != null)
                {
                    InformacaoUsuario.Criar(1, usuario.Entidade.Codigo, usuario.Email);
                    InformacaoUsuario infoUsuario = InformacaoUsuario.Recuperar();
                    infoUsuario.NomeCompleto = usuario.Descricao;
                    infoUsuario.IdUsuario = usuario.CodigoIdUsuario;
                    infoUsuario.TipoUsuario = usuario.TipoUsuario;
                    infoUsuario.HashEmail = hashEmail;
                    infoUsuario.FormaRecuperacaoSenha = formaRecuperacao;

                    InformacaoUsuario.Salvar(infoUsuario);
                }
            }
            else if (InformacaoUsuario.Existe()) //Trata-se do redirecionamento por expiração de senha
            {
                if (InformacaoUsuario.Recuperar().IdUsuario == 0)
                {


                    using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        usuario = contextoUsuario.
                                            Cliente.
                                            ConsultarDadosUsuario(out codigoRetorno,
                                                                InformacaoUsuario.Recuperar().GrupoEntidade,
                                                                InformacaoUsuario.Recuperar().NumeroPV,
                                                                InformacaoUsuario.Recuperar().EmailUsuario);

                    if (codigoRetorno == 0 && usuario != null)
                    {
                        InformacaoUsuario.Criar(1,
                                                InformacaoUsuario.Recuperar().NumeroPV,
                                                InformacaoUsuario.Recuperar().EmailUsuario);

                        InformacaoUsuario infoUsuario = InformacaoUsuario.Recuperar();
                        infoUsuario.NomeCompleto = usuario.Descricao;
                        infoUsuario.IdUsuario = usuario.CodigoIdUsuario;
                        infoUsuario.TipoUsuario = "M"; //Deve realizar a confirmação positiva completa
                        infoUsuario.HashEmail = Guid.Empty;
                        infoUsuario.FormaRecuperacaoSenha = FormaRecuperacao.EmailPrincipal;
                        infoUsuario.SenhaExpirada = true;

                        InformacaoUsuario.Salvar(infoUsuario);
                    }
                }
            }
        }

        /// <summary>
        /// Exibe uma mensagem de aviso
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        //private void ExibirAviso(String mensagem, String titulo, String urlVoltar)
        //{
        //    if (String.IsNullOrEmpty(urlVoltar))
        //        QdAvisoConfirmacaoPositiva.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Aviso);
        //    else
        //        QdAvisoConfirmacaoPositiva.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Aviso);

        //    pnlAvisoConfirmacaoPositiva.Visible = true;
        //    pnlConfirmacaoPositiva.Visible = false;
        //}

        /// <summary>
        /// Exibe uma mensagem de erro
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        //private void ExibirErro(String mensagem, String titulo, String urlVoltar)
        //{
        //    if (String.IsNullOrEmpty(urlVoltar))
        //        QdAvisoConfirmacaoPositiva.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Erro);
        //    else
        //        QdAvisoConfirmacaoPositiva.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Erro);

        //    pnlAvisoConfirmacaoPositiva.Visible = true;
        //    pnlConfirmacaoPositiva.Visible = false;
        //}

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
                QdAvisoConfirmacaoPositiva.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Erro);
            else
                QdAvisoConfirmacaoPositiva.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Erro);

            pnlAvisoConfirmacaoPositiva.Visible = true;
            pnlConfirmacaoPositiva.Visible = false;
        }

        #endregion
    }
}
