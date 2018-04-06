using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Helper;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Aberto.Model;
using Microsoft.SharePoint;
using System.Web.UI.HtmlControls;

namespace Redecard.Portal.Aberto.WebParts.RedecardMenuMiniSite
{
    public partial class RedecardMenuMiniSiteUserControl : UserControl
    {

        #region Propriedades__________________

        private RedecardMenuMiniSite WebPart
        {
            get
            {
                return (RedecardMenuMiniSite)this.Parent;
            }
        }

        #endregion

        #region Constantes____________________

        #endregion

        #region Métodos_______________________

        private void ListarMenu()
        {
            this.rptMenu.Visible = true;
            IList<DTOSubMenuMiniSite> itens = new List<DTOSubMenuMiniSite>();
            SPSite spSite = SPContext.Current.Site;
            SPWeb spWeb = SPContext.Current.Web;
            SPList spList = spWeb.Lists["SubMenu do MiniSites"];
            foreach (SPListItem item in spList.Items)
            {
                itens.Add(new DTOSubMenuMiniSite { Hiperlink = item["Hiperlink"].ToString().Split(new char[] { ',' })[0].Trim(), Titulo = item["Título"].ToString() });
            }
            this.imgMenu.ImageUrl = this.WebPart.urlImagem;
            this.rptMenu.ItemDataBound += new RepeaterItemEventHandler(rptMenu_ItemDataBound);
            this.rptMenu.DataSource = itens;
            this.rptMenu.DataBind();
        }

        #endregion

        #region Eventos_______________________

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ListarMenu();
        }

        protected void rptMenu_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            HyperLink hyperLink = e.Item.FindControl("hlkItemMenu") as HyperLink;
            if (Page.Request.Url.ToString().StartsWith(hyperLink.NavigateUrl))
            {
                hyperLink.CssClass = "selected";
            }
        }

        #endregion
    }
}
