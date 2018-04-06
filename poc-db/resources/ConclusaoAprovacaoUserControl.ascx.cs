using Redecard.PN.Comum;
using System;
using System.ServiceModel;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios.ConclusaoAprovacao
{
    public partial class ConclusaoAprovacaoUserControl : UsuariosUserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Verifica permissão (apenas Master e Central de Atendimento podem acessar)
            if (!base.PossuiPermissao)
            {
                pnlConfirmacao.Visible = false;
                qdAcessoNegado.Visible = true;
                return;
            }

            if (!IsPostBack)
            {
                if (ModoAprovacao)
                    this.AprovarUsuario();
            }
        }


        #region [ Métodos privados ]

        /// <summary>
        /// Atualização do Usuário como aprovado
        /// </summary>
        private void AprovarUsuario()
        {
            using (Logger log = Logger.IniciarLog("Alteração de Cadastro do Usuário"))
            {
                try
                {
                    Int32 codigoRetorno = default(Int32);
                    Guid hashConfirmacaoEmail = default(Guid);
                    Boolean possuiKomerci = default(Boolean);

                    //Verifica se algum PV associado ao usuário possui Komerci
                    using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        possuiKomerci = ctx.Cliente.PossuiKomerci(this.UsuarioSelecionado.Estabelecimentos.ToArray());
                        
                        //codigoRetorno = ctx.Cliente.ValidarCriarEntidade(this.UsuarioSelecionado.Estabelecimentos.ToArray(), 1);
                    }

                    //Verifica se alguma das entidades é inválida
                    //if (codigoRetorno != 0)
                    //{
                    //    base.ExibirPainelExcecao("EntidadeServico.ValidarCriarEntidade", codigoRetorno);
                    //    return;
                    //}

                    //Atualizando os dados do usuário
                    using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        codigoRetorno = ctx.Cliente.Atualizar2(
                            out hashConfirmacaoEmail,
                            possuiKomerci,
                            1, //grupo entidade fixo para Estabelecimentos
                            this.UsuarioSelecionado.CodigoEstabelecimentos,
                            String.Empty, //Login (parâmetro não utilizado no NovoAcesso)
                            this.UsuarioSelecionado.Nome,
                            this.UsuarioSelecionado.TipoUsuario, //TipoUsuário (não utilizado mais no NovoAcesso)
                            String.Empty, //Senha
                            this.UsuarioSelecionado.CodigoServicos,
                            this.UsuarioSelecionado.CodigoIdUsuario.Value,
                            this.UsuarioSelecionado.Email,
                            this.UsuarioSelecionado.EmailSecundario,
                            this.UsuarioSelecionado.Cpf,
                            this.UsuarioSelecionado.CelularDdd,
                            this.UsuarioSelecionado.CelularNumero,
                            UsuarioServico.Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario,
                            2,
                            DateTime.Now);

                    if (codigoRetorno != 0)
                    {
                        base.ExibirPainelExcecao("UsuarioServico.Atualizar2", codigoRetorno);
                        return;
                    }
                    else
                    {

                        //Registra no histórico/log de atividades
                        Historico.AprovacaoUsuario(SessaoAtual,
                            this.UsuarioSelecionado.CodigoIdUsuario.Value,
                            this.UsuarioSelecionado.Nome,
                            this.UsuarioSelecionado.Email,
                            this.UsuarioSelecionado.TipoUsuario);

                        //Segue fluxo de alteração de e-mail, com envio de e-mail ao usuário
                        //Envia e-mail para o usuário informando-o da atualização de seu e-mail
                        EmailNovoAcesso.EnviarEmailConfirmacaoCadastro12h(
                            this.UsuarioSelecionado.Email,
                            this.UsuarioSelecionado.CodigoIdUsuario.Value,
                            hashConfirmacaoEmail,
                            SessaoAtual.CodigoIdUsuario,
                            SessaoAtual.Email,
                            SessaoAtual.NomeUsuario,
                            SessaoAtual.TipoUsuario,
                            SessaoAtual.CodigoEntidade,
                            SessaoAtual.Funcional, true, this.UsuarioSelecionado.Cpf);


                        //Limpa variável de sessão que contém os dados do usuário em editado
                        this.Encerrar();
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
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
        /// Finaliza o processo de aprovação, removendo o usuário em edição
        /// da sessão
        /// </summary>
        private void Encerrar()
        {
            this.UsuarioSelecionado = null;
            this.UsuarioSelecionadoOriginal = null;
        }

        #endregion

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            base.RedirecionarParaUsuarios();
        }
    }
}