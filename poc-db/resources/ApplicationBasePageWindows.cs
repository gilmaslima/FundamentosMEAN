using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;

using Microsoft.SharePoint.WebControls;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe ApplicationBasePage.
    /// </summary>
    public class ApplicationBasePageWindows : LayoutsPageBase
    {
        /// <summary>
        /// Objeto Sessão
        /// </summary>
        private Sessao _sessao = null;

        /// <summary>
        /// Recupera sessão atual caso exista
        /// </summary>
        public Sessao SessaoAtual
        {
            get
            {
                if (object.ReferenceEquals(_sessao, null))
                    _sessao = Sessao.Obtem();

                return _sessao;
            }
        }

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // Se não existir a sessão e o usuário logado é via FBA exibe painel de exceção
            if (this.SessaoAtual == null || !Util.UsuarioLogadoFBA())
            {
                // redirecionar usuário para  a página de sessão expirada
                throw new UnauthorizedAccessException();
            }
        }

        /// <summary>
        /// AllowAnonymousAccess
        /// </summary>
        /// <value>false</value>
        protected override bool AllowAnonymousAccess
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Painel de erro padrão</returns>
        private Panel RetornarPainelExcecao(String mensagem, String codigo)
        {
            Panel painel = new Panel();
            painel.ID = "pnlErro";

            HtmlGenericControl div = new HtmlGenericControl("div");
            div.ID = "pnlMensagem";
            div.Attributes.Add("class", "aviso");
            div.Attributes.Add("style", "width: 100%; padding: 10px; color:#575757; font-size:12px; font-family:Arial;");

            StringBuilder sb = new StringBuilder();
            sb.Append("<span><center>");
            sb.Append(mensagem);
            sb.Append("&nbsp;");
            sb.Append(String.Format("({0})", codigo));
            sb.Append("</center></span>");

            div.InnerHtml = sb.ToString();

            painel.Controls.Add(div);
            return painel;
        }

        /// <summary>
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <returns>Painel de erro padrão</returns>
        public Panel RetornarPainelExcecao(String mensagem)
        {
            Panel painel = new Panel();
            painel.ID = "pnlErro";

            HtmlGenericControl div = new HtmlGenericControl("div");
            div.ID = "pnlMensagem";
            div.Attributes.Add("class", "aviso");
            div.Attributes.Add("style", "width: 100%; padding: 10px; color:#575757; font-size:12px; font-family:Arial;");

            StringBuilder sb = new StringBuilder();
            sb.Append("<span><center>");
            sb.Append(mensagem);
            sb.Append("</center></span>");

            div.InnerHtml = sb.ToString();

            painel.Controls.Add(div);
            return painel;
        }

        /// <summary>
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Painel de erro padrão</returns>
        public Panel RetornarPainelExcecao(String fonte, Int32 codigo)
        {
            using (TrataErroServico.TrataErroServicoClient trataErroServico = new TrataErroServico.TrataErroServicoClient())
            {
                var trataErro = trataErroServico.Consultar(fonte, codigo);

                if (trataErro.Codigo != 0)
                    return this.RetornarPainelExcecao(trataErro.Fonte, trataErro.Codigo.ToString());
                else
                    return this.RetornarPainelExcecao("Sistema Indisponível", "-1");
            }
        }
    }
}
