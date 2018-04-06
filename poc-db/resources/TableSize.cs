using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.Outro.Core.Web.Controles.Portal
{
    /// <summary>
    /// Controle limitador de registros em tabela/grid
    /// </summary>
    [ToolboxData("<{0}:TableSize runat=server></{0}:TableSize>")]
    public class TableSize : DropDownList
    {
        #region [ Propriedades Públicas ]

        /// <summary>
        /// Label primário que acompaha o seletor
        /// </summary>
        [Bindable(true), Category("Appearance"), Browsable(true), DefaultValue("visualizar:")]
        public String LabelPrimario
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Convert.ToString(ViewState["LabelPrimario"])))
                    ViewState["LabelPrimario"] = "visualizar:";

                return Convert.ToString(ViewState["LabelPrimario"]);
            }
            set
            {
                ViewState["LabelPrimario"] = value;
            }
        }

        /// <summary>
        /// Label secundário que acompanha o seletor
        /// </summary>
        [Bindable(true), Category("Appearance"), Browsable(true)]
        public String LabelSecundario
        {
            get
            {
                return Convert.ToString(ViewState["LabelSecundario"]);
            }
            set
            {
                ViewState["LabelSecundario"] = value;
            }
        }

        /// <summary>
        /// Define o alinhamento horizontal do controle
        /// </summary>
        [Bindable(true), Category("Appearance"), Browsable(true), DefaultValue(TableSizeHorizontalAlign.Right)]
        public TableSizeHorizontalAlign? AlinhamentoHorizontal
        {
            get
            {
                // se nenhum alinhamento foi definido, retorna "Right" por default
                if (ViewState["HorizontalAlign"] == null)
                    return TableSizeHorizontalAlign.Right;

                return (TableSizeHorizontalAlign)ViewState["HorizontalAlign"];
            }
            set
            {
                ViewState["HorizontalAlign"] = value;
            }
        }

        /// <summary>
        /// Evento de client side selecionar outro valor no dropdownlist
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public String OnClientValueChanged
        {
            get
            {
                return Convert.ToString(this.Attributes["onchange"]);
            }
            set
            {
                this.Attributes["onchange"] = value;
            }
        }

        /// <summary>
        /// Valor definido no combo do TableSize
        /// </summary>
        public Int32 SelectedSize
        {
            get
            {
                Int32 retorno = 0;
                Int32.TryParse(this.SelectedValue, out retorno);
                return retorno;
            }
        }

        #endregion

        #region [ Métodos Sobrescritos ]

        /// <summary>
        /// Evento OnInit do controle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // adiciona a listagem de itens default
            if (this.Items.Count == 0)
            {
                this.Items.AddRange(new ListItem[] {
                        new ListItem { Value = "10", Text = "10" },
                        new ListItem { Value = "20", Text = "20" },
                        new ListItem { Value = "30", Text = "30", Selected = true },
                        new ListItem { Value = "50", Text = "50" },
                        new ListItem { Value = "100", Text = "100" }
                    });
            }
        }

        /// <summary>
        /// Evento OnLoad do controle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // define propriedade AutoPostBack mediante existência de evento OnValueChanged
            if (this.ValueChanged != null)
                this.AutoPostBack = true;

            // evento de change
            this.SelectedIndexChanged += ddlTableSize_SelectedIndexChanged;

            base.OnLoad(e);
        }

        /// <summary>
        /// Renderiza o controle
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            // adiciona classes default ao controle principal
            string cssClass = "view-control";

            // adiciona class de alinhamento do controle
            switch (this.AlinhamentoHorizontal)
            {
                case TableSizeHorizontalAlign.Left:
                    cssClass = String.Format("{0} left", cssClass);
                    break;
                case TableSizeHorizontalAlign.Center:
                    cssClass = String.Format("{0} center", cssClass);
                    break;
                case TableSizeHorizontalAlign.Right:
                default:
                    cssClass = String.Format("{0} right", cssClass);
                    break;
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                // label primário que acompanha o controle
                if (!String.IsNullOrWhiteSpace(this.LabelPrimario))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.WriteLine(this.LabelPrimario);
                    writer.RenderEndTag();
                }

                base.CssClass = String.Format("view-control-select rede-select {0}", base.CssClass);
                base.Attributes["data-show-first-line"] = "true";
                this.AddAttributesToRender(writer);
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                {
                    base.Render(writer);
                }
                writer.RenderEndTag();

                // label secundário que acompanha o controle
                if (!String.IsNullOrWhiteSpace(this.LabelSecundario))
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.WriteLine(this.LabelSecundario);
                    writer.RenderEndTag();
                }
            }
            writer.RenderEndTag();
        }

        #endregion

        #region [ Eventos do controle ]

        /// <summary>
        /// Deleage para o evento a ser disparado ao selecionar um valor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        public delegate void ValueChangedEventHandler(Object sender, Int32 value);

        /// <summary>
        /// Exposição do evento a ser disparado ao selecionar um valor
        /// </summary>
        public event ValueChangedEventHandler ValueChanged;

        /// <summary>
        /// Evento disparado ao mudar o valor selecionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlTableSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ValueChanged != null)
                ValueChanged(this, this.SelectedSize);
        }

        #endregion
    }
}
