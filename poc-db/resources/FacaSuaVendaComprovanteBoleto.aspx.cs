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
using Redecard.PN.Comum;

namespace Redecard.PN.DataCash
{
    public partial class FacaSuaVendaComprovanteBoleto : PageBaseDataCash
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Page_Load - Faça sua Venda"))
            {
                try
                {
                    if (Session["FacaSuaVenda"] != null)
                    {
                        //base.AtualizaSession();

                        Modelo.Venda venda = (Modelo.Venda)Session["FacaSuaVenda"];

                        ucHeaderPassos.AtivarPasso(2);

                        // Gera XML e verifica procesamento da DataCash
                        this.VerificarRetornoDataCash((venda as Modelo.VendaBoleto));
                    }
                }
                catch (PortalRedecardException ex)
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
        private void VerificarRetornoDataCash(Modelo.VendaBoleto venda)
        {
            // Realiza chamada ao serviço DataCash
            Negocio.Vendas transacao = new Negocio.Vendas();
            int codigoRetorno = 0;
            String mensagem = string.Empty;

            Modelo.TransacaoVenda retornoTransacao =
                transacao.ExecutaTransacaoVenda((venda as Modelo.VendaBoleto), Request.UserHostAddress, out codigoRetorno, out mensagem);

            if (codigoRetorno != 1)
            {
                //exibe quadro de mensagem de erro
                qdAviso.Visible = true;
                qdAviso.CarregarMensagem("DataCashService.TransactionXMLPortal", codigoRetorno, "Atenção, transação não aprovada!", "~/FacaSuaVenda.aspx", QuadroAviso.TipoIcone.Erro);

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
            String url_popup = String.Format(
                "window.open('{0}','_blank','toolbar=0,menubar=0,directories=0,location=0,status=1,scrollbars=1,width=750,height=630');", url);

            this.ClientScript.RegisterClientScriptBlock(typeof(Page), "boleto", url_popup, true);
        }

        /// <summary>
        /// Exibe o comprovante com as informações da venda em caso de processamento com sucesso realizado na DataCash
        /// </summary>
        private void ExibirComprovante(Modelo.TransacaoVendaBoleto transacao)
        {

            ltTID.Text = transacao.TID;

            ltTipoTransacao.Text = transacao.TipoTransacao.ToString();
            ltTitulo.Text = transacao.DadosCliente.Titulo;
            ltNome.Text = transacao.DadosCliente.Nome;
            ltSobrenome.Text = transacao.DadosCliente.Sobrenome;
            ltEmail.Text = transacao.DadosCliente.Email;
            ltTelefone.Text = String.Format("({0}) {1}", transacao.DadosCliente.DDD, transacao.DadosCliente.Telefone);
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

        /// <summary>
        /// Ação do botão voltar
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("btnVoltar_Click - Faça sua Venda"))
            {
                try
                {
                    Response.Redirect("FacaSuaVenda.aspx");
                }
                catch (PortalRedecardException ex)
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
                    Registros = ObtemItensComprovante((transacao as Modelo.TransacaoVendaBoleto))
                };
            }
            else
                return null;
        }

        private IEnumerable<Object> ObtemItensComprovante(Modelo.TransacaoVendaBoleto transacao)
        {
            List<Tuple<String, String>> registros = new List<Tuple<string, string>>();
            using (Logger Log = Logger.IniciarLog("ObtemItensComprovante - Faça sua Venda"))
            {
                try
                {
                    registros.Add(new Tuple<String, String>("TID", transacao.TID));
                    registros.Add(new Tuple<String, String>("Tipo de Transação", transacao.TipoTransacao.GetTitle()));
                    registros.Add(new Tuple<String, String>("Título", transacao.DadosCliente.Titulo));
                    registros.Add(new Tuple<String, String>("Nome", transacao.DadosCliente.Nome));
                    registros.Add(new Tuple<String, String>("Sobrenome", transacao.DadosCliente.Sobrenome));
                    registros.Add(new Tuple<String, String>("E-mail", transacao.DadosCliente.Email));
                    registros.Add(new Tuple<String, String>("Telefone", String.Format("({0}) {1}", transacao.DadosCliente.DDD, transacao.DadosCliente.Telefone)));
                    registros.Add(new Tuple<String, String>("CEP", transacao.EnderecoCobranca.CEP));
                    registros.Add(new Tuple<String, String>("Endereço", transacao.EnderecoCobranca.Logradouro));
                    registros.Add(new Tuple<String, String>("Número", transacao.EnderecoCobranca.Numero));
                    registros.Add(new Tuple<String, String>("Complemento", transacao.EnderecoCobranca.Complemento));
                    registros.Add(new Tuple<String, String>("Cidade", transacao.EnderecoCobranca.Cidade));
                    registros.Add(new Tuple<String, String>("País", transacao.EnderecoCobranca.Pais));
                    registros.Add(new Tuple<String, String>("Valor", transacao.DadosPagamento.ValorFormatado));
                    registros.Add(new Tuple<String, String>("Data de Vencimento", transacao.DadosPagamento.DataVencimento));
                    registros.Add(new Tuple<String, String>("Multa de atraso", transacao.DadosPagamento.MultaAtrasoFormatado));
                    registros.Add(new Tuple<String, String>("Juros ao dia", transacao.DadosPagamento.JurosDiaFormatado));
                    registros.Add(new Tuple<String, String>("N° Pedido", transacao.DadosPagamento.NumeroPedido));
                    registros.Add(new Tuple<String, String>("Nota", transacao.DadosPagamento.Nota));

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                return registros;
            }
        }

        #endregion

    }
}