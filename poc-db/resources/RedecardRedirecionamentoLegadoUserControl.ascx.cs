using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web;
using System.Text;
using System.Collections.Generic;
using Redecard.PN.Comum;

namespace Redecard.PN.OutrasEntidades.SharePoint.WebParts.RedecardRedirecionamentoLegado
{
    public partial class RedecardRedirecionamentoLegadoUserControl : UserControlBase
    {

        #region Propriedades__________________

        private RedecardRedirecionamentoLegado WebPart
        {
            get
            {
                return (RedecardRedirecionamentoLegado)this.Parent;
            }
        }

        #endregion

        #region Constantes____________________

        #endregion

        #region Métodos_______________________

        private void InvocarLegado()
        {
            if (object.ReferenceEquals(this.SessaoAtual, null))
                return;

            string sTokenValue = string.Empty;
            string sSessionId = string.Empty;

            sTokenValue = this.SessaoAtual.TokenLegado;
            sSessionId = this.SessaoAtual.IDSessao.ToString();

            StringBuilder sbrEstilo = new StringBuilder();
            sbrEstilo.Append("<style type='text/css'>");
            sbrEstilo.Append("  #ifrLegado {{");
            sbrEstilo.Append("      width: {0};");
            sbrEstilo.Append("      height: {1};");
            sbrEstilo.Append("      border: none;");
            sbrEstilo.Append("  }}");
            sbrEstilo.Append("</style>");
            this.ltrEstilo.Text = String.Format(sbrEstilo.ToString(), this.WebPart.Largura, this.WebPart.Altura);

            StringBuilder sbrFormulario = new StringBuilder();
            sbrFormulario.Append("<script language='javascript' type='text/javascript'>");
            sbrFormulario.Append("   $('#ifrmLegadoContainer').append(\"");
            sbrFormulario.Append("       <form id='frmLegado' action='{0}' target='ifrLegado' method='post'>");
            sbrFormulario.Append("          <input id='token' name='token' type='text' value='{1}' />");
            sbrFormulario.Append("          <input id='token' name='ISSessionID' type='text' value='{2}' />");
            sbrFormulario.Append("          <input id='url' name='urlDestino' type='text' value='{3}' />");
            sbrFormulario.Append("          <input type='submit' />");
            sbrFormulario.Append("       </form>");
            sbrFormulario.Append("   \");");
            if (!String.IsNullOrEmpty(this.WebPart.urlLegado))
                sbrFormulario.Append("   $('#frmLegado').submit();");
            sbrFormulario.Append("</script>");
            this.ltrFormulario.Text = String.Format(sbrFormulario.ToString(), this.WebPart.urlLegado, sTokenValue, sSessionId, this.WebPart.urlDestino);

        }

        #endregion

        #region Eventos_______________________

        protected void Page_Load(object sender, EventArgs e)
        {
            this.InvocarLegado();
        }

        #endregion

    }
}
