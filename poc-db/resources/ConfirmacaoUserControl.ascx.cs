/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.Usuarios.Confirmacao
{
    /// <summary>
    /// Administração de Usuários - Criação/Edição de Usuário - Passo 4 - Confirmação de Ação
    /// </summary>
    public partial class ConfirmacaoUserControl : UsuariosUserControlBase
    {


        #region [ Eventos da Página ]

        /// <summary>
        /// Load da Página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("ConfirmacaoUserControl.ascx - Page_Load"))
            {
                try
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
                        if (base.ModoCriacao)
                        {
                            log.GravarMensagem("Criação de novo usuário");
                            CriarUsuario();
                        }
                        else if (base.ModoEdicao)
                        {
                            log.GravarMensagem("Alteração de usuário");
                            AlterarUsuario();
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
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

        #endregion

        #region [ Eventos dos Controles ]

        /// <summary>
        /// Ação Voltar para tela de Listagem de Usuários
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs args)
        {
            using (Logger log = Logger.IniciarLog("ConfirmacaoUserControl.ascx - btnVoltar_Click"))
            {
                try
                {
                    base.RedirecionarParaUsuarios();
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
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

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Criação do Usuário
        /// </summary>
        private void CriarUsuario()
        {
            Int32 codigoRetorno = default(Int32);
            Int32 codigoIdUsuario = default(Int32);
            Guid hashConfirmacao = default(Guid);
            String origemCriacao = "F"; //Área Fechada
            Boolean possuiKomerci = false;

            //Verifica se algum PV que foi associado ao usuário possui Komerci
            using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
            {
                possuiKomerci = ctx.Cliente.PossuiKomerci(this.UsuarioSelecionado.Estabelecimentos.ToArray());

                //codigoRetorno = ctx.Cliente.ValidarCriarEntidade(this.UsuarioSelecionado.Estabelecimentos.ToArray(), 1);
            }

            //Verifica se alguma das entidades é inválida
            if (codigoRetorno != 0)
            {
                base.ExibirPainelExcecao("EntidadeServico.ValidarCriarEntidade", codigoRetorno);
                return;
            }

            using (var ctx = new ContextoWCF<UsuarioServicoClient>())
                codigoRetorno = ctx.Cliente.InserirUsuario2(
                    out codigoIdUsuario,
                    out hashConfirmacao,
                    possuiKomerci,
                    1, //fixo: sempre deve ser Estabelecimento
                    this.UsuarioSelecionado.CodigoEstabelecimentos,
                    String.Empty, //Login (parâmetro não utilizado no NovoAcesso)
                    this.UsuarioSelecionado.Nome,
                    this.UsuarioSelecionado.TipoUsuario,
                    this.UsuarioSelecionado.Senha,
                    this.UsuarioSelecionado.CodigoServicos,
                    this.UsuarioSelecionado.Email,
                    this.UsuarioSelecionado.EmailSecundario,
                    this.UsuarioSelecionado.Cpf,
                    this.UsuarioSelecionado.CelularDdd,
                    this.UsuarioSelecionado.CelularNumero,
                    Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario,
                    origemCriacao);

            //Verifica se usuário foi criado com sucesso
            if (codigoRetorno != 0)
            {
                base.ExibirPainelExcecao("UsuarioServico.InserirUsuario2", codigoRetorno);
                return;
            }

            //Armazena o ID do usuário criado
            this.UsuarioSelecionado.CodigoIdUsuario = codigoIdUsuario;

            //Armazena no histórico/log de atividades
            Historico.CriacaoUsuario(SessaoAtual,
                this.UsuarioSelecionado.CodigoIdUsuario.Value,
                this.UsuarioSelecionado.Nome,
                this.UsuarioSelecionado.Email,
                this.UsuarioSelecionado.TipoUsuario,
                this.UsuarioSelecionado.Estabelecimentos);

            List<Int32> estabelecimentos = new System.Collections.Generic.List<int>();
            if (this.UsuarioSelecionado.Estabelecimentos.Any())
                estabelecimentos = new System.Collections.Generic.List<int>() { this.UsuarioSelecionado.Estabelecimentos.First() };

            //Envia e-mail de confirmação de e-mail para o usuário criado
            //Não há prazo de validade
            EmailNovoAcesso.EnviarEmailConfirmacaoCadastro(
                SessaoAtual,
                this.UsuarioSelecionado.Email,
                estabelecimentos,
                this.UsuarioSelecionado.CodigoIdUsuario.Value,
                hashConfirmacao);

            ltrCadastroTitulo.Visible = pnlCadastro.Visible = true;

            //Limpa variável de sessão que contém os dados do usuário criado
            this.Encerrar();
        }

        /// <summary>
        /// Alteração do Usuário
        /// </summary>
        private void AlterarUsuario()
        {
            Boolean emailAtualizado;
            base.AlterarUsuario("Confirmacao", out emailAtualizado);

            if (emailAtualizado)
            {
                if (this.UsuarioSelecionadoOriginal.Legado)
                    ltrEdicaoComAlteracaoEmailTexto.Text = "Dentro de instantes o usuário receberá um e-mail de confirmação. O usuário deverá acessar o link informado no e-mail para concluir o cadastro.";
                else
                    ltrEdicaoComAlteracaoEmailTexto.Text = "Dentro de instantes o usuário receberá um e-mail de confirmação. O usuário deverá acessar o link informado no e-mail em até 48h para concluir o cadastro.";

                ltrEdicaoComAlteracaoEmailTitulo.Visible = pnlEdicaoComAlteracaoEmail.Visible = true;
            }
            else
            {
                ltrEdicaoSemAlteracaoEmailTitulo.Visible = pnlEdicaoSemAlteracaoEmail.Visible = true;
            }

            //Limpa variável de sessão que contém os dados do usuário em edição
            base.Encerrar();
        }
        #endregion
    }
}
