/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Linq;
using System.ServiceModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using System.Web;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.OutrosServicos.SharePoint.OfertaServico;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Globalization;

namespace Redecard.PN.OutrosServicos.SharePoint.ControlTemplates.OutrosServicos.Mdr
{
    public partial class TaxaOferta : UserControlBase
    {
        #region [Declaração de Controles]

        /// <summary>
        /// Quadro de aviso
        /// </summary>
        protected QuadroAviso qavSemDados;

        /// <summary>
        /// Quadro de aviso para quando não encontrar faixas
        /// </summary>
        protected QuadroAviso qavNenhumaOferta;

        /// <summary>
        /// Lightbox de Taxas para as faixas
        /// </summary>
        protected LightBox lbxTaxas;

        /// <summary>
        /// Quadro de aviso para quando não encontrar histórico da oferta
        /// </summary>
        protected QuadroAviso qavNenhumHistorico;

        /// <summary>
        /// Lightbox de Benefício Aplicado para o Histórico
        /// </summary>
        protected LightBox lbxBeneficioAplicado;

        /// <summary>
        /// Lightbox de Estabelecimentos para o Histórico
        /// </summary>
        protected LightBox lbxEstabelcimentosVinculados;

        #endregion

        #region [Propriedades do Controle]

        /// <summary>
        /// Código da Oferta na QueryString
        /// </summary>
        private Int32 CodigoOferta
        {
            get
            {
                if (!Object.ReferenceEquals(ViewState["_CodigoOferta"], null))
                    return Convert.ToInt32(ViewState["_CodigoOferta"].ToString());
                else
                    return 0;
            }
            set
            {
                ViewState["_CodigoOferta"] = value;
            }
        }

        /// <summary>
        /// Código da Proposta na QueryString
        /// </summary>
        private Int64 CodigoProposta
        {
            get
            {
                if (!Object.ReferenceEquals(ViewState["_CodigoProposta"], null))
                    return Convert.ToInt64(ViewState["_CodigoProposta"].ToString());
                else
                    return 0;
            }
            set
            {
                ViewState["_CodigoProposta"] = value;
            }
        }

        /// <summary>
        /// Código do Contrato da Oferta na QueryString
        /// </summary>
        private Int64 CodigoContrato
        {
            get
            {
                if (!Object.ReferenceEquals(ViewState["_CodigoContrato"], null))
                    return Convert.ToInt64(ViewState["_CodigoContrato"].ToString());
                else
                    return 0;
            }
            set
            {
                ViewState["_CodigoContrato"] = value;
            }
        }

        /// <summary>
        /// Código da Estrutura de Meta da Oferta na QueryString
        /// </summary>
        private Int32 CodigoEstrutura
        {
            get
            {
                if (!Object.ReferenceEquals(ViewState["_CodigoEstrutura"], null))
                    return Convert.ToInt32(ViewState["_CodigoEstrutura"].ToString());
                else
                    return 0;
            }
            set
            {
                ViewState["_CodigoEstrutura"] = value;
            }
        }

        #endregion

        #region [Eventos do Controle]

        /// <summary>
        /// Carregamento do Controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Consulta as Ofertas e carrega no grid"))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        if (Sessao.Contem())
                        {
                            Int32 numeroPv = SessaoAtual.CodigoEntidade;

                            //if (SessaoAtual.AcessoFilial)
                            //    numeroPv = SessaoAtual.CodigoEntidadeMatriz;
                            //else if (SessaoAtual.CodigoMatriz > 0)
                            //    numeroPv = SessaoAtual.CodigoMatriz;

                            var ofertas = default(List<Oferta>);

                            QueryStringSegura qrStringOferta = new QueryStringSegura(Request.QueryString["q"].ToString());

                            Int32 codigoOferta = Convert.ToInt32(qrStringOferta["CodigoOferta"].ToString());
                            Int64 codigoProposta = Convert.ToInt64(qrStringOferta["CodigoProposta"].ToString());

                            using (var ctx = new ContextoWCF<ServicoOfertaClient>())
                                ofertas = ctx.Cliente.ConsultarOfertas(numeroPv);

                            ofertas = ofertas.Where(o => o.CodigoOferta == codigoOferta
                                                      && o.CodigoProposta == codigoProposta).ToList();

                            if (ofertas.Count == 0)
                            {
                                pnlDadosOferta.Visible = false;
                                qavSemDados.Visible = true;
                                qavSemDados.CarregarMensagem();
                            }
                            else
                            {
                                this.CodigoOferta = codigoOferta;
                                this.CodigoProposta = codigoProposta;

                                pnlDadosOferta.Visible = true;
                                qavSemDados.Visible = false;
                                this.CarregarDetalhesOferta();
                            }

                        }
                    }

