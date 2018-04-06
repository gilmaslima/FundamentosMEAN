using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.Boston.Sharepoint.WebParts.TransacaoNaoAprovada
{
    public partial class TransacaoNaoAprovadaUserControl : UserControl
    {
        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Evento do Clique no botão continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("pn_PedidoNovoEquipamento.aspx");
        }
    }
}
