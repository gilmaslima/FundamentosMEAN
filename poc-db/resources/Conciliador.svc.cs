using AutoMapper;
using Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Contratos;
using Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Request;
using Rede.PN.Conciliador.SharePoint.ConciliadorServicos.Model.Response;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;

namespace Rede.PN.Conciliador.SharePoint.ConciliadorServicos {
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Conciliador : IConciliador {
        #region [Cto]

        /// <summary>
        /// Define que o mapeamento foi realizado.
        /// </summary>
        private static readonly Boolean MapeamentoRealizado;

        /// <summary>
        /// Inicializa uma instância da classe de serviço.
        /// </summary>
        static Conciliador() {
            // Definindo os mapeamentos do serviço
            if (!MapeamentoRealizado) {
                Mapper.CreateMap<ServicoAtivoRequest, ServicoPortalGEServicos.ConsultaServicoAtivoPorPVRequest>();
                Mapper.CreateMap<ServicoPortalGEServicos.ResponseConsultaServico, ServicoAtivoResponse>();
                Mapper.CreateMap<ServicoPortalGEServicos.ResponseConsultaServicoPatamar, Model.Response.ConciliadorPatamarResponse>();

                Mapper.CreateMap<ServicoRegimeRequest, ServicoPortalGEServicos.ConsultaDetalheServicoRegimeRequest>();
                Mapper.CreateMap<ServicoPortalGEServicos.ResponseConsultaDetalheServicoRegime, ServicoRegimeResponse>();

                Mapper.CreateMap<ServicoPortalWFConciliador.ResponseBase, Model.Response.ResponseBase>();

                MapeamentoRealizado = true;
            }
        }

        #endregion

        #region [Propriedades do Serviço]

        /// <summary>
        /// Retorna a Célula que representa o Portal Rede para acessos no GE/WF.
        /// </summary>
        static readonly Int32 CodigoCelula = 354;

        /// <summary>
        /// Retorna o Canal que representa o Portal Rede para acessos no GE/WF.
        /// </summary>
        static readonly Int32 CodigoCanal = 15;

        /// <summary>
        /// Retorna o código do regime isento.
        /// </summary>
        static readonly Int32 CodigoRegimeIsento = 2;

        /// <summary>
        /// Retorna o Usuário que representa o Portal Rede para acessos no GE/WF.
        /// </summary>
        static readonly String UsuarioPortal = "Portal";

        /// <summary>
        /// <para>Código dos Serviços do Conciliador:</para> 
        /// <para>206 - Control Rede</para>
        /// <para>207 - Control Rede Retroativo</para>
        /// </summary>
        enum TipoServico : int {
            ControlRede = 206,
            Retroativo = 207
        };

        /// <summary>
        /// Sessão Atual do Usuário no Portal
        /// </summary>
        private Sessao SessaoAtual {
            get {
                Sessao sessaoUsuario = System.Web.HttpContext.Current.Session[Sessao.ChaveSessao] as Sessao;
                if (sessaoUsuario != null) {
                    return sessaoUsuario;
                }
                else
                    return null;
            }
        }

        #endregion

        #region [WebOperations]

        /// <summary>
        /// Consulta os serviços ativos do Conciliador.
        /// </summary>
        /// <returns></returns>
        public ResponseBaseItem<ServicoAtivoResponse> ConsultarServicoAtivo(ServicoAtivoRequest request) {
            using (Logger log = Logger.IniciarLog("ConciliadorServicos - ConsultaServicoAtivo - REST")) {
                ResponseBaseItem<ServicoAtivoResponse> dadosResponse = null;

                try {
                    dadosResponse = new ResponseBaseItem<ServicoAtivoResponse>();

                    log.GravarLog(EventoLog.InicioServico, request);

                    if (ValidarParametrosResquest(request)) {
                        ServicoPortalGEServicos.ConsultaServicoAtivoPorPVRequest servicoGERequest =
                            Mapper.Map<ServicoAtivoRequest, ServicoPortalGEServicos.ConsultaServicoAtivoPorPVRequest>(request);

                        if (request.CodigoServico == 1)
                            servicoGERequest.CodigoServico = (Int32)TipoServico.ControlRede;
                        else
                            servicoGERequest.CodigoServico = (Int32)TipoServico.Retroativo;

                        servicoGERequest.NumeroPV = SessaoAtual.CodigoEntidade;
                        servicoGERequest.Usuario = Conciliador.UsuarioPortal;

                        using (ServicoPortalGEServicos.ServicoPortalGEServicosClient client = new ServicoPortalGEServicos.ServicoPortalGEServicosClient()) {
                            var servicoGEResponse = client.ConsultaServicoAtivoPorPV(servicoGERequest);

                            if (servicoGEResponse != null) {
                                dadosResponse = new ResponseBaseItem<ServicoAtivoResponse>();
                                dadosResponse.CodigoRetorno = servicoGEResponse.CodigoRetorno;
                                dadosResponse.DescricaoRetorno = servicoGEResponse.DescricaoRetorno;
                                dadosResponse.Item =
                                    Mapper.Map<ServicoPortalGEServicos.ResponseConsultaServico, ServicoAtivoResponse>(servicoGEResponse.Item);
                            }
                        }

                        LogMonitoracao.LogHistorico
                            .GravarHistorico(DateTime.Now,
                                DateTime.Now,
                                LogMonitoracao.LogHistorico.StatusEnvio.Sucesso,
                                LogMonitoracao.LogHistorico.Etapa.Consulta,
                                String.Empty,
                                0,
                                SessaoAtual.Email,
                                SessaoAtual.CodigoEntidade);
                    }
                    else {
                        dadosResponse.CodigoRetorno = 400;
                        dadosResponse.DescricaoRetorno = "Dados inválidos";

                        throw new WebFaultException<ResponseBaseItem<ServicoAtivoResponse>>(dadosResponse, System.Net.HttpStatusCode.BadRequest);
                    }

                    log.GravarLog(EventoLog.FimServico);
                }
                catch (WebFaultException<ResponseBaseItem<ServicoAtivoResponse>> ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    LogMonitoracao.LogHistorico
                        .GravarHistorico(DateTime.Now,
                            DateTime.Now,
                            LogMonitoracao.LogHistorico.StatusEnvio.Erro,
                            LogMonitoracao.LogHistorico.Etapa.Consulta,
                            ex.Message,
                            Convert.ToInt32(ex.StatusCode),
                            null,
                            0);

                    throw ex;
                }
                catch (FaultException ex) {
                    dadosResponse.DescricaoRetorno = ex.Message;
                    log.GravarErro(ex);

                    LogMonitoracao.LogHistorico
                        .GravarHistorico(DateTime.Now,
                            DateTime.Now,
                            LogMonitoracao.LogHistorico.StatusEnvio.Erro,
                            LogMonitoracao.LogHistorico.Etapa.Consulta,
                            ex.Message,
                            500,
                            null,
                            0);

                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex) {
                    dadosResponse.DescricaoRetorno = ex.Message;
                    log.GravarErro(ex);

                    LogMonitoracao.LogHistorico
                        .GravarHistorico(DateTime.Now,
                            DateTime.Now,
                            LogMonitoracao.LogHistorico.StatusEnvio.Erro,
                            LogMonitoracao.LogHistorico.Etapa.Consulta,
                            ex.Message,
                            500,
                            null,
                            0);

                    SharePointUlsLog.LogErro(ex);
                }

                return dadosResponse;
            }
        }

        /// <summary>
        /// Consulta os detalhes de regimes do serviço
        /// </summary>
        /// <returns></returns>
        public ResponseBaseList<ServicoRegimeResponse> ConsultarServicoRegime(ServicoRegimeRequest request) {
            using (Logger log = Logger.IniciarLog("ConciliadorServicos - ConsultarServicoRegime - REST")) {
                ResponseBaseList<ServicoRegimeResponse> dadosResponse = null;

                try {
                    dadosResponse = new ResponseBaseList<ServicoRegimeResponse>();
                    log.GravarLog(EventoLog.InicioServico, request);

                    if (request != null) {
                        ServicoPortalGEServicos.ConsultaDetalheServicoRegimeRequest servicoGERequest =
                            Mapper.Map<ServicoRegimeRequest, ServicoPortalGEServicos.ConsultaDetalheServicoRegimeRequest>(request);

                        if (request.CodigoServico == 1)
                            servicoGERequest.CodigoServico = (Int32)TipoServico.ControlRede;
                        else
                            servicoGERequest.CodigoServico = (Int32)TipoServico.Retroativo;

                        servicoGERequest.Usuario = Conciliador.UsuarioPortal;
                        servicoGERequest.CodigoCanal = Conciliador.CodigoCanal;
                        servicoGERequest.CodigoCelula = Conciliador.CodigoCelula;

                        using (ServicoPortalGEServicos.ServicoPortalGEServicosClient client = new ServicoPortalGEServicos.ServicoPortalGEServicosClient()) {
                            var servicoGEResponse = client.ConsultaDetalheServicoRegime(servicoGERequest);

                            if (servicoGEResponse != null) {
                                List<ServicoRegimeResponse> listaRetorno = new List<ServicoRegimeResponse>();

                                foreach (ServicoPortalGEServicos.ResponseConsultaDetalheServicoRegime item in servicoGEResponse.Itens) {
                                    ServicoRegimeResponse regimeResponse = new ServicoRegimeResponse();
                                    regimeResponse = Mapper.Map<ServicoPortalGEServicos.ResponseConsultaDetalheServicoRegime, ServicoRegimeResponse>(item);

                                    listaRetorno.Add(regimeResponse);
                                }

                                dadosResponse.CodigoRetorno = servicoGEResponse.CodigoRetorno;
                                dadosResponse.DescricaoRetorno = servicoGEResponse.DescricaoRetorno;
                                dadosResponse.Itens = listaRetorno.ToArray();
                            }
                        }
                    }

                    log.GravarLog(EventoLog.FimServico);
                }
                catch (FaultException ex) {
                    dadosResponse.DescricaoRetorno = ex.Message;
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex) {
                    dadosResponse.DescricaoRetorno = ex.Message;
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                }

                return dadosResponse;
            }
        }

        /// <summary>
        /// Serviço que contrata o serviço do Conciliador
        /// </summary>
        /// <returns></returns>
        public ResponseBase ContratarCancelarConciliador(ContratarCancelarRequest request) {
            using (Logger log = Logger.IniciarLog("ConciliadorServicos - ContratarCancelarConciliador - REST")) {
                ResponseBase response = null;

                try {
                    log.GravarLog(EventoLog.InicioServico, request);

                    response = new ResponseBase();

                    if (this.ValidarParametrosResquest(request)) {
                        LogMonitoracao.LogHistorico.Etapa etapaOperacao = request.TipoSolicitacao == 1 ?
                            LogMonitoracao.LogHistorico.Etapa.Contratacao : LogMonitoracao.LogHistorico.Etapa.Cancelamento;

                        foreach (int codigoTipoServico in request.Servicos) {
                            int codigoServico = 0;

                            if (codigoTipoServico == 1)
                                codigoServico = (Int32)TipoServico.ControlRede;
                            else
                                codigoServico = (Int32)TipoServico.Retroativo;

                            var servicoRegime = ConsultarServicoRegime(new ServicoRegimeRequest { CodigoServico = codigoServico });

                            if (servicoRegime != null && servicoRegime.Itens != null && servicoRegime.Itens.Length > 0) {
                                if (servicoRegime.DescricaoRetorno == "Sucesso") {
                                    using (ServicoPortalWFConciliador.ServicoPortalWFPropostaConciliadorClient client = new ServicoPortalWFConciliador.ServicoPortalWFPropostaConciliadorClient()) {
                                        ServicoPortalWFConciliador.ConciliadorRequest contratarCancelarRequest = new ServicoPortalWFConciliador.ConciliadorRequest {
                                            PV = SessaoAtual.CodigoEntidade,
                                            CodigoServico = codigoServico,
                                            CodigoRegime = Conciliador.CodigoRegimeIsento,
                                            Canal = Conciliador.CodigoCanal,
                                            Celula = Conciliador.CodigoCelula,
                                            CodigoVendedor = 0,
                                            Usuario = Conciliador.UsuarioPortal,
                                            TipoSolicitacao = request.TipoSolicitacao
                                        };

                                        DateTime inicio = DateTime.Now;
                                        ServicoPortalWFConciliador.ResponseBase servicoWFResponse = client.ContratarCancelarConciliador(contratarCancelarRequest);
                                        DateTime fim = DateTime.Now;

                                        if (servicoWFResponse != null && servicoWFResponse.DescricaoRetorno == "Sucesso") {
                                            response =
                                                Mapper.Map<ServicoPortalWFConciliador.ResponseBase, Model.Response.ResponseBase>(servicoWFResponse);

                                            LogMonitoracao.LogHistorico
                                                .GravarHistorico(inicio,
                                                    fim,
                                                    LogMonitoracao.LogHistorico.StatusEnvio.Sucesso,
                                                    etapaOperacao,
                                                    String.Empty,
                                                    0,
                                                    SessaoAtual.Email,
                                                    SessaoAtual.CodigoEntidade);
                                        }
                                        else {
                                            response.CodigoRetorno = servicoWFResponse.CodigoRetorno;
                                            response.DescricaoRetorno = servicoWFResponse.DescricaoRetorno;

                                            LogMonitoracao.LogHistorico
                                                .GravarHistorico(inicio,
                                                    fim,
                                                    LogMonitoracao.LogHistorico.StatusEnvio.Erro,
                                                    etapaOperacao,
                                                    response.DescricaoRetorno,
                                                    response.CodigoRetorno.Value,
                                                    SessaoAtual.Email,
                                                    SessaoAtual.CodigoEntidade);

                                            break;
                                        }
                                    }
                                }
                                else {
                                    response.CodigoRetorno = servicoRegime.CodigoRetorno;
                                    response.DescricaoRetorno = servicoRegime.DescricaoRetorno;

                                    LogMonitoracao.LogHistorico
                                                .GravarHistorico(DateTime.Now,
                                                    DateTime.Now,
                                                    LogMonitoracao.LogHistorico.StatusEnvio.Erro,
                                                    etapaOperacao,
                                                    response.DescricaoRetorno,
                                                    response.CodigoRetorno.Value,
                                                    SessaoAtual.Email,
                                                    SessaoAtual.CodigoEntidade);

                                    break;
                                }
                            }
                            else {
                                response.CodigoRetorno = 0;
                                response.DescricaoRetorno = string.Format("Não foi encontrado o regime para o código de serviço {0}", codigoServico);

                                LogMonitoracao.LogHistorico
                                            .GravarHistorico(DateTime.Now,
                                                DateTime.Now,
                                                LogMonitoracao.LogHistorico.StatusEnvio.Erro,
                                                etapaOperacao,
                                                response.DescricaoRetorno,
                                                response.CodigoRetorno.Value,
                                                SessaoAtual.Email,
                                                SessaoAtual.CodigoEntidade);

                                break;
                            }
                        }

                    }
                    else {
                        response.CodigoRetorno = 400;
                        response.DescricaoRetorno = "Dados inválidos";

                        LogMonitoracao.LogHistorico
                            .GravarHistorico(DateTime.Now,
                                DateTime.Now,
                                LogMonitoracao.LogHistorico.StatusEnvio.Erro,
                                LogMonitoracao.LogHistorico.Etapa.Contratacao,
                                response.DescricaoRetorno,
                                response.CodigoRetorno.Value,
                                SessaoAtual.Email,
                                SessaoAtual.CodigoEntidade);

                        throw new WebFaultException<ResponseBase>(response, System.Net.HttpStatusCode.BadRequest);
                    }

                    log.GravarLog(EventoLog.FimServico);
                }
                catch (WebFaultException<ResponseBase> ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    throw ex;
                }
                catch (FaultException ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    LogMonitoracao.LogHistorico
                           .GravarHistorico(DateTime.Now,
                               DateTime.Now,
                               LogMonitoracao.LogHistorico.StatusEnvio.Erro,
                               LogMonitoracao.LogHistorico.Etapa.Contratacao,
                               ex.Message,
                               500,
                               String.Empty,
                               0);
                }
                catch (Exception ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    LogMonitoracao.LogHistorico
                           .GravarHistorico(DateTime.Now,
                               DateTime.Now,
                               LogMonitoracao.LogHistorico.StatusEnvio.Erro,
                               LogMonitoracao.LogHistorico.Etapa.Contratacao,
                               ex.Message,
                               500,
                               String.Empty,
                               0);
                }
                return response;
            }
        }

        /// <summary>
        /// Serviço que envia o email de comprovante de contratação/cancelamento
        /// </summary>
        /// <returns></returns>
        public bool EnviarEmailComprovante(ComprovanteRequest request) {
            using (Logger log = Logger.IniciarLog("ConciliadorServicos - EnviarEmailComprovante - REST")) {
                try {
                    log.GravarLog(EventoLog.InicioServico, request);

                    if (ValidarParametrosResquest(request)) {
                        MailMessage mailMessage = new MailMessage();
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Subject = request.Assunto;
                        mailMessage.Body = request.ConteudoHtml;

                        mailMessage.To.Add(new MailAddress(this.SessaoAtual.Email));

                        string emailBackoffice = ConfigurationManager.AppSettings["EmailBackoffice"];
                        if (!string.IsNullOrEmpty(emailBackoffice))
                            mailMessage.To.Add(emailBackoffice);

                        SmtpClient smtpClient = new SmtpClient();
                        smtpClient.Send(mailMessage);

                        log.GravarLog(EventoLog.FimServico);

                        return true;
                    }
                    else
                        throw new WebFaultException<Boolean>(false, System.Net.HttpStatusCode.BadRequest);

                }
                catch (WebFaultException<Boolean> ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    throw ex;
                }
                catch (SmtpException ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                }

                return false;
            }
        }

        /// <summary>
        /// Obtém o token da Login do Usuário
        /// </summary>
        /// <returns>JWT para o Login</returns>
        public LoginResponse SingleSignOn() {
            if (SessaoAtual != null) {
                return SingleSignOn(SessaoAtual.CodigoIdUsuario.ToString(), "false");
            } else {
                throw new WebFaultException<String>("sessão não encontrada", System.Net.HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codUsuario"></param>
        /// <returns></returns>
        public LoginResponse SingleSignOn(string codUsuario, string atendimento) {
            bool bAtendimento = false;
            if (!Boolean.TryParse(atendimento, out bAtendimento)) {
                throw new WebFaultException<String>("condição de atendimento inválida", System.Net.HttpStatusCode.BadRequest);
            }
            else {
                using (Logger log = Logger.IniciarLog("ConciliadorServicos - SingleSignOn - REST")) {
                    LoginResponse login = new LoginResponse();

                    try {
                        log.GravarLog(EventoLog.InicioServico);

                        if (ValidarParametrosResquest()) {

                            if (ConfigurationManager.AppSettings["UrlServicoLogin"] != null && !String.IsNullOrEmpty(ConfigurationManager.AppSettings["UrlServicoLogin"])) {
                                string urlLogin = ConfigurationManager.AppSettings["UrlServicoLogin"];

                                if (bAtendimento) {
                                    urlLogin = String.Format("{0}/{1}/{2}/{3}", urlLogin, codUsuario, "true", SessaoAtual.Funcional);
                                } else {
                                    urlLogin = String.Format("{0}/{1}", urlLogin, codUsuario);
                                }

                                WebRequest request = WebRequest.Create(urlLogin);

                                request.Method = "GET";
                                request.ContentType = "application/json";

                                using (var response = (HttpWebResponse)request.GetResponse()) {
                                    if (response.StatusCode != HttpStatusCode.OK) {
                                        throw new WebFaultException<String>("Falha ao logar", System.Net.HttpStatusCode.BadRequest);
                                    }

                                    using (var responseStream = response.GetResponseStream()) {
                                        if (responseStream != null) {
                                            StreamReader reader = new StreamReader(responseStream);
                                            login.AccessToken = reader.ReadToEnd();
                                            login.AccessToken = login.AccessToken.Replace("\"", "");
                                            login.TokenType = "Bearer";
                                        }
                                        else {
                                            SharePointUlsLog.LogMensagem("API - Houve retorno com erros ao obter o token do usuário");
                                            log.GravarMensagem("API - Houve retorno com erros ao obter o token do usuário");

                                            throw new WebFaultException<String>("Retorno inválido", System.Net.HttpStatusCode.BadRequest);
                                        }
                                    }
                                }
                            }
                            else
                                throw new WebFaultException<String>("Parâmetro não encontrado", System.Net.HttpStatusCode.BadRequest);
                        }
                        else
                            throw new WebFaultException<String>("Dados inválidos", System.Net.HttpStatusCode.BadRequest);

                    }
                    catch (WebException ex) {
                        log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);

                        if (ex.Response != null && ex.Status == WebExceptionStatus.ProtocolError)
                            throw new WebFaultException<String>("Dados inválidos", System.Net.HttpStatusCode.BadRequest);
                    }
                    catch (WebFaultException<String> ex) {
                        log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);

                        throw ex;
                    }
                    catch (WebFaultException<Boolean> ex) {
                        log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);

                        throw ex;
                    }
                    catch (Exception ex) {
                        log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);
                    }

                    return login;
                }
            }
        }

