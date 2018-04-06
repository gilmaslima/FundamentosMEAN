using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Data;
using System.Text;
using Redecard.PN.DataCash.BasePage;

namespace Redecard.PN.DataCash.controles.comprovantes
{
    public partial class QuadroAcoes : UserControlBaseDataCash
    {
        /// <summary>
        /// Delegate para obtenção de tabela a ser rendereizada.
        /// </summary>
        public delegate TabelaExportacao ObterTabelaExportacaoEventHandler();

        /// <summary>
        /// Evento para obtenção de dados da tabela (DataTable) que será renderizada no PDF/Excel.
        /// Se o evento não for implementado, recupera da variável de sessão "TabelaPDF".
        /// </summary>
        public event ObterTabelaExportacaoEventHandler ObterTabelaExportacao;

        /// <summary>
        /// Delegate para obtenção de conteúdo customizado a ser renderizado.
        /// </summary>
        public delegate List<PdfPTable> ExportarConteudoCustomizadoEventHandler();

        /// <summary>
        /// Evento para obtenção de conteúdo customizado que será renderizada no PDF/Excel.
        /// </summary>
        public event ExportarConteudoCustomizadoEventHandler ExportarConteudoCustomizado;

        /// <summary>
        /// Configuração se deve exibir ou não o link de download Excel
        /// </summary>
        public Boolean ExibirLinkExcel 
        {
            get { return liExcel.Visible; }
            set { liExcel.Visible = value; }
        }

        /// <summary>
        /// Conteúdo binário da imagem do Logo Redecard/Rede
        /// </summary>
        private static Byte[] imagemLogo = null;
        /// <summary>
        /// Conteúdo binário da imagem do Logo Redecard/Rede
        /// </summary>
        private Byte[] ImagemLogo
        {
            get
            {
                if (imagemLogo == null)
                {
                    String logoPath = Server.MapPath(@"images\LogoRedeLaranja.png");
                    imagemLogo = File.ReadAllBytes(logoPath);
                }
                return imagemLogo;
            }
        }

