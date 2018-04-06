using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.Comum.BlacklistValidations;
using Redecard.PN.Comum.Enumerador;
using Redecard.PNCadastrais.Core.Web.Controles.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using ControlTemplates = Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;

namespace Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario
{
    /// <summary>
    /// Classe de negocio para ações do Usuario
    /// </summary>
    public static class UsuarioNegocio
    {
        /// <summary>
        /// Obtem o usuário com base no e-mail/cpf.
        /// </summary>
        /// <param name="cpf">CPF do usuário.</param>
        /// <param name="email">Email do usuário.</param>
        /// <returns></returns>
        public static UsuarioServico.Usuario ObterUsuario(Int64? cpf, String email)
        {
            UsuarioServico.Usuario usuario = null;

            using (Logger log = Logger.IniciarLog("Método Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario.ObterUsuario"))
            {
                //Se não existir dados no cache consulta o usuário
                //Caso exista mas o PV informado não contem nos selecionados realiza a pesquisa novamente
                if (InformacaoUsuario.Existe())
                {
                    InformacaoUsuario cache = InformacaoUsuario.Recuperar();

                    //Verifica se o email/cpf que está na tela é o mesmo que está no cahe, 
                    //caso não seja limpa o cache e pega novamente o usuário
                    if (cpf.HasValue)
                    {
                        if (cache.CpfUsuario != cpf.Value)
                        {
                            InformacaoUsuario.Limpar();
                            return ObterUsuario(cpf, email);
                        }
                    }
                    else //if (!string.IsNullOrEmpty(email))
                    {
                        if (cache.EmailUsuario != email)
                        {
                            InformacaoUsuario.Limpar();
                            return ObterUsuario(cpf, email);
                        }
                    }

                    log.GravarMensagem("Informações existentes no cache");

                    usuario = new UsuarioServico.Usuario();
                    usuario.Entidade = new UsuarioServico.Entidade();
                    usuario.Entidade.GrupoEntidade = new UsuarioServico.GrupoEntidade();
                    usuario.Status = new UsuarioServico.Status();

                    usuario.Entidade.GrupoEntidade.Codigo = cache.GrupoEntidade;
                    usuario.Entidade.Codigo = cache.NumeroPV;
                    usuario.Status.Codigo = cache.Status;
                    usuario.Email = cache.EmailUsuario;

                    usuario.CodigoIdUsuario = cache.IdUsuario;
                    usuario.TipoUsuario = cache.TipoUsuario;
                    usuario.CPF = cache.CpfUsuario;

                    usuario.DDDCelular = cache.DddCelularUsuario;
                    usuario.Celular = cache.CelularUsuario;
                    usuario.EmailSecundario = cache.EmailSecundario;
                }
                else
                {
                    log.GravarMensagem("Informações não existentes no cache");

                    using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    {
                        if (cpf.HasValue)
                        {
                            UsuarioServico.Usuario[] usuarios = contextoUsuario.Cliente.ConsultarPorCpf(cpf.Value, 0, 0);

                            if (usuarios != null && usuarios.Any())
                            {
                                usuario = usuarios.FirstOrDefault();
                            }
                        }
                        else //if (!string.IsNullOrEmpty(email))
                        {
                            int codigoRetorno;
                            usuario = contextoUsuario.Cliente.ConsultarPorEmailPrincipal(out codigoRetorno, email, 0, 0);
                        }
                    }

                    if (usuario != null)
                    {
                        log.GravarMensagem("Usuário encontrado", new { usuario });
                        InformacaoUsuario.Salvar(CriarInformacaoUsuario(usuario));
                    }
                    else
                    {
                        log.GravarMensagem("Usuário não encontrado");
                    }
                }
            }
            return usuario;
        }

        /// <summary>
        /// Valida o usuário.
        /// </summary>
        /// <param name="usuario">Usuário</param>
        /// <param name="model">Modelo de retorno</param>
        public static Boolean ValidarUsuario(UsuarioServico.Usuario usuario, out ConsultaPvsHandlerResponse model)
        {
            Boolean retorno = false;
            model = null;
            switch (usuario.Status.Codigo)
            {
                case (Int32)Comum.Enumerador.Status.UsuarioAtivo:
                case (Int32)Comum.Enumerador.Status.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:
                case (Int32)Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoMaster:
                case (Int32)Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoAlteracaoEmail:
                case (Int32)Comum.Enumerador.Status.RespostaIdPosPendente:
                case (Int32)Comum.Enumerador.Status.UsuarioAtivoAguardandoConfirmacaoRecuperacaoSenha:
                    {
                        retorno = true;
                        break;
                    }
                case (Int32)Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                    {
                        model = ConstruirMensagemAviso(500, "Verificamos que você ainda não concluiu sua última solicitação. Acesse seu endereço de e-mail e clique no link enviado.");
                        retorno = false;
                        break;
                    }
                case (Int32)Comum.Enumerador.Status.UsuarioBloqueadoRecuperacaoSenha: //Cadastrar mensagem de bloqueio para recuperação
                case (Int32)Comum.Enumerador.Status.UsuarioBloqueadoRecuperacaoUsuario:
                    {
                        model = ConstruirMensagemErro(500, "RecuperacaoIdentificacao.ValidarIdentificacao", 1160, "A quantidade de tentativas foi esgotada");
                        retorno = false;
                        break;
                    }
                default:
                    {
                        model = ConstruirMensagemAviso(500, "Você não tem permissão para recuperar este usuário.");
                        retorno = false;
                        break;
                    }
            }
            return retorno;
        }

        /// <summary>
        /// Obtém a lista de PV's.
        /// </summary>
        /// <param name="cpf">CPF do usuário</param>
        /// <param name="email">Email do usuário.</param>
        /// <returns></returns>
        public static List<ConsultaPvsHandlerModel> ObterPvs(Int64? cpf, String email)
        {
            EntidadeServico.EntidadeServicoModel[] pvs = ObterPvsCache(cpf, email);

            var listPvs = new List<ConsultaPvsHandlerModel>();
            var pvsList = pvs.ToList();
            if (pvs != null && pvs.Length > 0)
            {
                listPvs = pvsList.ConvertAll<ConsultaPvsHandlerModel>(x => new ConsultaPvsHandlerModel
                {
                    Celular = x.Celular,
                    Email = x.Email,
                    EmailSecundario = x.EmailSecundario,
                    NomeEstabelecimento = x.RazaoSocial,
                    NumeroPv = x.NumeroPV,
                    DDDCelular = x.DDDCelular,
                    IdUsuario = x.IdUsuario,
                    NumeroPvMascarado = Util.TruncarNumeroPV(x.NumeroPV.ToString())
                });
            }

            return listPvs;
        }

