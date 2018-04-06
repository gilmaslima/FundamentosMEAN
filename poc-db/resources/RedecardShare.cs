using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using Redecard.Portal.Aberto.Model;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Helper;

namespace Redecard.Portal.Aberto.WebParts.RedecardShare {

    /*  André de Castro Garcia
        ------------------------------------------------------
        Lista de provedores disponíveis no Portal Redecard

        //Facebook
        //http://www.facebook.com/sharer.php?u=<url>
        //Twitter
        //http://migre.me/compartilhar?msg=<title> <url>
        //Delicious
        //http://del.icio.us/post?url=<url>&title=<title>
        //Google
        //http://www.google.com/bookmarks/mark?op=edit&bkmk=<url>
        //Technorati
        //http://technorati.com/faves/tcelestino?add=<url>
        //Yahoo
        //http://myweb2.search.yahoo.com/myresults/bookmarklet?u=<url>
        //Digg
        //http://digg.com/submit?phase=2&url=<url>
        //Windows Live
        //https://favorites.live.com/quickadd.aspx?marklet=1&mkt=en-us&url=<url>
     */

    //<div class="tools">
    //    <ul>
    //        <li><a href="#" title="Imprimir" class="ico_print">Imprimir</a></li>
    //        <li class="last"><a href="#" title="Enviar" class="ico_send">Enviar</a></li>
    //    </ul>
    //    <ul class="share">
    //        <li class="first">Compartilhe</li>
    //        <li><a href="#" title="Facebbok" class="ico_small_facebook">Facebook</a></li>
    //        <li><a href="#" title="Twitter" class="ico_small_twitter">Twitter</a></li>
    //        <li><a href="#" title="Del.icio.us" class="ico_small_delicious">delicious</a></li>
    //        <li class="last"><a href="#" title="Google" class="ico_small_google">Google</a></li>
    //    </ul>
    //</div>

    /// <summary>
    /// Web part de compartilhamento de informações do Portal Redecard
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class RedecardShare : WebPart {

        /// <summary>
        /// Inicia a renderização da web part de compartilhamento
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderBeginTag(HtmlTextWriter writer) {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "tools");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            // renderização dos links de impressão e enviar um e-mail
            this.RenderLeftLinks(writer);
            // renderização dos links de compartilhamento de provider
            RenderRightLinks(writer);
        }

        /// <summary>
        /// Renderiza os links dos providers de compartilhamento
        /// </summary>
        /// <param name="writer"></param>
        private void RenderRightLinks(HtmlTextWriter writer) {
            string sCurrentTitle = string.Empty;
            if (!object.ReferenceEquals(SPContext.Current.ListItem, null))
                sCurrentTitle = (String.IsNullOrEmpty(SPContext.Current.ListItem.DisplayName) ? string.Empty : SPContext.Current.ListItem.DisplayName);
            string sCurrentUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "share");
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);
            IList<DTOCompartilhe> providers = this.GetProviders();
            int i = 1;
            int iCount = providers.Count;
            foreach (DTOCompartilhe compartilhe in providers) {
                if (i == 1)
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "first");
                if (i == iCount)
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "last");
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                this.RenderAElement(
                    true,
                    compartilhe.GetLink(sCurrentUrl, sCurrentTitle),
                    compartilhe.Titulo,
                    compartilhe.Class,
                    compartilhe.Titulo, writer);
                writer.RenderEndTag();
                i++;
            }
            writer.RenderEndTag();
        }

        /// <summary>
        /// Renderiza os links de impressão e de enviar para um amigo
        /// </summary>
        /// <param name="writer"></param>
        private void RenderLeftLinks(HtmlTextWriter writer) {
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);
            writer.RenderBeginTag(HtmlTextWriterTag.Li);
            this.RenderAElement(false, "javascript:window.print();", "Imprimir", "ico_print", RedecardHelper.ObterResource("imprimir"), writer);
            writer.RenderEndTag();
            // TODO: Colocar tratamento para o Enviar para um amigo
            //writer.AddAttribute(HtmlTextWriterAttribute.Class, "last");
            //writer.RenderBeginTag(HtmlTextWriterTag.Li);
            //this.RenderAElement(false, "#", "Enviar", "ico_send", "Enviar", writer);
            //writer.RenderEndTag();
            writer.RenderEndTag();
        }

        /// <summary>
        /// Renderiza o HTML de um elemento do tipo link
        /// </summary>
        public void RenderAElement(bool openInNewWindow, string href, string title, string cssclass, string text, HtmlTextWriter writer) {
            writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
            writer.AddAttribute(HtmlTextWriterAttribute.Title, title);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssclass);
            if (openInNewWindow)
                writer.AddAttribute(HtmlTextWriterAttribute.Target, "_blank");
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write(text);
            writer.RenderEndTag();
        }

        /// <summary>
        /// Finaliza a renderização da web part
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderEndTag(HtmlTextWriter writer) {
            writer.RenderEndTag();
        }

        /// <summary>
        /// Retorna a relação de providers configurados no Portal Redecard
        /// </summary>
        protected IList<DTOCompartilhe> GetProviders() {
            IList<DTOCompartilhe> items = null;
            using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTOCompartilhe, CompartilheItem>>()) {
                AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                delegate {
                    items = repository.GetItems(item => item.Visível == true);
                });
            }
            // verificar se houve retorno da função
            if (!object.ReferenceEquals(items, null)) {
                return items;
            }
            return null;
        }
    }
}