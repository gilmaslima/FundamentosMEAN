/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Redecard.PN.Comum;
using System.Linq;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using System.ServiceModel;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios
{
    /// <summary>
    /// Classe base para os UserControls das WebParts de Usuários
    /// </summary>
    public abstract class UsuariosUserControlBase : UserControlBase
    {
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


        /// <summary>
        /// Verificação inicial das telas de Usuários
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
 	        base.OnInit(e);

            if (ValidarNavegacao)
                this.VerificarNavegacao();

            //Se for usuário não migrado, força redirecionamento para tela de Migração
            if (Sessao.Contem() && this.SessaoAtual.Legado)
            {
                //Nome da página de cadastro da migração do usuário
                String paginaCadastroUsuarioMigracao = "CadastroUsuarioMigracao.aspx";

                //Redireciona para página de cadastro de migração do usuário
                Response.Redirect(paginaCadastroUsuarioMigracao);
            }
        }

#if DEBUG
        /// <summary>
        /// DEBUG dos dados do usuário e Edição/Criação
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
 	        base.OnPreRender(e);

            if (this.UsuarioSelecionado != null)
            {
                this.Controls.Add(new LiteralControl("DEBUG"));
                this.Controls.Add(new HtmlGenericControl("br"));
                this.Controls.Add(new LiteralControl(this.GuidUsuarioSelecionado));
                this.Controls.Add(new HtmlGenericControl("br"));
                this.Controls.Add(new LiteralControl(this.UsuarioSelecionado.ToString()));
            }
        }
