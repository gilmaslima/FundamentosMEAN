using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel.Activation;
using System.Web;
using Microsoft.SharePoint;
using Redecard.PN.Comum;
using System;

namespace Redecard.PN.DadosCadastrais.SharePoint.ISAPI.WSAutorizacao
{
    /// <summary>
    /// Serviço utilizado para autorização de usuários no portal da Fielo
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class WSAutorizacao : IWSAutorizacao
    {
        /// <summary>
        /// Método que autoriza usuários a acessarem o portal da Fielo
        /// </summary>
        /// <param name="pv">Número do PV</param>
        /// <param name="hash">Hash de validação</param>
        /// <param name="appFabric">Cache AppFabric</param>
        /// <returns>Se o usuário tem autorização</returns>
        public bool AutorizarAcessoUsuario(string pv, string hash, string appFabric)
        {
            using (Logger log = Logger.IniciarLog("WSAutorizacao - AutorizarAcessoUsuario"))
            {
                log.GravarLog(EventoLog.ChamadaServico);

                try
                {
                    log.GravarLog(EventoLog.InicioServico, new { pv, hash, appFabric });

                    bool isVerificaIpRequisicao = true;

                    bool.TryParse((ConfigurationManager.AppSettings["VerificaIpsRequisicao"] ?? String.Empty), out isVerificaIpRequisicao);

                    if (isVerificaIpRequisicao)
                    {
                        String remoteIp = HttpContext.Current.Request.ServerVariables["HTTP_TRUE_CLIENT_IP"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] ?? string.Empty;

                        if (string.IsNullOrEmpty(remoteIp))
                        {
                            log.GravarMensagem("IP remoto vazio. HttpContext.Current.Request.ServerVariables['REMOTE_ADDR'] ", new { remoteIp });
                            return false;
                        }

                        if (!VerificarIPValido(remoteIp))
                        {
                            log.GravarMensagem("IP da requisição não está na lista do Sharepoint de Ips Autorizados", new { remoteIp });
                            return false;
                        }
                    }

                    if (string.IsNullOrEmpty(appFabric))
                    {
                        log.GravarMensagem("Código do cache vazio");
                        return false;
                    }

                    //Verifica se os dados recebidos são os mesmos que os enviados na primeira requisição.
                    var dados = CacheAdmin.Recuperar<List<KeyValuePair<string, string>>>(Cache.Fidelidade, appFabric);
                    if (dados.Equals(null))
                    {
                        log.GravarMensagem("Não foi possível recuperar parametros do cache.", new { appFabric });
                        return false;
                    }

                    var pvFiltro = dados.Find(x => x.Key.Equals("pv") && x.Value.Equals(pv));
                    if (pvFiltro.Equals(null))
                    {
                        log.GravarMensagem("Pv informado não encontrato no cache", new { pv });
                        return false;
                    }

                    var hashFiltro = dados.Find(x => x.Key.Equals("hash") && x.Value.Equals(hash));
                    if (hashFiltro.Equals(null))
                    {
                        log.GravarMensagem("Hash informado não encontrado no cache", new { hash });
                        return false;
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    log.GravarLog(EventoLog.RetornoServico, new { ex });
                    return false;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    log.GravarLog(EventoLog.RetornoServico, new { ex });
                    return false;
                }

                log.GravarLog(EventoLog.FimServico);
            }

            return true;
        }

        #region .VerificarIPValido.
        /// <summary>
        /// Verifica se o IP informado está na lista de IPs autorizados
        /// </summary>
        /// <param name="remoteIP">IP da origem</param>
        /// <returns></returns>
        private bool VerificarIPValido(string remoteIP)
        {
            bool isIpRequisicaoValido = false;

            //Busca na lista de ips autorizados se o IP da requisição está liberado para acesso.
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(ConfigurationManager.AppSettings["siteListaIpsAutorizados"]))
                {
                    //Verifica na Lista se o IP da requisição está liberado para utilização do método.
                    using (SPWeb spWeb = site.OpenWeb())
                    {
                        SPList spLists = spWeb.Lists[ConfigurationManager.AppSettings["ListaIPsFielo"]];

                        foreach (SPListItem spListItem in spLists.Items)
                        {
                            IPAddress address;
                            if (IPAddress.TryParse(remoteIP, out address))
                            {
                                if (spListItem.DisplayName.Equals(address.ToString()))
                                {
                                    isIpRequisicaoValido = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            });

            return isIpRequisicaoValido;
        }
        #endregion
    }
}
