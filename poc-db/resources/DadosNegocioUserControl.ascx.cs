using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Sharepoint.GETaxaFiliacao;
using System.ServiceModel;
using Redecard.PN.Credenciamento.Sharepoint.TGCenarios;
using Redecard.PN.Credenciamento.Sharepoint.TGTipoEquip;
using Redecard.PN.Credenciamento.Sharepoint.WFCampanhas;
using Redecard.PN.Credenciamento.Sharepoint.GERamosAtd;
using Redecard.PN.Credenciamento.Sharepoint.WFProposta;
using Redecard.PN.Credenciamento.Sharepoint.WFTecnologia;
using Redecard.PN.Credenciamento.Sharepoint.WFProdutos;
using Redecard.PN.Credenciamento.Sharepoint.GEProdutos;
using System.Linq;
using System.Collections.Generic;
using Redecard.PN.Credenciamento.Sharepoint.PNTransicoesServico;
using Redecard.PN.Credenciamento.Sharepoint.GEPontoVen;

namespace Redecard.PN.Credenciamento.Sharepoint.WebParts.DadosNegocio
{
    public partial class DadosNegocioUserControl : UserControlCredenciamentoBase
    {
        #region [ Propriedades ]

        public List<ProdutosListaDadosProdutosPorRamoCanal> ListaProdutosConstrucard
        {
            get
            {
                if (ViewState["ProdutosConstrucard"] == null)
                    ViewState["ProdutosConstrucard"] = new List<ProdutosListaDadosProdutosPorRamoCanal>();

                return (List<ProdutosListaDadosProdutosPorRamoCanal>)ViewState["ProdutosConstrucard"];
            }
            set
            {
                ViewState["ProdutosConstrucard"] = value;
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
                if (Credenciamento.Fase < 2)
                    Credenciamento.Fase = 2;

                Page.MaintainScrollPositionOnPostBack = true;
                Page.Title = "Dados Negócio";

                if (!Page.IsPostBack)
                {
                    if (Credenciamento.RefazerNegociacao)
                    {
                        Credenciamento.TipoEquipamento = String.Empty;
                        Credenciamento.CodCenario = null;
                        Credenciamento.CodCampanha = String.Empty;
                        Credenciamento.RefazerNegociacao = false;
                        base.ExibirPainelMensagem("Dados principais alterados, refaça a negociação.");
                    }

                    CarregarTipoEquipamento();
                    CarregarTaxaAdesao();
                    CarregarProdutosCredito();
                    CarregarProdutosDebito();
                    CarregarProdutosConstrucard();

                    // Carrega dados dos controles
                    if (!String.IsNullOrEmpty(Credenciamento.TipoEquipamento))
                    {
                        ddlTipoEquipamento.SelectedValue = Credenciamento.TipoEquipamento;
                        CarregarCenarios(null);
                        CarregarCampanhas();

                        lblDescontoPor1.Text = "0%";
                        lblDescontoPor2.Text = "0%";
                        lblDescontoPor3.Text = "0%";
                        lblDescontoPor4.Text = "0%";
                        lblDescontoPor5.Text = "0%";
                        lblDescontoPor6.Text = "0%";
                        lblDescontoPor7.Text = "0%";
                        lblDescontoPor8.Text = "0%";
                        lblDescontoPor9.Text = "0%";
                        lblDescontoPor10.Text = "0%";
                        lblDescontoPor11.Text = "0%";
                        lblDescontoPor12.Text = "0%";

                        lblDescontoVal1.Text = "0,00";
                        lblDescontoVal2.Text = "0,00";
                        lblDescontoVal3.Text = "0,00";
                        lblDescontoVal4.Text = "0,00";
                        lblDescontoVal5.Text = "0,00";
                        lblDescontoVal6.Text = "0,00";
                        lblDescontoVal7.Text = "0,00";
                        lblDescontoVal8.Text = "0,00";
                        lblDescontoVal9.Text = "0,00";
                        lblDescontoVal10.Text = "0,00";
                        lblDescontoVal11.Text = "0,00";
                        lblDescontoVal12.Text = "0,00";

                        lblSaz1.Text = "0,00";
                        lblSaz2.Text = "0,00";
                        lblSaz3.Text = "0,00";
                        lblSaz4.Text = "0,00";
                        lblSaz5.Text = "0,00";
                        lblSaz6.Text = "0,00";
                        lblSaz7.Text = "0,00";
                        lblSaz8.Text = "0,00";
                        lblSaz9.Text = "0,00";
                        lblSaz10.Text = "0,00";
                        lblSaz11.Text = "0,00";
                        lblSaz12.Text = "0,00";
                    }

                    txtValorAluguel.Text = String.Format("{0:f2}", Credenciamento.ValorAluguelTela);
                    txtTaxaAdesao.Text = Credenciamento.TaxaAdesao != null ? String.Format(@"{0:c}", Credenciamento.TaxaAdesao).Replace("R$", "") : txtTaxaAdesao.Text;
                    txtTaxaAdesao.Enabled = false;

                    if (Credenciamento.CodCenario != null)
                    {
                        ddlCenario.SelectedValue = Credenciamento.CodCenario.ToString();
                        CarregarCenario();
                    }

                    if (!(String.IsNullOrEmpty(Credenciamento.CodCampanha) || String.IsNullOrEmpty(Credenciamento.CodCampanha.Trim())))
                    {
                        ddlCampanha.SelectedValue = Credenciamento.CodCampanha;
                        CarregaCenarioPorCampanha();
                        CarregarCenario();
                        CarregarProdutosPorCampanha();
                        CarregarTaxaAdesaoPorCampanha();
                        ddlCenario.Enabled = false;
                        txtResumoCampanha.Text = GetResumoCampanha(Credenciamento.CodCampanha);
                    }

                    if (Credenciamento.Canal == 2 && String.IsNullOrEmpty(ddlCenario.SelectedValue))
                        txtValorAluguel.Enabled = true;
                    else
                        txtValorAluguel.Enabled = false;

                    if (String.Compare(Credenciamento.TipoEquipamento, "TOL") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "SNT") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "TOF") == 0)
                    {
                        ddlCenario.Enabled = false;
                        txtValorAluguel.Enabled = false;
                        txtValorAluguel.Text = "0,00";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
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
            if (!((String.Compare(ddlTipoEquipamento.SelectedValue, "TOL") == 0 || String.Compare(ddlTipoEquipamento.SelectedValue, "TOF") == 0 ||
                String.Compare(ddlTipoEquipamento.SelectedValue, "SNT") == 0) && String.IsNullOrEmpty(Credenciamento.NomeEmail)))
            {
                if (!(String.Compare(ddlTipoEquipamento.SelectedValue, "SNT") == 0 && String.IsNullOrEmpty(Credenciamento.NomeHomePage)))
                {
                    Int32 codRetorno = SalvarDados();
                    if (codRetorno == 0)
                        Response.Redirect("pn_dadosendereco.aspx", false);
                    else if (codRetorno != 399)
                        base.ExibirPainelExcecao("Redecard.PN.Credenciamento.Servicos", codRetorno);
                }
                else
                    base.ExibirPainelMensagem("Obrigatório o preenchimento do campo site na página Dados do Cliente");
            }
            else
                base.ExibirPainelMensagem("Obrigatório o preenchimento do campo e-mail na página Dados do Cliente");
        }

        /// <summary>
        /// Evento do botão Parar e Salvar Proposta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            SalvarDados();

            Credenciamento = new Modelo.Credenciamento();
            Response.Redirect("pn_dadosiniciais.aspx", false);
        }

        /// <summary>
        /// Evento do Botão Voltar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("pn_dadoscliente.aspx", false);
        }

        /// <summary>
        /// Carrega Campanhas, Cenários e o valor do aluguel para um tipo de equipamento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlTipoEquipamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarCampanhas();
            CarregarCenarios(null);
            CarregarValorAluguel();

            lblDescontoPor1.Text = "0%";
            lblDescontoPor2.Text = "0%";
            lblDescontoPor3.Text = "0%";
            lblDescontoPor4.Text = "0%";
            lblDescontoPor5.Text = "0%";
            lblDescontoPor6.Text = "0%";
            lblDescontoPor7.Text = "0%";
            lblDescontoPor8.Text = "0%";
            lblDescontoPor9.Text = "0%";
            lblDescontoPor10.Text = "0%";
            lblDescontoPor11.Text = "0%";
            lblDescontoPor12.Text = "0%";

            lblDescontoVal1.Text = "0,00";
            lblDescontoVal2.Text = "0,00";
            lblDescontoVal3.Text = "0,00";
            lblDescontoVal4.Text = "0,00";
            lblDescontoVal5.Text = "0,00";
            lblDescontoVal6.Text = "0,00";
            lblDescontoVal7.Text = "0,00";
            lblDescontoVal8.Text = "0,00";
            lblDescontoVal9.Text = "0,00";
            lblDescontoVal10.Text = "0,00";
            lblDescontoVal11.Text = "0,00";
            lblDescontoVal12.Text = "0,00";

