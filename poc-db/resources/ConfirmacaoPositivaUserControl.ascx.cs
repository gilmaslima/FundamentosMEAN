using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.ServiceModel;
using System.Web;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using System.Linq;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.ConfirmacaoPositiva
{
    /// <summary>
    /// Página responsável para confirmação positiva
    /// </summary>
    public partial class ConfirmacaoPositivaUserControl : UserControlBase
    {
        #region [Controles Página]

        /// <summary>
        /// cpCriacaoAcesso control.
        /// </summary>
        protected ConfirmacaoPositivaNovoAcesso CpCriacaoAcesso { get { return (ConfirmacaoPositivaNovoAcesso)cpCriacaoAcesso; } }

        /// <summary>
        /// qdAvisoConfirmacaoPositiva control.
        /// </summary>
        protected QuadroAviso QdAvisoConfirmacaoPositiva { get { return (QuadroAviso)qdAvisoConfirmacaoPositiva; } }

        #endregion

        #region [Eventos Página]

        /// <summary>
        /// Inicicialização da webpart de Confirmação positiva de Criação de Usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Inicicialização da webpart de Confirmação positiva de Criação de Usuário"))
            {
                try
                {
                    InformacaoUsuario usuario = null;

                    if (InformacaoUsuario.Existe())
                    {
                        usuario = InformacaoUsuario.Recuperar();

                        Boolean usuarioJaCadastrado = usuario.IdUsuario > 0 && !usuario.CriacaoAcessoLegado;
                        if (usuarioJaCadastrado)
                        {
                            InformacaoUsuario.Limpar();
                            this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", base.RecuperarEnderecoPortal());
                            return;
                        }
                    }

                    if (!IsPostBack)
                        CpCriacaoAcesso.ConfirmacaoCompleta = usuario.TipoUsuario.Equals("P") || usuario.TipoUsuario.Equals("M");
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", base.RecuperarEnderecoPortal());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                }
            }
        }

        /// <summary>
        /// Continuação o processo de criação de usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void Continuar(object sender, EventArgs args)
        {
            using (Logger log = Logger.IniciarLog("Continuando o processo de criação de usuário"))
            {
                try
                {
                    if (InformacaoUsuario.Existe())
                    {
                        InformacaoUsuario info = InformacaoUsuario.Recuperar();
                        String servicosSplit = String.Empty;
                        Boolean possuiUsuarios = false;
                        Boolean possuiUsuariosMaster = false;
                        Boolean possuiSenhaTemporaria = false;

                        using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        {
                            contextoEntidade.Cliente.PossuiUsuario(out possuiUsuarios, out possuiUsuariosMaster, out possuiSenhaTemporaria, info.NumeroPV, info.GrupoEntidade);
                        }

                        using (var contextoServicos = new ContextoWCF<AdministracaoServico.AdministracaoServicoClient>())
                        {

                            if (info.IdUsuario > 0 && info.TipoUsuario.Equals("P")) //Deverá obter os serviços do usuários
                            {
                                info.TipoUsuario = "P";
                                servicosSplit = this.RecuperarPermissoesUsuario(info.IdUsuario, info.NumeroPV);

                                if (String.IsNullOrEmpty(servicosSplit))
                                {
                                    this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", "");
                                    return;
                                }
                            }
                            else if (info.IdUsuario > 0 && info.TipoUsuario.Equals("M"))
                                info.TipoUsuario = "M";
                            else if (info.IdUsuario > 0 && info.TipoUsuario.Equals("B"))
                                info.TipoUsuario = "B";
                            else
                                info.TipoUsuario = "B";
                        }

                        using (var contexto = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            Int32 idUsuario = 0;
                            Int32 idUsuarioLegado = info.IdUsuario;
                            Guid hashEmail;

                            UsuarioServico.Status1 statusUsuario;
                            if (possuiUsuariosMaster && !possuiSenhaTemporaria && idUsuarioLegado == 0)
                                statusUsuario = UsuarioServico.Status1.UsuarioAguardandoConfirmacaoMaster;
                            else
                                statusUsuario = UsuarioServico.Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario;

                            Int32 codigoRetorno = contexto.Cliente.InserirUsuario2(out idUsuario,
                                                             out hashEmail,
                                                             info.PossuiKomerci,
                                                             info.GrupoEntidade,
                                                             info.NumeroPV.ToString(),
                                                             String.Empty,
                                                             info.NomeCompleto,
                                                             info.TipoUsuario,
                                                             info.SenhaUsuario,
                                                             servicosSplit,
                                                             info.EmailUsuario,
                                                             info.EmailSecundario,
                                                             info.CpfUsuario,
                                                             info.DddCelularUsuario > 0 ? info.DddCelularUsuario : (Int32?)null,
                                                             info.CelularUsuario > 0 ? info.CelularUsuario : (Int32?)null,
                                                             statusUsuario,
                                                             info.IdUsuario > 0 ? "M" : "A"); //M: Migração; A: Aberto
                            if (codigoRetorno > 0)
                            {
                                this.ExibirErro("UsuarioServico.InserirUsuario", codigoRetorno, "Atenção", "");
                            }
                            else
                            {
                                if (possuiUsuariosMaster && !possuiSenhaTemporaria && idUsuarioLegado == 0)
                                {
                                    codigoRetorno = 0;

                                    var usuariosMaster = default(EntidadeServico.Usuario[]);

                                    using (var ctxEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                                        usuariosMaster = ctxEntidade.Cliente.ConsultarUsuariosPorPerfil(out codigoRetorno, info.NumeroPV, info.GrupoEntidade, 'M');

                                    if (usuariosMaster != null && usuariosMaster.Length > 0)
                                    {
                                        String emailsMaster = String.Join(",",
                                            usuariosMaster.Select(usuario => usuario.Email)
                                            .Where(email => !String.IsNullOrEmpty(email)).ToArray());

                                        log.GravarMensagem("EnviarEmailAprovacaoAcesso", new { emailsMaster });

                                        if (!String.IsNullOrEmpty(emailsMaster))
                                            EmailNovoAcesso.EnviarEmailAprovacaoAcesso(emailsMaster, info.NomeCompleto,
                                                info.EmailUsuario, info.IdUsuario, info.TipoUsuario, info.NumeroPV, null);
                                    }
                                }
                                else
                                {
                                    EmailNovoAcesso.EnviarEmailConfirmacaoCadastro48h(info.EmailUsuario,
                                        idUsuario, hashEmail, idUsuario, info.EmailUsuario,
                                        info.NomeCompleto, info.TipoUsuario, info.NumeroPV, null);
                                }

                                info.IdUsuario = idUsuario;
                                info.HashEmail = hashEmail;
                                info.EntidadePossuiMaster = possuiSenhaTemporaria ? false : possuiUsuariosMaster;
                                info.PodeRecuperarCriarAcesso = false;

                                InformacaoUsuario.Salvar(info);

                                //Registra no log/histórico de atividades
                                Historico.CriacaoUsuario(
                                    info.IdUsuario, info.NomeCompleto, info.EmailUsuario, info.TipoUsuario ?? "B", 
                                    info.NumeroPV, possuiUsuariosMaster, possuiUsuarios);

                                if (possuiSenhaTemporaria)
                                {
                                    var usuariosSenhaProvisioria = contexto.Cliente
                                            .ConsultarPorCodigoEntidade(out codigoRetorno, info.NumeroPV.ToString(),
                                                                        new UsuarioServico.Entidade() 
                                                                            { 
                                                                                Codigo = info.NumeroPV, 
                                                                                GrupoEntidade = new UsuarioServico.GrupoEntidade() 
                                                                                                { 
                                                                                                    Codigo = 1 
                                                                                                } 
                                                                            });

                                    if (usuariosSenhaProvisioria != null && usuariosSenhaProvisioria.Length > 0)
                                    {
                                        var usuario = usuariosSenhaProvisioria[0];
                                        idUsuarioLegado = usuario.CodigoIdUsuario;
                                    }
                                }

                                if (idUsuarioLegado > 0)
                                    this.ExcluirUsuarioLegado(idUsuarioLegado);

                                this.RedirecionaPasso4();
                            }
                        }
                    }
                    else
                    {
                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", base.RecuperarEnderecoPortal());
                    }
                }
                catch (FaultException<AdministracaoServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", base.RecuperarEnderecoPortal());
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", base.RecuperarEnderecoPortal());
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", base.RecuperarEnderecoPortal());
                }
                catch (SmtpException ex)
                {
                    log.GravarErro(ex);
                    ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    ExibirErro(FONTE, CODIGO_ERRO, "Atenção", base.RecuperarEnderecoPortal());
                }
            }
        }

        /// <summary>
        /// Voltar a exibir o controle de confirmação positiva após erro 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            pnlAvisoConfirmacaoPositiva.Visible = false;
            pnlConfirmacaoPositiva.Visible = true;
        }

        #endregion

        #region [Métodos auxiliares]

        /// <summary>
        /// Recupera as permissões atuais do usuário separadas por ',' para recriá-lo
        /// </summary>
        private String RecuperarPermissoesUsuario(Int32 codigoIdUsuario, Int32 codigoEntidade)
        {
            var permissoes = default(UsuarioServico.Menu[]);
            String codigoServicos = String.Empty;

            using (Logger log = Logger.IniciarLog("Recupera as permissões atuais do usuário separadas por ',' para recriá-lo"))
            {
                using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    permissoes = ctx.Cliente.ConsultarMenu(null,
                        1, //fixo: sempre deve ser Estabelecimento
                        codigoEntidade,
                        codigoIdUsuario);

                var permissoesMenu = permissoes.Flatten(servico => servico.Items);

                codigoServicos = String.Join(",", permissoesMenu.Distinct().Select(permissao => permissao.Codigo.ToString()).ToArray());
            }

            return codigoServicos;
        }

        /// <summary>
        /// Recupera as permissões de Serviços Básicos atuais do Grupo de Entidade separadas por ','
        /// </summary>
        private String RecuperarPermissoesUsuarioBasico(Int32 codigoGrupoEntidade)
        {
            AdministracaoServico.Servico[] servicos = default(AdministracaoServico.Servico[]);
            String codigoServicos = String.Empty;

            using (Logger log = Logger.IniciarLog("Recupera as permissões de Serviços Básicos atuais do Grupo de Entidade separadas por ','"))
            {
                using (var ctx = new ContextoWCF<AdministracaoServico.AdministracaoServicoClient>())
                    servicos = ctx.Cliente.ConsultarPorGrupoEntidade2(codigoGrupoEntidade, true);

                if (servicos.Length > 0)
                {
                    var servicosBasicos = servicos.Where
                        (servico =>
                        {
                            return servico.ServicoBasico == true;
                        }
                        ).ToArray();

                    codigoServicos = String.Join(",", servicosBasicos.Distinct().Select(servico => servico.Codigo.ToString()).ToArray());
                }
            }

            return codigoServicos;
        }

        /// <summary>
        /// Recupera as permissões de Serviços Master do Grupo de Entidade separadas por ','
        /// </summary>
        /// <param name="codigoGrupoEntidade">Código do Grupo de Entidade</param>
        /// <returns></returns>
        private String RecuperarPermissoesUsuarioMaster(Int32 codigoGrupoEntidade)
        {
            AdministracaoServico.Servico[] servicos = default(AdministracaoServico.Servico[]);
            String codigoServicos = String.Empty;

            using (Logger log = Logger.IniciarLog("Recupera as permissões de Serviços Master do Grupo de Entidade separadas por ','"))
            {
                using (var ctx = new ContextoWCF<AdministracaoServico.AdministracaoServicoClient>())
                    servicos = ctx.Cliente.ConsultarPorGrupoEntidade2(codigoGrupoEntidade, true);

                if (servicos.Length > 0)
                {
                    codigoServicos = String.Join(",", servicos.Distinct().Select(servico => servico.Codigo.ToString()).ToArray());
                }
            }

            return codigoServicos;
        }

        /// <summary>
        /// Excluir o usuário legado que está migrando seu acesso
        /// </summary>
        /// <param name="idUsuarioLegado">ID do usuário Legado</param>
        private void ExcluirUsuarioLegado(Int32 idUsuarioLegado)
        {
            using (Logger log = Logger.IniciarLog("Excluir o usuário legado que está migrando seu acesso"))
            {
                try
                {
                    using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        ctx.Cliente.ExcluirEmLoteNovoAcesso(idUsuarioLegado.ToString());
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    //ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    //ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                }
            }
        }

        /// <summary>
        /// Exibe a mensagem de erro
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <param name="titulo">Título para o quadro de aviso</param>
        /// <param name="urlVoltar">Mensagem adicional</param>
        private void ExibirErro(String fonte, Int32 codigo, String titulo, String urlVoltar)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);

            if (String.IsNullOrEmpty(urlVoltar))
            {
                btnVoltar.Visible = true;
                QdAvisoConfirmacaoPositiva.CarregarMensagem(titulo, mensagem, false, QuadroAviso.IconeMensagem.Erro);
            }
            else
            {
                QdAvisoConfirmacaoPositiva.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAviso.IconeMensagem.Erro);
                btnVoltar.Visible = false;
            }

            pnlAvisoConfirmacaoPositiva.Visible = true;
            pnlConfirmacaoPositiva.Visible = false;
        }

        /// <summary>
        /// Redireciona para o Passo 4 de Conclusão de criação de usuário
        /// </summary>
        private void RedirecionaPasso4()
        {
            using (Logger log = Logger.IniciarLog("Redireciona para o Passo 4 de Conclusão de criação de usuário"))
            {
                try
                {
                    String urlConclusao = String.Format("{0}/Paginas/CriacaoUsrConclusao.aspx", base.web.ServerRelativeUrl);
                    this.Response.Redirect(urlConclusao, false);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                }
            }
        }

        #endregion
    }
}