#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [23/05/2012] – [André Garcia] – [Criação]
*/
#endregion


using System;
using System.Web.UI;
using Redecard.PN.Comum;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login
{

    /// <summary>
    /// 
    /// </summary>
    public partial class Menu : UserControl
    {

        /// <summary>
        /// Carregamento do controle, recuperar os items de menu da sessão e
        /// apresentar na tela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CarregarItemsMenu();
            // Verificar se é a página atual
            if (Request.Url.AbsolutePath.Contains(homeLink.HRef))
            {
                homeLink.Attributes.Add("class", "menuareafechadaactive");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CarregarItemsMenu()
        {
            if (Sessao.Contem())
            {
                Sessao sessao = Sessao.Obtem();

                // PV não cancelado
                if (!sessao.StatusPVCancelado())
                {
                    List<Comum.Menu> menu = new List<Comum.Menu>();
                    List<Comum.Menu> menuSessao = sessao.Menu;

                    // PV que possui Komerci não poderá ter E-COMMERCE(DataCash)
                    if (sessao.PossuiKomerci)
                    {
                        foreach (Comum.Menu _menu in menuSessao)
                        {
                            if (!_menu.Texto.ToLowerInvariant().Trim().Equals("e-commerce"))
                                menu.Add(_menu);
                        }
                        rptRootMenu.DataSource = menu;
                        rptRootMenu.DataBind();
                    }
                    // PV que possui E-COMMERCE(DataCash) não poderá ter Komerci
                    else if (sessao.PossuiDataCash)
                    {

                        foreach (Comum.Menu _menu in menuSessao)
                        {
                            if (!_menu.Texto.ToLowerInvariant().Trim().Equals("komerci"))
                                menu.Add(_menu);
                        }
                        rptRootMenu.DataSource = menu;
                        rptRootMenu.DataBind();
                    }
                    // Caso o PV não tenha Komerci ou E-COMMERCE(DataCash)
                    else
                    {
                        foreach (Comum.Menu _menu in menuSessao)
                        {
                            if (!_menu.Texto.ToLowerInvariant().Trim().Equals("komerci") ||
                                !_menu.Texto.ToLowerInvariant().Trim().Equals("e-commerce"))
                                menu.Add(_menu);
                        }
                        rptRootMenu.DataSource = menu;
                        rptRootMenu.DataBind();
                    }
                }
                // PV Cancelado
                else
                {
                    List<Comum.Menu> menu = new List<Comum.Menu>();
                    foreach (Comum.Menu _menu in sessao.Menu)
                    {
                        if (_menu.Texto.ToLowerInvariant().Trim().Equals("extrato"))
                            menu.Add(_menu);
                        if (_menu.Texto.ToLowerInvariant().Trim().Equals("minha conta") || _menu.Texto.ToLowerInvariant().Trim().Equals("dados cadastrais"))
                            menu.Add(_menu);
                    }
                    rptRootMenu.DataSource = menu;
                    rptRootMenu.DataBind();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RootItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Comum.Menu dataItem = e.Item.DataItem as Comum.Menu;
                Comum.Pagina pagina = (dataItem.Paginas.Count > 0 ? dataItem.Paginas[0] : null);

                // verificar se o item atual contém uma URL e se é o menu atual de navegação
                if (!object.ReferenceEquals(pagina, null) && !String.IsNullOrEmpty(pagina.Url))
                {
                    HtmlAnchor rootLinkMenu = e.Item.FindControl("rootLinkMenu") as HtmlAnchor;
                    rootLinkMenu.Attributes.Add("href", pagina.Url);
                    if (Request.Url.AbsolutePath.Contains(pagina.Url))
                    {
                        rootLinkMenu.Attributes.Add("class", "menuareafechadaactive");
                    }
                }

                // Verificar se é o último item da lista de items
                List<Comum.Menu> parentItems = ((Repeater)sender).DataSource as List<Comum.Menu>;
                if ((e.Item.ItemIndex + 1) == parentItems.Count)
                {
                    HtmlGenericControl rootItemMenu = e.Item.FindControl("rootItemMenu") as HtmlGenericControl;
                    rootItemMenu.Attributes.Add("class", "last");
                }

                // verificar se o item de menu pai contém filhos, recuperar o repeater e executar um
                // bind com o repeater
                if (dataItem.Items.Count > 0)
                {
                    Repeater rptChildMenu = e.Item.FindControl("rptChildMenu") as Repeater;
                    if (!object.ReferenceEquals(rptChildMenu, null))
                    {
                        rptChildMenu.DataSource = dataItem.Items;
                        rptChildMenu.DataBind();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ChildItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Comum.Menu dataItem = e.Item.DataItem as Comum.Menu;
                Comum.Pagina pagina = (dataItem.Paginas.Count > 0 ? dataItem.Paginas[0] : null);

                // verificar se o item atual contém uma URL
                if (!object.ReferenceEquals(pagina, null) && !String.IsNullOrEmpty(pagina.Url))
                {
                    String urlLocation = String.Empty;
                    urlLocation = this.ExtrairUrlPartes(pagina.Url);

                    HtmlAnchor childLinkMenu = e.Item.FindControl("childLinkMenu") as HtmlAnchor;
                    childLinkMenu.Attributes.Add("href", pagina.Url);
                    if (Request.Url.AbsolutePath.Contains(urlLocation))
                    {
                        childLinkMenu.Attributes.Add("class", "menuareafechadaactive");
                    }
                }

                // Verificar se é o último item da lista de items
                List<Comum.Menu> parentItems = ((Repeater)sender).DataSource as List<Comum.Menu>;
                if ((e.Item.ItemIndex + 1) == parentItems.Count)
                {
                    HtmlGenericControl childItemMenu = e.Item.FindControl("childItemMenu") as HtmlGenericControl;
                    childItemMenu.Attributes.Add("class", "last");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private String ExtrairUrlPartes(String url)
        {
            String urlLocation = String.Empty;
            String[] urlParts = url.Split('/');
            for (int i = 0; i < (urlParts.Length - 1); i++)
            {
                urlLocation = urlLocation + urlParts[i] + "/";
            }
            return urlLocation;
        }
    }
}
