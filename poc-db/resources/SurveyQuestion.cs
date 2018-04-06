using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.PesquisaSatisfacao.Sharepoint.Helper
{
    [ToolboxData("<{0}:SurveyQuestion runat=server></{0}:SurveyQuestion>")]
    public class SurveyQuestion : RadioButtonList
    {
        /// <summary>
        /// Descrição da questão a ser apresentada
        /// </summary>
        public String Question
        {
            get
            {
                return Convert.ToString(this.ViewState["Question"]);
            }
            set
            {
                this.ViewState["Question"] = value;
            }
        }

        /// <summary>
        /// Valida se deve apresentar o validador
        /// </summary>
        public String ValidationMessage
        {
            get
            {
                return Convert.ToString(this.ViewState["ValidatorMessage"]);
            }
            set
            {
                this.ViewState["ValidatorMessage"] = value;
            }
        }

        /// <summary>
        /// Grupo de validação do componente
        /// </summary>
        public String ValidationGroup
        {
            get
            {
                return Convert.ToString(this.ViewState["ValidationGroup"]);
            }
            set
            {
                this.ViewState["ValidationGroup"] = value;
            }
        }

        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            if (!string.IsNullOrWhiteSpace(this.ValidationMessage))
            {
                RequiredFieldValidator validator = new RequiredFieldValidator()
                {
                    ControlToValidate = this.ID,
                    Display = ValidatorDisplay.Dynamic,
                    EnableClientScript = true,
                    ValidationGroup = this.ValidationGroup,
                    ErrorMessage = this.ValidationMessage,
                    CssClass = "pesquisa-error-message"
                };
                this.Controls.Add(validator);
            }

            foreach (ListItem item in this.Items)
            {
                if (item == null)
                    continue;

                RadioButton rb = new RadioButton();
                rb.Attributes["value"] = item.Value;
                rb.Text = item.Text;

                this.Controls.Add(rb);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            this.CssClass = string.Format("{0} rede-radio-group", this.CssClass);
            this.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        writer.Write(this.Question);
                        if (!string.IsNullOrWhiteSpace(this.ValidationMessage))
                        {
                            var validator = this.Controls.OfType<RequiredFieldValidator>().ToArray();
                            if (validator != null && validator.Length > 0)
                            {
                                validator[0].RenderControl(writer);
                            }
                        }
                        writer.RenderEndTag();

                        foreach (var item in this.Controls.OfType<RadioButton>())
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Td);
                            item.RenderControl(writer);
                            writer.RenderEndTag();
                        }
                    }
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }
    }
}
