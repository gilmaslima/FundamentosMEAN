using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.Portal.Aberto.WebControls {

    /// <summary>
    /// Controle de Paginação do Portal, este foi criado para substituir o Paginador Híbrido, o antigo faz a
    /// paginação por QueryString, isso foi um problema quando os dados de filtro estão em Postback da página :(
    /// </summary>
    public class Paginador : WebControl, IPostBackDataHandler {

        /// <summary>
        /// 
        /// </summary>
        private LinkButton previousButton;

        /// <summary>
        /// 
        /// </summary>
        private LinkButton nextButton;

        // HTML que deve ser Gerado pelo controle
        //<ul class="paginate">
        //    <li class="previous">
        //        <a class="png" href="#" title="&lt;">&lt;</a>
        //    </li>
        //    <li>
        //        <a href="#" class="selected" title="1">1</a>
        //    </li>
        //    <li>
        //        <a href="?pagina=2#wptListagem" title="2">2</a>
        //    </li>
        //    <li class="next">
        //        <a class="png" href="?pagina=2#wptListagem" title="&gt;">&gt;</a>
        //    </li>
        //</ul>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(System.EventArgs e) {
            Page.RegisterRequiresPostBack(this);
        }

        protected override void CreateChildControls() {
            if (this.TotalPaginas > 1) {
                this.Controls.Add(new Literal() { Text = "<ul class=\"paginate\">" });
                this.Controls.Add(new Literal() { Text = "<li class=\"previous\">" });
                this.Controls.Add(RenderPrevious());
                this.Controls.Add(new Literal() { Text = "</li>" });

                for (int i = 1; i <= this.TotalPaginas; i++) {
                    this.Controls.Add(new Literal() { Text = "<li>" });
                    this.Controls.Add(RenderPageLink(i));
                    this.Controls.Add(new Literal() { Text = "</li>" });
                }

                this.Controls.Add(new Literal() { Text = "<li class=\"next\">" });
                this.Controls.Add(RenderNext());
                this.Controls.Add(new Literal() { Text = "</li>" });
                this.Controls.Add(new Literal() { Text = "</ul>" });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        protected Control RenderPageLink(int pageNumber) {
            LinkButton pageButton = new LinkButton();
            pageButton.ID = "numberButton_" + pageNumber.ToString();
            if (this.PaginaAtual == pageNumber) {
                pageButton.CssClass = "selected";
                pageButton.Enabled = false;
            }
            pageButton.Attributes.Add("title", pageNumber.ToString());
            pageButton.Text = pageNumber.ToString();
            return pageButton;
        }

        protected Control RenderNext() {
            nextButton = new LinkButton();
            nextButton.CssClass = "png";
            nextButton.ID = "nextButton";
            if (this.PaginaAtual >= this.TotalPaginas)
                nextButton.Enabled = false;
            nextButton.Attributes.Add("title", ">");
            nextButton.Text = "&gt;";
            return nextButton;
        }

        protected Control RenderPrevious() { 
            previousButton = new LinkButton();
            previousButton.CssClass = "png";
            previousButton.ID = "previousButton";
            if (this.PaginaAtual == 1)
                previousButton.Enabled = false;
            previousButton.Attributes.Add("title", "<");
            previousButton.Text = "&lt;";
            return previousButton;
        }

        /// <summary>
        /// Constante que define a chave de ViewState do Paginador
        /// </summary>
        const string g_sPaginaIndexKey = "_key_paging_portal_redecard";

        /// <summary>
        /// 
        /// </summary>
        bool g_bHasValue = false;

        /// <summary>
        /// 
        /// </summary>
        int g_iTotalPaginas = 1;

        /// <summary>
        /// 
        /// </summary>
        public int TotalPaginas {
            get {
                return g_iTotalPaginas;
            }
            set {
                if (value > 1)
                    g_iTotalPaginas = value;
            }
        }

        /// <summary>
        /// Garanti que o paginador já tem uma variavel de ViewState criada na tela
        /// </summary>
        /// <returns></returns>
        public void EnsureHasValue() {
            if (!g_bHasValue) {
                ArrayList keys = new ArrayList();
                keys.AddRange(ViewState.Keys);

                if (!keys.Contains(g_sPaginaIndexKey)) {
                    ViewState.Add(g_sPaginaIndexKey, (int)1);
                }
                g_bHasValue = true;
            }
        }

        /// <summary>
        /// Reinicia a paginador a posição 1
        /// </summary>
        public void Reset() {
            EnsureHasValue();
            ViewState[g_sPaginaIndexKey] = (int)1;
        }

        /// <summary>
        /// Retorna o página atual que deve ser exibida
        /// </summary>
        public int PaginaAtual {
            get {
                EnsureHasValue();
                return (int)ViewState[g_sPaginaIndexKey];
            }
            set {
                EnsureHasValue();
                ViewState[g_sPaginaIndexKey] = value;
            }
        }

        public void RunToPrevious() {
            if (this.PaginaAtual > 1)
                this.PaginaAtual = this.PaginaAtual - 1;
        }

        public void RunToNext() {
            this.PaginaAtual = this.PaginaAtual + 1;
        }

        public void RunToPageNumber(int pageNumber) {
            this.PaginaAtual = pageNumber;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postDataKey"></param>
        /// <param name="postCollection"></param>
        /// <returns></returns>
        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection) {
            if (postCollection["__EVENTTARGET"].Contains("previousButton")) {
                RunToPrevious();
            } else if (postCollection["__EVENTTARGET"].Contains("nextButton")) {
                RunToNext();
            }
            else if (postCollection["__EVENTTARGET"].Contains("numberButton_")) {
                string sControlId = postCollection["__EVENTTARGET"];
                sControlId = sControlId.Substring(sControlId.Length - 1, 1);
                int pageNumber = 1;
                int.TryParse(sControlId,out pageNumber);
                RunToPageNumber(pageNumber);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RaisePostDataChangedEvent() {
            
        }
    }
}
