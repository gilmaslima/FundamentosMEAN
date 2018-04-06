using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Security.Permissions;
using System.Web.UI.HtmlControls;

[assembly: TagPrefix("Redecard.PN.Comum", "Redecard")]
namespace Redecard.PN.Comum
{
    /// <summary>WebControl que adiciona paginação a um Repeater ou GridView.</summary>                
    /// <remarks>
    /// Passo 1: Registrar “Redecard.PN.Comum” na página .ASPX
    /// <code>
    /// &lt;%@ Register Namespace="Redecard.PN.Comum" TagPrefix="Redecard"
    /// Assembly="Redecard.PN.Comum, Version=1.0.0.0, Culture=neutral, PublicKeyToken=427b1f4483a16eb3" %&gt;
    /// </code>
    /// Passo 2: Adicionar o controle “Paginacao” na página .ASPX
    /// <code>
    /// &lt;asp:Repeater runat=”server” ID=”repeater” OnItemDataBound=”repeater_ItemDataBound”&gt;
    /// …
    /// &lt;/asp:Repeater&gt; 
    /// &lt;Redecard:Paginacao ID=” aginação” runat=”server” ControleAssociadoID=”repeater” OnObterDados=” aginação_ObterDados”&gt;
    ///     &lt;Configuracao CarregarAutomaticamente=”false” TamanhoPagina=”10” Exibir=”true” QuantidadePaginasExibidas=”10”&gt;
    ///         &lt;BotaoPrimeiraPagina Exibir=”true” Texto=”&lt;&lt;” /&gt;
    ///         &lt;BotaoPaginasAnteriores Exibir=”true” Texto=”&lt;” /&gt;
    ///         &lt;BotaoProximasPaginas Exibir=”true” Texto=”&gt;” /&gt;
    ///         &lt;BotaoUltimaPagina Exibir=”true” Texto=”&gt;&gt;” /&gt;
    ///     &lt;/Configuracao&gt;
    /// &lt;/Redecard:Paginacao&gt;
    /// </code>
    /// </remarks>
    [ParseChildren(true),
    ToolboxData(@"<{0}:Paginacao runat=""server"" ControleAssociadoID="""" OnObterDados=""""/>"),
    DefaultEvent("ObterDados"),
    DefaultProperty("ControleAssociadoID")]
    public class Paginacao : WebControl
    {
        #region [ Constantes ]
        private const Int32 IndiceUltimaPagina = -1;
        #endregion

        #region [ Classes Internas ]

        /// <summary>Configurações dos botões do controle de paginação.</summary>
        [Serializable]
        public class BotaoPaginacao
        {
            /// <summary>Flag indicando se botão estará visível</summary>
            public Boolean Exibir { get; set; }

            /// <summary>Texto a ser exibido no botão</summary>
            public String Texto { get; set; }

            /// <summary>Construtor.</summary>
            /// <param name="exibir">Se botão estará visível</param>
            /// <param name="texto">Texto a ser exibido no botão</param>
            public BotaoPaginacao(Boolean exibir, String texto)
            {
                this.Exibir = exibir;
                this.Texto = texto;
            }

            /// <summary>
            /// Construtor padrão da classe.
            /// </summary>
            public BotaoPaginacao() { }
        }

        /// <summary>Classe de configuração do controle de paginação.</summary>
        [Serializable]
        public class ConfiguracaoPaginacao
        {
            /// <summary>Quantidade de páginas a serem exibidas no controle de paginação. Valor padrão: 10</summary>
            public Int32 QuantidadePaginasExibidas { get; set; }

            /// <summary>Quantidade de registros por página. Valro padrão: 10</summary>
            public Int32 TamanhoPagina { get; set; }

            /// <summary>Botão "Próximas Páginas". Padrão: Exibir (">")</summary>        
            [PersistenceMode(PersistenceMode.InnerProperty)]
            public BotaoPaginacao BotaoProximasPaginas { get; set; }

            /// <summary>Botão "Páginas Anteriores". Padrão: Exibir ("&lt;")</summary>
            [PersistenceMode(PersistenceMode.InnerProperty)]
            public BotaoPaginacao BotaoPaginasAnteriores { get; set; }

            /// <summary>Botão "Primeira Página". Padrão: Não Exibir ("&lt;&lt;")</summary>
            [PersistenceMode(PersistenceMode.InnerProperty)]
            public BotaoPaginacao BotaoPrimeiraPagina { get; set; }

