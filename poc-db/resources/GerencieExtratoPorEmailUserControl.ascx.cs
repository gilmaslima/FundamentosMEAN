using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates
{
    public partial class ExtratoPorEmailUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void drpDiaSemana_SelectedIndexChanged(object sender, EventArgs e)
        {   
            //todo: 
            //CarregaSemana(WA453CA_DIA_ENV)
        }
    }
}
