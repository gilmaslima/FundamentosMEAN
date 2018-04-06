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
    public partial class ConfirmacaoPagamentoRecorrente : UserControlBaseDataCash
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

                        Modelo.VendaPagamentoRecorrente venda = (Modelo.VendaPagamentoRecorrente)Session["FacaSuaVenda"];

                        this.PreencherCampos(venda.TipoTransacao.GetTitle(),
                            venda.FormaRecorrencia.GetTitle(),
                            venda.DadosCartao.NomePortador,
                            venda.DadosCartao.Bandeira.GetTitle(),
                            venda.DadosCartao.Numero.Right(4),
                            venda.NumeroPedido);

                        if (venda.FormaRecorrencia == Modelo.enFormaRecorrencia.FireForget)
                        {
                            Modelo.VendaPagamentoRecorrenteFireForget vendaFireForget = (Modelo.VendaPagamentoRecorrenteFireForget)venda;

                            ltValorRecorrencia.Text = vendaFireForget.ValorRecorrenciaFormatada;
                            ltFrequencia.Text = vendaFireForget.FrequenciaExibicao;
                            ltDataInicio.Text = vendaFireForget.DataInicioFormatada;
                            ltQtdRecorrencia.Text = vendaFireForget.QuantidadeRecorencia;
                            ltValorUltimaCobranca.Text = vendaFireForget.ValorUltimaCobrancaFormatada;
                            ltDataUltimaCobranca.Text = vendaFireForget.DataUltimaCobrancaFormatada;

                            trFireForget.Visible = true;
                            trValor.Visible = false;
                        }
                        else
                        {
                            Modelo.VendaPagamentoRecorrenteHistoricRecurring vendaHistoricRecurring = (Modelo.VendaPagamentoRecorrenteHistoricRecurring)venda;

                            ltValor.Text = vendaHistoricRecurring.ValorFormatado;

                            trFireForget.Visible = false;
                            trValor.Visible = true;
                        }
                        ucHeaderPassos.AtivarPasso(1);
                        Session.Add("PaginaComprovante", "FacaSuaVendaComprovantePagamentoRecorrente.aspx");

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
            String formaRecorrencia,
            String nomePortador,
            String bandeira,
            String numeroCartao,
            String pedido)
        {
            ltTipoTransacao.Text = tipoTransacao;
            ltFormaRecorrencia.Text = formaRecorrencia;
            ltBandeira.Text = bandeira;
            ltNomePortador.Text = nomePortador;
            ltNumeroCartao.Text = numeroCartao;
            ltPedido.Text = pedido;
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