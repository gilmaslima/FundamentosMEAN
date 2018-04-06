using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
	/// <summary>
	/// Disponibiliza o controle de download Txt.
	/// </summary>
	internal class MenuAcaoDownloadTxt : WebControl, INamingContainer, IPostBackEventHandler
    {
        public MenuAcaoDownloadTxt(EventHandler click, String clientClick)
            : base(HtmlTextWriterTag.A)
        {
            this.ID = "btnDownloadTxt";
            this.Click = click;
            this.ClientClick = clientClick;
        }

        private String ClientClick
        {
            get
            {
                if (this.ViewState["ClientClick"] == null)
                    return String.Empty;

                return this.ViewState["ClientClick"].ToString();
            }
            set
            {
                this.ViewState["ClientClick"] = value;
            }
        }
        public event EventHandler Click;

        /// <summary>
        /// Evento de clique
        /// </summary>
        /// <param name="e">Argumentos</param>
        protected void OnClick(EventArgs e)
        {
            EventHandler eventHandler = this.Click;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        /// <summary>
        /// Evento de postback
        /// </summary>
        /// <param name="eventArgument">The argument for the event.</param>
        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            this.OnClick(EventArgs.Empty);
        }

        /// <summary>
        /// Rederiza o controle.
        /// </summary>
        /// <param name="writer">O objeto que receberá o conteúdo do controle.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            String postBackScript = String.Empty;
            if (this.Click != null)
                postBackScript = this.Page.ClientScript.GetPostBackEventReference(this, String.Empty, false);

            String urlImagem = "/sites/fechado/Style%20Library/pt-br/Redecard/Img/redeareafechada-facelift/txt-file.png";
            String texto = "download em TXT";
            MenuAcoes.RenderControleAcao(this, writer, urlImagem, texto, postBackScript, this.ClientClick);
        }
    }
}
