using System;
using System.Globalization;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Workflow;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using System.Collections.Generic;
using System.IO;
using Redecard.PN.Comum;

namespace Redecard.PN.Cancelamento.Sharepoint.LAYOUTS
{
    public partial class RelatorioCancelamento : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Relatório Cancelamento - Page Load"))
            {
                if (!IsPostBack)
                    CarregaTabelaLista();

                DateTime horaimpressao = DateTime.Now;
                //this.lblData.Text = "Data da Consulta: " + horaimpressao.ToShortDateString() + " às " + horaimpressao.ToShortTimeString();
            }
        }
        
        private void CarregaTabelaLista()
        {
            if (Session["ItensSaida"] != null)
            {
                List<ItemCancelamentoSaida> comprovantes = (List<ItemCancelamentoSaida>)Session["ItensSaida"];

                if (comprovantes == null) comprovantes = new List<ItemCancelamentoSaida>();

                rptProtocoloVendas.DataSource = comprovantes;
                rptProtocoloVendas.DataBind();
            }
        }        
    }
}