        /// <summary>
        /// Obter os PV's do cache. Caso não existam informações dentro do cache, obtém do serviço.
        /// </summary>
        /// <param name="cpf">CPF do usuário</param>
        /// <param name="email">Email do usuário.</param>
        /// <param name="atualizar">Atualização</param>
        /// <returns></returns>
        private static EntidadeServico.EntidadeServicoModel[] ObterPvsCache(Int64? cpf, String email, Boolean atualizar = false)
        {
            int codigoRetorno = 0;
            EntidadeServico.EntidadeServicoModel[] estabelecimentos;
            InformacaoUsuario cache;
            bool? pvSenhasIguais = false;
            Int32 quantidadeEmaislDiferentes = 0;

            using (Logger log = Logger.IniciarLog("Método Redecard.PN.DadosCadastrais.SharePoint.WPAberto.RecuperacaoIdentificao.GetPVsCache"))
            {
                if (InformacaoUsuario.Existe() && atualizar == false)
                {
                    log.GravarMensagem("Existem dados no cache", new { possuiDadosCache = InformacaoUsuario.Existe() });

                    cache = InformacaoUsuario.Recuperar();
                    estabelecimentos = cache.EstabelecimentosRelacinados;
                }
                else
                {
                    log.GravarMensagem("Não existem dados no cache", new { possuiDadosCache = InformacaoUsuario.Existe() });

                    estabelecimentos = new EntidadeServico.EntidadeServicoModel[0];

                    ObterUsuario(cpf, email);
                    if (cpf.HasValue)
                    {
                        int totalLinhas = 0;
                        using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        {
                            estabelecimentos = contextoEntidade.Cliente.ConsultarPvPorCpfComPaginacao(out codigoRetorno, out totalLinhas, out quantidadeEmaislDiferentes, cpf.Value, 0, 9999, true, String.Empty, String.Empty);
                        }
                    }
                    else //if (!string.IsNullOrEmpty(email))
                    {
                        using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        {
                            estabelecimentos = contextoEntidade.Cliente.ConsultarPvPorEmail(out codigoRetorno, out pvSenhasIguais, email, null, String.Empty);
                        }
                    }

                    if (codigoRetorno > 0)
                    {
                        throw new Exception("Problemas para consultar pvs");
                    }

                    cache = InformacaoUsuario.Recuperar();

                    if (cache == null)
                    {
                        return estabelecimentos;
                    }

                    cache.PvSenhasIguais = pvSenhasIguais.Value;
                    cache.QuantidadeEmaislDiferentes = quantidadeEmaislDiferentes;
                    cache.EstabelecimentosRelacinados = estabelecimentos;

                    InformacaoUsuario.Salvar(cache);
                }
            }

            return estabelecimentos;
        }

        /// <summary>
        /// Criar a sessão aberta do usuário para prosseguir aos próximos passos
        /// </summary>
        /// <param name="usuario">Usuário selecionado</param>
        private static InformacaoUsuario CriarInformacaoUsuario(UsuarioServico.Usuario usuario)
        {
            InformacaoUsuario.Criar(usuario.Entidade.GrupoEntidade.Codigo, usuario.Entidade.Codigo, usuario.Email);
            InformacaoUsuario info = InformacaoUsuario.Recuperar();
            info.IdUsuario = usuario.CodigoIdUsuario;
            info.TipoUsuario = usuario.TipoUsuario;
            info.CpfUsuario = usuario.CPF.HasValue ? usuario.CPF.Value : 0;
            info.Status = usuario.Status.Codigo;
            info.DddCelularUsuario = usuario.DDDCelular.HasValue ? usuario.DDDCelular.Value : 0;
            info.CelularUsuario = usuario.Celular.HasValue ? usuario.Celular.Value : 0;
            info.EmailSecundario = usuario.EmailSecundario;

            info.PodeRecuperarCriarAcesso = true;

            InformacaoUsuario.Salvar(info);

            return info;
        }

        /// <summary>
        /// Valida o status do usuário
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="email">Email do usuário</param>
        /// <param name="model">Model de out</param>
        public static Boolean ValidarStatusUsuario(Int64? cpf, String email, out ConsultaPvsHandlerResponse model)
        {
            Boolean retorno = true;
            model = null;

            using (Logger log = Logger.IniciarLog("Validar status do usuário"))
            {
                UsuarioServico.Usuario[] usuarios = null;
                int codigoRetorno;

                using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                {
                    if (cpf.HasValue)
                    {
                        usuarios = contextoUsuario.Cliente.ConsultarPorCpfPrincipalPorStatus(out codigoRetorno, cpf.Value, 0, 0, null);
                    }
                    else
                    {
                        usuarios = contextoUsuario.Cliente.ConsultarPorEmailPrincipalPorStatus(out codigoRetorno, email, 0, 0, null);
                    }
                }

                if (usuarios != null && usuarios.Length == 1)
                {
                    retorno = ValidarStatusUsuario(usuarios.FirstOrDefault(), out model);
                }
                else if (usuarios != null && usuarios.Length > 1)
                {
                    foreach (var item in usuarios)
                    {
                        if (item.Status.Codigo == (Int32)Comum.Enumerador.Status.UsuarioBloqueadoSenhaInvalida)
                        {
                            model = ConstruirMensagemAviso(500, "Solicite o desbloqueio ao usuário master do seu estabelecimento.", titulo: "Acesso bloqueado");
                            retorno = false;
                        }
                    }
                }
                else
                {
                    model = ConstruirMensagemAviso(500, "Os dados informados estão inválidos. Por favor, verifique se os dados digitados estão corretos e tente novamente.", true, "Os dados informados estão inválidos");
                }

                return retorno;
            }
        }

        /// <summary>
        /// Valida o status do usuário
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private static Boolean ValidarStatusUsuario(UsuarioServico.Usuario usuario, out ConsultaPvsHandlerResponse model)
        {
            model = null;

            switch (usuario.Status.Codigo)
            {
                case (Int32)Comum.Enumerador.Status.UsuarioAtivo:
                case (Int32)Comum.Enumerador.Status.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:
                case (Int32)Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoAlteracaoEmail:
                case (Int32)Comum.Enumerador.Status.RespostaIdPosPendente:
                case (Int32)Comum.Enumerador.Status.UsuarioAtivoAguardandoConfirmacaoRecuperacaoSenha:
                    {
                        return true;
                    }
                case (Int32)Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                    {
                        model = new ConsultaPvsHandlerResponse()
                        {
                            FlagModal = true,
                            TipoModal = Usuario.TipoModal.Warning,
                            TituloModal = "Atenção",
                            StatusCode = 500,
                            FieldValidationReturn = "email|cpf",
                            MensagemRetorno = String.Format(@"
<span class=""other-text"">
Este e-mail (usuário) está aguardando confirmação.<br /><br />
<span class=""semibold"">Não recebeu o e-mail?</span><br />
Verifique se o e-mail não está na sua caixa de spam ou na lixeira:
<br /><br />
<ul class=""bullets-list-rede"">
    <li><p>o remetente do e-mail é <span class=""semibold"">faleconosco@userede.com.br</span></p></li>
    <li><p>o título do e-mail é <span class=""semibold"">confirmação de cadastro portal Rede</span></p></li>
</ul><br />
<a href=""javascript:void(0)"" onclick=""CustomPostBack('ReenviarEmail', '{0}')"">Clique aqui</a> 
para reenviar o e-mail de confirmação.</span>", 
                                              String.Format("ReenviarEmailConfirmacao|{0}|{1}|{2}", usuario.Entidade.Codigo, usuario.Email, usuario.CPF.GetValueOrDefault(0).ToString()))
                        };
                        return false;
                    }
                case (Int32)Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoMaster:
                    {
                        model = new ConsultaPvsHandlerResponse()
                        {
                            StatusCode = 500,
                            FieldValidationReturn = "email|cpf",
                            MensagemRetorno = String.Format(@"
<span class=""other-text"">Usuário aguardando aprovação. Por favor, entre em contato com o usuário master ou
<a href=""javascript:void(0)"" onclick=""CustomPostBack('ReenviarEmail', '{0}')"">clique aqui</a> 
para reenviar o e-mail de aprovação.</span>",
                                            String.Format("ReenviarSolicitacaoAprovacao|{0}|{1}|{2}", usuario.Entidade.Codigo, usuario.Email, usuario.CPF.GetValueOrDefault(0).ToString()))
                        };
                        return false;
                    }
                case (Int32)Comum.Enumerador.Status.UsuarioBloqueadoRecuperacaoSenha: //Cadastrar mensagem de bloqueio para recuperação
                case (Int32)Comum.Enumerador.Status.UsuarioBloqueadoRecuperacaoUsuario:
                    {
                        model = ConstruirMensagemErro(500, "RecuperacaoIdentificacao.ValidarIdentificacao", 1160, "A quantidade de tentativas foi esgotada.");
                        return false;
                    }
                case (Int32)Comum.Enumerador.Status.UsuarioBloqueadoSenhaInvalida:
                    {
                        model = ConstruirMensagemAviso(500, "Solicite o desbloqueio ao usuário master do seu estabelecimento.", titulo: "Acesso bloqueado");
                        return false;
                    }
                default:
                    {
                        model = ConstruirMensagemAviso(500, "Você não tem permissão para recuperar este usuário.");
                        return false;
                    }
            }
        }

