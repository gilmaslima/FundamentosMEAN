using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Xml;
using System.Configuration;

using Redecard.PN.DataCash.Modelo.Util;
using Redecard.PN.DataCash.BasePage;

namespace Redecard.PN.DataCash.controles.comprovantes
{
    public partial class ComprovanteCredito : UserControlBaseDataCash
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                base.AtualizaSession();

                Modelo.Venda venda = (Modelo.Venda)Session["FacaSuaVenda"];

                ucHeaderPassos.AtivarPasso(2);

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
                retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaCreditoAVista), Request.UserHostAddress, out codigoRetorno, out mensagem);
            else if (venda.FormaPagamento == Modelo.enFormaPagamento.ParceladoEmissor)
                retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaCreditoParceladoEmissor), Request.UserHostAddress, out codigoRetorno, out mensagem);
            else if (venda.FormaPagamento == Modelo.enFormaPagamento.ParceladoEstabelecimento)
                retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaCreditoParceladoEstabelecimento), Request.UserHostAddress, out codigoRetorno, out mensagem);


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
            ltNSU.Text = transacao.NSU;
            ltTID.Text = transacao.TID;
            ltNumeroEstabelecimento.Text = this.SessaoAtual.CodigoEntidade.ToString();
            ltNomeEstabelecimento.Text = this.SessaoAtual.NomeEntidade.ToString();
            ltDataVenda.Text = DateTime.Now.ToString("dd/MM/yyyy");
            ltHoraVenda.Text = DateTime.Now.ToString("HH:mm");
            ltNumeroAutorizacao.Text = transacao.NumeroAutorizacao.ToString();
            ltTipoTransacao.Text = transacao.TipoTransacao.GetTitle();
            ltFormaPagamento.Text = transacao.FormaPagamento.GetTitle();
            ltBandeira.Text = transacao.DadosCartao.Bandeira.GetTitle();
            ltNomePortador.Text = transacao.DadosCartao.NomePortador;
            ltNumeroCartao.Text = transacao.DadosCartao.NumeroCriptografado;

            if (transacao.FormaPagamento != Modelo.enFormaPagamento.Avista)
                ltParcelas.Text = transacao.DadosCartao.Parcelas;

            ltValor.Text = transacao.ValorFormatado;
            ltNumeroPedido.Text = transacao.NumeroPedido;

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

        /// <summary>Obtenção dos dados da tabela para geração do PDF/Excel. 
        /// Realiza consulta completa do relatório, com todas as páginas</summary>
        /// <returns>Dados da tabela</returns>
        protected TabelaExportacao ucAcoes_ObterTabelaExportacao()
        {
            Modelo.TransacaoVenda transacao = (Session["transacao"] as Modelo.TransacaoVenda);

            if (transacao != null)
            {
                return new TabelaExportacao
                {
                    Titulo = String.Format("Faça sua venda aqui - Comprovante ({0} - {1})",
                        SessaoAtual.NomeEntidade, SessaoAtual.CodigoEntidade),
                    Colunas = new[] 
                {
                    new Coluna("Descrição").SetAlinhamento(HorizontalAlign.Right),
                    new Coluna("Valor").SetAlinhamento(HorizontalAlign.Left).SetBordaInterna(false)
                },
                    FuncaoValores = (registro) =>
                    {
                        var item = registro as Tuple<String, String>;
                        return new Object[] {
                        item.Item1,
                        item.Item2
                    };
                    },
                    ExibirTituloColunas = false,
                    ModoRetrato = true,
                    LarguraTabela = 30,
                    Registros = new List<Tuple<String, String>>(new[] {

                    new Tuple<String, String>("Nº do Comprovante de vendas (NSU)", transacao.NSU),
                    new Tuple<String, String>("TID", transacao.TID),
                    new Tuple<String, String>("N° Estabelecimento", this.SessaoAtual.CodigoEntidade.ToString()),
                    new Tuple<String, String>("Nome Estabelecimento", this.SessaoAtual.NomeEntidade.ToString()),
                    new Tuple<String, String>("Data da Venda", transacao.DataTransacao.ToString("dd/MM/yyyy")),
                    new Tuple<String, String>("Hora da Venda", transacao.DataTransacao.ToString("HH:mm")),
                    new Tuple<String, String>("N° Autorização", transacao.NumeroAutorizacao.ToString()),
                    new Tuple<String, String>("Tipo de Transação", transacao.TipoTransacao.GetTitle()),
                    new Tuple<String, String>("Forma de Pagamento", transacao.FormaPagamento.GetTitle()),
                    new Tuple<String, String>("Bandeira", transacao.DadosCartao.Bandeira.GetTitle()),
                    new Tuple<String, String>("Nome do Portador", transacao.DadosCartao.NomePortador),
                    new Tuple<String, String>("N° Cartão (ùltimos 4 dig.)", transacao.DadosCartao.NumeroCriptografado),

                    //if (transacao.FormaPagamento != Modelo.enFormaPagamento.Avista)
                    //    new Tuple<String, String>("TID", transacao.DadosCartao.Parcelas),

                    new Tuple<String, String>("Valor", transacao.ValorFormatado),
                    new Tuple<String, String>("N° Pedido", transacao.NumeroPedido)})

                    //if (transacao.FormaPagamento != Modelo.enFormaPagamento.Avista)
                    //{
                    //    //trParcelas.Visible = false;
                    //    //ucParceladoEmissor.Visible = false;
                    //}
                    //else if (transacao.FormaPagamento == Modelo.enFormaPagamento.ParceladoEstabelecimento)
                    //{
                    //    ucParceladoEmissor.Visible = false;
                    //}
                    //else if (transacao.FormaPagamento == Modelo.enFormaPagamento.ParceladoEmissor)
                    //{
                    //    ucParceladoEmissor.ValorParcela = 0;
                    //    ucParceladoEmissor.Encargos = 0;
                    //    ucParceladoEmissor.ValorTotalPagar = 0;
                    //}
                };
            }
            else
                return null;

        }

        private IEnumerable<Object> ObterDadosTransacao(Modelo.TransacaoVenda transacao)
        {
            return null;
        }



        #endregion
    }
}