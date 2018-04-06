using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace _safeprojectname_.Core.Web.Controles.Portal
{
    /// <summary>
    /// Disponibiliza o controle de Botao.
    /// </summary>
    [ToolboxData("<{0}:Botao runat=server></{0}:Botao>")]
    public class Botao : Button
    {
        #region Propriedades
        /// <summary>
        /// Define o título a ser exibido.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("False")]
        [Browsable(true)]
        public Boolean BotaoPrimario
        {
            get
            {
                if (this.ViewState["BotaoPrimario"] == null)
                    return false;

                return Convert.ToBoolean(this.ViewState["BotaoPrimario"]);
            }
            set
            {
                this.ViewState["BotaoPrimario"] = value;
            }
        }

        /// <summary>
        /// Caso precise adicionar uma classe na div do botao
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(default(String))]
        [Browsable(true)]
        public String ClasseAdicional
        {
            get
            {
                if (this.ViewState["ClasseAdicional"] == null)
                    return String.Empty;

                return this.ViewState["ClasseAdicional"].ToString();
            }
            set
            {
                this.ViewState["ClasseAdicional"] = value;
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
            String classeDiv = String.Format("btn-rede {0}", this.ClasseAdicional);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, classeDiv);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            //<div class="btn-rede">

            String classeBotao = this.BotaoPrimario ? "btn-primary" : "btn-secondary";
            if (!this.Enabled)
                classeBotao += " portal-disabled";

            writer.AddAttribute(HtmlTextWriterAttribute.Class, classeBotao);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            //<div class="btn-primary|btn-secondary">

            base.Render(writer);
            //<input type="button" value="">

            writer.RenderEndTag();
            writer.RenderEndTag();
        }
        #endregion
    }
}