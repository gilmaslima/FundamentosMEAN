/*
© Copyright 2015 Rede S.A.
Autor : William Santos
Empresa : Rede
*/
using System;
using System.ServiceModel;
using System.Web.UI;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using Rede.PN.DadosCadastraisMobile.SharePoint.CONTROLTEMPLATES.DadosCadastraisMobile;
//using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using System.Web;
using System.Linq;
using Rede.PN.DadosCadastraisMobile.SharePoint.Util;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.RecNovaSenhaBasicoMob
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RecNovaSenhaBasicoMobUserControl : UserControlBase
    {
        #region [Controles WebPart]

        /// <summary>
        /// qdAviso control.
        /// </summary>
        protected QuadroAvisosResponsivo QdAviso { get { return (QuadroAvisosResponsivo)qdAviso; } }


        /// <summary>
        /// Webpart de Conclusao Recuperação Senha
        /// </summary>
        public RecNovaSenhaBasicoMob WebPartNovaSenha { get; set; }

        /// <summary>
        /// txtSenha control.
        /// </summary>
        //protected CampoNovoAcesso TxtSenha { get { return (CampoNovoAcesso)txtSenha; } }

        /// <summary>
        /// txtSenhaConfirmacao control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        //protected CampoNovoAcesso TxtSenhaConfirmacao { get { return (CampoNovoAcesso)txtSenhaConfirmacao; } }           

        #endregion

        #region [Eventos da Webpart]

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
                    this.VerificarInformacaoUsuario();

                    if (this.WebPartNovaSenha.ConclusaoBasico)
                    {
                        pnlSenhaBasico.Visible = true;
                        pnlSenhaCompleto.Visible = false;
                    }
                    else
                    {
                        pnlSenhaBasico.Visible = false;
                        pnlSenhaCompleto.Visible = true;
                    }

                    if (!InformacaoUsuario.Existe())
                    {
                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", this.RecuperarEnderecoPortal());
                        return;
                    }
                    else if (!InformacaoUsuario.Recuperar().PodeRecuperarCriarAcesso)
                    {
                        this.ExibirErro("SharePoint.PodeRecuperarAcessoSenha", 1154, "Atenção", this.RecuperarEnderecoPortal());
                        return;
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal());
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal());
                    return;
                }
            }
        }

        /// <summary>
        /// Validar a alteração de senha e continuar o acesso para o Portal de Serviços
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            string serverRelativeUr = base.web.ServerRelativeUrl;
            string url;
            Boolean possuiKomerci;
            String novaSenha;
            Int32[] pvs;

            using (Logger log = Logger.IniciarLog("Validar Senha e continuar o acesso para o Portal de Serviços"))
            {
                try
                {
                    if (hdfSenhaValidar.Value == "true")
                    {

                        if (InformacaoUsuario.Existe())
                        {
                            log.GravarMensagem("Dados existentes no cache");

                            InformacaoUsuario usuario = InformacaoUsuario.Recuperar();

                            if (usuario.PodeRecuperarCriarAcesso)
                            {
                                pvs = new Int32[] {  
                                    usuario.NumeroPV
                                };

                                log.GravarMensagem("Criptografando senha");

                                novaSenha = EncriptadorSHA1.EncryptString(txtSenha.Text);

                                log.GravarMensagem("Verificando se é komerci");

                                possuiKomerci = false;

                                using (var ctxEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                                {
                                    possuiKomerci = ctxEntidade.Cliente.PossuiKomerci(pvs);
                                }

                                log.GravarMensagem(string.Format("Estabelecimentos são komerci {0}", possuiKomerci));

                                using (var ctxUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                                {
                                    log.GravarMensagem("Atualizando senha do usuário");

                                    Int32 codigoRetorno = ctxUsuario.Cliente
                                                            .AtualizarSenhaUsuarioNovoAcesso(
                                                                usuario.IdUsuario,
                                                                novaSenha,
                                                                possuiKomerci,
                                                                usuario.PvsSelecionados,
                                                                true, true); //Senha Provisória

                                    log.GravarMensagem("Código de retorno atualização", new { codigoRetorno });

                                    if (codigoRetorno > 0)
                                    {
                                        log.GravarMensagem("Retorno inesperado");

                                        this.ExibirErro("UsuarioServico.AtualizarSenhaUsuario", codigoRetorno, "Atenção", Request.Url.AbsoluteUri);

                                        return;
                                    }
                                    else
                                    {
                                        log.GravarMensagem("Atualização realizada com sucesso");

                                        //Registra no histórico/log de atividades
                                        Historico.RecuperacaoSenha(usuario.IdUsuario,
                                                                   usuario.NomeCompleto,
                                                                   usuario.EmailUsuario,
                                                                   usuario.TipoUsuario,
                                                                   usuario.NumeroPV,
                                                                   usuario.FormaRecuperacaoSenha.GetDescription());

                                        // Enviar e-mail ao finalizar recuperação de senha
                                        this.EnviarEmailConclusaoRecuperacaoSenha(usuario);

                                        url = String.Format(@"{0}/Paginas/Mobile/RecuperacaoSenhaConclusao.aspx", serverRelativeUr);

                                        try
                                        {
                                            Response.Redirect(url);
                                        }
                                        catch (Exception ex)
                                        {
                                            log.GravarErro(ex);
                                        }

                                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                                        InformacaoUsuario.Limpar();
                                    }
                                }
                            }
                            else
                            {
                                this.ExibirErro("SharePoint.PodeRecuperarAcessoSenha", 1154, "Atenção", this.RecuperarEnderecoPortal());
                                return;
                            }
                        }
                    }

                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", this.RecuperarEnderecoPortal());
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal());
                    return;
                }
            }
        }

        #endregion

        #region [Métodos privados]

        /// <summary>
        /// Verificar se há informações do usuário na URL para criação dos dados na sessão
        /// </summary>
        private void VerificarInformacaoUsuario()
        {
            if (Request.QueryString["dados"] != null)
            {
                //Assegura de limpar os dados de usuário que possam estar na sessão
                InformacaoUsuario.Limpar();

                QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);

                Int32 codigoIdUsuario = queryString["CodigoIdUsuario"].ToInt32();
                Guid hashEmail = new Guid(queryString["Hash"].ToString());
                String pvs = queryString["Pvs"];

                Int32[] pvsSelecionados = pvs.Split('|').Select(x => Convert.ToInt32(x)).ToArray();

                FormaRecuperacao formaRecuperacao = (FormaRecuperacao)queryString["FormaRecuperacao"].ToInt32();

                Int32 codigoRetorno = 0;

                UsuarioServico.Usuario usuario;
                
                using (Logger log = Logger.IniciarLog("Consultando dados do usuário"))
                {
                    using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    {
                        usuario = contextoUsuario.Cliente.ConsultarPorID(out codigoRetorno, codigoIdUsuario);

                        log.GravarMensagem("Usuário consultado com suscesso", new { usuario = usuario });
                        log.GravarMensagem("Código de retorno consulta", new { codigoRetorno });
                    }
                }

                if (codigoRetorno == 0 && usuario != null)
                {
                    InformacaoUsuario.Criar(1, usuario.Entidade.Codigo, usuario.Email);
                    InformacaoUsuario infoUsuario = InformacaoUsuario.Recuperar();
                    infoUsuario.NomeCompleto = usuario.Descricao;
                    infoUsuario.IdUsuario = usuario.CodigoIdUsuario;
                    infoUsuario.TipoUsuario = usuario.TipoUsuario;
                    infoUsuario.PodeRecuperarCriarAcesso = true;
                    infoUsuario.HashEmail = hashEmail;
                    infoUsuario.FormaRecuperacaoSenha = formaRecuperacao;
                    infoUsuario.PvsSelecionados = pvsSelecionados;
                    
                    using (Logger log = Logger.IniciarLog("Gravar dados cache"))
                    {
                        log.GravarMensagem("Gravando dados no cache", new { infoUsuario });
                    }

                    InformacaoUsuario.Salvar(infoUsuario);
                }
            }
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
            pnlNovaSenha.Visible = false;
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
            pnlNovaSenha.Visible = false;
        }

        /// <summary>
        /// Autentica o usuário no Portal
        /// </summary>
        /// <param name="numPdv">Número do Estabelecimento</param>
        /// <param name="emailUsuario">E-mail/login do Usuário</param>
        /// <param name="senha">Senha Criptografadas</param>
        /// <param name="liberarAcesso">
        /// <para>S - Indica que deve direcionar para a página de Liberação de Acesso</para>
        /// <para>N - Não direcionará para a Liberação de acesso completo</para>
        /// </param>
        private void LoginUsuario(String numPdv, String emailUsuario, String senha, String liberarAcesso)
        {
            var queryString = new QueryStringSegura();
            queryString["estabelecimento"] = "1";
            queryString["ncadastro"] = numPdv;
            queryString["usuario"] = emailUsuario;
            queryString["senha"] = senha;
            queryString["liberarAcesso"] = liberarAcesso;
            queryString["indSenhaCript"] = "S";

            String url = String.Format("/_layouts/DadosCadastrais/Login.aspx?dados={0}", queryString.ToString());
            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Enviar e-mail mediante conclusão do processo de recuperação de senha do usuário
        /// </summary>
        /// <param name="usuario"></param>
        private void EnviarEmailConclusaoRecuperacaoSenha(InformacaoUsuario usuario)
        {
            // consulta os e-mails do GE
            using (Logger log = Logger.IniciarLog("Método EnviarEmailConclusaoRecuperacaoSenha()"))
            {
                using (var entidadeServico = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    int codigoRetorno = 0;
                    var dicEmailPvs = entidadeServico.Cliente.ConsultarEmailPVs(out codigoRetorno, usuario.PvsSelecionados);
                    foreach (var emailPv in dicEmailPvs)
                    {
                        if (string.IsNullOrEmpty(emailPv.Value))
                        {
                            continue;
                        }

                        try
                        {
                            EmailNovoAcesso.EnviarEmailRecuperacaoSenhaConclusao(
                                emailPv.Value,
                                usuario.IdUsuario,
                                usuario.EmailUsuario,
                                usuario.NomeCompleto,
                                usuario.TipoUsuario,
                                usuario.NumeroPV,
                                string.Empty,
                                usuario.PvsSelecionados);
                        }
                        catch (NullReferenceException ex)
                        {
                            log.GravarMensagem("Problemas para enviar e-mail.", new { NumeroPV = usuario.NumeroPV, Email = emailPv.Value });
                            log.GravarErro(ex);
                            return;
                        }
                        catch (Exception ex)
                        {
                            log.GravarMensagem("Problemas para enviar e-mail.", new { NumeroPV = usuario.NumeroPV, Email = emailPv.Value });
                            log.GravarErro(ex);
                            return;
                        }
                    }
                }
            }
        }

        #endregion
    }
}

