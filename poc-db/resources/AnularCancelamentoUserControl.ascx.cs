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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using System.Collections.Generic;
using Redecard.PN.Comum;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.AnularCancelamento
{
    public partial class AnularCancelamentoUserControl : UserControlBase
    {

        #region Constantes
        public const string NOME_PAGINA = "AnularCancelamentoUserControl.aspx";
        public const int CODIGO_ERRO_LOAD = 3012;
        public const int CODIGO_ERRO_ANULACAO = 3013;
        public const int CODIGO_ERRO_SALVAR = 3014;
        public const string IP_USUARIO = "172.16.4.86";
        #endregion

        #region Metodos de Pagina
        /// <summary>
        /// Metodo de inicialização da tela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(base.SessaoAtual, null))
            {
                using (Logger Log = Logger.IniciarLog("Anular Cancelamento - Page Load"))
                {
                    EstabelecimentoCancelamento estabelecimento;
                    using (ServicoPortalCancelamentoClient client = new ServicoPortalCancelamentoClient())
                    {
                        estabelecimento = client.RetornaDadosEstabelecimentoCancelamento(this.SessaoAtual.CodigoEntidade);

                    }

                    if (estabelecimento == null || !estabelecimento.Centralizadora)
                    {
                        // ScriptManager.RegisterStartupScript(Page, GetType(), "key", "$('.ocultar').hide();", true);
                        ScriptManager.RegisterStartupScript(Page, GetType(), "key", "$('.ocultar').hide();", true);
                        ScriptManager.RegisterStartupScript(Page, GetType(), "key", "document.getElementsByClassName('ocultar').style.display = 'none';", true);
                    }

                    if (!Page.IsPostBack)
                    {
                        //if (SessaoAtual.TipoUsuario == "Centralizador") { 
                        //ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Informe um arquivo para upload.');", true);
                        //}

                        try
                        {
                            using (ServiceCancelamento.ServicoPortalCancelamentoClient client = new ServicoPortalCancelamentoClient())
                            {
                                List<ModComprovante> lista = client.ConsultaAnulacao(this.SessaoAtual.CodigoEntidade);
                                SharePointUlsLog.LogMensagem("Registros Retorno: " + lista.Count.ToString());
                                Log.GravarMensagem("Registros Retorno: " + lista.Count.ToString());
                                if (lista == null) lista = new List<ModComprovante>();

                                Session["ItensAnulamento"] = lista;

                                while (lista.Count < 10)
                                {
                                    ModComprovante itemModAnularCancelamento = new ModComprovante();

                                    lista.Add(itemModAnularCancelamento);
                                }

                                ExecutarBindFiliais(lista);

                                SharePointUlsLog.LogMensagem("Registros Bind: " + lista.Count.ToString());
                                Log.GravarMensagem("Registros Bind: " + lista.Count.ToString());

                                int contador = 0;
                                foreach (RepeaterItem itemrptDados in rptDados.Items)
                                {

                                    Label numCNo = (Label)(rptDados.Items[contador].FindControl("numCno"));
                                    CheckBox CheckExcluir = (CheckBox)(rptDados.Items[contador].FindControl("chkSel"));

                                    Label TipoCancelamento = (Label)(rptDados.Items[contador].FindControl("TipoCancelamento"));
                                    Label TipoTransacao = (Label)(rptDados.Items[contador].FindControl("TipoTransacao"));

                                    //if (TipoCancelamento.Text.ToLower() == "T")
                                    //{
                                    //     TipoCancelamento.Text == "Total";
                                    //}

                                    //if (TipoCancelamento.Text.ToLower() == "P")
                                    //{
                                    //    TipoCancelamento.Text == "Parcial";
                                    //}

                                    if (numCNo.Text == "")
                                    {
                                        CheckExcluir.Visible = false;
                                    }
                                    contador++;
                                }

                            }

                        }
                        catch (PortalRedecardException exp)
                        {
                            this.ExibirPainelExcecao(exp);
                            SharePointUlsLog.LogErro(exp.Message);
                            Log.GravarErro(exp);
                        }
                        catch (Exception ex)
                        {
                            this.ExibirPainelExcecao(NOME_PAGINA, CODIGO_ERRO_LOAD);
                            SharePointUlsLog.LogErro(ex.Message);
                            Log.GravarErro(ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Metodo chamado no clique do botão anular
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AnularSelecionados_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Anular Cancelamento - Anular Selecionados"))
            {
                List<ModComprovante> listAnular = (List<ModComprovante>)Session["ItensAnulamento"];
                List<ModAnularCancelamento> listSaida = new List<ModAnularCancelamento>();

                List<ModComprovante> listAnularExcluir = new List<ModComprovante>();

                foreach (RepeaterItem itemrptDados in rptDados.Items)
                {
                    if (itemrptDados.ItemType == ListItemType.AlternatingItem || itemrptDados.ItemType == ListItemType.Item)
                    {
                        Label NumCartao = itemrptDados.FindControl("nuCartao") as Label;
                        Label NumNsu = itemrptDados.FindControl("nuNsu") as Label;
                        Label NumCno = itemrptDados.FindControl("numCno") as Label;
                        CheckBox CheckExcluir = itemrptDados.FindControl("chkSel") as CheckBox;

                        if (!object.ReferenceEquals(NumCartao, null)
                            && !object.ReferenceEquals(NumNsu, null)
                            && !object.ReferenceEquals(CheckExcluir, null))
                        {
                            if (CheckExcluir.Checked)
                            {
                                ModComprovante comprovante = listAnular.Where(x => x.NumNSU.Equals(NumNsu.Text) && x.NumCartao.Equals(NumCartao.Text) && x.NumAvisoCancelamentoFormatada.Equals(NumCno.Text)).FirstOrDefault();
                                if (comprovante != null)
                                {
                                    listAnularExcluir.Add(comprovante);
                                }
                            }
                        }
                    }
                }

                if (listAnularExcluir.Count > 0)
                {

                    int proxima = listAnularExcluir.Count;

                    Session["ItensAnulamento"] = listAnularExcluir;

                    try
                    {
                        foreach (ModComprovante comprovante in listAnularExcluir)
                        {
                            System.Diagnostics.Trace.WriteLine("ANULAR CANC. - COD CANC" + comprovante.CodigoCancelamento);
                            System.Diagnostics.Trace.WriteLine("ANULAR CANC. - NUM AVSO" + comprovante.NumAvisoCancel);
                            System.Diagnostics.Trace.WriteLine("ANULAR CANC. - NUM  NSU" + comprovante.NumNSU);
                            System.Diagnostics.Trace.WriteLine("ANULAR CANC. - NUM CART" + comprovante.NumCartao);
                            System.Diagnostics.Trace.WriteLine("ANULAR CANC. - NUM ESTB" + comprovante.NumEstabelecimento);
                            System.Diagnostics.Trace.WriteLine("ANULAR CANC. - DES ESTB" + comprovante.DescEstabelecimento);
                            System.Diagnostics.Trace.WriteLine("ANULAR CANC. - NUM   PV" + comprovante.NumPV);
                        }

                        using (ServicoPortalCancelamentoClient client = new ServicoPortalCancelamentoClient())
                        {
                            listSaida = client.RealizarAnulacaoCancelamento(this.Truncar(this.SessaoAtual.LoginUsuario, 20), IP_USUARIO, listAnularExcluir);
                            Session["ItensAnulamentoSaida"] = listSaida;
                        }
                    }
                    catch (PortalRedecardException exp)
                    {
                        this.ExibirPainelExcecao(exp);
                        SharePointUlsLog.LogErro(exp.Message);
                        Log.GravarErro(exp);
                    }
                    catch (Exception ex)
                    {
                        this.ExibirPainelExcecao(NOME_PAGINA, CODIGO_ERRO_ANULACAO);
                        SharePointUlsLog.LogErro(ex.Message);
                        Log.GravarErro(ex);
                    }

                    try
                    {
                        if (proxima > 0)
                        {
                            Response.Redirect("pn_ComprovanteAnulacao.aspx");
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(Page, GetType(), "key", "alert('Selecionar um registro para anulação.');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.GravarErro(ex);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="tamanho"></param>
        protected String Truncar(String valor, Int32 tamanho)
        {
            if (valor.Length > tamanho)
            {
                return valor.Substring(0, 20);
            }
            else
                return valor;
        }

        /// <summary>
        /// Método que salva os dados como PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Salvar(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Anular Cancelamento - Salvar PDF"))
            {
                try
                {
                    PdfPTable table = RetornaTabela();
                    Document doc = new Document(PageSize.A4);
                    MemoryStream ms = new MemoryStream();
                    PdfWriter.GetInstance(doc, ms);
                    doc.Open();
                    doc.Add(table);
                    doc.Close();

                    byte[] file = ms.GetBuffer();
                    byte[] buffer = new byte[4096];

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment; filename=Comprovante.pdf");
                    Response.BinaryWrite(file);
                    Response.End();                    
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex.Message);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO_SALVAR);
                    Log.GravarErro(ex);
                }
            }
        }

        private PdfPTable RetornaTabela()
        {
            PdfPTable table = new PdfPTable(2);

            table.AddCell((new PdfPCell(new Phrase(new Phrase("COMPROVANTE DE SOLICITAÇÃO DE RAV AVULSO", new Font(Font.FontFamily.HELVETICA, 12, 1, BaseColor.WHITE)))) { Padding = 5, Colspan = 2, BackgroundColor = new BaseColor(25, 60, 145), HorizontalAlignment = 0 }));

            table.AddCell(new PdfPCell(new Phrase("   ")) { Colspan = 2 });

            return table;
        }


        /// <summary>
        /// Metodo executado na clique do botão voltar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Cancelamento_Voltar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Anular Cancelamento - Voltar"))
            {
                this.Session.Remove("ItensAnulamento");

                this.Session.Remove("ItensAnulamentoSaida");
                try
                {
                    Response.Redirect("pn_cancelamento.aspx");
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                }
            }
        }
        #endregion

        #region Metodos Privados
        /// <summary>
        /// Metodo para carregar a grid com os cancelamentos.
        /// </summary>
        /// <param name="filiais"></param>
        private void ExecutarBindFiliais(List<ModComprovante> filiais)
        {
            rptDados.DataSource = filiais;
            rptDados.DataBind();
        }

        #endregion
    }
}
