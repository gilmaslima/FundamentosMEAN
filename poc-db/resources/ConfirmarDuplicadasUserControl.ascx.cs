using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.ConfirmarDuplicadas
{
    public partial class ConfirmarDuplicadasUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Logger.GravarLog("Confirmar Duplicadas - Page Load");
        }
    }
}
