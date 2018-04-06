using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.DataCash.BasePage;
using Redecard.PN.DataCash.Modelo;
using System.Data;
using Redecard.PN.DataCash.controles.comprovantes;
using Redecard.PN.Comum;

namespace Redecard.PN.DataCash
{
    public partial class GerenciamentoVendasEstorno : PageBaseDataCash
    {
        #region [ Propriedades ]

        private Modelos.DadosTransacao DadosTransacao
        {
            get { return Util.DeserializarDado<Modelos.DadosTransacao>((Byte[])ViewState["DadosTransacao"]); }
            set { ViewState["DadosTransacao"] = Util.SerializarDado(value); }
        }

        #endregion

        /// <summary>
        /// Carrega página.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Page_Load - Faça sua Venda"))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        lblDataTransacao.Text = DateTime.Now.ToString("dd/MM/yyyy");
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
        /// Volta para a página inicial do Gerenciamento de Vendas - Estorno
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("btnVoltar_Click - Faça sua Venda"))
            {
                try
                {
                    base.Redirecionar("GerVendasEstorno.aspx");
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
        /// Volta para a página inicial do Gerenciamento de Vendas - Estorno
        /// </summary>
        protected void btnVoltarConfirmacao_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("btnVoltar_Click - Faça sua Venda"))
            {
                try
                {
                    ucAssistente.Voltar();
                    mvEstorno.SetActiveView(vwInicial);
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
        /// Evento do botão continuar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Estorno - Continuar"))
            {
                try
                {
                    Int32 pv = this.SessaoAtual.CodigoEntidade;
                    String tid = txtTID.Text;

                    this.DadosTransacao = new Negocio.Gerenciamento().GetDadosTransacao(tid, pv);

                    if (this.DadosTransacao.CodigoRetorno != 1)
                    {
                        base.ExibirPainelExcecao("DataCash.Negocio.Gerenciamento.GetDadosTransacao",
                            this.DadosTransacao.CodigoRetorno);
                    }
                    else
                    {
                        ucAssistente.Avancar();
                        CarregaControlesConfirmacao(this.DadosTransacao, txtTID.Text);
                        mvEstorno.SetActiveView(vwConfirmacao);
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
        /// Evento do botão confirmar estorno. Chama o serviço de transação com o datacash.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Estorno - Continuar"))
            {
                try
                {
                    Int32 numeroPV = this.SessaoAtual.CodigoEntidade;
                    String tid = txtTID.Text;
                    Modelos.DadosTransacao transacao = this.DadosTransacao;

                    Modelo.RetornoTransacaoXML retorno = new Negocio.Gerenciamento().ExecutarEstorno(numeroPV, tid);

                    ucAssistente.Avancar();
                    CarregaControlesComprovante(transacao, retorno);
                    mvEstorno.SetActiveView(vwComprovante);
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
        /// Carrega as informações da tabela de confirmação.
        /// </summary>
        protected void CarregaControlesConfirmacao(Modelos.DadosTransacao transacao, String tid)
        {
            ltNSU.Text = transacao.NSU;
            ltTID.Text = tid;
            ltTipoTransacao.Text = transacao.TipoTransacao;
            ltNroAutorizacao.Text = transacao.AuthCode;
            ltDataTransacao.Text = transacao.DataPreAutorizacao.ToString("dd/MM/yyyy");
            ltValorTransacao.Text = transacao.Valor.ToString("C2", ptBR);
        }

        /// <summary>
        /// Carrega as informações da tabela de comprovante.
        /// </summary>
        protected void CarregaControlesComprovante(Modelos.DadosTransacao transacao, Modelo.RetornoTransacaoXML retornoEstorno)
        {
            ltNSUComprovante.Text = transacao.NSU;
            ltTIDComprovante.Text = retornoEstorno.GatewayReference;
            ltTipoTransacaoComprovante.Text = transacao.TipoTransacao;
            ltNroAutorizacaoComprovante.Text = transacao.AuthCode;
            ltDataTransacaoComprovante.Text = transacao.DataPreAutorizacao.ToString("dd/MM/yyyy");
            ltValorTransacaoComprovante.Text = transacao.Valor.ToString("C2", ptBR);            
        }

        /// <summary>Obtenção dos dados da tabela para geração do PDF/Excel.</summary>
        /// <returns>Dados da tabela</returns>
        protected TabelaExportacao ucAcoes_ObterTabelaExportacao()
        {
            return new TabelaExportacao
            {
                Titulo = String.Format("Estorno de Vendas - Comprovante ({0} - {1})",
                    SessaoAtual.NomeEntidade, SessaoAtual.CodigoEntidade),
                Colunas = new[] 
                {
                    new Coluna("Descrição").SetAlinhamento(HorizontalAlign.Right),
                    new Coluna("Valor").SetAlinhamento(HorizontalAlign.Left).SetBordaInterna(false)
                },
                FuncaoValores = (registro) =>
                {
                    var item = registro as Tuple<String,String>;
                    return new Object[] {
                        item.Item1,
                        item.Item2
                    };
                },
                ExibirTituloColunas = false,
                ModoRetrato = true,
                LarguraTabela = 30,
                Registros = new List<Tuple<String,String>>(new [] {
                    new Tuple<String, String>("Nº do Comprovante de vendas (NSU)", this.DadosTransacao.NSU),
                    new Tuple<String, String>("TID", txtTID.Text),
                    new Tuple<String, String>("Tipo de Transação", this.DadosTransacao.TipoTransacao),
                    new Tuple<String, String>("Nº de autorização", this.DadosTransacao.AuthCode),
                    new Tuple<String, String>("Data da Transação", this.DadosTransacao.DataPreAutorizacao.ToString("dd/MM/yyyy")),
                    new Tuple<String, String>("Valor da Transação", this.DadosTransacao.Valor.ToString("C2", ptBR))})
            };
        }
    }
}