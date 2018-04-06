using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.DadosCadastrais.SharePoint.WPAberto.DadosUsuario
{
    /// <summary>
    /// Webpart com o 2º Passo de criação de usuário
    /// </summary>
    public partial class DadosUsuarioUserControl : UserControlBase
    {
        
        /// <summary>
        /// Indica se o usuario veio do Mobile ou nao
        /// </summary>
        protected Boolean OrigemMobile
        {
            get
            {
                if (Session["criacaoacessoorigemmobile"] != null)
                {
                    return Convert.ToBoolean(Session["criacaoacessoorigemmobile"]);
                }
                return false;
            }
            set
            {
                Session["criacaoacessoorigemmobile"] = value;
            }
        }

        #region [Controles da Página]

        /// <summary>
        /// qdAviso control.
        /// </summary>
        protected QuadroAviso QdAviso { get { return (QuadroAviso)qdAviso; } }

        /// <summary>
        /// qdAvisoConclusao control.
        /// </summary>
        protected QuadroAviso QdAvisoConclusao { get { return (QuadroAviso)qdAvisoConclusao; } }

        /// <summary>
        /// txtNomeCompleto control.
        /// </summary>
        protected CampoNovoAcesso TxtNomeCompleto { get { return (CampoNovoAcesso)txtNomeCompleto; } }

        /// <summary>
        /// txtSenha control.
        /// </summary>
        protected CampoNovoAcesso TxtSenha { get { return (CampoNovoAcesso)txtSenha; } }

        /// <summary>
        /// txtCelular control.
        /// </summary>
        protected CampoNovoAcesso TxtCelular { get { return (CampoNovoAcesso)txtCelular; } }

        /// <summary>
        /// Banco para confirmação positiva
        /// </summary>
        protected CampoNovoAcesso DdlBanco { get { return (CampoNovoAcesso)ddlBanco; } }

        /// <summary>
        /// Agência para confirmação positiva
        /// </summary>
        protected CampoNovoAcesso TxtAgencia { get { return (CampoNovoAcesso)txtAgencia; } }

        /// <summary>
        /// Conta corrente para confirmação positiva
        /// </summary>
        protected CampoNovoAcesso TxtContaCorrente { get { return (CampoNovoAcesso)txtContaCorrente; } }

        /// <summary>
        /// CPF/CNPJ do sócio para confirmação positiva
        /// </summary>
        protected CampoNovoAcesso TxtCpfCnpjSocio { get { return (CampoNovoAcesso)txtCpfCnpjSocio; } }

        /// <summary>
        /// CPF do usuário, para cadastro de PF
        /// </summary>
        protected CampoNovoAcesso TxtCpfUsuario { get { return (CampoNovoAcesso)txtCpfUsuario; } }

        #endregion

        #region [Eventos da página]

        /// <summary>
        /// Inicialização da WebPart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                using (Logger log = Logger.IniciarLog("Inicialização da WebPart de 2º Passo"))
                {
                    try
                    {
                        if (!InformacaoUsuario.Existe())
                        {
                            log.GravarMensagem("Não existem dados no Cache");

                            this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", String.Empty, base.RecuperarEnderecoPortal());

                            return;
                        }
                    }
                    catch (FaultException<EntidadeServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        
                        return;
                    }
                    catch (HttpException ex)
                    {
                        log.GravarErro(ex);
                        
                        return;
                    }
                    catch (PortalRedecardException ex)
                    {
                        log.GravarErro(ex);
                        return;
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        return;
                    }

                    var pvsSelecionados = this.GetPVsCache();

                    // apresenta erro ao usuário caso nenhum PV seja identificado
                    if (!(pvsSelecionados != null && pvsSelecionados.Length > 0))
                    {
                        log.GravarMensagem("Nenhum estabelecimento selecionado");

                        this.ExibirErro(0, "Nenhum estabelecimento selecionado", "Atenção:");
                        return;
                    }

                    if (pvsSelecionados.Length == 1)
                    {
                        log.GravarMensagem("Apenas um Pv selecionado", new { pvsSelecionados = pvsSelecionados.Length });

                        // traz o Nome Completo preenchido com a razão social do único PV selecionado
                        var pv = pvsSelecionados.FirstOrDefault();
                        if (!pv.PossuiUsuario.GetValueOrDefault(false))
                        {
                            TxtNomeCompleto.Text = pv.RazaoSocial;
                            TxtNomeCompleto.TextBox.Attributes.Add("exibir-link-editar", "true");
                        }
                    }

                    // por default, oculta o campo CPF/CNPJ do sócio
                    this.trCpfCnpjSocio.Visible = false;

                    // por default, oculta o campo CPF do usuário
                    this.trCpfUsuario.Visible = false;

                    InformacaoUsuario infoUsuario = InformacaoUsuario.Recuperar();

                    if (infoUsuario.Empresa)
                    {
                        // Consulta se CPF informado pelo usuário no primeiro passo corresponde a um sócio vinculado ao(s) PV(s) selecionado(s)
                        log.GravarMensagem("Consulta se CPF informado pelo usuário no primeiro passo corresponde a um sócio vinculado ao(s) PV(s) selecionado(s)");
                        using (var ctxUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            // consulta o serviço para verificar o CPF do usuário
                            if (ctxUsuario.Cliente.ValidarCpfCnpjSocio(pvsSelecionados.FirstOrDefault().NumeroPV, infoUsuario.CpfUsuario))
                            {
                                log.GravarMensagem("CPF informado é do sócio", new { CpfSocio = infoUsuario.CpfUsuario });

                                // grava CPF do sócio para posterior confirmação positiva
                                infoUsuario.CpfCnpjSocio = infoUsuario.CpfUsuario;
                                InformacaoUsuario.Salvar();

                                // oculta o campo CPF/CNPJ do sócio, pois não há mais a necessidade de preenchimento manual
                                this.trCpfCnpjSocio.Visible = false;
                            }
                            else
                            {
                                log.GravarMensagem("CPF informado não é do sócio", new { CpfSocio = infoUsuario.CpfUsuario });

                                // exibe o campo CPF/CNPJ do sócio para preenchimento manual
                                this.trCpfCnpjSocio.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        log.GravarMensagem("Usuário PF não será verificado o CPF sócio");

                        this.trCpfUsuario.Visible = true;

                        var pv = pvsSelecionados.FirstOrDefault();

                        // se for o primeiro usuário do PV, carrega o CPF do proprietário
                        if (!pv.PossuiUsuario.GetValueOrDefault(false) && infoUsuario.CpfProprietario.HasValue)
                        {
                            (TxtCpfUsuario.Campo as CampoCpf).Cpf = infoUsuario.CpfProprietario.Value;
                            TxtCpfUsuario.TextBox.Attributes.Add("exibir-link-editar", "true");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Valida a página para continuar para o passo de Confirmação Positiva
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(Object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Validar os dados do Usuário para continuar para o passo de Confirmação Positiva"))
            {
                try
                {
                    if (InformacaoUsuario.Existe())
                    {
                        log.GravarMensagem("Dados existentes no cache");

                        if (this.ValidarDados() && ValidarConfirmacaoPositiva())
                        {
                            log.GravarMensagem("Proceguindo para o proximo passo");

                            InformacaoUsuario info = InformacaoUsuario.Recuperar();

                            info.EmailSecundario = String.Empty;
                            info.DddCelularUsuario = (TxtCelular.Campo as CampoCelular).DDD.ToEmptyString().ToInt32Null(0).Value;
                            info.CelularUsuario = (TxtCelular.Campo as CampoCelular).Numero.ToEmptyString().ToInt32Null(0).Value;
                            info.NomeCompleto = (TxtNomeCompleto.Campo as CampoTexto).Texto;
                            info.SenhaUsuario = (TxtSenha.Campo as CampoSenha).SenhaCriptografada;

                            // obtém o "CPF do Usuário" se for PF
                            if (!info.Empresa)
                                info.CpfUsuario = (TxtCpfUsuario.Campo as CampoCpf).Cpf.GetValueOrDefault(0);
                            
                            InformacaoUsuario.Salvar(info);
                            this.ConcluirProcesso();
                        }
                    }
                    else
                    {
                        log.GravarMensagem("Dados inexistentes no cache");

                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", String.Empty, base.RecuperarEnderecoPortal());

                        return;
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, String.Empty);

                    return;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, String.Empty);

                    return;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, String.Empty);

                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, String.Empty);

                    return;
                }
            }
        }


        /// <summary>
        /// Retornar para a visualização dos dados do Usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Retornar para a visualização dos dados do Usuário"))
            {
                try
                {
                    pnlAviso.Visible = false;
                    pnlDadosIniciais.Visible = true;
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, String.Empty);

                    return;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, String.Empty);

                    return;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, String.Empty);

                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, String.Empty);

                    return;
                }
            }
        }

        #endregion

        #region [Métodos auxiliares]

        /// <summary>
        /// Valida se os dados preenchidos estão corretos
        /// </summary>
        /// <returns>
        /// <para>True - Dados válidos</para>
        /// <para>False - Dados inválidos</para>
        /// </returns>
        private Boolean ValidarDados()
        {
            Boolean valida = true;

            if (!TxtCelular.Validar(true))
            {
                valida = false;
            }

            if (!TxtNomeCompleto.Validar(true))
            {
                valida = false;
            }

            if (!TxtSenha.Validar(true))
            {
                valida = false;
            }

            if (!TxtCelular.Validar(true))
            {
                valida = false;
            }

            if (!DdlBanco.Validar(true))
            {
                valida = false;
            }

            if (!TxtAgencia.Validar(true))
            {
                valida = false;
            }

            if (!TxtContaCorrente.Validar(true))
            {
                valida = false;
            }

            if (trCpfCnpjSocio.Visible && !TxtCpfCnpjSocio.Validar(true))
            {
                valida = false;
            }

            return valida;
        }

        #region Novo processo passo 2

        /// <summary>
        /// Valida os dados bancários e bloqueia as entidades caso não 
        /// </summary>
        /// <returns></returns>
        public bool ValidarConfirmacaoPositiva()
        {
            ConfirmacaoPositivaPrimeiroAcessoResponse response = new ConfirmacaoPositivaPrimeiroAcessoResponse();
            string banco = (this.DdlBanco.Campo as CampoBanco).Value;
            string agencia = this.TxtAgencia.Text;
            string contaCorrente = TxtContaCorrente.Text;
            EntidadeServico.EntidadeServicoModel[] estabelecimentos = GetPVsCache();
            EntidadeServicoModel[] entidadesPossuemUsuario;
            EntidadeServicoModel[] entidadesPossuemMaster;

            long? cpf = GetCpfProprietario();
            long? cnpj = GetCnpj();

            using (Logger log = Logger.IniciarLog("Confirmação positiva"))
            {
                UsuarioServico.Pergunta[] perguntasIncorretas = new UsuarioServico.Pergunta[] { 
                    new UsuarioServico.Pergunta(){
                    Codigo = 3, 
                    Descricao = descricaoPerguntasBasicas[3]},
                    new UsuarioServico.Pergunta(){
                    Codigo = 20, 
                    Descricao = descricaoPerguntasBasicas[20]}
                };

                
                if (cpf == null && cnpj == null && (estabelecimentos == null || !estabelecimentos.Any()))
                {
                    this.ExibirErro(0, "Não foi possível validar os dados informados", "Atenção:");
                    return false;
                }

                using (var entidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    log.GravarMensagem("Chamando serviço de confirmação positiva");

                    response = entidade.Cliente.ValidarConfirmacaoPositivaPrimeiroAcesso(
                        out entidadesPossuemUsuario,
                        out entidadesPossuemMaster,
                        GetEmailUsuario(),
                        new ConfirmacaoPositivaPrimeiroAcessoRequest()
                        {
                            Agencia = agencia,
                            Banco = banco,
                            ContaCorrente = contaCorrente,
                            EntidadesPNSelecionadas = estabelecimentos,
                            CpfProprietario = cpf,
                            CnpjEstabelecimento = cnpj,
                            CpfCnpjSocio = this.trCpfCnpjSocio.Visible ? GetCpfCnpjSocio() : null
                        });

                    log.GravarMensagem("Retorno confirmação positiva", new { codigoRetorno = response == null ? 0 : response.CodigoRetorno });
                }

                if (response == null)
                {
                    log.GravarMensagem("Problemas para realizar a validação da confirmação positiva");
                    this.ExibirErro(0, "Atenção:", "Não foi possível validar os dados informados");
                    return false;
                }

                //999 Entidades bloqueadas por tentativas foi esgotada
                if (response != null && response.CodigoRetorno == 999)
                {
                    EnviarEmailAcessoBloqueado(entidadesPossuemUsuario, entidadesPossuemMaster);

                    GravarHistoricoBloqueioFormularioSolicitacaoAcesso();
                    this.ExibirErro(
                        1106, @"Solicite o desbloqueio para o usuário Master ou com
                            a nossa Central de Atendimento através dos telefones: 
                            <br /><b>4001 4433 (Capitais e Regiões Metropolitanas)</b>
                            <br /><b>0800 728 4433 (Demais Regiões)</b>",
                        "Atenção: A quantidade de tentativas esgotou e a criação de usuário foi bloqueada.",
                        urlVoltar: string.Format("{0}/Paginas/CriacaoUsrDadosIniciais.aspx", base.web.ServerRelativeUrl));
                    return false;
                }

                if (response != null && (response.CodigoRetorno > 0 || !response.Retorno))
                {
                    log.GravarMensagem(string.Format("Pvs possuem {0} tentativas para confirmação positiva", response.TentativasRestantes));
                    
                    RegistrarHistorico(perguntasIncorretas, estabelecimentos);
                    
                    string msg = @"
Você ainda possui <b>{0}</b> tentativas.<br /><br />
Caso seu estabelecimento já possua usuário Master, 
você pode solicitar que ele faça a criação do seu usuário.";
                    this.ExibirErro(1106, string.Format(msg, response.TentativasRestantes), "Atenção: os dados informados estão incorretos.");
                    return false;
                }
                
                return response.Retorno;
            }
        }


        /// <summary>
        /// Descrições dos grupos de informações validadas em cada pergunta básica.
        /// Utilizado para log.
        /// </summary>
        private static Dictionary<Int32, String> descricaoPerguntasBasicas = new Dictionary<Int32, String>
        {
            { 3, "CNPJ ou CPF de um dos sócios" },
            { 20, "domicílio bancário de crédito ou débito" }
        };

        /// <summary>
        /// Registra no Histórico o erro na confirmação positiva
        /// </summary>
        /// <param name="perguntasIncorretas">Lista contendo as perguntas respondidas incorretamente</param>
        private void RegistrarHistorico(UsuarioServico.Pergunta[] perguntasIncorretas, EntidadeServicoModel[] estabelecimentos)
        {
            if (perguntasIncorretas.Length > 0)
            {
                //Obtém a descrição das perguntas básicas que forma respondidas incorretamente
                var basicasIncorretas = perguntasIncorretas
                    .Where(p => descricaoPerguntasBasicas.ContainsKey(p.Codigo))
                    .Select(p => descricaoPerguntasBasicas[p.Codigo]).ToList();

                //Cria coleção única com todas as descrições das perguntas respondidas incorretamente
                var dadosIncorretos = new List<String>();
                if (basicasIncorretas.Count > 0)
                    dadosIncorretos.AddRange(basicasIncorretas);

                //Armazena no histórico
                if (InformacaoUsuario.Existe())
                {
                    InformacaoUsuario usuario = InformacaoUsuario.Recuperar();

                    foreach (var numeroPv in usuario.PvsSelecionados)
                    {
                        Historico.ErroConfirmacaoPositivaPrimeiroAcesso(  usuario.IdUsuario, 
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
        /// Caso a entidade tenha sido bloqueada envia e-mail relatando o caso.
        /// </summary>
        /// <param name="entidadesPossuemUsuario">Entidades que possuem usuários mas não possuem usuário master</param>
        /// <param name="entidadesPossuemMaster">Entidades que possuem usuários master</param>
        private void EnviarEmailAcessoBloqueado(EntidadeServicoModel[] entidadesPossuemUsuario, EntidadeServicoModel[] entidadesPossuemMaster)
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
        private void GravarHistoricoBloqueioFormularioSolicitacaoAcesso()
        {
            EntidadeServicoModel[] entidades = GetPVsCache();
            string emailUsuario = GetEmailUsuario();

            foreach (var entidade in entidades)
            {
                Historico.BloqueioFormularioSolicitacaoAcesso(0, null, emailUsuario, null, entidade.NumeroPV);
            }
        }

        /// <summary>
        /// Retorna o emial do usuário informado no passo anterior
        /// </summary>
        /// <returns></returns>
        private string GetEmailUsuario()
        {
            InformacaoUsuario cache;

            if (InformacaoUsuario.Existe())
            {
                cache = InformacaoUsuario.Recuperar();

                if (string.IsNullOrEmpty(cache.EmailUsuario))
                {
                    return null;
                }

                return cache.EmailUsuario.ToLower();
            }

            return null;
        }

        /// <summary>
        /// Retorna o Cnpj gravado na sessão
        /// </summary>
        /// <returns></returns>
        private long? GetCnpj()
        {
            InformacaoUsuario cache;

            if (InformacaoUsuario.Existe())
            {
                cache = InformacaoUsuario.Recuperar();

                if (cache.CnpjEstabelecimento == null || cache.CnpjEstabelecimento == 0)
                {
                    return null;
                }

                return cache.CnpjEstabelecimento;
            }

            return null;

        }

        /// <summary>
        /// Retorna o Cpf gravado na sessão
        /// </summary>
        /// <returns></returns>
        private long? GetCpfProprietario()
        {
            InformacaoUsuario cache;

            if (InformacaoUsuario.Existe())
            {
                cache = InformacaoUsuario.Recuperar();

                if (cache.CpfProprietario == null || cache.CpfProprietario == 0)
                {
                    return null;
                }

                return cache.CpfProprietario;
            }

            return null;

        }

        private long? GetCpfUsuario()
        {
            InformacaoUsuario cache;

            if (InformacaoUsuario.Existe())
            {
                cache = InformacaoUsuario.Recuperar();

                if (cache.CpfUsuario == null || cache.CpfUsuario == 0)
                {
                    return null;
                }

                return cache.CpfUsuario;
            }

            return null;
        }

        /// <summary>
        /// Retorna o CPF/CNPJ do sócio gravado na sessão
        /// </summary>
        /// <returns></returns>
        private long? GetCpfCnpjSocio()
        {
            return (this.TxtCpfCnpjSocio.Campo as CampoCpfCnpj).ValorCpfCnpj;
        }

        /// <summary>
        /// Pega os Pvs relacionados ao CPF/CNPJ selecionados no passo anterior
        /// </summary>
        /// <returns></returns>
        private EntidadeServico.EntidadeServicoModel[] GetPVsCache()
        {
            EntidadeServico.EntidadeServicoModel[] result = new EntidadeServico.EntidadeServicoModel[0];
            InformacaoUsuario cache;

            if (InformacaoUsuario.Existe())
            {
                cache = InformacaoUsuario.Recuperar();
                result = cache.EstabelecimentosRelacinados;
            }
            else
            {
                this.ExibirErro(0, "Não foi possível identificar os estabelecimentos relacionados aos dados informados", "Atenção:");
                return result;
            }

            return Filtro(result);
        }

        /// <summary>
        /// Filta as entidades relacionadas para retornar apenas as selecionadas no passo anterior
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private EntidadeServicoModel[] Filtro(EntidadeServicoModel[] result)
        {
            InformacaoUsuario cache;

            if (InformacaoUsuario.Existe())
            {
                cache = InformacaoUsuario.Recuperar();

                if (result != null && result.Any())
                {
                    return result.Where(x => cache.PvsSelecionados.Contains(x.NumeroPV)).ToArray();
                }
            }

            return result;
        }

        /// <summary>
        /// Método responsável por chamar o serviço para comcluir a criação de usuário
        /// </summary>
        private void ConcluirProcesso()
        {
            UsuarioServico.EntidadeServicoModel[] entidade = ConvertToUsuarioServicoEntidadeServicoModel(GetPVsCache());
            UsuarioServico.EntidadeServicoModel[] entidadesPossuemUsuMaster;
            UsuarioServico.EntidadeServicoModel[] entidadesNPossuemUsuMaster;
            Guid hash;
            int celular = GetCelular(TxtCelular.Text);
            int dDDCelular = GetDDDCelular(TxtCelular.Text);
            string nomeCompleto = TxtNomeCompleto.Text;
            string senhaCriptografada = (TxtSenha.Campo as CampoSenha).SenhaCriptografada;
            
            for (int i = 0; i < entidade.Count(); i++)
            {
                entidade[i].Email = GetEmailUsuario();
                entidade[i].DDDCelular = dDDCelular;
                entidade[i].Celular = celular;
                entidade[i].NomeCompletoUsuario = nomeCompleto;
                entidade[i].SenhaCriptografada = senhaCriptografada;
                entidade[i].CpfUsuario = GetCpfUsuario().GetValueOrDefault(0);
            }

            using (Logger log = Logger.IniciarLog("Consulção processo de criação do usuário"))
            {
                try
                {
                    int codigoRetorno = 0;
                    String mensagem = string.Empty;

                    log.GravarMensagem("Chamando servico CriarUsuarioVariosPvs");
                    
                    using (var ctxUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    {
                        ctxUsuario.Cliente.CriarUsuarioVariosPvs(out entidadesPossuemUsuMaster, 
                                                                 out entidadesNPossuemUsuMaster, 
                                                                 out hash, 
                                                                 out codigoRetorno, 
                                                                 out mensagem, 
                                                                 entidade, 
                                                                 (double)0.5);
                    }

                    if (codigoRetorno == 0)
                    {
                        var info = InformacaoUsuario.Recuperar();

                        info.EntidadePossuiMaster = (entidade.Count().Equals(1) && entidadesPossuemUsuMaster.Count() > 0);

                        InformacaoUsuario.Salvar(info);

                        EnviarEmailAprovacao(entidadesPossuemUsuMaster, entidadesNPossuemUsuMaster, "B", hash);

                        this.EnviarEmailNotificacaoCriacaoUsuario();

                        List<int> pvs = entidade.Select(p => p.NumeroPV).ToList();
                        
                        pvs.ForEach(p =>
                        {
                            Historico.CriacaoUsuario(
                                info.IdUsuario, info.NomeCompleto, info.EmailUsuario, info.TipoUsuario ?? "B",
                                p, info.EntidadePossuiMaster, info.EntidadePossuiUsuario, this.OrigemMobile);
                        });

                        this.ExibirSucesso();

                        //Registra no log/histórico de atividades
                       

                        log.GravarMensagem("Usuários gravados com sucesso");
                    }
                    else
                    {
                        this.ExibirErro(codigoRetorno, mensagem, "Atenção", string.Empty);
                    }

                    return;
                }
                catch (System.ServiceModel.FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    return;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, String.Empty);

                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, String.Empty);

                    return;
                }
            }
        }

        /// <summary>
        /// Exibe uma mensagem de sucesso na página
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        private void ExibirSucesso()
        {
            pnlAviso.Visible = false;
            pnlDadosIniciais.Visible = false;

            var info = InformacaoUsuario.Recuperar();
            if (info.EntidadePossuiMaster)
            {
                string serverRelativeUr = base.web.ServerRelativeUrl;
                string url = String.Format(@"{0}/Paginas/CriacaoUsrConclusao.aspx", serverRelativeUr);
                using (Logger log = Logger.IniciarLog("Exibir msg sucesso"))
                {
                    try
                    {
                        Response.Redirect(url);
                    }
                    catch (HttpException ex)
                    {
                        log.GravarErro(ex);
                        
                        return;
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);

                        return;
                    }
                }
                HttpContext.Current.ApplicationInstance.CompleteRequest();
                InformacaoUsuario.Limpar();
            }
            else
            {
                pnlAvisoEmail.Visible = true;
                QdAvisoConclusao.CarregarMensagem();
            }
        }


        public int GetCelular(string celular)
        {
            string value = NormalizaString(celular);

            value = value.Substring(2, value.Length - 2);

            return Convert.ToInt32(value);

        }

        public int GetDDDCelular(string celular)
        {
            string value = NormalizaString(celular);

            value = value.Substring(0, 2);

            return Convert.ToInt32(value);
        }

        public string NormalizaString(string value)
        {
            return value.Replace("(", "")
                        .Replace(")", "")
                        .Replace("-", "")
                        .Replace(" ", "")
                        .Replace(".", "");
        }

        #endregion

        /// <summary>
        /// Exibe a mensagem de erro
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <param name="titulo">Título para o quadro de aviso</param>
        /// <param name="urlVoltar">Url de redirecionamento</param>
        /// <param name="mensagemAdicional"></param>
        private void ExibirErro(String fonte, Int32 codigo, String titulo, String mensagemAdicional, String urlVoltar)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);

            if (!String.IsNullOrEmpty(mensagemAdicional))
            {
                mensagem += String.Concat("<br>", mensagemAdicional);
            }

            if (String.IsNullOrEmpty(urlVoltar))
            {
                QdAviso.CarregarMensagem(titulo, mensagem, QuadroAviso.IconeMensagem.Erro);
                btnVoltar.Visible = true;
            }
            else
            {
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAviso.IconeMensagem.Erro);
                btnVoltar.Visible = false;
            }

            pnlAviso.Visible = true;
            pnlDadosIniciais.Visible = false;
        }

        /// <summary>
        /// Exibe uma mensagem de erro personalizada
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="mensagem"></param>
        /// <param name="titulo"></param>
        private void ExibirErro(Int32 codigo, String mensagem, String titulo, String urlVoltar = "")
        {
            if (!String.IsNullOrEmpty(mensagem))
            {
                mensagem = String.Concat(mensagem, String.Format(" ({0})", codigo.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(urlVoltar))
            {
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAviso.IconeMensagem.Erro);
                btnVoltar.Visible = false;
            }
            else
            {
                QdAviso.CarregarMensagem(titulo, mensagem, QuadroAviso.IconeMensagem.Erro);
                btnVoltar.Visible = true;
            }

            pnlAvisoEmail.Visible = false;
            pnlAviso.Visible = true;
            pnlDadosIniciais.Visible = false;
        }


        /// <summary>
        /// Recupera a quantidade de tentativas da Entidade para realizar a confirmação Positiva
        /// </summary>
        /// <param name="mensagem"></param>
        /// <returns></returns>
        private Int32 RecuperarQuantidadeTentivas(out String mensagem, int numeroPv)
        {
            Int32 qtdTentativas = 0;
            Int32 codigoRetornoIS = 0;
            Int32 codigoRetornoGE = 0;
            mensagem = String.Empty;

            if (InformacaoUsuario.Existe())
            {
                InformacaoUsuario info = InformacaoUsuario.Recuperar();
                using (var client = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    var entidades = client.Cliente.Consultar(out codigoRetornoIS, out codigoRetornoGE, numeroPv, info.GrupoEntidade);
                    if (codigoRetornoGE == 0 && codigoRetornoIS == 0 && entidades.Length > 0)
                    {
                        var entidade = entidades[0];
                        if (!entidade.StatusPN.Codigo.Equals(UsuarioServico.Status1.EntidadeBloqueadaConfirmacaoPositiva))
                        {
                            mensagem = String.Format(@"Você ainda possui <b>{0}</b> tentativas.<br /><br />
                                                       Caso seu estabelecimento já possua usuário Master, 
                                                       você pode solicitar que ele faça a criação do seu usuário.",
                                /*this.qtdMaximaTentativasValidacaoDados*/ 0 - entidade.QuantidadeConfirmacaoPositiva);
                        }
                        else
                        {
                            mensagem = "Atenção: A quantidade de confirmação positiva foi esgotada";
                        }

                        qtdTentativas = entidade.QuantidadeConfirmacaoPositiva;
                    }
                }
            }

            return qtdTentativas;
        }

        #endregion

        public UsuarioServico.EntidadeServicoModel ConvertToUsuarioServicoEntidadeServicoModel(EntidadeServico.EntidadeServicoModel entidate)
        {
            return new UsuarioServico.EntidadeServicoModel()
                {
                    Celular = entidate.Celular,
                    CpfUsuario = entidate.CpfUsuario,
                    DDDCelular = entidate.DDDCelular,
                    Email = entidate.Email,
                    EmailEstabelecimento = entidate.EmailEstabelecimento,
                    EmailSecundario = entidate.EmailSecundario,
                    ExtensionData = entidate.ExtensionData,
                    GrupoPV = entidate.GrupoPV,
                    IdUsuario = entidate.IdUsuario,
                    NomeCompletoUsuario = entidate.NomeCompletoUsuario,
                    NumeroPV = entidate.NumeroPV,
                    PossuiMaster = entidate.PossuiMaster,
                    PossuiSenhaTemporaria = entidate.PossuiSenhaTemporaria,
                    PossuiUsuario = entidate.PossuiUsuario,
                    RazaoSocial = entidate.RazaoSocial,
                    SenhaCriptografada = entidate.SenhaCriptografada,
                    Status = entidate.Status
                };
        }

        public UsuarioServico.EntidadeServicoModel[] ConvertToUsuarioServicoEntidadeServicoModel(EntidadeServico.EntidadeServicoModel[] entidate)
        {
            return entidate.Select(x => ConvertToUsuarioServicoEntidadeServicoModel(x)).ToArray();
        }

        public EntidadeServico.EntidadeServicoModel ConvertToEntidadeServicoEntidadeServicoModel(UsuarioServico.EntidadeServicoModel entidate)
        {
            return new EntidadeServico.EntidadeServicoModel()
            {
                Celular = entidate.Celular,
                CpfUsuario = entidate.CpfUsuario,
                DDDCelular = entidate.DDDCelular,
                Email = entidate.Email,
                EmailEstabelecimento = entidate.EmailEstabelecimento,
                EmailSecundario = entidate.EmailSecundario,
                ExtensionData = entidate.ExtensionData,
                GrupoPV = entidate.GrupoPV,
                IdUsuario = entidate.IdUsuario,
                NomeCompletoUsuario = entidate.NomeCompletoUsuario,
                NumeroPV = entidate.NumeroPV,
                PossuiMaster = entidate.PossuiMaster,
                PossuiSenhaTemporaria = entidate.PossuiSenhaTemporaria,
                PossuiUsuario = entidate.PossuiUsuario,
                RazaoSocial = entidate.RazaoSocial,
                SenhaCriptografada = entidate.SenhaCriptografada,
                Status = entidate.Status
            };
        }

        public EntidadeServico.EntidadeServicoModel[] ConvertToUsuarioServicoEntidadeServicoModel(UsuarioServico.EntidadeServicoModel[] entidate)
        {
            return entidate.Select(x => ConvertToEntidadeServicoEntidadeServicoModel(x)).ToArray();
        }


        /// <summary>
        /// Para as entidades que possuem usuários master envia email para todo os usuários masters
        /// Para as entidades que não possuem usuários master envia e-mail apenas para o usuário
        /// </summary>
        /// <param name="entidadesPossuemUsuMaster"></param>
        /// <param name="entidadesNPossuemUsuMaster"></param>
        private void EnviarEmailAprovacao(UsuarioServico.EntidadeServicoModel[] entidadesPossuemUsuMaster, UsuarioServico.EntidadeServicoModel[] entidadesNPossuemUsuMaster, string tipoUsuario, Guid pvsHashEmail)
        {
            EnviarEmailAprovacaoUsuMaster(entidadesPossuemUsuMaster, tipoUsuario);

            EnviarEmailAprovacaoUsuNaoMaster(entidadesNPossuemUsuMaster, tipoUsuario, pvsHashEmail);
        }

        /// <summary>
        /// Envia email para os usuarios que não possuem Pv com usuário master
        /// </summary>
        /// <param name="entidadesNPossuemUsuMaster"></param>
        /// <param name="tipoUsuario"></param>
        /// <param name="pvsHashEmail"></param>
        private void EnviarEmailAprovacaoUsuNaoMaster(UsuarioServico.EntidadeServicoModel[] entidadesNPossuemUsuMaster, string tipoUsuario, Guid pvsHashEmail)
        {
            int[] pvsSelecionados;
            UsuarioServico.EntidadeServicoModel entidade;

            if (entidadesNPossuemUsuMaster != null && entidadesNPossuemUsuMaster.Any())
            {
                using (Logger log = Logger.IniciarLog("Envio de emails para usuários masters"))
                {
                    log.GravarMensagem(string.Format("Quantidade total de emails que seram enviados = {0}", entidadesNPossuemUsuMaster.Count()));

                    pvsSelecionados = entidadesNPossuemUsuMaster.Select(x => x.NumeroPV).ToArray();
                    entidade = entidadesNPossuemUsuMaster.FirstOrDefault();
                    
                    EmailNovoAcesso.EnviarEmailConfirmacaoCadastro12h(entidade.Email,
                                                                        entidade.IdUsuario,
                                                                        pvsHashEmail,
                                                                        entidade.IdUsuario,
                                                                        entidade.Email,
                                                                        entidade.NomeCompletoUsuario,
                                                                        tipoUsuario,
                                                                        entidade.NumeroPV,
                                                                        null,
                                                                        true,
                                                                        GetCpfUsuario(), 
                                                                        pvsSelecionados);

                        log.GravarMensagem(string.Format("Enviado e-mail para {0}", entidade.Email));

                    
                }
            }
        }

        /// <summary>
        /// Envia email para os usuários masters solicitando a aprovação
        /// </summary>
        /// <param name="entidadesPossuemUsuMaster"></param>
        /// <param name="tipoUsuario"></param>
        private void EnviarEmailAprovacaoUsuMaster(UsuarioServico.EntidadeServicoModel[] entidadesPossuemUsuMaster, string tipoUsuario)
        {
            UsuarioServico.EntidadeServicoModel entidade;
            UsuarioServico.EntidadeServicoModel[] emailsUsuariosMasters;
            string masterEmails;
            string[] arrayMasterEmails;

            if (entidadesPossuemUsuMaster != null && entidadesPossuemUsuMaster.Any())
            {
                entidade = entidadesPossuemUsuMaster.FirstOrDefault();

                using (var ctxUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                {
                    //Pega os emials dos usuários master das entidades
                    emailsUsuariosMasters = ctxUsuario.Cliente.ConsultarEmailsUsuMaster(entidadesPossuemUsuMaster.Select(x => x.NumeroPV).ToArray(), entidade.CpfUsuario, entidade.Email);
                }

                if (emailsUsuariosMasters != null && emailsUsuariosMasters.Any())
                {
                    using (Logger log = Logger.IniciarLog("Envio de emails para usuários masters"))
                    {
                        foreach (var item in entidadesPossuemUsuMaster)
                        {
                            arrayMasterEmails = emailsUsuariosMasters.Where(x => x.NumeroPV == item.NumeroPV &&
                                                                                 !string.IsNullOrEmpty(x.Email))
                                                                     .Select(x => x.Email)
                                                                     .ToArray();

                            masterEmails = string.Join(",", arrayMasterEmails);

                            log.GravarMensagem("Emails de usuários masters", new { masterEmails });

                            if (!string.IsNullOrEmpty(masterEmails))
                            {
                                EmailNovoAcesso.EnviarEmailAprovacaoAcesso(masterEmails,
                                                                           item.NomeCompletoUsuario,
                                                                           item.Email,
                                                                           item.IdUsuario,
                                                                           tipoUsuario,
                                                                           item.NumeroPV,
                                                                           null);

                                log.GravarMensagem(string.Format("Enviado e-mail para {0}", masterEmails));
                            }

                            arrayMasterEmails = null;
                            masterEmails = null;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Envia e-mail de notificação após criação de usuário na área aberta
        /// </summary>
        /// <param name="pvs"></param>
        private void EnviarEmailNotificacaoCriacaoUsuario()
        {
            // consulta os e-mails do GE
            using (Logger log = Logger.IniciarLog("Método EnviarEmailNotificacaoCriacaoUsuario()"))
            {
                try
                {
                    using (var entidadeServico = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        // obtém PVs selecionados no passo 1
                        InformacaoUsuario infoUsuario = InformacaoUsuario.Recuperar();
                        var pvs = GetPVsCache().Where(x => infoUsuario.PvsSelecionados.Contains(x.NumeroPV));

                        int codigoRetorno = 0;
                        var dicEmailPvs = entidadeServico.Cliente.ConsultarEmailPVs(out codigoRetorno, infoUsuario.PvsSelecionados);

                        foreach (var pv in pvs)
                        {
                            string emailEstabelecimento = string.Empty;

                            if (!dicEmailPvs.TryGetValue(pv.NumeroPV, out emailEstabelecimento))
                            {
                                continue;
                            }

                            if (string.IsNullOrEmpty(emailEstabelecimento))
                            {
                                continue;
                            }

                            EmailNovoAcesso.EnviarEmailNotificacaoCriacaoUsuario(
                                emailEstabelecimento,
                                pv.IdUsuario,
                                pv.Email,
                                pv.NomeCompletoUsuario,
                                string.Empty,
                                pv.NumeroPV,
                                string.Empty,
                                infoUsuario.PvsSelecionados);
                        }
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    
                    return;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    
                    return;
                }
                catch (NullReferenceException ex)
                {
                    log.GravarLog(EventoLog.ChamadaServico, ex);

                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);

                    return;
                }
            }
        }

    }


}

