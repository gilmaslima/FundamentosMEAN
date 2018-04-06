using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using System.Web.UI;
using System.IO;
using Winnovative.WnvHtmlConvert;
using Redecard.PN.Comum;

namespace Redecard.PN.Cancelamento.Sharepoint.LAYOUTS
{
    public partial class RelatorioAnulacao : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Relatório Anulação - Page Load"))
            {
                if (!IsPostBack)
                    CarregaTabelaLista();

                DateTime horaimpressao = DateTime.Now;
                this.lblData.Text = "Data da Consulta: " + horaimpressao.ToShortDateString() + " às " + horaimpressao.ToShortTimeString();
            }
        }

        public string pagina
        {
            get { return ViewState["pagina"] != null ? ViewState["pagina"].ToString() : string.Empty; }
            set { ViewState["pagina"] = value; }
        }

        /// <summary>
        /// Captura o Render da página para fazer exportar para excel ou pdf quando selecionado
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {

            //// setup a TextWriter to capture the markup
            //TextWriter tw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(tw);

            //// render the markup into our surrogate TextWriter
            //base.Render(htw);

            //// get the captured markup as a string
            //pagina = tw.ToString();

            //// render the markup into the output stream verbatim
            //writer.Write(pagina);
            //    ExportarPDF();
            
        }

        /// <summary>
        /// Método para carregar os dados do comprovante.
        /// </summary>
        private void CarregaTabelaLista()
        {
            if (Session["ItensAnulamento"] != null)
            {
                List<ModComprovante> comprovantes = (List<ModComprovante>)Session["ItensAnulamento"];

                if (comprovantes == null) comprovantes = new List<ModComprovante>();

                rptDados.DataSource = comprovantes;
                rptDados.DataBind();
            }
        }

        private void ExportarPDF()
        {
            using (Logger Log = Logger.IniciarLog("Relatório Anulação - Exportar PDF"))
            {
                try
                {
                    string attachment = "AnulacaoCancelamentoVendas_" + DateTime.Now.ToString("yyyyMMdd") + ".pdf";
                    StringWriter stw = new StringWriter();

                    HtmlTextWriter htextw = new HtmlTextWriter(stw);

                    pnlImpressao.RenderControl(htextw);
                    PdfConverter pdf = new PdfConverter();
                    pdf.LicenseKey = "rIedjJ2Mnp+YjJmCnIyfnYKdnoKVlZWV";

                    byte[] file = pdf.GetPdfBytesFromHtmlString(pagina);

                    System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.AddHeader("Content-Type", "binary/octet-stream");
                    response.AddHeader("Content-Disposition",
                        "attachment; filename=" + attachment + "; size=" + file.Length.ToString());
                    response.Flush();
                    response.BinaryWrite(file);
                    response.Flush();
                    response.End();


                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                }
            }
        }
    }
}
