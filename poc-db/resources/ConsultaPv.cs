using CoreSeguranca = Rede.PN.MultivanAlelo.Core.Seguranca.Portal;
using CoreWeb = Rede.PN.MultivanAlelo.Core.Web.Controles.Portal;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Rede.PN.MultivanAlelo.Core.Web.Controles.Portal
{
    /// <summary>
    /// Disponibiliza o controle de Botao.
    /// </summary>
    [ToolboxData("<{0}:ConsultaPv runat=\"server\"></{0}:ConsultaPv>")]
    public class ConsultaPv : CompositeControl
    {
        #region [ Atributos auxiliares ]

        private JavaScriptSerializer jsSerializer;
        private JavaScriptSerializer JsSerializer
        {
            get
            {
                if (this.jsSerializer == null)
                    this.jsSerializer = new JavaScriptSerializer();

                return this.jsSerializer;
            }
        }

        private Sessao sessaoAtual;
        public Sessao SessaoAtual
        {
            get
            {
                if (this.sessaoAtual == null)
                    this.sessaoAtual = Sessao.Obtem();

                return this.sessaoAtual;
            }
        }

        #endregion

        #region [ Controles ]
        private CoreWeb.DropDownList ddlTipoAssociacao;
        private Panel pnlTipoAssossiacao;
        private HiddenField hdnSessao;
        private HiddenField hdnFiliais;
        private HiddenField hdnMsgFiliais;
        private HiddenField hdnPVsSelecionados;
        private HiddenField hdnOnClientSelectionChanged;
        private HiddenField hdnOnClientPopupClosed;
        private HiddenField hdnOnNoDataFound;
        private HiddenField hdnOrigem;
        private HiddenField hdnModo;
        private HiddenField hdnPVObrigatorio;

        // controles do Lightbox para seleção dos PVs
        private Panel pnlLightBoxPvs;
        private TextBox txtBusca;
        private RadioButtonList rdtipoMoeda;
        private CheckBox chkTodos;
        private CheckBox chkAtivos;
        private CheckBox chkCancelados;
        private Panel pnlListaPV;
        private Botao btnSelecionar;
        private HtmlGenericControl spanAlterarEstabelecimento;
        #endregion

        #region [ Propriedades Públicas ]

        public ConsultaPvTipoAssociacao TipoAssociacao
        {
            get
            {
                EnsureChildControls();
                return (ConsultaPvTipoAssociacao)ddlTipoAssociacao.SelectedValue.ToInt32(0);
            }
        }

        public List<Int32> PVsSelecionados
        {
            get
            {
                EnsureChildControls();
                return hdnPVsSelecionados.Value.ToString()
                    .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(pvCripto => CoreSeguranca.Criptografia.DescriptografarPV(pvCripto, SessaoAtual.CodigoEntidade))
                    .ToList().Distinct().ToList();
            }
            set
            {
                EnsureChildControls();
                hdnPVsSelecionados.Value = String.Join(";", value.Select(pv => CoreSeguranca.Criptografia.CriptografarPV(pv, SessaoAtual.CodigoEntidade)).ToArray());
            }
        }

        public String OnClientSelectionChanged
        {
            get
            {
                EnsureChildControls();
                return hdnOnClientSelectionChanged.Value.ToString();
            }
            set
            {
                EnsureChildControls();
                hdnOnClientSelectionChanged.Value = value;
            }
        }

        public String OnClientPopupClosed
        {
            get
            {
                EnsureChildControls();
                return hdnOnClientPopupClosed.Value.ToString();
            }
            set
            {
                EnsureChildControls();
                hdnOnClientPopupClosed.Value = value;
            }
        }

        public String OnNoDataFound
        {
            get
            {
                EnsureChildControls();
                return hdnOnNoDataFound.Value.ToString();
            }
            set
            {
                EnsureChildControls();
                hdnOnNoDataFound.Value = value;
            }
        }

        public ConsultaPvOrigemSolicitacao Origem
        {
            get
            {
                EnsureChildControls();

                String origem = hdnOrigem.Value.ToString();
                if (!Enum.IsDefined(typeof(ConsultaPvOrigemSolicitacao), origem))
                    return ConsultaPvOrigemSolicitacao.FiltroRelatorios;

                return (ConsultaPvOrigemSolicitacao)Enum.Parse(typeof(ConsultaPvOrigemSolicitacao), origem);
            }
            set
            {
                EnsureChildControls();
                hdnOrigem.Value = value.ToString();
            }
        }

        public ConsultaPvModo ModoSelecao
        {
            get
            {
                EnsureChildControls();

                String modo = hdnModo.Value.ToString();
                if (!Enum.IsDefined(typeof(ConsultaPvModo), modo))
                    return ConsultaPvModo.Multiplo;

                return (ConsultaPvModo)Enum.Parse(typeof(ConsultaPvModo), modo);
            }
            set
            {
                EnsureChildControls();
                hdnModo.Value = value.ToString();
            }
        }

        public CoreWeb.DropDownList DropDownList { get { return this.ddlTipoAssociacao; } }

        public Boolean UseSubmitBehavior
        {
            get
            {
                EnsureChildControls();
                return btnSelecionar.UseSubmitBehavior;
            }
            set
            {
                EnsureChildControls();
                btnSelecionar.UseSubmitBehavior = value;
            }
        }

        public Boolean PVObrigatorio
        {
            get
            {
                EnsureChildControls();
                String pvObrigatorio = hdnPVObrigatorio.Value.ToString();
                return pvObrigatorio.ToLower() != "false";
            }
            set
            {
                EnsureChildControls();
                hdnPVObrigatorio.Value = value.ToString().ToLower();
            }
        }

        public String ButtonText
        {
            get
            {
                EnsureChildControls();
                return btnSelecionar.Text;
            }
            set
            {
                EnsureChildControls();
                btnSelecionar.Text = value;
            }
        }

        public String TextAlteracaoEstabelecimento
        {
            get
            {
                this.EnsureChildControls();
                return spanAlterarEstabelecimento.InnerText;
            }
            set
            {
                this.EnsureChildControls();
                this.spanAlterarEstabelecimento.InnerText = value;
            }
        }

        public event EventHandler Click;

        public ConsultaPvTipoEntidade? TiposEntidade
        {
            set { ViewState["TiposEntidade"] = value; }
            get { return (ConsultaPvTipoEntidade?)ViewState["TiposEntidade"]; }
        }

        /// <summary>
        /// Definição da primeira linha do DropDownList
        /// - TRUE: considera o primeiro item
        /// - FALSE: trata o primeiro item como label
        /// </summary>
        public Boolean ShowFirstLine
        {
            get
            {
                EnsureChildControls();
                return this.ddlTipoAssociacao.ShowFirstLine;
            }
            set
            {
                EnsureChildControls();
                this.ddlTipoAssociacao.ShowFirstLine = value;
            }
        }

        #endregion

        #region [ Métodos Sobrescritos / Implementações Interfaces ]

        /// <summary>
        /// Recria os controles filhos
        /// </summary>
        protected override void RecreateChildControls()
        {
            EnsureChildControls();
        }

        /// <summary>
        /// Cria os controls filhos
        /// </summary>
        protected override void CreateChildControls()
        {
            this.Controls.Clear();

            // controles genéricos para uso na contrução dos elementos
            HtmlGenericControl div = null;
            HtmlGenericControl span = null;

            #region ddlTipoAssociacao

            this.pnlTipoAssossiacao = new Panel
            {
                ID = "pnlTipoAssossiacao",
                CssClass = "pnl-tipo-assossiacao"
            };

            this.ddlTipoAssociacao = new CoreWeb.DropDownList { ID = "ddlTipoAssociacao", ShowFirstLine = false };
            this.ddlTipoAssociacao.Items.AddRange(
                new ListItem[] {
                            new ListItem("selecione o estabelecimento", "-1", true),
                            new ListItem("próprio", "0"),
                            new ListItem("filiais", "2"),
                            new ListItem("centralizados", "1"),
                            new ListItem("consignados", "3"),
                            new ListItem("mesmo cnpj", "4")
                        });
            this.ddlTipoAssociacao.AutoPostBack = false;
            this.ddlTipoAssociacao.Attributes.Add("onchange", "ConsultaPV.ddlTipoAssociacao_onchange(this);");
            this.pnlTipoAssossiacao.Controls.Add(this.ddlTipoAssociacao);

            this.spanAlterarEstabelecimento = new HtmlGenericControl("span") { ID = "spanAlterarPv" };
            this.spanAlterarEstabelecimento.Attributes.Add("onclick", "ConsultaPV.spanAlterarPv_onclick(this);");
            this.spanAlterarEstabelecimento.InnerText = "alterar estabelecimento";
            this.pnlTipoAssossiacao.Controls.Add(this.spanAlterarEstabelecimento);

            this.Controls.Add(this.pnlTipoAssossiacao);
            #endregion

            #region Hidden Fields

            this.hdnSessao = new HiddenField() { ID = "hdnSessao" };
            this.hdnFiliais = new HiddenField() { ID = "hdnFiliais" };
            this.hdnMsgFiliais = new HiddenField() { ID = "hdnMsgFiliais" };
            this.hdnPVsSelecionados = new HiddenField() { ID = "hdnPVsSelecionados" };
            this.hdnOnClientSelectionChanged = new HiddenField() { ID = "hdnOnClientSelectionChanged" };
            this.hdnOnClientPopupClosed = new HiddenField() { ID = "hdnOnClientPopupClosed" };
            this.hdnOnNoDataFound = new HiddenField() { ID = "hdnOnNoDataFound" };
            this.hdnOrigem = new HiddenField() { ID = "hdnOrigem" };
            this.hdnModo = new HiddenField() { ID = "hdnModo" };
            this.hdnPVObrigatorio = new HiddenField() { ID = "hdnPVObrigatorio" };

            this.Controls.Add(hdnSessao);
            this.Controls.Add(hdnFiliais);
            this.Controls.Add(hdnMsgFiliais);
            this.Controls.Add(hdnPVsSelecionados);
            this.Controls.Add(hdnOnClientSelectionChanged);
            this.Controls.Add(hdnOnClientPopupClosed);
            this.Controls.Add(hdnOnNoDataFound);
            this.Controls.Add(hdnOrigem);
            this.Controls.Add(hdnModo);
            this.Controls.Add(hdnPVObrigatorio);
            #endregion

            #region LightBox Seleção de PVs

            // painel principal
            this.pnlLightBoxPvs = new Panel();
            this.pnlLightBoxPvs.ID = "pnlLightBoxPvs";
            this.pnlLightBoxPvs.CssClass = "modal-rede-lightbox";
            this.pnlLightBoxPvs.Style.Add("display", "none");
            this.pnlLightBoxPvs.Attributes.Add("title", "selecione o estabelecimento");

            // title default do lightbox
            HiddenField hdnLightBoxTitle = new HiddenField() { ID = "hdnLightBoxTitle", Value = "selecione o estabelecimento" };
            this.pnlLightBoxPvs.Controls.Add(hdnLightBoxTitle);

            // título do controle original (contornando o problema do deslocamento do modal)
            HiddenField hdnOriginalFieldTitle = new HiddenField() { ID = "hdnOriginalFieldTitle", Value = this.ClientID };
            this.pnlLightBoxPvs.Controls.Add(hdnOriginalFieldTitle);

            #region Busca

            Busca busca = new Busca();
            busca.LabelBusca = "filtrar por:";
            busca.CampoBuscaAttribute.Add("onkeyup", "ConsultaPV.txtBusca_onkeyup(this);");
            busca.CampoBuscaAttribute.Add("placeholder", "digite aqui o número ou nome do estabelecimento");

            this.pnlLightBoxPvs.Controls.Add(busca);

            #endregion

            #region Filtro

            Panel pnlSearchFilter = new Panel() { CssClass = "search-check" };

            // Radio TipoMoeda
            Panel pnlSearchFilterMoeda = new Panel() { CssClass = "left" };
            this.rdtipoMoeda = new RadioButtonList()
            {
                ID = "tipoMoeda",
                RepeatDirection = RepeatDirection.Horizontal,
                RepeatLayout = RepeatLayout.Flow,
                CssClass = "rede-input"
            };
            this.rdtipoMoeda.Items.AddRange(new ListItem[] {
                new ListItem("R$", "r"),
                new ListItem("US$", "d")
            });
            this.rdtipoMoeda.Items[0].Selected = true;
            this.rdtipoMoeda.Attributes.Add("onclick", "ConsultaPV.rbtnTipoMoeda_onclick(this);");
            pnlSearchFilterMoeda.Controls.Add(this.rdtipoMoeda);
            pnlSearchFilter.Controls.Add(pnlSearchFilterMoeda);

            // Checks
            Panel pnlSearchFilterChecks = new Panel() { CssClass = "right" };

            // checkbox Todos
            span = new HtmlGenericControl("span");
            span.Attributes.Add("class", "check-item");
            this.chkTodos = new CheckBox() { ID = "chkTodos", CssClass = "rede-input", Text = "todos" };
            this.chkTodos.Attributes.Add("onclick", "ConsultaPV.chkTodos_onclick(this);");
            span.Controls.Add(this.chkTodos);
            pnlSearchFilterChecks.Controls.Add(span);

            // checkbox Ativos
            span = new HtmlGenericControl("span");
            span.Attributes.Add("class", "check-item");
            this.chkAtivos = new CheckBox() { ID = "chkAtivos", CssClass = "rede-input", Text = "ativos" };
            this.chkAtivos.Attributes.Add("onclick", "ConsultaPV.chkAtivos_onclick(this);");
            span.Controls.Add(this.chkAtivos);
            pnlSearchFilterChecks.Controls.Add(span);

            // checkbox Ativos
            span = new HtmlGenericControl("span");
            span.Attributes.Add("class", "check-item");
            this.chkCancelados = new CheckBox() { ID = "chkCancelados", CssClass = "rede-input", Text = "cancelados" };
            this.chkCancelados.Attributes.Add("onclick", "ConsultaPV.chkCancelados_onclick(this);");
            span.Controls.Add(this.chkCancelados);
            pnlSearchFilterChecks.Controls.Add(span);

            pnlSearchFilter.Controls.Add(pnlSearchFilterChecks);

            this.pnlLightBoxPvs.Controls.Add(pnlSearchFilter);

            #endregion

            #region Listagem dos PVs

            this.pnlListaPV = new Panel()
            {
                ID = "listaPV",
                CssClass = "listaPV"
            };

            this.pnlLightBoxPvs.Controls.Add(this.pnlListaPV);

            #endregion

            #region Mensagem de validação

            // span de mensagem para validação referente à seleção dos PVs
            div = new HtmlGenericControl("div");
            div.Attributes.Add("class", "mensagem-validacao-pvs validation-message");

            span = new HtmlGenericControl("span") { ID = "lblValidacao" };
            span.Attributes.Add("class", "validation-message");

            div.Controls.Add(span);
            this.pnlLightBoxPvs.Controls.Add(div);

            #endregion

            #region Submit selecionar

            div = new HtmlGenericControl("div");
            div.Attributes.Add("class", "modal-rede-lightbox-submit right");
            this.btnSelecionar = new Botao
            {
                ID = "btnSelecionar",
                Text = "selecionar",
                UseSubmitBehavior = false,
                BotaoPrimario = true,
                CssClass = "right"
            };
            this.btnSelecionar.OnClientClick = "return ConsultaPV.btnSelecionar_onclick(this);";
            this.btnSelecionar.Click += this.btnSelecionar_Click;
            div.Controls.Add(btnSelecionar);

            this.pnlLightBoxPvs.Controls.Add(div);

            #endregion

            this.Controls.Add(this.pnlLightBoxPvs);

            #endregion
        }

        /// <summary>
        /// Define valores no evento OnLoad do controle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!object.ReferenceEquals(SessaoAtual, null))
            {
                //Serializa o objeto de sessão em JSON, para utilização pelo JS do controle
                var sessao = new
                {
                    CNPJEntidade = SessaoAtual.CNPJEntidade,
                    CodigoEntidade = SessaoAtual.CodigoEntidade,
                    CodigoEntidadeMatriz = SessaoAtual.CodigoEntidadeMatriz,
                    GrupoEntidade = SessaoAtual.GrupoEntidade,
                    NomeEntidade = SessaoAtual.NomeEntidade,
                    NomeEntidadeMatriz = SessaoAtual.NomeEntidadeMatriz,
                    MoedaEntidade = SessaoAtual.TransacionaDolar ? "D" : "R",
                    Chave = CoreSeguranca.Criptografia.CriptografarPV(SessaoAtual.CodigoEntidade, SessaoAtual.CodigoEntidade)
                };

                hdnSessao.Value = this.JsSerializer.Serialize(sessao);
                hdnModo.Value = ModoSelecao.ToString();
                hdnOrigem.Value = Origem.ToString();
                hdnPVObrigatorio.Value = PVObrigatorio.ToString().ToLower();
                this.UseSubmitBehavior &= Click != null;
            }

            //Se modo seleção é Único, oculta checkboxes de Todos/Ativos/Cancelados
            if (ModoSelecao == ConsultaPvModo.Unico)
            {
                this.chkAtivos.Visible = false;
                this.chkCancelados.Visible = false;
                this.chkTodos.Visible = false;
            }

            //Se foi definida uma coleção de opções de Tipos de Entidades, aplica coleção definida na combo
            if (this.TiposEntidade.HasValue)
            {
                foreach (ConsultaPvTipoEntidade tipoEntidade in Enum.GetValues(typeof(ConsultaPvTipoEntidade)))
                {
                    if (tipoEntidade != (tipoEntidade & this.TiposEntidade))
                    {
                        //mapeia enumerador de tipoEntidade para enum de ETipoAssociacao
                        ConsultaPvTipoAssociacao tipoAssociacao = (ConsultaPvTipoAssociacao)tipoEntidade.GetDescription().ToInt32(0);

                        //Desabilita aqueles que não estão na coleção
                        this.ddlTipoAssociacao.Items.Cast<ListItem>().Single(
                            item => item.Value.CompareTo(((Int32)tipoAssociacao).ToString()) == 0).Enabled = false;
                    }
                }
            }

            ScriptManager.RegisterStartupScript(this, Page.GetType(), "consultaPV_init_" + this.ClientID,
                "try { " +
                "   var " + this.ID + " = ConsultaPV.get('#" + this.ClientID + "'); " +
                "   var " + this.ClientID + " = ConsultaPV.get('#" + this.ClientID + "'); " +
                    this.ClientID + ".alterouTipoAssociacao(false, false); " +
                "} catch(e) { }", true);
        }

        /// <summary>
        /// Inicializa a configuração do controle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.Attributes.Add("consultapv", "true");

            // registra scripts necessários
            string jsPath = "/sites/fechado/Style%20Library/pt-br/Redecard/JScript/redeareafechada-consulta-pv.js?versao={0}";
            ScriptManager.RegisterClientScriptInclude(
                this, this.GetType(), "redeareafechada-consulta-pv",
                String.Format(jsPath, WebConfigurationManager.AppSettings["Versao"]));
        }

        /// <summary>
        /// Evento de clique do botão selecionar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSelecionar_Click(object sender, EventArgs e)
        {
            if (this.Click != null)
                this.Click(sender, e);
        }

        #endregion
    }
}
