using Redecard.PN.Comum;
using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    public partial class RecaptchaGoogle : UserControl
    {
        /// <summary>
        /// Quantidade máxima de tentativas antes de exibir o Captcha
        /// </summary>
        public const Int32 quantidadeMaximaTentativas = 3;

        #region [ Atributos do controle ]

        /// <summary>
        /// Define se a validação será inteiramente em client-side
        /// </summary>
        public Boolean ValidacaoTotalClientSide
        {
            get
            {
                return ((Boolean?)ViewState["ValidacaoTotalClientSide"]).GetValueOrDefault(false);
            }
            set
            {
                ViewState["ValidacaoTotalClientSide"] = value;
                this.pnlGrcCaptcha.Style["display"] = value ? "none" : "block";
            }
        }

        /// <summary>
        /// Function js a ser chamada quanto o captcha estiver OK
        /// </summary>
        public String ClientValidationExternalFunction
        {
            get
            {
                return this.pnlGrcCaptcha.Attributes["data-captcha-external-action"];
            }
            set
            {
                this.pnlGrcCaptcha.Attributes["data-captcha-external-action"] = value;
            }
        }

        /// <summary>
        /// Validation Group
        /// </summary>
        public String ValidationGroup
        {
            get
            {
                return this.cvCaptcha.ValidationGroup;
            }
            set
            {
                this.cvCaptcha.ValidationGroup = value;
            }
        }

        /// <summary>
        /// Controla o número de tentativas que o usuário faz o post do conteúdo
        /// </summary>
        public Int32 QuantidadeTentativas
        {
            get
            {
                return GetQuantidadeTentativas(HttpContext.Current);
            }
            set
            {
                SetQuantidadeTentativas(value, HttpContext.Current);
            }
        }

        /// <summary>
        /// Retorna se o Recaptcha está válido
        /// </summary>
        public Boolean IsValid
        {
            get
            {
                return this.ValidateCaptcha();
            }
        }

        /// <summary>
        /// Class padrão para o container do captcha
        /// </summary>
        public String CssClass
        {
            get
            {
                return this.pnlGrcCaptcha.CssClass;
            }
            set
            {
                this.pnlGrcCaptcha.CssClass = value;
            }
        }

        /// <summary>
        /// Modo de visualização do validator
        /// </summary>
        public ValidatorDisplay Display
        {
            get
            {
                return this.cvCaptcha.Display;
            }
            set
            {
                this.cvCaptcha.Display = value;
            }
        }

        /// <summary>
        /// Define a mensagem de erro do validator
        /// </summary>
        public String ErrorMessage
        {
            get
            {
                return this.cvCaptcha.ErrorMessage;
            }
            set
            {
                this.cvCaptcha.ErrorMessage = value;
            }
        }

        /// <summary>
        /// Retorna se o componente está visível
        /// </summary>
        public Boolean IsVisible
        {
            get
            {
                if (this.ValidacaoTotalClientSide)
                {
                    return String.Compare(this.pnlGrcCaptcha.Style["display"], "block", true) == 0;
                }
                else
                {
                    return this.Visible;
                }
            }
        }

        /// <summary>
        /// Identifica se o captcha deve ser inicializado mesmo após um PostBack
        /// </summary>
        [DefaultValue(false)]
        public Boolean LoadOnPostBack
        {
            get
            {
                return ((Boolean?)ViewState["LoadOnPostBack"]).GetValueOrDefault(false);
            }
            set
            {
                ViewState["LoadOnPostBack"] = value;
            }
        }
        #endregion

        #region [ Métodos sobrescritos ou nativos ]

        /// <summary>
        /// Carregamento da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || this.LoadOnPostBack)
                this.Iniciar();

            if (this.Visible)
            {
                String scriptIncludeKey = "_RecaptchaScriptInclude";
                if (!Page.ClientScript.IsClientScriptIncludeRegistered(scriptIncludeKey))
                {
                    String url = String.Format(
                        "/_layouts/DadosCadastrais/SCRIPTS/google-recaptcha.min.js?versao={0}",
                        ConfigurationManager.AppSettings["Versao"]);
                    Page.ClientScript.RegisterClientScriptInclude(scriptIncludeKey, url);
                }
            }

            // persiste a private key para consultas futuras
            Session["CaptchaPrivateKey"] = this.grcCaptcha.PrivateKey;
        }

        /// <summary>
        /// Antes de renderizar o controle
        /// </summary>
        /// <param name="e">Argumentos passados pela plataforma</param>
        protected override void OnPreRender(EventArgs e)
        {
            this.pnlGrcCaptcha.CssClass = String.Format("{0} recaptcha-container", this.pnlGrcCaptcha.CssClass);
            base.OnPreRender(e);
        }

        #endregion

        #region [ Métodos de controle do Recaptcha ]

        /// <summary>
        /// Inicia o processo de configuração e validação do captcha ao carregar o formulário
        /// </summary>
        private void Iniciar()
        {
            // oculta o controller do captcha
            if (this.ValidacaoTotalClientSide)
            {
                if (CheckShowCaptcha(HttpContext.Current))
                    this.pnlGrcCaptcha.Style["display"] = "block";
            }
            else
            {
                this.Visible = CheckShowCaptcha(HttpContext.Current);
            }
        }

        /// <summary>
        /// Valida o captcha informado
        /// </summary>
        /// <returns>TRUE: captcha válido</returns>
        public Boolean ValidateCaptcha()
        {
            // valida o captcha somente se estiver visível
            if (!this.IsVisible)
                return true;

            // verifica se o número de tentativas requer validade do Captcha
            if (this.QuantidadeTentativas <= quantidadeMaximaTentativas)
                return true;

            if (this.grcCaptcha.Validate())
            {
                // valida o captcha junto à camada de serviços (WCF)
                String privateKey = this.grcCaptcha.PrivateKey;
                String captchaResponse = Request.Form["g-recaptcha-response"];
                return ValidateCaptchaService(privateKey, captchaResponse);
            }

            return false;
        }

        /// <summary>
        /// Valida o captcha pelo response informado
        /// </summary>
        /// <param name="context">Contexto do usuário</param>
        /// <param name="captchaResponse">Response do ReCaptcha fornecido pela Google</param>
        /// <returns>TRUE: captcha válido</returns>
        public static Boolean ValidateCaptcha(HttpContext context, String captchaResponse, Boolean usePageReferrer = false)
        {
            // verifica se o número de tentativas requer validade do Captcha
            if (GetQuantidadeTentativas(context, usePageReferrer) <= quantidadeMaximaTentativas)
                return true;

            // considera OK se o Response foi fornecido e está preenchido
            if (String.IsNullOrWhiteSpace(captchaResponse))
                return false;

            // valida o captcha junto à camada de serviços (WCF)
            String privateKey = Convert.ToString(context.Session["CaptchaPrivateKey"]);
            return ValidateCaptchaService(privateKey, captchaResponse);
        }

        /// <summary>
        /// Valida o response do captcha através do serviço WCF
        /// </summary>
        /// <param name="privateKey">Chave privada para consulta junto à API do Google</param>
        /// <param name="captchaResponse">Response fornecido pela API do Google em client-side</param>
        /// <returns>TRUE: captcha válido</returns>
        private static bool ValidateCaptchaService(String privateKey, String captchaResponse)
        {
            using (var contextoUsuario = new ContextoWCF<CaptchaServico.CaptchaServicoClient>())
                return contextoUsuario.Cliente.ValidarCaptcha(privateKey, captchaResponse);
        }

        /// <summary>
        /// Reinicia o captcha de validação do fomrulário
        /// </summary>
        public void ReiniciarCaptcha()
        {
            this.QuantidadeTentativas = 0;
            this.Iniciar();
        }

        /// <summary>
        /// Retorna a quantidade de tentativas persistida em sessão
        /// </summary>
        /// <param name="context">Contexto do usuário</param>
        /// <returns>Quantidade de tentativas</returns>
        public static Int32 GetQuantidadeTentativas(HttpContext context, Boolean usePageReferrer = false)
        {
            // valida o context informado
            if (context == null || context.Session == null)
                return 0;

            return ((Int32?)context.Session[GetSessionKey(context, usePageReferrer)]).GetValueOrDefault(0);
        }

        /// <summary>
        /// Incrementa o número de tentativas
        /// </summary>
        /// <param name="context">Contexto do usuário</param>
        public static void IncrementTentativas(HttpContext context, Boolean usePageReferrer = false)
        {
            Int32 numeroTentativas = GetQuantidadeTentativas(context, usePageReferrer) + 1;
            SetQuantidadeTentativas(numeroTentativas, context, usePageReferrer);
        }

        /// <summary>
        /// Incrementa o número de tentativas
        /// </summary>
        /// <param name="context">Contexto do usuário</param>
        public static void DecreaseTentativas(HttpContext context, Boolean usePageReferrer = false)
        {
            Int32 numeroTentativas = GetQuantidadeTentativas(context, usePageReferrer) - 1;
            SetQuantidadeTentativas(numeroTentativas, context, usePageReferrer);
        }

        /// <summary>
        /// Reinicia a contagem de tentativas
        /// </summary>
        /// <param name="context">Contexto do usuário</param>
        public static void RestartTentativas(HttpContext context, Boolean usePageReferrer = false)
        {
            SetQuantidadeTentativas(0, context, usePageReferrer);
        }

        /// <summary>
        /// Verifica se deve exibir o Captcha
        /// </summary>
        /// <param name="context">Contexto do usuário</param>
        /// <returns>TRUE: atingiu ao limite de tentativas e deve exibir o Recaptcha</returns>
        public static Boolean CheckShowCaptcha(HttpContext context, Boolean usePageReferrer = false)
        {
            return GetQuantidadeTentativas(context, usePageReferrer) >= quantidadeMaximaTentativas;
        }

        /// <summary>
        /// Método para atribuir a quantidade de tentativas
        /// </summary>
        /// <param name="quantidade">Quantidade definida</param>
        /// <param name="context">Contexto do usuário</param>
        private static void SetQuantidadeTentativas(Int32 quantidade, HttpContext context, Boolean usePageReferrer = false)
        {
            // valida o context informado
            if (context == null || context.Session == null)
                return;

            context.Session[GetSessionKey(context, usePageReferrer)] = quantidade;
        }

        /// <summary>
        /// Obtem a descrição da chave para criar a sessão
        /// </summary>
        /// <param name="context">Contexto do usuário</param>
        /// <returns>Key para acesso ao conteúdo persistido em sessão</returns>
        private static String GetSessionKey(HttpContext context, Boolean usePageReferrer = false)
        {
            // valida o context informado
            if (context == null || context.Request == null)
                return String.Empty;

            String pageName = Path.GetFileName(context.Request.Url.AbsolutePath);
            if (usePageReferrer)
            {
                // se estiver chamando o ashx direto pelo browser, nao tem urlreferrer
                if (context.Request.UrlReferrer == null || String.IsNullOrWhiteSpace(context.Request.UrlReferrer.AbsolutePath))
                    return String.Empty; 

                // obtém a página de onde veio a requisição
                pageName = Path.GetFileName(context.Request.UrlReferrer.AbsolutePath);
            }

            return string.Concat("CaptchaQuantidadeTentativa", pageName);
        }

        /// <summary>
        /// Obtem a descrição da chave para criar a sessão
        /// </summary>
        /// <returns>descrição da chave para criar a sessão</returns>
        private String GetSessionKey(Boolean usePageReferrer = false)
        {
            return GetSessionKey(HttpContext.Current);
        }

        #endregion
    }
}
