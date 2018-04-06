using Microsoft.SharePoint;
using Redecard.PN.Comum;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Rede.PN.MultivanAlelo.Sharepoint.ControlTemplates.Voucher;

namespace Rede.PN.MultivanAlelo.Sharepoint.WebPartsVoucher.ConsultarStatusVoucher
{
    [ToolboxItemAttribute(false)]
    public partial class ConsultarStatusVoucher : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public ConsultarStatusVoucher()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        /// <summary>
        /// Ao carregar a página da webpart, carrega o controle de Status Voucher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarNovoLog("Carrega a página da webpart e o controle de Status Voucher"))
            {
                try
                {
                    var ucStatusVoucher = Page.LoadControl("~/_controltemplates/15/Voucher/StatusVoucher.ascx") as StatusVoucher;
                    pnlConsultaSolicitacao.Controls.Add(ucStatusVoucher);
                }
                catch (NullReferenceException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(
                            String.Format("Erro ao carregar o UserControl. \n Ex: {0}",
                            ex.ToString()));
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(
                            String.Format("Erro ao carregar o UserControl. \n Ex: {0}",
                            ex.ToString()));
                }
            }
        }
    }
}
