using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI;

using Microsoft.SharePoint.WebControls;
using System.Web;

namespace Redecard.PN.Comum
{
    /// <summary>
    /// Classe ApplicationPageBaseAnonima.
    /// </summary>
    public class ApplicationPageBaseAnonima : UnsecuredLayoutsPageBase
    {
        /// <summary>
        /// Constante com o código genérico de erro das classes de dados
        /// </summary>
        /// <value>301</value>
        public const Int32 CODIGO_ERRO = 301;

        /// <summary>
        /// Constante com o nome completo da classe
        /// </summary>
        /// <value>Redecard.PN.SharePoint.LAYOUTS</value>
        public const String FONTE = "Redecard.PN.SharePoint.LAYOUTS";

        /// <summary>
        /// Sobrepõe a propriedade base para a página sempre ser acessada por qualquer usuário
        /// </summary>
        /// <value>true</value>
        protected override bool AllowAnonymousAccess
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <returns>Painel de erro padrão</returns>
        public Panel RetornarPainelExcecao(String fonte, Int32 codigo)
        {
            ApplicationBasePage page = new ApplicationBasePage();
            return page.RetornarPainelExcecao(fonte, codigo);
        }

        /// <summary>
        /// Retornar a mensagem de erro de acordo com a fonte/codigo informados
        /// </summary>
        /// <param name="fonte"></param>
        /// <param name="codigo"></param>
        /// <returns></returns>
        public String RetornarMensagemErro(String fonte, Int32 codigo)
        {
            ApplicationBasePage page = new ApplicationBasePage();
            return page.RetornarMensagemErro(fonte, codigo);
        }

        /// <summary>
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <returns>Painel de erro padrão</returns>
        public Panel RetornarPainelExcecao(String erro)
        {
            ApplicationBasePage page = new ApplicationBasePage();
            return page.RetornarPainelExcecao(erro);
        }

        /// <summary>
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de errp</param>
        /// <param name="codigo">Código do erro</param>
        public void ExibirPainelExcecao(String mensagem, Int32 codigo)
        {
            String titulo = this.GetGlobalResourceObject("redecard", "titMensagemErro").ToString();
            String script = String.Format("exibirPainelMensagem('{0}', '{1}');",
                HttpUtility.HtmlEncode(titulo), HttpUtility.HtmlEncode(String.Format("{0} ({1})", mensagem, codigo)));
            this.Page.ClientScript.RegisterStartupScript(this.GetType(), "Key_" + this.ClientID, script, true);
        }

        /// <summary>
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de errp</param>
        /// <param name="codigo">Código do erro</param>
        /// <param name="panel">UpdatePanel que fez a chamada</param>
        public void ExibirPainelExcecaoAsync(String mensagem, Int32 codigo, UpdatePanel panel)
        {
            String titulo = this.GetGlobalResourceObject("redecard", "titMensagemErro").ToString();
            String script = String.Format("exibirPainelMensagem('{0}', '{1}');",
                HttpUtility.HtmlEncode(titulo), HttpUtility.HtmlEncode(String.Format("{0} ({1})", mensagem, codigo)));
            ScriptManager.RegisterStartupScript(panel, panel.GetType(), "Key_" + panel.ClientID, script, true);
        }
    }
}
