using System;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{
    public partial class ConfirmacaoAcao : UserControlBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.ValidarPermissao = false;
            base.OnLoad(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Exibe mensagem de confimação de operação
        /// </summary>
        /// <param name="titulo">Título a ser exibido</param>
        /// <param name="mensagem"></param>
        /// <param name="telaVoltar"></param>
        public void ExibirMensagem(String titulo, String mensagem, String telaVoltar)
        {
            try
            {
                if (!String.IsNullOrEmpty(titulo))
                    ltTituloConfirmacao.Text = titulo;

                if (!String.IsNullOrEmpty(mensagem))
                    ltMensagemConfirmacao.Text = mensagem;

                if (!String.IsNullOrEmpty(telaVoltar))
                    lnkVoltar.NavigateUrl = telaVoltar;

                pnlConfirmacao.Visible = true;

            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao("Redecard.PN.SharePoint", 300);
            }
        }
    }
}
