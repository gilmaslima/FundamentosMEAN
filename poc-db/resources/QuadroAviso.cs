using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Rede.PN.MultivanAlelo.Core.Web.Controles.Portal
{
    /// <summary>
    /// Disponibiliza o quadro de aviso
    /// </summary>
    [ToolboxData("<{0}:QuadroAviso runat=\"server\"></{0}:QuadroAviso>")]
    public class QuadroAviso : CompositeControl
    {
        #region [ Controles ]
        private Botao btnQuadroAviso;
        #endregion

        #region [ Propriedades Públicas ]
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(TipoQuadroAviso.Sucesso)]
        [Browsable(true)]
        public TipoQuadroAviso TipoQuadro
        {
            get
            {
                if (this.ViewState["TipoQuadro"] == null)
                    return TipoQuadroAviso.Sucesso;

                return (TipoQuadroAviso)Enum.Parse(typeof(TipoQuadroAviso), this.ViewState["TipoQuadro"].ToString(), true);
            }
            set
            {
                this.ViewState["TipoQuadro"] = value;
            }
        }

        /// <summary>
        /// Caso essa propriedade esteja preenchida é removido os filhos nativos
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [TemplateInstance(TemplateInstance.Single)]
        public ITemplate ConteudoHtml
        {
            get
            {
                return conteudoHtml;
            }
            set
            {
                conteudoHtml = value;
            }
        }
        private ITemplate conteudoHtml;

        /// <summary>
        /// Define o título a ser exibido.
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(default(String))]
        [Browsable(true)]
        public String Titulo
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
        [DefaultValue(default(String))]
        [Browsable(true)]
        public String Mensagem
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

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("Botao quadro")]
        [Browsable(true)]
        public String ButtonText
        {
            get
            {
                this.EnsureChildControls();
                return this.btnQuadroAviso.Text;
            }
            set
            {
                this.EnsureChildControls();
                this.btnQuadroAviso.Text = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Browsable(true)]
        public Boolean BotaoPrimario
        {
            get
            {
                this.EnsureChildControls();
                return this.btnQuadroAviso.BotaoPrimario;
            }
            set
            {
                this.EnsureChildControls();
                this.btnQuadroAviso.BotaoPrimario = value;
            }
        }

        [
        Bindable(true),
        Category("Appearance"),
        DefaultValue(default(String)),
        Description("Define o evento client click do botao"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerProperty),
        Browsable(true)
        ]
        public String ClientClick
        {
            get
            {
                this.EnsureChildControls();
                return this.btnQuadroAviso.OnClientClick;
            }
            set
            {
                this.EnsureChildControls();
                this.btnQuadroAviso.OnClientClick = value;
            }
        }

        public event EventHandler Click;

        #endregion

        #region [ Métodos Sobrescritos / Implementações Interfaces ]

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.Click != null || !String.IsNullOrWhiteSpace(this.ClientClick))
            {
                this.btnQuadroAviso.Visible = true;
            }
        }

        /// <summary>
        /// Recria os controles filhos
        /// </summary>
        protected override void RecreateChildControls()
        {
            this.EnsureChildControls();
        }

        /// <summary>
        /// Cria os controls filhos
        /// </summary>
        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            this.btnQuadroAviso = new Botao
            {
                ID = "btnQuadroAviso",
                ClasseAdicional = "alert-button",
                BotaoPrimario = true,
                Visible = false
            };

            this.btnQuadroAviso.Click += this.btnQuadroAviso_Click;
            this.Controls.Add(btnQuadroAviso);

            // Se existir conteudo HTML remove os filhos e deixa apenas ele
            if (this.ConteudoHtml != null)
            {
                this.Controls.Clear();
                this.ConteudoHtml.InstantiateIn(this);
            }
        }

        /// <summary>
        /// Evento de clique do botão selecionar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnQuadroAviso_Click(object sender, EventArgs e)
        {
            if (this.Click != null)
                this.Click(sender, e);
        }

        /// <summary>
        /// Rederiza o controle.
        /// </summary>
        /// <param name="writer">O objeto que receberá o conteúdo do controle.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            String classeBotao = String.Empty;

            switch (this.TipoQuadro)
            {
                case TipoQuadroAviso.Aviso:
                    classeBotao = "warning";
                    break;
                case TipoQuadroAviso.Erro:
                    classeBotao = "error";
                    break;
                case TipoQuadroAviso.Sucesso:
                default:
                    classeBotao = "smiley";
                    break;
            }

            classeBotao = String.Format("alert-rede {0}", classeBotao);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, classeBotao);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "alert-icon");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "alert-content");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (this.ConteudoHtml != null)
            {
                base.RenderChildren(writer);
            }
            else
            {
                writer.Write(this.Mensagem);
            }

            writer.RenderEndTag();

            if (this.ConteudoHtml == null)
                base.RenderChildren(writer);

            writer.RenderEndTag();
        }

        #endregion

    }

    public enum TipoQuadroAviso
    {
        Sucesso,
        Aviso,
        Erro
    }
}
