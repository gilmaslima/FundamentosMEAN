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
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Rede.PN.DadosCadastraisMobile.SharePoint.UsuarioServico;
using System.Web;
using System.ServiceModel;
using Microsoft.SharePoint.Administration;
using Rede.PN.DadosCadastraisMobile.SharePoint.CONTROLTEMPLATES.DadosCadastraisMobile;
using Rede.PN.DadosCadastraisMobile.SharePoint.Util;
using System.Linq;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.ConfirmarAcessoMob
{
    public partial class ConfirmarAcessoMobUserControl : UserControlBase
    {
        #region [ Propriedades ]

        /// <summary>QueryString</summary>
        public QueryStringSegura QueryString
        {
            get
            {
                var queryString = new QueryStringSegura();
                String dados = Request["dados"];
                if (!String.IsNullOrEmpty(dados))
                {
                    try
                    {
                        queryString = new QueryStringSegura(dados);
                    }
                    //Retorna QueryString nula sempre que inválida
                    catch (QueryStringExpiradaException ex)
                    {
                        Logger.GravarErro("Erro durante recuperação da QueryString", ex);
                    }
                    catch (QueryStringInvalidaException ex)
                    {
                        Logger.GravarErro("Erro durante recuperação da QueryString", ex);
                    }
                    catch (Exception ex)
                    {
                        Logger.GravarErro("Erro durante recuperação da QueryString", ex);
                    }
                }
                return queryString;
            }
        }

        /// <summary>
        /// Código ID do usuário
        /// </summary>
        private Int32? CodigoIdUsuario { get { return QueryString["CodigoIdUsuario"].ToInt32Null(); } }

        /// <summary>
        /// Timestamp com a data de envio do e-mail
        /// </summary>
        private DateTime? Timestamp { get { return QueryString["TimeStamp"].ToDateTimeNull("ddMMyyyyHHmmss"); } }

        /// <summary>
        /// Status do usuário
        /// </summary>
        private Int32? Status { get { return QueryString["Status"].ToInt32Null(); } }

        /// <summary>
        /// Hash de confirmação de e-mail
        /// </summary>
        private Guid? Hash { get { return QueryString["Hash"].ToGuidNull(); } }

        /// <summary>
        /// Retorna um PV
        /// </summary>
        private string Pvs { get { return QueryString["Pvs"]; } }

        /// <summary>
        /// Retorna a lista de Pvs selecionados no passo anterior pelo usuário
        /// </summary>
        private string PvsSelecionados { get { return QueryString["PvsSelecionados"]; } }

        /// <summary>
        /// Retorna a lista de Pvs selecionados no passo anterior pelo usuário
        /// </summary>
        private string Email { get { return QueryString["Email"]; } }

        /// <summary>
        /// Informa se é referente ao processo de criação de usuário
        /// </summary>
        private bool CriacaoUsuario
        {
            get
            {
                bool value;

                if (bool.TryParse(QueryString["CriacaoUsuario"], out value))
                {
                    return value;
                }

                return false;
            }
        }

        /// <summary>
        /// Retorna o CPF do usuário
        /// </summary>
        private long CpfUsuario
        {
            get
            {
                long value;

                if (long.TryParse(QueryString["CpfUsuario"], out value))
                {
                    return value;
                }

                return 0;
            }
        }

        /// <summary>
        /// Forma de recuperação de senha. Utilizada apenas nos e-mails de recuperação de senha
        /// </summary>
        private FormaRecuperacao FormaRecuperacao { get { return (FormaRecuperacao)QueryString["FormaRecuperacao"].ToInt32(); } }

        #endregion

        #region [ Controle ]

        /// <summary>
        /// ucConfirmacaoExpirada control.
        /// </summary>
        protected QuadroAvisosResponsivo UcConfirmacaoExpirada { get { return (QuadroAvisosResponsivo)ucConfirmacaoExpirada; } }

        #endregion

        #region [ Eventos da Página ]

        /// <summary>
        /// Page_Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CriacaoUsuario)
            {
                ProcessarConfirmarAcessoCriacaoUsuario();
            }
            else
            {
                ProcessarConfirmarAcessoRecuperacaoUsuarioSenha();
            }
        }

#if DEBUG
        /// <summary>
        /// OnPreRender.
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            pnlFooter.Controls.Add(new LiteralControl("Codigo ID Usuário: " + this.CodigoIdUsuario));
            pnlFooter.Controls.Add(new LiteralControl("<br/>"));
            pnlFooter.Controls.Add(new LiteralControl("Hash E-mail: " + this.Hash));
        }