#endif

        #region [ Propriedades ]

        /// <summary>
        /// Valida se o processo de navegação está correto.
        /// Se o usuário digitar diretamente a URL, retorna para
        /// tela principal de Usuários.
        /// </summary>
        public virtual Boolean ValidarNavegacao { get { return true; } }

        /// <summary>
        /// Indica se o usuário autenticado é usuário Master ou Central de Atendimento
        /// </summary>
        public Boolean PossuiPermissao
        {
            get
            {
                Boolean usuarioMaster = SessaoAtual != null && SessaoAtual.UsuarioMaster();
                Boolean usuarioAtendimento = SessaoAtual != null && SessaoAtual.UsuarioAtendimento;
                return usuarioMaster || usuarioAtendimento;
            }
        }

        /// <summary>
        /// Modo de funcionamento da página: Edicao ou Criacao
        /// </summary>
        public String Modo { get; set; }

        /// <summary>
        /// Modo de Edição de Usuário
        /// </summary>
        protected Boolean ModoEdicao
        {
            get { return String.Compare("Edicao", Modo, true) == 0; }
        }

        /// <summary>
        /// Modo de Criação de Usuário
        /// </summary>
        protected Boolean ModoCriacao
        {
            get { return String.Compare("Criacao", Modo, true) == 0; }
        }

        /// <summary>
        /// Modo de Criação de Usuário
        /// </summary>
        protected Boolean ModoAprovacao
        {
            get { return String.Compare("Aprovacao", Modo, true) == 0; }
        }

        /// <summary>
        /// QueryStringSegura
        /// </summary>
        protected QueryStringSegura QS
        {
            get
            {
                var qs = default(QueryStringSegura);
                String dados = Request.QueryString["dados"];
                if (!String.IsNullOrEmpty(dados))
                {
                    try { qs = new QueryStringSegura(dados); }
                    catch (QueryStringExpiradaException ex) 
                    {
                        Logger.GravarErro("Erro durante recuperação da QueryString", ex, dados);
                    }
                    catch (QueryStringInvalidaException ex) 
                    {
                        Logger.GravarErro("Erro durante recuperação da QueryString", ex, dados);
                    }
                }
                return qs;
            }
        }

        /// <summary>
        /// GUID do usuário selecionado
        /// </summary>
        public static String ChaveGuidUsuarioSelecionado { get { return "GuidUsuarioSelecionado"; } }

        /// <summary>
        /// GUID do usuário selecionado (dados originais, sem modificações)
        /// </summary>
        public static String ChaveGuidUsuarioSelecionadoOriginal { get { return "GuidUsuarioSelecionadoOriginal"; } }

        /// <summary>
        /// Chave do objeto de sessão do usuário sendo Criado ou Editado.
        /// </summary>
        protected String GuidUsuarioSelecionado
        {
            get 
            {
                if (ViewState[ChaveGuidUsuarioSelecionado] == null)
                {
                    if (QS != null && QS[ChaveGuidUsuarioSelecionado] != null)
                        ViewState[ChaveGuidUsuarioSelecionado] = QS[ChaveGuidUsuarioSelecionado];
                    else
                        ViewState[ChaveGuidUsuarioSelecionado] = Guid.NewGuid().ToString();
                }

                return (String)ViewState[ChaveGuidUsuarioSelecionado];
            }
            set
            {
                ViewState[ChaveGuidUsuarioSelecionado] = value;
            }
        }

        /// <summary>
        /// Chave do objeto de sessão do usuário sendo Criado ou Editado (dados originais).
        /// </summary>
        protected String GuidUsuarioSelecionadoOriginal
        {
            get
            {
                if (ViewState[ChaveGuidUsuarioSelecionadoOriginal] == null)
                {
                    if (QS != null && QS[ChaveGuidUsuarioSelecionadoOriginal] != null)
                        ViewState[ChaveGuidUsuarioSelecionadoOriginal] = QS[ChaveGuidUsuarioSelecionadoOriginal];
                    else
                        ViewState[ChaveGuidUsuarioSelecionadoOriginal] = Guid.NewGuid().ToString();
                }

                return (String)ViewState[ChaveGuidUsuarioSelecionadoOriginal];
            }
            set
            {
                ViewState[ChaveGuidUsuarioSelecionadoOriginal] = value;
            }
        }

        /// <summary>
        /// Objeto referente ao usuário sendo Criado ou Editado
        /// </summary>
        protected DadosUsuario UsuarioSelecionado
        {
            get { return (DadosUsuario) Session[this.GuidUsuarioSelecionado]; }
            set { Session[this.GuidUsuarioSelecionado] = value; }
        }

        /// <summary>
        /// Objeto referente ao usuário sendo Criado ou Editado (dados originais)
        /// </summary>
        protected DadosUsuario UsuarioSelecionadoOriginal
        {
            get { return (DadosUsuario)Session[this.GuidUsuarioSelecionadoOriginal]; }
            set { Session[this.GuidUsuarioSelecionadoOriginal] = value; }
        }

        #endregion

        #region [ Métodos ]

        /// <summary>
        /// Redireciona para a tela principal de Usuários
        /// </summary>
        protected void RedirecionarParaUsuarios()
        {
            Response.Redirect("Usuarios.aspx", false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Redireciona para a tela consulta de aprovações pendentes
        /// </summary>
        protected void RedirecionarParaAprovacao()
        {
            String url = "Usuarios.aspx"; // aprovação de acessos fica junto com a tela adm de usuarios

            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Redireciona para alguma página dos Passos de Criação/Edição de Usuários,
        /// passando o Guid do objeto em sessão do usuário sendo editado/criado.
        /// </summary>
        /// <param name="pagina">URL da página do passo</param>
        /// <param name="queryString">QueryString com informações adicionais, caso necessário</param>
        protected void RedirecionarPasso(String pagina, QueryStringSegura queryString)
        {
            if (queryString == null)
                queryString = new QueryStringSegura();

            queryString[ChaveGuidUsuarioSelecionado] = this.GuidUsuarioSelecionado;
            queryString[ChaveGuidUsuarioSelecionadoOriginal] = this.GuidUsuarioSelecionadoOriginal;

            String url = String.Format("{0}?dados={1}", pagina, queryString.ToString());

            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }        

        /// <summary>
        /// Verifica se a navegação foi feita corretamente, ou seja,
        /// a partir da tela inicial de Usuários, acessando em modo
        /// de edição ou criação
        /// </summary>
        private void VerificarNavegacao()
        {
            // tratamento para a página se estiver em modo edição
            if (SPContext.Current.FormContext.FormMode == SPControlMode.Edit)
                return;

            // tratamento para a página se estiver em modo edição
            if (Util.UsusarioAdministrador())
                return; 

            Boolean permitirNavegacao = this.UsuarioSelecionado != null;                    

            if (!permitirNavegacao)
            {
                //Interrompe execução da página, e transfere 
                //para a página principal de usuários
                Response.Redirect("Usuarios.aspx", true);
            }
        }

        /// <summary>
        /// Altera os dados do usuario na base
        /// </summary>
        protected void AlterarUsuario(String passo, out Boolean emailAtualizado)
        {
            using (Logger log = Logger.IniciarLog(String.Format("Alteração de Cadastro do Usuário - {0}", passo)))
            {
                //Verifica se deverá ser atualizar a senha
                Boolean atualizarSenha = String.Compare(this.UsuarioSelecionado.Senha,
                    this.UsuarioSelecionadoOriginal.Senha, true) != 0;

                //Verifica se e-mail será atualizado
                emailAtualizado = String.Compare(this.UsuarioSelecionado.Email,
                    this.UsuarioSelecionadoOriginal.Email, true) != 0;

                Int32 codigoRetorno = default(Int32);
                Guid hashConfirmacaoEmail = default(Guid);
                Boolean possuiKomerci = false;
                UsuarioServico.Status1? status = emailAtualizado ?
                    UsuarioServico.Status1.UsuarioAguardandoConfirmacaoAlteracaoEmail : (UsuarioServico.Status1?)null;

                //Verifica se algum PV do estabelecimento possui Komerci
                using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    possuiKomerci = ctx.Cliente.PossuiKomerci(this.UsuarioSelecionado.Estabelecimentos.ToArray());

                    //codigoRetorno = ctx.Cliente.ValidarCriarEntidade(this.UsuarioSelecionado.Estabelecimentos.ToArray(), 1);
                }

                //Verifica se alguma das entidades é inválida
                if (codigoRetorno != 0)
                {
                    base.ExibirPainelExcecao("EntidadeServico.ValidarCriarEntidade", codigoRetorno);
                    return;
                }

                //Atualizando os dados do usuário
                using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                    codigoRetorno = ctx.Cliente.Atualizar2(
                        out hashConfirmacaoEmail,
                        possuiKomerci,
                        1, //fixo: sempre deve ser Estabelecimento
                        this.UsuarioSelecionado.CodigoEstabelecimentos,
                        String.Empty, //Login (parâmetro não utilizado no NovoAcesso)
                        this.UsuarioSelecionado.Nome,
                        this.UsuarioSelecionado.TipoUsuario,
                        atualizarSenha ? this.UsuarioSelecionado.Senha : String.Empty,
                        this.UsuarioSelecionado.CodigoServicos,
                        this.UsuarioSelecionado.CodigoIdUsuario.Value,
                        this.UsuarioSelecionado.Email,
                        this.UsuarioSelecionado.EmailSecundario,
                        this.UsuarioSelecionado.Cpf,
                        this.UsuarioSelecionado.CelularDdd,
                        this.UsuarioSelecionado.CelularNumero,
                        status,
                        this.UsuarioSelecionadoOriginal.Legado ? 60.ToString().ToInt32Null() : null,
                        this.UsuarioSelecionadoOriginal.Legado ? ObterDataMigracao() : null);

                //Verifica se atualização do usuário foi realizado com sucesso
                if (codigoRetorno != 0)
                {
                    base.ExibirPainelExcecao("UsuarioServico.Atualizar2", codigoRetorno);
                    return;
                }

                //Segue fluxo de alteração de e-mail, com envio de e-mail ao usuário
                if (emailAtualizado)
                {
                    List<Int32> estabelecimentos = new System.Collections.Generic.List<int>();
                    if (this.UsuarioSelecionado.Estabelecimentos.Any())
                        estabelecimentos = new System.Collections.Generic.List<int>() { this.UsuarioSelecionado.Estabelecimentos.First() };
                    //Envia e-mail para o usuário informando-o da atualização de seu e-mail,
                    //sem prazo de expiração
                    EmailNovoAcesso.EnviarEmailConfirmacaoCadastro(
                        SessaoAtual,
                        this.UsuarioSelecionado.Email,
                        estabelecimentos,
                        this.UsuarioSelecionado.CodigoIdUsuario.Value,
                        hashConfirmacaoEmail);
                }

                //Armazena no histórico/log de atividades
                Historico.CompararModelos(this.UsuarioSelecionado, this.UsuarioSelecionadoOriginal)
                    .Campo(u => u.Nome, "nome")
                    .Campo(u => u.Email, "e-mail")
                    .Campo(u => u.Cpf, "CPF")
                    .Campo(u => u.TipoUsuario, "perfil")
                    .Campo(u => u.EmailSecundario, "e-mail secundário")
                    .Campo(u => u.CelularDdd, "DDD celular")
                    .Campo(u => u.CelularNumero, "celular")
                    .Campo(u => u.Estabelecimentos, "estabelecimentos")
                    .Campo(u => u.Servicos, "serviços")
                    .Campo(atualizarSenha ? this.UsuarioSelecionado.Senha : this.UsuarioSelecionadoOriginal.Senha,
                        this.UsuarioSelecionadoOriginal.Senha, "senha")
                    .AlteracaoDadosOutroUsuario(SessaoAtual,
                        this.UsuarioSelecionado.CodigoIdUsuario.Value,
                        this.UsuarioSelecionado.Nome,
                        this.UsuarioSelecionado.Email,
                        this.UsuarioSelecionado.TipoUsuario);
            }
        }

        /// <summary>
        /// Recuperar a data de expiração a ser contabilizada para o Usuário Legado
        /// </summary>
        /// <returns></returns>
        private DateTime? ObterDataMigracao()
        {
            DateTime dataMigracao = new DateTime();

            if (DateTime.Now < Convert.ToDateTime(ListaPeriodoMigracao.Items[0]["DataFinal"]))
                dataMigracao = Convert.ToDateTime(ListaPeriodoMigracao.Items[0]["DataInicio"]);
            else
                dataMigracao = Convert.ToDateTime(ListaPeriodoMigracao.Items[0]["DataFinal"]);

            return dataMigracao;
        }

        /// <summary>
        /// Finaliza o processo de criação, removendo o usuário em edição/criação
        /// da sessão
        /// </summary>
        protected void Encerrar()
        {
            this.UsuarioSelecionado = null;
            this.UsuarioSelecionadoOriginal = null;
        }

        /// <summary>
        /// Redireciona para o passo de dados cadastro,
        /// de acordo com o modo de funcionamento da página
        /// </summary>
        protected void RedirecionarPassoDadosCadastro()
        {
            String pagina = default(String);
            if (this.ModoCriacao)
                pagina = "UsuariosCriacaoDadosCadastro.aspx";
            else if (this.ModoEdicao)
                pagina = "UsuariosEdicaoDadosCadastro.aspx";

            RedirecionarPasso(pagina, null);
        }

        /// <summary>
        /// Redireciona para o próximo passo, de Estabelecimentos Permitidos,
        /// de acordo com o modo de funcionamento da página
        /// </summary>
        protected void RedirecionarPassoEstabelecimentos()
        {
            String pagina = default(String);
            if (this.ModoCriacao)
                pagina = "UsuariosCriacaoEstabelecimentos.aspx";
            else if (this.ModoEdicao)
                pagina = "UsuariosEdicaoEstabelecimentos.aspx";

            RedirecionarPasso(pagina, null);
        }

        /// <summary>
        /// Redireciona para o passo, de Permissões,
        /// de acordo com o modo de funcionamento da página
        /// </summary>
        protected void RedirecionarPassoPermissoes()
        {
            String pagina = default(String);
            if (this.ModoCriacao)
                pagina = "UsuariosCriacaoPermissoes.aspx";
            else if (this.ModoEdicao)
                pagina = "UsuariosEdicaoPermissoes.aspx";
            else if (this.ModoAprovacao)
                pagina = "AprovacaoSolicitacaoPermissoes.aspx";

            RedirecionarPasso(pagina, null);
        }



        /// <summary>
        /// Redireciona para o próximo passo, de Confirmação,
        /// de acordo com o modo de funcionamento da página
        /// </summary>
        protected void RedirecionarPassoConfirmacao()
        {
            String pagina = default(String);
            if (this.ModoCriacao)
                pagina = "UsuariosCriacaoConfirmacao.aspx";
            else if (this.ModoEdicao)
                pagina = "UsuariosEdicaoConfirmacao.aspx";
            else if (this.ModoAprovacao)
                pagina = "AprovacaoUsuarioConfirmacao.aspx";

            this.RedirecionarPasso(pagina, null);
        }
        #endregion
    }
}