            /// <summary>Botão "Última Página". Padrão: Não Exibir (">>")</summary>
            [PersistenceMode(PersistenceMode.InnerProperty)]
            public BotaoPaginacao BotaoUltimaPagina { get; set; }
            
            /// <summary>Construtor padrão da classe.</summary>
            public ConfiguracaoPaginacao()
            {
                QuantidadePaginasExibidas = 10;
                TamanhoPagina = 10;
                BotaoProximasPaginas = new BotaoPaginacao(true, ">");
                BotaoPaginasAnteriores = new BotaoPaginacao(true, "<");
                BotaoPrimeiraPagina = new BotaoPaginacao(true, "<<");
                BotaoUltimaPagina = new BotaoPaginacao(true, ">>");                
            }
        }

        #endregion

        #region [ Propriedades ]

        /// <summary>ID do controle GridView/Repeater que será paginado.</summary>        
        public String ControleAssociadoID { get; set; }

        /// <summary>Controle GridView/Repeater que será paginado.</summary>
        public Control ControleAssociado { get { return this.NamingContainer.FindControl(ControleAssociadoID); } }
        private Repeater Repeater { get { return ControleAssociado as Repeater; } }
        private GridView GridView { get { return ControleAssociado as GridView; } }

        /// <summary>Configurações do controle de paginação.</summary>        
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ConfiguracaoPaginacao Configuracao
        {
            get
            {
                if (ViewState["Configuracao"] == null)
                    Configuracao = new ConfiguracaoPaginacao();
                return (ConfiguracaoPaginacao)ViewState["Configuracao"];
            }
            set { ViewState["Configuracao"] = value; }
        }

        /// <summary>Alinhamento horizontal dos controle de paginação.</summary>        
        public HorizontalAlign AlinhamentoHorizontal
        {
            get
            {
                if (ViewState["AlinhamentoHorizontal"] == null)
                    AlinhamentoHorizontal = HorizontalAlign.Center;
                return (HorizontalAlign)ViewState["AlinhamentoHorizontal"];
            }
            set { ViewState["AlinhamentoHorizontal"] = value; }
        }

        /// <summary>Se deve carregar automaticamente os dados no PostBack</summary>
        public Boolean CarregarAutomaticamente 
        {
            get
            {
                if (ViewState["CarregarAutomaticamente"] == null)
                    CarregarAutomaticamente = false;
                return (Boolean)ViewState["CarregarAutomaticamente"];
            }
            set { ViewState["CarregarAutomaticamente"] = value; }
        }

        /// <summary>Exibe controle de paginação.</summary>
        public Boolean Exibir
        {
            get
            {
                if (ViewState["Exibir"] == null)
                    Exibir = true;
                return (Boolean)ViewState["Exibir"];
            }
            set { ViewState["Exibir"] = value; }
        }

        /// <summary>Exibe BlockUI ao trocar de página.</summary>
        public Boolean ExibirBlockUI
        {
            get
            {
                if (ViewState["ExibirBlockUI"] == null)
                    ExibirBlockUI = false;
                return (Boolean)ViewState["ExibirBlockUI"];
            }
            set { ViewState["ExibirBlockUI"] = value; }
        }

        /// <summary>Id da pesquisa associada ao controle de paginação. 
        /// É automaticamente renovado quando o método público Atualizar é chamado.</summary>
        public Guid IdPesquisa
        {
            get
            {
                if (ViewState["IdPesquisa"] == null)
                    IdPesquisa = Guid.NewGuid();
                return (Guid)ViewState["IdPesquisa"];
            }
            set { ViewState["IdPesquisa"] = value; }
        }

        /// <summary>Página atual</summary>
        public Int32 PaginaAtual
        {
            get { return ((Int32?)ViewState["PaginaAtual"]) ?? 0; }
            private set { ViewState["PaginaAtual"] = value; }
        }

        /// <summary>Grupo de página atual.</summary>
        public Int32 GrupoPaginaAtual { get { return PaginaAtual / Configuracao.QuantidadePaginasExibidas; } }

        /// <summary>Quantidade total de páginas</summary>
        private Int32 TotalPaginas { get { return (Int32)Math.Ceiling(1d * this.TotalRegistros / this.Configuracao.TamanhoPagina); } }

        /// <summary>Quantidade total de grupos de página</summary>
        private Int32 TotalGrupoPaginas { get { return (Int32)Math.Ceiling(1d * this.TotalPaginas / this.Configuracao.QuantidadePaginasExibidas); } }

