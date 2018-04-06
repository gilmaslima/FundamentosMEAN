using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Text;
using System.Configuration;

using Redecard.PN.DataCash.Modelo.Util;
using Redecard.PN.DataCash.BasePage;

namespace Redecard.PN.DataCash.controles.comprovantes
{
    public partial class ConfirmacaoBoleto : UserControlBaseDataCash
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                base.AtualizaSession();

                Modelo.VendaBoleto venda = (Modelo.VendaBoleto)Session["FacaSuaVenda"];

                // Gera XML e verifica procesamento da DataCash
                this.VerificarRetornoDataCash(venda);
            }
            else
                Response.Redirect("FacaSuaVenda.aspx", true);
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

            retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaBoleto), Request.UserHostAddress, out codigoRetorno, out mensagem);

            if (codigoRetorno != 1)
            {
                base.GeraPainelExcecao(mensagem, codigoRetorno.ToString());
            }
            else
            {
                this.ExibirComprovante((retornoTransacao as Modelo.TransacaoVendaBoleto));
                this.ExibirBoleto((retornoTransacao as Modelo.TransacaoVendaBoleto).URL);
            }

            Session.Remove("FacaSuaVenda");
        }
        /// <summary>
        /// Exibe o boleto gerado pela transação
        /// </summary>
        private void ExibirBoleto(String url)
        {
            Response.Write(string.Format("<script>window.open('{0}','_blank','toolbar=0,menubar=0,directories=0,location=0,status=1,scrollbars=1,width=750,height=630');</script>", url));
        }

        /// <summary>
        /// Ação do botão voltar
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("FacaSuaVenda.aspx");
        }

        /// <summary>
        /// Exibe o comprovante com as informações da venda em caso de processamento com sucesso realizado na DataCash
        /// </summary>
        private void ExibirComprovante(Modelo.TransacaoVendaBoleto transacao)
        {
            ltNSU.Text =  transacao.NSU; //Construção DataCash
            ltTID.Text = transacao.TID;

            ltTipoTransacao.Text = transacao.TipoTransacao.ToString();
            ltTitulo.Text = transacao.DadosCliente.Titulo;
            ltNome.Text = transacao.DadosCliente.Nome;
            ltSobrenome.Text = transacao.DadosCliente.Sobrenome;
            ltEmail.Text = transacao.DadosCliente.Email;
            ltTelefone.Text = transacao.DadosCliente.Telefone;
            ltCEP.Text = transacao.EnderecoCobranca.CEP;
            ltEndereco.Text = transacao.EnderecoCobranca.Logradouro;
            ltNumero.Text = transacao.EnderecoCobranca.Numero;
            ltComplemento.Text = transacao.EnderecoCobranca.Complemento;
            ltCidade.Text = transacao.EnderecoCobranca.Cidade;
            ltEstado.Text = transacao.EnderecoCobranca.Estado;
            ltValor.Text = transacao.DadosPagamento.ValorFormatado;
            ltDataVencimento.Text = transacao.DadosPagamento.DataVencimento;
            ltMultaAtraso.Text = transacao.DadosPagamento.MultaAtrasoFormatado;
            ltJurosDia.Text = transacao.DadosPagamento.JurosDiaFormatado;
            ltNota.Text = transacao.DadosPagamento.Nota;
            ltNumeroPedido.Text = transacao.DadosPagamento.NumeroPedido;

            Session.Add("transacao", transacao);

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