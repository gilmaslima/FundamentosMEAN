using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Redecard.PN.DataCash.Handlers
{
    /// <summary>
    /// Summary description for CarregaCidades
    /// </summary>
    public class CarregaCidades : IHttpHandler
    {

        /// <summary>
        /// Processa a requisição
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            var retorno = ProcessaListas.ObterCidadesEstado(context.Request.Form["Estado"]);

            context.Response.Write((new JavaScriptSerializer()).Serialize(retorno));
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