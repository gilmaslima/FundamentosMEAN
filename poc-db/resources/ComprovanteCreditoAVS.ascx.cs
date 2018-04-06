using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Redecard.PN.DataCash.Modelo.Util;
using Redecard.PN.DataCash.BasePage;
using System.Xml;
using System.Text;
using System.Configuration;

namespace Redecard.PN.DataCash.controles.comprovantes
{
    public partial class ComprovanteCreditoAVS : UserControlBaseDataCash
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                base.AtualizaSession();

                Modelo.Venda venda = (Modelo.Venda)Session["FacaSuaVenda"];

                // Gera XML e verifica procesamento da DataCash
                this.VerificarRetornoDataCash(venda);
            }
        }

        /// <summary>
        /// Processa retorno da resposta do serviço da DataCash se houve sucesso ou erro
        /// </summary>
        /// <param name="dataCashXML">XML de envio para DataCash</param>
        private void VerificarRetornoDataCash(Modelo.Venda venda)
        {
            // Realiza chamada ao serviço DataCash
            //DataCashService.TransactionToDatacashClient dataCash = new DataCashService.TransactionToDatacashClient();
            //String retornoXML =  dataCash.TransactionXML(dataCashXML, Request.UserHostAddress);
            Negocio.Vendas transacao = new Negocio.Vendas();
            int codigoRetorno = 0;
            String mensagem = string.Empty;
            Modelo.TransacaoVenda retornoTransacao = null;

            if (venda.FormaPagamento == Modelo.enFormaPagamento.Avista)
                retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaCreditoAVSAVista), Request.UserHostAddress, out codigoRetorno, out mensagem);
            else if (venda.FormaPagamento == Modelo.enFormaPagamento.ParceladoEmissor)
                retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaCreditoAVSParceladoEmissor), Request.UserHostAddress, out codigoRetorno, out mensagem);
            else if (venda.FormaPagamento == Modelo.enFormaPagamento.ParceladoEstabelecimento)
                retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaCreditoAVSParceladoEstabelecimento), Request.UserHostAddress, out codigoRetorno, out mensagem);


            if (codigoRetorno != 1)
            {
                base.GeraPainelExcecao(mensagem, codigoRetorno.ToString());
            }
            else
            {
                this.ExibirComprovante(retornoTransacao);
            }

            Session.Remove("FacaSuaVenda");
        }

        /// <summary>
        /// Exibe o comprovante com as informações da venda em caso de processamento com sucesso realizado na DataCash
        /// </summary>
        private void ExibirComprovante(Modelo.TransacaoVenda transacao)
        {

            ltNSU.Text = transacao.NSU; //Construção DataCash
            ltTID.Text = transacao.TID;
            ltNumeroEstabelecimento.Text = this.SessaoAtual.CodigoEntidade.ToString();
            ltNomeEstabelecimento.Text = this.SessaoAtual.NomeEntidade;
            ltDataVenda.Text = DateTime.Now.ToString("dd/MM/yyyy");
            ltHoraVenda.Text = DateTime.Now.ToString("HH:mm");
            //ltNumeroTerminal.Text = "-";
            ltNumeroAutorizacao.Text = transacao.NumeroAutorizacao.ToString();
            ltTipoTransacao.Text = transacao.TipoTransacao.GetTitle();
            ltFormaPagamento.Text = transacao.FormaPagamento.GetTitle();
            ltBandeira.Text = transacao.DadosCartao.Bandeira.GetTitle();
            ltNomePortador.Text = transacao.DadosCartao.NomePortador;
            ltNumeroCartao.Text = transacao.DadosCartao.Numero.Right(4);
            ltValor.Text = transacao.ValorFormatado;
            ltNumeroPedido.Text = transacao.NumeroPedido;
            ltParcelas.Text = transacao.DadosCartao.Parcelas;
            if (transacao.FormaPagamento == Modelo.enFormaPagamento.Avista)
            {
                trParcelas.Visible = false;
                ucParceladoEmissor.Visible = false;
            }
            else if (transacao.FormaPagamento == Modelo.enFormaPagamento.ParceladoEstabelecimento)
            {
                ucParceladoEmissor.Visible = false;
            }
            else if (transacao.FormaPagamento == Modelo.enFormaPagamento.ParceladoEmissor)
            {
                trParcelas.Visible = true;
                ucParceladoEmissor.ValorParcela = 0;
                ucParceladoEmissor.Encargos = 0;
                ucParceladoEmissor.ValorTotalPagar = 0;
            }

            Session.Add("transacao", transacao);

            tbComprovante.Visible = true;
        }
        /// <summary>
        /// Ação do botão voltar
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("FacaSuaVenda.aspx");
        }


        #region Obtenção de Pagina de Redirecionamento
        /// <summary>
        /// Página de redirecionamento do Lightbox
        /// </summary>
        /// <returns>String com o redirecionamento</returns>
        public override String ObterPaginaRedirecionamento()
        {
            return "pn_FacaSuaVenda.aspx";
        }
        #endregion
    }
}