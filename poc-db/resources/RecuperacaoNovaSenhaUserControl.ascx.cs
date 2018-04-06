using System;
using System.ServiceModel;
using System.Web.UI;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using System.Web;
using System.Linq;
using Redecard.PNCadastrais.Core.Web.Controles.Portal;
using Microsoft.SharePoint;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.RecuperacaoNovaSenha
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RecuperacaoNovaSenhaUserControl : UserControlBase
    {
        public RecuperacaoNovaSenha WebPartRecuperacaoNovaSenha { get; set; }

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
                    log.GravarMensagem("Page_Load RecuperacaoNovaSenhaUserControl");

                    this.VerificarInformacaoUsuario();

                    if (!InformacaoUsuario.Existe())
                    {
                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", SPContext.Current.Site.Url);
                        log.GravarMensagem("Não foi possivel encontrar os dados no Cache. InformacaoUsuario.Existe()", new { InformacaoUsuarioExiste = InformacaoUsuario.Existe() });

                        return;
                    }

                    var usuario = InformacaoUsuario.Recuperar();
                    if (!usuario.PodeRecuperarCriarAcesso)
                    {
                        this.ExibirErro("SharePoint.PodeRecuperarAcessoSenha", 1154, "Atenção", SPContext.Current.Site.Url);
                        log.GravarMensagem(string.Format("Usuário não possui acesso. PodeRecuperarCriarAcesso = {0}.", InformacaoUsuario.Recuperar().PodeRecuperarCriarAcesso), new { PodeRecuperarCriarAcesso = InformacaoUsuario.Recuperar().PodeRecuperarCriarAcesso });

                        return;
                    }

                    if (usuario.PvsSelecionados != null && usuario.PvsSelecionados.Length > 0)
                    {
                        using (var ctxEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        {
                            // define a exibição do conteúdo descritivo do campo de senha
                            phSenhaAtencao.Visible = ctxEntidade.Cliente.PossuiKomerci(usuario.PvsSelecionados);
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", SPContext.Current.Site.Url);

                    return;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", SPContext.Current.Site.Url);

                    return;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", SPContext.Current.Site.Url);

                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", SPContext.Current.Site.Url);

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
            using (Logger log = Logger.IniciarLog("Registrar historico e continuar o acesso para o Portal de Serviços - btnContinuar_Click"))
            {
                try
                {
                    if (Page.IsValid)
                    {
                        InformacaoUsuario usuario = InformacaoUsuario.Recuperar();

                        if (usuario.PvsSelecionados != null)
                        {
                            HistoricoRecuperacaoSenha(usuario.IdUsuario,
                                                       usuario.NomeCompleto,
                                                       usuario.EmailUsuario,
                                                       usuario.TipoUsuario,
                                                       usuario.PvsSelecionados,
                                                       usuario.FormaRecuperacaoSenha.GetDescription());
                        }

                        // Enviar e-mail ao finalizar recuperação de senha
                        this.EnviarEmailConclusaoRecuperacaoSenha(usuario);

                        // Direciona para tela de sucesso, independente se houver 1 ou mais PVs.
                        this.ExibirSucesso("Senha alterada com sucesso.", "", string.Empty);
                        // this.LoginUsuario(usuario.PvsSelecionados.FirstOrDefault().ToString(), usuario.EmailUsuario, novaSenha, "N");

                        InformacaoUsuario.Limpar();
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, string.Concat("Atenção", ex.Message), SPContext.Current.Site.Url);

                    return;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", SPContext.Current.Site.Url);

                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);

                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", SPContext.Current.Site.Url);

                    return;
                }
            }
        }

        /// <summary>
        /// Redireciona o usuário para a página inicial do Portal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAcessarPortal_Click(object sender, EventArgs e)
        {
            Response.Redirect(SPContext.Current.Site.Url);
        }

        /// <summary>
        /// Grava um histórico dos pvs que realizaram alteração de senha
        /// </summary>
        /// <param name="idUsuario">Id do usuário</param>
        /// <param name="nomeCompleto">Nome completo</param>
        /// <param name="emailUsuario">Email usuário</param>
        /// <param name="tipoUsuario">Tipo usuário</param>
        /// <param name="pvsSelecionados">Pvs selecionados</param>
        /// <param name="formaRecuperacaoSenha">Forma de recuperação de senha</param>
        private void HistoricoRecuperacaoSenha(int idUsuario, string nomeCompleto, string emailUsuario, string tipoUsuario, int[] pvsSelecionados, string formaRecuperacaoSenha)
        {
            if (pvsSelecionados != null && pvsSelecionados.Any())
            {
                foreach (var numPv in pvsSelecionados)
                {
                    //Registra no histórico/log de atividades
                    Historico.RecuperacaoSenha(idUsuario,
                                               nomeCompleto,
                                               emailUsuario,
                                               tipoUsuario,
                                               numPv,
                                               formaRecuperacaoSenha);
                }
            }
        }

        /// <summary>
        /// Validação complementar do campo de senha
        /// </summary>
        /// <param name="source">Objeto fornecido pelo .NET</param>
        /// <param name="args">Argumentos complementares da validação</param>
        public void txtSenha_ServerValidate(object source, SenhaServerValidateEventArgs args)
        {
            args.IsValid = true;
            args.ErrorMessage = String.Empty;

            using (Logger log = Logger.IniciarLog("Validar e atualiza Senha para o acesso para o Portal de Serviços - txtSenha_ServerValidate"))
            {
                if (!InformacaoUsuario.Existe())
                {
                    args.ErrorMessage = "Usuário não localizado, tente novamente.";
                    args.IsValid = false;
                    return;
                }

                InformacaoUsuario usuario = InformacaoUsuario.Recuperar();

                log.GravarMensagem("Verificando a possibilidade de atualizar a senha", new { result = usuario.PodeRecuperarCriarAcesso && (usuario.PvsSelecionados != null && usuario.PvsSelecionados.Any()) });

                if (!usuario.PodeRecuperarCriarAcesso)
                {
                    log.GravarMensagem("Problemas método txtSenha_ServerValidate usuario.PodeRecuperarCriarAcesso && (usuario.PvsSelecionados != null && usuario.PvsSelecionados.Any())", new { PvsSelecionados = usuario.PvsSelecionados });

                    args.ErrorMessage = base.RetornarMensagemErro("SharePoint.PodeRecuperarAcessoSenha", 1154);
                    args.IsValid = false;

                    return;
                }

                bool possuiKomerci;
                Int32 codigoRetorno;

                if (usuario.PvsSelecionados == null || !usuario.PvsSelecionados.Any())
                    usuario.PvsSelecionados = new Int32[] { usuario.NumeroPV };

                String novaSenha = EncriptadorSHA1.EncryptString(txtSenha.Value);

                using (var ctxEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    //Verifica se ao menos um estabelecimento é komerci
                    possuiKomerci = ctxEntidade.Cliente.PossuiKomerci(usuario.PvsSelecionados);
                }

                using (var ctxUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                {
                    codigoRetorno = ctxUsuario.Cliente.AtualizarSenhaUsuarioNovoAcesso(
                                                usuario.IdUsuario,
                                                novaSenha,
                                                possuiKomerci,
                                                usuario.PvsSelecionados,
                                                false,
                                                WebPartRecuperacaoNovaSenha.RecuperacaoSenha);
                }

                if (codigoRetorno > 0)
                {
                    args.ErrorMessage = base.RetornarMensagemErro("UsuarioServico.AtualizarSenhaUsuario", codigoRetorno);
                    args.IsValid = false;
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
                FormaRecuperacao formaRecuperacao = (FormaRecuperacao)queryString["FormaRecuperacao"].ToInt32();
                string pvs = queryString["Pvs"].ToString();

                Int32 codigoRetorno = 0;

                UsuarioServico.Usuario usuario;

                using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    usuario = contextoUsuario.Cliente.ConsultarPorID(out codigoRetorno, codigoIdUsuario);

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
                    infoUsuario.SetPvsSelecionados(pvs);

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

            pnMensagem.Titulo = titulo;
            pnMensagem.Mensagem = mensagem;
            pnMensagem.TipoMensagem = PainelMensagemIcon.Erro;

            pnlAviso.Visible = true;
            pnlNovaSenha.Visible = false;

            pnlVoltar.Visible = !String.IsNullOrEmpty(urlVoltar);
            if (pnlVoltar.Visible)
                btnVoltar.OnClientClick = String.Format("window.location.href = '{0}'; return false;", urlVoltar);
        }

        /// <summary>
        /// Exibe uma mensagem de sucesso na página
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        private void ExibirSucesso(String mensagem, String titulo, String urlVoltar)
        {
            pnlSenhaAlterada.Visible = true;
            pnlAviso.Visible = false;
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

            String url = Util.BuscarUrlRedirecionamento(
                                String.Format("/_layouts/DadosCadastrais/Login.aspx?dados={0}",
                                                queryString.ToString()),
                                SPUrlZone.Internet);
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
                    catch (HttpException ex)
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

        #endregion
    }
}
