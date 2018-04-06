#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [06/05/2012] – [André Garcia] – [Criação]
- [26/09/2016] - [Roger Santos] - Criação de consulta em uma lista para verificar se o login deve ser feito via WCF ou API
*/
#endregion


using System;
using System.ServiceModel;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.IdentityModel;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using System.Configuration;
using Redecard.PN.DadosCadastrais.SharePoint.Login;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{
    /// <summary>
    /// Página de login seguro no Portal Redecard
    /// </summary>
    public class LoginPage : ApplicationPageBaseAnonima
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("LOGIN - Login no portal Redecard área fechada"))
            {
                SharePointUlsLog.LogMensagem("LOGIN - Login no portal Redecard área fechada");

                String redirectUrl = String.Empty;
                String codigoGrupoEntidade = RecuperarParametro("estabelecimento");
                String codigoEntidade = RecuperarParametro("ncadastro");
                String codigoNomeUsuario = RecuperarParametro("usuario");
                String senha = RecuperarParametro("senha");

                //Indica se a senha obtida de Request.Form["senha"] já está criptografada.
                //Login preenchido no formulário de Liberação de Acesso Completo
                String indicadorSenhaCriptografada = RecuperarParametro("indSenhaCript");

                //Indicador se deverá direcionar para a liberação de acesso
                String liberarAcesso = RecuperarParametro("liberarAcesso");

                Int32 iCodigoGrupoEntidade = 0;
                Int32 iCodigoEntidade = 0;
                Int32 codigoRetorno;
                Boolean statusLogin = false;
                PortalApi.LoginApi loginApi = new PortalApi.LoginApi();

                //Recuperar Parametro para rotina de redirecionamento ao "Suporte à maquininha"
                String urlRedirect = RecuperarParametro("urlRedirect");
                //

                try
                {
                    SharePointUlsLog.LogMensagem("LOGIN - Validações");
                    Log.GravarMensagem("LOGIN - Validações");

                    if (!Int32.TryParse(codigoGrupoEntidade, out iCodigoGrupoEntidade))
                    {
                        this.ExibirErro("Redecard.PN.DadosCadastrais.SharePoint.Login.Logar", 1102, 3);
                        return;
                    }

                    if (!Int32.TryParse(codigoEntidade, out iCodigoEntidade))
                    {
                        this.ExibirErro("Redecard.PN.DadosCadastrais.SharePoint.Login.Logar", 1027, 2);
                        return;
                    }

                    String _loginNameFormat = "{0};{1};{2};{3};";
                    String loginName = String.Format(_loginNameFormat, iCodigoGrupoEntidade, iCodigoEntidade, codigoNomeUsuario, statusLogin);

                    if (String.IsNullOrEmpty(loginName) || String.IsNullOrEmpty(senha))
                    {
                        this.ExibirErro("Redecard.PN.DadosCadastrais.SharePoint.Login.Logar", 1028, 2);
                        return;
                    }

                    //Verifica a partir de uma lista no Sharepoint se deve consumir o login pela nova API ou WCF
                    bool utilizaApiLogin = ConfiguracaoLogin.UtilizaApiLogin();

                    if (utilizaApiLogin)
                    {
                        SharePointUlsLog.LogMensagem("LOGIN - Validar na API");
                        Log.GravarMensagem("LOGIN - Validar na API");
                        statusLogin = loginApi.ValidarLogin(codigoNomeUsuario, senha, iCodigoEntidade, iCodigoGrupoEntidade);
                    }
                    else
                    {
                        if (String.Compare("N", indicadorSenhaCriptografada, true) == 0
                            || String.IsNullOrEmpty(indicadorSenhaCriptografada))
                        {
                            Log.GravarMensagem("Aplicar criptografia");
                            SharePointUlsLog.LogMensagem("Aplicar criptografia");
                            // Aplicar criptografia
                            senha = EncriptadorSHA1.EncryptString(senha);
                        }

                        int retornoWcf = 0;
                        SharePointUlsLog.LogMensagem("LOGIN - Validar no WCF");
                        Log.GravarMensagem("LOGIN - Validar no WCF");
                        statusLogin = ValidaLoginWcf(iCodigoGrupoEntidade, iCodigoEntidade, codigoNomeUsuario, loginName, senha, Log, out retornoWcf);

                        loginApi.StatusRetornoLogin = new PortalApi.Modelo.StatusRetorno() { Codigo = retornoWcf };
                    }

                    loginName = String.Format(_loginNameFormat, iCodigoGrupoEntidade, iCodigoEntidade, codigoNomeUsuario, statusLogin);

                    SharePointUlsLog.LogMensagem("LOGIN - Finalização das validações");
                    Log.GravarMensagem("LOGIN - Finalização das validações");

                    try
                    {
                        if (statusLogin)
                        {
                        SharePointUlsLog.LogMensagem("SPClaimsUtility.AuthenticateFormsUser");
                        Log.GravarMensagem("SPClaimsUtility.AuthenticateFormsUser");

                            //AAL - 18/03/2016: Alterado para não criptografar mais a senha. A API irá criptografar
                            //if (String.Compare("N", indicadorSenhaCriptografada, true) == 0
                            //    || String.IsNullOrEmpty(indicadorSenhaCriptografada))
                            //{
                            //    Log.GravarMensagem("Aplicar criptografia");
                            //    SharePointUlsLog.LogMensagem("Aplicar criptografia");
                            //    // Aplicar criptografia
                            //    senha = EncriptadorSHA1.EncryptString(senha);
                            //}

                        String url = String.Empty;
                        url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Internet); //portal.userede.com.br

                        //Se a flag de liberar acesso for nula, origem é do Login; caso contrário, da Confirmação de Cadastro OU Cadastro Nova Senha
                        //if (String.IsNullOrEmpty(liberarAcesso)) 
                        //    url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Internet); //portal.userede.com.br
                        //else
                        //    url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default); //www.userede.com.br

                        Uri uri = new Uri(url);

                        if (SPClaimsUtility.AuthenticateFormsUser(HttpContext.Current.Request.Url, loginName, senha))
                        {
                            SharePointUlsLog.LogMensagem("Criar sessão do usuário");
                            Log.GravarMensagem("Criar sessão do usuário");

                            try
                            {
                                // Usuário Autenticado, criar objeto de sessão no AppFabric e redirecionar para o Portal de Serviços
                                LoginClass login = new LoginClass();
                                //Verifica se o login foi feito via API ou WCF para criar a sessão do usuario
                                if (utilizaApiLogin)
                                {
                                    codigoRetorno = login.CriarSessaoUsuario(codigoNomeUsuario, iCodigoGrupoEntidade, iCodigoEntidade, senha, loginApi.InformacoesLoginEstabelecimento, loginApi.InformacoesLoginOutrasEntidades);
                                }
                                else
                                {
                                    codigoRetorno = login.CriarSessaoUsuario(codigoNomeUsuario, iCodigoGrupoEntidade, iCodigoEntidade, senha);
                                }
                            }
                            catch (Exception ex)
                            {
                                    SharePointUlsLog.LogErro(String.Format("Erro genérico na criação da Sessão. Exceptio:{0}\n{1}", ex.ToString(), ex.StackTrace));
                                    Log.GravarMensagem(String.Format("Erro genérico na criação da Sessão. Exceptio:{0}\n{1}",ex.ToString(), ex.StackTrace));
                                this.ExibirErro("LoginClass.CriarSessaoUsuario", 1047, 2);
                                return;
                            }

                            if (codigoRetorno > 0)
                            {
                                SharePointUlsLog.LogMensagem("Código de retorno do login maior que 0");
                                Log.GravarMensagem("Código de retorno do login maior que 0");
                                this.ExibirErro("EntidadeServico.Consultar", codigoRetorno, 2);
                                return;
                            }

                            SharePointUlsLog.LogMensagem("Login OK, redirecionar o usuário para o Portal Fechado");

                            if (!String.IsNullOrEmpty(liberarAcesso))
                                if (liberarAcesso.Equals("N"))
                                    RedirecionarPortalServicos(urlRedirect);
                                else
                                    RedirecionarLiberacaoAcesso();
                            else
                            {
                                //Change request: Verifica se precisa exibir a mensagem de liberação
                                //de acesso completo quando o usuário fizer login
                                //if (Sessao.Contem() && Sessao.Obtem().ExibirMensagemLiberacaoAcessoCompleto)
                                //    RedirecionarLiberacaoAcesso();
                                //else
                                RedirecionarPortalServicos(urlRedirect);
                            }

                                //Caso seja sucesso, retonar o método
                                return;
                        }
                        }

                        if (loginApi.StatusRetornoLogin == null)
                        {
                            //Caso de erro ou Login inválido
                            SharePointUlsLog.LogMensagem("Recupera mensagem de retorno do banco de dados.");
                            Log.GravarMensagem("Recupera mensagem de retorno do banco de dados.");
                            SharePointUlsLog.LogMensagem("Acessa serviço de usuario ConsultarErroUsuarioLogin.");
                            Log.GravarMensagem("Acessa serviço de usuario ConsultarErroUsuarioLogin.");

                            using (UsuarioServico.UsuarioServicoClient usuario = new UsuarioServico.UsuarioServicoClient())
                            {
                                UsuarioServico.TrataErroUsuarioLogin trataErro = usuario.ConsultarErroUsuarioLogin(loginName);
                                SharePointUlsLog.LogMensagem("Verifica retorno do ConsultarErroUsuarioLogin.");
                                Log.GravarMensagem("Verifica retorno do ConsultarErroUsuarioLogin.");
                                if (!object.ReferenceEquals(trataErro, null))
                                {
                                    loginApi.StatusRetornoLogin = new PortalApi.Modelo.StatusRetorno() { Codigo = trataErro.Codigo };
                               
                                    SharePointUlsLog.LogMensagem("Exclui retorno do banco de dados.");
                                    Log.GravarMensagem("Exclui retorno do banco de dados.");
                                    usuario.ExcluirErroUsuarioLogin(loginName, trataErro.Codigo);
                                }
                                else
                                {
                                    SharePointUlsLog.LogMensagem("Exibe erro genérico SharePoint.");
                                    Log.GravarMensagem("Exibe erro genérico SharePoint.");
                                    this.ExibirErro(FONTE, CODIGO_ERRO, 2);
                                }
                            }
                        }

                        //Codigo == 415 - Senha Expirada, solicita Confirmação positiva
                        if (loginApi.StatusRetornoLogin.Codigo.HasValue && loginApi.StatusRetornoLogin.Codigo.Value == 415 && iCodigoGrupoEntidade == 1) // Só existe confirmação positiva para estabelecimentos
                                    {
                                        // Exibir tela de confirmação positiva do Novo Acesso para criação de nova senha
                                        // Gravar dados do PV na sessão de confirmação positiva
                            InformacaoUsuario.Criar(iCodigoGrupoEntidade, iCodigoEntidade, codigoNomeUsuario, loginApi.StatusRetornoLogin.Codigo.HasValue ? loginApi.StatusRetornoLogin.Codigo.Value : -1);

                                        if (InformacaoUsuario.Existe())
                                            InformacaoUsuario.Recuperar().SenhaExpirada = true;

                                        String urlConfirmacaoPositiva = "/pt-br/NovoAcesso/Paginas/RecSenhaExpiradaConfirmacaoPositiva.aspx";
                                        Page.Response.Redirect(urlConfirmacaoPositiva, false);
                                    }
                                    //NOVO ACESSO: trataErro.Codigo == 390 - Usuário Não Cadastrado, redireciona para tela de aviso para Criar novamente o usuário
                                    // Tratamento específico para Estabelecimentos duranta o período de Migração do Novo iAcesso
                        else if (loginApi.StatusRetornoLogin.Codigo.HasValue && loginApi.StatusRetornoLogin.Codigo.Value == 390 && iCodigoGrupoEntidade == 1)
                                    {
                            this.ExibirErro(String.Empty, loginApi.StatusRetornoLogin.Codigo.HasValue ? loginApi.StatusRetornoLogin.Codigo.Value : -1, 2);
                                    }
                                    //NOVO ACESSO: trataErro.Codigo == 497 - Senha Temporária, redireciona para tela de aviso para informar e Criar um usuário novo Acesso
                                    // Tratamento específico para Estabelecimentos duranta o período de Migração do Novo iAcesso
                        else if (loginApi.StatusRetornoLogin.Codigo.HasValue && loginApi.StatusRetornoLogin.Codigo.Value == 497 && iCodigoGrupoEntidade == 1)
                                    {
                            this.ExibirErro(String.Empty, loginApi.StatusRetornoLogin.Codigo.HasValue ? loginApi.StatusRetornoLogin.Codigo.Value : -1, 2);
                                    }
                                    else
                                    {
                                        SharePointUlsLog.LogMensagem("Exibe retorno do banco de dados.");
                                        Log.GravarMensagem("Exibe retorno do banco de dados.");
                            this.ExibirErro("Redecard.PN.DadosCadastrais.SharePoint.Login.Logar", loginApi.StatusRetornoLogin.Codigo.HasValue ? loginApi.StatusRetornoLogin.Codigo.Value : -1, 2);
                        }
                    }
                    catch (FaultException ex)
                    {
                        Log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex.GetBaseException().Message);

                        // Exception diferenciada pois o provider ja esta sendo chamado de um serviço.
                        //String[] erro = ex.Message.Split('|');
                        this.ExibirMensagemErro(ex.Reason.ToString(), ex.Code.Name.ToInt32(), "Atenção!", 1);
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirMensagemErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32(), "Atenção!", 1);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    this.ExibirMensagemErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32(), "Atenção!", 1);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirMensagemErro(ex.Fonte, ex.Codigo, "Atenção!", 1);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    this.ExibirMensagemErro(FONTE, CODIGO_ERRO, "Atenção!", 1);
                }
            }
        }

        /// <summary>
        /// Valida o login do usuário no serviço WCF
        /// </summary>
        /// <param name="codigoGrupoEntidade"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="codigoNomeUsuario"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="Log"></param>
        /// <param name="codigoRetorno"></param>
        private Boolean ValidaLoginWcf(int codigoGrupoEntidade, int codigoEntidade, string codigoNomeUsuario, string username, string password, Logger Log, out int codigoRetorno)
        {
            try
            {
                SharePointUlsLog.LogMensagem("Login WCF - Chamada ao serviço de usuário");
                Log.GravarMensagem("Login WCF - Chamada ao serviço de usuário");

                Boolean pvKomerci = false;
                codigoRetorno = 0;

                // Verifica tecnologia Komerci ou DataCash somente para entidade de estabelecimentos 
                if (codigoGrupoEntidade.Equals(1))
                {
                    Int32 tecnologia = 0;

                    using (EntidadeServicoClient entidadeServico = new EntidadeServicoClient())
                    {
                        SharePointUlsLog.LogMensagem("Login WCF - Chamada ao método consulta tecnologia estabelecimento");

                        // Verificar se tem DataCash. (Nunca um PV poderá ter Komerci e DataCash)
                        Int32 codigoRetornoEntidade = 0;
                        EntidadeServico.Entidade ent = entidadeServico.ConsultarDadosPV(out codigoRetornoEntidade, codigoEntidade);

                        if (!Object.ReferenceEquals(null, ent) && codigoRetornoEntidade == 0)
                        {
                            if (String.IsNullOrEmpty(ent.IndicadorDataCash))
                                ent.IndicadorDataCash = String.Empty;

                            // Caso o indicador = S e a data de ativação diferente de nula(DateTime.MinValue) o PV tem DataCash
                            if (ent.IndicadorDataCash.Equals("S") && !ent.DataAtivacaoDataCash.Equals(DateTime.MinValue))
                                pvKomerci = true;

                            if (!pvKomerci)
                            {
                                // Verificação caso o PV tenha Komerci
                                tecnologia = entidadeServico.ConsultarTecnologiaEstabelecimento(out codigoRetorno, codigoEntidade);
                                pvKomerci = (tecnologia == 25 || tecnologia == 26 || tecnologia == 23);

                                String logTipoTecnologia = String.Format("Log - Tecnologia: {0}; PvKomerci: {1}", tecnologia, pvKomerci.ToString());
                                SharePointUlsLog.LogMensagem(logTipoTecnologia);
                                Log.GravarMensagem(logTipoTecnologia);
                            }
                        }
                        else
                        {
                            SharePointUlsLog.LogMensagem("Login WCF - Entidade não encontrada no GE");
                            Log.GravarMensagem("Login WCF - Entidade não encontrada no GE");
                            this.CriarMensagemRetorno(username, 1102);
                            return false;
                        }
                    }
                }

                using (UsuarioServicoClient usuarioServico = new UsuarioServicoClient())
                {
                    SharePointUlsLog.LogMensagem("Login WCF - Chamada ao método validar");
                    Log.GravarMensagem("Login WCF - Chamada ao método validar");

                    codigoRetorno = usuarioServico.Validar(codigoGrupoEntidade, codigoEntidade, codigoNomeUsuario, password, pvKomerci);
                    // Se o código de retorno for 0 ou 397 o login foi realizado com sucesso
                    if (codigoRetorno == 0 || codigoRetorno == 397)
                    {
                        SharePointUlsLog.LogMensagem("Login WCF - Retorno válido 0 ou 397 retorna TRUE");
                        Log.GravarMensagem("Login WCF - Retorno válido 0 ou 397 retorna TRUE");

                        return true;
                    }
                    else
                    {
                        SharePointUlsLog.LogMensagem("Login WCF - Retorno inválido FALSE");
                        Log.GravarMensagem("Login WCF - Retorno inválido FALSE");
                        this.CriarMensagemRetorno(username, codigoRetorno);
                        return false;
                    }
                }
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                Log.GravarErro(ex);
                SharePointUlsLog.LogErro(ex.GetBaseException());

                this.CriarMensagemRetorno(username, ex.Detail.Codigo);
                codigoRetorno = ex.Detail.Codigo;
                return false;
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                Log.GravarErro(ex);
                SharePointUlsLog.LogErro(ex.GetBaseException());

                this.CriarMensagemRetorno(username, ex.Detail.Codigo);
                codigoRetorno = ex.Detail.Codigo;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chave"></param>
        /// <param name="codigoRetorno"></param>
        private void CriarMensagemRetorno(String chave, Int32 codigoRetorno)
        {
            using (Logger Log = Logger.IniciarLog("Login WCF - Chama serviço para incluir retorno Login"))
            {
                SharePointUlsLog.LogMensagem("Login WCF - Chama serviço para incluir retorno Login");
                using (UsuarioServico.UsuarioServicoClient usuario = new UsuarioServico.UsuarioServicoClient())
                {
                    usuario.InserirErroUsuarioLogin(chave, codigoRetorno);
                }
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

        /// <summary>
        /// Redireciona o usuário para a área fechada do portal Rede
        /// </summary>
        private void RedirecionarPortalServicos(String urlRedirect = "")
        {
            if (Sessao.Contem())
                Sessao.Obtem().LiberarAcessoCompleto = false;

            String url = Util.BuscarUrlRedirecionamento((String.IsNullOrEmpty(urlRedirect) ? "/sites/fechado/" : urlRedirect), SPUrlZone.Internet);
            Response.Redirect(url, false);
        }

        /// <summary>
        /// Redireciona o usuário para a Liberação de Acesso Completo na área fechada do portal Rede
        /// </summary>
        private void RedirecionarLiberacaoAcesso()
        {
            if (Sessao.Contem())
                Sessao.Obtem().LiberarAcessoCompleto = true;

            String url = Util.BuscarUrlRedirecionamento("/sites/fechado/", SPUrlZone.Internet);
            Response.Redirect(url, false);

            //QueryStringSegura qs = new QueryStringSegura();
            //qs["ExibirInformacaoPermissao"] = "true";

            //String url = Util.BuscarUrlRedirecionamento("/sites/fechado/Paginas/LiberacaoAcessoCompleto.aspx?dados={0}", SPUrlZone.Internet);
            //url = String.Format(url, qs.ToString());

            //Response.Redirect(url, false);
        }

        /// <summary>
        /// Exibir painel de erro
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <param name="aviso">Tipo de aviso do quadro Erro / Aviso / Confirmação</param>
        protected void ExibirErro(String fonte, Int32 codigo, Int16 aviso)
        {
            using (Logger Log = Logger.IniciarLog("Exibindo erro"))
            {

                switch (codigo)
                {
                    // Usuário Não Cadastrado
                    case 390:
                        ExibirMensagemUsuarioNaoCadastrado();
                        break;

                    // Usuário Legado com senha Temporária
                    case 497:
                        ExibirMensagemUsuarioLegadoSenhaTemporaria();
                        break;

                    // Senha Incorreta
                    case 391:
                        ExibirMensagemSenhaIncorreta(fonte, codigo);
                        break;

                    // Estabelecimento inválido
                    case 1102:
                        ExibirMensagemErro(fonte, codigo, "Atenção!", aviso);
                        break;

                    // Usuário bloqueado
                    case 1103:
                        ExibirMensagemErro(fonte, codigo, "Atenção: A quantidade de tentativas foi esgotada", 1);
                        break;

                    default:
                        ExibirMensagemErro(fonte, codigo, "Atenção!", aviso);
                        break;
                }

            }
        }

        /// <summary>
        /// Verifica a quantidade de tentativas de login que o usuário ainda tem e exibe mensagem
        /// </summary>
        private void ExibirMensagemSenhaIncorreta(String fonte, Int32 codigo)
        {
            using (UsuarioServicoClient client = new UsuarioServicoClient())
            {
                Int32 retorno = 0;

                Int32 numeroPv = Int32.Parse(RecuperarParametro("ncadastro"));

                var usuarios = client.ConsultarPorCodigoEntidade(out retorno, RecuperarParametro("usuario"),
                    new UsuarioServico.Entidade()
                    {
                        Codigo = numeroPv,
                        GrupoEntidade = new UsuarioServico.GrupoEntidade() { Codigo = 1 }
                    }
                );

                if (retorno.Equals(0) && usuarios.Length > 0)
                {
                    UsuarioServico.Usuario usuario = usuarios[0];
                    
                    String mensagem = String.Empty;
                    Int32 tentativas = (6 - usuarios[0].QuantidadeTentativaLoginIncorreta);

                    if (tentativas <= 0)
                    {
                        //Armazena no histórico/log de atividades
                        Historico.BloqueioUsuarioErroSenha(
                            usuario.CodigoIdUsuario, usuario.Descricao, usuario.Email, usuario.TipoUsuario, numeroPv);

                        //Envia e-mail de bloqueio de usuário para o usuário
                        EmailNovoAcesso.EnviarEmailAcessoBloqueado(usuario.Descricao, usuario.Email,
                            usuario.CodigoIdUsuario, usuario.TipoUsuario, numeroPv, null);

                        this.RedirecionarAviso("Atenção: A quantidade de tentativas foi esgotada", base.RetornarMensagemErro(fonte, 1103), 1);
                    }
                    else
                        this.RedirecionarAviso("Atenção!", String.Format(base.RetornarMensagemErro(fonte, codigo), tentativas), 2);
                }
                else
                    this.ExibirMensagemErro(fonte, 1103, "Atenção: A quantidade de tentativas foi esgotada", 1);
                /*this.RedirecionarAviso("Limite de tentativas excedido!", 
                    "Seu acesso ao Portal da Rede foi bloqueado.<br><a href=''>Clique Aqui</a> para desbloquear seu acesso ou solicite o desbloqueio ao usuário Master.", 
                    1);*/
            }
        }

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
        /// Exibir mensagem de usuário não cadastrado, mostrar link para a tela de Solicitação de Cadastro
        /// </summary>
        private void ExibirMensagemUsuarioNaoCadastrado()
        {
            String url = new Uri(SPContext.Current.Site.Url).GetLeftPart(UriPartial.Authority);
            url = String.Format("{0}/pt-br/novoacesso/Paginas/CriacaoUsrDadosIniciais.aspx", url);

            String mensagem = @"E-mail ou senha inválidos. Por favor, verifique se os 
                                dados digitados estão corretos e tente novamente.<br><br>
                                Se você não possui usuário/e-mail cadastrado, <a href='{0}'>clique aqui</a> e cadastre-se.";

            mensagem = String.Format(mensagem, url);

            String titulo = "Atenção!";
            Int16 iconeAviso = 2; //ícone de aviso
            this.RedirecionarAviso(titulo, mensagem, iconeAviso);
        }

        /// <summary>
        /// Exibir mensagem de usuário legado com senha provisória, mostrar link para a tela Aberta de Criação de Usuário
        /// </summary>
        private void ExibirMensagemUsuarioLegadoSenhaTemporaria()
        {
            using (var servicoUsuario = new ContextoWCF<UsuarioServicoClient>())
            {
                Int32 numeroPv = Int32.Parse(RecuperarParametro("ncadastro"));
                String login = RecuperarParametro("usuario");
                Int32 retorno = 0;

                var usuarios = servicoUsuario.Cliente.ConsultarPorCodigoEntidade(out retorno, login,
                    new UsuarioServico.Entidade()
                    {
                        Codigo = numeroPv,
                        GrupoEntidade = new UsuarioServico.GrupoEntidade() { Codigo = 1 }
                    }
                );

                String url = new Uri(SPContext.Current.Site.Url).GetLeftPart(UriPartial.Authority);

                String mensagem = @"Seu acesso ao Portal Rede mudou! O usuário passa a ser um endereço de e-mail.<br /><br />
                                Os dados de acesso recebidos via carta/e-mail não são mais válidos. Por favor, <a href='{0}'>clique aqui</a> e
                                realize o novo cadastramento de usuário/e-mail e senha.";

                if (retorno.Equals(0) && usuarios.Length > 0)
                {
                    QueryStringSegura qs = new QueryStringSegura();
                    qs["IdUsuario"] = usuarios[0].CodigoIdUsuario.ToString();

                    url = String.Format("{0}/pt-br/novoacesso/Paginas/CriacaoUsrDadosIniciais.aspx?dados={1}", url, qs.ToString());

                    mensagem = String.Format(mensagem, url);

                    String titulo = "Atenção!";
                    Int16 iconeAviso = 2; //ícone de aviso
                    this.RedirecionarAviso(titulo, mensagem, iconeAviso);
                }
                else
                {
                    url = String.Format("{0}/pt-br/novoacesso/Paginas/CriacaoUsrDadosIniciais.aspx", url);

                    mensagem = String.Format(mensagem, url);

                    String titulo = "Atenção!";
                    Int16 iconeAviso = 2; //ícone de aviso
                    this.RedirecionarAviso(titulo, mensagem, iconeAviso);
                }
            }
        }
    }
}
