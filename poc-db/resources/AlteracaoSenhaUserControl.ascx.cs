/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using System.Linq;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.MeuUsuario.AlteracaoSenha
{
    /// <summary>
    /// Controle de Alteração de Senha do Usuário - Meu Usuário
    /// </summary>
    public partial class AlteracaoSenhaUserControl : MeuUsuarioUserControlBase
    {
        #region [ Controles ]

        /// <summary>
        /// txtSenhaAtual control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected CampoNovoAcesso TxtSenhaAtual { get { return (CampoNovoAcesso)txtSenhaAtual; } }

        /// <summary>
        /// txtNovaSenha control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected CampoNovoAcesso TxtNovaSenha { get { return (CampoNovoAcesso)txtNovaSenha; } }

        /// <summary>
        /// txtNovaSenhaConfirmacao control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected CampoNovoAcesso TxtNovaSenhaConfirmacao { get { return (CampoNovoAcesso)txtNovaSenhaConfirmacao; } }

        #endregion

        #region [ Eventos de Página ]

        /// <summary>
        /// Load da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Form.DefaultButton = btnConfirmar.UniqueID;
        }

        #endregion

        #region [ Eventos dos Controles ]

        /// <summary>
        /// Clique no botão Cancelar, retornando para a tela inicial de "Meu Usuário"
        /// </summary>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            base.RedirecionarParaMeuUsuario(false, false);
        }

        /// <summary>
        /// Clique no botão Confirmar, efetuando a alteração da senha do usuário
        /// </summary>
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            Boolean valido = CampoNovoAcesso.Validar(true, txtSenhaAtual, txtNovaSenha, txtNovaSenhaConfirmacao);

            if (valido)
                this.AlterarSenha();                           
        }

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Altera a senha do usuário
        /// </summary>
        private void AlterarSenha()
        {
            String senhaAtual = TxtSenhaAtual.Text;

            //Verifica se a senha atual com a senha do usuário
            if (base.VerificarSenhaAtual(senhaAtual))
            {
                String novaSenha = CampoSenha.Criptografar(TxtNovaSenha.Text);
                Int32 codigoRetorno = default(Int32);

                using (Logger log = Logger.IniciarLog("Atualização de Senha do Usuário Atual"))
                {
                    try
                    {
                        //Verifica se algum estabelecimento associado ao usuário possui Komerci
                        Boolean possuiKomerci = SessaoAtual.PossuiKomerci;
                        using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        {                            
                            EntidadeServico.Entidade1[] entidadesUsuario = ctx.Cliente.ConsultarPorUsuario(
                                out codigoRetorno, this.UsuarioAtual.CodigoIdUsuario);
                            if(codigoRetorno == 0 && entidadesUsuario != null && entidadesUsuario.Length > 0)
                                possuiKomerci = 
                                    ctx.Cliente.PossuiKomerci(entidadesUsuario.Select(ent => ent.Codigo).ToArray());
                        }

                        using (var ctx = new ContextoWCF<UsuarioServicoClient>())                        
                            codigoRetorno = ctx.Cliente.AtualizarSenha(
                                this.UsuarioAtual, novaSenha, possuiKomerci, false);

                        if (codigoRetorno > 0)
                            base.ExibirPainelExcecao("UsuarioServico.AtualizarSenha", codigoRetorno);
                        else
                        {
                            //Armazena no histórico/log de atividades
                            Historico.AlteracaoSenha(SessaoAtual);

                            this.RedirecionarParaMeuUsuario(true, false);
                        }
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
            }
            else
                TxtSenhaAtual.ExibirMensagemErro("Senha incorrta");
        }

        #endregion
    }
}