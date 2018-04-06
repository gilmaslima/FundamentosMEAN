using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.DataCash.SharePoint.DataCashService;
using System.Globalization;
using System.Web;
using System.ServiceModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace Redecard.PN.DataCash.SharePoint.WebParts.PerformanceAutorizacao
{
    public partial class PerformanceAutorizacaoUserControl : UserControlBaseDataCash
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Carrega os campos de data com o valor inicial
                txtDataInicial.Text = DateTime.Today.ToString("'01'/MM/yyyy");
                txtDataFinal.Text = DateTime.Today.ToString("dd/MM/yyyy");

                //Carrega a combo de status
                ddlStatus.Items.Clear();
                foreach (StatusTransacoes status in Enum.GetValues(typeof(StatusTransacoes)))
                    ddlStatus.Items.Add(new ListItem(status.GetDescription(), ((Int32)status).ToString()));
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Carregando gráfico de performance de autorização"))
            {
                try
                {
                    DateTime dataInicial = txtDataInicial.Text.ToDate("dd/MM/yyyy");
                    DateTime dataFinal = txtDataFinal.Text.ToDate("dd/MM/yyyy");
                    StatusTransacoes status = (StatusTransacoes) ddlStatus.SelectedValue.ToInt32(0);
                    Int32 pv = base.SessaoAtual.CodigoEntidade;

                    TotalTransacoes totalTransacoes = null;

                    using (var contexto = new ContextoWCF<DataCashServiceClient>())
                        totalTransacoes = contexto.Cliente.GetTotalTransacoes(dataInicial, dataFinal, pv);

                    if (totalTransacoes != null && totalTransacoes.CodigoRetorno == 1)
                    {
                        Int32 aprovadas = totalTransacoes.TotalTransacoesAprovadas;
                        Int32 reprovadas = totalTransacoes.TotalTransacoesReprovadas;
                        Int32 aprovadasPorcentagem = -1; //-1: valor default representando a não exibição do gráfico para o item
                        Int32 reprovadasPorcentagem = -1; //-1: valor default representando a não exibição do gráfico para o item

                        DadosPerformance dadosAprovadas = new DadosPerformance(StatusTransacoes.Aprovada, aprovadas);
                        DadosPerformance dadosReprovadas = new DadosPerformance(StatusTransacoes.NaoAprovada, reprovadas);
                        DadosPerformance dadosTotal = new DadosPerformance();

                        var dados = new List<DadosPerformance>();
                        switch (status)
                        {
                            case StatusTransacoes.Todas:
                                dados.Add(dadosAprovadas);
                                dados.Add(dadosReprovadas);
                                if (aprovadas + reprovadas > 0)
                                {
                                    aprovadasPorcentagem = (Int32)Math.Round(100m * aprovadas / (aprovadas + reprovadas));
                                    reprovadasPorcentagem = 100 - aprovadasPorcentagem;
                                }
                                else //0 e 0: representam 50% e 50%
                                    aprovadasPorcentagem = reprovadasPorcentagem = 50;
                                dadosTotal.QuantidadeTransacoes = aprovadas + reprovadas;
                                break;
                            case StatusTransacoes.Aprovada:
                                dados.Add(dadosAprovadas);
                                aprovadasPorcentagem = 100; //representa sempre 100%
                                dadosTotal.QuantidadeTransacoes = aprovadas;
                                break;
                            case StatusTransacoes.NaoAprovada:
                                dados.Add(dadosReprovadas);                                
                                reprovadasPorcentagem = 100; //representa sempre 100%
                                dadosTotal.QuantidadeTransacoes = reprovadas;
                                break;
                            default: break;
                        }

                        dados.Add(dadosTotal);
                        rptTransacoes.DataSource = dados;
                        rptTransacoes.DataBind();
  
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Buscar", 
                            String.Format("buscar({0}, {1});", aprovadasPorcentagem, reprovadasPorcentagem), true);

                        pnlDados.Visible = true;
                    }
                    else
                    {
                        pnlDados.Visible = false;
                        pnlErro.Visible = !(pnlMain.Visible = false);
                        ((QuadroAviso)qdAviso).CarregarMensagem("Aviso", totalTransacoes.MensagemRetorno);
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        protected void rptTransacoes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lnkRelatorioTransacoes = e.Item.FindControl("lnkRelatorioTransacoes") as LinkButton;
                var lblQuantidade = e.Item.FindControl("lblQuantidade") as Literal;

                var item = e.Item.DataItem as DadosPerformance;

                if(item.Status.HasValue)
                {
                    lnkRelatorioTransacoes.Text = item.Status.GetDescription();
                    lnkRelatorioTransacoes.CommandArgument = item.Status.Value == StatusTransacoes.Aprovada ? "S" : "N";
                }
                else
                    lnkRelatorioTransacoes.Visible = false;
                
                lblQuantidade.Text = item.QuantidadeTransacoes.ToString("N0", new CultureInfo("pt-BR"));
            }
        }
        
        /// <summary>
        /// Redireciona para página de Relatório de Transações
        /// </summary>
        protected void lnkRelatorioTransacoes_Click(object sender, EventArgs e)
        {
            if (!(String.IsNullOrEmpty(txtDataFinal.Text) || String.IsNullOrEmpty(txtDataInicial.Text)))
            {
                LinkButton lnk = sender as LinkButton;                    
                QueryStringSegura queryString = new QueryStringSegura();
                queryString.Add("DataInicio", txtDataInicial.Text);
                queryString.Add("DataFinal", txtDataFinal.Text);
                queryString.Add("Aprovada", lnk.CommandArgument); //S ou N

                Response.Redirect("pn_GerVendasRelatorioTransacao.aspx?dados=" + queryString.ToString(), false);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }

        /// <summary>
        /// Enumeração auxiliar representando o Status das Transações
        /// </summary>
        private enum StatusTransacoes
        {
            [Description("Todas")]
            Todas = 0,
            [Description("Aprovada")]
            Aprovada = 1,
            [Description("Não Aprovada")]
            NaoAprovada = 2
        }

        /// <summary>
        /// Classe auxiliar para utilização interna na tela
        /// </summary>
        private class DadosPerformance
        {
            public Int32 QuantidadeTransacoes { get; set; }
            public StatusTransacoes? Status { get; set; }

            public DadosPerformance() { }

            public DadosPerformance(StatusTransacoes status, Int32 quantidadeTransacoes)
            {
                this.Status = status;
                this.QuantidadeTransacoes = quantidadeTransacoes;
            }
        }
    }
}