#endif

        /// <summary>
        /// Direciona o usuário para a confirmação de criação de usuário
        /// </summary>
        public void ProcessarConfirmarAcessoCriacaoUsuario()
        {
            var mensagemPadrao = String.Format(String.Concat(
                        "O prazo para confirmação do seu cadastro via e-mail expirou.<br />",
                        "<a href='{0}/Paginas/Mobile/CriacaoUsrDadosIniciais.aspx'>Clique aqui</a> ",
                        "para realizar novamente o cadastro de usuário e senha."),
                        base.web.ServerRelativeUrl);

            using (Logger log = Logger.IniciarLog("Confirmacao criação usuário"))
            {
                if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Pvs) || CpfUsuario == 0)
                {
                    this.ExibirErro("Erro ao validar confirmação de acesso", "Atenção", base.RecuperarEnderecoPortal());
                    return;
                }

                try
                {
                    //Consulta dados do hash para validação
                    UsuarioHash hash = ConsultarHash(this.Hash.Value);
                    Boolean hashValido = VerificarValidadeHash(hash);

                    //Usuário existe, porém o hash não foi encontrado: exibe mensagem genérica de expiração de hash
                    //Ocorre quando o hash do usuário foi sobreposto por outro hash
                    if (!hashValido)
                    {
                        ExibirMensagemExpiracao(mensagemPadrao);
                        return;
                    }

                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                    {
                        ctx.Cliente.AtualizarStatusPorPvs(ConvertToArrayPvs(), CpfUsuario, Email, Status1.UsuarioAtivo);

                        ctx.Cliente.ExcluirHashPorGuid(this.Hash.Value);
                    }

                    this.ExibirSucesso("Aproveite e baixe grátis o app Rede no seu celular.", "Parabéns, agora você tem acesso aos canais Rede.", string.Empty);

                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Método para processar a confirmação de acesso para a recuperação de usuário/senha
        /// </summary>
        public void ProcessarConfirmarAcessoRecuperacaoUsuarioSenha()
        {
            var mensagemPadrao = String.Format(String.Concat(
                        "O prazo para confirmação do seu cadastro via e-mail expirou.<br />",
                        "<a href='{0}/Paginas/Mobile/CriacaoUsrDadosIniciais.aspx'>Clique aqui</a> ",
                        "para realizar novamente o cadastro de usuário e senha."),
                        base.web.ServerRelativeUrl);

            if (this.CodigoIdUsuario.HasValue && this.Hash.HasValue)
            {
                //Consulta usuário
                Usuario usuario = ConsultarUsuario(this.CodigoIdUsuario.Value);

                //Se usuário não encontrado (excluído) ou inválido, exibe mensagem de expiração
                //com link sugestivo para criação de novo usuário
                if (usuario == null)
                {
                    ExibirMensagemExpiracao(mensagemPadrao);
                    return;
                }

                //Consulta dados do hash para validação
                UsuarioHash hash = ConsultarHash(this.Hash.Value);
                Boolean hashValido = VerificarValidadeHash(hash);

                //Tratamentos se Usuário for Legado
                if (usuario.Legado)
                {
                    //Se usuário for legado e hash não for mais válido (inválido ou expirado),
                    //exibe instruções para legado
                    if (!hashValido)
                    {
                        String mensagem = String.Concat(
                            "O prazo para confirmação do seu cadastro via e-mail expirou.",
                            "<br/><br/>",
                            "Por favor, acesse novamente o Portal Rede com os dados antigos ",
                            "(usuário alfanumérico) para reenviarmos o e-mail.");
                        ExibirMensagemExpiracao(mensagem);
                        return;
                    }
                    else
                    {
                        //Hash Válido!
                        //Redireciona para confirmação de migração
                        this.RedirecionarConfirmacaoAlteracaoEmail(this.CodigoIdUsuario.Value, this.Hash.Value);
                        return;
                    }
                }
                else
                {
                    //Usuário existe, porém o hash não foi encontrado: exibe mensagem genérica de expiração de hash
                    //Ocorre quando o hash do usuário foi sobreposto por outro hash
                    if (hash == null)
                    {
                        ExibirMensagemExpiracao(mensagemPadrao);
                        return;
                    }

                    //Usuário e hash existem, porém o hash está expirado: exibe mensagem 
                    //de expiração de hash de acordo com o status do hash
                    if (!hashValido)
                    {
                        if (hash.Status.Codigo.HasValue)
                        {
                            Status1 status = (Status1)hash.Status.Codigo.Value;
                            String mensagem = default(String);

                            switch (status)
                            {
                                //Mensagem de expiração de hash de Confirmação de Alteração de E-mail
                                case Status1.UsuarioAguardandoConfirmacaoAlteracaoEmail:
                                    mensagem = String.Concat(
                                        "O prazo para confirmação do seu cadastro via e-mail expirou.<br/><br/>",
                                        "Por favor, realize a alteração de e-mail novamente, acessando o Portal Rede ",
                                        "com seu usuário (e-mail) e senha.");
                                    break;

                                //Mensagem de expiração de hash de Confirmação de E-mail de Criação
                                case Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                                    if (!usuario.Origem.Equals("M")) //Usuário não é de Origem de Migração
                                    {
                                        pnlAvisoExpiracaoReenvio.Visible = true;
                                        ucConfirmacaoExpirada.Visible = false;
                                    }
                                    else //Usuário não é de Origem de Migração
                                    {
                                        mensagem = String.Format(String.Concat(
                                            "O prazo para confirmação do seu cadastro via e-mail expirou.<br />",
                                            "<a href='{0}/Paginas/Mobile/CriacaoUsrDadosIniciais.aspx'>Clique aqui</a> ",
                                            "para realizar novamente o cadastro de usuário e senha."),
                                            base.web.ServerRelativeUrl);
                                    }
                                    break;

                                //Mensagem genérica de expiração de hash
                                case Status1.UsuarioAtivoAguardandoConfirmacaoRecuperacaoSenha:
                                default:
                                    mensagem = "O prazo para confirmação via e-mail expirou.";
                                    break;
                            }

                            ExibirMensagemExpiracao(mensagem);
                        }

                        return;
                    }

                    //Hash Válido!
                    //Redireciona de acordo com o status do usuário
                    if (usuario.Status.Codigo.HasValue)
                    {
                        Status1 status = (Status1)usuario.Status.Codigo.Value;
                        switch (status)
                        {
                            //Confirmação de E-mail
                            case Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                            case Status1.UsuarioAguardandoConfirmacaoAlteracaoEmail:
                                this.RedirecionarConfirmacaoAlteracaoEmail(this.CodigoIdUsuario.Value, this.Hash.Value);
                                return;
                            //Solicitação de Senha
                            case Status1.UsuarioAtivoAguardandoConfirmacaoRecuperacaoSenha:
                            case Status1.UsuarioBloqueadoAguardandoConfirmacaoRecuperacaoSenha:
                                this.RedirecionarConfirmacaoSenha(usuario, this.Hash.Value, this.FormaRecuperacao, this.Pvs);
                                return;
                            default:
                                break;
                        }
                    }
                }
            }

            //Se algum dado é inválido, redireciona usuário para a Home do Portal
            RedirecionarHome();
        }

        /// <summary>
        /// Exibe uma mensagem de sucesso na página
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        private void ExibirSucesso(String mensagem, String titulo, String urlVoltar)
        {
            pnlAvisoExpiracaoReenvio.Visible = false;
            pnlAvisoEmail.Visible = false;
            pnlConfirmacaoCriacaoUsuario.Visible = true;
        }

        /// <summary>
        /// Exibe uma mensagem de sucesso na página
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        private void ExibirErro(String mensagem, String titulo, String urlVoltar)
        {
            pnlAvisoExpiracaoReenvio.Visible = true;
            pnlAvisoEmail.Visible = false;
            pnlConfirmacaoCriacaoUsuario.Visible = false;
            lblMensagem.Text = mensagem;
            lblTitulo.Text = titulo;
        }

        public int[] ConvertToArrayPvs()
        {
            string[] result;


            if (!string.IsNullOrEmpty(PvsSelecionados))
            {
                result = PvsSelecionados.Split('|');

                return result.Count() == 0 ? new int[0] : result.Select(x => Convert.ToInt32(x)).ToArray();
            }
            else if (!string.IsNullOrEmpty(Pvs))
            {
                result = Pvs.Split('|');

                return result.Count() == 0 ? new int[0] : result.Select(x => Convert.ToInt32(x)).ToArray();
            }

            return null;

        }

        /// <summary>
        /// Ação de Voltar para o Portal Aberto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Voltar(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Ação de Voltar para Home do Portal Aberto"))
            {
                try
                {
                    this.RedirecionarHome();
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Reenvio/Revalidação de e-mail de confirmação de cadastro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lnkReenviar_Click(object sender, EventArgs e)
        {
            var usuario = default(Usuario);
            var hashUsuario = default(UsuarioHash);
            Int32 codigoIdUsuario = 0;

            if (this.CodigoIdUsuario.HasValue)
            {
                codigoIdUsuario = this.CodigoIdUsuario.Value;

                using (Logger log = Logger.IniciarLog("Consultando usuário por ID"))
                {
                    var codigoRetorno = default(Int32);

                    try
                    {
                        using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                            usuario = ctx.Cliente.ConsultarPorID(out codigoRetorno, codigoIdUsuario);

                        // Caso o código de retorno seja != de 0, ocorreu um erro
                        if (codigoRetorno > 0)
                            base.ExibirPainelExcecao("UsuarioServico.Consultar", codigoRetorno);
                        else
                        {
                            using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                                hashUsuario = ctx.Cliente.ReiniciarHash(out codigoRetorno, codigoIdUsuario);

                            if (hashUsuario != null)
                            {
                                EmailNovoAcesso.EnviarEmailConfirmacaoCadastro48h(usuario.Email,
                                        usuario.CodigoIdUsuario, hashUsuario.Hash, usuario.CodigoIdUsuario, usuario.Email,
                                        usuario.Descricao, usuario.TipoUsuario, usuario.Entidade.Codigo, null);

                                pnlAvisoExpiracaoReenvio.Visible = false;
                                //btnVoltar.Visible = true;
                                UcConfirmacaoExpirada.Visible = true;
                                UcConfirmacaoExpirada.ClasseImagem = "ok";
                                UcConfirmacaoExpirada.IconeImagem = "ok";
                                UcConfirmacaoExpirada.CarregarMensagem("E-mail enviado com sucesso.",
                                                                            @"Dentro de instantes você receberá um e-mail de confirmação.<br />
                                                                             Acesse o link informado no e-mail em até 12h para concluir seu cadastro.");
                            }
                        }
                    }
                    catch (FaultException<UsuarioServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }
            else
                base.ExibirPainelExcecao("SharePoint.SessaoUsuario", 1151);
        }

        #endregion

        /// <summary>
        /// Redireciona usuário para a Home do Portal
        /// </summary>
        private void RedirecionarHome()
        {
            String url = Redecard.PN.Comum.Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Identifica para qual redirecionamento de alteração de senha o usuário será encaminhado
        /// </summary>
        /// <param name="usuario">Usuário</param>
        /// <param name="hashEmail">Hash do e-mail</param>
        /// <param name="formaRecuperacao">Forma de recuperação da senha (principal, secundário, sms)</param>
        private void RedirecionarConfirmacaoSenha(Usuario usuario, Guid hashEmail, FormaRecuperacao formaRecuperacao, string pvs)
        {
            Boolean bloqueadoAguardandoConfirmacaoRecuperacaoSenha = (usuario.Status.Codigo.Value
                                                                                == (Int32)Redecard.PN.Comum.Enumerador.
                                                                                    Status.UsuarioBloqueadoAguardandoConfirmacaoRecuperacaoSenha);
            this.RedirecionarConfirmacaoAlteracaoSenha(usuario.CodigoIdUsuario, hashEmail, formaRecuperacao, this.Pvs);
        }

        /// <summary>
        /// Redireciona para a tela de Confirmação de Alteração de E-mail
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="hashConfirmacao">Hash de confirmação do e-mail</param>
        private void RedirecionarConfirmacaoAlteracaoEmail(Int32 codigoIdUsuario, Guid hashConfirmacao)
        {
            var qs = new QueryStringSegura();
            qs["CodigoIdUsuario"] = codigoIdUsuario.ToString();
            qs["Hash"] = hashConfirmacao.ToString("N");

            String urlPagina = String.Format("{0}/Paginas/Mobile/CriacaoUsrConfirmacao.aspx", base.web.ServerRelativeUrl);
            String qsDados = qs.ToString();

            Response.Redirect(String.Format("{0}?dados={1}", urlPagina, qsDados), false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Redireciona para a tela de Alteração de senha
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="hashConfirmacao">Hash de confirmação do e-mail</param>
        /// <param name="formaRecuperacao">Forma de recuperação da senha (principal, secundário, sms)</param>
        private void RedirecionarConfirmacaoAlteracaoSenha(Int32 codigoIdUsuario, Guid hashConfirmacao, FormaRecuperacao formaRecuperacao, string pvs)
        {
            var qs = new QueryStringSegura();
            qs["CodigoIdUsuario"] = codigoIdUsuario.ToString();
            qs["Hash"] = hashConfirmacao.ToString("N");
            qs["FormaRecuperacao"] = ((Int32)formaRecuperacao).ToString();
            qs["Pvs"] = pvs;

            String urlPagina = String.Format("{0}/Paginas/Mobile/RecNovaSenhaBasico.aspx", base.web.ServerRelativeUrl);
            String qsDados = qs.ToString();

            Response.Redirect(String.Format("{0}?dados={1}", urlPagina, qsDados), false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Redireciona para a tela de Confirmação Positiva de Alteração de Senha
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="hashConfirmacao">Hash de confirmação do e-mail</param>
        /// <param name="formaRecuperacao">Forma de recuperação da senha (principal, secundário, sms)</param>
        private void RedirecionarConfirmacaoPositivaAlteracaoSenha(Int32 codigoIdUsuario, Guid hashConfirmacao, FormaRecuperacao formaRecuperacao)
        {
            var qs = new QueryStringSegura();
            qs["CodigoIdUsuario"] = codigoIdUsuario.ToString();
            qs["Hash"] = hashConfirmacao.ToString("N");
            qs["FormaRecuperacao"] = ((Int32)formaRecuperacao).ToString();

            String urlPagina = String.Format("{0}/Paginas/Mobile/RecuperacaoSenhaConfirmacaoPositiva.aspx", base.web.ServerRelativeUrl);
            String qsDados = qs.ToString();

            Response.Redirect(String.Format("{0}?dados={1}", urlPagina, qsDados), false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Método para verificação se o Hash de Confirmação de e-mail existe e ainda não expirou
        /// </summary>
        /// <param name="hash">Hash de confirmação de e-mail</param>
        /// <returns>TRUE: hash válido (existe e não expirou); FALSE, C.C.</returns>
        private static Boolean VerificarValidadeHash(UsuarioHash hash)
        {
            //Verifica a data de expiração
            if (hash != null)
            {
                //Se não existe data de expiração, hash não expira
                if (!hash.DataExpiracao.HasValue)
                    return true;
                return
                    DateTime.Now <= hash.DataExpiracao;
            }
            else
                return false;
        }

        /// <summary>
        /// Exibir mensagem de expiração.
        /// </summary>
        /// <param name="mensagem">Mensagem a ser exibida</param>
        private void ExibirMensagemExpiracao(String mensagem)
        {
            UcConfirmacaoExpirada.IconeImagem = "atencao";
            UcConfirmacaoExpirada.ClasseImagem = "atencao";
            UcConfirmacaoExpirada.Mensagem = mensagem;
            UcConfirmacaoExpirada.CarregarMensagem();
        }

        #region [ Consultas ]

        /// <summary>
        /// Consulta Hash de confirmação de e-mail
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="hashConfirmacao">Hash de confirmação do e-mail</param>
        private static UsuarioHash ConsultarHash(Guid hashConfirmacao)
        {
            using (Logger log = Logger.IniciarLog("Consulta Hash de confirmação de e-mail"))
            {
                var hashes = default(UsuarioHash[]);
                var codigoRetorno = default(Int32);

                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        hashes = ctx.Cliente.ConsultarHash(out codigoRetorno,null , null, hashConfirmacao);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }

                if (codigoRetorno == 0 && hashes.Length > 0)
                    return hashes[0];
                else
                    return null;
            }
        }

        /// <summary>
        /// Consulta um usuário por ID
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        private Usuario ConsultarUsuario(Int32 codigoIdUsuario)
        {
            var usuario = default(Usuario);

            using (Logger log = Logger.IniciarLog("Consultando usuário por ID"))
            {
                var codigoRetorno = default(Int32);

                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        usuario = ctx.Cliente.ConsultarPorID(out codigoRetorno, codigoIdUsuario);

                    // Caso o código de retorno seja != de 0, ocorreu um erro
                    if (codigoRetorno > 0)
                        base.ExibirPainelExcecao("UsuarioServico.Consultar", codigoRetorno);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return usuario;
        }

        #endregion
    }
}
