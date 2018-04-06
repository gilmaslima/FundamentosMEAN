using Redecard.PN.Comum;
using System;
using Redecard.PN.Extrato.SharePoint.Modelo;
using System.Web.UI;
using Microsoft.SharePoint;
using Redecard.PN.Extrato.SharePoint.Helper;

namespace Redecard.PN.Extrato.SharePoint.Layouts
{
    /// <summary>
    /// 
    /// </summary>
    public partial class RelatorioExcel : ApplicationPageBaseAnonima
    {
        /// <summary>
        /// Verificar ser o controle está sendo renderizado dentro de uma
        /// tag runat=server. Este método foi sobrescito para gerar o HTML do controle sem
        /// essa verificação
        /// </summary>
        public override void VerifyRenderingInServerForm(Control control) { }
        public override bool EnableEventValidation
        {
            get { return false; }
            set { }
        }

        protected override void OnPreRender(EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Download Relatório"))
            {
                try
                {
                    String html = String.Empty;
                    
                    QueryStringSegura query = new QueryStringSegura(Request.QueryString["dados"]);                    
                    Int32 maxLinhas = Int32.Parse(query["MAXLINHAS"]);
                    String relatorio = query["SRC"] as String;
                    String guidBuscarDados = query["GUID_DADOS"];
                    BuscarDados dados = BaseUserControl.RetiraInformacaoTransicaoSession<BuscarDados>(guidBuscarDados, Session);

                    //Carrega o controle
                    Control control = LoadControl(relatorio);
                    DownloadRelatorioCSV(control, dados, maxLinhas);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogMensagem("RelatorioExcel.aspx.cs");
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }

        /// <summary>Efetua o download do Relatório Excel</summary>
        private void DownloadRelatorioExcel(Control controle, BuscarDados dados, Int32 maxLinhas)
        {
            String nomeArquivo = String.Format("Relatorio_{0}.xls", DateTime.Now.ToString("ddMMyyyyHHmmss"));

            controle.ID = ((IRelatorioHandler)controle).IdControl;
            this.Page.Controls.Add(controle);

            String html = ((IRelatorioHandler)controle).ObterTabelaExcel(dados, maxLinhas, false);

            Response.Clear();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + nomeArquivo);
            Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
            Response.AppendHeader("Content-Length", html.Length.ToString());
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.ContentType = "application/ms-excel";
            Response.Write(html);
            Response.Flush();
            Response.End();
        }

        private void DownloadRelatorioCSV(Control controle, BuscarDados dados, Int32 maxLinhas)
        {
            String nomeArquivo = String.Format("Relatorio_{0}.xls", DateTime.Now.ToString("ddMMyyyyHHmmss"));

            Response.Clear();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + nomeArquivo);
            Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
            Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            Response.ContentType = "application/ms-excel";

            //Se relatório implementa interface para consulta paginada e escrita paginada
            if (controle is IRelatorioCSV)
            {
                var relatorio = (IRelatorioCSV)controle;
                controle.ID = relatorio.IdControl;
                this.Page.Controls.Add(controle);

                relatorio.GerarConteudoRelatorio(dados, (conteudoCSV) => {
                    Response.Write(conteudoCSV);
                    Response.Flush();
                });
            }
            //Caso contrário, chama método padrão de geração de relatório, com conversão para CSV
            else
            {                                
                String html = ((IRelatorioHandler)controle).ObterTabelaExcel(dados, maxLinhas, false);
                html = CSVExporter.GerarCSV(html, "\t");

                Response.AppendHeader("Content-Length", html.Length.ToString());
                Response.Write(html);
                Response.Flush();
            }

            Response.End();
        }
    }
}