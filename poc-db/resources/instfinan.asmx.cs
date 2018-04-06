using System;
using System.Net;
using System.Text;
using System.Web.Services;
using System.IO;
using System.Xml;

namespace Redecard.Portal.Aberto.WebParts.ISAPI.Redecard {

    /// <summary>
    /// Resultado da página de instituições financeiras
    /// </summary>
    public class instfinan : WebService {

        /// <summary>
        /// Retorna a cotação atual do site da Finan-Site
        /// http://finansite-a.ae.com.br/redecard/xml/cot_redecard.xml
        /// </summary>
        /// <returns></returns>
        [WebMethod()]
        public XmlDocument GetCotacao() {
            try {
                string sUrl = "http://finansite-a.ae.com.br/redecard/xml/cot_redecard.xml";
                using (WebClient client = new WebClient()) {
                    //IWebProxy proxy = HttpWebRequest.DefaultWebProxy;
                    //proxy.Credentials = new NetworkCredential("andre.garcia","Resource@123","resource_brsp");
                    //proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                    //client.Proxy = proxy;
                    //client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    //client.Encoding = Encoding.GetEncoding("ISO-8859-1");
                    string sContent = client.DownloadString(sUrl);
                    XmlDocument xml_Doc = new XmlDocument();
                    xml_Doc.LoadXml(sContent);
                    return xml_Doc;
                }
            }
            catch (Exception ex) {
                //TODO: Fazer tratamento para web services
            }
            return null;
        }
    }
}