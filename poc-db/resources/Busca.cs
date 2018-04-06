using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace Redecard.PN.RAV.Core.Web.Controles.Portal
{
    /// <summary>
    /// Disponibiliza o controle de Botao.
    /// </summary>
    [ToolboxData("<{0}:Busca runat=server></{0}:Busca>")]
    public class Busca : Panel
    {
        public Busca()
        {
            this.CssClass = "rede-search";
        }

        #region Controles filhos

        private HtmlGenericControl containerLabelBusca;
        private Panel pnlContainerCampoBusca;
        private TextBox txtBusca;
        private HtmlGenericControl botaoBusca;

        #endregion

        #region Atributos expostos

        /// <summary>
        /// Class(ess) adicional(is) para o container do label
        /// </summary>
        public string ClassLabelBusca
        {
            get
            {
                this.EnsureChildControls();
                return this.containerLabelBusca.Attributes["class"];
            }
            set
            {
                this.EnsureChildControls();
                this.containerLabelBusca.Attributes.Add("class", value);
            }
        }

        /// <summary>
        /// Class(ess) adicional(is) para o campo de busca
        /// </summary>
        public string ClassCampoBusca
        {
            get
            {
                this.EnsureChildControls();
                return this.txtBusca.Attributes["class"];
            }
            set
            {
                this.EnsureChildControls();
                this.txtBusca.Attributes.Add("class", value);
            }
        }

        /// <summary>
        /// Class(ess) adicional(is) para o botão de busca
        /// </summary>
        public string ClassBotaoBusca
        {
            get
            {
                this.EnsureChildControls();
                return this.botaoBusca.Attributes["class"];
            }
            set
            {
                this.EnsureChildControls();
                this.botaoBusca.Attributes.Add("class", value);
            }
        }

        /// <summary>
        /// Controla se deve exibir o botão de busca
        /// </summary>
        public bool ExibirBotaoBusca
        {
            get
            {
                this.EnsureChildControls();
                return this.botaoBusca.Visible;
            }
            set
            {
                this.EnsureChildControls();
                this.botaoBusca.Visible = value;
            }
        }

        /// <summary>
        /// Define se deve exibir a label
        /// </summary>
        public bool ExibirLabel
        {
            get
            {
                this.EnsureChildControls();
                return this.containerLabelBusca.Visible;
            }
            set
            {
                this.EnsureChildControls();
                this.containerLabelBusca.Visible = value;
            }
        }

        /// <summary>
        /// Label do campo de busca
        /// </summary>
        public string LabelBusca
        {
            get
            {
                this.EnsureChildControls();
                if (string.IsNullOrWhiteSpace(this.containerLabelBusca.InnerText))
                    return "filtrar por:";

                return this.containerLabelBusca.InnerText;
            }
            set
            {
                this.EnsureChildControls();
                this.containerLabelBusca.InnerText = value;
            }
        }

        /// <summary>
        /// Atributos customizados ao campo de texto
        /// </summary>
        public AttributeCollection CampoBuscaAttribute
        {
            get
            {
                this.EnsureChildControls();
                return this.txtBusca.Attributes;
            }
            set
            {
                this.EnsureChildControls();
                if (value != null)
                {
                    var attrKeys = value.Keys.GetEnumerator();
                    while (attrKeys.MoveNext())
                    {
                        string key = Convert.ToString(attrKeys.Current);
                        this.txtBusca.Attributes.Add(key, value[key]);
                    }
                }
            }
        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Criação dos controles filhos
        /// </summary>
        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            // container do label
            this.containerLabelBusca = new HtmlGenericControl("div");
            this.containerLabelBusca.Attributes.Add("class", "search-text");
            this.containerLabelBusca.InnerText = this.LabelBusca;
            this.Controls.Add(this.containerLabelBusca);

            // container dos controles de busca
            this.pnlContainerCampoBusca = new Panel() { CssClass = "search-box" };
            this.Controls.Add(this.pnlContainerCampoBusca);

            // campo texto de pesquisa
            this.txtBusca = new TextBox() { ID = "txtBusca", CssClass = "search-input" };
            this.txtBusca.Attributes.Add("type", "search");
            this.pnlContainerCampoBusca.Controls.Add(this.txtBusca);

            // imagem/botão de busca
            this.botaoBusca = new HtmlGenericControl("input");
            this.botaoBusca.Attributes.Add("class", "search-button");
            this.pnlContainerCampoBusca.Controls.Add(this.botaoBusca);
        }

        #endregion
    }
}
