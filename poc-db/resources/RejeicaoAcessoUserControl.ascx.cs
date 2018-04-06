using Redecard.PN.Comum;
using System;
using System.ServiceModel;
using System.Web;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios.RejeicaoAcesso
{
    public partial class RejeicaoAcessoUserControl : UsuariosUserControlBase
    {
        
        /// <summary>
        /// Inicialização da WebPart de Rejeição de Acesso
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Inicialização da WebPart de Rejeição de Acesso"))
            {
                try
                {
                    //Verifica permissão (apenas Master e Central de Atendimento podem acessar)
                    if (!base.PossuiPermissao)
                    {
                        pnlAcessoNegado.Visible = true;
                        pnlQuadroAcesso.Visible = false;
                        return;
                    }

                    if (this.UsuarioSelecionado != null)
                    {
                        this.CarregarDadosUsuario();
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Cancela a rejeição da Solicitação do Usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Cancela a rejeição da Solicitação do Usuário"))
            {
                try
                {
                    String url = "Usuarios.aspx"; // aprovação de acessos fica junto com a tela adm de usuarios

                    this.Response.Redirect(url, false);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Confirmar a rejeição da Solicitação de Acesso
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Cancela a rejeição da Solicitação do Usuário"))
            {
                try
                {
                    using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    {
                        Int32 codigoRetorno = 0;
                        codigoRetorno = contextoUsuario.Cliente.ExcluirEmLoteNovoAcesso(this.UsuarioSelecionado.CodigoIdUsuario.ToString());

                        if (codigoRetorno > 0)
                        {
                            base.ExibirPainelExcecao("UsuarioServico.ExcluirEmLote", codigoRetorno);
                        }
                        else
                        {                            
                            //Registra no histórico/log de atividades
                            Historico.RejeicaoUsuario(SessaoAtual,
                                this.UsuarioSelecionado.CodigoIdUsuario.Value,
                                this.UsuarioSelecionado.Nome,
                                this.UsuarioSelecionado.Email,
                                this.UsuarioSelecionado.TipoUsuario);

                            //Rejeição de usuário realiza exclusão do usuário
                            Historico.ExclusaoUsuario(SessaoAtual,
                                this.UsuarioSelecionado.CodigoIdUsuario.Value,
                                this.UsuarioSelecionado.Nome,
                                this.UsuarioSelecionado.Email,
                                this.UsuarioSelecionado.TipoUsuario);

                            String justificativa = txtJustificativa.Text.Trim();
                            justificativa = justificativa.Replace(Environment.NewLine, "<br/>");

                            EmailNovoAcesso.EnviarEmailSolicitacaoAcessoRejeitada(
                                SessaoAtual,
                                this.UsuarioSelecionado.Email, 
                                justificativa);

                            String url = "Usuarios.aspx"; // aprovação de acessos fica junto com a tela adm de usuarios
                            this.Response.Redirect(url, false);
                        }
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carregar o Nome e Email do Usuário na tabela de identificação
        /// </summary>
        private void CarregarDadosUsuario()
        {
            lblEmailUsurio.Text = this.UsuarioSelecionado.Email;
            lblNomeUsuario.Text = this.UsuarioSelecionado.Nome;
        }
    }
}