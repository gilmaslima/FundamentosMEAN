using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using Redecard.PN.Comum;
using Rede.PN.Cancelamento.Sharepoint.CancelamentoServico;

namespace Rede.PN.Cancelamento.Sharepoint.Layouts.Rede.PN.Cancelamento.Handlers
{
    public partial class BuscarTransacoesParaCancelamento : IHttpHandler
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
        /// Executa o serviço que busca os detalhes de uma transação para cancelamento
        /// </summary>
        /// <param name="context">Contexto Http</param>
        public void ProcessRequest(HttpContext context)
        {
            using (var log = Logger.IniciarLog("BuscarTransacoesParaCancelamento - Handler"))
            {
                try
                {
                    Int32 numeroEstabelecimentoVenda = context.Request["numeroEstabelecimentoVenda"].ToInt32();
                    TipoVenda tipoVenda = context.Request["tipoVenda"].GetEnumByDescription<TipoVenda>();
                    DateTime dataVenda = context.Request["dataVenda"].ToDate("dd/MM/yyyy");
                    String nsu = context.Request["nsu"];
                    Decimal valorBruto = context.Request["valorBruto"].ToDecimal();
                    Int16 numeroMes = context.Request["numeroMes"].ToInt16();
                    String timestampTransacao = context.Request["timestampTransacao"].ToString();
                    TipoTransacao tipoTransacao = (TipoTransacao)context.Request["tipoTransacao"].ToInt32();
                    String dados = context.Request["dados"];

                    var cancelamento = new SolicitacaoCancelamento
                    {
                        NumeroEstabelecimentoVenda = numeroEstabelecimentoVenda,
                        TipoVenda = tipoVenda,
                        DataVenda = dataVenda,
                        NSU = nsu,
                        NumeroMes = numeroMes,
                        TimestampTransacao = timestampTransacao,
                        TipoTransacao = tipoTransacao
                    };

                    var transacao = Services.BuscarTransacaoParaCancelamento(cancelamento, dados);

                    context.Response.Write(JsSerializer.Serialize(new { Transacao = transacao }));
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
