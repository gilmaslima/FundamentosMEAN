using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using Redecard.PNCadastrais.Core.Web.Controles.Portal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls; 

namespace Redecard.PN.DadosCadastrais.SharePoint
{
    /// <summary>
    /// Confirmação Positiva do Novo Acesso
    /// </summary>
    public partial class ConfirmacaoPositivaNovoAcesso : UserControlBase
    {
        #region [Atributos do Controle]
        /// <summary>
        /// EventHandler para ação de Continuar da Confirmação Posivitiva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void ContinuarHandle(object sender, EventArgs args);

        /// <summary>
        /// Envento para ação de Continuar da Confirmação Positiva
        /// </summary>
        public event ContinuarHandle Continuar;

        /// <summary>
        /// Dicionário com as perguntas variáveis para serem exibidas ao usuário
        /// </summary>
        private static Dictionary<Int32, String> perguntasAleatorias = new Dictionary<Int32, String> {
            /* 
                Relação de Campos Variáveis Disponíveis 
                ------------------------------------------------------------------------------------------------------------
                ID  | Nome                                  | Ativo | Motivo
                ------------------------------------------------------------------------------------------------------------
                1   | CEP do Estabelecimento                | Sim   |                 
                2   | DDD + Telefone do Estabelecimento     | Sim   | 
                3   | Limite de Parcelas sem Juros          | Não   | Foi removida por solicitação de Canais
                4   | Domicilio Bancário de Débito          | Sim   | 
                5   | Estado e Cidade do Estabelecimento    | Não   | Não existe dados de Estado/Cidade ou Dados do Correio
                6   | Tipo do Ponto de Venda                | Não   | Foi removida por solicitação de Segurança da Informação
		        7   | Domicilio Bancário de Crédito         | Sim   |
                8   | Nr do Endereço                        | Sim   |
		        9   | Nr do Terminal                        | Sim   | Validação de uso da pergunta através do serviço do Máximo
		        10  | Alteração de domicílio                | Sim   | 
		        11  | Domínio de E-mail                     | Sim   | 
            */
            { 1, "pnlCEP" },
            { 2, "pnlTelefone" },
            { 4, "pnlDomicilioDebito" },
            { 7, "pnlDomicilioCredito" },
            { 8, "pnlNumeroResidencia" },
            { 9, "pnlTerminal" },
            { 10, "pnlAlteracaoDomicilio" },
            { 11, "pnlDominioEmail" },
        };

        /// <summary>
        /// Descrições dos grupos de informações validadas em cada pergunta básica.
        /// Utilizado para log.
        /// </summary>
        private static Dictionary<Int32, String> descricaoPerguntasBasicas = new Dictionary<Int32, String>
        {
            { 1, "CNPJ ou CPF do estabelecimento" },
            { 2, "domicílio bancário de crédito" },
            { 3, "CNPJ ou CPF de um dos sócios" },
            { 20, "domicílio bancário de crédito ou débito" }
        };

        /// <summary>
        /// Descrições dos grupos de informações validadas em cada pergunta aleatória.
        /// Utilizado para log.
        /// </summary>
        private static Dictionary<Int32, String> descricaoPerguntasAleatorias = new Dictionary<Int32, String>
        {
            { 1, "CEP do estabelecimento" },
            { 2, "DDD + telefone" },
            { 4, "domicílio bancário de débito" },
            { 7, "número do endereço" },
            { 8, "alteração de domicílio bancário" },
            { 9, "domínio de e-mail corporativo" },
            { 10, "número do terminal" }
        };

        /// <summary>
        /// Identificação da ViewState que indica se a Confirmação Positiva é do tipo Completa
        /// </summary>
        private String bloquearUsuario = "BloquearUsuario";

        /// <summary>
        /// <para>True - Indica que a Confirmação Positiva bloqueará o USUÁRIO</para>
        /// <para>em caso de exceder o limite de tentativas.</para>
        /// <para>False - Indica que a Confirmação Positiva bloqueará a ENTIDADE</para>
        /// <para>em caso de exceder o limite de tentativas.</para>
        /// </summary>
        public Boolean BloquearUsuario
        {
            get
            {
                if (!object.ReferenceEquals(ViewState[bloquearUsuario], null))
                {
                    return (Boolean)ViewState[bloquearUsuario];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                ViewState[bloquearUsuario] = value;
            }
        }

        /// <summary>
        /// Identificação da ViewState que indica se a Confirmação Positiva é do tipo Completa
        /// </summary>
        private String sessaoAberto = "ObterSessaoAberto";

        /// <summary>
        /// <para>True - Indica que a sessão a ser ultizada é do </para>
        /// <para>tipo InformacaoUsuario (sessão de usuário não logado).</para>
        /// <para>False - Indica que a sessão a ser ultizada é do fechado</para>
        /// </summary>
        public Boolean ObterSessaoAberto
        {
            get
            {
                if (!object.ReferenceEquals(ViewState[sessaoAberto], null))
                {
                    return (Boolean)ViewState[sessaoAberto];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                ViewState[sessaoAberto] = value;
            }
        }

        /// <summary>
        /// Identificação da ViewState que indica se a Confirmação Positiva é do tipo Completa
        /// </summary>
        private String confirmacaoCompleta = "ConfirmacaoCompleta";

        /// <summary>
        /// Verifica se a tela irá funcionar a com a Confirmação Positiva Completa ou Básica
        /// </summary>
        public Boolean ConfirmacaoCompleta
        {
            get
            {
                if (!object.ReferenceEquals(ViewState[confirmacaoCompleta], null))
                {
                    return (Boolean)ViewState[confirmacaoCompleta];
                }
                else
                {
                    return false;
                }
            }
            set
            {
                ViewState[confirmacaoCompleta] = value;
            }
        }

        /// <summary>
        /// Quantidade de tentativas para confirmação positiva
        /// </summary>
        private String qtdTentativas = "QtdTentativas";

        /// <summary>
        /// Verifica se a tela irá funcionar a com a Confirmação Positiva Completa ou Básica
        /// </summary>
        public Int32 QtdTentativas
        {
            get
            {
                if (!object.ReferenceEquals(ViewState[qtdTentativas], null))
                {
                    return (Int32)ViewState[qtdTentativas];
                }
                else
                {
                    //Valor padrão de tentativas
                    return 6;
                }
            }
            set
            {
                ViewState[qtdTentativas] = value;
            }
        }

        /// <summary>
        /// Quantidade de tentativas para confirmação positiva
        /// </summary>
        private String recuperacaoUsuario = "RecuperacaoUsuario";

        /// <summary>
        /// Verifica se a Recuperação é de Usuário ou Senha
        /// </summary>
        public Boolean RecuperacaoUsuario
        {
            get
            {
                if (!object.ReferenceEquals(ViewState[recuperacaoUsuario], null))
                {
                    return (Boolean)ViewState[recuperacaoUsuario];
                }
                else
                {
                    //Valor padrão pro indicador
                    return false;
                }
            }
            set
            {
                ViewState[recuperacaoUsuario] = value;
            }
        }

        /// <summary>
        /// Tela já exibiu a pergunta com a Alteração de Domicílio
        /// </summary>
        private String paresDomicilio = "ParesDomiclio";

        /// <summary>
        /// Verificar se a tela já exibiu a pergunta com a Alteração de Domicílio
        /// </summary>
        public List<Int32> PerguntasParesAlteracaoDomicilio
        {
            get
            {
                if (!object.ReferenceEquals(Session[paresDomicilio], null))
                {
                    return (List<Int32>)Session[paresDomicilio];
                }
                else
                {
                    //Valor padrão
                    return new List<int>();
                }
            }
            set
            {
                Session[paresDomicilio] = value;
            }
        }

        /// <summary>
        /// Controle PainelMensagem na página
        /// </summary>
        public PainelMensagem PainelMensagem
        {
            get
            {
                return (PainelMensagem)this.pnMensagem;
            }
        }

        /// <summary>
        /// Define um subtítulo customizado para a página
        /// </summary>
        public String PageSubTitle
        {
            get
            {
                return this.ltrSubtitle.Text;
            }
            set
            {
                this.ltrSubtitle.Text = value;
                this.phSubtitle.Visible = !String.IsNullOrEmpty(value);
            }
        }

        /// <summary>
        /// Verifica se deve validar a senha no submit
        /// </summary>
        [DefaultValue(false)]
        public Boolean ValidaSenhaSubmit
        {
            get
            {
                return this.phSenhaConfirmacao.Visible;
            }
            set
            {
                this.phSenhaConfirmacao.Visible = value;
            }
        }

        /// <summary>
        /// Obtém a senha informada durante a confirmação positiva
        /// </summary>
        public String SenhaValidacao
        {
            get
            {
                return this.txtSenhaAtual.Text;
            }
        }

        #endregion

        #region [Eventos da página]

        /// <summary>
        /// Page_Load Inicializando o controle de confirmação positiva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // verifica se a página está sendo acessada por administrador para edição de conteúdo
                #region edição de conteúdo
                if (SPContext.Current.Site.RootWeb.CurrentUser != null
                    && SPContext.Current.Site.RootWeb.CurrentUser.IsSiteAdmin
                    || SPContext.Current.FormContext.FormMode == SPControlMode.Edit)
                {
                    return;
                }
                #endregion

                if (!Page.IsPostBack)
                {
                    if (!this.ObterSessaoAberto)
                    {
                        if (VerificarConfirmacaoPositiaBasico() && base.EntidadePossuiMaster())
                        {
                            pnlAcessoNegado.Visible = true;
                            pnlConfirmacaoPositiva.Visible = false;
                            phSubtitle.Visible = false;
                            pnlMensagem.Visible = false;
                            return;
                        }
                    }
                    InicializarConfirmacaoPositiva();
                }
            }
            catch (NullReferenceException ex)
            {
                this.ExibirErro("Atenção", ex.Message, false, PainelMensagemIcon.Erro);
            }
            catch (Exception ex)
            {
                this.ExibirErro("Atenção", ex.Message, false, PainelMensagemIcon.Erro);
            }
        }

