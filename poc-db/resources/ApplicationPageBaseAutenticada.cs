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
    /// Classe ApplicationPageBaseAutenticada.
    /// </summary>
    public class ApplicationPageBaseAutenticada : ApplicationBasePage
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
        /// Gera painel de erro padrão do sistema
        /// </summary>
        /// <param name="mensagem">Mensagem de erro</param>
        /// <returns>Painel de erro padrão</returns>
        public Panel RetornarPainelExcecao(String erro)
        {
            ApplicationBasePage page = new ApplicationBasePage();
            return page.RetornarPainelExcecao(erro);
        }
    }
}
