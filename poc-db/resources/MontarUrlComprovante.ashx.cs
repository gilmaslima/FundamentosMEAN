using System;
using System.Web;
using Redecard.PN.Comum;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace Redecard.PN.Boston.Sharepoint.Layouts.Boston.Handlers
{
    public partial class MontarUrlComprovante : IHttpHandler
    {
        #region [ Propriedades ]

        /// <summary>JS Serializer</summary>
        private static JavaScriptSerializer _jsSerializer;
        private static JavaScriptSerializer JsSerializer
        {
            get
            {
                return _jsSerializer ?? (_jsSerializer = new JavaScriptSerializer());
            }
        }

        /// <summary>Valor padrão de propriedades</summary>
        public bool IsReusable { get { return false; } }

        #endregion

        public void ProcessRequest(HttpContext context)
        {
            QueryStringSegura queryString = new QueryStringSegura();
            queryString.Add("nsu", HttpUtility.UrlEncode(context.Request["nsu"]));
            queryString.Add("tid", HttpUtility.UrlEncode(context.Request["tid"]));
            queryString.Add("numeroEstabelecimento", HttpUtility.UrlEncode(context.Request["numeroEstabelecimento"]));
            queryString.Add("nomeEstabelecimento", HttpUtility.UrlEncode(context.Request["nomeEstabelecimento"]));
            queryString.Add("dataPagamento", HttpUtility.UrlEncode(context.Request["dataPagamento"]));
            queryString.Add("horaPagamento", HttpUtility.UrlEncode(context.Request["horaPagamento"]));
            queryString.Add("numeroAutorizacao", HttpUtility.UrlEncode(context.Request["numeroAutorizacao"]));
            queryString.Add("tipoTransacao", HttpUtility.UrlEncode(context.Request["tipoTransacao"]));
            queryString.Add("formaPagamento", HttpUtility.UrlEncode(context.Request["formaPagamento"]));
            queryString.Add("bandeira", HttpUtility.UrlEncode(context.Request["bandeira"]));
            queryString.Add("nomePortador", HttpUtility.UrlEncode(context.Request["nomePortador"]));
            queryString.Add("numeroCartao", HttpUtility.UrlEncode(context.Request["numeroCartao"]));
            queryString.Add("valor", HttpUtility.UrlEncode(context.Request["valor"]));
            queryString.Add("numeroParcelas", HttpUtility.UrlEncode(context.Request["numeroParcelas"]));
            queryString.Add("numeroPedido", HttpUtility.UrlEncode(context.Request["numeroPedido"]));

            String url = String.Format("pn_Comprovante.aspx?dados={0}", queryString.ToString());

            context.Response.Write(JsSerializer.Serialize(new { Url = url }));
            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }
    }
}
