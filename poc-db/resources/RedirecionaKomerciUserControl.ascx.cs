using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Redecard.PN.Comum;

namespace Redecard.PN.OutrasEntidades.SharePoint.WebParts.Komerci.RedirecionaKomerci
{
    public partial class RedirecionaKomerciUserControl : UserControlBase
    {
        private RedirecionaKomerci WebPart
        {
            get
            {
                return (RedirecionaKomerci)this.Parent;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ifrLegado.Attributes.Add("scrolling", this.WebPart.Scroll);
            ifrLegado.Attributes.Add("src", this.MontarUrl());
            ifrLegado.Attributes.Add("width", this.WebPart.Largura);
            ifrLegado.Attributes.Add("height", this.WebPart.Altura);
        }

        /// <summary>
        /// Montar a URL de envio para o Komerci
        /// </summary>
        protected String MontarUrl()
        {
            if (!object.ReferenceEquals(this.SessaoAtual, null))
            {
                String formato = "{0}?dados={1}&urlPai={2}";
                QueryStringSegura dados = new QueryStringSegura();
                dados.Add("codEtd", this.SessaoAtual.CodigoEntidade.ToString());
                dados.Add("codGruEtd", this.SessaoAtual.GrupoEntidade.ToString());
                dados.Add("codUsr", this.SessaoAtual.LoginUsuario.ToString().Replace("@","").Replace(".","").Left(20));
                return String.Format(formato, this.WebPart.URLKomerci, dados.ToString(), Request.Url.GetLeftPart(UriPartial.Path));
            }
            else
                return this.WebPart.URLKomerci;
        }
    }
}