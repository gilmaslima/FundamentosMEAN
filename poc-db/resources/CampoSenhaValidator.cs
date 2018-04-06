using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PNCadastrais.Core.Web.Controles.Portal
{
    [DefaultEvent("ServerValidate")]
    [ToolboxData(@"<{0}:CampoSenhaValidator runat=""server""></{0}:CampoSenhaValidator>")]
    public class CampoSenhaValidator : CustomValidator
    {
        #region [ Propriedades públicas ]

        /// <summary>
        /// Controle relacionado segundo o ID informado
        /// </summary>
        public CampoSenha SenhaControl
        {
            get
            {
                return (CampoSenha)ViewState["SenhaControl"];
            }
            set
            {
                ViewState["SenhaControl"] = value;
            }
        }

        /// <summary>
        /// ClientID do campo de senha relacionado
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
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
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
            // valida o preenchimento do campo
            if (this.SenhaControl.Required && String.IsNullOrWhiteSpace(this.SenhaControl.Value))
            {
                this.ErrorMessage = !String.IsNullOrWhiteSpace(this.SenhaControl.ErrorMessageRequired) ? this.SenhaControl.ErrorMessageRequired : "campo obrigatório";
                return false;
            }

            // Só deve validar as rules se o campo estiver preenchido
            if (!String.IsNullOrWhiteSpace(this.SenhaControl.Value))
            {
                // percorre as regras definidas
                foreach (var rule in this.SenhaControl.Rules)
                {
                    // valida se o regex configurado corresponde ao conteúdo digitado
                    Regex regex = new Regex(rule.RegexPattern);
                    if (!regex.IsMatch(this.SenhaControl.Value))
                    {
                        this.ErrorMessage = !String.IsNullOrWhiteSpace(rule.ErrorMessage) ? rule.ErrorMessage : "senha inválida";
                        return false;
                    }
                }
            }

            // compara com o campo relacionado
            if (this.SenhaControl.RelatesToControl != null &&
                String.Compare(this.SenhaControl.RelatesToControl.Value, this.SenhaControl.Value, false) != 0)
            {
                this.ErrorMessage = this.SenhaControl.ErrorMessageCompareTo;
                return (this.IsValid = false);
            }

            return this.OnServerValidate(this.SenhaControl.Value);
        }

        #endregion
    }
}
