using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
    /// <summary>
    /// Componente de e-mail
    /// </summary>
    [ToolboxData("<{0}:CampoEmail runat=\"server\"></{0}:CampoEmail>")]
    public class CampoEmail : CompositeControl
    {
        #region [ Propriedades privadas ]

        /// <summary>
        /// Campo de texto para o CPF/CNPJ a ser informado/persistido
        /// </summary>
        private TextBox txtField;

        /// <summary>
        /// Validator para o CPF/CNPJ informado
        /// </summary>
        private CampoEmailValidator fieldValidator;

        #endregion

        #region [ Propriedades públicas ]

        /// <summary>
        /// Label acompanha o campo de senha
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(""),
        Browsable(true)]
        public String Label
        {
            get
            {
                return Convert.ToString(ViewState["Label"]);
            }
            set
            {
                ViewState["Label"] = value;
            }
        }

        /// <summary>
        /// CSS Class para o label acompanha o campo de senha
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(""),
        Browsable(true)]
        public String CssClassLabel
        {
            get
            {
                return Convert.ToString(ViewState["CssClassLabel"]);
            }
            set
            {
                ViewState["CssClassLabel"] = value;
            }
        }

        /// <summary>
        /// CSS Class para o container que suporta o componente
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(""),
        Browsable(true)]
        public String CssClassContainer
        {
            get
            {
                return Convert.ToString(ViewState["CssClassContainer"]);
            }
            set
            {
                ViewState["CssClassContainer"] = value;
            }
        }

        /// <summary>
        /// Sobrescrita do atributo CssClass para definição do campo de texto
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(""),
        Browsable(true)]
        public override string CssClass
        {
            get
            {
                this.EnsureChildControls();
                return this.txtField.CssClass;
            }
            set
            {
                this.EnsureChildControls();
                this.txtField.CssClass = value;
            }
        }

        /// <summary>
        /// Sobrescrita do atributo CssClass para definição do campo de texto
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(""),
        Browsable(true)]
        public string PlaceHolder
        {
            get
            {
                this.EnsureChildControls();
                return this.txtField.Attributes["placeholder"];
            }
            set
            {
                this.EnsureChildControls();
                this.txtField.Attributes["placeholder"] = value;
            }
        }

        /// <summary>
        /// Define se o campo de obrigatório
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(true),
        Browsable(true)]
        public Boolean Required
        {
            get
            {
                return ((bool?)ViewState["Required"]).GetValueOrDefault(true);
            }
            set
            {
                ViewState["Required"] = value;
                this.ValidateEmptyText = value;
            }
        }

        /// <summary>
        /// Grupo de validação do formulário
        /// </summary>
        [Bindable(true),
        Category("Behavior"),
        DefaultValue(""),
        Browsable(true)]
        public String ValidationGroup
        {
            get
            {
                this.EnsureChildControls();
                return this.fieldValidator.ValidationGroup;
            }
            set
            {
                this.EnsureChildControls();
                this.fieldValidator.ValidationGroup = value;
            }
        }

        /// <summary>
        /// Exposisão do atributo do field validator para habilitação de validação em client-side
        /// </summary>
        [Bindable(true),
        Category("Behavior"),
        DefaultValue(""),
        Browsable(true)]
        public Boolean EnableClientScript
        {
            get
            {
                this.EnsureChildControls();
                return this.fieldValidator.EnableClientScript;
            }
            set
            {
                this.EnsureChildControls();
                this.fieldValidator.EnableClientScript = value;
            }
        }

        /// <summary>
        /// Exposisão do atributo do field validator configurando o controle a ser validado quando não houver preenchido conteúdo
        /// </summary>
        [Bindable(true),
        Category("Behavior"),
        DefaultValue(""),
        Browsable(true)]
        public Boolean ValidateEmptyText
        {
            get
            {
                this.EnsureChildControls();
                return this.fieldValidator.ValidateEmptyText;
            }
            set
            {
                this.EnsureChildControls();
                this.fieldValidator.ValidateEmptyText = value;
            }
        }

        /// <summary>
        /// Function JS para validação do componente em client-side
        /// </summary>
        [Bindable(true),
        Category("Behavior"),
        DefaultValue(""),
        Browsable(true)]
        public String ClientValidationFunction
        {
            get
            {
                this.EnsureChildControls();
                return this.fieldValidator.ClientValidationFunction;
            }
            set
            {
                this.EnsureChildControls();
                this.fieldValidator.ClientValidationFunction = value;

                if (!String.IsNullOrWhiteSpace(value))
                    this.fieldValidator.EnableClientScript = true;
            }
        }

        /// <summary>
        /// Modo de display do validator
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(""),
        Browsable(true)]
        public ValidatorDisplay ValidatorDisplay
        {
            get
            {
                this.EnsureChildControls();
                return this.fieldValidator.Display;
            }
            set
            {
                this.EnsureChildControls();
                this.fieldValidator.Display = value;
            }
        }

        /// <summary>
        /// ID do campo ao qual será comparado (no caso de confirmação de e-mail)
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(true),
        Browsable(true)]
        public String CompareTo
        {
            get
            {
                return Convert.ToString(ViewState["CompareTo"]);
            }
            set
            {
                ViewState["CompareTo"] = value;
            }
        }

        /// <summary>
        /// Validação do campo por limite máximo de caracteres
        /// </summary>
        [Bindable(true),
        Category("Behavior"),
        DefaultValue(""),
        Browsable(true)]
        public Int32? MaxLengthValidator
        {
            get
            {
                return (Int32?)ViewState["MaxLengthValidator"];
            }
            set
            {
                ViewState["MaxLengthValidator"] = value;

                this.EnsureChildControls();
                this.txtField.Attributes["data-maxlength-validator"] = value.HasValue ? value.Value.ToString() : "";
            }
        }

        /// <summary>
        /// Propriedade de TabIndex para o campo de texto
        /// </summary>
        [Bindable(true),
        Category("Behavior"),
        DefaultValue(0),
        Browsable(true)]
        public override short TabIndex
        {
            get
            {
                this.EnsureChildControls();

                short tabIndex = 0;
                short.TryParse(this.txtField.Attributes["tabindex"], out tabIndex);
                return tabIndex;
            }
            set
            {
                this.EnsureChildControls();
                this.txtField.Attributes["tabindex"] = value > 0 ? value.ToString() : "";
            }
        }

        /// <summary>
        /// E-mail completo inserido ao campo
        /// </summary>
        public String Value
        {
            get
            {
                return this.txtField.Text;
            }
            set
            {
                this.txtField.Text = value;
            }
        }

        /// <summary>
        /// Define a mensagem de erro para campo obrigatório
        /// </summary>
        [Bindable(true),
        Category("Behavior"),
        DefaultValue(""),
        Browsable(true)]
        public string ErrorMessageRequired
        {
            get
            {
                return Convert.ToString(this.ViewState["ErrorMessageRequired"]);
            }
            set
            {
                this.ViewState["ErrorMessageRequired"] = value;

                this.EnsureChildControls();
                this.txtField.Attributes["data-error-message-required"] = value;

                if (!String.IsNullOrWhiteSpace(value))
                    this.Required = true;
            }
        }

        /// <summary>
        /// Define a mensagem de erro para contéudo diferente do relacionado
        /// </summary>
        [Bindable(true),
        Category("Behavior"),
        DefaultValue(""),
        Browsable(true)]
        public string ErrorMessageCompareTo
        {
            get
            {
                return Convert.ToString(this.ViewState["ErrorMessageCompareTo"]);
            }
            set
            {
                this.ViewState["ErrorMessageCompareTo"] = value;

                this.EnsureChildControls();
                this.txtField.Attributes["data-error-message-compare-to"] = value;
            }
        }

        /// <summary>
        /// Define a mensagem de erro para ocorrência de domínio bloquado
        /// Se necessário, pode-se incluir "{0}" para explicitação do domónio bloqueado, ex: "domínio @{0} bloqueado."
        /// </summary>
        [Bindable(true),
        Category("Behavior"),
        DefaultValue(""),
        Browsable(true)]
        public string ErrorMessageBlockedDomain
        {
            get
            {
                return Convert.ToString(this.ViewState["ErrorMessageBlockedDomain"]);
            }
            set
            {
                this.ViewState["ErrorMessageBlockedDomain"] = value;

                this.EnsureChildControls();
                this.txtField.Attributes["data-error-message-blocked-domain"] = value;
            }
        }

        /// <summary>
        /// Define a mensagem de erro para campo inválido
        /// </summary>
        [Bindable(true),
        Category("Behavior"),
        DefaultValue(""),
        Browsable(true)]
        public string ErrorMessageInvalid
        {
            get
            {
                return Convert.ToString(this.ViewState["ErrorMessageInvalid"]);
            }
            set
            {
                this.ViewState["ErrorMessageInvalid"] = value;

                this.EnsureChildControls();
                this.txtField.Attributes["data-error-message-invalid"] = value;
            }
        }

        /// <summary>
        /// Mensagem de exceção para quantidade de caracteres inválida
        /// </summary>
        /// <see cref="CampoEmail.MaxLengthValidator"/>
        /// remarks: Se não informado, define 50 caracteres por default na propriedade MaxLengthValidator
        [Bindable(true),
        Category("Behavior"),
        DefaultValue(""),
        Browsable(true)]
        public String ErrorMessageMaxLength
        {
            get
            {
                return Convert.ToString(ViewState["ErrorMessageMaxLength"]);
            }
            set
            {
                ViewState["ErrorMessageMaxLength"] = value;

                this.EnsureChildControls();
                this.txtField.Attributes["data-error-message-maxlength"] = value;

                // define 50 caracteres por default
                if (!String.IsNullOrWhiteSpace(value) && MaxLengthValidator.GetValueOrDefault(0) <= 0)
                    MaxLengthValidator = 50;
            }
        }

        /// <summary>
        /// Occurs when validation is performed on the server.
        /// </summary>
        public event EmailServerValidateEventHandler ServerValidate;

        /// <summary>
        /// Delegate customizado para receber o eventargs customizado
        /// </summary>
        /// <param name="source">controle</param>
        /// <param name="args">args customizado</param>
        public delegate void EmailServerValidateEventHandler(object source, EmailServerValidateEventArgs args);

        #endregion

        #region [ Atributos somente-leitura ]

        /// <summary>
        /// Obtém o controle relacionado segundo o ID informado
        /// </summary>
        public CampoEmail RelatesToControl
        {
            get
            {
                if (!String.IsNullOrEmpty(this.CompareTo))
                {
                    object controlEmail = this.Parent.FindControl(this.CompareTo);
                    if (controlEmail is CampoEmail)
                    {
                        return (CampoEmail)controlEmail;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// SubDomínios do e-mail (o que sucede o '@').
        /// Exemplo: email@dominio.com.br retorna new String[] { "dominio", "com", "br" }
        /// </summary>
        public String[] SubDomains
        {
            get
            {
                Match match = CampoEmailValidator.RegexEmail.Match(this.Value);
                if (match.Success)
                {
                    return match.Groups["Domains"].Captures.Cast<Capture>().Select(capture => capture.Value).ToArray();
                }

                return null;
            }
        }

        /// <summary>
        /// Retorna o que sucede o '@'.
        /// Exemplo: email@dominio.com.br retorna "dominio.com.br".
        /// </summary>
        public String Domains
        {
            get
            {
                return String.Join(".", this.SubDomains);
            }
        }

        /// <summary>
        /// Conta do e-mail (precede o @).
        /// Exemplo: email@dominio.com.br retorna "email"
        /// </summary>
        public String Account
        {
            get
            {
                Match match = CampoEmailValidator.RegexEmail.Match(this.Value);
                if (match.Success)
                {
                    return match.Groups["Conta"].Value;
                }

                return null;
            }
        }

        /// <summary>
        /// Listagem de domínios bloqueados
        /// </summary>
        public List<String> BlockedDomains
        {
            get
            {
                if (ViewState["BlockedDomains"] == null)
                    ViewState["BlockedDomains"] = new List<String>();

                return (List<String>)ViewState["BlockedDomains"];
            }
        }

        /// <summary>
        /// Retorna se os dados informados estão válidos
        /// </summary>
        /// <exception cref="System.ArgumentException">A regular expression parsing error occurred</exception>
        /// <exception cref="System.ArgumentNullException">Input/pattern is null</exception>
        /// <exception cref="System.Text.RegularExpressions.RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section</exception>
        public Boolean IsValid
        {
            get
            {
                return this.fieldValidator.IsValid;
            }
        }

        #endregion

        #region [ Métodos sobrescritos ]

        /// <summary>
        /// Instancia os controles para renderização
        /// </summary>
        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            this.txtField = new TextBox()
            {
                ID = "txtField",
            };
            this.Controls.Add(this.txtField);

            this.fieldValidator = new CampoEmailValidator()
            {
                ControlToValidate = this.txtField.ID,
                CampoEmailControl = this,
                Display = ValidatorDisplay.Static
            };
            this.fieldValidator.ServerValidate += CustomServerValidate;
            this.Controls.Add(this.fieldValidator);
        }

        /// <summary>
        /// Evento de clique do botão selecionar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CustomServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (this.ServerValidate != null)
            {
                EmailServerValidateEventArgs args = new EmailServerValidateEventArgs(e.Value, e.IsValid);
                this.ServerValidate(sender, args);

                e.IsValid = args.IsValid;

                if (!e.IsValid)
                {
                    this.fieldValidator.ErrorMessage = args.ErrorMessage;
                }
            }
        }

        /// <summary>
        /// Definições do controle antes da renderização
        /// </summary>
        /// <param name="e">Argumentos passados pela plataforma</param>
        protected override void OnPreRender(EventArgs e)
        {
            // controle relacionado
            if (!String.IsNullOrWhiteSpace(this.CompareTo))
                this.txtField.Attributes["data-compare-to"] = this.RelatesToControl != null ? this.RelatesToControl.txtField.ClientID : this.CompareTo;

            // renderiza o campo de senha
            this.txtField.Attributes["data-regex-validation"] = CampoEmailValidator.RegexEmail.ToString();

            // fornece o ID do campo de texto ao validator
            this.fieldValidator.DataErrorFor = this.txtField.ClientID;

            base.OnPreRender(e);
        }

        /// <summary>
        /// Renderiza os controles na tela
        /// </summary>
        /// <param name="writer">Writer para renderizar os elementos na tela</param>
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (!String.IsNullOrWhiteSpace(this.CssClassContainer))
                writer.AddAttribute("class", this.CssClassContainer);

            this.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                // renderiza a label que acompanha o campo de seha
                if (!String.IsNullOrWhiteSpace(this.Label))
                {
                    if (!String.IsNullOrWhiteSpace(this.CssClassLabel))
                        writer.AddAttribute("class", this.CssClassLabel);

                    writer.RenderBeginTag(HtmlTextWriterTag.Label);
                    writer.Write(this.Label);
                    writer.RenderEndTag();
                }

                // renderiza o campo de senha
                this.txtField.RenderControl(writer);
            }
            writer.RenderEndTag();

            // renderiza o controle de validação
            this.fieldValidator.RenderControl(writer);
        }

        #endregion
    }
}
