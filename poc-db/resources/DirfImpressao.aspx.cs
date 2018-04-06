using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.DirfSolicita;
using Redecard.PN.Extrato.SharePoint.DirfAnual;
using Redecard.PN.Extrato.SharePoint.WebParts.Dirf.DirfConheca;
using System.Text;
using Redecard.PN.Extrato.SharePoint.Helper;
using System.Web;

namespace Redecard.PN.Extrato.SharePoint.Layouts.Redecard.PN.Extrato.SharePoint
{
    public partial class DirfImpressao : ApplicationPageBaseAnonima
    {
        private QueryStringSegura QS
        {
            get
            {
                if (Request.QueryString["dados"] != null)
                    return new QueryStringSegura(Request.QueryString["dados"]);
                else
                    return null;
            }
        }

        private Int16 AnoBase { get { return Request.QueryString["anoBase"].ToInt16(0); } }

        private String Tipo { get { return Request.QueryString["tipo"]; } }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger.IniciarLog("Dirf Impressão"))
            {
                var dirfSolicita = ucDirfSolicita as DirfSolicitaUserControl;
                dirfSolicita.ModoImpressao = true;
                dirfSolicita.Visualizar(AnoBase);

                hdnTipo.Value = Tipo;

                if (Tipo == "texto")
                {
                    pnlDirfExplicacao.Visible = false;
                    //pnlDescricaoRendimento.Visible = false;
                }
            }
        }    
    }
}
