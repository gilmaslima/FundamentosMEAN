using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Credenciamento.Sharepoint.GECelulas;
using Redecard.PN.Credenciamento.Sharepoint.GERamosAtd;
using Redecard.PN.Credenciamento.Sharepoint.GECanais;
using Redecard.PN.Credenciamento.Sharepoint.GETaxaFiliacao;
using Redecard.PN.Credenciamento.Sharepoint.GEPontoVen;
using Redecard.PN.Credenciamento.Sharepoint.GETipoCom;
using Redecard.PN.Credenciamento.Sharepoint.DRCepServico;
using Redecard.PN.Credenciamento.Sharepoint.WFProposta;
using Redecard.PN.Comum;
using System.ServiceModel;
using Redecard.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Redecard.PN.Credenciamento;
using Microsoft.SharePoint;
using System.Xml;
using Redecard.PN.Credenciamento.Sharepoint.WFEnderecos;
using Redecard.PN.Credenciamento.Sharepoint.Modelo;
using Redecard.PN.Credenciamento.Sharepoint.WFSerasa;
using System.Collections.Generic;
using Redecard.PN.Credenciamento.Sharepoint.WFProprietarios;
using Redecard.PN.Credenciamento.Sharepoint.PNTransicoesServico;
using Redecard.PN.Credenciamento.Sharepoint.GEProprietarios;
using Redecard.PN.Credenciamento.Sharepoint.GEProdutos;
using System.Text.RegularExpressions;
using Redecard.PN.Credenciamento.Sharepoint.GEDomBancario;

namespace Redecard.PN.Credenciamento.Sharepoint.WebParts.DadosIniciais
{
    public partial class DadosIniciaisUserControl : UserControlCredenciamentoBase
    {
        #region [ Propriedades ]

        /// <summary>
        /// Endereço
        /// </summary>
        public Modelo.Endereco Endereco
        {
            get
            {
                if (ViewState["Endereco"] == null)
                    ViewState["Endereco"] = new Modelo.Endereco();

                return (Modelo.Endereco)ViewState["Endereco"];
            }
            set
            {
                ViewState["Endereco"] = value;
            }
        }
        #endregion

        #region [ Eventos de Página ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Page.MaintainScrollPositionOnPostBack = true;
                Page.Title = "Dados Iniciais";

