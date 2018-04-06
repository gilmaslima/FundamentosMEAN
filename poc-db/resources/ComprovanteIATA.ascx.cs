using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;
using System.Configuration;

using Redecard.PN.DataCash.BasePage;
using Redecard.PN.DataCash.Modelo.Util;

namespace Redecard.PN.DataCash.controles.comprovantes
{
    public partial class ComprovanteIATA : UserControlBaseDataCash
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                base.AtualizaSession();

                Modelo.VendaPagamentoIATA venda = (Modelo.VendaPagamentoIATA)Session["FacaSuaVenda"];

                // Gera XML e verifica procesamento da DataCash
                this.VerificarRetornoDataCash(venda);
            }
        }

        /// <summary>
        /// Processa retorno da resposta do serviço da DataCash se houve sucesso ou erro
        /// </summary>
        /// <param name="dataCashXML">XML de envio para DataCash</param>
        private void VerificarRetornoDataCash(Modelo.VendaPagamentoIATA venda)
        {
            Negocio.Vendas transacao = new Negocio.Vendas();
            int codigoRetorno = 0;
            String mensagem = string.Empty;
            Modelo.TransacaoVenda retornoTransacao = null;

            if (venda.FormaPagamento == Modelo.enFormaPagamento.Avista)
                retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaPagamentoIATAAvista), Request.UserHostAddress, out codigoRetorno, out mensagem);
            else if (venda.FormaPagamento == Modelo.enFormaPagamento.ParceladoEstabelecimento)
                retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaPagamentoIATAParceladoEstabelecimento), Request.UserHostAddress, out codigoRetorno, out mensagem);


            if (codigoRetorno != 1)
            {
                base.GeraPainelExcecao(mensagem, codigoRetorno.ToString());
            }
            else
            {
                this.ExibirComprovante(retornoTransacao);
                PreencheDadosIATA((retornoTransacao as Modelo.TransacaoVendaPagamentoIATA));
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
            ltNumeroAutorizacao.Text = transacao.NumeroAutorizacao.ToString();
            ltTipoTransacao.Text = transacao.TipoTransacao.GetTitle();
            ltFormaPagamento.Text = transacao.FormaPagamento.GetTitle();
            ltBandeira.Text = transacao.DadosCartao.Bandeira.GetTitle();
            ltNomePortador.Text = transacao.DadosCartao.NomePortador;
            ltNumeroCartao.Text = transacao.DadosCartao.NumeroCriptografado;
            ltValor.Text = transacao.ValorFormatado;
            ltParcelas.Text = transacao.DadosCartao.Parcelas;
            ltNumeroPedido.Text = transacao.NumeroPedido;
            //ltFusoHorario = transacao.FusoHorario

            if (transacao.FormaPagamento == Modelo.enFormaPagamento.Avista)
                trParcelas.Visible = false;

            Session.Add("transacao", transacao);

            tbComprovante.Visible = true;
        }

        private void PreencheDadosIATA(Modelo.TransacaoVendaPagamentoIATA transacao)
        {
            ltTaxaEmbarque.Text = transacao.TaxaEmbarqueFormatado;
            ltValorTotal.Text = transacao.ValorTotalPagarFormatado;
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