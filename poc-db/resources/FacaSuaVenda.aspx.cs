using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using Redecard.PN.DataCash.Modelo.Util;
using Redecard.PN.DataCash.BasePage;
using System.Globalization;
using Redecard.PN.DataCash.controles;
using Redecard.PN.Comum;
using Redecard.PN.DataCash.Modelo;

namespace Redecard.PN.DataCash
{
    public partial class FacaSuaVenda : PageBaseDataCash
    {
        #region [Propriedades]

        private Boolean _formaPagamentoSelecionada = false;
        public Boolean formaPagamentoSelecionada
        {
            get
            {
                return _formaPagamentoSelecionada;
            }
            set
            {
                _formaPagamentoSelecionada = value;
            }
        }

        private String _formaPagamento = Modelos.EFormaPagamento.Avista.GetTitle();
        public String FormaPagamento
        {
            get
            {
                return _formaPagamento;
            }
            set
            {
                _formaPagamento = value;
            }
        }
        #endregion

        public List<Modelo.Passageiro> ListaPassageiros
        {
            get
            {
                if (ViewState["LstPassageiros"] == null)
                    ViewState["LstPassageiros"] = new List<Modelo.Passageiro>();

                return ViewState["LstPassageiros"] as List<Modelo.Passageiro>;
            }
            set { ViewState["LstPassageiros"] = value; }
        }

        public Boolean PVDistribuidor
        {
            get
            {
                if (ViewState["PVDistribuidor"] == null)
                    ViewState["PVDistribuidor"] = false;

                return Convert.ToBoolean(ViewState["PVDistribuidor"]);
            }
            set { ViewState["PVDistribuidor"] = value; }
        }

        /// <summary>
        /// Carregamento da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Load - Faça sua Venda"))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        Int32 pv = this.SessaoAtual.CodigoEntidade;

                        this.DefineDiaValidadeCartao();
                        this.DefineAnoValidadeCartao();
                        this.DefineParcelasDinamicas(pv);
                        this.DefineQuantidadeRecorrencia();
                        this.DefinirGrupoFornecedor(pv);
                        this.DefinirAnaliseRisco(pv);
                        this.DefinirBandeirasAdicionais(pv);

                        // Utilizado pelo botão voltar e limpar
                        if (Session["FacaSuaVenda"] != null)
                        {
                            formaPagamentoSelecionada = true;
                            this.CarregaDados();
                        }
                        else
                            this.FormaPagamento = Modelos.EFormaPagamento.Avista.GetTitle();
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        private void CarregaDados()
        {
            Modelo.Venda venda = (Modelo.Venda)Session["FacaSuaVenda"];

            if (venda.PVDistribuidor)
            {
                ddlGrupoFornecedor.SelectedValue = venda.GrupoFornecedor.ToString();
                ddlFornecedor.SelectedValue = venda.PVFornecedor.ToString();
            }

            ddlTipoTransacao.SelectedValue = venda.TipoTransacao.GetTitle();

            switch (venda.TipoTransacao.GetTitle())
            {
                case "Crédito":
                    CarregaDadosCredito(venda);
                    break;
                case "Crédito AVS":
                    CarregaDadosCreditoAVS(venda);
                    break;
                case "Pré-Autorização":
                    CarregaDadosPreAutorizacao(venda);
                    break;
                case "Pagamento Recorrente":
                    CarregaDadosPagamentoRecorrente(venda);
                    break;
                case "IATA":
                    CarregaDadosIATA(venda);
                    break;
                case "Boleto":
                    CarregaDadosBoleto(venda);
                    break;
            }

            //Carrega dados de risco
            if (venda.AnalisedeRisco != null)
            {
                cbxIncluirAnalise.Visible = true;
                cbxIncluirAnalise.Checked = true;
                PreparaTelaAnaliseRisco(venda, venda.TipoTransacao.GetTitle());
            }
        }

