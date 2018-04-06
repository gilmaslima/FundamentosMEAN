using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Logger = Redecard.PN.Comum.Logger;
using Redecard.PN.Comum;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.DebitosDesagendamento
{
    public partial class MotivoDebitoDetalhe : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using(Logger Log = Logger.IniciarLog("Débitos e Desagendamentos - Motivo Débito - Detalhe"))
            {
                if (!IsPostBack)
                {
                    try
                    {
                        string dados = Request.QueryString["dados"];

                        if (!string.IsNullOrEmpty(dados))
                        {
                            Redecard.PN.Comum.QueryStringSegura queryString = new Redecard.PN.Comum.QueryStringSegura(dados);
                            string numeroEstabelecimento = queryString["numeroEstabelecimento"];
                            string dataPesquisa = queryString["dataPesquisa"];
                            string timestamp = queryString["timestamp"];
                            string numeroDebito = queryString["numeroDebito"];
                            string tipoPesquisa = queryString["tipoPesquisa"];

                            ViewState["numeroEstabelecimento"] = numeroEstabelecimento;
                            ViewState["dataPesquisa"] = dataPesquisa;
                            ViewState["timestamp"] = timestamp;
                            ViewState["numeroDebito"] = numeroDebito;
                            ViewState["tipoPesquisa"] = tipoPesquisa;
                        }

                        Consultar();
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        Redecard.PN.Comum.SharePointUlsLog.LogErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }
        }

        private void Consultar()
        {
            Servico.DD.ConsultarMotivoDebitoEnvio envio = new Servico.DD.ConsultarMotivoDebitoEnvio()
            {
                NumeroEstabelecimento = int.Parse((string)ViewState["numeroEstabelecimento"]),
                DataPesquisa = DateTime.ParseExact((string)ViewState["dataPesquisa"], "dd/MM/yyyy", null),
                Timestamp = (string)ViewState["timestamp"],
                NumeroDebito = decimal.Parse((string)ViewState["numeroDebito"]),
                TipoPesquisa = (string)ViewState["tipoPesquisa"]
            };

            Servico.DD.StatusRetorno statusRetorno;

            using (var contexto = new ContextoWCF<Servico.DD.RelatorioDebitosDesagendamentosClient>())
            {
                Servico.DD.ConsultarMotivoDebitoRetorno objRetorno = contexto.Cliente.ConsultarMotivoDebito(out statusRetorno, envio);

                PreencherHeader();

                if (statusRetorno.CodigoRetorno != 0)
                {
                    base.ExibirPainelExcecao(statusRetorno.Fonte, statusRetorno.CodigoRetorno);
                    return;
                }

                MontaTabelaMotivo(objRetorno);

                MontaTabelaFormaDePagamento(objRetorno.FormasPagamento);
            }
        }

        private void PreencherHeader()
        {
            lbldataConsulta.Text = DateTime.Today.ToString("dd/MM/yy");
            lblNomeEstabelecimento.Text = this.SessaoAtual.NomeEntidade;
            lblNumeroEstabelecimento.Text = this.SessaoAtual.CodigoEntidade.ToString();
        }

        private void MontaTabelaMotivo(Servico.DD.ConsultarMotivoDebitoRetorno motivo)
        {
            grvDadosMotivoDebito.DataSource = new Servico.DD.ConsultarMotivoDebitoRetorno[] { motivo };
            grvDadosMotivoDebito.DataBind();
        }

        private void MontaTabelaFormaDePagamento(Servico.DD.ConsultarMotivoDebitoFormaPagamentoRetorno[] list)
        {
            grvFormaPagamento.DataSource = list;
            grvFormaPagamento.DataBind();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }

        protected void grvFormaPagamento_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Servico.DD.ConsultarMotivoDebitoFormaPagamentoRetorno item = ((Servico.DD.ConsultarMotivoDebitoFormaPagamentoRetorno)e.Row.DataItem);
                e.Row.Cells[2].Text = item.SinalDebitoCredito == "D" ? "Débito" : item.SinalDebitoCredito == "C" ? "Crédito" : "";
            }
        }
    }
}
