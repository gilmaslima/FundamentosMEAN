using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace Redecard.PN.DataCash.Handlers
{
    /// <summary>
    /// Summary description for ControleEndereco
    /// </summary>
    public class ControleEndereco : IHttpHandler, IRequiresSessionState
    {
        /// <summary>
        /// Processa a requisição
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            var cidadeSelecionada = context.Request.Form["CidadeSelecionada"];
            var nomeSessao = context.Request.Form["NomeSessao"];

            HttpContext.Current.Session[nomeSessao] = cidadeSelecionada;

            context.Response.Write((new JavaScriptSerializer()).Serialize(true));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}