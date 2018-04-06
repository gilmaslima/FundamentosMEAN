using Redecard.PN.Comum;
using System;

namespace Redecard.PN.Request.SharePoint.WebParts.RequestProtocolo
{
    public partial class RequestProtocoloUserControl : UserControlBase
    {
        private QueryStringSegura QS
        {
            get
            {
                if (Request.QueryString["dados"] != null)
                    return new QueryStringSegura(Request.QueryString["dados"]);
                else
                    return null;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.QS != null)
            {
                CarregarDados(this.QS);
                hdfUrlImprimir.Value = MontarUrl("Imprimir");
                hdfUrlExcel.Value = MontarUrl("Excel");
                hdfUrlPdf.Value = MontarUrl("Pdf");
            }
            else
                SetarAviso("erro ao recuperar as informações passadas via querystring.");
        }

        private String MontarUrl(String tipo)
        {
            QueryStringSegura queryStringAnt = new QueryStringSegura(Request.QueryString["dados"]);
            QueryStringSegura queryString = new QueryStringSegura();
            queryString["NumEstabelecimento"] = queryStringAnt["NumEstabelecimento"];
            queryString["Documentos"] = queryStringAnt["Documentos"];
            queryString["DataEnvio"] = DateTime.Now.ToString("dd/MM/yyyy");
            queryString["HoraEnvio"] = DateTime.Now.ToString("HH'h'mm");
            queryString["origem"] = "Protocolo";
            queryString["tipo"] = tipo;
            return String.Format("/_layouts/Request/PaginaImpressao.aspx?dados={0}", queryString);
        }

        private void CarregarDados(QueryStringSegura queryString)
        {
            try
            {
                lblNomeEstabelecimento.Text = SessaoAtual.NomeEntidade;
                lblNumEstabelecimento.Text = queryString["NumEstabelecimento"];
                lblDocumentos.Text = queryString["Documentos"];
                lblDataEnvio.Text = queryString["DataEnvio"] + " às " + queryString["HoraEnvio"];
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnInicial_Click(object sender, EventArgs e)
        {
            Response.Redirect(base.web.ServerRelativeUrl + "/Paginas/pn_ComprovacaoVendas.aspx");
        }

        /// <summary>Mostra o painel de exceções</summary>        
        private void SetarAviso(String aviso)
        {
            lblMensagem.Text = aviso;
            lblMensagem.Visible = true;
            pnlErro.Visible = true;
            pnlConteudo.Visible = false;
        }
    }
}
