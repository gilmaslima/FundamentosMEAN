#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [06/05/2012] – [André Garcia] – [Criação]
*/
#endregion

using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using Redecard.PN.Comum;
using Microsoft.SharePoint.Utilities;
using System.Web;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Redecard.PN.DadosCadastrais.SharePoint.Layouts.DadosCadastrais
{
    /// <summary>
    /// Página que realiza a confirmação positiva do usuário antes do login
    /// </summary>
    public class ConfirmarEmail : ApplicationPageBaseAnonima
    {
        #region Controles
        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlErroPrincipal;
        /// <summary>
        /// 
        /// </summary>
        protected Literal ltErro;
        /// <summary>
        /// 
        /// </summary>
        protected SharePoint.ConfirmarEmail ctrlConfirmarEmail;
        /// <summary>
        /// 
        /// </summary>
        protected Literal ltTipoRecuperacao;
        /// <summary>
        /// 
        /// </summary>
        protected QuadroAviso quadroAviso;
        /// <summary>
        /// 
        /// </summary>
        protected Literal ltTipoRecuperacaoTitulo;
        #endregion

        /// <summary>
        /// Verificar se o usuário realizou a confirmação positiva com sucesso.
        /// </summary>
        private void ValidarConfirmacaoPositiva()
        {
            try
            {
                if (InformacaoUsuario.Existe())
                {
                    InformacaoUsuario dados = InformacaoUsuario.Recuperar();

                    if (!dados.Confirmado)
                        ExibirErro("Não é possível a abertura da tela de confirmação positiva manualmente, tente efetuar login novamente.");
                }
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                ExibirErro(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Voltar para o inicio do processo
        /// </summary>
        protected void Voltar(Object sender, EventArgs e)
        {
            String url = String.Empty;
            if (InformacaoUsuario.Existe())
            {
                InformacaoUsuario usuario = InformacaoUsuario.Recuperar();
                if (usuario.EsqueciUsuario)
                    url = Util.BuscarUrlRedirecionamento("/_layouts/DadosCadastrais/EsqueciUsuario.aspx", SPUrlZone.Internet);
                else
                    url = Util.BuscarUrlRedirecionamento("/_layouts/DadosCadastrais/EsqueciSenha.aspx", SPUrlZone.Internet);
            }
            else
            {
                url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            }
            if (!String.IsNullOrEmpty(url))
            {
                Response.Redirect(url, true);
            }
        }

        /// <summary>
        /// Exibe painel de erro customizado
        /// </summary>
        /// <param name="erro">Mensagem de erro</param>
        private void ExibirErro(String erro)
        {
            //quadroAviso.CarregarImagem("/_layouts/DadosCadastrais/IMAGES/CFP/Ico_Alerta.png");
            quadroAviso.CarregarMensagem("Atenção, dados inválidos.", erro);
            pnlErroPrincipal.Visible = true;
            ctrlConfirmarEmail.pnlPagina1.Visible = false;
        }

        /// <summary>
        /// Exibe erro no painel
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        private void ExibirErro(String fonte, Int32 codigo)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            //quadroAviso.CarregarImagem("/_layouts/DadosCadastrais/IMAGES/CFP/Ico_Alerta.png");
            quadroAviso.CarregarMensagem("Atenção, dados inválidos.", mensagem);
            pnlErroPrincipal.Visible = true;
            ctrlConfirmarEmail.pnlPagina1.Visible = false;
        }

        /// <summary>
        /// Carregamento da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                this.ValidarConfirmacaoPositiva();

            if (InformacaoUsuario.Existe())
            {
                InformacaoUsuario info = InformacaoUsuario.Recuperar();
                if (info.EsqueciUsuario)
                {
                    ltTipoRecuperacaoTitulo.Text = "meu Usuário";
                    ltTipoRecuperacao.Text = "Usuário";
                }
                else
                {
                    ltTipoRecuperacaoTitulo.Text = "minha Senha";
                    ltTipoRecuperacao.Text = "Senha";
                }
            }

            // Evento executado quando ocorre a confirmação do E-mail
            ctrlConfirmarEmail.ConfirmarEmailClick += new SharePoint.ConfirmarEmail.ConfirmarEmailHandle(EmailConfirmado);
        }

        /// <summary>
        /// Evento disparado após a confirmacão de e-mail ou carta.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="EmailConfirmado"></param>
        void EmailConfirmado(object sender, int tipoConfirmacao, string emailConfirmado)
        {
            // atualizar sessão temporária e enviar para a página seguinte
            if (InformacaoUsuario.Existe())
            {
                if (tipoConfirmacao == 1) // Enviar por e-mail
                {
                    InformacaoUsuario _dados = InformacaoUsuario.Recuperar();
                    _dados.EmailEntidade = emailConfirmado;
                    InformacaoUsuario.Salvar(_dados);

                    // enviar para a tela seguinte
                    Response.Redirect("Concluido.aspx");
                }
                else
                {
                    // enviar para a tela seguinte
                    Response.Redirect("ConcluidoCarta.aspx");
                }
            }
            else
                this.ExibirErro("Ocorreu um erro na validação da sessão, por favor, tente novamente.");
        }

        /// <summary>
        /// 
        /// </summary>
        private void VerificarSessaoDadosUsuario()
        {
            if (!InformacaoUsuario.Existe())
                pnlErroPrincipal.Visible = true;
        }
    }
}