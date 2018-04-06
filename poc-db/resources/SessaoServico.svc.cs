using Rede.PN.SessaoPortal.SharePoint.SessaoPortal.Modelos;
using Redecard.PN.Comum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Rede.PN.SessaoPortal.SharePoint.SessaoPortal {
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SessaoServico : ISessaoServico {
        /// <summary>
        /// Renova a sessão Sharepoint do usuário
        /// </summary>
        /// <returns></returns>
        public ResponseBase RenovarSessao() {
            ResponseBase response = new ResponseBase();

            try {
                if (Sessao.Contem()) {
                    response.CodigoRetorno = 200;
                    response.DescricaoRetorno = "Sessão renovada com sucesso.";
                }
                else {
                    response.CodigoRetorno = Convert.ToInt32(HttpStatusCode.Forbidden);
                    response.DescricaoRetorno = "Sessão inválida.";
                    throw new WebFaultException<ResponseBase>(response, HttpStatusCode.Forbidden);
                }

                return response;
            }
            catch (WebFaultException<ResponseBase> ex) {
                throw new WebFaultException<ResponseBase>(ex.Detail, ex.StatusCode);
            }
            catch (Exception ex) {
                SharePointUlsLog.LogErro(ex);

                response.CodigoRetorno = Convert.ToInt32(HttpStatusCode.InternalServerError);
                response.DescricaoRetorno = ex.Message;
                throw new WebFaultException<ResponseBase>(response, HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Recupera os dados da sessão ativa do Sharepoint
        /// </summary>
        /// <returns></returns>
        public ResponseBaseItem<SessaoResponse> ConsultarSessao() {
            ResponseBaseItem<SessaoResponse> response = new ResponseBaseItem<SessaoResponse>();

            try {
                if (Sessao.Contem()) {
                    Sessao sessaoAtual = Sessao.Obtem();

                    response.Item = new SessaoResponse();
                    response.Item.CodigoEntidade = sessaoAtual.CodigoEntidade;
                    response.Item.NomeEntidade = sessaoAtual.NomeEntidade;
                    response.Item.TipoTokenApi = sessaoAtual.TipoTokenApi;
                    response.Item.TokenApi = sessaoAtual.TokenApi;
                    response.Item.LoginUsuario = sessaoAtual.LoginUsuario;
                    response.Item.UltimoAcesso = sessaoAtual.UltimoAcesso.ToString("yyyy-MM-dd HH:mm:ss");
                    response.Item.StatusPV = sessaoAtual.StatusPVCancelado() ? "C" : "A";
                    response.Item.TransacionaDolar = sessaoAtual.TransacionaDolar;
                    response.Item.PVFisico = sessaoAtual.PVFisico;
                    response.Item.PVLogico = sessaoAtual.PVLogico;
                    response.Item.NomeUsuario = sessaoAtual.NomeUsuario;
                    response.Item.CodigoSegmento = sessaoAtual.CodigoSegmento;
                    response.Item.UsuarioAtendimento = sessaoAtual.UsuarioAtendimento;
                    response.Item.CodigoIdUsuario = sessaoAtual.CodigoIdUsuario;
                    response.Item.AcessoFilial = sessaoAtual.AcessoFilial;
                    response.Item.Legado = sessaoAtual.Legado;
                    response.Item.GrupoEntidade = sessaoAtual.GrupoEntidade;
                }
                else {
                    response.CodigoRetorno = Convert.ToInt32(HttpStatusCode.Forbidden);
                    response.DescricaoRetorno = "Sessão inválida.";
                    response.Item = null;
                    throw new WebFaultException<ResponseBaseItem<SessaoResponse>>(response, HttpStatusCode.Forbidden);
                }

                return response;
            }
            catch (WebFaultException<ResponseBaseItem<SessaoResponse>> ex) {
                throw new WebFaultException<ResponseBaseItem<SessaoResponse>>(ex.Detail, ex.StatusCode);
            }
            catch (Exception ex) {
                SharePointUlsLog.LogErro(ex);

                response.CodigoRetorno = Convert.ToInt32(HttpStatusCode.InternalServerError);
                response.DescricaoRetorno = ex.Message;
                throw new WebFaultException<ResponseBaseItem<SessaoResponse>>(response, HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Criptografa chave/valor recebido
        /// </summary>
        /// <returns></returns>
        public ResponseBaseItem<String> GerarQsSegura(Dictionary<String, String> keys) {
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Methods", "GET,PUT,POST,DELETE,OPTIONS");
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Headers", "Authorization, X-Requested-With, X-HTTP-Method-Override, Content-Type, Accept");
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Public", "OPTIONS, GET, POST");

            ResponseBaseItem<String> response = new ResponseBaseItem<String>();

            try {
                //if (Sessao.Contem())
                //{

                //}
                //else
                //{
                //    response.CodigoRetorno = Convert.ToInt32(HttpStatusCode.Forbidden);
                //    response.DescricaoRetorno = "Sessão inválida.";
                //    response.Item = null;
                //    throw new WebFaultException<ResponseBaseItem<String>>(response, HttpStatusCode.Forbidden);
                //}

                //Sessao sessaoAtual = Sessao.Obtem();

                QueryStringSegura qs = new QueryStringSegura();
                foreach (var item in keys) {
                    qs.Add(item.Key, item.Value);
                }
                response.Item = qs.ToString();

                return response;
            }
            catch (WebFaultException<ResponseBaseItem<String>> ex) {
                throw new WebFaultException<ResponseBaseItem<String>>(ex.Detail, ex.StatusCode);
            }
            catch (Exception ex) {
                SharePointUlsLog.LogErro(ex);

                response.CodigoRetorno = Convert.ToInt32(HttpStatusCode.InternalServerError);
                response.DescricaoRetorno = ex.Message;
                throw new WebFaultException<ResponseBaseItem<String>>(response, HttpStatusCode.InternalServerError);
            }
        }


        public void Options() {
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Origin", "*");
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Methods", "GET,PUT,POST,DELETE,OPTIONS");
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Access-Control-Allow-Headers", "Authorization, X-Requested-With, X-HTTP-Method-Override, Content-Type, Accept");
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Public", "OPTIONS, GET, POST");
        }

        public ResponseBaseItem<Dictionary<String, Int32>> ObterPermissoesExtrato() {
            ResponseBaseItem<Dictionary<String, Int32>> response = new ResponseBaseItem<Dictionary<String, Int32>>();
            try {
                if (Sessao.Contem()) {

                    Sessao sessaoAtual = Sessao.Obtem();

                    /*
                     * Verificar o acesso as páginas dos relatórios e montar o dicionário
                     * de controle de permissões
                    */

                    if (object.ReferenceEquals(response.Item, null))
                        response.Item = new Dictionary<string, int>();

                    /* Vendas de Hoje (D0) */
                    if (sessaoAtual.VerificarAcessoPagina("/sites/fechado/PortalSpa/index.html#!/extrato"))
                        response.Item.Add("vendas de hoje", 14);

                    /* Diário de Vendas (Conta Corrente) */
                    if (sessaoAtual.VerificarAcessoPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=13"))
                        response.Item.Add("diário de vendas", 13);

                    /* Vendas */
                    if (sessaoAtual.VerificarAcessoPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=0"))
                        response.Item.Add("vendas", 0);

                    /* Valores Pagos */
                    if (sessaoAtual.VerificarAcessoPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?&tipo=1"))
                        response.Item.Add("valores pagos", 1);

                    /* Lançamentos Futuros */
                    if (sessaoAtual.VerificarAcessoPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=4"))
                        response.Item.Add("lançamentos futuros", 4);

                    /* Ordens de Crédito */
                    if (sessaoAtual.VerificarAcessoPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=2"))
                        response.Item.Add("ordens de crédito", 2);

                    /* Pagamentos Ajustados */
                    if (sessaoAtual.VerificarAcessoPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=3"))
                        response.Item.Add("pagamentos ajustados", 3);

                    /* Antecipações */
                    if (sessaoAtual.VerificarAcessoPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=5"))
                        response.Item.Add("antecipações", 5);

                    /* Débitos e Desagendamentos */
                    if (sessaoAtual.VerificarAcessoPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=6"))
                        response.Item.Add("débitos e desagendamentos", 6);

                    /* Serviços */
                    if (sessaoAtual.VerificarAcessoPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=7"))
                        response.Item.Add("serviços", 7);

                    /* Suspensos, Retidos e Penhorados */
                    if (sessaoAtual.VerificarAcessoPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=8"))
                        response.Item.Add("suspensos, retidos e penhorados", 8);
						
					/* Estornos */
                    if (sessaoAtual.VerificarAcessoPagina("/sites/fechado/extrato/Paginas/pn_Relatorios.aspx?tipo=12"))
                        response.Item.Add("estornos", 12);
                }
                return response;
            }
            catch (Exception ex) {
                SharePointUlsLog.LogErro(ex);

                response.CodigoRetorno = Convert.ToInt32(HttpStatusCode.InternalServerError);
                response.DescricaoRetorno = ex.Message;
                throw new WebFaultException<ResponseBaseItem<Dictionary<String, Int32>>>(response, HttpStatusCode.InternalServerError);
            }
        }
    }
}