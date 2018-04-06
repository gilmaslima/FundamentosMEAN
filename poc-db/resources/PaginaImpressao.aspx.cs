using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

using Redecard.PN.Comum;
using System.Web;
using System.IO;
using System.Linq;
using System.Web.UI;


using Winnovative.WnvHtmlConvert.PdfDocument;

using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Request.SharePoint.WebParts.RequestProtocolo;
using Redecard.PN.Request.SharePoint.WebParts.RequestResumoVendas;
using System.Drawing;

using System.Text;
using Winnovative.WnvHtmlConvert;
using Redecard.PN.Request.Core.Web.Controles.Portal;



namespace Redecard.PN.Request.SharePoint.Layouts.Request
{
    public partial class PaginaImpressao : ApplicationPageBaseAnonima
    {

        public string pagina
        {
            get { return ViewState["pagina"] != null ? ViewState["pagina"].ToString() : string.Empty; }
            set { ViewState["pagina"] = value; }
        }
        public string Tipo
        {
            get
            {
                if (Request.QueryString["dados"] != null)
                {
                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                    return queryString["tipo"];
                }
                else { return string.Empty; }
            }
        }
        public string Origem
        {
            get
            {
                if (Request.QueryString["dados"] != null)
                {
                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                    return queryString["origem"];
                }
                else { return string.Empty; }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lblErro.Text = string.Empty;
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["dados"] != null)
                {
                    MostraForm();

                    if (Origem.Equals("Protocolo"))
                        tdTitulo.InnerText = "Recebimento de Documentos";
                    else if (Origem.Equals("ResumoVendas"))
                        tdTitulo.InnerText = "Resumo de Vendas Após Cancelamento";
                }
            }
        }
        Control RetornaControle(string origem)
        {
            Control ctrl = null;

            if (origem.Equals("Protocolo"))
            {
                ctrl = (RequestProtocoloUserControl)LoadControl("~/_CONTROLTEMPLATES/Redecard.PN.Request.SharePoint.WebParts/RequestProtocolo/RequestProtocoloUserControl.ascx");
            }

            if (origem.Equals("ResumoVendas"))
            {
                ctrl = (RequestResumoVendasUserControl)LoadControl("~/_CONTROLTEMPLATES/Redecard.PN.Request.SharePoint.WebParts/RequestResumoVendas/RequestResumoVendasUserControl.ascx");
            }
            OcultarControles(ctrl);

            QuadroAcao acao = (QuadroAcao)ctrl.FindControl("qAcao");
            if (acao != null)
            {
                acao.Visible = false;
            }
            QuadroAtalho atalhos = (QuadroAtalho)ctrl.FindControl("qAtalho");
            if (atalhos != null)
            {
                atalhos.Visible = false;
            }
            MenuAcoes mnuAcoes = (MenuAcoes)ctrl.FindControl("mnuAcoes");
            if (mnuAcoes != null)
            {
                mnuAcoes.Visible = false;
            }
            return ctrl;
        }
        void OcultarControles(Control controles)
        {
            var itemsToBeDeleted = controles.Controls.Cast<Control>().ToList();
            foreach (Control childControl in itemsToBeDeleted)
            {
                if (childControl.GetType() == typeof(System.Web.UI.WebControls.HiddenField))
                {
                    ((System.Web.UI.WebControls.HiddenField)childControl).Visible = false;
                    controles.Controls.Remove(childControl);
                }
            }

            foreach (Control childControl in controles.Controls)
            {
                if (childControl.GetType() == typeof(System.Web.UI.WebControls.Button))
                {
                    ((System.Web.UI.WebControls.Button)childControl).Visible = false;

                }
                else
                {
                    OcultarControles(childControl);
                }
            }
        }
        void MostraForm()
        {
            try
            {

                ltlData.Text = DateTime.Now.ToString("dd/MM/yyyy 'às' HH'h'mm");

                //decodificação da query string com os dados do chargeback
                QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                if (Tipo.Equals("Imprimir"))
                {
                    Control controle = RetornaControle(queryString["origem"].ToString());

                    lblErro.Visible = false;
                    pnlImpressao.Controls.Add(controle);
                    Imprimir();
                }
                if (Tipo.Equals("Pdf"))
                {
                    Control controle = RetornaControle(queryString["origem"].ToString());

                    lblErro.Visible = false;
                    pnlImpressao.Controls.Add(controle);
                }
                if (Tipo.Equals("Excel"))
                {
                    Control controle = RetornaControle(queryString["origem"].ToString());

                    lblErro.Visible = false;
                    pnlImpressao.Controls.Add(controle);
                }

            }
            catch (Exception ex)
            {
                lblErro.Text = "erro ao recuperar as informações passadas via querystring. por favor, digite os dados desejados ou retorne a tela de comprovantes pendentes para selecionar um processo.";
            }
        }

        void ExportarExcel()
        {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename=" + Origem +"_"+DateTime.Now.ToString("yyyyMMdd")+ ".xls"));
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            //Abaixo codifica os caracteres para o alfabeto latino
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("Windows-1252");
            HttpContext.Current.Response.Charset = "ISO-8859-1";
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter htw = new HtmlTextWriter(sw))
                {
                    pnlImpressao.RenderControl(htw);

                    HttpContext.Current.Response.Write(sw.ToString());
                    HttpContext.Current.Response.End();
                }
            }
        }
        /// <summary>
        /// Captura o Render da página para fazer exportar para excel ou pdf quando selecionado
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {

            // setup a TextWriter to capture the markup
            TextWriter tw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(tw);

            // render the markup into our surrogate TextWriter
            base.Render(htw);

            // get the captured markup as a string
            pagina = tw.ToString();

            // render the markup into the output stream verbatim
            writer.Write(pagina);
            if (Tipo.Equals("Pdf"))
                ExportarPDF();
            if (Tipo.Equals("Excel"))
                ExportarExcel();
        }


        private void ExportarPDF()
        {

            try
            {
                string attachment = Origem + "_" + DateTime.Now.ToString("yyyyMMdd") + ".pdf";
                StringWriter stw = new StringWriter();

                HtmlTextWriter htextw = new HtmlTextWriter(stw);

                pnlImpressao.RenderControl(htextw);
                PdfConverter pdf = new PdfConverter();
                pdf.LicenseKey = "rIedjJ2Mnp+YjJmCnIyfnYKdnoKVlZWV";
                if (Origem.Equals("ResumoVendas"))
                {
                    pdf.PageHeight = 0;
                    pdf.PageWidth = 0;
                }
                byte[] file = pdf.GetPdfBytesFromHtmlString(pagina, this.CurrentRequestUrlAndQuery, this.CurrentRequestUrlAndQuery);

                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.AddHeader("Content-Type", "binary/octet-stream");
                response.AddHeader("Content-Disposition",
                    "attachment; filename=" + attachment + "; size=" + file.Length.ToString());
                response.Flush();
                response.BinaryWrite(file);
                response.Flush();
                response.End();


            }
            catch (Exception ex)
            {

                lblErro.Text = "erro ao recuperar as informações passadas via querystring. por favor, digite os dados desejados ou retorne a tela de comprovantes pendentes para selecionar um processo.";
            }
        }
        void Imprimir()
        {
            //Abrir tela de impressão
            ClientScript.RegisterStartupScript(GetType(), "print", "window.print();", true);
        }
    }
}
