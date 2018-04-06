#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [05/06/2012] – [André Garcia] – [Criação]
- [15/04/2016] – [Seygi Kutani] – [Rotina para que essa página possa ser acessada apos o usuario ja estar logado e ja ter selecionado um estabelecimento]
- [26/09/2016] - [Roger Santos] - Criação de consulta em uma lista para verificar se o login deve ser feito via WCF ou API
*/
#endregion

using Microsoft.IdentityModel.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using Redecard.PN.DadosCadastrais.SharePoint.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using ControlesDadosCadastrais = Redecard.PNCadastrais.Core.Web.Controles.Portal;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{

    /// <summary>
    /// Página que exibe o quadro de aviso conforma parâmetros enviados para a página
    /// </summary>
    public partial class LoginSelecionaEstabelecimento : ApplicationPageBaseAnonima
    {
        protected string LoginSelecionaEstabelecimentoTipoEntidade
        {
            get
            {
                if (ViewState["LoginSelecionaEstabelecimentoTipoEntidade"] != null)
                {
                    return Convert.ToString(ViewState["LoginSelecionaEstabelecimentoTipoEntidade"]);
                }
                return null;
            }
            set
            {
                ViewState["LoginSelecionaEstabelecimentoTipoEntidade"] = value;
            }
        }

        protected string LoginSelecionaEstabelecimentoUsuario
        {
            get
            {
                if (ViewState["LoginSelecionaEstabelecimentoUsuario"] != null)
                {
                    return Convert.ToString(ViewState["LoginSelecionaEstabelecimentoUsuario"]);
                }
                return null;
            }
            set
            {
                ViewState["LoginSelecionaEstabelecimentoUsuario"] = value;
            }
        }

        protected string LoginSelecionaEstabelecimentoSenha
        {
            get
            {
                if (ViewState["LoginSelecionaEstabelecimentoSenha"] != null)
                {
                    return Convert.ToString(ViewState["LoginSelecionaEstabelecimentoSenha"]);
                }
                return null;
            }
            set
            {
                ViewState["LoginSelecionaEstabelecimentoSenha"] = value;
            }
        }

        protected string LoginIndicadorSenhaCriptografada
        {
            get
            {
                if (ViewState["LoginIndicadorSenhaCriptografada"] != null)
                {
                    return Convert.ToString(ViewState["LoginIndicadorSenhaCriptografada"]);
                }
                return null;
            }
            set
            {
                ViewState["LoginIndicadorSenhaCriptografada"] = value;
            }
        }

        /// <summary>
        /// URL para redirecionamento após o login do usuário
        /// </summary>
        protected String LoginRedirect
        {
            get
            {
                if (ViewState["LoginRedirect"] != null)
                {
                    return Convert.ToString(ViewState["LoginRedirect"]);
                }
                return null;
            }
            set
            {
                ViewState["LoginRedirect"] = value;
            }
        }

        protected int TipoNaoDefinido
        {
            get
            {
                return -1;
            }
        }


        protected string HashUsuario
        {
            get
            {
                if (Session["hashacessopn"] != null)
                {
                    return Convert.ToString(Session["hashacessopn"]);
                }
                return null;
            }
            set
            {
                Session["hashacessopn"] = value;
            }
        }

        protected string HashCriptografada
        {
            get
            {
                if (Session["hashacessopnhasencript"] != null)
                {
                    return Convert.ToString(Session["hashacessopnhasencript"]);
                }
                return null;
            }
            set
            {
                Session["hashacessopnhasencript"] = value;
            }
        }

        /// <summary>
        /// Exibe quadro de aviso personalizado conforma parâmetros passados
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (Logger Log = Logger.IniciarLog("LOGIN - Login no portal Redecard área fechada selecionar Estabelecimento"))
                {
                    //PEGAR PARÂMETROS DO POST
                    String redirectUrl = String.Empty;
                    String codigoGrupoEntidade = RecuperarParametro("estabelecimento");
                    String codigoEntidade = RecuperarParametro("ncadastro");
                    String codigoNomeUsuario = RecuperarParametro("usuario");
                    String senha = RecuperarParametro("senha");
                    String senhaCriptografada = RecuperarParametro("senha");

                    //Caso venha do header deve mudar o titulo da pagina
                    String origem = RecuperarParametro("origem");
                    if (String.Compare(origem, "Header", true) == 0)
                    {
                        txtHeader.Text = "Acesso a estabelecimentos";
                        btnVoltar.Visible = true;
                    }

                    //Indica se a senha obtida de Request.Form["senha"] já está criptografada.//Login preenchido no formulário de Liberação de Acesso Completo
                    String indicadorSenhaCriptografada = RecuperarParametro("indSenhaCript");
                    //Indicador se deverá direcionar para a liberação de acesso
                    String liberarAcesso = RecuperarParametro("liberarAcesso");


                    //Recuperar Parametro para rotina de redirecionamento
                    String urlRedirect = RecuperarParametro("urlRedirect");
                    //

                    try
                    {
                        //Validar campos, se um deles for nulo chamar Login.aspx e delegar a validação dos campos para a tela Login.aspx (codigoEntidade vai ser selecionado agora)
                        if (codigoGrupoEntidade == null || codigoNomeUsuario == null || senha == null || codigoGrupoEntidade == string.Empty || codigoNomeUsuario == string.Empty || senha == string.Empty)
                            AutenticaLogin(codigoGrupoEntidade, codigoEntidade, codigoNomeUsuario, senha, indicadorSenhaCriptografada, urlRedirect);

                        //AAL - 18/03/2016: Alterado para não criptografar mais a senha. A API irá criptografar
                        //Chamar Criptografia
                        //CriptografiaSenha(ref senhaCriptografada, ref indicadorSenhaCriptografada, Log);
                        if (String.Compare("N", indicadorSenhaCriptografada, true) == 0
                            || String.IsNullOrEmpty(indicadorSenhaCriptografada))
                        {
                            Log.GravarMensagem("Aplicar criptografia");
                            SharePointUlsLog.LogMensagem("Aplicar criptografia");
                            // Aplicar criptografia
                            senhaCriptografada = EncriptadorSHA1.EncryptString(senhaCriptografada);
                        }

                        //VERIFICAR SE EXISTE MAIS DE UM ESTABELECIMENTO //sender sim renderizar grid //se não enviar para Login.aspx um post para logar usando o PV codigo entidade retornado na lista
                        //Chamar serviço que lista os Estabelecimentos (PVs) para este e-mail
                        //List<Entidade1> lstEstabelecimentos = ConsultarEstabelecimentosEmail(codigoNomeUsuario);

                        //SUBISTITUIR POR PROCEDURE QUE RETORNA DATA ULTIMO ACESSO E FILTRA POR SENHA
                        //Chamar serviço que lista os Estabelecimentos (PVs) para este e-mail
                        //List<Entidade1> lstEstabelecimentos = ConsultarEstabelecimentosEmailSenhaHash(codigoNomeUsuario, senha);

                        //Verifica a partir de uma lista no Sharepoint se deve consumir o login pela nova API ou WCF
                        bool utilizaApiLogin = ConfiguracaoLogin.UtilizaApiLogin();

                        //SUBISTITUIR POR PROCEDURE QUE RETORNA DATA ULTIMO ACESSO E FILTRA POR SENHA
                        //Chamar serviço que lista os Estabelecimentos (PVs) para este e-mail
                        List<PortalApi.Modelo.EntidadeRetorno> lstEstabelecimentos = null;

                        if (utilizaApiLogin)
                        {
                            lstEstabelecimentos = ConsultarApiEstabelecimentosEmailSenhaHash(codigoNomeUsuario, senha);

                            //Convertendo a Data de último Login para DateTime para ordenação
                            foreach (PortalApi.Modelo.EntidadeRetorno entidade in lstEstabelecimentos)
                            {
                                entidade.DataAlteracaoDt = Convert.ToDateTime(entidade.DataAlteracao);
                            }
                        }
                        else
                        {
                            List<Entidade1> lstWcfEstabelecimentos = ConsultarWcfEstabelecimentosEmailSenhaHash(codigoNomeUsuario, senhaCriptografada);
                            lstEstabelecimentos = ConfiguracaoLogin.ConverteParaEntidadeApi(lstWcfEstabelecimentos);
                        }

                        //for (int i = 0; i < 10; i++)
                        //{
                        //    Entidade1 obj = new Entidade1();
                        //    obj.Codigo = 123456;
                        //    obj.Descricao = "teste";
                        //    obj.TipoEstabelecimento = 1;
                        //    obj.Status = "x";

                        //    lstEstabelecimentos.Add(obj);
                        //}

                        //Para cada Item retornado da lista, consultar dados no GE para exibir na tela
                        //ComplementarDados(lstEstabelecimentos);


                        //Entidade1 obj = new Entidade1();
                        //obj.Status;
                        //obj.TipoEntidade;

                        //Existe mais de um estabelecimento utilizando este e-mail como login e tipo é ESTABELECIMENTO
                        if (lstEstabelecimentos.Count > 1)
                        {
                            //Renderizar o grid
                            //grvEstabelecimentos.DataSource = lstEstabelecimentos;
                            //grvEstabelecimentos.DataBind();

                            //Convertendo a Data de último Login para DateTime para ordenação
                            foreach (PortalApi.Modelo.EntidadeRetorno entidade in lstEstabelecimentos)
                            {
                                entidade.DataAlteracaoDt = Convert.ToDateTime(entidade.DataAlteracao);
                            }

                            //Renderizar o item mais recente
                            var obj = lstEstabelecimentos.OrderByDescending(p => p.DataAlteracaoDt).First();
                            ControlesDadosCadastrais.QuadroAviso quadro = (ControlesDadosCadastrais.QuadroAviso)qdConfirmacao;
                            hdfCodEstabelecimentoContinuar.Value = obj.Codigo.ToString();
                            quadro.Mensagem = String.Format("Seu último acesso foi com o estabelecimento <span class='bold'>{0} - {1}</span>", obj.Codigo.ToString(), obj.Nome.ToString());

                            //Renderizar o Repeater
                            rptEstabelecimentos.DataSource = lstEstabelecimentos.OrderByDescending(p => p.DataAlteracao);
                            rptEstabelecimentos.DataBind();

                            //Adiciona a função de JS de paginação do grid para as funções a serem executadas após renderização
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao", "pageResultTable('tblEstabelecimentos', 1, 10, 5);", true);

                            //Preencher dados de campo auxiliares
                            LoginSelecionaEstabelecimentoTipoEntidade = codigoGrupoEntidade;
                            LoginSelecionaEstabelecimentoUsuario = codigoNomeUsuario;
                            LoginSelecionaEstabelecimentoSenha = senha;
                            LoginIndicadorSenhaCriptografada = indicadorSenhaCriptografada;
                            LoginRedirect = urlRedirect;
                        }
                        else
                        {
                            if (lstEstabelecimentos.Count == 1)
                                //Existe apenas um estabelecimento utilizando este e-mail como login - logar com este item
                                AutenticaLogin(codigoGrupoEntidade, lstEstabelecimentos[0].Codigo.ToString(), codigoNomeUsuario, senha, indicadorSenhaCriptografada, urlRedirect);
                            else
                                //Tratamento de erro de senha ocorre nesta página pois o filtro é baseado no usuário e senha digitados (Requisição do cliente)
                                ExibirMensagemUsuarioSenhaIncorreta(codigoNomeUsuario, "Redecard.PN.DadosCadastrais.SharePoint.Login.Logar", 391);

                        }
                    }
                    catch (FaultException<EntidadeServico.GeneralFault> ex)
                    {
                        Log.GravarErro(ex);
                        this.ExibirMensagemErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32(), "Atenção", 1);
                    }
                    catch (FaultException<UsuarioServico.GeneralFault> ex)
                    {
                        Log.GravarErro(ex);
                        this.ExibirMensagemErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32(), "Atenção", 1);
                    }
                    catch (PortalRedecardException ex)
                    {
                        Log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);
                        this.ExibirMensagemErro(ex.Fonte, ex.Codigo, "Atenção", 1);
                    }
                    catch (ThreadAbortException ex)
                    {
                        SharePointUlsLog.LogMensagem("Redirecionamento do Login");
                        Log.GravarMensagem("Redirecionamento do Login", new { Mensagem = ex.Message });
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);
                        this.ExibirMensagemErro(FONTE, CODIGO_ERRO, "Atenção", 1);
                    }
                }
            }
        }

        /// <summary>
        /// Verifica a quantidade de tentativas de login que o usuário ainda tem e exibe mensagem
        /// </summary>
        private void ExibirMensagemUsuarioSenhaIncorreta(String codigoNomeUsuario, String fonte, Int32 codigo)
        {

            using (Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico.UsuarioServicoClient client = new Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico.UsuarioServicoClient())
            {
                Int32 retorno = 0;

                // ALTERADO PARA O CONSUMO DA API REST NODEJS 31-03-2016
                //Pegar todos os PVs do usuário de email e executar um foreach para envio de email de negção
                //List<Entidade1> lstEstabelecimentos = ConsultarEstabelecimentosEmailSenhaHash(codigoNomeUsuario, null);
                //List<PortalApi.Modelo.EntidadeRetorno> lstEstabelecimentos = ConsultarApiEstabelecimentosEmailSenhaHash(codigoNomeUsuario, null);

                //Verifica a partir de uma lista no Sharepoint se deve consumir o login pela nova API ou WCF
                bool utilizaApiLogin = ConfiguracaoLogin.UtilizaApiLogin();

                List<PortalApi.Modelo.EntidadeRetorno> lstEstabelecimentos = null;

                if (utilizaApiLogin)
                {
                    lstEstabelecimentos = ConsultarApiEstabelecimentosEmailSenhaHash(codigoNomeUsuario, null);
                }
                else
                {
                    List<Entidade1> lstWcfEstabelecimentos = ConsultarWcfEstabelecimentosEmailSenhaHash(codigoNomeUsuario, null);
                    lstEstabelecimentos = ConfiguracaoLogin.ConverteParaEntidadeApi(lstWcfEstabelecimentos);
                }

                //Senha Incorreta
                if (lstEstabelecimentos != null && lstEstabelecimentos.Count() > 0)
                {

                    //codigo de uma Entidade para recuperar os dados
                    Int32 numeroPv = lstEstabelecimentos[0].Codigo;

                    //codigoNomeUsuario
                    var usuarios = client.ConsultarPorCodigoEntidade(out retorno, codigoNomeUsuario,
                        new UsuarioServico.Entidade()
                        {
                            Codigo = numeroPv,
                            GrupoEntidade = new UsuarioServico.GrupoEntidade() { Codigo = 1 }
                        }
                    );

                    if (retorno.Equals(0) && usuarios.Length > 0)
                    {
                        Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico.Usuario usuario = usuarios[0];

                        String mensagem = String.Empty;
                        Int32 tentativas = (6 - usuarios[0].QuantidadeTentativaLoginIncorreta);

                        if (tentativas <= 0)
                        {
                            // ALTERADO PARA O CONSUMO DA API REST NODEJS 31-03-2016
                            //Enviar email para cada usuário bloqueado
                            foreach (/*Entidade1*/PortalApi.Modelo.EntidadeRetorno oEntidade in lstEstabelecimentos)
                            {

                                //Armazena no histórico/log de atividades
                                Historico.BloqueioUsuarioErroSenha(
                                    usuario.CodigoIdUsuario, usuario.Descricao, usuario.Email, usuario.TipoUsuario, oEntidade.Codigo);

                                //Envia e-mail de bloqueio de usuário para o usuário
                                EmailNovoAcesso.EnviarEmailAcessoBloqueado(usuario.Descricao, usuario.Email,
                                    usuario.CodigoIdUsuario, usuario.TipoUsuario, oEntidade.Codigo, null);
                            }

                            this.RedirecionarAviso("Atenção: A quantidade de tentativas foi esgotada", base.RetornarMensagemErro(fonte, 1103), 1);
                        }
                        else
                        {
                            // this.RedirecionarAviso("Atenção", String.Format(base.RetornarMensagemErro(fonte, codigo), tentativas), 2);
                            this.ExibirMensagemLoginInvalido(exibirQuantidadeTentativas: tentativas == 1);
                        }
                    }
                    else
                        this.ExibirMensagemErro(fonte, 1103, "Atenção: A quantidade de tentativas foi esgotada", 1);
                }
                else
                {
                    // Usuário incorreto
                    ExibirMensagemLoginInvalido();
                }
            }
        }

        /// <summary>
        /// Exibir mensagem de usuário não cadastrado, mostrar link para a tela de Solicitação de Cadastro
        /// </summary>
        private void ExibirMensagemLoginInvalido(Boolean exibirQuantidadeTentativas = false)
        {
            String urlHost = new Uri(SPContext.Current.Site.Url).GetLeftPart(UriPartial.Authority);
            String urlCriacao = String.Format("{0}/pt-br/novoacesso/Paginas/CriacaoUsrDadosIniciais.aspx", urlHost);
            String urlRecuperacao = String.Format("{0}/pt-br/novoacesso/Paginas/RecuperacaoSenhaIdentificacao.aspx", urlHost);

            String mensagem = @"
E-mail ou senha inválidos. Por favor, verifique se os dados digitados estão corretos e tente novamente.
<br /><br />
{2}
Esqueceu sua senha? <a href='{0}'>clique aqui</a>.
<br /><br />
Se você não possui usuário/e-mail cadastrado, <a href='{1}'>clique aqui</a> e cadastre-se.";

            String textoTentativas = exibirQuantidadeTentativas ? "Você tem 1 tentativa. (1033)<br /><br />" : "";

            mensagem = String.Format(mensagem, urlRecuperacao, urlCriacao, textoTentativas);

            String titulo = "Atenção";
            Int16 iconeAviso = 2; //ícone de aviso
            this.RedirecionarAviso(titulo, mensagem, iconeAviso);
        }

        /// <summary>
        /// Método que criptografa a senha em Hash
        /// </summary>
        /// <param name="senha"></param>
        /// <param name="indicadorSenhaCriptografada"></param>
        /// <param name="log"></param>
        private void CriptografiaSenha(ref string senha, ref string indicadorSenhaCriptografada, Logger log)
        {
            if (senha == null)
                senha = string.Empty;

            try
            {
                log.GravarMensagem("Aplicar criptografia");

                if (String.Compare("N", indicadorSenhaCriptografada, true) == 0 || String.IsNullOrEmpty(indicadorSenhaCriptografada))
                {
                    SharePointUlsLog.LogMensagem("Aplicar criptografia");
                    // Aplicar criptografia
                    senha = EncriptadorSHA1.EncryptString(senha);
                    indicadorSenhaCriptografada = "S";
                }
            }
            catch (FaultException ex)
            {
                log.GravarErro(ex);
                SharePointUlsLog.LogErro(ex.GetBaseException().Message);
                this.ExibirMensagemErro(ex.Reason.ToString(), ex.Code.Name.ToInt32(), "Atenção", 1);
            }
        }

        /// <summary>
        /// Recupera o valor do parâmetro do Request.Form[nomeParametro] (Login tradicional, pela Área Aberta).
        /// Caso não exista, tenta retornar do Context.Items[nomeParametro]
        /// (Login realizado automaticamente nos processos de Liberação de Acesso Completo, Confirmação de Acesso, Recuperação de Senha)
        /// </summary>
        /// <param name="nomeParametro">Nome do parâmetro</param>
        /// <returns>Valor do parâmetro</returns>
        private String RecuperarParametro(String nomeParametro)
        {
            String parametro = Request.Form[nomeParametro] ?? (String)Context.Items[nomeParametro];

            if (String.IsNullOrEmpty(parametro))
            {
                String qsDados = Request.QueryString["dados"];
                if (!String.IsNullOrEmpty(qsDados))
                {
                    try { parametro = new QueryStringSegura(qsDados)[nomeParametro]; }
                    catch (QueryStringInvalidaException ex) { Logger.GravarErro("QueryString Inválida", ex); }
                    catch (Exception ex) { Logger.GravarErro("Erro leitura QueryString", ex); }
                }
            }

            return parametro;
        }

        /// <summary>
        /// Consulta todos os estabelecimentos associados ao e-mail
        /// </summary>
        /// <param name="email">E-mail</param>
        private List<Entidade1> ConsultarEstabelecimentosEmail(String email)
        {
            var codigoRetorno = default(Int32);
            var pvs = new List<Int32>();
            List<Entidade1> lstEntidades = new List<Entidade1>();

            using (Logger log = Logger.IniciarLog("Consulta estabelecimento por e-mail"))
            {
                try
                {
                    var entidades = new Entidade1[0];

                    using (var ctx = new ContextoWCF<EntidadeServicoClient>())
                        entidades = ctx.Cliente.ConsultarPorEmail(out codigoRetorno, email);

                    if (codigoRetorno != 0)
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarPorEmail", codigoRetorno);

                    if (entidades != null && entidades.Length > 0)
                        lstEntidades = entidades.ToList();
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

            return lstEntidades;
        }

        ///// <summary>
        ///// Consulta todos os estabelecimentos associados ao e-mail e senha criptografada Hash
        ///// </summary>
        ///// <param name="email">E-mail</param>
        //private List<Entidade1> ConsultarEstabelecimentosEmailSenhaHash(String email, String senha)
        //{
        //    var codigoRetorno = default(Int32);
        //    var pvs = new List<Int32>();
        //    List<Entidade1> lstEntidades = new List<Entidade1>();

        //    using (Logger log = Logger.IniciarLog("Consulta estabelecimento por e-mail"))
        //    {
        //        try
        //        {
        //            var entidades = new Entidade1[0];

        //            using (var ctx = new ContextoWCF<EntidadeServicoClient>())
        //                entidades = ctx.Cliente.ConsultarPorEmailSenhaHash(out codigoRetorno, email, senha);

        //            if (codigoRetorno != 0)
        //                base.ExibirPainelExcecao("EntidadeServico.ConsultarPorEmail", codigoRetorno);

        //            if (entidades != null && entidades.Length > 0)
        //                lstEntidades = entidades.ToList();
        //        }
        //        catch (FaultException<UsuarioServico.GeneralFault> ex)
        //        {
        //            log.GravarErro(ex);
        //            base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
        //        }
        //        catch (Exception ex)
        //        {
        //            log.GravarErro(ex);
        //            SharePointUlsLog.LogErro(ex);
        //            base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
        //        }
        //    }

        //    return lstEntidades;
        //}

        ///// <summary>
        ///// Consulta todos os estabelecimentos associados ao e-mail e senha criptografada Hash
        ///// </summary>
        ///// <param name="email">E-mail</param>
        private List<Entidade1> ConsultarWcfEstabelecimentosEmailSenhaHash(String email, String senha)
        {
            var codigoRetorno = default(Int32);
            var pvs = new List<Int32>();
            List<Entidade1> lstEntidades = new List<Entidade1>();

            using (Logger log = Logger.IniciarLog("Consulta estabelecimento por e-mail"))
            {
                try
                {
                    var entidades = new Entidade1[0];

                    using (var ctx = new ContextoWCF<EntidadeServicoClient>())
                        entidades = ctx.Cliente.ConsultarPorEmailSenhaHash(out codigoRetorno, email, senha);

                    if (codigoRetorno != 0)
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarPorEmail", codigoRetorno);

                    if (entidades != null && entidades.Length > 0)
                        lstEntidades = entidades.ToList();
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

            return lstEntidades;
        }

        /// <summary>
        /// Obtem a lista de entidades da Api
        /// </summary>
        /// <param name="email"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        private List<PortalApi.Modelo.EntidadeRetorno> ConsultarApiEstabelecimentosEmailSenhaHash(String email, String senha)
        {
            var codigoRetorno = default(Int32);
            var pvs = new List<Int32>();
            Boolean statusBuscaLista = false;

            List<PortalApi.Modelo.EntidadeRetorno> lstEntidades = new List<PortalApi.Modelo.EntidadeRetorno>();

            PortalApi.ListaEstabelecimentosApi apiEstabelecimentos = new PortalApi.ListaEstabelecimentosApi();

            using (Logger log = Logger.IniciarLog("Consulta estabelecimento por e-mail"))
            {
                try
                {
                    statusBuscaLista = apiEstabelecimentos.ConsultarPorEmailSenhaHash(email, senha, out codigoRetorno);

                    if (statusBuscaLista)
                    {

                        if (codigoRetorno != 0)
                            base.ExibirPainelExcecao("EntidadeServico.ConsultarPorEmail", codigoRetorno);


                        if (apiEstabelecimentos.ListaEntidadeRetorno != null &&
                            apiEstabelecimentos.ListaEntidadeRetorno.Count > 0)
                            lstEntidades = apiEstabelecimentos.ListaEntidadeRetorno;
                    }
                    else
                    {
                        //Preenche o status com o status de erro
                        apiEstabelecimentos.StatusRetornoLoginEstabelecimento = new PortalApi.Modelo.StatusRetorno() { Codigo = 330, Mensagem = "Houve um erro ao carregar esta página. Por favor, tente novamente mais tarde." };
                    }


                    if (!String.IsNullOrEmpty(apiEstabelecimentos.StatusRetornoLoginEstabelecimento.CodigoApi))
                    {
                        SharePointUlsLog.LogMensagem("Exibe retorno da API.");
                        log.GravarMensagem("Exibe retorno da API.");
                        base.ExibirPainelExcecao(apiEstabelecimentos.StatusRetornoLoginEstabelecimento.Mensagem, 330);
                    }
                    else
                    {
                        if (apiEstabelecimentos.StatusRetornoLoginEstabelecimento.Codigo.HasValue && apiEstabelecimentos.StatusRetornoLoginEstabelecimento.Codigo > 0)
                        {
                            SharePointUlsLog.LogMensagem("Exibe retorno da API.");
                            log.GravarMensagem("Exibe retorno da API.");
                            base.ExibirPainelExcecao(apiEstabelecimentos.StatusRetornoLoginEstabelecimento.Mensagem, apiEstabelecimentos.StatusRetornoLoginEstabelecimento.Codigo.HasValue ? apiEstabelecimentos.StatusRetornoLoginEstabelecimento.Codigo.Value : -1);
                        }
                    }
                }
                catch (NullReferenceException ex)
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

            return lstEntidades;
        }

        /// <summary>
        /// Autentica usuário digitado na tela, passando os parâmetros de login para a tela Login.aspx
        /// </summary>
        /// <param name="codigoGrupoEntidade"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="codigoNomeUsuario"></param>
        /// <param name="senha"></param>
        private void AutenticaLogin(String codigoGrupoEntidade, String codigoEntidade, String codigoNomeUsuario, String senha, String indicadorSenhaCriptografada, String urlRedirect = "")
        {

            FederatedAuthentication.SessionAuthenticationModule.SignOut();
            // Remover sessão do usuário
            Session.RemoveAll();
            this.HashUsuario = senha;
            this.HashCriptografada = indicadorSenhaCriptografada;

            //chamada de post para Login.aspx
            var url = "/_layouts/DadosCadastrais/Login.aspx";

            Response.Clear();
            var sb = new System.Text.StringBuilder();
            sb.Append("<html>");
            sb.AppendFormat("<body onload='document.forms[0].submit()'>");
            sb.AppendFormat("<form action='{0}' method='post'>", url);
            sb.AppendFormat("<input name='estabelecimento' id='_estabelecimento' type='hidden' autocomplete='off' value='{0}'>", codigoGrupoEntidade);
            sb.AppendFormat("<input name='nCadastro' id='_nCadastro' type='hidden' autocomplete='off' value='{0}'>", codigoEntidade);
            sb.AppendFormat("<input name='usuario' id='_usuario' type='hidden' autocomplete='off' value='{0}'>", codigoNomeUsuario);
            sb.AppendFormat("<input name='senha' id='_senha' type='hidden' autocomplete='off' value='{0}'>", senha);
            sb.AppendFormat("<input name='indSenhaCript' id='_indSenhaCript' type='hidden' autocomplete='off' value='{0}'>", indicadorSenhaCriptografada);

            if (!String.IsNullOrEmpty(urlRedirect))
                sb.AppendFormat("<input name='urlRedirect' id='_urlRedirect' type='hidden' autocomplete='off' value='{0}'>", urlRedirect);

            sb.Append("</form>");
            sb.Append("</body>");
            sb.Append("</html>");
            Response.Write(sb.ToString());
            Response.End();

        }

        /// <summary>
        /// Renderiza na tela os dados dos estabelecimentos em um GridView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //protected void grvEstabelecimentos_RowDataBound(object sender, GridViewRowEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Row.RowType == DataControlRowType.DataRow)
        //        {
        //            if (((Entidade1)e.Row.DataItem).Codigo != null)
        //                ((Label)e.Row.FindControl("lblCodEstabelecimento")).Text = ((Entidade1)e.Row.DataItem).Codigo.ToString();
        //            if (((Entidade1)e.Row.DataItem).Descricao != null)
        //                ((Label)e.Row.FindControl("lblNomeEstabelecimento")).Text = ((Entidade1)e.Row.DataItem).Descricao.ToString();
        //            if (((Entidade1)e.Row.DataItem).TipoEstabelecimento != null)
        //                ((Label)e.Row.FindControl("lblTipo")).Text = ((Entidade1)e.Row.DataItem).TipoEstabelecimento.ToString();
        //            if (((Entidade1)e.Row.DataItem).Status != null)
        //                ((Label)e.Row.FindControl("lblStatus")).Text = ((Entidade1)e.Row.DataItem).Status.ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// Carrega a tabela de cancelamentos
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptEstabelecimentos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (PortalApi.Modelo.EntidadeRetorno)e.Item.DataItem;

                var lblCodEstabelecimento = (Label)e.Item.FindControl("lblCodEstabelecimento");
                var lblNomeEstabelecimento = (Label)e.Item.FindControl("lblNomeEstabelecimento");
                var btnSelecionar = (LinkButton)e.Item.FindControl("btnSelecionar");

                lblCodEstabelecimento.Text = item.Codigo.ToString();
                lblNomeEstabelecimento.Text = item.Nome != null ? item.Nome.ToString() : string.Empty;
                btnSelecionar.Attributes.Add("CodigoEstabelecimento", item.Codigo.ToString());
            }
        }

        /// <summary>
        /// Converter ID de estabelecimento para String amigável
        /// </summary>
        /// <param name="tipoEstabelecimento"></param>
        /// <returns></returns>
        private string ConverterEstabelecimento(int tipoEstabelecimento)
        {
            string strEstabelecimento = string.Empty;

            if (tipoEstabelecimento == TipoNaoDefinido)
                strEstabelecimento = "-";

            if (tipoEstabelecimento == 1)
                strEstabelecimento = "Filial";

            if (tipoEstabelecimento == 2)
                strEstabelecimento = "Matriz";

            return strEstabelecimento;
        }

        /// <summary>
        /// Converter Char de estabelecimento para String amigável
        /// </summary>
        /// <param name="tipoEstabelecimento"></param>
        /// <returns></returns>
        private string ConverterStatus(string status)
        {
            string strEstabelecimento = status;

            if (!status.ToLower().Equals("-"))
            {
                if (status.ToLower().Equals("e") || status.ToLower().Equals("x"))
                    strEstabelecimento = "Cancelado";
                else
                    strEstabelecimento = "Ativo";
            }

            return strEstabelecimento;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AcessarEstabelecimento(object sender, EventArgs e)
        {
            var codigoEstabelecimento = hdfEstabelecimento.Value;
            AcessarEstabelecimentoInternal(codigoEstabelecimento);
        }

        /// <summary>
        /// Ação do botão selecionar Estabelecimento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AcessarEstabelecimentoInternal(string codigoEstabelecimento)
        {
            String codigoGrupoEntidade = LoginSelecionaEstabelecimentoTipoEntidade;
            String codigoNomeUsuario = LoginSelecionaEstabelecimentoUsuario;
            String senha = LoginSelecionaEstabelecimentoSenha;
            String indicadorSenhaCriptografada = LoginIndicadorSenhaCriptografada;
            String urlRedirect = LoginRedirect;

            SharePointUlsLog.LogMensagem(String.Format("LOGIN CÓDIGO ESTABELECIMENTO: {0}", codigoEstabelecimento));
            SharePointUlsLog.LogMensagem(String.Format("LOGIN GRUPO ENTIDADE: {0}", codigoGrupoEntidade));
            SharePointUlsLog.LogMensagem(String.Format("LOGIN NOME USUÁRIO: {0}", codigoNomeUsuario));
            SharePointUlsLog.LogMensagem(String.Format("LOGIN SENHA: {0}", senha));
            SharePointUlsLog.LogMensagem(String.Format("LOGIN INDICADOR SENHA CRIPTOGRAFADA: {0}", indicadorSenhaCriptografada));

            //Setar como nulo todos os dados do ViewState
            LoginSelecionaEstabelecimentoTipoEntidade = null;
            LoginSelecionaEstabelecimentoUsuario = null;
            LoginSelecionaEstabelecimentoSenha = null;
            LoginIndicadorSenhaCriptografada = null;
            LoginRedirect = null;

            //Autenticar
            AutenticaLogin(codigoGrupoEntidade, codigoEstabelecimento, codigoNomeUsuario, senha, indicadorSenhaCriptografada, urlRedirect);

            //Antigo com check

            //Boolean existeSelecao;
            //string codigoEntidade = GetSelectedRow(out existeSelecao);

            //String codigoGrupoEntidade = LoginSelecionaEstabelecimentoTipoEntidade;
            //String codigoNomeUsuario = LoginSelecionaEstabelecimentoUsuario;
            //String senha = LoginSelecionaEstabelecimentoSenha;
            //String indicadorSenhaCriptografada = LoginIndicadorSenhaCriptografada;

            ////Setar como nulo todos os dados do ViewState
            //LoginSelecionaEstabelecimentoTipoEntidade = null;
            //LoginSelecionaEstabelecimentoUsuario = null;
            //LoginSelecionaEstabelecimentoSenha = null;
            //LoginIndicadorSenhaCriptografada = null;

            ////Autenticar
            //AutenticaLogin(codigoGrupoEntidade, codigoEntidade, codigoNomeUsuario, senha, indicadorSenhaCriptografada);
        }

        /// <summary>
        /// Ação do botão Continuar acesso do último estabelecimento logado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuarAcesso_Click(object sender, EventArgs e)
        {
            //Novo com botões
            // Busca o Repeater Item do botão
            string codigoEntidade = string.Empty;

            //RepeaterItem item = ((RepeaterItem)((Control)sender).NamingContainer);

            codigoEntidade = hdfCodEstabelecimentoContinuar.Value.Trim();

            String codigoGrupoEntidade = LoginSelecionaEstabelecimentoTipoEntidade;
            String codigoNomeUsuario = LoginSelecionaEstabelecimentoUsuario;
            String senha = LoginSelecionaEstabelecimentoSenha;
            String indicadorSenhaCriptografada = LoginIndicadorSenhaCriptografada;
            String urlRedirect = LoginRedirect;

            //Setar como nulo todos os dados do ViewState
            LoginSelecionaEstabelecimentoTipoEntidade = null;
            LoginSelecionaEstabelecimentoUsuario = null;
            LoginSelecionaEstabelecimentoSenha = null;
            LoginIndicadorSenhaCriptografada = null;

            //Autenticar
            AutenticaLogin(codigoGrupoEntidade, codigoEntidade, codigoNomeUsuario, senha, indicadorSenhaCriptografada, urlRedirect);

        }

        /// <summary>
        /// Recuperar dado do PV Estabelecimento selecionado
        /// </summary>
        /// <param name="existeSelecao"></param>
        /// <returns></returns>
        //private string GetSelectedRow(out Boolean existeSelecao)
        //{
        //    existeSelecao = false;
        //    string strEstabelecimento = string.Empty;

        //    GridViewRowCollection rows = grvEstabelecimentos.Rows;

        //    for (int i = 0; i < rows.Count; i++)
        //    {
        //        RadioButton rb = (RadioButton)rows[i].FindControl("rbSelect");
        //        if (rb.Checked)
        //        {
        //            existeSelecao = true;

        //            strEstabelecimento = ((Label)rows[i].FindControl("lblCodEstabelecimento")).Text;
        //        }
        //    }

        //    return strEstabelecimento;
        //}




        /// <summary>
        /// Exibir mensagem de erro
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <param name="titulo">Título da mensagem</param>
        /// <param name="aviso">Tipo de aviso do quadro Erro / Aviso / Confirmação</param>
        private void ExibirMensagemErro(String fonte, Int32 codigo, String titulo, Int16 aviso)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            this.RedirecionarAviso(titulo, mensagem, aviso);
        }

        /// <summary>
        /// Redireciona o usuário para a tela de aviso de erro
        /// </summary>
        /// <param name="titulo">Titulo do quadro</param>
        /// <param name="mensagem">Mensagem do quadro</param>
        /// <param name="aviso">Tipo de aviso do quadro Erro / Aviso / Confirmação </param>
        private void RedirecionarAviso(String titulo, String mensagem, Int16 aviso)
        {
            String codigoGrupoEntidade = RecuperarParametro("estabelecimento");
            String codigoEntidade = RecuperarParametro("ncadastro");
            String codigoNomeUsuario = RecuperarParametro("usuario");

            QueryStringSegura query = new QueryStringSegura();

            query.Add("mensagem", Server.UrlEncode(mensagem));
            query.Add("codigoGrupoEntidade", codigoGrupoEntidade);
            query.Add("codigoEntidade", codigoEntidade);
            query.Add("codigoNomeUsuario", codigoNomeUsuario);
            query.Add("titulo", Server.UrlEncode(titulo));
            query.Add("imagem", aviso.ToString());

            String url = Util.BuscarUrlRedirecionamento("/_layouts/DadosCadastrais/Aviso.aspx?dados=" + query.ToString(), SPUrlZone.Default);
            Response.Redirect(url, false);
        }
    }
}