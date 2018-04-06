using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Redecard.PN.Comum.BlacklistValidations
{
    public class BlacklistIPs
    {
        /// <summary>
        /// Retorna a lista dos IPs bloqueados cadastrados no SharePoint
        /// </summary>
        public SPList ListaIPsBloqueados
        {
            get
            {
                SPList lista = null;
                // SPUtility.ValidateFormDigest();

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite spSite = SPContext.Current.Site.WebApplication.Sites["sites/fechado"])
                    using (SPWeb spWeb = spSite.AllWebs["minhaconta"])
                    {
                        lista = spWeb.Lists.TryGetList("IPs Bloqueados para Criação de Acesso");
                    }
                });

                return lista;
            }
        }

        /// <summary>
        /// Retorna o filtro dos IPs bloqueados aplicado à lista do SharePoint
        /// </summary>
        public List<string> IPsBloqueados
        {
            get
            {
                try
                {
                    var ipsBloqueados = ListaIPsBloqueados;
                    if (ipsBloqueados != null)
                    {
                        var query = new SPQuery();
                        query.Query = String.Concat(
                            "<Where>",
                                "<Eq>",
                                    "<FieldRef Name=\"Ativo\" />",
                                    "<Value Type=\"Boolean\">1</Value>",
                                "</Eq>",
                            "</Where>");

                        return ipsBloqueados.GetItems(query)
                            .Cast<SPListItem>()
                            .Select(x => Convert.ToString(x["Endereço IP"]))
                            .ToList();
                    }
                }
                catch (SPException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                }

                return new List<string>();
            }
        }

        /// <summary>
        /// Valida se IP é válido, segundo a lista de IPs bloqueados
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool ValidarIP(string ip)
        {
            List<string> ipReprovado;
            return this.ValidarIPs(new List<string> { ip }, out ipReprovado);
        }

        /// <summary>
        /// Valida se IPs informados não se encontram na Blacklist de IPs
        /// </summary>
        /// <param name="ipsValidar">Listagem dos IPs a serem validados</param>
        /// <returns>
        ///     TRUE: IPs validados
        ///     FALSE: Algum IP inválido
        /// </returns>
        public bool ValidarIPs(List<string> ipsValidar, out List<string> ipsReprovados)
        {
            ipsReprovados = new List<string>();
            foreach (string ip in ipsValidar)
            {
                if (this.IPsBloqueados.Contains(ip))
                {
                    ipsReprovados.Add(ip);
                }
            }

            // se houver algum IP reprovado, retorna como false
            return ipsReprovados.Count == 0;
        }

        /// <summary>
        /// Obtém o IP do cliente que está acessando o ambiente
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            using (Logger log = Logger.IniciarLog("Blacklist de IP - Obtendo IP do cliente"))
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;

                // obtém variáveis do servidor
                string httpTrueClientIp = context.Request.ServerVariables["HTTP_TRUE_CLIENT_IP"];
                string httpForwardedFor = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                string remoteAddr = context.Request.ServerVariables["REMOTE_ADDR"];

                // grava no log para rastreabilidade
                log.GravarMensagem(string.Concat("HTTP_TRUE_CLIENT_IP: ", httpTrueClientIp));
                log.GravarMensagem(string.Concat("HTTP_X_FORWARDED_FOR: ", httpForwardedFor));
                log.GravarMensagem(string.Concat("REMOTE_ADDR: ", remoteAddr));

                // prevê alteração de endereço pelo serviço do Akamai
                if (!string.IsNullOrEmpty(httpTrueClientIp))
                    return httpTrueClientIp;

                // valida se o cliente foi manipulado por um serviço de proxy
                if (!string.IsNullOrEmpty(httpForwardedFor))
                    return httpForwardedFor;

                // retorna o IP do cliente
                return remoteAddr;
            }
        }
    }
}
