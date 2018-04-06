/*
(c) Copyright 2014 Rede S.A.
Autor       : Alexandre Shiroma
Empresa     : Iteris
Histórico   : 03/11/2014 - Criação
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.ZPPlanoContasServico;
using Menu = Redecard.PN.Comum.Menu;

namespace Redecard.PN.Extrato.SharePoint.WebParts.QuadroAtalhos
{
    /// <summary>
    /// Quadro de Atalhos do Extrato
    /// </summary>
    public partial class QuadroAtalhosUserControl : UserControlBase
    {
        #region [ Propriedades / Variáveis ]

        /// <summary>
        /// Título do Quadro de Links
        /// </summary>
        public String TituloQuadro { get; set; }

        #endregion

        #region [ Eventos Página ]

        /// <summary>
        /// Page Load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && Sessao.Contem())
            {
                this.CarregarAtalhos();
            }
        }

        #endregion

        #region [ Métodos ]

        /// <summary>
        /// Carrega os atalhos
        /// </summary>
        private void CarregarAtalhos()
        {
            Menu itemAtual = base.ObterMenuItemAtual();

            if(itemAtual != null)
            {
                var menuConsulta = new List<Menu>();
                var menuRelatorios = new List<Menu>();

                foreach (Menu itemMenu in itemAtual.Items)
                {
                    if (itemMenu.Texto.Contains("Consulta") || itemMenu.Texto.Contains("Gerencie"))
                        menuConsulta.Add(itemMenu);
                    else
                    {
                        //Verifica se existe algum tratamento específico para exibição do Link no grupo de Relatórios
                        Boolean exibirNoMenu = VerificarItemMenuRelatorios(itemMenu);
                        if(exibirNoMenu)
                            menuRelatorios.Add(itemMenu);
                    }
                }

                if (menuRelatorios.Count > 0 || menuConsulta.Count > 0)
                {
                    pnlMenu.Visible = true;
                    pnlAviso.Visible = false;

                    spnTituloRelatorio.Visible = (menuRelatorios.Count > 0);
                    // carregar boxes filhos
                    repLinks.DataSource = menuRelatorios;
                    repLinks.DataBind();

                    spnTituloConsultas.Visible = (menuConsulta.Count > 0);
                    // carregar Consultas
                    repLinksConsulta.DataSource = menuConsulta;
                    repLinksConsulta.DataBind();
                }
                else
                {
                    pnlMenu.Visible = false;
                    pnlAviso.Visible = true;
                }
            }
        }

        /// <summary>
        /// Item DataBound de links dos relatórios
        /// </summary>
        protected void repLinks_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Menu menu = e.Item.DataItem as Menu;
                HyperLink hlkLink = e.Item.FindControl("hlkLink") as HyperLink;
                if(hlkLink != null && menu != null)
                {
                    if (menu.Paginas != null && menu.Paginas.Count > 0)
                        hlkLink.NavigateUrl = menu.Paginas[0].Url;
                    else
                        hlkLink.NavigateUrl = "#";
                }
            }
        }

        /// <summary>
        /// Item DataBound de links de consulta
        /// </summary>
        protected void repLinksConsulta_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Menu menu = e.Item.DataItem as Menu;
                HyperLink hlkConsultaLink = e.Item.FindControl("hlkConsultaLink") as HyperLink;
                if(hlkConsultaLink != null && menu != null)
                {
                    if (menu.Paginas != null && menu.Paginas.Count > 0)
                        hlkConsultaLink.NavigateUrl = menu.Paginas[0].Url;
                    else
                        hlkConsultaLink.NavigateUrl = "#";
                }
            }
        }

        /// <summary>
        /// Verifica se existe algum tratamento específico para exibição do Link no grupo de Relatórios
        /// </summary>
        /// <param name="itemMenu">Item do Menu de Relatório</param>
        /// <returns>TRUE: exibe link; FALSE: caso contrário</returns>
        private Boolean VerificarItemMenuRelatorios(Menu itemMenu)
        {
            //Se for Valores Consolidados de Vendas (tipo==11), deve verificar se possui oferta Turquia Ativa
            Boolean valoresConsolidadosVendas = itemMenu.Paginas.Any(pagina =>
                pagina.Url.ToLower().Contains("pn_relatorios.aspx") && pagina.Url.ToLower().Contains("tipo=11"));
            if (valoresConsolidadosVendas)
            {
                TipoOferta ofertaAtiva = ConsultarTipoOfertaAtiva();
                return ofertaAtiva == TipoOferta.OfertaTurquia;
            }
                
            //Para os demais menus, exibe
            return true;
        }

        #endregion

        #region

        #region [ Consultas ]

        /// <summary>
        /// Consulta o tipo de oferta Ativa que deve ser exibida para o usuário
        /// Japão, Plano de Contas, Turquia, Sem Oferta, ...
        /// </summary>
        /// <returns>Tipo de oferta ativa</returns>
        private TipoOferta ConsultarTipoOfertaAtiva()
        {
            var codigoRetorno = default(Int16);
            var tipoOferta = default(TipoOferta);
            var numeroPv = SessaoAtual.CodigoEntidade;

            using (Logger log = Logger.IniciarLog("Consulta tipo de oferta ativa"))
            {
                try
                {
                    using (var ctx = new ContextoWCF<HISServicoZP_PlanoContasClient>())
                        codigoRetorno = ctx.Cliente.ConsultarTipoOfertaAtiva(out tipoOferta, numeroPv);
                }
                catch (FaultException<ZPPlanoContasServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

            return codigoRetorno == 0 ? tipoOferta : ZPPlanoContasServico.TipoOferta.SemOferta;
        }

        #endregion

        #endregion
    }
}