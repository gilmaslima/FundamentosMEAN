/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Rede.PN.AtendimentoDigital.SharePoint.Handlers;
using Redecard.PN.Comum;

namespace Rede.PN.AtendimentoDigital.SharePoint.Layouts.PN.AtendimentoDigital
{
    /// <summary>
    /// Handler genérico.
    /// </summary>
    public partial class Handler : HttpTaskAsyncHandler, IReadOnlySessionState
    {
        #region [ Implementação IHttpHandler ]

        /// <summary>
        /// IsReusable
        /// </summary>
        public new Boolean IsReusable { get { return false; } }

        /// <summary>
        /// ProcessRequest.<br/>
        /// Recebe como parâmetro de entrada o JSON no formato:
        /// <code>
        ///  { 
        ///     'handler': 'identificação do handler que será acionado',
        ///     'method': 'identificação do método que será acionado',
        ///     'param1': 'parâmetro 1',
        ///     'param2': 'parâmetro 2',
        ///     ...
        ///     'paramN': 'parâmetro N'
        ///  }
        /// </code>
        /// Retorna como resposta:
        /// <code>
        /// {
        ///     'dados'   : 'dados de retorno',
        ///     'mensagem': 'mensagem de retorno',
        ///     'codigo'  : 0 -- 0 (sucesso) ou 300 (erro genérico)
        /// }
        /// </code>
        /// </summary>
        public override Task ProcessRequestAsync(HttpContext context)
        {
            //Verifica se usuário está autenticado no SharePoint
            Boolean isAuthenticated = context != null &&
                context.Request != null &&
                context.Request.IsAuthenticated;
            
            //se usuário não está autenticado, verifica se executa algum handler anônimo
            if(!isAuthenticated)
            {
                //Guarda dados atuais do contexto
                SPContext currentSPContext = SPContext.Current;

                Action<HttpContext, SPContext> acao = (httpContext, spContext) => Executar(httpContext, null, spContext);
                return Task.Factory.StartNew(() => acao(context, currentSPContext));
            }

            //Validação usuário autenticado e validação CSRF (validação de X-RequestDigest do Header da requisição)
            if (ValidateFormDigest(context))
            {
                //Guarda dados atuais do contexto
                Sessao sessaoAtual = Sessao.Obtem();
                SPContext currentSPContext = SPContext.Current;

                //Cria Action para execução assíncrona
                Action<HttpContext, Sessao, SPContext> acao = (httpContext, sessao, spContext) => Executar(httpContext, sessao, spContext);
                return Task.Factory.StartNew(() => acao(context, sessaoAtual, currentSPContext));
            }
            else
            {
                //tratamento padrão caso não possua permissão
                WriteResponse(context, HttpStatusCode.Forbidden, new HandlerResponse((Int32)HttpStatusCode.Forbidden, "acesso negado"));
                return Task.FromResult<Object>(null);
            }
        }

