using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using SWUI = System.Web.UI.WebControls;

namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
    [DefaultEvent("ServerValidate")]
    [ToolboxData(@"<{0}:CustomRulesValidator runat=""server""></{0}:CustomRulesValidator>"),
    ParseChildren(ChildrenAsProperties = true, DefaultProperty = "Rules")]
    public class CustomRulesValidator : SWUI.CustomValidator
    {
        #region [ Propriedades públicas ]

        /// <summary>
        /// Regex default para validação do componente
        /// </summary>
        public String RegexPattern
        {
            get
            {
                return Convert.ToString(ViewState["RegexPattern"]);
            }
            set
            {
                ViewState["RegexPattern"] = value;
                this.Attributes["data-regex-pattern"] = value;
            }
        }

        /// <summary>
        /// Define a mensagem de erro para campo obrigatório
        /// </summary>
        public String ErrorMessageRequired
        {
            get
            {
                return Convert.ToString(this.ViewState["ErrorMessageRequired"]);
            }
            set
            {
                this.ViewState["ErrorMessageRequired"] = value;
                this.Attributes["data-error-message-required"] = value;
                
                // se houver mensagem configurada, marca o controle que de validar campo vazio
                if (!String.IsNullOrWhiteSpace(value))
                    ValidateEmptyText = true;
            }
        }

        /// <summary>
        /// Define a mensagem de erro default para valor inválido
        /// </summary>
        public String ErrorMessageInvalid
        {
            get
            {
                return Convert.ToString(this.ViewState["ErrorMessageInvalid"]);
            }
            set
            {
                this.ViewState["ErrorMessageInvalid"] = value;
                this.Attributes["data-error-message-invalid"] = value;
            }
        }

        /// <summary>
        /// Regras de validação do campo
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public List<CustomRulesValidatorRule> Rules
        {
            get
            {
                if (ViewState["Rules"] == null)
                    ViewState["Rules"] = new List<CustomRulesValidatorRule>();

                return (List<CustomRulesValidatorRule>)ViewState["Rules"];
            }
            set
            {
                ViewState["Rules"] = value;
            }
        }

        /// <summary>
        /// Sobrescrita da propriedade que determina a função JavaScript que será usada para validação do controle em client-side
        /// </summary>
        public String ClientValidationFunction
        {
            get
            {
                return base.ClientValidationFunction;
            }
            set
            {
                base.ClientValidationFunction = value;

                // se for passado algum valor, identifica se que está sendo utilizada validação em client-side
                if (!String.IsNullOrWhiteSpace(value))
                    this.EnableClientScript = true;
            }
        }

        /// <summary>
        /// Retorna o controle relacionado
        /// </summary>
        public SWUI.WebControl RelatedControl
        {
            get
            {
                if (String.IsNullOrEmpty(this.ControlToValidate))
                    return null;

                return (SWUI.WebControl)this.Parent.FindControl(this.ControlToValidate);
            }
        }

        /// <summary>
        /// Occurs when validation is performed on the server.
        /// </summary>
        public event CustomServerValidateEventHandler CustomServerValidate;

        /// <summary>
        /// Delegate customizado para receber o eventargs customizado
        /// </summary>
        /// <param name="source">controle</param>
        /// <param name="args">args customizado</param>
        public delegate void CustomServerValidateEventHandler(object source, CustomServerValidateEventArgs args);


        #endregion

        #region [ Métodos sobrescritos ]

        /// <summary>
        /// Inicialização do componente
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.ServerValidate += CustomServerValidateEvent;
        }

        /// <summary>
        /// Definições do componente antes da renderização
        /// </summary>
        /// <param name="e">Argumentos informados pela plataforma</param>
        protected override void OnPreRender(EventArgs e)
        {
            // remove a mensagem se o controle estiver válido
            if (this.IsValid)
                this.ErrorMessage = "";

            // inclui atributos ao controle relacionado
            if (!String.IsNullOrEmpty(this.ControlToValidate))
            {
                var control = this.RelatedControl;
                if (control != null)
                {
                    control.Attributes["data-error-message-required"] = this.ErrorMessageRequired;
                    control.Attributes["data-error-message-invalid"] = this.ErrorMessageInvalid;

                    if (!String.IsNullOrWhiteSpace(this.RegexPattern))
                        control.Attributes["data-regex-pattern"] = this.RegexPattern;

                    this.Attributes["data-error-for"] = control.ClientID;
                }
            }

            base.OnPreRender(e);
        }

        /// <summary>
        /// Renderização do controle
        /// </summary>
        /// <param name="writer">Handler para renderização do conteúdo na tela</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.Enabled && (!this.IsValid || this.Display == SWUI.ValidatorDisplay.Static))
            {
                this.Style["display"] = "block";
            }
            else
            {
                this.Style["display"] = "none";
            }

            base.RegisterValidatorDeclaration();
            this.AddAttributesToRender(writer);
            writer.AddAttribute("class", String.Format("input-error {0}", this.CssClass));
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write(this.ErrorMessage);
                writer.RenderEndTag();

                // renderiza o bloco de validações
                if (this.EnableClientScript && this.Rules.Count > 0)
                {
                    this.RenderRulesValidationContainer(writer);
                }
            }
            writer.RenderEndTag();
        }

        /// <summary>
        /// Valida se o campo está válido
        /// </summary>
        /// <returns></returns>
        protected override bool EvaluateIsValid()
        {
            this.IsValid = true;

            // se nenhum ControlToValidate foi especificado
            // repassa a responsabilidade da validação para o método custom implementado
            if (String.IsNullOrWhiteSpace(this.ControlToValidate))
                return this.OnServerValidate(String.Empty);

            String controlValidationValue = base.GetControlValidationValue(this.ControlToValidate);
            if (controlValidationValue == null)
                return true;

            if (String.IsNullOrWhiteSpace(controlValidationValue))
            {
                // valida segundo a regra de required
                if (!String.IsNullOrWhiteSpace(this.ErrorMessageRequired))
                {
                    this.ErrorMessage = this.ErrorMessageRequired;
                    return (this.IsValid = false);
                }
                else
                {
                    // ignora demais validações se o campo não estiver preenchido
                    return (this.IsValid = true);
                }
            }
            
            // valida lista de regex
            if (this.Rules.Count > 0)
            {
                foreach (var rule in this.Rules)
                {
                    // valida se o regex configurado corresponde ao conteúdo digitado
                    if (!String.IsNullOrEmpty(rule.RegexPattern))
                    {
                        Regex regex = new Regex(rule.RegexPattern);
                        if (!regex.IsMatch(controlValidationValue))
                        {
                            this.ErrorMessage = rule.ErrorMessage;
                            return (this.IsValid = false);
                        }
                    }

                    // validação por valor mínimo e máximo
                    if (rule.MinimumValue.HasValue || rule.MaximumValue.HasValue)
                    {
                        Double valorNumerico = 0;
                        controlValidationValue = Regex.Replace(controlValidationValue, @"[^0-9\,]", "");
                        if (Double.TryParse(controlValidationValue, out valorNumerico))
                        {
                            this.ErrorMessage = rule.ErrorMessage;
                            return
                                (!rule.MinimumValue.HasValue || valorNumerico >= rule.MinimumValue.Value) &&
                                (!rule.MaximumValue.HasValue || valorNumerico <= rule.MaximumValue.Value);
                        }
                    }
                }
            }
            // valida regex default
            else if (!String.IsNullOrEmpty(this.RegexPattern) && !String.IsNullOrEmpty(this.ErrorMessageInvalid))
            {
                Regex regex = new Regex(this.RegexPattern);
                if (!regex.IsMatch(controlValidationValue))
                {
                    this.ErrorMessage = this.ErrorMessageInvalid;
                    return (this.IsValid = false);
                }
            }

            // retorna validando algum método customizado em server side
            return this.OnServerValidate(controlValidationValue);
        }

        #endregion

        #region [ Métodos complementares ]

        /// <summary>
        /// Renderiza o container com as linhas de validação do campo de senha
        /// </summary>
        /// <param name="writer">Writer para renderização do conteúdo</param>
        public void RenderRulesValidationContainer(HtmlTextWriter writer)
        {
            var control = this.RelatedControl;

            writer.AddStyleAttribute("display", "none !important");
            writer.AddAttribute("data-validation-for", control != null ? control.ClientID : this.ControlToValidate);
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);
            {
                // percorre apenas os itens configurados para aparecer na tela
                foreach (var rule in this.Rules)
                {
                    if (!String.IsNullOrEmpty(rule.RegexPattern))
                        writer.AddAttribute("data-regex-validation", rule.RegexPattern);

                    if (rule.MinimumValue.HasValue)
                        writer.AddAttribute("data-min-value-validation", rule.MinimumValue.Value.ToString());

                    if (rule.MaximumValue.HasValue)
                        writer.AddAttribute("data-max-value-validation", rule.MaximumValue.Value.ToString());

                    writer.AddAttribute("data-error-message", rule.ErrorMessage);
                    writer.RenderBeginTag(HtmlTextWriterTag.Li);
                    {
                        writer.Write(rule.RegexPattern);
                    }
                    writer.RenderEndTag();
                }
            }
            writer.RenderEndTag();
        }

        /// <summary>
        /// Evento extendido para validação de forma customizada
        /// </summary>
        /// <param name="sender">Objeto que solicitou o comando</param>
        /// <param name="e">Objeto complementar para validação</param>
        protected void CustomServerValidateEvent(object sender, ServerValidateEventArgs e)
        {
            if (this.CustomServerValidate != null)
            {
                CustomServerValidateEventArgs args = new CustomServerValidateEventArgs(e.Value, e.IsValid);
                this.CustomServerValidate(sender, args);

                e.IsValid = args.IsValid;

                if (!e.IsValid)
                {
                    this.ErrorMessage = args.ErrorMessage;
                }
            }
        }

        #endregion
    }
}

