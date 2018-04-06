using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
	/// <summary>
	/// Disponibiliza o controle de Botao.
	/// </summary>
	[ToolboxData("<{0}:MenuAcoes runat=server></{0}:MenuAcoes>")]
    public class MenuAcoes : WebControl, INamingContainer
    {
        public MenuAcoes()
            : base(HtmlTextWriterTag.Div)
        {
        }

        #region Imprimir
        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(false),
        Description("Define se deve exibir o controle Imprimir."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public Boolean BotaoImprimir
        {
            get
            {
                if (this.ViewState["BotaoImprimir"] == null)
                    return false;

                return Convert.ToBoolean(this.ViewState["BotaoImprimir"]);
            }
            set
            {
                this.ViewState["BotaoImprimir"] = value;
            }
        }

        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(default(String)),
        Description("Define o evento client click do controle Imprimir"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public String ClientClickImprimir
        {
            get
            {
                if (this.ViewState["ClientClickImprimir"] == null)
                    return String.Empty;

                return this.ViewState["ClientClickImprimir"].ToString();
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    this.BotaoImprimir = true;

                this.ViewState["ClientClickImprimir"] = value;
            }
        }

        [
        Bindable(true),
        Description("Evento customizado para Imprimir."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public event EventHandler ClickImprimir;
        #endregion

        #region Excel
        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(false),
        Description("Define se deve exibir o controle Excel."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public Boolean BotaoExcel
        {
            get
            {
                if (this.ViewState["BotaoExcel"] == null)
                    return false;

                return Convert.ToBoolean(this.ViewState["BotaoExcel"]);
            }
            set
            {
                this.ViewState["BotaoExcel"] = value;
            }
        }

        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(default(String)),
        Description("Define o evento client click do controle Excel"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public String ClientClickExcel
        {
            get
            {
                if (this.ViewState["ClientClickExcel"] == null)
                    return String.Empty;

                return this.ViewState["ClientClickExcel"].ToString();
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    this.BotaoExcel = true;

                this.ViewState["ClientClickExcel"] = value;
            }
        }

        [
        Bindable(true),
        Description("Evento customizado para baixar Excel."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public event EventHandler ClickExcel;
        #endregion

        #region PDF
        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(false),
        Description("Define se deve exibir o controle Pdf."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public Boolean BotaoPdf
        {
            get
            {
                if (this.ViewState["BotaoPdf"] == null)
                    return false;

                return Convert.ToBoolean(this.ViewState["BotaoPdf"]);
            }
            set
            {
                this.ViewState["BotaoPdf"] = value;
            }
        }

        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(default(String)),
        Description("Define o evento client click do controle Pdf"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public String ClientClickPdf
        {
            get
            {
                if (this.ViewState["ClientClickPdf"] == null)
                    return String.Empty;

                return this.ViewState["ClientClickPdf"].ToString();
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    this.BotaoPdf = true;

                this.ViewState["ClientClickPdf"] = value;
            }
        }

        [
        Bindable(true),
        Description("Evento customizado para baixar PDF."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public event EventHandler ClickPdf;
        #endregion

        #region Email
        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(false),
        Description("Define se deve exibir o controle Email."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public Boolean BotaoEmail
        {
            get
            {
                if (this.ViewState["BotaoEmail"] == null)
                    return false;

                return Convert.ToBoolean(this.ViewState["BotaoEmail"]);
            }
            set
            {
                this.ViewState["BotaoEmail"] = value;
            }
        }

        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(default(String)),
        Description("Define o evento client click do controle Email"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public String ClientClickEmail
        {
            get
            {
                if (this.ViewState["ClientClickEmail"] == null)
                    return String.Empty;

                return this.ViewState["ClientClickEmail"].ToString();
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    this.BotaoEmail = true;

                this.ViewState["ClientClickEmail"] = value;
            }
        }

        [
        Bindable(true),
        Description("Evento customizado para Email."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public event EventHandler ClickEmail;
        #endregion

        #region Csv
        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(false),
        Description("Define se deve exibir o controle Csv."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public Boolean BotaoCsv
        {
            get
            {
                if (this.ViewState["BotaoCsv"] == null)
                    return false;

                return Convert.ToBoolean(this.ViewState["BotaoCsv"]);
            }
            set
            {
                this.ViewState["BotaoCsv"] = value;
            }
        }

        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(default(String)),
        Description("Define o evento client click do controle Csv"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public String ClientClickCsv
        {
            get
            {
                if (this.ViewState["ClientClickCsv"] == null)
                    return String.Empty;

                return this.ViewState["ClientClickCsv"].ToString();
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    this.BotaoCsv = true;

                this.ViewState["ClientClickCsv"] = value;
            }
        }

        [
        Bindable(true),
        Description("Evento customizado para Csv."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public event EventHandler ClickCsv;
        #endregion

        #region Html
        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(false),
        Description("Define se deve exibir o controle Html."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public Boolean BotaoHtml
        {
            get
            {
                if (this.ViewState["BotaoHtml"] == null)
                    return false;

                return Convert.ToBoolean(this.ViewState["BotaoHtml"]);
            }
            set
            {
                this.ViewState["BotaoHtml"] = value;
            }
        }

        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(default(String)),
        Description("Define o evento client click do controle Html"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public String ClientClickHtml
        {
            get
            {
                if (this.ViewState["ClientClickHtml"] == null)
                    return String.Empty;

                return this.ViewState["ClientClickHtml"].ToString();
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    this.BotaoHtml = true;

                this.ViewState["ClientClickHtml"] = value;
            }
        }

        [
        Bindable(true),
        Description("Evento customizado para Html."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public event EventHandler ClickHtml;
        #endregion

        #region Txt
        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(false),
        Description("Define se deve exibir o controle Txt."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public Boolean BotaoTxt
        {
            get
            {
                if (this.ViewState["BotaoTxt"] == null)
                    return false;

                return Convert.ToBoolean(this.ViewState["BotaoTxt"]);
            }
            set
            {
                this.ViewState["BotaoTxt"] = value;
            }
        }

        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(default(String)),
        Description("Define o evento client click do controle Txt"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public String ClientClickTxt
        {
            get
            {
                if (this.ViewState["ClientClickTxt"] == null)
                    return String.Empty;

                return this.ViewState["ClientClickTxt"].ToString();
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    this.BotaoTxt = true;

                this.ViewState["ClientClickTxt"] = value;
            }
        }

        [
        Bindable(true),
        Description("Evento customizado para Txt."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public event EventHandler ClickTxt;
        #endregion

        #region Doc
        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(false),
        Description("Define se deve exibir o controle Doc."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public Boolean BotaoDoc
        {
            get
            {
                if (this.ViewState["BotaoDoc"] == null)
                    return false;

                return Convert.ToBoolean(this.ViewState["BotaoDoc"]);
            }
            set
            {
                this.ViewState["BotaoDoc"] = value;
            }
        }

        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(default(String)),
        Description("Define o evento client click do controle Doc"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public String ClientClickDoc
        {
            get
            {
                if (this.ViewState["ClientClickDoc"] == null)
                    return String.Empty;

                return this.ViewState["ClientClickDoc"].ToString();
            }
            set
            {
                if (!String.IsNullOrWhiteSpace(value))
                    this.BotaoDoc = true;

                this.ViewState["ClientClickDoc"] = value;
            }
        }

        [
        Bindable(true),
        Description("Evento customizado para Doc."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public event EventHandler ClickDoc;
        #endregion

        /// <summary>
        /// Verifica quais controles devem ser inseridos
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.BotaoImprimir || this.ClickImprimir != null)
            {
                this.Controls.Add(new MenuAcaoImprimir(this.ClickImprimir, this.ClientClickImprimir));
            }

            if (this.BotaoExcel || this.ClickExcel != null)
            {
                this.Controls.Add(new MenuAcaoDownloadExcel(this.ClickExcel, this.ClientClickExcel));
            }

            if (this.BotaoPdf || this.ClickPdf != null)
            {
                this.Controls.Add(new MenuAcaoDownloadPdf(this.ClickPdf, this.ClientClickPdf));
            }

            if (this.BotaoEmail || this.ClickEmail != null)
            {
                this.Controls.Add(new MenuAcaoEnviarEmail(this.ClickEmail, this.ClientClickEmail));
            }

            if (this.BotaoCsv || this.ClickCsv != null)
            {
                this.Controls.Add(new MenuAcaoDownloadCsv(this.ClickCsv, this.ClientClickCsv));
            }

            if (this.BotaoHtml || this.ClickHtml != null)
            {
                this.Controls.Add(new MenuAcaoDownloadHtml(this.ClickHtml, this.ClientClickHtml));
            }

            if (this.BotaoTxt || this.ClickTxt != null)
            {
                this.Controls.Add(new MenuAcaoDownloadTxt(this.ClickTxt, this.ClientClickTxt));
            }

            if (this.BotaoDoc || this.ClickDoc != null)
            {
                this.Controls.Add(new MenuAcaoDownloadDoc(this.ClickDoc, this.ClientClickDoc));
            }
        }

        /// <summary>
        /// Rederiza o controle.
        /// </summary>
        /// <param name="writer">O objeto que receberá o conteúdo do controle.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            //Adicionando CssClass padrão
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "side-menu");
            this.RenderBeginTag(writer);

            // renderiza os controles filhos
            this.RenderChildren(writer);

            // Fechando os controles
            this.RenderEndTag(writer);
        }

        /// <summary>
        /// Renderiza o controle filho
        /// </summary>
        /// <param name="controle">Controle</param>
        /// <param name="writer">Html Text writer</param>
        /// <param name="urlImagem">Url da imagem</param>
        /// <param name="texto">Texto</param>
        /// <param name="href">Href do controle</param>
        /// <param name="clientClick">Evento client click</param>
        internal static void RenderControleAcao(WebControl controle, HtmlTextWriter writer, String urlImagem, String texto, String href, String clientClick)
        {
            //<div class="side-menu-item">
            //    <a href="#">
            //        <div class="icon">
            //            <img src="img/.png"/>
            //        </div>
            //        <div class="item-content">
            //            texto
            //        </div>
            //    </a>
            //</div>

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "side-menu-item");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (String.IsNullOrWhiteSpace(href))
                href = "void(0);";

            writer.AddAttribute(HtmlTextWriterAttribute.Href, String.Format("javascript:{0}", href));

            if (!String.IsNullOrWhiteSpace(clientClick))
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, clientClick);

            controle.RenderBeginTag(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "icon");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.AddAttribute(HtmlTextWriterAttribute.Src, urlImagem);
            writer.RenderBeginTag(HtmlTextWriterTag.Img);
            writer.RenderEndTag();//IMG
            writer.RenderEndTag();//DIV

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "item-content");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write(texto);
            writer.RenderEndTag();//DIV

            controle.RenderEndTag(writer);//A
            writer.RenderEndTag();//DIV
        }
    }
}