        /// <summary>
        /// Verifica requisição e aciona handler específico.
        /// </summary>
        /// <param name="httpContext">HttpContext da requisição</param>
        /// <param name="sessao">Dados da sessão do usuário</param>
        /// <param name="spContext">SPContext.Current</param>
        private void Executar(HttpContext httpContext, Sessao sessao, SPContext spContext)
        {
            String handlerName = String.Empty;
            String methodName = String.Empty;
            String requestType = String.Empty;

            try
            {                
                //recupera configuração da requisição
                handlerName = httpContext.Request["handler"];
                methodName = httpContext.Request["method"];
                requestType = httpContext.Request.RequestType.ToUpper();

                //Verifica se foi informado a identificação do handler que será acionado
                if (String.IsNullOrWhiteSpace(handlerName))
                {
                    Logger.GravarErro("Requisição inválida: handler não informado");
                    WriteResponse(httpContext,
                        HttpStatusCode.BadRequest,
                        new HandlerResponse((Int32)HttpStatusCode.BadRequest, "requisição inválida"));
                    return;
                }

                //Recupera a referência do Type do handler que será acionado
                String handlerClassName = String.Concat(typeof(HandlerBase).Namespace, ".", handlerName);
                Type handlerType = typeof(HandlerBase).Assembly.GetType(handlerClassName, false);
                if (handlerType == null)
                {
                    Logger.GravarErro("Requisição inválida: handler " + handlerName + " não encontrado");
                    WriteResponse(httpContext,
                        HttpStatusCode.BadRequest,
                        new HandlerResponse((Int32)HttpStatusCode.BadRequest, "requisição inválida"));
                    return;
                }

                //Verifica se foi informado a identificação do handler que será acionado
                if (String.IsNullOrWhiteSpace(methodName))
                {
                    Logger.GravarErro("Requisição inválida: método não informado");
                    WriteResponse(httpContext,
                        HttpStatusCode.BadRequest,
                        new HandlerResponse((Int32)HttpStatusCode.BadRequest, "requisição inválida"));
                    return;
                } 

                //Recupera método informado
                MethodInfo metodo = handlerType.GetMethod(methodName);
                if (metodo == null)
                {
                    Logger.GravarErro("Requisição inválida: método " + methodName + " não encontrado");
                    WriteResponse(httpContext, 
                        HttpStatusCode.BadRequest,
                        new HandlerResponse((Int32)HttpStatusCode.BadRequest, "requisição inválida"));
                    return;
                }

                //Verifica se método permite ser chamado sem objeto de Sessão (quando chamado via AuthenticationService - app/mobile)
                HttpNoSessionAttribute noSessionAttr = GetAttribute<HttpNoSessionAttribute>(metodo);
                Boolean sessionRequired = true;
                if (noSessionAttr != null && noSessionAttr.AllowNoSession)
                {
                    sessionRequired = false;
                }

                if (sessionRequired && sessao == null)
                {
                    Logger.GravarLog("Requisição inválida: sessão é obrigatória [" + handlerName + ":" + methodName + "]");
                    WriteResponse(httpContext,
                        HttpStatusCode.Forbidden,
                        new HandlerResponse((Int32)HttpStatusCode.Forbidden, "acesso anônimo negado"));
                    return;
                }

                //Verifica se método possui configuração de permissão por código de serviço ou url da página
                AuthorizeAttribute authorizeAttr = GetAttribute<AuthorizeAttribute>(metodo);
                if (authorizeAttr != null)
                {
                    //sessão é obrigatória para validação de autorização
                    if (sessao == null)
                    {
                        Logger.GravarLog("Requisição inválida: sessão é obrigatória para método com autorização [" + handlerName + ":" + methodName + "]");
                        WriteResponse(httpContext,
                            HttpStatusCode.Forbidden,
                            new HandlerResponse((Int32)HttpStatusCode.Forbidden, "autorização para acesso anônimo negado"));
                        return;
                    }

                    //se não for usuário master, precisa validar permissões
                    if (!sessao.UsuarioMaster())
                    {                        
                        //validação através da url da página
                        if(authorizeAttr.UrlPaginas != null)
                        {
                            //verifica se a intersecção entre as urls das páginas que o usuário possui acesso
                            //e as páginas que o método exige é maior que 0 (ou seja, usuário possui autorização)
                            Boolean hasAuthorization = authorizeAttr.UrlPaginas.Any(urlPagina => sessao.VerificarAcessoPagina(urlPagina));
                            if (!hasAuthorization)
                            {
                                Logger.GravarLog("Requisição inválida: usuário não possui autorização de URL para chamar método [" + handlerName + ":" + methodName + "]");
                                WriteResponse(httpContext,
                                    HttpStatusCode.Forbidden,
                                    new HandlerResponse((Int32)HttpStatusCode.Forbidden, "sem autorização de url"));
                                return;
                            }
                        }
                        
                        //validação através do código do serviço
                        if(authorizeAttr.CodigoServicos != null)
                        {
                            //recupera serviços que o usuário possui permissão
                            Menu[] menus = sessao.Servicos.Flatten(itemMenu => itemMenu.Items).ToArray();
                            Int32[] codigoServicos = menus.Select(itemMenu => itemMenu.Codigo).ToArray();
                            Boolean hasAuthorization = codigoServicos.Intersect(authorizeAttr.CodigoServicos).Any();

                            if (!hasAuthorization)
                            {
                                Logger.GravarLog("Requisição inválida: usuário não possui autorização de serviço para chamar método [" + handlerName + ":" + methodName + "]");
                                WriteResponse(httpContext,
                                    HttpStatusCode.Forbidden,
                                    new HandlerResponse((Int32)HttpStatusCode.Forbidden, "sem autorização de serviço"));
                                return;
                            }
                        }
                    }
                }

                //Verifica se o tipo de requisição (GET,POST,DELETE,PUT) está sendo tratado pelo método do handler.
                Boolean requisicaoValida = 
                    (requestType.EqualsIgnoreCase("GET") && HasAttribute(metodo, typeof(HttpGetAttribute))) ||
                    (requestType.EqualsIgnoreCase("POST") && HasAttribute(metodo, typeof(HttpPostAttribute))) ||
                    (requestType.EqualsIgnoreCase("DELETE") && HasAttribute(metodo, typeof(HttpDeleteAttribute))) ||
                    (requestType.EqualsIgnoreCase("PUT") && HasAttribute(metodo, typeof(HttpPutAttribute)));

                //Em caso positivo, executa a ação. Caso contrário, a requisição é inválida.
                if (requisicaoValida)
                {                    
                    //Criação da instância do handler
                    HandlerBase handler = (HandlerBase)Activator.CreateInstance(handlerType);
                    handler.Request = httpContext.Request;
                    handler.Sessao = sessao;
                    handler.CurrentSPContext = spContext;

                    //Execução do método do handler
                    HandlerResponse retorno = (HandlerResponse) metodo.Invoke(handler, null);
                    if (retorno == null)
                    {
                        Logger.GravarErro("O método chamada não está implementado corretamente: é obrigatório o retorno do objeto HttpResponse");
                        WriteResponse(httpContext,
                            HttpStatusCode.InternalServerError,
                            new HandlerResponse((Int32)HttpStatusCode.InternalServerError, "erro interno", null, 
                                Criptografia.Encrypt("O método chamada não está implementado corretamente: é obrigatório o retorno do objeto HttpResponse")));
                        return;
                    }

                    //Retorna o resultado da execução
                    WriteResponse(httpContext, HttpStatusCode.OK, retorno);
                }
                else
                {
                    Logger.GravarErro("Requisição inválida [" + handlerName + ":" + methodName + "]");
                    WriteResponse(httpContext,
                        HttpStatusCode.MethodNotAllowed,
                        new HandlerResponse((Int32)HttpStatusCode.MethodNotAllowed, "requisição inválida"));
                }                
            }
            catch (ArgumentException ex)
            {
                //tratamento de erro genérico 1. A mensagem do erro estará criptografada.
                Logger.GravarErro("Erro genérico 1 [" + handlerName + ":" + methodName + "]", ex);
                WriteResponse(httpContext,
                    HttpStatusCode.InternalServerError,                
                    new HandlerResponse(300, "erro genérico 1", null, Criptografia.Encrypt(ex.Message)));
            }
            catch (Exception ex)
            {
                //tratamento de erro genérico 1. A mensagem do erro estará criptografada.
                Logger.GravarErro("Erro genérico 2 [" + handlerName + ":" + methodName + "]", ex);
                WriteResponse(httpContext,
                    HttpStatusCode.InternalServerError,
                    new HandlerResponse(301, "erro genérico 2", null, Criptografia.Encrypt(ex.Message)));
            }
        }

