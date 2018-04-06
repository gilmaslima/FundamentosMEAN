using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Web.UI.HtmlControls;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{
    /// <summary>
    /// 
    /// </summary>
    public partial class WebTrends : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Sessao.Contem())
            {
                Sessao sessao = Sessao.Obtem();

                // adicionar meta tags do WebTrens
                this.RenderMetaTags(sessao);

                // escrever no log do IIS
                StringBuilder sb = new StringBuilder();
                sb.Append("&nu_pdv=");
                sb.Append(sessao.CodigoEntidade.ToString());
                sb.Append("&nu_grupoentidade=");
                sb.Append(sessao.GrupoEntidade.ToString());
                sb.Append("&nu_tipoentidade=1");
                sb.Append("&NomeEstab=");
                sb.Append(sessao.NomeEntidade);
                sb.Append("&NomeUsuario=");
                sb.Append(sessao.NomeUsuario);
                sb.Append("&strTipoUsr=");
                sb.Append(sessao.TipoUsuario);
                sb.Append("&UF=");
                sb.Append(sessao.UFEntidade);

                Response.AppendToLog(sb.ToString());
            }
            else
                Response.AppendToLog("Erro, sessão não encontrada.");

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessao"></param>
        private void RenderMetaTags(Sessao sessao)
        {
            HtmlMeta metaNumeroPdv = new HtmlMeta() { Name = "WT.pdv", Content = sessao.CodigoEntidade.ToString() };
            HtmlMeta metaNumeroGrupoPdv = new HtmlMeta() { Name = "WT.grupo_pdv", Content = sessao.GrupoEntidade.ToString() };
            this.Page.Header.Controls.Add(metaNumeroPdv);
            this.Page.Header.Controls.Add(metaNumeroGrupoPdv);
        }
    }
}
