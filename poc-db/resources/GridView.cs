using System;
using System.ComponentModel;
using System.Web.UI;
using WebControls = System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.Core.Web.Controles.Portal
{
    /// <summary>
    /// Disponibiliza o controle de GridView.
    /// </summary>
    [ToolboxData("<{0}:GridView runat=server></{0}:GridView>")]
    public class GridView : WebControls.GridView
    {
        #region Propriedades
        /// <summary>
        /// Define o título a ser exibido.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(default(String))]
        [Browsable(true)]
        public String TituloTabela
        {
            get
            {
                if (this.ViewState["TituloTabela"] == null)
                    return String.Empty;

                return this.ViewState["TituloTabela"].ToString();
            }
            set
            {
                this.ViewState["TituloTabela"] = value;
            }
        }

        /// <summary>
        /// Define classes para a div do titulo
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(default(String))]
        [Browsable(true)]
        public String TituloClass
        {
            get
            {
                if (this.ViewState["TituloClass"] == null)
                    return String.Empty;

                return this.ViewState["TituloClass"].ToString();
            }
            set
            {
                this.ViewState["TituloClass"] = value;
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
            if (!String.IsNullOrWhiteSpace(this.TituloTabela) && base.Rows.Count > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class,String.Format( "table-title {0}", this.TituloClass));
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(this.TituloTabela);
                writer.RenderEndTag();
            }

            base.CssClass = String.Format("{0} rede-table", base.CssClass);
            base.Render(writer);
        }
        #endregion
    }
}
