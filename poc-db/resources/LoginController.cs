using Rede.PN.ApiLogin.Models;
using Rede.PN.ApiLogin.Models.Response;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Rede.PN.ApiLogin.Controllers
{
    public class LoginController : ApiController
    {
        /// <summary>
        /// Informações de sessão do usuário para registro de Login
        /// </summary>
        private Sessao SessaoUsuario { get; set; }
        
        // POST api/login
        [HttpPost]
        public HttpResponseMessage ValidarLogin(Models.Request.LoginConciliadorRequest modeloRequisicao)
        {
            HttpResponseMessage response = default(HttpResponseMessage);
            using (Logger log = Logger.IniciarLog("Validação do Login"))
            {
                try
                {
                    Usuario usuario = new Usuario() { Email = modeloRequisicao.Usuario, Senha = modeloRequisicao.Senha };
                    GrupoEntidade grupo = new GrupoEntidade() { Codigo = modeloRequisicao.GrupoEntidade };

                    var loginResposta = this.Validar(usuario, grupo, modeloRequisicao.CodigoCliente);

                    Int32[] codigosErroNaoPermitido = { 391, 1103, 404, 415 };

                    if (loginResposta.StatusRetorno != null && loginResposta.StatusRetorno.Codigo == 0)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK, loginResposta);

                        //Registra login no histórico/log
                        Historico.Login(SessaoUsuario, (DateTime?)null);
                    }
                    else if (loginResposta.StatusRetorno != null && codigosErroNaoPermitido.Contains(loginResposta.StatusRetorno.Codigo))
                        response = Request.CreateResponse(HttpStatusCode.Unauthorized, loginResposta);
                    else
                        response = Request.CreateResponse(HttpStatusCode.BadRequest, loginResposta);

                }
                catch (Jose.IntegrityException ex)
                {
                    log.GravarErro(ex);
                    response = Request.CreateResponse(HttpStatusCode.Forbidden, new { mensagem = "Sistema indisponível." });
                }
                catch (Jose.JoseException ex)
                {
                    log.GravarErro(ex);
                    response = Request.CreateResponse(HttpStatusCode.Forbidden, new { mensagem = "Sistema indisponível." });
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensagem = ex.Message, stack = ex.StackTrace });
                }
            }
            return response;
        }

        /// <summary>
        /// Validar informações e recuperar dados do Login
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="grupoEntidade"></param>
        /// <param name="codigoCanal"></param>
        /// <returns></returns>
        private ValidacaoLoginResponse Validar(Usuario usuario, GrupoEntidade grupoEntidade, String codigoCanal)
        {
            ValidacaoLoginResponse resposta = new ValidacaoLoginResponse();

            Modelo.Usuario usuarioServico = new Modelo.Usuario();
            usuarioServico.Email = usuario.Email;
            usuarioServico.Senha = usuario.Senha;

            usuarioServico.Entidade = new Modelo.Entidade();
            usuarioServico.Entidade.GrupoEntidade = grupoEntidade.Codigo;

            var loginResposta = new Negocio.Usuario().ValidarLogin(usuarioServico);

            if (loginResposta != null && loginResposta.StatusRetorno != null)
            {
                resposta.StatusRetorno = new StatusRetorno()
                {
                    Codigo = loginResposta.StatusRetorno.Codigo,
                    Mensagem = loginResposta.StatusRetorno.Mensagem
                };

                if (loginResposta.StatusRetorno.Codigo == 0)
                {
                    resposta.Entidades = new List<Entidade>();
                    Usuario informacoesUsuario = default(Usuario);

                    foreach (var estabelecimento in loginResposta.EstabelecimentosRelacionados)
                    {
                        resposta.Entidades.Add(new Entidade()
                        {
                            Codigo = estabelecimento.Codigo,
                            Nome = estabelecimento.NomeEntidade,
                            GrupoEntidade = new GrupoEntidade() { Codigo = estabelecimento.GrupoEntidade }
                        });
                    }

                    if (loginResposta.Usuario != null)
                    {
                        informacoesUsuario = new Usuario() { Nome = loginResposta.Usuario.Nome, Email = loginResposta.Usuario.Email };

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

                    var payload = new Dictionary<String, Object>()
                    {
                        { "iss", "authentication-server" },
                        //{ "entidades", estabelecimentosRelacionados },
                        { "usuario", informacoesUsuario }
                    };

                    var secretKey = Keys.TokenKeyManager.ReadPrivateKey();
                    
                    //string token = JWT.JsonWebToken.Encode(payload, secretKey, JWT.JwtHashAlgorithm.RS256);

                    string token = Jose.JWT.Encode(payload, secretKey, Jose.JwsAlgorithm.RS256);
                    
                    resposta.TokenType = "Bearer";
                    resposta.AccessToken = token; 
                }
            }

            return resposta;
        }

        [HttpGet]
        public HttpResponseMessage ValidarToken()
        {
            HttpResponseMessage response = default(HttpResponseMessage);

            using (Logger log = Logger.IniciarLog("Validação do Token"))
            {
                try
                {
                    string token = Request.Headers.Authorization.ToString();

                    log.GravarLog(EventoLog.RetornoServico, token);

                    var secretKey = Keys.TokenKeyManager.ReadPublicKey();

                    //string validacao = JWT.JsonWebToken.Decode(token, secreKey);

                    string validacao = Jose.JWT.Decode(token, secretKey);

                    response = Request.CreateResponse(HttpStatusCode.OK, !String.IsNullOrWhiteSpace(validacao));
                }
                catch (Jose.IntegrityException ex)
                {
                    log.GravarErro(ex);
                    response = Request.CreateResponse(HttpStatusCode.Forbidden, new { mensagem = "Token inválido." });
                }
                catch (Jose.JoseException ex)
                {
                    log.GravarErro(ex);
                    response = Request.CreateResponse(HttpStatusCode.Forbidden, new { mensagem = "Token inválido." });
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    response = Request.CreateResponse(HttpStatusCode.InternalServerError, new { mensagem = "Sistema indisponível." });
                }
                
                log.GravarLog(EventoLog.RetornoServico, response);

            }
            return response;
        }
    }
}
