using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Rede.PN.CondicaoComercial.SharePoint.Business;
using Redecard.PN.Comum;
using System;
using System.Globalization;
using System.Text;

namespace Rede.PN.CondicaoComercial.SharePoint.WebParts.TaxasCreditoDebito
{
    /// <summary>
    /// Pagina que envolve Bandeiras | Terminais | Ofertas
    /// </summary>
    public partial class TaxasCreditoDebitoUserControl : UserControlBase
    {
        /// <summary>
        /// Page load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Clique para baixar as condicoes comerciais
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void mnuAcoes_ClickPdf(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("TaxasCreditoDebitoUserControl -  mnuAcoes_ClickPdf - Download das Condições comerciais"))
            {
                try
                {
                    String url;

                    QueryStringSegura queryString = new QueryStringSegura();
                    queryString.Add("NUM_PDV", SessaoAtual.CodigoEntidade.ToString());
                    queryString.Add("ModoReimpressao", Boolean.TrueString);

#if DEBUG
                    //Esta é a versão somente para testes locais.
                    using (SPSite site = new SPSite(SPContext.Current.Site.ID, SPUrlZone.Default))
                        url = site.MakeFullUrl(String.Format("/_layouts/Redecard.PN.Extrato.SharePoint/DownloadAceiteCondicoesComerciais.aspx?num_pdv={0}", queryString.ToString()));
# else
                    //Esta é a versão para ambientes Rede.
                    StringBuilder urlComposta = new StringBuilder();
                    urlComposta.Append("http://");
                    urlComposta.Append("localhost/");
                    urlComposta.Append("_layouts/Redecard.PN.Extrato.SharePoint/DownloadAceiteCondicoesComerciais.aspx?num_pdv=");
                    urlComposta.Append(queryString.ToString());
                    url = urlComposta.ToString();
#endif

                    log.GravarMensagem(url);

                    Pdf pdf = new Pdf();
                    byte[] pdfBytes = pdf.GerarPdfUrl(url);
                    log.GravarMensagem("GerarPdfUrl", pdfBytes);
                    Response.AddHeader("Content-Type", "application/pdf");
                    Response.AddHeader("Content-Disposition", String.Format("attachment; filename=Condicoes Comerciais {0}.pdf; size={1}", DateTime.Now.Ticks, pdfBytes.Length.ToString(CultureInfo.InvariantCulture)));
                    Response.BinaryWrite(pdfBytes);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    SharePointUlsLog.LogMensagem(String.Concat("@@@Taxas credito e debito: ", ex.StackTrace));
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    SharePointUlsLog.LogMensagem(String.Concat("@@@Taxas credito e debito: ", ex.StackTrace));
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                Response.Flush();
                Response.End();
            }
        }
    }
}
