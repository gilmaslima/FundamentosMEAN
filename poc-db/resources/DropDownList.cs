using System;
using System.ComponentModel;
using System.Web.UI;
using WebControls = System.Web.UI.WebControls;

namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
    /// <summary>
    /// Disponibiliza o controle de Botao.
    /// </summary>
    [ToolboxData("<{0}:DropDownList runat=server></{0}:DropDownList>")]
    public class DropDownList : WebControls.DropDownList
    {
        #region Propriedades

        /// <summary>
        /// Label que acompanha o componente
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(default(String))]
        [Browsable(true)]
        public String Label
        {
            get
            {
                return Convert.ToString(this.ViewState["Label"]);
            }
            set
            {
                this.ViewState["Label"] = value;
            }
        }

        /// <summary>
        /// Class customizado para o label que acompanha o componente
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(default(String))]
        [Browsable(true)]
        public String LabelClass
        {
            get
            {
                return Convert.ToString(this.ViewState["LabelClass"]);
            }
            set
            {
                this.ViewState["LabelClass"] = value;
            }
        }

        /// <summary>
        /// Definição da primeira linha do DropDownList
        /// - TRUE: considera o primeiro item
        /// - FALSE: trata o primeiro item como label
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Browsable(true)]
        public Boolean ShowFirstLine
        {
            get
            {
                return ((Boolean?)this.ViewState["ShowFirstLine"]).GetValueOrDefault(false);
            }
            set
            {
                this.ViewState["ShowFirstLine"] = value;
            }
        }

        /// <summary>
        /// Define o conteúdo CSS da div.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(default(String))]
        [Browsable(true)]
        public String DivCss
        {
            get
            {
                if (this.ViewState["DivCss"] == null)
                    return String.Empty;

                return this.ViewState["DivCss"].ToString();
            }
            set
            {
                this.ViewState["DivCss"] = value;
            }
        }
        #endregion

        #region [ Métodos Sobrescritos / Implementações Interfaces ]

        /// <summary>
        /// Rederiza o controle.
        /// </summary>
        /// <param name="writer">O objeto que receberá o conteúdo do controle.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, this.DivCss);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            base.CssClass = String.Format("{0} rede-select", base.CssClass);

            // adiciona o label ao componente
            if (!string.IsNullOrEmpty(this.Label))
            {
                if (!string.IsNullOrWhiteSpace(this.LabelClass))
                    this.LabelClass = string.Concat(" ", this.LabelClass.Trim());

                writer.AddAttribute(HtmlTextWriterAttribute.Class, string.Concat("label-rede", this.LabelClass));
                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                writer.Write(this.Label);
                writer.RenderEndTag();
            }

            // verifica se a primeira linha deve ser exibida
            if (this.ShowFirstLine)
            {
                base.Attributes["data-show-first-line"] = "true";
            }

            base.Render(writer);

            writer.RenderEndTag();
        }
        #endregion
    }
}
