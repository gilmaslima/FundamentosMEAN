using System;
using System.Runtime.CompilerServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Boston.Sharepoint.Base;
using Redecard.PN.Comum;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.pdf;
using System.Web;
using Redecard.PN.Boston.Sharepoint.Negocio;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.Boston.Sharepoint.Layouts.Redecard.PN.Boston.Sharepoint
{
    public partial class Comprovante : BostonBasePage
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
                    MudarLogoOrigemCredenciamento();

                    Int32 codigoRetorno = 0;
                    String mensagemRetorno = String.Empty;

                    codigoRetorno = AtualizaTaxaAtivacaoPropostaMPOS(out mensagemRetorno);

                    if (codigoRetorno != 0)
                    {
                        Logger.GravarErro(mensagemRetorno);
                        SharePointUlsLog.LogErro(mensagemRetorno);
                        base.ExibirPainelExcecao(mensagemRetorno, codigoRetorno);
                    }
                    else
                        CarregarDadosComprovante();
                }
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Boston - Comprovante", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodigoErro ?? 600);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Boston - Comprovante", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Boston - Comprovante", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Boston - Comprovante", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Muda a imagem e background do logotipo da Masterpage caso a origem seja diferente
        /// </summary>
        private void MudarLogoOrigemCredenciamento()
        {
            HiddenField hdnJsOrigem = (HiddenField)this.Master.FindControl("hdnJsOrigem");
            hdnJsOrigem.Value = String.Format("{0}-{1}", DadosCredenciamento.Canal, DadosCredenciamento.Celula);
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

                String logoPath = Server.MapPath(@"../Redecard.Comum/IMAGES/LogoRedeLaranja.png");
                byte[] array = File.ReadAllBytes(logoPath);
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
                base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ Métodos ]

        /// <summary>
        /// Carrega os dados da sessão para os controles da página
        /// </summary>
        private void CarregarDadosComprovante()
        {
            String query = Request["dados"];
            if (!String.IsNullOrEmpty(query))
            {
                QueryStringSegura _query = new QueryStringSegura(query);
                DadosCredenciamento.Comprovante = new ComprovanteVenda();
                DadosCredenciamento.CCMExecutada = true;

                ltlNSU.Text = DadosCredenciamento.Comprovante.NSU = HttpUtility.UrlDecode(_query["nsu"]);
                ltlTID.Text = DadosCredenciamento.Comprovante.TID = HttpUtility.UrlDecode(_query["tid"]);
                ltlNroEstabelecimento.Text = DadosCredenciamento.Comprovante.NumeroEstabelecimento = HttpUtility.UrlDecode(_query["numeroEstabelecimento"]);
                ltlNomeEstabelecimento.Text = DadosCredenciamento.Comprovante.NomeEstabelecimento = HttpUtility.UrlDecode(_query["nomeEstabelecimento"]);
                ltlDataPagamento.Text = DadosCredenciamento.Comprovante.DataPagamento = HttpUtility.UrlDecode(_query["dataPagamento"]);
                ltlHoraPagamento.Text = DadosCredenciamento.Comprovante.HoraPagamento = HttpUtility.UrlDecode(_query["horaPagamento"]);
                ltlNroAutorizacao.Text = DadosCredenciamento.Comprovante.NumeroAutorizacao = HttpUtility.UrlDecode(_query["numeroAutorizacao"]);
                ltlTipoTransacao.Text = DadosCredenciamento.Comprovante.TipoTransacao = HttpUtility.UrlDecode(_query["tipoTransacao"]);
                ltlFormaPagamento.Text = DadosCredenciamento.Comprovante.FormaPagamento = HttpUtility.UrlDecode(_query["formaPagamento"]);
                ltlBandeira.Text = DadosCredenciamento.Comprovante.Bandeira = HttpUtility.UrlDecode(_query["bandeira"]);
                ltlNomePortador.Text = DadosCredenciamento.Comprovante.NomePortador = "-";
                ltlNroCartao.Text = DadosCredenciamento.Comprovante.NumeroCartao = HttpUtility.UrlDecode(_query["numeroCartao"]);
                ltlValor.Text = DadosCredenciamento.Comprovante.Valor = HttpUtility.UrlDecode(_query["valor"]);
                ltlParcelas.Text = DadosCredenciamento.Comprovante.NumeroParcelas = HttpUtility.UrlDecode(_query["numeroParcelas"]);
                ltlNroPedido.Text = DadosCredenciamento.Comprovante.NumeroPedido = HttpUtility.UrlDecode(_query["numeroPedido"]);

                pnlLeitorSolicitado.Visible = true;
            }

            ltlLogradouro.Text = DadosCredenciamento.EnderecoCorrespondencia.Logradouro;
            ltlNumero.Text = DadosCredenciamento.EnderecoCorrespondencia.Numero;
            ltlComplemento.Text = DadosCredenciamento.EnderecoCorrespondencia.Complemento;
            ltlBairro.Text = DadosCredenciamento.EnderecoCorrespondencia.Bairro;
            ltlCidade.Text = DadosCredenciamento.EnderecoCorrespondencia.Cidade;
            ltlEstado.Text = DadosCredenciamento.EnderecoCorrespondencia.Estado;
            ltlCEP.Text = DadosCredenciamento.EnderecoCorrespondencia.CEP;
            ltlPais.Text = "Brasil";
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

            tabelaPDF.AddCell((new PdfPCell(new Phrase(new Phrase(DadosCredenciamento.Comprovante.NSU,
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
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.TID, true));

            tabelaPDF.AddCell(CreateLeftCell("N° Estabelecimento:", false));
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.NumeroEstabelecimento, false));

            tabelaPDF.AddCell(CreateLeftCell("Nome Estabelecimento:", true));
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.NomeEstabelecimento, true));

            tabelaPDF.AddCell(CreateLeftCell("Data do Pagamento:", false));
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.DataPagamento, false));

            tabelaPDF.AddCell(CreateLeftCell("Hora do Pagamento:", true));
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.HoraPagamento, true));

            tabelaPDF.AddCell(CreateLeftCell("N° Autorização:", false));
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.NumeroAutorizacao, false));

            tabelaPDF.AddCell(CreateLeftCell("Tipo de Transação:", true));
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.TipoTransacao, true));

            tabelaPDF.AddCell(CreateLeftCell("Forma de Pagamento:", false));
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.FormaPagamento, false));

            tabelaPDF.AddCell(CreateLeftCell("Bandeira:", true));
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.Bandeira, true));

            tabelaPDF.AddCell(CreateLeftCell("Nome do Portador:", false));
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.NomePortador, false));

            tabelaPDF.AddCell(CreateLeftCell("N° Cartão (Últimos 4 dig.):", true));
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.NumeroCartao, true));

            tabelaPDF.AddCell(CreateLeftCell("Valor:", false));
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.Valor, false));

            tabelaPDF.AddCell(CreateLeftCell("N° Parcelas:", true));
            tabelaPDF.AddCell(CreateRightCell(DadosCredenciamento.Comprovante.NumeroParcelas, true));

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
            tabelaPDF.AddCell(new PdfPCell(new Phrase(new Phrase(DadosCredenciamento.Comprovante.NumeroPedido,
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

        /// <summary>
        /// Atualiza a fase de filiação da proposta
        /// </summary>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        private Int32 AtualizaTaxaAtivacaoPropostaMPOS(out String mensagemRetorno)
        {
            return Servicos.AtualizaTaxaAtivacaoPropostaMPOS(DadosCredenciamento.TipoPessoa, DadosCredenciamento.CPF_CNPJ.CpfCnpjToLong(), DadosCredenciamento.NumeroSequencia,
                    DadosCredenciamento.TaxaAtivacao, 8, DadosCredenciamento.Usuario, out mensagemRetorno);
        }

        #endregion
    }
}
