using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web;
using System.Web.Script.Serialization;
using Redecard.PN.Comum;

namespace Rede.PN.Cancelamento.Sharepoint.Layouts.Rede.PN.Cancelamento.Handlers
{
    public partial class VerificarEstabelecimentoEmCentralizadora : IHttpHandler
    {
        #region [ Propriedades ]

        /// <summary>JS Serializer</summary>
        private static JavaScriptSerializer jsSerializer;
        private static JavaScriptSerializer JsSerializer
        {
            get
            {
                return jsSerializer ?? (jsSerializer = new JavaScriptSerializer());
            }
        }

        /// <summary>Valor padrão de propriedades</summary>
        public bool IsReusable { get { return false; } }

        #endregion

        /// <summary>
        /// Executa o serviço que verifica se uma filial pertence a dada matriz
        /// </summary>
        /// <param name="context">Contexto Http</param>
        public void ProcessRequest(HttpContext context)
        {
            using (var log = Logger.IniciarLog("VerificarEstabelecimentoEmCentralizadora - Handler"))
            {

                try
                {
                    Int32 numeroEstabelecimentoVenda = context.Request["numeroEstabelecimentoVenda"].ToInt32();
                    Int32 numeroEstabelecimentoCentralizadora = context.Request["numeroEstabelecimentoCentralizadora"].ToInt32();

                    var retorno = Services.VerificarEstabelecimentoEmCentralizadora(numeroEstabelecimentoVenda, numeroEstabelecimentoCentralizadora);
#if DEBUG
                    retorno = true;
#endif

                    context.Response.Write(JsSerializer.Serialize(new { Retorno = retorno, MensagemErro = "Estabelecimento inválido." }));
                    context.Response.ContentType = "application/json";
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
                catch (PortalRedecardException ex)
                {
                    context.Response.Write(JsSerializer.Serialize(new { MensagemErro = ex.Fonte }));
                    context.Response.ContentType = "application/json";
                    context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
