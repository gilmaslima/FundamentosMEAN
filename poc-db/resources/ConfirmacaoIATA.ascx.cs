using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Redecard.PN.DataCash.Modelo.Util;
using Redecard.PN.DataCash.BasePage;
using System.Web.UI.HtmlControls;

namespace Redecard.PN.DataCash.controles.confirmacao
{
    public partial class ConfirmacaoIATA : UserControlBaseDataCash
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Redecard.PN.Comum.Logger Log = Redecard.PN.Comum.Logger.IniciarLog("Page_Load - Faça sua Venda"))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        base.AtualizaSession();

                        Modelo.VendaPagamentoIATA venda = (Session["FacaSuaVenda"] as Modelo.VendaPagamentoIATA);

                        this.PreencherCampos(venda.TipoTransacao.GetTitle(),
                            venda.FormaPagamento.GetTitle(),
                            venda.DadosCartao.NomePortador,
                            venda.DadosCartao.Bandeira.GetTitle(),
                            venda.DadosCartao.Numero.Right(4),
                            venda.DadosCartao.Parcelas,
                            venda.ValorFormatado,
                            venda.NumeroPedido);

                        PreencheCamposIATA(venda);

                        if (venda.FormaPagamento == Modelo.enFormaPagamento.Avista)
                        {
                            trParcelas.Visible = false;
                            //trValorTotal.Visible = true;
                        }
                        else
                        {
                            trParcelas.Visible = true;
                            //trValorTotal.Visible = false;
                        }
                        ucHeaderPassos.AtivarPasso(1);
                        Session.Add("PaginaComprovante", "FacaSuaVendaComprovanteIATA.aspx");

                    }
                }
                catch (Redecard.PN.Comum.PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Preenche os campos da tela de confirmação sem forma de pagamento
        /// </summary>
        private void PreencherCampos(String tipoTransacao,
            String formaPagamento,
            String nomePortador,
            String bandeira,
            String numeroCartao,
            String parcelas,
            String valor,
            String pedido)
        {
            ltTipoTransacao.Text = tipoTransacao;
            ltFormaPagamento.Text = formaPagamento;
            ltBandeira.Text = bandeira;
            ltNomePortador.Text = nomePortador;
            ltNumeroCartao.Text = numeroCartao;
            ltValor.Text = valor;
            ltPedido.Text = pedido;
            ltParcelas.Text = parcelas;
        }

        /// <summary>
        /// Preenche os campos referentes à IATA
        /// </summary>
        /// <param name="venda"></param>
        private void PreencheCamposIATA(Modelo.VendaPagamentoIATA venda)
        {
            ltNomeAgencia.Text = venda.NomeAgenciaViagem;
            ltCodigoIATA.Text = venda.Codigo.ToString();
            ltCodigoCompanhia.Text = venda.CodigoCompanhia;
            ltClasse.Text = venda.ClasseDescricao;
            ltAeroportoPartida.Text = venda.CodigoAeroportoPartida;
            ltDataPartida.Text = venda.DataPartida;
            ltFusoHorario.Text = venda.FusoHorarioPartidaDescricao;
            ltAeroportoDestino.Text = venda.CodigoAeroportoDestino;
            ltTaxaEmbarque.Text = venda.TaxaEmbarqueFormatada;
            ltValorTotalPagar.Text = venda.TotalaPagarFormatada;

            rptPassageiros.DataSource = venda.Passageiros;
            rptPassageiros.DataBind();

        }
        #region Obtenção de Pagina de Redirecionamento
        /// <summary>
        /// Página de redirecionamento do Lightbox
        /// </summary>
        /// <returns>String com o redirecionamento</returns>
        public override String ObterPaginaRedirecionamento()
        {
            return "#";
        }
        #endregion

        protected void rptPassageiros_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Modelo.Passageiro passageiro = e.Item.DataItem as Modelo.Passageiro;

                Modelo.VendaPagamentoIATA venda = (Modelo.VendaPagamentoIATA)Session["FacaSuaVenda"];

                Literal ltlLabelPassageiro = (e.Item.FindControl("ltlLabelPassageiro") as Literal);
                Literal ltlPassageiro = (e.Item.FindControl("ltlPassageiro") as Literal);
                Literal ltlLabelCodRefPassageiro = (e.Item.FindControl("ltlLabelCodRefPassageiro") as Literal);
                Literal ltlCodRefPassageiro = (e.Item.FindControl("ltlCodRefPassageiro") as Literal);
                Literal ltlLabelNumeroBilhete = (e.Item.FindControl("ltlLabelNumeroBilhete") as Literal);
                Literal ltlNumeroBilhete = (e.Item.FindControl("ltlNumeroBilhete") as Literal);

                ltlLabelPassageiro.Text = string.Format(ltlLabelPassageiro.Text, venda.Passageiros.IndexOf(passageiro) + 1);
                ltlPassageiro.Text = passageiro.Nome;
                ltlLabelCodRefPassageiro.Text = string.Format(ltlLabelCodRefPassageiro.Text, venda.Passageiros.IndexOf(passageiro) + 1);
                ltlCodRefPassageiro.Text = passageiro.CodigoReferencia;
                ltlLabelNumeroBilhete.Text = string.Format(ltlLabelNumeroBilhete.Text, venda.Passageiros.IndexOf(passageiro) + 1);
                ltlNumeroBilhete.Text = passageiro.NumeroBilhete;
            }
        }
    }
}