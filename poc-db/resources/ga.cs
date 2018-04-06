using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

namespace Redecard.Portal.Aberto.WebControls {

    /// <summary>
    /// Renderiza os parametros de análise do Google Analytics
    /// </summary>
    public class ga : WebControl {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer) {
            HttpCookieCollection cookies = Page.Request.Cookies;
            List<string> dicKeys = new List<string>();
            dicKeys.AddRange(cookies.AllKeys);
            if (dicKeys.Contains("NEstabelecimento")) {
                string numeroPV = cookies["NEstabelecimento"].Value;
                writer.Write(String.Format("pageTracker._setCustomVar(1, \"Estabelecimento\", \"{0}\", 1); pageTracker._initData();", numeroPV)); 
            }
        }
    }
}