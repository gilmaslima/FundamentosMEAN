using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.DataCash.BasePage;
using System.Data;
using Redecard.PN.DataCash.Modelos;
using Redecard.PN.Comum;
using Redecard.PN.DataCash.controles.comprovantes;

namespace Redecard.PN.DataCash
{
    public partial class GerenciamentoVendasPreAutorizacaoCancelar : PageBaseDataCash
    {
        #region [ Propriedades ]

        private Modelos.DadosTransacao DadosTransacao
        {
            get { return Util.DeserializarDado<Modelos.DadosTransacao>((Byte[])ViewState["DadosTransacao"]); }
            set { ViewState["DadosTransacao"] = Util.SerializarDado(value); }
        }

        private Modelo.RetornoTransacaoXML Retorno
        {
            get { return (Modelo.RetornoTransacaoXML)ViewState["Retorno"]; }
            set { ViewState["Retorno"] = value; }
        }

        #endregion

        /// <summary>
        /// Carrega a página.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblDataTransacao.Text = DateTime.Now.ToString("dd/MM/yyyy");
            }
        }

        /// <summary>
        /// Volta para a página inicial do Gerenciamento de Vendas - Pré-Autorização - Cancelar Confirmação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            base.Redirecionar("GerVendasPreAutorizacaoCancelar.aspx");
        }

        /// <summary>
        /// Volta para a página inicial do Gerenciamento de Vendas - Pré-Autorização - Cancelar Confirmação
        /// </summary>
        protected void btnVoltarConfirmacao_Click(object sender, EventArgs e)
        {
            ucAssistente.Voltar();
            mvwPreAutorizacaoCancelar.SetActiveView(vwInicial);
        }

        /// <summary>
        /// Evento do botão continuar.
        /// </summary>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Pré-Autorização - Cancelar Confirmação - Continuar"))
            {
                try
                {
                    String tid = txtTID.Text;
                    Int32 pv = this.SessaoAtual.CodigoEntidade;
                    String dataTransacao = lblDataTransacao.Text;

                    this.DadosTransacao = new Negocio.Gerenciamento().GetDadosTransacao(tid, pv);

                    if (this.DadosTransacao.CodigoRetorno != 1)
                    {
                        base.ExibirPainelExcecao("DataCash.Negocio.Gerenciamento.GetDadosTransacao",
                            this.DadosTransacao.CodigoRetorno);
                    }
                    else
                    {
                        ltDataTransacaoConfirmacao.Text = dataTransacao;
                        ltTIDConfirmacao.Text = tid;
                        ltValorTransacaoConfirmacao.Text = this.DadosTransacao.ValorPreAutorizacao.ToString("C2", ptBR);

                        ucAssistente.Avancar();
                        mvwPreAutorizacaoCancelar.SetActiveView(vwConfirmacao);
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
        /// Evento do botão confirmar. Chama o serviço de transação com o datacash.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Pré-Autorização - Cancelar Confirmação - Confirmar"))
            {
                try
                {
                    Int32 pv = this.SessaoAtual.CodigoEntidade;
                    String tid = txtTID.Text;

                    this.Retorno = new Negocio.Gerenciamento().ExecutarCancelarConfirmacao(pv, tid);
                    
                    ltNSUComprovante.Text = this.DadosTransacao.NSU;
                    ltTIDComprovante.Text = this.Retorno.GatewayReference;
                    ltNroEstabelecimentoComprovante.Text = this.SessaoAtual.CodigoEntidade.ToString();
                    ltNomeEstabelecimentoComprovante.Text = this.SessaoAtual.NomeEntidade;
                    ltNroAutorizacaoComprovante.Text = this.DadosTransacao.AuthCode;
                    ltDataConfirmacaoComprovante.Text = this.DadosTransacao.DataConfimacao.ToString("dd/MM/yyyy");
                    ltHoraConfirmacaoComprovante.Text = this.DadosTransacao.HoraConfimacao;
                    ltValorTransacaoComprovante.Text = this.DadosTransacao.ValorPreAutorizacao.ToString("C2", ptBR);
                    ltNroCartaoComprovante.Text = this.DadosTransacao.NumeroCartao.ToString().PadLeft(4, '0').Right(4);
                    ltDataCancelamentoComprovante.Text = this.Retorno.Time.HasValue ? this.Retorno.Time.Value.ToString("dd/MM/yyyy") : "-";
                    ltHoraCancelamentoComprovante.Text = this.Retorno.Time.HasValue ? this.Retorno.Time.Value.ToString("HH:mm") : "-";

                    ucAssistente.Avancar();
                    mvwPreAutorizacaoCancelar.SetActiveView(vwComprovante);
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

        /// <summary>Obtenção dos dados da tabela para geração do PDF/Excel.</summary>
        /// <returns>Dados da tabela</returns>
        protected TabelaExportacao ucAcoes_ObterTabelaExportacao()
        {
            return new TabelaExportacao
            {
                Titulo = String.Format("Cancelar Confirmação - Comprovante ({0} - {1})",
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
                LarguraTabela = 35,
                Registros = new List<Tuple<String, String>>(new[] {
                    new Tuple<String, String>("Nº do Comprovante de vendas (NSU)", this.DadosTransacao.NSU),
                    new Tuple<String, String>("TID", txtTID.Text),
                    new Tuple<String, String>("Nº Estabelecimento", this.SessaoAtual.CodigoEntidade.ToString()),
                    new Tuple<String, String>("Nome do Estabelecimento", this.SessaoAtual.NomeEntidade),
                    new Tuple<String, String>("Nº Autorização", this.DadosTransacao.AuthCode),
                    new Tuple<String, String>("Data da Confirmação", this.DadosTransacao.DataConfimacao.ToString("dd/MM/yyyy")),
                    new Tuple<String, String>("Hora da Confirmação", this.DadosTransacao.HoraConfimacao),
                    new Tuple<String, String>("Valor da Transação", this.DadosTransacao.ValorPreAutorizacao.ToString("C2", ptBR)),
                    new Tuple<String, String>("Nº do cartão (Últimos 4 dig.)", 
                        this.DadosTransacao.NumeroCartao.ToString().PadLeft(4, '0').Right(4)),
                    new Tuple<String, String>("Data do Cancelamento", 
                        this.Retorno.Time.HasValue ? this.Retorno.Time.Value.ToString("dd/MM/yyyy") : "-"),
                    new Tuple<String, String>("Hora do Cancelamento", 
                        this.Retorno.Time.HasValue ? this.Retorno.Time.Value.ToString("HH:mm") : "-")
                })
            };
        }
    }
}