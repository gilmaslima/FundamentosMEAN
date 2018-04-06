using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Redecard.PN.DataCash.Modelo.Util;
using Redecard.PN.DataCash.BasePage;
namespace Redecard.PN.DataCash.controles.confirmacao
{
    public partial class ConfirmacaoCreditoAVS : UserControlBaseDataCash
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

                        Modelo.Venda venda = (Modelo.Venda)Session["FacaSuaVenda"];

                        this.PreencherCampos(venda.TipoTransacao.GetTitle(),
                            venda.FormaPagamento.GetTitle(),
                            venda.DadosCartao.NomePortador,
                            venda.DadosCartao.Bandeira.GetTitle(),
                            venda.DadosCartao.Numero.Right(4),
                            venda.DadosCartao.Parcelas,
                            venda.ValorFormatado,
                            venda.NumeroPedido);

                        if (venda.FormaPagamento == Modelo.enFormaPagamento.Avista)
                        {
                            this.PreencherInfoTitular((venda as Modelo.VendaCreditoAVSAVista).InfoTitular);
                            trParcelas.Visible = false;
                        }
                        else if (venda.FormaPagamento == Modelo.enFormaPagamento.ParceladoEmissor)
                        {
                            this.PreencherInfoTitular((venda as Modelo.VendaCreditoAVSParceladoEmissor).InfoTitular);
                            trParcelas.Visible = true;
                        }
                        else if (venda.FormaPagamento == Modelo.enFormaPagamento.ParceladoEstabelecimento)
                        {
                            this.PreencherInfoTitular((venda as Modelo.VendaCreditoAVSParceladoEstabelecimento).InfoTitular);
                            trParcelas.Visible = true;
                        }

                        ucHeaderPassos.AtivarPasso(1);
                        Session.Add("PaginaComprovante", "FacaSuaVendaComprovanteCreditoAVS.aspx");

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
        private void PreencherInfoTitular(Modelo.InfoTitular infoTitular)
        {
            ltCPF.Text = infoTitular.CPF;
            ltCEP.Text = infoTitular.Endereco.CEP;
            ltEndereco.Text = infoTitular.Endereco.Logradouro;
            ltNumero.Text = infoTitular.Endereco.Numero;
            ltComplemento.Text = infoTitular.Endereco.Complemento;
            ltCidade.Text = infoTitular.Endereco.Cidade;
            ltEstado.Text = infoTitular.Endereco.Estado;
            //ltPais.Text = infoTitular.Endereco.Pais;
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
    }
}