        /// <summary>
        /// Remove da lista de PVs selecionados aqueles que não estiverem na listagem de PVs base
        /// </summary>
        /// <param name="pvsSelecionados">Lista de PVs selecionados que precisam ser saneados</param>
        /// <param name="pvsBase">Lista de PVs base</param>
        /// <returns>Lista dos números dos PVs que receberão o filtro</returns>
        public static Int32[] SanearPvs(Int32[] pvsSelecionados, Int32[] pvsBase)
        {
            return SanearPvs(pvsSelecionados.ToList(), pvsBase.ToList()).ToArray();
        }

        /// <summary>
        /// Remove da lista de PVs selecionados aqueles que não estiverem na listagem de PVs base
        /// </summary>
        /// <param name="pvsSelecionados">Lista de PVs selecionados que precisam ser saneados</param>
        /// <param name="pvsBase">Lista de PVs base</param>
        /// <returns>Lista dos números dos PVs que receberão o filtro</returns>
        public static List<Int32> SanearPvs(List<Int32> pvsSelecionados, List<Int32> pvsBase)
        {
            // trata apenas se a lista de PVs base tiver conteúdo
            if (pvsBase == null || pvsBase.Count == 0)
                return pvsSelecionados;

            return pvsSelecionados.Where(x => pvsBase.Contains(x)).ToList();
        }

        /// <summary>
        /// Remove da lista de PVs selecionados aqueles que não estiverem na listagem de PVs base
        /// </summary>
        /// <param name="pvsSelecionados">Lista de PVs selecionados que precisam ser saneados</param>
        /// <param name="pvsBase">Lista de PVs base</param>
        /// <returns>Lista dos PVs que receberão o filtro</returns>
        public static List<ConsultaPvsHandlerModel> SanearPvs(List<ConsultaPvsHandlerModel> pvsSelecionados, List<ConsultaPvsHandlerModel> pvsBase)
        {
            // trata apenas se a lista de PVs base tiver conteúdo
            if (pvsBase == null || pvsBase.Count == 0)
                return pvsSelecionados;

            return pvsSelecionados.Where(x => pvsBase.Any(b => b.NumeroPv == x.NumeroPv)).ToList();
        }

        /// <summary>
        /// Remove da lista de PVs selecionados aqueles que não estiverem na listagem de PVs base
        /// </summary>
        /// <param name="pvsSelecionados">Lista de PVs selecionados que precisam ser saneados</param>
        /// <param name="pvsBase">Lista de PVs base</param>
        /// <returns>Lista dos PVs que receberão o filtro</returns>
        public static List<ConsultaPvsHandlerModel> SanearPvs(List<Int32> pvsSelecionados, List<ConsultaPvsHandlerModel> pvsBase)
        {
            // trata apenas se a lista de PVs base tiver conteúdo
            if (pvsBase == null || pvsBase.Count == 0)
                return new List<ConsultaPvsHandlerModel>();

            return pvsBase.Where(x => pvsSelecionados.Contains(x.NumeroPv)).ToList();
        }

        /// <summary>
        /// Verifica se deve exibir o captcha
        /// </summary>
        /// <param name="context">Context da requisição atual</param>
        /// <returns>TRUE: deve exibir</returns>
        public static Boolean ExibirRecaptcha(HttpContext context)
        {
            using (Logger log = Logger.IniciarLog("CriacaoUsuario.ExibirCaptcha()"))
            {
                try
                {
                    return ControlTemplates.RecaptchaGoogle.CheckShowCaptcha(context, true);
                }
                catch (ArgumentException ex)
                {
                    log.GravarErro(ex);
                    return false;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    return false;
                }
                catch (InvalidOperationException ex)
                {
                    log.GravarErro(ex);
                    return false;
                }
                catch (UriFormatException ex)
                {
                    log.GravarErro(ex);
                    return false;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    return false;
                }
            }
        }

        #region [ Mensagens Erro/Aviso ]
        /// <summary>
        /// Constrói mensagem de erro para ser exibida no front-end, obtendo a mensagem do serviço e exibindo um título customizado.
        /// </summary>
        /// <param name="httpStatusCode">Código do retorno HTTP</param>
        /// <param name="fonte">Fonte do erro (PN)</param>
        /// <param name="codigo">Contexto do erro (PN)</param>
        /// <param name="titulo">Título customizado</param>
        /// <param name="tentarNovamente">Define se o usuário poderá tentar novamente a inserção.</param>
        public static ConsultaPvsHandlerResponse ConstruirMensagemErro(Int32 httpStatusCode, String fonte, Int32 codigo, String titulo = "Atenção", Boolean showModal = true)
        {
            String mensagemErroServidor = new UserControlBase().RetornarMensagemErro(fonte, codigo);
            return ConstruirMensagemErro(httpStatusCode, mensagemErroServidor, String.Empty, titulo, showModal);
        }

        /// <summary>
        /// Constrói mensagem de erro para ser exibida no front-end, obtendo a mensagem do serviço e exibindo um título customizado.
        /// </summary>
        /// <param name="httpStatusCode">Código do retorno HTTP</param>
        /// <param name="mensagemErro">mensagem de ero</param>
        /// <param name="stackTraceExcecao">detalhes do erro</param>
        /// <param name="titulo">Título customizado</param>
        /// <returns></returns>
        public static ConsultaPvsHandlerResponse ConstruirMensagemErro(Int32 httpStatusCode, String mensagemErro, String stackTraceExcecao = null, String titulo = "Atenção", Boolean showModal = true, Boolean dispararTagGtm = false, String labelGtm = "")
        {
            ConsultaPvsHandlerResponse model = new ConsultaPvsHandlerResponse();
            model.TipoModal = TipoModal.Error;
            model.FlagModal = showModal;
            model.StatusCode = httpStatusCode;
            model.TituloModal = titulo;
            model.MensagemRetorno = mensagemErro;
            model.DetalheExcecao = stackTraceExcecao;
            model.DispararTagGtm = dispararTagGtm;
            model.LabelGtm = labelGtm;
            return model;
        }

        /// <summary>
        /// Constrói mensagem de aviso para ser exibida no front-end, obtendo a mensagem do serviço.
        /// </summary>
        /// <param name="httpStatusCode">Código do retorno HTTP</param>
        /// <param name="fonte">Fonte do erro (PN)</param>
        /// <param name="codigo">Contexto do erro (PN)</param>
        /// <param name="tentarNovamente">Define se o usuário poderá tentar novamente a inserção.</param>
        public static ConsultaPvsHandlerResponse ConstruirMensagemAviso(Int32 httpStatusCode, String fonte, Int32 codigo)
        {
            String mensagemAviso = new UserControlBase().RetornarMensagemErro(fonte, codigo);
            return ConstruirMensagemAviso(httpStatusCode, mensagemAviso);
        }

