using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Fechado.Model.Repository;
using Redecard.Portal.Fechado.Model.Repository.DTOs;
using Redecard.Portal.Fechado.Model;
using Redecard.Portal.Helper;
using System.Collections.Generic;

namespace Redecard.Portal.Fechado.WebParts.RedecardAcessoRapido
{
    public partial class RedecardAcessoRapidoUserControl : UserControl
    {

        #region Propriedades__________________

        private RedecardAcessoRapido WebPart
        {
            get
            {
                return (RedecardAcessoRapido)this.Parent;
            }
        }

        #endregion

        #region Constantes____________________

        #endregion

        #region Métodos_______________________

        private void PopularLinks()
        {
            IList<DTOAcessoRapido> itens = null;

            using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOAcessoRapido, AcessoRápidoItem>>())
            {
                AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                delegate
                {
                    itens = repository.GetAllItems();
                });
                this.ddlAcessoRapido.DataSource = itens;
                this.ddlAcessoRapido.DataTextField = "Titulo";
                this.ddlAcessoRapido.DataValueField = "Hiperlink";
                this.ddlAcessoRapido.DataBind();
            }
            this.ddlAcessoRapido.Items.Insert(0, "Selecione sua atuação");
            this.ddlAcessoRapido.SelectedIndex = 0;
        }

        #endregion

        #region Eventos_______________________

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.PopularLinks();
            }

            this.ddlAcessoRapido.SelectedIndexChanged += new EventHandler(ddlAcessoRapido_SelectedIndexChanged);
        }

        protected void ddlAcessoRapido_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList dropDownList = (DropDownList)sender;
            if (dropDownList.SelectedIndex > 0)
            {
                Page.Response.Redirect(dropDownList.SelectedValue.ToString().Split( new char[] {','})[0].Trim());
            }
        }

        #endregion

    }
}
