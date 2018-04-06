using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.PortalApi.Modelo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;


namespace Redecard.PN.DadosCadastrais.SharePoint.PortalApi
{   
    /// <summary>
    /// Class para obter a lista de estabelecimentos 
    /// </summary>
    public class ListaEstabelecimentosApi
    {

        /// <summary>
        /// Status de Retorno da Api de Login Estabelecimento
        /// </summary>
        public StatusRetorno StatusRetornoLoginEstabelecimento { get; set; }

        /// <summary>
        /// Lista de Retorno de Estabelecimentos a Api
        /// </summary>
        public List<EntidadeRetorno> ListaEntidadeRetorno { get; set; }


        /// <summary>
        /// Consulta entidades associadas a um usuário por e-mail e senha criptografada Hash
        /// </summary>
        /// <param name="email">E-mail usuário</param>
        /// <param name="senha">Senha Criptografada</param>
        /// <param name="codigoRetorno">Código de Retorno</param>
        /// <returns>Listagem das entidades</returns>
        public Boolean ConsultarPorEmailSenhaHash(String email, String senha, out Int32 codigoRetorno)
        {

            DataContractJsonSerializer statusSerializer = new DataContractJsonSerializer(typeof(StatusRetorno));
            DataContractJsonSerializer estabelecimentosRetornoSerializer = new DataContractJsonSerializer(typeof(List<EntidadeRetorno>));

            using (Logger log = Logger.IniciarLog("Consulta a entidade pelo e-mail e senha do usuário"))
            {
                log.GravarLog(EventoLog.InicioServico);

                codigoRetorno = 0;

                try
                {

                    String estabelecimentosApiKey = default(String);
                    String estabelecimentosApiUrl = default(String);
                    String parametrosApi = default(String);


                    SPWeb web = default(SPWeb);

                    if (SPContext.Current != null && SPContext.Current.Web != null)
                    {
                        web = SPContext.Current.Web;

                        if (web.AllProperties.ContainsKey("EstabelecimentosApiKey") && web.AllProperties.ContainsKey("EstabelecimentosApiUrl"))
                        {
                            estabelecimentosApiKey = web.AllProperties["EstabelecimentosApiKey"].ToString();
                            estabelecimentosApiUrl = web.AllProperties["EstabelecimentosApiUrl"].ToString();
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

                    log.GravarMensagem("API - Chamada ao serviço de login");
                    log.GravarLog(EventoLog.ChamadaServico, new
                    {
                        email,
                        estabelecimentosApiKey,
                        estabelecimentosApiUlr = estabelecimentosApiUrl
                    });

                    parametrosApi = String.Format("usuario={0}&senha={1}&codigo_cliente={2}", email, senha, estabelecimentosApiKey);

                    //// para ignorar certificado inválido ou expirado (caso necessário)
                    //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    ////Número máximo de conexões simultâneas permitidas por um objeto ServicePoint utilizado pelo WebRequest
                    //ServicePointManager.DefaultConnectionLimit = 10;

                    ////Tempo de "keep-alive" para as conexões do ServicePoint
                    //ServicePointManager.MaxServicePointIdleTime = 10000;

                    WebRequest request = PortalWebClient.GetWebRequest(String.Format("{0}", estabelecimentosApiUrl));

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
                                SharePointUlsLog.LogMensagem("API - Houve retorno sem erros na lista de entidades estabelecimentos");
                                log.GravarMensagem("API - Houve retorno sem erros na lista de entidades estabelecimentos");


                                this.ListaEntidadeRetorno = (List<EntidadeRetorno>)estabelecimentosRetornoSerializer.ReadObject(responseStream);
                                log.GravarLog(EventoLog.RetornoServico, new { ListEntidadeRetorno = this.ListaEntidadeRetorno });

                                //Preenche o status com o status 0 para indicar sucesso
                                this.StatusRetornoLoginEstabelecimento = new StatusRetorno() { Codigo = 0 };

                                return true;
                            }
                            else
                            {
                                SharePointUlsLog.LogMensagem("API - Houve retorno com erros na lista de entidades estabelecimentos");
                                log.GravarMensagem("API - Houve retorno com erros na lista de entidades estabelecimentos");

                                return false;
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);

                    if (ex.Response != null && ex.Status == WebExceptionStatus.ProtocolError)
                        this.StatusRetornoLoginEstabelecimento = (StatusRetorno)statusSerializer.ReadObject(ex.Response.GetResponseStream());
                    
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
       
    }
}


