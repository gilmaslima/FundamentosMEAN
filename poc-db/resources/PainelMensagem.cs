using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
    /// <summary>
    /// Controle de painel informativo
    /// </summary>
    [ParseChildren(true, "Botoes"),
    ToolboxData("<{0}:PainelMensagem runat=\"server\"></{0}:PainelMensagem>")]
    public class PainelMensagem : CompositeControl, INamingContainer
    {
        #region [ Controles Filhos ]

        /// <summary>
        /// Placeholder para botões customizados
        /// </summary>
        private PlaceHolder phBotoesCustom;

        /// <summary>
        /// Placeholder para conteúdo complementar customizado
        /// </summary>
        private PlaceHolder phCustomContent;

        #endregion

        #region [ Atributos do controle ]

        /// <summary>
        /// Tipo da mensagem a ser exibida ao usuário (Sucesso, Aviso, Erro)
        /// </summary>
        [DefaultValue(PainelMensagemIcon.Aviso)]
        public PainelMensagemIcon TipoMensagem
        {
            get
            {
                if (ViewState["TipoMensagem"] == null)
                    ViewState["TipoMensagem"] = PainelMensagemIcon.Aviso;

                return (PainelMensagemIcon)ViewState["TipoMensagem"];
            }
            set
            {
                ViewState["TipoMensagem"] = value;
            }
        }

        /// <summary>
        /// Título da mensagem
        /// </summary>
        public String Titulo
        {
            get
            {
                return Convert.ToString(ViewState["Titulo"]);
            }
            set
            {
                ViewState["Titulo"] = value;
            }
        }

        /// <summary>
        /// Mensagem para exibição
        /// </summary>
        public String Mensagem
        {
            get
            {
                return Convert.ToString(ViewState["Mensagem"]);
            }
            set
            {
                ViewState["Mensagem"] = value;
            }
        }

        /// <summary>
        /// Define a classe CSS para o container principal
        /// </summary>
        public string MessageContainerCssClass
        {
            get
            {
                return Convert.ToString(ViewState["MessageContainerCssClass"]);
            }
            set
            {
                ViewState["MessageContainerCssClass"] = value;
            }
        }

        /// <summary>
        /// Define se deve exibir os botões de download do app
        /// </summary>
        [DefaultValue(false)]
        public Boolean ShowBotoesApp
        {
            get
            {
                return ((Boolean?)ViewState["ShowBotoesApp"]).GetValueOrDefault(false);
            }
            set
            {
                ViewState["ShowBotoesApp"] = value;
            }
        }

        /// <summary>
        /// Css class para o container de botões
        /// </summary>
        public String BotoesAppCustomCssClass
        {
            get
            {
                return Convert.ToString(ViewState["BotoesAppCustomCssClass"]);
            }
            set
            {
                ViewState["BotoesAppCustomCssClass"] = value;
            }
        }

        /// <summary>
        /// Define se deve ocultar o container dos botões customizados
        /// </summary>
        [DefaultValue(false)]
        public Boolean HideBotoesCustom
        {
            get
            {
                return ((Boolean?)ViewState["HideBotoesCustom"]).GetValueOrDefault(false);
            }
            set
            {
                ViewState["HideBotoesCustom"] = value;
            }
        }

        /// <summary>
        /// Define se deve ocultar o container de conteúdo customizável
        /// </summary>
        [DefaultValue(false)]
        public Boolean HideCustomContent
        {
            get
            {
                this.EnsureChildControls();
                return this.phCustomContent.Visible;
            }
            set
            {
                this.EnsureChildControls();
                this.phCustomContent.Visible = false;
            }
        }

        /// <summary>
        /// Listagem de botões
        /// </summary>
        [Browsable(true),
        TemplateContainer(typeof(PainelMensagemBotoesTemplate)),
        TemplateInstance(TemplateInstance.Single),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ITemplate Botoes { get; set; }

        /// <summary>
        /// Conteúdo complementar customizável
        /// </summary>
        [Browsable(true),
        TemplateContainer(typeof(PainelMensagemCustomContentTemplate)),
        TemplateInstance(TemplateInstance.Single),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ITemplate CustomContent { get; set; }

        #endregion

        #region [ Eventos nativos ]

        /// <summary>
        /// Criação dos controles filhos
        /// </summary>
        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            this.phCustomContent = new PlaceHolder();
            this.Controls.Add(this.phCustomContent);

            this.phBotoesCustom = new PlaceHolder();
            this.Controls.Add(this.phBotoesCustom);
        }

        /// <summary>
        /// Evento ao iniciar o controle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.EnsureChildControls();

            // conteúdo complementar customizado
            this.phCustomContent.Controls.Clear();
            if (this.CustomContent != null)
            {
                PainelMensagemCustomContentTemplate content = new PainelMensagemCustomContentTemplate();
                this.CustomContent.InstantiateIn(content);
                this.phCustomContent.Controls.Add(content);
            }

            // botões de ação customizados
            this.phBotoesCustom.Controls.Clear();
            if (this.Botoes != null)
            {
                PainelMensagemBotoesTemplate botoesTemplate = new PainelMensagemBotoesTemplate();
                this.Botoes.InstantiateIn(botoesTemplate);
                this.phBotoesCustom.Controls.Add(botoesTemplate);
                this.HideBotoesCustom = botoesTemplate.Controls.Count == 0;
            }
            else
            {
                this.HideBotoesCustom = true;
            }
        }

        /// <summary>
        /// Renderização dos controles
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            // contéudo principal
            this.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, String.Format("feedback-rede margin-top-sm {0}", this.MessageContainerCssClass));
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "display-table");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                {
                    #region Icon

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "feedback-icon");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    {

                        // define a classe do ícone segundo o tipo de quadro a ser exibido
                        switch (this.TipoMensagem)
                        {
                            case PainelMensagemIcon.Aviso:
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, "icon-itaufonts_exclamacao");
                                break;
                            case PainelMensagemIcon.Erro:
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, "icon-itaufonts_fechar");
                                break;
                            case PainelMensagemIcon.Sucesso:
                            default:
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, "icon-itaufonts_check");
                                break;
                        }
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, "icon");
                        writer.RenderBeginTag(HtmlTextWriterTag.I);
                        writer.RenderEndTag();

                    }
                    writer.RenderEndTag();

                    #endregion

                    #region Título / Mensagem

                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    {
                        // título
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "feedback-title");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.Write(this.Titulo);
                        writer.RenderEndTag();

                        // mensagem
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "feedback-text");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.Write(this.Mensagem);
                        writer.RenderEndTag();
                    }
                    writer.RenderEndTag();

                    #endregion
                }
                writer.RenderEndTag();
            }
            writer.RenderEndTag();

            // verifica se deve exibir o container de conteúdo customizado
            if (this.phCustomContent.Controls.Count > 0 && this.phCustomContent.Visible)
                this.phCustomContent.RenderControl(writer);

            // verifica se deve exibir o container principal de botões
            if (this.phBotoesCustom.Controls.Count > 0 && !this.HideBotoesCustom || this.ShowBotoesApp)
            {
                // se não há botões do app, exibe o container dos botões em branco
                writer.AddAttribute(HtmlTextWriterAttribute.Class, String.Format(
                    "feedback-box app-download {0} {1}",
                    !this.ShowBotoesApp ? "feedback-box-white" : "",
                    this.BotoesAppCustomCssClass));
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                {
                    if (this.ShowBotoesApp)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "box-title-rede");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.Write("baixe nosso aplicativo Rede");
                        writer.RenderEndTag();
                    }

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "row");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    {
                        #region Botões App

                        if (this.ShowBotoesApp)
                        {
                            Boolean customVisible = this.phBotoesCustom.Controls.Count > 0 && !this.HideBotoesCustom;

                            this.RenderAppButton(
                                writer, "Google Play", "google-play",
                                "/sites/fechado/Style Library/pt-br/Redecard/Img/redeareafechada-facelift/google-play-logo.png",
                                "https://play.google.com/store/apps/details?id=br.com.userede",
                                customVisible);

                            this.RenderAppButton(
                                writer, "App Store", "app-store",
                                "/sites/fechado/Style Library/pt-br/Redecard/Img/redeareafechada-facelift/apple_store_logo.png",
                                "https://itunes.apple.com/br/app/rede/id1020661630?mt=8",
                                customVisible);
                        }

                        #endregion

                        #region Conteúdo customizado

                        if (this.phBotoesCustom.Controls.Count > 0 && !this.HideBotoesCustom)
                            this.phBotoesCustom.RenderControl(writer);

                        #endregion
                    }
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();
            }
        }

        /// <summary>
        /// Renderiza um botão para download do App
        /// </summary>
        /// <param name="writer">Writer para renderização do conteúdo</param>
        /// <param name="gtmAttr">Atributo do GTM para cada botão</param>
        /// <param name="imageClass">Class específica para cada tipo de botão</param>
        /// <param name="imageSrc">Source do elemento de imagem</param>
        /// <param name="urlApp">URL para download do app</param>
        /// <param name="customVisible">Identifica se o container customizado de botões está visível para melhor redimencionamento do conteúdo</param>
        private void RenderAppButton(
            HtmlTextWriter writer,
            String gtmAttr,
            String imageClass,
            String imageSrc,
            String urlApp,
            Boolean customVisible)
        {
            string colCssClass = customVisible ? "col-sm-4 col-xs-6" : "col-sm-6 col-xs-6";

            writer.AddAttribute(HtmlTextWriterAttribute.Class, colCssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn-rede btn-app");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "btn-secondary");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, urlApp);
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, String.Format("app-link-img {0}", imageClass));
                            writer.AddAttribute(HtmlTextWriterAttribute.Src, imageSrc);
                            writer.AddAttribute("data-gtm-attr", gtmAttr);
                            writer.RenderBeginTag(HtmlTextWriterTag.Img);
                            writer.RenderEndTag();
                        }
                        writer.RenderEndTag();
                    }
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        #endregion

        /// <summary>
        /// Class para instância do template dos botões
        /// </summary>
        [ToolboxItem(false)]
        private class PainelMensagemBotoesTemplate : Control, INamingContainer { }

        /// <summary>
        /// Class para instância de template para conteúdo complementar customizado
        /// </summary>
        [ToolboxItem(false)]
        private class PainelMensagemCustomContentTemplate : Control, INamingContainer { }
    }
}
