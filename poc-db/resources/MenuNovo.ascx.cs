using Redecard.PN.DadosCadastrais.SharePoint.Providers.SiteMap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login
{
    /// <summary>
    /// Controle para renderização do novo menu do portal
    /// </summary>
    [ParseChildren(true, "CustomMenu")]
    public partial class MenuNovo : UserControl
    {
        #region [ Controles e propriedades privadas ]

        /// <summary>
        /// Classes CSS para cada nível do menu
        /// </summary>
        private Dictionary<Int32, String> dicClassMenuLevel = new Dictionary<Int32, String>()
        {
            { 1, "first-lvl" },
            { 2, "second-lvl" },
            { 3, "third-lvl" },
        };

        /// <summary>
        /// Menus adicionais
        /// </summary>
        [Browsable(true),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Control CustomMenu { get; set; }

        #endregion

        /// <summary>
        /// Carregamento do controle: recupera os items de menu da sessão e apresentar na tela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.EnsureChildControls();

            PNSiteMapProvider provider = new PNSiteMapProvider();
            SiteMapNode siteMap = provider.BuildSiteMap();

            // elemento parent
            HtmlGenericControl ul = new HtmlGenericControl("ul");

            // itens da sessão
            ul.Attributes["class"] = GetClassMenuLevel(1);
            AddChildNodes(ul, siteMap, 2);

            // itens customizados
            if (this.CustomMenu != null)
                ul.Controls.Add(this.CustomMenu);

            // adiciona o menu ao placeholder
            pchMenuContainer.Controls.Add(ul);
        }

        /// <summary>
        /// Adiciona itens filhos ao menu
        /// </summary>
        /// <param name="root">Item root onde serão inseridos os itens filhos</param>
        /// <param name="node">Node corrente</param>
        /// <param name="level">Identificador de nível do menu</param>
        public void AddChildNodes(HtmlControl root, SiteMapNode node, Int32 level)
        {
            if (!node.HasChildNodes)
                return;

            foreach (SiteMapNode child in node.ChildNodes)
            {
                if (String.Compare(child.Description, "hide-menu-item", true) == 0)
                    continue;

                // exibe apenas itens com um dos critérios:
                // - possui página para acesso
                // - possui ao menos um filho marcado para exibir no menu
                if (String.IsNullOrEmpty(child.Url) &&
                    !(child.ChildNodes.Count > 0 &&
                    child.ChildNodes.Cast<SiteMapNode>().Any(x => Convert.ToBoolean(x["FlagMenu"]))))
                {
                    continue;
                }

                HtmlGenericControl li = new HtmlGenericControl("li");
                HtmlGenericControl a = new HtmlGenericControl("a");
                a.InnerHtml = child.Title;

                if (!String.IsNullOrWhiteSpace(child.Url))
                    a.Attributes["href"] = child.Url;

                li.Controls.Add(a);

                // valida se a lista de menus suporta o próximo nível
                if (child.HasChildNodes && dicClassMenuLevel.Count >= level)
                {
                    HtmlGenericControl ul = new HtmlGenericControl("ul");
                    ul.Attributes["class"] = GetClassMenuLevel(level);
                    AddChildNodes(ul, child, level + 1);
                    li.Controls.Add(ul);
                    li.Attributes["class"] = "has-child-rede";
                }

                root.Controls.Add(li);
            }
        }

        /// <summary>
        /// Obtém a classe CSS segundo o nível do menu
        /// </summary>
        /// <param name="menuLevel">Nível do menu</param>
        /// <returns>Nome da classe CSS a ser empregada</returns>
        private String GetClassMenuLevel(Int32 menuLevel)
        {
            String classCss = String.Empty;
            dicClassMenuLevel.TryGetValue(menuLevel, out classCss);
            return classCss;
        }
    }
}