        private void PreparaTelaAnaliseRisco(Modelo.Venda venda, string tipoTransacao)
        {
            AnalisedeRisco1.CarregaAnaliseRisco(venda);
        }
        private void CarregaDadosBoleto(Modelo.Venda venda)
        {
            //Dados do cliente
            ucDadosCliente.CarregarDados((venda as Modelo.VendaBoleto).DadosCliente);
            //Endereço de Cobrança
            ucEndereco.CarregarEndereco((venda as Modelo.VendaBoleto).EnderecoCobranca);
            //Dados de Pagamento
            ucDadosPagamento.CarregarPagamento((venda as Modelo.VendaBoleto).DadosPagamento);
        }

        private void CarregaDadosIATA(Modelo.Venda venda)
        {
            CarregaDadosCredito(venda);

            dadosIATA.CarregarIata((venda as Modelo.VendaPagamentoIATA));
            
        }

        private void CarregaDadosPagamentoRecorrente(Modelo.Venda venda)
        {
            CarregaDadosCredito(venda);

            if ((venda as Modelo.VendaPagamentoRecorrente).FormaRecorrencia.GetTitle() == "Agendado")
            {
                txtValorRecorrencia.Text = ((Modelo.VendaPagamentoRecorrenteFireForget)venda).ValorRecorrencia.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR"));
                ddlFrequencia.SelectedValue = ((Modelo.VendaPagamentoRecorrenteFireForget)venda).Frequencia;
                txtDataInicio.Text = ((Modelo.VendaPagamentoRecorrenteFireForget)venda).DataInicio;
                ddlQuantidadeRecorrencia.SelectedValue = ((Modelo.VendaPagamentoRecorrenteFireForget)venda).QuantidadeRecorencia;
                txtValorUltimaCobranca.Text = ((Modelo.VendaPagamentoRecorrenteFireForget)venda).ValorUltimaCobranca.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR"));
                txtDataUltimaCobranca.Text = ((Modelo.VendaPagamentoRecorrenteFireForget)venda).DataUltimaCobranca;
            }
            if (((Modelo.VendaPagamentoRecorrente)venda).FormaRecorrencia == Modelo.enFormaRecorrencia.FireForget)
                ddlFormaRecorrencia.SelectedValue = "Fire and Forget";
            else
                ddlFormaRecorrencia.SelectedValue = "Historic Recurring";
        }

        private void CarregaDadosPreAutorizacao(Modelo.Venda venda)
        {
            CarregaDadosCredito(venda);
        }

        private void CarregaDadosCreditoAVS(Modelo.Venda venda)
        {
            CarregaDadosCredito(venda);
            switch (ddlFormaPagamento.SelectedValue)
            {
                case "À vista":
                    if ((venda as Modelo.VendaCreditoAVSAVista).InfoTitular != null)
                    {
                        txtCPF.Text = (venda as Modelo.VendaCreditoAVSAVista).InfoTitular.CPF;
                        ucEndereco.CarregarEndereco((venda as Modelo.VendaCreditoAVSAVista).InfoTitular.Endereco);
                    }
                    break;
                case "Parcelado Estabelecimento":
                    if ((venda as Modelo.VendaCreditoAVSParceladoEstabelecimento).InfoTitular != null)
                    {
                        txtCPF.Text = (venda as Modelo.VendaCreditoAVSParceladoEstabelecimento).InfoTitular.CPF;
                        ucEndereco.CarregarEndereco((venda as Modelo.VendaCreditoAVSParceladoEstabelecimento).InfoTitular.Endereco);
                    }
                    break;
                case "Parcelado Emissor":
                    if ((venda as Modelo.VendaCreditoAVSParceladoEmissor).InfoTitular != null)
                    {
                        txtCPF.Text = (venda as Modelo.VendaCreditoAVSParceladoEmissor).InfoTitular.CPF;
                        ucEndereco.CarregarEndereco((venda as Modelo.VendaCreditoAVSParceladoEmissor).InfoTitular.Endereco);
                    }
                    break;
            }
        }

