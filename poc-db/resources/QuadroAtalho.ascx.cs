using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Redecard.PN.Comum;

using System.Linq;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{
    public partial class QuadroAtalho : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.ValidarPermissao = false;
            base.OnLoad(e);
        }

        private string _urlContexto = string.Empty;
        public string UrlContexto { get { return _urlContexto; } set { _urlContexto = value; } }
        /// <summary>
        /// Carregamento da página, pesquisar pelo item de menu atual e exibir os boxes
        /// das demais funcionalidades
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !object.ReferenceEquals(this.SessaoAtual, null))
            {
                this.CarregarBoxes();
            }
        }

        /// <summary>
        /// Carrega a relação de boxes (items de menu) ao qual o usuário possui acesso.
        /// </summary>
        private void CarregarBoxes()
        {
            Menu itemAtual = this.ObterMenuItemAtual();
            List<Menu> lstMenu = new List<Menu>();
            if (!object.ReferenceEquals(itemAtual, null))
            {
                foreach (Menu item in itemAtual.Items)
                {
                    Pagina pgm = item.Paginas.FirstOrDefault(x => !String.IsNullOrEmpty(x.Url) && Request.Path.Contains(x.Url));
                    if (object.ReferenceEquals(pgm, null))
                    {
                        lstMenu.Add(item);
                    }
                }
                // carregar boxes filhos
                rptBoxes.DataSource = lstMenu;
                rptBoxes.DataBind();
            }

        }

        /// <summary>
        /// Carregamento dos links
        /// </summary>
        protected void ServicoDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item ||
                e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
            {
                LinkButton lnkAtalho = (LinkButton)e.Item.FindControl("lnkAtalho");
                if (lnkAtalho != null)
                {
                    Menu menu = (Menu)e.Item.DataItem;
                    lnkAtalho.Text = menu.Texto;
                    //pega somente o primeiro item
                    lnkAtalho.PostBackUrl = menu.Paginas[0].Url;
                }

            }
        }

        /// <summary>
        /// Retorno o item de menu atual da página, caso não encontre nenhum item, retorno nulo.
        /// </summary>
        /// <returns></returns>
        private Menu ObterMenuItemAtual()
        {

            Menu itemAtual = null;
            itemAtual = this.ObterMenuItemAtual(this.SessaoAtual.Menu);
            return itemAtual;
        }

        /// <summary>
        /// Metódo auxilixar para pesquisar em toda a estrutura de Menu
        /// </summary>
        /// <param name="itemAtual"></param>
        /// <returns></returns>
        private Menu ObterMenuItemAtual(List<Menu> items)
        {
            if (!object.ReferenceEquals(items, null) && items.Count > 0)
            {
                Menu itemAtual = null;
                Pagina paginaAtual = null;

                String absolutePah = UrlContexto;// Request.Url.AbsolutePath;

                foreach (Menu item in items)
                {
                    paginaAtual = item.Paginas.FirstOrDefault(x => !String.IsNullOrEmpty(x.Url) && absolutePah.Contains(x.Url));
                    if (!object.ReferenceEquals(paginaAtual, null))
                    {
                        itemAtual = item;
                        break;
                    }
                }

                if (!object.ReferenceEquals(itemAtual, null))
                    return itemAtual;
                else
                {
                    foreach (Menu item in items)
                    {
                        itemAtual = this.ObterMenuItemAtual(item.Items);
                        if (!object.ReferenceEquals(itemAtual, null))
                            break;
                    }
                    if (!object.ReferenceEquals(itemAtual, null))
                    {
                        return itemAtual;
                    }
                }
            }
            return null;
        }
    }
}
