using Newtonsoft.Json;
using Rede.PN.ApiLogin.Sharepoint.ISAPI.Login.Models;
using Rede.PN.ApiLogin.Sharepoint.ISAPI.Login.Models.Request;
using Rede.PN.ApiLogin.Sharepoint.ISAPI.Login.Models.Response;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace Rede.PN.ApiLogin.Sharepoint.ISAPI.Login {
    /// <summary>
    /// Serviço (api) para login do Conciliador
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class LoginServico : ILoginServico {
        #region [Propriedades da Classe]

        /// <summary>
        /// Informações de sessão do usuário para registro de Login
        /// </summary>
        private Sessao SessaoUsuario { get; set; }

        /// <summary>
        /// Indicador se a senha já está criptografada
        /// </summary>
        private Boolean SenhaCriptografada { get; set; }

        #endregion

        #region [Operações do Serviço]

        /// <summary>
        /// [POST] Validar o login do usuário
        /// </summary>
        /// <param name="modeloRequisicao">Objeto com as informações de login do usuário</param>
        /// <returns>Objeto com as informações de login</returns>
        public ValidacaoLoginResponse ValidarLogin(LoginConciliadorRequest modeloRequisicao) {
            ValidacaoLoginResponse response = default(ValidacaoLoginResponse);

            using (Logger log = Logger.IniciarLog("Validação do Login")) {
                try {
                    Usuario usuario = new Usuario() { Email = modeloRequisicao.Usuario, Senha = modeloRequisicao.Senha };
                    GrupoEntidade grupo = new GrupoEntidade() { Codigo = modeloRequisicao.GrupoEntidade };

                    response = this.Validar(usuario, grupo, modeloRequisicao.CodigoCliente);

                    Int32[] codigosErroNaoPermitido = { 391, 1103, 404, 415 };

                    log.GravarLog(EventoLog.RetornoServico, response);

                    if (response.StatusRetorno != null && response.StatusRetorno.Codigo == 0) {
                        //Registra login no histórico/log 
                        //TODO: AAL - Verificar por que no ambiente de Homologação não está funcionando
                        //Historico.Login(SessaoUsuario, (DateTime?)null);
                    }
                    else if (response.StatusRetorno != null && codigosErroNaoPermitido.Contains(response.StatusRetorno.Codigo))
                        throw new WebFaultException<ValidacaoLoginResponse>(response, System.Net.HttpStatusCode.Forbidden);
                    else
                        throw new WebFaultException<ValidacaoLoginResponse>(response, System.Net.HttpStatusCode.Forbidden);

                }
                catch (Jose.IntegrityException ex) {
                    log.GravarErro(ex);

                    response = new ValidacaoLoginResponse();
                    response.StatusRetorno = new StatusRetorno() { Codigo = Convert.ToInt32(System.Net.HttpStatusCode.Forbidden), Mensagem = "Sistema indisponível." };
                    throw new WebFaultException<ValidacaoLoginResponse>(response, System.Net.HttpStatusCode.InternalServerError);
                }
                catch (Jose.JoseException ex) {
                    log.GravarErro(ex);

                    response = new ValidacaoLoginResponse();
                    response.StatusRetorno = new StatusRetorno() { Codigo = Convert.ToInt32(System.Net.HttpStatusCode.Forbidden), Mensagem = "Sistema indisponível." };
                    throw new WebFaultException<ValidacaoLoginResponse>(response, System.Net.HttpStatusCode.InternalServerError);
                }
                catch (WebFaultException<ValidacaoLoginResponse> ex) {
                    //log.GravarErro(ex);
                    throw new WebFaultException<ValidacaoLoginResponse>(ex.Detail, ex.StatusCode);
                }
                catch (Exception ex) {
                    log.GravarErro(ex);

                    response = new ValidacaoLoginResponse();
                    response.StatusRetorno = new StatusRetorno() { Codigo = Convert.ToInt32(System.Net.HttpStatusCode.InternalServerError), Mensagem = "Sistema indisponível." };
                    throw new WebFaultException<ValidacaoLoginResponse>(response, System.Net.HttpStatusCode.InternalServerError);
                }
            }

            return response;
        }

        /// <summary>
        /// [GET] Valida um token recebido
        /// </summary>
        /// <returns>Conteúdo do Token</returns>
        public ValidacaoLoginResponse ValidarToken() {
            WebOperationContext ctx = WebOperationContext.Current;
            String authorization = default(String);
            String validacao = default(String);
            ValidacaoLoginResponse response = default(ValidacaoLoginResponse);

            using (Logger log = Logger.IniciarLog("Validação do Token")) {
                try {
                    if (ctx.IncomingRequest.Headers != null && ctx.IncomingRequest.Headers.Count > 0 && ctx.IncomingRequest.Headers.AllKeys.Contains("Authorization"))
                        authorization = ctx.IncomingRequest.Headers["Authorization"];

                    log.GravarLog(EventoLog.ChamadaServico, authorization);

                    var secretKey = Keys.TokenKeyManager.ReadPublicKey();

                    validacao = Jose.JWT.Decode(authorization, secretKey);

                    var payload = JsonConvert.DeserializeObject<PayLoadToken>(validacao);

                    if (payload != null) {
                        Modelo.Usuario usuario = new Negocio.Usuario().ObterUsuario(Convert.ToInt32(payload.Usuario.Id));

                        LoginConciliadorRequest modeloRequisicao = new LoginConciliadorRequest();
                        modeloRequisicao.CodigoCliente = String.Empty;
                        modeloRequisicao.GrupoEntidade = 20;
                        modeloRequisicao.Senha = usuario.Senha;
                        modeloRequisicao.Usuario = usuario.Email;
                        this.SenhaCriptografada = true;

                        log.GravarLog(EventoLog.RetornoServico, !String.IsNullOrEmpty(validacao));

                        ValidacaoLoginResponse login = this.ValidarLogin(modeloRequisicao);

                        login.AccessToken = authorization;

                        return login;
                    }
                    else {
                        response = new ValidacaoLoginResponse();
                        response.StatusRetorno = new StatusRetorno() { Codigo = Convert.ToInt32(System.Net.HttpStatusCode.Forbidden), Mensagem = "Acesso negado." };

                        throw new WebFaultException<ValidacaoLoginResponse>(response, System.Net.HttpStatusCode.Forbidden);
                    }
                }
                catch (Jose.IntegrityException ex) {
                    log.GravarErro(ex);

                    response = new ValidacaoLoginResponse();
                    response.StatusRetorno = new StatusRetorno() { Codigo = Convert.ToInt32(System.Net.HttpStatusCode.Forbidden), Mensagem = "Acesso negado." };
                    throw new WebFaultException<ValidacaoLoginResponse>(response, System.Net.HttpStatusCode.Forbidden);
                }
                catch (Jose.JoseException ex) {
                    log.GravarErro(ex);

                    response = new ValidacaoLoginResponse();
                    response.StatusRetorno = new StatusRetorno() { Codigo = Convert.ToInt32(System.Net.HttpStatusCode.Forbidden), Mensagem = "Acesso negado." };
                    throw new WebFaultException<ValidacaoLoginResponse>(response, System.Net.HttpStatusCode.Forbidden);
                }
                catch (WebFaultException<ValidacaoLoginResponse> ex) {
                    //log.GravarErro(ex);
                    throw new WebFaultException<ValidacaoLoginResponse>(ex.Detail, ex.StatusCode);
                }
                catch (Exception ex) {
                    log.GravarErro(ex);

                    response = new ValidacaoLoginResponse();
                    response.StatusRetorno = new StatusRetorno() { Codigo = Convert.ToInt32(System.Net.HttpStatusCode.InternalServerError), Mensagem = "Sistema indisponível." };
                    throw new WebFaultException<ValidacaoLoginResponse>(response, System.Net.HttpStatusCode.InternalServerError);
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="atendimento"></param>
        /// <param name="funcional"></param>
        /// <returns></returns>
        public string Token(string id, string atendimento, string funcional) {
            bool batendimento = false;
            if (!Boolean.TryParse(atendimento, out batendimento)) {
                throw new WebFaultException<String>("condição de atendimento inválida", System.Net.HttpStatusCode.Forbidden);
            }
            else {
                WebOperationContext ctx = WebOperationContext.Current;
                ValidacaoLoginResponse response = default(ValidacaoLoginResponse);

                using (Logger log = Logger.IniciarLog("Obtém um token válido para o usuário com ID passado")) {
                    try {
                        if (String.IsNullOrEmpty(id))
                            throw new WebFaultException<String>("Dados inválidos", System.Net.HttpStatusCode.BadRequest);

                        log.GravarLog(EventoLog.ChamadaServico, id);

                        var dadosUsuario = new Negocio.Usuario().ObterUsuario(Convert.ToInt32(id));

                        var utc = new DateTime(1970, 1, 1, 0, 0, 0, 0);


                        var tokenTimeData = DateTime.UtcNow.Subtract(utc);
                        var iat = (int)tokenTimeData.TotalSeconds;
                        var exp = (int)tokenTimeData.Add(new TimeSpan(0, 15, 0)).TotalSeconds;

                        Dictionary<String, Object> payload = null;
                        if (batendimento) {
                            payload = new Dictionary<String, Object>()
                            {
                                { "iss", "authentication-server" },
                                { "iat", iat },
                                { "exp", exp },
                                { "usuario", new
                                    {
                                        Email = dadosUsuario.Email,
                                        Id = dadosUsuario.CodigoIdUsuario,
                                        Nome = dadosUsuario.Nome,
                                        Funcional = funcional,
                                        Atendimento = true
                                    }
                                }
                            };
                        }
                        else {
                            payload = new Dictionary<String, Object>()
                            {
                                { "iss", "authentication-server" },
                                { "iat", iat },
                                { "usuario", new
                                    {
                                        Email = dadosUsuario.Email,
                                        Id = dadosUsuario.CodigoIdUsuario,
                                        Nome = dadosUsuario.Nome
                                    }
                                }
                            };
                        }

                        var secretKey = Keys.TokenKeyManager.ReadPrivateKey();

                        string token = Jose.JWT.Encode(payload, secretKey, Jose.JwsAlgorithm.RS256);

                        log.GravarLog(EventoLog.RetornoServico, dadosUsuario);

                        return token;
                    }
                    catch (Jose.IntegrityException ex) {
                        log.GravarErro(ex);

                        response = new ValidacaoLoginResponse();
                        response.StatusRetorno = new StatusRetorno() { Codigo = Convert.ToInt32(System.Net.HttpStatusCode.Forbidden), Mensagem = "Acesso negado." };
                        throw new WebFaultException<ValidacaoLoginResponse>(response, System.Net.HttpStatusCode.Forbidden);
                    }
                    catch (Jose.JoseException ex) {
                        log.GravarErro(ex);

                        response = new ValidacaoLoginResponse();
                        response.StatusRetorno = new StatusRetorno() { Codigo = Convert.ToInt32(System.Net.HttpStatusCode.Forbidden), Mensagem = "Acesso negado." };
                        throw new WebFaultException<ValidacaoLoginResponse>(response, System.Net.HttpStatusCode.Forbidden);
                    }
                    catch (WebFaultException<ValidacaoLoginResponse> ex) {
                        //log.GravarErro(ex);
                        throw new WebFaultException<ValidacaoLoginResponse>(ex.Detail, ex.StatusCode);
                    }
                    catch (Exception ex) {
                        log.GravarErro(ex);

                        response = new ValidacaoLoginResponse();
                        response.StatusRetorno = new StatusRetorno() { Codigo = Convert.ToInt32(System.Net.HttpStatusCode.InternalServerError), Mensagem = "Sistema indisponível." };
                        throw new WebFaultException<ValidacaoLoginResponse>(response, System.Net.HttpStatusCode.InternalServerError);
                    }
                }
            }
        }

        /// <summary>
        /// Obtém um token válido para o usuário com ID passado
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>JWT para o usuário</returns>
        public String Token(String id) {
            return Token(id, "false", String.Empty);
        }

        #endregion

        #region [Métodos auxiliares]

        /// <summary>
        /// Validar informações e recuperar dados do Login
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="codigoCanal"></param>
        /// <returns></returns>
        private ValidacaoLoginResponse Validar(Usuario usuario, GrupoEntidade grupoEntidade, String codigoCanal) {
            ValidacaoLoginResponse resposta = new ValidacaoLoginResponse();

            Modelo.Usuario usuarioServico = new Modelo.Usuario();
            usuarioServico.Email = usuario.Email;
            usuarioServico.Senha = usuario.Senha;

            if (this.SenhaCriptografada)
                usuarioServico.SenhaCriptografada = usuario.Senha;

            usuarioServico.Entidade = new Modelo.Entidade();
            usuarioServico.Entidade.GrupoEntidade = grupoEntidade.Codigo;

            var loginResposta = new Negocio.Usuario().ValidarLogin(usuarioServico);

            if (loginResposta != null && loginResposta.StatusRetorno != null) {
                resposta.StatusRetorno = new StatusRetorno() {
                    Codigo = loginResposta.StatusRetorno.Codigo,
                    Mensagem = loginResposta.StatusRetorno.Mensagem
                };

                if (loginResposta.StatusRetorno.Codigo == 0) {
                    resposta.Entidades = new List<Entidade>();
                    Usuario informacoesUsuario = default(Usuario);

                    foreach (var estabelecimento in loginResposta.EstabelecimentosRelacionados) {
                        resposta.Entidades.Add(new Entidade() {
                            Codigo = estabelecimento.Codigo,
                            Nome = estabelecimento.NomeEntidade,
                            GrupoEntidade = new GrupoEntidade() { Codigo = estabelecimento.GrupoEntidade }
                        });
                    }

                    if (loginResposta.Usuario != null) {
                        informacoesUsuario = new Usuario() { Nome = loginResposta.Usuario.Nome, Email = loginResposta.Usuario.Email, Senha = loginResposta.Usuario.Senha };

                        this.SessaoUsuario = new Sessao();
                        this.SessaoUsuario.CodigoStatus = (Redecard.PN.Comum.Enumerador.Status)loginResposta.Usuario.Status.Codigo;
                        this.SessaoUsuario.UltimoAcesso = loginResposta.Usuario.DataUltimoAcesso;
                        this.SessaoUsuario.CodigoIdUsuario = loginResposta.Usuario.CodigoIdUsuario;
                        this.SessaoUsuario.Email = loginResposta.Usuario.Email;
                        this.SessaoUsuario.NomeUsuario = loginResposta.Usuario.Nome;
                        this.SessaoUsuario.TipoUsuario = loginResposta.Usuario.TipoUsuario;
                        this.SessaoUsuario.CodigoEntidade = loginResposta.Usuario.Entidade.Codigo;
                        this.SessaoUsuario.Funcional = "CNCLDR";
                        this.SessaoUsuario.CodigoMatriz = 0;
                    }

                    var utc = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    var iat = (int)DateTime.Now.Subtract(utc).TotalSeconds;

                    var payload = new Dictionary<String, Object>()
                    {
                        { "iss", "authentication-server" },
                        { "iat", iat },
                        //{ "entidades", estabelecimentosRelacionados },
                        { "usuario", new
                            {
                                Email = informacoesUsuario.Email,
                                Id = loginResposta.Usuario.CodigoIdUsuario,
                                Nome = informacoesUsuario.Nome
                            }
                        }
                    };

                    var secretKey = Keys.TokenKeyManager.ReadPrivateKey();

                    string token = Jose.JWT.Encode(payload, secretKey, Jose.JwsAlgorithm.RS256);

                    resposta.TokenType = "Bearer";
                    resposta.AccessToken = token;
                }
            }

            return resposta;
        }

        #endregion
    }
}