        private void CarregaDadosCredito(Modelo.Venda venda)
        {
            if (venda.DadosCartao != null)
            {
                if (venda.FormaPagamento.GetTitle() == "Parcelado Emissor")
                    ddlParcelas.SelectedValue = venda.DadosCartao.Parcelas;
                else
                    ddlParcelasDinamico.SelectedValue = venda.DadosCartao.Parcelas;

                DefineBandeiraSelecionada(venda.DadosCartao.Bandeira.GetTitle());

                ddlValidadeCartaoMes.SelectedValue = venda.DadosCartao.MesValidade;
                ddlValidadeCartaoAno.SelectedValue = venda.DadosCartao.AnoValidade;
            }

            if (venda.FormaPagamento.GetTitle().Contains("Parcelado"))
                ddlFormaPagamento.SelectedValue = venda.FormaPagamento.GetTitle();

            txtValor.Text = venda.Valor.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR"));
            txtNPedido.Text = venda.NumeroPedido;
        }

        private void DefineBandeiraSelecionada(string bandeira)
        {
            switch (bandeira)
            {
                case "Mastercard":
                    rdbMaster.Checked = true;
                    break;
                case "Visa":
                    rdbVisa.Checked = true;
                    break;
                case "Diners":
                    rdbDiners.Checked = true;
                    break;
                case "Hipercard":
                    rdbHiperCard.Checked = true;
                    break;
                case "Hiper": 
                    rdbHiper.Checked = true;
                    break;
                case "Amex": 
                    rdbAmex.Checked = true;
                    break;
                case "Elo":
                    rdbElo.Checked = true;
                    break;
                default:
                    break;
            }
        }
        
        private void DefinirAnaliseRisco(Int32 pv)
        {
            Modelos.RetornoServicoPV retornoServicoPV = null;

            Negocio.Distribuidores distribuidores = new Negocio.Distribuidores();
            retornoServicoPV = distribuidores.ListaServicoPV(pv);

            cbxIncluirAnalise.Visible = (retornoServicoPV.Produtos.Any((Redecard.PN.DataCash.Modelos.Produto p) => p.Codigo == 602) &&
                !retornoServicoPV.Produtos.Any((Redecard.PN.DataCash.Modelos.Produto p) => p.Codigo == 603));

            if (!retornoServicoPV.Produtos.Any((Redecard.PN.DataCash.Modelos.Produto p) => p.Codigo == 601))
                this.ddlTipoTransacao.Items.Remove(new System.Web.UI.WebControls.ListItem("Boleto", "Boleto"));
            
            if (!retornoServicoPV.Produtos.Any((Redecard.PN.DataCash.Modelos.Produto p) => p.Codigo == 605))
                this.ddlTipoTransacao.Items.Remove(new System.Web.UI.WebControls.ListItem("Pagamento Recorrente", "Pagamento Recorrente"));
        }

        private void DefinirGrupoFornecedor(Int32 pv)
        {
            ddlGrupoFornecedor.Items.Clear();
            ddlFornecedor.Items.Clear();

            Int32 retornoPerfil = 0;
            Modelos.MensagemErro mensagemErro = null;

            Negocio.Distribuidores distribuidores = new Negocio.Distribuidores();
            retornoPerfil = distribuidores.ConsultaPerfilPV(pv, out mensagemErro);

            if (retornoPerfil == 2) //Distribuidor
            {
                Modelos.RetornoGrupoRamoAtividade retornoGrupoRamoAtividade = distribuidores.ListaGrupoRamoAtividade(pv);
                CarregaGruposFornecedores(retornoGrupoRamoAtividade);

                Modelos.RetornoFornecedores retornoFornecedores = distribuidores.ListaFornecedorPVDistr(pv);
                CarregaFornecedores(retornoFornecedores);

                phGrupoFornecedor.Visible = true;
                this.PVDistribuidor = true;

            }
            else
            {
                phGrupoFornecedor.Visible = false;
            }
        }