        /// <summary>
        /// Constrói mensagem de aviso para ser exibida no front-end.
        /// </summary>
        /// <param name="httpStatusCode">Código do retorno HTTP</param>
        /// <param name="mensagemAviso">Mensagem de aviso a ser exibida.</param>
        /// 
        /// 
        /// <param name="tentarNovamente">Define se o usuário poderá tentar novamente a inserção.</param>
        public static ConsultaPvsHandlerResponse ConstruirMensagemAviso(Int32 httpStatusCode, String mensagemAviso, Boolean dispararTagGtm = false, String labelGtm = "", Boolean tentarNovamente = false, String titulo = "Atenção")
        {
            ConsultaPvsHandlerResponse model = new ConsultaPvsHandlerResponse();
            model.TipoModal = TipoModal.Warning;
            model.FlagModal = true;
            model.StatusCode = httpStatusCode;
            model.TituloModal = titulo;
            model.MensagemRetorno = mensagemAviso;
            model.DispararTagGtm = dispararTagGtm;
            model.LabelGtm = labelGtm;
            return model;
        }
        #endregion

        #region [ Recuperação de usuario ]
        /// <summary>
        /// Grava um histórico dos pvs 
        /// </summary>
        /// <param name="idUsuario">Id do usuário</param>
        /// <param name="nomeCompleto">Nome completo</param>
        /// <param name="emailUsuario">Email usuário</param>
        /// <param name="tipoUsuario">Tipo usuário</param>
        /// <param name="pvsSelecionados">Pvs selecionados</param>
        public static void HistoricoRecuperacaoUsuario(int idUsuario, string nomeCompleto, string emailUsuario, string tipoUsuario, int[] pvsSelecionados)
        {
            foreach (var numPv in pvsSelecionados)
            {
                Historico.RecuperacaoUsuario(idUsuario,
                                             nomeCompleto,
                                             emailUsuario,
                                             tipoUsuario,
                                             numPv);
            }
        }

        /// <summary>
        /// Atualização do Status do usuário
        /// </summary>
        /// <param name="usuarioCache">usuario que esta em cache</param>
        public static void AtualizarStatusPosPendente(InformacaoUsuario usuarioCache)
        {
            if (usuarioCache.PvsSelecionados == null || !usuarioCache.PvsSelecionados.Any())
                usuarioCache.PvsSelecionados = new Int32[] { usuarioCache.NumeroPV };

            using (Logger log = Logger.IniciarLog("Atualizando status usuário para UsuarioServico.Status1.RespostaIdPosPendente"))
            {
                log.GravarMensagem("Preparando para atualizar status");
                using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>().Cliente)
                {
                    ctx.AtualizarStatusPorPvs(usuarioCache.PvsSelecionados, usuarioCache.CpfUsuario, null, UsuarioServico.Status1.RespostaIdPosPendente);
                }
                log.GravarMensagem("Staus atualizado com sucesso", new { status = UsuarioServico.Status1.RespostaIdPosPendente });
            }
        }
        #endregion

        #region [ Criação de usuário ]

        /// <summary>
        /// Método para limpar o conteúdo salvo em sessão para reiniciar o processo de cadastro
        /// </summary>
        public static void LimparDadosSessaoCriacaoUsuario()
        {
            HttpContext.Current.Session.Remove("PvsDictionary");
        }

        /// <summary>
        /// Método para consulta se o CPF/CNPJ informado pertence a um sócio
        /// </summary>
        /// <param name="pv">Número do PV para consulta</param>
        /// <param name="cpfCnpj">CPF/CNPJ a ser verificado</param>
        /// <returns></returns>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        public static Boolean VerificarCpfSocioCriacaoUsuario(Int32 pv, long cpfCnpj)
        {
            using (var ctxUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                return ctxUsuario.Cliente.ValidarCpfCnpjSocio(pv, cpfCnpj);
        }

        /// <summary>
        /// Método para consultar os PVs para a criação de usuários
        /// </summary>
        /// <param name="cpf">CPF do estabelecimento (informar NULL se for PJ)</param>
        /// <param name="cnpj">CNPJ do estabelecimento (informar NULL se for PF)</param>
        /// <param name="msgRetorno">Mensagem de retorno para comunicação com o usuário</param>
        /// <returns></returns>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        public static List<ConsultaPvsHandlerModel> GetPvsCriacaoUsuario(String email, long? cpf, long? cnpj, out QuadroAvisoModel msgRetorno)
        {
            msgRetorno = null;

            var listPvs = GetPvsSessionCriacaoUsuario(cnpj.HasValue ? cnpj.Value : cpf.Value);
            if (listPvs == null)
            {
                // consulta os PVs na camada de serviços
                using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    int codigoRetorno = 0;
                    var pvsServico = contextoEntidade.Cliente.ConsultarEntidadeGeCriarNoPN(out codigoRetorno, cpf, cnpj);
                    if (pvsServico == null || codigoRetorno > 0)
                    {
                        msgRetorno = new QuadroAvisoModel(TipoModal.Error, "Erro ao consultar os estabelecimentos", "Tente novamente mais tarde");
                        return null;
                    }

                    // aplica o filtro à relação dos PVs retornados
                    if (pvsServico.Length > 1)
                    {
                        pvsServico = pvsServico.Where(x =>
                            x.PossuiMaster.GetValueOrDefault(true) == false &&
                            x.PossuiUsuario.GetValueOrDefault(true) == false &&
                            x.Status != (Int32)Comum.Enumerador.Status.EntidadeBloqueadaConfirmacaoPositiva).ToArray();
                    }

                    // verifica se foi retornado algum estabelecimento
                    if (pvsServico.Length == 0)
                    {
                        msgRetorno = new QuadroAvisoModel(TipoModal.Error, "Atenção", "Nenhum estabelecimento encontrado com os dados informados");
                        return new List<ConsultaPvsHandlerModel>();
                    }

                    // valida IP do cliente na blacklist
                    BlacklistIPs blacklist = new BlacklistIPs();
                    if (!blacklist.ValidarIP(BlacklistIPs.GetClientIP()))
                    {
                        msgRetorno = new QuadroAvisoModel(
                            TipoModal.Error,
                            "Criação de usuário não permitida",
                            "Favor entrar em contato com a central de atendimento para mais informações.");
                        Historico.BloqueioIPBlackListPrimeiroAcesso(email, cpf, cnpj);
                        return null;
                    }

                    Int32 pvLog = pvsServico.FirstOrDefault().NumeroPV;

                    // remove as entidades cujos PVs foram inclusos na BlackList
                    BlacklistPVs blacklistPvs = new BlacklistPVs();
                    var pvsBloqueados = blacklistPvs.PVsBloqueados;
                    pvsServico = pvsServico.Where(x => !pvsBloqueados.Contains(x.NumeroPV)).ToArray();

                    if (pvsServico.Length == 0)
                    {
                        msgRetorno = new QuadroAvisoModel(
                            TipoModal.Error,
                            "Criação de usuário não permitida",
                            "Favor entrar em contato com a central de atendimento para mais informações.");
                        Historico.BloqueioPVBlackListPrimeiroAcesso(pvLog, email);
                        return null;
                    }

                    // inclui a listagem de PVs ao modelo
                    listPvs = pvsServico.Select(x => new ConsultaPvsHandlerModel
                    {
                        NumeroPv = x.NumeroPV,
                        NomeEstabelecimento = x.RazaoSocial,
                        Email = x.Email,
                        NumeroPvMascarado = Util.TruncarNumeroPV(x.NumeroPV.ToString()),
                        Status = x.Status,
                        PossuiMaster = x.PossuiMaster,
                        PossuiUsuario = x.PossuiUsuario
                    }).ToList();

                    if (listPvs.Count > 0)
                    {
                        // insere na sessão para otimizar futuras consultas
                        SetPvsSessionCriacaoUsuario(cnpj.HasValue ? cnpj.Value : cpf.Value, listPvs);
                    }
                }
            }

            return listPvs;
        }