                if (!Page.IsPostBack)
                {
                    ((NavegacaoCredenciamento)NavegacaoCredenciamento1).PassoMax = 1;

                    if (Request.QueryString["NovaProposta"] == "true")
                        Credenciamento = new Modelo.Credenciamento();

                    if (Credenciamento.Fase == null || Credenciamento.Fase < 0)
                        Credenciamento.Fase = 0;

                    CarregarCanais();
                    CarregarCelulas();
                    CarregarRamoAtividade();
                    CarregarCamposPorTipoPessoa();
                    CarregarTipoComercializacao();

                    // Carrega dados da tela
                    rbTipoPessoa.SelectedValue = Credenciamento.TipoPessoa != null ? Credenciamento.TipoPessoa : "J";
                    txtCNPJ.Text = !String.IsNullOrEmpty(Credenciamento.CNPJ) ? Credenciamento.CNPJ : txtCNPJ.Text;
                    txtCPF.Text = !String.IsNullOrEmpty(Credenciamento.CPF) ? Credenciamento.CPF : txtCPF.Text;
                    ddlTipoComercializacao.SelectedValue = !String.IsNullOrEmpty(Credenciamento.TipoComercializacao) ? Credenciamento.TipoComercializacao : ddlTipoComercializacao.SelectedValue;

                    if (!String.IsNullOrEmpty(Credenciamento.CEP))
                    {
                        txtCEP.Text = Credenciamento.CEP;
                        CarregarEndereco();
                    }

                    if (String.Compare(Credenciamento.TipoPessoa, "F") == 0)
                    {
                        txtCNPJ.Visible = false;
                        rfvCNPJ.Enabled = false;
                        rfvCNPJ.Visible = false;
                        cvCNPJ.Enabled = false;
                        cvCNPJ.Visible = false;

                        txtCPF.Visible = true;
                        rfvCPF.Enabled = true;
                        rfvCPF.Visible = true;
                        cvCPF.Enabled = true;
                        cvCPF.Visible = true;

                        pnlPessoaFisica.Visible = true;
                        pnlPessoaJuridica.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Iniciais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ Eventos de Controles ]

        /// <summary>
        /// Evento do botão continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate();
                if (Page.IsValid)
                {
                    // Limpa Sessão para iniciar outro Credenciamento
                    Char? tempExgPartInt = Credenciamento.ExigeParticipacaoIntegral;
                    Credenciamento = new Modelo.Credenciamento();
                    Credenciamento.ExigeParticipacaoIntegral = tempExgPartInt;
                    Credenciamento.CodTipoMovimento = 'I';
                    Credenciamento.Duplicacao = false;
                    Credenciamento.Recredenciamento = false;

                    Credenciamento.CEP = txtCEP.Text;
                    Credenciamento.EnderecoComercial = Endereco;
                    if (Endereco.CEP == null)
                    {
                        Endereco.IndTipoEndereco = '1';
                        Endereco.CEP = Credenciamento.CEP;
                        Endereco.Bairro = String.Empty;
                        Endereco.Cidade = String.Empty;
                        Endereco.Complemento = String.Empty;
                        Endereco.Estado = String.Empty;
                        Endereco.Logradouro = String.Empty;
                        Endereco.Numero = String.Empty;
                    }

                    Credenciamento.Celula = ddlCelula.SelectedValue.ToInt32();
                    Credenciamento.Canal = ddlCanal.SelectedValue.ToInt32();
                    Credenciamento.TipoComercializacao = ddlTipoComercializacao.SelectedValue;
                    Credenciamento.TipoComercializacaoDescricao = ddlTipoComercializacao.SelectedItem.Text;
                    Credenciamento.Fase = 0;

                    if (String.Compare(Credenciamento.TipoComercializacao, "00000") != 0)
                    {
                        Credenciamento.GrupoRamo = Credenciamento.TipoComercializacao.Substring(0, 1).ToInt32();
                        Credenciamento.RamoAtividade = Credenciamento.TipoComercializacao.Substring(1, 4).ToInt32();
                    }

                    Credenciamento.TipoPessoa = rbTipoPessoa.SelectedValue;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        Credenciamento.CNPJ = txtCNPJ.Text;
                    else
                    {
                        Credenciamento.CPF = txtCPF.Text;
                        Credenciamento.GrupoRamo = ddlRamoAtividade.SelectedValue.Substring(0, 1).ToInt32();
                        Credenciamento.RamoAtividade = ddlRamoAtividade.SelectedValue.Substring(1, 4).ToInt32();
                    }

                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        GravaInfoTipoEstabCredenciamento();

                    if (ConsultaQtdePropostasPendentes() || BuscaQtdePVs() > 0)
                    {
                        Response.Redirect("pn_propostasandamento.aspx", false);
                    }
                    else
                    {
                        ConsultaProximaSequencia();
                        CarregarSerasa();

                        if (Credenciamento.CodTipoEstabelecimento == 1)
                        {
                            //Credenciamento.ProdutosCredito = ListaDadosProdutosPorPontoDeVenda((Int32)Credenciamento.NumPdvMatriz, 'C');
                            //Credenciamento.ProdutosDebito = ListaDadosProdutosPorPontoDeVenda((Int32)Credenciamento.NumPdvMatriz, 'D').FindAll(p => p.CodCCA != 22);
                            //Credenciamento.ProdutosConstrucard = ListaDadosProdutosPorPontoDeVenda((Int32)Credenciamento.NumPdvMatriz, 'D').FindAll(p => p.CodCCA == 22);
                            CarregarDadosDomicilioBancarioCreditoPorPontoVenda((Int32)Credenciamento.NumPdvMatriz);

                            CarregarDadosPorPontoVenda((Int32)Credenciamento.NumPdvMatriz);
                            ListaProprietariosPontoVenda((Int32)Credenciamento.NumPdvMatriz);
                            Credenciamento.RecuperadaGE = true;
                        }

                        Int32 retornoGravar = GravarAtualizarPasso1();
                        Logger.GravarLog("Término gravar passo 1");
                        if (retornoGravar == 0)
                        {
                            Logger.GravarLog("Redirect");
                            Response.Redirect("pn_dadoscliente.aspx", false);
                        }
                        else
                            base.ExibirPainelExcecao("Redecard.PN.Credenciamento.Servicos", retornoGravar);
                    }
                }
            }
            catch (FaultException<GEDomBancario.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Iniciais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.CodErro.ToString());
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Iniciais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.CodErro.ToString());
            }
            catch (FaultException<PNTransicoesServico.GeneralFault> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Iniciais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Dados Iniciais", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Iniciais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }

        }

        /// <summary>
        /// Evento que carrega lista de celulas ao selecionar um canal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCanal_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarCelulas();
            CarregarRamoAtividade();
        }

        /// <summary>
        /// Método para alternar entre textbox de CPF e CNPJ de acordo com a tipo de pessoa selecionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rbTipoPessoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarCamposPorTipoPessoa();
        }

        /// <summary>
        /// Evento disparado ao mudar o texto do campo CEP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCEP_TextChanged(object sender, EventArgs e)
        {
            if (Request.Form.Get("__EVENTTARGET") == txtCEP.UniqueID)
                if (!String.IsNullOrEmpty(txtCEP.Text) && txtCEP.Text.Length == 9)
                    CarregarEndereco();
                else
                    lblEndereco.Text = String.Empty;
        }

        /// <summary>
        /// Valida CNPJ
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvCNPJ_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtCNPJ.Text.Replace(".", "").Replace("-", "").Replace("/", "").IsValidCPFCNPJ();
        }

        /// <summary>
        /// Valida CPF
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvCPF_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = txtCPF.Text.Replace(".", "").Replace("-", "").IsValidCPFCNPJ();
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Busca lista de celulas e carrega o dropdown
        /// </summary>
        private void CarregarCelulas()
        {
            ServicoPortalGECelulasClient client = new ServicoPortalGECelulasClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar Células"))
                {
                    Int32 numCanal = ddlCanal.SelectedValue.ToInt32();
                    Int32 numCelula = SessaoAtual.CodigoEntidade;

                    ddlCelula.Items.Clear();
                    ddlCelula.Enabled = ddlCanal.Enabled;

                    CelulasoListaDadosCadastraisPorCanal[] celulas = client.ListaDadosCadastraisPorCanal(numCanal, null, null);
                    client.Close();

                    foreach (CelulasoListaDadosCadastraisPorCanal celula in celulas)
                    {
                        ListItem item = new ListItem(String.Format("{0} - {1}", celula.CodCelula, celula.NomeCelula), celula.CodCelula.ToString());
                        ddlCelula.Items.Add(item);
                    }

                    ddlCelula.SelectedIndex = Credenciamento.Celula != 0 ? ddlCelula.Items.IndexOf(ddlCelula.Items.FindByValue(Credenciamento.Celula.ToString())) : ddlCelula.Items.IndexOf(ddlCelula.Items.FindByValue(numCelula.ToString()));
                }
            }
            catch (FaultException<GECelulas.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de ramos de atividade e carrega o dropdown
        /// </summary>
        private void CarregarRamoAtividade()
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar Ramo de Atividade"))
                {
                    Int32 numCanal = ddlCanal.SelectedValue.ToInt32();
                    Char tipoPessoa = 'F';

                    ddlRamoAtividade.Items.Clear();
                    ListaRamosAtividadesPorCanalTipoPessoa[] ramos = client.ListaRamosAtividadesPorCanalTipoPessoa(tipoPessoa, numCanal, null, null);
                    client.Close();

                    foreach (ListaRamosAtividadesPorCanalTipoPessoa ramo in ramos)
                    {
                        ListItem item = new ListItem(ramo.DescRamoAtividade, String.Format(@"{0}{1:0000}", ramo.CodGrupoRamo, ramo.CodRamoAtivididade));
                        ddlRamoAtividade.Items.Add(item);
                    }
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de Canais e carrega dropdown
        /// </summary>
        private void CarregarCanais()
        {
            ServicoPortalGECanaisClient client = new ServicoPortalGECanaisClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar Tipo de Captura"))
                {
                    ddlCanal.Items.Clear();

                    Int32 numCanal = GetIDCanal();
                    if (numCanal == 1 || numCanal == 4 || numCanal == 26)
                        ddlCanal.Enabled = false;

                    CanaisListaDadosCadastrais[] canais = client.ListaDadosCadastrais(null, null, "=");
                    client.Close();

                    foreach (CanaisListaDadosCadastrais canal in canais)
                    {
                        ListItem item = new ListItem(String.Format("{0} - {1}", canal.CodCanal, canal.NomeCanal), canal.CodCanal.ToString());
                        ddlCanal.Items.Add(item);
                    }

                    ddlCanal.SelectedValue = numCanal.ToString();

                    if (Credenciamento.Canal != 0)
                        ddlCanal.SelectedValue = Credenciamento.Canal.ToString();

                    if (canais.Length > 0)
                        Credenciamento.ExigeParticipacaoIntegral = canais[0].IndExigeParticipacaoIntegral;
                }
            }
            catch (FaultException<GECanais.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Método para alternar entre textbox de CPF e CNPJ de acordo com a tipo de pessoa selecionado
        /// </summary>
        private void CarregarCamposPorTipoPessoa()
        {
            try
            {
                if (String.Compare(rbTipoPessoa.SelectedValue, "F") == 0)
                {
                    txtCNPJ.Visible = false;
                    rfvCNPJ.Enabled = false;
                    rfvCNPJ.Visible = false;
                    cvCNPJ.Enabled = false;
                    cvCNPJ.Visible = false;

                    txtCPF.Visible = true;
                    rfvCPF.Enabled = true;
                    rfvCPF.Visible = true;
                    cvCPF.Enabled = true;
                    cvCPF.Visible = true;

                    pnlPessoaFisica.Visible = true;
                    pnlPessoaJuridica.Visible = false;
                }
                else
                {
                    txtCNPJ.Visible = true;
                    rfvCNPJ.Enabled = true;
                    rfvCNPJ.Visible = true;
                    cvCNPJ.Enabled = true;
                    cvCNPJ.Visible = true;

                    txtCPF.Visible = false;
                    rfvCPF.Enabled = false;
                    rfvCPF.Visible = false;
                    cvCPF.Enabled = false;
                    cvCPF.Visible = false;

                    pnlPessoaFisica.Visible = false;
                    pnlPessoaJuridica.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Iniciais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca o logradouro de um determinado CEP e carrega o controle de endereço
        /// </summary>
        private void CarregarEndereco()
        {
            DRCepServicoClient client = new DRCepServicoClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar Endereço"))
                {
                    String cep = txtCEP.Text.Replace("-", "");
                    String endereco = String.Empty;
                    String bairro = String.Empty;
                    String cidade = String.Empty;
                    String uf = String.Empty;

                    Int32 codRetorno = client.BuscaLogradouro(cep, ref endereco, ref bairro, ref cidade, ref uf);
                    client.Close();

                    Endereco.IndTipoEndereco = '1';
                    Endereco.CEP = txtCEP.Text;
                    Endereco.Logradouro = endereco;
                    Endereco.Bairro = bairro;
                    Endereco.Cidade = cidade;
                    Endereco.Estado = uf;

                    if (codRetorno == 38)
                        lblEndereco.Text = String.Format("{0}, {1}, {2}, {3}", endereco, bairro, cidade, uf);
                    else if (codRetorno == 1)
                        lblEndereco.Text = String.Format("{0}, {1}", cidade, uf);
                    else
                        lblEndereco.Text = String.Empty;
                }
            }
            catch (FaultException<DRCepServico.GeneralFault> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", fe);
                SharePointUlsLog.LogErro(fe);
                //base.ExibirPainelExcecao(fe.Message, fe.Detail.Codigo.ToString());
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de tipos de comercialização e carrega drop down
        /// </summary>
        private void CarregarTipoComercializacao()
        {
            ServicoPortalGETipoComercializacaoClient client = new ServicoPortalGETipoComercializacaoClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar Tipo de Comercialização"))
                {
                    ListaDadosCadastrais[] comercializacoes = client.ListaDadosCadastrais('A');
                    client.Close();

                    ddlTipoComercializacao.Items.Clear();
                    foreach (ListaDadosCadastrais comercializacao in comercializacoes)
                    {
                        ListItem item = new ListItem(comercializacao.DescTipoComercializacao, String.Format(@"{0}{1:0000}", comercializacao.CodGrupoRamo, comercializacao.CodRamoAtividade));
                        ddlTipoComercializacao.Items.Add(item);
                    }
                }
            }
            catch (FaultException<GETipoCom.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Verifica se já existe o PV Cadastrado
        /// </summary>
        private Boolean VerificaExistenciaPontoVenda()
        {
            ServicoPortalGEPontoVendaClient client = new ServicoPortalGEPontoVendaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Verifica existência do ponto de venda"))
                {
                    Int64 cnpj = 0;
                    Int64.TryParse(txtCNPJ.Text.Replace(".", "").Replace("/", "").Replace("-", ""), out cnpj);

                    PontoVendaListaCadastroReduzidoPorCNPJ[] pontosVenda = client.ListaCadastroReduzidoPorCNPJ('J', cnpj);
                    client.Close();

                    if (pontosVenda.Length > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Busca ID do Canal de acordo com a Entidade logada no portal
        /// </summary>
        /// <returns></returns>
        private Int32 GetIDCanal()
        {
            SPList list = SPContext.Current.Web.Lists.TryGetList("EntidadeCanal");
            SPQuery query = new SPQuery();
            query.Query = String.Format("<Where><Eq><FieldRef Name='IDGrupoEntidade' /><Value Type='Text'>{0}</Value></Eq></Where>", SessaoAtual.GrupoEntidade);
            query.RowLimit = 1;

            SPListItemCollection items = list.GetItems(query);
            if (items.Count > 0)
                return items[0]["IDCanal"].ToString().ToInt32(4);

            return 4;

        }

        /// <summary>
        /// Consulta se existe propostas pendentes para dado CPF/CNPJ
        /// </summary>
        /// <returns></returns>
        private Boolean ConsultaQtdePropostasPendentes()
        {
            ServicoPortalWFPropostaClient client = new ServicoPortalWFPropostaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Consulta Quantidades de Propostas Pendentes"))
                {
                    Char codTipoPessoa = rbTipoPessoa.SelectedValue.ToCharArray()[0];
                    Int64 numCNPJ = 0;
                    if (codTipoPessoa == 'J')
                        Int64.TryParse(txtCNPJ.Text.Replace(".", "").Replace("-", "").Replace("/", ""), out numCNPJ);
                    else
                        Int64.TryParse(txtCPF.Text.Replace(".", "").Replace("-", ""), out numCNPJ);

                    PropostasPendentes[] propostas = client.ConsultaQtdePropostasPendentes(codTipoPessoa, numCNPJ);
                    client.Close();

                    if (propostas.Length > 0 && (propostas[0].QtdePropostasPendentes > 0 || propostas[0].QtdePVsAtivos > 0))
                    {
                        Credenciamento.QtdePVsAtivos = propostas[0].QtdePVsAtivos;

                        //if (Credenciamento.CodTipoEstabelecimento == 1)
                        //    return false;

                        return true;
                    }
                    return false;
                }
            }
            catch (FaultException<WFProposta.ModelosErroServicos>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Busca Informações do Tipo de Estabelecimento e grava na sessão
        /// </summary>
        private void GravaInfoTipoEstabCredenciamento()
        {
            ServicoPortalGEPontoVendaClient client = new ServicoPortalGEPontoVendaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Consulta Tipo de Estabelecimento Credenciamento"))
                {
                    Int64 numCNPJ = 0;
                    if (String.Compare(rbTipoPessoa.SelectedValue, "J") == 0)
                        Int64.TryParse(txtCNPJ.Text.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                    else
                        Int64.TryParse(txtCPF.Text.Replace(".", "").Replace("-", ""), out numCNPJ);

                    PontoVendaConsultaTipoEstabCredenciamento[] pontoVenda = client.ConsultaTipoEstabCredenciamento(numCNPJ, null);
                    client.Close();

                    if (pontoVenda.Length > 0)
                    {
                        Credenciamento.CodTipoEstabelecimento = pontoVenda[0].CodTipoEstabelecimento;
                        Credenciamento.CodTipoEstabMatriz = pontoVenda[0].CodTipoEstabMatriz;
                        Credenciamento.NumPdv = 0;
                        Credenciamento.NumPdvMatriz = pontoVenda[0].NumPdvMatriz;
                    }
                }
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Consulta o número de sequência
        /// </summary>
        private void ConsultaProximaSequencia()
        {
            ServicoPortalWFPropostaClient client = new ServicoPortalWFPropostaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Consulta Próxima Sequência"))
                {
                    Char tipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJCPF = 0;
                    if (tipoPessoa == 'J')
                        Int64.TryParse(txtCNPJ.Text.Replace(".", "").Replace("-", "").Replace("/", ""), out numCNPJCPF);
                    else
                        Int64.TryParse(txtCPF.Text.Replace(".", "").Replace("-", ""), out numCNPJCPF);

                    Logger.GravarLog("Chamada ao Serviço ConsultaProximaSequencia");
                    Logger.GravarLog(String.Format("numCNPJCPF: {0}", numCNPJCPF));
                    ConsultaProximaSequencia[] proximaSequencia = client.ConsultaProximaSequencia(tipoPessoa, numCNPJCPF);

                    Logger.GravarLog(String.Format("Lenght: {0}", proximaSequencia.Length));
                    if (proximaSequencia.Length > 0)
                    {
                        Logger.GravarLog(String.Format("NumSequencia: {0}", proximaSequencia[0].NumSequencia));
                        Credenciamento.NumSequencia = proximaSequencia[0].NumSequencia;
                    }
                }
            }
            catch (FaultException<WFProposta.ModelosErroServicos>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Grava ou atualiza dados da primeira tela
        /// </summary>
        /// <returns></returns>
        private Int32 GravarAtualizarPasso1()
        {
            TransicoesClient client = new TransicoesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Gravar Dados - Dados Iniciais"))
                {
                    PNTransicoesServico.Proposta proposta = PreencheProposta();
                    PNTransicoesServico.Endereco endereco = PreencheEndereco(Endereco);
                    List<PNTransicoesServico.Proprietario> proprietarios = PreencheListaProprietarios();
                    PNTransicoesServico.Endereco enderecoCorrespondencia = null;
                    PNTransicoesServico.DomicilioBancario domCredito = null;
                    if (Credenciamento.CodTipoEstabelecimento == 1)
                    {
                        domCredito = PreencheDomicilioBancario(1, Credenciamento.CodBancoCredito,
                            Credenciamento.NomeBancoCredito, Credenciamento.AgenciaCredito, Credenciamento.ContaCredito);
                        enderecoCorrespondencia = PreencheEndereco(Credenciamento.EnderecoCorrespondencia);
                    }

                    Int32 retorno = client.GravarAtualizarPasso1(proposta, endereco, enderecoCorrespondencia, domCredito, proprietarios);
                    client.Close();

                    return retorno;
                }
            }
            catch (FaultException<PNTransicoesServico.GeneralFault>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Consulta dados do Serasa para pessoas físca ou jurídica
        /// </summary>
        private void CarregarSerasa()
        {
            SerasaServicoClient client = new SerasaServicoClient();

            try
            {
                if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                {
                    using (Logger log = Logger.IniciarLog("Carregar Dados do Serasa PJ"))
                    {
                        String cnpj = Credenciamento.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "");
                        PJ dadosPJ = client.ConsultaSerasaPJ(cnpj);
                        client.Close();

                        Credenciamento.RetornoSerasa = dadosPJ.CodRetorno;

                        if (String.Compare(dadosPJ.CodRetorno, "00") == 0)
                        {
                            if (!String.IsNullOrEmpty(dadosPJ.ComplGrafia))
                            {
                                Credenciamento.RazaoSocial = dadosPJ.ComplGrafia;

                            }
                            if (!String.IsNullOrEmpty(dadosPJ.DataFundacao) && String.Compare(dadosPJ.DataFundacao, "00000000") != 0)
                            {
                                Credenciamento.DataFundacao = dadosPJ.DataFundacao.ToDate("yyyyMMdd");
                            }
                            else
                                Credenciamento.DataFundacao = DateTime.MinValue;

                            if (!String.IsNullOrEmpty(dadosPJ.CNAEs[0].CodCNAE))
                            {
                                Credenciamento.CNAE = dadosPJ.CNAEs[0].CodCNAE;
                            }
                            if (dadosPJ.Socios.Length > 0)
                            {
                                if (Credenciamento.Proprietarios == null)
                                {
                                    Credenciamento.Proprietarios = new List<Modelo.Proprietario>();
                                    foreach (Socio socio in dadosPJ.Socios)
                                    {
                                        Credenciamento.Proprietarios.Add(new Modelo.Proprietario()
                                        {
                                            CPF_CNPJ = socio.CPF_CNPJ,
                                            Nome = socio.Nome,
                                            Participacao = socio.Participacao,
                                            TipoPessoa = socio.TipoPessoa,
                                            Relato = String.Empty
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
                //else
                //{
                //    using (Logger log = Logger.IniciarLog("Carregar Dados do Serasa PF"))
                //    {
                //        String cpf = Credenciamento.CPF.Replace(".", "").Replace("-", "");
                //        PF dadosPF = client.ConsultaSerasaPF(cpf);
                //        client.Close();

                //        if (String.Compare(dadosPF.CodRetorno, "00") == 0)
                //        {
                            
                //        }
                //        else
                //        {
                //            base.ExibirPainelExcecao("Não foi retornado dados do SERASA para esse CPF", "300");
                //        }
                //    }
                //}
            }
            catch (FaultException<WFSerasa.GeneralFault> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Iniciais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Cliente", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega dados da proposta por ponto de venda
        /// </summary>
        /// <param name="proposta"></param>
        private void CarregarDadosPorPontoVenda(Int32 numPdv)
        {
            ServicoPortalGEPontoVendaClient client = new ServicoPortalGEPontoVendaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados por ponto de venda"))
                {
                    ListaCadastroPorPontoVenda[] dados = client.ListaCadastroPorPontoVenda(numPdv);
                    client.Close();

                    if (dados.Length > 0)
                    {
                        //Credenciamento.CEP = String.Format("{0}-{1}", dados[0].CodCEP, dados[0].CodCompCEP);
                        //Credenciamento.EnderecoComercial.CEP = String.Format("{0}-{1}", dados[0].CodCEP, dados[0].CodCompCEP);
                        Credenciamento.EnderecoCorrespondencia.CEP = String.Format("{0}-{1}", dados[0].CodCEPCorrespondencia, dados[0].CodCompCEPCorrespondencia);
                        //Credenciamento.EnderecoInstalacao.CEP = String.Format("{0}-{1}", dados[0].CodCEPTecnologia, dados[0].CodCompCEPTecnologia);
                        //if (dados[0].CodCanal != null)
                        //    Credenciamento.Canal = (Int32)dados[0].CodCanal;
                        //if (dados[0].CodCelula != null)
                        //    Credenciamento.Celula = (Int32)dados[0].CodCelula;
                        Credenciamento.CodFilial = dados[0].CodFilial;
                        if (dados[0].CodGrupoRamo != null)
                            Credenciamento.GrupoRamo = (Int32)dados[0].CodGrupoRamo;
                        Credenciamento.HorarioFuncionamento = dados[0].CodHorarioFuncionamento;
                        if (dados[0].CodRamoAtivididade != null)
                            Credenciamento.RamoAtividade = (Int32)dados[0].CodRamoAtivididade;
                        //Credenciamento.CodTipoEstabelecimento = dados[0].CodTipoEstabelecimento;
                        //Credenciamento.TipoPessoa = dados[0].CodTipoPessoa.ToString();
                        Credenciamento.CodZonaVenda = dados[0].CodZona;
                        //Credenciamento.EnderecoComercial.Complemento = dados[0].CompEndereco;
                        Credenciamento.EnderecoCorrespondencia.Complemento = dados[0].CompEnderecoCorrespondencia;
                        //Credenciamento.EnderecoInstalacao.Complemento = dados[0].CompEnderecoTecnologia;
                        //if (dados[0].DataAssinaturaProposta != null)
                        //    Credenciamento.DataCadastroProposta = (DateTime)dados[0].DataAssinaturaProposta;

                        if (dados[0].DataFundacao != null)
                            if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                                Credenciamento.DataFundacao = (DateTime)dados[0].DataFundacao;
                            else
                                Credenciamento.DataNascimento = (DateTime)dados[0].DataFundacao;

                        //Credenciamento.DescricaoCanal = dados[0].DescCanal;
                        //Credenciamento.DescricaoCelula = dados[0].DescCelula;
                        Credenciamento.DescricaoRamoAtividade = dados[0].DescRamoAtividade;
                        Credenciamento.NomeEmail = dados[0].Email;
                        Credenciamento.NomeHomePage = dados[0].HomePage;
                        Credenciamento.IndExtratoEmail = dados[0].IndEnvioExtratoEmail;
                        //Credenciamento.EnderecoComercial.Bairro = dados[0].NomeBairro;
                        Credenciamento.EnderecoCorrespondencia.IndTipoEndereco = '2';
                        Credenciamento.EnderecoCorrespondencia.Bairro = dados[0].NomeBairroCorrespondencia;
                        //Credenciamento.EnderecoInstalacao.Bairro = dados[0].NomeBairroTecnologia;
                        //Credenciamento.EnderecoComercial.Cidade = dados[0].NomeCidade;
                        Credenciamento.EnderecoCorrespondencia.Cidade = dados[0].NomeCidadeCorrespondencia;
                        //Credenciamento.EnderecoInstalacao.Cidade = dados[0].NomeCidadeTecnologia;
                        Credenciamento.NomeFatura = dados[0].NomeFatura;
                        Credenciamento.NomeFilial = dados[0].NomeFilial;
                        //Credenciamento.EnderecoComercial.Logradouro = dados[0].NomeLogradouro;
                        Credenciamento.EnderecoCorrespondencia.Logradouro = dados[0].NomeLogradouroCorrespondencia;
                        //Credenciamento.EnderecoInstalacao.Logradouro = dados[0].NomeLogradouroTecnologia;
                        //Credenciamento.EnderecoComercial.Estado = dados[0].NomeUF;
                        Credenciamento.EnderecoCorrespondencia.Estado = dados[0].NomeUFCorrespondencia;
                        //Credenciamento.EnderecoInstalacao.Estado = dados[0].NomeUFTecnologia;
                        Credenciamento.NomeZonaVenda = dados[0].NomeZona;

                        //Credenciamento.CPFVendedor = dados[0].NumCPFVendedor;
                        Credenciamento.Centralizadora = dados[0].NumCentralizadora;
                        //Credenciamento.NumDDD1 = dados[0].NumDDD1;
                        Credenciamento.NumDDD2 = dados[0].NumDDD2;
                        Credenciamento.NumDDDFax = dados[0].NumDDDFax;
                        //Credenciamento.NumDDDInstalacao = dados[0].NumDDDCV;
                        Credenciamento.GrupoComercial = dados[0].NumGrupoComercial;
                        Credenciamento.GrupoGerencial = dados[0].NumGrupoGerencial;
                        //Credenciamento.EnderecoComercial.Numero = dados[0].NumLogradouro;
                        Credenciamento.EnderecoCorrespondencia.Numero = dados[0].NumLogradouroCorrespondencia;
                        //Credenciamento.EnderecoInstalacao.Numero = dados[0].NumLogradouroTecnologia;
                        //Credenciamento.NumPdv = dados[0].NumPdv;
                        //Credenciamento.Ramal1 = dados[0].NumRamal1;
                        Credenciamento.Ramal2 = dados[0].NumRamal2;
                        //if (dados[0].NumRamalCV != null)
                        //    Credenciamento.RamalInstalacao = (Int32)dados[0].NumRamalCV;
                        //Credenciamento.NumTelefone1 = dados[0].NumTelefone1;
                        Credenciamento.NumTelefone2 = dados[0].NumTelefone2;
                        //if (dados[0].NumTelefoneCV != null)
                        //    Credenciamento.NumTelefoneInstalacao = (Int32)dados[0].NumTelefoneCV;
                        Credenciamento.NumTelefoneFax = dados[0].NumTelefoneFax;
                        Credenciamento.NumPdvMatriz = dados[0].NumeroMatriz;
                        Credenciamento.PessoaContato = dados[0].PessoaContato;
                        if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                            Credenciamento.RazaoSocial = dados[0].RazaoSocial;
                        else
                            Credenciamento.NomeCompleto = dados[0].RazaoSocial;

                        //if (dados[0].ValorTaxaAdesao != null)
                        //    Credenciamento.TaxaAdesao = (Double)dados[0].ValorTaxaAdesao;
                    }
                }
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Busca Quantidade de PVs
        /// </summary>
        private Int32 BuscaQtdePVs()
        {
            ServicoPortalGEPontoVendaClient client = new ServicoPortalGEPontoVendaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega lista de PVs"))
                {
                    Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJ = 0;
                    if (Credenciamento.TipoPessoa == "J")
                        Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                    PontoVendaListaCadastroReduzidoPorCNPJ[] pontosVenda = client.ListaCadastroReduzidoPorCNPJ(codTipoPessoa, numCNPJ);
                    client.Close();

                    return pontosVenda.Length;
                }
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Lista proprietários de um ponto de venda
        /// </summary>
        /// <param name="proposta"></param>
        private void ListaProprietariosPontoVenda(Int32 numPdv)
        {
            ServicoPortalGEProprietariosClient client = new ServicoPortalGEProprietariosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Lista Proprietários por ponto de venda"))
                {
                    ProprietarioListaDadosPorPontoVenda[] proprietarios = client.ListaDadosPorPontoVenda(numPdv);
                    client.Close();

                    Credenciamento.Proprietarios = new List<Modelo.Proprietario>();

                    foreach (ProprietarioListaDadosPorPontoVenda proprietario in proprietarios)
                    {
                        Credenciamento.Proprietarios.Add(new Modelo.Proprietario
                        {
                            CPF_CNPJ = proprietario.NumCNPJCPFProprietario.ToString(),
                            Nome = proprietario.NomeProprietario,
                            Participacao = proprietario.ValorParticipacaoSocietaria.ToString(),
                            TipoPessoa = proprietario.CodTipoPessoaProprietario.ToString()
                        });
                    }
                }
            }
            catch (FaultException<GEProprietarios.ModelosErroServicos>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Lista dados produtos por ponto de venda
        /// </summary>
        private List<ProdutosListaDadosProdutosPorRamoCanal> ListaDadosProdutosPorPontoDeVenda(Int32 numPdv, Char? tipoOperacao)
        {
            ServicoPortalGEProdutosClient client = new ServicoPortalGEProdutosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Lista Produtos por ponto de venda"))
                {
                    List<ProdutosListaDadosProdutosPorRamoCanal> retorno = new List<ProdutosListaDadosProdutosPorRamoCanal>();

                    ProdutosListaDadosProdutosPorPontoVenda[] produtos = client.ListaDadosProdutoscomRegimePorPontoVenda(numPdv, tipoOperacao);
                    client.Close();

                    foreach (ProdutosListaDadosProdutosPorPontoVenda produto in produtos)
                    {
                        if (produto.CodCCA == 1 && produto.CodFeature == 1)
                        {
                            Credenciamento.AgenciaCredito = (Int32)produto.CodAgenciaDomicilio;
                            Credenciamento.CodBancoCredito = (Int32)produto.CodBancoDomicilio;
                            Credenciamento.ContaCredito = produto.NumContaCorrenteDomicilio;
                            Credenciamento.NomeBancoCredito = String.Empty;
                        }

                        if (produto.CodCCA == 5)
                        {
                            Credenciamento.AgenciaDebito = (Int32)produto.CodAgenciaDomicilio;
                            Credenciamento.CodBancoDebito = (Int32)produto.CodBancoDomicilio;
                            Credenciamento.ContaDebito = produto.NumContaCorrenteDomicilio;
                        }

                        if (produto.CodCCA == 22)
                        {
                            Credenciamento.AgenciaConstrucard = (Int32)produto.CodAgenciaDomicilio;
                            Credenciamento.CodBancoConstrucard = (Int32)produto.CodBancoDomicilio;
                            Credenciamento.ContaConstrucard = produto.NumContaCorrenteDomicilio;
                        }

                        if (produto.CodFeature == 3)
                        {
                            if (Credenciamento.Patamares == null)
                                Credenciamento.Patamares = new List<Modelo.Patamar>();

                            Credenciamento.Patamares.Add(new Modelo.Patamar
                            {
                                CodCca = (Int32)produto.CodCCA,
                                CodFeature = (Int32)produto.CodFeature,
                                PatamarInicial = produto.PatamarInicioNovo,
                                PatamarFinal = produto.PatamarFimNovo,
                                SequenciaPatamar = produto.NumSequenciaPatamar != null ? (Int32)produto.NumSequenciaPatamar : 1,
                                TaxaPatamar = produto.ValorTaxaRegime,
                                Prazo = (Int32)produto.PrazoRegime
                            });
                        }

                        ProdutosListaDadosProdutosPorRamoCanal p = new ProdutosListaDadosProdutosPorRamoCanal();
                        p.CodCCA = produto.CodCCA;
                        p.CodFeature = produto.CodFeature;
                        p.IndFormaPagamento = produto.IndUtilizacaoTarifa == 'N' ? 'X' : 'T';
                        p.ValorPrazoDefault = produto.PrazoRegime;
                        p.ValorTaxaDefault = produto.ValorTaxaRegime;
                        p.CodTipoNegocio = produto.CodTipoTransacaoOperacao;
                        p.QtdeMaximaParcela = produto.QtdeLimiteParcelaNovo;
                        p.NomeFeature = produto.NomeFeature;
                        p.NomeCCA = produto.NomeCCA;

                        retorno.Add(p);
                    }

                    return retorno;
                }
            }
            catch (FaultException<GEProdutos.ModelosErroServicos>)
            {
                client.Abort();
                throw;
            }
            catch (TimeoutException)
            {
                client.Abort();
                throw;
            }
            catch (CommunicationException)
            {
                client.Abort();
                throw;
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Carrega dados domicílio bancário crédito da matriz
        /// </summary>
        /// <param name="proposta"></param>
        private void CarregarDadosDomicilioBancarioCreditoPorPontoVenda(Int32 numPontoVenda)
        {
            ServicoPortalGEDomicilioBancarioClient client = new ServicoPortalGEDomicilioBancarioClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados domicílio bancário crédito da matriz"))
                {
                    Char? codTipoOperacao = 'C';
                    String siglaProduto = "CR";

                    DomBancariosPorPVOperProd[] dados = client.ConsultaDomBancariosPorPVOperProd(numPontoVenda, codTipoOperacao, siglaProduto);
                    client.Close();

                    if (dados[0].CodigoErro == 0)
                    {
                        Credenciamento.CodBancoCredito = (Int32)dados[0].CodBancoAtual;
                        Credenciamento.AgenciaCredito = (Int32)dados[0].CodAgenciaAtual;
                        Credenciamento.ContaCredito = dados[0].NumeroContaAtual;
                        Credenciamento.NomeBancoCredito = dados[0].NomeBancoAtual;
                        Logger.GravarLog("1 " + dados[0].NomeBancoAtual);
                        Logger.GravarLog("2 " + Credenciamento.NomeBancoCredito);
                    }
                }
            }
            catch (FaultException<GEDomBancario.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        #endregion
    }
}
