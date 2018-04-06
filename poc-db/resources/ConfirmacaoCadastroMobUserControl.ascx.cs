/*
© Copyright 2015 Rede S.A.
Autor : Felipe Siatiquosque
Empresa : Rede
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Redecard.PN.Comum.SharePoint;
using Rede.PN.DadosCadastraisMobile.SharePoint.CONTROLTEMPLATES.DadosCadastraisMobile;
using Redecard.PN.Comum;
using System.Web;
using System.ServiceModel;
using Microsoft.SharePoint.Administration;
using Rede.PN.DadosCadastraisMobile.SharePoint.Util;
using System.Linq;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.ConfirmacaoCadastroMob
{
    public partial class ConfirmacaoCadastroMobUserControl : UserControlBase
    {
        #region [Atributos da WebPart]

        /// <summary>
        /// Chave da ViewState
        /// </summary>
        private String chaveUsuario = "UsuarioConfirmacao";

        /// <summary>
        /// Atributo com objeto de Usuário
        /// </summary>
        private UsuarioServico.Usuario Usuario
        {
            get
            {
                if (!object.ReferenceEquals(ViewState[chaveUsuario], null))
                {
                    return (UsuarioServico.Usuario)ViewState[chaveUsuario];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                ViewState[chaveUsuario] = value;
            }
        }

        /// <summary>
        /// Chave da ViewState
        /// </summary>
        private String chaveEntidades = "EntidadesUsuario";

        /// <summary>
        /// Atributo com objeto de Entidades do Usuário
        /// </summary>
        private List<Redecard.PN.Comum.SharePoint.EntidadeServico.Entidade1> Entidades
        {
            get
            {
                if (!object.ReferenceEquals(ViewState[chaveEntidades], null))
                {
                    return (List<Redecard.PN.Comum.SharePoint.EntidadeServico.Entidade1>)ViewState[chaveEntidades];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                ViewState[chaveEntidades] = value;
            }
        }

        #endregion

        #region [Controles da Webpart]
        /// <summary>
        /// qdAviso control.
        /// </summary>
        protected QuadroAvisosResponsivo QdAviso { get { return (QuadroAvisosResponsivo)qdAviso; } }

        /// <summary>
        /// txtEstabelecimento control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>

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
                        CarregarDadosUsuario();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
            }
        }

        /// <summary>
        /// Cancela as tentativas de confirmar o cadastro
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sender"></param>
        protected void btnCancelar_Click(Object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Cancela as tentativas de confirmar o cadastro"))
            {
                try
                {
                    Response.Redirect(base.RecuperarEnderecoPortal(), false);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
            }
        }

        /// <summary>
        /// Tentar confirmar o cadastro novamente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnTentarNovamente_Click(Object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Tentar confirmar o cadastro novamente"))
            {
                try
                {
                    pnlQuadroAcesso.Visible = true;
                    pnlQuadroAviso.Visible = false;
                    senhaAcesso.Text = String.Empty;
                    email.Text = String.Empty;

                    if (pv.Enabled)
                        pv.Text = String.Empty;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
            }
        }

        /// <summary>
        /// Validar as informações de acesso preenchidas para acessar o Portal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAcessar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Validar as informações de acesso preenchidas"))
            {
                try
                {

                    Int32 numPdv = Convert.ToInt32(pv.Text);
                    String emailUsuario = email.Text.ToLower().Trim();
                    String senhaCriptografada = EncriptadorSHA1.EncryptString(senhaAcesso.Text);

                    Boolean possuiUsuario = true;
                    Boolean possuiMaster = true;
                    Boolean possuiSenhaTemporaria = true;

                    using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        contextoEntidade.Cliente.PossuiUsuario(out possuiUsuario, out possuiMaster, out possuiSenhaTemporaria, numPdv, 1);

                    if (this.Usuario != null && this.Entidades != null)
                    {
                        Boolean emailCorreto = (this.Usuario.Email.ToLower().Trim().Equals(emailUsuario)
                                                || this.Usuario.EmailTemporario.ToLower().Trim().Equals(emailUsuario));

                        Boolean pvValido = this.Entidades.Find(ent => ent.Codigo == numPdv) != null;

                        Boolean senhaValida = this.Usuario.Senha.Equals(senhaCriptografada);

                        if (emailCorreto && pvValido && senhaValida)
                        {
                            Boolean usuarioOrigemAberta = this.Usuario.Origem == 'A' || this.Usuario.Origem == 'a';
                            Boolean confirmacaoEmailPendente =
                                this.Usuario.Status.Codigo == (Int32)UsuarioServico.Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario;

                            String redirecionarLiberacaoAcesso = !possuiMaster
                                                                && ((confirmacaoEmailPendente && usuarioOrigemAberta)
                                                                     || this.Usuario.Origem.Equals('M'))
                                                                 ? "S" : "N";

                            //Change request: 
                            //Atualiza a flag para exibir mensagem de liberação de acesso completo (quando necessário)
                            if (String.Compare(redirecionarLiberacaoAcesso, "S", true) == 0)
                                AtualizarFlagExibicaoMensagemAcessoCompleto();

                            if (this.AtualizarStatusUsuario())
                            {
                                //Se for Mobile
                                if (Util.Validacoes.IsMobile(Request.ServerVariables["HTTP_USER_AGENT"]))
                                {
                                    /* Redireciona para a página de sucesso */
                                    Response.Redirect(String.Format("{0}/Paginas/Mobile/CriacaoUsrConfConclusao.aspx", base.web.ServerRelativeUrl));
                                }
                                else
                                {
                                    /* Autentica o Usuário e redireciona para o Portal */
                                    this.LoginUsuario(numPdv.ToString(), emailUsuario, senhaAcesso.Text, redirecionarLiberacaoAcesso);
                                }
                            }
                        }
                        else
                        {
                            String mensagem = String.Empty;

                            if (!senhaValida)
                                this.ValidarErroConfirmacao();
                            else if (!emailCorreto)
                            {
                                mensagem = base.RetornarMensagemErro("ConfirmacaoCadatro.ValidarAcesso", 1165);
                                this.ExibirAvisoTentativas(mensagem, "E-mail ou senha incorreto", QuadroAvisosResponsivo.IconeMensagem.Aviso);
                            }
                            //else if (!pvValido)
                            //{
                            //    mensagem = base.RetornarMensagemErro("ConfirmacaoCadatro.ValidarAcesso", 1166);
                            //    this.ExibirAvisoTentativas(mensagem, "Atenção", QuadroAvisosResponsivo.IconeMensagem.Aviso);
                            //}
                            else
                                this.ValidarErroConfirmacao();
                        }
                    }

                }
                catch (FaultException<Redecard.PN.Comum.SharePoint.EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", base.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", base.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
            }
        }

        #endregion

        #region [Métodos auxiliares]

        /// <summary>
        /// Carregar e validar os dados do Usuário
        /// </summary>
        private void CarregarDadosUsuario()
        {
            using (Logger log = Logger.IniciarLog("Carregar e validar os dados do Usuário"))
            {
                try
                {
                    if (!Object.ReferenceEquals(Request.QueryString["dados"], null))
                    {
                        QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);

                        Int32 codigoRetorno = 0;

                        Int32 codigoIdUsuario = queryString["CodigoIdUsuario"].ToString().ToInt32();
                        Guid hashEmail = new Guid(queryString["Hash"].ToString());

                        using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            UsuarioServico.Usuario usuario = contextoUsuario.Cliente.ConsultarPorID(out codigoRetorno, codigoIdUsuario);

                            if (codigoRetorno > 0)
                            {
                                this.ExibirErro("UsuarioServico.ConsultarPorID", codigoRetorno, "Atenção", base.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Aviso);
                                return;
                            }
                            else
                            {
                                using (var contextoEntidade = new ContextoWCF<Redecard.PN.Comum.SharePoint.EntidadeServico.EntidadeServicoClient>())
                                {
                                    //Verificar se o usuário está associado a mais de 1 Entidade
                                    Redecard.PN.Comum.SharePoint.EntidadeServico.Entidade1[] entidades = contextoEntidade.Cliente.ConsultarPorUsuario(out codigoRetorno, usuario.CodigoIdUsuario);

                                    this.Entidades = new List<Redecard.PN.Comum.SharePoint.EntidadeServico.Entidade1>();
                                    this.Entidades.AddRange(entidades);

                                    this.Usuario = usuario;

                                    //Se o usuário estiver associado a mais de 1 entidade, usuário deve informar o número do PV
                                    if (entidades.Length > 1)
                                    {
                                        pv.Text = String.Empty;
                                        pv.Enabled = true;
                                        
                                    }
                                    else //Se o usuário estiver associado a 1 entidade, usuário deve informar o número do PV
                                    {
                                        pv.Text = entidades[0].Codigo.ToString();
                                        pv.Enabled = false;
                                        
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        this.ExibirErro("SharePoint.RequestQueryString", 1152, "Atenção", base.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                    }
                }
                catch (FaultException<Redecard.PN.Comum.SharePoint.EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", this.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", this.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", this.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
            }
        }

        /// <summary>
        /// Exibe o quadro de aviso com a mensagem de erro
        /// </summary>
        /// <param name="fonte">Base fonte da mensagem de erro</param>
        /// <param name="codigo">Código da Mensagem</param>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="urlVoltar">Url que o botão Voltar do Aviso deve redirecionar</param>
        /// <param name="icone">ícone a ser exibido</param>
        private void ExibirErro(String fonte, Int32 codigo, String titulo, String urlVoltar, QuadroAvisosResponsivo.IconeMensagem icone)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);

            if (!String.IsNullOrEmpty(urlVoltar))
            {
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, icone);
            }
            else
            {
                QdAviso.CarregarMensagem(titulo, mensagem, false, icone);
            }

            pnlQuadroAcesso.Visible = false;
            pnlBotaoTentativas.Visible = false;
            pnlQuadroAviso.Visible = true;
        }

        /// <summary>
        /// Valida a quantidade de tentativas que o usuário possui, caso erre os dados de confirmação
        /// </summary>
        private void ValidarErroConfirmacao()
        {
            using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
            {
                Int32 codigoRetorno = 0;

                codigoRetorno = contextoUsuario.Cliente.IncrementarQuantidadeSenhaInvalida(this.Usuario.CodigoIdUsuario);

                if (codigoRetorno == 0)
                {
                    var usuario = contextoUsuario.Cliente.ConsultarPorID(out codigoRetorno, this.Usuario.CodigoIdUsuario);

                    if (usuario.QuantidadeTentativaLoginIncorreta >= 6)
                    {
                        Guid hashEmail = Guid.Empty;
                        codigoRetorno = contextoUsuario.Cliente.ExcluirEmLoteNovoAcesso(this.Usuario.CodigoIdUsuario.ToString());

                        if (codigoRetorno == 0)
                        {
                            if (this.ExcluirHashUsuario())
                            {
                                String urlRecriarUsuario = String.Format("{0}/Paginas/Mobile/CriacaoUsrDadosIniciais.aspx", base.web.ServerRelativeUrl);
                                String mensagem = String.Format("Para acessar o Portal Rede, repita o processo de criação de usuário <a href='{0}'>clicando aqui</a>.", urlRecriarUsuario);

                                this.ExibirErro(mensagem, "Limite de tentativas excedido", String.Empty, QuadroAvisosResponsivo.IconeMensagem.Erro);
                            }
                        }
                        else
                        {
                            this.ExibirErro("UsuarioServico.ExcluirEmLote", codigoRetorno, "Atenção", this.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                        }
                    }
                    else
                    {
                        String mensagem = base.RetornarMensagemErro("ConfirmacaoCadatro.ValidarAcesso", 1167)
                                              .Format(6 - usuario.QuantidadeTentativaLoginIncorreta); // String.Format("Você tem {0} tentativas.", 6 - usuario.QuantidadeTentativaLoginIncorreta);
                        this.ExibirAvisoTentativas(mensagem, "E-mail ou senha incorreto", QuadroAvisosResponsivo.IconeMensagem.Aviso);
                    }
                }
                else
                {
                    this.ExibirErro("UsuarioServico.IncrementarQuantidadeSenhaInvalida", codigoRetorno, "Atenção", this.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                }
            }
        }

        /// <summary>
        /// Exibe o quadro de aviso com a mensagem passada em parâmetro
        /// </summary>
        /// <param name="mensagem">Mensagem de erro/aviso</param>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="urlVoltar">Url que o botão Voltar do Aviso deve redirecionar</param>
        /// <param name="icone">ícone a ser exibido</param>
        private void ExibirErro(String mensagem, String titulo, String urlVoltar, QuadroAvisosResponsivo.IconeMensagem icone)
        {
            if (!String.IsNullOrEmpty(urlVoltar))
            {
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, icone);
            }
            else
            {
                QdAviso.CarregarMensagem(titulo, mensagem, false, icone);
            }

            pnlQuadroAcesso.Visible = false;
            pnlBotaoTentativas.Visible = false;
            pnlQuadroAviso.Visible = true;
        }

        /// <summary>
        /// Exibe o quadro de aviso com a mensagem de aviso
        /// </summary>
        /// <param name="mensagem">Mensagem a ser exibida na mensagem</param>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="icone">ícone a ser exibido</param>
        private void ExibirAvisoTentativas(String mensagem, String titulo, QuadroAvisosResponsivo.IconeMensagem icone)
        {
            QdAviso.CarregarMensagem(titulo, mensagem, false, icone);

            pnlBotaoTentativas.Visible = true;
            pnlQuadroAviso.Visible = true;
            pnlQuadroAcesso.Visible = false;
        }

        /// <summary>
        /// Marca a flag de exibição de liberação de acesso completo para true
        /// </summary>
        private void AtualizarFlagExibicaoMensagemAcessoCompleto()
        {
            using (Logger log = Logger.IniciarLog("Atualizar flag exibição mensagem acesso completo"))
            {
                try
                {
                    //Change request: Atualiza a flag de exibição de mensagem de liberação de acesso completo para true
                    //A flag deve ser alterada para false apenas quando o usuário clicar em uma das opções
                    //"Continuar como acesso básico" ou "Liberar acesso completo"
                    using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    {
                        Int32 codigoRetorno = ctx.Cliente
                            .AtualizarFlagExibicaoMensagemLiberacaoAcessoCompleto(this.Usuario.CodigoIdUsuario, true);
                        if (codigoRetorno > 0)
                        {
                            Logger.GravarErro("Erro durante atualização de flag de exibição de mensagem de acesso completo",
                                new Exception("Erro durante atualização de flag de exibição de mensagem de acesso completo"),
                                new { this.Usuario.CodigoIdUsuario, codigoRetorno });
                        }
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }
            }
        }

        /// <summary>
        /// Atualiza o status do usuário e exclui o hash de e-mail de confirmação dele.
        /// </summary>
        private Boolean AtualizarStatusUsuario()
        {
            using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
            {
                Int32 codigoRetorno = contextoUsuario.Cliente.ReiniciarQuantidadeSenhaInvalida(this.Usuario.CodigoIdUsuario);

                if (codigoRetorno == 0)
                {
                    if (this.Usuario.Status.Codigo == (Int32)UsuarioServico.Status1.UsuarioAguardandoConfirmacaoAlteracaoEmail)
                    {
                        codigoRetorno = contextoUsuario.Cliente.ConfirmarAtualizacaoEmail(this.Usuario.CodigoIdUsuario);

                        if (codigoRetorno > 0)
                        {
                            this.ExibirErro("UsuarioServico.ConfirmarAtualizacaoEmail", codigoRetorno, "Atenção", this.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                            return false;
                        }
                        else
                        {
                            if (this.Usuario.Legado && this.Usuario.TipoUsuario.Equals("M"))
                                contextoUsuario.Cliente.AtualizarPermissoes(this.RecuperarPermissoesUsuarioMaster(1), this.Usuario.CodigoIdUsuario);

                        }
                    }
                    else
                    {
                        Guid hashEmail = Guid.Empty;
                        codigoRetorno = contextoUsuario.Cliente.AtualizarStatus(out hashEmail, this.Usuario.CodigoIdUsuario, UsuarioServico.Status1.UsuarioAtivo, 2, DateTime.Now);

                        if (codigoRetorno > 0)
                        {
                            this.ExibirErro("UsuarioServico.AtualizarStatus", codigoRetorno, "Atenção", this.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                            return false;
                        }
                    }

                    return this.ExcluirHashUsuario();
                }
                else
                {
                    this.ExibirErro("UsuarioServico.ReiniciarQuantidadeSenhaInvalida", codigoRetorno, "Atenção", this.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                    return false;
                }
            }
        }

        /// <summary>
        /// Exclui o hash de e-mail do usuário
        /// </summary>
        /// <returns>
        /// <para>True - Com sucesso</para>
        /// <para>False - Com falha</para>
        /// </returns>
        private Boolean ExcluirHashUsuario()
        {
            using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
            {
                Int32 codigoRetorno = contextoUsuario.Cliente.ExcluirHash(this.Usuario.CodigoIdUsuario);

                if (codigoRetorno > 0)
                {
                    this.ExibirErro("UsuarioServico.ExcluirHash", codigoRetorno, "Atenção", this.RecuperarEnderecoPortal(), QuadroAvisosResponsivo.IconeMensagem.Erro);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Autentica o usuário para recarregar as permissões
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
            queryString["indSenhaCript"] = "N";

            String url = Redecard.PN.Comum.Util.BuscarUrlRedirecionamento(
                                String.Format("/_layouts/DadosCadastrais/Login.aspx?dados={0}",
                                                queryString.ToString()),
                                SPUrlZone.Internet);
            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Recupera as permissões de Serviços Master do Grupo de Entidade separadas por ','
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do Grupo de Entidade</param>
        /// <returns></returns>
        private String RecuperarPermissoesUsuarioMaster(Int32 codigoGrupoEntidade)
        {
            AdministracaoServico.Servico[] servicos = default(AdministracaoServico.Servico[]);
            String codigoServicos = String.Empty;

            using (Logger log = Logger.IniciarLog("Recupera as permissões de Serviços Master do Grupo de Entidade separadas por ','"))
            {
                using (var ctx = new ContextoWCF<AdministracaoServico.AdministracaoServicoClient>())
                    servicos = ctx.Cliente.ConsultarPorGrupoEntidade2(codigoGrupoEntidade, true);

                if (servicos.Length > 0)
                {
                    codigoServicos = String.Join(",", servicos.Distinct().Select(servico => servico.Codigo.ToString()).ToArray());
                }
            }

            return codigoServicos;
        }
        #endregion
    }
}
