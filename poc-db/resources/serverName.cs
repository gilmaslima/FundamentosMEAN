using System.Web.UI.WebControls;
using System.Web;

namespace Redecard.Portal.Aberto.WebControls {

    /// <summary>
    /// 
    /// </summary>
    public class serverName : WebControl {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer) {
            string sServerName = HttpContext.Current.Server.MachineName;
            // Verificar se o nome do servidor tem + de 4 caracteres, 
            // caso seja verdadeiro, remove as 4 primeiras letras do nome. 
            // Geralmente os nomes das máquinas FRONT-END do SharePoint começam
            // com "CARD".
            if (sServerName.Length > 4)
                sServerName = sServerName.Remove(0, 4);
            writer.Write(sServerName.ToUpperInvariant());
        }
    }
}