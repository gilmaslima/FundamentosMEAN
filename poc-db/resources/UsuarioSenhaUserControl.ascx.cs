using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.ServiceModel;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

using Redecard.PN.Comum;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.UsuarioSenha
{
    /// <summary>
    /// Webpart de Alteração de senha do usuário logado
    /// </summary>
    public partial class UsuarioSenhaUserControl : UserControlBase
    {

        /// <summary>
        /// Inicialização da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Sessao.Contem())
                {
                    lblUsuarioAtual.Text = SessaoAtual.LoginUsuario;

                    this.CarregarValidacoes();
                }
            }
        }

        /// <summary>
        /// Configuração de validações para entidades Komerci
        /// </summary>
        private void CarregarValidacoes()
        {
            using (Logger log = Logger.IniciarLog("Início configuração de validações para entidades Komerci"))
            {
                try
                {
                    if (SessaoAtual.PossuiKomerci)
                    {
                        lblMensagemSenha.Text = "Utilize de 8 a 20 dígitos, sendo números e letras, com no máximo dois caracteres repetidos e não pode ter nenhum espaço, caracteres especiais ou acentuação(ex: &, >, *, $, @, ç, á).";

                        lblMensagemAcesso.Text = SessaoAtual.SenhaMigrada ? "Altere periodicamente sua senha. Isso garante mais segurança à sua empresa." : "O acesso ao Portal Redecard mudou. Confirme os dados abaixo e cadastre uma nova senha.";

                        lblMensagemHistoricoSenha.Visible = true;

                        revTamanhoSenha.ValidationExpression = "[\\w\\s]{8,20}";
                        revTamanhoSenha.Text = "A senha deve conter, no mínimo, 8 caracteres, e, no máximo, 20 caracteres e sem caracteres especiais.";

                        revLetraNumeroSenha.ValidationExpression = "^.*(?=.{8,20})(?=.*[a-zA-Z])(?=.*[0-9]).*$";
                    }
                    else
                    {
                        lblMensagemSenha.Text = "Utilize de 6 a 20 dígitos, sendo números e letras, com no máximo dois caracteres repetidos e não pode ter nenhum espaço, caracteres especiais ou acentuação(ex: &, >, *, $, @, ç, á).";

                        lblMensagemHistoricoSenha.Visible = false;

                        lblMensagemAcesso.Text = "Altere periodicamente sua senha. Isso garante mais segurança à sua empresa.";

                        revTamanhoSenha.ValidationExpression = "[\\w\\s]{6,20}";
                        revTamanhoSenha.Text = "A senha deve conter, no mínimo, 6 caracteres, e, no máximo, 20 caracteres e sem caracteres especiais.";

                        revLetraNumeroSenha.ValidationExpression = "^.*(?=.{6,20})(?=.*[a-zA-Z])(?=.*[0-9]).*$";
                    }
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Gravar a nova senha do usuário
        /// </summary>
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Gravar nova senha do usuário"))
            {
                try
                {
                    Int32 codigoRetorno;

                    using (var usuarioClient = new UsuarioServico.UsuarioServicoClient())
                    {
                        // Consulta usuário
                        var usuario = usuarioClient.ConsultarPorID(out codigoRetorno, SessaoAtual.CodigoIdUsuario);

                        if (codigoRetorno > 0)
                            base.ExibirPainelExcecao("UsuarioServico.Consultar", codigoRetorno);
                        else
                        {
                            String _senhaAtual = null;

                            if (!String.IsNullOrEmpty(txtSenhaAtual.Text))
                                _senhaAtual = EncriptadorSHA1.EncryptString(txtSenhaAtual.Text);

                            if (usuario.Senha != _senhaAtual)
                            {
                                base.ExibirPainelExcecao("SharePoint.UsuarioSenha", 1362);
                                return;
                            }

                            String _senhaNova = null;

                            if (!String.IsNullOrEmpty(txtNovaSenha.Text))
                                _senhaNova = EncriptadorSHA1.EncryptString(txtNovaSenha.Text);

                            if (_senhaNova == null ||
                                 usuario.Senha.Trim().ToUpper() == _senhaNova.ToUpper() ||
                                 usuario.SenhaTemporaria.Trim().ToUpper() == _senhaNova.ToUpper())
                                base.ExibirPainelExcecao("SharePoint.UsuarioSenha", 1363);


                            Panel[] paineis = new Panel[1]{
                            pnlAlterarSenha
                        };

                            codigoRetorno = usuarioClient.AtualizarSenha(usuario, _senhaNova, this.SessaoAtual.PossuiKomerci, false);

                            if (codigoRetorno > 0)
                                base.ExibirPainelExcecao("UsuarioServico.AtualizarSenha", codigoRetorno);
                            else
                            {
                                String linkRedirecionar = SessaoAtual.PossuiKomerci && !SessaoAtual.SenhaMigrada ? "/sites/fechado" : SPUtility.GetPageUrlPath(HttpContext.Current);
                                SessaoAtual.SenhaMigrada = true;
                                
                                base.ExibirPainelConfirmacaoAcao("Alteração de Senha", "Operação executada com sucesso.", linkRedirecionar, paineis);
                            }

                            //base.Alert("Operação executada com sucesso", SPUtility.GetPageUrlPath(HttpContext.Current));
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        
        /// <summary>
        /// Valida os caracteres repetidos na senha
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ctvCaracterRepetido_ServerValidate(object source, ServerValidateEventArgs args)
        {
            String senha = args.Value;
            Char caracter1 = args.Value.ToString()[0];
            Char caracter2 = args.Value.ToString()[1];
            
            for (int i = 2; i < args.Value.Length; i++)
            {
                if (senha[i].Equals(caracter1) && senha[i].Equals(caracter2))
                {
                    args.IsValid = false;
                    return;
                }
                else
                {
                    caracter1 = senha[i - 2];
                    caracter2 = senha[i - 1];
                }
            }
            args.IsValid = true;
        }
    }
}
