using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.DataCash.BasePage;

namespace Redecard.PN.DataCash.controles
{
    public partial class ConfirmacaoBotoes : UserControl
    {
        public enum ETipoBotoes
        {
            Confirmacao = 0,
            Comprovante = 1
        }

        private ETipoBotoes _tipoBotoes = ETipoBotoes.Confirmacao;
        public ETipoBotoes TipoBotoes
        {
            get
            {
                return _tipoBotoes;
            }
            set
            {
                _tipoBotoes = value;
            }
            
        }

        public String PaginaComprovante
        {
            get
            {
                return Session["PaginaComprovante"].ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (this.TipoBotoes == ETipoBotoes.Comprovante)
                {
                    botoesVoltarContinuar.Visible = false;
                    botoesVoltar.Visible = true;
                }
                else
                {
                    botoesVoltarContinuar.Visible = true;
                    botoesVoltar.Visible = false;
                }
            }
        }

        /// <summary>
        /// Redireciona usuário
        /// </summary>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            Response.Redirect(this.PaginaComprovante);
        }

        /// <summary>
        /// Ação do botão voltar
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("FacaSuaVenda.aspx");
        }
    }
}