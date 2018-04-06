#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [14/05/2012] – [André Garcia] – [Criação]
- [26/01/2017] - [Roger Santos] - [Implementação da chamada de logout da API de Login]
*/
#endregion

using System;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI.WebControls;
using Microsoft.IdentityModel.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using System.Net;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login
{

    /// <summary>
    /// Controle de Logout do Usuário no Portal de Serviços
    /// </summary>
    public partial class Logout : UserControlBase
    {
        /// <summary>
        /// Inicialização do Controle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            this.ValidarPermissao = false;
            base.OnInit(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Método disparado para efetuar o logout do usuário no Portal de Serviços da Redecard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ConfirmarLogout(object sender, EventArgs e)
        {
            SPIisSettings iisSettingsWithFallback = SPContext.Current.Site.WebApplication.GetIisSettingsWithFallback(SPUrlZone.Default);
            if (iisSettingsWithFallback.UseClaimsAuthentication)
            {
                FederatedAuthentication.SessionAuthenticationModule.SignOut();
                LogoutApi();
                // Remover sessão do usuário
                LimparSessao();

                int num = 0;
                foreach (SPAuthenticationProvider provider in iisSettingsWithFallback.ClaimsAuthenticationProviders)
                {
                    num++;
                }
                if (num != 1 || !iisSettingsWithFallback.UseWindowsIntegratedAuthentication)
                {
                    String url = RecuperarEnderecoPortal();
                    Response.Redirect(url, true);
                    return;
                }
            }
            else
            {
                if (AuthenticationMode.Forms == SPSecurity.AuthenticationMode)
                {                   
                    FormsAuthentication.SignOut();
                    // Remover sessão do usuário
                    LimparSessao();
                    String url = RecuperarEnderecoPortal();
                    Response.Redirect(url, true);
                    return;
                }
                if (AuthenticationMode.Windows != SPSecurity.AuthenticationMode)
                {
                    throw new SPException();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static String RecuperarEnderecoPortal()
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            return url;
        }

        /// <summary>
        /// Limpa as variavéis de sessão do usuário atual
        /// </summary>
        private void LimparSessao()
        {
            Session.Abandon();
        }

        /// <summary>
        /// Chama a rota de logout da API do Login
        /// </summary>
        private void LogoutApi()
        {
            try
            {
                if (Sessao.Contem() &&
                    !string.IsNullOrEmpty(SessaoAtual.TipoTokenApi) &&
                    !string.IsNullOrEmpty(SessaoAtual.TokenApi))
                {
                    string logoutApiUrl = default(string);
                    SPWeb web = default(SPWeb);

                    if (SPContext.Current != null && SPContext.Current.Site != null && SPContext.Current.Site.RootWeb != null)
                    {
                        web = SPContext.Current.Site.RootWeb;

                        if (web.AllProperties.ContainsKey("LogoutApiUrl"))
                        {
                            logoutApiUrl = web.AllProperties["LogoutApiUrl"].ToString();
                        }
                    }

                    if (!string.IsNullOrEmpty(logoutApiUrl))
                    {
                        //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                        WebRequest request = PortalWebClient.GetWebRequest(logoutApiUrl);
                        request.Method = "DELETE";
                        request.ContentType = "application/x-www-form-urlencoded";

                        string parametrosToken = string.Format("authorization={0} {1}", SessaoAtual.TipoTokenApi, SessaoAtual.TokenApi);
                        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(parametrosToken);
                        request.ContentLength = byteArray.Length;

                        using (System.IO.Stream dataStream = request.GetRequestStream())
                        {
                            dataStream.Write(byteArray, 0, byteArray.Length);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                Logger.GravarErro("Erro ao efetuar o logout na API de Login", ex);
                SharePointUlsLog.LogErro(ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro ao efetuar o logout na API de Login", ex);
                SharePointUlsLog.LogErro(ex);
            }
        }

        /// <summary>
        /// Evento de clique do botão Voltar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("/sites/fechado");
        }
    }
}
