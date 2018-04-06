using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.Core.Web.Controles.Portal
{
    /// <summary>
    /// Controle QuadroInformacao
    /// </summary>
    [ToolboxData(
    @"<{0}:QuadroInformacao ID="""" runat=""server"">
        <{0}:QuadroInformacaoItem Descricao="""" Valor="""" />
    </{0}:QuadroInformacao>"),
    ParseChildren(typeof(QuadroInformacaoItem), DefaultProperty = "QuadroInformacaoItems", ChildrenAsProperties = true)]
    public class QuadroInformacao : CompositeControl
    {
        #region [ Controles / Propriedades privadas ]
        private Panel pnlContainerBotao;
        private Botao btnAcao;
        #endregion

        #region [ Propriedades Públicas ]

        /// <summary>
        /// Items com os valores e descritivos
        /// </summary>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public List<QuadroInformacaoItem> QuadroInformacaoItems
        {
            get
            {
                if (ViewState["QuadroInformacaoItems"] == null)
                    ViewState["QuadroInformacaoItems"] = new List<QuadroInformacaoItem>();

                return (List<QuadroInformacaoItem>)ViewState["QuadroInformacaoItems"];
            }
            set
            {
                ViewState["QuadroInformacaoItems"] = value;
            }
        }

        /// <summary>
        /// Define o título a ser exibido
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(default(String))]
        [Browsable(true)]
        public String Titulo
        {
            get
            {
                this.EnsureChildControls();
                return Convert.ToString(ViewState["Titulo"]);
            }
            set
            {
                this.EnsureChildControls();
                ViewState["Titulo"] = value;
            }
        }

        /// <summary>
        /// Define se o botão é primário ou secundário
        /// - True: primário
        /// - False: secundário
        /// - Default (true)
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Browsable(true)]
        public Boolean BotaoPrimario
        {
            get
            {
                this.EnsureChildControls();
                return this.btnAcao.BotaoPrimario;
            }
            set
            {
                this.EnsureChildControls();
                this.btnAcao.BotaoPrimario = value;
            }
        }

        /// <summary>
        /// Define se o botão está visível na página
        /// - True: visível
        /// - False: invisível
        /// - Default (false)
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Browsable(true)]
        public Boolean BotaoVisible
        {
            get
            {
                this.EnsureChildControls();
                return this.btnAcao.Visible;
            }
            set
            {
                this.EnsureChildControls();
                this.btnAcao.Visible = value;
            }
        }

        /// <summary>
        /// Texto do botão do quadro de informação
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Browsable(true)]
        public String BotaoText
        {
            get
            {
                this.EnsureChildControls();
                return this.btnAcao.Text;
            }
            set
            {
                this.EnsureChildControls();
                this.btnAcao.Text = value;
            }
        }

        /// <summary>
        /// Evento de click implementado em Client Side do botão de ação
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(default(String)),
        Description("Define o evento client click do botao"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)]
        public String ClientClick
        {
            get
            {
                this.EnsureChildControls();
                return this.btnAcao.OnClientClick;
            }
            set
            {
                this.EnsureChildControls();
                this.btnAcao.OnClientClick = value;
            }
        }

        /// <summary>
        /// Command Argument para o botao customizado
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(default(String)),
        Description("Define o valor do Command Argument"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)]
        public String CommandArgument
        {
            get
            {
                this.EnsureChildControls();
                return this.btnAcao.CommandArgument;
            }
            set
            {
                this.EnsureChildControls();
                this.btnAcao.CommandArgument = value;
            }
        }

        /// <summary>
        /// Evento de click implementado em Code Behind do botão de ação
        /// </summary>
        public event EventHandler Click;

        #endregion

        #region [ Métodos Sobrescritos / Implementações Interfaces ]

        /// <summary>
        /// Cria os controls filhos
        /// </summary>
        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            // botão do quadro de informação
            this.pnlContainerBotao = new Panel() { CssClass = "value-box" };
            this.btnAcao = new Botao
            {
                ID = "btnAcao",
                ClasseAdicional = "alert-button",
                BotaoPrimario = true,
                Visible = false
            };
            this.btnAcao.Click += this.btnAcao_Click;
            this.pnlContainerBotao.Controls.Add(this.btnAcao);
            this.Controls.Add(this.pnlContainerBotao);
        }

        /// <summary>
        /// Renderiza o controle
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            // define visibilidade do quadro do botão pelos critérios (em ordem):
            // - se o botão está visível
            // - se foi informado o texto do botão
            // - se há evento de click (codebehind ou client) atribuído
            this.pnlContainerBotao.Visible =
                this.BotaoVisible
                && !string.IsNullOrWhiteSpace(this.BotaoText)
                && (this.Click != null || !string.IsNullOrWhiteSpace(this.ClientClick));

            // início do controle sendo renderizado
            this.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                // define visibilidade do título
                if (!string.IsNullOrWhiteSpace(this.Titulo))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "content-title");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.WriteLine(this.Titulo);
                    writer.RenderEndTag();
                }

                // container principal
                string cssClass = string.Format("money-values {0}", this.pnlContainerBotao.Visible ? "values-button" : "");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                {
                    // items do QuadroInformacao
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "value-box");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    {
                        // adiciona controles filhos
                        foreach (var item in this.QuadroInformacaoItems)
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "value-rede");
                            writer.RenderBeginTag(HtmlTextWriterTag.Div);
                            {
                                // título
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, "commom-text-bold");
                                writer.RenderBeginTag("div");
                                writer.WriteLine(item.Descricao);
                                writer.RenderEndTag();

                                // descrição
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, "value-number");
                                writer.RenderBeginTag("div");
                                writer.WriteLine(item.Valor);
                                writer.RenderEndTag();
                            }
                            writer.RenderEndTag();
                        }
                    }
                    writer.RenderEndTag();

                    // quadro de botões
                    this.RenderChildren(writer);
                }
                writer.RenderEndTag();
            }
            this.RenderEndTag(writer);
        }

        /// <summary>
        /// Evento de clique do botão de ação do quadro de informações
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAcao_Click(object sender, EventArgs e)
        {
            if (this.Click != null)
                this.Click(sender, e);
        }

        #endregion

    }
}
