using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.VendasDuplicadas
{
    public partial class VendasDuplicadasUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Logger.GravarLog("Vendas Duplicadas - Page Load");
        }
    }
}
