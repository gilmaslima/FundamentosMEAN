using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe para Montagem das requisições HTTP do Portal
    /// </summary>
    public class PortalWebClient : WebClient
    {

        /// <summary>
        /// Instancia um novo WebRequest para um Serviço
        /// </summary>
        /// <param name="address">Endereço do Serviço</param>
        /// <returns></returns>
        public static WebRequest GetWebRequest(string address)
        {
            Uri uriEndereco = new Uri(address);

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uriEndereco);
            
            //Número máximo de conexões simultâneas permitidas por um objeto ServicePoint utilizado pelo WebRequest
            req.ServicePoint.ConnectionLimit = 1000;
            req.ServicePoint.Expect100Continue = false;
			req.ServicePoint.UseNagleAlgorithm = false;

            return (WebRequest)req;
        }
    }
}
