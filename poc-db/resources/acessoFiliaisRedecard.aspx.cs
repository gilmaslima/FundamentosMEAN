using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web;
using Microsoft.SharePoint.Utilities;

namespace Redecard.Portal.Fechado.SD.Layouts {

    /// <summary>
    /// Página de Login da Redecard
    /// </summary>
    public class acessoFiliaisRedecard : Page {

        /// <summary>
        /// 
        /// </summary>
        const string _NEmpresaAlternative = "NEmpresa_2";

        /// <summary>
        /// 
        /// </summary>
        const string _NEstabelecimentoAlternative = "NEstabelecimento_2";

        /// <summary>
        /// 
        /// </summary>
        const string _NLoginNameAlternative = "NLoginName_2";

        /// <summary>
        /// Verificar se os parâmetros foram corretamente enviados
        /// </summary>
        /// <returns></returns>
        public bool CheckParams() {
            HttpRequest request = this.Page.Request;
            if (!String.IsNullOrEmpty(request.QueryString[_NEmpresaAlternative]) &&
                !String.IsNullOrEmpty(request.QueryString[_NLoginNameAlternative]) &&
                !String.IsNullOrEmpty(request.QueryString[_NEstabelecimentoAlternative]))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Carregamento da página, verificar se os parametros de alteração foram passados e fazer a troca dos valores
        /// de cookies para o SharePoint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e) {
            if (this.CheckParams()) {
                // recuperar coleção de cookies e setar os novos valores
                this.SetNewAuthCookies();
            }
            else {
                // a página foi chamada sem a passagem de parâmetros, fazer a chamada de retorno
                this.ClearAuthCookies();
            }
            // redirecionar para a home do portal fechado
            this.RedirecionarHomePage();
        }

        /// <summary>
        /// Limpar os cookies de usados para impersonar a sessão.... :) ..
        /// </summary>
        private void ClearAuthCookies() {
            HttpCookieCollection cookies = this.Page.Request.Cookies;
            List<string> dicKeys = new List<string>();
            dicKeys.AddRange(cookies.AllKeys);

            //remover cookies criados para impersonar a sessão
            // dar uma olhada no link "http://msdn.microsoft.com/en-us/library/ms178195.aspx"
            if (dicKeys.Contains(_NEmpresaAlternative)) this.Page.Response.Cookies[_NEmpresaAlternative].Expires = DateTime.MinValue;
            if (dicKeys.Contains(_NLoginNameAlternative)) this.Page.Response.Cookies[_NLoginNameAlternative].Expires = DateTime.MinValue;
            if (dicKeys.Contains(_NEstabelecimentoAlternative)) this.Page.Response.Cookies[_NEstabelecimentoAlternative].Expires = DateTime.MinValue;
            if (dicKeys.Contains("avisoAcessoFiliais")) this.Page.Response.Cookies["avisoAcessoFiliais"].Expires = DateTime.MinValue;
            if (dicKeys.Contains("acessoFiliais")) this.Page.Response.Cookies["acessoFiliais"].Expires = DateTime.MinValue;

            // exibir mensagem de volta para matriz
            if (dicKeys.Contains("avisoMatrizRollback")) this.Page.Response.Cookies["avisoMatrizRollback"].Value = true.ToString();
        }

        /// <summary>
        /// Redirecionar o usuário para a homepage do portal fechado
        /// </summary>
        void RedirecionarHomePage() {
            SPUtility.Redirect("/sites/fechado", SPRedirectFlags.Default, HttpContext.Current);
        }

        /// <summary>
        /// Setar os novos cookies de sessão de usuário
        /// </summary>
        private void SetNewAuthCookies() {
            HttpCookieCollection cookies = this.Page.Request.Cookies;
            List<string> dicKeys = new List<string>();
            dicKeys.AddRange(cookies.AllKeys);

            // Setar novos cookies de autenticação
            this.SetNewCookie(dicKeys, _NLoginNameAlternative);
            this.SetNewCookie(dicKeys, _NEmpresaAlternative);
            this.SetNewCookie(dicKeys, _NEstabelecimentoAlternative);

            // Setar cookie para apresentação de uma mensagem de mudança na home do portal
            if (dicKeys.Contains("avisoAcessoFiliais")) {
                Page.Response.Cookies["avisoAcessoFiliais"].Value = true.ToString();
                Page.Response.Cookies["acessoFiliais"].Value = true.ToString();
                Page.Response.Cookies["avisoMatrizRollback"].Value = false.ToString();
            }
            else {
                Page.Response.Cookies.Add(new HttpCookie("avisoAcessoFiliais") { Value = true.ToString() });
                Page.Response.Cookies.Add(new HttpCookie("acessoFiliais") { Value = true.ToString() });
                Page.Response.Cookies.Add(new HttpCookie("avisoMatrizRollback") { Value = false.ToString() });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookies"></param>
        /// <param name="dicKeys"></param>
        private void SetNewCookie(List<string> dicKeys, string cookieKey) {
            if (dicKeys.Contains(cookieKey))
                Page.Response.Cookies[cookieKey].Value = this.Page.Request[cookieKey];
            else
                Page.Response.Cookies.Add(new HttpCookie(cookieKey) { Value =  this.Page.Request[cookieKey] });
        }   
    }
}