        /// <summary>
        /// Sobrescreve o Onload do Base para nao validar as permissoes
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            this.ValidarPermissao = false;
            base.OnLoad(e);
        }

        /// <summary>
        /// Inicia a confirmação positiva.
        /// </summary>
        private void InicializarConfirmacaoPositiva()
        {
            using (Logger log = Logger.IniciarNovoLog("Page_Load Iniciliazando a o controle de confirmação positiva"))
            {
                try
                {
                    if (!this.ConfirmacaoPositivaBloqueada())
                    {
                        if (this.PerguntasParesAlteracaoDomicilio.Count == 0)
                            this.PerguntasParesAlteracaoDomicilio = new List<int>();

                        pnlMensagem.Visible = false;
                        pnlConfirmacaoPositiva.Visible = true;
                        phSubtitle.Visible = true;

                        this.MontarConfirmacaoPositiva();
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, base.RecuperarEnderecoPortal());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, base.RecuperarEnderecoPortal());
                }
            }
        }

        /// <summary>
        /// Marca a flag de exibição de liberação de acesso completo para false
        /// </summary>
        public void AtualizarFlagExibicaoMensagemAcessoCompleto()
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
                            .AtualizarFlagExibicaoMensagemLiberacaoAcessoCompleto(this.SessaoAtual.CodigoIdUsuario, false);
                        if (codigoRetorno > 0)
                        {
                            Logger.GravarErro("Erro durante atualização de flag de exibição de mensagem de acesso completo",
                                new Exception("Erro durante atualização de flag de exibição de mensagem de acesso completo"),
                                new { this.SessaoAtual.CodigoIdUsuario, codigoRetorno });
                        }
                        else
                        {
                            this.SessaoAtual.ExibirMensagemLiberacaoAcessoCompleto = false;
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
        /// Evento de Confirmação da Ação da Confirmação Positiva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void ContinuarConfirmacaoPositiva(Object sender, EventArgs args)
        {
            using (Logger log = Logger.IniciarLog("Confirmação da Ação da Confirmação Positiva"))
            {
                try
                {
                    // valida se os campos estão validados
                    if (!this.ValidateFields())
                    {
                        return;
                    }

                    if (this.ValidarConfirmacaoPositiva())
                    {
                        if (Continuar != null)
                            Continuar(sender, null);
                    }
                    else
                    {
                        using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                            {
                                Int32 codigoRetornoBloqueio = 0;
                                Int32 idUsuario = 0;
                                String emailUsuario = String.Empty;
                                UsuarioServico.Entidade entidade;

                                #region [Identificação do Usuário]

                                if (this.ObterSessaoAberto)
                                {
                                    InformacaoUsuario info = InformacaoUsuario.Recuperar();

                                    entidade = new UsuarioServico.Entidade()
                                    {
                                        GrupoEntidade = new UsuarioServico.GrupoEntidade()
                                        {
                                            Codigo = info.GrupoEntidade
                                        },
                                        Codigo = info.NumeroPV
                                    };

                                    idUsuario = info.IdUsuario;
                                    emailUsuario = info.EmailUsuario;
                                }
                                else
                                {
                                    entidade = new UsuarioServico.Entidade()
                                    {
                                        GrupoEntidade = new UsuarioServico.GrupoEntidade()
                                        {
                                            Codigo = SessaoAtual.GrupoEntidade
                                        },
                                        Codigo = SessaoAtual.CodigoEntidade
                                    };

                                    idUsuario = SessaoAtual.CodigoIdUsuario;
                                    emailUsuario = SessaoAtual.Email;
                                }
                                #endregion

                                var usuario = contextoUsuario.Cliente.ConsultarPorID(out codigoRetornoBloqueio, idUsuario);
                                Guid hashEmail = Guid.Empty;
                                Int32 codigoRetorno = 0;

                                if (usuario != null)
                                {
                                    if (this.ModoLiberacaoAcesso() || this.ModoRecuperacaoUsuarioSenha())
                                    {

                                        if (usuario.QuantidadeTentativaConfirmacaoPositiva >= this.QtdTentativas)
                                        {
                                            codigoRetorno = contextoUsuario.Cliente.AtualizarStatus(out hashEmail, usuario.CodigoIdUsuario, UsuarioServico.Status1.UsuarioAtivoLiberacaoAcessoCompletoBloqueada, 2, DateTime.Now);

                                            this.ExibirAvisoBloqueioSenhaUsuario("Acesso completo bloqueado", PainelMensagemIcon.Erro);

                                            InformacaoUsuario.Limpar();
                                        }
                                        else //Usuário ainda possui tentativas para confirmação positiva
                                        {
                                            Int32 quantidadeRetorno = 0;
                                            String mensagem = String.Empty;
                                            quantidadeRetorno = RecuperarQuantidadeTentivas(out mensagem);
                                            if (quantidadeRetorno > 0)
                                            {
                                                this.ExibirErro("Os dados informados estão incorretos.", mensagem, true, PainelMensagemIcon.Aviso);
                                            }
                                        }
                                        return;
                                    }
                                    if (this.ObterSessaoAberto) //Recuperação Senha/Usuário
                                    {
                                        if (usuario.QuantidadeTentativaConfirmacaoPositiva >= this.QtdTentativas)
                                        {
                                            codigoRetorno = contextoUsuario.Cliente.AtualizarStatus(
                                                          out hashEmail,
                                                            usuario.CodigoIdUsuario,
                                                            this.RecuperacaoUsuario ? UsuarioServico.Status1.UsuarioBloqueadoRecuperacaoUsuario
                                                                                    : UsuarioServico.Status1.UsuarioBloqueadoRecuperacaoSenha,
                                                            2,
                                                            DateTime.Now);

                                            this.ExibirAvisoBloqueioSenhaUsuario("Acesso completo bloqueado", PainelMensagemIcon.Erro);

                                            InformacaoUsuario.Limpar();
                                        }
                                        else //Usuário ainda possui tentativas para confirmação positiva
                                        {
                                            Int32 quantidadeRetorno = 0;
                                            String mensagem = String.Empty;
                                            quantidadeRetorno = RecuperarQuantidadeTentivas(out mensagem);
                                            if (quantidadeRetorno > 0)
                                            {
                                                this.ExibirErro("Os dados informados estão incorretos.", mensagem, true, PainelMensagemIcon.Aviso);
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, String.Empty);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, String.Empty);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, String.Empty);
                }
            }
        }

        /// <summary>
        /// Voltar a exibir o formulário de Confirmação Positiva e esconde o painel de aviso
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Voltar a exibir o formulário de Confirmação Positiva e esconde o painel de aviso"))
            {
                try
                {
                    pnlMensagem.Visible = false;
                    pnlConfirmacaoPositiva.Visible = true;
                    phSubtitle.Visible = true;
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
        }

        /// <summary>
        /// Cancelar a tentativa de confirmação positiva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Cancelar a tentativa de confirmação positiva"))
            {
                try
                {
                    Response.Redirect(base.RecuperarEnderecoPortal(), false);
                }
                catch (HttpException ex)
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
        /// Tentar Responder novamente a Confirmação Positiva com novas perguntas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnTentarNovamente_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Tentar Responder novamente a Confirmação Positiva com novas perguntas"))
            {
                try
                {
                    if (this.ModoLiberacaoAcesso())
                    {
                        pnlConfirmacaoPositiva.Visible = true;
                        phSubtitle.Visible = true;
                        pnlMensagem.Visible = false;

                        this.MontarConfirmacaoPositiva();
                    }
                    else
                        Response.Redirect(Request.Url.AbsoluteUri, false);

                }
                catch (HttpException ex)
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
        /// Evento para validação server-side do campo crvSenhaAtual.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void crvSenhaAtual_ServerValidate(object source, CustomServerValidateEventArgs args)
        {
            if (this.VerificarSenhaAtual(txtSenhaAtual.Text) || SessaoAtual.Legado || !this.ValidaSenhaSubmit)
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

        #region [Métodos de página]

        /// <summary>
        /// Monta a tela de acordo com o tipo de confirmação positiva
        /// </summary>
        public void MontarConfirmacaoPositiva()
        {
            if (!ConfirmacaoPositivaBloqueada())
            {
                CarregarBancos();
                EsconderTodasPerguntas();
#if DEBUG
                if (String.Compare(Request.QueryString["debug"], "true", true) == 0)
                {
                    // exibe as perguntas conforme passado na URL (ou todas se nenhuma form especificada)
                    String[] showQuestions = new String[0];
                    if (!String.IsNullOrWhiteSpace(Request.QueryString["questions"]))
                        showQuestions = Request.QueryString["questions"].Split(',');
                    if (showQuestions.Length > 0)
                    {
                        foreach (KeyValuePair<Int32, String> pergunta in perguntasAleatorias)
                        {
                            String nomeControle = pergunta.Value;
                            Panel pnl = this.FindControl(nomeControle) as Panel;
                            pnl.Visible = String.Compare(showQuestions[0], "0") == 0 || showQuestions.Contains(pergunta.Key.ToString());

                            // exibe as labels filhas
                            pnl.Controls.OfType<Label>().ToList().ForEach(x => x.Visible = pnl.Visible);
                        }
                    }
                    else
                    {
                        ExibirValidacoesConfirmacao();
                    }

                    // exibe controles individualmente
                    pnlMensagem.Visible = false;
                    pnlAcessoNegado.Visible = false;
                    if (!String.IsNullOrWhiteSpace(Request.QueryString["idcontrols"]))
                    {
                        String[] showControlId = Request.QueryString["idcontrols"].Split(',');
                        if (showControlId.Length > 0)
                        {
                            // oculta todos os containers
                            pnlConfirmacaoPositiva.Visible = false;
                            phSubtitle.Visible = false;

                            foreach (var controlId in showControlId)
                            {
                                if (!String.IsNullOrWhiteSpace(controlId))
                                {
                                    Control c = this.FindControl(controlId);
                                    if (c != null)
                                    {
                                        c.Visible = true;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    ExibirValidacoesConfirmacao();
                }
#else
                ExibirValidacoesConfirmacao();
#endif
            }
        }

        /// <summary>
        /// Consulta os bancos e preenche combo
        /// </summary>
        private void CarregarBancos()
        {
            using (Logger log = Logger.IniciarLog("Consultando bancos e preenchendo combos"))
            using (var contexto = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
            {
                EntidadeServico.Banco[] bancos = contexto.Cliente.ConsultarBancosConfirmacaoPositiva();
                foreach (EntidadeServico.Banco banco in bancos)
                {
                    ListItem item = new ListItem(
                        String.Format("{0} - {1}", banco.Codigo.PadLeft(3, '0'), banco.Descricao), 
                        banco.Codigo);
                    ddlBanco.Items.Add(item);
                    ddlBancoDebito.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Formata o domicílio bancário para validação na procedure
        /// </summary>
        /// <param name="banco">Código do Banco</param>
        /// <param name="agencia">Número da Agência</param>
        /// <param name="contacorrente">Número da Conta Corrente</param>
        /// <param name="debito">Identificação do tipod e conta (true: débito / false: crédito)</param>
        /// <returns>Domicílio bancário formatado em uma String para validação</returns>
        public string FormatarDomicilioBancario(String banco, String agencia, String contacorrente, Boolean debito)
        {
            // Regra de Conta Corrente para quando o banco selecionado for
            // caixa economica federal, as c/c das caixa tem sempre 10 posições, começando
            // por 3 (Pessoa Jurídica) e 1 (Pessoa Física)
            if (!String.IsNullOrEmpty(banco) && banco == "104") // CEF
            {
                contacorrente = contacorrente.Trim();
                if (contacorrente.Length < 10)
                {
                    contacorrente = 
                        debito ? 
                        ddlOperacaoDebito.SelectedValue + AtribuirZerosEsquerda(contacorrente, 9) : 
                        ddlOperacao.SelectedValue + AtribuirZerosEsquerda(contacorrente, 9);
                    
                    // caso o dígito seja composto de alguma letra, substitui a mesma por 0.
                    contacorrente = Regex.Replace(contacorrente, @"[a-zA-Z]+", "0");
                }
            }

            string bancoFormatado = (banco.Length < 3 ? this.AtribuirZerosEsquerda(banco, 3) : banco);
            string agenciaFormatada = (agencia.Length < 5 ? this.AtribuirZerosEsquerda(agencia, 5) : agencia);
            string contaCorrenteFormatada = (contacorrente.Length < 15 ? this.AtribuirZerosEsquerda(contacorrente, 15) : contacorrente);

            return String.Format("{0}{1}{2}", bancoFormatado, agenciaFormatada, contaCorrenteFormatada);
        }

        /// <summary>
        /// Verifica se a Entidade possui POO ou POS através do máximo
        /// </summary>
        /// <param name="numPdv">Número do PV</param>
        /// <returns>
        /// <para>True - Caso possua</para>
        /// <para>False - Caso não possua</para>
        /// </returns>
        private Boolean PossuiPooPos(Int32 numPdv)
        {
            Boolean possui = false;

            using (Logger log = Logger.IniciarLog("Verificar se a Entidade possui POO ou POS através do máximo"))
            {
                try
                {
                    using (MaximoServico.MaximoServicoClient maximoServico = new MaximoServico.MaximoServicoClient())
                    {
                        MaximoServico.FiltroTerminal filtro = new MaximoServico.FiltroTerminal();
                        filtro.PontoVenda = numPdv.ToString();
                        filtro.Situacao = MaximoServico.TipoTerminalStatus.EMPRODUCAO;
                        filtro.TipoEquipamento = "POO";

                        List<MaximoServico.TerminalConsulta> equipamentosPoo = maximoServico.ConsultarTerminal(filtro);

                        filtro.TipoEquipamento = "POS";
                        List<MaximoServico.TerminalConsulta> equipamentosPos = maximoServico.ConsultarTerminal(filtro);

                        if (equipamentosPoo.Count > 0 || equipamentosPos.Count > 0)
                            possui = true;
                    }
                }
                catch (FaultException<MaximoServico.GeneralFault> ex)
                {
                    possui = false;
                    log.GravarMensagem("Ocorreu um erro ao consultar no Maximo", new { numPdv });
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    possui = false;
                    log.GravarMensagem("Ocorreu um erro ao consultar no Maximo", new { numPdv });
                    log.GravarErro(ex);
                }
            }

            return possui;
        }

        /// <summary>
        /// Pergunta já fez par com a pergunta de alteração de domicílio
        /// </summary>
        /// <param name="idPergunta"></param>
        /// <returns></returns>
        private Boolean PossuiParAlteracaoDomicilio(Int32 idPergunta)
        {
            Boolean possui = false;

            if (this.PerguntasParesAlteracaoDomicilio.Contains(idPergunta))
                possui = true;

            return possui;
        }

        /// <summary>
        /// Valida se ainda é possível formar par de perguntas com o campo de Domicílio Alterado
        /// </summary>
        /// <param name="perguntasPermitidas">Lista de perguntas permitidas para Entidade</param>
        /// <returns></returns>
        private List<EntidadeServico.Pergunta> ValidarPerguntasPossiveisAlteracaoDomicilio(List<EntidadeServico.Pergunta> perguntasPermitidas)
        {
            Boolean possivel = false;

            EntidadeServico.Pergunta pergunta = new EntidadeServico.Pergunta();

            //CEP
            pergunta = perguntasPermitidas.Find(p => p.CodigoPergunta == 1);
            if (pergunta != null
                && !PossuiParAlteracaoDomicilio(1))
                possivel = true;

            //Telefone
            pergunta = perguntasPermitidas.Find(p => p.CodigoPergunta == 2);
            if (pergunta != null
                && !PossuiParAlteracaoDomicilio(2))
                possivel = true;


            //Nr do Endereço
            pergunta = perguntasPermitidas.Find(p => p.CodigoPergunta == 8);
            if (pergunta != null
                && !PossuiParAlteracaoDomicilio(8))
                possivel = true;

            //Terminal
            pergunta = perguntasPermitidas.Find(p => p.CodigoPergunta == 9);
            if (pergunta != null
                && !PossuiParAlteracaoDomicilio(9))
                possivel = true;

            //Domínio Email
            pergunta = perguntasPermitidas.Find(p => p.CodigoPergunta == 11);
            if (pergunta != null
                && !PossuiParAlteracaoDomicilio(11))
                possivel = true;

            if (!possivel)
            {
                //Remove a pergunta de Alteração caso 
                pergunta = perguntasPermitidas.Find(p => p.CodigoPergunta == 10);
                perguntasPermitidas.Remove(pergunta);
            }

            return perguntasPermitidas;
        }

        /// <summary>
        /// Atribui zeros a esquerda do valor
        /// </summary>
        private string AtribuirZerosEsquerda(String original, Int32 numeroTotal)
        {
            string format = "000000000000000";
            int number = numeroTotal - original.Length;
            return original.Insert(0, format.Substring(0, number));
        }

        /// <summary>
        /// Exibe duas propriedades aleatórias de validação da Confirmação Positiva
        /// </summary>
        private void ExibirValidacoesConfirmacao()
        {
            using (Logger log = Logger.IniciarLog("Exibindo campos de validação"))
            {
                try
                {
                    Int32 numeroPv = 0;
                    Boolean empIbba = false;
                    Boolean entidadeEmpresa = false;

                    #region [Identificando os dados da Sessão]
                    if (this.ObterSessaoAberto)
                    {
                        if (InformacaoUsuario.Existe())
                        {
                            this.VerificarDadosSessao();
                            InformacaoUsuario info = InformacaoUsuario.Recuperar();

                            numeroPv = info.NumeroPV;
                            empIbba = info.EstabelecimentoEmpIbba;
                            entidadeEmpresa = info.Empresa;
                        }
                        else
                        {
                            this.ExibirErro("SharePoint.ConfirmacaoPositiva", 1051, "Atenção", String.Empty, base.RecuperarEnderecoPortal());
                            return;
                        }
                    }
                    else
                    {
                        if (base.SessaoAtual != null)
                        {
                            numeroPv = SessaoAtual.CodigoEntidade;
                            empIbba = !SessaoAtual.CodigoSegmento.ToString().ToLower().Equals("v");
                            entidadeEmpresa = (SessaoAtual.CNPJEntidade.Length > 11); //Verifica se é um CNPJ
                        }
                        else
                        {
                            this.ExibirErro("SharePoint.ConfirmacaoPositiva", 1051, "Atenção", String.Empty, base.RecuperarEnderecoPortal());
                            return;
                        }
                    }
                    #endregion

                    using (var client = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        pnlMensagem.Visible = false;
                        pnlConfirmacaoPositiva.Visible = true;
                        phSubtitle.Visible = true;

                        EntidadeServico.Pergunta[] perguntas = client.Cliente.ConsultarPerguntasAleatorias(numeroPv); //info.NumeroPV

                        Int16 numeroPergunta = 1;

                        List<EntidadeServico.Pergunta> perguntasTemp = new List<EntidadeServico.Pergunta>();
                        perguntasTemp.AddRange(perguntas);

                        #region [Confirmação Positiva Básica]
                        if (!this.ConfirmacaoCompleta)
                        {
                            // Exibir Domicilio Bancário de Crédito/Débito se Houver um deles
                            EntidadeServico.Pergunta pergunta = perguntasTemp.Find(i => i.CodigoPergunta == 7 || i.CodigoPergunta == 4);
                            if (!Object.ReferenceEquals(pergunta, null))
                            {
                                numeroPergunta++;
                                pnlDomicilioCredito.Visible = true;
                                lblPerguntaDomicilios.Visible = true;
                            }

                            lblNumeroPerguntaSocio.Text = numeroPergunta.ToString();
                            pnlCPFSocio.Visible = true;
                            lblCPFSocioConfirmacaoBasica.Visible = true;
                            //rfvCPFSocio.Enabled = true;
                        }
                        #endregion
                        #region [Confirmação Positiva Completa]
                        else
                        {
                            if (this.ObterSessaoAberto || ModoRecuperacaoUsuarioSenha())
                            {
                                // Exibir Domicilio Bancário de Crédito se Houver
                                EntidadeServico.Pergunta pergunta = perguntasTemp.Find(i => i.CodigoPergunta == 7);
                                if (!Object.ReferenceEquals(pergunta, null))
                                {
                                    pnlDomicilioCredito.Visible = true;
                                    lblPerguntaDomicilioCredito.Visible = true;

                                    perguntasTemp.Remove(pergunta);
                                }

                                pergunta = perguntasTemp.Find(i => i.CodigoPergunta == 4);
                                pnlDomicilioDebito.Visible = (!Object.ReferenceEquals(pergunta, null) && !pnlDomicilioCredito.Visible);
                                perguntasTemp.Remove(pergunta);

                                //pnlCPFSocio.Visible = true;
                                //lblCPFSocioConfirmacaoBasica.Visible = true;
                                //rfvCPFSocio.Enabled = true;

                                //Possui Terminal e é EMP/IBBA? Se for Varejo, remover pergunta (só deve exibir para Varejo)
                                pergunta = perguntasTemp.Find(i => i.CodigoPergunta == 9);
                                if (!Object.ReferenceEquals(pergunta, null) && (!this.PossuiPooPos(numeroPv) || empIbba))
                                    perguntasTemp.Remove(pergunta);

                                //É EMP/IBBA? Se não for, remover a pergunta de e-mail
                                pergunta = perguntasTemp.Find(i => i.CodigoPergunta == 11);
                                if (!Object.ReferenceEquals(pergunta, null) && !empIbba)
                                    perguntasTemp.Remove(pergunta);

                                if (perguntasTemp.Count > 1)
                                {
                                    perguntasTemp = this.ValidarPerguntasPossiveisAlteracaoDomicilio(perguntasTemp);

                                    this.ExibirCamposAleatorios(perguntasTemp, numeroPv, empIbba);
                                }

                                this.HabilitarPerguntas();
                            }
                            else
                            {
                                // Exibir CEP do Estabelecimento se existir
                                EntidadeServico.Pergunta pergunta = perguntasTemp.Find(i => i.CodigoPergunta == 1);
                                if (!Object.ReferenceEquals(pergunta, null))
                                {
                                    pnlCEP.Visible = true;
                                    perguntasTemp.Remove(pergunta);
                                }

                                //Exibir DDD + Telefone do Estabelecimento se existir
                                pergunta = perguntasTemp.Find(i => i.CodigoPergunta == 2);
                                if (!Object.ReferenceEquals(pergunta, null))
                                {
                                    pnlTelefone.Visible = true;
                                    perguntasTemp.Remove(pergunta);
                                }

                                //Remover as perguntas de Domicílio Bancário

                                int[] perguntasExcluidas = new int[] { 4, 7 };
                                perguntasTemp.RemoveAll(p => perguntasExcluidas.Contains(p.CodigoPergunta));

                                //pnlCPFSocio.Visible = true;
                                //lblCPFSocioConfirmacaoBasica.Visible = true;
                                //rfvCPFSocio.Enabled = true;

                                //Possui Terminal e é EMP/IBBA? Se for Varejo, remover pergunta (só deve exibir para Varejo)
                                pergunta = perguntasTemp.Find(i => i.CodigoPergunta == 9);
                                if (!Object.ReferenceEquals(pergunta, null) && (!this.PossuiPooPos(numeroPv) || empIbba))
                                    perguntasTemp.Remove(pergunta);

                                //É EMP/IBBA? Se não for, remover a pergunta de e-mail
                                pergunta = perguntasTemp.Find(i => i.CodigoPergunta == 11);
                                if (!Object.ReferenceEquals(pergunta, null) && !empIbba)
                                    perguntasTemp.Remove(pergunta);

                                if (perguntasTemp.Count > 1)
                                {
                                    perguntasTemp = this.ValidarPerguntasPossiveisAlteracaoDomicilio(perguntasTemp);

                                    this.ExibirCamposAleatorios(perguntasTemp, numeroPv, empIbba);
                                }

                                this.HabilitarPerguntas();
                            }
                        }
                        #endregion

                        // define a exibição do container de confirmação básica mediante o status de visibilidade dos filhos,
                        // para não existir margin-top desnecessária na página
                        pnlConfirmacaoBasica.Visible = pnlDomicilioCredito.Visible || pnlDomicilioDebito.Visible || pnlCPFSocio.Visible;
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, base.RecuperarEnderecoPortal());
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, base.RecuperarEnderecoPortal());
                }
                catch (FaultException<MaximoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, base.RecuperarEnderecoPortal());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, base.RecuperarEnderecoPortal());
                }
            }
        }

        /// <summary>
        /// Exibir os campos aleatórios da Confirmação Positiva
        /// </summary>
        /// <param name="perguntas">Listagem de perguntas possíveis para o PV</param>
        /// <param name="numeroPv"></param>
        /// <param name="empIbba"></param>
        private void ExibirCamposAleatorios(List<EntidadeServico.Pergunta> perguntas, Int32 numeroPv, Boolean empIbba)
        {
            using (Logger log = Logger.IniciarLog("Exibir os campos aleatórios da Confirmação Positiva"))
            {
                Int32 idControleAnterior = 0;

                Random r = new Random();
                Int32 maxValue = perguntas.Count;

                Int32 panelIndex = 0;
                Int32 idControle = 0;
                String nomeControle = String.Empty;

                try
                {
                    panelIndex = r.Next(maxValue);
                    idControle = perguntas[panelIndex].CodigoPergunta;

                    nomeControle = perguntasAleatorias[idControle];
                    Panel pnl = this.FindControl(nomeControle) as Panel;

                    if (!object.ReferenceEquals(pnl, null))
                    {
                        if ((pnl.Visible)
                            || (idControle == 8 && !pnlCEP.Visible.Equals(true)) //Número do endereço
                            || (idControle == 9 && (!this.PossuiPooPos(numeroPv) || empIbba)) //Número do terminal
                            || (idControle == 11 && !empIbba) //Domínio de e-mail
                            || (idControle == 10 && this.PossuiParAlteracaoDomicilio(idControleAnterior)) //Alteração de Domicílio
                            || (idControleAnterior == 10 && this.PossuiParAlteracaoDomicilio(idControle)))  //Alteração de Domicílio
                        {
                            pnl.Visible = true;
                        }
                        else
                        {
                            pnl.Visible = true;

                            if (idControleAnterior == 10)
                                this.PerguntasParesAlteracaoDomicilio.Add(idControle);
                            else if (idControle == 10 && idControleAnterior != 0)
                                this.PerguntasParesAlteracaoDomicilio.Add(idControleAnterior);

                            idControleAnterior = idControle;
                        }
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarMensagem("Erro ao exibir a pergunta.", new { panelIndex, nomeControle, idControle });
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarMensagem("Erro ao exibir a pergunta.", new { panelIndex, nomeControle, idControle });
                    log.GravarErro(ex);
                }
            }
        }

        /// <summary>
        /// Numera e ativa os validadores das perguntas aleatórias exibidas de acordo com a ordem das perguntas exibidas
        /// </summary>
        private void HabilitarPerguntas()
        {
            Int32 numeroPergunta = 1;

            if (pnlDomicilioCredito.Visible)
                numeroPergunta++;

            if (pnlDomicilioDebito.Visible)
            {
                //rfvAgenciaDebito.Enabled = true;
                //rfvBancoDebito.Enabled = true;
                //rfvContaDebito.Enabled = true;

                lblNumPerguntaDomicilioDebito.Text = numeroPergunta.ToString();
                numeroPergunta++;
            }

            if (pnlCPFSocio.Visible)
            {
                //rfvCPFSocio.Enabled = true;

                lblNumeroPerguntaSocio.Text = numeroPergunta.ToString();
                numeroPergunta++;
            }

            if (pnlCEP.Visible)
            {
                //rfvCep.Enabled = true;

                lblNumeroPerguntaCep.Text = numeroPergunta.ToString();
                numeroPergunta++;
            }

            if (pnlNumeroResidencia.Visible)
            {
                //rfvNumeroResidencia.Enabled = true;
                lblNumeroEndereco.Text = numeroPergunta.ToString();
                numeroPergunta++;
            }

            if (pnlTelefone.Visible)
            {
                //rfvTelefone.Enabled = true;

                lblNumeroPerguntaTelefone.Text = numeroPergunta.ToString();
                numeroPergunta++;
            }

            if (pnlTerminal.Visible)
            {
                //rfvTerminal.Enabled = true;

                lblNumeroPerguntaTerminal.Text = numeroPergunta.ToString();
                numeroPergunta++;
            }

            if (pnlAlteracaoDomicilio.Visible)
            {
                lblNumeroPerguntaAlteracaoDomicilio.Text = numeroPergunta.ToString();
                numeroPergunta++;
            }

            if (pnlDominioEmail.Visible)
            {
                //rfvDominioEmail.Enabled = true;

                lblNumeroPergundaEmail.Text = numeroPergunta.ToString();
                numeroPergunta++;
            }
        }

        /// <summary>
        /// Esconde e limpa todas as perguntas exibidas
        /// </summary>
        private void EsconderTodasPerguntas()
        {
            foreach (KeyValuePair<Int32, String> pergunta in perguntasAleatorias)
            {
                String nomeControle = pergunta.Value;
                Panel pnl = this.FindControl(nomeControle) as Panel;
                if (!object.ReferenceEquals(pnl, null))
                {
                    pnl.Visible = false;
                }
            }

            txtAgencia.Text = String.Empty;
            txtAgenciaDebito.Text = String.Empty;
            txtCep.Text = String.Empty;
            txtContaCorrente.Text = String.Empty;
            txtContaCorrenteDebito.Text = String.Empty;
            txtCPFSocio.Text = String.Empty;
            txtDominioEmail.Text = String.Empty;
            txtNumeroResidencia.Text = String.Empty;
            txtNumeroTerminal.Text = String.Empty;
            txtTelefone.Text = String.Empty;
            rblAlteracaoDomicilio.SelectedIndex = -1;
            ddlBanco.SelectedIndex = 0;
            ddlBancoDebito.SelectedIndex = 0;
            chkSemNumero.Checked = false;
        }

        /// <summary>
        /// Valida os dados preenchidos da Confirmação Positiva
        /// </summary>
        /// <returns></returns>
        private Boolean ValidarConfirmacaoPositiva()
        {
            using (Logger log = Logger.IniciarLog("Validando campos variáveis"))
            {
                try
                {
                    Int32 numeroPV = 0;
                    Int32 idUsuario = 0;

                    if (this.ObterSessaoAberto)
                    {
                        if (InformacaoUsuario.Existe())
                        {
                            InformacaoUsuario usuario = InformacaoUsuario.Recuperar();
                            numeroPV = usuario.NumeroPV;
                            idUsuario = usuario.IdUsuario;
                        }
                        else
                        {
                            // Não é possível a abertura da tela de confirmação positiva manualmente, tente efetuar login novamente.
                            this.ExibirErro("SharePoint.ConfirmacaoPositiva", 1051, "Atenção", String.Empty, base.RecuperarEnderecoPortal());
                            return false;
                        }
                    }
                    else
                    {
                        if (SessaoAtual != null)
                        {
                            numeroPV = SessaoAtual.CodigoEntidade;
                            idUsuario = SessaoAtual.CodigoIdUsuario;
                        }
                        else
                        {
                            // Não é possível a abertura da tela de confirmação positiva manualmente, tente efetuar login novamente.
                            this.ExibirErro("SharePoint.ConfirmacaoPositiva", 1051, "Atenção", String.Empty, base.RecuperarEnderecoPortal());
                            return false;
                        }
                    }

                    List<UsuarioServico.Pergunta> perguntas = new List<UsuarioServico.Pergunta>();
                    var perguntasIncorretas = default(UsuarioServico.Pergunta[]);

                    perguntas.AddRange(this.RecuperarRespostasBasicas());

                    if (this.ConfirmacaoCompleta)
                    {
                        perguntas.AddRange(this.RecuperarRespotasAleatorias());
                    }

                    Int32 codigoRetorno = 0;
                    Boolean terminalValido = false;
                    using (var clientUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    {
                        using (var clientEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        {
                            codigoRetorno = clientUsuario.Cliente.ValidarConfirmacaoPositivaVariavel(out perguntasIncorretas, numeroPV, perguntas.ToArray());
                            terminalValido = this.ValidarRespostaTerminal(numeroPV);

                            if (codigoRetorno == 0 && terminalValido)
                            {
                                clientUsuario.Cliente.ReiniciarQuantidadeConfirmacaoPositiva(idUsuario);
                                return true;
                            }
                            else
                            {
                                clientUsuario.Cliente.IncrementarQuantidadeConfirmacaoPositiva(idUsuario);

                                //Armazena no histórico/log de atividades
                                RegistrarHistorico(perguntasIncorretas, terminalValido);

                                return false;
                            }
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, String.Empty);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, String.Empty);
                }
                catch (FaultException<MaximoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, String.Empty);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, String.Empty);
                }
            }

            return false;
        }

        /// <summary>
        /// Recupera as resposta da Confirmação Positiva quando no Modo Básico
        /// </summary>
        /// <returns>List of UsuarioServico.Pergunta - Respostas da Confirmação Positiva</returns>
        private List<UsuarioServico.Pergunta> RecuperarRespostasBasicas()
        {
            List<UsuarioServico.Pergunta> perguntas = new List<UsuarioServico.Pergunta>();

            Decimal cpf = NormalizarString(txtCPFSocio.Text).ToDecimalNull(0).Value;
            Int32 bancoCredito = 0;
            String agenciaCredito = String.Empty;
            String contaCredito = String.Empty;
            String domicilioBancarioCredito = String.Empty;

            #region [IDs para validações básicas na procedure]
            /* 
                    Validar Propriedades Básicas
                    --------------------------------------------------
                    ID  | Nome
                    --------------------------------------------------
                    1   | CNPJ ou CPF do Estabelecimento [Agnaldo Costa - Verificação desativada no Novo Acesso ]
                    2   | Domicilio Bancário de Crédito
                    3   | CPF/CNPJ de um dos Proprietários
			        20  | Domicilio Bancário de Crédito/Débito
                    --------------------------------------------------
            */
            #endregion

            //UsuarioServico.Pergunta p1 = new UsuarioServico.Pergunta()
            //{
            //    CodigoPergunta = 3, //Código CPF CNPJ do Proprietário
            //    PerguntaVariavel = false,
            //    Resposta = cpf.ToString()
            //};

            //perguntas.Add(p1);

            if (!this.ConfirmacaoCompleta)
            {
                if (pnlDomicilioCredito.Visible)
                {
                    bancoCredito = Int32.Parse(NormalizarString(ddlBanco.SelectedValue));
                    agenciaCredito = NormalizarString(txtAgencia.Text);
                    contaCredito = NormalizarString(txtContaCorrente.Text);
					
					if (bancoCredito == 104)
					{
						//Garante que terá 10 caracteres
						if (contaCredito.Length < 10)
							contaCredito = AtribuirZerosEsquerda(contaCredito, 10);

						contaCredito = ddlOperacao.SelectedValue + contaCredito.Substring(1);
					}

                    domicilioBancarioCredito = this.FormatarDomicilioBancario(bancoCredito.ToString(), agenciaCredito, contaCredito, false);

                    UsuarioServico.Pergunta p = new UsuarioServico.Pergunta()
                    {
                        CodigoPergunta = 20, //Código Pergunta Domicílio Crédito e Débito
                        PerguntaVariavel = false,
                        Resposta = domicilioBancarioCredito
                    };

                    perguntas.Add(p);
                }
            }

            return perguntas;
        }

        /// <summary>
        /// Recupera as resposta da Confirmação Positiva quando no Modo Completo
        /// </summary>
        /// <returns>List of UsuarioServico.Pergunta - Respostas da Confirmação Positiva</returns>
        private List<UsuarioServico.Pergunta> RecuperarRespotasAleatorias()
        {
            List<UsuarioServico.Pergunta> perguntas = new List<UsuarioServico.Pergunta>();

            Int32 bancoCreditoDebito = 0;
            String agenciaCreditoDebito = String.Empty;
            String contaCreditoDebito = String.Empty;
            String domicilioBancarioCredito = String.Empty;
            String domicilioBancarioDebito = String.Empty;

            #region [IDs para validações aleatórias na procedure]
            /* 
                        Validar Propriedades Aleatórias
                        --------------------------------------------------
                        ID  | Nome
                        --------------------------------------------------
                        1   | CEP do Estabelecimento
                        2   | DDD + Telefone do Estabelecimento
                        3   | Limite de Parcelas sem Juros
                        4   | Domicilio Bancário de Débito
                        5   | Estado e Cidade do Estabelecimento
                        6   | Tipo do Ponto de Venda
                        7   | Nr do Endereço
                        8   | Alteração de domicílio
                        9   | Domínio de E-mail    
                        10  | Nr Terminal (Validar pelo Serviço Máximo)
                        --------------------------------------------------
                        */
            #endregion

            if (pnlDomicilioCredito.Visible)
            {
                bancoCreditoDebito = Int32.Parse(NormalizarString(ddlBanco.SelectedValue));
                agenciaCreditoDebito = NormalizarString(txtAgencia.Text);
                contaCreditoDebito = NormalizarString(txtContaCorrente.Text);
				
				if (bancoCreditoDebito == 104)
                {
                    //Garante que terá 10 caracteres
                    if (contaCreditoDebito.Length < 10)
                        contaCreditoDebito = AtribuirZerosEsquerda(contaCreditoDebito, 10);

                    contaCreditoDebito = ddlOperacao.SelectedValue + contaCreditoDebito.Substring(1);
                }

                domicilioBancarioCredito = this.FormatarDomicilioBancario(bancoCreditoDebito.ToString(), agenciaCreditoDebito, contaCreditoDebito, false);

                UsuarioServico.Pergunta p = new UsuarioServico.Pergunta()
                {
                    CodigoPergunta = 20, //Código Pergunta Domicílio Crédito
                    PerguntaVariavel = false,
                    Resposta = domicilioBancarioCredito
                };

                perguntas.Add(p);
            }

            if (pnlDomicilioDebito.Visible)
            {
                bancoCreditoDebito = Int32.Parse(NormalizarString(ddlBancoDebito.SelectedValue));
                agenciaCreditoDebito = NormalizarString(txtAgenciaDebito.Text);
                contaCreditoDebito = NormalizarString(txtContaCorrenteDebito.Text);

				if (bancoCreditoDebito == 104)
                {
                    //Garante que terá 10 caracteres
                    if (contaCreditoDebito.Length < 10)
                        contaCreditoDebito = AtribuirZerosEsquerda(contaCreditoDebito, 10);

                    contaCreditoDebito = ddlOperacaoDebito.SelectedValue + contaCreditoDebito.Substring(1);
                }
				
                domicilioBancarioDebito = this.FormatarDomicilioBancario(bancoCreditoDebito.ToString(), agenciaCreditoDebito, contaCreditoDebito, true);

                UsuarioServico.Pergunta p = new UsuarioServico.Pergunta()
                {
                    CodigoPergunta = 20, //Código Pergunta Domicílio Débito
                    PerguntaVariavel = true,
                    Resposta = domicilioBancarioDebito
                };

                perguntas.Add(p);
            }

            if (pnlCEP.Visible)
            {
                String resposta = String.Empty;
                resposta = NormalizarString(txtCep.Text);

                UsuarioServico.Pergunta p = new UsuarioServico.Pergunta()
                {
                    CodigoPergunta = 1, //CEP
                    PerguntaVariavel = true,
                    Resposta = resposta
                };

                perguntas.Add(p);
            }

            if (pnlTelefone.Visible)
            {
                String resposta = String.Empty;
                resposta = NormalizarString(txtTelefone.Text);

                UsuarioServico.Pergunta p = new UsuarioServico.Pergunta()
                {
                    CodigoPergunta = 2, //Telefone
                    PerguntaVariavel = true,
                    Resposta = resposta
                };

                perguntas.Add(p);
            }

            if (pnlNumeroResidencia.Visible)
            {
                String resposta = String.Empty;
                resposta = NormalizarString(chkSemNumero.Checked ? "SN" : txtNumeroResidencia.Text);

                UsuarioServico.Pergunta p = new UsuarioServico.Pergunta()
                {
                    CodigoPergunta = 7, //Número da residência
                    PerguntaVariavel = true,
                    Resposta = resposta
                };

                perguntas.Add(p);
            }

            if (pnlAlteracaoDomicilio.Visible)
            {
                String resposta = String.Empty;
                resposta = rblAlteracaoDomicilio.SelectedValue;

                UsuarioServico.Pergunta p = new UsuarioServico.Pergunta()
                {
                    CodigoPergunta = 8, //Alterou um domicílio
                    PerguntaVariavel = true,
                    Resposta = resposta
                };

                perguntas.Add(p);
            }

            if (pnlDominioEmail.Visible)
            {
                String resposta = String.Empty;
                resposta = txtDominioEmail.Text;

                UsuarioServico.Pergunta p = new UsuarioServico.Pergunta()
                {
                    CodigoPergunta = 9, //Domínio e-mail
                    PerguntaVariavel = true,
                    Resposta = resposta
                };

                perguntas.Add(p);
            }

            return perguntas;
        }

        /// <summary>
        /// Recupera a quantidade de tentativas da Entidade para realizar a confirmação Positiva
        /// </summary>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        private Int32 RecuperarQuantidadeTentivas(out String mensagem)
        {
            Int32 qtdTentativas = 0;
            Int32 codigoRetornoIS = 0;
            Int32 codigoRetornoGE = 0;
            mensagem = String.Empty;
            using (Logger log = Logger.IniciarLog("RecuperarQuantidadeTentivas"))
            {
                using (var client = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                {
                    Int32 codigoIdUsuario = default(Int32);

                    // Liberação de acesso completo
                    log.GravarMensagem("Verificando a sessão do usuário", SessaoAtual);
                    if (SessaoAtual != null)
                        codigoIdUsuario = SessaoAtual.CodigoIdUsuario;

                    // Se o código está zerado, verifica se veio da área aberta
                    if (codigoIdUsuario == 0 && InformacaoUsuario.Existe())
                        codigoIdUsuario = InformacaoUsuario.Recuperar().IdUsuario;

                    // impede prosseguir se o ID do usuário ainda estiver zerado
                    if (codigoIdUsuario == 0)
                        throw new PortalRedecardException(500, MethodBase.GetCurrentMethod().Name);

                    var usuario = client.Cliente.ConsultarPorID(out qtdTentativas, codigoIdUsuario);
                    if (qtdTentativas == 0 && usuario != null)
                    {
                        if (!usuario.BloqueadoConfirmacaoPositiva)
                        {
                            mensagem = String.Format(@"Você ainda tem <b>{0}</b> tentativas.",
                                                    this.QtdTentativas - usuario.QuantidadeTentativaConfirmacaoPositiva);
                        }
                        qtdTentativas = usuario.QuantidadeTentativaConfirmacaoPositiva;
                    }
                }
            }
            return qtdTentativas;
        }

        /// <summary>
        /// Valida se o número do terminal informado está correto caso a pergunta esteja habilitada
        /// </summary>
        /// <param name="numeroPV">Número do PV</param>
        /// <returns>
        /// <para>True - Válido</para>
        /// <para>False - Inválido</para>
        /// </returns>
        private Boolean ValidarRespostaTerminal(Int32 numeroPV)
        {
            Boolean terminalValido = true;

            using (Logger log = Logger.IniciarLog("Valida se o número do terminal informado está correto caso a pergunta esteja habilitada"))
            {
                if (pnlTerminal.Visible)
                {
                    try
                    {
                        String numeroTerminal = txtNumeroTerminal.Text;

                        using (var contexto = new ContextoWCF<MaximoServico.MaximoServicoClient>())
                        {
                            MaximoServico.FiltroTerminal filtro = new MaximoServico.FiltroTerminal();
                            filtro.NumeroLogico = numeroTerminal;
                            filtro.PontoVenda = numeroPV.ToString();

                            List<MaximoServico.TerminalConsulta> terminais = contexto.Cliente.ConsultarTerminal(filtro);
                            terminalValido = terminais.Count > 0;
                        }
                    }
                    catch (FaultException<MaximoServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        terminalValido = false;
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        terminalValido = false;
                    }
                }
            }

            return terminalValido;
        }

        /// <summary>
        /// Indicador se a Confirmação Positiva é para Recuperação de Senha
        /// </summary>
        /// <returns></returns>
        public Boolean ModoRecuperacaoUsuarioSenha()
        {
            return !this.ObterSessaoAberto && (this.SessaoAtual != null && (this.SessaoAtual.CodigoStatus == Comum.Enumerador.Status.RespostaIdPosPendente
                && (this.SessaoAtual.TipoUsuario.Equals("M")
                    || this.SessaoAtual.TipoUsuario.Equals("P")
                    && this.ValidarPagina())));
        }

        /// <summary>
        /// Indicador se a Confirmação Positiva é para Recuperação de Senha
        /// </summary>
        /// <returns></returns>
        public Boolean ModoLiberacaoAcesso()
        {
            return !this.ObterSessaoAberto && (this.SessaoAtual != null
                        && (this.SessaoAtual.TipoUsuario.Equals("B")
                        && !PossuiMaster())
                        || (!this.ObterSessaoAberto
                        && this.BloquearUsuario));
        }

        /// <summary>
        ///Verifica se a entidade possui master.
        /// </summary>
        /// <returns></returns>
        public bool PossuiMaster()
        {
            return base.EntidadePossuiMaster();
        }

        /// <summary>
        /// Recuperar String com o Tipo de Confirmação Positiva para o Histórico de Log
        /// </summary>
        /// <returns>Tipo de Confirmação Positiva</returns>
        private String RecuperarTipoRecuperacao()
        {
            String tipo = String.Empty;

            if (this.ModoLiberacaoAcesso())
                tipo = "Acesso Completo";
            else
                tipo = "Recuperação de Usuário/Senha";

            return tipo;
        }

        /// <summary>
        /// Registra no Histórico o erro na confirmação positiva
        /// </summary>
        /// <param name="perguntasIncorretas">Lista contendo as perguntas respondidas incorretamente</param>
        private void RegistrarHistorico(UsuarioServico.Pergunta[] perguntasIncorretas, Boolean terminalValido)
        {
            if (perguntasIncorretas.Length > 0)
            {
                //Tipo de confirmação positiva envolvida
                String tipoConfirmacaoPositiva = RecuperarTipoRecuperacao();

                //Obtém a descrição das perguntas aleatórias que foram respondidas incorretamentes
                var aleatoriasIncorretas = perguntasIncorretas
                    .Where(p => p.PerguntaVariavel && descricaoPerguntasAleatorias.ContainsKey(p.CodigoPergunta))
                    .Select(p => descricaoPerguntasAleatorias[p.CodigoPergunta] + " (" + p.Resposta + ")").ToList();

                //Obtém a descrição das perguntas básicas que forma respondidas incorretamente
                var basicasIncorretas = perguntasIncorretas
                    .Where(p => !p.PerguntaVariavel && descricaoPerguntasBasicas.ContainsKey(p.CodigoPergunta))
                    .Select(p => descricaoPerguntasBasicas[p.CodigoPergunta] + " (" + p.Resposta + ")").ToList();

                //Cria coleção única com todas as descrições das perguntas respondidas incorretamente
                var dadosIncorretos = new List<String>();
                if (aleatoriasIncorretas.Count > 0)
                    dadosIncorretos.AddRange(aleatoriasIncorretas);
                if (basicasIncorretas.Count > 0)
                    dadosIncorretos.AddRange(basicasIncorretas);
                if (!terminalValido && descricaoPerguntasAleatorias.ContainsKey(10)) //10: código da resposta do terminal
                    dadosIncorretos.Add(descricaoPerguntasAleatorias[10]);

                //Armazena no histórico
                if (this.ObterSessaoAberto && InformacaoUsuario.Existe())
                {
                    InformacaoUsuario usuario = InformacaoUsuario.Recuperar();
                    Historico.ErroConfirmacaoPositivaUsuarioMaster(
                        usuario.IdUsuario, usuario.NomeCompleto, usuario.EmailUsuario,
                        usuario.TipoUsuario, usuario.NumeroPV, tipoConfirmacaoPositiva,
                        dadosIncorretos.Distinct().ToArray());
                }
                else if (SessaoAtual != null)
                {
                    Historico.ErroConfirmacaoPositivaUsuarioMaster(
                        SessaoAtual.CodigoIdUsuario, SessaoAtual.NomeUsuario, SessaoAtual.Email,
                        SessaoAtual.TipoUsuario, SessaoAtual.CodigoEntidade, tipoConfirmacaoPositiva,
                        dadosIncorretos.Distinct().ToArray());
                }
            }
        }

        /// <summary>
        /// Verificar se alguns dados da Entidade existem na sessão
        /// </summary>
        private void VerificarDadosSessao()
        {
            using (var client = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
            {
                Int32 codigoRetorno = 0;
                EntidadeServico.Entidade entidade = null;
                if (InformacaoUsuario.Existe())
                {
                    entidade = client.Cliente.ConsultarDadosPV(out codigoRetorno, InformacaoUsuario.Recuperar().NumeroPV);

                    if (codigoRetorno == 0)
                    {
                        InformacaoUsuario.Recuperar().Empresa = entidade.CNPJEntidade.Length > 11;
                        InformacaoUsuario.Recuperar().EstabelecimentoEmpIbba = !entidade.CodigoSegmento.ToString().ToLower().Equals("v");
                    }
                }
            }
        }

        /// <summary>
        /// Validar se a Entidade já está com a Confirmação Positiva Bloqueada
        /// </summary>
        /// <returns></returns>
        private Boolean ConfirmacaoPositivaBloqueada()
        {
            Boolean bloqueada = false;

            using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
            {
                using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    Int32 codigoRetornoBloqueio = 0;
                    Int32 codigoRetornoIs = 0;
                    Int32 codigoRetornoGe = 0;
                    Int32 idUsuario = 0;
                    String emailUsuario = String.Empty;
                    UsuarioServico.Entidade entidade;

                    #region [Dados Entidade/Usuário]
                    if (this.ObterSessaoAberto)
                    {
                        InformacaoUsuario info = InformacaoUsuario.Recuperar();

                        entidade = new UsuarioServico.Entidade()
                        {
                            GrupoEntidade = new UsuarioServico.GrupoEntidade()
                            {
                                Codigo = info.GrupoEntidade
                            },
                            Codigo = info.NumeroPV
                        };

                        idUsuario = info.IdUsuario;
                        emailUsuario = info.EmailUsuario;
                    }
                    else
                    {
                        entidade = new UsuarioServico.Entidade()
                        {
                            GrupoEntidade = new UsuarioServico.GrupoEntidade()
                            {
                                Codigo = SessaoAtual.GrupoEntidade
                            },
                            Codigo = SessaoAtual.CodigoEntidade
                        };

                        idUsuario = SessaoAtual.CodigoIdUsuario;
                        emailUsuario = SessaoAtual.Email;
                    }
                    #endregion

                    if (this.ModoLiberacaoAcesso()
                        || this.ModoRecuperacaoUsuarioSenha())
                    {
                        var usuario = contextoUsuario.Cliente.ConsultarPorID(out codigoRetornoBloqueio, idUsuario);

                        if (usuario != null && usuario.QuantidadeTentativaConfirmacaoPositiva >= this.QtdTentativas)
                        {
                            bloqueada = true;

                            this.ExibirAvisoBloqueioSenhaUsuario("Acesso completo bloqueado", PainelMensagemIcon.Erro);

                            //SessaoAtual.CodigoStatus = Status.EntidadeBloqueadaConfirmacaoPositiva;
                            InformacaoUsuario.Limpar();
                        }
                    }
                    else //Bloqueio por Entidade
                    {
                        var entidades = contextoEntidade.Cliente.Consultar(out codigoRetornoIs, out codigoRetornoGe, entidade.Codigo, entidade.GrupoEntidade.Codigo);

                        if (entidades.Length > 0)
                        {
                            var entidadeConfirmacao = entidades[0];

                            if (entidadeConfirmacao.QuantidadeConfirmacaoPositiva >= this.QtdTentativas) //Bloquear o formulário de criação de usuário
                            {
                                bloqueada = true;

                                this.ExibirAvisoBloqueioSenhaUsuario("Acesso completo bloqueado", PainelMensagemIcon.Erro);

                                InformacaoUsuario.Limpar();
                            }
                        }
                    }
                }
            }

            return bloqueada;
        }

        public bool ConfirmarConfirmacaoPositiva()
        {
            using (Logger log = Logger.IniciarLog("Continuando Processo de liberação de Alteração de Domicílio bancário após recuperação de usuário e senha"))
            {
                try
                {
                    if (SessaoAtual != null)
                    {
                        Guid hashEmail = Guid.Empty;
                        Int32 codigoRetorno = 0;
                        EntidadeServico.Entidade1[] entidades = null;
                        Boolean possuiKomerci = false;

                        //Recupera os PVs associados ao usuário (para verificar se alguma possui Komerci)
                        using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                            entidades = ctx.Cliente.ConsultarPorUsuario(out codigoRetorno, SessaoAtual.CodigoIdUsuario);
                        if (codigoRetorno > 0)
                        {
                            base.ExibirPainelExcecao("EntidadeServico.ConsultarPorUsuario", codigoRetorno);
                            return false;
                        }

                        //Verifica se algum PV associado ao usuário possui Komerci
                        using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                            possuiKomerci = ctx.Cliente.PossuiKomerci(entidades.Select(entidade => entidade.Codigo).ToArray());

                        if (this.ModoRecuperacaoUsuarioSenha())
                        {
                            //Atualiza os dados do usuário para liberação de acesso completo
                            using (var contexto = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                                codigoRetorno = contexto.Cliente.Atualizar2(out hashEmail,
                                                                            possuiKomerci,
                                                                            SessaoAtual.GrupoEntidade,
                                                                            SessaoAtual.CodigoEntidade.ToString(),
                                                                            SessaoAtual.LoginUsuario,
                                                                            SessaoAtual.NomeUsuario,
                                                                            SessaoAtual.TipoUsuario,
                                                                            String.Empty,
                                                                            String.Empty,
                                                                            SessaoAtual.CodigoIdUsuario,
                                                                            SessaoAtual.Email,
                                                                            SessaoAtual.EmailSecundario,
                                                                            SessaoAtual.CPF,
                                                                            SessaoAtual.DDDCelular,
                                                                            SessaoAtual.Celular,
                                                                            UsuarioServico.Status1.UsuarioAtivo,  // Altera o status de 11 para 1.
                                                                            2,

                                                                            DateTime.Now);
                        }
                        else if (this.ModoLiberacaoAcesso())
                        {
                            //Atualiza os dados do usuário para liberação de acesso completo
                            using (var contexto = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                            {

                                // Implementacao de validao de perfil de usuario, para evitar que problemas
                                // como realizar a confirmacao de usuario personalizado 2x modifiquem
                                // o usuario para master e ativo.
                                // Alteracao originada das ocorrencias O501811 e O516110.
                                if (SessaoAtual.TipoUsuario != "P")
                                {
                                    SessaoAtual.TipoUsuario = "M";
                                }

                                codigoRetorno = contexto.Cliente.Atualizar2(out hashEmail,
                                                                            possuiKomerci,
                                                                            SessaoAtual.GrupoEntidade,
                                                                            SessaoAtual.CodigoEntidade.ToString(),
                                                                            SessaoAtual.LoginUsuario,
                                                                            SessaoAtual.NomeUsuario,
                                                                            SessaoAtual.TipoUsuario, //Liberação de Acesso completo = Usuário Master
                                                                            String.Empty,
                                                                            String.Empty,
                                                                            SessaoAtual.CodigoIdUsuario,
                                                                            SessaoAtual.Email,
                                                                            SessaoAtual.EmailSecundario,
                                                                            SessaoAtual.CPF,
                                                                            SessaoAtual.DDDCelular,
                                                                            SessaoAtual.Celular,
                                                                            UsuarioServico.Status1.UsuarioAtivo,
                                                                            2,
                                                                            DateTime.Now);
                            }
                        }
                        SessaoAtual.CodigoStatus = Comum.Enumerador.Status.UsuarioAtivo;
                        if (codigoRetorno > 0)
                        {
                            base.ExibirPainelExcecao("UsuarioServico.Atualizar", codigoRetorno);
                            return false;
                        }

                        HttpContext.Current.Session.Timeout = 15;
                        HttpContext.Current.Session.Add(Sessao.ChaveSessao, SessaoAtual);

                        UsuarioServico.Usuario usuario = null;
                        using (var contexto = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                            usuario = contexto.Cliente.ConsultarPorID(out codigoRetorno, SessaoAtual.CodigoIdUsuario);

                        if (this.ModoLiberacaoAcesso())
                        {
                            //Registra no histórico/log de atividades
                            Historico.LiberacaoAcessoCompleto(SessaoAtual);

                            //Persiste a informação de que o usuário já clicou no botão "Liberar Acesso Completo"
                            //e concluiu a confirmação positiva
                            this.AtualizarFlagExibicaoMensagemAcessoCompleto();
                        }

                    }
                    else
                    {
                        base.ExibirPainelExcecao("SharePoint.ConfirmacaoPositiva", 1051);
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    return false;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }

            }
            return true;
        }

        #endregion

        #region [Métodos auxiliares]

        /// <summary>
        /// Valida se os campos informados na tela estão válidos
        /// </summary>
        /// <returns></returns>
        private Boolean ValidateFields()
        {
            Boolean revalidate = false;

            #region habilita/desabilita validators em função dos dados informados

            // se a chkSemNumero estiver habilitada, refaz a validação do campo "número de endereço"
            if (chkSemNumero.Checked)
            {
                crvNumeroResidencia.Enabled = false;
                revalidate = true;
            }

            // ativa o validator para caixa ecnomômica (crédito)
            if (ddlBanco.SelectedValue == "104")
            {
                crvOperacao.Enabled = true;
                revalidate = true;
            }

            // ativa o validator para caixa ecnomômica (débito)
            if (ddlBancoDebito.SelectedValue == "104")
            {
                crvOperacaoDebito.Enabled = true;
                revalidate = true;
            }

            #endregion

            // revalida os campos na página
            if (revalidate)
                Page.Validate();

            Boolean isValid = Page.IsValid;

            // volta os validator às configurações padrão
            crvNumeroResidencia.Enabled = true;
            crvOperacao.Enabled = !isValid && ddlBanco.SelectedValue == "104";
            crvOperacaoDebito.Enabled = !isValid && ddlBancoDebito.SelectedValue == "104";

            return isValid;
        }

        /// <summary>
        /// Normaliza uma string, removendo os caracteres especiais dela
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        protected String NormalizarString(String original)
        {
            return Regex.Replace(original, @"[^\w]", "");
        }

        /// <summary>
        /// Método auxiliar para exibir o conteúdo do quadro de aviso
        /// </summary>
        /// <param name="mensagem">Mensagem para exibição</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="tipoAviso">Tipo do quadro de aviso</param>
        private void ConfigurarAviso(String mensagem, String titulo, PainelMensagemIcon tipoAviso)
        {
            this.ConfigurarAviso(mensagem, titulo, tipoAviso, false, String.Empty);
        }

        /// <summary>
        /// Método auxiliar para exibir o conteúdo do quadro de aviso
        /// </summary>
        /// <param name="mensagem">Mensagem para exibição</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="tipoAviso">Tipo do quadro de aviso</param>
        /// <param name="urlVoltar">(Opcional) URL para redirecionamento a partir do botão de voltar</param>
        private void ConfigurarAviso(String mensagem, String titulo, PainelMensagemIcon tipoAviso, String urlVoltar)
        {
            this.ConfigurarAviso(mensagem, titulo, tipoAviso, true, urlVoltar);
        }

        /// <summary>
        /// Método auxiliar para exibir o conteúdo do quadro de aviso
        /// </summary>
        /// <param name="mensagem">Mensagem para exibição</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="tipoAviso">Tipo do quadro de aviso</param>
        /// <param name="enableVoltar">Identifica se deve habilitar o botão de voltar</param>
        /// <param name="urlVoltar">(Opcional) URL para redirecionamento a partir do botão de voltar</param>
        private void ConfigurarAviso(String mensagem, String titulo, PainelMensagemIcon tipoAviso, Boolean enableVoltar, String urlVoltar = "")
        {
            pnlConfirmacaoPositiva.Visible = false;
            phSubtitle.Visible = false;

            pnlMensagem.Visible = true;
            PainelMensagem.Mensagem = mensagem;
            PainelMensagem.Titulo = titulo;
            PainelMensagem.TipoMensagem = tipoAviso;

            // habilita o botão conforme os parâmetros informados
            phVoltar.Visible = enableVoltar;
            if (!String.IsNullOrWhiteSpace(urlVoltar))
            {
                btnVoltar.OnClientClick = String.Format("window.location.href='{0}'; return false;", urlVoltar);
            }
        }

        /// <summary>
        /// Exibe o quadro de aviso com a mensagem de erro
        /// </summary>
        /// <param name="fonte">Base fonte da mensagem de erro</param>
        /// <param name="codigo">Código da Mensagem</param>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="mensagemAdicional">Mensagem adicional para ser concatenada a mensagem do banco</param>
        /// <param name="urlVoltar">Url que o botão Voltar do Aviso deve redirecionar</param>
        private void ExibirErro(String fonte, Int32 codigo, String titulo, String mensagemAdicional, String urlVoltar)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);

            if (!String.IsNullOrEmpty(mensagemAdicional))
            {
                mensagem += "<br /><br />" + mensagemAdicional;
            }

            if (!String.IsNullOrEmpty(urlVoltar))
            {
                //Redirecionar na home fechada
                if (!this.ObterSessaoAberto)
                    urlVoltar = base.web.ServerRelativeUrl;

                this.ConfigurarAviso(mensagem, titulo, PainelMensagemIcon.Erro, urlVoltar);
            }
            else
            {
                this.ConfigurarAviso(mensagem, titulo, PainelMensagemIcon.Erro, true);
            }

            pnlConfirmacaoPositiva.Visible = false;
            phSubtitle.Visible = false;
            pnlMensagem.Visible = true;
        }

        /// <summary>
        /// Exibe o quadro de aviso com a mensagem de erro
        /// </summary>
        /// <param name="fonte">Base fonte da mensagem de erro</param>
        /// <param name="codigo">Código da Mensagem</param>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="tipoAviso">ícone mensagem</param>
        private void ExibirErro(String fonte, Int32 codigo, String titulo, PainelMensagemIcon tipoAviso)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            this.ConfigurarAviso(mensagem, titulo, tipoAviso);
        }

        /// <summary>
        /// Exibe o quadro de aviso com a mensagem de erro
        /// </summary>
        /// <param name="fonte">Base fonte da mensagem de erro</param>
        /// <param name="codigo">Código da Mensagem</param>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="tipoAviso">ícone mensagem</param>
        private void ExibirErroVoltarHome(String fonte, Int32 codigo, String titulo, PainelMensagemIcon tipoAviso)
        {
            pnlConfirmacaoPositiva.Visible = false;
            phSubtitle.Visible = false;
            pnlMensagem.Visible = true;

            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            this.ConfigurarAviso(mensagem, titulo, tipoAviso, base.RecuperarEnderecoPortal());
        }

        /// <summary>
        /// Exibe mensagem personalizada no controle de aviso
        /// </summary>
        /// <param name="titulo">Título da Mensagem</param>
        /// <param name="mensagem">Mensagem do aviso</param>
        /// <param name="tentarNovamente">Indica se deve exibir os botões de tentar novamente ou o de voltar</param>
        /// <param name="tipoAviso">Ícone da mensagem de aviso</param>
        private void ExibirErro(String titulo, String mensagem, Boolean tentarNovamente, PainelMensagemIcon tipoAviso)
        {
            phTentativas.Visible = tentarNovamente;

            pnlConfirmacaoPositiva.Visible = false;
            phSubtitle.Visible = false;
            pnlMensagem.Visible = true;

            this.ConfigurarAviso(mensagem, titulo, tipoAviso, !tentarNovamente);
        }

        /// <summary>
        /// Exibe mensagem personalizada no controle de aviso
        /// </summary>
        /// <param name="titulo">Título da Mensagem</param>
        /// <param name="fonte">Fonte da mensagem de erro</param>
        /// <param name="codigoErro">Código da mensagem de erro</param>
        /// <param name="tentarNovamente">Indica se deve exibir os botões de tentar novamente ou o de voltar</param>
        /// <param name="tipoAviso">Ícone da mensagem de aviso</param>
        private void ExibirAvisoBloqueioEntidade(String titulo, String fonte, Int32 codigoErro, Boolean tentarNovamente, PainelMensagemIcon tipoAviso)
        {
            phTentativas.Visible = tentarNovamente;

            pnlConfirmacaoPositiva.Visible = false;
            phSubtitle.Visible = false;
            pnlMensagem.Visible = true;

            if (tentarNovamente)
                this.ExibirErro(fonte, codigoErro, titulo, tipoAviso);
            else
                this.ExibirErro(fonte, codigoErro, titulo, String.Empty, base.RecuperarEnderecoPortal());
        }

        /// <summary>
        /// Exibe mensagem personalizada no controle de aviso
        /// </summary>
        /// <param name="titulo">Título da Mensagem</param>
        /// <param name="mensagem">Mensagem</param>
        /// <param name="tipoAviso">Ícone da mensagem de aviso</param>
        private void ExibirAvisoBloqueioSenhaUsuario(String titulo, PainelMensagemIcon tipoAviso)
        {
            String mensagem = @"
Você excedeu o limite de tentativas na Confirmação Positiva 
e só poderá acessar os seguintes serviços:<br /><br />
<ul class=""bullets-list-rede"">
    <li>Extratos</li>
    <li>Recebimento Antecipado de Vendas (RAV)</li>
    <li>Comprovantes de Vendas</li>
    <li>Pedidos de materiais: solicitação de bobina e material de divulgação</li>
    <li>Outros serviços: consultas a taxas, aluguéis e demais</li>
</ul><br /><br />
Para realizar o desbloqueio e liberar o seu acesso completo entre em contato com a nossa
Central de Atendimento e verifique seus dados de cadastro.<br /><br />
<b>4001 4433 (Capital e Regiões Metropolitanas)</b><br />
<b>0800 728 4433 (Demais Regiões)</b>";

            phTentativas.Visible = false;
            this.ConfigurarAviso(mensagem, titulo, tipoAviso, base.RecuperarEnderecoPortalFechado());
        }


        /// <summary>
        /// Valida a senha informada com a senha do usuário logado
        /// </summary>
        protected Boolean VerificarSenhaAtual(String senhaInformadaDescriptografada)
        {
            Usuario usuario = this.ConsultarUsuario(this.SessaoAtual.CodigoIdUsuario);
            if (usuario == null)
                return false;

            String senhaInformada = EncriptadorSHA1.EncryptString(senhaInformadaDescriptografada);
            return String.Compare(senhaInformada, usuario.Senha, false) == 0;
        }

        /// <summary>
        /// Consulta um usuário por ID
        /// </summary>
        /// <param name="codigoIdUsuario">Código ID do usuário</param>
        protected Usuario ConsultarUsuario(Int32 codigoIdUsuario)
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
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                }
            }

            return usuario;
        }

        #endregion
    }
}