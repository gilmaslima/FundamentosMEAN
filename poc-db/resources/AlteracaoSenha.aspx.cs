#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [31/05/2012] – [André Garcia] – [Criação]
*/
#endregion

using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using Microsoft.SharePoint.IdentityModel;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.Login;

using Redecard.PN.Comum;
using System.ServiceModel;
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
    public class AlteracaoSenha : ApplicationPageBaseAnonima
    {
        #region Controles
        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlConfirmarLogin;
        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlErroPrincipal;
        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlBotaoErro;
        /// <summary>
        /// 
        /// </summary>
        protected Literal ltErro;
        /// <summary>
        /// 
        /// </summary>
        protected SharePoint.AlteracaoSenha crtlAlteracaoSenha;
        /// <summary>
        /// 
        /// </summary>
        protected QuadroAviso quadroAviso;
        #endregion

        /// <summary>
        /// Voltar para a página inicial de confirmação positiva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Voltar(object sender, EventArgs e)
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/_layouts/DadosCadastrais/AlteracaoSenha.aspx", SPUrlZone.Internet);
            Response.Redirect(url, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                this.ValidarConfirmacaoPositiva();

            if (crtlAlteracaoSenha.Visible)
            {
                crtlAlteracaoSenha.SenhaTrocada += new SharePoint.AlteracaoSenha.TrocarSenhaClickHandle(SenhaTrocada);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="codigoRetorno"></param>
        void SenhaTrocada(object sender, Int32 codigoRetorno)
        {
            try
            {
                if (codigoRetorno > 0)
                    pnlErroPrincipal.Controls.Add(base.RetornarPainelExcecao("UsuarioServico.AtualizarSenha", codigoRetorno));
                else
                    // redirecionar para o portal fechado automaticamente
                    this.EfetuarLogin();
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                pnlErroPrincipal.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                pnlErroPrincipal.Controls.Add(base.RetornarPainelExcecao("Redecard.PN.SharePoint", 300));
            }
        }

        /// <summary>
        /// Faz o login com os novos dados de acesso e redireciona o usuário para o portal
        /// de serviços
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EfetuarLogin()
        {
            if (InformacaoUsuario.Existe())
            {
                try
                {
                    InformacaoUsuario dadosUsuarioCP = InformacaoUsuario.Recuperar();

                    String codigoGrupoEntidade = dadosUsuarioCP.GrupoEntidade.ToString();
                    String codigoEntidade = dadosUsuarioCP.NumeroPV.ToString();
                    String codigoNomeUsuario = dadosUsuarioCP.Usuario;
                    String senha = EncriptadorSHA1.EncryptString(dadosUsuarioCP.Senha);

                    String _loginNameFormat = "{0};{1};{2}";
                    String loginName = String.Format(_loginNameFormat, codigoGrupoEntidade, codigoEntidade, codigoNomeUsuario);

                    // chama métodos de login adicionais
                    if (SPClaimsUtility.AuthenticateFormsUser(Request.Url, loginName, senha))
                    {
                        // Usuário Autenticado, criar objeto de sessão no AppFabric e redirecionar
                        // para o Portal de Serviços
                        LoginClass login = new LoginClass();
                        login.CriarSessaoUsuario(codigoNomeUsuario, dadosUsuarioCP.GrupoEntidade, dadosUsuarioCP.NumeroPV, senha);

                        // Redirecionar o usuário para o Portal Fechado
                        pnlConfirmarLogin.Visible = true;
                        InformacaoUsuario.Limpar();
                    }
                }
                catch(Exception) {
                    ExibirErro("Ocorreu um erro na tentativa de login, por favor, feche esta janela e tente novamente.");
                }
            }
        }

        /// <summary>
        /// Verificar se o usuário realizou a confirmação positiva com sucesso.
        /// </summary>
        private void ValidarConfirmacaoPositiva()
        {
            if (InformacaoUsuario.Existe())
            {
                InformacaoUsuario dados = InformacaoUsuario.Recuperar();
                if (!dados.Confirmado)
                {
                    this.ExibirErro("Você precisa efetuar a confirmação positiva para trocar a senha.");
                }
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
            pnlBotaoErro.Visible = true;
            crtlAlteracaoSenha.Visible = false;
        }
    }
}
