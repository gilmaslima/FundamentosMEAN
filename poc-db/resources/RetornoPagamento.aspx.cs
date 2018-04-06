using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text;
using Redecard.PN.Comum;
using System.Web.Configuration;
using Redecard.PN.Boston.Sharepoint.Base;
using Redecard.PN.Boston.Sharepoint.Negocio;
using System.Web.UI.WebControls;

namespace Redecard.PN.Boston.Sharepoint.Layouts.Redecard.PN.Boston.Sharepoint
{
    public partial class RetornoPagamento : BostonBasePage
    {
        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            MudarLogoOrigemCredenciamento();

            Int32 codigoRetorno = 0;
            String mensagemRetorno = String.Empty;

            urlParent.Value = String.Format("{0}/{1}", SPContext.Current.Web.Url, "_layouts/MobileRede/EscolhaEquipamento.aspx");
            String status = Request.Form["status"].Trim();
            String nsu = Request.Form["auth_host_reference"];
            String tid = Request.Form["gateway_reference"];
            String numEstabelecimento = DadosCredenciamento.Canal == 26 && DadosCredenciamento.Celula == 503 ?
                WebConfigurationManager.AppSettings["numPdvEstabelecimentoVivo"].ToString() :
                WebConfigurationManager.AppSettings["numPdvEstabelecimento"].ToString();
            String nomeEstabelecimento = DadosCredenciamento.Canal == 26 && DadosCredenciamento.Celula == 503 ?
                WebConfigurationManager.AppSettings["nomeEstabelecimentoVivo"].ToString() :
                WebConfigurationManager.AppSettings["nomeEstabelecimento"].ToString();
            String dataPagamento = DateTime.Now.ToString("dd/MM/yyyy");
            String horaPagamento = DateTime.Now.ToShortTimeString();
            String numAutorizacao = Request.Form["auth_code"];
            String tipoTransacao = Request.Form["forma_pagamento"];
            String formaPagamento = Request.Form["parcelas"].ToInt32(0) > 0 ? "Parcelado" : "À vista";
            String bandeira = Request.Form["card_scheme"];
            String nomePortador = String.Empty;
            String numeroCartao = String.Empty;
            String valor = String.Format("{0:C}", DadosCredenciamento.TaxaAtivacao);
            String numPedido = Request.Form["pedido"];
            String numParcelas = Request.Form["parcelas"];

            if (status.Equals("1"))
            {
                codigoRetorno = IncluirPropostaVendaTecnologiaPorPV(out mensagemRetorno);
                if (codigoRetorno == 0)
                    DadosCredenciamento.CCMExecutada = true;
            }

            if (codigoRetorno == 0)
            {
                String msg = FormataComprovanteJSON(nsu, tid, numEstabelecimento, nomeEstabelecimento, dataPagamento, horaPagamento, numAutorizacao, tipoTransacao, formaPagamento, bandeira, nomePortador, numeroCartao, valor, numParcelas, numPedido, status);
                String script = String.Format("postMessage('{0}');", msg);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, script, true);
            }

        }

        /// <summary>
        /// Muda a imagem e background do logotipo da Masterpage caso a origem seja diferente
        /// </summary>
        private void MudarLogoOrigemCredenciamento()
        {
            HiddenField hdnJsOrigem = (HiddenField)this.Master.FindControl("hdnJsOrigem");
            hdnJsOrigem.Value = String.Format("{0}-{1}", DadosCredenciamento.Canal, DadosCredenciamento.Celula);
        }

        /// <summary>
        /// Formata dados de retorno
        /// </summary>
        /// <returns></returns>
        private String FormataComprovanteJSON(String nsu, String tid, String numEstabelecimento, String nomeEstabelecimento, String dataPagamento, String horaPagamento,
            String numeroAutorizacao, String tipoTransacao, String formaPagamento, String bandeira, String nomePortador, String numeroCartao, String valor, String numParcelas, String numPedido, String codigoErro)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            sb.Append("\"NSU\" : \"");
            sb.Append(nsu);
            sb.Append("\",");

            sb.Append("\"TID\" : \"");
            sb.Append(tid);
            sb.Append("\",");

            sb.Append("\"NumeroEstabelecimento\" : \"");
            sb.Append(numEstabelecimento);
            sb.Append("\",");

            sb.Append("\"NomeEstabelecimento\" : \"");
            sb.Append(nomeEstabelecimento);
            sb.Append("\",");

            sb.Append("\"DataPagamento\" : \"");
            sb.Append(dataPagamento);
            sb.Append("\",");

            sb.Append("\"HoraPagamento\" : \"");
            sb.Append(horaPagamento);
            sb.Append("\",");

            sb.Append("\"NumeroAutorizacao\" : \"");
            sb.Append(numeroAutorizacao);
            sb.Append("\",");

            sb.Append("\"TipoTransacao\" : \"");
            sb.Append(tipoTransacao);
            sb.Append("\",");

            sb.Append("\"FormaPagamento\" : \"");
            sb.Append(formaPagamento);
            sb.Append("\",");

            sb.Append("\"Bandeira\" : \"");
            sb.Append(bandeira);
            sb.Append("\",");

            sb.Append("\"NomePortador\" : \"");
            sb.Append(nomePortador);
            sb.Append("\",");

            sb.Append("\"NumeroCartao\" : \"");
            sb.Append(numeroCartao);
            sb.Append("\",");

            sb.Append("\"Valor\" : \"");
            sb.Append(valor);
            sb.Append("\",");

            sb.Append("\"NumeroParcelas\" : \"");
            sb.Append(numParcelas);
            sb.Append("\",");

            sb.Append("\"NumeroPedido\" : \"");
            sb.Append(numPedido);
            sb.Append("\",");

            sb.Append("\"CodigoErro\" : \"");
            sb.Append(codigoErro);
            sb.Append("\"");

            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// Inclui Proposta Venda Tecnologia Por PV
        /// </summary>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        private Int32 IncluirPropostaVendaTecnologiaPorPV(out String mensagemRetorno)
        {
            return Servicos.IncluirPropostaVendaTecnologiaPorPV(DadosCredenciamento.NumPdv, "CCM", DadosCredenciamento.Usuario, DadosCredenciamento.Canal, DadosCredenciamento.Celula, 0, DadosCredenciamento.TaxaAtivacao, out mensagemRetorno);
        }
    }
}
