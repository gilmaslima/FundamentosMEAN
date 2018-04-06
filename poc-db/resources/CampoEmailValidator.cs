using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
    [DefaultEvent("ServerValidate")]
    [ToolboxData(@"<{0}:CampoEmailValidator runat=""server""></{0}:CampoEmailValidator>")]
    public class CampoEmailValidator : CustomValidator
    {
        #region [ Propriedades públicas ]

        /// <summary>
        /// Controle relacionado segundo o ID informado
        /// </summary>
        public CampoEmail CampoEmailControl
        {
            get
            {
                return (CampoEmail)ViewState["SenhaControl"];
            }
            set
            {
                ViewState["SenhaControl"] = value;
            }
        }

        /// <summary>
        /// ClientID do campo de CPF/CNPJ relacionado
        /// </summary>
        public String DataErrorFor
        {
            get
            {
                return Convert.ToString(this.Attributes["data-error-for"]);
            }
            set
            {
                this.Attributes["data-error-for"] = value;
            }
        }

        /// <summary>
        /// Regular expression para E-mails
        /// </summary>
        /// <exception cref="System.ArgumentException">A regular expression parsing error occurred</exception>
        /// <exception cref="System.ArgumentNullException">Input/pattern is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Options contains an invalid flag</exception>
        public static Regex RegexEmail
        {
            get
            {
                return new Regex(@"^([a-zA-Z0-9=*!$&_.+-]+@[a-zA-Z0-9-]+)$|^[a-zA-Z0-9=*!$&_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]*[^\W]+$", RegexOptions.IgnoreCase);
            }
        }

        #endregion

        #region [ Métodos sobrescritos ]

        /// <summary>
        /// Definições do componente antes da renderização
        /// </summary>
        /// <param name="e">Argumentos informados pela plataforma</param>
        protected override void OnPreRender(EventArgs e)
        {
            // remove a mensagem se o controle estiver válido
            if (this.IsValid)
                this.ErrorMessage = "";

            base.OnPreRender(e);
        }

        /// <summary>
        /// Renderização do controle
        /// </summary>
        /// <param name="writer">Handler para renderização do conteúdo na tela</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.Enabled && (!this.IsValid || this.Display == ValidatorDisplay.Static))
            {
                this.Style["display"] = "block";
            }
            else
            {
                this.Style["display"] = "none";
            }

            base.RegisterValidatorDeclaration();
            this.AddAttributesToRender(writer);
            writer.AddAttribute("class", "input-error");
            writer.AddAttribute("data-error-for", this.DataErrorFor);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write(this.ErrorMessage);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        /// <summary>
        /// Valida se o campo está válido
        /// </summary>
        /// <returns></returns>
        protected override bool EvaluateIsValid()
        {
            // valida se o campo é requerido
            if (this.CampoEmailControl.Required && String.IsNullOrWhiteSpace(this.CampoEmailControl.Value))
            {
                this.ErrorMessage = this.CampoEmailControl.ErrorMessageRequired;
                return (this.IsValid = false);
            }

            // verifica domínios bloqueados
            String blockedDomain = String.Empty;
            if (!ValidateBlockedDomain(this.CampoEmailControl.Value, this.CampoEmailControl.BlockedDomains, out blockedDomain))
            {
                this.ErrorMessage = String.Format(this.CampoEmailControl.ErrorMessageBlockedDomain, blockedDomain);
                return (this.IsValid = false);
            }

            // valida o campo de e-mail - só deve validar o REGEX se o campo estiver preenchido
            if (!String.IsNullOrWhiteSpace(this.CampoEmailControl.Value) && !ValidateEmailByRegex(this.CampoEmailControl.Value, RegexEmail))
            {
                this.ErrorMessage = this.CampoEmailControl.ErrorMessageInvalid;
                return (this.IsValid = false);
            }

            // valida a quantidade máxima de caracteres
            if (this.CampoEmailControl.MaxLengthValidator.GetValueOrDefault(0) > 0
                && !String.IsNullOrWhiteSpace(this.CampoEmailControl.ErrorMessageMaxLength)
                && this.CampoEmailControl.Value.Length > this.CampoEmailControl.MaxLengthValidator.Value)
            {
                this.ErrorMessage = this.CampoEmailControl.ErrorMessageMaxLength;
                return (this.IsValid = false);
            }

            // compara com o campo relacionado
            if (this.CampoEmailControl.RelatesToControl != null &&
                String.Compare(this.CampoEmailControl.RelatesToControl.Value, this.CampoEmailControl.Value, false) != 0)
            {
                this.ErrorMessage = this.CampoEmailControl.ErrorMessageCompareTo;
                return (this.IsValid = false);
            }

            // retorna validando algum método customizado em server side
            return this.OnServerValidate(this.CampoEmailControl.Value);
        }

        #endregion

        #region [ Métodos auxiliares ]

        /// <summary>
        /// Valida o e-mail segundo o regex passado
        /// </summary>
        /// <param name="emailToValidate">E-mail a ser validado</param>
        /// <param name="regexEmail">Regex de validação</param>
        /// <returns>TRUE: e-mail válido</returns>
        public static Boolean ValidateEmailByRegex(String emailToValidate, Regex regexEmail)
        {
            return regexEmail.Match(emailToValidate).Success;
        }

        /// <summary>
        /// Valida o e-mail segundo o regex passado
        /// </summary>
        /// <param name="emailToValidate">E-mail a ser validado</param>
        /// <param name="regexPattern">Regex de validação</param>
        /// <returns>TRUE: e-mail válido</returns>
        public static Boolean ValidateEmailByRegex(String emailToValidate, String regexPattern)
        {
            Regex regexEmail = new Regex(regexPattern);
            return regexEmail.Match(emailToValidate).Success;
        }

        /// <summary>
        /// Valida se o e-mail pertence a uma lista de domínios bloqueados
        /// </summary>
        /// <param name="emailToValidate">E-mail para validação</param>
        /// <param name="blockedDomainList">Lista com os domínios bloqueados para validação</param>
        /// <param name="blockedDomain">RETORNO: domínio bloqueado</param>
        /// <returns></returns>
        public static Boolean ValidateBlockedDomain(String emailToValidate, List<String> blockedDomainList, out String blockedDomain)
        {
            blockedDomain = String.Empty;

            // valida os parâmetros de entrada
            if (blockedDomainList == null || blockedDomainList.Count == 0 || String.IsNullOrWhiteSpace(emailToValidate))
                return true;

            // filtra a lista de domínios bloqueados
            var blockedDomains = blockedDomainList.Where(x =>
                emailToValidate.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (blockedDomains.Count > 0)
            {
                blockedDomain = blockedDomains[0];
                return false;
            }

            return true;
        }

        #endregion
    }
}
