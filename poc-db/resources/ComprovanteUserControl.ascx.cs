using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Redecard.PN.Boston.Sharepoint.Negocio;
using System.ServiceModel;
using System.Net;
using Microsoft.SharePoint;
using System.Globalization;

namespace Redecard.PN.Boston.Sharepoint.WebParts.Comprovante
{
    public partial class ComprovanteUserControl : UserControlBase
    {
        #region [ Eventos ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    CarregarDadosControles();
                }
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                Logger.GravarErro("MPos - Comprovante", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.CodigoErro ?? 600);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("MPos - Comprovante", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("MPos - Comprovante", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("MPos - Comprovante", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Evento do clique no botão de voltar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Evento do clique no botão imprimir em PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void linkPDF_Click(object sender, EventArgs e)
        {
            try
            {
                Document doc = new Document(PageSize.A4);
                MemoryStream ms = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                doc.Open();
                doc.SetMargins(40, 40, 40, 40);

                WebClient webClient = new WebClient();
                byte[] array = webClient.DownloadData(String.Format("{0}/{1}", SPContext.Current.Web.Url, "_layouts/Redecard.Comum/IMAGES/LogoRedeLaranja.png")); ;
                iTextSharp.text.Image headerImage = iTextSharp.text.Image.GetInstance(array);
                headerImage.Alignment = iTextSharp.text.Image.LEFT_ALIGN;
                doc.Add(headerImage);

                doc.Add(new Paragraph("Comprovante", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, new BaseColor(24, 60, 142))) { SpacingBefore = 10f });

                PdfPTable tabelaPDF = ConvertComprovanteToPDFFormat();
                if (tabelaPDF != null)
                    doc.Add(tabelaPDF);

                doc.Close();

                byte[] file = ms.GetBuffer();
                byte[] buffer = new byte[4096];

                string nomeArquivo = string.Format("attachment; filename=Comprovante_{0}.pdf", DateTime.Now.ToString("ddMMyyyy_hhmmss"));

                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", nomeArquivo);
                Response.BinaryWrite(file);

                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Boston - Comprovante", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ Métodos ]

        /// <summary>
        /// Carrega os dados dos controles da página
        /// </summary>
        private void CarregarDadosControles()
        {
            CarregarDadosEndereco();
            CarregarDadosComprovante();
        }

        /// <summary>
        /// Carrega os dados do endereço do PV
        /// </summary>
        private void CarregarDadosEndereco()
        {
            var endereco = Servicos.GetEnderecoInstalacaoPorPV(SessaoAtual.CodigoEntidade);
            ltlCEP.Text = endereco.CEP;
            ltlLogradouro.Text = endereco.Logradouro;
            ltlNumero.Text = endereco.Numero;
            ltlComplemento.Text = endereco.Complemento;
            ltlEstado.Text = endereco.Estado;
            ltlCidade.Text = endereco.Cidade;
            ltlBairro.Text = endereco.Bairro;
        }

        /// <summary>
        /// Carrega os dados do comprovante de pagamento
        /// </summary>
        private void CarregarDadosComprovante()
        {
            String query = Request["dados"];
            QueryStringSegura _query = new QueryStringSegura(query);

            ltlNSU.Text = HttpUtility.UrlDecode(_query["nsu"]);
            ltlTID.Text = HttpUtility.UrlDecode(_query["tid"]);
            ltlNroEstabelecimento.Text = HttpUtility.UrlDecode(_query["numeroEstabelecimento"]);
            ltlNomeEstabelecimento.Text = HttpUtility.UrlDecode(_query["nomeEstabelecimento"]);
            ltlDataPagamento.Text = HttpUtility.UrlDecode(_query["dataPagamento"]);
            ltlHoraPagamento.Text = HttpUtility.UrlDecode(_query["horaPagamento"]);
            ltlNroAutorizacao.Text = HttpUtility.UrlDecode(_query["numeroAutorizacao"]);
            ltlTipoTransacao.Text = HttpUtility.UrlDecode(_query["tipoTransacao"]);
            ltlFormaPagamento.Text = HttpUtility.UrlDecode(_query["formaPagamento"]);
            ltlBandeira.Text = HttpUtility.UrlDecode(_query["bandeira"]);
            ltlNomePortador.Text = "-";
            ltlNroCartao.Text = HttpUtility.UrlDecode(_query["numeroCartao"]);
            ltlValor.Text = HttpUtility.UrlDecode(_query["valor"]);
            ltlParcelas.Text = HttpUtility.UrlDecode(_query["numeroParcelas"]);
            ltlNroPedido.Text = HttpUtility.UrlDecode(_query["numeroPedido"]);
        }

        /// <summary>
        /// Formata os dados dos controles no formato PDF
        /// </summary>
        /// <returns></returns>
        private PdfPTable ConvertComprovanteToPDFFormat()
        {
            PdfPTable tabelaPDF = new PdfPTable(2);
            tabelaPDF.SpacingBefore = 10f;
            tabelaPDF.WidthPercentage = 50f;
            tabelaPDF.HeaderRows = 0;
            tabelaPDF.HorizontalAlignment = 0;

            tabelaPDF.AddCell((new PdfPCell(new Phrase(new Phrase("N° do comprovante de vendas (NSU):",
                new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL, new BaseColor(84, 84, 84)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(255, 255, 255),
                HorizontalAlignment = PdfPCell.ALIGN_RIGHT,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthLeft = 0.1f,
                BorderWidthRight = 0,
                BorderWidthTop = 0.1f,
                BorderColor = new BaseColor(163, 163, 163),
                BorderWidthBottom = 0
            }));

            tabelaPDF.AddCell((new PdfPCell(new Phrase(new Phrase(ltlNSU.Text,
                new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL, BaseColor.BLACK))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(255, 255, 255),
                HorizontalAlignment = PdfPCell.ALIGN_LEFT,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0.1f,
                BorderWidthLeft = 0,
                BorderWidthTop = 0.1f,
                BorderColor = new BaseColor(163, 163, 163),
                BorderWidthBottom = 0
            }));

            tabelaPDF.AddCell(CreateLeftCell("TID:", true));
            tabelaPDF.AddCell(CreateRightCell(ltlTID.Text, true));

            tabelaPDF.AddCell(CreateLeftCell("N° Estabelecimento:", false));
            tabelaPDF.AddCell(CreateRightCell(ltlNroEstabelecimento.Text, false));

            tabelaPDF.AddCell(CreateLeftCell("Nome Estabelecimento:", true));
            tabelaPDF.AddCell(CreateRightCell(ltlNomeEstabelecimento.Text, true));

            tabelaPDF.AddCell(CreateLeftCell("Data do Pagamento:", false));
            tabelaPDF.AddCell(CreateRightCell(ltlDataPagamento.Text, false));

            tabelaPDF.AddCell(CreateLeftCell("Hora do Pagamento:", true));
            tabelaPDF.AddCell(CreateRightCell(ltlHoraPagamento.Text, true));

            tabelaPDF.AddCell(CreateLeftCell("N° Autorização:", false));
            tabelaPDF.AddCell(CreateRightCell(ltlNroAutorizacao.Text, false));

            tabelaPDF.AddCell(CreateLeftCell("Tipo de Transação:", true));
            tabelaPDF.AddCell(CreateRightCell(ltlTipoTransacao.Text, true));

            tabelaPDF.AddCell(CreateLeftCell("Forma de Pagamento:", false));
            tabelaPDF.AddCell(CreateRightCell(ltlFormaPagamento.Text, false));

            tabelaPDF.AddCell(CreateLeftCell("Bandeira:", true));
            tabelaPDF.AddCell(CreateRightCell(ltlBandeira.Text, true));

            tabelaPDF.AddCell(CreateLeftCell("Nome do Portador:", false));
            tabelaPDF.AddCell(CreateRightCell(ltlNomePortador.Text, false));

            tabelaPDF.AddCell(CreateLeftCell("N° Cartão (Últimos 4 dig.):", true));
            tabelaPDF.AddCell(CreateRightCell(ltlNroCartao.Text, true));

            tabelaPDF.AddCell(CreateLeftCell("Valor:", false));
            tabelaPDF.AddCell(CreateRightCell(ltlValor.Text, false));

            tabelaPDF.AddCell(CreateLeftCell("N° Parcelas:", true));
            tabelaPDF.AddCell(CreateRightCell(ltlParcelas.Text, true));

            tabelaPDF.AddCell((new PdfPCell(new Phrase(new Phrase("N° do Pedido:",
                new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL, new BaseColor(84, 84, 84)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(255, 255, 255),
                HorizontalAlignment = PdfPCell.ALIGN_RIGHT,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0,
                BorderWidthLeft = 0.1f,
                BorderWidthTop = 0,
                BorderColor = new BaseColor(163, 163, 163),
                BorderWidthBottom = 0.1f
            }));
            tabelaPDF.AddCell(new PdfPCell(new Phrase(new Phrase(ltlNroPedido.Text,
                new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL, BaseColor.BLACK))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(255, 255, 255),
                HorizontalAlignment = PdfPCell.ALIGN_LEFT,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0.1f,
                BorderWidthLeft = 0,
                BorderWidthTop = 0,
                BorderColor = new BaseColor(163, 163, 163),
                BorderWidthBottom = 0.1f
            });


            return tabelaPDF;
        }

        /// <summary>
        /// Cria uma célula do lado esquerdo da tabela
        /// </summary>
        /// <param name="title"></param>
        /// <param name="alttr"></param>
        /// <returns></returns>
        private PdfPCell CreateLeftCell(String title, Boolean alttr)
        {
            return new PdfPCell(new Phrase(new Phrase(title,
                new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL, new BaseColor(84, 84, 84)))))
            {
                Padding = 5,
                BackgroundColor = alttr ? new BaseColor(221, 221, 221) : new BaseColor(255, 255, 255),
                HorizontalAlignment = PdfPCell.ALIGN_RIGHT,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0,
                BorderWidthLeft = 0.1f,
                BorderWidthTop = 0,
                BorderColor = new BaseColor(163, 163, 163),
                BorderWidthBottom = 0
            };
        }

        /// <summary>
        /// Cria uma célula do lado direito da tabela
        /// </summary>
        /// <param name="value"></param>
        /// <param name="alttr"></param>
        /// <returns></returns>
        private PdfPCell CreateRightCell(String value, Boolean alttr)
        {
            return new PdfPCell(new Phrase(new Phrase(value,
                new Font(Font.FontFamily.HELVETICA, 6, Font.NORMAL, BaseColor.BLACK))))
            {
                Padding = 5,
                BackgroundColor = alttr ? new BaseColor(221, 221, 221) : new BaseColor(255, 255, 255),
                HorizontalAlignment = PdfPCell.ALIGN_LEFT,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0.1f,
                BorderWidthLeft = 0,
                BorderWidthTop = 0,
                BorderColor = new BaseColor(163, 163, 163),
                BorderWidthBottom = 0
            };
        }

        #endregion
    }
}
