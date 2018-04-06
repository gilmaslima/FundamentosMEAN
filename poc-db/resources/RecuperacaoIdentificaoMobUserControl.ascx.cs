/*
© Copyright 2015 Rede S.A.
Autor : Yuri Lamonica Chuck
Empresa : Rede
*/
using System;
using System.Linq;
using System.ServiceModel;
using System.Web;
using Microsoft.SharePoint.Administration;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Rede.PN.DadosCadastraisMobile.SharePoint.CONTROLTEMPLATES.DadosCadastraisMobile;
using Rede.PN.DadosCadastraisMobile.SharePoint.Util;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Rede.PN.DadosCadastraisMobile.SharePoint.EntidadeServico;
using System.Web.Script.Serialization;
using System.Net;
using System.Web.UI;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.RecuperacaoIdentificaoMob
{
    public partial class RecuperacaoIdentificaoMobUserControl : UserControlBase, IPostBackEventHandler
    {

        public Boolean UsuarioOuSenhaValidado
        {
            get
            {
                if (ViewState["UsuarioOuSenhaValidado"] == null)
                    ViewState["UsuarioOuSenhaValidado"] = false;
                return (Boolean)ViewState["UsuarioOuSenhaValidado"];
            }
            set
            {
                ViewState["UsuarioOuSenhaValidado"] = value;
            }
        }

        #region [Atributos da WebPart]

        /// <summary>
        /// Passos da Webpart
        /// </summary>
        public RecuperacaoIdentificaoMob WebPartRecuperacao { get; set; }

        #endregion

        #region [Controles WebPart]

        /// <summary>
        /// qdAvisoConfirmacaoPositiva control.
        /// </summary>
        protected QuadroAvisosResponsivo QdAviso { get { return (QuadroAvisosResponsivo)qdAviso; } }

        #endregion

        #region [Eventos da WebPart]

        /// <summary>
        /// Incialização da WebPart de Identificação da Recuperação de Usuário/Senha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (this.WebPartRecuperacao.RecuperacaoUsuario)
                {
                    pnlRecUsuario.Visible = true;
                    pnlRecSenha.Visible = false;
                    lblMsgSelecaoEstabelecimento.Text = "Selecione o estabelecimento para o qual você deseja recuperar o usuário";
                }
                else
                {
                    pnlRecUsuario.Visible = false;
                    pnlRecSenha.Visible = true;
                    lblMsgSelecaoEstabelecimento.Text = "Selecione o estabelecimento para o qual você deseja recuperar a senha";
                }
            }

            ChangeFlagPvsFiltrados();

            using (Logger log = Logger.IniciarLog("Incialização da WebPart de Identificação da Recuperação de Usuário/Senha"))
            {
                try
                {
                    if (!Page.IsPostBack)
                        InformacaoUsuario.Limpar();

                    if (!this.WebPartRecuperacao.RecuperacaoUsuario)
                    {
                        lblTitulo.Text = "Senha";
                        pnlRecSenha.Visible = true;
                        pnlRecUsuario.Visible = false;
                        pnlBoxSenha.Visible = true;
                        pnlEmailSenha.Visible = true;
                        pnlCPFEmailUsuario.Visible = false;
                        hfRecuperacaoUsuario.Value = "false";
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), false);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), false);
                    return;
                }

            }
        }

        /// <summary>
        /// Continuar para o próximo Passo de Recuperação de Usuário/Senha
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Continuar para o próximo Passo de Recuperação de Usuário/Senha"))
            {
                try
                {
                    if (!IsVisibleListaPvs())
                    {
                        InformacaoUsuario.Limpar();
                    }

                    CarregarPvsRelacionadosAoUsu();

                    if (!ValidarStatusUsuario(GetEmail(), GetCPF()))
                    {
                        return;
                    }

                    if (!ValidarProximoPasso())
                    {
                        return;
                    }

                    if (this.WebPartRecuperacao.RecuperacaoUsuario)
                    {
                        this.ValidarRecuperacaoUsuario();
                    }
                    else
                    {
                        this.ValidarRecuperacaoSenha();
                    }
                }
                catch (FaultException<Redecard.PN.Comum.SharePoint.EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, this.RetornarHome(), true);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), true);
                    return;
                }
            }
        }

        /// <summary>
        /// Caso o relacionamento seja de um pra um o sistema verifica o status no usuário
        /// </summary>
        /// <returns></returns>
        private bool ValidarStatusUsuario(string email, long? cpf)
        {
            UsuarioServico.Usuario[] usuarios = null;
            int codigoRetorno;

            using (Logger log = Logger.IniciarLog("Validar status do usuário"))
            {
                try
                {
                    if (!string.IsNullOrEmpty(email))
                    {
                        using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            usuarios = contextoUsuario.Cliente.ConsultarPorEmailPrincipalPorStatus(out codigoRetorno, email, 0, 0, null);
                        }
                    }
                    else if (cpf.HasValue)
                    {
                        using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            usuarios = contextoUsuario.Cliente.ConsultarPorCpfPrincipalPorStatus(out codigoRetorno, cpf.Value, 0, 0, null);
                        }
                    }

                    if (usuarios != null && usuarios.Count() == 1)
                    {
                        var usuario = usuarios.FirstOrDefault();

                        switch (usuario.Status.Codigo)
                        {
                            case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioAtivo:
                            case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:
                            case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoAlteracaoEmail:
                            case (Int32)Redecard.PN.Comum.Enumerador.Status.RespostaIdPosPendente:
                                {
                                    return true;
                                }
                            case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                                {
                                    this.ExibirAviso("RecuperacaoIdentificacao.ValidarIdentificacao", 1158, "Atenção", base.RecuperarEnderecoPortal(), false);
                                    return false;
                                }
                            case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoMaster:
                                {
                                    string postBack = this.Page.ClientScript.GetPostBackEventReference(
                                        this, string.Concat("ReenviarSolicitacaoAprovacao|", WebUtility.HtmlEncode(new JavaScriptSerializer().Serialize(new
                                        {
                                            CodigoUsuario = usuario.CodigoIdUsuario,
                                            CodigoEntidade = usuario.Entidade.Codigo,
                                            CodigoGrupoEntidade = usuario.Entidade.GrupoEntidade.Codigo,
                                            EmailUsuario = usuario.Email,
                                            CpfUsuario = usuario.CPF
                                        }))));
                                    string msg = string.Format(
                                        "Usuário aguardando aprovação.<br /></br />" +
                                        "Por favor, entre em contato com o usuário Master " +
                                        "ou clique <a href=\"#\" onclick=\"{0}\">aqui</a> para reenviar o e-mail de aprovação. (1159)", postBack);
                                    this.ExibirAviso(msg, "Atenção", base.RecuperarEnderecoPortal(), false);
                                    return false;
                                }
                            case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioBloqueadoRecuperacaoSenha: //Cadastrar mensagem de bloqueio para recuperação
                            case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioBloqueadoRecuperacaoUsuario:
                                {
                                    this.ExibirErro("RecuperacaoIdentificacao.ValidarIdentificacao", 1160, "Atenção: a quantidade de tentativas foi esgotada.", base.RecuperarEnderecoPortal(), false);
                                    return false;
                                }
                            default:
                                {
                                    this.ExibirAviso("Você não tem permissão para recuperar este usuário.", "Atenção", base.RecuperarEnderecoPortal(), false);
                                    return false;
                                }
                        }
                    }

                    return true;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), false);
                    return false;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), false);
                    return false;
                }
            }
        }


        /// <summary>
        /// Verifica se usuário pode ir para o proximo passo
        /// </summary>
        /// <returns></returns>
        private bool ValidarProximoPasso()
        {
            EntidadeServico.EntidadeServicoModel[] arrayPvs = GetPVsSemPaginacao();

            //Caso o usuário não tenha peenxido o (CPF ou Email) e Pvs, não permite ir para o proximo passo
            if (GetCPF() == 0 && string.IsNullOrEmpty(GetEmail()))
            {
                return false;
            }
            else if (arrayPvs != null && arrayPvs.Count() > 1 && !GetPvsSelecionados().Any())
            {
                InformacaoUsuario info = InformacaoUsuario.Recuperar();
                if (info != null)
                {
                    if (this.WebPartRecuperacao.RecuperacaoUsuario)
                    {
                        return (info.QuantidadeEmaislDiferentes == 1);
                    }
                    else
                    {
                        return (info.PvSenhasIguais);
                    }
                }
                return false;
            }

            // se houver somente um PV, grava na sessão para ser usada nu próximo passo
            if (arrayPvs != null && arrayPvs.Count() == 1)
            {
                InformacaoUsuario info = InformacaoUsuario.Recuperar();
                info.PvsSelecionados = arrayPvs.Select(x => x.NumeroPV).ToArray();
                InformacaoUsuario.Salvar(info);
            }

            if (pnlAviso.Visible)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Verifica se existem pvs relacionados ao usuário, caso exista chama o método para carregar a grid
        /// </summary>
        /// <param name="maisDeUmPvMesmoUsu">Informa que exinstem mais de um pv para o mesmo usuário (CPF ou E-mail)</param>
        private void CarregarPvsRelacionadosAoUsu()
        {
            if (!IsVisibleListaPvs())
            {
                EntidadeServico.EntidadeServicoModel[] arrayPvs = GetPVsSemPaginacao();

                if (arrayPvs != null && arrayPvs.Count() > 1)
                {
                    CarregarListaDePvs(arrayPvs);
                }
            }
        }

        /// <summary>
        /// Seleciona os PVs sem paginação.
        /// </summary>
        /// <returns></returns>
        public EntidadeServico.EntidadeServicoModel[] GetPVsSemPaginacao()
        {
            int totalRows;
            int quantidadeEmaislDiferentes;
            return GetPVs(null, null, out totalRows, out quantidadeEmaislDiferentes);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsVisibleListaPvs()
        {
            return pnlListaPvs.Visible && rptListaPvs.Visible;
        }


        /// <summary>
        /// Cancelar a tentativa de recuperar Usuário/Senha e retornar à Home Aberta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Cancelar a tentativa de recuperar Usuário/Senha e retornar à Home Aberta"))
            {
                try
                {
                    this.Response.Redirect(this.RetornarHome(), false);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), false);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), false);
                    return;
                }
            }
        }

        /// <summary>
        /// Tentar responder os dados de recuperacação novamente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnTentarNovamente_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Cancelar a tentativa de recuperar Usuário/Senha e retornar à Home Aberta"))
            {
                try
                {
                    this.Response.Redirect(Request.Url.AbsoluteUri, false);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), false);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), false);
                    return;
                }
            }
        }

        #endregion

        #region [Métodos auxiliares]

        /// <summary>
        /// Retorna pvs selecinados na pagina
        /// </summary>
        /// <returns></returns>
        private int[] GetPvsSelecionados()
        {
            bool chkTodos = false;
            EntidadeServicoModel[] estabelecimentosRelacionados;
            InformacaoUsuario cache;
            List<int> pvs = new List<int>();

            estabelecimentosRelacionados = GetPVsSemPaginacao();

            cache = InformacaoUsuario.Recuperar();

            if (rptListaPvs.FindControl("chkTodosPdvs") != null)
            {
                CheckBox chkTodosPdvs = (CheckBox)rptListaPvs.FindControl("chkTodosPdvs");
                chkTodos = chkTodosPdvs.Checked;
            }

            if (chkTodos)
            {
                cache.PvsSelecionados = estabelecimentosRelacionados.Select(x => x.NumeroPV).ToArray();
            }
            else
            {
                foreach (RepeaterItem item in rptListaPvs.Items)
                {
                    CheckBox chkCodigo = (CheckBox)item.FindControl("chkPdv");

                    if (chkCodigo.Checked)
                    {
                        pvs.Add(Convert.ToInt32(chkCodigo.Attributes["data-value"]));
                    }
                }

                cache.PvsSelecionados = estabelecimentosRelacionados.Where(x => pvs.Contains(x.NumeroPV))
                                                                    .Select(x => x.NumeroPV)
                                                                    .ToArray();
            }

            if ((estabelecimentosRelacionados != null && estabelecimentosRelacionados.Any()) && (cache.PvsSelecionados != null && cache.PvsSelecionados.Any()))
            {
                cache.IdUsuario = estabelecimentosRelacionados.FirstOrDefault(x => cache.PvsSelecionados.Contains(x.NumeroPV)).IdUsuario;

                // se for selecionado apenas 1 PV, carrega os dados adequadamente
                if (cache.PvsSelecionados.Length == 1)
                {
                    var estbSelecionado = estabelecimentosRelacionados.FirstOrDefault(x => x.NumeroPV == cache.PvsSelecionados[0]);
                    cache.EmailSecundario = estbSelecionado.EmailSecundario;
                }
            }

            InformacaoUsuario.Salvar(cache);

            return cache.PvsSelecionados;
        }

        /// <summary>
        /// Valida o usuário.
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        private Boolean ValidarUsuario(UsuarioServico.Usuario usuario)
        {
            switch (usuario.Status.Codigo)
            {
                case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioAtivo:
                case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioAtivoLiberacaoAcessoCompletoBloqueada:
                case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoMaster:
                case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoAlteracaoEmail:
                case (Int32)Redecard.PN.Comum.Enumerador.Status.RespostaIdPosPendente:
                case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioAtivoAguardandoConfirmacaoRecuperacaoSenha:
                    {
                        return true;
                    }
                case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioAguardandoConfirmacaoCriacaoUsuario:
                    {
                        this.ExibirAviso("Verificamos que você ainda não concluiu sua última solicitação. Acesse seu endereço de e-mail e clique no link enviado.", "Atenção", base.RecuperarEnderecoPortal(), false);
                        break;
                    }
                case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioBloqueadoRecuperacaoSenha: //Cadastrar mensagem de bloqueio para recuperação
                case (Int32)Redecard.PN.Comum.Enumerador.Status.UsuarioBloqueadoRecuperacaoUsuario:
                    {
                        this.ExibirErro("RecuperacaoIdentificacao.ValidarIdentificacao", 1160, "Atenção: a quantidade de tentativas foi esgotada.", base.RecuperarEnderecoPortal(), false);
                        break;
                    }
                default:
                    {
                        this.ExibirAviso("Você não tem permissão para recuperar este usuário.", "Atenção", base.RecuperarEnderecoPortal(), false);
                        break;
                    }
            }
            return false;
        }


        /// <summary>
        /// Retorna PVs selecionados
        /// </summary>
        /// <returns></returns>
        private EntidadeServico.EntidadeServicoModel[] GetPVs(int? pagina, int? qtdRegistros, out int totalRows, out int quantidadeEmaislDiferentes)
        {
            Int64 cpf = 0;
            totalRows = 0;
            quantidadeEmaislDiferentes = 0;
            string email;
            EntidadeServico.EntidadeServicoModel[] arrayPvs = null;
            int codigoRetorno = 0;

            cpf = GetCPF();
            email = GetEmail();

            using (Logger log = Logger.IniciarLog("Seleciona os Pvs que estão relacionados ao CPF ou E-mail"))
            {
                try
                {
                    if (cpf != 0)
                    {
                        if (pagina.HasValue && qtdRegistros.HasValue)
                        {
                            log.GravarMensagem("Chamanda o serviço de consulta de Pvs por CPF");

                            using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                            {
                                arrayPvs = contextoEntidade.Cliente.ConsultarPvPorCpfComPaginacao(out codigoRetorno, out totalRows, out quantidadeEmaislDiferentes, cpf, 1, int.MaxValue, false, null, txtBuscaPv.Text);
                            }
                        }
                        else
                        {
                            log.GravarMensagem("Consultando cache por CPF");

                            arrayPvs = GetPVsCache(null, cpf);
                        }
                    }
                    else if (!string.IsNullOrEmpty(email))
                    {
                        if (pagina.HasValue && qtdRegistros.HasValue)
                        {
                            log.GravarMensagem("Chamanda o serviço de consulta de Pvs por email");

                            using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                            {
                                arrayPvs = contextoEntidade.Cliente.ConsultarPvPorEmailComPaginacao(out codigoRetorno, out totalRows, email, 1, int.MaxValue, null, txtBuscaPv.Text);
                            }
                        }
                        else
                        {
                            log.GravarMensagem("Consultando cache por email");

                            arrayPvs = GetPVsCache(email, null);
                        }
                    }

                    if (codigoRetorno > 0)
                    {
                        log.GravarMensagem("Erro inesperado na consulta dos pvs", new { codigoRetorno = codigoRetorno });

                        throw new Exception("Problemas para consultar pvs");
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, this.RetornarHome(), true);
                    return null;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), true);
                    return null;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), true);
                    return null;
                }
            }

            using (Logger log = Logger.IniciarLog("Validação estabelecimentos relacionados"))
            {
                if (arrayPvs == null || arrayPvs.Count() == 0)
                {
                    log.GravarErro(new Exception("Não foram encontrados estavelecimentos relacionados ao usuário"));

                    if (this.WebPartRecuperacao.RecuperacaoUsuario)
                    {
                        this.ExibirAviso("Não existe usuário para o CPF informado.", "Atenção", this.RetornarHome(), true);
                        return null;
                    }
                    else
                    {
                        this.ExibirAviso("Não existe estabelecimento para o e-mail informado.", "Atenção", this.RetornarHome(), true);
                        return null;
                    }
                }
            }

            return arrayPvs;
        }

        /// <summary>
        /// Retorna os Pvs relacionados ao e-mai/cpf, caso não exista no cache seleciona eles e os salva no cache.
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="cpf">CPF</param>
        /// <returns></returns>
        public EntidadeServico.EntidadeServicoModel[] GetPVsCache(string email, Int64? cpf, bool atualizar = false)
        {
            int codigoRetorno = 0;
            EntidadeServico.EntidadeServicoModel[] estabelecimentos;
            InformacaoUsuario cache;
            bool? pvSenhasIguais = false;
            Int32 quantidadeEmaislDiferentes = 0;

            using (Logger log = Logger.IniciarLog(@"Consulta no cache os Pvs relacionados ao CPF/Email"))
            {
                if (InformacaoUsuario.Existe() && atualizar == false)
                {
                    log.GravarMensagem("Dados existentes no cache");

                    cache = InformacaoUsuario.Recuperar();

                    log.GravarMensagem("Cache diferente de null", new { CacheIsnull = cache == null});
                    
                    estabelecimentos = cache.EstabelecimentosRelacinados;

                }
                else
                {
                    estabelecimentos = new EntidadeServico.EntidadeServicoModel[0];

                    if (!string.IsNullOrEmpty(email))
                    {
                        log.GravarMensagem("Email usuário informado", new { email = email});

                        GetUsuario(email, null);

                        using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        {
                            estabelecimentos = contextoEntidade.Cliente.ConsultarPvPorEmail(out codigoRetorno, out pvSenhasIguais, email, null, txtBuscaPv.Text);
                        }
                    }
                    else if (cpf.HasValue)
                    {
                        log.GravarMensagem("CPF usuário informado", new { cpf = cpf });

                        GetUsuario(null, cpf);

                        using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        {
                            estabelecimentos = contextoEntidade.Cliente.ConsultarPvPorCpf(out codigoRetorno, out quantidadeEmaislDiferentes, cpf.Value, null, txtBuscaPv.Text);
                        }
                    }

                    if (codigoRetorno > 0)
                    {
                        log.GravarMensagem("Retorno inesperado na consulta", new { codigoRetorno = codigoRetorno });

                        throw new Exception("Problemas para consultar pvs");
                    }

                    cache = InformacaoUsuario.Recuperar();

                    if (cache == null)
                    {
                        return estabelecimentos;
                    }

                    cache.QuantidadeEmaislDiferentes = quantidadeEmaislDiferentes;
                    cache.PvSenhasIguais = pvSenhasIguais.Value;
                    cache.EstabelecimentosRelacinados = estabelecimentos;

                    log.GravarMensagem("Salvando dados no cache", new { cache = cache });

                    InformacaoUsuario.Salvar(cache);
                }

                log.GravarMensagem("Quantidade de estabelecimentos relacionados", new { QuantidadeEstab = estabelecimentos.Count() });

            }

            return estabelecimentos;
        }

        /// <summary>
        /// Obtem o usuário por e-mail ou cpf
        /// </summary>
        /// <param name="email">e-mail do usuário</param>
        /// <param name="cpf">cpf do usuário</param>
        /// <returns></returns>
        private UsuarioServico.Usuario GetUsuario(string email, Int64? cpf)
        {
            UsuarioServico.Usuario info = null;
            UsuarioServico.Usuario[] infos;
            InformacaoUsuario cache;
            int codigoRetorno;
            string emailTela;
            Int64 cpfTela;

            emailTela = GetEmail();
            cpfTela = GetCPF();

            //Se não existir dados no cache consulta o usuário
            //Caso exista mas o PV informado não contem nos selecionados realiza a pesquisa novamente
            if (!InformacaoUsuario.Existe())
            {
                using (Logger log = Logger.IniciarLog("Consultando usuário"))
                {
                    using (var contextoUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                    {
                        if (!string.IsNullOrEmpty(email))
                        {
                            log.GravarMensagem("Consultando usuário por email");

                            info = contextoUsuario.Cliente.ConsultarPorEmailPrincipal(out codigoRetorno, email, 0, 0);
                        }
                        else if (cpf.HasValue)
                        {
                            log.GravarMensagem("Consultando usuário por CPF");

                            infos = contextoUsuario.Cliente.ConsultarPorCpf(cpf.Value, 0, 0);

                            if (infos != null && infos.Any())
                            {
                                info = infos.FirstOrDefault();
                            }
                        }
                    }

                    if (info != null)
                    {
                        log.GravarMensagem("Usuário encontrado", new { usuario = info });

                        cache = this.CriarInformacaoUsuario(info);
                    }
                    else
                    {
                        log.GravarMensagem("Usuário não encontrado");

                        this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", "Usuário não encontrado", this.RetornarHome(), true);
                        return null;
                    }
                }
            }
            else
            {
                info = new UsuarioServico.Usuario();
                cache = InformacaoUsuario.Recuperar();

                //Verifica se o email/cpf que está na tela é o mesmo que está no cahe, 
                //caso não seja limpa o cache e pega novamente o usuário
                if (!string.IsNullOrEmpty(emailTela))
                {
                    if (cache.EmailUsuario != emailTela)
                    {
                        InformacaoUsuario.Limpar();
                        return GetUsuario(emailTela, null);
                    }
                }
                else if (cpfTela != 0)
                {
                    if (cache.CpfUsuario != cpfTela)
                    {
                        InformacaoUsuario.Limpar();
                        return GetUsuario(null, cpfTela);
                    }
                }

                using (Logger log = Logger.IniciarLog("Retornando dados do cache"))
                {
                    log.GravarMensagem("Dados existentes no cache");

                    info.Entidade = new UsuarioServico.Entidade();
                    info.Entidade.GrupoEntidade = new UsuarioServico.GrupoEntidade();
                    info.Status = new UsuarioServico.Status();

                    info.Entidade.GrupoEntidade.Codigo = cache.GrupoEntidade;
                    info.Entidade.Codigo = cache.NumeroPV;
                    info.Status.Codigo = cache.Status;
                    info.Email = cache.EmailUsuario;

                    info.CodigoIdUsuario = cache.IdUsuario;
                    info.TipoUsuario = cache.TipoUsuario;
                    info.CPF = cache.CpfUsuario;

                    info.DDDCelular = cache.DddCelularUsuario;
                    info.Celular = cache.CelularUsuario;
                    info.EmailSecundario = cache.EmailSecundario;

                    log.GravarMensagem("Dados usuário", new { usuario = info });
                }
            }

            return info;
        }


        /// <summary>
        /// Retorna o CPF preenxido na tela
        /// </summary>
        /// <returns></returns>
        public Int64 GetCPF()
        {
            long retorno = 0;
            long.TryParse(txtCPF.Text.Replace(".", "").Replace("-", ""), out retorno);
            return retorno;
        }

        /// <summary>
        /// Retorna E-mail preenxido na tela
        /// </summary>
        /// <returns></returns>
        public string GetEmail()
        {
            if (string.IsNullOrEmpty(txtEmail.Text))
                return string.Empty;
            else
                return txtEmail.Text.ToLower();
        }

        /// <summary>
        /// Validar se os dados do usuário estão corretos
        /// </summary>
        private void ValidarRecuperacaoUsuario()
        {
            using (Logger log = Logger.IniciarLog("Validar se os dados do usuário estão corretos"))
            {
                try
                {
                    //Int32 entidade = txtEstab.Text.ToInt32();
                    if (ValidarCampos())
                    {
                        Int64 cpf = GetCPF();
                        UsuarioServico.Usuario usuario = GetUsuario(null, cpf);
                        if (usuario == null)
                        {
                            this.ExibirAviso("Os dados informados estão incorretos.", "Atenção", String.Empty, true);
                            return;

                        }
                        if (ValidarUsuario(usuario))
                            this.RedirecionarProximoPasso(usuario.TipoUsuario);
                        return;
                    }
                    this.ExibirAviso("Os dados informados estão incorretos.", "Atenção", String.Empty, true);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, this.RetornarHome(), true);
                    return;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), true);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), true);
                    return;
                }
            }
        }

        /// <summary>
        /// Carrega a lista de PV´s
        /// </summary>
        /// <param name="entidades"></param>
        private void CarregarListaDePvs(EntidadeServico.EntidadeServicoModel[] entidades)
        {

            pnlListaPvs.Visible = true;
            rptListaPvs.Visible = true;
            rptListaPvs.DataSource = entidades;
            rptListaPvs.DataBind();
            txtCPF.Enabled = false;
            txtEmail.Enabled = false;

        }


        /// <summary>
        /// Validar se os dados do usuário estão corretos
        /// </summary>
        private void ValidarRecuperacaoSenha()
        {
            using (Logger log = Logger.IniciarLog("Validar se os dados do usuário estão corretos"))
            {
                try
                {
                    InformacaoUsuario info = null;
                    if (ValidarCampos())
                    {
                        String email = GetEmail();

                        UsuarioServico.Usuario usuario = GetUsuario(email, null);
                        
                        if (usuario == null)
                        {
                            log.GravarMensagem("Os dados informados estão incorretos.");

                            this.ExibirAviso("Os dados informados estão incorretos.", "Atenção", String.Empty, true);
                            return;
                        }

                        if (ValidarUsuario(usuario))
                        {
                            info = InformacaoUsuario.Recuperar();
                        }

                        if (info != null)
                        {
                            log.GravarMensagem("Pvs com senhas iguais =" + info.PvSenhasIguais.ToString());

                            if (info.PvSenhasIguais)
                            {
                                info.PvsSelecionados = info.EstabelecimentosRelacinados.Select(p => p.NumeroPV).ToArray();
                            }

                            InformacaoUsuario.Salvar(info);
                            this.RedirecionarProximoPasso(usuario.TipoUsuario);
                        }
                    }
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, this.RetornarHome(), true);
                    return;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), true);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), true);
                    return;
                }
            }
        }

        /// <summary>
        /// Criar a sessão aberta do usuário para prosseguir aos próximos passos
        /// </summary>
        /// <param name="usuario"></param>
        private InformacaoUsuario CriarInformacaoUsuario(UsuarioServico.Usuario usuario)
        {
            return CriarInformacaoUsuario(usuario, null);
        }

        /// <summary>
        /// Criar a sessão aberta do usuário para prosseguir aos próximos passos
        /// </summary>
        /// <param name="usuario"></param>
        private InformacaoUsuario CriarInformacaoUsuario(UsuarioServico.Usuario usuario, EntidadeServico.EntidadeServicoModel[] listaPvs)
        {
            InformacaoUsuario.Criar(usuario.Entidade.GrupoEntidade.Codigo, usuario.Entidade.Codigo, usuario.Email);
            InformacaoUsuario info = InformacaoUsuario.Recuperar();
            info.IdUsuario = usuario.CodigoIdUsuario;
            info.TipoUsuario = usuario.TipoUsuario;
            info.CpfUsuario = usuario.CPF.HasValue ? usuario.CPF.Value : 0;

            info.DddCelularUsuario = usuario.DDDCelular.HasValue ? usuario.DDDCelular.Value : 0;
            info.CelularUsuario = usuario.Celular.HasValue ? usuario.Celular.Value : 0;
            info.EmailSecundario = usuario.EmailSecundario;
            info.Status = usuario.Status.Codigo;
            info.PodeRecuperarCriarAcesso = true;

            if (listaPvs != null && listaPvs.Length > 0)
            {
                info.EstabelecimentosRelacinados = listaPvs;
            }

            InformacaoUsuario.Salvar(info);

            return info;
        }


        /// <summary>
        /// Exibe a mensagem de erro com o link de retorno
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <param name="titulo">Título para o quadro de aviso</param>
        /// <param name="mensagemAdicional">Mensagem adicional</param>
        /// <param name="urlVoltar">Url de retorno após o erro</param>
        /// <param name="tentarNovamente">Indica se o botão de tentar novamente deve ser exibido</param>
        private void ExibirErro(String fonte, Int32 codigo, String titulo, String mensagemAdicional, String urlVoltar, Boolean tentarNovamente)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);

            if (!String.IsNullOrEmpty(mensagemAdicional))
            {
                mensagem += String.Concat("<br>", mensagemAdicional);
            }

            if (String.IsNullOrEmpty(urlVoltar))
                QdAviso.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Erro);
            else
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Erro);

            pnlAviso.Visible = true;
            pnlDadosIniciais.Visible = false;
        }

        /// <summary>
        /// Exibe uma mensagem de aviso
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        /// <param name="tentarNovamente">Indica se o botão de tentar novamente deve ser exibido</param>
        private void ExibirAviso(String mensagem, String titulo, String urlVoltar, Boolean tentarNovamente)
        {
            using (Logger log = Logger.IniciarLog("Exibição de aviso"))
            {
                log.GravarMensagem(string.Concat(titulo, mensagem
                    .Replace("{", "{{")
                    .Replace("}", "}}")));

                if (String.IsNullOrEmpty(urlVoltar))
                {
                    QdAviso.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Aviso);
                }
                else
                {
                    QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Aviso);
                }

                pnlTentarNovamente.Visible = tentarNovamente;

                pnlAviso.Visible = true;
                pnlDadosIniciais.Visible = false;
            }
        }

        /// <summary>
        /// Exibe uma mensagem de aviso gravada no banco de dados
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        /// <param name="tentarNovamente">Indica se o botão de tentar novamente deve ser exibido</param>
        private void ExibirAviso(String fonte, Int32 codigoErro, String titulo, String urlVoltar, Boolean tentarNovamente)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigoErro);

            if (String.IsNullOrEmpty(urlVoltar))
                QdAviso.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Aviso);
            else
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Aviso);

            pnlTentarNovamente.Visible = tentarNovamente;

            pnlAviso.Visible = true;
            pnlDadosIniciais.Visible = false;
        }

        /// <summary>
        /// Exibe uma mensagem de erro gravada no banco de dados
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        /// <param name="tentarNovamente">Indica se o botão de tentar novamente deve ser exibido</param>
        private void ExibirErro(String fonte, Int32 codigoErro, String titulo, String urlVoltar, Boolean tentarNovamente)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigoErro);

            if (String.IsNullOrEmpty(urlVoltar))
                QdAviso.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Erro);
            else
                QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Erro);

            pnlTentarNovamente.Visible = tentarNovamente;

            pnlAviso.Visible = true;
            pnlDadosIniciais.Visible = false;
        }

        /// <summary>
        /// Exibe uma mensagem de sucesso
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        /// <param name="tentarNovamente">Indica se o botão de tentar novamente deve ser exibido</param>
        private void ExibirSucesso(String mensagem, String titulo, String urlVoltar, Boolean tentarNovamente)
        {
            using (Logger log = Logger.IniciarLog("Exibição de aviso"))
            {
                log.GravarMensagem(string.Concat(titulo, mensagem));

                if (String.IsNullOrEmpty(urlVoltar))
                {
                    QdAviso.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Confirmacao);
                }
                else
                {
                    QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Confirmacao);
                }

                pnlTentarNovamente.Visible = tentarNovamente;

                pnlAviso.Visible = true;
                pnlDadosIniciais.Visible = false;
            }
        }

        /// <summary>
        /// Retorna a URL da HomePage do site
        /// </summary>
        /// <returns></returns>
        private String RetornarHome()
        {
            String url = String.Empty;
            url = Redecard.PN.Comum.Util.BuscarUrlRedirecionamento("/", SPUrlZone.Default);
            return url;
        }

        /// <summary>
        /// Redireciona o usuário para a Confirmação Positiva de Recuperação de Usuário
        /// </summary>
        private void RedirecionarProximoPasso(String tipoUsuario)
        {
            using (Logger log = Logger.IniciarLog("Redireciona o usuário para a Confirmação Positiva de Recuperação de Usuário"))
            {
                try
                {
                    String urlConfirmacao;

                    if (this.WebPartRecuperacao.RecuperacaoUsuario)
                    {
                        urlConfirmacao = String.Format("{0}/Paginas/Mobile/RecUsuarioConclusaoBasico.aspx", base.web.ServerRelativeUrl);
                    }
                    else
                    {
                        urlConfirmacao = String.Format("{0}/Paginas/Mobile/RecuperacaoSenhaFormaEnvio.aspx", base.web.ServerRelativeUrl);
                    }

                    log.GravarMensagem("Sendo direcionado para", new { urlConfirmacao });

                    this.Response.Redirect(urlConfirmacao, false);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), false);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), false);
                    return;
                }
            }
        }

        /// <summary>
        /// Exibe a mensagem de erro
        /// </summary>
        /// <param name="fonte">Fonte do erro</param>
        /// <param name="codigo">Código do erro</param>
        /// <param name="titulo">Título para o quadro de aviso</param>
        /// <param name="mensagemAdicional">Mensagem adicional</param>
        private void ExibirErro(String fonte, Int32 codigo, String titulo, String mensagemAdicional)
        {
            String mensagem = base.RetornarMensagemErro(fonte, codigo);

            if (!String.IsNullOrEmpty(mensagemAdicional))
            {
                mensagem += String.Concat("<br>", mensagemAdicional);
            }

            QdAviso.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Erro);
            pnlAviso.Visible = true;
            pnlDadosIniciais.Visible = false;
        }

        /// <summary>
        /// Exibe uma mensagem de erro personalizada
        /// </summary>
        /// <param name="codigo"></param>
        /// <param name="mensagem"></param>
        /// <param name="titulo"></param>
        private void ExibirErro(Int32 codigo, String mensagem, String titulo)
        {

            if (!String.IsNullOrEmpty(mensagem))
            {
                mensagem = String.Concat(mensagem, String.Format(" ({0})", codigo.ToString()));
            }

            QdAviso.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Erro);
            pnlAviso.Visible = true;
            pnlDadosIniciais.Visible = false;
        }

        /// <summary>
        /// Exibe uma mensagem de erro personalizada
        /// </summary>
        /// <param name="mensagem"></param>
        /// <param name="titulo"></param>
        /// <param name="urlVoltar"></param>
        private void ExibirErro(String mensagem, String titulo, String urlVoltar)
        {
            QdAviso.CarregarMensagem(titulo, mensagem, urlVoltar, QuadroAvisosResponsivo.IconeMensagem.Erro);

            pnlAviso.Visible = true;

            pnlDadosIniciais.Visible = false;

        }

        /// <summary>
        /// Método para validar os campos da tela.
        /// </summary>
        /// <returns></returns>
        private Boolean ValidarCampos()
        {
            if (!WebPartRecuperacao.RecuperacaoUsuario)
            {
                if (txtEmail.Text != String.Empty && !ValidarCamposEmail.ValidarFormatoEmail(txtEmail.Text))
                    return false;
                return true;
            }
            if (!ValidarCpf.Validar(true, new ControleCampos(txtCPF, cpfContainer, cpfTexto), true))
                return false;
            return true;
        }

        /// <summary>
        /// EventHandler genérico para PostBack customizado
        /// </summary>
        /// <param name="eventArgument"></param>
        public void RaisePostBackEvent(string eventArgument)
        {
            if (string.Compare(eventArgument, "ReenviarSolicitacaoAprovacao", true) >= 0)
            {
                using (Logger log = Logger.IniciarLog("RaisePostBackEvent opção ReenviarSolicitacaoAprovacao"))
                {
                    // valida se foi passado algum código válido
                    if (eventArgument.Split('|').Length <= 1)
                        return;

                    var dicArguments = (Dictionary<string, object>)new JavaScriptSerializer().Deserialize(eventArgument.Split('|')[1], typeof(object));

                    int codigoEntidade = Convert.ToInt32(dicArguments["CodigoEntidade"]);
                    int codigoUsuario = Convert.ToInt32(dicArguments["CodigoUsuario"]);
                    int codigoGrupoEntidade = Convert.ToInt32(dicArguments["CodigoGrupoEntidade"]);
                    string emailUsuario = Convert.ToString(dicArguments["EmailUsuario"]);

                    UsuarioServico.Usuario usuario = null;
                    EntidadeServico.Usuario[] usuariosMaster = null;

                    try
                    {
                        using (var contexto = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            log.GravarMensagem("Chamando método ConsultarPorEmailPrincipalPorStatus");

                            int codigoRetorno;
                            var arrayUsuarios = contexto.Cliente.ConsultarPorEmailPrincipalPorStatus(
                                out codigoRetorno,
                                emailUsuario,
                                codigoGrupoEntidade,
                                codigoEntidade,
                                new int[] { 10 });

                            log.GravarMensagem("Resultado método ConsultarPorEmailPrincipalPorStatus", new { result = arrayUsuarios });

                            if (arrayUsuarios != null)
                                usuario = arrayUsuarios.FirstOrDefault(x => x.CodigoIdUsuario == codigoUsuario);

                            if (usuario == null)
                                throw new Exception("Nenhum usuário encontrado com os dados informados");
                        }
                    }
                    catch (FaultException<UsuarioServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, this.RetornarHome(), true);

                        return;
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), true);

                        return;
                    }

                    try
                    {
                        using (var contexto = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                        {
                            log.GravarMensagem("Chamando método ConsultarUsuariosPorPerfil");

                            int codigoRetorno;
                            usuariosMaster = contexto.Cliente.ConsultarUsuariosPorPerfil(out codigoRetorno, codigoEntidade, 1, 'M');

                            log.GravarMensagem("Resultado método ConsultarUsuariosPorPerfil", new { result = usuariosMaster });

                            if (!(usuariosMaster != null && usuariosMaster.Length > 0))
                                throw new Exception("Nenhum usuário master encontrado relacionado ao estabelecimento informado");
                        }
                    }
                    catch (FaultException<EntidadeServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty, this.RetornarHome(), true);

                        return;
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty, this.RetornarHome(), true);

                        return;
                    }

                    if (usuario != null && usuariosMaster != null && usuariosMaster.Length > 0)
                    {
                        string[] emails = usuariosMaster
                            .Select(master => master.Email)
                            .Where(email => !String.IsNullOrEmpty(email)).ToArray();

                        //Envia e-mail de aprovação
                        EmailNovoAcesso.EnviarEmailAprovacaoAcesso(
                            string.Join(",", emails),
                            usuario.Descricao,
                            usuario.Email,
                            usuario.CodigoIdUsuario,
                            usuario.TipoUsuario,
                            codigoEntidade,
                            null);

                        this.ExibirSucesso(@"
Dentro de instantes o usuário Master receberá o e-mail solicitando a<br/>
aprovação do seu acesso ao Portal Rede.", "E-mail reenviado com sucesso.", this.RetornarHome(), false);
                    }
                }
            }
        }

        #endregion

        protected void btnBuscarPvs_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ChangeFlagPvsFiltrados();

            if (!InformacaoUsuario.Existe())
            {
                return;
            }

            InformacaoUsuario info = InformacaoUsuario.Recuperar();
            EntidadeServico.EntidadeServicoModel[] entidades = info.EstabelecimentosRelacinados;
            EntidadeServico.EntidadeServicoModel[] entidadeFiltro; 

            if (entidades != null && entidades.Any())
            {
                entidadeFiltro = entidades.Where(x => string.Concat(x.NumeroPV, x.RazaoSocial)
                                                                         .ToUpper()
                                                                         .Contains(this.txtBuscaPv.Text.ToUpper()))
                                                                         .ToArray();

                this.rptListaPvs.DataSource = entidadeFiltro;

                this.rptListaPvs.DataBind();
            }
        }

        private void ChangeFlagPvsFiltrados()
        {
            if (!string.IsNullOrEmpty(txtBuscaPv.Text))
            {
                hdnPvsFiltrados.Value = "1";
                return;
            }

            hdnPvsFiltrados.Value = "0";

        }
    }
}
