#region Histórico do Arquivo
/*
(c) Copyright [2012] BRQ IT Solutions.
Autor       : [- 2012/08/21 - Lucas Nicoletto da Cunha]
Empresa     : [BRQ IT Solutions]
Histórico   : Criação da Classe
- [08/06/2012] – [Lucas Nicoletto da Cunha] – [Criação]
 *
*/
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using System.Collections.Generic;
using Redecard.PN.Comum;
using Redecard.PN.Cancelamento.Sharepoint.Modelos;
using System.Web;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.ErroArquivo
{
    public partial class ErroArquivoUserControl : UserControlBase
    {
        List<ErroLote> ListaErroArquivo = new List<ErroLote>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(base.SessaoAtual, null))
            {
                using (Logger Log = Logger.IniciarLog("Erro Arquivo - Page Load"))
                {
                    if (!Page.IsPostBack)
                    {
                        if (Session["ValidaArquivo"] != null)
                        {
                            ListaErroArquivo = (List<ErroLote>)Session["ValidaArquivo"];
                        }

                        rptDados.DataSource = ListaErroArquivo;
                        rptDados.DataBind();
                    }
                }
            }
        }

        protected void btCancelar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Erro Arquivo - Cancelar"))
            {
                Response.Redirect("pn_cancelamentovendas.aspx");
            }
        }

        protected void GerarExcel_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Erro Arquivo - Gerar Excel"))
            {
                if (Session["ValidaArquivo"] != null)
                {
                    ListaErroArquivo = (List<ErroLote>)Session["ValidaArquivo"];
                }

                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=errolote.xls");
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.ContentType = "application/vnd.ms-excel";

                System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                rptDados.RenderControl(htmlWrite);
                Response.Write("<table>");
                Response.Write(stringWrite.ToString());
                Response.Write("</table>");
                Response.Flush();
            }
        }

        protected void GerarPDF_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Erro Arquivo - Gerar PDF"))
            {
                if (Session["ValidaArquivo"] != null)
                {
                    ListaErroArquivo = (List<ErroLote>)Session["ValidaArquivo"];
                }

                DateTime DataGeracaoPDF = DateTime.Now;
                PdfPTable table = new PdfPTable(9);

                Document doc = new Document(PageSize.A2);
                DateTime horaimpressao = DateTime.Now;

                PdfPCell cell = new PdfPCell((new PdfPCell(new Phrase(new Phrase("ERRO ARQUIVO LOTE", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.WHITE)))) { Padding = 5, Colspan = 2, BackgroundColor = new BaseColor(25, 60, 145), HorizontalAlignment = 0 }));

                table.AddCell(Convert.ToString("Linha"));
                table.AddCell(Convert.ToString("Menssagem"));

                foreach (ErroLote ItemErro in ListaErroArquivo)
                {
                    table.AddCell(Convert.ToString(ItemErro.Linha));
                    table.AddCell(Convert.ToString(ItemErro.MensagemErro));
                }

                MemoryStream ms = new MemoryStream();
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                doc.Add(table);
                doc.Close();

                byte[] file = ms.GetBuffer();
                byte[] buffer = new byte[4096];

                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment; filename=ErroLote.pdf");
                Response.BinaryWrite(file);
                Response.End();
            }
        }
    }
}
