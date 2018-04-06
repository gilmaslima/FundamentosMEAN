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
    public partial class TesteSessao : PageBaseDataCash
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Teste Sessão"))
            {
                lblPv.Text = SessaoAtual.CodigoEntidade.ToString();
                Log.GravarMensagem("Sessão", this.SessaoAtual);
            }
        }

        protected void btnTesteException_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("btnLimpar - Faça sua Venda"))
            {
                try
                {
                    throw new Exception("Teste de geração de exception");
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