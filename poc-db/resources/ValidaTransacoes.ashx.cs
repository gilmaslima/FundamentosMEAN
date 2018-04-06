using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web;
using System.Web.Script.Serialization;
using Redecard.PN.Comum;
using System.Collections.Generic;
using System.Linq;
using Rede.PN.Cancelamento.Sharepoint.CancelamentoServico;

namespace Rede.PN.Cancelamento.Sharepoint.Layouts.Rede.PN.Cancelamento.Handlers
{
    public partial class ValidaTransacoes : IHttpHandler
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
        /// Executa o serviço que valida as transações
        /// </summary>
        /// <param name="context">Contexto Http</param>
        public void ProcessRequest(HttpContext context)
        {
            using (var log = Logger.IniciarLog("ValidaTransacoes - Handler"))
            {
                try
                {
                    List<CancelamentoServico.SolicitacaoCancelamento> cancelamentos = new List<CancelamentoServico.SolicitacaoCancelamento>();

                    Int32 codigoRamoAtividade = context.Request["codigoRamoAtividade"].ToInt32();
                    String valorBrutoVenda1 = context.Request["valorBrutoVenda1"];
                    String saldoDisponivel1 = context.Request["saldoDisponivel1"];
                    String dados = context.Request["dados"];

                    if (!String.IsNullOrEmpty(valorBrutoVenda1) && !String.IsNullOrEmpty(saldoDisponivel1))
                    {
                        Int32 numeroEstabelecimento1 = context.Request["numeroEstabelecimento1"].ToInt32();
                        TipoVenda tipoVenda1 = context.Request["tipoVenda1"].GetEnumValueFromDescription<TipoVenda>();
                        DateTime dataVenda1 = context.Request["dataVenda1"].ToDate("dd/MM/yyyy");
                        String nsu1 = context.Request["nsu1"];
                        TipoCancelamento tipoCancelamento1 = context.Request["tipoCancelamento1"].GetEnumValueFromDescription<TipoCancelamento>();
                        Decimal valorCancelamento1 = ToDecimalString(context.Request["valorCancelamento1"]).ToDecimal();
                        Int16 mes1 = context.Request["mes1"].ToInt16();
                        String timestamp1 = context.Request["timestamp1"];
                        TipoTransacao tipoTransacao1 = (TipoTransacao)context.Request["tipoTransacao1"].ToInt32();

                        cancelamentos.Add(new CancelamentoServico.SolicitacaoCancelamento
                        {
                            CodigoRamoAtividade = codigoRamoAtividade,
                            NumeroEstabelecimentoVenda = numeroEstabelecimento1,
                            TipoVenda = tipoVenda1,
                            DataVenda = dataVenda1,
                            NSU = nsu1,
                            ValorBruto = ToDecimalString(valorBrutoVenda1).ToDecimal(),
                            SaldoDisponivel = ToDecimalString(saldoDisponivel1).ToDecimal(),
                            TipoCancelamento = tipoCancelamento1,
                            ValorCancelamento = valorCancelamento1,
                            NumeroMes = mes1,
                            TimestampTransacao = timestamp1,
                            TipoTransacao = tipoTransacao1,
                            Linha = 1
                        });
                    }

                    String valorBrutoVenda2 = context.Request["valorBrutoVenda2"];
                    String saldoDisponivel2 = context.Request["saldoDisponivel2"];

                    if (!String.IsNullOrEmpty(valorBrutoVenda2) && !String.IsNullOrEmpty(saldoDisponivel2))
                    {
                        Int32 numeroEstabelecimento2 = context.Request["numeroEstabelecimento2"].ToInt32();
                        TipoVenda tipoVenda2 = context.Request["tipoVenda2"].GetEnumValueFromDescription<TipoVenda>();
                        DateTime dataVenda2 = context.Request["dataVenda2"].ToDate("dd/MM/yyyy");
                        String nsu2 = context.Request["nsu2"];
                        TipoCancelamento tipoCancelamento2 = context.Request["tipoCancelamento2"].GetEnumValueFromDescription<TipoCancelamento>();
                        Decimal valorCancelamento2 = ToDecimalString(context.Request["valorCancelamento2"]).ToDecimal();
                        Int16 mes2 = context.Request["mes2"].ToInt16();
                        String timestamp2 = context.Request["timestamp2"];
                        TipoTransacao tipoTransacao2 = (TipoTransacao)context.Request["tipoTransacao2"].ToInt32();

                        cancelamentos.Add(new CancelamentoServico.SolicitacaoCancelamento
                        {
                            CodigoRamoAtividade = codigoRamoAtividade,
                            NumeroEstabelecimentoVenda = numeroEstabelecimento2,
                            TipoVenda = tipoVenda2,
                            DataVenda = dataVenda2,
                            NSU = nsu2,
                            ValorBruto = ToDecimalString(valorBrutoVenda2).ToDecimal(),
                            SaldoDisponivel = ToDecimalString(saldoDisponivel2).ToDecimal(),
                            TipoCancelamento = tipoCancelamento2,
                            ValorCancelamento = valorCancelamento2,
                            NumeroMes = mes2,
                            TimestampTransacao = timestamp2,
                            TipoTransacao = tipoTransacao2,
                            Linha = 2
                        });
                    }

                    String valorBrutoVenda3 = context.Request["valorBrutoVenda3"];
                    String saldoDisponivel3 = context.Request["saldoDisponivel3"];

                    if (!String.IsNullOrEmpty(valorBrutoVenda3) && !String.IsNullOrEmpty(saldoDisponivel3))
                    {
                        Int32 numeroEstabelecimento3 = context.Request["numeroEstabelecimento3"].ToInt32();
                        TipoVenda tipoVenda3 = context.Request["tipoVenda3"].GetEnumValueFromDescription<TipoVenda>();
                        DateTime dataVenda3 = context.Request["dataVenda3"].ToDate("dd/MM/yyyy");
                        String nsu3 = context.Request["nsu3"];
                        TipoCancelamento tipoCancelamento3 = context.Request["tipoCancelamento3"].GetEnumValueFromDescription<TipoCancelamento>();
                        Decimal valorCancelamento3 = ToDecimalString(context.Request["valorCancelamento3"]).ToDecimal();
                        Int16 mes3 = context.Request["mes3"].ToInt16();
                        String timestamp3 = context.Request["timestamp3"];
                        TipoTransacao tipoTransacao3 = (TipoTransacao)context.Request["tipoTransacao3"].ToInt32();

                        cancelamentos.Add(new CancelamentoServico.SolicitacaoCancelamento
                        {
                            CodigoRamoAtividade = codigoRamoAtividade,
                            NumeroEstabelecimentoVenda = numeroEstabelecimento3,
                            TipoVenda = tipoVenda3,
                            DataVenda = dataVenda3,
                            NSU = nsu3,
                            ValorBruto = ToDecimalString(valorBrutoVenda3).ToDecimal(),
                            SaldoDisponivel = ToDecimalString(saldoDisponivel3).ToDecimal(),
                            TipoCancelamento = tipoCancelamento3,
                            ValorCancelamento = valorCancelamento3,
                            NumeroMes = mes3,
                            TimestampTransacao = timestamp3,
                            TipoTransacao = tipoTransacao3,
                            Linha = 3
                        });
                    }

                    String valorBrutoVenda4 = context.Request["valorBrutoVenda4"];
                    String saldoDisponivel4 = context.Request["saldoDisponivel4"];

                    if (!String.IsNullOrEmpty(valorBrutoVenda4) && !String.IsNullOrEmpty(saldoDisponivel4))
                    {
                        Int32 numeroEstabelecimento4 = context.Request["numeroEstabelecimento4"].ToInt32();
                        TipoVenda tipoVenda4 = context.Request["tipoVenda4"].GetEnumValueFromDescription<TipoVenda>();
                        DateTime dataVenda4 = context.Request["dataVenda4"].ToDate("dd/MM/yyyy");
                        String nsu4 = context.Request["nsu4"];
                        TipoCancelamento tipoCancelamento4 = context.Request["tipoCancelamento4"].GetEnumValueFromDescription<TipoCancelamento>();
                        Decimal valorCancelamento4 = ToDecimalString(context.Request["valorCancelamento4"]).ToDecimal();
                        Int16 mes4 = context.Request["mes4"].ToInt16();
                        String timestamp4 = context.Request["timestamp4"];
                        TipoTransacao tipoTransacao4 = (TipoTransacao)context.Request["tipoTransacao4"].ToInt32();

                        cancelamentos.Add(new CancelamentoServico.SolicitacaoCancelamento
                        {
                            CodigoRamoAtividade = codigoRamoAtividade,
                            NumeroEstabelecimentoVenda = numeroEstabelecimento4,
                            TipoVenda = tipoVenda4,
                            DataVenda = dataVenda4,
                            NSU = nsu4,
                            ValorBruto = ToDecimalString(valorBrutoVenda4).ToDecimal(),
                            SaldoDisponivel = ToDecimalString(saldoDisponivel4).ToDecimal(),
                            TipoCancelamento = tipoCancelamento4,
                            ValorCancelamento = valorCancelamento4,
                            NumeroMes = mes4,
                            TimestampTransacao = timestamp4,
                            TipoTransacao = tipoTransacao4,
                            Linha = 4
                        });
                    }

                    String valorBrutoVenda5 = context.Request["valorBrutoVenda5"];
                    String saldoDisponivel5 = context.Request["saldoDisponivel5"];

                    if (!String.IsNullOrEmpty(valorBrutoVenda5) && !String.IsNullOrEmpty(saldoDisponivel5))
                    {
                        Int32 numeroEstabelecimento5 = context.Request["numeroEstabelecimento5"].ToInt32();
                        TipoVenda tipoVenda5 = context.Request["tipoVenda5"].GetEnumValueFromDescription<TipoVenda>();
                        DateTime dataVenda5 = context.Request["dataVenda5"].ToDate("dd/MM/yyyy");
                        String nsu5 = context.Request["nsu5"];
                        TipoCancelamento tipoCancelamento5 = context.Request["tipoCancelamento5"].GetEnumValueFromDescription<TipoCancelamento>();
                        Decimal valorCancelamento5 = ToDecimalString(context.Request["valorCancelamento5"]).ToDecimal();
                        Int16 mes5 = context.Request["mes5"].ToInt16();
                        String timestamp5 = context.Request["timestamp5"];
                        TipoTransacao tipoTransacao5 = (TipoTransacao)context.Request["tipoTransacao5"].ToInt32();

                        cancelamentos.Add(new CancelamentoServico.SolicitacaoCancelamento
                        {
                            CodigoRamoAtividade = codigoRamoAtividade,
                            NumeroEstabelecimentoVenda = numeroEstabelecimento5,
                            TipoVenda = tipoVenda5,
                            DataVenda = dataVenda5,
                            NSU = nsu5,
                            ValorBruto = ToDecimalString(valorBrutoVenda5).ToDecimal(),
                            SaldoDisponivel = ToDecimalString(saldoDisponivel5).ToDecimal(),
                            TipoCancelamento = tipoCancelamento5,
                            ValorCancelamento = valorCancelamento5,
                            NumeroMes = mes5,
                            TimestampTransacao = timestamp5,
                            TipoTransacao = tipoTransacao5,
                            Linha = 5
                        });
                    }

                    var validacoes = new List<CancelamentoServico.Validacao>();

                    if (cancelamentos.Count == 0)
                        validacoes.Add(new Validacao { Status = 2, Descricao = "Venda não encontrada, verifique os dados que foram digitados.", Linha = 1 });
                    else
                        validacoes = Services.ValidarCancelamentos('C', cancelamentos, dados);

                    context.Response.Write(JsSerializer.Serialize(new { Validacoes = validacoes }));
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

        /// <summary>
        /// Remove todos os caractere '.' e depois substitui o caractere ',' por '.'
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private String ToDecimalString(String number)
        {
            return number.Replace(".", "").Replace(",", ".");
        }
    }
}
