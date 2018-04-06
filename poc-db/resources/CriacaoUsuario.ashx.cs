using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario;
using System;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using ControlTemplates = Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais.Handlers.Usuario
{
    public class CriacaoUsuario : UserControlBase, IHttpHandler, IRequiresSessionState
    {
        /// <summary>
        /// Marca handler como reusável
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Controla o request e o retorno para a camada de client side
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            // instancia a model de retorno
            CriacaoUsuarioHandlerResponse modelException = new CriacaoUsuarioHandlerResponse();

            using (Logger log = Logger.IniciarLog("GetPvsAsync() - Busca os Pvs pelo CNPJ/CPF informado"))
            {
                try
                {
                    String nomeMetodo = context.Request.Params["method"];

                    // valida os CPF/CNPJ informado
                    if (String.IsNullOrWhiteSpace(nomeMetodo) && String.IsNullOrWhiteSpace(nomeMetodo))
                    {
                        CriacaoUsuarioHandlerResponse model = new CriacaoUsuarioHandlerResponse();
                        model.MensagemRetorno = "Nome do método não informado para a consulta";
                        this.ReturnSerialize(context, model, 500);
                        return;
                    }

                    var method = this.GetType().GetMethod(nomeMetodo,
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    method.Invoke(this, new Object[] { context });
                }
                catch (TargetException ex)
                {
                    GravarParametrosLog(context, log);
                    log.GravarErro(ex);
                    var model = UsuarioNegocio.ConstruirMensagemErro(500, ex.Message, stackTraceExcecao: ex.ToString(), showModal: false);

                    // verifica se deve exibir o captcha
                    model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);

                    this.ReturnSerialize(context, model);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    GravarParametrosLog(context, log);
                    log.GravarErro(ex);

                    var model = UsuarioNegocio.ConstruirMensagemErro(500, ex.Detail.Fonte, ex.Detail.Codigo, showModal: false);

                    // verifica se deve exibir o captcha
                    model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);

                    this.ReturnSerialize(context, model);
                }
                catch (CommunicationException ex)
                {
                    GravarParametrosLog(context, log);
                    log.GravarErro(ex);

                    var model = UsuarioNegocio.ConstruirMensagemErro(500, ex.Message, stackTraceExcecao: ex.ToString(), showModal: false);

                    // verifica se deve exibir o captcha
                    model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);

                    this.ReturnSerialize(context, model);
                }
                catch (TimeoutException ex)
                {
                    GravarParametrosLog(context, log);
                    log.GravarErro(ex);

                    var model = UsuarioNegocio.ConstruirMensagemErro(500, ex.Message, stackTraceExcecao: ex.ToString(), showModal: false);

                    // verifica se deve exibir o captcha
                    model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);

                    this.ReturnSerialize(context, model);
                }
                catch (HttpException ex)
                {
                    GravarParametrosLog(context, log);
                    log.GravarErro(ex);

                    var model = UsuarioNegocio.ConstruirMensagemErro(500, ex.Message, stackTraceExcecao: ex.ToString(), showModal: false);

                    // verifica se deve exibir o captcha
                    model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);

                    this.ReturnSerialize(context, model);
                }
                catch (Exception ex)
                {
                    GravarParametrosLog(context, log);
                    log.GravarErro(ex);

                    var model = UsuarioNegocio.ConstruirMensagemErro(500, ex.Message, stackTraceExcecao: ex.ToString(), showModal: false);

                    // verifica se deve exibir o captcha
                    model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);

                    this.ReturnSerialize(context, model);
                }
            }
        }
        
        #region [ Negócio ]

        /// <summary>
        /// Método para consulta dos PVs junto à camada de serviços
        /// </summary>
        /// <param name="context">Contexto da comunicação atual</param>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        private void ConsultarPvs(HttpContext context)
        {
            // model para retorno
            CriacaoUsuarioHandlerResponse model = new CriacaoUsuarioHandlerResponse();

            // mensagens de retorno
            Boolean useModal = false;
            String msgRetorno = String.Empty;
            QuadroAvisoModel avisoRetorno = null;

            string
                cpfEstb = context.Request.Params["cpfEstb"],
                cnpjEstb = context.Request.Params["cnpjEstb"],
                cpfUsuario = context.Request.Params["cpfUsuario"],
                email = context.Request.Params["emailUsuario"],
                captchaResponse = context.Request.Params["captchaResponse"];

            #region Valida se os parâmetros foram devidamente informados

            // valida os CPF/CNPJ informado
            if (String.IsNullOrWhiteSpace(cpfEstb) && String.IsNullOrWhiteSpace(cnpjEstb))
            {
                model.MensagemRetorno = "CPF/CNPJ não informado para a consulta";
                this.ReturnSerialize(context, model, 500);
                return;
            }

            // valida o e-mail
            if (String.IsNullOrWhiteSpace(email))
            {
                model.MensagemRetorno = "E-mail não informado para a consulta";
                this.ReturnSerialize(context, model, 500);
                return;
            }

            // incrementa e valida o contador do Recaptcha
            ControlTemplates.RecaptchaGoogle.IncrementTentativas(context, true);
            model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);
            if (!ControlTemplates.RecaptchaGoogle.ValidateCaptcha(context, captchaResponse, true))
            {
                model.TituloModal = "Atenção";
                model.MensagemRetorno = "Código (captcha) inválido.";
                model.FlagModal = true;
                model.TipoModal = TipoModal.Error;
                this.ReturnSerialize(context, model, 500);
                return;
            }

            #endregion

            // remove caracteres especiais
            Regex regexNumber = new Regex("[^0-9]");
            cpfEstb = regexNumber.Replace(cpfEstb, "");
            cnpjEstb = regexNumber.Replace(cnpjEstb, "");
            cpfUsuario = regexNumber.Replace(cpfUsuario, "");

            // valida se o CNPJ é uma matriz
            if (!String.IsNullOrEmpty(cnpjEstb) &&
                !UsuarioNegocio.ValidarCnpjEstabelecimentoCriacaoUsuario(cnpjEstb, out avisoRetorno))
            {
                model.TituloModal = avisoRetorno.Titulo;
                model.MensagemRetorno = avisoRetorno.Mensagem;
                model.FlagModal = true;
                model.TipoModal = avisoRetorno.Tipo;
                this.ReturnSerialize(context, model, 500);
                return;
            }

            // converte o CPF/CNPJ em formato aceito pelo serviço
            long?
                cpfEstbLong = cpfEstb.ToInt64Null(null),
                cnpjEstbLong = cnpjEstb.ToInt64Null(null),
                cpfUsuarioLong = cpfUsuario.ToInt64Null(null);

            // consulta os PVs pelos dados informados
            model.Pvs = UsuarioNegocio.GetPvsCriacaoUsuario(email, cpfEstbLong, cnpjEstbLong, out avisoRetorno);
            if (avisoRetorno != null)
            {
                model.TituloModal = avisoRetorno.Titulo;
                model.MensagemRetorno = avisoRetorno.Mensagem;
                model.TipoModal = avisoRetorno.Tipo;
                model.FlagModal = model.TipoModal != TipoModal.None;

                if (model.TipoModal == TipoModal.Error)
                {
                    // mediante erro decrementa o contador do captcha
                    ControlTemplates.RecaptchaGoogle.DecreaseTentativas(context, true);
                    model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);

                    this.ReturnSerialize(context, model, 500);
                    return;
                }
            }

            // valida o domínio do e-mail informado
            if (!UsuarioNegocio.ValidarDominioEmailCriacaoUsuario(email, cpfEstbLong, cnpjEstbLong, out msgRetorno))
            {
                model.MensagemRetorno = msgRetorno;
                model.FieldValidationReturn = "email";
                this.ReturnSerialize(context, model);
                return;
            }

            model.OcultarCpfSocio = false;
            model.PrimeiroUsuarioPv = false;

            // regras exclusivas para o cenário: 1 CPF/CNPJ <-> 1 PV
            if (model.Pvs.Count == 1)
            {
                // valida o e-mail do usuário
                if (!UsuarioNegocio.ValidarEmailCriacaoUsuario(email, model.Pvs[0].NumeroPv, out msgRetorno, out useModal))
                {
                    model.MensagemRetorno = msgRetorno;
                    model.FieldValidationReturn = "email";
                    model.FlagModal = useModal;
                    if (model.FlagModal)
                        model.TipoModal = TipoModal.Warning;
                    this.ReturnSerialize(context, model);
                    return;
                }

                // verifica se deve ocultar o campo de COF/CNPJ do sócio (somente para cadastro de PJ)
                if (cnpjEstbLong.GetValueOrDefault(0) > 0 && cpfUsuarioLong.HasValue)
                {
                    model.OcultarCpfSocio = UsuarioNegocio.VerificarCpfSocioCriacaoUsuario(
                        model.Pvs[0].NumeroPv,
                        cpfUsuarioLong.Value);
                }

                // verifica se é o primeiro usuário do PV
                model.PrimeiroUsuarioPv = !model.Pvs[0].PossuiUsuario.GetValueOrDefault(false);
            }

            // validação final para os PVs
            if (!UsuarioNegocio.TrataPvsCriacaoUsuario(model.Pvs, out avisoRetorno))
            {
                model.TituloModal = avisoRetorno.Titulo;
                model.MensagemRetorno = avisoRetorno.Mensagem;
                model.TipoModal = avisoRetorno.Tipo;
                model.FlagModal = model.TipoModal != TipoModal.None;

                if (model.TipoModal == TipoModal.Error)
                {
                    // mediante erro decrementa o contador do captcha
                    ControlTemplates.RecaptchaGoogle.DecreaseTentativas(context, true);
                    model.ExibirRecaptcha = UsuarioNegocio.ExibirRecaptcha(context);

                    this.ReturnSerialize(context, model, 500);
                    return;
                }
            }

            this.ReturnSerialize(context, model);
        }

        /// <summary>
        /// Consulta se deve exibir o campo de CPF/CNPJ do sócio
        /// </summary>
        /// <param name="context">Contexto da comunicação</param>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        private void ConsultarOcultarCpfSocio(HttpContext context)
        {
            // model para retorno
            CriacaoUsuarioHandlerResponse model = new CriacaoUsuarioHandlerResponse();

            Regex regexNumber = new Regex("[^0-9]");
            String cpfUsuario = regexNumber.Replace(context.Request.Params["cpfUsuario"], "");
            String pvSelecionado = regexNumber.Replace(context.Request.Params["pvSelecionado"], "");

            // valida parâmetros de entrada
            if (String.IsNullOrEmpty(cpfUsuario))
            {
                model.MensagemRetorno = "CPF não informado";
                this.ReturnSerialize(context, model, 500);
                return;
            }

            // valida parâmetros de entrada
            if (String.IsNullOrEmpty(pvSelecionado))
            {
                model.MensagemRetorno = "Estabelecimento não informado";
                this.ReturnSerialize(context, model, 500);
                return;
            }

            long cpfUsuarioLong = 0;
            if (!long.TryParse(cpfUsuario, out cpfUsuarioLong))
            {
                model.MensagemRetorno = "CPF inválido";
                this.ReturnSerialize(context, model, 500);
                return;
            }

            int pvSelecionadoInt = 0;
            if (!int.TryParse(pvSelecionado, out pvSelecionadoInt))
            {
                model.MensagemRetorno = "Estabelecimento inválido";
                this.ReturnSerialize(context, model, 500);
                return;
            }

            model.OcultarCpfSocio = UsuarioNegocio.VerificarCpfSocioCriacaoUsuario(pvSelecionadoInt, cpfUsuarioLong);

            this.ReturnSerialize(context, model);
        }

        /// <summary>
        /// Consulta se é o primeiro usuário do PV
        /// </summary>
        /// <param name="context">Contexto da comunicação atual</param>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException">Timeout na implementação do Regex para tratamento dos valores informados</exception>
        private void ConsultarPrimeiroUsuarioPv(HttpContext context)
        {
            // model para retorno
            CriacaoUsuarioHandlerResponse model = new CriacaoUsuarioHandlerResponse();

            string
                email = context.Request.Params["emailUsuario"],
                cpfEstb = context.Request.Params["cpfEstb"],
                pv = context.Request.Params["pv"];

            // valida parâmetros de entrada
            if (string.IsNullOrWhiteSpace(cpfEstb) && string.IsNullOrWhiteSpace(pv))
            {
                model.MensagemRetorno = "Nenhum parâmetro informado para a consulta";
                this.ReturnSerialize(context, model, 500);
                return;
            }

            // remove caracteres especiais
            Regex regexNumber = new Regex("[^0-9]");
            cpfEstb = regexNumber.Replace(cpfEstb, "");
            pv = regexNumber.Replace(pv, "");

            long? cpfEstbLong = !String.IsNullOrEmpty(cpfEstb) ? (long?)long.Parse(cpfEstb) : null;

            Int32 pvInt = 0;
            if (!Int32.TryParse(pv, out pvInt))
            {
                model.MensagemRetorno = "Estabelecimento não informado";
                this.ReturnSerialize(context, model, 500);
                return;
            }

            // consulta os PVs pelos dados informados
            QuadroAvisoModel msgRetorno = null;
            model.Pvs = UsuarioNegocio.GetPvsCriacaoUsuario(email, cpfEstbLong, null, out msgRetorno);
            if (msgRetorno != null)
            {
                model.TituloModal = msgRetorno.Titulo;
                model.MensagemRetorno = msgRetorno.Mensagem;
                model.TipoModal = msgRetorno.Tipo;
                model.FlagModal = true;

                if (model.TipoModal == TipoModal.Error)
                {
                    this.ReturnSerialize(context, model, 500);
                    return;
                }
            }

            model.Pvs = model.Pvs.Where(x => x.NumeroPv == pvInt).ToList();
            if (model.Pvs.Count == 0)
            {
                model.MensagemRetorno = "Estabelecimento informado é inválido";
                this.ReturnSerialize(context, model, 500);
                return;
            }

            // verifica se é o primeiro usuário do PV
            model.PrimeiroUsuarioPv = !model.Pvs[0].PossuiUsuario.GetValueOrDefault(false);

            this.ReturnSerialize(context, model);
        }

        /// <summary>
        /// Método de business para validar a confirmação positiva
        /// </summary>
        /// <param name="context">Contexto da comunicação atual</param>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException">Timeout na implementação do Regex para tratamento dos valores informados</exception>
        private void ValidarConfirmacaoPositiva(HttpContext context)
        {
            // model para retorno
            CriacaoUsuarioHandlerResponse model = new CriacaoUsuarioHandlerResponse();

            String
                cnpjEstb = context.Request.Params["cnpjEstb"],
                cpfProprietario = context.Request.Params["cpfProprietario"],
                emailUsuario = context.Request.Params["emailUsuario"],
                banco = context.Request.Params["banco"],
                agencia = context.Request.Params["agencia"],
                conta = context.Request.Params["conta"],
                cpfSocio = context.Request.Params["cpfSocio"];

            // remove caracteres especiais
            Regex regexNumber = new Regex("[^0-9]");
            cnpjEstb = regexNumber.Replace(cnpjEstb, "");
            cpfProprietario = regexNumber.Replace(cpfProprietario, "");
            cpfSocio = regexNumber.Replace(cpfSocio, "");

            #region Validação dos parâmetros informados

            if (String.IsNullOrWhiteSpace(cnpjEstb) && String.IsNullOrWhiteSpace(cpfProprietario))
            {
                model.MensagemRetorno = "Dados do estabelecimento não informados";
                model.FlagModal = true;
                model.TipoModal = TipoModal.Error;
                this.ReturnSerialize(context, model, 500);
                return;
            }

            if (String.IsNullOrWhiteSpace(emailUsuario))
            {
                model.MensagemRetorno = "E-mail do usuário não informado";
                model.FlagModal = true;
                model.TipoModal = TipoModal.Error;
                this.ReturnSerialize(context, model, 500);
                return;
            }

            if (String.IsNullOrWhiteSpace(banco))
            {
                model.MensagemRetorno = "Banco não informado para validação";
                model.FlagModal = true;
                model.TipoModal = TipoModal.Error;
                this.ReturnSerialize(context, model, 500);
                return;
            }

            if (String.IsNullOrWhiteSpace(agencia))
            {
                model.MensagemRetorno = "Agência não informado para validação";
                model.FlagModal = true;
                model.TipoModal = TipoModal.Error;
                this.ReturnSerialize(context, model, 500);
                return;
            }

            if (String.IsNullOrWhiteSpace(conta))
            {
                model.MensagemRetorno = "Conta corrente não informado para validação";
                model.FlagModal = true;
                model.TipoModal = TipoModal.Error;
                this.ReturnSerialize(context, model, 500);
                return;
            }

            String pvsSerialized = Convert.ToString(context.Request.Params["pvs"]);
            if (String.IsNullOrEmpty(pvsSerialized))
            {
                model.MensagemRetorno = "Estabelecimento não informado";
                this.ReturnSerialize(context, model, 500);
                return;
            }

            String[] pvs = (String[])new JavaScriptSerializer().Deserialize(pvsSerialized, typeof(String[]));
            if (pvs == null || pvs.Length == 0)
            {
                model.MensagemRetorno = "Estabelecimento não informado";
                this.ReturnSerialize(context, model, 500);
                return;
            }

            regexNumber = new Regex("[^0-9]");
            Int32[] pvsInt = pvs.Select(x => Convert.ToInt32(regexNumber.Replace(x, ""))).ToArray();

            #endregion

            long?
                cnpjEstbLong = cnpjEstb.ToInt64Null(),
                cpfProprietarioLong = cpfProprietario.ToInt64Null(),
                cpfSocioLong = cpfSocio.ToInt64Null();

            QuadroAvisoModel msgRetorno = null;
            if (!UsuarioNegocio.ValidarConfirmacaoPositiva(
                cnpjEstbLong,
                cpfProprietarioLong,
                emailUsuario, pvsInt,
                cpfSocioLong, banco, agencia, conta,
                out msgRetorno))
            {
                model.MensagemRetorno = msgRetorno.Mensagem;
                model.TipoModal = msgRetorno.Tipo;
                model.FlagModal = true;
            }

            this.ReturnSerialize(context, model);
        }

        #endregion

        #region [ Métodos auxiliares ]
        /// <summary>
        /// Caso ocorra uma excecao, salva os dados no LOG
        /// </summary>
        /// <param name="context"></param>
        /// <param name="log"></param>
        private void GravarParametrosLog(HttpContext context, Logger log)
        {
            StringBuilder parametros = new StringBuilder();
            foreach (String qsItem in context.Request.Params.AllKeys)
            {
                parametros.AppendFormat("{0}: {1} {2}", qsItem, context.Request.Params[qsItem], Environment.NewLine);
            }

            log.GravarErro(new Exception(parametros.ToString()));
        }

        /// <summary>
        /// Serializa o conteúdo
        /// </summary>
        /// <param name="context">Objeto de contexto</param>
        /// <param name="statusCode">Código de retorno HTTP (default 200)</param>
        /// <exception cref="System.InvalidOperationException">Operação inválida no serializer</exception>
        /// <exception cref="System.ArgumentException">Argumento inválido passado para o serializer</exception>
        private void ReturnSerialize(HttpContext context, ConsultaPvsHandlerResponse model, Int32 statusCode = 200)
        {
            if (model.StatusCode == 0)
                model.StatusCode = statusCode;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            context.Response.StatusCode = model.StatusCode;
            context.Response.Write(serializer.Serialize(model));
        }

        #endregion
    }
}
