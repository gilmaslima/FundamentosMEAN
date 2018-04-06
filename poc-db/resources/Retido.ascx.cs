using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Extrato.SharePoint.Helper;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.RelatorioCreditoSuspensosRetidosPenhorados
{
    public partial class Retido : UserControl, IControlesParaDownload
    {
        #region Property
        private decimal TotalPeriodoValorRetidoCredito { get; set; }
        private decimal TotalPeriodoValorRetidoDebito { get; set; }
        #endregion

        #region Event
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void grvDadosRetidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Footer)
            {
                return;
            }

            e.Row.Cells[0].Text = "Total de Retidos no Período";
            e.Row.Cells[3].Text = TotalPeriodoValorRetidoCredito.ToString("N2");

            e.Row.Cells.RemoveAt(2);
            e.Row.Cells.RemoveAt(1);
            e.Row.Cells[0].ColumnSpan = 3;
        }

        protected void grvDadosDebitosRetidos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Footer)
            {
                return;
            }

            e.Row.Cells[0].Text = "Total no Período";
            e.Row.Cells[8].Text = TotalPeriodoValorRetidoDebito.ToString("N2");
            e.Row.Cells.RemoveAt(7);
            e.Row.Cells.RemoveAt(6);
            e.Row.Cells.RemoveAt(5);
            e.Row.Cells.RemoveAt(4);
            e.Row.Cells.RemoveAt(3);
            e.Row.Cells.RemoveAt(2);
            e.Row.Cells.RemoveAt(1);
            e.Row.Cells[0].ColumnSpan = 8;
        }
        #endregion

        #region Method
        public List<Control> ObterControlesParaDownload()
        {
            List<Control> result = new List<Control>();
            result.Add(this.divTituloNumeroProcesso);
            if (this.grvDadosRetidos.Controls.Count > 0)
            {
                result.Add(this.grvDadosRetidos.Controls[0]);
            }
            result.Add(this.divTotalDebitosRetidos);
            if (this.grvDadosDebitosRetidos.Controls.Count > 0)
            {
                result.Add(this.grvDadosDebitosRetidos.Controls[0]);
            }
            return result;
        }

        public void CarregarDados(Servico.CSP.ConsultarRetencaoNumeroProcessoRetorno itemPR,
                                    List<Servico.CSP.ConsultarRetencaoDetalheProcessoCreditoRetorno> listDC,
                                    List<Servico.CSP.ConsultarRetencaoDetalheProcessoDebitoRetorno> listDD,
                                    Servico.CSP.ConsultarRetencaoDescricaoComValorRetorno itemD1,
                                    Servico.CSP.ConsultarRetencaoDescricaoSemValorRetorno itemD2)
        {
            spnTituloNumeroProcesso.InnerText = itemPR.NumeroProcesso;
            spnTituloNumeroProcessoValorTotal.InnerText = itemPR.ValorTotalProcesso.ToString("N2");

            TotalPeriodoValorRetidoCredito = itemD1 == null ? 0.0M : itemD1.ValorRetencao;
            grvDadosRetidos.DataSource = listDC;
            grvDadosRetidos.DataBind();

            TotalPeriodoValorRetidoDebito = itemD2 == null ? 0.0M : itemD2.ValorRetencao;
            grvDadosDebitosRetidos.DataSource = listDD;
            grvDadosDebitosRetidos.DataBind();
        }
        #endregion
    }
}
