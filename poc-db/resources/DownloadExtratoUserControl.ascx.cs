using System;
using System.Linq;
using System.Web.UI.WebControls;
using Redecard.PN.Emissores.Sharepoint.ServicoEmissores;
using System.Web.UI;
using Redecard.PN.Comum;
using System.ServiceModel;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Web;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;

namespace Redecard.PN.Emissores.Sharepoint.WebParts.DownloadExtrato
{
    public partial class DownloadExtratoUserControl : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                List<DownloadAnual> anos = new List<DownloadAnual>();

                for (int i = (DateTime.Today.Year); i >= DateTime.Today.Year - 5; i--)
                {
                    DdlDe.Items.Add(i.ToString());
                    DdlAte.Items.Add(i.ToString());
                }
                if (Request.QueryString["dados"] != null)
                {
                    QueryStringSegura qsDados = new QueryStringSegura(Request.QueryString["dados"]);
                    if (!string.IsNullOrEmpty(qsDados["Ano"].ToString()))
                    {
                        String ano = qsDados["Ano"].ToString();
                        DdlDe.SelectedValue = ano;
                        DdlAte.SelectedValue = ano;

                        ClickBuscar(sender, e);
                    }

                }

            }
        }

        private void GerarArquivoEmissor(String mes, String ano)
        {
            Stream streamBuffer = null;
            byte[] buffer = null;

            Logger.IniciarLog("Início método GerarArquivoEmissor");
            try
            {
                using (var context = new ContextoWCF<ArquivoEmissoresServico.ArquivoEmissoresServicoClient>())
                {
                    Logger.GravarLog("Chamada ao método DownloadArquivo ", new { SessaoAtual.CodigoEntidade, mes, ano });
                    streamBuffer = context.Cliente.DownloadArquivo(SessaoAtual.CodigoEntidade.ToString(), mes, ano);
                    Logger.GravarLog("Retorno chamada ao método DownloadArquivo ", new { });
                }

                if (streamBuffer == null)
                {
                    Logger.GravarLog("Retorno chamada ao método DownloadArquivo vazio", new { });
                    List<Panel> Painel = new List<Panel>();
                    ExibirPainelConfirmacaoAcao("Downlaod Arquivo", "Não há download para o período informado", Request.Url.AbsolutePath, Painel.ToArray(), "icone-aviso");
                    return;
                }
                Logger.GravarLog("Chamdno método Download", new { });

                Download(streamBuffer, "Download.txt");
            }
            catch (FaultException<PortalRedecardException> ex)
            {
                Logger.GravarErro("GerarArquivoEmissor - ", ex);
                ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                SharePointUlsLog.LogErro(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("GerarArquivoEmissor - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return;
            }
        }
        protected void btnDownload_Click(object sender, EventArgs e)
        {
            Button btnDownload = sender as Button;
            if (!String.IsNullOrEmpty(btnDownload.CommandArgument))
            {
                String[] argumentos = btnDownload.CommandArgument.Split('|');
                String mes, ano;
                if (argumentos.Length > 0)
                {
                    mes = "00" + argumentos[0];
                    mes = mes.Substring(mes.Length - 2, 2);
                    ano = argumentos[1];

                    GerarArquivoEmissor(mes, ano);
                }
            }
        }
        private void Download(Stream stremFile, string nomeArquivo)
        {
            byte[] fileBytes = null;
            byte[] buffer = new byte[4096];
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            int chunkSize = 0;
            do
            {
                chunkSize = stremFile.Read(buffer, 0, buffer.Length);
                memoryStream.Write(buffer, 0, chunkSize);
            } while (chunkSize != 0);

            memoryStream.Position = 0;
            fileBytes = memoryStream.ToArray();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/download";
            Response.AddHeader("content-disposition", String.Format("attachment;filename={0}", nomeArquivo));
            Response.AppendHeader("Content-Lenght", fileBytes.Length.ToString());

            Response.BinaryWrite(fileBytes);

            Response.Flush();
            Response.End();

        }

        protected void ClickBuscar(object sender, EventArgs e)
        {

            Logger.IniciarLog("Início método ClickBuscar");
            lblInvalido.Text = "";
            if (Convert.ToInt32(DdlDe.SelectedValue) > Convert.ToInt32(DdlAte.SelectedValue))
            {
                lblInvalido.Text = "Data inválida";
                pnlBuscar.Visible = false;
                return;
            }

            try
            {
                string anoDe = DdlDe.SelectedValue;
                string anoAte = DdlAte.SelectedValue;

                using (var context = new ContextoWCF<ArquivoEmissoresServico.ArquivoEmissoresServicoClient>())
                {
                    Logger.GravarLog("Chamada ao método ObterPeriodosDisponiveis ", new { SessaoAtual.CodigoEntidade, anoDe, anoAte });
                    List<ArquivoEmissoresServico.DownloadMes> meses = context.Cliente.ObterPeriodosDisponiveis(SessaoAtual.CodigoEntidade.ToString(), anoDe, anoAte);

                    Logger.GravarLog("Retorno chamada ao método ObterPeriodosDisponiveis ", new { meses });
                    if (meses.Count > 0)
                    {
                        rptDados.DataSource = meses;
                        rptDados.DataBind();
                        pnlBuscar.Visible = true;
                        ((QuadroAviso)qdAviso).Visible = false;
                    }
                    else
                    {
                        pnlBuscar.Visible = false;
                        ((QuadroAviso)qdAviso).Visible = true;
                        ((QuadroAviso)qdAviso).CarregarMensagem("Aviso", "Não há dados para o filtro informado!", false, "icone-aviso");

                    }
                }
                Logger.GravarLog("Retorno do método ", new { });
            }
            catch (FaultException<PortalRedecardException> ex)
            {
                Logger.GravarErro("ClickBuscar - ", ex);
                ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                SharePointUlsLog.LogErro(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("ClickBuscar - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return;
            }
        }

        protected void rptDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Literal ltlAno = (Literal)e.Item.FindControl("ltlAno");
                Literal ltlMes = (Literal)e.Item.FindControl("ltlMes");
                Button btnDownload = (Button)e.Item.FindControl("btnDownload");

                ArquivoEmissoresServico.DownloadMes item = (ArquivoEmissoresServico.DownloadMes)e.Item.DataItem;
                if (!object.Equals(item, null))
                {
                    ltlAno.Text = item.Ano.ToString();
                    ltlMes.Text = item.Mes.ToString();
                    btnDownload.CommandArgument = String.Format("{0}|{1}", item.MesId, item.Ano);

                }

            }
        }
    }
}


