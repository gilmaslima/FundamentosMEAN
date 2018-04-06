using System;
using System.Web;
using System.Web.UI;
using Microsoft.IdentityModel.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;

namespace Redecard.PN.Comum.SharePoint
{
    public partial class SessaoExpirada : ApplicationPageBaseAnonima
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Cancela Login do usuário caso ainda exista uma sessão no SharePoint
        /// </summary>
        private void CancelarLogin()
        {
            SPIisSettings iisSettingsWithFallback = SPContext.Current.Site.WebApplication.GetIisSettingsWithFallback(SPUrlZone.Default);
            if (iisSettingsWithFallback.UseClaimsAuthentication && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                FederatedAuthentication.SessionAuthenticationModule.SignOut();
            }
        }

        /// <summary>
        /// Faz o loggof caso ainda haja uma sessão aberta e redireciona o usuário para a página
        /// inicial do Portal
        /// </summary>
        protected void btnAcessar_Click(object sender, EventArgs e)
        {
            // cancelar login do usuário caso haja alguma sessão no SharePoint
            this.CancelarLogin();
            String url = String.Empty;
            using (SPSite site = new SPSite(SPContext.Current.Site.ID, SPUrlZone.Default))
            {
                url = site.MakeFullUrl("/");
            }
            Response.Redirect(url, true);
        }
    }
}