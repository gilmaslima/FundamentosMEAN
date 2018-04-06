using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.Portal.Helper.Web.Mails;
using Microsoft.SharePoint;
using System.Web;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using System.Collections.Generic;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model;
using Redecard.Portal.Helper;
using System.Web.UI.HtmlControls;

namespace Redecard.Portal.Aberto.WebParts.RedecardShareIndiqueAmigo
{
    public partial class RedecardShareIndiqueAmigoUserControl : UserControl
    {

        #region Propriedades__________________

        private RedecardShareIndiqueAmigo WebPart
        {
            get
            {
                return (RedecardShareIndiqueAmigo)this.Parent;
            }
        }

        private int Total = 0;
        private string Titulo = string.Empty;

        #endregion

        #region Constantes____________________

        #endregion

        #region Métodos_______________________

        protected void CarregarLinks()
        {
            if (!object.ReferenceEquals(SPContext.Current.ListItem, null) && !String.IsNullOrEmpty(SPContext.Current.ListItem.DisplayName))
            {
                this.Titulo = SPContext.Current.ListItem.DisplayName;
            }

            IList<DTOCompartilhe> itens = new List<DTOCompartilhe>();
            using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOCompartilhe, CompartilheItem>>())
            {
                AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                delegate
                {
                    itens = repository.GetItems(item => item.Visível == true);
                    this.Total = itens.Count;
                });
                this.rptCompartilhe.DataSource = itens;
                this.rptCompartilhe.DataBind();
            }

            /*itens.Add(new DTOCompartilhe() { Titulo = "Facebook", Class = "ico_small_facebook", Url = "http://www.facebook.com/sharer.php?u=[url]", Visivel = true });
            itens.Add(new DTOCompartilhe() { Titulo = "Twitter", Class = "ico_small_twitter", Url = "http://migre.me/compartilhar?msg=[title] [url]", Visivel = true });
            itens.Add(new DTOCompartilhe() { Titulo = "Del.icio.us", Class = "ico_small_delicious", Url = "http://del.icio.us/post?url=[url]&amp;title=[title]", Visivel = true });
            itens.Add(new DTOCompartilhe() { Titulo = "Google", Class = "ico_small_google", Url = "http://www.google.com/bookmarks/mark?op=edit&amp;amp;bkmk=[url]", Visivel = true });*/

            this.Total = itens.Count;
            this.rptCompartilhe.DataSource = itens;
            this.rptCompartilhe.DataBind();
        }
        protected void IndicarAmigo()
        {

            WebPartManager mng = WebPartManager.GetCurrentWebPartManager(this.Page);
            if (mng.DisplayMode == WebPartManager.BrowseDisplayMode) {
                
                string mensagem = String.Format("<p>Olá, <br> seu amigo <a href=\"mailto:{1}\">{0}</a> indicou o site da <a href=\"http://www.userede.com.br\">Rede</a> e fez o seguinte comentário: <br><br>{2}<br><br>Não deixe de visitar e muito obrigado!</p>", 
                    new object[] { Request.Form["txtSeuNome"], Request.Form["txtSeuEmail"], Request.Form["txtMensagem"] });

                using (SPWeb oWeb = new SPSite(SPContext.Current.Web.Url).OpenWeb()) {
                    if (EmailUtils.EnviarEmail(oWeb, "rede@userede.com.br", Request.Form["txtEmailAmigos"], "", "", Request.Form["hdnAssunto"], true, mensagem))
                        this.pnlIndiqueAmigoSucessoPanel.Visible = true;
                    else
                        this.pnlIndiqueAmigoFalhaPanel.Visible = true;
                }
            }
        }

        #endregion

        #region Eventos_______________________

        protected void Page_Load(object sender, EventArgs e)
        {
            /*System.Collections.Specialized.StringDictionary messageHeader = new System.Collections.Specialized.StringDictionary();
            messageHeader.Add("to", "rafael.carnauba@gmail.com");
            messageHeader.Add("from", "rafael.carnauba@gmail.com");
            messageHeader.Add("subject", "Teste0");
            messageHeader.Add("content-type", "text/plain");
            Microsoft.SharePoint.Utilities.SPUtility.SendEmail(SPContext.Current.Web, messageHeader, "Teste1");
            */
            if (this.Page.IsPostBack)
            {
                string sIndiqueAmigo = Request.Form["idIndiqueAmigo"] as string;
                if (!string.IsNullOrEmpty(sIndiqueAmigo))
                    this.IndicarAmigo();
            }
            this.CarregarLinks();
        }

        protected void rptCompartilhe_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            HtmlControl li = e.Item.FindControl("li") as HtmlControl;
            HyperLink hyperLink = e.Item.FindControl("hlkItemCompartilhe") as HyperLink;
            if (e.Item.ItemIndex == 0) {
                li.Attributes.Add("class", "first");
            }
            if (e.Item.ItemIndex == this.Total - 1) {
                li.Attributes.Add("class", "last");
            }
            hyperLink.NavigateUrl = ((DTOCompartilhe)e.Item.DataItem).GetLink(HttpContext.Current.Request.Url.AbsoluteUri, this.Titulo);

        }

        #endregion

    }
}
