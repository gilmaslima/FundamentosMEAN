using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;

namespace Redecard.PN.DadosCadastrais.SharePoint.Providers.SiteMap
{
    /// <summary>
    /// Provider de SiteMap construído para o Portal, usado no breadcrumb e menu
    /// </summary>
    public class PNSiteMapProvider : StaticSiteMapProvider
    {
        private readonly object siteMapLock = new object();
        private SiteMapNode siteMapRoot;
        private Boolean mountNodes;

        /// <summary>
        /// Obtem o nó root.
        /// </summary>
        /// <returns>Retorna o SiteMapNode</returns>
        protected override SiteMapNode GetRootNodeCore()
        {
            return this.BuildSiteMap();
        }

        /// <summary>
        /// Constroi os itens
        /// </summary>
        /// <returns>Retorna o SiteMapNode</returns>
        public override SiteMapNode BuildSiteMap()
        {
            // Construindo o mapa do site
            lock (this.siteMapLock)
            {
                if (this.siteMapRoot != null && mountNodes)
                    return this.siteMapRoot;

                this.mountNodes = true;

                try
                {
                    base.Clear();
                    this.CreateSiteMapNodes();
                }
                catch (ArgumentException ex)
                {
                    SharePointUlsLog.LogErro("Erro ao montar menu provider");
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro("Erro ao montar menu provider");
                    SharePointUlsLog.LogErro(ex);
                }
                this.mountNodes = false;
                return this.siteMapRoot;
            }
        }

        private void CriarSiteMapNode(Menu menu, SiteMapNode nodePai = null)
        {
            if (nodePai == null)
                nodePai = this.siteMapRoot;

            // Necessario para montar os filhos, caso tenha mais de 1 pagina os filhos sempre serao filhos da primeira pagina
            SiteMapNode firstNode = null;

            if (!menu.FlagMenu)
            {
                firstNode = new SiteMapNode(this, ObterCodigoUnico(menu.Codigo), String.Empty, menu.Texto, "hide-menu-item");
                firstNode["FlagMenu"] = menu.FlagMenu.ToString();
                firstNode["FlagFooter"] = menu.FlagFooter.ToString();
                base.AddNode(firstNode, nodePai);
                return;
            }

            if (menu.Paginas.Count > 0 && menu.Paginas.Any(x => x.Navegacao))
            {
                foreach (Comum.Pagina pagina in menu.Paginas)
                {
                    String url = String.Empty;

                    //Existe algumas paginas com o full name da URL, é removido e deixado apenas a virtual path
                    if (pagina != null && !String.IsNullOrWhiteSpace(pagina.Url))
                    {
                        if (pagina.Url.Contains("://"))
                            url = new Uri(pagina.Url).LocalPath;
                        else
                            url = pagina.Url;
                    }

                    // [SKU] - Pode acontecer de ter mais de 1 item com o mesmo código do menu porem para uma pagina diferente.
                    String codigo = ObterCodigoUnico(menu.Codigo);

                    // Caso o item nao seja o primeiro(o código estara diferente) com isso ele nao deve aparecer no menu
                    String descricao = pagina.Navegacao || menu.Paginas.Count == 1 ? String.Empty : "hide-menu-item";

                    SiteMapNode node = new SiteMapNode(this, codigo, url, menu.Texto, descricao);
                    node["FlagMenu"] = menu.FlagMenu.ToString();
                    node["FlagFooter"] = menu.FlagFooter.ToString();

                    base.AddNode(node, nodePai);

                    if (firstNode == null)
                        firstNode = node;
                }
            }
            else
            {
                // [SKU] - Pode acontecer de ter mais de 1 item com o mesmo código porem para uma pagina diferente.
                String codigo = ObterCodigoUnico(menu.Codigo);

                // Caso o item nao seja o primeiro(o código estara diferente) com isso ele nao deve aparecer no menu
                String descricao = String.Compare(codigo, menu.Codigo.ToString(), true) == 0 ? String.Empty : "hide-menu-item";

                firstNode = new SiteMapNode(this, codigo, String.Empty, menu.Texto, descricao);
                firstNode["FlagMenu"] = menu.FlagMenu.ToString();
                firstNode["FlagFooter"] = menu.FlagFooter.ToString();
                base.AddNode(firstNode, nodePai);
            }

            foreach (Menu subMenu in menu.Items)
                CriarSiteMapNode(subMenu, firstNode);

        }

        /// <summary>
        /// Caso ja exista o codigo no provider é devolvido um codigo novo
        /// </summary>
        /// <param name="codigo">Código do menu que vem do serviço</param>
        /// <returns></returns>
        private String ObterCodigoUnico(Int32 codigo)
        {
            String codigoUnico = codigo.ToString();
            SiteMapNode nodeExistente;
            do
            {
                nodeExistente = null;
                nodeExistente = base.FindSiteMapNodeFromKey(codigoUnico);
                if (nodeExistente != null)
                    codigoUnico = String.Format("{0}_1", codigoUnico);
            } while (nodeExistente != null);
            return codigoUnico;
        }

        /// <summary>
        /// Cria todo o sitemap
        /// </summary>
        private void CreateSiteMapNodes()
        {
            // Criando o nó root
            this.siteMapRoot = new SiteMapNode(this, "Home", "/sites/fechado/Paginas/Redireciona.aspx", "Home");
            base.AddNode(this.siteMapRoot);

            List<Menu> menuRede = ObterItensMenu();
            foreach (Menu menu in menuRede)
                CriarSiteMapNode(menu);
        }

        /// <summary>
        /// Obtens os itens do Menu
        /// </summary>
        private List<Menu> ObterItensMenu()
        {
            List<Menu> rootMenu = new List<Menu>();
            if (Sessao.Contem())
            {
                Sessao sessao = Sessao.Obtem();
                rootMenu = sessao.Menu;
            }
            return rootMenu;
        }
    }
}