        /// <summary>Quantidade total de registros</summary>
        private Int32 TotalRegistros
        {
            get { return ((Int32?)ViewState["TotalRegistros"]) ?? 0; }
            set { ViewState["TotalRegistros"] = value; }
        }

        /// <summary>Variável para armazenar a coleção de parâmetros repassados no método Atualizar. 
        /// É repassado para o método ObterDados. Devem ser serializáveis.</summary>       
        private Object[] Parametros
        {
            get { return (Object[])ViewState["Parametros"]; }
            set { ViewState["Parametros"] = value; }
        }

        #endregion

        #region [ Eventos Customizados ]

        /// <summary>Definição do handler utilizado para obtenção de dados para o controle.</summary>
        /// <param name="IdPesquisa">Id da pesquisa associada ao controle de paginação.</param>
        /// <param name="registroInicial">Registro inicial (zero-based index).</param>
        /// <param name="quantidadeRegistrosRetornar">Quantidade de registros a serem retornados.</param>
        /// <param name="quantidadeRegistrosPesquisar">Quantidade de registros a serem pesquisados (já solicita a quantidade de registros necessários para o cache do atual grupo de páginas).</param>
        /// <param name="quantidadeTotalRegistrosEmCache">Quantidade total de registros em cache.</param>
        /// <param name="parametros">Parâmetros que serão repassados para o método ObterDados.</param>
        /// <returns>Coleção contendo o resultado da pesquisa</returns>        
        public delegate IEnumerable<Object> ObterDadosEventHandler(Guid IdPesquisa, Int32 registroInicial, Int32 quantidadeRegistrosRetornar, Int32 quantidadeRegistrosPesquisar, out Int32 quantidadeTotalRegistrosEmCache, params Object[] parametros);

        /// <summary>Evento executado para alimentação dos dados do controle</summary>
        public event ObterDadosEventHandler ObterDados;

        /// <summary>Definição do handler utilizado ao clicar em um botão de Página</summary>
        /// <param name="pagina">Página clicada (zero-based index)</param>
        public delegate void CliquePaginaEventHandler(Int32 pagina);

        /// <summary>Evento executado ao clicar em um Botão de Página.</summary>
        public event CliquePaginaEventHandler PaginaClique;

        #endregion

        #region [ Construtor ]

        /// <summary>
        /// Construtor padrão da classe.
        /// </summary>
        public Paginacao() : base() { }

        #endregion

        #region [ Eventos do Controle e Controles Filhos ]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                //Verifica se todos as propriedades obrigatórias possuem valores válidos
                if (this.ObterDados == null)
                    throw new Exception("ObterDados não está definido.");
                if (this.ControleAssociadoID == null || this.ControleAssociadoID.Trim() == String.Empty)
                    throw new Exception("ControleAssociadoID não está definido.");
                if (this.ControleAssociado == null)
                    throw new Exception("ControleAssociadoID '" + this.ControleAssociadoID + "' não encontrado.");
                if (this.Repeater == null && this.GridView == null)
                    throw new Exception("ControleAssociadoID '" + this.ControleAssociadoID + "' não é suportado. ControleAssociadoID deve ser um Repeater ou GridView.");

