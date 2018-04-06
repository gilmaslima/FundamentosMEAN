using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text;
using System.Web.Configuration;
using Redecard.PN.Boston.Sharepoint.Base;
using Redecard.PN.Comum;
using Redecard.PN.Boston.Sharepoint.Negocio;
using System.Globalization;
using System.Web.UI.WebControls;

namespace Redecard.PN.Boston.Sharepoint.Layouts.Boston
{
    public partial class RetornoPagamentoFechado : BostonBasePage
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
            Int32 numPdv = Request.Form["pedido"].Split('-')[0].ToInt32();
            Int32 canal = Servicos.GetCanalPontoVenda(numPdv);
            Int32 celula = Servicos.GetCelulaPontoVenda(numPdv);
            Decimal taxaAtivacao = Decimal.Parse(Servicos.GetTaxaAtivacao(canal, celula, null, null, null, null, null, 30), NumberStyles.Currency);

            urlParent.Value = String.Format("{0}/{1}", SPContext.Current.Web.Url, "sites/fechado/MobileRede/Paginas/pn_PedidoNovoEquipamento.aspx");
            String status = Request.Form["status"].Trim();
            String nsu = Request.Form["auth_host_reference"];
            String tid = Request.Form["gateway_reference"];
            String numEstabelecimento = canal == 26 && celula == 503 ?
                WebConfigurationManager.AppSettings["numPdvEstabelecimentoVivo"].ToString() : 
                WebConfigurationManager.AppSettings["numPdvEstabelecimento"].ToString();
            String nomeEstabelecimento = canal == 26 && celula == 503 ?
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
            String valor = String.Format("{0:C}", taxaAtivacao);
            String numPedido = Request.Form["pedido"];
            String numParcelas = Request.Form["parcelas"];

            if (status.Equals("1"))
                codigoRetorno = IncluirPropostaVendaTecnologiaPorPV(out mensagemRetorno, taxaAtivacao, numPdv);

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
        private Int32 IncluirPropostaVendaTecnologiaPorPV(out String mensagemRetorno, Decimal taxaAtivacao, Int32 numPdv)
        {
            return Servicos.IncluirPropostaVendaTecnologiaPorPV(numPdv, "CCM", "PORTAL", 15, 482, 0, taxaAtivacao, out mensagemRetorno);
        }
    }
}
