using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{    
    public partial class QuadroAvisoHome : UserControlBase
    {
        /// <summary>
        /// Tipos de ícones do quadro de aviso
        /// </summary>
        public enum Icone {         
            /// <summary>
            /// Ícone de aviso
            /// </summary>
            Aviso = 1,
            /// <summary>
            /// Ícone de confirmação
            /// </summary>
            Confirmacao = 2,
            /// <summary>
            /// Ícone de erro
            /// </summary>
            Erro = 3,
            /// <summary>
            /// Ícone customizado
            /// </summary>
            Customizado = 4
        }

        #region [Atributos da página]

        private String titulo = String.Empty;
        private String mensagem = String.Empty;
        private Icone iconeMensagem = Icone.Aviso;

        /// <summary>
        /// Título do quadro de aviso
        /// </summary>
        public String Titulo 
        {
            get { return this.titulo; }
            set { this.titulo = value; }
        }
        
        /// <summary>
        /// Mensagem do quadro de aviso
        /// </summary>
        public String Mensagem 
        {
            get { return this.mensagem; }
            set { this.mensagem = value; }
        }
        
        /// <summary>
        /// Ícone do quadro de mensagem
        /// </summary>
        public Icone IconeMensagem
        {
            get { return this.iconeMensagem; }
            set { this.iconeMensagem = value; }
        }

        /// <summary>
        /// Ícone customizado
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public Panel IconeCustomizado { get; set; }

        #endregion

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
        /// Inicialização da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    this.CarregarMensagem();
                }
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega os parâmetros do quadro de mensagem nos atributos da mensagem
        /// </summary>
        /// <param name="tituloMensagem"></param>
        /// <param name="mensagem"></param>
        /// <param name="icone"></param>
        public void CarregarMensagem(String tituloMensagem, String mensagem, Icone icone)
        {
            this.IconeMensagem = icone;
            this.CarregarMensagem(tituloMensagem, mensagem);
        }

        /// <summary>
        /// Carrega o título e a mensagem nos atributos da mensagem
        /// </summary>
        /// <param name="tituloMensagem"></param>
        /// <param name="mensagem"></param>
        public void CarregarMensagem(String tituloMensagem, String mensagem)
        {
            this.Titulo = tituloMensagem;
            this.Mensagem = mensagem;
            this.CarregarMensagem();
        }

        /// <summary>
        /// Carrega a mensagem nos controles HTML
        /// </summary>
        public void CarregarMensagem()
        {            
            lblTitulo.Text = this.Titulo;
            lblMensagem.Text = this.Mensagem;

            if (String.IsNullOrEmpty(this.Titulo))
                lblTitulo.Visible = false;

            if (this.IconeMensagem == Icone.Customizado)
            {
                pnlIcone.Controls.Clear();

                var pnlConteudo = new HtmlGenericControl("div");
                pnlIcone.Controls.Add(pnlConteudo);

                if (!String.IsNullOrEmpty(this.IconeCustomizado.CssClass))
                    pnlConteudo.Attributes["class"] = String.Concat("customizado ", this.IconeCustomizado.CssClass);
                else
                    pnlConteudo.Attributes["class"] = "customizado";

                foreach (Control controle in this.IconeCustomizado.Controls)
                    pnlConteudo.Controls.Add(controle);                
            }
            else
            {
                divIcone.Attributes["class"] = this.ClasseIcone(this.IconeMensagem);
            }
        }

        /// <summary>
        /// Identifica a classe da imagem do ícone
        /// </summary>
        /// <param name="ico"></param>
        /// <returns></returns>
        private String ClasseIcone(Icone ico)
        {
            String classe = "aviso";
            switch (ico)
            {
                case Icone.Aviso:
                    classe = "aviso";
                    break;
                case Icone.Confirmacao:
                    classe = "confirmacao";
                    break;
                case Icone.Erro:
                    classe = "erro";
                    break;
                default:
                    classe = "aviso";
                    break;
            }
            return classe;
        }
    }
}
