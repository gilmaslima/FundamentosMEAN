using System;
using System.Web.UI;

namespace Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum
{
    /// <summary>
    /// Quadro de aviso de ação concluída/Busca Vazia
    /// </summary>
    public partial class QuadroAviso : UserControlBase
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
        /// Enumeração de ícones de erro
        /// </summary>
        public enum IconeMensagem
        { 
            /// <summary>
            /// Erro
            /// </summary>
            Erro = 1,
            /// <summary>
            /// Aviso
            /// </summary>
            Aviso = 2,
            /// <summary>
            /// Confirmacao
            /// </summary>
            Confirmacao = 3
        }

        /// <summary>
        /// Classe da imagem que deve ser carregada no quadro de aviso
        /// </summary>
        public String ClasseImagem { get; set; }
        
        /// <summary>
        /// Mensagem que será carregada
        /// </summary>
        public String Mensagem { get; set; }

        /// <summary>
        /// Título que será exibido
        /// </summary>
        public String TituloMensagem { get; set; }

        /// <summary>
        /// Inicialização do Controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    if (!String.IsNullOrEmpty(Convert.ToString(Session["AvisoConfirmacaoAcao"])))
                    {
                        if (Session["AvisoConfirmacaoAcao"].ToString().Equals("S"))
                        {
                            String titulo = Session["TituloMensagem"].ToString();
                            String mensagem = Session["Mensagem"].ToString();
                            String urlVoltar = Session["UrlVoltar"].ToString();
                            String classeImagem = (!String.IsNullOrEmpty(Session["classeImagem"].ToString()) ? Session["classeImagem"].ToString() : "icone-green");

                            if (urlVoltar.ToUpper().Equals("VOLTAR"))
                                this.CarregarMensagem(titulo, mensagem, true, classeImagem);
                            else
                                this.CarregarMensagem(titulo, mensagem, urlVoltar, classeImagem);

                            Session["AvisoConfirmacaoAcao"] = "N";
                        }
                    }
                }
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Exibe painel de aviso sem botão voltar
        /// </summary>
        public void CarregarMensagem()
        {
            try
            {
                pnlAviso.Visible = true;
                //pnlBotaoVoltar.Visible = false;
                lblTitulo.Text = this.TituloMensagem;
                lblMensagem.Text = this.Mensagem;
                CarregarClasseIcone(this.ClasseImagem);
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Exibe painel de aviso sem botão voltar (com ícone especificado)
        /// </summary>
        /// <param name="tituloMensagem"></param>
        /// <param name="mensagem"></param>
        public void CarregarMensagem(String tituloMensagem, String mensagem)
        {
            try
            {
                pnlAviso.Visible = true;
                //pnlBotaoVoltar.Visible = false;
                lblTitulo.Text = tituloMensagem;
                lblMensagem.Text = mensagem;
                CarregarClasseIcone(this.ClasseImagem);
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Exibe painel de aviso sem botão voltar (com ícone especificado)
        /// </summary>
        /// <param name="tituloMensagem"></param>
        /// <param name="mensagem"></param>
        public void CarregarMensagem(String tituloMensagem, String mensagem, IconeMensagem icone)
        {
            try
            {
                pnlAviso.Visible = true;
                //pnlBotaoVoltar.Visible = false;
                this.ClasseImagem = GetClasseImagem(icone);
                this.CarregarMensagem(tituloMensagem, mensagem);
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Exibe o painel de aviso com botão Voltar para página anterior
        /// </summary>
        /// <param name="tituloMensagem">Título do aviso</param>
        /// <param name="mensagem">Mensagem do aviso</param>
        /// <param name="voltarPagina">Indica se deve voltar para a página anterior</param>
        public void CarregarMensagem(String tituloMensagem, String mensagem, Boolean voltarPagina)
        {
            pnlAviso.Visible = true;
            pnlVoltarPagina.Visible = voltarPagina;
            this.CarregarMensagem(tituloMensagem, mensagem);
        }

        /// <summary>
        /// Exibe o painel de aviso com botão voltar para página especificada (com ícone específico)
        /// </summary>
        /// <param name="tituloMensagem"></param>
        /// <param name="mensagem"></param>
        /// <param name="voltarPagina"></param>
        /// <param name="classeImagem"></param>
        public void CarregarMensagem(String tituloMensagem, String mensagem, Boolean voltarPagina, String classeImagem)
        {
            try
            {
                pnlAviso.Visible = true;
                this.ClasseImagem = classeImagem;
                this.CarregarMensagem(tituloMensagem, mensagem, voltarPagina);
                
                //CarregarClasseIcone(classeImagem);
                //lblTitulo.Text = tituloMensagem;
                //lblMensagem.Text = mensagem;
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Exibe o painel de aviso com botão voltar para página especificada (com ícone específico)
        /// </summary>
        /// <param name="tituloMensagem"></param>
        /// <param name="mensagem"></param>
        /// <param name="voltarPagina"></param>
        /// <param name="icone"></param>
        public void CarregarMensagem(String tituloMensagem, String mensagem, Boolean voltarPagina, IconeMensagem icone)
        {
            try
            {
                pnlAviso.Visible = true;
                this.ClasseImagem = GetClasseImagem(icone);
                this.CarregarMensagem(tituloMensagem, mensagem, voltarPagina);

                //CarregarClasseIcone(classeImagem);
                //lblTitulo.Text = tituloMensagem;
                //lblMensagem.Text = mensagem;
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Exibe o painel de aviso com botão voltar para página especificada (com ícone padrão)
        /// </summary>
        /// <param name="tituloMensagem"></param>
        /// <param name="mensagem"></param>
        /// <param name="urlVoltar"></param>
        public void CarregarMensagem(String tituloMensagem, String mensagem, String urlVoltar)
        {
            this.CarregarMensagem(tituloMensagem, mensagem, urlVoltar, String.Empty);
        }

        /// <summary>
        /// Exibe painel de aviso com botão voltar
        /// </summary>
        /// <param name="tituloMensagem"></param>
        /// <param name="mensagem"></param>
        /// <param name="urlVoltar"></param>
        public void CarregarMensagem(String tituloMensagem, String mensagem, String urlVoltar, String classeImagem)
        {
            try
            {
                pnlAviso.Visible = true;

                pnlBotaoVoltar.Visible = true;
                lnkVoltar.NavigateUrl = urlVoltar;

                this.ClasseImagem = classeImagem;
                this.CarregarMensagem(tituloMensagem, mensagem);
                
                //lblTitulo.Text = tituloMensagem;
                //lblMensagem.Text = mensagem;
                //CarregarClasseIcone(classeImagem);
                //Session["AvisoConfirmacaoAcao"] = "N";
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Exibe painel de aviso com botão voltar
        /// </summary>
        /// <param name="tituloMensagem"></param>
        /// <param name="mensagem"></param>
        /// <param name="urlVoltar"></param>
        /// <param name="icone"></param>
        public void CarregarMensagem(String tituloMensagem, String mensagem, String urlVoltar, IconeMensagem icone)
        {
            try
            {
                pnlAviso.Visible = true;

                pnlBotaoVoltar.Visible = true;
                lnkVoltar.NavigateUrl = urlVoltar;

                this.ClasseImagem = GetClasseImagem(icone);
                this.CarregarMensagem(tituloMensagem, mensagem);

                //lblTitulo.Text = tituloMensagem;
                //lblMensagem.Text = mensagem;
                //CarregarClasseIcone(classeImagem);
                //Session["AvisoConfirmacaoAcao"] = "N";
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega a classe da imagem do ícone
        /// </summary>
        private void CarregarClasseIcone(String classeImagem)
        {
            try
            {
                if (!String.IsNullOrEmpty(classeImagem))
                    divIcone.Attributes["class"] = classeImagem;
                else
                {
                    if (!String.IsNullOrEmpty(ClasseImagem))
                        divIcone.Attributes["class"] = ClasseImagem;
                    else
                        divIcone.Attributes["class"] = "icone-aviso";
                }
            }
            catch (PortalRedecardException ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Exibe uma imagem na lateral do controle de Aviso, usado
        /// para mostrar um ícone referente a mensagem
        /// </summary>
        /// <param name="urlImagem"></param>
        public void CarregarImagem(String urlImagem)
        {
            imgAviso.Visible = true;
            imgAviso.ImageUrl = urlImagem;
        }

        /// <summary>
        /// Classe da imagem que deve ser carregada no quadro de aviso
        /// </summary>
        /// <param name="icone">Tipo de ícone</param>
        /// <returns>Nome da classe</returns>
        private String GetClasseImagem(IconeMensagem icone)
        {
            String classeIcone = "";
            switch (icone)
            {
                case IconeMensagem.Aviso:
                    classeIcone = "icone-aviso";
                    break;
                case IconeMensagem.Confirmacao:
                    classeIcone = "icone-green";
                    break;
                case IconeMensagem.Erro:
                    classeIcone = "icone-red";
                    break;
                default:
                    classeIcone = "icone-green";
                    break;
            }

            return classeIcone;
        }
    }
}
