using Rede.PN.Cancelamento.Core.Web.Controles.Portal;
using Rede.PN.Cancelamento.Sharepoint.CancelamentoServico;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rede.PN.Cancelamento.Sharepoint.WebParts.ConsultarCancelamento
{
    /// <summary>
    /// Controle de consultar cancelamentos
    /// </summary>
    public partial class ConsultarCancelamentoUserControl : UserControlBase
    {
        #region [ Eventos ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Consultar Cancelamento - Page Load"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        
                    }
                }
                catch (FaultException<CancelamentoServico.GeneralFault> fe)
                {
                    Logger.GravarErro("Consultar Cancelamento - Page Load", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Consultar Cancelamento - Page Load", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento do botão buscar
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Consultar Cancelamento - Buscar"))
            {
                try
                {
                    Page.Validate();
                    if (Page.IsValid)
                    {
                        List<SolicitacaoCancelamento> solicitacoesCancelamento = new List<SolicitacaoCancelamento>();

                        if(ddlStatus.SelectedValue.ToLowerInvariant().Equals("efetivados"))
                            solicitacoesCancelamento = GetSolicitacoesCancelamentoEfetivados();
                        else
                            solicitacoesCancelamento = GetSolicitacoesCancelamentoNaoEfetivados();

                        MostraControles(solicitacoesCancelamento.Count);
                        if (solicitacoesCancelamento.Count > 0)
                            CarregaTabelaSolicitacoesCancelamento(solicitacoesCancelamento);
                        else
                            qaMensagemAtencao.Visible = false;

                        //Adiciona a função de JS de paginação do grid para as funções a serem executadas após renderização
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "HideShowColunaOrigem", "jQuery_1_12_3.hideShowColunaOrigem();", true);
                    }
                }
                catch (FaultException<CancelamentoServico.GeneralFault> fe)
                {
                    Logger.GravarErro("Consultar Cancelamentos - Buscar", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Consultar Cancelamento - Buscar", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carrega a tabela de cancelamentos
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptCancelamentos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (SolicitacaoCancelamento)e.Item.DataItem;

                var ltlNumeroEstabelecimentoVenda = (Literal)e.Item.FindControl("ltlNumeroEstabelecimentoVenda");
                var ltlTipoVenda = (Literal)e.Item.FindControl("ltlTipoVenda");
                var ltlDataVenda = (Literal)e.Item.FindControl("ltlDataVenda");
                var ltlNSU = (Literal)e.Item.FindControl("ltlNSU");
                var ltlValorBrutoVenda = (Literal)e.Item.FindControl("ltlValorBrutoVenda");
                var ltlSaldoDisponivel = (Literal)e.Item.FindControl("ltlSaldoDisponivel");
                var ltlTipoCancelamento = (Literal)e.Item.FindControl("ltlTipoCancelamento");
                var ltlValorCancelamento = (Literal)e.Item.FindControl("ltlValorCancelamento");
                var ltlNumeroAvisoCancelamento = (Literal)e.Item.FindControl("ltlNumeroAvisoCancelamento");
                var ltlOrigem = (Literal)e.Item.FindControl("ltlOrigem");
                var ltlStatus = (Literal)e.Item.FindControl("ltlStatus");
                var ltlDataCancelamento = (Literal)e.Item.FindControl("ltlDataCancelamento");

                ltlNumeroEstabelecimentoVenda.Text = item.NumeroEstabelecimentoVenda.ToString();
                ltlDataCancelamento.Text = item.DataCancelamento.ToString("dd/MM/yyyy");
                ltlTipoVenda.Text = item.TipoVendaDetalhado;
                ltlDataVenda.Text = item.DataVenda.ToString("dd/MM/yyyy");
                ltlNSU.Text = item.NSU;
                ltlValorBrutoVenda.Text = String.Format("{0:C}", item.ValorBruto);
                ltlSaldoDisponivel.Text = String.Format("{0:C}", item.SaldoDisponivel);
                ltlTipoCancelamento.Text = item.TipoCancelamento.GetDescription();
                ltlValorCancelamento.Text = String.Format("{0:C}", item.ValorCancelamento);
                ltlNumeroAvisoCancelamento.Text = item.NumeroAvisoCancelamento;
                ltlOrigem.Text = item.Origem;
                ltlStatus.Text = item.Status;
            }
        }

        #endregion

        #region [ Métodos ]

        /// <summary>
        /// Busca solicitações de cancelamento efetivados de acordo com os filtros selecionados
        /// </summary>
        /// <returns>Retorna uma lista de cancelamentos</returns>
        private List<SolicitacaoCancelamento> GetSolicitacoesCancelamentoEfetivados()
        {
            List<SolicitacaoCancelamento> cancelamentos = new List<SolicitacaoCancelamento>();

            String codigoUsuario = SessaoAtual.LoginUsuario;
            Int32 numeroEstabelecimento  = ((ConsultaPv)consultaPv).PVsSelecionados.FirstOrDefault();
            String indicadorPesquisa = "P";
            DateTime dataInicial = txtDe.Text.ToDate("dd/MM/yyyy");
            DateTime dataFinal = txtAte.Text.ToDate("dd/MM/yyyy");
            Int64 numeroAvisoCancelamento = txtNumeroAvisoCancelamento.Text.ToInt64();
            Int64 numeroNsu = txtNSU.Text.ToInt64();
            TipoVenda tipoVenda;
            Enum.TryParse<TipoVenda>(ddlTipoVenda.SelectedValue, out tipoVenda);
            String numeroCartao = String.Empty;

            cancelamentos.AddRange(Services.BucarCancelamentos(codigoUsuario, numeroEstabelecimento,
                        indicadorPesquisa, dataInicial, dataFinal, numeroAvisoCancelamento, numeroNsu, tipoVenda)
                        .Where(s => s.DataCancelamento.Date <= DateTime.Now.Date).ToList());

            return cancelamentos;
        }

        private List<SolicitacaoCancelamento> GetSolicitacoesCancelamentoNaoEfetivados()
        {
            List<SolicitacaoCancelamento> cancelamentos = new List<SolicitacaoCancelamento>();

            String codigoUsuario = SessaoAtual.LoginUsuario;
            Int32 numeroEstabelecimento = ((ConsultaPv)consultaPv).PVsSelecionados.FirstOrDefault();
            DateTime dataInicial = txtDe.Text.ToDate("dd/MM/yyyy");
            DateTime dataFinal = txtAte.Text.ToDate("dd/MM/yyyy");
            Int64 numeroAvisoCancelamento = txtNumeroAvisoCancelamento.Text.ToInt64();
            Int64 numeroNsu = txtNSU.Text.ToInt64();
            TipoVenda tipoVenda;
            Enum.TryParse<TipoVenda>(ddlTipoVenda.SelectedValue, out tipoVenda);
            String numeroCartao = String.Empty;

            Boolean existeNumeroAviso = !String.IsNullOrEmpty(txtNumeroAvisoCancelamento.Text);
            Boolean existePeriodo = !String.IsNullOrEmpty(txtDe.Text) && !String.IsNullOrEmpty(txtAte.Text);

            if(existeNumeroAviso)
                cancelamentos.AddRange(Services.BuscaCancelamentosDesfeitosPorPontoVendaENumeroAviso(numeroAvisoCancelamento, numeroCartao, numeroEstabelecimento));
            else if(existePeriodo)
                cancelamentos.AddRange(Services.BuscaCancelamentosDesfeitosPorPeriodo(dataInicial, dataFinal, 0, numeroCartao, numeroEstabelecimento));

            return cancelamentos;
        }

        /// <summary>
        /// Método que carrega os dados da tabela de solicitações de cancelamento com os dados retornados do serviço
        /// </summary>
        /// <param name="solicitacoesCancelamento">Lista de solicitações de cancelamento</param>
        private void CarregaTabelaSolicitacoesCancelamento(List<SolicitacaoCancelamento> solicitacoesCancelamento)
        {
            solicitacoesCancelamento.OrderByDescending(x => x.DataCancelamento);

            rptCancelamentos.DataSource = solicitacoesCancelamento;
            rptCancelamentos.DataBind();

            var existeAlgumaVendaDeDebito = solicitacoesCancelamento.Any(s => s.TipoVenda == TipoVenda.Debito);
            qaMensagemAtencao.Visible = existeAlgumaVendaDeDebito;

            //Adiciona a função de JS de paginação do grid para as funções a serem executadas após renderização
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao", "pageResultTable('tblCancelamentos', 1, 10, 5);", true);
        }

        /// <summary>
        /// Exibe ou esconde controles da tela
        /// </summary>
        /// <param name="numeroCancelamentos">Número de cancelamentos</param>
        private void MostraControles(Int32 numeroCancelamentos)
        {
            Boolean existeMinimoUmCancelamento = numeroCancelamentos > 0;
            pnlQuadroAviso.Visible = !existeMinimoUmCancelamento;
            rptCancelamentos.Visible = existeMinimoUmCancelamento;
        }

        #endregion
    }
}
