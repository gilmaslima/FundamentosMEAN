using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint
{
    public partial class LoginTitulo : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!object.ReferenceEquals(Session[Sessao.ChaveSessao], null))
            {
                Sessao sessao = Session[Sessao.ChaveSessao] as Sessao;

                if (sessao.GrupoEntidade == 1)
                    ltlTitulo.Visible = true;
            }
        }
    }
}

