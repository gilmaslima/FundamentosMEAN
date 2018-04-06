using Microsoft.SharePoint;
using System;
using System.Net;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TokenLogin : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                base.ValidarPermissao = false;

                if (Sessao.Contem() &&
                    !string.IsNullOrEmpty(SessaoAtual.TipoTokenApi) &&
                    !string.IsNullOrEmpty(SessaoAtual.TokenApi))
                {
                    RenovaTokenLogin();
                }
            }
            //Quando a API retorna um código HTTP diferente de 200 (OK), ocorre uma WebException
            catch (WebException ex)
            {
                SharePointUlsLog.LogErro(ex);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
            }
        }

        /// <summary>
        /// Chama a API para renovação do token do Login
        /// </summary>
        private void RenovaTokenLogin()
        {
            string tokenApiUrl = default(string);
            SPWeb web = default(SPWeb);

            if (SPContext.Current != null && SPContext.Current.Site != null && SPContext.Current.Site.RootWeb != null)
            {
                web = SPContext.Current.Site.RootWeb;

                if (web.AllProperties.ContainsKey("TokenApiUrl"))
                {
                    tokenApiUrl = web.AllProperties["TokenApiUrl"].ToString();
                }
            }

            if (!string.IsNullOrEmpty(tokenApiUrl))
            {
                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                ////Número máximo de conexões simultâneas permitidas por um objeto ServicePoint utilizado pelo WebRequest
                //ServicePointManager.DefaultConnectionLimit = 10;

                ////Tempo de "keep-alive" para as conexões do ServicePoint
                //ServicePointManager.MaxServicePointIdleTime = 10000;

                WebRequest request = PortalWebClient.GetWebRequest(tokenApiUrl);
                request.Method = "POST";
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
}