                    this.RegistrarAssyncCommands();
                }
                catch (FaultException<OfertaServico.GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Trocando a Sazonalidade e listando as faixas de meta dela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlSazonalidade_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Trocando a Sazonalidade e listando as faixas de meta dela"))
            {
                this.CarregarDadosFaixasMeta();
            }
        }

        /// <summary>
        /// Preenchendo os campos da Tabela de Faixas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void repFaixas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger log = Logger.IniciarNovoLog("Preenchendo os campos da Tabela de Faixas"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        FaixaMetaOferta faixa = default(FaixaMetaOferta);

                        if (!Object.ReferenceEquals(e.Item.DataItem, null))
                        {
                            faixa = (FaixaMetaOferta)e.Item.DataItem;

                            Literal ltrFaixa = e.Item.FindControl("ltrFaixa") as Literal;
                            Literal ltrFaixaInicial = e.Item.FindControl("ltrFaixaInicial") as Literal;
                            Literal ltrFaixaFinal = e.Item.FindControl("ltrFaixaFinal") as Literal;
                            HiddenField hdnFaixaFinal = e.Item.FindControl("hdnFaixaFinal") as HiddenField;

                            LinkButton lkbTaxaCredito = e.Item.FindControl("lkbTaxaCredito") as LinkButton;
                            LinkButton lkbTaxaDebito = e.Item.FindControl("lkbTaxaDebito") as LinkButton;

                            ltrFaixa.Text = faixa.Codigo.ToString();
                            ltrFaixaInicial.Text = faixa.ValorInicial.HasValue ? this.FormatarValorMonetario(faixa.ValorInicial.Value) : "-";
                            ltrFaixaFinal.Text = faixa.ValorFinal.HasValue && faixa.ValorFinal.Value != 999999999.99 ? 
                                this.FormatarValorMonetario(faixa.ValorFinal.Value) : "-";
                            hdnFaixaFinal.Value = faixa.ValorFinal.HasValue ? this.FormatarValorMonetario(faixa.ValorFinal.Value) : "-";

                            lkbTaxaCredito.CommandArgument = String.Format("{0}|{1}|{2}",
                                                                           faixa.Codigo.ToString(),
                                                                           ltrFaixaInicial.Text,
                                                                           hdnFaixaFinal.Value);
                            lkbTaxaDebito.CommandArgument = String.Format("{0}|{1}|{2}",
                                                                           faixa.Codigo.ToString(),
                                                                           ltrFaixaInicial.Text,
                                                                           hdnFaixaFinal.Value);
                        }
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Preencher os campos de Taxa da Oferta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void repTaxas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger log = Logger.IniciarNovoLog("Preencher os campos de Taxa da Oferta"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        ProdutoBandeiraMeta produtoMeta = default(ProdutoBandeiraMeta);

                        if (!Object.ReferenceEquals(e.Item.DataItem, null))
                        {
                            produtoMeta = (ProdutoBandeiraMeta)e.Item.DataItem;

                            Label lblProdutoDescricaoBandeira = e.Item.FindControl("lblProdutoDescricaoBandeira") as Label;
                            Repeater repValoresTaxa = e.Item.FindControl("repValoresTaxa") as Repeater;

                            lblProdutoDescricaoBandeira.Text = String.Format("PRODUTO {0}", produtoMeta.DescricaoBandeira.ToUpper());

                            if (produtoMeta.Taxas != null && produtoMeta.Taxas.Count > 0)
                            {
                                repValoresTaxa.Visible = true;
                                repValoresTaxa.DataSource = produtoMeta.Taxas;
                                repValoresTaxa.DataBind();
                            }
                            else
                                repValoresTaxa.Visible = false;
                        }
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Preencher os campos de Taxa da Oferta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void repValoresTaxa_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger log = Logger.IniciarNovoLog("Preencher os campos de Taxa da Oferta (valores)"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        TaxaMeta taxa = default(TaxaMeta);

                        if (!Object.ReferenceEquals(e.Item.DataItem, null))
                        {
                            taxa = (TaxaMeta)e.Item.DataItem;

                            Literal ltrBandeira = e.Item.FindControl("ltrBandeira") as Literal;
                            Literal ltrTipoParecela = e.Item.FindControl("ltrTipoParecela") as Literal;
                            Literal ltrParcelas = e.Item.FindControl("ltrParcelas") as Literal;
                            Literal ltrPrazo = e.Item.FindControl("ltrPrazo") as Literal;
                            Literal ltrTaxa = e.Item.FindControl("ltrTaxa") as Literal;
                            Literal ltrTarifa = e.Item.FindControl("ltrTarifa") as Literal;

                            ltrBandeira.Text = taxa.DescricaoBandeira;
                            ltrTipoParecela.Text = taxa.DescricaoModalidade;

                            if (taxa.NumeroParcelaInicial.HasValue && taxa.NumeroParcelaFinal.HasValue)
                            {
                                if (taxa.NumeroParcelaFinal.Value > 2)
                                    ltrParcelas.Text = String.Format("{0} a {1}", taxa.NumeroParcelaInicial.Value.ToString(), taxa.NumeroParcelaFinal.Value.ToString());
                                else
                                    ltrParcelas.Text = taxa.NumeroParcelaFinal.Value.ToString();
                            }
                            else
                                ltrParcelas.Text = "-";

                            ltrPrazo.Text = taxa.Prazo != 0 ? String.Format("{0} dias", taxa.Prazo.ToString()) : "-" ;
                            ltrTaxa.Text = taxa.Taxa.HasValue ? String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:0.000}", taxa.Taxa.Value) : "-";
                            ltrTarifa.Text = taxa.Tarifa.HasValue ? String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:0.00}", taxa.Tarifa.Value) : "-";
                        }
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }


        /// <summary>
        /// Preencher os campos de Histórico da Oferta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void repHistoricoOferta_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger log = Logger.IniciarNovoLog("Preencher os campos de Histórico da Oferta"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        HistoricoOferta historico = default(HistoricoOferta);

                        if (!Object.ReferenceEquals(e.Item.DataItem, null))
                        {
                            historico = (HistoricoOferta)e.Item.DataItem;

                            Literal ltrPeriodo = e.Item.FindControl("ltrPeriodo") as Literal;
                            Literal ltrFaixaMeta = e.Item.FindControl("ltrFaixaMeta") as Literal;
                            Literal ltrValorRealizado = e.Item.FindControl("ltrValorRealizado") as Literal;
                            Literal ltrPeriodoCarencia = e.Item.FindControl("ltrPeriodoCarencia") as Literal;

                            LinkButton lkbBeneficioAplicado = e.Item.FindControl("lkbBeneficioAplicado") as LinkButton;
                            Image imgBeneficioAprovado = e.Item.FindControl("imgBeneficioAprovado") as Image;
                            Image imgBeneficioNaoAprovado = e.Item.FindControl("imgBeneficioNaoAprovado") as Image;

                            LinkButton lkbTipoEstabelecimento = e.Item.FindControl("lkbTipoEstabelecimento") as LinkButton;
                            Literal ltrTipoEstabelecimento = e.Item.FindControl("ltrTipoEstabelecimento") as Literal;
                            LinkButton lkbTaxaCreditoHistorico = e.Item.FindControl("lkbTaxaCreditoHistorico") as LinkButton;
                            LinkButton lkbTaxaDebitoHistorico = e.Item.FindControl("lkbTaxaDebitoHistorico") as LinkButton;


                            ltrPeriodo.Text = historico.PeriodoApuracao;
                            String faixaMeta;
                            if (historico.ValorFinalFaixa != 999999999.99)
                            {
                                faixaMeta = String.Format("{0} a {1}",
                                                              this.FormatarValorMonetario(historico.ValorInicialFaixa.GetValueOrDefault()),
                                                              this.FormatarValorMonetario(historico.ValorFinalFaixa.GetValueOrDefault()));
                            }
                            else
                            {
                                faixaMeta = String.Format("{0} a {1}",
                                                              this.FormatarValorMonetario(historico.ValorInicialFaixa.GetValueOrDefault()),
                                                              "-");
                            }

                            ltrFaixaMeta.Text = faixaMeta;

                            ltrValorRealizado.Text = this.FormatarValorMonetario(historico.ValorFaturamento);

                            ltrPeriodoCarencia.Text = historico.PossuiCarencia ? "Sim" : "Não";

                            lkbBeneficioAplicado.Text = historico.RecebeuBeneficio ? "Sim" : "Não";
                            lkbBeneficioAplicado.CommandArgument = String.Format("{0}|{1}",
                                                                    historico.PeriodoApuracao,
                                                                    historico.DescricaoMensagemApuracao);

                            imgBeneficioAprovado.Visible = historico.RecebeuBeneficio;
                            imgBeneficioNaoAprovado.Visible = !historico.RecebeuBeneficio;

                            String dadosCommandHistorico = String.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                                                                        historico.CodigoProposta,
                                                                        historico.CodigoOferta,
                                                                        historico.DataAceitePropostaOferta,
                                                                        historico.MesReferenciaApuracao,
                                                                        historico.AnoReferenciaApuracao,
                                                                        historico.PeriodoApuracao);

                            if (historico.TipoEstabelecimento == TipoEstabelecimentoHistoricoOferta.Filial
                                || historico.TipoEstabelecimento == TipoEstabelecimentoHistoricoOferta.Matriz
                                || historico.TipoEstabelecimento == TipoEstabelecimentoHistoricoOferta.GrupoComercial)
                            {
                                if (historico.QuantidadePontosVenda > 1)
                                {
                                    lkbTipoEstabelecimento.Visible = true;

                                    if (historico.TipoEstabelecimento == TipoEstabelecimentoHistoricoOferta.GrupoComercial)
                                        lkbTipoEstabelecimento.Text = historico.DescricaoTipoEstabelecimento;
                                    else
                                        lkbTipoEstabelecimento.Text = String.Format("{0} estabelec.", historico.QuantidadePontosVenda.ToString());

                                    lkbTipoEstabelecimento.CommandArgument = dadosCommandHistorico;

                                    ltrTipoEstabelecimento.Visible = false;
                                }
                                else
                                {
                                    lkbTipoEstabelecimento.Visible = false;

                                    ltrTipoEstabelecimento.Visible = true;
                                    ltrTipoEstabelecimento.Text = historico.NumeroPontoVenda.ToString();
                                }
                            }
                            else
                            {
                                lkbTipoEstabelecimento.Visible = false;

                                ltrTipoEstabelecimento.Visible = true;
                                ltrTipoEstabelecimento.Text = historico.DescricaoTipoEstabelecimento;
                            }

                            lkbTaxaDebitoHistorico.Text = "Detalhar";
                            lkbTaxaDebitoHistorico.CommandArgument = dadosCommandHistorico;

                            lkbTaxaCreditoHistorico.Text = "Detalhar";
                            lkbTaxaCreditoHistorico.CommandArgument = dadosCommandHistorico;
                        }

                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Preencher os campos de Estabelecimentos do Histórico da Oferta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void repEstabelecimentosHistorico_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (Logger log = Logger.IniciarNovoLog("Preencher os campos de Estabelecimentos do Histórico da Oferta"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        EstabelecimentoHistoricoOferta estabelecimento = default(EstabelecimentoHistoricoOferta);

                        if (!Object.ReferenceEquals(e.Item.DataItem, null))
                        {
                            estabelecimento = (EstabelecimentoHistoricoOferta)e.Item.DataItem;

                            Literal ltrNrEstabelecimento = e.Item.FindControl("ltrNrEstabelecimento") as Literal;
                            Literal ltrNomeEstabelecimento = e.Item.FindControl("ltrNomeEstabelecimento") as Literal;

                            ltrNrEstabelecimento.Text = String.Format("{0}{1}",
                                    estabelecimento.NumeroEstabelecimento.ToString(),
                                    estabelecimento.AtingiuMeta.Equals(true) ? "" : "*");
                            ltrNomeEstabelecimento.Text = estabelecimento.RazaoSocial;

                        }
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }


        /// <summary>
        /// Exibir lightbox com a informação de Benefício
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lkbBeneficioAplicado_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Exibir lightbox com a informação de Benefício"))
            {
                try
                {
                    String periodo = default(String);
                    String motivo = default(String);

                    LinkButton lkbBeneficioAplicado = (LinkButton)sender;

                    log.GravarMensagem("CommandArguments", new { lkbBeneficioAplicado.CommandArgument });

                    if (!Object.ReferenceEquals(lkbBeneficioAplicado.CommandArgument, null) && !String.IsNullOrEmpty(lkbBeneficioAplicado.CommandArgument.ToString()))
                    {
                        periodo = lkbBeneficioAplicado.CommandArgument.ToString().Split('|')[0];
                        motivo = lkbBeneficioAplicado.CommandArgument.ToString().Split('|')[1];

                        lblPeriodoMotivo.Text = periodo;
                        ltrMotivo.Text = motivo;

                        pnlNenhumMotivo.Visible = false;
                        pnlMotivo.Visible = true;
                    }
                    else
                    {
                        pnlNenhumMotivo.Visible = true;
                        pnlMotivo.Visible = false;
                    }

                    lbxBeneficioAplicado.Exibir();
                    upnlBeneficioAplicado.Update();
                }
                catch (IndexOutOfRangeException ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Exibir lightbox com a informação de Estabelecimentos
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lkbTipoEstabelecimento_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Exibir lightbox com a informação de Estabelecimentos"))
            {
                try
                {
                    LinkButton lkbTipoEstabelecimento = (LinkButton)sender;

                    String periodoApuracao = default(String);
                    HistoricoOferta historico = new HistoricoOferta();
                    List<EstabelecimentoHistoricoOferta> estabelecimentos = default(List<EstabelecimentoHistoricoOferta>);

                    if (!Object.ReferenceEquals(lkbTipoEstabelecimento.CommandArgument, null) && !String.IsNullOrEmpty(lkbTipoEstabelecimento.CommandArgument.ToString()))
                    {
                        historico.CodigoProposta = lkbTipoEstabelecimento.CommandArgument.ToString().Split('|')[0].ToInt64Null(0).Value;
                        historico.CodigoOferta = lkbTipoEstabelecimento.CommandArgument.ToString().Split('|')[1].ToInt64Null(0).Value;
                        historico.DataAceitePropostaOferta = Convert.ToDateTime(lkbTipoEstabelecimento.CommandArgument.ToString().Split('|')[2]);
                        historico.MesReferenciaApuracao = lkbTipoEstabelecimento.CommandArgument.ToString().Split('|')[3].ToInt32Null(0).Value;
                        historico.AnoReferenciaApuracao = lkbTipoEstabelecimento.CommandArgument.ToString().Split('|')[4].ToInt32Null(0).Value;

                        periodoApuracao = lkbTipoEstabelecimento.CommandArgument.ToString().Split('|')[5];
                        lblPeriodoEstabelecimentos.Text = periodoApuracao;

                        using (var ctx = new ContextoWCF<ServicoOfertaClient>())
                        {
                            estabelecimentos = ctx.Cliente.ConsultarEstabelecimentosOferta(historico);

                            if (!Object.ReferenceEquals(estabelecimentos, null) && estabelecimentos.Count > 0)
                            {
                                pnlNenhumEstabelecimento.Visible = false;
                                pnlEstabelcimento.Visible = true;
                                repEstabelecimentosHistorico.DataSource = estabelecimentos;
                                repEstabelecimentosHistorico.DataBind();
                            }
                            else
                            {
                                pnlNenhumEstabelecimento.Visible = true;
                                pnlEstabelcimento.Visible = false;
                            }
                        }
                    }

                    lbxEstabelcimentosVinculados.Exibir();
                    upnlEstabelcimentosVinculados.Update();
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Exibir Taxa de Crédito da Faixa do Histórico da Oferta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lkbTaxaCreditoHistorico_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Exibir Taxa de Crédito da Faixa do Histórico da Oferta"))
            {
                try
                {
                    LinkButton lkbTaxaCreditoHistorico = (LinkButton)sender;

                    String periodoApuracao = default(String);
                    HistoricoOferta historico = new HistoricoOferta();

                    if (!Object.ReferenceEquals(lkbTaxaCreditoHistorico.CommandArgument, null) && !String.IsNullOrEmpty(lkbTaxaCreditoHistorico.CommandArgument.ToString()))
                    {
                        historico.CodigoProposta = lkbTaxaCreditoHistorico.CommandArgument.ToString().Split('|')[0].ToInt64Null(0).Value;
                        historico.CodigoOferta = lkbTaxaCreditoHistorico.CommandArgument.ToString().Split('|')[1].ToInt64Null(0).Value;
                        historico.DataAceitePropostaOferta = Convert.ToDateTime(lkbTaxaCreditoHistorico.CommandArgument.ToString().Split('|')[2]);
                        historico.MesReferenciaApuracao = lkbTaxaCreditoHistorico.CommandArgument.ToString().Split('|')[3].ToInt32Null(0).Value;
                        historico.AnoReferenciaApuracao = lkbTaxaCreditoHistorico.CommandArgument.ToString().Split('|')[4].ToInt32Null(0).Value;

                        periodoApuracao = lkbTaxaCreditoHistorico.CommandArgument.ToString().Split('|')[5];
                    }

                    this.CarregarDadosTaxaFaixaHistorico("C", historico, periodoApuracao);
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Exibir Taxa de Débito da Faixa do Histórico da Oferta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lkbTaxaDebitoHistorico_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Exibir Taxa de Débito da Faixa do Histórico da Oferta"))
            {
                try
                {
                    LinkButton lkbTaxaDebitoHistorico = (LinkButton)sender;

                    String periodoApuracao = default(String);
                    HistoricoOferta historico = new HistoricoOferta();

                    if (!Object.ReferenceEquals(lkbTaxaDebitoHistorico.CommandArgument, null) && !String.IsNullOrEmpty(lkbTaxaDebitoHistorico.CommandArgument.ToString()))
                    {
                        historico.CodigoProposta = lkbTaxaDebitoHistorico.CommandArgument.ToString().Split('|')[0].ToInt64Null(0).Value;
                        historico.CodigoOferta = lkbTaxaDebitoHistorico.CommandArgument.ToString().Split('|')[1].ToInt64Null(0).Value;
                        historico.DataAceitePropostaOferta = Convert.ToDateTime(lkbTaxaDebitoHistorico.CommandArgument.ToString().Split('|')[2]);
                        historico.MesReferenciaApuracao = lkbTaxaDebitoHistorico.CommandArgument.ToString().Split('|')[3].ToInt32Null(0).Value;
                        historico.AnoReferenciaApuracao = lkbTaxaDebitoHistorico.CommandArgument.ToString().Split('|')[4].ToInt32Null(0).Value;

                        periodoApuracao = lkbTaxaDebitoHistorico.CommandArgument.ToString().Split('|')[5];
                    }

                    this.CarregarDadosTaxaFaixaHistorico("D", historico, periodoApuracao);
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Exibir Taxa de Crédito da Faixa da Oferta no Ramo selecionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lkbTaxaCredito_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Exibir Taxa de Crédito da Faixa da Oferta no Ramo selecionado"))
            {
                LinkButton lkbTaxaCredito = (LinkButton)sender;

                Int32 codigoFaixa = default(Int32);
                String faixaInicial = default(String);
                String faixaFinal = default(String);

                if (!Object.ReferenceEquals(lkbTaxaCredito.CommandArgument, null))
                {
                    codigoFaixa = lkbTaxaCredito.CommandArgument.ToString().Split('|')[0].ToInt32Null(0).Value;
                    faixaInicial = lkbTaxaCredito.CommandArgument.ToString().Split('|')[1];
                    faixaFinal = lkbTaxaCredito.CommandArgument.ToString().Split('|')[2];
                }

                this.CarregarDadosTaxaFaixa("C", codigoFaixa, faixaInicial, faixaFinal);
            }
        }

        /// <summary>
        /// Exibir Taxa de Débito da Faixa da Oferta no Ramo selecionado
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void lkbTaxaDebito_Click(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Exibir Taxa de Débito da Faixa da Oferta no Ramo selecionado"))
            {
                LinkButton lkbTaxaDebito = (LinkButton)sender;

                Int32 codigoFaixa = default(Int32);
                String faixaInicial = default(String);
                String faixaFinal = default(String);

                if (!Object.ReferenceEquals(lkbTaxaDebito.CommandArgument, null))
                {
                    codigoFaixa = lkbTaxaDebito.CommandArgument.ToString().Split('|')[0].ToInt32Null(0).Value;
                    faixaInicial = lkbTaxaDebito.CommandArgument.ToString().Split('|')[1];
                    faixaFinal = lkbTaxaDebito.CommandArgument.ToString().Split('|')[2];
                }

                this.CarregarDadosTaxaFaixa("D", codigoFaixa, faixaInicial, faixaFinal);
            }
        }

        #endregion

        #region [Metódos WCF]

        /// <summary>
        /// Carrega os dados da Oferta na página
        /// </summary>
        private void CarregarDetalhesOferta()
        {
            using (Logger log = Logger.IniciarLog("Consulta as Ofertas e carrega no grid"))
            {
                if (Sessao.Contem())
                {
                    Int64 numeroPv = SessaoAtual.CodigoEntidade;

                    //if (SessaoAtual.AcessoFilial)
                    //    numeroPv = SessaoAtual.CodigoEntidadeMatriz;
                    //else if (SessaoAtual.CodigoMatriz > 0)
                    //    numeroPv = SessaoAtual.CodigoMatriz;

                    var contrato = default(ContratoOferta);

                    try
                    {
                        using (var ctx = new ContextoWCF<ServicoOfertaClient>())
                        {
                            contrato = ctx.Cliente.ConsultarContratoOferta(this.CodigoOferta, this.CodigoProposta, numeroPv);

                            //Se nulo, exibe mensagem customizada
                            if (contrato == null)
                            {
                                pnlDadosOferta.Visible = false;
                                qavSemDados.Visible = true;
                                qavSemDados.CarregarMensagem();
                            }
                            else
                            {
                                pnlDadosOferta.Visible = true;
                                qavSemDados.Visible = false;
                                this.CodigoContrato = contrato.CodigoContrato;
                                this.CodigoEstrutura = contrato.CodigoEstruturaMeta;
                                this.CarregarRamos(contrato.PossuiRamo);
                                this.CarregarSazonalidades();
                                this.CarregarDadosFaixasMeta();
                                this.CarregarDadosHistoricoOferta();
                            }
                        }
                    }
                    catch (FaultException<OfertaServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }
        }

        /// <summary>
        /// Carregar os dados de Histórico de Oferta
        /// </summary>
        private void CarregarDadosHistoricoOferta()
        {
            using (Logger log = Logger.IniciarLog("Carregar os dados de Histórico de Oferta"))
            {
                if (Sessao.Contem())
                {
                    var historicos = default(List<HistoricoOferta>);

                    try
                    {
                        using (var ctx = new ContextoWCF<ServicoOfertaClient>())
                        {
                            ContratoOferta contrato = new ContratoOferta()
                            {
                                CodigoContrato = this.CodigoContrato,
                                CodigoProposta = this.CodigoProposta,
                                CodigoEstruturaMeta = this.CodigoEstrutura,
                                CodigoOferta = this.CodigoOferta
                            };

                            historicos = ctx.Cliente.ConsultarHistoricoOferta(contrato);

                            //Se nulo, exibe mensagem customizada
                            if (historicos != null && historicos.Count > 0)
                            {
                                qavNenhumHistorico.Visible = false;
                                pnlHistoricoOferta.Visible = true;
                                repHistoricoOferta.DataSource = historicos;
                                repHistoricoOferta.DataBind();
                            }
                            else
                            {
                                qavNenhumHistorico.Visible = true;
                                qavNenhumHistorico.CarregarMensagem();
                                pnlHistoricoOferta.Visible = false;
                            }
                        }
                    }
                    catch (FaultException<OfertaServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }
        }

        /// <summary>
        /// Carrega os ramos do Estabelecimento caso o contrato possua RAMO
        /// </summary>
        /// <param name="possuiRamo">Indica se o contrato possui Ramos</param>
        private void CarregarRamos(bool possuiRamo)
        {
            List<RamosAtividadeOferta> ramos = default(List<RamosAtividadeOferta>);

            if (possuiRamo)
            {
                pnlRamos.Visible = false;

                Int32 codigoRetorno = default(Int32);
                Int32 numeroPv = SessaoAtual.CodigoEntidade;
                Int64 cnpjPdv = default(Int64);

                //if (SessaoAtual.AcessoFilial)
                //    numeroPv = SessaoAtual.CodigoEntidadeMatriz;
                //else if (SessaoAtual.CodigoMatriz > 0)
                //    numeroPv = SessaoAtual.CodigoMatriz;

                using (var ctx = new ContextoWCF<EntidadeServico.EntidadeServicoClient>())
                {
                    EntidadeServico.Entidade entidade = ctx.Cliente.ConsultarDadosPV(out codigoRetorno, numeroPv);
                    cnpjPdv = Convert.ToInt64(entidade.CNPJEntidade);
                }

                using (var ctx = new ContextoWCF<ServicoOfertaClient>())
                    ramos = ctx.Cliente
                                .ConsultarRamosOferta(this.CodigoOferta,
                                                      this.CodigoContrato,
                                                      cnpjPdv);

                if (ramos != null && ramos.Count > 0)
                {
                    pnlRamos.Visible = true;
                    ramos = ramos
                                .OrderBy("DescricaoRamoAtividade")
                                .ToList();
                    ddlRamos.DataSource = ramos;
                    ddlRamos.DataTextField = "DescricaoRamoAtividade";
                    ddlRamos.DataValueField = "CodigoRamoAtividade";
                    ddlRamos.DataBind();
                }
                else
                    pnlRamos.Visible = false;
            }
            else
                pnlRamos.Visible = false;
        }

        /// <summary>
        /// Carregar as sazonalidades da Oferta caso haja alguma
        /// </summary>
        private void CarregarSazonalidades()
        {
            List<SazonalidadeOferta> sazonalidades = default(List<SazonalidadeOferta>);

            using (var ctx = new ContextoWCF<ServicoOfertaClient>())
                sazonalidades = ctx.Cliente
                                    .ConsultarSazonalizades(this.CodigoOferta,
                                                            this.CodigoContrato,
                                                            this.CodigoEstrutura);

            if (sazonalidades != null && sazonalidades.Count > 0)
            {
                pnlSazonalidade.Visible = true;
                sazonalidades = sazonalidades
                                .OrderBy("MesAnoInicio")
                                .ToList();
                ddlSazonalidade.DataSource = sazonalidades;
                ddlSazonalidade.DataTextField = "MesAnoInicioDescricaoNaoAbreviada";
                ddlSazonalidade.DataValueField = "MesAnoInicio";
                ddlSazonalidade.DataBind();
                ddlSazonalidade.SelectedValue = String.Format("{0}/{1}", DateTime.Now.Month, DateTime.Now.Year);
            }
            else
                pnlSazonalidade.Visible = false;
        }

        /// <summary>
        /// Carregar as faixas de Meta da Oferta
        /// </summary>
        private void CarregarDadosFaixasMeta()
        {
            using (Logger log = Logger.IniciarLog("Carregar as faixas de Meta da Oferta"))
            {
                if (Sessao.Contem())
                {
                    Int64 numeroPv = SessaoAtual.CodigoEntidade;

                    //if (SessaoAtual.AcessoFilial)
                    //    numeroPv = SessaoAtual.CodigoEntidadeMatriz;
                    //else if (SessaoAtual.CodigoMatriz > 0)
                    //    numeroPv = SessaoAtual.CodigoMatriz;

                    var faixas = default(List<FaixaMetaOferta>);

                    try
                    {
                        using (var ctx = new ContextoWCF<ServicoOfertaClient>())
                        {
                            ContratoOferta contrato = new ContratoOferta()
                            {
                                CodigoContrato = this.CodigoContrato,
                                CodigoEstruturaMeta = this.CodigoEstrutura,
                                CodigoOferta = this.CodigoOferta
                            };

                            SazonalidadeOferta sazonalidade = default(SazonalidadeOferta);

                            if (ddlSazonalidade.SelectedValue != String.Empty)
                            {
                                sazonalidade = new SazonalidadeOferta();
                                sazonalidade.MesInicio = Convert.ToInt32(ddlSazonalidade.SelectedValue.Split('/')[0]);
                                sazonalidade.AnoInicio = Convert.ToInt32(ddlSazonalidade.SelectedValue.Split('/')[1]);
                            }
                            faixas = ctx.Cliente.ConsultarFaixasMeta(contrato, sazonalidade);

                            //Se nulo, exibe mensagem customizada
                            if (faixas != null && faixas.Count > 0)
                            {
                                qavNenhumaOferta.Visible = false;
                                pnlFaixasMeta.Visible = true;
                                repFaixas.DataSource = faixas;
                                repFaixas.DataBind();
                            }
                            else
                            {
                                qavNenhumaOferta.Visible = true;
                                qavNenhumaOferta.CarregarMensagem();
                                pnlFaixasMeta.Visible = false;
                            }
                        }
                    }
                    catch (FaultException<OfertaServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }
        }

        /// <summary>
        /// Consultar e carregar os dados de taxa
        /// </summary>
        /// <param name="tipoTaxa"></param>
        /// <param name="codigoFaixa"></param>
        /// <param name="faixaInicial"></param>
        /// <param name="faixaFinal"></param>
        private void CarregarDadosTaxaFaixa(String tipoTaxa, Int32 codigoFaixa, String faixaInicial, String faixaFinal)
        {
            using (Logger log = Logger.IniciarLog("Consultar e carregar os dados de taxa"))
            {
                if (Sessao.Contem())
                {
                    Int64 numeroPv = SessaoAtual.CodigoEntidade;

                    //if (SessaoAtual.AcessoFilial)
                    //    numeroPv = SessaoAtual.CodigoEntidadeMatriz;
                    //else if (SessaoAtual.CodigoMatriz > 0)
                    //    numeroPv = SessaoAtual.CodigoMatriz;

                    Int32? codigoRamo = default(Int32?);
                    var produtosMeta = default(List<ProdutoBandeiraMeta>);

                    try
                    {
                        lblMetaPeriodoFaixa.Text = "Meta de Faturamento: ";
                        if(String.Compare(faixaFinal, "R$ 999.999.999,99") != 0)
                            ltrMetaFaixa.Text = String.Format("{0} - {1}", faixaInicial, faixaFinal);
                        else
                            ltrMetaFaixa.Text = String.Format("{0} a {1}", faixaInicial, "-");
                        lbxTaxas.Titulo = tipoTaxa.ToUpper().Equals("C") ? "Taxas Contratadas - Crédito" : "Taxas Contratadas - Débito";

                        ContratoOferta contrato = new ContratoOferta()
                        {
                            CodigoContrato = this.CodigoContrato,
                            CodigoEstruturaMeta = this.CodigoEstrutura,
                            CodigoOferta = this.CodigoOferta
                        };

                        if (!String.IsNullOrWhiteSpace(ddlRamos.SelectedValue))
                            codigoRamo = Convert.ToInt32(ddlRamos.SelectedValue);

                        SazonalidadeOferta sazonalidade = default(SazonalidadeOferta);

                        if (!String.IsNullOrWhiteSpace(ddlSazonalidade.SelectedValue))
                        {
                            sazonalidade = new SazonalidadeOferta();
                            sazonalidade.MesInicio = Convert.ToInt32(ddlSazonalidade.SelectedValue.Split('/')[0]);
                            sazonalidade.AnoInicio = Convert.ToInt32(ddlSazonalidade.SelectedValue.Split('/')[1]);
                        }

                        using (var ctx = new ContextoWCF<ServicoOfertaClient>())
                        {
                            if (tipoTaxa.ToUpper().Equals("C"))
                                produtosMeta = ctx.Cliente.ConsultarTaxasCredito(
                                    contrato,
                                    sazonalidade,
                                    numeroPv,
                                    codigoRamo,
                                    codigoFaixa);
                            else if (tipoTaxa.ToUpper().Equals("D"))
                                produtosMeta = ctx.Cliente.ConsultarTaxasDebito(
                                    contrato,
                                    sazonalidade,
                                    numeroPv,
                                    codigoRamo,
                                    codigoFaixa);
                        }

                        //Se nulo, exibe mensagem customizada
                        if (produtosMeta != null && produtosMeta.Count > 0)
                        {
                            pnlAvisoDetalheTaxa.Visible = false;
                            repTaxas.DataSource = produtosMeta;
                            repTaxas.DataBind();
                        }
                        else
                        {
                            pnlAvisoDetalheTaxa.Visible = true;
                            repTaxas.Visible = false;
                        }

                        lbxTaxas.Exibir();
                        upnlTaxas.Update();

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "_OcultarTaxas", "esconderTodos();", true);
                    }
                    catch (FaultException<OfertaServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }
        }

        /// <summary>
        /// Consultar e carregar os dados de taxa
        /// </summary>
        /// <param name="tipoTaxa"></param>
        /// <param name="historico"></param>
        private void CarregarDadosTaxaFaixaHistorico(String tipoTaxa, HistoricoOferta historico, String periodoApuracao)
        {
            using (Logger log = Logger.IniciarLog("Consultar e carregar os dados de taxa"))
            {
                if (Sessao.Contem())
                {
                    var produtosMeta = default(List<ProdutoBandeiraMeta>);

                    try
                    {

                        lblMetaPeriodoFaixa.Text = "Perído: ";
                        ltrMetaFaixa.Text = String.Format("{0}", periodoApuracao);
                        lbxTaxas.Titulo = tipoTaxa.ToUpper().Equals("C") ? "Taxas aplicadas - Crédito" : "Taxas aplicadas - Débito";

                        using (var ctx = new ContextoWCF<ServicoOfertaClient>())
                        {
                            if (tipoTaxa.ToUpper().Equals("C"))
                                produtosMeta = ctx.Cliente.ConsultarTaxasCreditoHistorico(historico);
                            else if (tipoTaxa.ToUpper().Equals("D"))
                                produtosMeta = ctx.Cliente.ConsultarTaxasDebitoHistorico(historico);
                        }

                        //Se nulo, exibe mensagem customizada
                        if (produtosMeta != null && produtosMeta.Count > 0)
                        {
                            pnlAvisoDetalheTaxa.Visible = false;
                            repTaxas.DataSource = produtosMeta;
                            repTaxas.DataBind();
                        }
                        else
                        {
                            pnlAvisoDetalheTaxa.Visible = true;
                            repTaxas.Visible = false;
                        }

                        lbxTaxas.Exibir();
                        upnlTaxas.Update();

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "_OcultarTaxas", "esconderTodos();", true);
                    }
                    catch (FaultException<OfertaServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }
        }

        #endregion

        #region [Métodos Auxialires]

        private void RegistrarAssyncCommands()
        {
            if (repFaixas.Items != null && repFaixas.Items.Count > 0)
            {
                foreach (RepeaterItem item in repFaixas.Items)
                {
                    var lkbTaxaCredito = (LinkButton)item.FindControl("lkbTaxaCredito");
                    var lkbTaxaDebito = (LinkButton)item.FindControl("lkbTaxaDebito");

                    ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(lkbTaxaCredito);
                    ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(lkbTaxaDebito);
                }
            }

            if (repHistoricoOferta.Items != null && repHistoricoOferta.Items.Count > 0)
            {
                foreach (RepeaterItem item in repHistoricoOferta.Items)
                {
                    var lkbBeneficioAplicado = (LinkButton)item.FindControl("lkbBeneficioAplicado");
                    var lkbTipoEstabelecimento = (LinkButton)item.FindControl("lkbTipoEstabelecimento");

                    var lkbTaxaCreditoHistorico = (LinkButton)item.FindControl("lkbTaxaCreditoHistorico");
                    var lkbTaxaDebitoHistorico = (LinkButton)item.FindControl("lkbTaxaDebitoHistorico");

                    ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(lkbBeneficioAplicado);
                    ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(lkbTipoEstabelecimento);

                    ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(lkbTaxaCreditoHistorico);
                    ScriptManager.GetCurrent(this.Page).RegisterAsyncPostBackControl(lkbTaxaDebitoHistorico);
                }
            }
        }

        /// <summary>
        /// Retornar o valor monetário formatado como R$9.999,00
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        private String FormatarValorMonetario(Double valor)
        {
            return String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", valor);
        }
        #endregion
    }
}
