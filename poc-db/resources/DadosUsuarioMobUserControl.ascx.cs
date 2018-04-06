/*
© Copyright 2015 Rede S.A.
Autor : Felipe Siatiquosque
Empresa : Rede
*/

using Rede.PN.DadosCadastraisMobile.SharePoint.CONTROLTEMPLATES.DadosCadastraisMobile;
using Rede.PN.DadosCadastraisMobile.SharePoint.EntidadeServico;
using Rede.PN.DadosCadastraisMobile.SharePoint.Util;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.DadosUsuarioMob
{
    public partial class DadosUsuarioMobUserControl : UserControlBase
    {
        #region [Controles da Página]

        /// <summary>
        /// qdAviso control.
        /// </summary>
        protected QuadroAvisosResponsivo QdAviso { get { return (QuadroAvisosResponsivo)qdAviso; } }

        #endregion

        #region [Eventos da página]

        /// <summary>
        /// Inicialização da WebPart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            InformacaoUsuario infoUsuario;

            if (!Page.IsPostBack)
            {
                using (Logger log = Logger.IniciarLog("Inicialização da WebPart de 2º Passo"))
                {
                    try
                    {
                        if (!InformacaoUsuario.Existe())
                        {
                            log.GravarMensagem("Dados inexistentes no cache");

                            this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", String.Empty, base.RecuperarEnderecoPortal());
                            return;
                        }
                        else
                        {
                            log.GravarMensagem("Dados existentes no cache");

                            this.ConsultarBancos(this.banco);
                        }
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

                    infoUsuario = InformacaoUsuario.Recuperar();

                    var pvsSelecionados = GetPvsSelecionados();

                    log.GravarMensagem("Verificando PVs selecionados no passo anterior");

                    // apresenta erro ao usuário caso nenhum PV seja identificado
                    if (!(pvsSelecionados != null && pvsSelecionados.Length > 0))
                    {
                        log.GravarMensagem("Nenhum pv selecionado");

                        this.ExibirErro(0, "Nenhum estabelecimento selecionado", "Atenção:");
                        return;
                    }

                    if (pvsSelecionados.Length == 1)
                    {
                        log.GravarMensagem("Apenas 1 pv selecionado");

                        // traz o Nome Completo preenchido com a razão social do único PV selecionado
                        var pv = pvsSelecionados.FirstOrDefault();
                        if (!pv.PossuiUsuario.GetValueOrDefault(false))
                        {
                            nomeCompleto.Text = pv.RazaoSocial;
                            // nomeCompleto.Attributes.Add("exibir-link-editar", "true");
                        }
                    }

                    if (infoUsuario.Empresa)
                    {
                        // Consulta se CPF informado pelo usuário no primeiro passo corresponde a um sócio vinculado ao(s) PV(s) selecionado(s)
                        log.GravarMensagem("Consulta se CPF informado pelo usuário no primeiro passo corresponde a um sócio vinculado ao(s) PV(s) selecionado(s)");

                        using (var ctxUsuario = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            // consulta o serviço para verificar o CPF do usuário
                            if (ctxUsuario.Cliente.ValidarCpfCnpjSocio(pvsSelecionados.FirstOrDefault().NumeroPV, infoUsuario.CpfUsuario))
                            {
                                log.GravarMensagem("Cpf informado foi de um sócio");

                                // grava CPF do sócio para posterior confirmação positiva
                                infoUsuario.CpfCnpjSocio = infoUsuario.CpfUsuario;
                                InformacaoUsuario.Salvar();

                                // oculta o campo CPF/CNPJ do sócio, pois não há mais a necessidade de preenchimento manual
                                this.divCpfCnpjSocio.Visible = false;
                            }
                            else
                            {
                                log.GravarMensagem("Cpf informado não foi de um sócio");

                                // exibe o campo CPF/CNPJ do sócio para preenchimento manual
                                this.divCpfCnpjSocio.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        divCpfUsuario.Visible = true;

                        var pv = pvsSelecionados.FirstOrDefault();

                        // se for o primeiro usuário do PV, carrega o CPF do proprietário
                        if (!pv.PossuiUsuario.GetValueOrDefault(false) && infoUsuario.CpfProprietario.HasValue)
                        {
                            cpfUsuario.Text = this.GetCpfFormatado(infoUsuario.CpfProprietario.Value);
                            // cpfUsuario.Attributes.Add("exibir-link-editar", "true");
                        }
                    }
                }
            }
        }

        private EntidadeServicoModel[] GetPvsSelecionados()
        {
            InformacaoUsuario infoUsuario;
            
            if (InformacaoUsuario.Existe())
            {
                infoUsuario = InformacaoUsuario.Recuperar();

                if (infoUsuario != null && infoUsuario.PvsSelecionados != null && infoUsuario.PvsSelecionados.Any())
                {
                    return infoUsuario.EstabelecimentosRelacinados.Where(x => infoUsuario.PvsSelecionados.Contains(x.NumeroPV)).ToArray();
                }
            }

            return null;
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
                        if (ValidarDados() && ValidarConfirmacaoPositiva())
                        {
                            InformacaoUsuario info = InformacaoUsuario.Recuperar();

                            info.EmailSecundario = String.Empty;
                            info.DddCelularUsuario = ValidarCelular.GetDDD(celular.Text).ToEmptyString().ToInt32Null(0).Value;
                            info.CelularUsuario = ValidarCelular.GetNumero(celular.Text).ToEmptyString().ToInt32Null(0).Value;
                            info.NomeCompleto = nomeCompleto.Text;
                            info.SenhaUsuario = EncriptadorSHA1.EncryptString(senhaAcesso.Text);

                            // obtém o "CPF do Usuário" se for PF
                            if (!info.Empresa)
                                info.CpfUsuario = GetCpfUsuarioDigitado().GetValueOrDefault(0);

                            InformacaoUsuario.Salvar(info);
                            this.ConcluirProcesso();
                        }

                    }
                    else
                    {
                        log.GravarMensagem("Problemas para validar os dados (ValidarDados() && ValidarConfirmacaoPositiva()) = false");

                        this.ExibirErro("SharePoint.SessaoUsuario", 1151, "Atenção", String.Empty, base.RecuperarEnderecoPortal());

                        return;
                    }
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

        public int GetDDDCelular()
        {
            return Convert.ToInt32(ValidarCelular.GetDDD(celular.Text).ToEmptyString().ToInt32Null(0).Value);

        }

        public int GetCelular()
        {
            return ValidarCelular.GetNumero(celular.Text).ToEmptyString().ToInt32Null(0).Value;
        }

        public string NormalizaString(string value)
        {
            return value.Replace("(", "")
                        .Replace(")", "")
                        .Replace("-", "")
                        .Replace(" ", "")
                        .Replace(".", "");
        }

        private long? GetCpfUsuario()
        {
            InformacaoUsuario cache;

            if (InformacaoUsuario.Existe())
            {
                cache = InformacaoUsuario.Recuperar();

                if (cache == null || cache.CpfUsuario == 0)
                {
                    return null;
                }

                return cache.CpfUsuario;
            }

            return null;
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
            int intCelular = GetCelular();
            int dDDCelular = GetDDDCelular();
            string strNomeCompleto = nomeCompleto.Text;
            string senha = senhaAcesso.Text;
            
            for (int i = 0; i < entidade.Count(); i++)
            {
                entidade[i].Email = GetEmailUsuario();
                entidade[i].DDDCelular = dDDCelular;
                entidade[i].Celular = intCelular;
                entidade[i].NomeCompletoUsuario = strNomeCompleto;
                entidade[i].SenhaCriptografada = EncriptadorSHA1.EncryptString(senha);
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
                                p, info.EntidadePossuiMaster, info.EntidadePossuiUsuario);
                        });

                        this.ExibirSucesso();

                        log.GravarMensagem("Usuários gravados com sucesso");
                    }
                    else
                        this.ExibirErro(FONTE, codigoRetorno, "Atenção", mensagem, string.Empty);


                }
                catch (System.ServiceModel.FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
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
        /// Exibe uma mensagem de sucesso na página
        /// </summary>
        /// <param name="mensagem">Mensagem de aviso</param>
        /// <param name="titulo">Título do aviso</param>
        /// <param name="urlVoltar">Url de retorno</param>
        private void ExibirSucesso()
        {
            string serverRelativeUr = base.web.ServerRelativeUrl;
            string url = String.Format(@"{0}/Paginas/Mobile/CriacaoUsrConclusao.aspx", serverRelativeUr);

            using (Logger log = Logger.IniciarLog("Exibir msg sucesso"))
            {   
                try
                {
                    Response.Redirect(url);
                }
                catch (ThreadAbortException ex)
                {
                    log.GravarErro(ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }
            }

            HttpContext.Current.ApplicationInstance.CompleteRequest();
            InformacaoUsuario.Limpar();
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

                    if (!string.IsNullOrWhiteSpace(hdfBloqueadoConfirmacaoPositiva.Value))
                    {
                        Response.Redirect(string.Format(
                            "{0}/Paginas/Mobile/CriacaoUsrDadosIniciais.aspx", 
                            base.web.ServerRelativeUrl));
                    }
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
                // se o retorno da mensagem não tem algo relevante,
                // considera a mensagem adicional em parâmetro
                if (mensagem.Contains("-1"))
                    mensagem = string.Empty;
                else
                    mensagem = string.Concat(mensagem, "<br />");

                mensagem = String.Concat(mensagem, mensagemAdicional);
            }

            QdAviso.CarregarMensagem(titulo, mensagem, QuadroAvisosResponsivo.IconeMensagem.Erro);
            btnVoltar.Visible = false;

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
        /// Valida se os dados preenchidos estão corretos
        /// </summary>
        /// <returns>
        /// <para>True - Dados válidos</para>
        /// <para>False - Dados inválidos</para>
        /// </returns>
        private Boolean ValidarDados()
        {
            Boolean valida = true;

            if (!(ValidarSenha.Validar(true, new ControleCampos(this.senhaAcesso, senhaAcessoContainer, spanForcaSenha))))
            {
                valida = false;
            }

            if (!(banco.SelectedItem != null && !string.IsNullOrEmpty(banco.SelectedItem.Value)))
            {
                valida = false;
            }

            if (string.IsNullOrEmpty(agencia.Text))
            {
                valida = false;
            }

            if (string.IsNullOrEmpty(contaCorrente.Text))
            {
                valida = false;
            }

            if (divCpfCnpjSocio.Visible && string.IsNullOrEmpty(cpfCnpjSocio.Text))
            {
                valida = false;
            }

            using (Logger log = Logger.IniciarLog("Validando dados informados"))
            {
                log.GravarMensagem("Validando dados", new { resultado = valida });
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
            string infoBanco = banco.SelectedItem.Value;
            string infoAgencia = agencia.Text;
            string infoContaCorrente = contaCorrente.Text;
            EntidadeServicoModel[] estabelecimentos = GetPVsCache();
            EntidadeServicoModel[] entidadesPossuemUsuario;
            EntidadeServicoModel[] entidadesPossuemMaster;

            long? cpf = GetCpf();
            long? cnpj = GetCnpj();

            if (cpf == null && cnpj == null && (estabelecimentos == null || !estabelecimentos.Any()))
            {
                this.ExibirErro(0, "Atenção:", "Não foi possível validar os dados informados");
                return false;
            }

            using (Logger log = Logger.IniciarLog("Validando confirmação positiva"))
            {
                UsuarioServico.Pergunta[] perguntasIncorretas = new UsuarioServico.Pergunta[] { 
                    new UsuarioServico.Pergunta(){
                    Codigo = 3, 
                    Descricao = descricaoPerguntasBasicas[3]},
                    new UsuarioServico.Pergunta(){
                    Codigo = 20, 
                    Descricao = descricaoPerguntasBasicas[20]}
                };

                using (var entidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    response = entidade.Cliente.ValidarConfirmacaoPositivaPrimeiroAcesso(
                        out entidadesPossuemUsuario,
                        out entidadesPossuemMaster,
                        new ConfirmacaoPositivaPrimeiroAcessoRequest()
                        {
                            Agencia = infoAgencia,
                            Banco = infoBanco,
                            ContaCorrente = infoContaCorrente,
                            EntidadesPNSelecionadas = estabelecimentos,
                            CpfProprietario = cpf,
                            CnpjEstabelecimento = cnpj,
                            CpfCnpjSocio = this.divCpfCnpjSocio.Visible ? GetCpfCnpjSocio() : null
                        });

                    log.GravarMensagem("Retorno confirmação positiva", new {result = response != null ? response.CodigoRetorno : 0 });

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
                    log.GravarMensagem("PVs bloqueados por exesso de tentativas de confirmação positiva");

                    EnviarEmailAcessoBloqueado(entidadesPossuemUsuario, entidadesPossuemMaster);

                    GravarHistoricoBloqueioFormularioSolicitacaoAcesso();
                    this.ExibirErro(
                        1106, @"Solicite o desbloqueio para o usuário Master ou com
                            a nossa Central de Atendimento através dos telefones: 
                            <br /><b>4001 4433 (Capitais e Regiões Metropolitanas)</b>
                            <br /><b>0800 728 4433 (Demais Regiões)</b>",
                        "Atenção: A quantidade de tentativas esgotou e a criação de usuário foi bloqueada.");

                    hdfBloqueadoConfirmacaoPositiva.Value = "true";
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

                    foreach (var numeroPV in usuario.PvsSelecionados)
                    {
                        Historico.ErroConfirmacaoPositiva(  usuario.IdUsuario, 
                                                            usuario.NomeCompleto, 
                                                            usuario.EmailUsuario,
                                                            usuario.TipoUsuario, 
                                                            numeroPV, 
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
        private long? GetCpf()
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

        /// <summary>
        /// Retorna o CPF/CNPJ do sócio gravado na sessão
        /// </summary>
        /// <returns></returns>
        private long? GetCpfCnpjSocio()
        {
            long cpfCnpj = 0;
            long.TryParse(NormalizaString(this.cpfCnpjSocio.Text), out cpfCnpj);
            return cpfCnpj;
        }

        private long? GetCpfUsuarioDigitado()
        {
            long cpfUsuario = 0;
            long.TryParse(NormalizaString(this.cpfUsuario.Text), out cpfUsuario);
            return cpfUsuario;
        }

        /// <summary>
        /// Pega os Pvs relacionados ao CPF/CNPJ selecionados no passo anterior
        /// </summary>
        /// <returns></returns>
        private EntidadeServicoModel[] GetPVsCache()
        {
            EntidadeServico.EntidadeServicoModel[] result = new EntidadeServicoModel[0];
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
                    if (result.Count() > 1)
                        return result.Where(x => cache.PvsSelecionados.Contains(x.NumeroPV)).ToArray();
                    else
                        return result;
                }
            }

            return result;
        }

        #endregion 

        /// <summary>
        /// Consulta os bancos e preenche combo
        /// </summary>
        private void ConsultarBancos(DropDownList ddlBanco)
        {
            using (Logger Log = Logger.IniciarLog("Consultando bancos e preenchendo combos"))
            {
                using (var entidadeClient = new EntidadeServico.EntidadeServicoClient())
                {
                    var bancos = entidadeClient.ConsultarBancosConfirmacaoPositiva();
                    foreach (EntidadeServico.Banco banco in bancos)
                    {
                        ddlBanco.Items.Add(new ListItem(
                            string.Concat(banco.Codigo.ToString(), " - ", banco.Descricao),
                            banco.Codigo.ToString()));
                    }
                }
            }
        }

        /// <summary>
        /// Aplica máscara CPF (999.999.999-99)
        /// </summary>
        public String GetCpfFormatado(Int64? cpf)
        {
            if (cpf.HasValue)
            {
                String valor = cpf.Value.ToString().PadLeft(11, '0');

                if (valor.Length == 11)
                {
                    valor = valor.Insert(3, ".");
                    valor = valor.Insert(7, ".");
                    valor = valor.Insert(11, "-");
                }
                return valor;
            }
            else
                return String.Empty;
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
                                continue;

                            if (string.IsNullOrEmpty(emailEstabelecimento))
                                continue;

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
                catch (NullReferenceException ex)
                {
                    log.GravarLog(EventoLog.ChamadaServico, ex);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                }
            }
        }

        #endregion
    }
}