        /// <summary>
        /// Prepara response.
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="httpStatusCode">Código HTTP do response</param>
        /// <param name="responseData">Dados que serão retornados em JSON</param>
        private static void WriteResponse(HttpContext context, HttpStatusCode httpStatusCode, HandlerResponse responseData)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (Int32)httpStatusCode;
            context.Response.Write(SerializarResposta(responseData));
        }

        /// <summary>
        /// Verifica se um método possui determinado atributo.
        /// </summary>
        /// <param name="method">Método</param>
        /// <param name="attributeType">Tipo do atributo</param>
        /// <returns>Se possui o atributo</returns>
        private static Boolean HasAttribute(MethodInfo method, Type attributeType)
        {
            return method.GetCustomAttributes(attributeType).Any();
        }

        /// <summary>
        /// Obtém determinado atributo de um método.
        /// </summary>
        /// <param name="method">Método</param>
        /// <param name="attributeType">Tipo do atributo</param>
        /// <returns>Atributo, caso exista</returns>
        private static TAttr GetAttribute<TAttr>(MethodInfo method) where TAttr : Attribute
        {
            return method.GetCustomAttributes<TAttr>().FirstOrDefault();
        }

        /// <summary>
        /// Serializar resposta JSON.
        /// </summary>
        /// <param name="retorno">Objeto de retorno a ser serializado</param>
        /// <returns>Objeto de retorno serializado</returns>
        private static String SerializarResposta(HandlerResponse retorno)
        {
            var jsSerializer = new JavaScriptSerializer();

            if (retorno == null)
            {
                retorno = new HandlerResponse();
                retorno.Mensagem = "void";
            }

            return jsSerializer.Serialize(new
            {
                dados = retorno.Dados,
                mensagem = retorno.Mensagem,
                codigo = retorno.Codigo,
                detalhes = retorno.DetalhesErro
            });
        }

        /// <summary>
        /// ValidateFormDigest
        /// </summary>
        /// <returns>Validação</returns>
        private Boolean ValidateFormDigest(HttpContext httpContext)
        {
            Boolean valido = false;
            try
            {
                String requestType = httpContext.Request.RequestType.ToUpper();
                if(requestType.EqualsIgnoreCase("GET"))
                    valido = true;
                else
                    valido = SPUtility.ValidateFormDigest();
            }
            catch (SPException ex)
            {
                Logger.GravarErro("ValidateFormDigest=false", ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("ValidateFormDigest=false", ex);
            }
            return valido;
        }

        #endregion
    }
}