using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rede.PN.MultivanAlelo.Core.Web.Controles.Portal
{
    [DefaultEvent("ServerValidate")]
    [ToolboxData(@"<{0}:CampoCpfCnpjValidator runat=""server""></{0}:CampoCpfCnpjValidator>")]
    public class CampoCpfCnpjValidator : CustomValidator
    {
        #region [ Propriedades públicas ]

        /// <summary>
        /// Controle relacionado segundo o ID informado
        /// </summary>
        public CampoCpfCnpj CpfCnpjControl
        {
            get
            {
                return (CampoCpfCnpj)ViewState["SenhaControl"];
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
            String errorMessage = "";
            this.IsValid = ValidateField(
                this.CpfCnpjControl, 
                out errorMessage, 
                errorMessageRequired: this.CpfCnpjControl.ErrorMessageRequired,
                errorMessageInvalid: this.CpfCnpjControl.ErrorMessageInvalid);

            if (!this.IsValid)
            {
                this.ErrorMessage = errorMessage;
                return false;
            }

            return this.OnServerValidate(this.CpfCnpjControl.Value);            
        }

        #endregion

        #region [ Métodos auxiliares ]

        /// <summary>
        /// Valida o CPF/CNPJ informado
        /// </summary>
        /// <param name="control">Controle sobre o qual será aplicada a validação</param>
        /// <param name="errorMessage">Mensagem de erro de saída</param>
        /// <returns></returns>
        public static Boolean ValidateField(
            CampoCpfCnpj control,
            out String errorMessage)
        {
            return ValidateField(control, out errorMessage, errorMessageRequired: "", errorMessageInvalid: "");
        }

        /// <summary>
        /// Valida o CPF/CNPJ informado
        /// </summary>
        /// <param name="control">Controle sobre o qual será aplicada a validação</param>
        /// <param name="errorMessage">Mensagem de erro de saída</param>
        /// <param name="errorMessageRequired">Mensagem de erro customizada para o caso de o campo ser obrigatório e não possuir valor</param>
        /// <param name="errorMessageInvalid">Mensage de erro customizada para o caso do conteúdo do campo ser inválido</param>
        /// <returns></returns>
        public static Boolean ValidateField(
            CampoCpfCnpj control, 
            out String errorMessage, 
            String errorMessageRequired = "", 
            String errorMessageInvalid = "")
        {
            errorMessage = String.Empty; 

            // valida o preenchimento do campo
            if (control.Required && String.IsNullOrEmpty(control.Value))
            {
                errorMessage = Regex.Replace(errorMessageRequired, "{type}", control.IsCpf ? "CPF" : "CNPJ", RegexOptions.IgnoreCase);
                return false;
            }

            // valida campo segundo o seu tipo
            Boolean valid = true;
            switch (control.Type)
            {
                case CampoCpfCnpjType.Cnpj:
                    valid = ValidateCnpj(control.Value);
                    break;
                case CampoCpfCnpjType.Cpf:
                    valid = ValidateCpf(control.Value);
                    break;
                case CampoCpfCnpjType.Both:
                default:
                    valid = ValidateCnpj(control.Value) || ValidateCpf(control.Value);
                    break;
            }

            if (!valid)
            {
                errorMessage = Regex.Replace(errorMessageInvalid, "{type}", control.IsCpf ? "CPF" : "CNPJ", RegexOptions.IgnoreCase);
            }

            return valid;
        }
        
        /// <summary>
        /// Método para validar CPF
        /// </summary>
        /// <param name="cpf">CPF a ser validado</param>
        /// <returns>TRUE: CPF válido</returns>
        public static Boolean ValidateCpf(String cpf)
        {
            Int32[] multiplicador1 = new Int32[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            Int32[] multiplicador2 = new Int32[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            String tempCpf = default(String);
            String digito = default(String);
            Int32 soma = default(Int32);
            Int32 resto = default(Int32);

            cpf = Regex.Replace(cpf, "[^0-9]", "");

            // valida a quantidade de caracteres
            if (cpf.Length > 11)
                return false;

            // completa com zeros à esquerda
            cpf = cpf.PadLeft(11, '0');

            // obtém o primeiro dígito verificador
            tempCpf = cpf.Substring(0, 9);
            soma = 0;
            for (Int32 i = 0; i < 9; i++)
                soma += (Int32)Char.GetNumericValue(tempCpf[i]) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCpf = String.Concat(tempCpf, digito);

            // obtém o segundo dígito verificador
            soma = 0;
            for (Int32 i = 0; i < 10; i++)
                soma += (Int32)Char.GetNumericValue(tempCpf[i]) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = String.Concat(digito, resto.ToString());

            return cpf.EndsWith(digito);
        }

        /// <summary>
        /// Método para validar CNPJ
        /// </summary>
        /// <param name="cnpj">CNPJ a ser validado</param>
        /// <returns>TRUE: CNPJ válido</returns>
        public static Boolean ValidateCnpj(String cnpj)
        {
            Int32[] multiplicador1 = new Int32[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            Int32[] multiplicador2 = new Int32[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            Int32 soma = default(Int32);
            Int32 resto = default(Int32);
            String digito = default(String);
            String tempCnpj = default(String);

            cnpj = Regex.Replace(cnpj, "[^0-9]", "");

            // valida a quantidade de caracteres
            if (cnpj.Length > 14)
                return false;

            // completa com zeros à esquerda
            cnpj = cnpj.PadLeft(14, '0');

            // obtém o primeiro dígito verificador
            tempCnpj = cnpj.Substring(0, 12);
            for (Int32 i = 0; i < 12; i++)
                soma += (Int32)Char.GetNumericValue(tempCnpj[i]) * multiplicador1[i];

            resto = (soma % 11);

            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCnpj = String.Concat(tempCnpj, digito);

            // obtém o segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += (Int32)Char.GetNumericValue(tempCnpj[i]) * multiplicador2[i];

            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = String.Concat(digito, resto.ToString());

            return cnpj.EndsWith(digito);
        }

        #endregion
    }
}
