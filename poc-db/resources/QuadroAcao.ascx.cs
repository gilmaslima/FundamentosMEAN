using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Microsoft.SharePoint;
using System.Web;
using System.IO;
using Winnovative.WnvHtmlConvert;
using System.Net;
using System.Text;
using Winnovative.WnvHtmlConvert.PdfDocument;
using System.Drawing;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{
    public partial class QuadroAcao : UserControl
    {
        private string _origem = string.Empty;
        public string Origem
        {
            get { return _origem; }
            set { _origem = value; }
        }
        private string _caminhoPaginaImpressao = string.Empty;
        public string CaminhoPaginaImpressao
        {
            get { return _caminhoPaginaImpressao; }
            set { _caminhoPaginaImpressao = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lnkImprimir.OnClientClick = "fImpressao('" + SPContext.Current.Web.ServerRelativeUrl + FormarUrl("Imprimir") + "'); return false;";
                lnkExcel.OnClientClick = "fImpressao('" + SPContext.Current.Web.ServerRelativeUrl + FormarUrl("Excel") + "'); return false;";
                lnkPdf.OnClientClick = "fImpressao('" + FormarUrl("Pdf") + "'); return false;";
            }
        }

        private string FormarUrl(string tipo)
        {
            QueryStringSegura queryStringAnt = new QueryStringSegura(Request.QueryString["dados"]);
            
            QueryStringSegura queryString = new QueryStringSegura();
            if (Origem.Equals("Protocolo"))
            {
                queryString["NumEstabelecimento"] = queryStringAnt["NumEstabelecimento"];
                queryString["Documentos"] = queryStringAnt["Documentos"];
            }
            else
            {
                queryString["NumProcesso"] = queryStringAnt["NumProcesso"];
                queryString["PV"] = queryStringAnt["PV"];
                queryString["TipoVenda"] = queryStringAnt["TipoVenda"];

            }
            queryString["DataEnvio"] = DateTime.Now.ToString("dd/MM/yyyy");
            queryString["HoraEnvio"] = DateTime.Now.ToString("HH'h'mm");
            queryString["origem"] = Origem;
            queryString["tipo"] = tipo;
            return CaminhoPaginaImpressao + "?dados=" + queryString.ToString();
        }


        protected void lnkImprimir_Click(object sender, EventArgs e)
        {

        }

        protected void lnkPdf_Click(object sender, EventArgs e)
        {


            string baseURL = SPContext.Current.Web.Url + FormarUrl("Pdf");
            PdfConverter pdfConverter = new PdfConverter();

            // call the converter and get a Document object from URL
            //Document pdfDocument = pdfConverter.GetPdfBytesFromHtmlString(SPContext.Current.Web.GetFileAsString(baseURL));

            byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(SPContext.Current.Web.GetFileAsString(baseURL));
            /*
            try
            {
                pdfBytes = pdfDocument.Save();
            }
            finally
            {
                // close the Document to realease all the resources
                pdfDocument.Close();
            }*/

            // send the PDF document as a response to the browser for download
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.Clear();
            response.AddHeader("Content-Type", "binary/octet-stream");
            response.AddHeader("Content-Disposition",
                "attachment; filename=ConversionResult.pdf; size=" + pdfBytes.Length.ToString());
            response.Flush();
            response.BinaryWrite(pdfBytes);
            response.Flush();
            response.End();
        }


        protected void lnkExcel_Click(object sender, EventArgs e)
        {
            // lnkExcel.OnClientClick = "fImpressao('" + FormarUrl("Excel") + "'); return false;";
        }
    }
}

