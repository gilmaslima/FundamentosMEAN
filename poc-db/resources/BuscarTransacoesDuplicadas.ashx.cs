using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using Redecard.PN.Comum;
using Rede.PN.Cancelamento.Sharepoint.CancelamentoServico;
using System.ServiceModel;

namespace Rede.PN.Cancelamento.Sharepoint.Layouts.Rede.PN.Cancelamento.Handlers
{
    public partial class BuscarTransacoesDuplicadas : IHttpHandler
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
        /// Executa o serviço que busca transações duplicadas para cancelamento
        /// </summary>
        /// <param name="context">Contexto Http</param>
        public void ProcessRequest(HttpContext context)
        {
            using (var log = Logger.IniciarLog("BuscarTransacoesDuplicadas - Handler"))
            {
                Int32 numeroEstabelecimentoVenda = context.Request["numeroEstabelecimentoVenda"].ToInt32();
                TipoVenda tipoVenda = context.Request["tipoVenda"].GetEnumByDescription<TipoVenda>();
                DateTime dataVenda = context.Request["dataVenda"].ToDate("dd/MM/yyyy");
                String dados = context.Request["dados"];
                String nsu = context.Request["nsu"];

                try
                {
                    if (dataVenda >= new DateTime(1980, 1, 1))
                    {
                        var cancelamento = new SolicitacaoCancelamento
                        {
                            NumeroEstabelecimentoVenda = numeroEstabelecimentoVenda,
                            TipoVenda = tipoVenda,
                            DataVenda = dataVenda,
                            NSU = nsu
                        };

                        var transacoes = Services.BuscarTransacaoDuplicadaParaCancelamento(cancelamento, dados);

                        context.Response.Write(JsSerializer.Serialize(new { Transacoes = transacoes }));
                        context.Response.ContentType = "application/json";
                        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    }
                    else
                    {
                        context.Response.Write(JsSerializer.Serialize(new { MensagemErro = "Formato de data incorreto ou inválido." }));
                        context.Response.ContentType = "application/json";
                        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    }
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
