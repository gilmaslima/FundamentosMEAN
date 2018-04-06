using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Redecard.PN.DataCash.Modelo.Util;
using Redecard.PN.DataCash.BasePage;
using Redecard.PN.DataCash.Modelo;
using System.Configuration;

namespace Redecard.PN.DataCash.controles
{
    public partial class ConfirmacaoBoleto : UserControlBaseDataCash
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

                        Modelo.VendaBoleto venda = (Modelo.VendaBoleto)Session["FacaSuaVenda"];

                        this.PreencherCampos(venda.TipoTransacao.GetTitle(),
                            venda.DadosCliente
                            , venda.EnderecoCobranca
                            , venda.DadosPagamento);

                        ucHeaderPassos.AtivarPasso(1);
                        Session.Add("PaginaComprovante", "FacaSuaVendaComprovanteBoleto.aspx");

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
            DadosCliente dadosCliente,
            Endereco dadosCobranca,
            DadosPagamento dadosPagamento
            )
        {
            ltTipoTransacao.Text = tipoTransacao;
            ltTitulo.Text = dadosCliente.Titulo;
            ltNome.Text = dadosCliente.Nome;
            ltSobrenome.Text = dadosCliente.Sobrenome;
            ltEmail.Text = dadosCliente.Email;
            ltTelefone.Text = String.Format("({0}) {1}", dadosCliente.DDD, dadosCliente.Telefone);
            ltCEP.Text = dadosCobranca.CEP;
            ltEndereco.Text = dadosCobranca.Logradouro;
            ltNumero.Text = dadosCobranca.Numero;
            ltComplemento.Text = dadosCobranca.Complemento;
            ltCidade.Text = dadosCobranca.Cidade;
            ltEstado.Text = dadosCobranca.Estado;
            ltValor.Text = dadosPagamento.ValorFormatado;
            ltDataVencimento.Text = dadosPagamento.DataVencimento;
            ltMultaAtraso.Text = dadosPagamento.MultaAtrasoFormatado;
            ltJurosDia.Text = dadosPagamento.JurosDiaFormatado;
            ltNumeroPedido.Text = dadosPagamento.NumeroPedido;
            ltNota.Text = dadosPagamento.Nota;

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