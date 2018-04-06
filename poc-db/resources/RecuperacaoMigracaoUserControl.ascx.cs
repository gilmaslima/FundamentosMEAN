using System;
using System.ServiceModel;
using System.Web;
using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.RecuperacaoMigracao
{
    public partial class RecuperacaoMigracaoUserControl : UserControlBase
    {
        #region [Controles da WebPart]
        
        /// <summary>
        /// qdAviso control.
        /// </summary>
        protected QuadroAviso QdAviso { get { return (QuadroAviso)qdAviso; } }

        #endregion

        #region [Atributos da WebPart]

        /// <summary>
        /// Passos da Webpart
        /// </summary>
        public RecuperacaoMigracao WebPartRecuperacao { get; set; }

        #endregion

        #region [Eventos WebPart]
        /// <summary>
        /// Inicializar a WebPart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Inicializar a WebPart"))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        if (this.WebPartRecuperacao.RecuperacaoUsuario)
                        {
                            btnRecuperarUsuarioSenha.Text = "Recuperar usuário";
                            vlsRecuperacao.HeaderText = "Preencha os campos obrigatórios ou clique em Recuperar usuário";
                            pnlDadosRecuperacaoUsuario.Visible = true;
                        }
                        else
                        {
                            btnRecuperarUsuarioSenha.Text = "Recuperar senha";
                            vlsRecuperacao.HeaderText = "Preencha os campos obrigatórios ou clique em Recuperar senha";
                            pnlDadosRecuperacaoSenha.Visible = true;
                        }
                    }

                    //Migração finalizada - 11/02/2015
                    pnlDadosRecuperacaoUsuario.Visible = false;
                    pnlDadosRecuperacaoSenha.Visible = false;
                    pnlRecuperacao.Visible = false;

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
        /// Redirecionar para a Recuperação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRecuperarUsuarioSenha_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Redirecionar para a Recuperação"))
            {
                try
                {
                    String urlRecuperacao = String.Empty;

                    if (this.WebPartRecuperacao.RecuperacaoUsuario)
                        urlRecuperacao = String.Format("{0}/Paginas/RecuperacaoUsuarioIdentificacao.aspx", base.web.ServerRelativeUrl);
                    else
                        urlRecuperacao = String.Format("{0}/Paginas/RecuperacaoSenhaIdentificacao.aspx", base.web.ServerRelativeUrl);

                    Response.Redirect(urlRecuperacao, false);
                }
                catch (HttpException ex)
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
        /// Validar os dados e redirecionar o usuário para atualizar seus dados
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Redirecionar para a Recuperação"))
            {
                try
                {
                    Boolean valido = false;
                    
                    if (this.WebPartRecuperacao.RecuperacaoUsuario)
                        valido = this.ValidarRecuperacaoUsuario();
                    else
                        valido = this.ValidarRecuperacaoSenha();

                    if (!valido)
                    {
                        InformacaoUsuario.Limpar();
                        this.ExibirAvisoUsuarioInexistente();
                    }
                    else
                        this.RedirecionarCriacaoNovoAcesso();
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", base.RecuperarEnderecoPortal());
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", base.RecuperarEnderecoPortal());
                }
                catch (HttpException ex)
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

        #region [Metódos auxiliares]

        /// <summary>
        /// Validar os dados informados para recuperar usuário legado
        /// </summary>
        /// <returns></returns>
        private Boolean ValidarRecuperacaoUsuario()
        {
            Int32 codigoEntidade = 0;

            if (!Int32.TryParse(txtEstabelecimento.Text, out codigoEntidade))
                return false;

            String senhaUsuario = txtSenha.Text.Trim();
            if (String.IsNullOrEmpty(senhaUsuario))
                return false;
            else
                senhaUsuario = EncriptadorSHA1.EncryptString(senhaUsuario);

            using (Logger log = Logger.IniciarLog("Validar os dados informados para recuperar usuário legado"))
            {
                using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                {
                    UsuarioServico.Entidade entidade = new UsuarioServico.Entidade()
                    {
                        Codigo = codigoEntidade,
                        GrupoEntidade = new UsuarioServico.GrupoEntidade()
                        {
                            Codigo = 1
                        }
                    };

                    Int32 codigoRetorno = 0;
                    var usuarios = contextoUsuario.Cliente.ConsultarPorSenha(out codigoRetorno, senhaUsuario, entidade);

                    if (codigoRetorno > 0 || usuarios.Length == 0)
                        return false;
                    
                    var usuario = usuarios[0];
                    if (!usuario.Legado)
                        return false;

                    this.CriarInformacaoUsuario(usuario);
                    
                    return true;
                }
            }
        }

        /// <summary>
        /// Validar os dados informados para recuperar senha do usuário legado
        /// </summary>
        /// <returns></returns>
        private Boolean ValidarRecuperacaoSenha()
        {
            Int32 codigoEntidade = 0;

            if (!Int32.TryParse(txtEstabelecimento.Text, out codigoEntidade))
                return false;

            String loginUsuario = txtUsuario.Text.Trim();
            if (String.IsNullOrEmpty(loginUsuario))
                return false;

            using (Logger log = Logger.IniciarLog("Validar os dados informados para recuperar senha do usuário legado"))
            {
                using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                {
                    UsuarioServico.Entidade entidade = new UsuarioServico.Entidade()
                    {
                        Codigo = codigoEntidade,
                        GrupoEntidade = new UsuarioServico.GrupoEntidade()
                        {
                            Codigo = 1
                        }
                    };

                    Int32 codigoRetorno = 0;
                    var usuarios = contextoUsuario.Cliente.ConsultarPorCodigoEntidade(out codigoRetorno, loginUsuario, entidade);

                    if (codigoRetorno > 0 || usuarios.Length == 0)
                        return false;

                    var usuario = usuarios[0];
                    if (!usuario.Legado)
                        return false;

                    this.CriarInformacaoUsuario(usuario);

                    return true;
                }
            }
        }

        /// <summary>
        /// Criar na sessão os dados do usuário legado
        /// </summary>
        /// <param name="usuario"></param>
        private void CriarInformacaoUsuario(UsuarioServico.Usuario usuario)
        {
            InformacaoUsuario.Criar(usuario.Entidade.GrupoEntidade.Codigo,
                                            usuario.Entidade.Codigo,
                                            usuario.Codigo);

            InformacaoUsuario infoUsuario = InformacaoUsuario.Recuperar();

            infoUsuario.IdUsuario = usuario.CodigoIdUsuario;
            infoUsuario.Usuario = usuario.Codigo;
            infoUsuario.TipoUsuario = usuario.TipoUsuario;

            InformacaoUsuario.Salvar(infoUsuario);
        }

        /// <summary>
        /// Redirecionar o usuário legado para a criação do novo acesso
        /// </summary>
        private void RedirecionarCriacaoNovoAcesso()
        {
            String urlRecuperacao = String.Empty;
            urlRecuperacao = String.Format("{0}/Paginas/CriacaoUsrDadosIniciaisLegado.aspx", base.web.ServerRelativeUrl);

            Response.Redirect(urlRecuperacao, false);
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
                QdAviso.CarregarMensagem(titulo, mensagem, QuadroAviso.IconeMensagem.Erro);
            else
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAviso.IconeMensagem.Erro);

            pnlAviso.Visible = true;
            pnlRecuperacao.Visible = false;
        }

        /// <summary>
        /// Exibe a mensagem de aviso de que o usuário não existe e deve criar um novo
        /// </summary>
        private void ExibirAvisoUsuarioInexistente()
        {
            String url = new Uri(SPContext.Current.Site.Url).GetLeftPart(UriPartial.Authority);
            url = String.Format("{0}/pt-br/novoacesso/Paginas/CriacaoUsrDadosIniciais.aspx", url);

            String mensagem = @"Dados inválidos. Por favor, verifique se os 
                                dados digitados estão corretos e tente novamente.<br><br>
                                Se você não possui usuário/e-mail cadastrado, <a href='{0}'>clique aqui</a> e cadastre-se.";

            mensagem = String.Format(mensagem, url);

            QdAviso.CarregarMensagem("Atenção!",
                                     mensagem,
                                     base.RecuperarEnderecoPortal(),
                                     QuadroAviso.IconeMensagem.Aviso);

            pnlAviso.Visible = true;
            pnlRecuperacao.Visible = false;
        }

        #endregion

    }
}
