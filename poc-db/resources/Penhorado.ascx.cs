using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Extrato.SharePoint.Helper;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.RelatorioCreditoSuspensosRetidosPenhorados
{
    public partial class Penhorado : UserControl, IControlesParaDownload
    {
        #region Property
        private decimal TotalPeriodoValorPenhorado { get; set; }
        #endregion

        #region Event
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void grvDadosPenhorados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.Footer)
            {
                return;
            }

            e.Row.Cells[0].Text = "Total no Período";
            e.Row.Cells[8].Text = TotalPeriodoValorPenhorado.ToString("N2");
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
            if (this.grvDadosPenhorados.Controls.Count > 0)
            {
                result.Add(this.grvDadosPenhorados.Controls[0]);
            }
            return result;
        }

        public void CarregarDados(Servico.CSP.ConsultarPenhoraNumeroProcessoRetorno itemPR,
                                    List<Servico.CSP.ConsultarPenhoraDetalheProcessoCreditoRetorno> listDT,
                                    Servico.CSP.ConsultarPenhoraTotaisRetorno totais)
        {
            spnTituloNumeroProcesso.InnerText = itemPR.NumeroProcesso;
            spnTituloNumeroProcessoValorTotal.InnerText = itemPR.ValorTotalProcesso.ToString("N2");

            TotalPeriodoValorPenhorado = totais.TotalValorPenhorado;
            grvDadosPenhorados.DataSource = listDT;
            grvDadosPenhorados.DataBind();
        }
        #endregion
    }
}
