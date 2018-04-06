using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.RecuperacaoUsuario
{
    /// <summary>
    /// Recuperação de usuário
    /// </summary>
    public partial class RecuperacaoUsuarioUserControl : UserControlBase
    {
        /// <summary>
        /// Incialização da WebPart de Identificação da Recuperação de Usuário/Senha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Inicialização da WebPart de Identificação da Recuperação de Usuário/Senha"))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        InformacaoUsuario.Limpar();
                    }
                    else
                    {
                        if (String.Compare(Request.Params["__EVENTTARGET"], "ReenviarEmail", true) == 0)
                            this.ReenviarEmail(Request.Params["__EVENTARGUMENT"]);
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Fonte, ex.Codigo);
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
        /// Reenvia o email de aprovação ao usuário master.
        /// </summary>
        /// <param name="argument">Argumento fornecido em client-side</param>
        protected void ReenviarEmail(String argument)
        {
            var model = new ConsultaPvsHandlerResponse();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            using (Logger log = Logger.IniciarLog("Evento para reenviar o email de aprovação ao usuário master."))
            {
                try
                {
                    Int32 codigoEntidade = (Int32)argument.Split('|')[1].ToInt32Null(0);
                    String email = argument.Split('|')[2];
                    Int64 cpf = argument.Split('|')[3].ToInt64Null().GetValueOrDefault(0);
                    
                    Int32 codigoRetorno = 0;
                    UsuarioServico.Usuario usuario = null;

                    // reenvio de solicitação
                    if (String.Compare(argument, "ReenviarSolicitacaoAprovacao", true) >= 0)
                    {
                        UsuarioServico.Usuario[] usuarios = null;
                        EntidadeServico.Usuario[] usuariosMaster = null;

                        using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            log.GravarMensagem("Chamando método ConsultarPorEmailPrincipalPorStatus");
                            usuarios = contextoUsuario.Cliente.ConsultarPorCpfPrincipalPorStatus(out codigoRetorno, cpf, 0, codigoEntidade, null);
                            log.GravarMensagem("Resultado método ConsultarPorEmailPrincipalPorStatus", new { result = usuarios });
                        }

                        if (usuarios == null || usuarios.Length == 0)
                        {
                            model = UsuarioNegocio.ConstruirMensagemErro(500, "Nenhum usuário encontrado com os dados informados.");
                            this.ProcessarRequestServerSide(model);
                            return;
                        }

                        usuario = usuarios.FirstOrDefault(u => u.Status.Codigo == (Int32)Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoMaster);
                        if (usuario == null)
                        {
                            model = UsuarioNegocio.ConstruirMensagemErro(500, "Nenhum usuário encontrado com os dados informados.");
                            this.ProcessarRequestServerSide(model);
                            return;
                        }

                        using (var contexto = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        {
                            log.GravarMensagem("Chamando método ConsultarUsuariosPorPerfil");
                            usuariosMaster = contexto.Cliente.ConsultarUsuariosPorPerfil(out codigoRetorno, usuario.Entidade.Codigo, 1, 'M');
                            log.GravarMensagem("Resultado método ConsultarUsuariosPorPerfil", new { result = usuariosMaster });
                        }

                        if (usuariosMaster == null || usuariosMaster.Length == 0)
                        {
                            model = UsuarioNegocio.ConstruirMensagemErro(500, "Nenhum usuário encontrado");
                            this.ProcessarRequestServerSide(model);
                            return;
                        }

                        string[] emails = usuariosMaster
                                .Select(master => master.Email)
                                .Where(mail => !String.IsNullOrEmpty(mail)).ToArray();

                        //Envia e-mail de aprovação
                        EmailNovoAcesso.EnviarEmailAprovacaoAcesso(
                            string.Join(",", emails),
                            usuario.Descricao,
                            usuario.Email,
                            usuario.CodigoIdUsuario,
                            usuario.TipoUsuario,
                            usuario.Entidade.Codigo,
                            null);

                        String mensagem = "Dentro de instantes o usuário Master receberá o e-mail solicitando a aprovação do seu acesso ao Portal Rede.";

                        // verifica se o usuario atual esta na lista dos usuario que receberão o email de aprovacao
                        Boolean recebeuEmailAprovacao = usuariosMaster.ToList().Exists(us => us.CodigoIdUsuario == usuario.CodigoIdUsuario);

                        if (recebeuEmailAprovacao)
                        {
                            mensagem = "Você receberá em instantes um e-mail solicitando a aprovação do seu acesso aos canais digitais Rede.";
                        }

                        model.FlagModal = true;
                        model.TipoModal = TipoModal.Success;
                        model.TituloModal = "E-mail reenviado com sucesso.";
                        model.StatusCode = 200;
                        model.MensagemRetorno = mensagem;

                        this.ProcessarRequestServerSide(model);
                    }
                    // reenvio de solicitação para confirmação do e-mail
                    else if (string.Compare(argument, "ReenviarEmailConfirmacao", true) >= 0)
                    {
                        Guid hash = Guid.Empty;

                        // busca usuário na base pelo e-mail informado
                        Boolean sucesso = false;
                        usuario = UsuarioNegocio.ConsultarUsuarioPorEmail(email, codigoEntidade, out sucesso, out hash);

                        Sessao sessaoAtual = new Sessao
                        {
                            Celular = usuario.Celular,
                            CPF = usuario.CPF,
                            CodigoEntidade = usuario.Entidade.Codigo,
                            GrupoEntidade = usuario.Entidade.GrupoEntidade.Codigo,
                            CodigoIdUsuario = usuario.CodigoIdUsuario,
                            DDDCelular = usuario.DDDCelular,
                            Email = usuario.Email,
                            EmailSecundario = usuario.EmailSecundario,
                            EmailTemporario = usuario.EmailTemporario,
                            ExibirMensagemLiberacaoAcessoCompleto = usuario.ExibirMensagemLiberacaoAcesso,
                            Legado = usuario.Legado,
                            NomeUsuario = usuario.NomeResponsavelInclusao,
                            TipoUsuario = usuario.TipoUsuario
                        };

                        using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            if (hash == Guid.Empty)
                            {
                                hash = ctx.Cliente.InserirHash(
                                    usuario.CodigoIdUsuario,
                                    (UsuarioServico.Status1)usuario.Status.Codigo.GetValueOrDefault(0),
                                    0.5,
                                    null);
                            }
                            else
                            {
                                codigoRetorno = 0;
                                var usuarioHash = ctx.Cliente.ReiniciarHash(out codigoRetorno, usuario.CodigoIdUsuario);

                                if (usuarioHash != null && usuarioHash.Hash != null)
                                    hash = usuarioHash.Hash;
                            }
                        }

                        // reenvio de e-mail para 
                        EmailNovoAcesso.EnviarEmailConfirmacaoCadastro(
                            sessaoAtual,
                            usuario.Email,
                            new List<Int32>() { usuario.Entidade.Codigo },
                            usuario.CodigoIdUsuario,
                            hash);

                        model.FlagModal = true;
                        model.TipoModal = TipoModal.Success;
                        model.TituloModal = "E-mail reenviado com sucesso.";
                        model.StatusCode = 200;
                        model.MensagemRetorno = "Verifique a caixa de entrada do seu e-mail.";

                        this.ProcessarRequestServerSide(model);
                    }
                }

                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    model = UsuarioNegocio.ConstruirMensagemErro(500, ex.Detail.Fonte, ex.Detail.Codigo);
                    this.ProcessarRequestServerSide(model);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    model = UsuarioNegocio.ConstruirMensagemErro(500, "Sistema indisponível (-1).", String.Format("{0} - {1}", ex.Message, ex.StackTrace));
                    this.ProcessarRequestServerSide(model);
                    return;
                }
            }
        }

        /// <summary>
        /// Registra o comando para execução da function ProcessarRequestServerside em client-side
        /// </summary>
        /// <param name="model">Modelo com os dados de retorno</param>
        private void ProcessarRequestServerSide(ConsultaPvsHandlerResponse model)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ScriptManager.RegisterStartupScript(
                this, this.GetType(),
                "ProcessarRequestServerSide",
                String.Format("ProcessarRequestServerside({0});",
                serializer.Serialize(model)),
                true);
        }
    }
}
