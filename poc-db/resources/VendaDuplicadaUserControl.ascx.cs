using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Redecard.PN.Cancelamento.Sharepoint.ServiceCancelamento;
using Redecard.PN.Comum;

namespace Redecard.PN.Cancelamento.Sharepoint.WebParts.VendaDuplicada
{
    public partial class VendaDuplicadaUserControl : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(base.SessaoAtual, null))
            {
                using (Logger Log = Logger.IniciarLog("Venda Duplicada - Page Load"))
                {
                    CarregaTabelaLista();
                }
            }
        }

        protected void Cancelamento_Voltar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Venda Duplicada - Voltar"))
            {
                Response.Redirect("pn_VendaDuplicadaConfirmar.aspx");
            }
        }

        private void CarregaTabelaLista()
        {
            if (Session["ItensDuplicados"] != null)
            {
                List<ModComprovante> comprovantesDuplicado = (List<ModComprovante>)Session["ItensDuplicados"];

                if (comprovantesDuplicado == null) comprovantesDuplicado = new List<ModComprovante>();
                rptProtocoloVendas.DataSource = comprovantesDuplicado;
                rptProtocoloVendas.DataBind();
            }
        }

    }
}
