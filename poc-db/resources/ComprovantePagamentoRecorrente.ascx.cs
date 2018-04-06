using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using Redecard.PN.DataCash.Modelo.Util;
using Redecard.PN.DataCash.BasePage;
using System.Text;
using System.Configuration;

namespace Redecard.PN.DataCash.controles.comprovantes
{
    public partial class ComprovantePagamentoRecorrente : UserControlBaseDataCash
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!IsPostBack)
                {
                    base.AtualizaSession();

                    Modelo.VendaPagamentoRecorrente venda = (Modelo.VendaPagamentoRecorrente)Session["FacaSuaVenda"];

                    // Gera XML e verifica procesamento da DataCash
                    this.VerificarRetornoDataCash(venda);
                }
                else
                    Response.Redirect("FacaSuaVenda.aspx", true);
            }
        }

        /// <summary>
        /// Processa retorno da resposta do serviço da DataCash se houve sucesso ou erro
        /// </summary>
        /// <param name="dataCashXML">XML de envio para DataCash</param>
        private void VerificarRetornoDataCash(Modelo.VendaPagamentoRecorrente venda)
        {
            Negocio.Vendas transacao = new Negocio.Vendas();
            int codigoRetorno = 0;
            String mensagem = string.Empty;
            Modelo.TransacaoVenda retornoTransacao = null;

            if (venda.FormaRecorrencia == Modelo.enFormaRecorrencia.FireForget)
                retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaPagamentoRecorrenteFireForget), Request.UserHostAddress, out codigoRetorno, out mensagem) ;
            else if (venda.FormaRecorrencia == Modelo.enFormaRecorrencia.HistoricRecurring)
                retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaPagamentoRecorrenteHistoricRecurring), Request.UserHostAddress, out codigoRetorno, out mensagem) ;


            if (codigoRetorno != 1)
            {
                base.GeraPainelExcecao(mensagem, codigoRetorno.ToString());
            }
            else
            {
                this.ExibirComprovante((retornoTransacao as Modelo.TransacaoVendaPagamentoRecorrente));
            }

            Session.Remove("FacaSuaVenda");
        }

        /// <summary>
        /// Exibe o comprovante com as informações da venda em caso de processamento com sucesso realizado na DataCash
        /// </summary>
        private void ExibirComprovante(Modelo.TransacaoVendaPagamentoRecorrente transacao)
        {
  
            ltNSU.Text = transacao.NSU; //Construção DataCash
            ltTID.Text = transacao.TID;

            ltNumeroEstabelecimento.Text = this.SessaoAtual.CodigoEntidade.ToString();
            ltNomeEstabelecimento.Text = this.SessaoAtual.NomeEntidade.ToString();
            ltDataVenda.Text = DateTime.Now.ToString("dd/MM/yyyy");
            ltHoraVenda.Text = DateTime.Now.ToString("HH:mm");
            ltNumeroAutorizacao.Text = transacao.NumeroAutorizacao.ToString();
            ltTipoTransacao.Text = transacao.TipoTransacao.GetTitle();
            ltFormaRecorrencia.Text = transacao.FormaRecorrencia.GetTitle();
            ltBandeira.Text = transacao.DadosCartao.Bandeira.GetTitle();
            ltNomePortador.Text = transacao.DadosCartao.NomePortador;
            ltNumeroCartao.Text = transacao.DadosCartao.NumeroCriptografado;
            ltNumeroPedido.Text = transacao.NumeroPedido;

            if (transacao.FormaRecorrencia == Modelo.enFormaRecorrencia.FireForget)
            {
                Modelo.TransacaoVendaPagamentoRecorrenteFireForget transacaoFireForget = (Modelo.TransacaoVendaPagamentoRecorrenteFireForget)transacao;

                ltValorRecorrencia.Text = transacaoFireForget.ValorRecorrenciaFormatada;
                ltFrequencia.Text = transacaoFireForget.Frequencia;
                ltDataInicio.Text = transacaoFireForget.DataInicioFormatada;
                ltQtdRecorrencia.Text = transacaoFireForget.QuantidadeRecorencia;
                ltValorUltimaCobranca.Text = transacaoFireForget.ValorUltimaCobrancaFormatada;
                ltDataUltimaCobranca.Text = transacaoFireForget.DataUltimaCobrancaFormatada;

                trFireForget.Visible = true;
                trValor.Visible = false;
                trNumeroAutorizacao.Visible = false;
                trNSU.Visible = false;
            }
            else
            {
                Modelo.TransacaoVendaPagamentoRecorrenteHistoricRecurring transacaoHistoricRecurring = (Modelo.TransacaoVendaPagamentoRecorrenteHistoricRecurring)transacao;

                ltValor.Text = transacaoHistoricRecurring.ValorFormatado;

                trParcelas.Visible = false;
                trFireForget.Visible = false;
                trValor.Visible = true;

                trNumeroAutorizacao.Visible = true;
                trNSU.Visible = true;

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