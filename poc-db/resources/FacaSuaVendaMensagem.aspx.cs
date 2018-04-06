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
    public partial class FacaSuaVendaMensagem : PageBaseDataCash
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                this.CarregaMensagem();
        }

        /// <summary>
        /// Exibe o comprovante com as informações da venda em caso de processamento com erro realizado na DataCash
        /// </summary>
        private void CarregaMensagem()
        {
            String mensagem = (Session["mensagemQuadroAviso"] as String);
            ucAviso.Mensagem = mensagem;            
        }

        /// <summary>
        /// Ação do botão voltar
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("btnVoltar_Click - Faça sua Venda"))
            {
                try
                {
                    Response.Redirect("FacaSuaVenda.aspx");
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


        #region Obtenção de Pagina de Redirecionamento
        /// <summary>
        /// Página de redirecionamento do Lightbox
        /// </summary>
        /// <returns>String com o redirecionamento</returns>
        public String ObterPaginaRedirecionamento()
        {
            return "pn_FacaSuaVenda.aspx";
        }
        #endregion

    }
}