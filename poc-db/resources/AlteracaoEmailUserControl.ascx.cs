/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.MeuUsuario.AlteracaoEmail
{
    /// <summary>
    /// Controle de Alteração do E-mail do Usuário - Meu Usuário
    /// </summary>
    public partial class AlteracaoEmailUserControl : MeuUsuarioUserControlBase
    {
        #region [ Controles ]

        /// <summary>
        /// txtNovoEmail control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected CampoNovoAcesso TxtNovoEmail { get { return (CampoNovoAcesso)txtNovoEmail; } }

        /// <summary>
        /// txtNovoEmailConfirmacao control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected CampoNovoAcesso TxtNovoEmailConfirmacao { get { return (CampoNovoAcesso)txtNovoEmailConfirmacao; } }

        /// <summary>
        /// txtSenha control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected CampoNovoAcesso TxtSenha { get { return (CampoNovoAcesso)txtSenha; } }

        /// <summary>
        /// ucQuadroAviso control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected QuadroAviso UcQuadroAviso { get { return (QuadroAviso)ucQuadroAviso; } }

        #endregion
      
        #region [ Campos (Controles Novo Acesso) ]

        /// <summary>
        /// Campo Novo E-mail
        /// </summary>
        private CampoEmail CampoNovoEmail { 
            get { return TxtNovoEmail.Campo as CampoEmail; } 
        }

        /// <summary>
        /// Campo Novo E-mail Confirmação
        /// </summary>
        private CampoConfirmacaoEmail CampoNovoEmailConfirmacao { 
            get { return TxtNovoEmailConfirmacao.Campo as CampoConfirmacaoEmail; } 
        }

        /// <summary>
        /// Campo Senha
        /// </summary>
        private CampoTexto CampoSenha {
            get { return TxtSenha.Campo as CampoTexto; }
        }

        #endregion

        #region [ Eventos de Página ]

        /// <summary>
        /// Load da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Botão default
            this.Page.Form.DefaultButton = btnConfirmar.UniqueID;
        }
      
        #endregion

        #region [ Eventos dos Controles ]

        /// <summary>
        /// Clique no botão Cancelar, retornando para a tela inicial de Meu Usuário
        /// </summary>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            base.RedirecionarParaMeuUsuario(false, false);
        }

        /// <summary>
        /// Clique no botão de confirmação
        /// </summary>
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            CampoEmail campoNovoEmail = TxtNovoEmail.ObterCampo<CampoEmail>();
            Boolean valido = CampoNovoAcesso.ValidarCampos(txtNovoEmailConfirmacao, txtSenha)
                && campoNovoEmail.Validar(true, this.EstabelecimentosUsuarioAtual);
            String emailAtual = this.UsuarioAtual.Email.Trim();
            String novoEmail = campoNovoEmail.Email.Trim();

            if (valido)
            {
                //Verifica se o e-mail foi atualizado (diferente do atual)                
                if (String.Compare(emailAtual, novoEmail, true) == 0)
                {
                    TxtNovoEmail.ExibirMensagemAviso("O e-mail deve ser diferente do atual");
                    return;
                }

                //Verifica se a senha atual com a senha do usuário
                if (base.VerificarSenhaAtual(TxtSenha.Text))
                {
                    //Atualiza e-mail, se sucesso, exibe aviso de confirmação de alteração de e-mail
                    Guid hashConfirmacaoEmail = default(Guid);
                    Boolean emailAtualizado = this.AtualizarEmail(this.UsuarioAtual.CodigoIdUsuario, 
                        novoEmail, out hashConfirmacaoEmail);
                    if (emailAtualizado)
                    {
                        //Armazena no histórico/log de atividade
                        Historico.AlteracaoEmail(SessaoAtual, novoEmail);

                        //Envia e-mail de confirmação para o usuário, expira em 48h
                        EmailNovoAcesso.EnviarEmailConfirmacaoCadastro48h(novoEmail, 
                            this.UsuarioAtual.CodigoIdUsuario, hashConfirmacaoEmail,
                            this.UsuarioAtual.CodigoIdUsuario, this.UsuarioAtual.Email,
                            this.UsuarioAtual.Descricao, this.UsuarioAtual.TipoUsuario,
                            SessaoAtual.CodigoEntidade, SessaoAtual.Funcional);
                        this.ExibirMensagemSucesso();
                    }
                }
                else
                {
                    //Senha incorreta
                    TxtSenha.ExibirMensagemErro("Senha incorreta");
                }
            }
        }

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Atualiza e-mail do usuário logado
        /// </summary>
        private Boolean AtualizarEmail(Int32 codigoIdUsuario, String novoEmail, out Guid hashConfirmacaoEmail)
        {
            Boolean sucesso = false;            
            hashConfirmacaoEmail = default(Guid);

            using (Logger log = Logger.IniciarLog("Alteração de E-mail"))
            {
                try
                {
                    Int32 codigoRetorno = default(Int32);

                    using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                        codigoRetorno = ctx.Cliente.AtualizarEmail(out hashConfirmacaoEmail, codigoIdUsuario, novoEmail, 2, DateTime.Now);

                    if (codigoRetorno != 0)
                        base.ExibirPainelExcecao("UsuarioServico.AtualizarEmail", codigoRetorno);
                    else
                        sucesso = true;
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());                    
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
            return sucesso;
        }

        /// <summary>
        /// Exibe mensagem de sucesso na alteração de e-mail
        /// </summary>
        private void ExibirMensagemSucesso()
        {
            String titulo = "E-mail alterado com sucesso.";
            String mensagem = String.Concat(
                "Dentro de instantes você receberá no novo endereço de e-mail a solicitação para confirmação desta alteração.<br/><br/>",
                "Você deverá acessar o link que foi enviado em até 48h para concluir a alteração.");
            String classeImagem = "icone-green";

            String url = String.Format("{0}/Paginas/MeuUsuario.aspx", base.web.ServerRelativeUrl);
            
            var qs = new QueryStringSegura();
            qs["ExibirMensagemSucesso"] = Boolean.TrueString;
            url = String.Format("{0}?dados={1}", url, qs.ToString());

            UcQuadroAviso.CarregarMensagem(titulo, mensagem, url, classeImagem);
            mviewAlteracaoEmail.SetActiveView(pnlQuadroAviso);
        }

        #endregion
    }
}