        /// <summary>
        /// Exportação para Excel
        /// </summary>        
        protected void linkExcel_Click(object sender, EventArgs e)
        {
            using (Redecard.PN.Comum.Logger Log = Redecard.PN.Comum.Logger.IniciarLog("linkExcel_Click - Faça sua Venda"))
            {
                try
                {
                    String conteudo = String.Empty;

                    //Renderiza página atual carregada do relatório
                    if (this.ObterTabelaExportacao == null)
                    {
                        StringBuilder sb = new StringBuilder();
                        using (StringWriter writer = new StringWriter(sb))
                        {
                            using (HtmlTextWriter hwriter = new HtmlTextWriter(writer))
                            {
                                Control tabela = this.Parent.FindControl("tabelaDados");

                                if (tabela == null)
                                {
                                    tabela = this.Parent.FindControl("rptTransacoes");
                                    if (tabela == null)
                                    {
                                        Panel pnlFireForget = (Panel)this.Parent.FindControl("pnlFireForget");
                                        Panel pnlHistoricRecurring = (Panel)this.Parent.FindControl("pnlHistoricRecurring");
                                        if (pnlFireForget.Visible == true)
                                            tabela = this.Parent.FindControl("rptFireForget");
                                        else if (pnlHistoricRecurring.Visible == true)
                                            tabela = this.Parent.FindControl("rptHistoricRecurring");
                                    }
                                }

                                foreach (Control c in tabela.Controls)
                                {
                                    c.Controls.Remove(c.FindControl("chkSelecionar"));
                                }

                                tabela.RenderControl(hwriter);
                            }
                        }
                        conteudo = sb.ToString();
                    }
                    else
                    {
                        conteudo = RetornarTabelaExcel();
                    }

                    //Escreve arquivo no response
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.Buffer = true;
                    Response.AppendHeader("Content-Disposition",
                        String.Format("attachment; filename=Relatorio_{0}.xls", DateTime.Now.ToString("ddMMyyyy_HHmmss")));
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
                    Response.AppendHeader("Content-Length", conteudo.Length.ToString());
                    Response.ContentType = "application/ms-excel";
                    Response.Write(conteudo);
                    Response.Flush();
                    Response.End();
                }
                catch (Redecard.PN.Comum.PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }


        /// <summary>
        /// Exportação para PDF
        /// </summary>
        protected void linkPdf_Click(object sender, EventArgs e)
        {
            using (Redecard.PN.Comum.Logger Log = Redecard.PN.Comum.Logger.IniciarLog("linkPdf_Click - Faça sua Venda"))
            {
                try
                {
                    //Busca dados para montagem da tabela
                    TabelaExportacao config = null;
                    List<PdfPTable> tabelaPDF = RetornarTabelaPDF(out config);

                    //Incluir conteúdo customizado, caso implementado
                    if (ExportarConteudoCustomizado != null)
                    {
                        List<PdfPTable> pdfResumo = ExportarConteudoCustomizado();
                        if (pdfResumo != null && pdfResumo.Count > 0)
                        {
                            if (tabelaPDF == null)
                                tabelaPDF = new List<PdfPTable>();
                            tabelaPDF.InsertRange(1, pdfResumo);
                        }
                    }

                    Document doc = null;
                    if (config != null && config.ModoRetrato)
                        doc = new Document(PageSize.A4);
                    else
                        doc = new Document(PageSize.A4.Rotate());

                    MemoryStream ms = new MemoryStream();
                    PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                    writer.PageEvent = new MyPdfPageEventHelpPageNo();
                    doc.Open();
                    doc.SetMargins(40, 40, 40, 40);

                    //Adiciona Cabeçalho / Logo                
                    iTextSharp.text.Image headerImage = iTextSharp.text.Image.GetInstance(ImagemLogo);
                    headerImage.Alignment = iTextSharp.text.Image.LEFT_ALIGN;
                    doc.Add(headerImage);

                    //Adiciona tabela                
                    if (tabelaPDF != null)
                    {
                        foreach (PdfPTable pdfTable in tabelaPDF)
                            doc.Add(pdfTable);
                    }

                    doc.Close();

                    //Escreve como Response para download do PDF
                    Byte[] file = ms.GetBuffer();
                    Byte[] buffer = new byte[4096];

                    String nomeArquivo = String.Format("attachment; filename=Relatorio_{0}.pdf", DateTime.Now.ToString("ddMMyyyy_HHmmss"));

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", nomeArquivo);
                    Response.BinaryWrite(file);

                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (Redecard.PN.Comum.PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Monta tabela PDF para exportação
        /// </summary>
        private List<PdfPTable> RetornarTabelaPDF(out TabelaExportacao config)
        {
            DataTable dadosTabela = MontarDataTable(out config);            
                        
            if (dadosTabela != null)
            {                
                //Adiciona título do Relatório
                PdfPTable tabelaPDFTitulo = new PdfPTable(1);
                tabelaPDFTitulo.SpacingBefore = 10f;
                tabelaPDFTitulo.WidthPercentage = 100;
                tabelaPDFTitulo.HorizontalAlignment = Element.ALIGN_LEFT;
                tabelaPDFTitulo.AddCell((new PdfPCell(new Phrase(new Phrase(config.Titulo,
                    new Font(Font.FontFamily.HELVETICA, 12, 1, new BaseColor(8, 80, 146)))))
                {
                    PaddingLeft = 0,
                    PaddingTop = 5,
                    PaddingRight = 5,
                    PaddingBottom = 20,
                    Colspan = config.QuantidadeColunas(),
                    HorizontalAlignment = 0,
                    BorderWidth = 0
                }));

                PdfPTable tabelaPDF = new PdfPTable(dadosTabela.Columns.Count);
                tabelaPDF.SpacingBefore = 10f;
                tabelaPDF.WidthPercentage = config.LarguraTabela;                
                tabelaPDF.HorizontalAlignment = Element.ALIGN_LEFT;
                tabelaPDF.HeaderRows = 1;

                if (config.Colunas != null && config.Colunas.Length > 0)
                    tabelaPDF.SetWidths(config.Colunas.Select(col => col.Largura).ToArray());

                try
                {                    
                    //Linhas do Relatório
                    Int32 numeroLinha = 0;
                    for (Int32 iLinha = 0; iLinha < dadosTabela.Rows.Count; iLinha++)
                    {
                        DataRow linha = dadosTabela.Rows[iLinha];

                        for (Int32 iCelula = 0; iCelula < linha.ItemArray.Length; iCelula++)
                        {
                            Object celula = linha.ItemArray[iCelula];
                            Int32 alinhamentoH = PdfPCell.ALIGN_CENTER;
                            Boolean bordaEsquerda = true;

                            //Obtém as configurações para a célula/coluna
                            if (config.Colunas != null && config.Colunas.Length >= iCelula)
                            {
                                var configColuna = config.Colunas[iCelula];
                                switch(configColuna.Alinhamento)
                                {
                                    case HorizontalAlign.Justify: alinhamentoH = PdfPCell.ALIGN_JUSTIFIED; break;
                                    case HorizontalAlign.Left: alinhamentoH = PdfPCell.ALIGN_LEFT; break;
                                    case HorizontalAlign.Right: alinhamentoH = PdfPCell.ALIGN_RIGHT; break;
                                    default: alinhamentoH = PdfPCell.ALIGN_CENTER; break;
                                }
                                bordaEsquerda = configColuna.RenderizarBordaInterna;
                            }

                            //Linha de cabeçalho
                            if (iLinha == 0 && config.ExibirTituloColunas) 
                            {
                                tabelaPDF.AddCell((new PdfPCell(new Phrase(Convert.ToString(celula),
                                   new Font(Font.FontFamily.HELVETICA, 6, Font.BOLD, BaseColor.BLACK)))
                                {
                                    Padding = 5,
                                    BackgroundColor = new BaseColor(204, 204, 204),                                    
                                    HorizontalAlignment = alinhamentoH,
                                    VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                                    BorderWidthLeft = (iCelula == 0) ? 0.1f : 0,
                                    BorderWidthRight = (iCelula == linha.ItemArray.Length - 1) ? 0.1f : 0,
                                    BorderWidthTop = iLinha == 0 ? 0.1f : 0,
                                    BorderColor = new BaseColor(163, 163, 163),
                                    BorderWidthBottom = 0
                                }));
                            }
                            else
                            {
                                Phrase phrase = default(Phrase);

                                if (celula is System.Drawing.Image)
                                {
                                    var image = iTextSharp.text.Image.GetInstance(celula as System.Drawing.Image,
                                        numeroLinha % 2 == 0 ? new BaseColor(255, 255, 255) : new BaseColor(221, 221, 221));
                                    image.ScalePercent(50f);
                                    phrase = new Phrase(new Chunk(image, 0, 0));
                                }
                                else
                                {
                                    phrase = new Phrase(Convert.ToString(celula),
                                        new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL, BaseColor.BLACK));
                                }

                                tabelaPDF.AddCell(new PdfPCell(phrase) { 
                                    PaddingLeft = 3,
                                            PaddingRight = 3,
                                            PaddingBottom = 5,
                                            PaddingTop = 5,
                                            BackgroundColor = numeroLinha % 2 == 0 ?
                                                new BaseColor(255, 255, 255) : new BaseColor(221, 221, 221),
                                            HorizontalAlignment = alinhamentoH,
                                            VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                                            BorderWidthLeft = bordaEsquerda ? 0.1f : 0f,
                                            BorderColorLeft = (iCelula == 0) ? new BaseColor(163, 163, 163) : new BaseColor(238, 238, 238),
                                            BorderWidthRight = (iCelula == linha.ItemArray.Length - 1) ? 0.1f : 0,
                                            BorderWidthTop = iLinha == 0 ? 0.1f : 0,
                                            BorderWidthBottom = 0.1f,
                                            BorderColorBottom = (iLinha == dadosTabela.Rows.Count - 1) ? new BaseColor(163, 163, 163) : new BaseColor(221, 221, 221),
                                            BorderColor = new BaseColor(163, 163, 163)
                                });
                            }
                        }
                        numeroLinha++;
                    }

                    return new[] { tabelaPDFTitulo, tabelaPDF }.ToList();
                }
                catch { }                
            }

            return null;
        }

        /// <summary>
        /// Monta tabela Excel (CSV) para exportação
        /// </summary>
        private String RetornarTabelaExcel()
        {
            StringBuilder sb = new StringBuilder();
            TabelaExportacao config = null;
            DataTable dataTable = MontarDataTable(out config);

            foreach (DataRow linha in dataTable.Rows)
            {
                foreach (Object celula in linha.ItemArray)
                {
                    // se for String, adiciona campo formatado para string
                    if (celula is String)
                        sb.Append(String.Format("=\"{0}\"\t", celula));
                    else
                    {
                        Type type = celula != null ? celula.GetType() : null;
                        if (type != null && type.IsPrimitive)                       
                            sb.Append(String.Format("{0}\t", celula));
                        else
                            sb.Append("\t");
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Monta DataTable auxiliar
        /// </summary>        
        private DataTable MontarDataTable(out TabelaExportacao config)
        {
            DataTable dadosTabela = null;
            //Se delegate foi definido pela página, utiliza método de geração do relatório
            if (ObterTabelaExportacao != null)
            {
                config = ObterTabelaExportacao();

                if (config != null)
                {
                    dadosTabela = new DataTable(config.Titulo);

                    //Monta colunas do DataTable
                    for (Int32 iColuna = 0; iColuna < config.QuantidadeColunas(); iColuna++)
                        dadosTabela.Columns.Add(iColuna.ToString(), typeof(Object));

                    if (config.ExibirTituloColunas && config.Colunas != null && config.Colunas.Length > 0)
                        dadosTabela.Rows.Add().ItemArray = config.Colunas.Select(col => col.NomeColuna).ToArray();

                    foreach (var item in config.Registros)
                        dadosTabela.Rows.Add(config.FuncaoValores(item));
                }
            }
            //caso contrário, obtém da sessão
            else
            {
                dadosTabela = (DataTable)Session["TabelaPDF"];
                config = null;
            }
            return dadosTabela;
        }

        public override string ObterPaginaRedirecionamento()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Classe auxiliar para impressão do rodapé no relatório PDF exportado
    /// </summary>
    public class MyPdfPageEventHelpPageNo : PdfPageEventHelper
    {
        /// <summary>
        /// Template para o total de páginas (adicionado em cada página)
        /// </summary>
        protected PdfTemplate total;

        /// <summary>
        /// Classe fonte base do relatório
        /// </summary>
        private BaseFont helv;

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            total = writer.DirectContent.CreateTemplate(200, 100);
            helv = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);
        }

        /// <summary>
        /// Evento chamado ao terminar renderização de cada página
        /// </summary>        
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfContentByte cb = writer.DirectContent;
            cb.SaveState();
            cb.SetFontAndSize(helv, 8);

            //Escreve página atual no rodapé
            cb.BeginText();
            cb.SetTextMatrix(document.Left, document.Bottom - 20);
            cb.ShowText(String.Format("Página {0} de ", writer.PageNumber));
            cb.EndText();

            //Adiciona template de total de páginas no rodapé
            cb.AddTemplate(total, document.Left + 40 + 4*writer.PageNumber.ToString().Length, document.Bottom - 20);
            
            //Adiciona hora atual no rodapé
            cb.BeginText();
            cb.SetTextMatrix(document.Right - 72, document.Bottom - 20);
            cb.ShowText(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            cb.EndText();

            cb.RestoreState();
        }

        /// <summary>
        /// Evento chamado ao finalizar a renderização do relatório
        /// </summary>
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            //Substitui o template de total com o total de páginas do relatório
            total.BeginText();
            total.SetFontAndSize(helv, 8);
            total.SetTextMatrix(0, 0);
            String footerText = Convert.ToString(writer.PageNumber - 1);
            total.ShowText(footerText);
            total.EndText();
        }
    }

    /// <summary>
    /// Classe auxiliar de configuração do relatório que será exportado
    /// </summary>
    public class TabelaExportacao
    {
        /// <summary>Título do relatório</summary>
        public String Titulo { get; set; }

        /// <summary>Nome/Configuração das colunas do relatório</summary>
        public Coluna[] Colunas { get; set; }

        /// <summary>Function responsável por retornar os valores das colunas para cada registro</summary>
        public Func<Object, Object[]> FuncaoValores { get; set; }

        /// <summary>Registros que irão compor o relatório</summary>
        public IEnumerable<Object> Registros { get; set; }

        private Boolean exibirTituloColunas = true;
        /// <summary>Exibir ou não o título das colunas</summary>
        public Boolean ExibirTituloColunas 
        {
            get { return this.exibirTituloColunas; }
            set { this.exibirTituloColunas = value; }
        }

        /// <summary>
        /// Modo Retrato (default: false)
        /// </summary>
        public Boolean ModoRetrato { get; set; }

        private Single larguraTabelaPercentual = 100;       
        /// <summary>
        /// Percentual de Largura da tabela, de 0 a 100 (default: 100)
        /// </summary>       
        public Single LarguraTabela 
        {
            get { return this.larguraTabelaPercentual; }
            set { this.larguraTabelaPercentual = value; }
        }

        /// <summary>
        /// Quantidade de colunas
        /// </summary>
        public Int32 QuantidadeColunas()
        {
            //Calcula a quantidade de colunas: primeiro, com base nos títulos das colunas,
            //e caso não esteja definido, pelo tamanho de itens do array retornado pela FuncaoValores
            Int32 qtdColunas = 0;
            if (this.Colunas == null)
            {
                if (this.Registros.FirstOrDefault() != null)
                    qtdColunas = this.FuncaoValores(this.Registros.First()).Length;
            }
            else
                qtdColunas = this.Colunas.Length;
            return qtdColunas;
        }
    }

    /// <summary>
    /// Classe auxiliar para configuração das colunas da tabela a ser exportada
    /// </summary>
    public class Coluna
    {
        /// <summary>
        /// Nome da coluna
        /// </summary>
        public String NomeColuna { get; set; }

        /// <summary>
        /// Alinhamento horizontal da célula na tabela. (Default: Center)
        /// </summary>
        public HorizontalAlign Alinhamento { get; set; }

        /// <summary>
        /// Renderiza ou não a borda interna à esquerda (Default: true)
        /// </summary>
        public Boolean RenderizarBordaInterna { get; set; }

        /// <summary>
        /// Tamanho da coluna
        /// </summary>
        public Single Largura { get; set; }

        public Coluna(String nomeColuna)
        {
            this.NomeColuna = nomeColuna;
            this.Alinhamento = HorizontalAlign.Center;
            this.RenderizarBordaInterna = true;
            this.Largura = 1;
        }

        public Coluna SetAlinhamento(HorizontalAlign alinhamento)
        {
            this.Alinhamento = alinhamento;
            return this;
        }

        public Coluna SetBordaInterna(Boolean renderizarBordaInternaEsquerda)
        {
            this.RenderizarBordaInterna = renderizarBordaInternaEsquerda;
            return this;
        }

        public Coluna SetLargura(Single largura)
        {
            this.Largura = largura;
            return this;
        }
    }
}