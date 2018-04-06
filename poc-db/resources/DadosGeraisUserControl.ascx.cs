using Microsoft.SharePoint;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.Business.Usuario;
using Redecard.PN.DadosCadastrais.SharePoint.EntidadeServico;
using Redecard.PN.DadosCadastrais.SharePoint.MaximoServico;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.DadosCadastrais.SharePoint.WebParts.DadosGerais
{
    /// <summary>
    /// Dados Gerais da Entidade
    /// </summary>
    public partial class DadosGeraisUserControl : UserControlBase
    {
        /// <summary>
        /// Dados Cadastrais da Entidade
        /// </summary>
        private EntidadeServico.DadosGerais Dados
        {
            get { return (EntidadeServico.DadosGerais)ViewState["Dados"]; }
            set { ViewState["Dados"] = value; }
        }

        /// <summary>
        /// Lista de IPs habilitados para o Komerci
        /// </summary>
        private List<String> ListaIPs
        {
            get
            {
                if (!Object.ReferenceEquals(ViewState["ListaIPs"], null))
                    return (List<String>)ViewState["ListaIPs"];
                else
                    return new List<String>();
            }
            set
            {
                ViewState["ListaIPs"] = value;
            }
        }

        /// <summary>
        /// Indica se a página esta sendo editada ou nao
        /// </summary>
        protected Boolean ModoEdicao
        {
            get
            {
                if (Session["informacoescadastraismodoedicao"] != null)
                {
                    return Convert.ToBoolean(Session["informacoescadastraismodoedicao"]);
                }
                return false;
            }
            set
            {
                Session["informacoescadastraismodoedicao"] = value;
            }
        }

        /// <summary>
        /// Inicialização da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                this.Page.MaintainScrollPositionOnPostBack = true;
                if (!Page.IsPostBack)
                {
                    this.ModoEdicao = false;
                    CarregarDados();

                    //Os dados ficam desabilitados para edicao
                    HabilitarQuadroContato(false);
                    HabilitarQuadroUrlBack(false);

                    // O usuario do tipo atendimento tem permissao apenas para visualizar a pagina
                    if (SessaoAtual != null && SessaoAtual.UsuarioAtendimento)
                    {
                        // no modo edicao nao aparece nenhum botao para edicao 
                        this.ModoEdicao = true;
                    }
                }
                phEmailComprovante.Visible = this.PossuiTecnologiaPoynt();
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            try
            {
                AtualizarEdicaoPagina();
                phEmailComprovante.Visible = this.PossuiTecnologiaPoynt();
            }
            catch (FaultException<EntidadeServico.GeneralFault> ex)
            {
                base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void lnkBtnEditarContato_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Editando o quadro de contato"))
            {
                try
                {
                    HabilitarQuadroContato(true);
                }
                catch (ArgumentNullException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        protected void btnContatoCancelarEdicao_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Cancelando edição do quadro de contato"))
            {
                try
                {
                    this.CarregarDados();
                    HabilitarQuadroContato(false);
                }
                catch (ArgumentNullException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
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
        /// Salvar as alterações de Dados Cadastrais
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Salvando alterações de dados cadastrais"))
            {
                try
                {
                    if (this.SalvarDadosContato())
                    {
                        imgModalConfirmacao.ImageUrl = "/_layouts/DadosCadastrais/Images/smile.png";
                        ltrTituloModalConfirmacao.Text = "alteração realizada com sucesso!";

                        String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { dadosGeraisOpenModal('[id$=lbxModalConfirmacao]', true); }, 'SP.UI.Dialog.js');";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AbrirModalDialog", javaScript, true);
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

                HabilitarQuadroContato(false);
            }
        }

        protected void lnkBtnEditarIps_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Editando o quadro de IPs"))
            {
                try
                {
                    HabilitarQuadroUrlBack(true);
                    CarregarGridIPs();
                }
                catch (ArgumentNullException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        protected void btnUrlBackCancelarEdicao_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Cancelando edição do quadro de IPs"))
            {
                try
                {
                    HabilitarQuadroUrlBack(false);
                }
                catch (ArgumentNullException ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        protected void btnUrlBackConfirmar_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Salvando alterações do quadro de ips"))
            {
                HabilitarQuadroUrlBack(false);
                try
                {
                    if (this.SalvarDadosUrlBack())
                    {
                        imgModalConfirmacao.ImageUrl = "/_layouts/DadosCadastrais/Images/smile.png";
                        ltrTituloModalConfirmacao.Text = "alteração realizada com sucesso!";

                        String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { dadosGeraisOpenModal('[id$=lbxModalConfirmacao]', true); }, 'SP.UI.Dialog.js');";
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AbrirModalDialog", javaScript, true);
                    }
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        protected void lnkBtnAdicionarIps_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Adicionando mais uma linha de IPs"))
            {
                try
                {
                    lnkBtnAdicionarIps.Visible = false;
                    this.ListaIPs.Add(String.Empty);
                    CarregarGridIPs();
                }
                catch (FaultException<EntidadeServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
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
        /// Carrega os Dados Cadastrais da Entidade em tela
        /// </summary>
        private void CarregarDados()
        {
            using (Logger log = Logger.IniciarLog("Dados Cadastrais - Carregando página"))
            {
                Int32 codigoRetorno;
                using (var entidadeCliente = new EntidadeServicoClient())
                {
                    this.Dados = entidadeCliente.ConsultarDadosGerais(out codigoRetorno, SessaoAtual.CodigoEntidade);

                    if (codigoRetorno > 0)
                        base.ExibirPainelExcecao("EntidadeServico.ConsultarDadosGerais", codigoRetorno);
                    else
                    {
                        if (this.Dados != null)
                        {
                            lblRazaoSocial.Text = this.Dados.RazaoSocial.Trim().ToUpper();
                            lblNomeFantasia.Text = this.Dados.NomeFantasia.Trim().ToUpper();
                            lblCNPJ.Text = this.Dados.CPNJ.FormatarCnpjCnpj();
                            //[SKU] - 08/2016 - BBF Sprint 8 - Carol solicitou que esse campo fosse removido
                            //lblPlaqueta.Text = this.Dados.Plaqueta.Trim().ToUpper();

                            txtNomePSS.Text = this.Dados.Nome.Trim().ToUpper();
                            txtTelefone1.Text = this.Dados.Telefone.FormatarTelefone(this.Dados.DDD);
                            txtNumeroRamal1.Text = this.Dados.Ramal.TratarValorNulo();

                            txtTelefone2.Text = this.Dados.Telefone2.FormatarTelefone(this.Dados.DDD2);
                            txtNumeroRamal2.Text = this.Dados.Ramal2.TratarValorNulo();

                            txtFax.Text = this.Dados.FAX.FormatarTelefone(this.Dados.DDDFax);

                            txtEmail.Text = this.Dados.Email.Trim();
                            txtEmailComprovante.Text = this.Dados.EmailComprovante.Trim();
                            txtSite.Text = this.Dados.Site.Trim();

                            PreencherDadosProprietario(this.Dados);

                            lblRamo.Text = this.Dados.Ramo.Trim().ToUpper();
                            lblLocalPagamento.Text = this.Dados.LocalPagamento.Trim().ToUpper();
                            lblComercializacao.Text = this.Dados.Comercializacao.Trim().ToUpper();

                        }

                        //Verifica a entidade utiliza o Komerci (Tecnologia 25 ou 26) para habilitar cadastro de IPs
                        CarregarQuadroIps();
                    }
                }
            }
        }

        /// <summary>
        /// Preenche os dados dos Proprietários/Sócios da Entidade
        /// </summary>
        /// <param name="dados"></param>
        private void PreencherDadosProprietario(EntidadeServico.DadosGerais dados)
        {
            using (Logger log = Logger.IniciarLog("Preenchendo dados dos proprietários/sócios da entidade"))
            {
                //Se houver um proprietário cadastrado, exibe
                if (!String.IsNullOrEmpty(dados.Proprietario1))
                {
                    trProprietario1.Visible = true;

                    lblNomeSocio1.Text = dados.Proprietario1.Trim().ToUpper();
                    lblCPF1.Text = dados.CPFProprietario1.FormatarCnpjCnpj();
                    lblDataNascimento1.Text = dados.DataNascimentoProprietario1;
                }
                else
                    trProprietario1.Visible = false;

                //Se houver segundo proprietário cadastrado, exibe
                if (!String.IsNullOrEmpty(dados.Proprietario2))
                {
                    trProprietario2.Visible = true;

                    lblNomeSocio2.Text = dados.Proprietario2.Trim().ToUpper();
                    lblCPF2.Text = dados.CPFProprietario2.FormatarCnpjCnpj();
                    lblDataNascimento2.Text = dados.DataNascimentoProprietario2.Trim().ToUpper();
                }
                else
                    trProprietario2.Visible = false;

                //Se houver terceira proprietário cadastrado, exibe
                if (!String.IsNullOrEmpty(dados.Proprietario3))
                {
                    trProprietario3.Visible = true;

                    lblNomeSocio3.Text = dados.Proprietario3;
                    lblCPF3.Text = dados.CPFProprietario3.FormatarCnpjCnpj();
                    lblDataNascimento3.Text = dados.DataNascimentoProprietario3.Trim().ToUpper();
                }
                else
                {
                    trProprietario3.Visible = false;
                }

                pnlSocios.Visible = !(String.IsNullOrEmpty(dados.Proprietario1) &&
                        String.IsNullOrEmpty(dados.Proprietario2) && String.IsNullOrEmpty(dados.Proprietario3));
            }
        }

        /// <summary>
        /// Verificar se a Entidade possui Komerci
        /// </summary>
        private void CarregarQuadroIps()
        {
            //Se possui tecnologia DataCash ou Adquirencia, desabilita alteração de URLBACK e IPs
            if (this.SessaoAtual.PossuiDataCash || this.SessaoAtual.ServicoEadquirencia)
            {
                pnlURLBack.Visible = false;
            }
            else
            {
                using (Logger log = Logger.IniciarLog("Verificando se entidade possui Komerci"))
                {
                    using (var dadosCliente = new EntidadeServico.EntidadeServicoClient())
                    {
                        Int32 codigoTecnologia;
                        Int32 codigoRetorno;

                        codigoTecnologia = dadosCliente.ConsultarTecnologiaEstabelecimento(out codigoRetorno, SessaoAtual.CodigoEntidade);

                        if (codigoRetorno > 0)
                            base.ExibirPainelExcecao("EntidadeServico.ConsultarTecnologiaEstabelecimento", codigoRetorno);
                        else
                        {
                            //Para Entidade que possuem tecnologias Komerci (codigo 25 ou 26), habilita-se alteração de URLBACK
                            if (codigoTecnologia == 25 || codigoTecnologia == 26)
                            {
                                pnlURLBack.Visible = true;

                                Entidade entidade = new Entidade();
                                entidade.Codigo = SessaoAtual.CodigoEntidade;
                                entidade.GrupoEntidade = new GrupoEntidade();
                                entidade.GrupoEntidade.Codigo = SessaoAtual.GrupoEntidade;

                                //Consultar URLBACK
                                URLBack dadosKomerci = dadosCliente.ConsultarURLBack(out codigoRetorno, entidade);
                                if (codigoRetorno > 0)
                                {
                                    base.ExibirPainelExcecao("EntidadeServico.ConsultarURLBack", codigoRetorno);
                                }
                                else
                                {
                                    txtURLBack.Text = dadosKomerci.URLBackKomerci.Trim();
                                    this.ListaIPs = new List<String>();
                                    foreach (var ip in dadosKomerci.ListaIPs)
                                    {
                                        if (ip.Trim().Length > 0)
                                            this.ListaIPs.Add(ip.Trim());
                                    }
                                    CarregarGridIPs();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Carrega a tabela de IPs habilitados para o Komerci em tela
        /// </summary>
        private void CarregarGridIPs()
        {
            using (Logger log = Logger.IniciarLog("Carregando IPs"))
            {
                // se tiver menos de 5 itens insere 5
                while (this.ListaIPs.Count < 5)
                {
                    this.ListaIPs.Add(String.Empty);
                }

                // se for maior que 5 insere 10 registros
                if (this.ListaIPs.Count > 5)
                {
                    lnkBtnAdicionarIps.Visible = false;
                    while (this.ListaIPs.Count < 10)
                    {
                        this.ListaIPs.Add(String.Empty);
                    }
                }
                else if (this.ModoEdicao)
                {
                    lnkBtnAdicionarIps.Visible = true;
                }

                rptIps.DataSource = this.ListaIPs;
                rptIps.DataBind();
            }
        }

        /// <summary>
        /// Salva dados do cliente
        /// </summary>
        /// <returns>Status da operação salvar TRUE / FALSE</returns>
        private Boolean SalvarDadosContato()
        {
            var dadosCliente = new EntidadeServicoClient();
            Int32 codigoRetorno;
            Boolean resultado = false;

            EntidadeServico.DadosGerais dadoGerais = new EntidadeServico.DadosGerais();

            txtTelefone1.Text = Regex.Replace(txtTelefone1.Text, "[^0-9 ]", "");
            if (String.IsNullOrWhiteSpace(txtTelefone1.Text) || !txtTelefone1.Text.Contains(" "))
            {
                dadoGerais.DDD = String.Empty;
                dadoGerais.Telefone = String.Empty;
            }
            else
            {
                dadoGerais.DDD = txtTelefone1.Text.Split(' ')[0];
                dadoGerais.Telefone = txtTelefone1.Text.Split(' ')[1];
            }

            dadoGerais.Ramal = Regex.Replace(txtNumeroRamal1.Text, "[^0-9 ]", "");
            dadoGerais.Ramal = String.IsNullOrEmpty(txtNumeroRamal1.Text) ? "0" : txtNumeroRamal1.Text;

            txtTelefone2.Text = Regex.Replace(txtTelefone2.Text, "[^0-9 ]", "");
            if (String.IsNullOrWhiteSpace(txtTelefone2.Text) || !txtTelefone2.Text.Contains(" "))
            {
                dadoGerais.DDD2 = String.Empty;
                dadoGerais.Telefone2 = String.Empty;
            }
            else
            {
                dadoGerais.DDD2 = txtTelefone2.Text.Split(' ')[0];
                dadoGerais.Telefone2 = txtTelefone2.Text.Split(' ')[1];
            }

            dadoGerais.Ramal2 = Regex.Replace(txtNumeroRamal2.Text, "[^0-9 ]", "");
            dadoGerais.Ramal2 = String.IsNullOrEmpty(txtNumeroRamal2.Text) ? "0" : txtNumeroRamal2.Text;

            txtFax.Text = Regex.Replace(txtFax.Text, "[^0-9 ]", "");
            if (String.IsNullOrWhiteSpace(txtFax.Text) || !txtFax.Text.Contains(" "))
            {
                dadoGerais.DDDFax = String.Empty;
                dadoGerais.FAX = String.Empty;
            }
            else
            {
                dadoGerais.DDDFax = txtFax.Text.Split(' ')[0];
                dadoGerais.FAX = txtFax.Text.Split(' ')[1];
            }

            dadoGerais.Email = txtEmail.Text;
            dadoGerais.EmailComprovante = txtEmailComprovante.Text;
            dadoGerais.Site = txtSite.Text;
            dadoGerais.Nome = txtNomePSS.Text;

            codigoRetorno = dadosCliente.AtualizarDadosGerais(SessaoAtual.CodigoEntidade, dadoGerais);
            if (codigoRetorno > 0 && codigoRetorno != 60027)
            {
                base.ExibirPainelExcecao("EntidadeServico.AtualizarDadosGerais", codigoRetorno);
            }
            else
            {
                //Armazena no histórico
                Historico.CompararModelos(this.Dados, dadoGerais)
                    .Campo((campo) => campo.DDD, "DDD Telefone 1")
                    .Campo((campo) => campo.Telefone, "Número Telefone 1")
                    .Campo((campo) => campo.Ramal, "Ramal Telefone 1")
                    .Campo((campo) => campo.DDD2, "DDD Telefone 2")
                    .Campo((campo) => campo.Telefone2, "Número Telefone 2")
                    .Campo((campo) => campo.Ramal2, "Ramal Telefone 2")
                    .Campo((campo) => campo.DDDFax, "DDD Fax")
                    .Campo((campo) => campo.FAX, "Número Fax")
                    .Campo((campo) => campo.Nome, "Nome")
                    .Campo((campo) => campo.Email, "E-mail")
                    .Campo((campo) => campo.EmailComprovante, "E-mail Comprovante")
                    .Campo((campo) => campo.Site, "Site")
                    .AlteracaoDadosEstabelecimento(SessaoAtual);

                resultado = true;
            }

            if (resultado)
                this.CarregarDados();

            return resultado;
        }

        /// <summary>
        /// Salva dados do urlback
        /// </summary>
        /// <returns>Status da operação salvar TRUE / FALSE</returns>
        private Boolean SalvarDadosUrlBack()
        {
            var dadosCliente = new EntidadeServicoClient();
            Int32 codigoRetorno;
            Boolean retorno = false;

            //Salvar URLBACK
            EntidadeServico.URLBack urlBack = new URLBack();
            EntidadeServico.URLBack urlBackOriginal = new URLBack();

            Entidade entidade = new Entidade();

            entidade.Codigo = SessaoAtual.CodigoEntidade;
            entidade.GrupoEntidade = new GrupoEntidade();
            entidade.GrupoEntidade.Codigo = SessaoAtual.GrupoEntidade;

            urlBack = dadosCliente.ConsultarURLBack(out codigoRetorno, entidade);
            urlBackOriginal = dadosCliente.ConsultarURLBack(out codigoRetorno, entidade);

            urlBack.ListaIPs = new String[10];

            for (int i = 0; i <= 9; i++)
            {
                urlBack.ListaIPs[i] = String.Empty;
            }

            Int16 contadorLinha = 0;
            foreach (RepeaterItem item in rptIps.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    TextBox txtIp = (TextBox)item.FindControl("txtIp");
                    urlBack.ListaIPs[contadorLinha] = txtIp.Text;

                    contadorLinha++;
                }
            }
            urlBack.URLBackKomerci = txtURLBack.Text;

            codigoRetorno = dadosCliente.AtualizarURLBack(SessaoAtual.CodigoEntidade, urlBack, SessaoAtual.LoginUsuario);
            if (codigoRetorno > 0)
            {
                base.ExibirPainelExcecao("EntidadeServico.AtualizarURLBack", codigoRetorno);
            }
            else
            {
                //Armazena no histórico
                Historico.CompararModelos(urlBack, urlBackOriginal)
                    .Campo((campo) => campo.ListaIPs, "Lista de IPs")
                    .Campo((campo) => campo.URLBackKomerci, "URLBack Komerci")
                    .AlteracaoDadosEstabelecimento(SessaoAtual);
                retorno = true;
            }

            if (retorno)
                this.CarregarDados();

            return retorno;
        }

        /// <summary>
        /// Habilita/desabilita os botoes para que a pagina possa ser editada
        /// </summary>
        private void AtualizarEdicaoPagina()
        {
            lnkBtnEditarContato.Visible = !this.ModoEdicao;
            lnkBtnEditarIps.Visible = !this.ModoEdicao;
            //lnkBtnEditarProprietarios.Visible = !this.ModoEdicao;
        }

        private void HabilitarQuadroContato(bool habilitado)
        {
            this.ModoEdicao = habilitado;
            AtualizarEdicaoPagina();

            //Quadro Contato
            divQuadroContato.Attributes["data-edit-mode"] = habilitado.ToString();
            txtNomePSS.ReadOnly = !habilitado;
            txtFax.ReadOnly = !habilitado;
            txtTelefone1.ReadOnly = !habilitado;
            txtNumeroRamal1.ReadOnly = !habilitado;
            txtSite.ReadOnly = !habilitado;
            txtEmail.ReadOnly = !habilitado;
            txtEmailComprovante.ReadOnly = !habilitado;
            txtTelefone2.ReadOnly = !habilitado;
            txtNumeroRamal2.ReadOnly = !habilitado;

            divAcoesContato.Visible = habilitado;

            if (habilitado)
            {
                txtNumeroRamal1.Text = String.Compare(txtNumeroRamal1.Text, "-") != 0 ? txtNumeroRamal1.Text : "";
                txtNumeroRamal2.Text = String.Compare(txtNumeroRamal2.Text, "-") != 0 ? txtNumeroRamal2.Text : "";
            }
            else
            {
                txtNumeroRamal1.Text = !String.IsNullOrWhiteSpace(txtNumeroRamal1.Text) ? txtNumeroRamal1.Text : "-";
                txtNumeroRamal2.Text = !String.IsNullOrWhiteSpace(txtNumeroRamal2.Text) ? txtNumeroRamal2.Text : "-";
            }
        }

        private void HabilitarQuadroUrlBack(bool habilitado)
        {
            this.ModoEdicao = habilitado;
            AtualizarEdicaoPagina();

            //Quadro UrlBack
            pnlURLBack.Attributes["data-edit-mode"] = habilitado.ToString();
            txtURLBack.ReadOnly = !habilitado;
            lnkBtnAdicionarIps.Visible = habilitado;
            divAcoesUrlBack.Visible = habilitado;
        }

        /// <summary>
        /// Verifica se PV está atrelado à tecnologia Poynt
        /// </summary>
        /// <returns>True/False se está ou não atrelado à tecnologia Poynt</returns>
        private Boolean PossuiTecnologiaPoynt()
        {
            SPList tecnologiasList;
            FiltroTerminal filtro = new FiltroTerminal()
            {
                PontoVenda = SessaoAtual.CodigoEntidade.ToString(),
                Situacao = MaximoServico.TipoTerminalStatus.EMPRODUCAO
            };
            List<TerminalConsulta> retorno = new List<TerminalConsulta>();

            try
            {
                using (MaximoServicoClient client = new MaximoServicoClient())
                {
                    retorno = client.ConsultarTerminal(filtro);
                }

                using (SPSite spSite = SPContext.Current.Site.WebApplication.Sites["sites/fechado"])
                using (SPWeb spWeb = spSite.AllWebs["minhaconta"])
                {
                    tecnologiasList = spWeb.Lists.TryGetList("Tecnologias");
                }

                if (tecnologiasList != null && tecnologiasList.ItemCount > 0)
                {
                    foreach (SPListItem item in tecnologiasList.Items)
                    {
                        if (Convert.ToString(item["Status"]) == "Ativo" && retorno.Exists(s => s.Terminal.TipoEquipamento.Equals(Convert.ToString(item["Tecnologia"]))))
                            return true;
                    }
                }
            }
            catch (FaultException<MaximoServico.GeneralFault> ex)
            {
                if (ex.Detail.Codigo == 691001)
                    return false;
                else
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }

            return false;
        }
    }
}