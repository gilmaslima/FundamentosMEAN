/*
(c) Copyright [ANO] Redecard S.A.
Autor : [Tiago]
Empresa : [BRQ IT Services]
Histórico:
- [01/08/2012] – [Tiago] – [Etapa inicial]
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.RAV.Sharepoint.ModuloRAV;
using Redecard.PN.RAV.Sharepoint.WebParts.ComprovanteRavAvulso;
using System.IO;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.SharePoint;
using System.Collections.Specialized;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using System.ServiceModel;
using System.Web;

namespace Redecard.PN.RAV.Sharepoint.WebParts.RavAutomaticoComprovante
{
    public partial class RavAutomaticoComprovanteUserControl : UserControlBase
    {
        #region Constantes
        public const string FONTE = "RavAutomaticoComprovanteUserControl.ascx";
        public const Int32 COGIDO_ERRO_LOAD = 3010;
        public const Int32 COGIDO_ERRO_EMAIL = 3011;
        public const Int32 COGIDO_ERRO_SALVAR = 3012;
        public bool senha = false;

        #endregion

        #region Atributos
        private string _validaSenha = bool.FalseString;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Comprovante RAV Automático - Page Load"))
            {
                if (!Page.IsPostBack)
                {
                    try
                    {
                        if (Request.QueryString["dados"] != null)
                        {

                            QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                            if (string.IsNullOrEmpty(queryString["AcessoSenha"]))
                            {
                                Response.Redirect("pn_rav.aspx", false);
                                return;
                            }
                            if (queryString["AcessoSenha"].CompareTo(bool.TrueString) != 0)
                            {
                                Response.Redirect("pn_rav.aspx", false);
                                return;
                            }

                            SharePointUlsLog.LogMensagem(queryString["AcessoSenha"]);

                            _validaSenha = queryString["AcessoSenha"];


                            //ModRAVAutomatico automatico = (ModRAVAutomatico)Session["DadosRavAutomaticoComprovante"];
                            if (queryString["RavAutomaticoTipoAntecipacao"] != null)
                            {
                                lblTipoAntecipacao.Text = RAVComum.RetornaAliasProdutoAntecipacao(queryString["RavAutomaticoTipoAntecipacao"].ToString());
                            }

                            if (queryString["RavAutomaticoTipoVenda"] != null)
                            {
                                lblTipoVenda.Text = queryString["RavAutomaticoTipoVenda"].ToString().Replace('?', 'À');// automatico.TipoRAV.ToString();
                            }
                            if (queryString["RavAutomaticoPeriodoRecebimento"] != null)
                            {
                                lblRecebimento.Text = queryString["RavAutomaticoPeriodoRecebimento"].ToString().Replace('?', 'á');//automatico.Periodicidade.ToString();
                            }
                            if (queryString["RavAutomaticoValorMinimo"] != null)
                            {
                                lblValorMinimo.Text = queryString["RavAutomaticoValorMinimo"].ToString();//automatico.ValorMinimo.ToString();
                            }
                            if (queryString["RavAutomaticoBandeiras"] != null && !string.IsNullOrEmpty(queryString["RavAutomaticoBandeiras"].ToString()))
                            {
                                string[] bandeiras = queryString["RavAutomaticoBandeiras"].ToString().Split(';');
                                if (bandeiras.Length > 0)
                                {
                                    rptBandeiras.DataSource = bandeiras;
                                    rptBandeiras.DataBind();
                                }
                            }
                            if (queryString["RavAutomaticoTaxa"] != null)
                            {
                                lblTaxa.Text = queryString["RavAutomaticoTaxa"].ToString();//automatico.DadosRetorno.TaxaCategoria.ToString();
                            }
                            if (queryString["RavAutomaticoDataIni"] != null)
                            {
                                lblPeriodoAntecipacaoInicio.Text = queryString["RavAutomaticoDataIni"].ToString();//automatico.DataVigenciaIni.ToShortDateString();
                            }
                            if (queryString["RavAutomaticoDataFim"] != null)
                            {
                                lblPeriodoAntecipacaoFim.Text = queryString["RavAutomaticoDataFim"].ToString();//automatico.DataVigenciaFim.ToShortDateString();
                            }

                            Int32 codRetorno = 0;

                            using (var entidadeServico = new EntidadeServico.EntidadeServicoClient())
                            {
                                EntidadeServico.Entidade entidade = new EntidadeServico.Entidade();
                                entidade = entidadeServico.ConsultarDadosCompletos(out codRetorno, SessaoAtual.CodigoEntidade, false);

                                if (codRetorno == 0)
                                {
                                    lblCNPJ.Text = entidade.CNPJEntidade; //SessaoAtual.CNPJEntidade;
                                    lblRazaoSocial.Text = entidade.RazaoSocial; //SessaoAtual.NomeEntidade;
                                    lblNumEstabelecimento.Text = SessaoAtual.CodigoEntidade.ToString();
                                }
                            }
                        }
                    }
                    catch (FaultException<EntidadeServico.GeneralFault> ex)
                    {
                        Log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (FaultException<ServicoRAVException> ex)
                    {
                        Log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                        SharePointUlsLog.LogErro(ex);
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                        base.ExibirPainelExcecao(FONTE, COGIDO_ERRO_LOAD);
                        SharePointUlsLog.LogErro(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Método que envia e-mail conforme a numeroPV do cliente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Email(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Comprovante RAV Automático - E-mail"))
            {
                try
                {
                    //if (Session["DadosRAVAutomaticoComprovante"] == null)
                    //{
                    //    QueryStringSegura queryString = new QueryStringSegura();
                    //    queryString["AcessoSenha"] = _validaSenha;
                    //    Response.Redirect(string.Format(Request.UrlReferrer.AbsoluteUri + "?dados={0}", queryString.ToString()));
                    //}
                    //else
                    //{
                    using (ServicoPortalRAVClient cliente = new ServicoPortalRAVClient())
                    {
                        ModRAVEmailEntradaSaida dadosemails = cliente.ConsultarEmails(SessaoAtual.CodigoEntidade);
                        foreach (ModRAVEmail email in dadosemails.ListaEmails)
                        {
                            SPUtility.SendEmail(SPContext.Current.Web, false, false, email.Email, "Rede - Comprovante Antecipação Automática", retornaTabela().ToString());
                        }
                        this.ExibirMensagem("Comprovante Antecipação Automática", "O Comprovante foi enviado para o seu e-mail.");
                    }
                    //}
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, COGIDO_ERRO_EMAIL);
                    SharePointUlsLog.LogErro(ex);
                }
            }           
        }

        /// <summary>
        /// Método para exibir painel de informações
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="mensagem"></param>
        private void ExibirMensagem(string titulo, string mensagem)
        {
            using (Logger Log = Logger.IniciarLog("Método para exibir painel de informações - Comprovante RAV"))
            {
                try
                {
                    QueryStringSegura queryString = new QueryStringSegura();
                    queryString["AcessoSenha"] = _validaSenha;
                    queryString["RavAutomaticoValorMinimo"] = lblValorMinimo.Text;
                    queryString["RavAutomaticoTipoVenda"] = lblTipoVenda.Text;
                    queryString["RavAutomaticoPeriodoRecebimento"] = lblRecebimento.Text;
                    queryString["RavAutomaticoTaxa"] = lblTaxa.Text;
                    queryString["RavAutomaticoDataIni"] = lblPeriodoAntecipacaoInicio.Text;
                    queryString["RavAutomaticoDataFim"] = lblPeriodoAntecipacaoFim.Text;
                    queryString["RavAutomaticoTipoAntecipacao"] = lblTipoAntecipacao.Text.Replace("&atilde;", "ã").Replace("&eacute;", "é");
                    //Response.Redirect(string.Format("pn_ComprovanteRavAvulso.aspx?dados={0}", queryString.ToString()));
                    Panel[] paineisDados = new Panel[1]{
                            pnlDadosGerais
                    };

                    pnlDadosGerais.Visible = false;
                    base.ExibirPainelConfirmacaoAcao(titulo, mensagem, string.Format(SPUtility.GetPageUrlPath(HttpContext.Current) + "?dados={0}", queryString.ToString()), paineisDados);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, COGIDO_ERRO_EMAIL);
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }




        /// <summary>
        /// Método que salva os dados como PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Salvar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Comprovante RAV Automático - Salvar"))
            {
                try
                {
                    //ServicoPortalRAVClient cliente = new ServicoPortalRAVClient();

                    //if (Session["DadosRavAutomatico"] == null)
                    //{
                    //    QueryStringSegura queryString = new QueryStringSegura();
                    //    queryString["AcessoSenha"] = _validaSenha;
                    //    Response.Redirect(string.Format(Request.UrlReferrer.AbsoluteUri + "?dados={0}", queryString.ToString()));
                    //}

                    //if (Session["DadosRavAutomaticoComprovante"] != null)
                    //{
                    PdfPTable table = retornaTabela();
                    Document doc = new Document(PageSize.A4);
                    MemoryStream ms = new MemoryStream();
                    PdfWriter.GetInstance(doc, ms);
                    doc.Open();

                    //Adiciona Cabeçalho
                    string img = "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";
                    byte[] array = Convert.FromBase64String(img);
                    iTextSharp.text.Image headerImage = iTextSharp.text.Image.GetInstance(array);
                    headerImage.Alignment = iTextSharp.text.Image.LEFT_ALIGN;
                    doc.Add(headerImage);

                    doc.Add(table);
                    doc.Close();

                    byte[] file = ms.GetBuffer();
                    byte[] buffer = new byte[4096];

                    string nomeArquivo = string.Format("attachment; filename=Relatorio_{0}.pdf", DateTime.Now.ToString("ddMMyyyy_hhmmss"));

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", nomeArquivo);
                    Response.BinaryWrite(file);
                    //Response.Close();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    //}
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, COGIDO_ERRO_SALVAR);
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }

         /// <summary>
        /// Retorna os dados em uma tabela(motivo:não pegava estilos quando foi feito em divs)
        /// </summary>
        /// <returns></returns>
        private PdfPTable retornaTabela()
        {

            using (Logger Log = Logger.IniciarLog("Retorna dados de Tabela para PDF"))
            {
                //ModRAVAutomatico automatico = (Session["DadosRavAutomaticoComprovante"] as ModRAVAutomatico);
                PdfPTable table = new PdfPTable(2);
                table.SpacingBefore = 10f;
                //if (automatico != null)
                //{
                try
                {                    
                    table.AddCell((new PdfPCell(new Phrase(new Phrase("COMPROVANTE DE CADASTRO DE ANTECIPAÇÃO AUTOMÁTICA", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, Colspan = 2, BackgroundColor = new BaseColor(228, 228, 228), HorizontalAlignment = 0 }));

                    table.AddCell(new PdfPCell(new Phrase("   ")) { Colspan = 2 });

                    table.AddCell((new PdfPCell(new Phrase(new Phrase("Número do Estabelecimento:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(SessaoAtual.CodigoEntidade.ToString(), new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BorderWidthLeft = 0, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = 0 }));

                    table.AddCell((new PdfPCell(new Phrase(new Phrase("CNPJ:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(SessaoAtual.CNPJEntidade, new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BorderWidthLeft = 0, HorizontalAlignment = 0 }));

                    table.AddCell((new PdfPCell(new Phrase(new Phrase("Razão Social:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), BorderWidthRight = 0, HorizontalAlignment = PdfPCell.ALIGN_RIGHT }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(lblTipoVenda.Text, new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));

                    table.AddCell(new PdfPCell(new Phrase("   ")) { Colspan = 2 });

                    table.AddCell((new PdfPCell(new Phrase(new Phrase("Tipo de Venda:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(lblTipoVenda.Text, new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                    //table.AddCell((new PdfPCell(new Phrase(new Phrase(automatico.TipoRAV.ToString(), new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232,232,232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                    //MUDADO

                    table.AddCell((new PdfPCell(new Phrase(new Phrase("Tipo de Antecipação:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(lblTipoAntecipacao.Text.Replace("&atilde;", "ã").Replace("&eacute;", "é"), new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                    
                    if (rptBandeiras.Items.Count > 0)
                    {
                        foreach (RepeaterItem item in rptBandeiras.Items)
                        {
                            Label bandeira = (Label)item.FindControl("lblBandeira");
                            if (item.ItemIndex == 0)
                            {
                                table.AddCell((new PdfPCell(new Phrase(new Phrase("Bandeiras:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                                table.AddCell((new PdfPCell(new Phrase(new Phrase(bandeira.Text, new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                            }
                            else
                            {
                                if (item.ItemIndex % 2 != 0)
                                {
                                    table.AddCell((new PdfPCell(new Phrase(new Phrase("", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                                    table.AddCell((new PdfPCell(new Phrase(new Phrase(bandeira.Text, new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                                }
                                else
                                {
                                    table.AddCell((new PdfPCell(new Phrase(new Phrase("", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                                    table.AddCell((new PdfPCell(new Phrase(new Phrase(bandeira.Text, new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                                }
                            }
                        }
                    }
                    else
                    {
                        table.AddCell((new PdfPCell(new Phrase(new Phrase("Bandeiras:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                        table.AddCell((new PdfPCell(new Phrase(new Phrase("", new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                    }

                    // Dependendo do número de bandeiras selecionados pinta o background das linhas de uma cor diferente
                    if (rptBandeiras.Items.Count % 2 == 0)
                    {
                        table.AddCell((new PdfPCell(new Phrase(new Phrase("Recebimento:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                        table.AddCell((new PdfPCell(new Phrase(new Phrase(lblRecebimento.Text, new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                        //table.AddCell((new PdfPCell(new Phrase(new Phrase(automatico.Periodicidade.ToString(), new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232,232,232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                        //MUDADO

                        table.AddCell((new PdfPCell(new Phrase(new Phrase("Período de Antecipação:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                        table.AddCell((new PdfPCell(new Phrase(new Phrase(lblPeriodoAntecipacaoInicio.Text + " " + "a" + " " + lblPeriodoAntecipacaoFim.Text, new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                        //table.AddCell((new PdfPCell(new Phrase(new Phrase(automatico.DataVigenciaIni.ToShortDateString() + " " + "a" + " " + automatico.DataVigenciaFim.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = 0, BorderWidthLeft = 0 }));

                        table.AddCell((new PdfPCell(new Phrase(new Phrase("Taxa de Antecipação Automática:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                        table.AddCell((new PdfPCell(new Phrase(new Phrase(lblTaxa.Text, new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                        //table.AddCell((new PdfPCell(new Phrase(new Phrase(automatico.DadosRetorno.TaxaCategoria.ToString(), new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232,232,232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                    }
                    else
                    {
                        table.AddCell((new PdfPCell(new Phrase(new Phrase("Recebimento:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                        table.AddCell((new PdfPCell(new Phrase(new Phrase(lblRecebimento.Text, new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                        //table.AddCell((new PdfPCell(new Phrase(new Phrase(automatico.Periodicidade.ToString(), new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232,232,232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                        //MUDADO

                        table.AddCell((new PdfPCell(new Phrase(new Phrase("Período de Antecipação:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                        table.AddCell((new PdfPCell(new Phrase(new Phrase(lblPeriodoAntecipacaoInicio.Text + " " + "a" + " " + lblPeriodoAntecipacaoFim.Text, new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232, 232, 232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                        //table.AddCell((new PdfPCell(new Phrase(new Phrase(automatico.DataVigenciaIni.ToShortDateString() + " " + "a" + " " + automatico.DataVigenciaFim.ToShortDateString(), new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                    //MUDADO
                        table.AddCell((new PdfPCell(new Phrase(new Phrase("Taxa de Antecipação Automática:", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = PdfPCell.ALIGN_RIGHT, BorderWidthRight = 0 }));
                    table.AddCell((new PdfPCell(new Phrase(new Phrase(lblTaxa.Text, new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                    //table.AddCell((new PdfPCell(new Phrase(new Phrase(automatico.DadosRetorno.TaxaCategoria.ToString(), new Font(Font.FontFamily.HELVETICA, 12, 0, BaseColor.BLACK)))) { Padding = 5, BackgroundColor = new BaseColor(232,232,232), HorizontalAlignment = 0, BorderWidthLeft = 0 }));
                    }
                }
                catch (FaultException<ServicoRAVException> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    base.ExibirPainelExcecao(FONTE, COGIDO_ERRO_SALVAR);
                    SharePointUlsLog.LogErro(ex);
                }
            //}
                return table;
            }
        }
        
        internal static ServicoPortalRAVClient GetWebServiceInstance()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.SendTimeout = TimeSpan.FromMinutes(1);
            binding.OpenTimeout = TimeSpan.FromMinutes(1);
            binding.CloseTimeout = TimeSpan.FromMinutes(1);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
            binding.AllowCookies = false;
            binding.BypassProxyOnLocal = false;
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.TextEncoding = System.Text.Encoding.UTF8;
            binding.TransferMode = TransferMode.Buffered;
            binding.UseDefaultWebProxy = true;
            return new ServicoPortalRAVClient(binding, new EndpointAddress("http://localhost:36651/HIServiceMA_RAV.svc"));
        }

        protected void rptBandeiras_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    Label bandeira = (Label)e.Item.FindControl("lblBandeira");
                    bandeira.Text = e.Item.DataItem.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante bind de dados na tabela", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
            }
        }
    }
}
