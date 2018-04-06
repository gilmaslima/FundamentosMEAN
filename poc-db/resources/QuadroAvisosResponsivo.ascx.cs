/*
© Copyright 2015 Rede S.A.
Autor : Felipe Siatiquosque
Empresa : Rede
*/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.CONTROLTEMPLATES.DadosCadastraisMobile
{
    public partial class QuadroAvisosResponsivo : UserControlBase
    {
        #region Propriedades

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
        /// Icone da imagem que deve ser carregada no quadro de aviso
        /// </summary>
        public String IconeImagem { get; set; }

        /// <summary>
        /// Mensagem que será carregada
        /// </summary>
        public String Mensagem { get; set; }

        /// <summary>
        /// Título que será exibido
        /// </summary>
        public String TituloMensagem { get; set; }

        #endregion

        #region Metodos

        /// <summary>
        /// Exibe painel de aviso sem botão voltar
        /// </summary>
        public void CarregarMensagem()
        {
            try
            {
                this.pnlAviso.Visible = true;
                this.pnlBotaoVoltar.Visible = false;
                this.lblTitulo.InnerHtml = this.TituloMensagem;
                this.litMensagem.Text = this.Mensagem;
                this.CarregarClasseIcone(this.ClasseImagem);
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
                this.pnlAviso.Visible = true;
                this.pnlBotaoVoltar.Visible = false;
                this.lblTitulo.InnerHtml = tituloMensagem;
                this.litMensagem.Text = mensagem;
                this.CarregarClasseIcone(this.ClasseImagem);
                //this.CarregarIcone(this.IconeImagem);

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
                this.pnlAviso.Visible = true;
                this.pnlBotaoVoltar.Visible = false;
                this.ClasseImagem = GetClasse(icone);
                //this.IconeImagem = GetIcone(icone);
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
            this.pnlAviso.Visible = true;
            this.pnlVoltarPagina.Visible = voltarPagina;
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
                this.pnlAviso.Visible = true;
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
                this.pnlAviso.Visible = true;
                this.ClasseImagem = GetClasse(icone);
                //this.IconeImagem = GetIcone(icone);
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
                this.pnlAviso.Visible = true;

                this.pnlBotaoVoltar.Visible = true;
                this.lnkVoltar.HRef = urlVoltar;

                this.ClasseImagem = classeImagem;
                this.CarregarMensagem(tituloMensagem, mensagem);

                //lblTitulo.Text = tituloMensagem;
                //lblMensagem.Text = mensagem;
                //CarregarClasseIcone(classeImagem);
                Session["AvisoConfirmacaoAcao"] = "N";
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
                this.pnlAviso.Visible = true;

                this.pnlBotaoVoltar.Visible = true;
                this.lnkVoltar.HRef = urlVoltar;

                this.ClasseImagem = GetClasse(icone);
                this.CarregarMensagem(tituloMensagem, mensagem);

                //lblTitulo.Text = tituloMensagem;
                //lblMensagem.Text = mensagem;
                //CarregarClasseIcone(classeImagem);
                Session["AvisoConfirmacaoAcao"] = "N";
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
        /// <param name="isVoltar">se false, o botão não sera exibido.</param> 
        public void CarregarMensagem(String tituloMensagem, String mensagem, String urlVoltar, IconeMensagem icone, Boolean isVoltar)
        {
            try
            {
                this.pnlAviso.Visible = true;

                this.pnlBotaoVoltar.Visible = isVoltar;
                this.lnkVoltar.HRef = urlVoltar;

                this.ClasseImagem = GetClasse(icone);
                //this.IconeImagem = GetIcone(icone);
                this.CarregarMensagem(tituloMensagem, mensagem);

                //lblTitulo.Text = tituloMensagem;
                //lblMensagem.Text = mensagem;
                //CarregarClasseIcone(classeImagem);
                Session["AvisoConfirmacaoAcao"] = "N";
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
                    divMensagem.Attributes["class"] = "mensagem " + classeImagem;
                else
                {
                    if (!String.IsNullOrEmpty(classeImagem))
                        divMensagem.Attributes["class"] = "mensagem " + classeImagem;
                    else
                        divMensagem.Attributes["class"] = "mensagem atencao";
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
        public void CarregarIcone(String imagem)
        {
            divIcone.InnerHtml = imagem;
        }

        ///// <summary>
        ///// Classe da imagem que deve ser carregada no quadro de aviso
        ///// </summary>
        ///// <param name="icone">Tipo de ícone</param>
        ///// <returns>Nome da classe</returns>
        //private String GetIcone(IconeMensagem icone)
        //{
        //    String classeIcone = "";
        //    switch (icone)
        //    {
        //        case IconeMensagem.Aviso:
        //            classeIcone = "<span><span>!</span></span><em></em></span>";
        //            break;
        //        case IconeMensagem.Confirmacao:
        //            classeIcone = "<span><span>&#10003;</span></span><em></em></span>";
        //            break;
        //        case IconeMensagem.Erro:
        //            classeIcone = "<span><span>x</span></span><em></em></span>";
        //            break;
        //        default:
        //            classeIcone = "<span><span>&#10003;</span></span><em></em></span>";
        //            break;
        //    }

        //    return classeIcone;
        //}

        /// <summary>
        /// Classe da imagem que deve ser carregada no quadro de aviso
        /// </summary>
        /// <param name="icone">Tipo de ícone</param>
        /// <returns>Nome da classe</returns>
        private String GetClasse(IconeMensagem icone)
        {
            String classeIcone = String.Empty;
            switch (icone)
            {
                case IconeMensagem.Aviso:
                    classeIcone = "atencao";
                    break;
                case IconeMensagem.Confirmacao:
                    classeIcone = "confirmacao";
                    break;
                case IconeMensagem.Erro:
                    classeIcone = "erro";
                    break;
                default:
                    classeIcone = "confirmacao";
                    break;
            }

            return classeIcone;
        }

        #endregion

        #region Eventos

        /// <summary>
        /// Evento Load
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            //this.ValidarPermissao = false;
            base.OnLoad(e);
        }

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
                            String classeImagem = (!String.IsNullOrEmpty(Session["classeImagem"].ToString()) ? Session["classeImagem"].ToString() : "confirmacao");

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
        
        #endregion
       
    }
}
