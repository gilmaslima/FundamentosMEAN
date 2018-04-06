using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.Core.Web.Controles.Portal
{
    /// <summary>
    /// Disponibiliza o controle de Titulo.
    /// </summary>
    [ToolboxData("<{0}:Subtitulo runat=server></{0}:Subtitulo>")]
    public class Subtitulo : Panel
    {
        #region Propriedades

        /// <summary>
        /// Define o subtítulo a ser exibido.
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
            String classes = String.Format("subtitle {0}", this.CssClass);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, classes);

            this.RenderBeginTag(writer);

            writer.Write(this.Descricao);

            // Fechando os controles
            this.RenderEndTag(writer);
        }
        #endregion
    }
}
