using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.Core.Web.Controles.Portal
{
    /// <summary>
    /// Disponibiliza o controle de Titulo.
    /// </summary>
    [ToolboxData("<{0}:AreaTexto runat=server></{0}:AreaTexto>")]
    public class AreaTexto : Panel
    {
        #region Propriedades
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(TipoTexto.Comum)]
        [Browsable(true)]
        public TipoTexto TipoTexto
        {
            get
            {
                if (this.ViewState["TipoTexto"] == null)
                    return TipoTexto.Comum;

                return (TipoTexto)Enum.Parse(typeof(TipoTexto), this.ViewState["TipoTexto"].ToString(), true);
            }
            set
            {
                this.ViewState["TipoTexto"] = value;
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
            // define a classe CSS padrão de acordo com o tpio de conteúdo esperado
            String tipoTextoClass = String.Empty;
            switch (this.TipoTexto)
            {
                case TipoTexto.Outro:
                    tipoTextoClass = "other-text";
                    break;
                case TipoTexto.Comum:
                default:
                    tipoTextoClass = "commom-text";
                    break;
            }

            // adiciona a classe customizada ao controle a ser renderizado
            this.CssClass = string.Concat(tipoTextoClass, " ", this.CssClass);

            // renderiza o componente
            base.Render(writer);
        }
        #endregion
    }
}
