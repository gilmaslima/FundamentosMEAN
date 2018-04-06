using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using Redecard.PN.Comum;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Web;
using System.IO;
using iTextSharp.text.html.simpleparser;
using Microsoft.SharePoint;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.CompCancelamentoVenda
{
    public partial class CompCancelamentoVendaUserControl : UserControlBase
    {
        #region Constantes
        public const string NOME_PAGINA = "CompCancelamentoVendasUserControl.aspx";
        public const int CODIGO_ERRO_RETORNATABELA = 3015;
        #endregion

        #region Atributos
        List<ItemCancelamentoEntrada> ListaInicial
        {
            get { return ViewState["ListaInicial"] as List<ItemCancelamentoEntrada>; }
            set { ViewState["ListaInicial"] = value; }
        }
        List<ItemCancelamentoSaida> ListaFinal;

        #endregion

        #region Metodos Tela
        EstabelecimentoCancelamento estabelecimento = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(base.SessaoAtual, null))
            {
                using (Logger Log = Logger.IniciarLog("Comp. Cancelamento Venda - Page Load"))
                {
                    ScriptManager.RegisterStartupScript(Page, GetType(), "key", "setTimeout('window.location = 'pn_compCancelamentovenda.aspx'', 500);", true);

                    using (ServicoPortalCancelamentoClient client = new ServicoPortalCancelamentoClient())
                    {
                        try
                        {
                            estabelecimento = client.RetornaDadosEstabelecimentoCancelamento(this.SessaoAtual.CodigoEntidade);
                        }
                        catch (Exception ex)
                        {
                            SharePointUlsLog.LogErro(ex.Message);
                            Log.GravarErro(ex);
                        }
                    }

                    if (estabelecimento == null || !estabelecimento.Centralizadora)
                    {
                        ScriptManager.RegisterStartupScript(Page, GetType(), "key", "document.getElementsByClassName('ocultar').style.display = 'none';", true);
                        ScriptManager.RegisterStartupScript(Page, GetType(), "key", "$('.ocultar').hide();", true);
                        ScriptManager.RegisterStartupScript(Page, GetType(), "key", "$('.ocultar').hide();", true);
                    }

                    if (!IsPostBack)
                    {
                        if (estabelecimento != null)
                        {
                            //Carrega popula tabela oculta de dados do estabelecimento
                            nomedoestabelecimento.Text = estabelecimento.NomeEntidade;
                            endereco.Text = estabelecimento.Endereco;
                            numestabelecimento.Text = this.SessaoAtual.CodigoEntidade.ToString();
                            //datacancelamento.Text = estabelecimento.DataCriacao.ToString("dd/MM/yyyy");
                            datacancelamento.Text = DateTime.Now.ToString("dd/MM/yyyy");
                        }
                        txtDataImpressao.Text = "Data da Consulta: " + DateTime.Now.ToString("dd/MM/yyyy") + " às " + DateTime.Now.ToShortTimeString() + "";

                        ListaInicial = new List<ItemCancelamentoEntrada>();
                        List<ItemCancelamentoEntrada> input = new List<ItemCancelamentoEntrada>();
                        List<ItemCancelamentoSaida> output = new List<ItemCancelamentoSaida>();

                        if (Session["ItensEntrada"] != null)
                        {
                            input = (List<ItemCancelamentoEntrada>)Session["ItensEntrada"];

                            ListaInicial = input;
                        }
                        else
                        {
                            ListaInicial = new List<ItemCancelamentoEntrada>();
                        }

                        if (Session["ItensSaida"] != null)
                        {
                            output = (List<ItemCancelamentoSaida>)Session["ItensSaida"];
                        }

                        if (output != null)
                            foreach (ItemCancelamentoEntrada entrada in input)
                            {
                                if (int.Parse(output[input.IndexOf(entrada)].CodRetorno) == 0)
                                {
                                    entrada.NumAviso = output[input.IndexOf(entrada)].NumAvisoCanc;
                                }
                            }

                        ListaInicial = ListaInicial.Where(x => !string.IsNullOrEmpty(x.NumAviso)).ToList();

                        if (ListaInicial.Count != output.Count)
                        {
                            this.pnlErro.Visible = true;
                        }

                        rptProtocoloVendas.DataSource = ListaInicial;
                        rptProtocoloVendas.DataBind();
                    }
                }
            }
        }

        private PdfPTable RetornaTabela()
        {
            using (Logger Log = Logger.IniciarLog("Comp. Cancelamento Venda - Retorna Tabela"))
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


                    foreach (ItemCancelamentoEntrada ItemListaInicial in ListaInicial)
                    {
                        table.AddCell(ItemListaInicial.NumAviso);
                        table.AddCell(ItemListaInicial.NumEstabelecimento.ToString());
                        table.AddCell(ItemListaInicial.NumEstabelecimento.ToString());
                        table.AddCell(ItemListaInicial.NumCartao);
                        table.AddCell(ItemListaInicial.DataCancelamentoFormatada);
                        table.AddCell(Convert.ToString(ItemListaInicial.VlTrans));
                        table.AddCell(ItemListaInicial.TpVenda);
                        table.AddCell(Convert.ToString(ItemListaInicial.VlCanc));
                        table.AddCell(ItemListaInicial.TpVenda);
                    }

                    //doc.Add(table);
                }

                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex.Message);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO_RETORNATABELA);
                    Log.GravarErro(ex);
                }
                return table;
            }
        }

        /// <summary>
        /// Método executado no clique do botão voltar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Cancelamento_Voltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("pn_cancelamentovendas.aspx");
        }

        /// <summary>
        /// Link para encaminhar para a tela de erros de cancelamento.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void VisualizarErros(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Comp. Cancelamento Venda - Visualizar Erros"))
            {
                try
                {
                    List<ItemCancelamentoEntrada> itemCancelled = new List<ItemCancelamentoEntrada>();
                    List<ItemCancelamentoSaida> itemConfirm = new List<ItemCancelamentoSaida>();

                    if (Session["ItensEntrada"] != null)
                    {
                        List<ItemCancelamentoEntrada> input = (List<ItemCancelamentoEntrada>)Session["ItensEntrada"];

                        if (Session["ItensSaida"] != null)
                        {
                            List<ItemCancelamentoSaida> output = (List<ItemCancelamentoSaida>)Session["ItensSaida"];


                            if (output != null)
                                foreach (ItemCancelamentoEntrada entrada in input)
                                {
                                    if (int.Parse(output[input.IndexOf(entrada)].CodRetorno) == 0)
                                    {
                                        itemCancelled.Add(entrada);
                                        itemConfirm.Add(output[input.IndexOf(entrada)]);
                                    }
                                }

                            if (itemCancelled != null && itemCancelled.Count > 0)
                            {
                                Session["ItensEntrada"] = input.Except(itemCancelled).ToList();
                                Session["ItensSaida"] = output.Except(itemConfirm).ToList();
                            }
                        }
                    }

                    Response.Redirect("pn_CancelamentoVendasNaoRealizado.aspx");
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    Log.GravarErro(ex);
                }
            }
        }
        #endregion

        #region Metodos Acoes
        /// <summary>
        /// Método que salva os dados como PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void GerarExcel_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Comp. Cancelamento Venda - Gerar Excel"))
            {
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=comprovantecancelamento.xls");
                // HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Unicode;
                Response.ContentType = "application/vnd.ms-excel";

                Response.ContentEncoding = System.Text.Encoding.Unicode;
                Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

                System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
                //  HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Unicode;
                rptProtocoloVendas.RenderControl(htmlWrite);
                Response.Write("<table>");
                Response.Write(stringWrite.ToString());
                Response.Write("</table>");
                Response.End();
                //  HttpContext.Current.ApplicationInstance.CompleteRequest();
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
            using (Logger Log = Logger.IniciarLog("Comp. Cancelamento Venda - Gerar PDF"))
            {
                try
                {
                    DateTime DataGeracaoPDF = DateTime.Now;
                    PdfPTable table = new PdfPTable(9);
                    table.TotalWidth = 900;
                    Document doc = new Document(PageSize.A2);

                    //string imageFilePath = Server.MapPath(".") + "/sites/fechado/_layouts/images/cabecalho.png";
                    //iTextSharp.text.Image caminhoimagem = iTextSharp.text.Image.GetInstance(imageFilePath);

                    DateTime horaimpressao = DateTime.Now;

                    string img = "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";
                    byte[] array = Convert.FromBase64String(img);
                    iTextSharp.text.Image headerImage = iTextSharp.text.Image.GetInstance(array);

                    PdfPCell header = new PdfPCell(headerImage);
                    PdfPCell linha_impressao = new PdfPCell((new PdfPCell(new Phrase(new Phrase("Data da Consulta: " + horaimpressao.ToShortDateString() + " às " + horaimpressao.ToShortTimeString() + "", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, new BaseColor(25, 60, 145))))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 2 }));
                    PdfPCell cell = new PdfPCell((new PdfPCell(new Phrase(new Phrase("COMPROVANTE DE CANCELAMENTO DE VENDAS", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.WHITE)))) { Padding = 5, Top = 5, Colspan = 2, BackgroundColor = new BaseColor(25, 60, 145), HorizontalAlignment = 0 }));

                    header.Colspan = 9;
                    header.HorizontalAlignment = 0;
                    header.Border = 0;
                    cell.Colspan = 9;
                    cell.HorizontalAlignment = 0; //0=Left, 1=Centre, 2=Right
                    table.AddCell(header);
                    table.AddCell(linha_impressao);
                    table.AddCell(cell);

                    this.SessaoAtual.RetornarMatriz();

                    table.AddCell((new PdfPCell(new Phrase(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(" Nome de Estabelecimento: " + SessaoAtual.NomeEntidade + "", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 }));
                    if (estabelecimento != null) table.AddCell((new PdfPCell(new Phrase(new Phrase(" Endereço: " + estabelecimento.Endereco, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Colspan = 9, Border = 0, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(" Nº do Estabelecimento: " + SessaoAtual.CodigoEntidade + "", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(" Data do Cancelamento: " + horaimpressao.ToShortDateString() + "", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase("", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, 1, BaseColor.BLACK)))) { Padding = 5, Border = 0, Colspan = 9, BackgroundColor = BaseColor.WHITE, HorizontalAlignment = 0 }));

                    table.AddCell((new PdfPCell(new Phrase(new Phrase(" Nº Aviso do  \nEstabelec.\ndo Cancel.  ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(" Nº do Estabelec.\nde Venda ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(" Nº do Aviso de\nCancelamento ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(" NSU/CV", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(" Data da\nVenda ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(" Valor de\nVenda (R$) ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(" Tipo de\nCancel. ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(" valor de\nCancel. (R$) ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(" Tipo de\nVenda ", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(211, 211, 211), HorizontalAlignment = 1 }));

                    foreach (ItemCancelamentoEntrada ItemListaInicial in ListaInicial)
                    {
                        table.AddCell(Convert.ToString(ItemListaInicial.NumEstabelecimento));
                        table.AddCell(Convert.ToString(ItemListaInicial.NumEstabelecimento));
                        //   table.AddCell(ItemListaInicial.NumAviso);
                        table.AddCell(ItemListaInicial.NumAviso);
                        table.AddCell(ItemListaInicial.NSUFormatado);
                        table.AddCell(Convert.ToString(ItemListaInicial.DtTransfInt));
                        table.AddCell(Convert.ToString(ItemListaInicial.VlTransFormatado));
                        table.AddCell(ItemListaInicial.FormaVenda);
                        table.AddCell(Convert.ToString(ItemListaInicial.VlCancFormatado));
                        table.AddCell(ItemListaInicial.TpVendaFormatado);
                    }

                    MemoryStream ms = new MemoryStream();
                    PdfWriter.GetInstance(doc, ms);
                    doc.Open();
                    doc.Add(table);
                    doc.Close();

                    byte[] file = ms.GetBuffer();
                    byte[] buffer = new byte[4096];

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment; filename=comprovantecancelamentoVendas.pdf");
                    Response.BinaryWrite(file);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (Exception ex)
                {
                    ExibirPainelExcecao(FONTE, 300);
                    SharePointUlsLog.LogErro(ex);
                    Log.GravarErro(ex);
                }
            }
        }
        #endregion
    }
}
