using System;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{
    /// <summary>
    /// Controle de Confirmação de Ação
    /// </summary>
    public partial class QuadroConfirmacao : UserControlBase
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

        #region [Atributos da Página]

        /// <summary>
        /// EventHandler para ação de Continuar do painel de confirmação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void ContinuarHandle(object sender, EventArgs args);
        
        /// <summary>
        /// Envento para ação de Continuar do painel de confirmação
        /// </summary>
        public event ContinuarHandle Continuar;

        /// <summary>
        /// EventHandler para ação de Voltar do painel de confirmação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void VoltarHandle(object sender, EventArgs args);

        /// <summary>
        /// Envento para ação de Voltar do painel de confirmação
        /// </summary>
        public event VoltarHandle Voltar;
                
        /// <summary>
        /// Título do Painel de Confirmação
        /// </summary>
        public String TituloPainel {
            get { return (String)ViewState["TituloPainel"]; }
            set { ViewState["TituloPainel"] = value; }
        }

        /// <summary>
        /// Mensagem do Painel de Confirmação
        /// </summary>
        public String MensagemPainel {
            get { return (String)ViewState["MensagemPainel"]; }
            set { ViewState["MensagemPainel"] = value; }
        }

        /// <summary>
        /// Classe da imagem. <br/>
        /// default: icone-aviso
        /// </summary>
        public String ClasseImagem 
        {
            get { return this.divIcone.Attributes["class"]; }
            set { this.divIcone.Attributes["class"] = value; }
        }

        /// <summary>
        /// Exibe ou não botão Voltar (default: true)
        /// </summary>
        public Boolean ExibirVoltar 
        {
            get { return this.btnVoltar.Visible; }
            set { this.btnVoltar.Visible = value; }
        }

        /// <summary>
        /// Exibe ou não botão Continuar (default: true)
        /// </summary>
        public Boolean ExibirContinuar
        {
            get { return this.btnContinuar.Visible; }
            set { this.btnContinuar.Visible = value; }
        }

        #endregion

        /// <summary>
        /// Evento de Confirmação da Ação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void ContinuarAcao(Object sender, EventArgs args)
        {
            if (Continuar != null)
                Continuar(sender, null);
        }

        /// <summary>
        /// Evento de Cancelamentos da Ação
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void VoltarAcao(Object sender, EventArgs args)
        {
            if (Voltar != null)
                Voltar(sender, null);
        }

        /// <summary>
        /// Inicialização do Controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Carrega o painel de Confirmação
        /// </summary>
        public void CarregarMensagemQuadro()
        {
            try
            {
                lblTitulo.Text = TituloPainel;
                lblMensagem.Text = MensagemPainel;
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega o painel de Confirmação
        /// </summary>
        /// <param name="mensagem">Mensagem</param>
        /// <param name="titulo">Título</param>
        public void CarregarMensagemQuadro(String titulo, String mensagem)
        {
            this.TituloPainel = titulo;
            this.MensagemPainel = mensagem;
            this.CarregarMensagemQuadro();
        }
    }
}
