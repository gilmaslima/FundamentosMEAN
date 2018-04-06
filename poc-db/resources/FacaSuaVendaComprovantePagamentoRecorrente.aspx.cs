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
    public partial class FacaSuaVendaComprovantePagamentoRecorrente : PageBaseDataCash
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
                        this.VerificarRetornoDataCash((venda as Modelo.VendaPagamentoRecorrente));
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
        private void VerificarRetornoDataCash(Modelo.VendaPagamentoRecorrente venda)
        {
            Negocio.Vendas transacao = new Negocio.Vendas();
            int codigoRetorno = 0;
            String mensagem = string.Empty;
            Modelo.TransacaoVenda retornoTransacao = null;

            if (venda.FormaRecorrencia == Modelo.enFormaRecorrencia.FireForget)
                retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaPagamentoRecorrenteFireForget), Request.UserHostAddress, out codigoRetorno, out mensagem);
            else if (venda.FormaRecorrencia == Modelo.enFormaRecorrencia.HistoricRecurring)
                retornoTransacao = transacao.ExecutaTransacaoVenda((venda as Modelo.VendaPagamentoRecorrenteHistoricRecurring), Request.UserHostAddress, out codigoRetorno, out mensagem);


            if (codigoRetorno != 1)
            {
                //exibe quadro de mensagem de erro
                qdAviso.Visible = true;
                qdAviso.CarregarMensagem("DataCashService.TransactionXMLPortal", codigoRetorno, "Atenção, transação não aprovada!", "~/FacaSuaVenda.aspx", QuadroAviso.TipoIcone.Erro);
            }
            else
            {
                this.ExibirComprovante((retornoTransacao as Modelo.TransacaoVendaPagamentoRecorrente));
            }

            //base.UltimaTransacaoRealizada = String.Format("{0};{1}",
            //    retornoTransacao.TipoTransacao.GetTitle(), venda.FormaRecorrencia.GetTitle());


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

                trNumeroAutorizacao.Visible = false;
                trNSU.Visible = false;
                trFireForget.Visible = true;
                trValor.Visible = false;
            }
            else
            {
                Modelo.TransacaoVendaPagamentoRecorrenteHistoricRecurring transacaoHistoricRecurring = (Modelo.TransacaoVendaPagamentoRecorrenteHistoricRecurring)transacao;

                ltNumeroAutorizacao.Text = transacao.NumeroAutorizacao;
                ltValor.Text = transacaoHistoricRecurring.ValorFormatado;

                trNumeroAutorizacao.Visible = true;
                trNSU.Visible = true;
                trFireForget.Visible = false;
                trValor.Visible = true;
            }

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

        #endregion

        /// <summary>Obtenção dos dados da tabela para geração do PDF/Excel. 
        /// Realiza consulta completa do relatório, com todas as páginas</summary>
        /// <returns>Dados da tabela</returns>
        protected TabelaExportacao ucAcoes_ObterTabelaExportacao()
        {
            Modelo.TransacaoVendaPagamentoRecorrente transacao = (Session["transacao"] as Modelo.TransacaoVendaPagamentoRecorrente);

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

        private IEnumerable<Object> ObtemItensComprovante(Modelo.TransacaoVendaPagamentoRecorrente transacao)
        {
            List<Tuple<String, String>> registros = new List<Tuple<string,string>>();

            if (transacao.FormaRecorrencia == Modelo.enFormaRecorrencia.HistoricRecurring)
            {
                registros.Add(new Tuple<String, String>("Nº do Comprovante de vendas (NSU)", transacao.NSU));
            }
            registros.Add(new Tuple<String, String>("TID", transacao.TID));
            registros.Add(new Tuple<String, String>("N° Estabelecimento", this.SessaoAtual.CodigoEntidade.ToString()));
            registros.Add(new Tuple<String, String>("Nome Estabelecimento", this.SessaoAtual.NomeEntidade.ToString()));
            registros.Add(new Tuple<String, String>("Data da Venda", transacao.DataTransacao.ToString("dd/MM/yyyy")));
            registros.Add(new Tuple<String, String>("Hora da Venda", transacao.DataTransacao.ToString("HH:mm")));
            registros.Add(new Tuple<String, String>("Tipo de Transação", transacao.TipoTransacao.GetTitle()));
            registros.Add(new Tuple<String, String>("Forma de Recorrência", transacao.FormaRecorrencia.GetTitle()));
            registros.Add(new Tuple<String, String>("Bandeira", transacao.DadosCartao.Bandeira.GetTitle()));
            registros.Add(new Tuple<String, String>("Nome do Portador", transacao.DadosCartao.NomePortador));
            registros.Add(new Tuple<String, String>("N° Cartão (Últimos 4 dig.)", transacao.DadosCartao.NumeroCriptografado));

            if (transacao.FormaRecorrencia == Modelo.enFormaRecorrencia.FireForget)
            {
                Modelo.TransacaoVendaPagamentoRecorrenteFireForget transacaoFireForget = (Modelo.TransacaoVendaPagamentoRecorrenteFireForget)transacao;

                registros.Add(new Tuple<String, String>("N° Pedido", transacao.NumeroPedido));
                registros.Add(new Tuple<String, String>("Valor da Recorrência", transacaoFireForget.ValorRecorrenciaFormatada));
                registros.Add(new Tuple<String, String>("Frequência", transacaoFireForget.Frequencia));
                registros.Add(new Tuple<String, String>("Data de início", transacaoFireForget.DataInicioFormatada));
                registros.Add(new Tuple<String, String>("Qtde. de recorrências", transacaoFireForget.QuantidadeRecorencia));
                registros.Add(new Tuple<String, String>("Valor da última cobrança", transacaoFireForget.ValorUltimaCobrancaFormatada));
                registros.Add(new Tuple<String, String>("Data da cobrança", transacaoFireForget.DataUltimaCobrancaFormatada));
            }
            else
            {
                Modelo.TransacaoVendaPagamentoRecorrenteHistoricRecurring transacaoHistoricRecurring = (Modelo.TransacaoVendaPagamentoRecorrenteHistoricRecurring)transacao;

                registros.Add(new Tuple<String, String>("Valor", transacaoHistoricRecurring.ValorFormatado));
                registros.Add(new Tuple<String, String>("N° Pedido", transacao.NumeroPedido));

            }            
 
            return registros;
        }

    }
}