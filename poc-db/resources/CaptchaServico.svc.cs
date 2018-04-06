using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.Servicos.Modelos;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Web.Script.Serialization;

namespace Redecard.PN.DadosCadastrais.Servicos
{
    /// <summary>
    /// Serviço para validações de capctha
    /// </summary>
    public class CaptchaServico : ServicoBase, ICaptchaServico
    {
        /// <summary>
        /// Valida na api do google se o captcha informado na tela esta válido
        /// </summary>
        /// <param name="sharedKey"></param>
        /// <param name="captchaResponse"></param>
        /// <returns></returns>
        public bool ValidarCaptcha(string sharedKey, string captchaResponse)
        {
            using (Logger Log = Logger.IniciarLog("Validando o captcha do lado do servidor"))
            {
                Log.GravarLog(EventoLog.InicioServico);

                CaptchaResponse result = new CaptchaResponse();
                result.Success = false;

                try
                {

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    string urlApiCaptcha = ConfigurationManager.AppSettings["UrlApiCaptcha"];

                    string hostProxy = ConfigurationManager.AppSettings["HostProxy"];
                    string portProxy = ConfigurationManager.AppSettings["PortProxy"];

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Concat(urlApiCaptcha, "?secret=", sharedKey, "&response=", captchaResponse));
                    request.Method = "POST";
                    request.ContentLength = 0;

                    WebProxy proxy = new WebProxy(hostProxy, Convert.ToInt32(portProxy));
                    proxy.BypassProxyOnLocal = false;

                    request.Proxy = proxy;               
                    
                    string content = string.Empty;
                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream responseStream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(responseStream);
                            content = reader.ReadToEnd();
                        }
                    }

                    if (!String.IsNullOrEmpty(content))
                    {
                        result = serializer.Deserialize<CaptchaResponse>(content);
                    }
                    return result.Success;
                }
                catch (NullReferenceException ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    throw new FaultException<GeneralFault>(
                        new GeneralFault(CODIGO_ERRO, FONTE), base.RecuperarExcecao(ex));
                }
            }

        }
    }
}
