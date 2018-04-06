using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rede.PN.CondicaoComercial.Core.Web.Controles.Portal
{
    /// <summary>
    /// Disponibiliza o controle de Titulo.
    /// </summary>
    [ToolboxData("<{0}:BotaoInformacao runat=server></{0}:BotaoInformacao>")]
    public class BotaoInformacao : WebControl
    {
        #region Propriedades

        /// <summary>
        /// Define o título a ser exibido.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("Titulo")]
        [Browsable(true)]
        public string Titulo
        {
            get
            {
                if (this.ViewState["Titulo"] == null)
                    return String.Empty;

                return this.ViewState["Titulo"].ToString();
            }
            set
            {
                this.ViewState["Titulo"] = value;
            }
        }

        /// <summary>
        /// Define a Mensagem a ser exibido.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("Mensagem")]
        [Browsable(true)]
        public string Mensagem
        {
            get
            {
                if (this.ViewState["Mensagem"] == null)
                    return String.Empty;

                return this.ViewState["Mensagem"].ToString();
            }
            set
            {
                this.ViewState["Mensagem"] = value;
            }
        }


        /// <summary>
        /// Define a posição do popover a ser exibido próximo ao controle
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("Right")]
        [Browsable(true)]
        public BotaoInformacaoPosition Posicionamento
        {
            get
            {
                if (this.ViewState["PopoverPosition"] == null)
                    return BotaoInformacaoPosition.Right;

                return (BotaoInformacaoPosition)this.ViewState["PopoverPosition"];
            }
            set
            {
                this.ViewState["PopoverPosition"] = value;
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
            //Renderiza div wrapper do controle
            this.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:void(0);");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "info-button");
            writer.AddAttribute("data-toggle", "popover");
            writer.AddAttribute("title", this.Titulo);
            writer.AddAttribute("data-content", this.Mensagem);

            // define o posicionamento do elemento
            switch (this.Posicionamento)
            {
                case BotaoInformacaoPosition.Left:
                    writer.AddAttribute("data-placement", "left");
                    break;
                case BotaoInformacaoPosition.Top:
                    writer.AddAttribute("data-placement", "top");
                    break;
                case BotaoInformacaoPosition.Bottom:
                    writer.AddAttribute("data-placement", "bottom");
                    break;
                default:
                    writer.AddAttribute("data-placement", "right");
                    break;
            }

            writer.RenderBeginTag(HtmlTextWriterTag.A);
            // Fechando os controles
            this.RenderEndTag(writer);
        }
        #endregion
    }
}
