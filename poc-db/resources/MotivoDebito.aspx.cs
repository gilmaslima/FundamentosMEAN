using System;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.DebitosDesagendamento;

namespace Redecard.PN.Extrato.SharePoint.Layouts.Redecard.PN.Extrato.SharePoint
{
    public partial class MotivoDebito : ApplicationPageBaseAutenticada
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger.IniciarLog("Motivo Débito"))
            {
                MotivoDebitoDetalhe objUserControl = LoadControl("~/_CONTROLTEMPLATES/Extrato/DebitosDesagendamento/MotivoDebitoDetalhe.ascx") as MotivoDebitoDetalhe;
                Control objControl = this.FindControl("tdDebitoDetalhe");
                objControl.Controls.Add(objUserControl);
            }
        }

    }
}