            lblSaz1.Text = "0,00";
            lblSaz2.Text = "0,00";
            lblSaz3.Text = "0,00";
            lblSaz4.Text = "0,00";
            lblSaz5.Text = "0,00";
            lblSaz6.Text = "0,00";
            lblSaz7.Text = "0,00";
            lblSaz8.Text = "0,00";
            lblSaz9.Text = "0,00";
            lblSaz10.Text = "0,00";
            lblSaz11.Text = "0,00";
            lblSaz12.Text = "0,00";
        }

        /// <summary>
        /// Carrega os dados detalhados do cenário escolhido
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Credenciamento.Canal == 2 && String.IsNullOrEmpty(ddlCenario.SelectedValue))
            {
                txtValorAluguel.Enabled = true;
            }
            CarregarValorAluguel();
            CarregarCenario();
        }

        /// <summary>
        /// Carrega Cenário de acordo com a campanha selecionada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCampanha_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(ddlCampanha.SelectedValue))
                {

                    CarregarProdutosPorCampanha();
                    CarregarTaxaAdesaoPorCampanha();
                    CarregaCenarioPorCampanha();
                    CarregarCenario();
                    txtResumoCampanha.Text = GetResumoCampanha(ddlCampanha.SelectedValue);
                }
                else
                {
                    CarregarProdutosCredito();
                    CarregarProdutosDebito();
                    CarregarProdutosConstrucard();

                    CarregarTaxaAdesao();
                    ddlCenario.Enabled = true;
                    CarregarCenarios(null);
                    txtResumoCampanha.Text = String.Empty;

                    lblDescontoPor1.Text = "0%";
                    lblDescontoPor2.Text = "0%";
                    lblDescontoPor3.Text = "0%";
                    lblDescontoPor4.Text = "0%";
                    lblDescontoPor5.Text = "0%";
                    lblDescontoPor6.Text = "0%";
                    lblDescontoPor7.Text = "0%";
                    lblDescontoPor8.Text = "0%";
                    lblDescontoPor9.Text = "0%";
                    lblDescontoPor10.Text = "0%";
                    lblDescontoPor11.Text = "0%";
                    lblDescontoPor12.Text = "0%";

                    lblDescontoVal1.Text = "0,00";
                    lblDescontoVal2.Text = "0,00";
                    lblDescontoVal3.Text = "0,00";
                    lblDescontoVal4.Text = "0,00";
                    lblDescontoVal5.Text = "0,00";
                    lblDescontoVal6.Text = "0,00";
                    lblDescontoVal7.Text = "0,00";
                    lblDescontoVal8.Text = "0,00";
                    lblDescontoVal9.Text = "0,00";
                    lblDescontoVal10.Text = "0,00";
                    lblDescontoVal11.Text = "0,00";
                    lblDescontoVal12.Text = "0,00";

                    lblSaz1.Text = "0,00";
                    lblSaz2.Text = "0,00";
                    lblSaz3.Text = "0,00";
                    lblSaz4.Text = "0,00";
                    lblSaz5.Text = "0,00";
                    lblSaz6.Text = "0,00";
                    lblSaz7.Text = "0,00";
                    lblSaz8.Text = "0,00";
                    lblSaz9.Text = "0,00";
                    lblSaz10.Text = "0,00";
                    lblSaz11.Text = "0,00";
                    lblSaz12.Text = "0,00";
                }
            }
            catch (FaultException<WFCampanhas.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Muda Forma de Pagamento de uma venda Crédito
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlCreditoFormaPgto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                RepeaterItem row = (RepeaterItem)((DropDownList)sender).Parent;
                Int32 codFeature = (Int32)((HiddenField)row.FindControl("hiddenCodFeature")).Value.ToInt32();

                List<ProdutosListaDadosProdutosPorRamoCanal> produtos = Credenciamento.ProdutosCredito.FindAll(p => p.CodFeature == codFeature);
                foreach (ProdutosListaDadosProdutosPorRamoCanal p in produtos)
                {
                    p.IndFormaPagamento = ((DropDownList)sender).SelectedValue.ToCharArray()[0];
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Muda Forma de Pagamento de uma venda Débito
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlDebitoFormaPgto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                RepeaterItem row = (RepeaterItem)((DropDownList)sender).Parent;
                Label caracteristica = (Label)row.FindControl("lblDebitoCaracteristicas");

                List<ProdutosListaDadosProdutosPorRamoCanal> produtos = Credenciamento.ProdutosDebito.FindAll(p => p.NomeFeature == caracteristica.Text);
                foreach (ProdutosListaDadosProdutosPorRamoCanal p in produtos)
                {
                    p.IndFormaPagamento = ((DropDownList)sender).SelectedValue.ToCharArray()[0];
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Muda Forma de Pagamento de uma venda Construcard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlConstrucardFormaPgto_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                RepeaterItem row = (RepeaterItem)((DropDownList)sender).Parent;
                Label caracteristica = (Label)row.FindControl("lblConstrucardCaracteristicas");

                ProdutosListaDadosProdutosPorRamoCanal prod = ListaProdutosConstrucard.FirstOrDefault(p => p.NomeFeature == caracteristica.Text);
                if (prod != null)
                    prod.IndFormaPagamento = ((DropDownList)sender).SelectedValue.ToCharArray()[0];
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Data Bound do repeater de Produtos Crédito
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptProdutosCredito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                {
                    //((Label)e.Item.FindControl("lblCreditoBandeira")).Text = ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).NomeCCA;
                    ((Label)e.Item.FindControl("lblCreditoCaracteristicas")).Text = ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).NomeFeature;
                    ((Label)e.Item.FindControl("lblCreditoPrazoRecebimento")).Text = String.Format("{0} dia(s)", ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).ValorPrazoDefault);
                    ((Label)e.Item.FindControl("lblCreditoTaxa")).Text = String.Format("{0:f2}", ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).ValorTaxaDefault);
                    Int32 codCca = (Int32)((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).CodCCA;
                    ((HiddenField)e.Item.FindControl("hiddenCodCca")).Value = ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).CodCCA.ToString();
                    Int32 codFeature = (Int32)((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).CodFeature;
                    ((HiddenField)e.Item.FindControl("hiddenCodFeature")).Value = ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).CodFeature.ToString();

                    if (Credenciamento.RecuperadaGE || Credenciamento.CodTipoEstabelecimento == 1)
                    {
                        ((DropDownList)e.Item.FindControl("ddlCreditoFormaPgto")).Enabled = false;
                        ((TextBox)e.Item.FindControl("txtCreditoAte")).Enabled = false;
                        ((RequiredFieldValidator)e.Item.FindControl("rfvCreditoAte")).Enabled = false;
                    }

                    if (codFeature == 3)
                        ((DropDownList)e.Item.FindControl("ddlCreditoFormaPgto")).Enabled = false;
                    else
                    {
                        ((RequiredFieldValidator)e.Item.FindControl("rfvCreditoAte")).Visible = false;
                        ((CompareValidator)e.Item.FindControl("cvAteMaiorDe")).Visible = false;
                        ((CompareValidator)e.Item.FindControl("cvAteMenorLimite")).Visible = false;
                    }

                    if (codFeature == 3)
                    {
                        ((Label)e.Item.FindControl("lblCreditoDe")).Visible = false;
                        ((TextBox)e.Item.FindControl("txtCreditoDe")).Visible = true;
                        ((TextBox)e.Item.FindControl("txtCreditoDe")).Text = "2";

                        ((Label)e.Item.FindControl("lblCreditoAte")).Visible = false;
                        ((TextBox)e.Item.FindControl("txtCreditoAte")).Visible = true;
                        Int32 qtdeDefaultParcela = ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).QtdeDefaultParcela != null ? (Int32)((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).QtdeDefaultParcela : 0;
                        ((TextBox)e.Item.FindControl("txtCreditoAte")).Text = qtdeDefaultParcela.ToString();

                        ((Label)e.Item.FindControl("lblCreditoLimiteParcela")).Text = qtdeDefaultParcela.ToString();
                        ((TextBox)e.Item.FindControl("txtQtdeMaximaParcelas")).Text = ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).QtdeMaximaParcela.ToString();

                        if (((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).IndPatamarUnico != 'S' && Credenciamento.PermitePatamar == true)
                            ((ImageButton)e.Item.FindControl("ibtnMais")).Visible = true;
                    }

                    if (Credenciamento.Patamares != null)
                    {
                        List<Modelo.Patamar> patamares = Credenciamento.Patamares.FindAll(p => p.CodCca == codCca && p.CodFeature == codFeature);

                        if (patamares.Count > 0)
                        {
                            ((TextBox)e.Item.FindControl("txtCreditoDe")).Text = patamares[0].PatamarInicial.ToString();
                            ((TextBox)e.Item.FindControl("txtCreditoAte")).Text = patamares[0].PatamarFinal.ToString();

                            if (patamares.Count > 1)
                            {
                                Panel pnlPatamar1 = (Panel)e.Item.FindControl("pnlPatamar1");
                                pnlPatamar1.Visible = true;

                                ((TextBox)pnlPatamar1.FindControl("txtPatamar1De")).Text = patamares[1].PatamarInicial.ToString();
                                ((TextBox)pnlPatamar1.FindControl("txtPatamar1Ate")).Text = patamares[1].PatamarFinal.ToString();
                                ((TextBox)pnlPatamar1.FindControl("txtPatamar1Taxa")).Text = String.Format(@"{0:c}", patamares[1].TaxaPatamar);

                                if (Credenciamento.RecuperadaGE || Credenciamento.CodTipoEstabelecimento == 1)
                                {
                                    ((TextBox)pnlPatamar1.FindControl("txtPatamar1Ate")).Enabled = false;
                                    ((TextBox)pnlPatamar1.FindControl("txtPatamar1Taxa")).Enabled = false;
                                    ((ImageButton)pnlPatamar1.FindControl("ibtnMenosPatamar1")).Visible = false;
                                }

                                if (patamares.Count > 2)
                                    ((ImageButton)pnlPatamar1.FindControl("ibtnMenosPatamar1")).Visible = false;

                                ((Label)e.Item.FindControl("lblCreditoLimiteParcela")).Text = patamares[1].PatamarFinal.ToString();
                                //((TextBox)e.Item.FindControl("txtQtdeMaximaParcelas")).Text = patamares[1].PatamarFinal.ToString();
                            }

                            if (patamares.Count > 2)
                            {
                                Panel pnlPatamar2 = (Panel)e.Item.FindControl("pnlPatamar2");
                                pnlPatamar2.Visible = true;

                                ((TextBox)pnlPatamar2.FindControl("txtPatamar2De")).Text = patamares[2].PatamarInicial.ToString();
                                ((TextBox)pnlPatamar2.FindControl("txtPatamar2Ate")).Text = patamares[2].PatamarFinal.ToString();
                                ((TextBox)pnlPatamar2.FindControl("txtPatamar2Taxa")).Text = String.Format(@"{0:c}", patamares[2].TaxaPatamar);

                                if (Credenciamento.RecuperadaGE || Credenciamento.CodTipoEstabelecimento == 1)
                                {
                                    ((TextBox)pnlPatamar2.FindControl("txtPatamar2Ate")).Enabled = false;
                                    ((TextBox)pnlPatamar2.FindControl("txtPatamar2Taxa")).Enabled = false;
                                    ((ImageButton)pnlPatamar2.FindControl("ibtnMenosPatamar2")).Visible = false;
                                }

                                ((Label)e.Item.FindControl("lblCreditoLimiteParcela")).Text = patamares[2].PatamarFinal.ToString();
                                //((TextBox)e.Item.FindControl("txtQtdeMaximaParcelas")).Text = patamares[2].PatamarFinal.ToString();
                            }
                        }
                    }

                    ((DropDownList)e.Item.FindControl("ddlCreditoFormaPgto")).SelectedValue = ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).IndFormaPagamento.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Data Bound do repeater de Produtos Débito
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptProdutosDebito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                {
                    //((Label)e.Item.FindControl("lblDebitoBandeira")).Text = ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).NomeCCA;
                    ((Label)e.Item.FindControl("lblDebitoCaracteristicas")).Text = ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).NomeFeature;
                    ((Label)e.Item.FindControl("lblDebitoPrazoRecebimento")).Text = String.Format("{0} dia(s)", ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).ValorPrazoDefault);
                    ((Label)e.Item.FindControl("lblDebitoTaxa")).Text = String.Format("{0:f2}", ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).ValorTaxaDefault);

                    ((DropDownList)e.Item.FindControl("ddlDebitoFormaPgto")).SelectedValue = ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).IndFormaPagamento.ToString();

                    if (Credenciamento.RecuperadaGE)
                    {
                        ((DropDownList)e.Item.FindControl("ddlDebitoFormaPgto")).Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Data Bound do repeater de Produtos Construcard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptProdutosConstrucard_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                {
                    ((Label)e.Item.FindControl("lblConstrucardCaracteristicas")).Text = ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).NomeFeature;
                    ((Label)e.Item.FindControl("lblConstrucardPrazoRecebimento")).Text = String.Format("{0} dia(s)", ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).ValorPrazoDefault);
                    ((Label)e.Item.FindControl("lblConstrucardTaxa")).Text = String.Format("{0:f2}", ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).ValorTaxaDefault);

                    ((DropDownList)e.Item.FindControl("ddlConstrucardFormaPgto")).SelectedValue = ((ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem).IndFormaPagamento.ToString();

                    if (Credenciamento.RecuperadaGE)
                    {
                        ((DropDownList)e.Item.FindControl("ddlConstrucardFormaPgto")).Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados de Negócio", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Esconde ou mostra grid de produtos Construcard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkHabilitarConstrucard_CheckedChanged(object sender, EventArgs e)
        {
            pnlConstrucard.Visible = !pnlConstrucard.Visible;
        }

        /// <summary>
        /// Adiciona Patamares
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ibtnMais_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Panel patamar1 = (Panel)((ImageButton)sender).Parent.FindControl("pnlPatamar1");

                if (patamar1.Visible == false)
                {
                    patamar1.Visible = !patamar1.Visible;
                    ((TextBox)patamar1.FindControl("txtPatamar1De")).Text = (((TextBox)((ImageButton)sender).Parent.FindControl("txtCreditoAte")).Text.ToInt32() + 1).ToString();
                }
                else
                {
                    Panel patamar2 = (Panel)((ImageButton)sender).Parent.FindControl("pnlPatamar2");
                    if (patamar2.Visible == false)
                    {
                        patamar2.Visible = !patamar2.Visible;
                        ((TextBox)patamar2.FindControl("txtPatamar2De")).Text = (((TextBox)patamar1.FindControl("txtPatamar1Ate")).Text.ToInt32() + 1).ToString();

                        patamar1.FindControl("ibtnMenosPatamar1").Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados de Negócio", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Remove Patamar 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ibtnMenosPatamar1_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Panel patamar1 = (Panel)((ImageButton)sender).Parent;
                patamar1.Visible = false;

                ((TextBox)patamar1.FindControl("txtPatamar1Taxa")).Text = "0,00";
                ((TextBox)patamar1.FindControl("txtPatamar1Ate")).Text = String.Empty;

                ((Label)((ImageButton)sender).Parent.Parent.FindControl("lblCreditoLimiteParcela")).Text = ((TextBox)((ImageButton)sender).Parent.Parent.FindControl("txtCreditoAte")).Text;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados de Negócio", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Remove Patamar 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ibtnMenosPatamar2_Click(object sender, ImageClickEventArgs e)
        {
            try
            {
                Panel patamar2 = (Panel)((ImageButton)sender).Parent;
                patamar2.Visible = false;

                ((TextBox)patamar2.FindControl("txtPatamar2Taxa")).Text = "0,00";
                ((TextBox)patamar2.FindControl("txtPatamar2Ate")).Text = String.Empty;

                Panel patamar1 = (Panel)((ImageButton)sender).Parent.Parent.FindControl("pnlPatamar1");
                patamar1.FindControl("ibtnMenosPatamar1").Visible = true;

                ((Label)((ImageButton)sender).Parent.Parent.FindControl("lblCreditoLimiteParcela")).Text = ((TextBox)patamar1.FindControl("txtPatamar1Ate")).Text;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados de Negócio", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Muda o valor do limite de parcelas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtCreditoAte_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Int32 ate = ((TextBox)sender).Text.ToInt32();

                Panel patamar1 = (Panel)((TextBox)sender).Parent.FindControl("pnlPatamar1");
                if (patamar1.Visible)
                    ((TextBox)patamar1.FindControl("txtPatamar1De")).Text = (ate + 1).ToString();

            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Muda o valor do limite de parcelas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtPatamar1Ate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Int32 patamar1Ate = ((TextBox)sender).Text.ToInt32();
                Int32 limite = ((TextBox)((TextBox)sender).Parent.FindControl("txtCreditoAte")).Text.ToInt32();

                if (patamar1Ate > limite)
                    ((Label)((TextBox)sender).Parent.FindControl("lblCreditoLimiteParcela")).Text = patamar1Ate.ToString();

                Panel patamar2 = (Panel)((TextBox)sender).Parent.FindControl("pnlPatamar2");
                if (patamar2.Visible)
                {
                    Int32 patamar2Ate = ((TextBox)patamar2.FindControl("txtPatamar2Ate")).Text.ToInt32();
                    ((TextBox)patamar2.FindControl("txtPatamar2De")).Text = (patamar1Ate + 1).ToString();

                    if (patamar2Ate > patamar1Ate)
                        ((Label)((TextBox)sender).Parent.FindControl("lblCreditoLimiteParcela")).Text = patamar2Ate.ToString();
                }

                if (patamar1Ate == 0)
                {
                    if (patamar2.Visible && ((TextBox)patamar2.FindControl("txtPatamar2Ate")).Text.ToInt32() == 0)
                        ((Label)((TextBox)sender).Parent.FindControl("lblCreditoLimiteParcela")).Text = limite.ToString();
                    else if (!patamar2.Visible)
                        ((Label)((TextBox)sender).Parent.FindControl("lblCreditoLimiteParcela")).Text = limite.ToString();
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Muda o valor do limite de parcelas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtPatamar2Ate_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //if (!String.IsNullOrEmpty(((TextBox)sender).Text))
                //    ((RequiredFieldValidator)((TextBox)sender).Parent.FindControl("rfvPatamar1Ate")).Visible = true;
                //else
                //    ((RequiredFieldValidator)((TextBox)sender).Parent.FindControl("rfvPatamar1Ate")).Visible = false;

                Int32 patamar2Ate = ((TextBox)sender).Text.ToInt32();
                Int32 limite = ((TextBox)((TextBox)sender).Parent.FindControl("txtPatamar1Ate")).Text.ToInt32();

                if (patamar2Ate > limite)
                    ((Label)((TextBox)sender).Parent.FindControl("lblCreditoLimiteParcela")).Text = patamar2Ate.ToString();

                if (patamar2Ate == 0)
                {
                    Panel patamar1 = (Panel)((TextBox)sender).Parent.FindControl("pnlPatamar1");
                    Int32 patamar1Ate = ((TextBox)patamar1.FindControl("txtPatamar1Ate")).Text.ToInt32();

                    if (patamar1Ate != 0)
                        ((Label)((TextBox)sender).Parent.FindControl("lblCreditoLimiteParcela")).Text = patamar1Ate.ToString();
                    else
                    {
                        Int32 ate = ((TextBox)patamar1.Parent.FindControl("txtCreditoAte")).Text.ToInt32();
                        ((Label)((TextBox)sender).Parent.FindControl("lblCreditoLimiteParcela")).Text = ate.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Evento ao mudar o valor do campo Taxa de Adesão
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtTaxaAdesao_TextChanged(object sender, EventArgs e)
        {
            if (Request.Form.Get("__EVENTTARGET") == txtTaxaAdesao.UniqueID)
                base.ExibirPainelMensagem("A taxa de adesão deve ser cobrada integralmente.");
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Busca lista de tipos de equipamentos e carrega drop down
        /// </summary>
        private void CarregarTipoEquipamento()
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar Tipo de Equipamentos"))
                {

                    Int32 codGrupoRamo = Credenciamento.GrupoRamo;
                    Int32 codRamoAtividade = Credenciamento.RamoAtividade;

                    ListaEquipamentosPorRamoAtividade[] equipamentos = client.ListaEquipamentosPorRamoAtividade(codGrupoRamo, codRamoAtividade);
                    client.Close();

                    ddlTipoEquipamento.Items.Clear();
                    ddlTipoEquipamento.Items.Add("");
                    foreach (ListaEquipamentosPorRamoAtividade equipamento in equipamentos)
                    {
                        ListItem item = new ListItem(equipamento.CodTipoEquipamento, equipamento.CodTipoEquipamento);
                        ddlTipoEquipamento.Items.Add(item);
                    }
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de campanhas e carrega drop down
        /// </summary>
        private void CarregarCampanhas()
        {
            ServicoPortalWFCampanhasClient client = new ServicoPortalWFCampanhasClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar Campanhas"))
                {
                    Int32 codigoCanal = Credenciamento.Canal;
                    Int32 codigoCelula = Credenciamento.Celula;
                    Int32 codigoGrupoRamo = Credenciamento.GrupoRamo;
                    Int32 codRamoAtividade = Credenciamento.RamoAtividade;
                    String codigoCep = Credenciamento.CEP.Replace("-", "");
                    Char codTipoCampanha = 'C';
                    String codigoCampanha = null;
                    String codTipoEquipamento = ddlTipoEquipamento.SelectedValue;

                    ListaCampanhaPorCanalCelulaRamoCep[] campanhas = client.ListaCampanhaPorCanalCelulaRamoCep(codigoCanal, codigoCelula, codigoGrupoRamo, codRamoAtividade, codigoCep, codTipoCampanha, codigoCampanha, codTipoEquipamento);
                    client.Close();

                    ddlCampanha.Items.Clear();
                    ddlCampanha.Items.Add("");
                    foreach (ListaCampanhaPorCanalCelulaRamoCep campanha in campanhas)
                    {
                        ListItem item = new ListItem(campanha.NomeCampanha, campanha.CodigoCampanha);
                        ddlCampanha.Items.Add(item);
                    }

                    if (campanhas.Length > 0)
                        spExistemCampanhas.Visible = true;
                    else
                        spExistemCampanhas.Visible = false;

                }
            }
            catch (FaultException<WFCampanhas.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega dados dos produtos de acordo com a campanha selecionada 
        /// </summary>
        private void CarregarProdutosPorCampanha()
        {
            ServicoPortalWFCampanhasClient client = new ServicoPortalWFCampanhasClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar dados Produtos por Campanha"))
                {
                    if (!String.IsNullOrEmpty(ddlCampanha.SelectedValue))
                    {
                        String codigoCampanha = ddlCampanha.SelectedValue;
                        ListaParametrosCampanha[] parametros = client.ListaParametrosCampanha(codigoCampanha, 'P');
                        client.Close();

                        foreach (ListaParametrosCampanha parametro in parametros)
                        {
                            if (parametro.IndTipoOperacao == 'C')
                            {
                                ProdutosListaDadosProdutosPorRamoCanal produto = Credenciamento.ProdutosCredito.FirstOrDefault(p => p.CodCCA == parametro.CodigoCca && p.CodFeature == parametro.CodigoFeature);

                                if (produto != null)
                                {
                                    produto.ValorTaxaDefault = parametro.ValorTaxaParametro;
                                    produto.ValorPrazoDefault = parametro.PrazoParametro;
                                }
                            }

                            if (parametro.IndTipoOperacao == 'D' && parametro.CodigoCca != 22)
                            {
                                ProdutosListaDadosProdutosPorRamoCanal produto = Credenciamento.ProdutosDebito.FirstOrDefault(p => p.CodCCA == parametro.CodigoCca && p.CodFeature == parametro.CodigoFeature);

                                if (produto != null)
                                {
                                    produto.ValorTaxaDefault = parametro.ValorTaxaParametro;
                                    produto.ValorPrazoDefault = parametro.PrazoParametro;
                                }
                            }

                            if (parametro.IndTipoOperacao == 'D' && parametro.CodigoCca == 22)
                            {
                                ProdutosListaDadosProdutosPorRamoCanal produto = ListaProdutosConstrucard.FirstOrDefault(p => p.CodCCA == parametro.CodigoCca && p.CodFeature == parametro.CodigoFeature);

                                if (produto != null)
                                {
                                    produto.ValorTaxaDefault = parametro.ValorTaxaParametro;
                                    produto.ValorPrazoDefault = parametro.PrazoParametro;
                                }
                            }
                        }

                        if (Credenciamento.ProdutosCredito != null && Credenciamento.ProdutosCredito.Count > 0)
                        {
                            var produtos = (from p in Credenciamento.ProdutosCredito
                                            group p by p.CodFeature
                                                into grp
                                                select grp.First()).ToArray();

                            rptProdutosCredito.DataSource = produtos;
                            rptProdutosCredito.DataBind();
                        }

                        if (Credenciamento.ProdutosDebito != null && Credenciamento.ProdutosDebito.Count > 0)
                        {
                            var produtosDebito = (from p in Credenciamento.ProdutosDebito
                                                  group p by p.CodFeature
                                                      into grp
                                                      select grp.First()).ToArray();

                            rptProdutosDebito.DataSource = produtosDebito;
                            rptProdutosDebito.DataBind();
                        }

                        if (ListaProdutosConstrucard != null && ListaProdutosConstrucard.Count > 0)
                        {
                            rptProdutosConstrucard.DataSource = ListaProdutosConstrucard;
                            rptProdutosConstrucard.DataBind();
                        }
                    }

                }
            }
            catch (FaultException<WFCampanhas.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega a taxa de adesão de acordo com a campanha selecionada
        /// </summary>
        private void CarregarTaxaAdesaoPorCampanha()
        {
            ServicoPortalWFCampanhasClient client = new ServicoPortalWFCampanhasClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar Taxa Adesão por Campanha"))
                {
                    if (!String.IsNullOrEmpty(ddlCampanha.SelectedValue))
                    {
                        String codigoCampanha = ddlCampanha.SelectedValue;
                        ListaParametrosCampanha[] parametros = client.ListaParametrosCampanha(codigoCampanha, 'X');
                        client.Close();

                        txtTaxaAdesao.Text = parametros.Length > 0 ? String.Format("{0:C}", parametros[0].ValorTaxaParametro).Replace("R$", "") : txtTaxaAdesao.Text;
                    }
                }
            }
            catch (FaultException<WFCampanhas.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega o cenário de acordo com a campanha selecionada
        /// </summary>
        private void CarregaCenarioPorCampanha()
        {
            ServicoPortalWFCampanhasClient client = new ServicoPortalWFCampanhasClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar Cenário por Campanha"))
                {
                    if (!String.IsNullOrEmpty(ddlCampanha.SelectedValue))
                    {
                        String codigoCampanha = ddlCampanha.SelectedValue;
                        ListaParametrosCampanha[] parametros = client.ListaParametrosCampanha(codigoCampanha, 'T');
                        client.Close();

                        String tipoEquipamento = ddlTipoEquipamento.SelectedValue;
                        foreach (ListaParametrosCampanha parametro in parametros)
                        {
                            if (parametro.CodTipoEquipamento == tipoEquipamento)
                                if (parametro.CodigoCenario != null)
                                    CarregarCenarios(parametro.CodigoCenario);
                                else if (parametro.ValorTaxaParametro != null)
                                    txtValorAluguel.Text = String.Format("{0:f2}", parametro.ValorTaxaParametro);
                        }
                    }

                }
            }
            catch (FaultException<WFCampanhas.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca a taxa de adesão e carrega o textbox
        /// </summary>
        private void CarregarTaxaAdesao()
        {
            ServicoPortalGETaxaFiliacaoClient client = new ServicoPortalGETaxaFiliacaoClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados Credenciamento - Carregar Taxa de Adesão"))
                {
                    TaxaFiliacaoConsultaValorTaxaFiliacao[] taxas = client.ConsultaValorTaxaFiliacao();
                    client.Close();
                    txtTaxaAdesao.Text = taxas.Length > 0 ? String.Format("{0:C}", taxas[0].ValorParametro).Replace("R$", "") : String.Empty;
                }
            }
            catch (FaultException<GETaxaFiliacao.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de cenários e carrega o drop down
        /// </summary>
        private void CarregarCenarios(Int32? _codCenario)
        {
            if (String.Compare(ddlTipoEquipamento.SelectedValue, "TOL") == 0 ||
                        String.Compare(ddlTipoEquipamento.SelectedValue, "SNT") == 0 ||
                        String.Compare(ddlTipoEquipamento.SelectedValue, "TOF") == 0)
            {
                ddlCenario.SelectedValue = String.Empty;
                ddlCenario.Enabled = false;
            }
            else
            {
                ddlCenario.Enabled = true;
                ServicoPortalTGCenariosClient client = new ServicoPortalTGCenariosClient();

                try
                {
                    using (Logger log = Logger.IniciarLog("Carregar Lista de Cenários"))
                    {
                        Int32? codCenario = _codCenario;
                        Int32 codCanal = Credenciamento.Canal;
                        String codTipoEquipamento = ddlTipoEquipamento.SelectedValue;
                        Char codSituacaoCenarioCanal = 'A';
                        String codCampanha = !String.IsNullOrEmpty(ddlCampanha.SelectedValue) ? ddlCampanha.SelectedValue : null;
                        String codOrigemChamada = null;

                        Cenarios[] cenarios = client.ListaDadosCadastrais(codCenario, codCanal, codTipoEquipamento, codSituacaoCenarioCanal, codCampanha, codOrigemChamada);
                        client.Close();

                        log.GravarMensagem(String.Format("Cenários lenght: {0}", cenarios.Length));
                        ddlCenario.Items.Clear();
                        ddlCenario.Items.Add("");
                        foreach (Cenarios cenario in cenarios)
                        {
                            log.GravarMensagem(String.Format("Descrição Cenário: {0}", cenario.DescricaoCenario));
                            log.GravarMensagem(String.Format("Código Cenário: {0}", cenario.CodigoCenario));
                            ListItem item = new ListItem(cenario.DescricaoCenario, cenario.CodigoCenario.ToString());
                            ddlCenario.Items.Add(item);

                            if (_codCenario != null)
                                ddlCenario.SelectedValue = cenario.CodigoCenario.ToString();
                        }

                        if (_codCenario != null)
                            ddlCenario.Enabled = false;
                        else
                            ddlCenario.Enabled = true;

                    }
                }
                catch (FaultException<TGCenarios.ModelosErroServicos> fe)
                {
                    client.Abort();
                    Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
                }
                catch (TimeoutException te)
                {
                    client.Abort();
                    Logger.GravarErro("Credenciamento - Dados Negócios", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecao(te.Message, 300);
                }
                catch (CommunicationException ce)
                {
                    client.Abort();
                    Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecao(ce.Message, 300);
                }
                catch (Exception ex)
                {
                    client.Abort();
                    Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Busca informações detalhadas do cenário escolhido e preenche os controles
        /// </summary>
        private void CarregarCenario()
        {
            ServicoPortalTGCenariosClient client = new ServicoPortalTGCenariosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados do cenário"))
                {
                    Int32 codCenario = ddlCenario.SelectedValue.ToInt32();
                    Int32 codCanal = Credenciamento.Canal;
                    String codTipoEquipamento = ddlTipoEquipamento.SelectedValue;
                    Char codSituacaoCenarioCanal = 'A';
                    String codCampanha = !String.IsNullOrEmpty(ddlCampanha.SelectedValue) ? ddlCampanha.SelectedValue : null;
                    String codOrigemChamada = null;

                    Cenarios[] cenarios = client.ListaDadosCadastrais(codCenario, codCanal, codTipoEquipamento, codSituacaoCenarioCanal, codCampanha, codOrigemChamada);
                    client.Close();

                    Double aluguel = txtValorAluguel.Text.Replace("R$", "").ToDouble();

                    if (cenarios.Length > 0)
                    {
                        lblDescontoPor1.Text = cenarios[0].ValorEscalonamentoMes1 != null ? String.Format("{0}%", cenarios[0].ValorEscalonamentoMes1) : "-";
                        lblDescontoPor2.Text = cenarios[0].ValorEscalonamentoMes2 != null ? String.Format("{0}%", cenarios[0].ValorEscalonamentoMes2) : "-";
                        lblDescontoPor3.Text = cenarios[0].ValorEscalonamentoMes3 != null ? String.Format("{0}%", cenarios[0].ValorEscalonamentoMes3) : "-";
                        lblDescontoPor4.Text = cenarios[0].ValorEscalonamentoMes4 != null ? String.Format("{0}%", cenarios[0].ValorEscalonamentoMes4) : "-";
                        lblDescontoPor5.Text = cenarios[0].ValorEscalonamentoMes5 != null ? String.Format("{0}%", cenarios[0].ValorEscalonamentoMes5) : "-";
                        lblDescontoPor6.Text = cenarios[0].ValorEscalonamentoMes6 != null ? String.Format("{0}%", cenarios[0].ValorEscalonamentoMes6) : "-";
                        lblDescontoPor7.Text = cenarios[0].ValorEscalonamentoMes7 != null ? String.Format("{0}%", cenarios[0].ValorEscalonamentoMes7) : "-";
                        lblDescontoPor8.Text = cenarios[0].ValorEscalonamentoMes8 != null ? String.Format("{0}%", cenarios[0].ValorEscalonamentoMes8) : "-";
                        lblDescontoPor9.Text = cenarios[0].ValorEscalonamentoMes9 != null ? String.Format("{0}%", cenarios[0].ValorEscalonamentoMes9) : "-";
                        lblDescontoPor10.Text = cenarios[0].ValorEscalonamentoMes10 != null ? String.Format("{0}%", cenarios[0].ValorEscalonamentoMes10) : "-";
                        lblDescontoPor11.Text = cenarios[0].ValorEscalonamentoMes11 != null ? String.Format("{0}%", cenarios[0].ValorEscalonamentoMes11) : "-";
                        lblDescontoPor12.Text = cenarios[0].ValorEscalonamentoMes12 != null ? String.Format("{0}%", cenarios[0].ValorEscalonamentoMes12) : "-";

                        lblDescontoVal1.Text = cenarios[0].ValorEscalonamentoMes1 != null ? String.Format("{0:0.00}", (aluguel * cenarios[0].ValorEscalonamentoMes1 / 100)) : "-";
                        lblDescontoVal2.Text = cenarios[0].ValorEscalonamentoMes2 != null ? String.Format("{0:0.00}", (aluguel * cenarios[0].ValorEscalonamentoMes2 / 100)) : "-";
                        lblDescontoVal3.Text = cenarios[0].ValorEscalonamentoMes3 != null ? String.Format("{0:0.00}", (aluguel * cenarios[0].ValorEscalonamentoMes3 / 100)) : "-";
                        lblDescontoVal4.Text = cenarios[0].ValorEscalonamentoMes4 != null ? String.Format("{0:0.00}", (aluguel * cenarios[0].ValorEscalonamentoMes4 / 100)) : "-";
                        lblDescontoVal5.Text = cenarios[0].ValorEscalonamentoMes5 != null ? String.Format("{0:0.00}", (aluguel * cenarios[0].ValorEscalonamentoMes5 / 100)) : "-";
                        lblDescontoVal6.Text = cenarios[0].ValorEscalonamentoMes6 != null ? String.Format("{0:0.00}", (aluguel * cenarios[0].ValorEscalonamentoMes6 / 100)) : "-";
                        lblDescontoVal7.Text = cenarios[0].ValorEscalonamentoMes7 != null ? String.Format("{0:0.00}", (aluguel * cenarios[0].ValorEscalonamentoMes7 / 100)) : "-";
                        lblDescontoVal8.Text = cenarios[0].ValorEscalonamentoMes8 != null ? String.Format("{0:0.00}", (aluguel * cenarios[0].ValorEscalonamentoMes8 / 100)) : "-";
                        lblDescontoVal9.Text = cenarios[0].ValorEscalonamentoMes9 != null ? String.Format("{0:0.00}", (aluguel * cenarios[0].ValorEscalonamentoMes9 / 100)) : "-";
                        lblDescontoVal10.Text = cenarios[0].ValorEscalonamentoMes10 != null ? String.Format("{0:0.00}", (aluguel * cenarios[0].ValorEscalonamentoMes10 / 100)) : "-";
                        lblDescontoVal11.Text = cenarios[0].ValorEscalonamentoMes11 != null ? String.Format("{0:0.00}", (aluguel * cenarios[0].ValorEscalonamentoMes11 / 100)) : "-";
                        lblDescontoVal12.Text = cenarios[0].ValorEscalonamentoMes12 != null ? String.Format("{0:0.00}", (aluguel * cenarios[0].ValorEscalonamentoMes12 / 100)) : "-";

                        lblSaz1.Text = cenarios[0].PercentualSazonalidadeJan != null ? String.Format("{0:0.00}", cenarios[0].PercentualSazonalidadeJan) : "-";
                        lblSaz2.Text = cenarios[0].PercentualSazonalidadeFev != null ? String.Format("{0:0.00}", cenarios[0].PercentualSazonalidadeFev) : "-";
                        lblSaz3.Text = cenarios[0].PercentualSazonalidadeMar != null ? String.Format("{0:0.00}", cenarios[0].PercentualSazonalidadeMar) : "-";
                        lblSaz4.Text = cenarios[0].PercentualSazonalidadeAbr != null ? String.Format("{0:0.00}", cenarios[0].PercentualSazonalidadeAbr) : "-";
                        lblSaz5.Text = cenarios[0].PercentualSazonalidadeMai != null ? String.Format("{0:0.00}", cenarios[0].PercentualSazonalidadeMai) : "-";
                        lblSaz6.Text = cenarios[0].PercentualSazonalidadeJun != null ? String.Format("{0:0.00}", cenarios[0].PercentualSazonalidadeJun) : "-";
                        lblSaz7.Text = cenarios[0].PercentualSazonalidadeJul != null ? String.Format("{0:0.00}", cenarios[0].PercentualSazonalidadeJul) : "-";
                        lblSaz8.Text = cenarios[0].PercentualSazonalidadeAgo != null ? String.Format("{0:0.00}", cenarios[0].PercentualSazonalidadeAgo) : "-";
                        lblSaz9.Text = cenarios[0].PercentualSazonalidadeSet != null ? String.Format("{0:0.00}", cenarios[0].PercentualSazonalidadeSet) : "-";
                        lblSaz10.Text = cenarios[0].PercentualSazonalidadeOut != null ? String.Format("{0:0.00}", cenarios[0].PercentualSazonalidadeOut) : "-";
                        lblSaz11.Text = cenarios[0].PercentualSazonalidadeNov != null ? String.Format("{0:0.00}", cenarios[0].PercentualSazonalidadeNov) : "-";
                        lblSaz12.Text = cenarios[0].PercentualSazonalidadeDez != null ? String.Format("{0:0.00}", cenarios[0].PercentualSazonalidadeDez) : "-";
                    }
                    else
                    {
                        lblDescontoPor1.Text = "0%";
                        lblDescontoPor2.Text = "0%";
                        lblDescontoPor3.Text = "0%";
                        lblDescontoPor4.Text = "0%";
                        lblDescontoPor5.Text = "0%";
                        lblDescontoPor6.Text = "0%";
                        lblDescontoPor7.Text = "0%";
                        lblDescontoPor8.Text = "0%";
                        lblDescontoPor9.Text = "0%";
                        lblDescontoPor10.Text = "0%";
                        lblDescontoPor11.Text = "0%";
                        lblDescontoPor12.Text = "0%";

                        lblDescontoVal1.Text = "0,00";
                        lblDescontoVal2.Text = "0,00";
                        lblDescontoVal3.Text = "0,00";
                        lblDescontoVal4.Text = "0,00";
                        lblDescontoVal5.Text = "0,00";
                        lblDescontoVal6.Text = "0,00";
                        lblDescontoVal7.Text = "0,00";
                        lblDescontoVal8.Text = "0,00";
                        lblDescontoVal9.Text = "0,00";
                        lblDescontoVal10.Text = "0,00";
                        lblDescontoVal11.Text = "0,00";
                        lblDescontoVal12.Text = "0,00";

                        lblSaz1.Text = "0,00";
                        lblSaz2.Text = "0,00";
                        lblSaz3.Text = "0,00";
                        lblSaz4.Text = "0,00";
                        lblSaz5.Text = "0,00";
                        lblSaz6.Text = "0,00";
                        lblSaz7.Text = "0,00";
                        lblSaz8.Text = "0,00";
                        lblSaz9.Text = "0,00";
                        lblSaz10.Text = "0,00";
                        lblSaz11.Text = "0,00";
                        lblSaz12.Text = "0,00";
                    }

                }
            }
            catch (FaultException<TGTipoEquip.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca o valor do aluguel de um equipamento e carrega o controle
        /// </summary>
        private void CarregarValorAluguel()
        {
            if (String.Compare(ddlTipoEquipamento.SelectedValue, "TOL") == 0 ||
                        String.Compare(ddlTipoEquipamento.SelectedValue, "SNT") == 0 ||
                        String.Compare(ddlTipoEquipamento.SelectedValue, "TOF") == 0)
            {
                txtValorAluguel.Text = "0,01";
                txtValorAluguel.Enabled = false;
            }
            else
            {
                ServicoPortalTGTipoEquipamentoClient client = new ServicoPortalTGTipoEquipamentoClient();

                try
                {
                    using (Logger log = Logger.IniciarLog("Carregar Tipos de Equipamento"))
                    {
                        String codTipoEquipamento = ddlTipoEquipamento.SelectedValue;

                        TipoEquipamento[] tiposEquipamento = client.ListaDadosCadastrais(codTipoEquipamento, 'A', 'N');
                        client.Close();

                        if (tiposEquipamento.Length > 0)
                        {
                            txtValorAluguel.Text = tiposEquipamento.Length > 0 ? String.Format("{0:f2}", tiposEquipamento[0].ValorDefaultAluguel) : String.Empty;

                            Credenciamento.CodMascaraTnms = tiposEquipamento[0].CodMascaraTnms;
                            Credenciamento.CodTipoEquipamento = tiposEquipamento[0].CodTipoEquipamento;
                            Credenciamento.DataUltimaAtualizacao = tiposEquipamento[0].DataUltimaAtualizacao;
                            Credenciamento.IndFaturavel = tiposEquipamento[0].IndFaturavel;
                            Credenciamento.IndGeraFCT = tiposEquipamento[0].IndGeraFCT;
                            Credenciamento.IndGeraOS = tiposEquipamento[0].IndGeraOS;
                            Credenciamento.IndPermAltEndereco = tiposEquipamento[0].IndPermAltEndereco;
                            Credenciamento.IndTecnologiaCompartilhada = tiposEquipamento[0].IndTecnologiaCompartilhada;
                            Credenciamento.IndVendaDigitadaReceptivo = tiposEquipamento[0].IndVendaDigitadaReceptivo;
                            Credenciamento.IndVendaTelemarketing = tiposEquipamento[0].IndVendaTelemarketing;
                            Credenciamento.NomeTipoEquipamento = tiposEquipamento[0].NomeTipoEquipamento;
                            Credenciamento.SituacaoTipoEquipamento = tiposEquipamento[0].SituacaoTipoEquipamento;
                            Credenciamento.TimeStampCanal = tiposEquipamento[0].TimeStampCanal;
                            Credenciamento.UsuarioUltimaAtualizacao = tiposEquipamento[0].UsuarioUltimaAtualizacao;
                            Credenciamento.ValorDefaultAluguel = tiposEquipamento[0].ValorDefaultAluguel;
                            Credenciamento.ValorMinimoAluguel = tiposEquipamento[0].ValorMinimoAluguel;
                            Credenciamento.ValorMáximoAluguel = tiposEquipamento[0].ValorMáximoAluguel;
                        }
                        else
                            txtValorAluguel.Text = String.Format("{0:f2}", 0);

                        //if (Credenciamento.Canal == 2 && String.IsNullOrEmpty(ddlCenario.SelectedValue))
                        //    txtValorAluguel.Enabled = true;
                        //else
                        //    txtValorAluguel.Enabled = false;

                    }
                }
                catch (FaultException<TGTipoEquip.ModelosErroServicos> fe)
                {
                    Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
                }
                catch (TimeoutException te)
                {
                    client.Abort();
                    Logger.GravarErro("Credenciamento - Dados Negócios", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecao(te.Message, 300);
                }
                catch (CommunicationException ce)
                {
                    client.Abort();
                    Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecao(ce.Message, 300);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Carrega dados da grid de vendas no cartão de crédito
        /// </summary>
        private void CarregarProdutosCredito()
        {
            try
            {
                Int32 grupoRamoMatriz = 0;
                Int32 ramoAtividadeMatriz = 0;

                if (Credenciamento.CodTipoEstabelecimento == 1)
                    BuscaRamoAtividadeMatriz(Credenciamento.NumPdvMatriz, out grupoRamoMatriz, out ramoAtividadeMatriz);

                if (grupoRamoMatriz != Credenciamento.GrupoRamo || ramoAtividadeMatriz != Credenciamento.RamoAtividade)
                {
                    Credenciamento.ProdutosCredito = ListaDadosProdutosPorRamoCanal('S', null, null, null).ToList();
                }
                else
                {
                    if (Credenciamento.CodTipoEstabelecimento == 1)
                    {
                        if (Credenciamento.Patamares != null)
                            Credenciamento.Patamares.Clear();

                        Credenciamento.ProdutosCredito = ListaDadosProdutosPorPontoDeVenda((Int32)Credenciamento.NumPdvMatriz, 'C');
                    }
                }

                var produtos = (from p in Credenciamento.ProdutosCredito
                                group p by p.CodFeature
                                    into grp
                                    select grp.First()).ToArray();

                if (produtos.Count() > 0)
                {
                    rptProdutosCredito.DataSource = produtos;
                    rptProdutosCredito.DataBind();
                }
                else
                    pnlProdutosCredito.Visible = false;
            }
            catch (FaultException<GEProdutos.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, CODIGO_ERRO);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega dados da grid de vendas no cartão de débito
        /// </summary>
        private void CarregarProdutosDebito()
        {
            try
            {
                Int32 grupoRamoMatriz = 0;
                Int32 ramoAtividadeMatriz = 0;

                if (Credenciamento.CodTipoEstabelecimento == 1)
                    BuscaRamoAtividadeMatriz(Credenciamento.NumPdvMatriz, out grupoRamoMatriz, out ramoAtividadeMatriz);

                if (grupoRamoMatriz != Credenciamento.GrupoRamo || ramoAtividadeMatriz != Credenciamento.RamoAtividade)
                    Credenciamento.ProdutosDebito = ListaDadosProdutosPorRamoCanal(null, 'S', null, null).ToList().FindAll(p => p.CodCCA != 22 || p.CodFeature != 21);
                else
                    Credenciamento.ProdutosDebito = ListaDadosProdutosPorPontoDeVenda((Int32)Credenciamento.NumPdvMatriz, 'D').FindAll(p => p.CodCCA != 22);

                var produtos = (from p in Credenciamento.ProdutosDebito
                                group p by p.CodFeature
                                    into grp
                                    select grp.First()).ToArray();

                if (produtos.Count() > 0)
                {
                    rptProdutosDebito.DataSource = produtos;
                    rptProdutosDebito.DataBind();
                }
                else
                    pnlProdutosDebito.Visible = false;
            }
            catch (FaultException<GEProdutos.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega dados da grid de vendas no cartão construcard
        /// </summary>
        private void CarregarProdutosConstrucard()
        {
            try
            {
                Int32 grupoRamoMatriz = 0;
                Int32 ramoAtividadeMatriz = 0;

                if (Credenciamento.CodTipoEstabelecimento == 1)
                    BuscaRamoAtividadeMatriz(Credenciamento.NumPdvMatriz, out grupoRamoMatriz, out ramoAtividadeMatriz);

                if (grupoRamoMatriz != Credenciamento.GrupoRamo || ramoAtividadeMatriz != Credenciamento.RamoAtividade)
                    ListaProdutosConstrucard = ListaDadosProdutosPorRamoCanal(null, 'S', null, null).ToList().FindAll(p => p.CodCCA == 22 && p.CodFeature == 21);
                else
                    ListaProdutosConstrucard = ListaDadosProdutosPorPontoDeVenda((Int32)Credenciamento.NumPdvMatriz, 'D').FindAll(p => p.CodCCA == 22);

                if (Credenciamento.ProdutosConstrucard != null && Credenciamento.ProdutosConstrucard.Count > 0)
                {
                    chkHabilitarConstrucard.Checked = true;
                    pnlConstrucard.Visible = true;
                }
                else
                {
                    chkHabilitarConstrucard.Checked = false;
                }

                if (ListaProdutosConstrucard.Count == 0)
                {
                    pnlConstrucard.Visible = false;
                    chkHabilitarConstrucard.Visible = false;
                }

                rptProdutosConstrucard.DataSource = ListaProdutosConstrucard;
                rptProdutosConstrucard.DataBind();
            }
            catch (FaultException<GEProdutos.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de produtos
        /// </summary>
        private ProdutosListaDadosProdutosPorRamoCanal[] ListaDadosProdutosPorRamoCanal(Char? indicadorCredito, Char? indicadorDebito, Char? indicadorVoucher, Char? indicadorPrivate)
        {
            ServicoPortalGEProdutosClient client = new ServicoPortalGEProdutosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Busca Produtos por Ramo Canal"))
                {
                    Int32 codGrupoRamo = Credenciamento.GrupoRamo;
                    String codRamoAtividade = Credenciamento.RamoAtividade.ToString();
                    Int32 codCanalOrigem = Credenciamento.Canal;

                    ProdutosListaDadosProdutosPorRamoCanal[] retorno = client.ListaDadosProdutosPorRamoCanal(
                        indicadorCredito,
                        indicadorDebito,
                        indicadorVoucher,
                        indicadorPrivate,
                        codGrupoRamo,
                        codRamoAtividade,
                        codCanalOrigem);
                    client.Close();

                    if (Credenciamento.Patamares == null)
                        Credenciamento.Patamares = new List<Modelo.Patamar>();

                    Credenciamento.Patamares.Clear();

                    foreach (var produto in retorno.Where(p => p.CodFeature == 3))
                    {
                        Credenciamento.Patamares.Add(new Modelo.Patamar
                        {
                            CodCca = (Int32)produto.CodCCA,
                            CodFeature = (Int32)produto.CodFeature,
                            PatamarInicial = 2,
                            PatamarFinal = produto.QtdeDefaultParcela,
                            SequenciaPatamar = 1,
                            TaxaPatamar = produto.ValorTaxaDefault,
                            Prazo = (Int32)produto.ValorPrazoDefault
                        });    
                    }
                    

                    return retorno;
                }
            }
            catch (FaultException<GEProdutos.ModelosErroServicos> fe)
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

        /// <summary>
        /// Grava ou atualiza dados da terceira tela
        /// </summary>
        /// <returns></returns>
        private Int32 GravarAtualizarPasso3()
        {
            TransicoesClient client = new TransicoesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Gravar Dados - Dados Negócio"))
                {
                    PNTransicoesServico.Proposta proposta = PreencheProposta();
                    List<PNTransicoesServico.Produto> produtosCredito = PreencheListaProdutos(Credenciamento.ProdutosCredito);
                    List<PNTransicoesServico.Produto> produtosDebito = PreencheListaProdutos(Credenciamento.ProdutosDebito);
                    List<PNTransicoesServico.Produto> produtosConstrucard = PreencheListaProdutos(Credenciamento.ProdutosConstrucard);
                    List<PNTransicoesServico.Patamar> patamares = PreencheListaPatamares(Credenciamento.Patamares);
                    PNTransicoesServico.Tecnologia tecnologia = PreencheTecnologia();

                    Int32 retorno = client.GravarAtualizarPasso3(proposta, produtosCredito, produtosDebito, produtosConstrucard, patamares, tecnologia);
                    client.Close();

                    return retorno;
                }
            }
            catch (FaultException<PNTransicoesServico.GeneralFault> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (FaultException<GERegimes.ModelosErroServicos> fe)
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
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }

        /// <summary>
        /// Salava dados da tela
        /// </summary>
        /// <returns></returns>
        private Int32 SalvarDados()
        {
            try
            {
                Page.Validate();
                if (Page.IsValid)
                {
                    if (Credenciamento.CodTipoEstabelecimento == 1)
                    {
                        Int32 grupoRamoMatriz = 0;
                        Int32 ramoAtividadeMatriz = 0;

                        BuscaRamoAtividadeMatriz(Credenciamento.NumPdvMatriz, out grupoRamoMatriz, out ramoAtividadeMatriz);

                        if (Credenciamento.GrupoRamo != grupoRamoMatriz || Credenciamento.RamoAtividade != ramoAtividadeMatriz)
                        {
                            if (Credenciamento.Patamares != null)
                                Credenciamento.Patamares.Clear();

                            Credenciamento.Patamares = GetPatamares();
                        }
                    }
                    else
                    {
                        if (Credenciamento.Patamares != null)
                            Credenciamento.Patamares.Clear();

                        Credenciamento.Patamares = GetPatamares();
                    }

                    Int32 qdteLimiteParcelas = 0;
                    foreach (RepeaterItem item in rptProdutosCredito.Items)
                    {
                        Int32 codFeature = ((HiddenField)item.FindControl("hiddenCodFeature")).Value.ToInt32();
                        if (codFeature == 3)
                            qdteLimiteParcelas = ((Label)item.FindControl("lblCreditoLimiteParcela")).Text.ToInt32();
                    }

                    foreach (var produto in Credenciamento.ProdutosCredito.FindAll(p => p.CodFeature == 3))
                    {
                        //produto.QtdeMaximaParcela = qdteLimiteParcelas;
                        produto.QtdeMaximaPatamar = qdteLimiteParcelas;
                    }

                    Credenciamento.TipoEquipamento = ddlTipoEquipamento.SelectedValue;
                    Credenciamento.TaxaAdesao = txtTaxaAdesao.Text.Replace("R$", "").Replace(".", "").ToDouble();
                    Credenciamento.CodCenario = ddlCenario.SelectedValue.ToInt32Null();
                    Credenciamento.CodCampanha = ddlCampanha.SelectedValue;
                    Credenciamento.ValorAluguel = Credenciamento.CodCenario == null ? txtValorAluguel.Text.Replace("R$", "").Replace(".", "").ToDouble() : 0;
                    Credenciamento.ValorAluguelTela = txtValorAluguel.Text.Replace("R$", "").Replace(".", "").ToDouble();

                    if (chkHabilitarConstrucard.Checked)
                    {
                        Credenciamento.ProdutosConstrucard = ListaProdutosConstrucard;
                    }
                    else if (Credenciamento.ProdutosConstrucard != null)
                    {
                        Credenciamento.ProdutosConstrucard.Clear();
                    }

                    var retorno = RemoveTodosProdutos();

                    if (retorno.CodigoErro == 0)
                        return GravarAtualizarPasso3();
                    else
                        return (Int32)retorno.CodigoErro;
                }

                return 399;
            }
            catch (FaultException<WFProdutos.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
                return (Int32)fe.Detail.CodigoErro;
            }
            catch (FaultException<PNTransicoesServico.GeneralFault> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                return fe.Detail.Codigo;
            }
            catch (FaultException<GERegimes.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Cliente", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodErro.ToString());
                return (Int32)fe.Detail.CodErro;
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
                return 300;
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
                return 300;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return CODIGO_ERRO;
            }
        }

        private WFProdutos.RetornoErro RemoveTodosProdutos()
        {
            ServicoPortalWFProdutosClient client = new ServicoPortalWFProdutosClient();

            using (Logger log = Logger.IniciarLog("Apaga todos os produtos"))
            {
                try
                {
                    Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJ;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        numCNPJ = Credenciamento.CNPJ.CpfCnpjToLong();
                    else
                        numCNPJ = Credenciamento.CPF.CpfCnpjToLong();

                    Int32 numSeqProp = (Int32)Credenciamento.NumSequencia;

                    var retorno = client.ExclusaoTodosProduto(codTipoPessoa, numCNPJ, numSeqProp);
                    client.Close();

                    return retorno;
                }
                catch (FaultException<WFProdutos.ModelosErroServicos> fe)
                {
                    client.Close();
                    throw fe;
                }
                catch (TimeoutException te)
                {
                    client.Close();
                    throw te;
                }
                catch (CommunicationException ce)
                {
                    client.Close();
                    throw ce;
                }
                catch (Exception ex)
                {
                    client.Close();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Retorna lista de patamares da grid
        /// </summary>
        /// <returns></returns>
        private List<Modelo.Patamar> GetPatamares()
        {
            using (Logger log = Logger.IniciarLog("Recuperar patamares"))
            {
                List<Modelo.Patamar> patamares = new List<Modelo.Patamar>();

                Int32?[] codigosCca = (from p in Credenciamento.ProdutosCredito
                                       where p.CodFeature == 3
                                       select p.CodCCA).Distinct().ToArray();

                foreach (Int32? codCca in codigosCca)
                {
                    foreach (RepeaterItem item in rptProdutosCredito.Items)
                    {
                        if (((HiddenField)item.FindControl("hiddenCodFeature")).Value.ToInt32() == 3)
                        {
                            Modelo.Patamar patamar1 = new Modelo.Patamar();
                            patamar1.CodCca = (Int32)codCca;
                            patamar1.CodFeature = ((HiddenField)item.FindControl("hiddenCodFeature")).Value.ToInt32();
                            patamar1.PatamarInicial = ((TextBox)item.FindControl("txtCreditoDe")).Text.ToInt32();
                            patamar1.PatamarFinal = ((TextBox)item.FindControl("txtCreditoAte")).Text.ToInt32();
                            patamar1.SequenciaPatamar = 1;
                            patamar1.TaxaPatamar = ((Label)item.FindControl("lblCreditoTaxa")).Text.Replace(".", "").ToDouble();
                            patamar1.Prazo = ((Label)item.FindControl("lblCreditoPrazoRecebimento")).Text.Replace("dia(s)", "").ToInt32();

                            Logger.GravarLog(String.Format("CodCca: {0}", patamar1.CodCca));
                            Logger.GravarLog(String.Format("CodFeature: {0}", patamar1.CodFeature));
                            Logger.GravarLog(String.Format("PatamarInicial: {0}", patamar1.PatamarInicial));
                            Logger.GravarLog(String.Format("PatamarFinal: {0}", patamar1.PatamarFinal));
                            Logger.GravarLog(String.Format("SequenciaPatamar: {0}", patamar1.SequenciaPatamar));
                            Logger.GravarLog(String.Format("TaxaPatamar: {0}", patamar1.TaxaPatamar));
                            Logger.GravarLog(String.Format("Prazo: {0}", patamar1.Prazo));

                            patamares.Add(patamar1);
                        }

                        Panel pnlPatamar1 = (Panel)item.FindControl("pnlPatamar1");
                        if (pnlPatamar1.Visible)
                        {
                            if (!String.IsNullOrEmpty(((TextBox)pnlPatamar1.FindControl("txtPatamar1Ate")).Text))
                            {
                                Modelo.Patamar patamar = new Modelo.Patamar();
                                patamar.CodCca = (Int32)codCca;
                                patamar.CodFeature = ((HiddenField)item.FindControl("hiddenCodFeature")).Value.ToInt32();
                                patamar.PatamarInicial = ((TextBox)pnlPatamar1.FindControl("txtPatamar1De")).Text.ToInt32Null();
                                patamar.PatamarFinal = ((TextBox)pnlPatamar1.FindControl("txtPatamar1Ate")).Text.ToInt32Null();
                                patamar.SequenciaPatamar = 2;
                                patamar.TaxaPatamar = ((TextBox)pnlPatamar1.FindControl("txtPatamar1Taxa")).Text.Replace(".", "").ToDouble();
                                patamar.Prazo = ((Label)item.FindControl("lblCreditoPrazoRecebimento")).Text.Replace("dia(s)", "").ToInt32();

                                patamares.Add(patamar);
                            }
                        }

                        if (item.FindControl("pnlPatamar2").Visible)
                        {
                            if (!String.IsNullOrEmpty(((TextBox)pnlPatamar1.FindControl("txtPatamar2Ate")).Text))
                            {
                                Modelo.Patamar patamar = new Modelo.Patamar();

                                patamar.CodCca = (Int32)codCca;
                                patamar.CodFeature = ((HiddenField)item.FindControl("hiddenCodFeature")).Value.ToInt32(); ;
                                patamar.PatamarInicial = ((TextBox)pnlPatamar1.FindControl("txtPatamar2De")).Text.ToInt32Null();
                                patamar.PatamarFinal = ((TextBox)pnlPatamar1.FindControl("txtPatamar2Ate")).Text.ToInt32Null();
                                patamar.SequenciaPatamar = 3;
                                patamar.TaxaPatamar = ((TextBox)pnlPatamar1.FindControl("txtPatamar2Taxa")).Text.Replace(".", "").ToDouble();
                                patamar.Prazo = ((Label)item.FindControl("lblCreditoPrazoRecebimento")).Text.Replace("dia(s)", "").ToInt32();

                                patamares.Add(patamar);
                            }
                        }
                    }
                }

                return patamares;
            }
        }

        /// <summary>
        /// Excluí um produto da base WF
        /// </summary>
        /// <param name="produto"></param>
        /// <param name="tipoOperacao"></param>
        private WFProdutos.RetornoErro RemoveProduto(ProdutosListaDadosProdutosPorRamoCanal produto, Char tipoOperacao)
        {
            ServicoPortalWFProdutosClient client = new ServicoPortalWFProdutosClient();

            try
            {
                Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];

                Int64 numCNPJ = 0;
                if (Credenciamento.TipoPessoa == "J")
                    Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                else
                    Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                Int32 numSeqProp = (Int32)Credenciamento.NumSequencia;
                Int32 codCca = (Int32)produto.CodCCA;
                Char? indTipoOperacaoProd = tipoOperacao;
                Int32? codFeature = produto.CodFeature;
                Char? tipoRegimeNegociado = null;
                Int32? codRegimePadrao = null;
                Int32? prazoPadrao = null;
                Double? taxaPadrao = null;
                Int32? codRegimeMinimo = null;
                Int32? prazoMinimo = null;
                Double? taxaMinimo = null;
                Char? indAceitaFeature = null;
                String usuario = null;
                Double? valorLimiteParcela = null;
                Char? indFormaPagamento = null;

                WFProdutos.RetornoErro[] erros = client.ExclusaoProduto(
                    codTipoPessoa,
                    numCNPJ,
                    numSeqProp,
                    codCca,
                    indTipoOperacaoProd,
                    codFeature,
                    tipoRegimeNegociado,
                    codRegimePadrao,
                    prazoPadrao,
                    taxaPadrao,
                    codRegimeMinimo,
                    prazoMinimo,
                    taxaMinimo,
                    indAceitaFeature,
                    usuario,
                    valorLimiteParcela,
                    indFormaPagamento);
                client.Close();

                return erros[0];
            }
            catch (FaultException<WFProdutos.ModelosErroServicos> fe)
            {
                client.Close();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Close();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Close();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Close();
                throw ex;
            }
        }

        /// <summary>
        /// Remove uma lista de produtos da base WF
        /// </summary>
        /// <param name="produtos"></param>
        /// <param name="tipoOperacao"></param>
        private WFProdutos.RetornoErro RemoveListaProdutos(List<ProdutosListaDadosProdutosPorRamoCanal> produtos, Char tipoOperacao)
        {
            Int32 i = 0;
            WFProdutos.RetornoErro erro = new WFProdutos.RetornoErro { CodigoErro = 0 };

            while (produtos.Count > i && erro.CodigoErro == 0)
            {
                erro = RemoveProduto(produtos[i], tipoOperacao);
                i++;
            }

            return erro;
        }

        /// <summary>
        /// Remove um patamar da base WF
        /// </summary>
        /// <param name="patamar"></param>
        /// <param name="tipoOperacao"></param>
        /// <returns></returns>
        private WFProdutos.RetornoErro RemovePatamar(Modelo.Patamar patamar, Char tipoOperacao)
        {
            ServicoPortalWFProdutosClient client = new ServicoPortalWFProdutosClient();

            try
            {
                Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                Int64 numCNPJ = 0;
                if (Credenciamento.TipoPessoa == "J")
                    Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                else
                    Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                Int32 numSeqProp = (Int32)Credenciamento.NumSequencia;
                Int32 codCca = patamar.CodCca;
                Char indTipoOperacaoProd = tipoOperacao;
                Int32 codFeature = patamar.CodFeature;
                Int32 sequenciaPatamar = patamar.SequenciaPatamar;
                Int32? patamarInicial = null;
                Int32? patamarFinal = null;
                Double? taxaPatamar = null;
                Int32? codRegimePatamar = null;
                String usuario = null;

                WFProdutos.RetornoErro[] retorno = client.ExclusaoPatamares(codTipoPessoa,
                    numCNPJ,
                    numSeqProp,
                    codCca,
                    indTipoOperacaoProd,
                    codFeature,
                    sequenciaPatamar,
                    patamarInicial,
                    patamarFinal,
                    taxaPatamar,
                    codRegimePatamar,
                    usuario);
                client.Close();

                return retorno[0];
            }
            catch (FaultException<WFProdutos.ModelosErroServicos> fe)
            {
                client.Close();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Close();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Close();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Close();
                throw ex;
            }

        }

        /// <summary>
        /// Remove uma lista de Patamares da base WF
        /// </summary>
        /// <param name="patamares"></param>
        /// <returns></returns>
        private WFProdutos.RetornoErro RemoveListaPatamares(List<Modelo.Patamar> patamares)
        {
            Int32 i = 0;
            WFProdutos.RetornoErro erro = new WFProdutos.RetornoErro { CodigoErro = 0 };

            while (patamares.Count > i && erro.CodigoErro == 0)
            {
                erro = RemovePatamar(patamares[i], 'C');
                i++;
            }

            return erro;
        }

        /// <summary>
        /// Busca o Ramo de Atividade e o Grupo da Matriz
        /// </summary>
        /// <param name="nullable"></param>
        /// <param name="grupoRamoMatriz"></param>
        /// <param name="ramoAtividadeMatriz"></param>
        private void BuscaRamoAtividadeMatriz(Int32? pdvMatriz, out Int32 grupoRamoMatriz, out Int32 ramoAtividadeMatriz)
        {
            grupoRamoMatriz = 0;
            ramoAtividadeMatriz = 0;

            ServicoPortalGEPontoVendaClient client = new ServicoPortalGEPontoVendaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Busca Ramo Atividade Matriz"))
                {
                    Int32 numPdv = (Int32)pdvMatriz;

                    ListaCadastroPorPontoVenda[] dados = client.ListaCadastroPorPontoVenda(numPdv);
                    client.Close();

                    if (dados.Length > 0)
                    {
                        grupoRamoMatriz = dados[0].CodGrupoRamo ?? 0;
                        ramoAtividadeMatriz = dados[0].CodRamoAtivididade ?? 0;
                    }
                }
            }
            catch (FaultException<GEPontoVen.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, CODIGO_ERRO);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Negócios", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Lista dados produtos por ponto de venda
        /// </summary>
        private List<ProdutosListaDadosProdutosPorRamoCanal> ListaDadosProdutosPorPontoDeVenda(Int32 numPdvMatriz, Char? tipoOperacao)
        {
            ServicoPortalGEProdutosClient client = new ServicoPortalGEProdutosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Lista Produtos por ponto de venda"))
                {
                    List<ProdutosListaDadosProdutosPorRamoCanal> retorno = new List<ProdutosListaDadosProdutosPorRamoCanal>();


                    ProdutosListaDadosProdutosPorPontoVenda[] produtos = client.ListaProdutosComRegimePorPvCreditoDebito(numPdvMatriz, tipoOperacao);
                    client.Close();

                    foreach (ProdutosListaDadosProdutosPorPontoVenda produto in produtos)
                    {
                        if (produto.CodFeature == 3)
                        {
                            if (Credenciamento.Patamares == null)
                                Credenciamento.Patamares = new List<Modelo.Patamar>();

                            Credenciamento.Patamares.Add(new Modelo.Patamar
                            {
                                CodCca = (Int32)produto.CodCCA,
                                CodFeature = (Int32)produto.CodFeature,
                                PatamarInicial = produto.PatamarInicioNovo != null ? produto.PatamarInicioNovo : 0,
                                PatamarFinal = produto.PatamarFimNovo != null ? produto.PatamarFimNovo : 0,
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
                        p.QtdeDefaultParcela = produto.QtdeLimiteParcelaNovo;
                        p.NomeFeature = produto.NomeFeature;
                        p.NomeCCA = produto.NomeCCA;

                        retorno.Add(p);
                    }

                    return retorno;
                }
            }
            catch (FaultException<GEProdutos.ModelosErroServicos> fe)
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

        /// <summary>
        /// Retorna o resumo da campanha selecionada
        /// </summary>
        /// <param name="codigoCampanha"></param>
        /// <returns></returns>
        private String GetResumoCampanha(String codigoCampanha)
        {
            ListaDetalheCampanha[] retorno;

            using (var log = Logger.IniciarLog("Lista Detalhe Campanha"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new
                {
                    codigoCampanha
                });

                using (var contexto = new ContextoWCF<ServicoPortalWFCampanhasClient>())
                {

                    retorno = contexto.Cliente.ListaDetalheCampanha(codigoCampanha, null, null, null, null, null);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            if (retorno.Length > 0)
                return retorno[0].DescricaoCampanha;

            return String.Empty;
        }

        #endregion
    }
}
