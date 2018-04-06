/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Redecard.PN.Comum.SharePoint.HistoricoAtividadeServico;

namespace Redecard.PN.Comum.SharePoint.LAYOUTS.Redecard.Comum
{
    public partial class LogAtividade : ApplicationPageBaseAutenticadaWindows
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarFiltros();
            }
        }
        
        #region [ Eventos Controles ]

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CarregarRelatorio();
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            Int32 codigoRelatorio = ddlRelatorios.SelectedValue.ToInt32();
            DateTime? data = txtData.Text.ToDateTimeNull("dd/MM/yyyy");
            Int32? numeroPv = txtEstabelecimento.Text.ToInt32Null();

            DataSet ds = ConsultarRelatorio(codigoRelatorio, data, numeroPv);

            StringBuilder csv = new StringBuilder();

            foreach (DataTable dt in ds.Tables)
            {
                String csvTabela = CSVExporter.GerarCSV(dt.Rows.Cast<DataRow>(), 
                    dt.Columns.Cast<DataColumn>().Select(row => row.ColumnName).ToList(),
                    (row) => { return row.ItemArray.Select(item => Convert.ToString(item)).ToList(); }, "\t");

                csv.AppendLine(csvTabela);
                if (ds.Tables.Count > 1)
                    csv.AppendLine();
            }

            Response.Clear();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.AppendHeader("Content-Disposition", 
                String.Format("attachment;filename=Log_{0}.csv", DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss")));
            Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.ContentType = "text/csv";
            Response.AppendHeader("Content-Length", csv.Length.ToString());
            Response.Write(csv.ToString());
            Response.Flush();
            Response.End();
        }

        protected void grvRelatorio_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
            {
                for (Int32 iCell = 0; iCell < e.Row.Cells.Count; iCell++)
                {
                    var css = new StringBuilder();

                    TableCell cell = e.Row.Cells[iCell];
                    if (!cell.Text.IsNumber())                                            
                        css.Append("alinhaEsquerda ");

                    if (iCell == e.Row.Cells.Count - 1)
                        css.Append("last ");

                    cell.CssClass = css.ToString();
                    cell.Text = HttpUtility.HtmlDecode(cell.Text);
                }
            }
        }

        protected void repRelatorios_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dt = e.Item.DataItem as DataTable;
                var ltrRelatorio = (Literal)e.Item.FindControl("ltrRelatorio");
                var grvRelatorio = (GridView)e.Item.FindControl("grvRelatorio");

                dt.Rows.Cast<DataRow>().ToList().ForEach(row =>
                    row.ItemArray = row.ItemArray.Select(item => Convert.ToString(item).Replace(";", "<br/>")).ToArray());

                ltrRelatorio.Text = dt.TableName;
                grvRelatorio.DataSource = dt;
                grvRelatorio.DataBind();
            }
        }

        #endregion

        #region [ Privados ]

        private void CarregarFiltros()
        {
            ddlRelatorios.DataSource = ConsultarTiposRelatorios();
            ddlRelatorios.DataBind();
            txtData.Text = DateTime.Today.ToString("dd/MM/yyyy");
        }

        private void CarregarRelatorio()
        {
            Int32 codigoRelatorio = ddlRelatorios.SelectedValue.ToInt32();
            DateTime? data = txtData.Text.ToDateTimeNull("dd/MM/yyyy");
            Int32? numeroPv = txtEstabelecimento.Text.ToInt32Null();

            var colunas = new List<String>();
            var ds = ConsultarRelatorio(codigoRelatorio, data, numeroPv);

            repRelatorios.DataSource = ds.Tables.Cast<DataTable>().ToArray();
            repRelatorios.DataBind();
        }

        #endregion

        #region [ Consultas ]

        private static Dictionary<Int32, String> ConsultarTiposRelatorios()
        {
            var tipos = new Dictionary<Int32, String>();

            using (var ctx = new ContextoWCF<HistoricoAtividadeServicoClient>())
                tipos = ctx.Cliente.ConsultarTiposRelatorios(true);

            return tipos;
        }

        private static DataSet ConsultarRelatorio(Int32 codigoRelatorio, DateTime? data, Int32? codigoEntidade)
        {
            using (var ctx = new ContextoWCF<HistoricoAtividadeServicoClient>())
                return ctx.Cliente.ConsultarRelatorio(codigoRelatorio, data, codigoEntidade);
        }

        #endregion
    }
}