        private static void  GravarHistoricoPVBlackListPrimeiroAcesso(Int32 pv, String email)
        {
            Historico.BloqueioPVBlackListPrimeiroAcesso(pv, email);
        }


        /// <summary>
        /// Trata os PVs para a criação de usuário
        /// </summary>
        /// <param name="listPvs">Lista com os PVs para validação</param>
        /// <param name="msgRetorno">RETORNO: mensagem de erro no formato de quadro aviso</param>
        /// <returns></returns>
        public static Boolean TrataPvsCriacaoUsuario(List<ConsultaPvsHandlerModel> listPvs, out QuadroAvisoModel msgRetorno)
        {
            msgRetorno = null;

            if (listPvs != null && listPvs.Count == 1)
            {
                var pv = listPvs[0];
                if (pv.Status.GetValueOrDefault(0) == (Int32)Status.EntidadeBloqueadaConfirmacaoPositiva)
                {
                    msgRetorno = new QuadroAvisoModel(TipoModal.Error, "Acesso bloqueado", @"
Solicite o desbloqueio para um usuário master ou entre em contato com a central de atendimento.<br /><br />
<b>4001–4433 (capitais e regiões metropolitanas)</b><br />
<b>0800 728 4433 (demais localidades)</b>");
                    return false;
                }
                else if (pv.PossuiMaster.GetValueOrDefault(false))
                {
                    msgRetorno = new QuadroAvisoModel(TipoModal.Warning, "Atenção", @"
Este estabelecimento já possui usuários cadastrados. 
Conclua o preenchimento dos seus dados e aguarde a aprovação do usuário master.
<div style=""display: none;"" class=""display-content"" data-display-step=""3"">
    <span class=""bold"">Atenção!</span> Este estabelecimento já possui usuários cadastrados. Conclua o preenchimento dos seus dados e aguarde a aprovação do usuário master.
</div>
");
                    return false;
                }
                else if (pv.PossuiUsuario.GetValueOrDefault(false))
                {
                    msgRetorno = new QuadroAvisoModel(TipoModal.Error, "Atenção", @"
Este estabelecimento já possui um usuário cadastrado, porém ainda não ativou seu acesso completo.<br /><br />
Para obter seu acesso você deve entrar em contato com o usuário cadastrado e solicitar a criação do seu usuário.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Valida se o CNPJ informado atende aos requisitos para criação de usuário
        /// </summary>
        /// <param name="cnpj">CNPJ a ser validado</param>
        /// <param name="msgRetorno">Retorno da mensagem segundo a validação</param>
        /// <returns>TRUE: CNPJ válido</returns>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        /// <exception cref="System.FormatException">CNPJ em formato inapropriado segundo o tipo Int64</exception>
        /// <exception cref="System.OverflowException">CNPJ informado excede os limites aceitos para o tipo Int64</exception>
        public static Boolean ValidarCnpjEstabelecimentoCriacaoUsuario(String cnpj, out QuadroAvisoModel msgRetorno)
        {
            msgRetorno = null;

            using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
            {
                int codigoRetorno = 0;
                if (contextoEntidade.Cliente.PvsRelacionadosSaoFiliais(out codigoRetorno, Convert.ToInt64(cnpj)))
                {
                    msgRetorno = new QuadroAvisoModel(
                        TipoModal.Error,
                        "Criação de usuário não permitida para filial",
                        "O acesso deve ser criado com o CNPJ da matriz e a partir desse usuário, realizar a criação do usuário para filial.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Retorna se o PV possui tecnologia Komerci
        /// </summary>
        /// <param name="codigoEntidade">Entidade a ser verificada</param>
        /// <returns>TRUE: possui Komerci</returns>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        public static Boolean VerificarPossuiKomerci(Int32 codigoEntidade)
        {
            using (var entidadeServico = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
            {
                Int32 codigoTecnologia = 0;
                Int32 codigoRetorno = 0;

                codigoTecnologia = entidadeServico.Cliente.ConsultarTecnologiaEstabelecimento(out codigoRetorno, codigoEntidade);
                Boolean pvKomerci = (codigoTecnologia.Equals(26) || codigoTecnologia.Equals(25) || codigoTecnologia.Equals(23));
                return pvKomerci;
            }
        }

        /// <summary>
        /// Obtém o dicionário de PVs a partir da sessão do usuário
        /// </summary>
        /// <param name="context">Context da comunicação atual</param>
        /// <returns>Dicionário com a listagem de PVs por CPF/CNPJ</returns>
        private static Dictionary<long, List<ConsultaPvsHandlerModel>> GetPvsCriacaoUsuarioDictionary()
        {
            if (HttpContext.Current.Session["PvsDictionary"] == null)
                HttpContext.Current.Session["PvsDictionary"] = new Dictionary<long, List<ConsultaPvsHandlerModel>>();

            return (Dictionary<long, List<ConsultaPvsHandlerModel>>)HttpContext.Current.Session["PvsDictionary"];
        }

        /// <summary>
        /// Consulta lista de PVs salvos em sessão
        /// </summary>
        /// <param name="context">Context da comunicação atual</param>
        /// <param name="cpfCnpj">CPF/CNPJ usado para consulta</param>
        /// <returns>Listagem de PVs relacionados ao CPF/CNPJ informado</returns>
        private static List<ConsultaPvsHandlerModel> GetPvsSessionCriacaoUsuario(long cpfCnpj)
        {
            var dicPvs = GetPvsCriacaoUsuarioDictionary();
            List<ConsultaPvsHandlerModel> retorno = null;
            dicPvs.TryGetValue(cpfCnpj, out retorno);
            return retorno;
        }

        /// <summary>
        /// Perssite a listagem de PVs em sessão para otimização de futuras consultas
        /// </summary>
        /// <param name="context">Context da comunicação atual</param>
        /// <param name="cpfCnpj">CPF/CNPJ a ser usado como critério para persistência dos PVs</param>
        /// <param name="listPvs">Listagem dos PVs a serem persistidos em sessão</param>
        private static void SetPvsSessionCriacaoUsuario(long cpfCnpj, List<ConsultaPvsHandlerModel> listPvs)
        {
            var dicPvs = GetPvsCriacaoUsuarioDictionary();
            if (!dicPvs.ContainsKey(cpfCnpj))
            {
                dicPvs.Add(cpfCnpj, listPvs);
            }
        }

        /// <summary>
        /// Consulta a relação de bancos para confirmação positiva
        /// </summary>
        /// <returns>Lista de bancos em formato ListItemCollection</returns>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        public static ListItem[] ConsultarBancosConfirmacaoPositiva()
        {
            using (Logger Log = Logger.IniciarLog("Consultando bancos para o cadastro de usuário"))
            using (var entidadeClient = new EntidadeServico.EntidadeServicoClient())
            {
                var bancos = entidadeClient.ConsultarBancosConfirmacaoPositiva();
                return bancos.Select(banco => new ListItem()
                {
                    Text = String.Format("{0} - {1}", banco.Codigo.PadLeft(3, '0'), banco.Descricao),
                    Value = banco.Codigo
                }).ToArray();
            }
        }

        /// <summary>
        /// Verifica o e-mail do usuário antes de prosseguir
        /// </summary>
        /// <param name="email">E-mail para validação</param>
        /// <param name="codigoEntidade">Código do estabelecimento a ser validado</param>
        /// <param name="mensagemRetorno">Retorno: mensagem em formato HTML sobre o e-mail informado</param>
        /// <param name="modal">Retorno: Identifica se deve exibir a mensagem com modal</param>
        /// <returns>TRUE: E-mail do usuário válido</returns>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        public static Boolean ValidarEmailCriacaoUsuario(String email, Int32 codigoEntidade, out String mensagemRetorno, out Boolean modal)
        {
            Guid hash = Guid.Empty;
            mensagemRetorno = String.Empty;
            modal = false;

            // verifica se já existe usuário com o mesmo e-mail para o PV           
            Boolean statusConsulta = false;
            UsuarioServico.Usuario usuario = ConsultarUsuarioPorEmail(email, codigoEntidade, out statusConsulta, out hash);

            if (usuario == null)
            {
                // não existe usuário com o mesmo e-mail, então tenta verificar se existe usuário com e-mail temporário igual para o PV
                usuario = ConsultarUsuarioPorEmailTemporario(email, codigoEntidade, out statusConsulta);
            }

            // se o usuário existe na base para o PV informado, impede prosseguir com o cadastro
            if (usuario != null)
            {
                switch ((UsuarioServico.Status1)usuario.Status.Codigo.Value)
                {
                    // se e-mail é de usuário Aguardando Aprovação do Master, informa que está aguardando aprovação
                    // se está na Parte Aberta, exibe link para reenvio da solicitação de aprovação para o Master
                    case UsuarioServico.Status1.UsuarioAguardandoConfirmacaoMaster:
                        mensagemRetorno = string.Format(@"
<span class=""other-text"">Este e-mail (usuário) está aguardando confirmação. 
<a href=""javascript:void(0)"" onclick=""CustomPostBack('ReenviarEmail', '{0}')"">Clique aqui</a> 
para solicitar a confirmação do usuário master.</span>",
                            String.Concat("ReenviarSolicitacaoAprovacao|", codigoEntidade));
                        break;

                    // se usuário aguardando confirmação para validação do e-mail informado no cadastro
                    case UsuarioServico.Status1.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                        mensagemRetorno = string.Format(@"
<span class=""other-text"">
Este e-mail (usuário) está aguardando confirmação.<br /><br />
<span class=""semibold"">Não recebeu o e-mail?</span><br />
Verifique se o e-mail não está na sua caixa de spam ou na lixeira:
<br /><br />
<ul class=""bullets-list-rede"">
    <li><p>o remetente do e-mail é <span class=""semibold"">faleconosco@userede.com.br</span></p></li>
    <li><p>o título do e-mail é <span class=""semibold"">confirmação de cadastro portal Rede</span></p></li>
</ul><br />
<a href=""javascript:void(0)"" onclick=""CustomPostBack('ReenviarEmail', '{0}')"">Clique aqui</a> 
para reenviar o e-mail de confirmação.</span>",
                            String.Concat("ReenviarEmailConfirmacao|", codigoEntidade));
                        modal = true;
                        break;

                    // se usuário Ativo, informa que usuário já existe
                    // se está na Parte Aberta, exibe link para recuperação de senha
                    case UsuarioServico.Status1.UsuarioAtivo:
                    case UsuarioServico.Status1.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:
                        mensagemRetorno = @"
<span class=""other-text"">Usuário já existente. 
Utilize a <a href=""/pt-br/novoacesso/Paginas/RecuperacaoSenhaIdentificacao.aspx"">recuperação de senha</a></span>";
                        break;

                    // para qualquer outro status, informa que usuário já existe
                    default:
                        mensagemRetorno = "Usuário já existente";
                        break;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida o domínio do e-mail informado na criação de usuário
        /// </summary>
        /// <param name="email">E-mail a ser validado</param>
        /// <param name="mensagemRetorno">TRUE: domínio do e-mail válido</param>
        /// <returns></returns>
        public static Boolean ValidarDominioEmailCriacaoUsuario(String email, long? cpfEstab, long? cnpjEstab, out String mensagemRetorno)
        {
            mensagemRetorno = String.Empty;

            // valida domínios bloqueados
            String blockedDomain = String.Empty;
            if (!CampoEmailValidator.ValidateBlockedDomain(email, DominiosBloqueados, out blockedDomain))
            {
                mensagemRetorno = String.Format("@{0} é um domínio inválido. Favor inserir outro e-mail", blockedDomain);
                Historico.BloqueioDominioEmailBlackListPrimeiroAcesso(email, cpfEstab, cnpjEstab);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Consulta usuário através do e-mail, para determinado estabelecimento
        /// </summary>
        /// <param name="email">E-mail para busca</param>
        /// <param name="codigoEntidade">Código da entidade do usuário a ser buscado</param>
        /// <param name="sucesso">Retorno: se a verificação foi realizada com sucesso</param>
        /// <param name="hash">Retorno: hash relacionado ao registro de envio de e-mail ao usuário</param>
        /// <returns>Usuário encontrado</returns>        
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        public static UsuarioServico.Usuario ConsultarUsuarioPorEmail(
            String email,
            Int32 codigoEntidade,
            out Boolean sucesso,
            out Guid hash)
        {
            hash = Guid.Empty;
            sucesso = true;
            UsuarioServico.Usuario usuario = null;
            Int32 codigoRetorno = 0;

            using (Logger log = Logger.IniciarLog("Consulta usuário através do e-mail"))
            using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
            {
                var usuarios = ctx.Cliente.ConsultarPorEmailPrincipalPorStatus(out codigoRetorno, email, 0, codigoEntidade, null);
                if (usuarios != null && usuarios.Any())
                {
                    usuario = usuarios.FirstOrDefault();

                    var hashUsuarios = ctx.Cliente.ConsultarHash(out codigoRetorno, usuario.CodigoIdUsuario, null, null);
                    if (hashUsuarios != null && hashUsuarios.Any())
                        hash = hashUsuarios.FirstOrDefault().Hash;
                }
                else
                {
                    sucesso = false;
                }
            }

            return usuario;
        }


        /// <summary>
        /// Consulta usuário através do e-mail temporário, para determinado estabelecimento
        /// </summary>
        /// <param name="emailTemporario">E-mail</param>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <param name="sucesso">Se a verificação foi realizada com sucesso</param>
        /// <returns>Usuário encontrado (deve ser apenas 1)</returns>        
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        public static UsuarioServico.Usuario ConsultarUsuarioPorEmailTemporario(
            String emailTemporario,
            Int32 codigoEntidade,
            out Boolean sucesso)
        {
            sucesso = true;
            UsuarioServico.Usuario usuario = null;

            using (Logger log = Logger.IniciarLog("Consulta usuário através do e-mail temporário"))
            using (var wcf = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
            {
                Int32 codigoRetorno = 0;
                usuario = wcf.Cliente.ConsultarPorEmailTemporario(out codigoRetorno, emailTemporario, 1, codigoEntidade);
                sucesso = usuario != null;
            }

            return usuario;
        }

        /// <summary>
        /// Consulta usuários Master do estabelecimento
        /// </summary>
        /// <param name="codigoEntidade">Código da entidade</param>
        /// <returns>Array com os usuários master relacionados à entidade informada</returns>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        public static EntidadeServico.Usuario[] ConsultarUsuariosMaster(Int32 codigoEntidade)
        {
            Int32 codigoRetorno = 0;
            EntidadeServico.Usuario[] usuarios = null;

            using (Logger log = Logger.IniciarLog("Consulta usuários de Perfil Master do Estabelecimento"))
            using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
            {
                usuarios = ctx.Cliente.ConsultarUsuariosPorPerfil(out codigoRetorno, codigoEntidade, 1, 'M');
            }

            return usuarios;
        }

        /// <summary>
        /// Validação da confirmação positiva para o cadastro de usuário na áre aberta
        /// </summary>
        /// <param name="cnpjEstabelecimento">CNPJ do estabelecimento para consulta</param>
        /// <param name="cpfProprietario">CPF do proprietário para consulta</param>
        /// <param name="emailUsuario">E-mail do usuário para consulta</param>
        /// <param name="pvs">Estabelecimentos para consulta</param>
        /// <param name="banco">Banco para validação</param>
        /// <param name="agencia">Agência para validação</param>
        /// <param name="contaCorrente">Conta corrente para validação</param>
        /// <param name="cpfCnpjSocio">CPF do sócio para validação. 'NULL' caso não queira incluir o valor na validação</param>
        /// <param name="msgRetorno">Mensagem (em formatos texto e HTML) a respeito da validação</param>
        /// <returns>TRUE: estabelecimento válido</returns>
        /// <exception cref="System.TimeoutException">Timeout na espera por resposta da camada de serviços</exception>
        /// <exception cref="System.ServiceModel.FaultException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ServiceModel.CommunicationException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.Web.HttpException">Falha de comunicação ao consultar a camada de serviços</exception>
        /// <exception cref="System.ArgumentNullException">Falha devido inserção de algum argumento nulo</exception>
        public static Boolean ValidarConfirmacaoPositiva(
            long? cnpjEstabelecimento,
            long? cpfProprietario,
            String emailUsuario,
            Int32[] pvs,
            long? cpfCnpjSocio,
            String banco,
            String agencia,
            String contaCorrente,
            out QuadroAvisoModel msgRetorno)
        {
            msgRetorno = null;

            // valida os dados informados
            if (!cpfProprietario.HasValue && !cnpjEstabelecimento.HasValue
                || String.IsNullOrWhiteSpace(emailUsuario)
                || String.IsNullOrWhiteSpace(banco)
                || String.IsNullOrWhiteSpace(agencia)
                || String.IsNullOrWhiteSpace(contaCorrente)
                || pvs == null
                || pvs.Length == 0)
            {
                msgRetorno = new QuadroAvisoModel(TipoModal.Error, "Atenção", "Não foi possível validar os dados informados");
                return false;
            }

            // obtém os PVs do estabelecimento em questão
            var pvsEstabelecimento = GetPvsCriacaoUsuario(emailUsuario, cpfProprietario, cnpjEstabelecimento, out msgRetorno);
            pvs = UsuarioNegocio.SanearPvs(pvs, pvsEstabelecimento.Select(x => x.NumeroPv).ToArray());
            if (pvs.Length == 0)
            {
                msgRetorno = new QuadroAvisoModel(TipoModal.Error, "Atenção", "Não foi possível validar os dados informados");
                return false;
            }

            EntidadeServico.ConfirmacaoPositivaPrimeiroAcessoResponse response = new EntidadeServico.ConfirmacaoPositivaPrimeiroAcessoResponse();
            EntidadeServico.EntidadeServicoModel[] estabelecimentos = pvs.Select(x =>
            {
                return new EntidadeServico.EntidadeServicoModel()
                {
                    NumeroPV = x
                };
            }).ToArray();
            EntidadeServico.EntidadeServicoModel[] entidadesPossuemUsuario;
            EntidadeServico.EntidadeServicoModel[] entidadesPossuemMaster;

            using (Logger log = Logger.IniciarLog("Confirmação positiva"))
            {
                UsuarioServico.Pergunta[] perguntasIncorretas = new UsuarioServico.Pergunta[] { 
                    new UsuarioServico.Pergunta() {
                        Codigo = 3, 
                        Descricao = PerguntasBasicasConfirmacaoPositiva[3]
                    },
                    new UsuarioServico.Pergunta() {
                        Codigo = 20, 
                        Descricao = PerguntasBasicasConfirmacaoPositiva[20]
                    }
                };

                using (var entidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    log.GravarMensagem("Chamando serviço de confirmação positiva");

                    response = entidade.Cliente.ValidarConfirmacaoPositivaPrimeiroAcesso(
                        out entidadesPossuemUsuario,
                        out entidadesPossuemMaster,
                        emailUsuario,
                        new EntidadeServico.ConfirmacaoPositivaPrimeiroAcessoRequest()
                        {
                            Agencia = agencia,
                            Banco = banco,
                            ContaCorrente = contaCorrente,
                            EntidadesPNSelecionadas = estabelecimentos,
                            CpfProprietario = cpfProprietario,
                            CnpjEstabelecimento = cnpjEstabelecimento,
                            CpfCnpjSocio = cpfCnpjSocio
                        });

                    log.GravarMensagem("Retorno confirmação positiva", new
                    {
                        codigoRetorno = response == null ? 0 : response.CodigoRetorno
                    });
                }

                if (response == null)
                {
                    msgRetorno = new QuadroAvisoModel(TipoModal.Error, "Atenção", "Não foi possível validar os dados informados");
                    return false;
                }

                // código 999 - entidades bloqueadas por tentativas foi esgotada
                if (response.CodigoRetorno == 999)
                {
                    EnviarEmailAcessoBloqueado(entidadesPossuemUsuario, entidadesPossuemMaster);

                    RegistrarBloqueioConfirmacaoPositiva(estabelecimentos, emailUsuario);
                    msgRetorno = new QuadroAvisoModel(TipoModal.Error, "Criação de usuário bloqueada", @"
<span class='gtm-code-trigger-bloqueio-pv'>A quantidade de tentativas esgotou e por isso a criação de usuário foi bloqueada</span>.<br /><br />
Solicite o desbloqueio para um usuário master ou entre em contato com a central de atendimento<br /><br />
<b>4001-4433 (capitais e regiões metropolitanas)</b><br />
<b>0800 728 4433 (demais localidades)</b>");
                    return false;
                }

                if (response.CodigoRetorno > 0 || !response.Retorno)
                {
                    log.GravarMensagem(string.Format("Pvs possuem {0} tentativas para confirmação positiva", response.TentativasRestantes));

                    RegistrarHistoricoConfirmacaoPositiva(perguntasIncorretas, estabelecimentos);

                    msgRetorno = new QuadroAvisoModel(TipoModal.Error, "Dados inválidos", String.Format(@"
Você ainda possui <span class='bold gtm-code-trigger-erro-conf-pos' style='display: inline;'>{0}</span> tentativas.<br /><br />
Caso seu estabelecimento já possua usuário master, solicite que ele faça a criação do seu usuário.", response.TentativasRestantes));
                    return false;
                }

                return response.Retorno;
            }
        }

        /// <summary>
        /// Descrições dos grupos de informações validadas em cada pergunta básica.
        /// </summary>
        private static Dictionary<Int32, String> PerguntasBasicasConfirmacaoPositiva
        {
            get
            {
                return new Dictionary<Int32, String>() {
                    { 3, "CNPJ ou CPF de um dos sócios" },
                    { 20, "domicílio bancário de crédito ou débito" }
                };
            }
        }

        /// <summary>
        /// Caso a entidade tenha sido bloqueada envia e-mail relatando o caso.
        /// </summary>
        /// <param name="entidadesPossuemUsuario">Entidades que possuem usuários mas não possuem usuário master</param>
        /// <param name="entidadesPossuemMaster">Entidades que possuem usuários master</param>
        private static void EnviarEmailAcessoBloqueado(
            EntidadeServico.EntidadeServicoModel[] entidadesPossuemUsuario,
            EntidadeServico.EntidadeServicoModel[] entidadesPossuemMaster)
        {
            int codigoRetorno;
            EntidadeServico.Usuario[] usuarios;

            foreach (var item in entidadesPossuemUsuario)
            {
                EmailNovoAcesso.EnviarEmailSolicitacoesAcessoBloqueada(item.EmailEstabelecimento,
                                                                       item.Email,
                                                                       item.NumeroPV);
            }

            foreach (var item in entidadesPossuemMaster)
            {
                string emailsMaster = null;

                using (var entidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    usuarios = entidade.Cliente.ConsultarUsuariosPorPerfil(out codigoRetorno, item.NumeroPV, 1, 'M');
                }

                if (usuarios != null && usuarios.Any(x => !string.IsNullOrEmpty(x.Email)))
                {
                    emailsMaster = string.Join(",", usuarios
                        .Where(x => !string.IsNullOrEmpty(x.Email))
                        .Select(usuario => usuario.Email));
                }

                using (Logger log = Logger.IniciarLog("Camada Negócio : Validar os dados do Usuário para continuar para o passo de Confirmação Positiva"))
                {
                    log.GravarMensagem("Enviar Email Solicitações Acesso Bloqueada", new { emailsMaster });
                }

                if (!string.IsNullOrEmpty(emailsMaster))
                {
                    EmailNovoAcesso.EnviarEmailSolicitacoesAcessoBloqueada(
                        emailsMaster,
                        usuarios.FirstOrDefault(x => !string.IsNullOrEmpty(x.Email)).Email,
                        item.NumeroPV);
                }
            }
        }

        /// <summary>
        /// Grava histórico 
        /// </summary>
        private static void RegistrarBloqueioConfirmacaoPositiva(EntidadeServico.EntidadeServicoModel[] entidades, String emailUsuario)
        {
            foreach (var entidade in entidades)
                Historico.BloqueioFormularioSolicitacaoAcesso(0, null, emailUsuario, null, entidade.NumeroPV);
        }

        /// <summary>
        /// Registra no Histórico o erro na confirmação positiva
        /// </summary>
        /// <param name="perguntasIncorretas">Lista contendo as perguntas respondidas incorretamente</param>
        private static void RegistrarHistoricoConfirmacaoPositiva(UsuarioServico.Pergunta[] perguntasIncorretas, EntidadeServico.EntidadeServicoModel[] estabelecimentos)
        {
            if (perguntasIncorretas.Length > 0)
            {
                // obtém a descrição das perguntas básicas que forma respondidas incorretamente
                var basicasIncorretas = perguntasIncorretas
                    .Where(p => PerguntasBasicasConfirmacaoPositiva.ContainsKey(p.Codigo))
                    .Select(p => PerguntasBasicasConfirmacaoPositiva[p.Codigo]).ToList();

                //Cria coleção única com todas as descrições das perguntas respondidas incorretamente
                var dadosIncorretos = new List<String>();
                if (basicasIncorretas.Count > 0)
                    dadosIncorretos.AddRange(basicasIncorretas);

                // armazena no histórico
                if (InformacaoUsuario.Existe())
                {
                    InformacaoUsuario usuario = InformacaoUsuario.Recuperar();

                    foreach (var numeroPv in usuario.PvsSelecionados)
                    {
                        Historico.ErroConfirmacaoPositivaPrimeiroAcesso(usuario.IdUsuario,
                                                            usuario.NomeCompleto,
                                                            usuario.EmailUsuario,
                                                            usuario.TipoUsuario,
                                                            numeroPv,
                                                            "Criação de usuário e senha",
                                                            dadosIncorretos.Distinct().ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Traduz os parâmetros da QueryStringSegura para o modelo de usuário
        /// </summary>
        /// <param name="q">Instância da QueryStringSegura a ser lida</param>
        /// <returns>Model com os dados do usuário</returns>
        public static UsuarioCriacaoModel ConvertFromQueryStringSegura(QueryStringSegura q)
        {
            if (q == null)
                return null;

            UsuarioCriacaoModel model = new UsuarioCriacaoModel();
            foreach (var item in model.GetType().GetProperties())
            {
                // verifica se o item é "settable"
                if (item.GetSetMethod() == null || !item.GetSetMethod().IsPublic)
                    continue;

                switch (Type.GetTypeCode(item.PropertyType))
                {
                    case TypeCode.Boolean:
                        Boolean value = false;
                        Boolean.TryParse(q[item.Name], out value);
                        item.SetValue(model, value);
                        break;
                    case TypeCode.Int32:
                        item.SetValue(model, (Int32)q[item.Name].ToInt32Null(0));
                        break;
                    case TypeCode.Int64:
                        item.SetValue(model, (Int64)q[item.Name].ToInt64Null(0));
                        break;
                    case TypeCode.String:
                        item.SetValue(model, Convert.ToString(q[item.Name]));
                        break;
                    default:
                        if (String.Compare(item.Name, "PvsSelecionados", false) == 0)
                        {
                            Int32[] pvs = new Int32[0];
                            if (!String.IsNullOrEmpty(q["PvsSelecionados"]))
                            {
                                pvs = q["PvsSelecionados"].Split(';').Select(x =>
                                {
                                    x = x.Trim();
                                    Int32 val = 0;
                                    Int32.TryParse(x, out val);
                                    return val;
                                }).ToArray();
                            }
                            item.SetValue(model, pvs);
                        }
                        else if (String.Compare(item.Name, "HashEmail", false) == 0)
                        {
                            Guid val = Guid.Empty;
                            Guid.TryParse(q["HashEmail"], out val);
                            item.SetValue(model, val);
                        }
                        break;
                }
            }

            return model;
        }

        /// <summary>
        /// Converte uma instância do modelo de criação de usuário em uma QueryString segura
        /// </summary>
        /// <param name="model">Usuário modelo</param>
        /// <returns>QueryString segura</returns>
        public static QueryStringSegura ConvertToQueryStringSegura(UsuarioCriacaoModel model)
        {
            if (model == null)
                return null;

            QueryStringSegura q = new QueryStringSegura();
            foreach (var item in model.GetType().GetProperties())
            {
                // trata apenas atributos que sejam "gettable"
                if (item.GetGetMethod() == null || !item.GetGetMethod().IsPublic)
                    continue;

                if (item.GetValue(model) == null || item.GetValue(model) == DBNull.Value)
                    continue;

                if (String.Compare(item.Name, "PvsSelecionados", false) == 0)
                {
                    q[item.Name] = String.Join(";", model.PvsSelecionados);
                    continue;
                }

                q[item.Name] = Convert.ToString(item.GetValue(model));
            }
            return q;
        }

        #endregion

        #region [ Listas Sharepoint ]

        /// <summary>
        /// Domínios bloqueados/não permitidos para cadastro de usuários
        /// </summary>
        public static List<String> DominiosBloqueados
        {
            get
            {
                using (Logger log = Logger.IniciarLog("Obtendo lista de domínios bloqueados"))
                {
                    try
                    {
                        SPList lista = null;

                        // SPUtility.ValidateFormDigest();

                        // recupera a lista de "Domínios Bloqueados" em sites/fechado/minhaconta
                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            using (SPSite spSite = SPContext.Current.Site.WebApplication.Sites["sites/fechado"])
                            using (SPWeb spWeb = spSite.AllWebs["minhaconta"])
                            {
                                // spWeb.AllowUnsafeUpdates = true;
                                lista = spWeb.Lists.TryGetList("Domínios Bloqueados");
                            }
                        });

                        if (lista == null)
                            return new List<String>();

                        // query para recuperar apenas os registros ativos
                        var query = new SPQuery();
                        query.Query = String.Concat(
                            "<Where>",
                                "<Eq>",
                                    "<FieldRef Name=\"Ativo\" />",
                                    "<Value Type=\"Boolean\">1</Value>",
                                "</Eq>",
                            "</Where>");

                        // preparação do objeto de retorno contendo a lista de domínios bloqueados
                        return lista.GetItems(query)
                            .Cast<SPListItem>()
                            .Select(spItem => Convert.ToString(spItem["Dominio"])).ToList();
                    }
                    catch (SPException ex)
                    {
                        log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);
                        return new List<String>();
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        SharePointUlsLog.LogErro(ex);
                        return new List<String>();
                    }
                }
            }
        }

        #endregion
    }
}
