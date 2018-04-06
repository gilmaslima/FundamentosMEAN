using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Microsoft.SharePoint;
using System.Collections.Generic;
using Microsoft.SharePoint.Portal.WebControls;
using Microsoft.SharePoint.WebControls;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Utilities;
using System.IO;
using System.Web;
using Redecard.PN.DadosCadastrais.SharePoint.MaximoServico;
using Microsoft.SharePoint.Administration;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    public partial class SuporteTerminalMotivo : UserControlBase
    {
        #region [ Controles ]

        /// <summary>
        /// Controle Pai
        /// </summary>
        public SuporteTerminal rcSuporteTerminal
        {
            get 
            { 
                Control parent = this.Parent;
                while (parent != null && !(parent is SuporteTerminal))
                    parent = parent.Parent;
                return parent as SuporteTerminal;
            }
        }

        /// <summary>
        /// Cabeçalho contendo informações de identificação do terminal
        /// </summary>        
        protected SuporteTerminalHeader rcHeader;
        
        #endregion

        #region [ Listas ]

        /// <summary>Lista de Motivos</summary>
        private static SPList ListaMotivos { get { return SPContext.Current.Web.Lists.TryGetList("Suporte à Terminais - Motivos"); } }

        /// <summary>Lista de Passos</summary>
        private static SPList ListaPassos { get { return SPContext.Current.Web.Lists.TryGetList("Suporte à Terminais - Passos"); }}

        /// <summary>Lista de Motivos x Passos</summary>
        private static SPList ListaMotivoPassos { get { return SPContext.Current.Web.Lists.TryGetList("Suporte à Terminais - Motivo x Passos"); }}

        /// <summary>Lista de Conteúdos Relacionados</summary>
        private static SPList ListaConteudosRelacionados { get { return SPContext.Current.Web.Lists.TryGetList("Suporte à Terminais - Conteúdos Relacionados"); }}        

        #endregion

        #region [ Propriedades ]

        /// <summary>
        /// Dicionário para armazenar na Sessão se o acesso do usuário já foi
        /// contabilizado para o motivo acessado.
        /// Chave: ID do Motivo (SPListItem.ID)
        /// Valor: TRUE (já contabilizado); FALSE (ainda não contabilizado)
        /// </summary>
        private Dictionary<Int32, Boolean> ContabilizacaoAcesso
        {
            get 
            {
                String chave = "__SuporteTerminal_ContabilizacaoAcesso__";
                if (Session[chave] == null)
                    Session[chave] = new Dictionary<Int32, Boolean>();
                return (Dictionary<Int32, Boolean>)Session[chave]; 
            }
            set 
            {
                String chave = "__SuporteTerminal_ContabilizacaoAcesso__";
                Session[chave] = value; 
            }
        }

        /// <summary>
        /// Dicionário para armazenar na Sessão se a retenção para o usuário já foi
        /// contabilizada para o motivo acessado.
        /// Chave: ID do Motivo (SPListItem.ID)
        /// Valor: TRUE (já contabilizado); FALSE (ainda não contabilizado)
        /// </summary>
        private Dictionary<Int32, Boolean> ContabilizacaoRetencao
        {            
            get
            {
                String chave = "__SuporteTerminal_ContabilizacaoRetencao__";
                if (Session[chave] == null)
                    Session[chave] = new Dictionary<Int32, Boolean>();
                return (Dictionary<Int32, Boolean>)Session[chave];
            }
            set 
            {
                String chave = "__SuporteTerminal_ContabilizacaoRetencao__";
                Session[chave] = value; 
            }
        }

        #endregion

        /// <summary>Carrega os motivos para solicitação de suporte</summary>
        /// <param name="terminal">Terminal</param>
        internal void CarregarDadosMotivos(TerminalDetalhado terminal)
        {
            //Carrega dados do cabeçalho
            rcHeader.CarregarDadosTerminal(terminal);

            //Carrega a combo de motivos
            this.CarregarMotivos();
        }
     
        #region [ Eventos de Controles ]

        /// <summary>Seleção de um motivo</summary>
        protected void ddlMotivo_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlMotivo.Visible = !String.IsNullOrEmpty(ddlMotivo.SelectedValue);

            //Se válido, carrega os detalhes do motivo selecionado
            if (!String.IsNullOrEmpty(ddlMotivo.SelectedValue))
                this.CarregarDetalhesMotivo(ddlMotivo.SelectedValue.ToInt32());             
        }

        /// <summary>Bind dos dados dos passos do conteúdo passo-a-passo</summary>
        protected void rptConteudoPasso_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var imgPasso = e.Item.FindControl("imgPasso") as Image;
                var lblConteudoPasso = e.Item.FindControl("lblConteudoPasso") as Literal;
                var lblOrdemPasso = e.Item.FindControl("lblOrdemPasso") as Literal;
                var item = e.Item.DataItem as Passo;

                imgPasso.ImageUrl = item.ImagemUrl;
                lblConteudoPasso.Text = item.Conteudo;
                lblOrdemPasso.Text = (e.Item.ItemIndex + 1).ToString(); 
            }
        }

        /// <summary>Bind dos dados dos conteúdos relacionados</summary>        
        protected void repConteudoRelacionado_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var item = e.Item.DataItem as MotivoBase;
                var lkbConteudoRelacionado = e.Item.FindControl("lkbConteudoRelacionado") as LinkButton;
                lkbConteudoRelacionado.Text = item.Titulo;
                lkbConteudoRelacionado.CommandArgument = item.ID.ToString();
            }
        }

        /// <summary>Carregamento de um motivo relacionado</summary>        
        protected void lkbConteudoRelacionado_Click(object sender, EventArgs e)
        {
            LinkButton lkbConteudoRelacionado = sender as LinkButton;
            Int32 idMotivo = lkbConteudoRelacionado.CommandArgument.ToInt32(0);
            if(ddlMotivo.SelectedItem != null)
                ddlMotivo.SelectedItem.Selected = false;
            var item = ddlMotivo.Items.FindByValue(idMotivo.ToString());
            if (item != null)
            {
                item.Selected = true;
                this.CarregarDetalhesMotivo(idMotivo);
            }
        }

        /// <summary>Prossegue para o Troca de Terminal</summary>
        protected void btnTrocaTerminal_Click(object sender, EventArgs e)
        {
            Int32 idMotivo = ddlMotivo.SelectedValue.ToInt32();
            String descricaoMotivo = ddlMotivo.SelectedItem.Text;
            rcSuporteTerminal.CarregarTrocaTerminal(idMotivo, descricaoMotivo);
        }

        /// <summary>Prossegue para o passo de Atendimento por E-mail</summary>        
        protected void btnAtendimentoEmail_Click(object sender, EventArgs e)
        {
            String descricaoMotivo = ddlMotivo.SelectedItem.Text;
            rcSuporteTerminal.CarregarAtendimentoEmail(descricaoMotivo);
        }

        /// <summary>Problema resolvido sim/não</summary>        
        protected void btnProblemaResolvidoSim_Click(object sender, EventArgs e)
        {
            Int32 idMotivo = ddlMotivo.SelectedValue.ToInt32();
            
            //Contabiliza a retenção do usuário (problema resolvido) - apenas 1 por sessão por motivo
            ContabilizarRetencao(idMotivo);

            Response.Redirect(Request.RawUrl, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>Carrega todos os motivos na combo de motivos</summary>
        private void CarregarMotivos()
        {
            SPQuery query = new SPQuery();
            query.Query = String.Concat(
                "<Where><Eq><FieldRef Name=\"_ModerationStatus\" /><Value Type=\"ModStat\">0</Value></Eq></Where>",
                "<OrderBy Override=\"True\"><FieldRef Name=\"Ordem\" /></OrderBy>");
            ddlMotivo.DataSource = ListaMotivos.GetItems(query)
                .Cast<SPListItem>().Select(listItem => new
                {
                    ID = listItem.ID,
                    Descricao = listItem.Title
                }).ToArray();
            ddlMotivo.DataBind();
        }

        /// <summary>
        /// Carrega os detalhes de um motivo
        /// </summary>
        /// <param name="idMotivo">ID do motivo</param>
        private void CarregarDetalhesMotivo(Int32 idMotivo)
        {
            using (Logger Log = Logger.IniciarLog(String.Concat("Carrega Detalhamento do Motivo ", idMotivo)))
            {
                try
                {
                    SPListItem spItemMotivo = ListaMotivos.GetItemById(idMotivo);

                    if (spItemMotivo != null)
                    {
                        //Recupera configurações do motivo
                        Motivo motivo = ObterMotivo(spItemMotivo);

                        //Exibe dados do motivo conforme parametrização
                        lblTituloMotivo.Text = motivo.Titulo;

                        //Exibe procedimento caso TipoMotivo == "Com procedimento"                    
                        if (motivo.TipoMotivo == TipoMotivo.ComProcedimento)
                        {
                            //Preenche descrição do motivo
                            pnlDescricaoMotivo.Visible = !String.IsNullOrEmpty(motivo.Descricao);
                            lblDescricaoMotivo.Text = motivo.Descricao;

                            //Monta conteúdo do procedimento                    
                            rptConteudoPasso.DataSource = motivo.Passos;
                            rptConteudoPasso.DataBind();

                            //Se possui procedimento, oculta botões de atendimento por CSS,
                            //pois só são exibidos após usuário clicar em "Problema Não Resolvido"
                            pnlBotoesAtendimento.Style.Add("display", "none");
                            pnlProcedimento.Visible = true;
                        }
                        else
                        {
                            //Botões de atendimento já vêm visíveis
                            pnlBotoesAtendimento.Style.Remove("display");
                            pnlProcedimento.Visible = false;
                        }

                        //Configura exibição dos Botões de Atendimento                    
                        switch (motivo.BotoesAtendimento)
                        {
                            case BotoesAtendimento.SomentePorEmail:
                                pnlBotaoAtendimentoEmail.Visible = true;
                                pnlBotaoAtendimentoTroca.Visible = false;
                                break;
                            case BotoesAtendimento.SomentePorAberturaOS:
                                pnlBotaoAtendimentoEmail.Visible = false;
                                pnlBotaoAtendimentoTroca.Visible = true;
                                break;
                            case BotoesAtendimento.PorEmailEAberturaOS:
                                pnlBotaoAtendimentoEmail.Visible = true;
                                pnlBotaoAtendimentoTroca.Visible = true;
                                break;
                            default:
                                pnlBotaoAtendimentoTroca.Visible = false;
                                pnlBotaoAtendimentoEmail.Visible = false;
                                break;
                        }

                        //Exibe conteúdos relacionados              
                        pnlRelacionados.Visible = motivo.MotivosRelacionados.Count > 0;
                        repConteudoRelacionado.DataSource = motivo.MotivosRelacionados;
                        repConteudoRelacionado.DataBind();

                        //Associa o controle de Rating ao motivo selecionado
                        this.ConfigurarControleRating(spItemMotivo);

                        //Contabiliza o acesso do usuário no motivo (apenas 1 vez por sessão)
                        ContabilizarAcesso(idMotivo);
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Configura o controle de rating para determinado item da Lista de Motivos
        /// </summary>
        /// <param name="listItem">Item da lista de Motivos</param>
        private void ConfigurarControleRating(SPListItem listItem)
        {
            avgRating.ListId = listItem.ParentList.ID;
            avgRating.ControlMode = SPControlMode.Edit;
            avgRating.ItemContext = SPContext.GetContext(System.Web.HttpContext.Current,
                listItem.ID, listItem.ParentList.ID, listItem.ListItems.List.ParentWeb);
        }

        /// <summary>
        /// Recupera os dados completos do motivo.
        /// </summary>
        /// <param name="spItemMotivo">Motivo (SPListItem)</param>
        /// <returns>Motivo</returns>
        protected static Motivo ObterMotivo(SPListItem spItemMotivo)
        {
            Motivo motivo = null;

            //Recupera configurações básicas do motivo
            if (spItemMotivo != null)
            {
                motivo = new Motivo();
                motivo.ID = spItemMotivo.ID;
                motivo.Titulo = spItemMotivo.Title;
                motivo.Descricao = (String)spItemMotivo["Descricao"];

                String strTipoMotivo = (String)spItemMotivo["TipoMotivo"];
                switch (strTipoMotivo)
                {
                    case "Com procedimento":
                        motivo.TipoMotivo = TipoMotivo.ComProcedimento;
                        break;
                    case "Somente botões de atendimento":
                        motivo.TipoMotivo = TipoMotivo.SomenteBotoesAtendimento;
                        break;
                    default: break;
                }

                String strBotoesAtendimento = (String)spItemMotivo["BotoesAtendimento"];
                switch (strBotoesAtendimento)
                {
                    case "Somente por E-mail":
                        motivo.BotoesAtendimento = BotoesAtendimento.SomentePorEmail;
                        break;
                    case "Somente por Abertura de OS":
                        motivo.BotoesAtendimento = BotoesAtendimento.SomentePorAberturaOS;
                        break;
                    case "E-mail e Abertura de OS":
                        motivo.BotoesAtendimento = BotoesAtendimento.PorEmailEAberturaOS;
                        break;
                    default: break;
                }
            }

            //Recupera os passos do procedimento
            if (motivo.TipoMotivo == TipoMotivo.ComProcedimento)
            {
                //Monta conteúdo do procedimento
                SPQuery queryMotivoPassos = new SPQuery();
                queryMotivoPassos.Query = String.Concat(
                    "<Where><And>",
                    "<Eq><FieldRef Name=\"Motivo\" /><Value Type='Text'>", motivo.Titulo, "</Value></Eq>",
                    "<Eq><FieldRef Name=\"_ModerationStatus\" /><Value Type='ModStat'>0</Value></Eq>",
                    "</And></Where>",
                    "<OrderBy Override=\"True\"><FieldRef Name=\"Ordem\" /></OrderBy>");
                var motivoPassos = ListaMotivoPassos.GetItems(queryMotivoPassos);
                
                foreach (SPListItem motivoPasso in motivoPassos)
                {
                    Int32 passoId = new SPFieldLookupValue(motivoPasso["Passo"].ToString()).LookupId;

                    SPQuery queryPasso = new SPQuery();
                    queryPasso.Query = String.Concat(
                        "<Where><And>",
                        "<Eq><FieldRef Name=\"ID\" /><Value Type='Number'>", passoId, "</Value></Eq>",
                        "<Eq><FieldRef Name=\"_ModerationStatus\" /><Value Type='ModStat'>0</Value></Eq>",
                        "</And></Where>");

                    var spPasso = ListaPassos.GetItems(queryPasso).Cast<SPListItem>().FirstOrDefault();
                    if (spPasso != null)
                    {
                        motivo.Passos.Add(new Passo
                        {
                            Conteudo = (String) spPasso["Conteudo"],
                            Imagem = new SPFieldUrlValue((String)spPasso["ImagemPasso"]).Url
                        });
                    }
                }

                //Recupera os conteúdos relacionados                
                SPQuery queryRelacionados = new SPQuery();
                queryRelacionados.Query = String.Concat(
                        "<Where><And>",
                        "<Eq><FieldRef Name=\"Motivo\" /><Value Type='Text'>" + motivo.Titulo + "</Value></Eq>",
                        "<Eq><FieldRef Name=\"_ModerationStatus\" /><Value Type='ModStat'>0</Value></Eq>",
                        "</And></Where>",
                        "<OrderBy Override=\"True\"><FieldRef Name=\"Ordem\" /></OrderBy>");

                var conteudosRelacionados = ListaConteudosRelacionados.GetItems(queryRelacionados);

                foreach (SPListItem conteudoRelacionado in conteudosRelacionados)
                {
                    Int32 idMotivoRelacionado = new SPFieldLookupValue(conteudoRelacionado["ConteudoRelacionado"].ToString()).LookupId;
                    SPListItem spRelacionado = ListaMotivos.GetItemByIdSelectedFields(idMotivoRelacionado, "Title", "ID");
                    motivo.MotivosRelacionados.Add(new MotivoBase
                    {
                         ID = (Int32) spRelacionado["ID"],
                         Titulo = (String) spRelacionado["Title"]
                    });
                }
            }

            return motivo;
        }

        #endregion

        #region [ Métodos auxiliares para manipulação de listas dos Relatório ]

        /// <summary>
        /// Contabiliza Retenção para o Motivo, para o PV logado
        /// </summary>
        /// <param name="idMotivo">ID do Motivo na lista de Motivos</param>
        private void ContabilizarRetencao(Int32 idMotivo)
        {
            using (Logger Log = Logger.IniciarLog(String.Concat("Contabilização de Retenção - Motivo ", idMotivo)))
            {
                try
                {
                    Boolean retencaoContabilizada = this.ContabilizacaoRetencao.GetValueOrDefault(idMotivo, false);
                    if (!retencaoContabilizada)
                    {
                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                            {
                                using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                                {
                                    String nomeLista = "Suporte à Terminais - Relatório de Retenções";
                                    SPList listaRelatorioRetencao = web.Lists.TryGetList(nomeLista);

                                    web.AllowUnsafeUpdates = true;

                                    SPListItem item = listaRelatorioRetencao.Items.Add();
                                    item["Procedimento"] = new SPFieldLookupValue(idMotivo, "Motivo");
                                    item["PV"] = SessaoAtual.CodigoEntidade;
                                    item.Update();
                                }
                            }
                        });

                        this.ContabilizacaoRetencao[idMotivo] = true;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Contabiliza Acesso para o Motivo, para o PV logado
        /// </summary>
        /// <param name="idMotivo">ID do Motivo na lista de motivos</param>
        private void ContabilizarAcesso(Int32 idMotivo)
        {
            using (Logger Log = Logger.IniciarLog(String.Concat("Contabilização de Acesso - Motivo ", idMotivo)))
            {
                try
                {
                    Boolean acessoContabilizado = this.ContabilizacaoAcesso.GetValueOrDefault(idMotivo, false);
                    if (!acessoContabilizado)
                    {
                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                            {
                                using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                                {
                                    String nomeLista = "Suporte à Terminais - Relatório de Conteúdos Mais Acessados";
                                    SPList listaRelatorioAcesso = web.Lists.TryGetList(nomeLista);

                                    web.AllowUnsafeUpdates = true;

                                    SPListItem item = listaRelatorioAcesso.Items.Add();
                                    item["Procedimento"] = new SPFieldLookupValue(idMotivo, "Motivo");
                                    item["PV"] = SessaoAtual.CodigoEntidade;
                                    item.Update();
                                }
                            }
                        });

                        this.ContabilizacaoAcesso[idMotivo] = true;
                    }
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion

        #region [ Inner Classes auxiliares ]

        /// <summary>Motivo</summary>
        protected class MotivoBase
        {
            /// <summary>ID do motivo</summary>
            public Int32 ID { get; set; }
            /// <summary>Nome/Título do motivo</summary>
            public String Titulo { get; set; }            
        }

        /// <summary>Motivo (com detalhes do motivo)</summary>
        protected class Motivo : MotivoBase
        {            
            /// <summary>Subtítulo do motivo</summary>
            public String Descricao { get; set; }
            /// <summary>Tipo do motivo</summary>
            public TipoMotivo TipoMotivo { get; set; }
            /// <summary>Botões de Atendimento</summary>
            public BotoesAtendimento BotoesAtendimento { get; set; }
            /// <summary>Passos do Conteúdo</summary>
            public List<Passo> Passos { get; set; }
            /// <summary>Conteúdos Relacionados</summary>
            public List<MotivoBase> MotivosRelacionados { get; set; }

            /// <summary>Construtor</summary>
            public Motivo()
            {
                this.Passos = new List<Passo>();
                this.MotivosRelacionados = new List<MotivoBase>();
            }
        }

        /// <summary>Tipo do Motivo</summary>
        protected enum TipoMotivo
        {            
            /// <summary>Com procedimento passo-a-passo</summary>
            ComProcedimento,
            /// <summary>Apenas botões de atendimento</summary>
            SomenteBotoesAtendimento
        }

        /// <summary>Visibilidade dos Botões de Atendimento</summary>
        protected enum BotoesAtendimento
        {
            /// <summary>Apenas botão Atendimento Por E-mail</summary>
            SomentePorEmail,
            /// <summary>Apenas botão Solicitar Troca de Terminal</summary>            
            SomentePorAberturaOS,
            /// <summary>Botões Atendimento Por E-mail e Solicitar Troca de Terminal</summary>
            PorEmailEAberturaOS
        }

        /// <summary>Passo do Conteúdo</summary>
        protected class Passo
        {
            /// <summary>URL da Imagem do Passo</summary>
            public String ImagemUrl 
            { 
                get 
                {
                    var zone = SPAlternateUrl.Lookup(SPAlternateUrl.ContextUri).UrlZone;
                    var translateUri = SPFarm.Local.AlternateUrlCollections.RebaseUriWithAlternateUri(new Uri(this.Imagem), zone);
                    return translateUri.AbsoluteUri;
                } 
            }

            /// <summary>URL da Imagem do Passo</summary>
            public String Imagem { set; get; }

            /// <summary>Conteúdo HTML do Passo</summary>
            public String Conteudo { get; set; }
        }
        
        #endregion
    }
}