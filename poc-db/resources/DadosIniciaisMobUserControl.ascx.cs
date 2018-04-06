/*
© Copyright 2015 Rede S.A.
Autor : Felipe Siatiquosque
Empresa : Rede
*/
using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Rede.PN.DadosCadastraisMobile.SharePoint.CONTROLTEMPLATES.DadosCadastraisMobile;
using System.ServiceModel;
using System.Web;
using Rede.PN.DadosCadastraisMobile.SharePoint.Util;
using System.Text.RegularExpressions;
using Rede.PN.DadosCadastraisMobile.SharePoint.UsuarioServico;
using Rede.PN.DadosCadastraisMobile.SharePoint.EntidadeServico;
using System.Collections.Generic;
using Redecard.PN.Comum.BlacklistValidations;

namespace Rede.PN.DadosCadastraisMobile.SharePoint.WPMobile.DadosIniciaisMob
{
    public partial class DadosIniciaisMobUserControl : UserControlBase, IPostBackEventHandler
    {
        #region [Atributos da WebPart]

        /// <summary>
        /// Webpart de Dados Inciais
        /// </summary>
        public DadosIniciaisMob WebPartDadosIniciais { get; set; }

        public LinkButton linkBotao;


        #endregion

        #region [Controles]
        /// <summary>
        /// qdAviso control.
        /// </summary>
        protected QuadroAvisosResponsivo QdAviso { get { return (QuadroAvisosResponsivo)qdAviso; } }

        /// <summary>
        /// qdAvisoMigracao control.
        /// </summary>
        protected QuadroAvisosResponsivo QdAvisoMigracao { get { return (QuadroAvisosResponsivo)qdAvisoMigracao; } }

        #endregion

        #region [Eventos da Pagina]

        /// <summary>
        /// Inicialização da webpart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InformacaoUsuario.Limpar();

                using (Logger log = Logger.IniciarLog("Processo de black list"))
                {
                    // valida IP do cliente na blacklist
                    BlacklistIPs blacklist = new BlacklistIPs();

                    if (!blacklist.ValidarIP(BlacklistIPs.GetClientIP()))
                    {
                        log.GravarMensagem("Criação de usuário e senha não permitida. Para mais informações, entre em contato com a central de atendimento");

                        this.ExibirErro("Criação de usuário e senha não permitida. Para mais informações, entre em contato com a central de atendimento", "Atenção", base.RecuperarEnderecoPortal());
                        return;
                    }
                }
            }

