using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint.Utilities;

namespace Redecard.Portal.Fechado.SD.Layouts {

    /// <summary>
    /// 
    /// </summary>
    public class clearValidation : Page {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e) {
            HttpCookieCollection cookies = this.Page.Request.Cookies;
            List<string> dicKeys = new List<string>();
            dicKeys.AddRange(cookies.AllKeys);

            if (dicKeys.Contains("needValidation")) {
                ClearAuthCookies(dicKeys);
            }
            // redirecionar para a home do portal fechado
            this.RedirecionarHomePage();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearAuthCookies(List<string> dicKeys) {
            //remover cookies criados para impersonar a sessão
            // dar uma olhada no link "http://msdn.microsoft.com/en-us/library/ms178195.aspx"
            if (dicKeys.Contains("needValidation")) this.Page.Response.Cookies["needValidation"].Expires = DateTime.MinValue;
        }

        /// <summary>
        /// 
        /// </summary>
        void RedirecionarHomePage() {
            SPUtility.Redirect("/sites/fechado", SPRedirectFlags.Default, HttpContext.Current);
        }
    }
}