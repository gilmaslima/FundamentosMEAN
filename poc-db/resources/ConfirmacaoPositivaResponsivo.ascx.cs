/*
© Copyright 2015 Rede S.A.
Autor : Yuri Lamonica
Empresa : Rede
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Rede.PN.DadosCadastraisMobile.SharePoint.Util;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.CONTROLTEMPLATES.DadosCadastraisMobile
{
    public partial class ConfirmacaoPositivaResponsivo : UserControlBase
    {
        #region [Controles]

        /// <summary>
        /// qdAviso control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected QuadroAvisosResponsivo QdAviso { get { return (QuadroAvisosResponsivo)qdAviso; } }

        #endregion

        #region [Atributos do Controle]
        /// <summary>
        /// EventHandler para ação de Continuar da Confirmação Posivitiva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void ContinuarHandle(object sender, EventArgs args);

        /// <summary>
        /// Envento privado para ação de Continuar da Confirmação Positiva
        /// </summary>
        private event ContinuarHandle continuarPriv;

        /// <summary>
        /// Envento para ação de Continuar da Confirmação Positiva
        /// </summary>
        public event ContinuarHandle continuar
        {
            add { continuarPriv += value; }
            remove { continuarPriv -= value;}
        }        

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

        #endregion

        #region [Eventos da página]

        /// <summary>
        /// Page_Load Inicializando o controle de confirmação positiva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarNovoLog("Page_Load Iniciliazando a o controle de confirmação positiva"))
            {
                try
                {
                    this.ValidarPermissao = false;

                    this.ConfirmacaoPositivaBloqueada();

                    if (!Page.IsPostBack)
                    {
                        if (this.PerguntasParesAlteracaoDomicilio.Count == 0)
                            this.PerguntasParesAlteracaoDomicilio = new List<int>();

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
                    if (this.ValidarConfirmacaoPositiva())
                    {
                        if (continuarPriv != null)
                            continuarPriv(sender, null);
                    }
                    else
                    {
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

                                if (this.ModoLiberacaoAcesso()
                                    || this.ModoRecuperacaoSenha()
                                    || this.ModoRecuperacaoUsuario())
                                {
                                    var usuario = contextoUsuario.Cliente.ConsultarPorID(out codigoRetornoBloqueio, idUsuario);

                                    if (usuario != null)
                                    {
                                        if (usuario.QuantidadeTentativaConfirmacaoPositiva >= this.QtdTentativas)
                                        {
                                            Guid hashEmail = Guid.Empty;
                                            Int32 codigoRetorno = 0;

                                            if (this.ModoRecuperacaoSenha()
                                                || this.ModoRecuperacaoUsuario()) //Recuperação Senha/Usuário
                                            {
                                                codigoRetorno = contextoUsuario.Cliente.AtualizarStatus(
                                                                out hashEmail,
                                                                usuario.CodigoIdUsuario,
                                                                this.RecuperacaoUsuario ? UsuarioServico.Status1.UsuarioBloqueadoRecuperacaoUsuario
                                                                                        : UsuarioServico.Status1.UsuarioBloqueadoRecuperacaoSenha,
                                                                2,
                                                                DateTime.Now);

                                                //                                                String mensagemBloqueio = @"Para desbloquear seu usuário, entre em contato com um usuário Master 
                                                //                                                                ou com a nossa Central de Atendimento através dos telefones:<br />
                                                //                                                                <b>4001 4433 (Capital e Regiões Metropolitanas)</b><br />
                                                //                                                                <b>0800 728 4433 (Demais Regiões)</b>";
                                                this.ExibirAvisoBloqueioSenhaUsuario("Atenção: a quantidade de tentativas foi esgotada",
                                                                                     "Redecard.PN.DadosCadastrais.SharePoint.Login.Logar",
                                                                                     1103,
                                                                                     QuadroAvisosResponsivo.IconeMensagem.Erro);

                                                InformacaoUsuario.Limpar();
                                            }
                                            else if (this.ModoLiberacaoAcesso()) //Liberação de Acesso Completo
                                            {
                                                codigoRetorno = contextoUsuario.Cliente.AtualizarStatus(out hashEmail, usuario.CodigoIdUsuario, UsuarioServico.Status1.UsuarioAtivoLiberacaoAcessoCompletoBloqueada, 2, DateTime.Now);

                                                String mensagemBloqueio = @"Você excedeu o limite de tentativas na Confirmação Positiva 
                                                                            e só poderá acessar os seguintes serviços:<br />
                                                                            <ul style='padding-left:10px'>
                                                                                <li>Extratos</li>
                                                                                <li>Comprovantes de Vendas</li>
                                                                                <li>Pedidos de materiais: solicitação de bobina e material de divulgação</li>
                                                                                <li>Outros serviços: consultas a taxas, aluguéis e demais</li>
                                                                            </ul><br /><br />
                                                                            Para realizar o desbloqueio e liberar o seu acesso completo entre em contato com a nossa
                                                                            Central de Atendimento e verifique seus dados de cadastro.<br />
                                                                            <b>4001 4433 (Capital e Regiões Metropolitanas)</b><br />
                                                                            <b>0800 728 4433 (Demais Regiões)</b>";
                                                this.ExibirAvisoBloqueioSenhaUsuario("Acesso completo bloqueado",
                                                                                     mensagemBloqueio,
                                                                                     QuadroAvisosResponsivo.IconeMensagem.Erro);

                                                InformacaoUsuario.Limpar();
                                            }
                                        }
                                        else //Usuário ainda possui tentativas para confirmação positiva
                                        {
                                            Int32 quantidadeRetorno = 0;
                                            String mensagem = String.Empty;
                                            String titulo = String.Empty;
                                            quantidadeRetorno = RecuperarQuantidadeTentivas(out mensagem, out titulo);
                                            if (quantidadeRetorno > 0)
                                            {
                                                this.ExibirErro(titulo, mensagem, true, QuadroAvisosResponsivo.IconeMensagem.Aviso);
                                            }
                                        }
                                    }
                                }
                                else //Bloqueio por Entidade
                                {
                                    EntidadeServico.Entidade[] entidades = contextoEntidade.Cliente.Consultar(out codigoRetornoIs, out codigoRetornoGe, entidade.Codigo, entidade.GrupoEntidade.Codigo);

                                    if (entidades.Length > 0)
                                    {
                                        var entidadeConfirmacao = entidades[0];

                                        if (entidadeConfirmacao.QuantidadeConfirmacaoPositiva >= this.QtdTentativas) //Bloquear o formulário de criação de usuário
                                        {

                                            //Atualiza o Staus da Entidade para Bloqueada
                                            entidadeConfirmacao.StatusPN = new EntidadeServico.Status
                                            {
                                                Codigo = (Int32)Redecard.PN.Comum.Enumerador.Status.EntidadeBloqueadaConfirmacaoPositiva
                                            };

                                            codigoRetornoBloqueio = contextoEntidade.Cliente
                                                .AtualizarStatusAcesso(
                                                    entidadeConfirmacao.GrupoEntidade.Codigo,
                                                    entidadeConfirmacao.Codigo,
                                                    EntidadeServico.Status1.EntidadeBloqueadaConfirmacaoPositiva,
                                                    emailUsuario);
                                            //codigoRetornoBloqueio = contextoEntidade.Cliente.AlterarEntidade(entidadeConfirmacao);

                                            //Registra no log/histórico de atividades
                                            Historico.BloqueioFormularioSolicitacaoAcesso(
                                                idUsuario, null, emailUsuario, null, entidadeConfirmacao.Codigo);

                                            //                                            String mensagemBloqueio = @"Para desbloquear seu usuário, entre em contato com um usuário Master 
                                            //                                                                ou com a nossa Central de Atendimento através dos telefones:<br />
                                            //                                                                <b>4001 4433 (Capital e Regiões Metropolitanas)</b><br />
                                            //                                                                <b>0800 728 4433 (Demais Regiões)</b>";

                                            this.ExibirAvisoBloqueioEntidade("Atenção: seu usuário foi bloqueado",
                                                                             "ConfirmacaoPositiva.BloquearCriacaoUsuario",
                                                                             1106,
                                                                             false,
                                                                             QuadroAvisosResponsivo.IconeMensagem.Erro);

                                            Boolean possuiMaster = false;
                                            Boolean possuiUsuario = false;
                                            Boolean possuiSenhaTemporaria = false;

                                            contextoEntidade.Cliente.PossuiUsuario(out possuiUsuario, out possuiMaster, out possuiSenhaTemporaria, entidadeConfirmacao.Codigo, entidadeConfirmacao.GrupoEntidade.Codigo);

                                            if (possuiMaster)
                                            {
                                                String emailsMaster = String.Empty;
                                                var usuariosMaster = default(EntidadeServico.Usuario[]);

                                                usuariosMaster = contextoEntidade.Cliente.ConsultarUsuariosPorPerfil(out codigoRetornoIs, entidadeConfirmacao.Codigo, entidadeConfirmacao.GrupoEntidade.Codigo, 'M');

                                                if (usuariosMaster != null && usuariosMaster.Length > 0)
                                                {
                                                    emailsMaster = String.Join(",",
                                                        usuariosMaster.Select(usuario => usuario.Email)
                                                        .Where(email => !String.IsNullOrEmpty(email)).ToArray());

                                                    log.GravarMensagem("Enviar Email Solicitações Acesso Bloqueada", new { emailsMaster });

                                                    if (!String.IsNullOrEmpty(emailsMaster))
                                                        EmailNovoAcesso.EnviarEmailSolicitacoesAcessoBloqueada(
                                                            emailsMaster, emailUsuario, entidadeConfirmacao.Codigo);
                                                }
                                            }
                                            else if (!String.IsNullOrEmpty(entidade.Email))
                                            {
                                                EmailNovoAcesso.EnviarEmailSolicitacoesAcessoBloqueada(
                                                    entidade.Email, emailUsuario, entidade.Codigo);
                                            }//Caso não possua e-mails, somentes exibirá um aviso na parte fechada

                                            InformacaoUsuario.Limpar();
                                        }
                                        else //Entidade ainda possui tentativas de responder
                                        {
                                            Int32 codigoRetornoQuantidade = 0;
                                            String mensagem = String.Empty;
                                            String titulo = String.Empty;
                                            codigoRetornoQuantidade = RecuperarQuantidadeTentivas(out mensagem, out titulo);
                                            if (codigoRetornoQuantidade > 0)

                                                this.ExibirErro(titulo, mensagem, true, QuadroAvisosResponsivo.IconeMensagem.Aviso);
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
                    pnlQuadroAviso.Visible = false;
                    pnlConfirmacaoPositiva.Visible = true;
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
                        pnlQuadroAviso.Visible = false;

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

        #endregion

        #region [Métodos de página]

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
                pnlBotaoVoltar.Visible = false;
                pnlBotaoTentativas.Visible = false;

                if (!this.ObterSessaoAberto)
                    urlVoltar = base.web.ServerRelativeUrl; //Redirecionar na home fechada

                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Erro);
            }
            else
            {
                pnlBotaoVoltar.Visible = true;
                pnlBotaoTentativas.Visible = false;
                QdAviso.CarregarMensagem(titulo, mensagem, false, QuadroAvisosResponsivo.IconeMensagem.Erro);
            }

            pnlConfirmacaoPositiva.Visible = false;
            pnlQuadroAviso.Visible = true;
        }

        /// <summary>
        /// Exibe o quadro de aviso com a mensagem de erro
        /// </summary>
        /// <param name="fonte">Base fonte da mensagem de erro</param>
        /// <param name="codigo">Código da Mensagem</param>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="icone">ícone mensagem</param>
        private void ExibirErro(String fonte, Int32 codigo, String titulo, QuadroAvisosResponsivo.IconeMensagem icone)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            QdAviso.CarregarMensagem(titulo, mensagem, false, icone);
        }

        /// <summary>
        /// Exibe o quadro de aviso com a mensagem de erro
        /// </summary>
        /// <param name="fonte">Base fonte da mensagem de erro</param>
        /// <param name="codigo">Código da Mensagem</param>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="icone">ícone mensagem</param>
        private void ExibirErroVoltarHome(String fonte, Int32 codigo, String titulo, QuadroAvisosResponsivo.IconeMensagem icone)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            QdAviso.CarregarMensagem(titulo, mensagem, base.RecuperarEnderecoPortal(), icone, false);
        }

        /// <summary>
        /// Exibe mensagem personalizada no controle de aviso
        /// </summary>
        /// <param name="titulo">Título da Mensagem</param>
        /// <param name="mensagem">Mensagem do aviso</param>
        /// <param name="tentarNovamente">Indica se deve exibir os botões de tentar novamente ou o de voltar</param>
        /// <param name="icone">Ícone da mensagem de aviso</param>
        private void ExibirErro(String titulo, String mensagem, Boolean tentarNovamente,
            QuadroAvisosResponsivo.IconeMensagem icone)
        {
            pnlBotaoVoltar.Visible = !tentarNovamente;
            pnlBotaoTentativas.Visible = tentarNovamente;

            pnlConfirmacaoPositiva.Visible = false;
            pnlQuadroAviso.Visible = true;

            QdAviso.CarregarMensagem(titulo, mensagem, false, icone);
        }

        /// <summary>
        /// Exibe mensagem personalizada no controle de aviso
        /// </summary>
        /// <param name="titulo">Título da Mensagem</param>
        /// <param name="fonte">Fonte da mensagem de erro</param>
        /// <param name="codigoErro">Código da mensagem de erro</param>
        /// <param name="tentarNovamente">Indica se deve exibir os botões de tentar novamente ou o de voltar</param>
        /// <param name="icone">Ícone da mensagem de aviso</param>
        private void ExibirAvisoBloqueioEntidade(String titulo, String fonte, Int32 codigoErro, Boolean tentarNovamente,
            QuadroAvisosResponsivo.IconeMensagem icone)
        {
            //pnlBotaoVoltar.Visible = !tentarNovamente;
            pnlBotaoTentativas.Visible = tentarNovamente;

            pnlConfirmacaoPositiva.Visible = false;
            pnlQuadroAviso.Visible = true;

            if (tentarNovamente)
                this.ExibirErro(fonte, codigoErro, titulo, icone);
            else
                this.ExibirErro(fonte, codigoErro, titulo, String.Empty, base.RecuperarEnderecoPortal());

            //qdAviso.CarregarMensagem(titulo, mensagem, false, icone);
        }

        /// <summary>
        /// Exibe mensagem personalizada no controle de aviso
        /// </summary>
        /// <param name="titulo">Título da Mensagem</param>
        /// <param name="fonte">Fonte da Mensagem</param>
        /// <param name="codigoErro">Código do Errro</param>
        /// <param name="icone">Ícone da mensagem de aviso</param>
        private void ExibirAvisoBloqueioSenhaUsuario(String titulo, String fonte, Int32 codigoErro, QuadroAvisosResponsivo.IconeMensagem icone)
        {
            pnlBotaoVoltar.Visible = false;
            pnlBotaoTentativas.Visible = false;

            pnlConfirmacaoPositiva.Visible = false;
            pnlQuadroAviso.Visible = true;

            this.ExibirErroVoltarHome(fonte, codigoErro, titulo, icone);
            //qdAviso.CarregarMensagem(titulo, mensagem, base.RecuperarEnderecoPortal(), icone);
        }

        /// <summary>
        /// Exibe mensagem personalizada no controle de aviso
        /// </summary>
        /// <param name="titulo">Título da Mensagem</param>
        /// <param name="mensagem">Mensagem</param>
        /// <param name="icone">Ícone da mensagem de aviso</param>
        private void ExibirAvisoBloqueioSenhaUsuario(String titulo, String mensagem, QuadroAvisosResponsivo.IconeMensagem icone)
        {
            pnlBotaoVoltar.Visible = false;
            pnlBotaoTentativas.Visible = false;

            pnlConfirmacaoPositiva.Visible = false;
            pnlQuadroAviso.Visible = true;

            QdAviso.CarregarMensagem(titulo, mensagem, base.RecuperarEnderecoPortalFechado(), icone);
        }

        /// <summary>
        /// Monta a tela de acordo com o tipo de confirmação positiva
        /// </summary>
        public void MontarConfirmacaoPositiva()
        {
            if (!ConfirmacaoPositivaBloqueada())
            {
                CarregarBancos();
                EsconderTodasPerguntas();
                ExibirValidacoesConfirmacao();
            }
        }

        /// <summary>
        /// Consulta os bancos e preenche combo
        /// </summary>
        private void CarregarBancos()
        {
            using (Logger log = Logger.IniciarLog("Consultando bancos e preenchendo combos"))
            {
                using (var contexto = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    EntidadeServico.Banco[] bancos = contexto.Cliente.ConsultarBancosConfirmacaoPositiva();

                    foreach (EntidadeServico.Banco banco in bancos)
                    {
                        ddlBanco.Items.Add(
                            new ListItem(banco.Codigo.ToString() + " - " + banco.Descricao, banco.Codigo.ToString()));
                        ddlBancoDebito.Items.Add(
                            new ListItem(banco.Codigo.ToString() + " - " + banco.Descricao, banco.Codigo.ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// Formata o domicílio bancário para validação na procedure
        /// </summary>
        /// <param name="banco">Código do Banco</param>
        /// <param name="agencia">Número da Agência</param>
        /// <param name="contacorrente">Número da Conta Corrente</param>
        /// <returns>Domicílio bancário formatado em uma String para validação</returns>
        public string FormatarDomicilioBancario(String banco, string agencia, string contacorrente)
        {
            Boolean estabEmpresa = false;

            if (this.ObterSessaoAberto)
            {
                if (InformacaoUsuario.Existe())
                {
                    InformacaoUsuario info = InformacaoUsuario.Recuperar();

                    estabEmpresa = info.Empresa;
                }
                else
                {
                    this.ExibirErro("SharePoint.ConfirmacaoPositiva", 1051, "Atenção!", String.Empty, base.RecuperarEnderecoPortal());

                    return String.Empty;
                }
            }
            else
            {
                if (base.SessaoAtual != null)
                {
                    estabEmpresa = (SessaoAtual.CNPJEntidade.Length > 11); //Verifica se é um CNPJ
                }
                else
                {
                    this.ExibirErro("SharePoint.ConfirmacaoPositiva", 1051, "Atenção!", String.Empty, base.RecuperarEnderecoPortal());
                    return String.Empty;
                }
            }

            // Regra de Conta Corrente para quando o banco selecionado for
            // caixa economica federal, as c/c das caixa tem sempre 10 posições, começando
            // por 3 (Pessoa Jurídica) e 1 (Pessoa Física)
            if (!String.IsNullOrEmpty(banco) && banco == "104") // CEF
            {
                contacorrente = contacorrente.Trim();
                if (contacorrente.Length < 10)
                {
                    if (estabEmpresa)
                        contacorrente = "3" + AtribuirZerosEsquerda(contacorrente, 9);
                    else
                        contacorrente = "1" + AtribuirZerosEsquerda(contacorrente, 9);
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
                            this.ExibirErro("SharePoint.ConfirmacaoPositiva", 1051, "Atenção!", String.Empty, base.RecuperarEnderecoPortal());
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
                            this.ExibirErro("SharePoint.ConfirmacaoPositiva", 1051, "Atenção!", String.Empty, base.RecuperarEnderecoPortal());
                            return;
                        }
                    }
                    #endregion

                    using (var client = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        pnlQuadroAviso.Visible = false;
                        pnlConfirmacaoPositiva.Visible = true;

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

                            // Exibir Domicilio Bancário de Crédito se Houver
                            EntidadeServico.Pergunta pergunta = perguntasTemp.Find(i => i.CodigoPergunta == 7);
                            if (!Object.ReferenceEquals(pergunta, null))
                            {
                                pnlDomicilioCredito.Visible = true;
                                lblPerguntaDomicilioCredito.Visible = true;
                            }

                            perguntasTemp.Remove(pergunta);

                            pergunta = perguntasTemp.Find(i => i.CodigoPergunta == 4);
                            if (!Object.ReferenceEquals(pergunta, null))
                                pnlDomicilioDebito.Visible = true;

                            perguntasTemp.Remove(pergunta);

                            pnlCPFSocio.Visible = true;
                            lblCPFSocioConfirmacaoBasica.Visible = true;
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
                        #endregion
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

                for (int i = 0; i < 2; i++)
                {
                    Random r = new Random();
                    Int32 maxValue = perguntas.Count; //(from p in _perguntas
                    //                  select p.CodigoPergunta).Max();

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
                                || (idControleAnterior == 10 && this.PossuiParAlteracaoDomicilio(idControle))) //Alteração de Domicílio
                                i--;
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
                        else
                            i--;
                    }
                    catch (PortalRedecardException ex)
                    {
                        log.GravarMensagem("Erro ao exibir a pergunta.", new { panelIndex, nomeControle, idControle });
                        log.GravarErro(ex);
                        i--;
                    }
                    catch (Exception ex)
                    {
                        log.GravarMensagem("Erro ao exibir a pergunta.", new { panelIndex, nomeControle, idControle });
                        log.GravarErro(ex);
                        i--;
                    }
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

                //numeroPergunta++;
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
            ddlAlteracaoDomicilio.SelectedIndex = 0;
            ddlBanco.SelectedIndex = 0;
            ddlBancoDebito.SelectedIndex = 0;
            chkSemNumero.Checked = false;
        }

        /// <summary>
        /// Normaliza uma string, removendo os caracteres especiais dela
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        protected String NormalizarString(String original)
        {
            return Regex.Replace(original, @"[^\w]", String.Empty);
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
                            this.ExibirErro("SharePoint.ConfirmacaoPositiva", 1051, "Atenção!", String.Empty, base.RecuperarEnderecoPortal());
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
                            this.ExibirErro("SharePoint.ConfirmacaoPositiva", 1051, "Atenção!", String.Empty, base.RecuperarEnderecoPortal());
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
                                if (this.ModoCriacaoUsuario()) //Confirmação Positiva para Criação de Usuário
                                    clientEntidade.Cliente.ReiniciarQuantidadeConfirmacaoPositiva(numeroPV, 1);
                                else
                                    clientUsuario.Cliente.ReiniciarQuantidadeConfirmacaoPositiva(idUsuario);

                                return true;
                            }
                            else
                            {
                                if (this.ModoCriacaoUsuario())
                                    clientEntidade.Cliente.IncrementarQuantidadeConfirmacaoPositiva(numeroPV, 1);
                                else
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

            UsuarioServico.Pergunta p1 = new UsuarioServico.Pergunta()
            {
                CodigoPergunta = 3, //Código CPF CNPJ do Proprietário
                PerguntaVariavel = false,
                Resposta = cpf.ToString()
            };

            perguntas.Add(p1);

            if (!this.ConfirmacaoCompleta)
            {
                if (pnlDomicilioCredito.Visible)
                {
                    bancoCredito = Int32.Parse(NormalizarString(ddlBanco.SelectedValue));
                    agenciaCredito = NormalizarString(txtAgencia.Text);
                    contaCredito = NormalizarString(txtContaCorrente.Text);

                    domicilioBancarioCredito = this.FormatarDomicilioBancario(bancoCredito.ToString(), agenciaCredito, contaCredito);

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

                domicilioBancarioCredito = this.FormatarDomicilioBancario(bancoCreditoDebito.ToString(), agenciaCreditoDebito, contaCreditoDebito);

                UsuarioServico.Pergunta p = new UsuarioServico.Pergunta()
                {
                    CodigoPergunta = 2, //Código Pergunta Domicílio Crédito
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

                domicilioBancarioDebito = this.FormatarDomicilioBancario(bancoCreditoDebito.ToString(), agenciaCreditoDebito, contaCreditoDebito);

                UsuarioServico.Pergunta p = new UsuarioServico.Pergunta()
                {
                    CodigoPergunta = 4, //Código Pergunta Domicílio Débito
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
                resposta = NormalizarString(txtNumeroResidencia.Text);

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
                resposta = ddlAlteracaoDomicilio.SelectedValue;

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
        private Int32 RecuperarQuantidadeTentivas(out String mensagem, out String titulo)
        {
            Int32 qtdTentativas = 0;
            Int32 codigoRetornoIS = 0;
            Int32 codigoRetornoGE = 0;
            mensagem = String.Empty;
            titulo = String.Empty;
            Boolean  possuiUsr;
            Boolean possuiUsrMst;
            Boolean possuSenhaTemp;


            if (this.ModoCriacaoUsuario()) //Formulário de criação de usuário
            {
                if (InformacaoUsuario.Existe())
                {
                    InformacaoUsuario info = InformacaoUsuario.Recuperar();
                    using (var client = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        var entidades = client.Cliente.Consultar(out codigoRetornoIS, out codigoRetornoGE, info.NumeroPV, info.GrupoEntidade);
                        if (codigoRetornoGE == 0 && codigoRetornoIS == 0 && entidades.Length > 0)
                        {
                            var entidade = entidades[0];
                            if (!entidade.StatusPN.Codigo.Equals(UsuarioServico.Status1.EntidadeBloqueadaConfirmacaoPositiva))
                            {
                                titulo   = String.Format(@"Atenção: os dados informados estão incorretos. Você ainda tem <b>{0}</b> tentativas. <br />", this.QtdTentativas - entidade.QuantidadeConfirmacaoPositiva);
                                var possuiUsu = client.Cliente.PossuiUsuario(out possuiUsr, out possuiUsrMst, out possuSenhaTemp, entidade.Codigo, entidade.GrupoEntidade.Codigo);

                                if (possuiUsr)
                                {
                                    mensagem = "Caso seu estabelecimento já possua um usuário Master, você pode solicitar que ele faça a criação do seu usuário.";
                                }else{
                                    mensagem = String.Empty;
                                }
//                              mensagem = String.Format(@"Você ainda possui <b>{0}</b> tentativas.<br /><br />
//                                                       Caso seu estabelecimento já possua usuário Master, 
//                                                       você pode solicitar que ele faça a criação do seu usuário.",
//                                                    this.QtdTentativas - entidade.QuantidadeConfirmacaoPositiva);
                            }
                            else
                            {
                                mensagem = "Atenção: A quantidade de confirmação positiva foi esgotada";
                            }

                            qtdTentativas = entidade.QuantidadeConfirmacaoPositiva;
                        }
                    }
                }
            }
            else //Recuperação de Senha/Usuário e Liberação de acesso completo
            {
                using (var client = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                {
                    Int32 codigoIdUsuario = default(Int32);

                    //Recuperação de Senha/Usuário
                    if (InformacaoUsuario.Existe())
                        codigoIdUsuario = InformacaoUsuario.Recuperar().IdUsuario;
                    //Liberação de acesso completo
                    else
                        codigoIdUsuario = SessaoAtual.CodigoIdUsuario;

                    var usuario = client.Cliente.ConsultarPorID(out qtdTentativas, codigoIdUsuario);
                    if (qtdTentativas == 0 && usuario != null)
                    {
                        if (!usuario.BloqueadoConfirmacaoPositiva)
                        {

                            //mensagem = String.Format(@"Você ainda tem <b>{0}</b> tentativas.",
                            //                        this.QtdTentativas - usuario.QuantidadeTentativaConfirmacaoPositiva);
                            titulo = String.Format(@"Atenção: os dados informados estão incorretos. Você ainda tem <b>{0}</b> tentativas. <br />", this.QtdTentativas - usuario.QuantidadeTentativaConfirmacaoPositiva);
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
        /// Indicador se a Confirmação Positiva é para Criação de Usuário
        /// </summary>
        /// <returns></returns>
        private Boolean ModoCriacaoUsuario()
        {
            return (this.ObterSessaoAberto
                    && !this.BloquearUsuario);
        }

        /// <summary>
        /// Indicador se a Confirmação Positiva é para Recuperação de Usuário
        /// </summary>
        /// <returns></returns>
        private Boolean ModoRecuperacaoUsuario()
        {
            return (this.ObterSessaoAberto
                    && this.BloquearUsuario
                    && this.RecuperacaoUsuario);
        }

        /// <summary>
        /// Indicador se a Confirmação Positiva é para Recuperação de Senha
        /// </summary>
        /// <returns></returns>
        private Boolean ModoRecuperacaoSenha()
        {
            return (this.ObterSessaoAberto
                    && this.BloquearUsuario
                    && !this.RecuperacaoUsuario);
        }

        /// <summary>
        /// Indicador se a Confirmação Positiva é para Recuperação de Senha
        /// </summary>
        /// <returns></returns>
        private Boolean ModoLiberacaoAcesso()
        {
            return (!this.ObterSessaoAberto
                    && this.BloquearUsuario);
        }

        /// <summary>
        /// Recuperar String com o Tipo de Confirmação Positiva para o Histórico de Log
        /// </summary>
        /// <returns>Tipo de Confirmação Positiva</returns>
        private String RecuperarTipoRecuperacao()
        {
            String tipo = String.Empty;

            if (this.ModoCriacaoUsuario())
                tipo = "Acesso Básico";
            else if (this.ModoLiberacaoAcesso())
                tipo = "Acesso Completo";
            else if (this.ModoRecuperacaoSenha())
                tipo = "Recuperação de Senha";
            else if (this.ModoRecuperacaoUsuario())
                tipo = "Recuperação de Usuário";

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
                    .Select(p => descricaoPerguntasAleatorias[p.CodigoPergunta]).ToList();

                //Obtém a descrição das perguntas básicas que forma respondidas incorretamente
                var basicasIncorretas = perguntasIncorretas
                    .Where(p => !p.PerguntaVariavel && descricaoPerguntasBasicas.ContainsKey(p.CodigoPergunta))
                    .Select(p => descricaoPerguntasBasicas[p.CodigoPergunta]).ToList();

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
                    Historico.ErroConfirmacaoPositiva(
                        usuario.IdUsuario, usuario.NomeCompleto, usuario.EmailUsuario,
                        usuario.TipoUsuario, usuario.NumeroPV, tipoConfirmacaoPositiva,
                        dadosIncorretos.Distinct().ToArray());
                }
                else if (SessaoAtual != null)
                {
                    Historico.ErroConfirmacaoPositiva(
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
                    String url = String.Empty;

                    #region [Dados Entidade/Usuário]
                    if (this.ObterSessaoAberto)
                    {
                        InformacaoUsuario info = InformacaoUsuario.Recuperar();
                        entidade = new UsuarioServico.Entidade();

                        if (info != null)
                        {
                            entidade.GrupoEntidade = new UsuarioServico.GrupoEntidade()
                            {
                                Codigo = info.GrupoEntidade
                            };
                            entidade.Codigo = info.NumeroPV;
                            idUsuario = info.IdUsuario;
                            emailUsuario = info.EmailUsuario;
                        }
                        else
                        {
                            url = String.Format("{0}/Paginas/Mobile/RecuperacaoUsuarioIdentificacao.aspx", base.web.ServerRelativeUrl);
                            Response.Redirect(url, false);
                        }
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
                        || this.ModoRecuperacaoSenha()
                        || this.ModoRecuperacaoUsuario())
                    {
                        var usuario = contextoUsuario.Cliente.ConsultarPorID(out codigoRetornoBloqueio, idUsuario);

                        if (usuario != null)
                        {
                            if (usuario.QuantidadeTentativaConfirmacaoPositiva >= this.QtdTentativas)
                            {
                                bloqueada = true;

                                if (this.ModoRecuperacaoSenha()
                                    || this.ModoRecuperacaoUsuario()) //Recuperação Senha/Usuário
                                {
                                    this.ExibirAvisoBloqueioSenhaUsuario("Atenção: a quantidade de tentativas foi esgotada",
                                                                         "Redecard.PN.DadosCadastrais.SharePoint.Login.Logar",
                                                                         1103,
                                                                         QuadroAvisosResponsivo.IconeMensagem.Erro);

                                    InformacaoUsuario.Limpar();
                                }
                                else if (this.ModoLiberacaoAcesso()) //Liberação de Acesso Completo
                                {
                                    String mensagemBloqueio = @"Você excedeu o limite de tentativas na Confirmação Positiva 
                                                                            e só poderá acessar os seguintes serviços:<br />
                                                                            <ul style='padding-left:10px'>
                                                                                <li>Extratos</li>
                                                                                <li>Comprovantes de Vendas</li>
                                                                                <li>Pedidos de materiais: solicitação de bobina e material de divulgação</li>
                                                                                <li>Outros serviços: consultas a taxas, aluguéis e demais</li>
                                                                            </ul><br /><br />
                                                                            Para realizar o desbloqueio e liberar o seu acesso completo entre em contato com a nossa
                                                                            Central de Atendimento e verifique seus dados de cadastro.<br />
                                                                            <b>4001 4433 (Capital e Regiões Metropolitanas)</b><br />
                                                                            <b>0800 728 4433 (Demais Regiões)</b>";
                                    this.ExibirAvisoBloqueioSenhaUsuario("Acesso completo bloqueado",
                                                                         mensagemBloqueio,
                                                                         QuadroAvisosResponsivo.IconeMensagem.Erro);

                                    InformacaoUsuario.Limpar();
                                }
                            }
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

                                this.ExibirAvisoBloqueioEntidade("Atenção: a quantidade de tentativas foi esgotada",
                                                                 "ConfirmacaoPositiva.BloquearCriacaoUsuario",
                                                                 1106,
                                                                 false,
                                                                 QuadroAvisosResponsivo.IconeMensagem.Erro);

                                InformacaoUsuario.Limpar();
                            }
                        }
                    }
                }
            }

            return bloqueada;
        }

        #endregion
    }
}