        /// <summary>
        /// Mostra/Esconde bandeiras ELO e AMEX
        /// </summary>
        /// <param name="pv">Número do PV</param>
        private void DefinirBandeirasAdicionais(Int32 pv)
        {
            var bandeirasAdicionais = new Negocio.Configuracao().VerificaBandeirasAdicionais(pv);
            if (bandeirasAdicionais)
            {
                // Se possui o serviço de código 607, mostra as bandeiras AMEX e ELO
                pnlAMEX.Visible = true;
                pnlELO.Visible = true;
            }
        }

        private void CarregaFornecedores(Modelos.RetornoFornecedores retornoFornecedores)
        {
            foreach (Modelos.RegistroFornecedor fornecedor in retornoFornecedores.Fornecedores)
            {
                ddlFornecedor.Items.Add(
                    new ListItem(fornecedor.NomeFornecedor, fornecedor.NumPdvFornecedor.ToString()));
            }
            //ProcessaListas.AdicionarSelecioneDDL(ddlFornecedor);
        }

        private void CarregaGruposFornecedores(Modelos.RetornoGrupoRamoAtividade retornoGrupoRamoAtividade)
        {
            foreach (Modelos.GrupoRamoAtividade grupoRamoAtividade in retornoGrupoRamoAtividade.GrupoRamoAtividades)
            {
                ddlGrupoFornecedor.Items.Add(
                    new ListItem(grupoRamoAtividade.Descricao, grupoRamoAtividade.CodigoAtividade.ToString())); 
            }
            //ProcessaListas.AdicionarSelecioneDDL(ddlGrupoFornecedor);
        }

