using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Redecard.PN.DataCash.BasePage;
using Redecard.PN.Comum;

namespace Redecard.PN.DataCash
{
    public partial class FacaSuaVendaConfirmacao : PageBaseDataCash
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Page_Load - Faça sua Venda"))
            {
                try
                {
                    if (Session["FacaSuaVenda"] != null)
                    {
                        Modelo.Venda venda = (Modelo.Venda)Session["FacaSuaVenda"];

                        Control controle = Page.LoadControl(venda.CaminhoUserControlConfirmacao);
                        pnlControle.Controls.Add(controle);
                    }

                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }
    }
}