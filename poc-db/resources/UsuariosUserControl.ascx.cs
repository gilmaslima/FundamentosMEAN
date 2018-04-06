/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using Microsoft.SharePoint;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios.Usuarios
{
    /// <summary>
    /// Administração de Usuários - Página Inicial - Listagem de Usuários
    /// </summary>
    public partial class UsuariosUserControl : UsuariosUserControlBase
    {
        #region [ Propriedades ]

        /// <summary>
        /// Não valida se existe usuário selecionado: verificação não se aplica para esta página
        /// </summary>
        public override Boolean ValidarNavegacao { get { return false; } }

        /// <summary>
        /// Ordenacao da tabela de usuarios coluna
        /// </summary>
        private String OrdenacaoColuna
        {
            get
            {
                if (ViewState["OrdenacaoColuna"] == null)
                    ViewState["OrdenacaoColuna"] = String.Empty;
                return (String)ViewState["OrdenacaoColuna"];
            }
            set { ViewState["OrdenacaoColuna"] = value; }
        }

        /// <summary>
        /// Ordenacao da tabela de usuarios direcao
        /// </summary>
        private String OrdenacaoDirecao
        {
            get
            {
                if (ViewState["OrdenacaoDirecao"] == null)
                    ViewState["OrdenacaoDirecao"] = String.Empty;
                return (String)ViewState["OrdenacaoDirecao"];
            }
            set { ViewState["OrdenacaoDirecao"] = value; }
        }

        /// <summary>
        /// Lista dos usuarios
        /// </summary>
        private List<Usuario> Usuarios
        {
            get
            {
                if (ViewState["Usuarios"] == null)
                    ViewState["Usuarios"] = new List<Usuario>();
                return (List<Usuario>)ViewState["Usuarios"];
            }
            set { ViewState["Usuarios"] = value; }
        }

        /// <summary>
        /// Ordenacao da tabela de usuarios direcao
        /// </summary>
        private Boolean CentralAtendimento
        {
            get
            {
                return this.SessaoAtual != null && this.SessaoAtual.UsuarioAtendimento;
            }
        }
        #endregion
        
        #region [ Eventos de Página ]

        /// <summary>
        /// Load da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("administracao de usuarios - Pageload"))
            {
                try
                {
                    //Verifica permissão (apenas Master e Central de Atendimento podem acessar)
                    if (!base.PossuiPermissao)
                    {
                        mvwUsuarios.SetActiveView(pnlAcessoNegado);
                        return;
                    }

                    //Verifica se possui QueryString
                    if (QS != null)
                    {
                        //Recupera ID do usuário e tipo de ação
                        Int32? codigoIdUsuario = QS["CodigoIdUsuario"].ToInt32Null();
                        String acao = QS["Acao"];

                        if (codigoIdUsuario.HasValue)
                        {
                            //Caso seja para alteração de usuário, chaveia para fluxo de edição
                            if (String.Compare("AlterarPermissoes", acao, true) == 0)
                            {
                                AlterarPermissoesUsuario(codigoIdUsuario.Value);
                                return;
                            }
                        }
                    }

                    //Botão default da página
                    this.Page.Form.DefaultButton = btnBuscar.UniqueID;

                    if (!IsPostBack)
                    {
                        this.CarregarUsuarios();

                        if (this.Usuarios.Count > 0)
                        {
                            // popula a drop de status
                            var status = this.Usuarios.GroupBy(u => u.Status.Descricao).Select(group => group.Key).OrderBy(s => s).ToList();
                            status.Insert(0, "selecione um item");

                            ddlStatus.DataSource = status;
                            ddlStatus.DataBind();
                        }
                        else
                        {
                            divFiltroUsuario.Visible = false;
                            lblUsuariosEncontrados.Visible = false;
                            divRegistroNaoEncontrado.Visible = true;
                            lblSemUsuarios.Visible = true;
                        }

                        this.VerificarCriacaoAcessoBloqueada();
                    }
                    else
                    {
                        if (String.Compare(Request.Params.Get("__EVENTTARGET"), "desbloquearusuario", true) == 0)
                        {
                            DesbloquearUsuario(Request.Params.Get("__EVENTARGUMENT").ToInt32());
                        }
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
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

        #endregion

        #region [ Eventos dos Controles ]


        /// <summary>
        /// Evento de busca de usuário
        /// </summary>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("administracao de usuarios - btnBuscar_Click"))
            {
                try
                {
                    lblUsuariosEncontrados.Visible = true;
                    divRegistroNaoEncontrado.Visible = false;
                    lblSemUsuariosNoFiltro.Visible = false;

                    this.OrdenacaoColuna = String.Empty;
                    this.OrdenacaoDirecao = String.Empty;

                    this.Usuarios.Clear();
                    this.BuscarUsuarios();

                    if (this.Usuarios.Count == 0)
                    {
                        lblUsuariosEncontrados.Visible = false;
                        divRegistroNaoEncontrado.Visible = true;
                        lblSemUsuariosNoFiltro.Visible = true;
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
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
        /// Evento de exclusão de usuários
        /// </summary>
        protected void btnExcluirUsuarios_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("administracao de usuarios - btnExcluirUsuarios_Click"))
            {
                try
                {
                    //Recupera os usuários selecionados
                    List<Int32> codigosIdUsuarios = grvUsuarios.Marcados.Cast<Int32>().ToList();

                    //Garante que ele mesmo não esteja marcado para exclusão
                    codigosIdUsuarios = codigosIdUsuarios.Except(new[] { this.SessaoAtual.CodigoIdUsuario }).ToList();

                    if (codigosIdUsuarios.Count > 0)
                        this.ExcluirUsuarios(codigosIdUsuarios);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
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
        /// Evento de Download dos usuários
        /// </summary>
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("administracao de usuarios - btnDownload_Click"))
            {
                try
                {
                    String nomeArquivo = String.Format("Administracao de Usuarios - Usuarios do Portal - {0}.xls",
                                            DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"));
                    String conteudoCsv = GerarConteudoExportacao();

                    Response.Clear();
                    Response.ClearHeaders();
                    Response.Buffer = true;
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + nomeArquivo);
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
                    Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                    Response.ContentType = "application/ms-excel";
                    Response.Write(conteudoCsv);
                    Response.Flush();
                    Response.End();
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
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
        /// Botão Voltar do Quadro de Aviso
        /// </summary>
        protected void btnAvisoVoltar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("administracao de usuarios - btnAvisoVoltar_Click"))
            {
                try
                {
                    lblUsuariosEncontrados.Visible = true;
                    divRegistroNaoEncontrado.Visible = false;
                    lblSemUsuariosNoFiltro.Visible = false;

                    this.OrdenacaoColuna = String.Empty;
                    this.OrdenacaoDirecao = String.Empty;

                    txtBusca.Text = String.Empty;
                    ddlStatus.SelectedIndex = -1;

                    this.Usuarios.Clear();
                    this.BuscarUsuarios();

                    if (this.Usuarios.Count == 0)
                    {
                        lblUsuariosEncontrados.Visible = false;
                        divRegistroNaoEncontrado.Visible = true;
                        lblSemUsuariosNoFiltro.Visible = true;
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
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
        /// grvUsuarios RowDataBound
        /// </summary>
        protected void grvUsuarios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var usuario = e.Row.DataItem as Usuario;

                var lnkBtnAlterarUsuario = (LinkButton)e.Row.FindControl("lnkBtnAlterarUsuario");
                var lnkBtnDesbloquearUsuario = (LinkButton)e.Row.FindControl("lnkBtnDesbloquearUsuario");
                var lnkBtnAprovar = (LinkButton)e.Row.FindControl("lnkBtnAprovar");
                var lnkBtnRejeitar = (LinkButton)e.Row.FindControl("lnkBtnRejeitar");

                var ltrCpf = (Literal)e.Row.FindControl("ltrCpf");
                var ltrPerfil = (Literal)e.Row.FindControl("ltrPerfil");
                var lblStatus = (Label)e.Row.FindControl("lblStatus");
                var ltrEmail = (Literal)e.Row.FindControl("ltrEmail");
                var chkBoxSelecionar = (CheckBox)e.Row.FindControl(grvUsuarios.CheckBoxItemID);

                //Verifica status do usuário
                Status1? statusUsuario = usuario.Status.Codigo.HasValue ? (Status1)usuario.Status.Codigo.Value : (Status1?)null;
                Boolean usuarioProprio = usuario.CodigoIdUsuario == SessaoAtual.CodigoIdUsuario;
                Boolean usuarioDesbloqueavel =
                    (statusUsuario == Status1.UsuarioAtivoLiberacaoAcessoCompletoBloqueada && this.CentralAtendimento) ||
                     statusUsuario == Status1.UsuarioBloqueadoRecuperacaoSenha ||
                     statusUsuario == Status1.UsuarioBloqueadoRecuperacaoUsuario ||
                     statusUsuario == Status1.UsuarioBloqueadoSenhaInvalida ||
                     statusUsuario == Status1.UsuarioBloqueadoAguardandoConfirmacaoRecuperacaoSenha;

                Boolean usuarioEditavel = false;
                Boolean usuarioAguardandoConf = statusUsuario == Status1.UsuarioAguardandoConfirmacaoMaster;

                //Central de Atendimento tem acesso à tela de Edição em modo Somente Leitura
                //apenas para reenvio de e-mail de confirmação
                if (this.CentralAtendimento)
                    usuarioEditavel = statusUsuario == Status1.UsuarioAguardandoConfirmacaoAlteracaoEmail ||
                                      statusUsuario == Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario;
                //Se for usuário Master, pode editar usuários ativos ou que estejam aguardando confirmações de e-mail
                //Master só não pode editar ele mesmo
                else
                    usuarioEditavel =
                        //Não pode ser ele mesmo
                        !usuarioProprio &&
                        (statusUsuario == Status1.UsuarioAguardandoConfirmacaoAlteracaoEmail ||
                        statusUsuario == Status1.UsuarioAtivo ||
                        statusUsuario == Status1.UsuarioAtivoAguardandoConfirmacaoRecuperacaoSenha ||
                        statusUsuario == Status1.UsuarioAtivoLiberacaoAcessoCompletoBloqueada ||
                        statusUsuario == Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario ||
                        statusUsuario == Status1.RespostaIdPosPendente);

                //Atribui o Código ID do usuário nos botões
                lnkBtnAlterarUsuario.CommandArgument = usuario.CodigoIdUsuario.ToString();
                lnkBtnDesbloquearUsuario.Attributes.Add("codigoidusuario", usuario.CodigoIdUsuario.ToString());
                lnkBtnAprovar.CommandArgument = usuario.CodigoIdUsuario.ToString();
                lnkBtnRejeitar.CommandArgument = usuario.CodigoIdUsuario.ToString();

                //Desabilita botão Alterar para usuário não editáveis:
                if (!usuarioEditavel)
                    lnkBtnAlterarUsuario.Visible = false;

                //Desabilita botão Desbloquear para usuários "não bloqueados"
                if (!usuarioDesbloqueavel)
                    lnkBtnDesbloquearUsuario.Visible = false;

                //Desabilita botão Aprovar e Rejeitar para usuários que nao estao aguardando confirmacao
                if (!usuarioAguardandoConf || this.CentralAtendimento)
                {
                    lnkBtnAprovar.Visible = false;
                    lnkBtnRejeitar.Visible = false;
                }

                //Armazena, no botão de Desbloqueio, se o usuário é legado
                if (usuario.Legado)
                    lnkBtnDesbloquearUsuario.Attributes["leg"] = Boolean.TrueString.ToLower();

                //O próprio usuário não pode se excluir
                if (usuarioProprio)
                    chkBoxSelecionar.Enabled = false;

                //Central de atendimento não pode excluir
                if (this.CentralAtendimento)
                    chkBoxSelecionar.Visible = false;

                ltrEmail.Text = String.IsNullOrEmpty(usuario.Email) ? usuario.Codigo : usuario.Email;
                ltrCpf.Text = CampoCpf.AplicarMascara(usuario.CPF);

                //Descrição do Perfil do Usuário
                ltrPerfil.Text = ObterDescricaoPerfil(usuario.TipoUsuario);

                //Descrição do Status do Usuário
                lblStatus.Text = usuario.Status.Descricao;
                lblStatus.ToolTip = usuario.Status.Descricao;
            }
        }

        /// <summary>
        /// grvUsuarios PageIndexChanging
        /// </summary>
        protected void grvUsuarios_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("administracao de usuarios - grvUsuarios_PageIndexChanging"))
            {
                try
                {
                    grvUsuarios.PageIndex = e.NewPageIndex;
                    CarregarUsuarios();
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
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
        /// Evento de click do botão de desbloqueio da entidade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDesbloquearAgora_Click(object sender, EventArgs e)
        {
            if (DesbloquearEntidadeCriacaoAcesso())
                this.ExibirMensagemDesbloqueioCriacaoAcesso();
        }

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Redireciona para tela de Alteração de Usuário - Dados de Cadastro
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        private void AlterarUsuario(Int32 codigoIdUsuario)
        {
            //Redireciona para página inicial de edição de usuário
            RedirecionarPasso(codigoIdUsuario, "UsuariosEdicaoDadosCadastro.aspx", null);
        }

        /// <summary>
        /// Redireciona para a tela de Alteração de Permissões do Usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        private void AlterarPermissoesUsuario(Int32 codigoIdUsuario)
        {
            //Redireciona para página de alteração de permissões do usuário
            RedirecionarPasso(codigoIdUsuario, "UsuariosEdicaoPermissoes.aspx", null);
        }

        /// <summary>
        /// Redireciona para um passo específico ou tela de Usuários
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <param name="pagina">Página do submódulo Usuários (a página deve herdar de UsuariosUserControlBase)</param>
        /// <param name="qs">QueryString para passagem adicional de informações</param>
        private void RedirecionarPasso(Int32 codigoIdUsuario, String pagina, QueryStringSegura qs)
        {
            Usuario usuario = this.ConsultarUsuario(codigoIdUsuario);
            UsuarioServico.Menu[] permissoes = this.ConsultarPermissoes(codigoIdUsuario);
            EntidadeServico.Entidade1[] estabelecimentos = this.ConsultarEstabelecimentosUsuario(codigoIdUsuario);

            if (usuario != null)
            {
                //Recupera os dados do usuário, e mantém uma cópia dos dados
                //para verificação na etapa de confirmação da alteração/inclusão caso necessário
                this.UsuarioSelecionado = this.Converter(usuario, permissoes, estabelecimentos);
                this.UsuarioSelecionadoOriginal = this.Converter(usuario, permissoes, estabelecimentos);

                //Redireciona para página
                base.RedirecionarPasso(pagina, qs);
            }
        }

        /// <summary>
        /// Desbloqueia o usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        private void DesbloquearUsuario(Int32 codigoIdUsuario)
        {
            Usuario usuario = ConsultarUsuario(codigoIdUsuario);
            var mensagemPainel = default(String);
            var tituloPainel = "Desbloqueio realizado com sucesso.";
            var sucesso = default(Boolean);

//#if DEBUG
//            sucesso = true;
//            if (DateTime.Now.Minute == 50)
//            {
//                usuario = new Usuario
//                {
//                    Status = new Status
//                    {
//                        Codigo = (Int32)Status1.UsuarioBloqueadoSenhaInvalida
//                    }
//                };
//            }
//            else if (DateTime.Now.Minute == 51)
//            {
//                usuario = new Usuario
//                {
//                    Status = new Status
//                    {
//                        Codigo = (Int32)Status1.UsuarioBloqueadoRecuperacaoUsuario
//                    }
//                };
//            }
//            else
//            {
//                usuario = new Usuario
//                {
//                    Status = new Status
//                    {
//                        Codigo = (Int32)Status1.UsuarioAtivoLiberacaoAcessoCompletoBloqueada
//                    }
//                };
//            }
//#endif

            //Usuários legados não são desbloqueáveis
            if (usuario != null && usuario.Status.Codigo.HasValue && !usuario.Legado)
            {
                Status1 status = (Status1)usuario.Status.Codigo.Value;
                switch (status)
                {
                    /*  Bloqueado por senha, para usuários bloqueados por erro de senhas. 
                        O sistema deve desbloquear o usuário e enviar um e-mail com um link para cadastro de nova senha, 
                        nos mesmos moldes da recuperação de senha: primeiro enviando o e-mail da imagem 32 e ao clicar no 
                        link o usuário será direcionado para a mesma tela do item 5.3.3.(não terá confirmação positiva). 
                        Ao realizar o desbloqueio, deve exibir a tela da imagem 52a, e ao clicar no botão "Voltar" o 
                        usuário será direcionado para a tela da imagem 45 e o status do usuário deve ser alterado para 
                        aguardando confirmação de e-mail. */
                    case Status1.UsuarioBloqueadoSenhaInvalida:
                    /*  Bloqueado por recuperação de senha, para usuários bloqueado (6 tentativas excedidas) por erro na 
                        recuperação de senha. O sistema deve desbloquear o formulário de recuperação de senha para este 
                        usuário e ao mesmo o sistema deve desbloquear o usuário e enviar um e-mail com um link para cadastro 
                        de nova senha, nos mesmos moldes da recuperação de senha: primeiro enviando o e-mail da imagem 32 e 
                        ao clicar no link o usuário será direcionado para a mesma tela do item 5.3.3.(não terá confirmação positiva). 
                        Ao realizar o desbloqueio, deve exibir a tela da imagem 52a, e ao clicar no botão "Voltar" o usuário será 
                        direcionado para a tela da imagem 45 e o status do usuário deve ser alterado para aguardando confirmação 
                        de e-mail. */
                    case Status1.UsuarioBloqueadoRecuperacaoSenha:
                    case Status1.UsuarioBloqueadoAguardandoConfirmacaoRecuperacaoSenha:
                        {
                            Guid? hash = AtualizarStatusUsuario(codigoIdUsuario,
                                Status1.UsuarioBloqueadoAguardandoConfirmacaoRecuperacaoSenha, out sucesso);
                            EmailNovoAcesso.EnviarEmailRecuperacaoSenha(usuario.Email, codigoIdUsuario, hash.Value,
                                FormaRecuperacao.EmailPrincipal, SessaoAtual.CodigoIdUsuario, SessaoAtual.Email,
                                SessaoAtual.NomeUsuario, SessaoAtual.TipoUsuario, SessaoAtual.CodigoEntidade,
                                SessaoAtual.Funcional);

                            tituloPainel = "solicitação de desbloqueio enviada com sucesso.";
                            mensagemPainel = String.Concat(
                                "dentro de instantes o usuário bloqueado receberá um e-mail de confirmação para cadastrar uma nova senha. ",
                                "o usuário deverá acessar o link informado no e-mail em até 48h.<br/><br/>",
                                "<span class=\"bold\">importante:</span> O status deste usuário continuará bloqueado até o cadastramento da nova senha.");
                        }
                        break;

                    case Status1.UsuarioBloqueadoRecuperacaoUsuario:
                        /* Bloqueado por recuperação de usuário, para usuários bloqueado (6 tentativas excedidas) por erro na 
                           recuperação de usuário. O sistema deve desbloquear o formulário de recuperação de usuário para este 
                           usuário. Caso não consiga recuperar o usuário via formulário, o Master poderá excluí-lo. Ao realizar o 
                           desbloqueio, deve exibir a tela da imagem 52b, e ao clicar no botão "Voltar" o usuário será direcionado 
                           para a tela da imagem 45 e o status do usuário deve ser alterado para ativo. */
                        {
#if !DEBUG
                            AtualizarStatusUsuario(codigoIdUsuario, Status1.UsuarioAtivo, out sucesso);
#endif
                            tituloPainel = "solicitação de desbloqueio realizada com sucesso.";
                            mensagemPainel = String.Concat(
                                "<span>o usuário poderá:</span>",
                                "<ul class=\"box-list-rede\">",
                                "<li>Acessar o item Recuperação de Usuário novamente em  <a class=\"link-rede\" href=\"http://www.userede.com.br/\">userede.com.br</a></li>",
                                "<li>Você pode alterar o e-mail do usuário em Criação e alteração de usuário do site</li>",
                                "<li>Caso o usuário se lembre do e-mail utilizado, poderá acessar o Portal Rede normalmente</li>",
                                "</ul>");
                        }
                        break;

                    case Status1.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:
                        /* Bloqueado na confirmação positiva (liberação de acesso completo – 1° usuário), para quando usuário não 
                           conseguir realizar o processo de liberação de acesso completo. O sistema deve desbloquear o formulário 
                           de liberação de acesso completo para este usuário. Ao realizar o desbloqueio, deve exibir a tela da 
                           imagem 52c, e ao clicar no botão "Voltar" o usuário será direcionado para a tela da imagem 45 e o status 
                           do usuário deve ser alterado para ativo. */
                        {
#if !DEBUG
                            AtualizarStatusUsuario(codigoIdUsuario, Status1.RespostaIdPosPendente, out sucesso);
#endif
                            mensagemPainel = String.Concat(
                                "o usuário poderá acessar novamente o portal Rede e realizar a liberação<br/>",
                                "de acesso completo na opção Meu Usuário ou você pode alterar as<br/>",
                                "permissões de serviços em criação e alteração de usuário do site.");
                        }
                        break;
                    default:
                        Logger.GravarErro("Desbloqueio não realizado: status inválido", new Exception(), usuario);
                        break;
                }
            }

            if (sucesso)
            {
                //Armazena no histórico/log de atividades
#if !DEBUG
                Historico.DesbloqueioUsuario(SessaoAtual, usuario.CodigoIdUsuario,
                    usuario.Descricao, usuario.Email, usuario.TipoUsuario);
#endif

                if (!String.IsNullOrEmpty(mensagemPainel))
                {
                    ltrQuadroTitulo.Text = tituloPainel;
                    lblQuadroHtml.Text = mensagemPainel;
                    mvwUsuarios.SetActiveView(pnlAviso);
                }
            }
        }

        /// <summary>
        /// Converte para modelo de dados utilizado nas telas de Criação/Edição
        /// de usuário
        /// </summary>
        private DadosUsuario Converter(Usuario usuario, UsuarioServico.Menu[] servicosUsuario,
            EntidadeServico.Entidade1[] estabelecimentos)
        {
            var dadosUsuario = new DadosUsuario();
            dadosUsuario.CodigoIdUsuario = usuario.CodigoIdUsuario;
            dadosUsuario.Nome = usuario.Descricao;
            dadosUsuario.Email = usuario.Email;
            dadosUsuario.EmailTemporario = usuario.EmailTemporario;
            dadosUsuario.EmailSecundario = usuario.EmailSecundario;
            dadosUsuario.CelularDdd = usuario.DDDCelular;
            dadosUsuario.CelularNumero = usuario.Celular;
            dadosUsuario.Cpf = usuario.CPF;
            dadosUsuario.Senha = usuario.Senha;
            dadosUsuario.Estabelecimentos = estabelecimentos.Select(pv => pv.Codigo).ToList();
            if (servicosUsuario != null)
                dadosUsuario.Servicos = servicosUsuario.Select(servico => servico.Codigo).ToList();
            dadosUsuario.TipoUsuario = usuario.TipoUsuario;
            dadosUsuario.Status = usuario.Status;
            dadosUsuario.Origem = usuario.Origem;
            dadosUsuario.Legado = usuario.Legado;

            return dadosUsuario;
        }

        /// <summary>
        /// Obtém a descrição do perfil, de acordo com o TipoUsuario.
        /// </summary>
        private static String ObterDescricaoPerfil(String tipoUsuario)
        {
            tipoUsuario = (tipoUsuario ?? String.Empty).Trim();

            if (String.Compare("M", tipoUsuario, true) == 0)
                return "completo (master)";
            else if (String.Compare("P", tipoUsuario, true) == 0)
                return "personalizado";
            else if (String.Compare("B", tipoUsuario, true) == 0)
                return "básico";
            else
                return tipoUsuario;
        }

        /// <summary>
        /// Obtém a descrição resumida do status do usuário
        /// </summary>
        private static String ObterDescricaoStatus(Status statusUsuario)
        {
            if (statusUsuario.Codigo.HasValue)
            {
                switch ((Status1)statusUsuario.Codigo.Value)
                {
                    case Status1.UsuarioAguardandoConfirmacaoAlteracaoEmail:
                    case Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                        return "aguardando confirmação";
                    case Status1.UsuarioAguardandoConfirmacaoMaster:
                        return "aguardando aprovação";
                    case Status1.UsuarioAtivo:
                    case Status1.UsuarioAtivoAguardandoConfirmacaoRecuperacaoSenha:
                        return "ativo";
                    case Status1.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:
                        return "ativo com acesso completo bloqueado";
                    case Status1.UsuarioBloqueadoAguardandoConfirmacaoRecuperacaoSenha:
                    case Status1.UsuarioBloqueadoRecuperacaoSenha:
                    case Status1.UsuarioBloqueadoRecuperacaoUsuario:
                    case Status1.UsuarioBloqueadoSenhaInvalida:
                        return "bloqueado";
                    case Status1.RespostaIdPosPendente:
                        return "ativo";
                    default:
                        return statusUsuario.Descricao;
                }
            }
            else
                return String.Empty;
        }

        /// <summary>
        /// Gera conteúdo CSV para Exportação/Download
        /// </summary>
        public String GerarConteudoExportacao()
        {
            //Lista de colunas do CSV
            List<String> colunas = new[] { "Nome do Usuário", "Email", "CPF", "Perfil", "Status" }.ToList();

            //Função para conversão de um registro Usuario em List<String>
            //Deve aplicar o mesmo padrão de formatação utilizado na montagem da grid
            Func<Usuario, List<String>> dadosLinha = (usuario) =>
            {
                return new[] {
                    usuario.Descricao,
                    usuario.Email,
                    CampoCpf.AplicarMascara(usuario.CPF),
                    ObterDescricaoPerfil(usuario.TipoUsuario),
                    ObterDescricaoStatus(usuario.Status),
                }.ToList();
            };

            //Gera CSV, com TAB como delimitador
            return CSVExporter.GerarCSV(this.Usuarios, colunas, dadosLinha, "\t");
        }

        /// <summary>
        /// Busca os usuários.
        /// </summary>
        private void BuscarUsuarios()
        {
            //Visão de listagem de usuário
            mvwUsuarios.SetActiveView(pnlUsuarios);

            //Limpa as linhas marcadas
            grvUsuarios.Marcados.Clear();

            //Volta grid para primeira página
            grvUsuarios.PageIndex = 0;

            //Recarrega a pesquisa dos usuários
            this.CarregarUsuarios();
        }

        /// <summary>
        /// Recarrega os usuários
        /// </summary>
        private void CarregarUsuarios()
        {
            if (this.Usuarios == null || this.Usuarios.Count == 0)
            {
                //Recupera parâmetro de filtro da busca
                String busca = txtBusca.Text.Trim().ToUpper();
                this.Usuarios = this.ConsultarUsuarios(busca).ToList();

//#if DEBUG
//                Random random = new Random();
//                var mockUsers = this.Usuarios.ToList();
//                for (int i = 0; i < 300; i++)
//                {
//                    mockUsers.Add(new Usuario
//                    {
//                        Descricao = "nome usuario " + i,
//                        Codigo = i.ToString(),
//                        CodigoIdUsuario = i,
//                        Email = "email " + random.Next(100),
//                        CPF = 12345678902,
//                        TipoUsuario = i == 1 ? "B" : i % 2 == 0 ? "M" : "P",
//                        Status = new Status
//                        {
//                            Codigo = i == 1 ? 3 : i == 2 ? 1 : i == 3 ? 100 : i == 4 ? 101 : random.Next(1, 11),
//                            Descricao = "status "
//                        },
//                        Legado = i == 1

//                    });
//                }
//                this.Usuarios = mockUsers;

//                if (!string.IsNullOrWhiteSpace(busca))
//                {
//                    this.Usuarios = this.Usuarios.Where(u => u.Descricao.StartsWith(busca, true, new System.Globalization.CultureInfo("pt-BR"))).ToList();
//                }
//#endif

                this.Usuarios.ForEach(u => u.Status.Descricao = ObterDescricaoStatus(u.Status));

                if (ddlStatus.Items.Count > 0 && ddlStatus.SelectedIndex != 0)
                {
                    this.Usuarios = this.Usuarios.Where(u => String.Compare(u.Status.Descricao, ddlStatus.SelectedValue, true) == 0).ToList();
                }
            }

            this.Usuarios = OrdernarUsuarios(this.Usuarios);

            //Central de Atendimento não pode excluir usuários
            if (this.CentralAtendimento)
                grvUsuarios.ExibirSelecionarTodos = false;

            //Bind dos usuários na grid
            grvUsuarios.DataSource = this.Usuarios;
            grvUsuarios.DataBind();

            //Atualiza mensagem de quantidade de usuários encontrados/exibidos
            Int32 totalRegistros = this.Usuarios.Count;
            Int32 registroInicial = (grvUsuarios.PageIndex) * grvUsuarios.PageSize + Math.Min(1, totalRegistros);
            Int32 registroFinal = (1 + grvUsuarios.PageIndex) * grvUsuarios.PageSize;
            Boolean possuiRegistros = totalRegistros > 0;

            if (totalRegistros < registroFinal)
                registroFinal = totalRegistros;
            if (possuiRegistros)
                lblUsuariosEncontrados.Text = String.Format("Exibindo usuários {0} a {1} de {2} encontrado{3}",
                    registroInicial, registroFinal, totalRegistros, totalRegistros > 1 ? "s" : String.Empty);
            else
                lblUsuariosEncontrados.Text = "Nenhum usuário encontrado";


            //Central de Atendimento não pode Excluir nem Incluir usuário
            lnkBtnIncluirUsuario.Visible = !this.CentralAtendimento;
            lnkBtnExcluirUsuario.Visible = possuiRegistros && !this.CentralAtendimento;
        }

        /// <summary>
        /// Trata as solicitações de Acesso/Criação de Usuário para a Entidade estão bloqueadas no Portal Aberto
        /// </summary>
        private void VerificarCriacaoAcessoBloqueada()
        {
            using (Logger log = Logger.IniciarLog("Verificar se as solicitações de Acesso/Criação de Usuário para a Entidade estão bloqueadas no Portal Aberto"))
            {
                try
                {
                    using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        Int32 codigoRetornoPn = 0;
                        Int32 codigoRetornoGe = 0;
                        Int32 grupoEntidade = 1; // fixo para Estabelecimentos

                        var entidades = contextoEntidade.Cliente.Consultar(
                            out codigoRetornoPn, 
                            out codigoRetornoGe, 
                            SessaoAtual.CodigoEntidade, 
                            grupoEntidade);

                        if (entidades.Length > 0)
                        {
                            log.GravarMensagem("Status PN da Entidade", new { entidades[0].StatusPN });

                            if (entidades[0].StatusPN.Codigo == (Int32)Comum.Enumerador.Status.EntidadeBloqueadaConfirmacaoPositiva)
                            {
                                if (!String.IsNullOrEmpty(Request.QueryString["dados"]))
                                {
                                    QueryStringSegura qs = new QueryStringSegura(Request.QueryString["dados"]);
                                    if (qs["DesbloqueioDiretoEntidade"].ToBoolNull().Value)
                                    {
                                        // desbloqueio direto, a partir de parâmetro na URL (ex: vindo da home)
                                        if (this.DesbloquearEntidadeCriacaoAcesso())
                                            this.ExibirMensagemDesbloqueioCriacaoAcesso(qs["DesbloqueioDiretoEntidadeUrlBack"]);

                                        return;
                                    }
                                }

                                // exibe a mensagem ao usuário
                                String mensagem = @"
Identificamos que um usuário com e-mail {0} realizou uma tentativa de cadastrado 
de usuário para tentar acessar o Portal de Serviços Rede com o seu Estabelecimento.<br />
Desta forma, o formulário disponível em userede.com.br/pt-br/novoacesso foi bloqueado.";
                                ltrMsgModalDesbloqueioEntidade.Text = String.Format(mensagem, entidades[0].NomeResponsavel);

                                Page.ClientScript.RegisterStartupScript(
                                    Page.GetType(),
                                    "CriacaoAcessoBloqueada",
                                    "DesbloqueioEntidade.ExibirMensagem();",
                                    true);
                            }
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
        }

        /// <summary>
        /// Efetua o desbloqueio da entidade bloqueada na criação de usuário na área aberta
        /// </summary>
        /// <returns>TRUE: sucesso ao efetuar o desbloqueio da entidade</returns>
        private Boolean DesbloquearEntidadeCriacaoAcesso()
        {
            using (Logger log = Logger.IniciarLog("Desbloqueio da Criação de usuários para a Entidade"))
            {
                try
                {
                    using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        Int32 codigoRetorno = 0;
                        Int32 grupoEntidade = 1; // fixo para Estabelecimentos

                        // atualiza status da entidade para "Ativa"
                        codigoRetorno = contextoEntidade.Cliente.AtualizarStatusAcesso(
                            grupoEntidade,
                            SessaoAtual.CodigoEntidade,
                            EntidadeServico.Status2.EntidadeAtiva,
                            SessaoAtual.Email);

                        if (codigoRetorno > 0)
                        {
                            base.ExibirPainelExcecao("EntidadeServico.AtualizarStatusAcesso", codigoRetorno);
                            return false;
                        }

                        // reinicia contador de erros de confirmação positiva da entidade
                        codigoRetorno = contextoEntidade.Cliente.ReiniciarQuantidadeConfirmacaoPositiva(
                            SessaoAtual.CodigoEntidade,
                            grupoEntidade);

                        if (codigoRetorno > 0)
                        {
                            base.ExibirPainelExcecao("EntidadeServico.AtualizarStatusAcesso", codigoRetorno);
                            return false;
                        }

                        // registra no histórico/log de atividades
                        Historico.DesbloqueioFormularioSolicitacaoAcesso(SessaoAtual);

                        return true;
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

                return false;
            }
        }

        /// <summary>
        /// Exibe a mensagem após o desbloqueio da entidade bloqueada no fluxo de criação de usuário
        /// </summary>
        /// <param name="urlRedirect">
        ///     Opcional: URL de redirecionamento para a tela que ficará responsável por exibir a mensagem ao usuário
        /// </param>
        private void ExibirMensagemDesbloqueioCriacaoAcesso(String urlRedirect = null)
        {
            // retorna para a URL de origem (ou fornecida)
            if (!String.IsNullOrEmpty(urlRedirect))
            {
                Session["DesbloqueioDiretoEntidadeSucesso"] = true;
                Response.Redirect(urlRedirect, false);
                return;
            }

            // exibe a confirmação ao usuário
            String mensagem = "Desbloqueio efetuado com sucesso";
            String script = String.Format("DesbloqueioEntidade.ExibirSucesso('{0}');", mensagem);
            Page.ClientScript.RegisterStartupScript(Page.GetType(), "CriacaoAcessoBloqueada", script, true);
        }

        #endregion

        #region [ Chamadas Serviços ]

        /// <summary>
        /// Consulta as permissões do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código id do usuário</param>
        private UsuarioServico.Menu[] ConsultarPermissoes(Int32 codigoIdUsuario)
        {
            var permissoes = default(UsuarioServico.Menu[]);

            using (Logger log = Logger.IniciarLog("Consultando permissões do usuário"))
            {
                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        permissoes = ctx.Cliente.ConsultarMenu(String.Empty,
                            1, //fixo: sempre deve ser Estabelecimento
                            base.SessaoAtual.CodigoEntidade,
                            codigoIdUsuario);

                    Func<UsuarioServico.Menu, UsuarioServico.Menu[]> RecuperarServicos = null;
                    RecuperarServicos = (servico) =>
                    {
                        var retorno = new List<UsuarioServico.Menu>();
                        retorno.Add(servico);
                        retorno.AddRange(servico.Items.SelectMany(s => RecuperarServicos(s)));
                        return retorno.ToArray();
                    };

                    return permissoes.SelectMany(servico => RecuperarServicos(servico)).ToArray();
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return permissoes;
        }

        /// <summary>
        /// Consulta os Estabelecimentos do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código id do usuário</param>
        private EntidadeServico.Entidade1[] ConsultarEstabelecimentosUsuario(Int32 codigoIdUsuario)
        {
            var estabelecimentos = new EntidadeServico.Entidade1[0];

            using (Logger log = Logger.IniciarLog("Consultando permissões do usuário"))
            {
                try
                {
                    Int32 codigoRetorno = default(Int32);
                    using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        estabelecimentos = ctx.Cliente.ConsultarPorUsuario(out codigoRetorno, codigoIdUsuario);

                    // Caso o código de retorno seja != de 0, ocorreu um erro
                    if (codigoRetorno > 0)
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarPorUsuario", codigoRetorno);
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
            return estabelecimentos;
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
                    SharePointUlsLog.LogErro(ex);
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

        /// <summary>
        /// Consulta de todos os Usuários do Estabelecimento
        /// </summary>
        /// <param name="filtroBusca">Parâmetro de filtro da busca</param>
        private Usuario[] ConsultarUsuarios(String filtroBusca)
        {
            var usuarios = new Usuario[0];

            using (Logger log = Logger.IniciarLog("Consultando usuários do Estabelecimento"))
            {
                try
                {
                    var codigoRetorno = default(Int32);
                    var entidade = new Entidade
                    {
                        Codigo = SessaoAtual.CodigoEntidade,
                        GrupoEntidade = new GrupoEntidade { Codigo = 1 } //fixo: sempre deve ser Estabelecimento
                    };

                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        usuarios = ctx.Cliente.ConsultarPorEntidade(out codigoRetorno, entidade);

                    // Caso o código de retorno seja != de 0, ocorreu um erro
                    if (codigoRetorno > 0)
                        base.ExibirPainelExcecao("UsuarioServico.ConsultarPorEntidade", codigoRetorno);

                    if (usuarios != null && usuarios.Length > 0)
                    {
                        //Aplica filtro
                        usuarios = usuarios
                            .Where(usuario =>
                            {
                                return
                String.IsNullOrEmpty(filtroBusca) ||
                (usuario.Email ?? String.Empty).ToUpper().Contains(filtroBusca) ||
                (usuario.Descricao ?? String.Empty).ToUpper().Contains(filtroBusca);
                            })
                            .OrderBy(usuario => usuario.Descricao).ToArray();

                        //Hard-Coded: remove usuários de Código "operacional" e o próprio usuário
                        usuarios = usuarios.Where(usuario =>
                            String.Compare("operacional", usuario.Codigo, true) != 0
                            && usuario.CodigoIdUsuario != this.SessaoAtual.CodigoIdUsuario)
                            .ToArray();
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return usuarios;
        }

        /// <summary>
        /// Exclusão dos usuários
        /// </summary>
        /// <param name="codigosIdUsuarios">Códigos ID dos Usuários</param>
        private void ExcluirUsuarios(List<Int32> codigosIdUsuarios)
        {
            //Prepara para chamada do serviço, convertendo lista de código em string
            String codigos = String.Join("|", codigosIdUsuarios.Select(codigo => codigo.ToString()).ToArray());

            Int32 codRetorno = default(Int32);

            using (Logger log = Logger.IniciarLog("Exclusão de Usuários"))
            {
                try
                {
                    //Recupera os usuários que serão excluídos
                    List<Usuario> usuariosExcluir = new List<Usuario>();
                    foreach (Int32 codigoIdUsuario in codigosIdUsuarios)
                    {
                        Usuario usuario = this.ConsultarUsuario(codigoIdUsuario);
                        if (usuario != null)
                            usuariosExcluir.Add(usuario);
                    }

                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        codRetorno = ctx.Cliente.ExcluirEmLoteNovoAcesso(codigos);

                    // Caso seja maior que zero ocorreu um erro de negócio
                    if (codRetorno > 0)
                        base.ExibirPainelExcecao("UsuarioServico.ExcluirEmLote", codRetorno);
                    else
                    {
                        //Armazena no histórico/log de atividade a exclusão de cada usuário
                        foreach (Usuario usuarioExcluir in usuariosExcluir)
                            Historico.ExclusaoUsuario(SessaoAtual, usuarioExcluir.CodigoIdUsuario,
                                usuarioExcluir.Descricao, usuarioExcluir.Email, usuarioExcluir.TipoUsuario);

                        ltrQuadroTitulo.Text = "usuários excluídos com sucesso.";
                        lblQuadroHtml.Text = String.Empty;
                        mvwUsuarios.SetActiveView(pnlAviso);
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
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
        /// Atualização do status do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Códigos ID dos Usuários</param>
        /// <param name="status">Status</param>
        /// <param name="sucesso">Resultado da operação</param>
        private Guid? AtualizarStatusUsuario(Int32 codigoIdUsuario, Status1 status, out Boolean sucesso)
        {
            var codRetorno = default(Int32);
            var hashConfirmacao = default(Guid);
            sucesso = false;

            using (Logger log = Logger.IniciarLog("Atualização de status de Usuário"))
            {
                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                    {
                        codRetorno = ctx.Cliente.AtualizarStatus(out hashConfirmacao, codigoIdUsuario, status, 2, DateTime.Now);

                        //Sempre que o status  for ativo, irá resetar a quantidade de confirmação positivas inválidas
                        //Pois ocorre para Bloqueados por Confirmação Positiva na Libereção de Acesso Completo e Recuperação de Usuário
                        if ((status == Status1.UsuarioAtivo || status == Status1.RespostaIdPosPendente) && codRetorno == 0)
                        {
                            codRetorno = ctx.Cliente.ReiniciarQuantidadeConfirmacaoPositiva(codigoIdUsuario);
                        }

                    }

                    // Caso seja maior que zero ocorreu um erro de negócio
                    if (codRetorno > 0)
                    {
                        base.ExibirPainelExcecao("UsuarioServico.AtualizarStatus", codRetorno);
                    }
                    else
                    {
                        sucesso = true;
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return hashConfirmacao;
        }


        #endregion

        protected void grvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("administracao de usuarios - grvUsuarios_RowCommand"))
            {
                try
                {
                    if (e.CommandSource.GetType() == typeof(ImageButton))
                    {
                        ImageButton acionador = (ImageButton)e.CommandSource;
                        if (String.Compare(acionador.ID, "imbUsuarioOrdenacao", true) == 0 ||
                            String.Compare(acionador.ID, "imbEmailOrdenacao", true) == 0 ||
                            String.Compare(acionador.ID, "imbCPFOrdenacao", true) == 0 ||
                            String.Compare(acionador.ID, "imbPerfilOrdenacao", true) == 0 ||
                            String.Compare(acionador.ID, "imbStatusOrdenacao", true) == 0
                            )
                        {

                            if (String.Compare(e.CommandName, this.OrdenacaoColuna, true) == 0)
                            {
                                if (String.Compare(this.OrdenacaoDirecao, "ASC", true) == 0)
                                    this.OrdenacaoDirecao = "DESC";
                                else
                                    this.OrdenacaoDirecao = "ASC";
                            }
                            else
                            {
                                this.OrdenacaoColuna = e.CommandName;
                                this.OrdenacaoDirecao = "ASC";
                            }

                            this.BuscarUsuarios();
                        }
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
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

        private List<Usuario> OrdernarUsuarios(List<Usuario> usuarios)
        {
            List<Usuario> retorno = null;
            if (String.Compare(this.OrdenacaoColuna, "Status", true) == 0)
            {
                if (String.Compare(this.OrdenacaoDirecao, "ASC", true) == 0)
                    retorno = usuarios.OrderBy(u => u.Status.Descricao).ToList();
                else
                    retorno = usuarios.OrderByDescending(u => u.Status.Descricao).ToList();
            }
            else if (!String.IsNullOrWhiteSpace(this.OrdenacaoColuna))
            {
                if (String.Compare(this.OrdenacaoDirecao, "ASC", true) == 0)
                    retorno = usuarios.OrderBy(this.OrdenacaoColuna).ToList();
                else
                    retorno = usuarios.OrderByDescending(this.OrdenacaoColuna).ToList();
            }
            else
            {
                retorno = usuarios;
            }

            return retorno;
        }

        /// <summary>
        /// Inicia o processo de aprovação do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">ID do Usuário</param>
        private void AprovarAcesso(Int32 codigoIdUsuario)
        {
            using (Logger log = Logger.IniciarLog("Inicia o processo de aprovação do usuário"))
            {
                try
                {
                    UsuarioServico.Usuario usuario = this.ConsultarUsuario(codigoIdUsuario);
                    UsuarioServico.Menu[] permissoes = this.ConsultarPermissoes(codigoIdUsuario);
                    EntidadeServico.Entidade1[] estabelecimentos = this.ConsultarEstabelecimentosUsuario(codigoIdUsuario);

                    if (usuario != null)
                    {
                        //Recupera os dados do usuário, e mantém uma cópia dos dados
                        //para verificação na etapa de confirmação da alteração/inclusão
                        this.UsuarioSelecionado = this.Converter(usuario, permissoes, estabelecimentos);
                        this.UsuarioSelecionadoOriginal = this.Converter(usuario, permissoes, estabelecimentos);

                        //Redireciona para página inicial de edição de usuário
                        base.RedirecionarPasso("AprovacaoSolicitacaoEstabelecimentos.aspx", null);
                    }
                }
                catch (PortalRedecardException ex)
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
        /// Rejeita a solicitação de acesso de um Usuário
        /// </summary>
        /// <param name="codigoIdUsuario">ID do Usuário</param>
        private void RejeitarAcesso(Int32 codigoIdUsuario)
        {
            using (Logger log = Logger.IniciarLog("Aprova/Rejeita a Solicitação de Acesso do Usuário"))
            {
                try
                {
                    UsuarioServico.Usuario usuario = this.ConsultarUsuario(codigoIdUsuario);
                    UsuarioServico.Menu[] permissoes = this.ConsultarPermissoes(codigoIdUsuario);
                    EntidadeServico.Entidade1[] estabelecimentos = this.ConsultarEstabelecimentosUsuario(codigoIdUsuario);

                    if (usuario != null)
                    {
                        //Recupera os dados do usuário, e mantém uma cópia dos dados
                        //para verificação na etapa de confirmação da alteração/inclusão
                        this.UsuarioSelecionado = this.Converter(usuario, permissoes, estabelecimentos);

                        //Redireciona para página inicial de edição de usuário
                        base.RedirecionarPasso("RejeicaoSolicitacao.aspx", null);
                    }
                }
                catch (PortalRedecardException ex)
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

        protected void lnkBtnAcao_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("administracao de usuarios - lnkBtnAcao_Click"))
            {
                try
                {
                    var btnAcao = (LinkButton)sender;

                    //Código ID do Usuário "clicado"
                    Int32 codigoIdUsuario = btnAcao.CommandArgument.ToInt32();

                    //Garante que o próprio usuário logado não esteja marcado para ação
                    if (codigoIdUsuario == this.SessaoAtual.CodigoIdUsuario)
                        return;

                    log.GravarMensagem(btnAcao.CommandName);

                    //Identifica tipo de comando
                    switch (btnAcao.CommandName)
                    {
                        case "editar":
                            this.AlterarUsuario(codigoIdUsuario);
                            break;
                        case "aprovar":
                            this.AprovarAcesso(codigoIdUsuario);
                            break;
                        case "rejeitar":
                            this.RejeitarAcesso(codigoIdUsuario);
                            break;
                        default: break;
                    }
                }
                catch (PortalRedecardException ex)
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

        protected void lnkBtnIncluirUsuario_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("administracao de usuarios - lnkBtnEditarBancos_Click"))
            {
                try
                {
                    //Inicializa objeto modelo que armazenará os dados do novo usuário
                    this.UsuarioSelecionado = new DadosUsuario();

                    //Redireciona para página inicial de Criação de Usuário
                    base.RedirecionarPasso("UsuariosCriacaoDadosCadastro.aspx", null);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
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
    }
}