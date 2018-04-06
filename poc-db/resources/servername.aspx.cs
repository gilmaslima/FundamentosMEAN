using System.Web.UI;
using System;
using System.Web;

namespace Redecard.Portal.Aberto.SD {

    /// <summary>
    /// Página de apresentação do nome do servidor
    /// </summary>
    public class serverName : Page {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e) {
            this.WriteServerName();
        }

        /// <summary>
        /// Recupera e escreve o nome do servidor na TELA
        /// </summary>
        private void WriteServerName() {
            string sServerName = HttpContext.Current.Server.MachineName;
            // Verificar se o nome do servidor tem + de 4 caracteres, 
            // caso seja verdadeiro, remove as 4 primeiras letras do nome. 
            // Geralmente os nomes das máquinas FRONT-END do SharePoint começam
            // com "CARD".
            if (sServerName.Length > 4)
                sServerName = sServerName.Remove(0, 4);
            Response.Write(sServerName.ToUpperInvariant());
            Response.End();
        }
    }
}