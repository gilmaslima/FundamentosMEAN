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
    public class RecuperarDadosConfirmacao : ApplicationPageBaseAnonima
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
        protected Literal ltTipoRecuperacao;
        /// <summary>
        /// 
        /// </summary>
        protected Literal ltTipoRecuperacaoLabel;
        /// <summary>
        /// 
        /// </summary>
        protected Literal ltTipoRecuperacaoTitulo;
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
        /// Voltar para o inicio do processo
        /// </summary>
        protected void VoltarHome(Object sender, EventArgs e)
        {
            String url = String.Empty;
            url = Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            Response.Redirect(url, true);
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
        /// Carregamento da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Verificar se o usuário realizou a confirmação positiva com sucesso.
                this.VerificarSessaoDadosUsuario();
            }

            if (InformacaoUsuario.Existe())
            {
                InformacaoUsuario info = InformacaoUsuario.Recuperar();
                if (info.EsqueciUsuario)
                {
                    ltTipoRecuperacao.Text = "Usuário";
                    ltTipoRecuperacaoLabel.Text = "do seu usuário";
                    ltTipoRecuperacaoTitulo.Text = "meu Usuário";
                }
                else
                {
                    ltTipoRecuperacao.Text = "Senha";
                    ltTipoRecuperacaoLabel.Text = "da sua senha";
                    ltTipoRecuperacaoTitulo.Text = "minha Senha";
                }
            }

            // atachar evento de enviar dados do controle de validação positiva
            ctrlConfirmacaoPositiva.EnviarClick += new SharePoint.ConfirmacaoPositiva.EnviarClickHandle(Confirmar);
            ctrlConfirmacaoPositiva.VoltarClick += new SharePoint.ConfirmacaoPositiva.VoltarClickHandle(Voltar);
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
        /// 
        /// </summary>
        private void VerificarSessaoDadosUsuario()
        {
            if (!InformacaoUsuario.Existe())
                this.ExibirErro("SharePoint.RecuperarDadosConfirmacao", 1050, String.Empty);
        }

        /// <summary>
        /// Exibe erro no painel
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        private void ExibirErro(String fonte, Int32 codigo, String mensagemAdicional)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);

            if (!String.IsNullOrEmpty(mensagemAdicional))
            {
                mensagem += "<br /><br />" + mensagemAdicional;
            }
            //quadroAviso.CarregarImagem("/_layouts/DadosCadastrais/IMAGES/CFP/Ico_Alerta.png");
            quadroAviso.CarregarMensagem("Atenção, dados inválidos.", mensagem);
            pnlErroPrincipal.Visible = true;
            pnlBotaoErro.Visible = true;
            ctrlConfirmacaoPositiva.pnlPagina1.Visible = false;
            ctrlConfirmacaoPositiva.pnlPagina2.Visible = false;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Confirmar(object sender, EventArgs e)
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
                        Response.Redirect("ConfirmarEmail.aspx");
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
                ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32(), String.Empty);
            }
            catch (FaultException<UsuarioServico.GeneralFault> ex)
            {
                ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32(), String.Empty);
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                this.ExibirErro(FONTE, CODIGO_ERRO, String.Empty);
            }
        }
    }
}