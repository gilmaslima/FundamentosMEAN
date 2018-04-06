using System;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using System.Web.UI;
using System.Web.Script.Serialization;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.RecuperacaoIdentificao
{
    /// <summary>
    /// Identificação de usuário legado
    /// </summary>
    public partial class RecuperacaoIdentificaoUserControl : UserControlBase
    {
        #region [Eventos da WebPart]


        /// <summary>
        /// Incialização da WebPart de Identificação da Recuperação de Usuário/Senha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Caso tenha sido escolhida outra forma de recuperação, exibe novamente o painel de formas de envio.
            if (Request.QueryString["outraForma"] != null)
            {
                ConsultaPvsHandlerResponse model = new ConsultaPvsHandlerResponse();
                if (InformacaoUsuario.Existe())
                {
                    InformacaoUsuario info = InformacaoUsuario.Recuperar();
                    model.ExibirRadioSMS = info.CelularUsuario != 0 && info.DddCelularUsuario != 0;
                    model.ExibirRadioEmailSecundario = !String.IsNullOrEmpty(info.EmailSecundario);
                    model.ExibirPainelEnvioCodigo = true;
                    txtEmail.Value = info.EmailUsuario;
                    hdnOutraFormaRecuperacao.Value = "1";

                    var pvs = info.EstabelecimentosRelacinados.Select(x => new ConsultaPvsHandlerModel
                    {
                        Celular = x.Celular,
                        Email = x.Email,
                        EmailSecundario = x.EmailSecundario,
                        NomeEstabelecimento = x.RazaoSocial,
                        NumeroPv = x.NumeroPV,
                        DDDCelular = x.DDDCelular,
                        IdUsuario = x.IdUsuario,
                        NumeroPvMascarado = Util.TruncarNumeroPV(x.NumeroPV.ToString())
                    }).ToArray();

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    hdnOutraFormaRecuperacaoPvs.Value = serializer.Serialize(pvs);
                    hdnOutraFormaRecuperacaoPvsSelecionados.Value = serializer.Serialize(info.PvsSelecionados);

                    this.ProcessarRequestServerSide(model);
                }
                else
                {
                    model = UsuarioNegocio.ConstruirMensagemErro(500, "Houve um erro ao selecionar outra forma de recuperação.");
                    this.ProcessarRequestServerSide(model);
                }
            }
            else
            {
                //Limpa as informações do usuario, pois é o primeiro acesso na tela.
                InformacaoUsuario.Limpar();
            }

            if (IsPostBack && String.Compare(Request.Params["__EVENTTARGET"], "ReenviarEmail", true) == 0)
                this.ReenviarEmail(Request.Params["__EVENTARGUMENT"]);
        }

        #endregion

        /// <summary>
        /// Reenvia o email de aprovação ao usuário master.
        /// </summary>
        /// <param name="argument">Argumento fornecido em client-side</param>
        protected void ReenviarEmail(String argument)
        {
            var model = new ConsultaPvsHandlerResponse();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            using (Logger log = Logger.IniciarLog(String.Format("RaisePostBackEvent - {0}", argument)))
            {
                try
                {
                    UsuarioServico.Usuario usuario = null;
                    Int32 codigoEntidade = (Int32)argument.Split('|')[1].ToInt32Null(0);
                    String email = argument.Split('|')[2];
                    Int32 codigoRetorno = 0;

                    // reenvio de solicitação
                    if (String.Compare(argument, "ReenviarSolicitacaoAprovacao", true) >= 0)
                    {
                        UsuarioServico.Usuario[] usuarios = null;
                        EntidadeServico.Usuario[] usuariosMaster = null;

                        using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            log.GravarMensagem("Chamando método ConsultarPorEmailPrincipalPorStatus");
                            usuarios = contextoUsuario.Cliente.ConsultarPorEmailPrincipalPorStatus(out codigoRetorno, email, 0, codigoEntidade, null);
                            log.GravarMensagem("Resultado método ConsultarPorEmailPrincipalPorStatus", new { result = usuarios });
                        }

                        if (usuarios == null)
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
                            model = UsuarioNegocio.ConstruirMensagemErro(500, "Nenhum usuário master encontrado relacionado ao estabelecimento informado");
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

                        model.FlagModal = true;
                        model.TipoModal = TipoModal.Success;
                        model.TituloModal = "E-mail reenviado com sucesso.";
                        model.StatusCode = 200;
                        model.MensagemRetorno = "Dentro de instantes o usuário Master receberá o e-mail solicitando a aprovação do seu acesso ao Portal Rede.";

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
                            new List<int>() { usuario.Entidade.Codigo },
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
