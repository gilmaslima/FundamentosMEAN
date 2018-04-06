using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.Portal.Aberto.WebParts.RedecardRedesSociais {
    
    /// <summary>
    /// Web part para a exibição do compartilhe 
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class RedecardRedesSociais : WebPart {

        #region Propriedades__________________

        /// <summary>
        /// Título Do Compartilhe
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Título")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public string titulo {
            get;
            set;
        }

        /// <summary>
        /// Exibir Facebook?
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Facebook")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public bool blnFacebook {
            get;
            set;
        }

        /// <summary>
        /// Exibir Twitter?
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Twitter")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public bool blnTwitter {
            get;
            set;
        }

        /// <summary>
        /// Exibir Delicious?
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Del.icio.us")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public bool blnDelicious {
            get;
            set;
        }

        /// <summary>
        /// Exibir My Space?
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("My Space")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public bool blnMySpace {
            get;
            set;
        }

        /// <summary>
        /// Exibir Windows Live?
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Windows Live")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public bool blnWindowsLive {
            get;
            set;
        }

        /// <summary>
        /// Exibir Google Bookmarks?
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Google Bookmarks")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public bool blnGoogleBookmarks {
            get;
            set;
        }

        /// <summary>
        /// Exibir Orkut?
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Orkut")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public bool blnOrkut {
            get;
            set;
        }

        /// <summary>
        /// Exibir Digg?
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Digg")]
        [Description()]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        public bool blnDigg {
            get;
            set;
        }

        #endregion

        #region Constantes____________________

        /// <summary>
        /// Caminho do ASCX 
        /// </summary>
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/RedecardRedesSociais/RedecardRedesSociaisUserControl.ascx";

        /// <summary>
        /// URL Do TinyUrl
        /// </summary>
        private const string tinyURL = @"http://tinyurl.com/api-create.php?url=";

        /// <summary>
        /// URL Do Facebook
        /// </summary>
        private const string facebookURL = @"http://www.facebook.com/sharer.php?u={u}&amp;t={t}";

        /// <summary>
        /// URL Do Twitter
        /// </summary>
        private const string twitterURL = @"http://twitter.com/home?status={t}%20{u}";

        /// <summary>
        /// URL Do Delicious
        /// </summary>
        private const string deliciousURL = @"http://del.icio.us/post?url={u}&amp;title={t}";

        /// <summary>
        /// URL Do Facebook
        /// </summary>
        private const string mySpaceURL = @"http://www.myspace.com/Modules/PostTo/Pages/?u={u}&amp;t={t}";

        /// <summary>
        /// URL Do Windows Live
        /// </summary>
        private const string windowsLiveURL = @"https://favorites.live.com/quickadd.aspx?marklet=1&amp;mkt=en-us&amp;url={u}&amp;title={t}";

        /// <summary>
        /// URL Do Google Bookmarks
        /// </summary>
        private const string googleBookmarksURL = @"http://www.google.com/bookmarks/mark?op=edit&amp;bkmk={u}&amp;title={t}";

        /// <summary>
        /// URL Do Orkut
        /// </summary>
        private const string orkutURL = @"http://promote.orkut.com/preview?nt=orkut.com&amp;du={u}&amp;tt={t}&amp;cn=";

        /// <summary>
        /// URL Do Digg
        /// </summary>
        private const string diggURL = @"http://digg.com/submit?phase=2&amp;url={u}&amp;title={t}";


        #endregion

        #region Métodos_______________________

        /// <summary>
        /// Configurar Item
        /// </summary>
        /// <param name="button"></param>
        /// <param name="visibility"></param>
        /// <param name="socialBookmarkingUrl"></param>
        public void ConfigurarItem(HtmlAnchor link, Boolean visibilidade, string url) {
            link.Visible = visibilidade;
            link.Attributes.Add("OnClick", String.Format("window.open('{0}'); return false;", CriarTinyUrl(url.Replace("{u}", HttpContext.Current.Request.Url.ToString()).Replace("{t}", this.titulo))));
        }

        /// <summary>
        /// Criar TinyUrl
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        private string CriarTinyUrl(string url) {
            string saida = String.Empty;

            foreach (Match match in new Regex("http://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", RegexOptions.IgnoreCase).Matches(url)) {
                try {
                    saida = match.Value;

                    if (!(saida.Length <= 12)) {

                        if (!saida.ToLower().StartsWith("http") && !saida.ToLower().StartsWith("ftp")) {
                            saida = "http://" + saida;
                        }

                        WebResponse webResponse = WebRequest.Create(tinyURL + saida).GetResponse();

                        using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream())) {
                            saida = streamReader.ReadToEnd();
                        }

                    }
                }
                catch (Exception) {
                }

                url = url.Replace(match.Value, saida);

            }

            return url;
        }

        #endregion

        #region Eventos_______________________

        protected override void CreateChildControls() {
            try {
                Control control = Page.LoadControl(_ascxPath);

                this.ConfigurarItem((HtmlAnchor)control.FindControl("btnDelicious"), blnDelicious, deliciousURL);
                this.ConfigurarItem((HtmlAnchor)control.FindControl("btnDigg"), blnDigg, diggURL);
                this.ConfigurarItem((HtmlAnchor)control.FindControl("btnFacebook"), blnFacebook, facebookURL);
                this.ConfigurarItem((HtmlAnchor)control.FindControl("btnGoogleBookmarks"), blnGoogleBookmarks, googleBookmarksURL);
                this.ConfigurarItem((HtmlAnchor)control.FindControl("btnMySpace"), blnMySpace, mySpaceURL);
                this.ConfigurarItem((HtmlAnchor)control.FindControl("btnOrkut"), blnOrkut, orkutURL);
                this.ConfigurarItem((HtmlAnchor)control.FindControl("btnTwitter"), blnTwitter, twitterURL);
                this.ConfigurarItem((HtmlAnchor)control.FindControl("btnWindowsLive"), blnWindowsLive, windowsLiveURL);

                Controls.Add(control);
            }
            catch (Exception ex) {
                throw ex;
            }

        }

        #endregion

    }
}