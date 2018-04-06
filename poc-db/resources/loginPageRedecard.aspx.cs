using System;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using Microsoft.SharePoint;
using Microsoft.SharePoint.IdentityModel;
using Redecard.Portal.Helper;
using Redecard.Portal.SharePoint.Client.WCF;
using Redecard.Portal.SharePoint.Client.WCF.SRVLoginLegado;
using Microsoft.SharePoint.Utilities;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;

namespace Redecard.Portal.Fechado.SD.Layouts {

    /// <summary>
    /// Página de Login da Redecard
    /// </summary>
    public class loginPageRedecard : Page {

        /// <summary>
        /// 
        /// </summary>
        string _usuario = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        string _senha = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        string _ncadastro = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        string _estabelecimento = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        SPDiagnosticsCategory category = null;

        /// <summary>
        /// 
        /// </summary>
        string _categoryName = "Portal Redecard Login";

        /// <summary>
        /// 
        /// </summary>
        protected void GetParams() {
            if (!String.IsNullOrEmpty(Request["estabelecimento"] as string))
                _estabelecimento = Request["estabelecimento"] as string;
            if (!String.IsNullOrEmpty(Request["ncadastro"] as string))
                _ncadastro = Request["ncadastro"] as string;
            if (!String.IsNullOrEmpty(Request["usuario"] as string))
                _usuario = Request["usuario"] as string;
            if (!String.IsNullOrEmpty(Request["senha"] as string))
                if (!String.IsNullOrEmpty(EncriptadorSHA1.EncryptString(Request["senha"] as string)))
                    _senha = EncriptadorSHA1.EncryptString(Request["senha"] as string);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string FormatLogin() {
            string _loginFormat = "{0};{1};{2}";
            return String.Format(_loginFormat, _estabelecimento, _ncadastro, _usuario);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected void WriteULSLog(string message, string categoryName) {
            if (Object.ReferenceEquals(category, null)) {
                category = new SPDiagnosticsCategory(categoryName, TraceSeverity.Monitorable, EventSeverity.Error);
            }
            SPDiagnosticsService.Local.WriteTrace(0, category, TraceSeverity.Monitorable, message);
        }

        /// <summary>
        /// Carregamento da página de login
        /// </summary>
        protected void Page_Load() {
            // Criar Objeto de LOG da parte de Login
            WriteULSLog("Iniciando Rotina de Login", _categoryName);

            this.GetParams();
            string sUrlPortalAberto = System.Configuration.ConfigurationManager.AppSettings["portalRedecardAbertoUrl"];
            string Redirect = string.Empty;
            // Escrever recuperação de propriedades do PostBack
            WriteULSLog("As informações de Login foram carregadas.", _categoryName);
            try {
                if (!String.IsNullOrEmpty(_usuario) || !String.IsNullOrEmpty(_senha)) {
                    // Verificar login do usuário
                    WriteULSLog("Iniciar chamada do Cliente de Login (RealizaLoginLegado)", _categoryName);
                    RetornoLoginLegadoEstabelecimentoVO retLoginLegado = new RetornoLoginLegadoEstabelecimentoVO();
                    retLoginLegado = SharePointWCFHelper.RealizaLoginEstabelecimento(Int32.Parse(_ncadastro), _usuario, _senha);
                    WriteULSLog("Chamada do Login concluída. Escrever Parâmetros", _categoryName);
                    foreach (object obj in retLoginLegado.ValorParametros.Keys) {
                        WriteULSLog("Parâmetro: " + obj.ToString() + "; Valor: " + retLoginLegado.ValorParametros[obj].ToString(), _categoryName);
                    }

                    if (!object.ReferenceEquals(retLoginLegado, null)) {
                        if (RedecardHelper.IsLoginSucess(retLoginLegado.CodigoRetorno)) {
                            // Tenta criar a sessao no legado
                            {
                                WriteULSLog("Login realizado com sucesso. Iniciar chamada para criação do Token (ObterNovoTokenSessaoLegado).", _categoryName);
                                // gravar token de autenticação
                                string sToken = SharePointWCFHelper.ObterNovoTokenSessaoLegado();
                                if (String.IsNullOrEmpty(sToken)) {
                                    WriteULSLog("Ocorreu uma falha no retorno do Token, o mesmo retornou um valor vazio.", _categoryName);
                                    Redirect = sUrlPortalAberto + "?errorCode=-2&mot=sTokenNull";
                                    throw new Exception("O token de sessão foi retornado como uma string vazia.");
                                }
                                else {
                                    WriteULSLog("Token criado com sucesso. Iniciar chamada de criação de sessão do Legado (CriarSessaoLegadoEstabelecimento).", _categoryName);
                                    int sessionId = SharePointWCFHelper.CriarSessaoLegadoEstabelecimento(sToken, _usuario, _senha, retLoginLegado);
                                    if (sessionId > 0) {
                                        WriteULSLog("Sessão criada com sucesso. Setar cookies do SharePoint.", _categoryName);
                                        this.CreateCookies(retLoginLegado, sToken, sessionId);
                                        // Se for validação positiva, gravar um cookie no SharePoint para 
                                        // solicitar a confirmação na WebPart de Redirecionamento Legado
                                        if (retLoginLegado.CodigoRetorno == CodigosRetornoLogin.SenhaTemporariaNovaRequerValidacaoPositiva)
                                            Response.SetCookie(new HttpCookie("needValidation", "1"));
                                        WriteULSLog("Cookies SharePoint criados com sucesso.", _categoryName);
                                    }
                                    else {
                                        WriteULSLog("Ocorreu uma falha ao criar o Token do Legado. O mesmo retornou o valor '0'.", _categoryName);
                                        Redirect = sUrlPortalAberto + "?errorCode=-2&mot=errCriSess";
                                        throw new Exception("Ocorreu um falha na criação da sessão no legado.");
                                    }
                                }
                            }
                            // Realizar Autenticação no SharePoint
                            WriteULSLog("Tarefas do Legado concluídas com sucesso. Autenticar usuário no SharePoint.", _categoryName);
                            if (SPClaimsUtility.AuthenticateFormsUser(Request.Url, this.FormatLogin(), _senha)) {
                                WriteULSLog("Login SharePoint concluído com sucesso. Iniciar paramêtros transferidos do Legado.", _categoryName);
                                // existia nesse ponto uma gravação de log dos parametros, essa rotina nós passamos para
                                // depois da chamada do serviço de login.
                                WriteULSLog("Parâmetros gravados com sucesso. Iniciar parte de redirecionamentos.", _categoryName);
                                string sSource = Request.QueryString["Source"] as string;
                                // gravar token de autenticação
                                if (!String.IsNullOrEmpty(sSource)) {
                                    if (!sSource.Trim().StartsWith("http", StringComparison.InvariantCultureIgnoreCase)) {
                                        SPUtility.Redirect(sSource, SPRedirectFlags.Default, HttpContext.Current);
                                    }
                                    else {
                                        Redirect = sUrlPortalAberto + "?errorCode=-99&mot=InvSrc";
                                    }
                                }
                                else {
                                    Redirect = SPContext.Current.Web.Url;
                                }
                            }
                            else {
                                Redirect = sUrlPortalAberto + "?errorCode=-2";
                            }
                        }
                        else {
                            if (retLoginLegado.ValorParametros.ContainsKey("QTD_SEN_INV")) {
                                int iSenhaInvalida = Int32.Parse(retLoginLegado.ValorParametros["QTD_SEN_INV"].ToString());
                                int iSenhaVezes = (iSenhaInvalida < 6 ? 6 - iSenhaInvalida : 0);
                                Redirect = sUrlPortalAberto + "?errorCode=" + (int)retLoginLegado.CodigoRetorno + "&txtUsuario=" + _usuario + "&nCadastro=" + _ncadastro + "&qtdVezes=" + iSenhaVezes;
                            }
                            else
                                Redirect = sUrlPortalAberto + "?errorCode=" + (int)retLoginLegado.CodigoRetorno + "&txtUsuario=" + _usuario + "&nCadastro=" + _ncadastro;

                        }
                    }
                    else {
                        Redirect = sUrlPortalAberto + "?errorCode=-99";
                    }
                }
                else {
                    //if (this.Request.UrlReferrer.AbsoluteUri.Contains("/sites/fechado"))
                    Redirect = sUrlPortalAberto;
                    //else
                    //    Redirect = sUrlPortalAberto + "?errorCode=-1";
                }
            }
            catch (Exception e) {
                WriteULSLog("Ocorre um erro no Login do Portal. Observe os detalhes: " + e.Message + "; Stack Trace: " + e.StackTrace, _categoryName);
                Redirect = sUrlPortalAberto + "?errorCode=-99&mot=" + e.GetType().ToString();
            }
            WriteULSLog("Definição do redirecionamento concluída. Redirecionar para URL: " + Redirect, _categoryName);
            // Redirecionar usuário
            if (!String.IsNullOrEmpty(Redirect)) {
                WriteULSLog("Redirecionar usuário. Final da Chamada de Login.", _categoryName);
                Response.Redirect(Redirect, false);
            }
        }

        /// <summary>
        /// Criar os cookies de sessão do SharePoint
        /// </summary>
        /// <param name="retLoginLegado"></param>
        /// <param name="sToken"></param>
        /// <param name="sessionId"></param>
        private void CreateCookies(RetornoLoginLegadoEstabelecimentoVO retLoginLegado, string sToken, int sessionId) {
            HttpCookieCollection cookies = this.Page.Request.Cookies;
            List<string> dicKeys = new List<string>();
            dicKeys.AddRange(cookies.AllKeys);

            this.ClearAnotherCookies(dicKeys);
            this.CreateOrUpdateCookie("ISSession", sToken, dicKeys);
            this.CreateOrUpdateCookie("ISSessionID", Convert.ToString(sessionId), dicKeys);
            this.CreateOrUpdateCookie("NEmpresa", retLoginLegado.ValorParametros["NOM_FAT_PDV"].ToString(), dicKeys);
            this.CreateOrUpdateCookie("NEstabelecimento", _ncadastro, dicKeys);
            this.CreateOrUpdateCookie("NLoginName", _usuario, dicKeys);
        }

        /// <summary>
        /// Limpar cookies adicionais que já tenha sido criados em outro login
        /// </summary>
        /// <param name="dicKeys"></param>
        private void ClearAnotherCookies(List<string> dicKeys) {
            if (dicKeys.Contains("needValidation")) this.Page.Response.Cookies["needValidation"].Expires = DateTime.MinValue;
            if (dicKeys.Contains("NEmpresa_2")) this.Page.Response.Cookies["NEmpresa_2"].Expires = DateTime.MinValue;
            if (dicKeys.Contains("NLoginName_2")) this.Page.Response.Cookies["NLoginName_2"].Expires = DateTime.MinValue;
            if (dicKeys.Contains("NEstabelecimento_2")) this.Page.Response.Cookies["NEstabelecimento_2"].Expires = DateTime.MinValue;
            if (dicKeys.Contains("avisoAcessoFiliais")) this.Page.Response.Cookies["avisoAcessoFiliais"].Expires = DateTime.MinValue;
            if (dicKeys.Contains("acessoFiliais")) this.Page.Response.Cookies["acessoFiliais"].Expires = DateTime.MinValue;
            if (dicKeys.Contains("avisoMatrizRollback")) this.Page.Response.Cookies["avisoMatrizRollback"].Expires = DateTime.MinValue;
        }

        /// <summary>
        /// Cria ou atualiza o valor de um cookie
        /// </summary>
        /// <param name="sToken"></param>
        /// <param name="dicKeys"></param>
        private void CreateOrUpdateCookie(string key, string value, List<string> dicKeys) {
            if (!dicKeys.Contains(key))
                Response.SetCookie(new HttpCookie(key, value));
            else
                Response.Cookies[key].Value = value;
        }
    }
}