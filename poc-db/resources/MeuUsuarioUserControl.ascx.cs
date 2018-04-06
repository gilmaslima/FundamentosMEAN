/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.ServiceModel;
using System.Web;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;
using Redecard.PN.DadosCadastrais.SharePoint.UsuarioServico;
using Redecard.PNCadastrais.Core.Web.Controles.Portal;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.MeuUsuario.MeuUsuario
{
    /// <summary>
    /// Controle Principal/Inicial da tela "Meu usuário".
    /// </summary>
    public partial class MeuUsuarioUserControl : MeuUsuarioUserControlBase
    {
        #region [ Propriedades ]

        /// <summary>
        /// QueryString protegida. Se não existir ou inválida, retorna null. 
        /// </summary>
        public QueryStringSegura QS
        {
            get
            {
                String qsDados = Request.QueryString["dados"];
                QueryStringSegura qs = default(QueryStringSegura);

                if (!String.IsNullOrEmpty(qsDados))
                {
                    try
                    {
                        qs = new QueryStringSegura(qsDados);
                    }
                    catch (QueryStringInvalidaException ex)
                    {
                        Logger.GravarErro("Erro ao recuperar QueryString", ex, qsDados);
                    }
                    catch (Exception ex)
                    {
                        Logger.GravarErro("Erro ao recuperar QueryString", ex, qsDados);
                    }
                }
                return qs;
            }
        }

        #endregion

        #region [ Eventos de Página ]

        /// <summary>
        /// Load da página.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Load da página"))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        //Se possui QueryString
                        if (QS != null)
                        {
                            //Exibe banner de confirmação de alteração de dados se flag ExibirMensagemSucesso informada
                            if (String.Compare(Boolean.TrueString, QS["ExibirMensagemSucesso"], true) == 0)
                            {
                                String mensagemEmail = String.Empty;
                                if (String.Compare(Boolean.TrueString, QS["AlteracaoEmail"], true) == 0)
                                {
                                    mensagemEmail = "Dentro de instantes você receberá no novo endereço de e-mail a solicitação para confirmação desta alteração. Você deverá acessar o link que foi enviado em até 48h para concluir a alteração.";
                                }
                                qdAvisoSucesso.Visible = true;
                                qdAvisoSucesso.Mensagem = String.Format("Alterações efetuadas com sucesso. {0}",mensagemEmail);
                            }
                        }

                        //Carregamento da tela  
                        // O usuario do tipo atendimento tem permissao apenas para visualizar a pagina
                        if (this.SessaoAtual != null && !this.SessaoAtual.UsuarioAtendimento)
                        {
                            VerificarPrimeiroAcessoCompleto();
                            CarregarDadosUsuario();
                        }
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion

        #region [ Eventos de Controles ]

        /// <summary>
        /// Clique para acesso ao Acesso Completo
        /// </summary>
        protected void btnAcessoCompleto_Click(object sender, EventArgs e)
        {
            QueryStringSegura qs = new QueryStringSegura();
            qs["ExibirInformacaoPermissao"] = "false";

            String url = String.Format("{0}/Paginas/LiberacaoAcessoCompleto.aspx?dados={1}", base.web.ServerRelativeUrl, qs.ToString());

            Response.Redirect(url, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Clique para acesso à tela de Alteração de Usuário
        /// </summary>
        protected void btnAlterarUsuario_Click(object sender, EventArgs e)
        {
            if (SessaoAtual.Legado)
                RedirecionarMeuUsuario("CadastroUsuarioMigracao.aspx");
            else
                RedirecionarMeuUsuario("MeuUsuarioCadastroUsuario.aspx");
        }

        /// <summary>
        /// Clique para acesso à tela de Alteração de Senha
        /// </summary>
        protected void btnAlterarSenha_Click(object sender, EventArgs e)
        {
            RedirecionarMeuUsuario("MeuUsuarioAlteracaoSenha.aspx");
        }

        /// <summary>
        /// Clique para acesso à tela de Altearação de E-mail
        /// </summary>
        protected void btnAlterarEmail_Click(object sender, EventArgs e)
        {
            RedirecionarMeuUsuario("MeuUsuarioAlteracaoEmail.aspx");
        }

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Verifica se o usuário já possui acesso completo e se é o usuário do 1º acesso.
        /// Caso negativo, exibe banner de aviso.
        /// </summary>
        private void VerificarPrimeiroAcessoCompleto()
        {
            var acessoBasico = default(Boolean);
            var origemAbertaMigracao = default(Boolean);
            var existeMaster = default(Boolean);
            var pvCancelado = default(Boolean);
            var formularioLiberado = default(Boolean);
            Usuario usuario = base.UsuarioAtual;

            //Se usuário foi criado a partir da Parte Aberta ou Migração
            origemAbertaMigracao = usuario.Origem == 'A' || usuario.Origem == 'M';

            //Se o PV já possui usuário Master
            existeMaster = this.PossuiUsuarioMaster();

            //Se o tipo de usuário é de Acesso Básico
            acessoBasico = String.Compare("B", usuario.TipoUsuario, true) == 0;

            //Se o PV está cancelado por categoria
            pvCancelado = SessaoAtual.StatusPVCancelado();

            //Se usuário pode solicitar liberação de acesso completo
            formularioLiberado =
                usuario.Status.Codigo != (Int32)Comum.Enumerador.Status.UsuarioAtivoLiberacaoAcessoCompletoBloqueada;

            //Exibe painel de liberação de acesso completo para usuários (condições):
            //- com acesso básico
            //- que não esteja com a liberação de acesso completo liberada
            //- criados pela parte aberta
            //- para PV ativo 
            //- PV que não possua nenhum master
            qdAvisoAcessoCompleto.Visible = acessoBasico && origemAbertaMigracao && !existeMaster && !pvCancelado && formularioLiberado;
        }

        /// <summary>
        /// Método para obter a descrição do status do usuário e exibir de uma forma amigável para o usuário.
        /// </summary>
        /// <param name="usuario">Usuário</param>
        /// <returns></returns>
        private String ObterDescricaoStatusUsuario(Usuario usuario)
        {
            switch (usuario.Status.Codigo)
            {
                case (Int32)Comum.Enumerador.Status.NaoDefinido:
                    return "Não definido";
                    

                case (Int32)Comum.Enumerador.Status.UsuarioAtivo:
                    return "Ativo";
                    

                case (Int32)Comum.Enumerador.Status.UsuarioBloqueadoSenhaInvalida:
                    return "Bloqueado por senha inválida";
                    

                case (Int32)Comum.Enumerador.Status.UsuarioBloqueadoRecuperacaoSenha:
                    return "Bloqueado por recuperação de senha";
                    

                case (Int32)Comum.Enumerador.Status.UsuarioBloqueadoRecuperacaoUsuario:
                    return "Bloqueado por recuperação de usuário";
                    

                case (Int32)Comum.Enumerador.Status.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:
                    return "Ativo com Acesso Completo Bloqueado";
                    

                case (Int32)Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoAlteracaoEmail:
                    return "Ativo - Aguardando confirmação de e-mail";
                    

                case (Int32)Comum.Enumerador.Status.UsuarioAtivoAguardandoConfirmacaoRecuperacaoSenha:
                    return "Ativo - Aguardando confirmação de e-mail";
                    

                case (Int32)Comum.Enumerador.Status.UsuarioBloqueadoAguardandoConfirmacaoRecuperacaoSenha:
                    return "Bloqueado - Aguardando confirmação de e-mail";
                    

                case (Int32)Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                case (Int32)Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoMaster:
                    return "Aguardando confirmação de e-mail";
                    

                case (Int32)Comum.Enumerador.Status.RespostaIdPosPendente:
                    return "Confirmação positiva pendente";
                    

                case (Int32)Comum.Enumerador.Status.EntidadeAtiva:
                    return "Entidade Ativa";
                    

                case (Int32)Comum.Enumerador.Status.EntidadeBloqueadaConfirmacaoPositiva:
                    return "Entidade bloqueada na confirmação positiva";
                    

                default:
                    return "--";
                    
            }

        }

        /// <summary>
        /// Carrega os dados do usuário.
        /// </summary>
        private void CarregarDadosUsuario()
        {
            Usuario usuario = base.UsuarioAtual;
            if (usuario != null)
            {
                txtStatus.Text = ObterDescricaoStatusUsuario(usuario);
                
                //Só exibe o link de confirmação positiva caso o status do usuário seja 11 (aguardando Confirmação positiva).
                pnlLinkConfirmacaoPositiva.Visible = usuario.Status.Codigo == (Int32)Comum.Enumerador.Status.RespostaIdPosPendente;
                txtNome.Text = usuario.Descricao.EmptyToNull() ?? "-";
                txtCpf.Value = !usuario.Legado && usuario.CPF.HasValue ? usuario.CPF.ToString() : "-";
                txtEmail.Value = !usuario.Legado && !String.IsNullOrEmpty(usuario.Email.EmptyToNull()) ? usuario.Email : "-";
                txtEmailSecundario.Value = !usuario.Legado && !String.IsNullOrEmpty(usuario.EmailSecundario.EmptyToNull()) ?
                    usuario.EmailSecundario : "-";
                txtCelular.Text = !usuario.Legado && usuario.DDDCelular.HasValue && usuario.Celular.HasValue ?
                    CampoCelular.AplicarMascara(usuario.DDDCelular, usuario.Celular) : "-";

                txtUltimaAlteracao.Text = usuario.DataUltimaAtualizacaoSenha.HasValue ? String.Format("última alteração: {0:dd/MM/yyyy}", usuario.DataUltimaAtualizacaoSenha.Value) : "";
            }
        }

        /// <summary>
        /// Redireciona a tela Meu Usuário para as subtelas.
        /// </summary>
        /// <param name="pagina">Página</param>
        private void RedirecionarMeuUsuario(String pagina)
        {
            Response.Redirect(pagina, false);
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        #endregion

        #region [ Consultas Serviços ]

        /// <summary>
        /// Verifica se a entidade já possui usuário Master
        /// </summary>
        private Boolean PossuiUsuarioMaster()
        {
            using (Logger log = Logger.IniciarLog("Verificando se entidade possui outros usuários/masters"))
            {
                var codigoRetorno = default(Int32);
                var possuiUsuario = true;
                var possuiMaster = true;
                var possuiSenhaTemporaria = false;

                try
                {
                    using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        codigoRetorno = ctx.Cliente.PossuiUsuario(
                            out possuiUsuario, out possuiMaster, out possuiSenhaTemporaria,
                            SessaoAtual.CodigoEntidade, SessaoAtual.GrupoEntidade);

                    if (codigoRetorno != 0)
                        base.ExibirPainelExcecao("EntidadeServico.PossuiUsuario", codigoRetorno);
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                return possuiMaster;
            }
        }

        #endregion

        /// <summary>
        /// Evento de clique no botão de edição de dados do usuário.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEditarDadosUsuario_Click(object sender, EventArgs e)
        {
            if (SessaoAtual.Legado)
                RedirecionarMeuUsuario("CadastroUsuarioMigracao.aspx");
            else
                RedirecionarMeuUsuario("MeuUsuarioCadastroUsuario.aspx");
        }
    }
}