                //Carrega os dados
                if (this.CarregarAutomaticamente)
                    this.Carregar();
            }
            else
            {
                ////Cria o controle de paginação no footer do controle            
                this.CriarRodapePaginacao();
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Ação dos botões de paginação
        /// </summary>
        /// <param name="sender">Botão responsável pelo disparo do evento</param>
        /// <param name="e">EventArgs do evento</param>
        protected void lnkPagina_Click(object sender, EventArgs e)
        {
            //Obtém o índice da página selecionada
            Int32 indicePagina = Convert.ToInt32((sender as LinkButton).CommandArgument);
            //Verifica se é última página
            Boolean ehUltimaPagina = indicePagina == IndiceUltimaPagina;
            //Calcula o índice do grupo da página selecionada
            Int32 indiceGrupoPagina = indicePagina / this.Configuracao.QuantidadePaginasExibidas;
            //Calcula o registro inicial
            Int32 registroInicial = indicePagina * this.Configuracao.TamanhoPagina;
            //Variável auxiliar que irá receber a quantidade total de registros em cache
            Int32 quantidadeTotalRegistros = 0;

            //Invoca o evento responsável por trazer os dados para o controle
            //Solicita todos os registros necessários para a montagem da paginação do grupo de páginas exibidas
            Int32 qtdRegistrosCache = 0;
            IEnumerable<Object> dados = null;
            if (ehUltimaPagina)
            {
                //Faz uma primeira chamada apenas para colocar todos os dados no cache e saber o total de registros
                this.ObterDados(this.IdPesquisa, 0, 0, Int32.MaxValue, out quantidadeTotalRegistros);

                //Calcula o índice da última página
                indicePagina = (Int32)Math.Ceiling(1d * quantidadeTotalRegistros / this.Configuracao.TamanhoPagina) - 1;

                //Calcula o registro inicial da última página
                registroInicial = indicePagina * this.Configuracao.TamanhoPagina;

                //Invoca evento de clique de página, se definido
                if (PaginaClique != null)
                    PaginaClique(indicePagina);

                //realiza a busca dos dados da última página
                dados = this.ObterDados(
                    this.IdPesquisa,
                    registroInicial,
                    this.Configuracao.TamanhoPagina,
                    this.Configuracao.TamanhoPagina,
                    out quantidadeTotalRegistros,
                    Parametros);
            }
            else
            {
                //Calcula quantos registros são necessários em cache para saber quantas páginas deve exibir
                qtdRegistrosCache = 1 + (this.Configuracao.QuantidadePaginasExibidas * this.Configuracao.TamanhoPagina * (1 + indiceGrupoPagina)) - registroInicial;

                //Invoca evento de clique de página, se definido
                if (PaginaClique != null)
                    PaginaClique(indicePagina);

                //realiza a busca dos dados da página
                dados = this.ObterDados(
                    this.IdPesquisa,
                    registroInicial,
                    this.Configuracao.TamanhoPagina,
                    qtdRegistrosCache > 0 ? qtdRegistrosCache : Int32.MaxValue,
                    out quantidadeTotalRegistros,
                    Parametros);
            }

            //Bind dos dados, atualiza variáveis do controle de paginação, e monta paginação
            this.DataBind(dados);
            this.PaginaAtual = indicePagina;
            this.TotalRegistros = quantidadeTotalRegistros;
            this.CriarRodapePaginacao();
        }

        #endregion

        #region [ Criação do controle de paginação ]

        /// <summary>Criação do controle de paginação no footer do controle</summary>
        private void CriarRodapePaginacao()
        {
            //Se não deve exibir paginação, oculta
            if (!this.Exibir)
                return;

            //Verifica necessidade de criação de paginação: se possui 1 página não precisa de controle de paginação
            if (this.TotalPaginas <= 1)
                return;

            //Cria o Item referente ao item de paginação do Repeater/GridView            
            Control itemRodape = null;
            TableRow linhaPaginas = new TableRow();
            if (this.Repeater != null)
            {
                itemRodape = this.Repeater.Controls.Cast<Control>().Last().FindControl("Paginacao");
                if (itemRodape == null)
                {
                    Table tabelaPaginasRoot = new Table()
                    {
                        CssClass = "paginacao",
                        CellPadding = 0,
                        CellSpacing = 0
                    };
                    itemRodape = new TableRow();
                    itemRodape.ID = "Paginacao";
                    tabelaPaginasRoot.Rows.Add((TableRow)itemRodape);

                    //Cria a estrutura do container que será adicionado ao footer e conterá os controles de paginação
                    TableCell celulaRodape = new TableCell();
                    ((TableRow)itemRodape).Cells.Add(celulaRodape);
                    celulaRodape.ColumnSpan = 99;

                    Table tabelaPaginas = new Table();
                    tabelaPaginas.HorizontalAlign = AlinhamentoHorizontal;
                    tabelaPaginas.Attributes.CssStyle.Add("margin", "0 auto");
                    celulaRodape.Controls.Add(tabelaPaginas);

                    tabelaPaginas.Controls.Add(linhaPaginas);

                    this.Repeater.Controls.Cast<Control>().Last().Controls.Add(tabelaPaginasRoot);
                }
                else
                    itemRodape.Controls.Clear();
            }
            else if (this.GridView != null)
            {
                itemRodape = this.GridView.Controls.Cast<Control>().Last().FindControl("Paginacao");
                if (itemRodape == null)
                {
                    itemRodape = new GridViewRow(0, 0, DataControlRowType.Pager, DataControlRowState.Normal);
                    itemRodape.ID = "Paginacao";
                    this.GridView.Controls.Cast<Control>().Last().Controls.Add(itemRodape);
                }
                else
                    itemRodape.Controls.Clear();
            }

            //Adiciona o atalho para o primeiro registro
            if (this.Configuracao.BotaoPrimeiraPagina.Exibir && this.GrupoPaginaAtual > 0)
                linhaPaginas.Cells.Add(CriarBotaoPrimeiraPagina());

            //Adiciona o atalho para o grupo de registros anterior
            if (this.Configuracao.BotaoProximasPaginas.Exibir && this.GrupoPaginaAtual > 0)
                linhaPaginas.Cells.Add(CriarBotaoPaginasAnteriores());

            //Adiciona páginas
            for (int iPage = this.GrupoPaginaAtual * this.Configuracao.QuantidadePaginasExibidas;
                iPage < this.TotalPaginas && iPage < this.Configuracao.QuantidadePaginasExibidas * (1 + this.GrupoPaginaAtual); iPage++)
                linhaPaginas.Cells.Add(CriarBotoesPaginas(iPage));

            //Adiciona o atalho para o próximo grupo de registros
            if (this.Configuracao.BotaoProximasPaginas.Exibir && this.GrupoPaginaAtual < this.TotalGrupoPaginas - 1)
                linhaPaginas.Cells.Add(CriarBotaoProximasPaginas());

            //Adiciona o atalho para o último registro
            if (this.Configuracao.BotaoUltimaPagina.Exibir && this.GrupoPaginaAtual < this.TotalGrupoPaginas - 1)
                linhaPaginas.Cells.Add(CriarBotaoUltimaPagina());
        }

        /// <summary>Cria o botão de atalho para a primeira página</summary>        
        private TableCell CriarBotaoPrimeiraPagina()
        {
            TableCell celulaPagina = new TableCell();
            LinkButton lnkPagina = new LinkButton();
            lnkPagina.Text = this.Configuracao.BotaoPrimeiraPagina.Texto;
            lnkPagina.CommandArgument = "0";
            lnkPagina.ID = "Pagina_0";
            lnkPagina.Click += new EventHandler(lnkPagina_Click);
            if(this.ExibirBlockUI)
                lnkPagina.OnClientClick = "if(blockUI) blockUI();";
            celulaPagina.Controls.Add(lnkPagina);
            return celulaPagina;
        }

        /// <summary>Cria o botão de atalho para o grupo de páginas anterior</summary>        
        private TableCell CriarBotaoPaginasAnteriores()
        {
            TableCell celulaPagina = new TableCell();
            LinkButton lnkPagina = new LinkButton();
            lnkPagina.Text = this.Configuracao.BotaoPaginasAnteriores.Texto;
            lnkPagina.ID = "Pagina_Anteriores";
            lnkPagina.CommandArgument = (this.GrupoPaginaAtual * this.Configuracao.QuantidadePaginasExibidas - 1).ToString();
            lnkPagina.Click += new EventHandler(lnkPagina_Click);
            if (this.ExibirBlockUI)
                lnkPagina.OnClientClick = "if(blockUI) blockUI();";
            celulaPagina.Controls.Add(lnkPagina);
            return celulaPagina;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            return;
            //base.Render(writer);
        }

        /// <summary>Cria o botão de atalho para determinada página</summary>
        /// <param name="iPagina">Índice da página (zero-based index, ou seja, se for a "Página 1", iPage deve ser 0)</param>        
        private TableCell CriarBotoesPaginas(Int32 iPagina)
        {
            TableCell celulaPagina = new TableCell();

            if (iPagina == this.PaginaAtual)
            {
                LinkButton lnkPagina = new LinkButton();
                lnkPagina.Text = (iPagina + 1).ToString();
                lnkPagina.ID = "PaginaSelecionada_" + iPagina;
                lnkPagina.Enabled = false;
                celulaPagina.Controls.Add(lnkPagina);
                //HtmlGenericControl spanPagina = new HtmlGenericControl("span");
                //spanPagina.InnerText = (iPagina + 1).ToString();
                //spanPagina.ID = "PaginaSelecionada_" + iPagina;
                //celulaPagina.Controls.Add(spanPagina);
            }
            else
            {
                LinkButton lnkPagina = new LinkButton();
                lnkPagina.Text = (iPagina + 1).ToString();
                lnkPagina.ID = "Pagina_" + iPagina;
                lnkPagina.CommandArgument = iPagina.ToString();
                lnkPagina.Click += new EventHandler(lnkPagina_Click);
                if (this.ExibirBlockUI)
                    lnkPagina.OnClientClick = "if(blockUI) blockUI();";
                celulaPagina.Controls.Add(lnkPagina);
            }
            return celulaPagina;
        }

        /// <summary>Cria o botão de atalho para o próximo grupo de páginas</summary>        
        private TableCell CriarBotaoProximasPaginas()
        {
            TableCell celulaPagina = new TableCell();
            LinkButton lnkPagina = new LinkButton();
            lnkPagina.Text = this.Configuracao.BotaoProximasPaginas.Texto;
            lnkPagina.ID = "Pagina_Proximas";
            lnkPagina.CommandArgument = ((1 + this.GrupoPaginaAtual) * this.Configuracao.QuantidadePaginasExibidas).ToString();
            lnkPagina.Click += new EventHandler(lnkPagina_Click);
            if (this.ExibirBlockUI)
                lnkPagina.OnClientClick = "if(blockUI) blockUI();";
            celulaPagina.Controls.Add(lnkPagina);
            return celulaPagina;
        }

        /// <summary>Cria o botão de atalho para a última página</summary>        
        private TableCell CriarBotaoUltimaPagina()
        {
            TableCell celulaPagina = new TableCell();
            LinkButton lnkPagina = new LinkButton();
            lnkPagina.Text = this.Configuracao.BotaoUltimaPagina.Texto;
            lnkPagina.CommandArgument = "-1";
            lnkPagina.ID = "Pagina_Ultima";
            lnkPagina.Click += new EventHandler(lnkPagina_Click);
            if (this.ExibirBlockUI)
                lnkPagina.OnClientClick = "if(blockUI) blockUI();";
            celulaPagina.Controls.Add(lnkPagina);
            return celulaPagina;
        }

        #endregion

        #region [ Método Públicos ]

        /// <summary>Atualiza os dados do controle (DataBind).</summary>
        /// <param name="parametros">Parâmetros que serão repassados para o método de obtenção de dados</param>
        public void Carregar(params Object[] parametros)
        {
            //Renovação do IdPesquisa
            this.IdPesquisa = Guid.NewGuid();
            Atualizar(true, 0, parametros);
        }

        /// <summary>Atualiza os dados do controle (DataBind), carregando a página informada.</summary>
        /// <param name="pagina">Página a ser carregada.</param>
        /// <param name="parametros">Parâmetros que serão repassados para o método de obtenção de dados. Devem ser serializáveis</param>
        public void CarregarPagina(Guid IdPesquisa, Int32 pagina, params Object[] parametros)
        {
            //Renovação do IdPesquisa
            this.IdPesquisa = IdPesquisa;
            Atualizar(true, pagina, parametros);
        }

        #endregion

        #region [ Métodos Privados / Auxiliares ]

        /// <summary>Atualiza os dados do controle (DataBind)</summary>
        /// <param name="criarRodapePaginacao">Flag indicando se deve criar os controles de paginação</param>
        /// <param name="pagina">Página a ser carregada</param>
        /// <param name="parametros">Parâmetros que serão repassados para o método de obtenção de dados. Devem ser serializáveis</param>
        private void Atualizar(Boolean criarRodapePaginacao, Int32 pagina, params Object[] parametros)
        {
            Int32 qtdTotalRegistros;
            this.PaginaAtual = pagina;
            Int32 qtdRegistrosRetornar = this.Configuracao.QuantidadePaginasExibidas * this.Configuracao.TamanhoPagina + 1;

            //Busca os dados da página solicitada
            var dados = this.ObterDados(
                this.IdPesquisa,
                pagina * this.Configuracao.TamanhoPagina,
                this.Configuracao.TamanhoPagina,
                qtdRegistrosRetornar > 0 ? qtdRegistrosRetornar : Int32.MaxValue,
                out qtdTotalRegistros,
                parametros);

            //Bind dos dados
            DataBind(dados);
            this.TotalRegistros = qtdTotalRegistros;

            //Armazena os parâmetros para as próximas consultas            
            Parametros = parametros;

            //Recria os controles de paginação
            if (criarRodapePaginacao)
                this.CriarRodapePaginacao();
        }

        /// <summary>Bind dos dados</summary>
        /// <param name="dados">Dados a serem carregados</param>
        private void DataBind(Object dados)
        {
            if (this.Repeater != null)
            {
                this.Repeater.DataSource = dados;
                this.Repeater.DataBind();
            }
            else if (this.GridView != null)
            {
                this.GridView.DataSource = dados;
                this.GridView.DataBind();
            }
        }

        #endregion
    }
}
