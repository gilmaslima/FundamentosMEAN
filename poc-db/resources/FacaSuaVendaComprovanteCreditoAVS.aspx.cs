using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Redecard.PN.DataCash.BasePage;
using Redecard.PN.DataCash.controles.comprovantes;

using Redecard.PN.DataCash.Modelo.Util;
using Redecard.PN.DataCash.controles;

namespace Redecard.PN.DataCash
{
    public partial class FacaSuaVendaComprovanteCreditoAVS : PageBaseDataCash
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Redecard.PN.Comum.Logger Log = Redecard.PN.Comum.Logger.IniciarLog("Page_Load - Faça sua Venda"))
            {
                try
                {
                    if (Session["FacaSuaVenda"] != null)
                    {
                        //base.AtualizaSession();

                        Modelo.Venda venda = (Modelo.Venda)Session["FacaSuaVenda"];

                        ucHeaderPassos.AtivarPasso(2);

                        // Gera XML e verifica procesamento da DataCash
                        this.VerificarRetornoDataCash(venda);
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
                //exibe quadro de mensagem de erro

                qdAviso.Visible = true;
                qdAviso.CarregarMensagem("DataCashService.TransactionXMLPortal", codigoRetorno, "Atenção, transação não aprovada!", "~/FacaSuaVenda.aspx", QuadroAviso.TipoIcone.Erro);

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
            ltrNumeroPedido.Text = transacao.NumeroPedido;
            ltParcelas.Text = transacao.DadosCartao.Parcelas;

            if (transacao.FormaPagamento == Modelo.enFormaPagamento.Avista)
            {
                trParcelas.Visible = false;
                this.ExibirCamposJuros(false);
            }

            if (transacao.FormaPagamento == Modelo.enFormaPagamento.ParceladoEstabelecimento)
                this.ExibirCamposJuros(false);

            /* Alteração Release DataCash II - 20/03/2014 - Andre Rentes */
            if (transacao.FormaPagamento == Modelo.enFormaPagamento.ParceladoEmissor)
            {
                ltrCET.Text = ((Modelo.TransacaoVendaCreditoAVSParceladoEmissor)transacao).Taxa.CETFormatado;
                ltrTaxaJuros.Text = ((Modelo.TransacaoVendaCreditoAVSParceladoEmissor)transacao).Taxa.JurosFormatado;
                ltrValorParcelaJuros.Text = ((Modelo.TransacaoVendaCreditoAVSParceladoEmissor)transacao).Taxa.ValorParcelaJurosFormatado;
                ltrValorTotalJuros.Text = ((Modelo.TransacaoVendaCreditoAVSParceladoEmissor)transacao).Taxa.ValorTotalJurosFormatado;

                this.ExibirCamposJuros(true);
            }

            Session.Add("transacao", transacao);
            tbComprovante.Visible = true;
        }

        /// <summary>
        /// Método para exibir ou não os campos referentes a juros de compra parcelada.
        /// </summary>
        /// <param name="exibirCampos">Exibir campos?</param>
        private void ExibirCamposJuros(Boolean exibirCampos)
        {
            trCET.Visible = exibirCampos;
            trTaxaJuros.Visible = exibirCampos;
            trValorParcelaJuros.Visible = exibirCampos;
            trValorTotalJuros.Visible = exibirCampos;
        }

        /// <summary>
        /// Ação do botão voltar
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (Redecard.PN.Comum.Logger Log = Redecard.PN.Comum.Logger.IniciarLog("btnVoltar_Click - Faça sua Venda"))
            {
                try
                {
                    Response.Redirect("FacaSuaVenda.aspx");
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

        #region Obtenção de Pagina de Redirecionamento
        /// <summary>
        /// Página de redirecionamento do Lightbox
        /// </summary>
        /// <returns>String com o redirecionamento</returns>
        public String ObterPaginaRedirecionamento()
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
                    LarguraTabela = 60,
                    Registros = ObtemItensComprovante(transacao)
                };
            }
            else
                return null;
        }

        private IEnumerable<Object> ObtemItensComprovante(Modelo.TransacaoVenda transacao)
        {
            List<Tuple<String, String>> registros = new List<Tuple<string,string>>();

            using (Redecard.PN.Comum.Logger Log = Redecard.PN.Comum.Logger.IniciarLog("ObtemItensComprovante - Faça sua Venda"))
            {
                try
                {
                    registros.Add(new Tuple<String, String>("Nº do Comprovante de vendas (NSU)", transacao.NSU));
                    registros.Add(new Tuple<String, String>("TID", transacao.TID));
                    registros.Add(new Tuple<String, String>("N° Estabelecimento", this.SessaoAtual.CodigoEntidade.ToString()));
                    registros.Add(new Tuple<String, String>("Nome Estabelecimento", this.SessaoAtual.NomeEntidade.ToString()));
                    registros.Add(new Tuple<String, String>("Data da Venda", transacao.DataTransacao.ToString("dd/MM/yyyy")));
                    registros.Add(new Tuple<String, String>("Hora da Venda", transacao.DataTransacao.ToString("HH:mm")));
                    registros.Add(new Tuple<String, String>("N° Autorização", transacao.NumeroAutorizacao.ToString()));
                    registros.Add(new Tuple<String, String>("Tipo de Transação", transacao.TipoTransacao.GetTitle()));
                    registros.Add(new Tuple<String, String>("Forma de Pagamento", transacao.FormaPagamento.GetTitle()));
                    registros.Add(new Tuple<String, String>("Bandeira", transacao.DadosCartao.Bandeira.GetTitle()));
                    registros.Add(new Tuple<String, String>("Nome do Portador", transacao.DadosCartao.NomePortador));
                    registros.Add(new Tuple<String, String>("N° Cartão (Últimos 4 dig.)", transacao.DadosCartao.NumeroCriptografado));
                    registros.Add(new Tuple<String, String>("Valor", transacao.ValorFormatado));

                    if (transacao.FormaPagamento != Modelo.enFormaPagamento.Avista)
                        registros.Add(new Tuple<String, String>("N° Parcelas", transacao.DadosCartao.Parcelas));

                    registros.Add(new Tuple<String, String>("N° Pedido", transacao.NumeroPedido));
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

            return registros;
        }

        #endregion

    }
}