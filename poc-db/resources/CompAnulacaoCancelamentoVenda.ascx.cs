#region Histórico do Arquivo
/*
(c) Copyright [2012] BRQ IT Solutions.
Autor       : [- 2012/08/21 - Tiago Barbosa dos Santos]
Empresa     : [BRQ IT Solutions]
Histórico   : Criação da Classe
- [08/06/2012] – [Tiago Barbosa dos Santos] – [Criação]
 *
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using Redecard.PN.Comum;
using System.Web;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.ComprovanteAnulacaoCancelamentoVenda
{
    public partial class ComprovanteAnulacaoCancelamentoVendaUserControl : UserControlBase
    {
        List<ModComprovante> comprovantes
        {
            get { return ViewState["Comprovantes"] as List<ModComprovante>; }
            set { ViewState["Comprovantes"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(base.SessaoAtual, null))
            {
                using (Logger Log = Logger.IniciarLog("Comp. Anulação Cancelamento Venda - Page Load"))
                {
                    CarregaTabelaLista();
                    //rptDados.DataSource = new List<ModComprovante>();
                    //rptDados.DataBind();
                }
            }
        }

        private void CarregaTabela(int numAvisoCancel)
        {
            using (ServicoPortalCancelamentoClient client = new ServicoPortalCancelamentoClient())
            {
                client.Open();
                //    rptDados.DataSource = client.ComprovanteCancelamento(SessaoAtual.CodigoEntidade, SessaoAtual.IDSessao, "", 0, " ", numAvisoCancel);
                //    rptDados.DataBind();
                client.Close();
            }
        }

        private void CarregaTabelaLista()
        {
            List<ModComprovante> comprovantesValidos = (List<ModComprovante>)Session["ItensAnulamento"];
            List<ModAnularCancelamento> comprovantesSaida = (List<ModAnularCancelamento>)Session["ItensAnulamentoSaida"];

            List<ModComprovante> _comprovantes = new List<ModComprovante>();
            comprovantesValidos.ForEach(delegate(ModComprovante comprovante)
            {
                comprovantesSaida.ForEach(delegate(ModAnularCancelamento comprovanteSaida)
                {
                    if ((comprovante.NumCartao == comprovanteSaida.numCartao
                        || comprovante.NumNSU == comprovanteSaida.numNsu) 
                        && comprovante.ValorCancelamento == comprovanteSaida.valorCancel
                        && Int32.Parse(comprovanteSaida.CodRetorno) == 0)
                    {
                        _comprovantes.Add(comprovante.Clone() as ModComprovante);
                    }
                });
            });

            comprovantes = _comprovantes;

            rptDados.DataSource = comprovantes;
            rptDados.DataBind();
        }


        private PdfPTable RetornaTabela()
        {
            using (Logger Log = Logger.IniciarLog("Comp. Anulação Cancelamento Venda - Retorna Tabela Pdf"))
            {
                PdfPTable table = new PdfPTable(9);
                try
                {
                    PdfPCell cell = new PdfPCell(new Phrase("PROTOCOLO DE CANCELAMENTO DE VENDAS"));
                    cell.Colspan = 9;
                    cell.HorizontalAlignment = 2; //0=Left, 1=Centre, 2=Right
                    table.AddCell(cell);
                    table.AddCell("Nº Aviso do<br />Estabelec.<br />do Cancel.");
                    table.AddCell("Nº do Estabelec. <br /> de Venda");
                    table.AddCell("Nº do Aviso de <br /> Cancelamento");
                    table.AddCell("NSU/CV");
                    table.AddCell("Data de <br /> Cancel.");
                    table.AddCell("Valor de <br /> Venda (R$)");
                    table.AddCell("Tipo de<br /> Cancel.");
                    table.AddCell("valor de <br /> Cancel. (R$)");
                    table.AddCell("Tipo de <br /> Venda");


                    foreach (ModComprovante Itemcomprovantes in comprovantes)
                    {
                        table.AddCell(Itemcomprovantes.NumAvisoCancelamentoFormatada);
                        table.AddCell(Itemcomprovantes.NumEstabelecimento.ToString());
                        table.AddCell(Itemcomprovantes.NumEstabelecimento.ToString());
                        table.AddCell(Itemcomprovantes.NumCartao);
                        table.AddCell(Convert.ToString(Itemcomprovantes.DataTransacaoFormatada));
                        table.AddCell(Convert.ToString(Itemcomprovantes.ValorTransacao));
                        table.AddCell(Itemcomprovantes.TipoCancelamento);
                        table.AddCell(Convert.ToString(Itemcomprovantes.ValorCancelamento));
                        table.AddCell(Itemcomprovantes.TipoVendaFormatado);
                    }


                    //doc.Add(table);
                }

                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex.Message);
                    base.ExibirPainelExcecao(FONTE, 0);
                    Log.GravarErro(ex);
                }
                return table;
            }
        }

        protected void Cancelamento_Voltar_Click(object sender, System.EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Comp. Anulação Cancelamento Venda - Voltar"))
            {
                this.Session.Remove("ItensAnulamento");
                this.Session.Remove("ItensAnulamentoSaida");

                Response.Redirect("pn_cancelamento.aspx");
            }
        }

        /// <summary>
        /// Método para gerar PDF.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        protected void GerarExcel_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Comp. Anulação Cancelamento Vendas - Gerar Excel"))
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=protocoloCancelamentoVendas.xls");
                Response.ContentType = "application/vnd.ms-excel";
                //            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                rptDados.RenderControl(htmlWrite);
                Response.Write("<table>");
                //          HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                Response.Write(stringWrite.ToString());
                Response.Write("</table>");
                Response.Flush();
                Response.AppendHeader("Refresh", "5");
            }
        }

        /// <summary>
        /// Método para gerar PDF.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        protected void GerarPDF_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Comp. Anulação Cancelamento Venda - Gerar PDF"))
            {
                DateTime DataGeracaoPDF = DateTime.Now;
                PdfPTable table = new PdfPTable(9);
                Document doc = new Document(PageSize.A2);

                DateTime horaimpressao = DateTime.Now;

                string img = "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";
                byte[] array = Convert.FromBase64String(img);
                iTextSharp.text.Image headerImage = iTextSharp.text.Image.GetInstance(array);

                PdfPCell header = new PdfPCell(headerImage);
                PdfPCell linha_impressao = new PdfPCell((new PdfPCell(new Phrase(new Phrase("Data da Consulta: " + horaimpressao.ToShortDateString() + " às " + horaimpressao.ToShortTimeString() + "", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, new BaseColor(25, 60, 145))))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 2 }));
                PdfPCell cell = new PdfPCell((new PdfPCell(new Phrase(new Phrase("PROTOCOLO DE CANCELAMENTO DE VENDAS", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.WHITE)))) { Padding = 5, Colspan = 2, BackgroundColor = new BaseColor(25, 60, 145), HorizontalAlignment = 0 }));

                header.Colspan = 9;
                header.HorizontalAlignment = 0;
                header.Border = 0;
                cell.Colspan = 9;
                cell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                table.AddCell(header);
                table.AddCell(linha_impressao);
                table.AddCell(cell);

                table.AddCell((new PdfPCell(new Phrase(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("Nome de Estabelecimento:" + SessaoAtual.NomeEntidade + "", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("Endereço:" + SessaoAtual.NomeEntidadeMatriz, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Colspan = 9, Border = 0, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("Nº do Estabelecimento: " + SessaoAtual.CodigoEntidade + "", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("Data do Estabelecimento:" + horaimpressao.ToShortDateString() + "", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 }));

                table.AddCell((new PdfPCell(new Phrase(new Phrase("Nº Aviso do\nEstabelec.\ndo Cancel.", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("Nº do Estabelec.\nde Venda", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("Nº do Aviso de\nCancelamento", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("NSU/CV", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("Data de\nCancel.", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("Valor de\nVenda (R$)", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("Tipo de\nCancel.", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("valor de\nCancel. (R$)", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                table.AddCell((new PdfPCell(new Phrase(new Phrase("Tipo de\nVenda", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));

                foreach (ModComprovante ItemModComprovante in comprovantes)
                {
                    table.AddCell(Convert.ToString(ItemModComprovante.NumEstabelecimento));
                    table.AddCell(Convert.ToString(ItemModComprovante.NumEstabelecimento));
                    table.AddCell(ItemModComprovante.NumAvisoCancelamentoFormatada);
                    table.AddCell(ItemModComprovante.NumCartao);
                    table.AddCell(Convert.ToString(ItemModComprovante.DataCartaFormatada));
                    table.AddCell(Convert.ToString(ItemModComprovante.ValorTransacao));
                    table.AddCell(ItemModComprovante.TipoCancelamento);
                    table.AddCell(Convert.ToString(ItemModComprovante.ValorCancelamentoFormatada));
                    table.AddCell(ItemModComprovante.TipoVendaFormatado);
                }

                MemoryStream ms = new MemoryStream();
                PdfWriter.GetInstance(doc, ms);
                doc.Open();
                doc.Add(table);
                doc.Close();

                byte[] file = ms.GetBuffer();
                byte[] buffer = new byte[4096];

                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment; filename=Relatorio.pdf");
                Response.BinaryWrite(file);
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
    }
}
