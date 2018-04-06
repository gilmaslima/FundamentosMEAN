using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Redecard.Portal.Aberto.WebControls {
    
    /// <summary>
    /// Classe que renderiza o link alternativo para o site di SharePoint atual, esta classe
    /// sobrescreve a CssLink, ela não renderiza os arquivos de CORE do SharePoint, que são muito
    /// pesados.
    /// </summary>
    public class RedecardCssLink : SPControl {

        /// <summary>
        /// Renderizar somente o CSS Alternativo
        /// </summary>
        /// <param name="output"></param>
        protected override void Render(System.Web.UI.HtmlTextWriter output) {
            if (!object.ReferenceEquals(SPContext.Current.Web, null)) {
                string cssUrl = SPContext.Current.Web.AlternateCssUrl;
                output.Write(String.Format("<link rel=\"Stylesheet\" type=\"text/css\" href=\"{0}\" />", cssUrl));
            }
        }
    }
}
