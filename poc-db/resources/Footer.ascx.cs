using Comum = Redecard.PN.Comum;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Linq;
using System.Collections.Generic;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login
{
    /// <summary>
    /// Classe para renderização do footer a partir da master page
    /// </summary>
    public partial class Footer : UserControl
    {
        /// <summary>
        /// Constante para definição do maior nível de menu para renderização
        /// </summary>
        private const int MaxLevel = 3;

        /// <summary>
        /// Class para os containers de cada coluna
        /// </summary>
        public String CssClassContainers
        {
            get
            {
                return Convert.ToString(ViewState["CssClassContainers"]);
            }
            set
            {
                ViewState["CssClassContainers"] = value;
            }
        }

        /// <summary>
        /// Renderiza o controle na tela
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (!Comum.Sessao.Contem())
            {
                this.Visible = false;
                return;
            }

            var sessao = Comum.Sessao.Obtem();
            var menu = sessao.Menu.Where(x => x.FlagFooter);

            foreach (var menuItem in menu)
            {
                var pagina = menuItem.Paginas.FirstOrDefault(x => x.Navegacao && !String.IsNullOrWhiteSpace(x.Url));

                // exibe apenas itens com um dos critérios:
                // - possui página para acesso
                // - possui ao menos um filho marcado para exibir no footer
                if (pagina == null && !(menuItem.Items.Count > 0 && menuItem.Items.Any(x => x.FlagFooter)))
                    continue;

                writer.AddAttribute(HtmlTextWriterAttribute.Class, this.CssClassContainers);
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "site-map-item");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    {
                        // menu de nível 1
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "footer-title");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        {
                            // obtém a URL do elemento
                            String url = String.Empty;
                            if (pagina != null)
                                url = pagina.Url;

                            // se o elemento possui URL inclui como atributo "href" do link
                            if (!String.IsNullOrWhiteSpace(url))
                                writer.AddAttribute(HtmlTextWriterAttribute.Href, menuItem.Paginas[0].Url);

                            writer.RenderBeginTag(HtmlTextWriterTag.A);
                            writer.Write(menuItem.Texto);
                            writer.RenderEndTag();
                        }
                        writer.RenderEndTag();

                        // lista para menus de nível 2
                        this.RenderChildMenus(writer, menuItem.Items, "footer-list", 2);
                    }
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();
            }
        }

        /// <summary>
        /// Renderiza os itens filhos de menu
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="menu"></param>
        /// <param name="cssClass"></param>
        /// <param name="nivelLista"></param>
        private void RenderChildMenus(HtmlTextWriter writer, List<Comum.Menu> menu, string cssClass, int nivelLista)
        {
            // verifica se o nível da lista atende aos requisitos
            if (nivelLista > MaxLevel)
                return;

            // valida menus passados por parâmetro
            if (!(menu != null && menu.Count > 0))
                return;

            // adiciona class customizada
            if (!string.IsNullOrEmpty(cssClass))
                writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);

            writer.RenderBeginTag(HtmlTextWriterTag.Ul);
            foreach (var menuItem in menu)
            {
                // verifica se item está configurado para aparecer no footer
                if (!menuItem.FlagFooter)
                    continue;

                var paginas = menuItem.Paginas
                    .Where(x => x.Navegacao && !String.IsNullOrWhiteSpace(x.Url))
                    .ToArray();

                // exibe apenas itens com um dos critérios:
                // - possui página para acesso
                // - possui ao menos um filho marcado para exibir no footer
                if (paginas.Length == 0 && !(menuItem.Items.Count > 0 && menuItem.Items.Any(x => x.FlagFooter)))
                    continue;

                writer.RenderBeginTag(HtmlTextWriterTag.Li);

                if (paginas.Length > 0)
                {
                    string url = paginas[0].Url;
                    if (paginas[0].Url.Contains("://"))
                        url = new Uri(paginas[0].Url).LocalPath;

                    writer.AddAttribute(HtmlTextWriterAttribute.Href, url);
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write(menuItem.Texto);
                    writer.RenderEndTag();
                }
                else
                {
                    writer.Write(menuItem.Texto);
                }

                // renderiza subníveis de menu
                this.RenderChildMenus(writer, menuItem.Items, string.Empty, nivelLista + 1);

                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }
    }
}
