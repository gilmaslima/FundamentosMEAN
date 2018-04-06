#region Histórico do Arquivo
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   :
- [31/05/2012] – [André Garcia] – [Criação]
*/
/*
(c) Copyright [2012] Redecard S.A.
Autor       : [André Garcia]
Empresa     : [Iteris]
Histórico   : Inclusão do método "ValidarConfirmacaoPositiva"
- [31/05/2012] – [André Garcia] – [Alteração]
*/
#endregion

using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using System.ServiceModel;

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
    public class ConfirmacaoPositiva : ApplicationPageBaseAnonima
    {
        #region Controles
        /// <summary>
        /// 
        /// </summary>
        protected Redecard.PN.DadosCadastrais.SharePoint.ConfirmacaoPositiva ctrlConfirmacaoPositiva;
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
        protected Panel painelSteps1;
        /// <summary>
        /// 
        /// </summary>
        protected Panel painelSteps2;
        /// <summary>
        /// 
        /// </summary>
        protected Literal ltErro;
        /// <summary>
        /// 
        /// </summary>
        protected DropDownList ddlBancoCredito;
        /// <summary>
        /// 
        /// </summary>
        protected Panel pnlBloqueado;
        /// <summary>
        /// 
        /// </summary>
        protected QuadroAviso quadroAviso;
        #endregion

        /// <summary>
        /// Verificar se o usuário realizou a confirmação positiva com sucesso.
        /// </summary>
        private void ValidarConfirmacaoPositiva()
        {
            try
            {
                if (!InformacaoUsuario.Existe())
                {
                    this.ExibirErro("Não é possível a abertura da tela de confirmação positiva manualmente, tente efetuar login novamente.");
                }
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                ExibirErro(FONTE, CODIGO_ERRO, String.Empty);
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
            ctrlConfirmacaoPositiva.pnlPagina1.Visible = false;
            ctrlConfirmacaoPositiva.pnlPagina2.Visible = false;
        }

        /// <summary>
        /// Carregamento da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                    this.ValidarConfirmacaoPositiva();

                // atachar evento de enviar dados do controle de validação positiva
                ctrlConfirmacaoPositiva.EnviarClick += new SharePoint.ConfirmacaoPositiva.EnviarClickHandle(Confirmar);
                ctrlConfirmacaoPositiva.VoltarClick += new SharePoint.ConfirmacaoPositiva.VoltarClickHandle(Voltar);
                ctrlConfirmacaoPositiva.ProximaPaginaClick += new SharePoint.ConfirmacaoPositiva.ProximaPaginaClickHandle(ProximaPagina);

                // Verificar dados em sessão
                this.VerificarSessaoDadosUsuario();
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                ExibirErro(FONTE, CODIGO_ERRO, String.Empty);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ProximaPagina(object sender, EventArgs e)
        {
            painelSteps1.Visible = false;
            painelSteps2.Visible = true;
        }

        /// <summary>
        /// Voltar para a página inicial de confirmação positiva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Voltar(object sender, EventArgs e)
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/_layouts/DadosCadastrais/ConfirmacaoPositiva.aspx", SPUrlZone.Internet);
            Response.Redirect(url, true);
        }

        /// <summary>
        /// Verifica a sessão do usuário
        /// </summary>
        private void VerificarSessaoDadosUsuario()
        {
            if (!InformacaoUsuario.Existe())
            {
                pnlErroPrincipal.Visible = true;
                ctrlConfirmacaoPositiva.pnlPagina1.Visible = false;
                ctrlConfirmacaoPositiva.pnlPagina2.Visible = false;
            }
        }

        /// <summary>
        /// Exibe erro no painel
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        private void ExibirErro(String fonte, Int32 codigo, String mensagemAdicional)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);
            if (!String.IsNullOrEmpty(mensagem))
            {
                mensagem += "<br /><br />" + mensagemAdicional;
            }
            //quadroAviso.CarregarImagem("/_layouts/DadosCadastrais/IMAGES/CFP/Ico_Alerta.png");
            quadroAviso.CarregarMensagem("Atenção, dados inválidos.", mensagem);
            pnlErroPrincipal.Visible = true;
            pnlBotaoErro.Visible = true;
            ctrlConfirmacaoPositiva.Visible = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Confirmar(object sender, EventArgs e)
        {
            try
            {
                // ocorre quando o botão "Enviar Dados" do controle de validação e clicado. Chama o método
                // Validar() do controle para verificar se os dados estão corretos e validados
                int codigoRetorno = ctrlConfirmacaoPositiva.ValidarCamposVariaveis();
                if (codigoRetorno == 0)
                {
                    // a validação ocorreu com sucesso
                    if (InformacaoUsuario.Existe())
                    {
                        InformacaoUsuario usuario = InformacaoUsuario.Recuperar();
                        usuario.Confirmado = true;
                        InformacaoUsuario.Salvar(usuario);
                        Response.Redirect("AlteracaoSenha.aspx");
                    }
                }
                else
                {
                    using (UsuarioServico.UsuarioServicoClient client = new UsuarioServico.UsuarioServicoClient())
                    {
                        InformacaoUsuario usuario = InformacaoUsuario.Recuperar();
                        Int32 codigoRetornoBloqueio = 0;

                        UsuarioServico.Entidade entidade = new UsuarioServico.Entidade()
                        {
                            GrupoEntidade = new UsuarioServico.GrupoEntidade()
                            {
                                Codigo = usuario.GrupoEntidade
                            },
                            Codigo = usuario.NumeroPV
                        };
                        var usuarios = client.ConsultarPorCodigoEntidade(out codigoRetornoBloqueio, usuario.Usuario, entidade);
                        if (usuarios.Length > 0)
                        {
                            UsuarioServico.Usuario _usuario = usuarios[0];
                            if (_usuario.BloqueadoConfirmacaoPositiva)
                                ExibirTelaBloqueio();
                            else
                            {
                                Int32 codigoRetornoQuantidade = 0;
                                String mensagem = String.Empty;
                                codigoRetornoQuantidade = RecuperarQuantidadeTentivas(out mensagem);
                                if (codigoRetornoQuantidade > 0)
                                    this.ExibirErro("SharePoint.RecuperarDados", codigoRetornoQuantidade, mensagem);
                                else
                                    this.ExibirErro("SharePoint.RecuperarDados", codigoRetorno, mensagem);
                            }
                        }
                    }
                    
                    // pesquisar erros da validação positiva
                    //ltErro.Text = "Ocorreu um erro nos dados informados, por favor, feche a janela do navegador e tente novamente";
                }
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32(), String.Empty);
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32(), String.Empty);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                this.ExibirErro(FONTE, CODIGO_ERRO, String.Empty);
            }
        }

        /// <summary>
        /// Exibe a tela de Usuário Bloqueado caso o usuário já tenham tentado realizar 6 vezes a 
        /// Confirmação pPositiva.
        /// </summary>
        private void ExibirTelaBloqueio()
        {
            pnlErroPrincipal.Visible = false;
            pnlBotaoErro.Visible = false;
            pnlBloqueado.Visible = true;
            ctrlConfirmacaoPositiva.Visible = false;
        }

        /// <summary>
        /// Voltar para o inicio do processo
        /// </summary>
        protected void VoltarHome(Object sender, EventArgs e)
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            Response.Redirect(url, true);
        }

        /// <summary>
        /// Recuperar a quantidade de tentativas da confirmação positiva
        /// </summary>
        private Int32 RecuperarQuantidadeTentivas(out String mensagem)
        {
            Int32 codigoRetorno = 0;
            mensagem = String.Empty;

            // Recuperar a quantidade restantes de Confirmação Positiva
            if (InformacaoUsuario.Existe())
            {
                InformacaoUsuario usuario = InformacaoUsuario.Recuperar();
                using (UsuarioServico.UsuarioServicoClient client = new UsuarioServico.UsuarioServicoClient())
                {
                    UsuarioServico.Entidade entidade = new UsuarioServico.Entidade()
                    {
                        Codigo = usuario.NumeroPV,
                        GrupoEntidade = new UsuarioServico.GrupoEntidade()
                        {
                            Codigo = usuario.GrupoEntidade
                        }
                    };
                    var usuarios = client.ConsultarPorCodigoEntidade(out codigoRetorno, usuario.Usuario, entidade);
                    if (codigoRetorno == 0 && usuarios.Length > 0)
                    {
                        var _usuario = usuarios[0];
                        if (!_usuario.BloqueadoConfirmacaoPositiva)
                        {
                            mensagem = String.Format("Você ainda possui <b>{0}</b> tentativas.", 6 - _usuario.QuantidadeTentativaConfirmacaoPositiva);
                        }
                    }
                }
            }
            return codigoRetorno;
        }
    }
}