        #endregion

        #region [Métodos auxiliares]

        /// <summary>
        /// Validar os parâmetros obter Token
        /// </summary>
        /// <returns></returns>
        private bool ValidarParametrosResquest() {
            if (SessaoAtual == null)
                return false;

            return true;
        }

        /// <summary>
        /// Validar os parâmetros recebidos para Envio de Comprovante
        /// </summary>
        /// <param name="request">Parâmetros de request recebidos</param>
        /// <returns></returns>
        private bool ValidarParametrosResquest(ComprovanteRequest request) {
            if (SessaoAtual != null) {
                if (request == null)
                    return false;

                if (String.IsNullOrEmpty(request.Assunto) || String.IsNullOrEmpty(request.ConteudoHtml))
                    return false;
            }
            else
                return false;

            return true;
        }

        /// <summary>
        /// Validar os parâmetros recebidos para Contratação/Cancelamento
        /// </summary>
        /// <param name="request">Parâmetros de request recebidos</param>
        /// <returns></returns>
        private Boolean ValidarParametrosResquest(ContratarCancelarRequest request) {
            if (SessaoAtual != null) {
                if (request == null)
                    return false;

                if (request.Servicos != null && !(request.Servicos.Length > 0))
                    return false;

                if (!request.Servicos.Contains(1) && !request.Servicos.Contains(2))
                    return false;

                if (request.TipoSolicitacao != 1 && request.TipoSolicitacao != 2)
                    return false;

            }
            else
                return false;

            return true;
        }

