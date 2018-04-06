using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Redecard.PN.Extrato.SharePoint.SaldosEmAberto;
using Redecard.PN.Comum;
using System.ServiceModel;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.SaldosEmAberto
{
    public partial class ucOutrasOpcoes : BaseUserControl
    {
        public Int32 CodigoEntidade { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public Decimal ValorCarta { get; set; }
        public Redecard.PN.Extrato.SharePoint.SaldosEmAberto.DadosConsultaSaldosEmAberto DadosBusca { get; set; }
        List<Redecard.PN.Extrato.SharePoint.SaldosEmAberto.PeriodoDisponivel> lstPeriodos;


        public List<PeriodoDisponivel> PeriodosDisponiveis { get { return lstPeriodos; } }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Carrega os dados do quadro de opções
        /// </summary>
        public void Carregar()
        {
            pnlMensagem.Visible = false;

            Redecard.PN.Comum.QueryStringSegura queryString = new Redecard.PN.Comum.QueryStringSegura();
            queryString["circularizacao"] = "S";
            queryString["codigoEntidade"] = CodigoEntidade.ToString();
            queryString["dataSolicitacao"] = DataSolicitacao.ToString();
            queryString["valorCarta"] = ValorCarta.ToString();

            lnkCarta.Attributes["onclick"] = String.Format("javascript:VisualizarCarta('{0}');return false;", Server.HtmlEncode(queryString.ToString()));

            Redecard.PN.Extrato.SharePoint.SaldosEmAberto.StatusRetorno status = new Redecard.PN.Extrato.SharePoint.SaldosEmAberto.StatusRetorno();
            try
            {

                using (var contexto = new ContextoWCF<Redecard.PN.Extrato.SharePoint.SaldosEmAberto.RelatorioSaldosEmAbertoClient>())
                {
                    lstPeriodos = contexto.Cliente.ConsultarPeriodosDisponiveis(out status, DadosBusca, 0, 5, new Guid(), new Guid());
                    if (status.CodigoRetorno != 0)
                    {
                        //base.ExibirPainelExcecao("Redecard.PN.Extrato.SaldosEmAberto", status.CodigoRetorno);
                        lblMensagem.Text = "Não existem períodos disponíveis.";
                        pnlMensagem.Visible = true;
                        return;
                    }

                }

                //carrega o repeter de períodos disponíveis
                rptPeriodos.DataSource = PeriodosDisponiveis;
                rptPeriodos.DataBind();

            }
            catch (FaultException<Redecard.PN.Extrato.SharePoint.SaldosEmAberto.GeneralFault> ex)
            {
                Redecard.PN.Comum.SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);

            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);

            }
        }


        protected void rptPeriodos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                LinkButton lnkPeriodo = e.Item.FindControl("lnkPeriodo") as LinkButton;
                PeriodoDisponivel item = e.Item.DataItem as PeriodoDisponivel;
                lnkPeriodo.Text = item.DataPeriodoInicial.ToString("MM/yyyy") + " a " + item.DataPeriodoFinal.ToString("MM/yyyy");

                lnkPeriodo.Attributes["onclick"] = String.Format("javascript:Pesquisar('{0}','{1}', '{2}');return false;", item.DataPeriodoInicial.ToString("MM/yyyy"), item.DataPeriodoFinal.ToString("MM/yyyy"), item.CodigoSolicitacao);
            }
        }

    }
}
