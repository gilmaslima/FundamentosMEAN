/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [19/05/2012] - [André Garcia] - [Criação]
*/

using System;
using System.Linq;
using Redecard.PN.Comum;
using System.Collections.Generic;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.QuadrosAtalhos
{
    /// <summary>
    /// 
    /// </summary>
    public partial class QuadrosAtalhosUserControl : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack && !object.ReferenceEquals(this.SessaoAtual, null))
            {
                this.CarregarAtalhos();
            }
        }

        /// <summary>
        /// Título do Quadro de Links
        /// </summary>
        public String TituloQuadro
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CarregarAtalhos()
        {
            Menu itemAtual = base.ObterMenuItemAtual();

            if (!object.ReferenceEquals(itemAtual, null))
            {

                List<Menu> menuConsulta = new List<Menu>();
                List<Menu> menuRelatorios = new List<Menu>();

                foreach (Menu x in itemAtual.Items)
                {
                    if (x.Texto.Contains("Consulta") || x.Texto.Contains("Gerencie"))
                        menuConsulta.Add(x);
                    else
                        menuRelatorios.Add(x);
                }

                if (menuRelatorios.Count > 0 || menuConsulta.Count > 0)
                {
                    pnlMenu.Visible = true;
                    pnlAviso.Visible = false;

                    spnTituloRelatorio.Visible = (menuRelatorios.Count > 0);
                    // carregar boxes filhos
                    rptLinks.DataSource = menuRelatorios;
                    rptLinks.DataBind();

                    spnTituloConsultas.Visible = (menuConsulta.Count > 0);
                    // carregar Consultas
                    rptLinksConsulta.DataSource = menuConsulta;
                    rptLinksConsulta.DataBind();
                }
                else
                {
                    pnlMenu.Visible = false;
                    pnlAviso.Visible = true;

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LinksDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item ||
                e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
            {
                Menu menu = e.Item.DataItem as Menu;
                System.Web.UI.WebControls.HyperLink hlLink = e.Item.FindControl("hlLink") as System.Web.UI.WebControls.HyperLink;
                if (!object.ReferenceEquals(hlLink, null) && !object.ReferenceEquals(menu, null))
                {
                    if (menu.Paginas.Count > 0)
                    {
                        hlLink.NavigateUrl = menu.Paginas[0].Url;
                    }
                    else
                        hlLink.NavigateUrl = "#";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LinksConsultaDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item ||
                e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
            {
                Menu menu = e.Item.DataItem as Menu;
                System.Web.UI.WebControls.HyperLink hlLink = e.Item.FindControl("hlConsultaLink") as System.Web.UI.WebControls.HyperLink;
                if (!object.ReferenceEquals(hlLink, null) && !object.ReferenceEquals(menu, null))
                {
                    if (menu.Paginas.Count > 0)
                    {
                        hlLink.NavigateUrl = menu.Paginas[0].Url;
                    }
                    else
                        hlLink.NavigateUrl = "#";
                }
            }
        }
    }
}