        // Ação do botão Continuar
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("btnContinuar - Faça sua Venda"))
            {
                try
                {
                    Modelo.Venda venda = null;
                    switch (ddlTipoTransacao.SelectedValue)
                    {
                        case "Crédito":
                            venda = EnviarCredito();
                            break;

                        case "Pré-Autorização":
                            venda = EnviarPreAutorizacao();
                            break;

                        case "Pagamento Recorrente":
                            venda = EnviarPagamentoRecorrente();
                            break;
                        case "Crédito AVS":
                            venda = EnviarCreditoAVS();
                            break;
                        case "Pré-Autorização AVS":
                            venda = EnviarPreAutorizacaoAVS();
                            break;
                        case "IATA":
                            venda = EnviarIATA();
                            break;
                        case "Boleto":
                            venda = EnviarBoleto();
                            break;
                    }

                    if (ddlGrupoFornecedor.Visible && ddlFornecedor.Visible)
                    {
                        venda.GrupoFornecedor = Convert.ToInt32(ddlGrupoFornecedor.SelectedValue);
                        venda.PVFornecedor = Convert.ToInt32(ddlFornecedor.SelectedValue);
                    }

                    venda.PVDistribuidor = this.PVDistribuidor;
                    Session["FacaSuaVenda"] = venda;

                    Response.Redirect("FacaSuaVendaConfirmacao.aspx", true);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>Ação do botão Limpar</summary>
        protected void btnLimpar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("btnLimpar - Faça sua Venda"))
            {
                try
                {
                    enTipoTransacao tipoTransacao = Enum.GetValues(typeof(enTipoTransacao)).Cast<enTipoTransacao>()
                        .FirstOrDefault(tipo => tipo.GetTitle().CompareTo(ddlTipoTransacao.SelectedValue) == 0);
                    enFormaPagamento formaPagamento = Enum.GetValues(typeof(enFormaPagamento)).Cast<enFormaPagamento>()
                        .FirstOrDefault(forma => forma.GetTitle().CompareTo(ddlFormaPagamento.SelectedValue) == 0);
                    enFormaRecorrencia formaRecorrencia = default(enFormaRecorrencia);
                    if (ddlFormaRecorrencia.SelectedValue.CompareTo("Fire and Forget") == 0)
                        formaRecorrencia = enFormaRecorrencia.FireForget;
                    else
                        formaRecorrencia = enFormaRecorrencia.HistoricRecurring;

                    Modelo.Venda venda = Modelo.Venda.InstanciarVenda(tipoTransacao, formaPagamento, formaRecorrencia);

                    if (ddlGrupoFornecedor.Visible && ddlFornecedor.Visible)
                    {
                        venda.GrupoFornecedor = Convert.ToInt32(ddlGrupoFornecedor.SelectedValue);
                        venda.PVFornecedor = Convert.ToInt32(ddlFornecedor.SelectedValue);
                    }

                    if (cbxIncluirAnalise.Visible && cbxIncluirAnalise.Checked)
                        venda.AnalisedeRisco = new Modelo.AnalisedeRisco();

                    venda.PVDistribuidor = this.PVDistribuidor;
                    Session["FacaSuaVenda"] = venda;

                    //remove cidades selecionadas da session
                    for (Int32 i = 0; Session.Count > i; i++)
                    {
                        if (Session.Keys[i].Contains("_ddlEstado"))
                            Session[Session.Keys[i]] = "";
                        if (Session.Keys[i].Contains("_ddlCidade"))
                            Session[Session.Keys[i]] = "";
                    }

                    Response.Redirect("FacaSuaVenda.aspx", true);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #region AVS
        /// <summary>
        /// Envia informações de pré autorização AVS
        /// </summary>
        private Modelo.Venda EnviarPreAutorizacaoAVS()
        {

            Modelo.Venda venda = new Modelo.VendaPreAutorizacaoAVS();

            (venda as Modelo.VendaPreAutorizacaoAVS).InfoTitular = PreencheInfoTitular();

            venda.DadosCartao = PreencherCartao(venda);
            venda.Valor = Convert.ToDecimal(txtValor.Text);
            venda.NumeroPedido = txtNPedido.Text;
            venda.CaminhoUserControlConfirmacao = CAMINHO_PREAUTORIZACAOAVS_CONFIRMACAO;
            venda.CaminhoUserControlComprovante = CAMINHO_PREAUTORIZACAOAVS_COMPROVANTE;
            venda.AnalisedeRisco = cbxIncluirAnalise.Checked ? AnalisedeRisco1.ObterAnalisedeRisco() : null;

            venda.CodigoEntidade = this.SessaoAtual.CodigoEntidade;

            return venda;
        }
        /// <summary>
        /// envia Informações de Crédito AVS
        /// </summary>
        private Modelo.Venda EnviarCreditoAVS()
        {

            Modelo.Venda venda = null;

            switch (ddlFormaPagamento.SelectedValue)
            {
                case "À vista":
                    venda = new Modelo.VendaCreditoAVSAVista();
                    (venda as Modelo.VendaCreditoAVSAVista).InfoTitular = PreencheInfoTitular();
                    break;

                case "Parcelado Estabelecimento":
                    venda = new Modelo.VendaCreditoAVSParceladoEstabelecimento();
                    (venda as Modelo.VendaCreditoAVSAVista).InfoTitular = PreencheInfoTitular();
                    break;

                case "Parcelado Emissor":
                    venda = new Modelo.VendaCreditoAVSParceladoEmissor();
                    (venda as Modelo.VendaCreditoAVSAVista).InfoTitular = PreencheInfoTitular();
                    break;
            }

            venda.DadosCartao = PreencherCartao(venda);
            venda.Valor = Convert.ToDecimal(txtValor.Text);
            venda.NumeroPedido = txtNPedido.Text;
            venda.CaminhoUserControlConfirmacao = CAMINHO_CREDITOAVS_CONFIRMACAO;
            venda.CaminhoUserControlComprovante = CAMINHO_CREDITOAVS_COMPROVANTE;
            venda.AnalisedeRisco = cbxIncluirAnalise.Checked ? AnalisedeRisco1.ObterAnalisedeRisco() : null;

            venda.CodigoEntidade = this.SessaoAtual.CodigoEntidade;

            return venda;
        }
        #endregion

        #region Crédito

        /// <summary>
        /// Envia informações de venda de crédito à vista
        /// </summary>
        private Modelo.Venda EnviarCredito()
        {
            Modelo.Venda venda = null;

            switch (ddlFormaPagamento.SelectedValue)
            {
                case "À vista":
                    venda = new Modelo.VendaCreditoAVista();
                    break;

                case "Parcelado Estabelecimento":
                    venda = new Modelo.VendaCreditoParceladoEstabelecimento();
                    break;

                case "Parcelado Emissor":
                    venda = new Modelo.VendaCreditoParceladoEmissor();
                    break;
            }

            venda.DadosCartao = PreencherCartao(venda);
            venda.Valor = txtValor.Text == string.Empty ? 0 : Convert.ToDecimal(txtValor.Text); // Convert.ToDecimal(txtValor.Text);
            venda.NumeroPedido = txtNPedido.Text;
            venda.CaminhoUserControlConfirmacao = CAMINHO_CREDITO_CONFIRMACAO;
            venda.CaminhoUserControlComprovante = CAMINHO_CREDITO_COMPROVANTE;

            venda.AnalisedeRisco = cbxIncluirAnalise.Checked ? AnalisedeRisco1.ObterAnalisedeRisco() : null;

            venda.CodigoEntidade = this.SessaoAtual.CodigoEntidade;

            return venda;
        }

        #endregion

        #region Pré-Autorização
        /// <summary>
        /// Envia informações de venda de pré-autorização
        /// </summary>
        private Modelo.Venda EnviarPreAutorizacao()
        {
            Modelo.Venda venda = null;

            switch (ddlFormaPagamento.SelectedValue)
            {
                case "À vista":
                    venda = new Modelo.VendaPreAutorizacaoAVista();
                    break;

                case "Parcelado":
                    venda = new Modelo.VendaPreAutorizacaoParcelado();
                    break;
            }

            venda.DadosCartao = this.PreencherCartao(venda);
            venda.Valor = txtValor.Text.TryParseToCurrency(0);
            venda.NumeroPedido = txtNPedido.Text;
            venda.AnalisedeRisco = cbxIncluirAnalise.Checked ? AnalisedeRisco1.ObterAnalisedeRisco() : null;
            venda.CaminhoUserControlConfirmacao = CAMINHO_PREAUTORIZACAO_CONFIRMACAO;
            venda.CaminhoUserControlComprovante = CAMINHO_PREAUTORIZACAO_COMPROVANTE;

            venda.CodigoEntidade = this.SessaoAtual.CodigoEntidade;

            return venda;
        }

        #endregion

        #region Pagamento Recorrente
        /// <summary>
        /// Envia informações de venda de pré-autorização
        /// </summary>
        private Modelo.Venda EnviarPagamentoRecorrente()
        {
            Modelo.Venda venda = null;

            switch (ddlFormaRecorrencia.SelectedValue)
            {
                case "Fire and Forget":
                    venda = new Modelo.VendaPagamentoRecorrenteFireForget();
                    ((Modelo.VendaPagamentoRecorrenteFireForget)venda).ValorRecorrencia = txtValorRecorrencia.Text.TryParseToCurrency(0);
                    ((Modelo.VendaPagamentoRecorrenteFireForget)venda).Valor = txtValorRecorrencia.Text.TryParseToCurrency(0);
                    ((Modelo.VendaPagamentoRecorrenteFireForget)venda).Frequencia = ddlFrequencia.SelectedValue;
                    ((Modelo.VendaPagamentoRecorrenteFireForget)venda).FrequenciaExibicao = ddlFrequencia.SelectedItem.Text;
                    ((Modelo.VendaPagamentoRecorrenteFireForget)venda).DataInicio = txtDataInicio.Text;
                    ((Modelo.VendaPagamentoRecorrenteFireForget)venda).QuantidadeRecorencia = ddlQuantidadeRecorrencia.SelectedValue;
                    ((Modelo.VendaPagamentoRecorrenteFireForget)venda).ValorUltimaCobranca = txtValorUltimaCobranca.Text.TryParseToCurrency(0);
                    ((Modelo.VendaPagamentoRecorrenteFireForget)venda).DataUltimaCobranca = txtDataUltimaCobranca.Text;
                    break;

                case "Historic Recurring":
                    venda = new Modelo.VendaPagamentoRecorrenteHistoricRecurring();
                    venda.Valor = Convert.ToDecimal(txtValor.Text);
                    break;
            }

            venda.DadosCartao = this.PreencherCartao(venda);
            venda.NumeroPedido = txtNPedido.Text;
            venda.CaminhoUserControlConfirmacao = CAMINHO_PAGAMENTORECORRENTE_CONFIRMACAO;
            venda.CaminhoUserControlComprovante = CAMINHO_PAGAMENTORECORRENTE_COMPROVANTE;
            venda.AnalisedeRisco = cbxIncluirAnalise.Checked ? AnalisedeRisco1.ObterAnalisedeRisco() : null;

            venda.CodigoEntidade = this.SessaoAtual.CodigoEntidade;

            return venda;
        }

        #endregion

        #region Métodos Auxiliares

        /// <summary>
        /// Preenche dados do cartão com os dados digitados pelo usuário
        /// </summary>
        /// <returns>Cartão preenchido</returns>
        private Modelo.Cartao PreencherCartao(Modelo.Venda venda)
        {
            Modelo.Cartao cartao = new Modelo.Cartao();
            cartao.NomePortador = txtNomePortador.Text;
            cartao.Bandeira = this.VerificarBandeiraSelecionada();
            cartao.Numero = txtNumeroCartao.Text;
            cartao.MesValidade = ddlValidadeCartaoMes.SelectedValue;
            cartao.AnoValidade = ddlValidadeCartaoAno.SelectedValue;
            cartao.CodigoSeguranca = txtCodigoSeguranca.Text;
            
            if (venda.FormaPagamento == Modelo.enFormaPagamento.ParceladoEmissor)
                cartao.Parcelas = ddlParcelas.SelectedValue;
            else if (venda.FormaPagamento == Modelo.enFormaPagamento.ParceladoEstabelecimento ||
                     venda.FormaPagamento == Modelo.enFormaPagamento.Parcelado)
                cartao.Parcelas = ddlParcelasDinamico.SelectedValue;

            return cartao;
        }

        /// <summary>
        /// Preenche as informações do titular
        /// </summary>
        /// <returns>Informações do Titular</returns>
        private Modelo.InfoTitular PreencheInfoTitular()
        {
            Modelo.InfoTitular infoTitular = new Modelo.InfoTitular();
            infoTitular.CPF = txtCPF.Text;
            infoTitular.Endereco = ucEndereco.ObterEndereco();

            return infoTitular;
        }

        /// <summary>
        /// Verifica qual bandeira foi selecionada pelo usuário
        /// </summary>
        /// <returns>Bandeira selecionada</returns>
        private Modelo.enBandeiras VerificarBandeiraSelecionada()
        {
            if (rdbDiners.Checked)
                return Modelo.enBandeiras.Diners;

            if (rdbHiperCard.Checked)
                return Modelo.enBandeiras.Hipercard;

            if (rdbVisa.Checked)
                return Modelo.enBandeiras.Visa;

            if (rdbElo.Checked)
                return Modelo.enBandeiras.Elo;

            if (rdbHiper.Checked)
                return Modelo.enBandeiras.Hiper;

            if (rdbAmex.Checked)
                return Modelo.enBandeiras.Amex;

            // Senão Master
            return Modelo.enBandeiras.Master;
        }

        /// <summary>
        /// Define os meses que deverão ser exibidos na combo da validade do cartão
        /// </summary>
        private void DefineDiaValidadeCartao()
        {
            String dia;

            for (int i = 1; i <= 12; i++)
            {
                dia = "0" + i.ToString();
                dia = dia.Substring(dia.Length - 2);

                ddlValidadeCartaoMes.Items.Add(new ListItem(dia, dia));
            }
        }

        /// <summary>
        /// Define os anos que deverão ser exibidos na combo da validade do cartão
        /// </summary>
        private void DefineAnoValidadeCartao()
        {
            ddlValidadeCartaoAno.Items.Add(
                new ListItem(DateTime.Now.ToString("yy"), DateTime.Now.ToString("yy")));

            for (int i = 1; i <= 10; i++)
                ddlValidadeCartaoAno.Items.Add(
                    new ListItem(DateTime.Now.AddYears(i).ToString("yy"),
                                 DateTime.Now.AddYears(i).ToString("yy")));
        }

        /// <summary>
        /// Carrega a combo de parcelas provenientes do GE
        /// </summary>
        private void DefineParcelasDinamicas(Int32 pv)
        {
            DataCashServico.DataCashServiceClient datacashServico = new DataCashServico.DataCashServiceClient();
            Int32 numeroParcelas = datacashServico.ConsultarNumeroParcelas(pv);
            String item = "";

            for (int i = 2; i <= numeroParcelas; i++)
            {
                item = "0" + i.ToString();
                item = Redecard.PN.Comum.ExtensaoMetodo.Right(item, 2);

                ddlParcelasDinamico.Items.Add(new ListItem(item, item));
            }
        }

        /// <summary>
        /// Define a quantidade de recorrências
        /// </summary>
        private void DefineQuantidadeRecorrencia()
        {
            String rec;

            for (int i = 2; i <= 100; i++)
            {
                rec = "0" + i.ToString();

                if (i.ToString().Length == 2)
                    rec = rec.Substring(rec.Length - 2);
                else
                    rec = i.ToString();

                ddlQuantidadeRecorrencia.Items.Add(new ListItem(rec, rec));
            }
        }



        #endregion

        #region IATA
        private Modelo.Venda EnviarIATA()
        {

            Modelo.Venda venda = null;

            switch (ddlFormaPagamento.SelectedValue)
            {
                case "À vista":
                    venda = dadosIATA.ObterDadosIata(Modelo.enFormaPagamento.Avista);
                    break;

                case "Parcelado Estabelecimento":
                    venda = dadosIATA.ObterDadosIata(Modelo.enFormaPagamento.ParceladoEstabelecimento);
                    break;
            }

            venda.DadosCartao = PreencherCartao(venda);
            //venda.Valor = Convert.ToDecimal(txtValor.Text);
            venda.Valor = dadosIATA.TotalPassagem;
            venda.NumeroPedido = txtNPedido.Text;
            venda.CaminhoUserControlConfirmacao = CAMINHO_IATA_CONFIRMACAO;
            venda.CaminhoUserControlComprovante = CAMINHO_IATA_COMPROVANTE;
            
            venda.AnalisedeRisco = cbxIncluirAnalise.Checked ? AnalisedeRisco1.ObterAnalisedeRisco() : null;

            venda.CodigoEntidade = this.SessaoAtual.CodigoEntidade;

            if (ddlFornecedor.Visible)
            {
                venda.PVFornecedor = Convert.ToInt32(ddlFornecedor.SelectedValue);
            }

            return venda;
        }
        #endregion

        #region Boleto
        /// <summary>
        /// Envia informações de venda por boleto
        /// </summary>
        private Modelo.Venda EnviarBoleto()
        {
            Modelo.VendaBoleto venda = new Modelo.VendaBoleto();

            venda.DadosCliente = ucDadosCliente.ObterDadosCliente();
            venda.EnderecoCobranca = ucEndereco.ObterEndereco();
            venda.DadosPagamento = ucDadosPagamento.ObterDadosPagamento();

            venda.CaminhoUserControlConfirmacao = CAMINHO_BOLETO_CONFIRMACAO;
            venda.CaminhoUserControlComprovante = CAMINHO_BOLETO_COMPROVANTE;

            venda.CodigoEntidade = this.SessaoAtual.CodigoEntidade;

            if (ddlFornecedor.Visible)
            {
                venda.PVFornecedor = Convert.ToInt32(ddlFornecedor.SelectedValue);
            }

            return venda;
        }

        #endregion
    }
}