        /// <summary>
        /// Validar os parâmetros recebidos para verificar se o Serviço do Conciliador está ativo
        /// </summary>
        /// <param name="request">Parâmetros de request recebidos</param>
        /// <returns></returns>
        private bool ValidarParametrosResquest(ServicoAtivoRequest request) {
            if (SessaoAtual != null) {
                if (request == null)
                    return false;

                if (request.CodigoServico != 1 && request.CodigoServico != 2)
                    return false;
            }
            else
                return false;

            return true;
        }

        /// <summary>
        /// Retorna se o usuário atual do portal é um usuário do tipo atendimento.
        /// </summary>
        /// <returns></returns>
        public bool VerificarUsuarioAtendimento() {
            if (SessaoAtual != null) {
                return !String.IsNullOrEmpty(SessaoAtual.Funcional);
            }
            else
                return false;
        }

        /// <summary>
        /// Consulta o ID do usuário baseado nos dados de entrada informado
        /// </summary>
        /// <param name="codGrupoEntidade"></param>
        /// <param name="codEntidade"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public UsuarioServico.Usuario ConsultarUsuarioEmail(string email) {
            using (Logger log = Logger.IniciarLog("ConciliadorServicos - ConsultarUsuarioEmail - REST")) {
                try {
                    if (SessaoAtual != null) {
                        using (UsuarioServico.UsuarioServicoClient client = new UsuarioServico.UsuarioServicoClient()) {
                            int retorno = 0;

                            log.GravarLog(EventoLog.InicioServico);
                            UsuarioServico.Usuario[] usuarios = client.ConsultarPorEntidade(new UsuarioServico.Entidade() {
                                Codigo = SessaoAtual.CodigoEntidade,
                                GrupoEntidade = new UsuarioServico.GrupoEntidade() {
                                    Codigo = 1 // estabelecimento
                                }
                            }, out retorno);

                            log.GravarLog(EventoLog.RetornoServico, new { retorno = retorno });

                            if (retorno == 0) {
                                UsuarioServico.Usuario usuario = usuarios.FirstOrDefault(x => x.Email.ToLowerInvariant().Equals(email));
                                if (!object.ReferenceEquals(usuario, null)) {
                                    return usuario;
                                } else {
                                    throw new WebFaultException<String>("usuário inválido", System.Net.HttpStatusCode.BadRequest);
                                }
                            }
                            else {
                                throw new WebFaultException<String>("código retorno inválido", System.Net.HttpStatusCode.BadRequest);
                            }
                        }
                    }
                    else {
                        throw new WebFaultException<String>("Sessão de usuário inválida", System.Net.HttpStatusCode.BadRequest);
                    }
                }
                catch (WebException ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    if (ex.Response != null && ex.Status == WebExceptionStatus.ProtocolError)
                        throw new WebFaultException<String>("Dados inválidos", System.Net.HttpStatusCode.BadRequest);
                }
                catch (WebFaultException<String> ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    throw ex;
                }
                catch (WebFaultException<Boolean> ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);

                    throw ex;
                }
                catch (Exception ex) {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                }

                throw new WebFaultException<String>("condição inválida", System.Net.HttpStatusCode.BadRequest);
            }
        }

        #endregion
    }
}