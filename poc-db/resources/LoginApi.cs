using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.PortalApi.Modelo;
using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Redecard.PN.DadosCadastrais.SharePoint.PortalApi
{
    /// <summary>
    /// Classe para encapsular a validação do Login através da API
    /// </summary>
    public class LoginApi
    {
        /// <summary>
        /// Status de Retorno da Api de Login
        /// </summary>
        public StatusRetorno StatusRetornoLogin { get; set; }

        /// <summary>
        /// Informações do Login quando o Grupo de Entidade for Estabelecimento
        /// </summary>
        public LoginEstabelecimentoRetorno InformacoesLoginEstabelecimento { get; set; }

        /// <summary>
        /// Informações do Login quando o Grupo de Entidade for diferente de Estabelecimento (Emissores, Bancos SPB, FMS)
        /// </summary>
        public LoginOutrasEntidadesRetorno InformacoesLoginOutrasEntidades { get; set; }
        
        /// <summary>
        /// Chamada da API de Validação do Login
        /// </summary>
        /// <param name="loginUsuario"></param>
        /// <param name="senhaUsuario"></param>
        /// <param name="codigoEntidade"></param>
        /// <param name="codigoGrupoEntidade"></param>
        /// <returns></returns>
        private Boolean ValidarUsuarioApi(String loginUsuario, String senhaUsuario, Int32 codigoEntidade, Int32 codigoGrupoEntidade)
        {
            DataContractJsonSerializer statusSerializer = new DataContractJsonSerializer(typeof(StatusRetorno));
            DataContractJsonSerializer infoEstabelecimentoSerializer = new DataContractJsonSerializer(typeof(LoginEstabelecimentoRetorno));
            DataContractJsonSerializer infoOutrasEntidadesSerializer = new DataContractJsonSerializer(typeof(LoginOutrasEntidadesRetorno));

            using (Logger log = Logger.IniciarNovoLog("Chamada da API de Validação do Login"))
            {
                try
                {
                    String loginApiKey = default(String);
                    String loginApiUrl = default(String);
                    String parametrosApi = default(String);

                    #region [Obtendo Property Bag do Sharepoint com infos da API]
                    SPWeb web = default(SPWeb);

                    if (SPContext.Current != null && SPContext.Current.Web != null)
                    {
                        web = SPContext.Current.Web;

                        if (web.AllProperties.ContainsKey("LoginApiKey") && web.AllProperties.ContainsKey("LoginApiUrl"))
                        {
                            loginApiKey = web.AllProperties["LoginApiKey"].ToString();
                            loginApiUrl = web.AllProperties["LoginApiUrl"].ToString();
                        }
                        else
                        {
                            log.GravarMensagem("API - Configurações na Property Bag não encontradas");
                            SharePointUlsLog.LogMensagem("API - Configurações na Property Bag não encontradas");

                            return false;
                        }
                    }
                    else
                    {
                        log.GravarMensagem("API - Contexto WEB nulo");
                        SharePointUlsLog.LogMensagem("API - Contexto WEB nulo");

                        return false;
                    }
                    #endregion

                    SharePointUlsLog.LogMensagem("API- Chamada ao serviço de login");
                    SharePointUlsLog.LogMensagem(String.Format("API - APIURL: {0}; APIKey: {1}; Login:{2}; Etd:{3}; GrupoEtd:{4};", 
                                                                loginApiUrl, loginApiKey, loginUsuario, codigoEntidade, codigoGrupoEntidade));

                    log.GravarMensagem("API - Chamada ao serviço de login");
                    log.GravarLog(EventoLog.ChamadaServico, new
                    {
                        loginUsuario,
                        codigoEntidade,
                        codigoGrupoEntidade,
                        loginApiUlr = loginApiUrl,
                        loginApiKey
                    });

                    parametrosApi = String.Format("usuario={0}&senha={1}&codigo_cliente={2}&codigo_entidade={3}&tipo_entidade={4}",
                                          loginUsuario,
                                          senhaUsuario,
                                          loginApiKey,
                                          codigoEntidade,
                                          codigoGrupoEntidade);

                    //// para ignorar certificado inválido ou expirado (caso necessário)
                    //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    ////Número máximo de conexões simultâneas permitidas por um objeto ServicePoint utilizado pelo WebRequest
                    //ServicePointManager.DefaultConnectionLimit = 10;

                    ////Tempo de "keep-alive" para as conexões do ServicePoint
                    //ServicePointManager.MaxServicePointIdleTime = 10000;

                    WebRequest request = PortalWebClient.GetWebRequest(String.Format("{0}", loginApiUrl));

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";

                    byte[] byteArray = Encoding.UTF8.GetBytes(parametrosApi);
                    request.ContentLength = byteArray.Length;

                    using (Stream dataStream = request.GetRequestStream())
                    {
                        // Write the data to the request stream.
                        dataStream.Write(byteArray, 0, byteArray.Length);
                    }

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            SharePointUlsLog.LogMensagem(String.Format("API - Request falhou. HTTP Code: {0}", response.StatusCode.ToString()));
                            log.GravarMensagem(String.Format("API - Request falhou. HTTP Code: {0}", response.StatusCode.ToString()));

                            return false;
                        }

                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                            {
                                SharePointUlsLog.LogMensagem("API - Houve retorno sem erros no login");
                                log.GravarMensagem("API - Houve retorno sem erros no login");

                                if (codigoGrupoEntidade.Equals(1))
                                {
                                    this.InformacoesLoginEstabelecimento = (LoginEstabelecimentoRetorno)infoEstabelecimentoSerializer.ReadObject(responseStream);
                                    log.GravarLog(EventoLog.RetornoServico, new { this.InformacoesLoginEstabelecimento });
                                }
                                else
                                {
                                    this.InformacoesLoginOutrasEntidades = (LoginOutrasEntidadesRetorno)infoOutrasEntidadesSerializer.ReadObject(responseStream);
                                    log.GravarLog(EventoLog.RetornoServico, new { this.InformacoesLoginOutrasEntidades });
                                }

                                return true;
                            }
                            else
                            {
                                SharePointUlsLog.LogMensagem("API - Houve retorno com erros no login");
                                log.GravarMensagem("API - Houve retorno com erros no login");

                                return false;
                            }
                        }
                    }
                }
                // Quando o login é inválido, é retornado um HTTP Error no Header da Response, que o WebRequest trata como erro.
                // Porém, existem informações no body da Response com o status do retorno
                catch (WebException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);
                    
                    if (ex.Response != null && ex.Status == WebExceptionStatus.ProtocolError)
                        this.StatusRetornoLogin = (StatusRetorno)statusSerializer.ReadObject(ex.Response.GetResponseStream());

                    return false;
                }
                catch (InvalidDataContractException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);

                    return false;
                }
                catch (NullReferenceException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);

                    return false;
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);

                    return false;
                }
            }
        }

        /// <summary>
        /// Verificar o status do login
        /// </summary>
        /// <returns></returns>
        public Boolean ValidarLogin(String loginUsuario, String senhaUsuario, Int32 codigoEntidade, Int32 codigoGrupoEntidade)
        {
            Boolean valido = this.ValidarUsuarioApi(loginUsuario, senhaUsuario, codigoEntidade, codigoGrupoEntidade);

            using (Logger log = Logger.IniciarNovoLog("Verificar o status do login"))
            {
                if (!valido && this.StatusRetornoLogin == null)
                {
                    StatusRetornoLogin = new StatusRetorno() { Codigo = 330 };
                }
                else if (this.StatusRetornoLogin != null)
                {
                    if (StatusRetornoLogin.Codigo.HasValue && (StatusRetornoLogin.Codigo.Value == 0 || StatusRetornoLogin.Codigo.Value == 397))
                    {
                        SharePointUlsLog.LogMensagem("API - Retorno válido 0 ou 397 retorna TRUE");
                        log.GravarMensagem("API - Retorno válido 0 ou 397 retorna TRUE");

                        valido = true;
                    }
                    else
                    {
                        SharePointUlsLog.LogMensagem("API - Retorno inválido FALSE");
                        log.GravarMensagem("API - Retorno inválido FALSE");

                        valido = false;
                    }
                }
                else //Quando a validação ocorre com sucesso, a API não retorna o objeto de Status
                {
                    StatusRetornoLogin = new StatusRetorno() { Codigo = 0 };
                    valido = true;
                }
            }

            return valido;
        }
    }
}
