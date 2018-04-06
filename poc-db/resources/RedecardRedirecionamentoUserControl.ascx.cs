using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;

namespace Redecard.Portal.Aberto.WebParts.RedecardRedirecionamento
{
    public partial class RedecardRedirecionamentoUserControl : UserControl
    {
        #region Propriedades__________________

        private RedecardRedirecionamento WebPart
        {
            get
            {
                return (RedecardRedirecionamento)this.Parent;
            }
        }

        #endregion

        #region Constantes____________________

        #endregion

        #region Métodos_______________________

        private void InvocarUrl()
        {
            StringBuilder sbrEstilo = new StringBuilder();
            sbrEstilo.Append("<style type='text/css'>");
            sbrEstilo.Append("  iframe.consultaCliente {{");
            sbrEstilo.Append("      width: {0}px;");
            sbrEstilo.Append("      height: {1}px;");
            sbrEstilo.Append("      border: none;");
            sbrEstilo.Append("  }}");
            sbrEstilo.Append("</style>");
            this.ltrEstilo.Text = String.Format(sbrEstilo.ToString(), this.WebPart.Largura, this.WebPart.Altura);
            this.ifrUrl.Attributes.Add("src", this.WebPart.urlDestino);
        }

        #endregion

        #region Eventos_______________________

        protected void Page_Load(object sender, EventArgs e)
        {
            this.InvocarUrl();
        }

        #endregion
    }
}