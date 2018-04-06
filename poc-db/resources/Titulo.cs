using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.Core.Web.Controles.Portal
{
	/// <summary>
	/// Disponibiliza o controle de Titulo.
	/// </summary>
	[ToolboxData("<{0}:Titulo runat=server></{0}:Titulo>")]
    public class Titulo : Panel
    {
        #region Propriedades
        /// <summary>
        /// Propriedade oculta, pois não deverá ser utilizada.
        /// </summary>
        [DefaultValue("")]
        [Localizable(true)]
        private new string GroupingText { get; set; }

        /// <summary>
        /// Define o título a ser exibido.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("Descricao")]
        [Browsable(true)]
        public string Descricao
        {
            get
            {
                if (this.ViewState["Descricao"] == null)
                    return String.Empty;

                return this.ViewState["Descricao"].ToString();
            }
            set
            {
                this.ViewState["Descricao"] = value;
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
            //Adicionando CssClass padrão
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "title");

            this.RenderBeginTag(writer);

            writer.Write(this.Descricao);

            // Fechando os controles
            this.RenderEndTag(writer);
        } 
        #endregion
    }
}
