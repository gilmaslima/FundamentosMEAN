using Redecard.PN.Comum;
using System;
using System.Web.UI;

namespace Rede.PN.Conciliador.SharePoint
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ConciliadorUserControl : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Sessao.Contem())
                {
                    string scriptSessao = string.Format(
                        @"var sessaoAtual = {{
                        CodigoEntidade: {0},
                        NomeEntidade: '{1}'
                    }}",
                           SessaoAtual.CodigoEntidade,
                           SessaoAtual.NomeEntidade == null ? string.Empty : SessaoAtual.NomeEntidade.Trim());

                    ScriptManager.RegisterStartupScript(this, typeof(string), "SessionScript", scriptSessao, true);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (Sessao.Contem() && SessaoAtual.PVMatriz)
                base.RenderControl(writer);
            else
            {
                Control control = Page.LoadControl(@"~/_CONTROLTEMPLATES/Comum/QuadroAcessoNegado.ascx");
                control.RenderControl(writer);
            }
        }
    }
}