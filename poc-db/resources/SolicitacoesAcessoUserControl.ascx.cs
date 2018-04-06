using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios.SolicitacoesAcesso
{
    /// <summary>
    /// Solicitações de Aprovação de Criação
    /// </summary>
    public partial class SolicitacoesAcessoUserControl : UsuariosUserControlBase
    {
        #region [Propriedades]
        
        /// <summary>
        /// Não valida se existe usuário selecionado: verificação não se aplica para esta página
        /// </summary>
        public override Boolean ValidarNavegacao { get { return false; } }

        #endregion

        #region [Controles da webpart]

        /// <summary>
        /// qdAviso control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected QuadroAviso QdAviso { get { return (QuadroAviso)qdAviso; } }

        /// <summary>
        /// UcSucesso: quadro de aviso de sucesso.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected QuadroAviso UcSucesso { get { return (QuadroAviso)ucSucesso; } }

        /// <summary>
        /// lbxDesbloqueio control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected LightBox LbxDesbloqueio { get { return (LightBox)lbxDesbloqueio; } }

        /// <summary>
        /// ucQuadroAcessoNegado control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected QuadroConfirmacao UcQuadroAcessoNegado { get { return (QuadroConfirmacao)ucQuadroAcessoNegado; } }

        #endregion

        #region [Eventos da Webpart]

        /// <summary>
        /// Inicialização da webpart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Inicialização da webpart"))
            {
                try
                {
                    //Verifica permissão (apenas Master e Central de Atendimento podem acessar)
                    if (!base.PossuiPermissao)
                    {
                        pnlAcessoNegado.Visible = true;
                        pnlBloqueioSolicitacoes.Visible = false;
                        pnlSolicitacoes.Visible = false;
                        UcQuadroAcessoNegado.CarregarMensagemQuadro();
                        return;
                    }

                    //Botão default da página
                    this.Page.Form.DefaultButton = btnBuscar.UniqueID;

                    if (!Page.IsPostBack)
                    {
                        if (!this.CriacaoAcessoBloqueada())
                        {
                            pnlBloqueioSolicitacoes.Visible = false;
                            pnlSolicitacoes.Visible = true;
                            this.CarregarUsuarios();
                        }
                        lblMensagemBusca.Visible = false;
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Esconde o painel com a mensagem de bloqueio e exibe a listagem de usuários
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDesbloquearDepois_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Esconde o painel com a mensagem de bloqueio e exibe a listagem de usuários"))
            {
                try
                {
                    this.DesbloquearDepois();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Abre o lightbox com a confirmação de Desbloqueio do formulário de criação de usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDesbloquearAgora_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Abre o lightbox com a confirmação de Desbloqueio do formulário de criação de usuário"))
            {
                try
                {
                    pnlBloqueioSolicitacoes.Visible = false;
                    LbxDesbloqueio.Exibir();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Cancela o Desbloqueio da Criação de usuários para a Entidade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNao_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Cancela o Desbloqueio"))
            {
                try
                {
                    if (Request.QueryString["dados"] != null) //Se tiver sido direcionado pela Home Transacional, possui QueryString
                        Response.Redirect(base.RecuperarEnderecoPortalFechado(), false);
                    else
                        this.DesbloquearDepois();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Desbloquear da Criação de usuários para a Entidade
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSim_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Desbloqueio da Criação de usuários para a Entidade"))
            {
                try
                {
                    using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        Int32 codigoRetorno = 0;
                        Int32 grupoEntidade = 1; //Fixo para Estabelecimentos

                        //Atualiza status da entidade para Ativa
                        codigoRetorno = contextoEntidade.Cliente
                            .AtualizarStatusAcesso(
                                grupoEntidade,     
                                SessaoAtual.CodigoEntidade,                                
                                EntidadeServico.Status2.EntidadeAtiva, 
                                SessaoAtual.Email);

                        if (codigoRetorno > 0)
                        {
                            base.ExibirPainelExcecao("EntidadeServico.AtualizarStatusAcesso", codigoRetorno);
                        }
                        else
                        {
                            //Reinicia contador de erros de confirmação positiva da entidade
                            codigoRetorno = contextoEntidade.Cliente.ReiniciarQuantidadeConfirmacaoPositiva(
                                SessaoAtual.CodigoEntidade, grupoEntidade);

                            if (codigoRetorno > 0)
                            {
                                base.ExibirPainelExcecao("EntidadeServico.AtualizarStatusAcesso", codigoRetorno);
                            }
                            else
                            {
                                //Registra no histórico/log de atividades
                                Historico.DesbloqueioFormularioSolicitacaoAcesso(SessaoAtual);

                                pnlBloqueioSolicitacoes.Visible = false;
                                pnlSolicitacoes.Visible = false;
                                pnlSucesso.Visible = true;

                                String urlVoltar = String.Empty ;

                                if (Request.QueryString["dados"] == null)
                                    urlVoltar = Request.Url.AbsoluteUri;
                                else
                                    urlVoltar = base.RecuperarEnderecoPortalFechado();

                                UcSucesso.CarregarMensagem("Desbloqueio efetuado com sucesso com sucesso!", "", urlVoltar, QuadroAviso.IconeMensagem.Confirmacao);
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
        /// Troca de página a listagem de Solicitações
        /// </summary>
        protected void grvUsuarios_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Troca de página a listagem de Solicitações"))
            {
                try
                {
                    log.GravarMensagem("Nova Página", new { e.NewPageIndex });
                    grvUsuarios.PageIndex = e.NewPageIndex;
                    CarregarUsuarios();
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Aprova/Rejeita a Solicitação de Acesso do Usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAprovarRejeitar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Aprova/Rejeita a Solicitação de Acesso do Usuário"))
            {
                try
                {
                    var btnAcao = (Button)sender;

                    //Código ID do Usuário "clicado"
                    Int32 codigoIdUsuario = btnAcao.CommandArgument.ToInt32();

                    //Identifica tipo de comando
                    switch (btnAcao.CommandName)
                    {
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
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Filtrar a busca de solicitações de acesso
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Filtrar a busca de solicitações de acesso"))
            {
                try
                {
                    this.CarregarUsuarios();
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
        /// Download da consulta de usuários pendentes de aprovação de solicitação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Download da consulta de usuários pendentes de aprovação de solicitação"))
            {
                try
                {
                    String nomeArquivo = String.Format("Aprovacao de Acessos - {0}.xls",
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
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
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
                var usuario = e.Row.DataItem as EntidadeServico.Usuario;
                var btnAprovar = (Button)e.Row.FindControl("btnAprovar");
                var btnRejeitar = (Button)e.Row.FindControl("btnRejeitar");
                var ltrCpf = (Literal)e.Row.FindControl("ltrCpf");
                var ltrDataSolicitacao = (Literal)e.Row.FindControl("ltrDataSolicitacao");

                //Atribui o Código ID do usuário nos botões
                btnAprovar.CommandArgument = usuario.CodigoIdUsuario.ToString();
                btnRejeitar.CommandArgument = usuario.CodigoIdUsuario.ToString();

                if (SessaoAtual != null && SessaoAtual.UsuarioAtendimento)
                {
                    btnAprovar.Visible = false;
                    btnRejeitar.Visible = false;
                }

                ltrCpf.Text = CampoCpf.AplicarMascara(usuario.CPF);
                ltrDataSolicitacao.Text = usuario.DataInclusao.ToString("dd/MM/yyyy");
            }
        }
        #endregion

        #region [Métodos Auxiliares]

        /// <summary>
        /// Exibe tabela com a solicitações pendentes e esconde as informações sobre bloqueio da Entidade
        /// </summary>
        private void DesbloquearDepois()
        {
            lbxDesbloqueio.Visible = false;
            pnlSolicitacoes.Visible = true;
            pnlBloqueioSolicitacoes.Visible = false;
        }

        /// <summary>
        /// Verificar se as solicitações de Acesso/Criação de Usuário para a Entidade estão bloqueadas no Portal Aberto
        /// </summary>
        /// <returns>
        /// <para>False - Não estão bloquadas</para>
        /// <para>True - Estão bloqueadas</para>
        /// </returns>
        private Boolean CriacaoAcessoBloqueada()
        {
            Boolean bloqueada = false;

            using (Logger log = Logger.IniciarLog("Verificar se as solicitações de Acesso/Criação de Usuário para a Entidade estão bloqueadas no Portal Aberto"))
            {
                try
                {
                    using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        Int32 codigoRetornoPn = 0;
                        Int32 codigoRetornoGe = 0;
                        Int32 grupoEntidade = 1; //Fixo para Estabelecimentos

                        var entidades = contextoEntidade.Cliente.Consultar(
                            out codigoRetornoPn, out codigoRetornoGe, SessaoAtual.CodigoEntidade, grupoEntidade);

                        if (entidades.Length > 0)
                        {
                            log.GravarMensagem("Status PN da Entiade", new { entidades[0].StatusPN });
                            
                            bloqueada = (entidades[0].StatusPN.Codigo == 
                                        (Int32)Comum.Enumerador.Status.EntidadeBloqueadaConfirmacaoPositiva);

                            if (bloqueada)
                            {
                                pnlBloqueioSolicitacoes.Visible = true;
                                pnlSolicitacoes.Visible = false;

                                String mensagem = @"Identificamos que um usuário com e-mail {0} realizou uma tentativa de cadastrado 
                                                    de usuário para tentar acessar o Portal de Serviços Rede com o seu Estabelecimento.<br />
                                                    Desta forma, o formulário disponível em userede.com.br/pt-br/novoacesso foi bloqueado.";
                                mensagem = String.Format(mensagem, entidades[0].NomeResponsavel);

                                QdAviso.CarregarMensagem("Atenção", mensagem, QuadroAviso.IconeMensagem.Aviso);

                                if (Request.QueryString["dados"] != null)
                                {
                                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                                    Boolean exibirConfirmacaoDesbloqueio = queryString["ExibirConfirmacaoDesbloqueio"].ToBoolNull().Value;

                                    if (exibirConfirmacaoDesbloqueio)
                                    {
                                        pnlBloqueioSolicitacoes.Visible = false;
                                        LbxDesbloqueio.Exibir();
                                    }
                                }
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

            return bloqueada;
        }

        /// <summary>
        /// Recarrega os usuários
        /// </summary>
        private void CarregarUsuarios()
        {
            EntidadeServico.Usuario[] usuarios = this.ConsultarUsuariosPendentes();

            //Bind dos usuários na grid
            grvUsuarios.DataSource = usuarios;
            grvUsuarios.DataBind();

            //Atualiza mensagem de quantidade de usuários encontrados/exibidos
            Int32 totalRegistros = usuarios.Length;

            if (totalRegistros > 0)
            {
                Int32 registroInicial = (grvUsuarios.PageIndex) * grvUsuarios.PageSize + 1;

                if (totalRegistros == 0)
                    registroInicial = 0;

                Int32 registroFinal = (1 + grvUsuarios.PageIndex) * grvUsuarios.PageSize;
                if (totalRegistros < registroFinal)
                    registroFinal = totalRegistros;

                ltrUsuariosEncontrados.Visible = true;
                lblMensagemBusca.Visible = false;

                ltrUsuariosEncontrados.Text = String.Format("Exibindo requisições {0} a {1} de {2} encontrada{3}",
                    registroInicial, registroFinal, totalRegistros, totalRegistros > 1 ? "s" : String.Empty);
            }
            else
            {
                ltrUsuariosEncontrados.Text = "Nenhum usuário encontrado";
                ltrUsuariosEncontrados.Visible = false;
                lblMensagemBusca.Visible = true;
            }
        }

        /// <summary>
        /// Consulta os Usuários com Aprovação de Criação Pendente da Entidade
        /// </summary>
        /// <returns></returns>
        private EntidadeServico.Usuario[] ConsultarUsuariosPendentes()
        {
            EntidadeServico.Usuario[] usuarios = null;

            using (Logger log = Logger.IniciarLog("Consulta os Usuários com Aprovação de Criação Pendente da Entidade"))
            {
                try
                {
                    using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        Int32 codigoRetorno = 0;
                        Int32 grupoEntidade = 1; //Fixo para Estabelecimentos

                        usuarios = contextoEntidade.Cliente.ConsultarUsuariosPorStatus(
                            out codigoRetorno,
                            SessaoAtual.CodigoEntidade,
                            grupoEntidade,
                            EntidadeServico.Status2.UsuarioAguardandoConfirmacaoMaster);

                        

                        if (codigoRetorno > 0)
                        {
                            base.ExibirPainelExcecao("EntidadeServico.ConsultarUsuariosPorStatus", codigoRetorno);
                        }
                        else
                        {
                            if ((usuarios.Length > 0) && IsPostBack)
                            {
                                //Filtros
                                String nomeEmail = txtBusca.Text.ToUpper().Trim();
                                
                                usuarios = usuarios
                                    .Where(usuario =>
                                    {
                                        return String.IsNullOrEmpty(nomeEmail) ||
                                        (usuario.Email ?? String.Empty).ToUpper().Contains(nomeEmail) ||
                                        (usuario.Descricao ?? String.Empty).ToUpper().Contains(nomeEmail);
                                    })
                                    .OrderBy(usuario => usuario.Descricao).ToArray();

                                DateTime dataInicio = new DateTime();
                                DateTime dataFinal = new DateTime();

                                if (DateTime.TryParse(txtPeriodoInicial.Text, out dataInicio))
                                {
                                    usuarios = usuarios
                                                .Where(usuario =>
                                                    usuario.DataInclusao >= dataInicio
                                                )
                                                .OrderBy(usuario => usuario.Descricao).ToArray();
                                }

                                if (DateTime.TryParse(txtPeriodoFinal.Text, out dataFinal))
                                {
                                    //Acrescenta o horário final do dia para a DataFinal
                                    dataFinal = dataFinal.AddDays(1).AddTicks(-1);

                                    usuarios = usuarios
                                                .Where(usuario =>
                                                    usuario.DataInclusao <= dataFinal
                                                )
                                                .OrderBy(usuario => usuario.Descricao).ToArray();
                                }
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

            return usuarios;
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
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
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
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Consulta um usuário por ID
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        private UsuarioServico.Usuario ConsultarUsuario(Int32 codigoIdUsuario)
        {
            var usuario = default(UsuarioServico.Usuario);

            using (Logger log = Logger.IniciarLog("Consultando usuário por ID"))
            {
                var codigoRetorno = default(Int32);

                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
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
                    Int32 grupoEntidade = 1; //Fixo para Estabelecimentos
                    using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        permissoes = ctx.Cliente.ConsultarMenu(String.Empty,
                            grupoEntidade,
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
        /// Converte para modelo de dados utilizado nas telas de Edição de usuário
        /// </summary>
        private DadosUsuario Converter(UsuarioServico.Usuario usuario, UsuarioServico.Menu[] servicosUsuario,
            EntidadeServico.Entidade1[] estabelecimentos)
        {
            var dadosUsuario = new DadosUsuario();
            dadosUsuario.CodigoIdUsuario = usuario.CodigoIdUsuario;
            dadosUsuario.Nome = usuario.Descricao;
            dadosUsuario.Email = usuario.Email;
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
            return dadosUsuario;
        }

        /// <summary>
        /// Gera conteúdo CSV para Exportação/Download
        /// </summary>
        public String GerarConteudoExportacao()
        {
            //Consulta todos os usuários
            EntidadeServico.Usuario[] usuarios = this.ConsultarUsuariosPendentes();

            //Lista de colunas do CSV
            List<String> colunas = new[] { "Nome do Usuário", "E-mail", "CPF", "Data da Solicitação" }.ToList();

            //Função para conversão de um registro Usuario em List<String>
            //Deve aplicar o mesmo padrão de formatação utilizado na montagem da grid
            Func<EntidadeServico.Usuario, List<String>> dadosLinha = (usuario) =>
            {
                return new[] { 
                    usuario.Descricao, 
                    usuario.Email, 
                    CampoCpf.AplicarMascara(usuario.CPF),
                    usuario.DataUltimaAtualizacaoSenha.ToString()
                }.ToList();
            };

            //Gera CSV, com TAB como delimitador
            return CSVExporter.GerarCSV(usuarios, colunas, dadosLinha, "\t");
        }

        #endregion
    }
}
