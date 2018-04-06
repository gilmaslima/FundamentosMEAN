/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.Business;
using Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using Redecard.PNCadastrais.Core.Web.Controles.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios.DadosCadastro
{
    /// <summary>
    /// Administração de Usuários - Criação/Edição de Usuário - Passo 1 - Dados de Cadastro
    /// </summary>
    public partial class DadosCadastroUserControl : UsuariosUserControlBase
    {
        #region [ Eventos de Página ]

        /// <summary>
        /// Load da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("DadosCadastroUserControl.ascx - Page_Load"))
            {
                try
                {
                    //Verifica permissão (apenas Master e Central de Atendimento podem acessar)
                    if (!base.PossuiPermissao)
                    {
                        mvwDadosCadastro.SetActiveView(pnlAcessoNegado);
                        return;
                    }

                    //Botão default da página
                    this.Page.Form.DefaultButton = btnContinuar.UniqueID;

                    // para nao autopreencer o formulario
                    this.Page.Form.Attributes.Add("autocomplete", "off");

                    if (!IsPostBack)
                    {
                        //Configuração dos controles
                        this.ConfigurarControles();

                        // carrega o controle de e-mail com os domínios bloqueados
                        this.txtUsuarioEmail.BlockedDomains.AddRange(UsuarioNegocio.DominiosBloqueados);
                        this.txtUsuarioEmailSecundario.BlockedDomains.AddRange(UsuarioNegocio.DominiosBloqueados);

                        if (this.ModoEdicao)
                        {
                            this.CarregarDadosUsuario();
                            btnContinuar.Text = "salvar";
                        }
                    }
                    //else
                    //{
                    //    if (String.Compare(Request.Params.Get("__EVENTTARGET"), "estabelecimentosTab", true) == 0)
                    //    {
                    //        //Redireciona para o próximo passo
                    //        this.RedirecionarPassoEstabelecimentos();
                    //    }
                    //    else if (String.Compare(Request.Params.Get("__EVENTTARGET"), "permissoesTab", true) == 0)
                    //    {
                    //        //Redireciona para o passo 3
                    //        RedirecionarPassoPermissoes();
                    //    }
                    //}
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

        #endregion

        #region [ Eventos dos Controles ]

        /// <summary>
        /// Continua para o próximo passo
        /// </summary>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("DadosCadastroUserControl.ascx - btnContinuar_Click"))
            {
                try
                {
                    // valida os dados básicos da página
                    if (!Page.IsValid)
                        return;

                    //Armazena em sessão os dados digitados
                    this.SalvarDadosTemporariosUsuario();

                    // No modo de edicao, salva os itens e mantem na tela
                    if (this.ModoEdicao)
                    {
                        log.GravarMensagem("Modo edição");

                        Boolean emailAtualizado;
                        base.AlterarUsuario("DadosCadastro", out emailAtualizado);

                        //Abrir lightbox para indicar que foi atualizado
                        String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { dadosIniciaisOpenModal('#lbxModalConfirmacao'); }, 'SP.UI.Dialog.js');";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AbrirModalDialog", javaScript, true);
                    }
                    else
                    {
                        //Redireciona para o próximo passo
                        this.RedirecionarPassoEstabelecimentos();
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
        /// Cancela e volta para a tela inicial de Usuários
        /// </summary>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("DadosCadastroUserControl.ascx - btnCancelar_Click"))
            {
                try
                {
                    //Limpa variável de sessão que contém os dados do usuário em edição
                    base.Encerrar();

                    //Retorna para a tela principal de Usuários
                    this.RedirecionarParaUsuarios();
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
        /// Reenvia e-mail de confirmação de e-mail
        /// </summary>
        protected void btnReenviarEmailConfirmacao_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("DadosCadastroUserControl.ascx - btnReenviarEmailConfirmacao_Click"))
            {
                try
                {
                    String emailUsuario = this.UsuarioSelecionado.Email;
                    Int32 codigoIdUsuario = this.UsuarioSelecionado.CodigoIdUsuario.Value;
                    Status1? statusUsuario = (Status1?)this.UsuarioSelecionado.Status.Codigo;
                    List<Int32> estabelecimentos = this.UsuarioSelecionado.Estabelecimentos;
                    List<Int32> estabelecimentosAux = new List<Int32>();
                    if (estabelecimentos.Any() && estabelecimentos.Count > 0)
                        estabelecimentosAux.Add(estabelecimentos.FirstOrDefault());

                    if (statusUsuario == Status1.UsuarioAguardandoConfirmacaoAlteracaoEmail)
                    {
                        UsuarioHash hashConfirmacao = this.ReiniciarHashConfirmacao(codigoIdUsuario);

                        if (hashConfirmacao != null)
                        {
                            emailUsuario = this.UsuarioSelecionado.EmailTemporario;

                            //Re-envio do e-mail de confirmação com expiração
                            EmailNovoAcesso.EnviarEmailConfirmacaoCadastro12h(emailUsuario, codigoIdUsuario,
                                hashConfirmacao.Hash, this.SessaoAtual.CodigoIdUsuario, this.SessaoAtual.Email,
                                this.SessaoAtual.NomeUsuario, this.SessaoAtual.TipoUsuario, this.SessaoAtual.CodigoEntidade,
                                this.SessaoAtual.Funcional, true, this.UsuarioSelecionado.Cpf, estabelecimentosAux.ToArray());

                            //Exibe mensagem informativo de reenvio                    
                            mvwDadosCadastro.SetActiveView(pnlAvisoExpiracao);
                        }
                    }
                    else if (statusUsuario == Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario)
                    {
                        //De acordo com a origem de criação do usuário, dispara e-mail com ou sem expiração
                        if (this.UsuarioSelecionado.OrigemAberta || this.UsuarioSelecionado.OrigemMigracao)
                        {
                            Guid hash = Guid.Empty;
                            UsuarioHash hashConfirmacao = this.ReiniciarHashConfirmacao(codigoIdUsuario);

                            if (hashConfirmacao != null)
                                hash = hashConfirmacao.Hash;

                            // recria a hash do usuário se não foi devidamente retornada
                            if (hash == Guid.Empty)
                                hash = this.RecriarHashUsuario(codigoIdUsuario, Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario, 0.5);

                            if (hash != Guid.Empty)
                            {
                                //Re-envio do e-mail de confirmação com expiração
                                EmailNovoAcesso.EnviarEmailConfirmacaoCadastro12h(emailUsuario, codigoIdUsuario,
                                    hash, SessaoAtual.CodigoIdUsuario, SessaoAtual.Email,
                                    SessaoAtual.NomeUsuario, SessaoAtual.TipoUsuario, SessaoAtual.CodigoEntidade,
                                    SessaoAtual.Funcional, true, this.UsuarioSelecionado.Cpf, estabelecimentosAux.ToArray());

                                //Exibe mensagem informativo de reenvio
                                mvwDadosCadastro.SetActiveView(pnlAvisoExpiracao);
                            }
                        }
                        else if (this.UsuarioSelecionado.OrigemFechada)
                        {
                            Guid hash = Guid.Empty;
                            UsuarioHash hashConfirmacao = this.ConsultarHashConfirmacao(codigoIdUsuario);

                            if (hashConfirmacao != null)
                                hash = hashConfirmacao.Hash;

                            // recria a hash do usuário se não foi devidamente retornada
                            if (hash == Guid.Empty)
                                hash = this.RecriarHashUsuario(codigoIdUsuario, Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario, 0.5);

                            if (hash != Guid.Empty)
                            {
                                long cpfTemp = SessaoAtual.CPF.GetValueOrDefault();
                                SessaoAtual.CPF = UsuarioSelecionado.Cpf;
                                //Re-envio do e-mail de confirmação sem expiração
                                EmailNovoAcesso.EnviarEmailConfirmacaoCadastro(
                                    SessaoAtual, emailUsuario, estabelecimentosAux, codigoIdUsuario, hash);
                                SessaoAtual.CPF = cpfTemp == 0 ? null : (long?)cpfTemp;
                                //Exibe mensagem informativo de reenvio
                                mvwDadosCadastro.SetActiveView(pnlAvisoSemExpiracao);
                            }
                        }
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
        /// Descarta as alterações e redireciona o usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDescartar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("DadosCadastroUserControl.ascx - btnDescartar_Click"))
            {
                try
                {
                    // Redireciona o usuario para a pagina da aba clicada
                    RedirecionarPassoAba();
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
        /// Salva as informacoes no banco e redireciona o usuario
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAtualizar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("DadosCadastroUserControl.ascx - btnAtualizar_Click"))
            {
                try
                {
                    //Armazena em sessão os dados digitados
                    this.SalvarDadosTemporariosUsuario();

                    Boolean emailAtualizado;
                    base.AlterarUsuario("DadosCadastro", out emailAtualizado);

                    // Redireciona o usuario para a pagina da aba clicada
                    RedirecionarPassoAba();
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
        /// Retorna para a tela de adm de usuarios
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRetornarAdm_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("DadosCadastroUserControl.ascx - btnRetornarAdm_Click"))
            {
                try
                {
                    //Limpa variável de sessão que contém os dados do usuário em edição
                    base.Encerrar();

                    //Retorna para a tela principal de Usuários
                    this.RedirecionarParaUsuarios();
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

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Configuração dos controles
        /// </summary>
        private void ConfigurarControles()
        {
            //Campos de senha são obrigatórios apenas quando em modo de Criação
            campoSenha.Required = this.ModoCriacao;
            if (!this.ModoCriacao)
            {
                campoSenha.ErrorMessageRequired = String.Empty;
            }
            else
            {
                campoSenha.Label += "*";
            }
        }

        /// <summary>
        /// Carrega os dados do objeto dos dados do usuário na tela
        /// </summary>
        private void CarregarDadosUsuario()
        {
            divTabsEdicao.Visible = true;

            txtUsuarioNome.Text = UsuarioSelecionado.Nome;
            txtUsuarioEmail.Value = UsuarioSelecionado.Email;
            txtUsuarioEmailConfirmacao.Value = UsuarioSelecionado.Email;
            txtUsuarioCpf.Value = UsuarioSelecionado.Cpf.ToString();
            campoSenha.Value = String.Empty;
            txtUsuarioEmailSecundario.Value = UsuarioSelecionado.EmailSecundario;
            txtUsuarioEmailSecundarioConfirmacao.Value = UsuarioSelecionado.EmailSecundario;
            txtUsuarioCelular.Text = CelularHelper.AplicarMascara(UsuarioSelecionado.CelularDdd, UsuarioSelecionado.CelularNumero);

            //Se usuário ainda não confirmou e-mail, exibe box permitindo reenvio de e-mail de confirmação
            Status1? statusUsuario = (Status1?)UsuarioSelecionado.Status.Codigo;
            qdConfirmacaoEmail.Visible = statusUsuario == Status1.UsuarioAguardandoConfirmacaoAlteracaoEmail
                || statusUsuario == Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario;

            //Se usuário estiver no status de Aguardando Confirmacação de Criação de Usuário,
            //desabilita todos os controles de edição
            //E se for Central de Atendimento, também não permite edição
            if ((UsuarioSelecionado.Origem == 'A' && statusUsuario == Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario)
                || this.SessaoAtual != null && this.SessaoAtual.UsuarioAtendimento)
            {
                txtUsuarioNome.Enabled = false;
                txtUsuarioEmail.Enabled = false;
                txtUsuarioEmailConfirmacao.Enabled = false;
                txtUsuarioCpf.Enabled = false;
                campoSenha.Enabled = false;
                txtUsuarioEmailSecundario.Enabled = false;
                txtUsuarioEmailSecundarioConfirmacao.Enabled = false;
                txtUsuarioCelular.Enabled = false;
                btnContinuar.Visible = false;
            }
        }

        /// <summary>
        /// Armazena os dados temporários do usuário em sessão
        /// </summary>
        private void SalvarDadosTemporariosUsuario()
        {
            UsuarioSelecionado.Nome = txtUsuarioNome.Text;
            UsuarioSelecionado.Email = txtUsuarioEmail.Value.ToLower();
            UsuarioSelecionado.Cpf = txtUsuarioCpf.Value.ToInt64Null(0);
            UsuarioSelecionado.EmailSecundario = txtUsuarioEmailSecundario.Value.ToLower();
            UsuarioSelecionado.CelularDdd = CelularHelper.CelularDdd(txtUsuarioCelular.Text);
            UsuarioSelecionado.CelularNumero = CelularHelper.CelularNumero(txtUsuarioCelular.Text);

            //Considera alteração de senha apenas quando usuário informa a senha
            if (!String.IsNullOrEmpty(campoSenha.Value))
                //Armazena a senha informada como a nova senha
                UsuarioSelecionado.Senha = campoSenha.SenhaCriptografada;
            else
                //Rearmazena a senha original
                UsuarioSelecionado.Senha = UsuarioSelecionadoOriginal.Senha;
        }

        /// <summary>
        /// Redireciona o usuario para a tela da aba que foi clicada
        /// </summary>
        private void RedirecionarPassoAba()
        {
            String abaClicada = hdfAbaClicada.Value;
            if (String.Compare(abaClicada, "estabelecimentosTab", true) == 0)
            {
                //Redireciona para o próximo passo
                base.RedirecionarPassoEstabelecimentos();
            }
            else if (String.Compare(abaClicada, "permissoesTab", true) == 0)
            {
                //Redireciona para o passo 3
                base.RedirecionarPassoPermissoes();
            }
        }

        #endregion

        #region [ Consultas Serviços ]

        /// <summary>
        /// Reinicia hash do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        /// <returns>Hash reiniciado</returns>
        private UsuarioHash ReiniciarHashConfirmacao(Int32 codigoIdUsuario)
        {
            var hash = default(UsuarioHash);

            Int32 codigoRetorno = default(Int32);

            using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                hash = ctx.Cliente.ReiniciarHash(out codigoRetorno, codigoIdUsuario);

            return hash;
        }

        /// <summary>
        /// Consulta Hash de Confirmação por Usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        private UsuarioHash ConsultarHashConfirmacao(Int32 codigoIdUsuario)
        {
            var codigoRetorno = default(Int32);
            var hashes = default(UsuarioHash[]);

            using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                hashes = ctx.Cliente.ConsultarHash(out codigoRetorno, codigoIdUsuario, null, null);

            if (codigoRetorno != 0)
                base.ExibirPainelExcecao("UsuarioServico.ConsultarHash", codigoRetorno);

            if (hashes != null && hashes.Length > 0)
                return hashes[0];
            else
                return null;
        }

        /// <summary>
        /// Método para recriação do hash de e-mail do usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Id do usuário</param>
        /// <param name="status">Status do usuário</param>
        /// <param name="diasExpiracao">Quantidade de dias de previsão para expiração do hash do e-mail</param>
        /// <returns></returns>
        private Guid RecriarHashUsuario(Int32 codigoIdUsuario, Status1 status, double diasExpiracao)
        {
            Guid hash = Guid.Empty;

            using (Logger log = Logger.IniciarLog("Chamada do métdo RecriarHashUsuario"))
            using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
            {
                try
                {
                    log.GravarMensagem("Chamando método InserirHash");
                    hash = ctx.Cliente.InserirHash(codigoIdUsuario, status, diasExpiracao, null);
                    log.GravarMensagem("Resultado método InserirHash", new { result = hash });
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
            }

            return hash;
        }

        /// <summary>
        /// Consulta Hash de Confirmação por Usuário
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        private void ExcluirHashConfirmacao(Int32 codigoIdUsuario)
        {
            var codigoRetorno = default(Int32);

            using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                codigoRetorno = ctx.Cliente.ExcluirHash(codigoIdUsuario);

            if (codigoRetorno != 0)
                base.ExibirPainelExcecao("UsuarioServico.ExcluirHash", codigoRetorno);
        }

        #endregion

        /// <summary>
        /// Customvalidator para verificar se o email ja existe para o PV se for uma filial
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        public void ValidarEmailUsuarioExistente(object source, EmailServerValidateEventArgs args)
        {
            args.IsValid = true;

            //Se for filial, valida nesta etapa se o PV já possui o e-mail informado
            if (!SessaoAtual.PVMatriz)
            {
                var usuario = default(Usuario);
                string mensagemRetorno = string.Empty;

                Boolean usuarioNovoValidacao = ValidarEmailCriacaoUsuario(args.Value, this.SessaoAtual.CodigoEntidade, out mensagemRetorno, out usuario);
                if (!usuarioNovoValidacao)
                {
                    //Usuário com mesmo e-mail não pode ser o mesmo usuário sendo editado
                    if (usuario != null && usuario.CodigoIdUsuario != UsuarioSelecionado.CodigoIdUsuario)
                    {
                        args.ErrorMessage = "Usuário já existente";
                        args.IsValid = false;
                    }
                }
            }
        }

        #region temporario
        /// <summary>
        /// Verifica o e-mail do usuário antes de prosseguir
        /// </summary>
        /// <param name="email">E-mail para validação</param>
        /// <param name="codigoEntidade">Código do estabelecimento a ser validado</param>
        /// <param name="origemHandler">Identificação da origem. TRUE: proveniente do generic handler</param>
        /// <param name="mensagemRetorno">Retorno: mensagem em formato HTML sobre o e-mail informado</param>
        /// <param name="usuario">Retorno: Usuario caso encontrado</param>
        /// <returns>TRUE: E-mail do usuário válido</returns>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        public static Boolean ValidarEmailCriacaoUsuario(String email, Int32 codigoEntidade, out String mensagemRetorno, out Usuario usuario)
        {
            Guid hash = Guid.Empty;
            mensagemRetorno = String.Empty;

            // verifica se já existe usuário com o mesmo e-mail para o PV           
            Boolean statusConsulta = false;
            usuario = UsuarioNegocio.ConsultarUsuarioPorEmail(email, codigoEntidade, out statusConsulta, out hash);

            if (usuario == null)
            {
                // não existe usuário com o mesmo e-mail, então tenta verificar se existe usuário com e-mail temporário igual para o PV
                usuario = UsuarioNegocio.ConsultarUsuarioPorEmailTemporario(email, codigoEntidade, out statusConsulta);
            }

            // se o usuário existe na base para o PV informado, impede prosseguir com o cadastro
            if (usuario != null)
            {
                switch ((Status1)usuario.Status.Codigo.Value)
                {
                    // se e-mail é de usuário Aguardando Aprovação do Master, informa que está aguardando aprovação
                    // se está na Parte Aberta, exibe link para reenvio da solicitação de aprovação para o Master
                    case Status1.UsuarioAguardandoConfirmacaoMaster:
                        mensagemRetorno = string.Format(@"
<span class=""other-text"">Usuário aguardando confirmação. 
<a href=""javascript:void(0)"" onclick=""CustomPostBack('ReenviarEmail', '{0}')"">Clique aqui</a> 
para solicitar confirmação ao usuário Master</span>",
                            String.Concat("ReenviarSolicitacaoAprovacao|", codigoEntidade));
                        break;

                    // se usuário aguardando confirmação para validação do e-mail informado no cadastro
                    case Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                        mensagemRetorno = string.Format(@"
<span class=""other-text"">Usuário aguardando confirmação. 
<a href=""javascript:void(0)"" onclick=""CustomPostBack('ReenviarEmail', '{0}')"">Clique aqui</a> 
para reenviar o e-mail de confirmação</span>",
                            String.Concat("ReenviarEmailConfirmacao|", codigoEntidade));
                        break;

                    // se usuário Ativo, informa que usuário já existe
                    // se está na Parte Aberta, exibe link para recuperação de senha
                    case Status1.UsuarioAtivo:
                    case Status1.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:
                        mensagemRetorno = @"
<span class=""other-text"">Usuário já existente. 
Utilize a <a href=""/pt-br/novoacesso/Paginas/RecuperacaoSenhaIdentificacao.aspx"">recuperação de senha</a></span>";
                        break;

                    // para qualquer outro status, informa que usuário já existe
                    default:
                        mensagemRetorno = "Atenção: Usuário já existente";
                        break;
                }

                return false;
            }

            return true;
        }
        #endregion
    }
}