            using (Logger log = Logger.IniciarLog("Inicialização da webpart de Dados Iniciais"))
            {

                try
                {
                    this.pnlPassos.Visible = true;
                    pnlFormBloqueado.Visible = false;

                    if (!Page.IsPostBack)
                    {
                        this.ValidarPermissao = false;

                        if (this.WebPartDadosIniciais.AcessoUsuarioLegado)
                        {
                            if (InformacaoUsuario.Existe())
                            {
                                log.GravarMensagem("Informações existentes no cache");

                                InformacaoUsuario infoUsuario = InformacaoUsuario.Recuperar();
                            }

                            this.ExibirAvisoMigracao();
                            return;
                        }
                        else
                        {
                            InformacaoUsuario.Limpar();
                        }
                    }
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }

                //linkBotao = new LinkButton();
                //linkBotao.Click += new EventHandler(this.ReenviarSolicitacaoAprovacao);
            }
        }

        /// <summary>
        /// Realiza as validações dos dados para Continuar para o próximo Passo da Criação de usuário
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            if (!IsVisibleGrid())
            {
                // força reload dos dados (prevendo se usuário clicou em voltar apenas)
                InformacaoUsuario.Limpar();
            }

            using (Logger log = Logger.IniciarLog("Continuar para o próximo Passo da Criação de usuário"))
            {
                try
                {
                    var campoCpfCnpj = codPessoa.Text.ToString();
                    var campoEmail = this.email.Text;
                    bool permitirProximoPasso = false;
                    String email = campoEmail.ToLower().Trim();
                    String cpfCnpj = String.Empty;

                    cpfCnpj = NormalizarString(campoCpfCnpj);

                    if (this.ValidarDadosIniciais(cpfCnpj))
                    {
                        permitirProximoPasso = true;
                    }

                    if (permitirProximoPasso)
                    {
                        this.RedirecionarPasso2();
                    }

                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }
            }
        }

        /// <summary>
        /// Ação de voltar da mensagem de aviso
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Ação de voltar da mensagem de aviso"))
            {
                try
                {
                    pnlDadosIniciais.Visible = true;
                    pnlAviso.Visible = false;

                    if (this.WebPartDadosIniciais.AcessoUsuarioLegado)
                        qdAvisoMigracao.Visible = true;
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }
            }
        }

        /// <summary>
        /// Voltar para homepage aberta do Portal após aviso
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltarAviso_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Voltar para homepage aberta do Portal após aviso"))
            {
                try
                {
                    Response.Redirect(base.RecuperarEnderecoPortal());
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }
            }
        }

        /// <summary>
        /// Continuar para o próximo Passo após aviso
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuarPasso_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Continuar para o próximo Passo após aviso"))
            {
                try
                {
                    this.RedirecionarPasso2();
                }
                catch (PortalRedecardException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }
            }
        }

        /// <summary>
        /// Continuar para a Home aberta do Portal após exibir o aviso de que possui Usuário Legado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuarLegado_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Continuar para a Home aberta do Portal após exibir o aviso de que possui Usuário Legado"))
            {
                try
                {
                    this.Response.Redirect(base.RecuperarEnderecoPortal(), false);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }
            }
        }

        /// <summary>
        /// Evento chamado após o usuário solicitar o reenvio de e-mail de aprovação de acesso
        /// para o usuário Master, com envio de e-mail bem sucedido.
        /// </summary>
        private void txtEmailUsuario_SolicitacaoAprovacaoReenviada()
        {
            ExibirAvisoEmailReenviado();
        }

        protected void btnBuscarPvs_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (!InformacaoUsuario.Existe())
            {
                return;
            }

            InformacaoUsuario info = InformacaoUsuario.Recuperar();
            var entidades = info.EstabelecimentosRelacinados;

            if (entidades != null && entidades.Any())
            {
                this.rptListaPvs.DataSource = entidades.Where(x => string.Concat(x.NumeroPV, x.RazaoSocial)
                                                                         .ToUpper()
                                                                         .Contains(this.txtBuscaPv.Text.ToUpper()));

                this.rptListaPvs.DataBind();
            }
        }


        #endregion

        #region [Métodos auxiliares]

        /// <summary>
        /// Informa se a Grid está visivel
        /// </summary>
        /// <returns></returns>
        public bool IsVisibleGrid()
        {
            return pnlListaPvs.Visible &&
                   txtBuscaPv.Visible &&
                   btnBuscarPvs.Visible &&
                   rptListaPvs.Visible;
        }

        /// <summary>
        /// Altera os elementos da grid para visivel ou não
        /// </summary>
        /// <param name="visible"></param>
        private void VisibleGrid(bool visible)
        {
            pnlListaPvs.Visible = visible;
            txtBuscaPv.Visible = visible;
            btnBuscarPvs.Visible = visible;
            rptListaPvs.Visible = visible;

            // campos desabilitados na listagem dos PVs
            if (visible)
            {
                tipoPessoaHidden.Value = tipoJuridica.Checked ? "J" : "F";
                codPessoa.Attributes.Add("readonly", "true");
                codPessoa.Style.Add("background-color", "#dedede");
            }
        }

        /// <summary>
        /// Redirecionando para o Passo 2 da Criação de Usuário
        /// </summary>
        private void RedirecionarPasso2()
        {
            using (Logger log = Logger.IniciarLog("Redirecionando para o Passo 2 da Criação de Usuário"))
            {
                try
                {
                    String linkPasso2 = String.Format("{0}/Paginas/Mobile/CriacaoUsrCadastro.aspx", base.web.ServerRelativeUrl);

                    log.GravarMensagem("Indo para o proximo passo", new { linkPasso2 });

                    this.Response.Redirect(linkPasso2, false);
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return;
                }
            }
        }

        /// <summary>
        /// Retorna pvs selecinados na pagina
        /// </summary>
        /// <returns></returns>
        private void ObterPvsSelecionados(out List<EntidadeServico.EntidadeServicoModel> entidadesRelacionadas, out List<Int32> pvsSelecionados)
        {
            InformacaoUsuario cache;
            entidadesRelacionadas = new List<EntidadeServico.EntidadeServicoModel>();
            pvsSelecionados = new List<int>();

            if (InformacaoUsuario.Existe())
            {
                cache = InformacaoUsuario.Recuperar();

                if (cache.EstabelecimentosRelacinados != null)
                {
                    entidadesRelacionadas = cache.EstabelecimentosRelacinados.ToList();
                }
            }

            foreach (RepeaterItem item in rptListaPvs.Items)
            {
                CheckBox chkCodigo = (CheckBox)item.FindControl("chkPdv");

                if (chkCodigo.Checked)
                {
                    pvsSelecionados.Add(Convert.ToInt32(chkCodigo.Attributes["data-value"]));
                }
            }
        }

        /// <summary>
        /// Retorna os PVs selecionados
        /// </summary>
        /// <returns></returns>
        private int[] GetPvsSelecionados()
        {
            List<int> pvsSelecionados = new List<int>();
            String cpfCnpj = String.Empty;
            EntidadeServico.EntidadeServicoModel[] entidades;
            bool tipoPJ = tipoJuridica.Checked;
            long numCpfCnpj = 0;
            var campoCpfCnpj = codPessoa.Text.ToString();

            cpfCnpj = NormalizarString(campoCpfCnpj);

            long.TryParse(cpfCnpj, out numCpfCnpj);

            if (!IsVisibleGrid())
            {
                if (tipoPJ)
                {
                    entidades = this.GetPvs(cpfCnpj, null);
                }
                else
                {
                    entidades = this.GetPvs(null, numCpfCnpj);
                }

                if (entidades != null && entidades.Count() == 1)
                {
                    pvsSelecionados = entidades.Select(x => x.NumeroPV).ToList();
                }
            }
            else
            {
                foreach (RepeaterItem item in rptListaPvs.Items)
                {
                    CheckBox chkCodigo = (CheckBox)item.FindControl("chkPdv");

                    if (chkCodigo.Checked)
                    {
                        pvsSelecionados.Add(Convert.ToInt32(chkCodigo.Attributes["data-value"]));
                    }
                }
            }
            return pvsSelecionados.ToArray();
        }

        /// <summary>
        /// Verifica se CNPJ informado é uma matriz e pega o numero do PV
        /// </summary>
        /// <param name="cnpj">CNPJ</param>
        /// <returns></returns>
        private bool ValidarProximoPassoPJ(String cnpj)
        {
            using (Logger log = Logger.IniciarLog("Validando CNPJ"))
            {
                if (!IsMatriz(cnpj))
                {
                    log.GravarMensagem("Estabelecimento não autorizado.", new { cnpj });

                    this.ExibirErro(300, "Estabelecimento não autorizado.", string.Empty);

                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Verificar se o Cnpj é de uma matriz
        /// </summary>
        /// <param name="cnpj">CNPJ</param>
        /// <returns></returns>
        private bool IsMatriz(string cnpj)
        {
            if (cnpj != null && cnpj.Length == 14)
            {
                if (Convert.ToInt32(cnpj.Substring(8, 4)) == 1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validar se os dados iniciais estão válidos de acordo com as regras de permissões campos
        /// </summary>
        /// <returns></returns>
        private Boolean ValidarDadosIniciais(String cpfCnpj)
        {
            string fonte = "DadosIniciais.ValidarEntidade";
            int codigo = 1150;
            string titulo = "Atenção";

            EntidadeServico.EntidadeServicoModel[] entidades;
            EntidadeServico.EntidadeServicoModel entidade;
            EntidadeServico.Entidade convertEntidade;
            int codigoRetorno;
            int[] pvsSelecionados;
            int[] pvsValidacaoUsuario = null;

            //Limpa as mensagens de validação dos controles
            emailTexto.InnerText = "Erro";
            emailContainer.Attributes["class"] = "has-feedback";

            confEmailTexto.InnerText = "Erro";
            confEmailContainer.Attributes["class"] = "has-feedback";

            codPessoaTexto.InnerText = "Erro";
            codPessoaContainer.Attributes["class"] = "has-feedback";
            long numCpfCnpj = 0;

            long.TryParse(cpfCnpj, out numCpfCnpj);
            bool tipoPJ = tipoJuridica.Checked;

            Int32 codigoEntidade = 0;

            using (Logger log = Logger.IniciarLog("Validando dados inicias"))
            {
                if (tipoPJ)
                {
                    entidades = this.GetPvs(cpfCnpj, null);
                }
                else
                {
                    entidades = this.GetPvs(null, numCpfCnpj);
                }

                if (entidades != null && entidades.Any())
                {
                    pvsValidacaoUsuario = entidades.Select(x => x.NumeroPV).ToArray();
                }

                log.GravarMensagem("Validando campos");

                // Valida se os campos estão preenchidos
                if (!this.ValidarCampos())
                {
                    return false;
                }

                log.GravarMensagem("verifica se encontrou entidade relacionada ao CPF/CNPJ informado");
                // verifica se encontrou entidade relacionada ao CPF/CNPJ informado
                if (entidades == null || !entidades.Any())
                {
                    log.GravarMensagem("Nenhum estabelecimento encontrado relacionado aos dados informados");

                    this.ExibirErro("Nenhum estabelecimento encontrado relacionado aos dados informados", "Atenção", base.RecuperarEnderecoPortal());
                    return false;
                }

                if (tipoPJ)
                {
                    if (ValidarPvsRelacionadosSaoFiliais(cpfCnpj))
                    {
                        return false;
                    }
                }

                log.GravarMensagem("Validando email");

                //Valida se os campos estão preenchidos
                if (!this.ValidarEmailUsuario(pvsValidacaoUsuario))
                {
                    return false;
                }


                log.GravarMensagem("Validando black list");
                // remove as entidades cujos PVs foram inclusos na BlackList
                BlacklistPVs blacklistPvs = new BlacklistPVs();
                var pvsBloqueados = blacklistPvs.PVsBloqueados;
                entidades = entidades.Where(x => !pvsBloqueados.Contains(x.NumeroPV)).ToArray();

                if (entidades.Count() == 0)
                {
                    log.GravarMensagem("Entidades bloqueadas para a criação de usuário pois estão na black list");

                    // se nenhuma entidade sobrar após o filtro, apresenta erro ao usuário
                    this.ExibirErro("Criação de usuário e senha não permitida. Para mais informações, entre em contato com a central de atendimento", "Atenção", base.RecuperarEnderecoPortal());
                    return false;
                }

                if (IsVisibleGrid() == false)
                {
                    if ((entidades != null && entidades.Count() > 1))
                    {
                        log.GravarMensagem("Tornando o grid de PVs visível");

                        VisibleGrid(true);

                        this.rptListaPvs.DataSource = entidades;
                        this.rptListaPvs.DataBind();

                        return false;
                    }
                    //Caso exista apenas o relacionamento com um PV realiza as validações
                    //Caso exista para mais de um PV não traz os Pvs que possuem usuário e/ou possuem usuário master
                    else if (entidades.Count() == 1)
                    {
                        log.GravarMensagem("Apenas uma entidade possui relacionamento com CPF/CNPJ");

                        entidade = entidades.FirstOrDefault();

                        if (entidade.Status == (Int32)Redecard.PN.Comum.Enumerador.Status.EntidadeBloqueadaConfirmacaoPositiva)
                        {
                            log.GravarMensagem("Entidade bloqueada pela confirmação positiva");

                            this.ExibirErro(@"Solicite o desbloqueio para o usuário Master ou com 
                                  a nossa Central de Atendimento através dos telefones: 
                                 <br /><b>4001 4433 (Capitais e Regiões Metropolitanas)</b>
                                 <br /><b>0800 728 4433 (Demais Regiões)</b>",
                                        "Atenção: Formulário para criação de usuário está bloqueado.",
                                        base.RecuperarEnderecoPortal());

                            return false;
                        }

                        if ((entidade.PossuiMaster.HasValue && entidade.PossuiMaster.Value)
                            && !this.WebPartDadosIniciais.AcessoUsuarioLegado
                            && !this.OrigemSenhaTemporaria())
                        {
                            log.GravarMensagem("Entidade já possui usuario mastar");

                            this.CriarInformacoesSessao(email.Text);
                            this.ExibirAvisoPossuiMaster();
                            return false;
                        }
                        else if ((entidade.PossuiUsuario.HasValue && entidade.PossuiUsuario.Value)
                                && !this.WebPartDadosIniciais.AcessoUsuarioLegado
                                && !this.OrigemSenhaTemporaria())
                        {
                            log.GravarMensagem("Entidade já possui usuario");

                            this.CriarInformacoesSessao(email.Text);
                            this.ExibirAvisoPossuiUsuariosNaoMaster();
                            return false;
                        }
                        else
                        {
                            log.GravarMensagem("Entidade não possui usuário");

                            this.CriarInformacoesSessao(email.Text);
                            var info = InformacaoUsuario.Recuperar();
                            info.EntidadePossuiUsuario = entidade.PossuiUsuario.GetValueOrDefault();
                            info.PvsSelecionados = new[] { entidades[0].NumeroPV };
                            return true;
                        }
                    }
                    else
                    {
                        log.GravarMensagem("Várias entidades relacionadas ao CPF/CNPJ");

                        this.CriarInformacoesSessao(email.Text);
                        return true;
                    }
                }
                else
                {
                    log.GravarMensagem("Validado pvs selecionados");

                    pvsSelecionados = GetPvsSelecionados();

                    if (pvsSelecionados == null || !pvsSelecionados.Any())
                    {
                        log.GravarMensagem("Nenhum pv selecionado");

                        return false;
                    }

                    // grava os dados da sessão
                    this.CriarInformacoesSessao(email.Text);
                }
                return true;
            }
        }

        /// <summary>
        /// Verifica se o Cnpj informado apenas possui filiais
        /// </summary>
        /// <param name="cnpj"></param>
        /// <returns></returns>
        private bool ValidarPvsRelacionadosSaoFiliais(string cnpj)
        {
            bool result;
            int codigoRetorno;

            if (cnpj != null)
            {
                cnpj = NormalizarString(cnpj);
            }

            using (Logger log = Logger.IniciarLog("Verifica se os Pvs relacionados ao CNPJ não todos Filiais"))
            {
                try
                {
                    using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        log.GravarMensagem("Antes chamada servico PvsRelacionadosSaoFiliais", new { parametro = cnpj });

                        result = contextoEntidade.Cliente.PvsRelacionadosSaoFiliais(out codigoRetorno, Convert.ToInt64(cnpj));

                        log.GravarMensagem("Resultado servico PvsRelacionadosSaoFiliais", new { result = result });
                    }

                    if (result)
                    {
                        this.ExibirErro("Atenção Estabelecimento não autorizado a criar usuário. <br /> Para mais informações , entre em contato com a Central de Atendimento.", "Atenção", base.RecuperarEnderecoPortal());
                    }

                    return result;
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty);

                    return true;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);

                    return true;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);

                    return true;
                }
            }
        }


        /// <summary>
        /// Verifica se o CPF é válido, atualmente não existe nenhuma validação especifica para PF, método ficará aqui por questão de organização 
        /// </summary>
        /// <param name="campoCpf"></param>
        /// <returns></returns>
        private bool ValidarProximoPassoPF(long cpf)
        {
            return true;
        }


        /// <summary>
        /// Valida se os campos foram preenchidos corretamente
        /// </summary>
        /// <returns></returns>
        private Boolean ValidarCampos()
        {
            Boolean valido = true;

            if (tipoJuridica.Checked)
            {
                if (string.IsNullOrEmpty(codPessoa.Text))
                {
                    valido = false;
                }

                if (string.IsNullOrEmpty(txtCpfUsuario.Text))
                {
                    valido = false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(codPessoa.Text))
                {
                    valido = false;
                }
            }

            if (string.IsNullOrEmpty(email.Text))
            {
                valido = false;
            }

            if (string.IsNullOrEmpty(confEmail.Text))
            {
                valido = false;
            }

            return valido;
        }


        /// <summary>
        /// Valida se a Entidade não está com o formulário de criação bloqueado
        /// </summary>
        /// <param name="codigoEntidade">Código da Entiadde</param>
        /// <returns>
        /// <para>True - Permitido</para>
        /// <para>False - Formulário bloqueado</para>
        /// </returns>
        private Boolean ValidarPermissaoEntidade(Int32 codigoEntidade)
        {
            Boolean resultado = false;

            using (Logger log = Logger.IniciarLog("Valida se a Entidade não está com o formulário de criação bloqueado"))
            {
                Int32 codigoRetornoIs = 0;
                Int32 codigoRetornoGe = 0;

                using (var contexto = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    EntidadeServico.Entidade[] entidades = contexto.Cliente.Consultar(out codigoRetornoIs, out codigoRetornoGe, codigoEntidade, 1);

                    if (entidades.Length > 0)
                    {
                        EntidadeServico.Entidade entidade = entidades[0];

                        resultado = entidade.StatusPN.Codigo != (Int32)Redecard.PN.Comum.Enumerador.Status.EntidadeBloqueadaConfirmacaoPositiva;
                    }
                    else
                    {
                        resultado = false;
                    }
                }
            }

            return resultado;
        }

        /// <summary>
        /// Valida se a Entidade possui Usuário Legado
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <returns></returns>
        private Boolean PossuiUsuariosLegados(Int32 codigoEntidade)
        {
            Boolean possuiLegado = false;
            Boolean possuiUsuario = false;
            Boolean possuiMaster = false;
            Boolean possuiSenhaTemp = false;
            Boolean migracaoUsuarioSenhaTemporaria = false;

            using (Logger log = Logger.IniciarLog("Valida se a Entidade possui Usuário Legado"))
            {
                using (var contexto = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    possuiLegado = contexto.Cliente.PossuiUsuarioLegado(codigoEntidade, 1);

                    contexto.Cliente.PossuiUsuario(out possuiUsuario, out possuiMaster, out possuiSenhaTemp, codigoEntidade, 1);
                }

                migracaoUsuarioSenhaTemporaria = this.OrigemSenhaTemporaria();

                possuiLegado = this.WebPartDadosIniciais.AcessoUsuarioLegado || migracaoUsuarioSenhaTemporaria || possuiSenhaTemp ? false : possuiLegado;
            }

            return possuiLegado;
        }

        /// <summary>
        /// Origem por usuário com senha temporária
        /// </summary>
        /// <returns></returns>
        private Boolean OrigemSenhaTemporaria()
        {
            Boolean origemSenhaTemporaria = false;
            Int32 codigoRetorno = 0;

            if (!Object.ReferenceEquals(Request.QueryString["dados"], null))
            {
                QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                Int32 idUsuario = queryString["IdUsuario"].ToInt32Null().Value;

                UsuarioServico.Usuario usuario = null;

                if (idUsuario > 0)
                    using (var contexto = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        usuario = contexto.Cliente.ConsultarPorID(out codigoRetorno, idUsuario);

                if (usuario != null)
                    origemSenhaTemporaria = usuario.Legado && !usuario.SenhaTemporaria.Equals(String.Empty);
            }

            return origemSenhaTemporaria;
        }

        /// <summary>
        /// Valida os dados da Entidade preenchidos
        /// </summary>
        /// <returns>Retorna se os dados da Entidade</returns>
        private Boolean ValidarEntidade(Int32 codigoEntidade, String cpfCnpj)
        {
            Int32 codigoRetorno = 0;
            Boolean cpfCnpjValido = false;
            Boolean pvFilial = false;

            using (Logger log = Logger.IniciarLog("Valida os dados da Entidade preenchidos"))
            {
                try
                {
                    using (var contexto = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                    {
                        cpfCnpj = Convert.ToDouble(cpfCnpj).ToString();

                        //Consulta os dados do GE
                        EntidadeServico.Entidade entidade = contexto.Cliente.ConsultarDadosPV(out codigoRetorno, codigoEntidade);
                        if (codigoRetorno == 0)
                        {
                            cpfCnpjValido = entidade.CNPJEntidade.Equals(cpfCnpj);

                            pvFilial = entidade.CodigoMatriz != 0 && entidade.CodigoMatriz != codigoEntidade;

                            if (!cpfCnpjValido)
                            {
                                codPessoaTexto.InnerText = "Dados inválidos";
                                codPessoaContainer.Attributes["class"] = "has-feedback erro";
                            }

                            if (pvFilial)
                            {
                                codPessoaTexto.InnerText = "Para este estabelecimento, solicite a criação de usuário e senha com o Estabelecimento Matriz.";
                                codPessoaContainer.Attributes["class"] = "has-feedback erro";
                            }
                        }
                        else
                        {
                            codPessoaTexto.InnerText = "Estabelecimento não encontrado";
                            codPessoaContainer.Attributes["class"] = "has-feedback erro";
                        }

                        Int32[] pv = new Int32[] { codigoEntidade };
                        codigoRetorno = contexto.Cliente.ValidarCriarEntidade(pv, 1);
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty);
                    return false;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return false;
                }
            }

            return codigoRetorno.Equals(0) && cpfCnpjValido && !pvFilial;
        }

        /// <summary>
        /// Valida o status do e-mail do usuário caso exista na base de dados
        /// </summary>
        /// <param name="codigoEntidade">Código da Entidade</param>
        /// <param name="campo">Campo de E-mail</param>
        /// <returns></returns>
        private Boolean ValidarEmailUsuario(int[] pvsValidacaoUsuario)
        {
            bool result = true;
            ValidarCamposEmail campo = new ValidarCamposEmail(email, emailContainer, emailTexto);

            if (pvsValidacaoUsuario != null && pvsValidacaoUsuario.Count() > 1)
            {
                result = campo.ValidarDominio(true);
            }
            else
            {
                result = campo.Validar(true, pvsValidacaoUsuario, true, this);
            }

            return result;
        }

        /// <summary>
        /// Cria as informações necessárias no cache
        /// </summary>
        /// <param name="email"></param>
        private void CriarInformacoesSessao(String email)
        {
            if (!InformacaoUsuario.Existe())
            {
                InformacaoUsuario.Criar(1, 0, email);
            }

            InformacaoUsuario info = InformacaoUsuario.Recuperar();

            info.EmailUsuario = email;
            info.Empresa = tipoJuridica.Checked;
            info.PodeRecuperarCriarAcesso = false;
            info.CriacaoAcessoLegado = this.WebPartDadosIniciais.AcessoUsuarioLegado || this.OrigemSenhaTemporaria();

            info.TipoUsuario = this.WebPartDadosIniciais.AcessoUsuarioLegado ? info.TipoUsuario : String.Empty;

            // grava CPF do usuário para posterior recuperação
            if (!string.IsNullOrEmpty(txtCpfUsuario.Text) && tipoJuridica.Checked)
            {
                info.CpfUsuario = Convert.ToInt64(NormalizarString(txtCpfUsuario.Text));
            }

            // obtém e grava separadamente o CPF do proprietário / CNPJ do estabelecimento
            string cpfCnpjString = NormalizarString(codPessoa.Text);
            long cpfCnpjLong = 0;
            if (long.TryParse(cpfCnpjString, out cpfCnpjLong))
            {
                if (tipoJuridica.Checked)
                    info.CnpjEstabelecimento = cpfCnpjLong;
                else
                    info.CpfProprietario = cpfCnpjLong;
            }

            // registra PVs selecionados
            info.PvsSelecionados = GetPvsSelecionados();

            //Grava no cache os pvs relacionados
            info.EstabelecimentosRelacinados = GetPvs(info.CnpjEstabelecimento == null ? null : info.CnpjEstabelecimento.ToString(), info.CpfProprietario);

            if (this.OrigemSenhaTemporaria())
            {
                QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                Int32 idUsuario = queryString["IdUsuario"].ToInt32Null().Value;

                info.IdUsuario = idUsuario;
                info.TipoUsuario = "B";
            }

            InformacaoUsuario.Salvar(info);
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
            btnVoltar.Visible = true;

            btnVoltarAviso.Visible = false;
            btnContinuarPasso.Visible = false;

            qdAvisoMigracao.Visible = false;
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
            btnVoltar.Visible = true;

            btnVoltarAviso.Visible = false;
            btnContinuarPasso.Visible = false;

            qdAvisoMigracao.Visible = false;
        }

        /// <summary>
        /// Recupera se o PV é Komerci
        /// </summary>
        /// <param name="codigoEntidade"></param>
        /// <returns></returns>
        private Boolean PVKomerci(Int32 codigoEntidade)
        {
            using (Logger log = Logger.IniciarLog("Veririfcação se o PV é Komerci"))
            {
                try
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
                catch (FaultException<EntidadeServico.GeneralFault> ex)
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

            btnVoltar.Visible = false;

            btnVoltarAviso.Visible = false;
            btnContinuarPasso.Visible = false;
            btnContinuarLegado.Visible = false;

            qdAvisoMigracao.Visible = false;
        }

        /// <summary>
        /// Exibe o quadro de aviso para o PV que já possui usuário Master
        /// </summary>
        private void ExibirAvisoPossuiMaster()
        {
            QdAviso.CarregarMensagem("Atenção: Este estabelecimento já possui usuários cadastrados",
                                    @"Para obter seu acesso, você pode: <br /><br />
                                      <ul><li>Solicitar seu acesso por formulário e aguardar a aprovação de um usuário Master</li>
                                      <li>Entrar em contato com um usuário Master e solicitar a criação do seu usuário</li></ul>",
                                    false, QuadroAvisosResponsivo.IconeMensagem.Aviso);

            pnlAviso.Visible = true;
            pnlDadosIniciais.Visible = false;

            btnVoltar.Visible = false;
            btnVoltarAviso.Visible = true;
            btnContinuarPasso.Visible = true;
            btnContinuarLegado.Visible = false;

            qdAvisoMigracao.Visible = false;
        }

        /// <summary>
        /// Exibe o quadro de aviso para o PV que já possui usuário, porém nenhum Master
        /// </summary>
        private void ExibirAvisoPossuiUsuariosNaoMaster()
        {
            QdAviso.CarregarMensagem("Atenção: Este estabelecimento já possui usuários cadastrados",
                                    @"O estabelecimento selecionado já possui um usuário cadastrado, porém, ainda não ativou seu acesso completo.<br />
                                      Desta forma, não será possível solicitar seu acesso por meio deste formulário.<br />
                                      Entre em contato com o usuário já cadastrado e solicite a liberação do acesso completo.",
                                    true, QuadroAvisosResponsivo.IconeMensagem.Aviso);

            pnlAviso.Visible = true;
            pnlDadosIniciais.Visible = false;

            btnVoltar.Visible = false;
            btnVoltarAviso.Visible = false;
            btnContinuarPasso.Visible = false;
            btnContinuarLegado.Visible = false;

            qdAvisoMigracao.Visible = false;
        }

        /// <summary>
        /// Exibe o quadro de aviso de e-mail reenviado com sucesso para o usuário Master
        /// </summary>
        private void ExibirAvisoEmailReenviado()
        {
            QdAviso.CarregarMensagem("E-mail reenviado com sucesso.",
                                    @"Dentro de instantes o usuário Master receberá o e-mail solicitando a<br/>
                                      aprovação do seu acesso ao Portal Rede.",
                                    false, QuadroAvisosResponsivo.IconeMensagem.Confirmacao);
            pnlAviso.Visible = true;
            pnlDadosIniciais.Visible = false;

            btnVoltar.Visible = false;
            btnVoltarAviso.Visible = true;
            btnContinuarPasso.Visible = false;
            btnContinuarLegado.Visible = false;

            qdAvisoMigracao.Visible = false;
        }

        /// <summary>
        /// Exibe o quadro de aviso de e-mail reenviado com sucesso para o usuário Master
        /// </summary>
        private void ExibirAvisoEmailConfirmacaoReenviado()
        {
            QdAviso.CarregarMensagem("E-mail reenviado com sucesso.",
                                    @"Dentro de instantes você receberá um e-mail de confirmação.<br />
                                    Acesse o link informado no e-mail em até 12h para concluir seu cadastro.",
                                    false, QuadroAvisosResponsivo.IconeMensagem.Confirmacao);
            pnlAviso.Visible = true;
            pnlDadosIniciais.Visible = false;

            btnVoltar.Visible = false;
            btnVoltarAviso.Visible = true;
            btnContinuarPasso.Visible = false;
            btnContinuarLegado.Visible = false;

            qdAvisoMigracao.Visible = false;
        }

        /// <summary>
        /// Exibe o quadro de aviso para o PV que possui usuário legado e não pode acessar a tela de Criação de Usuário
        /// </summary>
        private void ExibirAvisoPossuiUsuarioLegado()
        {
            QdAviso.CarregarMensagem("Atenção: Este estabelecimento já possui usuários cadastrados",
                                    @"Para obter seu acesso, você pode:<br /><br />
                                      <ul>
                                          <li>Entrar em contato com um usuário Master e solicitar a criação do seu usuário</li>
                                          <li>Solicitar a recuperação do seu acesso em &quotEsqueci meu usuário/e-mail&quot ou &quotEsqueci minha senha&quot</li>
                                      <ul>",
                                    false, QuadroAvisosResponsivo.IconeMensagem.Aviso);

            pnlAviso.Visible = true;
            pnlDadosIniciais.Visible = false;

            btnVoltar.Visible = false;
            btnVoltarAviso.Visible = false;
            btnContinuarLegado.Visible = true;
            btnContinuarPasso.Visible = false;

            qdAvisoMigracao.Visible = false;
        }

        /// <summary>
        /// Exibe o quadro de aviso para o usuário legado que está migranda seus dados pela Parte Aberta
        /// </summary>
        private void ExibirAvisoMigracao()
        {
            QdAvisoMigracao.CarregarMensagem("Atenção!",
                                    @"Seu acesso ao Portal Rede mudou! O usuário passa a ser um endereço de e-mail.<br /><br />
                                      Não será possível recuperar o usuário e/ou senha anterior, caso não se lembre, faça um novo cadastro abaixo.",
                                    false, QuadroAvisosResponsivo.IconeMensagem.Aviso);
        }

        /// <summary>
        /// Normaliza uma string, removendo os caracteres especiais dela
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        private String NormalizarString(String original)
        {
            return Regex.Replace(original, @"[^\w]", String.Empty);
        }

        #endregion

        /// <summary>
        /// Gera string para postback
        /// </summary>
        public String GerarPostBackEventReference(String postBackArgument)
        {
            return this.Page.ClientScript.GetPostBackEventReference(this, postBackArgument);
        }

        public virtual void RaisePostBackEvent(String argument)
        {
            Guid hash = Guid.Empty;

            //Reenvio de solicitação
            if (String.Compare(argument, "ReenviarSolicitacaoAprovacao", true) >= 0)
            {
                //Recupera o Código da Entidade que foi validada
                Int32 codigoEntidade = argument.Split('|')[1].ToInt32();
                Boolean sucesso = false;

                //Obtém os dados do usuário do e-mail
                Rede.PN.DadosCadastraisMobile.SharePoint.UsuarioServico.Usuario usuario = ValidarCamposEmail.ConsultarUsuarioPorEmail(email.Text, codigoEntidade, out sucesso, out hash);
                //Consulta os usuários Master da entidade
                EntidadeServico.Usuario[] usuariosMaster = ValidarCamposEmail.ConsultarUsuariosMaster(codigoEntidade);

                if (usuario != null && usuariosMaster != null && usuariosMaster.Length > 0)
                {
                    String[] emails = usuariosMaster.Select(master => master.Email)
                        .Where(it => !String.IsNullOrEmpty(it)).ToArray();

                    //Envia e-mail de aprovação
                    EmailNovoAcesso.EnviarEmailAprovacaoAcesso(String.Join(",", emails), usuario.Descricao, usuario.Email,
                        usuario.CodigoIdUsuario, usuario.TipoUsuario, codigoEntidade, null);

                    txtEmailUsuario_SolicitacaoAprovacaoReenviada();
                }
            }
            // reenvio de solicitação para confirmação do e-mail
            else if (string.Compare(argument, "ReenviarEmailConfirmacao", true) >= 0)
            {
                using (Logger log = Logger.IniciarLog("RaisePostBackEvent opção ReenviarEmailConfirmacao"))
                {
                    // busca usuário na base pelo e-mail informado
                    Boolean sucesso = false;

                    Int32 codigoEntidade = argument.Split('|')[1].ToInt32();

                    Rede.PN.DadosCadastraisMobile.SharePoint.UsuarioServico.Usuario usuario = ValidarCamposEmail.ConsultarUsuarioPorEmail(email.Text, codigoEntidade, out sucesso, out hash);

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

                    try
                    {
                        using (var ctx = new ContextoWCF<UsuarioServico.UsuarioServicoClient>())
                        {
                            log.GravarMensagem("Chamando método ReiniciarHash");

                            int codigoRetorno = 0;
                            var usuarioHash = ctx.Cliente.ReiniciarHash(out codigoRetorno, usuario.CodigoIdUsuario);

                            log.GravarMensagem("Resultado método ReiniciarHash", new { result = usuarioHash });

                            if (usuarioHash != null && usuarioHash.Hash != null)
                            {
                                hash = usuarioHash.Hash;
                            }
                            else
                            {
                                log.GravarMensagem("Hash não retornada... usando a hash existente", new { result = hash });
                            }
                        }
                    }
                    catch (FaultException<EntidadeServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                    }
                    catch (PortalRedecardException ex)
                    {
                        log.GravarErro(ex);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                    }

                    // reenvio de e-mail para 
                    EmailNovoAcesso.EnviarEmailConfirmacaoCadastro(
                        sessaoAtual, email.Text, new List<int>() { codigoEntidade }, usuario.CodigoIdUsuario, hash);

                    // se definido, invoca handler para tratamento pós reenvio do e-mail
                    ExibirAvisoEmailConfirmacaoReenviado();
                }
            }
        }

        /// <summary>
        /// Retorna os pvs relacionados ao Cnpj ou Cpf
        /// </summary>
        /// <param name="cnpj">Cnpj</param>
        /// <param name="cpf">Cpf</param>
        /// <returns></returns>
        private EntidadeServico.EntidadeServicoModel[] GetPvs(string cnpj, long? cpf)
        {
            EntidadeServico.EntidadeServicoModel[] arrayPvs = null;

            using (Logger log = Logger.IniciarLog("Seleciona os Pvs que estão relacionados ao CPF ou E-mail"))
            {
                try
                {
                    arrayPvs = GetPVsCache(cnpj, cpf);
                }
                catch (FaultException<UsuarioServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(ex.Detail.Fonte, ex.Detail.Codigo, "Atenção", String.Empty);
                    return null;
                }
                catch (HttpException ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return null;
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    this.ExibirErro(FONTE, CODIGO_ERRO, "Atenção", String.Empty);
                    return null;
                }
            }

            return arrayPvs;
        }

        /// <summary>
        /// Retorna os PVs relacionados ao 
        /// </summary>
        /// <param name="cnpj"></param>
        /// <param name="cpf"></param>
        /// <returns></returns>
        private EntidadeServico.EntidadeServicoModel[] GetPVsCache(string cnpj, long? cpf)
        {
            int codigoRetorno = 0;
            EntidadeServico.EntidadeServicoModel[] estabelecimentos;
            InformacaoUsuario cache;

            if (cnpj != null)
            {
                cnpj = NormalizarString(cnpj);
            }

            if (InformacaoUsuario.Existe())
            {
                cache = InformacaoUsuario.Recuperar();
                estabelecimentos = cache.EstabelecimentosRelacinados;
            }
            else
            {
                InformacaoUsuario.Criar(0, 0, string.Empty);

                estabelecimentos = new EntidadeServico.EntidadeServicoModel[0];

                using (var contextoEntidade = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    estabelecimentos = contextoEntidade.Cliente.ConsultarEntidadeGeCriarNoPN(out codigoRetorno, cpf, Convert.ToInt64(cnpj));
                }
            }

            if (codigoRetorno > 0)
            {
                throw new Exception("Problemas para consultar pvs");
            }

            cache = InformacaoUsuario.Recuperar();
            cache.EstabelecimentosRelacinados = FiltrarEstabelecimentos(estabelecimentos);

            InformacaoUsuario.Salvar(cache);

            return cache.EstabelecimentosRelacinados;
        }

        /// <summary>
        /// Retorna todos os Pvs que não possuem usuario master, que não possuem usuário e usuarios que não possuem senha temp
        /// </summary>
        /// <param name="entidadeServicoModel"></param>
        /// <returns></returns>
        private EntidadeServico.EntidadeServicoModel[] FiltrarEstabelecimentos(EntidadeServico.EntidadeServicoModel[] entidadeServicoModel)
        {
            if (entidadeServicoModel != null && entidadeServicoModel.Count() == 1)
            {
                return entidadeServicoModel;
            }

            entidadeServicoModel = entidadeServicoModel.Where(x =>
                x.PossuiMaster.GetValueOrDefault(true) == false &&
                x.PossuiUsuario.GetValueOrDefault(true) == false &&
                x.Status != (Int32)Redecard.PN.Comum.Enumerador.Status.EntidadeBloqueadaConfirmacaoPositiva)
                .ToArray();

            return entidadeServicoModel;

        }

    }
}
