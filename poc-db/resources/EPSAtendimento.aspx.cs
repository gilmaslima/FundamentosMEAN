#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [03/08/2012] – [André Garcia] – [Criação]
*/
#endregion

using System;
using System.Configuration;
using System.Net;
using System.Web;
using Redecard.IZ.Comum;

namespace Redecard.PN.Intranet.Integracao
{
    /// <summary>
    /// Págia utilizada na integração da Intranet com o Portal Redecard legado,
    /// usada principalmente pelos operadores da Atento
    /// </summary>
    public partial class EPSAtendimento : System.Web.UI.Page
    {
        /// <summary>
        /// Hash de senha do operador, utilizada para autenticação no banco de dados IS (Sybase)
        /// </summary>
        private String _operadorHashSenha = "1BC7918A2757C2460DD3C19D46EC6E0314DB4A"; /* Senha: Rdm4ns@zpws2 */

        /// <summary>
        /// 
        /// </summary>
        private String _siglaServico = "SERVPORTAL";

        /// <summary>
        /// 
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                SessaoIZ sessao = SessaoIZ.Obter();

                String _funcional = sessao.Funcional;
                String _nome = sessao.Nome;
                ltNomeUsuario.Text = _nome;

                if (!String.IsNullOrEmpty(_funcional))
                {
                    if (!GAPerfil.VerificarPermissao(_funcional, _siglaServico))
                        this.ExibirErro("Acesso Negado.");
                    else
                        Acessar();
                }
                else
                    this.ExibirErro("Não foi possível encontrar o funcional do usuário.");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultado"></param>
        protected void ExibirErro(String mensagem)
        {
            ltErro.Text = mensagem;
            pnlErro.Visible = true;
            pnlNumeroPV.Visible = false;
        }

        /// <summary>
        /// Acessar o Portal Redecard (Área Fechada) usando as credenciais de Operador
        /// </summary>
        protected void Acessar()
        {
            SessaoIZ sessao = SessaoIZ.Obter();
            QueryStringSegura _query = new QueryStringSegura();
            _query.Add("operadorHashSenha", _operadorHashSenha);
            _query.Add("operadorFuncional", sessao.Funcional);

            String _urlLegado = ConfigurationManager.AppSettings["_portalRedecardLoginPage"];

            String _urlFormat = "{0}?dados={1}&tipo=EPS";
            String _url = String.Format(_urlFormat, _urlLegado, _query.ToString());

            // Redirecionar para o portal legado
            Response.Redirect(_url);
            Response.End();
        }
    }
}