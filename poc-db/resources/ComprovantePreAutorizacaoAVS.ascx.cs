using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Redecard.PN.DataCash.Modelo.Util;
using Redecard.PN.DataCash.BasePage;
using System.Configuration;
using System.Text;
using System.Xml;

namespace Redecard.PN.DataCash.controles.comprovantes
{
    public partial class ComprovantePreAutorizacaoAVS : UserControlBaseDataCash
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
            else
                Response.Redirect("FacaSuaVenda.aspx", true);
        }

        /// <summary>
        /// Ação do botão voltar
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("FacaSuaVenda.aspx");
        }
        
        /// <summary>
        /// Processa retorno da resposta do serviço da DataCash se houve sucesso ou erro
        /// </summary>
        /// <param name="dataCashXML">XML de envio para DataCash</param>
        private void VerificarRetornoDataCash(Modelo.Venda venda)
        {
            Negocio.Vendas transacao = new Negocio.Vendas();
            int codigoRetorno = 0;
            String mensagem = string.Empty;
            Modelo.TransacaoVenda retornoTransacao = null;

            retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaPreAutorizacaoAVS), Request.UserHostAddress, out codigoRetorno, out mensagem);


            if (codigoRetorno != 1)
            {
                base.GeraPainelExcecao(mensagem, codigoRetorno.ToString());
            }
            else
            {
                this.ExibirComprovante((retornoTransacao as Modelo.TransacaoVendaPreAutorizacaoAVS));
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
            ltNomeEstabelecimento.Text = this.SessaoAtual.NomeEntidade.ToString();
            ltDataVenda.Text = DateTime.Now.ToString("dd/MM/yyyy");
            ltHoraVenda.Text = DateTime.Now.ToString("HH:mm");
            ltNumeroTerminal.Text = "-";
            ltNumeroAutorizacao.Text = transacao.NumeroAutorizacao.ToString();
            ltTipoTransacao.Text = transacao.TipoTransacao.GetTitle();
            ltBandeira.Text = transacao.DadosCartao.Bandeira.GetTitle();
            ltNomePortador.Text = transacao.DadosCartao.NomePortador;
            ltNumeroCartao.Text = transacao.DadosCartao.NumeroCriptografado;
            ltValor.Text = transacao.ValorFormatado;
            // Alteração solicitada pela Ana Cruz/Caroline e-mail 24012014
            if (transacao.DadosCartao.Bandeira.GetTitle() == "American Express" ||
                transacao.DadosCartao.Bandeira.GetTitle() == "Elo")
                ltValidade.Text = "-";
            else
                ltValidade.Text = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy");
            ltNumeroPedido.Text = transacao.NumeroPedido;

            tbComprovante.Visible = true;
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