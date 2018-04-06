/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.ServiceModel;
using System.Web.Configuration;
using System.Web.Security;
using Microsoft.IdentityModel.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using System.Linq;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.UI;
using Redecard.PNCadastrais.Core.Web.Controles.Portal;
using System.Web.Script.Serialization;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
// DLL para verificação de vulnerabilidade XSS
using EncoderSecurity = Microsoft.Security.Application;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.MeuUsuario.CadastroUsuario
{
    /// <summary>
    /// Controle de Alteração do Cadastro do Usuário - Meu Usuário
    /// </summary>
    public partial class CadastroUsuarioUserControl : MeuUsuarioUserControlBase
    {
        #region [ Eventos customizados ]

        /// <summary>
        /// Evento chamado após o Reenvio de Solicitação de Aprovação para o Master com sucesso
        /// </summary>
        public event EventHandler SolicitacaoAprovacaoReenviada;

        #endregion

        #region [ Eventos de Página ]

        /// <summary>
        /// Load da Página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Load da Página"))
            {
                try
                {
                    this.Page.Form.DefaultButton = btnSalvarAlteracoes.UniqueID;

                    if (!IsPostBack)
                    {
                        if (SessaoAtual != null && SessaoAtual.UsuarioAtendimento)
                        {
                            btnSalvarAlteracoes.Visible = false;
                        }
                        else
                        {
                            if (this.MigracaoUsuarioLegado())
                            {
                                txtNome.Text = EncoderSecurity.Encoder.HtmlEncode(this.UsuarioAtual.Descricao);
                                pnlEmailPrincipal.Visible = true;

                                txtEmail.Required = true;
                                txtEmail.ErrorMessageRequired = "campo obrigatório";
                                txtEmailSecundario.Required = false;
                                txtNovaSenha.Required = true;
                                txtNovaSenha.ErrorMessageRequired = "campo obrigatório";

                                pnlSenhaAtual.Visible = false;
                                pnlNome.Visible = false;
                            }
                            else
                                CarregarDadosUsuario();
                        }
                    }
                    else if (String.Compare(Request.Params["__EVENTTARGET"], "ReenviarEmailConfirmacao", true) == 0)
                    {
                        ReenviarEmailConfirmacao(Request.Params["__EVENTARGUMENT"]);
                    }
                    else if (String.Compare(Request.Params["__EVENTTARGET"], "ReenviarSolicitacaoAprovacao", true) == 0)
                    {
                        ReenviarSolicitacaoAprovacao(Request.Params["__EVENTARGUMENT"]);
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

        #endregion

        #region [ Métodos auxiliares (negócio) ]
        /// <summary>
        /// Método para reenviar a solicitação de aprovação de usuário.
        /// </summary>
        /// <param name="argument"></param>
        private void ReenviarSolicitacaoAprovacao(String argument)
        {
            EntidadeServicoModel[] usuarios;
            Guid hash;
            String emailArgument = argument.Split('|')[2];

            using (Logger log = Logger.IniciarLog("ReenviarSolicitacaoAprovacao - Alteração do cadastro do usuário"))
            {
                try
                {

                    using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    {
                        log.GravarMensagem("Chamando método GetEmailMasterUsuarioAguardandoConfirmacaoMaster");

                        usuarios = ctx.Cliente.GetUsuarioAguardandoConfirmacaoMaster(emailArgument);

                        log.GravarMensagem("Resultado método GetEmailMasterUsuarioAguardandoConfirmacaoMaster", new { result = usuarios });
                    }

                    if (usuarios != null && usuarios.Any())
                    {
                        foreach (var item in usuarios)
                        {
                            //Envia e-mail de aprovação
                            EmailNovoAcesso.EnviarEmailAprovacaoAcesso(item.EmailsMaster,
                                                                       item.NomeUsuario,
                                                                       item.Email,
                                                                       item.IdUsuario,
                                                                       "B",
                                                                       item.NumeroPV,
                                                                       null);
                        }
                    }


                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }
            }

            //Recupera o Código da Entidade que foi validada
            Int32 codigoEntidade = argument.Split('|')[1].ToInt32();
            Boolean sucesso = false;

            //Obtém os dados do usuário do e-mail
            Usuario usuario = ConsultarUsuarioPorEmail(emailArgument, codigoEntidade, out sucesso, out hash);
            //Consulta os usuários Master da entidade
            EntidadeServico.Usuario[] usuariosMaster = ConsultarUsuariosMaster(codigoEntidade);

            if (usuario != null && usuariosMaster != null && usuariosMaster.Length > 0)
            {
                String[] emails = usuariosMaster.Select(master => master.Email)
                    .Where(email => !String.IsNullOrEmpty(email)).ToArray();

                //Envia e-mail de aprovação
                EmailNovoAcesso.EnviarEmailAprovacaoAcesso(String.Join(",", emails), usuario.Descricao, usuario.Email,
                    usuario.CodigoIdUsuario, usuario.TipoUsuario, codigoEntidade, null);

                //Se definido, invoca handler para tratamento pós reenvio do e-mail 
                if (SolicitacaoAprovacaoReenviada != null)
                    SolicitacaoAprovacaoReenviada(this, new EventArgs());
            }
        }

        /// <summary>
        /// Consulta usuários Master do estabelecimento
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        private static EntidadeServico.Usuario[] ConsultarUsuariosMaster(Int32 codigoEntidade)
        {
            var codigoRetorno = default(Int32);
            var usuarios = default(EntidadeServico.Usuario[]);

            using (Logger log = Logger.IniciarLog("Consulta usuários de Perfil Master do Estabelecimento"))
            {
                try
                {
                    using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        usuarios = ctx.Cliente.ConsultarUsuariosPorPerfil(out codigoRetorno, codigoEntidade, 1, 'M');
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }
            }

            return usuarios;
        }

        /// <summary>
        /// Método para reenviar o e-mail de confirmação.
        /// </summary>
        /// <param name="argument"></param>
        private void ReenviarEmailConfirmacao(String argument)
        {
            using (Logger log = Logger.IniciarLog("ReenviarEmailConfirmacao - Alteração do cadastro do usuário"))
            {
                Int32 codigoEntidade = argument.Split('|')[1].ToInt32();
                String email = argument.Split('|')[2];
                Guid hash = Guid.Empty;

                // busca usuário na base pelo e-mail informado
                Boolean sucesso = false;
                Usuario usuario = ConsultarUsuarioPorEmail(email, codigoEntidade, out sucesso, out hash);

                Sessao sessaoAtual = new Sessao
                {
                    Celular = usuario.Celular,
                    CPF = usuario.CPF,
                    CodigoEntidade = usuario.Entidade.Codigo,
                    GrupoEntidade = usuario.Entidade.GrupoEntidade.Codigo,
                    CodigoIdUsuario = usuario.CodigoIdUsuario,
                    DDDCelular = usuario.DDDCelular,
                    Email = usuario.Email,
                    EmailSecundario = usuario.EmailSecundario,
                    EmailTemporario = usuario.EmailTemporario,
                    ExibirMensagemLiberacaoAcessoCompleto = usuario.ExibirMensagemLiberacaoAcesso,
                    Legado = usuario.Legado,
                    NomeUsuario = usuario.NomeResponsavelInclusao,
                    TipoUsuario = usuario.TipoUsuario
                };

                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    {
                        if (hash == Guid.Empty)
                        {
                            log.GravarMensagem("Chamando método InserirHash");

                            hash = ctx.Cliente.InserirHash(
                                usuario.CodigoIdUsuario,
                                (Status1)usuario.Status.Codigo.GetValueOrDefault(0),
                                0.5,
                                null);

                            log.GravarMensagem("Resultado método InserirHash", new { result = hash });
                        }
                        else
                        {
                            log.GravarMensagem("Chamando método ReiniciarHash");

                            int codigoRetorno = 0;
                            var usuarioHash = ctx.Cliente.ReiniciarHash(out codigoRetorno, usuario.CodigoIdUsuario);

                            log.GravarMensagem("Resultado método ReiniciarHash", new { result = usuarioHash });

                            if (usuarioHash != null && usuarioHash.Hash != null)
                            {
                                hash = usuarioHash.Hash;
                            }
                            else
                            {
                                log.GravarMensagem("Hash não retornada... usando a hash existente", new { result = hash });
                            }
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }

                // reenvio de e-mail para 
                EmailNovoAcesso.EnviarEmailConfirmacaoCadastro(
                    sessaoAtual, email, new List<int>() { codigoEntidade }, usuario.CodigoIdUsuario, hash);

                // se definido, invoca handler para tratamento pós reenvio do e-mail
                if (SolicitacaoAprovacaoReenviada != null)
                    SolicitacaoAprovacaoReenviada(this, new EventArgs());
            }
        }

        /// <summary>
        /// Consulta usuário através do e-mail, para determinado estabelecimento
        /// </summary>
        /// <param name="email">E-mail</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="sucesso">Se a verificação foi realizada com sucesso</param>
        /// <returns>Usuário encontrado (deve ser apenas 1)</returns>        
        private static Usuario ConsultarUsuarioPorEmail(String email, Int32 codigoEntidade, out Boolean sucesso, out Guid hash)
        {
            hash = Guid.Empty;
            Usuario usuario = null;

            using (Logger log = Logger.IniciarLog("Consulta usuário através do e-mail"))
            {
                try
                {
                    var codigoRetorno = default(Int32);
                    var entidade = new Entidade
                    {
                        Codigo = codigoEntidade,
                        GrupoEntidade = new GrupoEntidade { Codigo = 1 }
                    };

                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                    {
                        var usuarios = ctx.Cliente.ConsultarPorEmailPrincipalPorStatus(out codigoRetorno, email, 0, codigoEntidade, null);

                        if (usuarios != null && usuarios.Any())
                        {
                            usuario = usuarios.FirstOrDefault();

                            var hashUsuarios = ctx.Cliente.ConsultarHash(out codigoRetorno, usuario.CodigoIdUsuario, null, null);

                            if (hashUsuarios != null || hashUsuarios.Any())
                            {
                                hash = hashUsuarios.FirstOrDefault().Hash;
                            }
                        }
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }
                sucesso = usuario != null;
                return usuario;
            }
        }

        /// <summary>
        /// Consulta usuário através do e-mail temporário, para determinado estabelecimento
        /// </summary>
        /// <param name="emailTemporario">E-mail</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="sucesso">Se a verificação foi realizada com sucesso</param>
        /// <returns>Usuário encontrado (deve ser apenas 1)</returns>        
        private static Usuario ConsultarUsuarioPorEmailTemporario(String emailTemporario, Int32 codigoEntidade, out Boolean sucesso)
        {
            var usuario = default(Usuario);
            using (Logger log = Logger.IniciarLog("Consulta usuário através do e-mail temporário"))
            {
                var codigoRetorno = default(Int32);

                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        usuario = ctx.Cliente.ConsultarPorEmailTemporario(out codigoRetorno, emailTemporario, 1, codigoEntidade);
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }
            }
            sucesso = usuario != null;
            return usuario;
        }

        /// <summary>
        /// Atualiza e-mail do usuário logado
        /// </summary>
        private Boolean AtualizarEmail(Int32 codigoIdUsuario, String novoEmail, out Guid hashConfirmacaoEmail)
        {
            Boolean sucesso = false;
            hashConfirmacaoEmail = default(Guid);

            using (Logger log = Logger.IniciarLog("Alteração de E-mail"))
            {
                try
                {
                    Int32 codigoRetorno = default(Int32);

                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        codigoRetorno = ctx.Cliente.AtualizarEmail(out hashConfirmacaoEmail, codigoIdUsuario, novoEmail, 2, DateTime.Now);

                    if (codigoRetorno != 0)
                        base.ExibirPainelExcecao("UsuarioServico.AtualizarEmail", codigoRetorno);
                    else
                        sucesso = true;
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
            return sucesso;
        }
        #endregion

        #region [ Listas ]

        /// <summary>
        /// Lista Período Migração
        /// </summary>
        private SPList ListaPeriodoMigracao
        {
            get
            {
                //Recupera a lista de "Período Migração" em sites/fechado/minhaconta
                using (SPSite spSite = SPContext.Current.Site.WebApplication.Sites["sites/fechado"])
                using (SPWeb spWeb = spSite.AllWebs["minhaconta"])
                    return spWeb.Lists.TryGetList("Período Migração");
            }
        }

        #endregion

        #region [ Eventos dos Controles ]

        /// <summary>
        /// Clique no botão de confirmação
        /// </summary>
        protected void btnSalvarAlteracoes_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                this.SalvarDadosUsuario();
            }
        }

        /// <summary>
        /// Clique no botão Cancelar, retornando para a tela inicial de Meu Usuário
        /// </summary>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            this.RedirecionarParaMeuUsuario(false, false);
        }

        /// <summary>
        /// Redirecionar para Homepage após preencher os dados de migração
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuarMigracao_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Redirecionar para Homepage após preencher os dados de migração"))
            {
                try
                {
                    if (DateTime.Now < Convert.ToDateTime(ListaPeriodoMigracao.Items[0]["DataFinal"]))
                        Response.Redirect(base.RecuperarEnderecoPortalFechado(), false); //Quando o período da primeira fase estiver ativo, deverá direcionar para a home transacional do Portal
                    else
                        this.LogoutUsuario(); //Quando o período de migração expirar, deverá direcionar para a home institucional do Portal encerrando a sessão
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Realiza o logout do usuário após migrar seus dados
        /// </summary>
        private void LogoutUsuario()
        {
            SPIisSettings iisSettingsWithFallback = SPContext.Current.Site.WebApplication.GetIisSettingsWithFallback(SPUrlZone.Default);
            if (iisSettingsWithFallback.UseClaimsAuthentication)
            {
                FederatedAuthentication.SessionAuthenticationModule.SignOut();

                // Remover sessão do usuário
                Session.Abandon();

                int num = 0;
                foreach (SPAuthenticationProvider provider in iisSettingsWithFallback.ClaimsAuthenticationProviders)
                {
                    num++;
                }
                if (num != 1 || !iisSettingsWithFallback.UseWindowsIntegratedAuthentication)
                {
                    String url = base.RecuperarEnderecoPortal();
                    Response.Redirect(url, true);
                    return;
                }
            }
            else
            {
                if (AuthenticationMode.Forms == SPSecurity.AuthenticationMode)
                {
                    FormsAuthentication.SignOut();
                    // Remover sessão do usuário
                    Session.Abandon();

                    String url = RecuperarEnderecoPortal();
                    Response.Redirect(url, true);
                    return;
                }
                if (AuthenticationMode.Windows != SPSecurity.AuthenticationMode)
                {
                    throw new SPException();
                }
            }
        }
        /// <summary>
        /// Valida se o usuário atual é legado e necessita atualizar seus dados
        /// </summary>
        /// <returns>
        /// <para>True: migrar usuário legado</para>
        /// <para>False: não migrar usuário</para>
        /// </returns>
        private Boolean MigracaoUsuarioLegado()
        {
            //return !SessaoAtual.MigrarDepois && SessaoAtual.Legado;
            return SessaoAtual.Legado;
        }

        /// <summary>
        /// Carregar dados de cadastro do usuário
        /// </summary>
        private void CarregarDadosUsuario()
        {
            Usuario usuario = this.UsuarioAtual;
            if (usuario != null)
            {
                txtNome.Text = EncoderSecurity.Encoder.HtmlEncode(usuario.Descricao);
                txtCpf.Value = usuario.CPF.ToString();
                txtCelular.Text = CampoCelular.AplicarMascara(usuario.DDDCelular, usuario.Celular);
            }
        }

        /// <summary>
        /// Salvar dados de cadastro do usuário
        /// </summary>
        public void SalvarDadosUsuario()
        {
            Usuario usuario = this.UsuarioAtual;
            String nome = EncoderSecurity.Encoder.HtmlEncode(txtNome.Text);
            Int64? cpf = txtCpf.Value.CpfCnpjToLong();
            String email = txtEmail.Value;
            String novaSenha = EncriptadorSHA1.EncryptString(txtNovaSenha.Value);
            String emailSecundario = String.IsNullOrEmpty(txtEmailSecundario.Value) ? usuario.EmailSecundario : txtEmailSecundario.Value;
            Int32 resultDDD = 0;
            Int32 resultCelular = 0;
            Int32? celularDDD = null;
            Int32? celularNumero = null;
            if (!String.IsNullOrWhiteSpace(txtCelular.Text))
            {
                celularDDD = Int32.TryParse(txtCelular.Text.Substring(1, 2), out resultDDD) ? resultDDD : default(int?);
                celularNumero = Int32.TryParse(txtCelular.Text.Substring(4).Trim().RemoverCaracteresEspeciais(), out resultCelular) ? resultCelular : default(int?);
            }

            using (Logger log = Logger.IniciarLog("Alterando dados do usuário atual"))
            {
                try
                {
                    var codigoRetorno = default(Int32);
                    var hashEmailConfirmacao = default(Guid);
                    var possuiKomerci = default(Boolean);
                    var entidades = default(EntidadeServico.Entidade1[]);

                    #region [ Consulta Entidades Usuário ]

                    //Consulta entidades associadas ao usuário
                    using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        entidades = ctx.Cliente.ConsultarPorUsuario(out codigoRetorno, usuario.CodigoIdUsuario);

                    if (codigoRetorno != 0)
                    {
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarPorUsuario", codigoRetorno);
                        return;
                    }

                    #endregion

                    #region [ Verificação Komerci ]

                    //Verifica se algum PV do usuário possui Komerci
                    using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        possuiKomerci = ctx.Cliente.PossuiKomerci(entidades.Select(entidade => entidade.Codigo).ToArray());

                    #endregion

                    #region [ Atualização Dados usuário ]
                    //Atualiza os dados básicos do cadastro do usuário
                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        codigoRetorno = ctx.Cliente.Atualizar2(
                            out hashEmailConfirmacao,
                            possuiKomerci,
                            SessaoAtual.GrupoEntidade,
                            String.Empty, //código entidades
                            String.Empty, //login
                            nome,
                            usuario.TipoUsuario,
                            String.Empty, //senha,
                            String.Empty, //código serviços
                            usuario.CodigoIdUsuario,
                            usuario.Email,
                            emailSecundario,
                            cpf,
                            celularDDD,
                            celularNumero,
                            null,
                            null,
                            null);

                    if (codigoRetorno != 0)
                    {
                        base.ExibirPainelExcecao("UsuarioServico.Atualizar2", codigoRetorno);
                        return;
                    }
                    #endregion

                    #region [ Atualização Senha Usuário ]
                    if (!String.IsNullOrWhiteSpace(txtNovaSenha.Value))
                    {
                        using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        {
                            codigoRetorno = ctx.Cliente.AtualizarSenha(this.UsuarioAtual, novaSenha, possuiKomerci, false);
                        }

                        if (codigoRetorno > 0)
                        {
                            base.ExibirPainelExcecao("UsuarioServico.AtualizarSenha", codigoRetorno);
                            return;
                        }
                        else
                        {
                            //Armazena no histórico/log de atividades
                            Historico.AlteracaoSenha(SessaoAtual);
                        }
                    }
                    #endregion

                    #region [ Atualização Email Usuario ]
                    var novoEmail = txtEmail.Value;
                    Boolean emailAtualizado = false;
                    if (!String.IsNullOrWhiteSpace(novoEmail))
                    {
                        //Verifica se a senha atual com a senha do usuário
                        //Atualiza e-mail, se sucesso, exibe aviso de confirmação de alteração de e-mail
                        Guid hashConfirmacaoEmail = default(Guid);
                        emailAtualizado = this.AtualizarEmail(this.UsuarioAtual.CodigoIdUsuario,
                            novoEmail, out hashConfirmacaoEmail);
                        if (emailAtualizado)
                        {
                            //Armazena no histórico/log de atividade
                            Historico.AlteracaoEmail(SessaoAtual, novoEmail);

                            //Envia e-mail de confirmação para o usuário, expira em 48h
                            EmailNovoAcesso.EnviarEmailConfirmacaoCadastro48h(novoEmail,
                                this.UsuarioAtual.CodigoIdUsuario, hashConfirmacaoEmail,
                                this.UsuarioAtual.CodigoIdUsuario, this.UsuarioAtual.Email,
                                this.UsuarioAtual.Descricao, this.UsuarioAtual.TipoUsuario,
                                SessaoAtual.CodigoEntidade, SessaoAtual.Funcional);
                        }
                    }
                    #endregion

                    #region [ Usuário Legado ]

                    if (this.MigracaoUsuarioLegado())
                    {
                        using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                            codigoRetorno = ctx.Cliente.AtualizarEmail(
                                out hashEmailConfirmacao,
                                usuario.CodigoIdUsuario,
                                novoEmail,
                                60,
                                this.ObterDataMigracao());

                        if (codigoRetorno != 0)
                        {
                            base.ExibirPainelExcecao("UsuarioServico.AtualizarEmail", codigoRetorno);
                            return;
                        }

                        EmailNovoAcesso.EnviarEmailConfirmacaoMigracao(
                            novoEmail,
                            usuario.CodigoIdUsuario,
                            hashEmailConfirmacao,
                            nome,
                            usuario.TipoUsuario,
                            SessaoAtual.CodigoEntidade,
                            SessaoAtual.Funcional);

                        //Marca o atributo como true para não exibir o aviso caso o usuário acesse a homepage novamente
                        SessaoAtual.MigrarDepois = true;
                    }
                    #endregion

                    ////Armazena no histórico/log de atividades
                    Historico
                        .Comparar
                        .Campo(usuario.Email, novoEmail, "e-mail")
                        .Campo(usuario.Descricao, nome, "nome")
                        .Campo(usuario.CPF, cpf, "CPF")
                        .Campo(usuario.EmailSecundario, emailSecundario, "e-mail secundário")
                        .Campo(usuario.DDDCelular, celularDDD, "DDD celular")
                        .Campo(usuario.Celular, celularNumero, "celular")
                        .AlteracaoDadosUsuario(SessaoAtual);

                    if (!this.MigracaoUsuarioLegado())
                        this.RedirecionarParaMeuUsuario(true, emailAtualizado);
                    else
                        this.ExibirAvisoEnvioEmail();
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
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
        /// Exibir o aviso de envio de E-mail de confirmação de migração
        /// </summary>
        private void ExibirAvisoEnvioEmail()
        {
            pnlCadastro.Visible = false;
            pnlAviso.Visible = true;

            qdAviso.Titulo = "Seu cadastro foi finalizado com sucesso";
            qdAviso.Mensagem = @"Dentro de instantes você receberá um e-mail de confirmação.<br />Acesse o link informado no e-mail para concluir a alteração dos seus dados de acesso.";
            qdAviso.TipoQuadro = TipoQuadroAviso.Sucesso;
        }

        /// <summary>
        /// Recuperar a data de expiração a ser contabilizada para o Usuário Legado
        /// </summary>
        /// <returns></returns>
        private DateTime ObterDataMigracao()
        {
            DateTime dataMigracao = new DateTime();

            if (DateTime.Now < Convert.ToDateTime(ListaPeriodoMigracao.Items[0]["DataFinal"]))
                dataMigracao = Convert.ToDateTime(ListaPeriodoMigracao.Items[0]["DataInicio"]);
            else
                dataMigracao = Convert.ToDateTime(ListaPeriodoMigracao.Items[0]["DataFinal"]);

            return dataMigracao;
        }

        #endregion

        #region [ Validações Server-side ]

        /// <summary>
        /// Validação do controle txtNovaSenha.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void txtNovaSenha_ServerValidate(object source, SenhaServerValidateEventArgs args)
        {
            Usuario usuario = this.UsuarioAtual;

            //Valida se a senha nova é igual a senha atual, se sim, retorna erro.
            args.IsValid = usuario.Senha != EncriptadorSHA1.EncryptString(txtNovaSenha.Value);

            if (!args.IsValid)
            {
                args.ErrorMessage = "a nova senha não pode ser igual à senha atual";
            }
        }

        /// <summary>
        /// Validação de CPF de Usuário
        /// </summary>
        /// <param name="exibirMensagem">Se deve exibir automaticamente as mensagens de validação</param>
        /// <param name="codigoEntidades">Código dos PVs</param>
        /// <param name="codigoIdUsuario">ID do Usuário a validar</param>
        /// <returns>Se CPF é válido</returns>
        protected void txtCpf_ServerValidate(object source, CpfCnpjServerValidateEventArgs args)
        {
            args.ErrorMessage = String.Empty;
            args.IsValid = true;

            using (Logger log = Logger.IniciarLog("Validação de CPF de Usuário"))
            {
                Boolean cpfDisponivel = true;
                var usuarios = default(UsuarioServico.Usuario[]);
                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        usuarios = ctx.Cliente.ConsultarPorCpf(this.txtCpf.Value.CpfCnpjToLong(), SessaoAtual.CodigoEntidade, 1);

                    if (usuarios != null)
                    {
                        var usuario = usuarios.Where(x => x.CodigoIdUsuario != SessaoAtual.CodigoIdUsuario);

                        if (usuario.FirstOrDefault() != null)
                        {
                            cpfDisponivel = false;
                        }
                    }

                    if (!cpfDisponivel)
                    {
                        args.IsValid = false;
                        args.ErrorMessage = "este CPF já possui usuário cadastrado";
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
        /// Evento para validação server-side do campo txtEmail.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void txtEmail_ServerValidate(object source, EmailServerValidateEventArgs args)
        {
            string emailDigitado = txtEmail.Value.Trim();
            if (String.Compare(UsuarioAtual.Email.Trim(), emailDigitado, true) == 0)
            {
                args.ErrorMessage = "O e-mail deve ser diferente do atual";
                args.IsValid = false;
            }
            else
            {
                if (String.IsNullOrWhiteSpace(emailDigitado) || !MigracaoUsuarioLegado())
                {
                    args.IsValid = true;
                }
                else
                {
                    Int32[] codigoEntidades = new Int32[] { SessaoAtual.CodigoEntidade };
                    Guid hash = Guid.Empty;

                    foreach (Int32 codigoEntidade in codigoEntidades)
                    {
                        //Verifica se já existe usuário com o mesmo e-mail para o PV           
                        Boolean statusConsulta = false;

                        var usuario = ConsultarUsuarioPorEmail(txtEmail.Value, codigoEntidade, out statusConsulta, out hash);

                        if (usuario == null)
                        {
                            //Não existe usuário com o mesmo e-mail, então tenta verificar se existe usuário com e-mail temporário igual para o PV
                            usuario = ConsultarUsuarioPorEmailTemporario(txtEmail.Value, codigoEntidade, out statusConsulta);
                        }

                        if (usuario != null)
                        {
                            String reference = String.Empty;
                            String titulo = String.Empty;
                            String mensagemRetorno = String.Empty;

                            switch ((Status1)usuario.Status.Codigo.Value)
                            {
                                //Se e-mail é de usuário Aguardando Aprovação do Master, informa que está aguardando aprovação
                                //Se está na Parte Aberta, exibe link para reenvio da solicitação de aprovação para o Master
                                case Status1.UsuarioAguardandoConfirmacaoMaster:
                                    titulo = "Usuário aguardando confirmação";
                                    reference = String.Concat("ReenviarSolicitacaoAprovacao", "|", codigoEntidade, "|", usuario.Email);
                                    mensagemRetorno = String.Format(@"<span><a href=""javascript:void(0)"" onclick=""CustomPostBack(\'ReenviarSolicitacaoAprovacao\', \'{0}\')"">Clique aqui</a> para solicitar confirmação ao usuário Master</span>", reference);
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ExibirModalStatusUsuario", String.Format(@"ConstruirModal(2,'{0}','{1}');", mensagemRetorno, titulo), true);
                                    break;
                                // Se usuário aguardando confirmação para validação do e-mail informado no cadastro
                                case Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario:

                                    titulo = "Usuário aguardando confirmação";
                                    reference = String.Concat("ReenviarEmailConfirmacao", "|", codigoEntidade, "|", usuario.Email);
                                    mensagemRetorno = String.Format(@"<span><a href=""javascript:void(0)"" onclick=""CustomPostBack(\'ReenviarEmailConfirmacao\', \'{0}\')"">Clique aqui</a> para reenviar o e-mail de confirmação</span>", reference);
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ExibirModalStatusUsuario", String.Format(@"ConstruirModal(2,'{0}','{1}');", mensagemRetorno, titulo), true);

                                    break;

                                //Se usuário Ativo, informa que usuário já existe
                                //Se está na Parte Aberta, exibe link para recuperação de senha
                                case Status1.UsuarioAtivo:
                                case Status1.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:

                                    titulo = "Usuário já existente";
                                    mensagemRetorno = String.Format(@"<span>Utilize a <a href=""{0}"">recuperação de senha</a></span>", "/pt-br/novoacesso/Paginas/RecuperacaoSenhaIdentificacao.aspx");
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ExibirModalStatusUsuario", String.Format(@"ConstruirModal(2,'{0}','{1}');", mensagemRetorno, titulo), true);

                                    break;
                                //Para qualquer outro status, informa que usuário já existe
                                default:
                                    mensagemRetorno = "<span>Usuário já existente</span>";
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ExibirModalStatusUsuario", String.Format(@"ConstruirModal(2,'{0}');", mensagemRetorno), true);
                                    break;
                            }

                            args.IsValid = false;
                        }
                        else
                        {
                            args.IsValid = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Evento para validação server-side do campo crvSenhaAtual.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void crvSenhaAtual_ServerValidate(object source, CustomServerValidateEventArgs args)
        {
            if (base.VerificarSenhaAtual(txtSenhaAtual.Text) || this.MigracaoUsuarioLegado())
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
                args.ErrorMessage = "senha incorreta";
            }
        }

        #endregion

    }
}
