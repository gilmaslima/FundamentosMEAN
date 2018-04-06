using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using Redecard.PN.Comum;
using Redecard.PN.DataCash.Base;

namespace Redecard.PN.DataCash.BasePage
{
    public abstract class UserControlBaseDataCash : System.Web.UI.UserControl
    {
        #region [ Constantes ]

        public const Int32 CODIGO_ERRO = 300;
        public const String FONTE = "Redecard.PN.Web";

        #endregion

        public void AtualizaSession()
        {
            if (Request.QueryString["dados"] != null)
            {
                QueryStringSegura qs = new QueryStringSegura(Request.QueryString["dados"]);
                Session["urlParent"] = qs["url"];
            }
        }

        /// <summary>Dados da sessão</summary>
        protected Sessao SessaoAtual
        {
            get { return Session[Sessao.ChaveSessao] as Sessao; }
            private set { Session[Sessao.ChaveSessao] = value; }
        }

        #region Painel de Erro

        /// <summary>
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <param name="codigo">Código do erro</param>
        [Obsolete("Utilizar métodos ExibirPainelExcecao")]
        public void GeraPainelExcecao(String mensagem, String codigo)
        {           
            String script = String.Format("exibirMensagem('{0}', '{1}', '{2}');", this.ObterPaginaRedirecionamento(), mensagem, codigo);
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, script, true);
        }

        public abstract String ObterPaginaRedirecionamento();

        #endregion

        #region [ Exibir Painel de Exceção ]
        
        /// <summary>
        /// Exibe painel de erro padrão do sistema
        /// </summary>        
        /// <param name="codigo">Código do erro</param>
        /// <param name="fonte">Fonte</param>
        /// <param name="titulo">Título</param>
        /// <param name="urlRedirecionamento">URL de redirecionamento</param>
        protected void ExibirPainelExcecao(String titulo, String fonte, Int32 codigo, String urlRedirecionamento)
        {
            PainelMensagem.ExibirPainelExcecao(this.Page, titulo, fonte, codigo, urlRedirecionamento);
        }

        /// <summary>
        /// Exibe painel de erro padrão do sistema
        /// </summary>        
        /// <param name="codigo">Código do erro</param>
        /// <param name="fonte">Fonte</param>
        /// <param name="urlRedirecionamento">URL de redirecionamento</param>
        protected void ExibirPainelExcecao(String fonte, Int32 codigo, String urlRedirecionamento)
        {
            PainelMensagem.ExibirPainelExcecao(this.Page, fonte, codigo, urlRedirecionamento);
        }

        /// <summary>
        /// Exibe painel de erro padrão do sistema
        /// </summary>        
        /// <param name="codigo">Código do erro</param>
        /// <param name="fonte">Fonte</param>
        protected void ExibirPainelExcecao(String fonte, Int32 codigo)
        {
            PainelMensagem.ExibirPainelExcecao(this.Page, fonte, codigo);
        }

        /// <summary>
        /// Exibe painel de erro padrão do sistema
        /// </summary>
        /// <param name="ex">Exceção contendo a mensagem e código do erro</param>
        protected void ExibirPainelExcecao(PortalRedecardException ex)
        {
            PainelMensagem.ExibirPainelExcecao(this.Page, ex);
        }

        #endregion

        /// <summary>
        /// Redirects a client to a new URL. Specifies the new URL and whether execution
        /// of the current page should terminate.
        /// </summary>
        /// <param name="url">The location of the target.</param>
        public void Redirecionar(String url)
        {
            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}