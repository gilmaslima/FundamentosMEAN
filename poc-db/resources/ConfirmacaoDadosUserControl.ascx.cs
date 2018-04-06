using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Sharepoint.Modelo;
using Redecard.PN.Credenciamento.Sharepoint.WFProposta;
using System.ServiceModel;
using Redecard.PN.Credenciamento.Sharepoint.GEProdutos;
using Redecard.PN.Credenciamento.Sharepoint.GERamosAtd;
using Redecard.PN.Credenciamento.Sharepoint.GECelulas;
using Redecard.PN.Credenciamento.Sharepoint.GECanais;
using Redecard.PN.Credenciamento.Sharepoint.TGCenarios;
using Redecard.PN.Credenciamento.Sharepoint.WMOcorrencia;
using Redecard.PN.Credenciamento.Sharepoint.WFAdministracao;
using Redecard.PN.Credenciamento.Sharepoint.WFScoreRisco;
using System.Linq;
using System.Web.Configuration;

namespace Redecard.PN.Credenciamento.Sharepoint.WebParts.ConfirmacaoDados
{
    public partial class ConfirmacaoDadosUserControl : UserControlCredenciamentoBase
    {
        #region [ Propriedades ]

        #endregion

        #region [ Eventos da Página ]

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.MaintainScrollPositionOnPostBack = true;
            if (!IsPostBack)
            {
                CarregaDados();
            }
        }

        #endregion

        #region [ Eventos de Controles ]

        protected void rptProprietarios_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlTituloProprietario = (Literal)e.Item.FindControl("ltlTituloProprietario");
                Literal ltlProprietario = (Literal)e.Item.FindControl("ltlProprietario");

                ltlTituloProprietario.Visible = (e.Item.ItemIndex == 0);

                String proprietario = String.Format("{0} - {1}%", ((Proprietario)e.Item.DataItem).Nome, ((Proprietario)e.Item.DataItem).Participacao, "%");
                ltlProprietario.Text = proprietario;
            }
        }

        protected void rptVendaCredito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlTipoVenda = (Literal)e.Item.FindControl("ltlTipoVenda");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxas = (Literal)e.Item.FindControl("ltlTaxas");
                Literal ltlQtdMinParcelas = (Literal)e.Item.FindControl("ltlQtdMinParcelas");
                Literal ltlQtdMAxParcelas = (Literal)e.Item.FindControl("ltlQtdMaxParcelas");

                ProdutosListaDadosProdutosPorRamoCanal item = (ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem;

                ltlTipoVenda.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxas.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                ltlQtdMinParcelas.Text = "-";
                ltlQtdMAxParcelas.Text = "-";

                Int32 codCca = (Int32)item.CodCCA;
                Int32 codFeature = (Int32)item.CodFeature;

                if (Credenciamento.Patamares != null)
                {
                    List<Modelo.Patamar> patamares = Credenciamento.Patamares.FindAll(p => p.CodCca == codCca && p.CodFeature == codFeature);

                    if (patamares.Count > 0)
                    {
                        ((Literal)e.Item.FindControl("ltlQtdMinParcelas")).Text = patamares[0].PatamarInicial.ToString();
                        ((Literal)e.Item.FindControl("ltlQtdMaxParcelas")).Text = patamares[0].PatamarFinal.ToString();

                        if (patamares.Count > 1)
                        {
                            Panel pnlPatamar1 = (Panel)e.Item.FindControl("pnlPatamar1");
                            pnlPatamar1.Visible = true;

                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1PrazoRecebimento")).Text = String.Format("{0} dia(s)", patamares[1].Prazo.ToString());
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1QtdMinParcelas")).Text = patamares[1].PatamarInicial.ToString();
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1QtdMaxParcelas")).Text = patamares[1].PatamarFinal.ToString();
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1Taxa")).Text = String.Format(@"{0:f2}", patamares[1].TaxaPatamar);
                        }

                        if (patamares.Count > 2)
                        {
                            Panel pnlPatamar2 = (Panel)e.Item.FindControl("pnlPatamar2");
                            pnlPatamar2.Visible = true;

                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2PrazoRecebimento")).Text = String.Format("{0} dia(s)", patamares[2].Prazo.ToString());
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2QtdMinParcelas")).Text = patamares[2].PatamarInicial.ToString();
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2QtdMaxParcelas")).Text = patamares[2].PatamarFinal.ToString();
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2Taxa")).Text = String.Format(@"{0:f2}", patamares[2].TaxaPatamar);
                        }
                    }
                }
            }

        }

        protected void rptVendasDebito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlTipoVenda = (Literal)e.Item.FindControl("ltlTipoVenda");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxas = (Literal)e.Item.FindControl("ltlTaxas");
                //Literal ltlQtdMinParcelas = (Literal)e.Item.FindControl("ltlQtdMinParcelas");
                //Literal ltlQtdMAxParcelas = (Literal)e.Item.FindControl("ltlQtdMAxParcelas");

                ProdutosListaDadosProdutosPorRamoCanal item = (ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem;

                ltlTipoVenda.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxas.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                //ltlQtdMinParcelas.Text = item.QtdeDefaultParcela.ToString();
                //ltlQtdMAxParcelas.Text = item.QtdeMaximaParcela.ToString();
            }
        }

        protected void rptCompletoVendasCredito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCaracteristica = (Literal)e.Item.FindControl("ltlCaracteristica");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxa = (Literal)e.Item.FindControl("ltlTaxa");
                Literal ltlDe = (Literal)e.Item.FindControl("ltlDe");
                Literal ltlAte = (Literal)e.Item.FindControl("ltlAte");
                Literal ltlLimiteParcela = (Literal)e.Item.FindControl("ltlLimiteParcelas");
                Literal ltlFormaPagto = (Literal)e.Item.FindControl("ltlFormaPgto");

                ProdutosListaDadosProdutosPorRamoCanal item = (ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem;

                ltlCaracteristica.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxa.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                ltlDe.Text = "-";
                ltlAte.Text = "-";
                ltlLimiteParcela.Text = String.Empty;
                ltlFormaPagto.Text = item.IndFormaPagamento == 'T' ? "Tarifa" : "Taxa";

                Int32 codCca = (Int32)item.CodCCA;
                Int32 codFeature = (Int32)item.CodFeature;

                if (Credenciamento.Patamares != null)
                {
                    List<Modelo.Patamar> patamares = Credenciamento.Patamares.FindAll(p => p.CodCca == codCca && p.CodFeature == codFeature);
                    if (patamares.Count > 0)
                    {
                        ((Literal)e.Item.FindControl("ltlDe")).Text = patamares[0].PatamarInicial.ToString();
                        ((Literal)e.Item.FindControl("ltlAte")).Text = patamares[0].PatamarFinal.ToString();
                        ltlLimiteParcela.Text = item.QtdeMaximaParcela.ToString();

                        if (patamares.Count > 1)
                        {
                            Panel pnlPatamar1 = (Panel)e.Item.FindControl("pnlPatamar1");
                            pnlPatamar1.Visible = true;

                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1PrazoRecebimento")).Text = String.Format("{0} dia(s)", patamares[1].Prazo.ToString());
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1Taxa")).Text = String.Format(@"{0:f2}", patamares[1].TaxaPatamar);
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1De")).Text = patamares[1].PatamarInicial.ToString();
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1Ate")).Text = patamares[1].PatamarFinal.ToString();
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1LimiteParcela")).Text = item.QtdeMaximaParcela.ToString();
                            ((Literal)pnlPatamar1.FindControl("ltlPatamar1FormaPagamento")).Text = item.IndFormaPagamento == 'T' ? "Tarifa" : "Taxa";
                        }

                        if (patamares.Count > 2)
                        {
                            Panel pnlPatamar2 = (Panel)e.Item.FindControl("pnlPatamar2");
                            pnlPatamar2.Visible = true;

                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2PrazoRecebimento")).Text = String.Format("{0} dia(s)", patamares[2].Prazo.ToString());
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2Taxa")).Text = String.Format(@"{0:f2}", patamares[2].TaxaPatamar);
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2De")).Text = patamares[2].PatamarInicial.ToString();
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2Ate")).Text = patamares[2].PatamarFinal.ToString();
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2LimiteParcela")).Text = item.QtdeMaximaParcela.ToString();
                            ((Literal)pnlPatamar2.FindControl("ltlPatamar2FormaPagamento")).Text = item.IndFormaPagamento == 'T' ? "Tarifa" : "Taxa";
                        }
                    }
                }
            }
        }

        protected void rptCompletoVendasDebito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCaracteristica = (Literal)e.Item.FindControl("ltlCaracteristica");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxa = (Literal)e.Item.FindControl("ltlTaxa");
                Literal ltlFormaPagto = (Literal)e.Item.FindControl("ltlFormaPgto");

                ProdutosListaDadosProdutosPorRamoCanal item = (ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem;

                ltlCaracteristica.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxa.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                ltlFormaPagto.Text = item.IndFormaPagamento == 'X' ? "Taxa" : "Tarifa";
            }
        }

        protected void rptCompletoVendasConstrucard_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCaracteristica = (Literal)e.Item.FindControl("ltlCaracteristica");
                Literal ltlPrazoRecebimento = (Literal)e.Item.FindControl("ltlPrazoRecebimento");
                Literal ltlTaxa = (Literal)e.Item.FindControl("ltlTaxa");
                Literal ltlFormaPagto = (Literal)e.Item.FindControl("ltlFormaPgto");

                ProdutosListaDadosProdutosPorRamoCanal item = (ProdutosListaDadosProdutosPorRamoCanal)e.Item.DataItem;

                ltlCaracteristica.Text = item.NomeFeature;
                ltlPrazoRecebimento.Text = String.Format("{0} dia(s)", item.ValorPrazoDefault);
                ltlTaxa.Text = String.Format("{0:f2}", item.ValorTaxaDefault);
                ltlFormaPagto.Text = item.IndFormaPagamento == 'X' ? "Taxa" : "Tarifa";
            }
        }

        protected void rptServicos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlCodigoServico = (Literal)e.Item.FindControl("ltlCodigoServico");
                Literal ltlNomeServico = (Literal)e.Item.FindControl("ltlNomeServico");
                Literal ltlCodigoRegime = (Literal)e.Item.FindControl("ltlCodigoRegime");
                Literal ltlQtde = (Literal)e.Item.FindControl("ltlQtde");
                Literal ltlValor = (Literal)e.Item.FindControl("ltlValor");
                Literal ltlExcedente = (Literal)e.Item.FindControl("ltlExcedente");

                Modelo.Servico item = (Modelo.Servico)e.Item.DataItem;

                ltlCodigoServico.Text = item.CodServico.ToString();
                ltlNomeServico.Text = item.DescServico;
                ltlCodigoRegime.Text = item.CodRegimeServico.ToString();
                ltlQtde.Text = item.QtdeMinima.ToString();
                ltlValor.Text = String.Format("{0:C}", item.ValorFranquia);
                ltlExcedente.Text = String.Format("{0:C}", 0);
            }
        }

        /// <summary>
        /// Data Bound da tabela de serviços
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptProdutosVan_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
                {
                    ((Literal)e.Item.FindControl("ltlCodigo")).Text = ((ProdutosListaDadosProdutosVanPorRamo)e.Item.DataItem).CodCCA.ToString();
                    ((Literal)e.Item.FindControl("ltlDescricao")).Text = ((ProdutosListaDadosProdutosVanPorRamo)e.Item.DataItem).NomeCCA;
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Contratação de Serviços", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            try
            {
                if (Credenciamento.NumSolicitacao != null && Credenciamento.NumSolicitacao != 0)
                    CancelaOcorrenciaCredenciamento();

                Credenciamento.DescricaoCanal = GetDescricaoCanal();
                Credenciamento.DescricaoCelula = GetDescricaoCelula();
                Credenciamento.DescricaoRamoAtividade = GetDescricaoRamoAtividade();
                AberturaOcorrenciaCredenciamento();
                ConsultaScript();
                CalculoScoreRisco();

                WFScoreRisco.RetornoErro erroScore = AtualizaOcorrenciaScoreRisco();

                if (erroScore.CodigoErro == 0)
                {
                    WFProposta.RetornoErro erroProposta = AtualizaSituacaoProposta();

                    if (erroProposta.CodigoErro == 0)
                    {
                        CodErroDescricaoErro erroOcorrencia = GravaOcorrenciaCredenciamento();

                        if (erroOcorrencia.CodErro == 0)
                        {
                            AtualizaOcorrenciaProposta();

                            //Limpa Sessão
                            Session["PassoMax"] = 0;
                            //System.Threading.Thread.Sleep(15000);

                            Response.Redirect("pn_conclusao.aspx", false);
                        }
                        else
                            base.ExibirPainelExcecao(erroOcorrencia.DescricaoErro, erroOcorrencia.CodErro.ToString());
                    }
                    else
                        base.ExibirPainelExcecao(erroProposta.DescricaoErro, erroProposta.CodigoErro.ToString());
                }
                else
                    base.ExibirPainelExcecao(erroScore.DescricaoErro, erroScore.CodigoErro.ToString());
            }
            catch (FaultException<WFScoreRisco.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.MsgErro, fe.Detail.CodigoErro.ToString());
            }
            catch (FaultException<WMOcorrencia.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (FaultException<GECelulas.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (FaultException<GECanais.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("pn_contracaoservicos.aspx", false);
        }

        protected void btnPropostas_Click(object sender, EventArgs e)
        {
            try
            {
                pnlDadosResumidos.Visible = !pnlDadosResumidos.Visible;
                pnlDadosCompletos.Visible = !pnlDadosCompletos.Visible;

                if (pnlDadosCompletos.Visible == true)
                {
                    // Dados Cadastrais
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                    {
                        pnlCompletoPF.Visible = false;
                        pnlCompletoPJ.Visible = true;

                        lblCPFCNPJ.Text = "CNPJ:";
                        ltlCompletoCPFCNPJ.Text = Credenciamento.CNPJ;
                        ltlCompletoCNAE.Text = Credenciamento.CNAE;
                        ltlCompletoRazaoSocial.Text = Credenciamento.RazaoSocial;
                        ltlCompletoDataFundacao.Text = Credenciamento.DataFundacao.ToString("dd/MM/yyyy");
                        rptCompletoProprietarios.DataSource = Credenciamento.Proprietarios;
                        rptCompletoProprietarios.DataBind();
                    }
                    else
                    {
                        pnlCompletoPF.Visible = true;
                        pnlCompletoPJ.Visible = false;

                        lblCPFCNPJ.Text = "CPF:";
                        ltlCompletoCPFCNPJ.Text = Credenciamento.CPF;
                        ltlCompletoNomeCompleto.Text = Credenciamento.NomeCompleto;
                        ltlCompletoDataNascimento.Text = Credenciamento.DataNascimento.ToString("dd/MM/yyyy");
                    }

                    ltlCompletoRamoAtuacao.Text = String.Format(@"{0} - {1}", Credenciamento.GrupoRamo, GetDescricaoGrupoRamo(Credenciamento.GrupoRamo));
                    ltlCompletoRamoAtividade.Text = String.Format(@"{0} - {1}", Credenciamento.RamoAtividade, GetDescricaoRamoAtividade());
                    ltlCompletoContato.Text = Credenciamento.PessoaContato;
                    ltlCompletoTelefones.Text = String.Format("({0}) {1}", Credenciamento.NumDDD1.Trim(), Credenciamento.NumTelefone1);
                    if (!String.IsNullOrEmpty(Credenciamento.NumDDDFax.Trim()) && Credenciamento.NumTelefoneFax != null && Credenciamento.NumTelefoneFax != 0)
                        ltlCompletoFax.Text = String.Format("({0}) {1}", Credenciamento.NumDDDFax.Trim(), Credenciamento.NumTelefoneFax);
                    ltlCompletoEmail.Text = Credenciamento.NomeEmail;
                    ltlCompletoSite.Text = Credenciamento.NomeHomePage;

                    // Dados Equipamento
                    ltlCompletoEquipamento.Text = Credenciamento.TipoEquipamento;
                    ltlCompletoQtde.Text = Credenciamento.QtdeTerminaisSolicitados.ToString();
                    ltlCompletoValorAluguel.Text = String.Format(@"{0:c}", Credenciamento.ValorAluguel);
                    ltlCompletoTaxaAdesao.Text = String.Format(@"{0:c}", Credenciamento.TaxaAdesao);
                    ltlCompletoEvento.Text = Credenciamento.CodEvento;

                    // Dados do Cenário
                    Cenarios cenario = BuscarDadosCenario();
                    if (cenario != null)
                    {
                        //ltlCompletoAcaoComercial.Text = cenario.DescricaoCenario;

                        ltlCompletoDesconto1.Text = cenario.ValorEscalonamentoMes1 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes1) : "-";
                        ltlCompletoDesconto2.Text = cenario.ValorEscalonamentoMes2 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes2) : "-";
                        ltlCompletoDesconto3.Text = cenario.ValorEscalonamentoMes3 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes3) : "-";
                        ltlCompletoDesconto4.Text = cenario.ValorEscalonamentoMes4 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes4) : "-";
                        ltlCompletoDesconto5.Text = cenario.ValorEscalonamentoMes5 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes5) : "-";
                        ltlCompletoDesconto6.Text = cenario.ValorEscalonamentoMes6 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes6) : "-";
                        ltlCompletoDesconto7.Text = cenario.ValorEscalonamentoMes7 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes7) : "-";
                        ltlCompletoDesconto8.Text = cenario.ValorEscalonamentoMes8 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes8) : "-";
                        ltlCompletoDesconto9.Text = cenario.ValorEscalonamentoMes9 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes9) : "-";
                        ltlCompletoDesconto10.Text = cenario.ValorEscalonamentoMes10 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes10) : "-";
                        ltlCompletoDesconto11.Text = cenario.ValorEscalonamentoMes11 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes11) : "-";
                        ltlCompletoDesconto12.Text = cenario.ValorEscalonamentoMes12 != null ? String.Format("{0}%", cenario.ValorEscalonamentoMes12) : "-";

                        Double aluguel = Credenciamento.ValorAluguel;
                        ltlCompletoValor1.Text = cenario.ValorEscalonamentoMes1 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes1 / 100)) : "-";
                        ltlCompletoValor2.Text = cenario.ValorEscalonamentoMes2 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes2 / 100)) : "-";
                        ltlCompletoValor3.Text = cenario.ValorEscalonamentoMes3 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes3 / 100)) : "-";
                        ltlCompletoValor4.Text = cenario.ValorEscalonamentoMes4 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes4 / 100)) : "-";
                        ltlCompletoValor5.Text = cenario.ValorEscalonamentoMes5 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes5 / 100)) : "-";
                        ltlCompletoValor6.Text = cenario.ValorEscalonamentoMes6 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes6 / 100)) : "-";
                        ltlCompletoValor7.Text = cenario.ValorEscalonamentoMes7 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes7 / 100)) : "-";
                        ltlCompletoValor8.Text = cenario.ValorEscalonamentoMes8 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes8 / 100)) : "-";
                        ltlCompletoValor9.Text = cenario.ValorEscalonamentoMes9 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes9 / 100)) : "-";
                        ltlCompletoValor10.Text = cenario.ValorEscalonamentoMes10 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes10 / 100)) : "-";
                        ltlCompletoValor11.Text = cenario.ValorEscalonamentoMes11 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes11 / 100)) : "-";
                        ltlCompletoValor12.Text = cenario.ValorEscalonamentoMes12 != null ? String.Format("{0:0.00}", (aluguel * cenario.ValorEscalonamentoMes12 / 100)) : "-";

                        ltlCompletoSaz1.Text = cenario.PercentualSazonalidadeJan != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeJan) : "-";
                        ltlCompletoSaz2.Text = cenario.PercentualSazonalidadeFev != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeFev) : "-";
                        ltlCompletoSaz3.Text = cenario.PercentualSazonalidadeMar != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeMar) : "-";
                        ltlCompletoSaz4.Text = cenario.PercentualSazonalidadeAbr != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeAbr) : "-";
                        ltlCompletoSaz5.Text = cenario.PercentualSazonalidadeMai != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeMai) : "-";
                        ltlCompletoSaz6.Text = cenario.PercentualSazonalidadeJun != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeJun) : "-";
                        ltlCompletoSaz7.Text = cenario.PercentualSazonalidadeJul != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeJul) : "-";
                        ltlCompletoSaz8.Text = cenario.PercentualSazonalidadeAgo != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeAgo) : "-";
                        ltlCompletoSaz9.Text = cenario.PercentualSazonalidadeSet != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeSet) : "-";
                        ltlCompletoSaz10.Text = cenario.PercentualSazonalidadeOut != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeOut) : "-";
                        ltlCompletoSaz11.Text = cenario.PercentualSazonalidadeNov != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeNov) : "-";
                        ltlCompletoSaz12.Text = cenario.PercentualSazonalidadeDez != null ? String.Format("{0:0.00}", cenario.PercentualSazonalidadeDez) : "-";
                    }
                    else
                    {
                        ltlCompletoDesconto1.Text = "0%";
                        ltlCompletoDesconto2.Text = "0%";
                        ltlCompletoDesconto3.Text = "0%";
                        ltlCompletoDesconto4.Text = "0%";
                        ltlCompletoDesconto5.Text = "0%";
                        ltlCompletoDesconto6.Text = "0%";
                        ltlCompletoDesconto7.Text = "0%";
                        ltlCompletoDesconto8.Text = "0%";
                        ltlCompletoDesconto9.Text = "0%";
                        ltlCompletoDesconto10.Text = "0%";
                        ltlCompletoDesconto11.Text = "0%";
                        ltlCompletoDesconto12.Text = "0%";

                        ltlCompletoValor1.Text = "0,00";
                        ltlCompletoValor2.Text = "0,00";
                        ltlCompletoValor3.Text = "0,00";
                        ltlCompletoValor4.Text = "0,00";
                        ltlCompletoValor5.Text = "0,00";
                        ltlCompletoValor6.Text = "0,00";
                        ltlCompletoValor7.Text = "0,00";
                        ltlCompletoValor8.Text = "0,00";
                        ltlCompletoValor9.Text = "0,00";
                        ltlCompletoValor10.Text = "0,00";
                        ltlCompletoValor11.Text = "0,00";
                        ltlCompletoValor12.Text = "0,00";

                        ltlCompletoSaz1.Text = "0,00";
                        ltlCompletoSaz2.Text = "0,00";
                        ltlCompletoSaz3.Text = "0,00";
                        ltlCompletoSaz4.Text = "0,00";
                        ltlCompletoSaz5.Text = "0,00";
                        ltlCompletoSaz6.Text = "0,00";
                        ltlCompletoSaz7.Text = "0,00";
                        ltlCompletoSaz8.Text = "0,00";
                        ltlCompletoSaz9.Text = "0,00";
                        ltlCompletoSaz10.Text = "0,00";
                        ltlCompletoSaz11.Text = "0,00";
                        ltlCompletoSaz12.Text = "0,00";
                    }

                    // Dados do PDV
                    ltlCompletoRENPAC.Text = Credenciamento.NroRenpac != 0 ? Credenciamento.NroRenpac.ToString() : String.Empty;
                    ltlCompletoSoftwareTEF.Text = Credenciamento.NomeSoftwareTEF;
                    ltlCompletoMarcaPDV.Text = Credenciamento.NomeMarcaPDV;

                    // Dados de Endereço
                    ltlCompletoEnderecoComercial.Text = String.Format("{0}, {1}, {2} - CEP {3}, {4}, {5}",
                        Credenciamento.EnderecoComercial.Logradouro, Credenciamento.EnderecoComercial.Numero,
                        Credenciamento.EnderecoComercial.Complemento, Credenciamento.EnderecoComercial.CEP,
                        Credenciamento.EnderecoComercial.Cidade, Credenciamento.EnderecoComercial.Estado);
                    ltlCompletoEnderecoCorrespondencia.Text = String.Format("{0}, {1}, {2} - CEP {3}, {4}, {5}",
                        Credenciamento.EnderecoCorrespondencia.Logradouro, Credenciamento.EnderecoCorrespondencia.Numero,
                        Credenciamento.EnderecoCorrespondencia.Complemento, Credenciamento.EnderecoCorrespondencia.CEP,
                        Credenciamento.EnderecoCorrespondencia.Cidade, Credenciamento.EnderecoCorrespondencia.Estado);
                    ltlCompletoEnderecoInstalacao.Text = String.Format("{0}, {1}, {2} - CEP {3}, {4}, {5}",
                        Credenciamento.EnderecoInstalacao.Logradouro, Credenciamento.EnderecoInstalacao.Numero,
                        Credenciamento.EnderecoInstalacao.Complemento, Credenciamento.EnderecoInstalacao.CEP,
                        Credenciamento.EnderecoInstalacao.Cidade, Credenciamento.EnderecoInstalacao.Estado);
                    ltlCompletoHorarioFuncionamento.Text = String.Format("{0} à {1} - {2} às {3}",
                        Credenciamento.DiaInicioFuncionamento, Credenciamento.DiaFimFuncionamento,
                        Credenciamento.HoraInicioFuncionamento, Credenciamento.HoraFimFuncionamento);
                    ltlCompletoContatoInstalacao.Text = Credenciamento.NomeContatoInstalacao;
                    ltlCompletoTelefoneInstalacao.Text = String.Format("({0}) {1}", Credenciamento.NumDDDInstalacao, Credenciamento.NumTelefoneInstalacao);
                    //ltlCompletoDataHorarioInstalacao.Text = String.Format("{0} à {1} - {2} às {3}",
                    //    Credenciamento.DiaInicioInstalacao, Credenciamento.DiaFimInstalacao,
                    //    Credenciamento.HoraInicioInstalacao, Credenciamento.HoraFimInstalacao);

                    ltlCompletoObs.Text = Credenciamento.Observacao;
                    //if (!String.IsNullOrEmpty(Credenciamento.Observacao))
                    //{
                    //    int obsIndex = Credenciamento.Observacao.LastIndexOf("#OBS:");
                    //    if (obsIndex != -1)
                    //    {
                    //        ltlCompletoPontoReferencia.Text = Credenciamento.Observacao.Substring(0, obsIndex).Trim().Replace("#PTREF:", "");
                    //    }
                    //}

                    // Dados Operacionais
                    ltlCompletoNomeFatura.Text = Credenciamento.NomeFatura;
                    ltlCompletoFuncionamento.Text = Credenciamento.HorarioFuncionamento == 0 ? "Comercial" : "Noturno";

                    String tipoEstabelecimento = "Autônomo";
                    if (Credenciamento.CodTipoEstabelecimento == 2)
                        tipoEstabelecimento = "Matriz";
                    else if (Credenciamento.CodTipoEstabelecimento == 1)
                        tipoEstabelecimento = "Filial";

                    ltlCompletoTipoEstabelecimento.Text = tipoEstabelecimento;
                    ltlCompletoDataAssinatura.Text = Credenciamento.DataCadastroProposta.ToString("dd/MM/yyyy");
                    ltlCompletoLocalPagamento.Text = Credenciamento.LocalPagamento == 1 ? "Estabelecimento" : "Centralizadora - PV Centralizador: " + Credenciamento.Centralizadora;

                    // Dados Bancários
                    if (Credenciamento.ProdutosCredito != null && Credenciamento.ProdutosCredito.Count > 0)
                    {
                        ltlCompletoBancoCredito.Text = Credenciamento.NomeBancoCredito;
                        ltlCompletoAgenciaCredito.Text = Credenciamento.AgenciaCredito.ToString(); //TODO
                        ltlCompletoContaCorrenteCredito.Text = Credenciamento.ContaCredito;
                    }
                    else
                        pnlDomicilioCredito.Visible = false;

                    if (Credenciamento.ProdutosDebito != null && Credenciamento.ProdutosDebito.Count > 0)
                    {
                        ltlCompletoBancoDebito.Text = Credenciamento.NomeBancoDebito;
                        ltlCompletoAgenciaDebito.Text = Credenciamento.AgenciaDebito.ToString(); //TODO
                        ltlCompletoContaCorrenteDebito.Text = Credenciamento.ContaDebito;
                    }
                    else
                        pnlDomicilioDebito.Visible = false;

                    if (Credenciamento.ContaConstrucard != null && Credenciamento.ProdutosConstrucard.Count > 0)
                    {
                        pnlConstrucard.Visible = true;
                        ltlCompletoBancoConstrucard.Text = Credenciamento.NomeBancoConstrucard;
                        ltlCompletoAgenciaConstrucard.Text = Credenciamento.AgenciaConstrucard.ToString(); //TODO
                        ltlCompletoContaCorrenteConstrucard.Text = Credenciamento.ContaConstrucard;
                    }

                    //Dados Produtos
                    var produtos = (from p in Credenciamento.ProdutosCredito
                                    group p by p.CodFeature
                                        into grp
                                        select grp.First()).ToArray();

                    if (produtos.Count() > 0)
                    {
                        rptCompletoVendasCredito.DataSource = produtos;
                        rptCompletoVendasCredito.DataBind();
                    }
                    else
                        pnlCompletoCredito.Visible = false;

                    var produtosDebito = (from p in Credenciamento.ProdutosDebito
                                    group p by p.CodFeature
                                        into grp
                                        select grp.First()).ToArray();

                    if (produtosDebito.Count() > 0)
                    {
                        rptCompletoVendasDebito.DataSource = produtosDebito;
                        rptCompletoVendasDebito.DataBind();
                    }
                    else
                        pnlCompletoDebito.Visible = false;

                    if (Credenciamento.ContaConstrucard != null && Credenciamento.ProdutosConstrucard.Count > 0)
                    {
                        rptCompletoVendasConstrucard.DataSource = Credenciamento.ProdutosConstrucard;
                        rptCompletoVendasConstrucard.DataBind();
                    }
                    else
                        pnlCompletoConstrucard.Visible = false;

                    //Dados Serviços
                    if (String.Compare(Credenciamento.TipoEquipamento, "TOL") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "SNT") == 0 ||
                        String.Compare(Credenciamento.TipoEquipamento, "TOF") == 0)
                    {
                        ltlHeaderValor.Text = "Valor (mensal)";
                    }

                    if (Credenciamento.Servicos.Count > 0)
                    {
                        pnlServicos.Visible = true;
                        rptServicos.DataSource = Credenciamento.Servicos;
                        rptServicos.DataBind();
                    }

                    //Dados Produtos Van
                    if (Credenciamento.ProdutosVan.Count > 0)
                    {
                        pnlProdutosVan.Visible = true;
                        rptProdutosVan.DataSource = Credenciamento.ProdutosVan;
                        rptProdutosVan.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Confirmação Dados", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            //TODO: Implementar impressão
        }

        protected void btnNovaProposta_Click(object sender, EventArgs e)
        {
            Credenciamento = new Modelo.Credenciamento();
            Response.Redirect("pn_dadosiniciais.aspx");
        }

        #endregion

        #region [ Métodos Auxiliares ]

        private void CarregaDados()
        {
            if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
            {
                pnlPJ.Visible = true;
                pnlPF.Visible = false;

                ltlCnpj.Text = Credenciamento.CNPJ;
                ltlRazaoSocial.Text = Credenciamento.RazaoSocial;

                rptProprietarios.DataSource = Credenciamento.Proprietarios;
                rptProprietarios.DataBind();
            }
            else
            {
                pnlPJ.Visible = false;
                pnlPF.Visible = true;

                ltlCPF.Text = Credenciamento.CPF;
                ltlNomeCompleto.Text = Credenciamento.NomeCompleto;
            }

            ltlCEP.Text = Credenciamento.EnderecoComercial.CEP;
            ltlEndereco.Text = Credenciamento.EnderecoComercial.Logradouro;
            ltlNumero.Text = Credenciamento.EnderecoComercial.Numero;
            ltlBairro.Text = Credenciamento.EnderecoComercial.Bairro;
            ltlCidade.Text = Credenciamento.EnderecoComercial.Cidade;
            ltlUF.Text = Credenciamento.EnderecoComercial.Estado;

            ltlEquipamento.Text = Credenciamento.TipoEquipamento;
            ltlQuantidade.Text = Credenciamento.QtdeTerminaisSolicitados.ToString();
            ltlValorAluguel.Text = String.Format(@"{0:c}", Credenciamento.ValorAluguel);

            if (Credenciamento.ProdutosCredito != null && Credenciamento.ProdutosCredito.Count > 0)
            {
                var produtos = (from p in Credenciamento.ProdutosCredito
                                group p by p.CodFeature
                                    into grp
                                    select grp.First()).ToArray();

                rptVendaCredito.DataSource = produtos;
                rptVendaCredito.DataBind();
            }
            else
                pnlCredito.Visible = false;

            if (Credenciamento.ProdutosDebito != null && Credenciamento.ProdutosDebito.Count > 0)
            {
                var produtos = (from p in Credenciamento.ProdutosDebito
                                group p by p.CodFeature
                                    into grp
                                    select grp.First()).ToArray();

                rptVendasDebito.DataSource = produtos;
                rptVendasDebito.DataBind();
            }
            else
                pnlDebito.Visible = false;
        }

        /// <summary>
        /// Busca informações detalhadas do cenário escolhido
        /// </summary>
        private Cenarios BuscarDadosCenario()
        {
            ServicoPortalTGCenariosClient client = new ServicoPortalTGCenariosClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carrega dados do cenário"))
                {
                    if (Credenciamento.CodCenario != null)
                    {
                        Int32 codCenario = (Int32)Credenciamento.CodCenario;
                        Int32 codCanal = Credenciamento.Canal;
                        String codTipoEquipamento = Credenciamento.CodTipoEquipamento;
                        Char codSituacaoCenarioCanal = 'A';
                        String codCampanha = !String.IsNullOrEmpty(Credenciamento.CodCampanha) ? Credenciamento.CodCampanha : null;
                        String codOrigemChamada = null;

                        Cenarios[] cenarios = client.ListaDadosCadastrais(codCenario, codCanal, codTipoEquipamento, codSituacaoCenarioCanal, codCampanha, codOrigemChamada);
                        client.Close();

                        if (cenarios.Length == 1)
                            return cenarios[0];
                    }
                    return null;
                }
            }
            catch (FaultException<TGCenarios.ModelosErroServicos> fe)
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
        /// Atualiza a situação da proposta
        /// </summary>
        private WFProposta.RetornoErro AtualizaSituacaoProposta()
        {
            ServicoPortalWFPropostaClient client = new ServicoPortalWFPropostaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Atualiza situação da proposta"))
                {
                    Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJ = 0;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                    Int32 numSeqProp = (Int32)Credenciamento.NumSequencia;
                    Char indSituacaoProposta = 'P';
                    String usuarioUltimaAtualizacao = SessaoAtual.LoginUsuario;
                    Int32? codMotivoRecusa = null;
                    Int32 numeroPontoVenda = Credenciamento.NumPdv != null ? (Int32)Credenciamento.NumPdv : 0;
                    Int32? indOrigemAtualizacao = 1;

                    WFProposta.RetornoErro[] retorno = client.AtualizaSituacaoProposta(
                        codTipoPessoa,
                        numCNPJ,
                        numSeqProp,
                        indSituacaoProposta,
                        usuarioUltimaAtualizacao,
                        codMotivoRecusa,
                        numeroPontoVenda,
                        indOrigemAtualizacao
                        );

                    client.Close();

                    return retorno[0];
                }
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
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
        /// Abertura de Ocorrência para o Credenciamento
        /// </summary>
        private void AberturaOcorrenciaCredenciamento()
        {
            ServicoPortalWMOcorrenciaClient client = new ServicoPortalWMOcorrenciaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Abertura Ocorrencia Credenciamento"))
                {
                    String usuarioOcorrencia = SessaoAtual.LoginUsuario;
                    Int32 numPontoVenda = Credenciamento.NumPdv != null ? (Int32)Credenciamento.NumPdv : 0;
                    String codTipoPessoa = Credenciamento.TipoPessoa;
                    String codTipoAmbiente = WebConfigurationManager.AppSettings["ambiente"].ToString();
                    String numCNPJCPFCliente = String.Empty;
                    if (Credenciamento.TipoPessoa == "J")
                        numCNPJCPFCliente = Credenciamento.CNPJ.Replace(".", "").Replace("-", "").Replace("/", "");
                    else
                        numCNPJCPFCliente = Credenciamento.CPF.Replace(".", "").Replace("-", "");
                    String descricaoOcorrencia = "ABERTURA DE SOLICITACAO PARA FILIACAO PELO PORTAL";

                    AberturaOcorrenciaCredenciamento[] retorno = client.AberturaOcorrenciaCredenciamento(
                        usuarioOcorrencia,
                        numPontoVenda,
                        codTipoPessoa,
                        codTipoAmbiente,
                        numCNPJCPFCliente,
                        descricaoOcorrencia
                        );
                    client.Close();

                    Credenciamento.CodCasoOcorrencia = retorno[0].CodCasoOcorrencia;
                    Logger.GravarLog(String.Format("retorno[0].CodCasoOcorrencia: {0}", retorno[0].CodCasoOcorrencia));
                    Credenciamento.DataRequisicaoOcorrencia = (DateTime)retorno[0].DataRequisicaoOcorrencia;
                    Credenciamento.NumRequisicaoOcorrencia = retorno[0].NumRequisicaoOcorrencia;
                    Logger.GravarLog(String.Format("retorno[0].NumRequisicaoOcorrencia: {0}", retorno[0].NumRequisicaoOcorrencia));
                    Credenciamento.NumSolicitacao = retorno[0].NumSolicitacao;
                    Logger.GravarLog(String.Format("retorno[0].NumSolicitacao: {0}", retorno[0].NumSolicitacao));
                }
            }
            catch (FaultException<WMOcorrencia.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Confirmação Dados", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Confirmação Dados", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Confirmação Dados", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Confirmação Dados", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Consulta Scripts
        /// </summary>
        private void ConsultaScript()
        {
            ServicoPortalWFAdministracaoClient client = new ServicoPortalWFAdministracaoClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Consulta Script"))
                {
                    ConsultaScripts[] retorno = client.ConsultaScripts(970, null);
                    client.Close();

                    Logger.GravarLog(String.Format("Retorno Consulta Script: {0}", retorno[0].DescricaoScript));
                    Credenciamento.DescricaoScript = retorno[0].DescricaoScript;
                }
            }
            catch (FaultException<WFAdministracao.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Confirmação Dados", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Confirmação Dados", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Confirmação Dados", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Confirmação Dados", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Grava Ocorrencia Credenciamento
        /// </summary>
        private CodErroDescricaoErro GravaOcorrenciaCredenciamento()
        {
            ServicoPortalWMOcorrenciaClient client = new ServicoPortalWMOcorrenciaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Grava Ocorrencia Credenciamento"))
                {
                    Int32 numRequisicaoOcorrencia = (Int32)Credenciamento.NumRequisicaoOcorrencia;
                    Logger.GravarLog(String.Format("NumRequisicaoOcorrencia: {0}", (Int32)Credenciamento.NumRequisicaoOcorrencia));
                    DateTime dataRequisicaoOcorrencia = Credenciamento.DataRequisicaoOcorrencia;
                    String usuarioOcorrencia = SessaoAtual.LoginUsuario;
                    Int32 numSolicitacao = (Int32)Credenciamento.NumSolicitacao;
                    Logger.GravarLog(String.Format("NumSolicitacao: {0}", (Int32)Credenciamento.NumSolicitacao));
                    Int32 codCasoOcorrencia = (Int32)Credenciamento.CodCasoOcorrencia;
                    Logger.GravarLog(String.Format("CodCasoOcorrencia: {0}", (Int32)Credenciamento.CodCasoOcorrencia));
                    String codTipoOcorrencia = "FILI9901";

                    Int64 numCNPJCPF = 0;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJCPF);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJCPF);
                    Int32 numSeqProposta = (Int32)Credenciamento.NumSequencia;
                    String razaoSocial = String.Empty;

                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        razaoSocial = Credenciamento.RazaoSocial;
                    else
                        razaoSocial = Credenciamento.NomeCompleto;

                    Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    DateTime dataFundacao = Credenciamento.DataFundacao;
                    Int32 codGrupoRamo = Credenciamento.GrupoRamo;
                    Int32 codRamoAtividade = Credenciamento.RamoAtividade;
                    String descRamoAtividade = Credenciamento.DescricaoRamoAtividade;
                    String nomeFatura = !String.IsNullOrEmpty(Credenciamento.NomeFatura) ? Credenciamento.NomeFatura : String.Empty;
                    String logradouro = !String.IsNullOrEmpty(Credenciamento.EnderecoComercial.Logradouro) ? Credenciamento.EnderecoComercial.Logradouro : String.Empty;
                    String complementoEndereco = !String.IsNullOrEmpty(Credenciamento.EnderecoComercial.Complemento) ? Credenciamento.EnderecoComercial.Complemento : String.Empty;
                    String numeroEndereco = !String.IsNullOrEmpty(Credenciamento.EnderecoComercial.Numero) ? Credenciamento.EnderecoComercial.Numero : String.Empty;
                    String bairro = !String.IsNullOrEmpty(Credenciamento.EnderecoComercial.Bairro) ? Credenciamento.EnderecoComercial.Bairro : String.Empty;
                    String cidade = !String.IsNullOrEmpty(Credenciamento.EnderecoComercial.Cidade) ? Credenciamento.EnderecoComercial.Cidade : String.Empty;
                    String estado = !String.IsNullOrEmpty(Credenciamento.EnderecoComercial.Estado) ? Credenciamento.EnderecoComercial.Estado : String.Empty;
                    String codigoCep = !String.IsNullOrEmpty(Credenciamento.EnderecoComercial.CEP) ? Credenciamento.EnderecoComercial.CEP.Replace("-", "").Substring(0, 5) : String.Empty;
                    String codComplementoCep = !String.IsNullOrEmpty(Credenciamento.EnderecoComercial.CEP) ? Credenciamento.EnderecoComercial.CEP.Replace("-", "").Substring(5, 3) : String.Empty;
                    String pessoaContato = Credenciamento.PessoaContato;
                    String numDDD1 = !String.IsNullOrEmpty(Credenciamento.NumDDD1) ? Credenciamento.NumDDD1 : String.Empty;
                    Int32 numTelefone1 = Credenciamento.NumTelefone1 != null ? (Int32)Credenciamento.NumTelefone1 : 0;
                    Int32 ramal1 = Credenciamento.Ramal1 != null ? (Int32)Credenciamento.Ramal1 : 0;
                    String numDDDFax = !String.IsNullOrEmpty(Credenciamento.NumDDDFax) ? Credenciamento.NumDDDFax : String.Empty;
                    Int32 numTelefoneFax = Credenciamento.NumTelefoneFax != null ? (Int32)Credenciamento.NumTelefoneFax : 0;
                    Int32 codFilial = Credenciamento.CodFilial != null ? (Int32)Credenciamento.CodFilial : 0;
                    Int32 numeroPontoVenda = Credenciamento.NumPdv != null ? (Int32)Credenciamento.NumPdv : 0;
                    Char codCategoriaPontoVenda = ' ';
                    Char indPropostaEmissor = Credenciamento.Canal == 1 ? 'S' : 'N';
                    Int32 codCanal = Credenciamento.Canal;
                    String descCanal = Credenciamento.DescricaoCanal;
                    Int32 codCelula = Credenciamento.Celula;
                    String descCelula = Credenciamento.DescricaoCelula;
                    Int32 codPesoTarget = 99;
                    Char indProntaInstalacao = 'N';
                    String textoScript = Credenciamento.DescricaoScript.ToUpper();
                    Char indMatrizRisco = 'S';

                    Logger.GravarLog("Chamada ao serviço WM");
                    CodErroDescricaoErro[] retorno = client.GravaOcorrenciaCredenciamento(
                        numRequisicaoOcorrencia,
                        dataRequisicaoOcorrencia,
                        usuarioOcorrencia,
                        numSolicitacao,
                        codCasoOcorrencia,
                        codTipoOcorrencia,
                        numCNPJCPF,
                        numSeqProposta,
                        razaoSocial,
                        codTipoPessoa,
                        dataFundacao,
                        codGrupoRamo,
                        codRamoAtividade,
                        descRamoAtividade,
                        nomeFatura,
                        logradouro,
                        complementoEndereco,
                        numeroEndereco,
                        bairro,
                        cidade,
                        estado,
                        codigoCep,
                        codComplementoCep,
                        pessoaContato,
                        numDDD1,
                        numTelefone1,
                        ramal1,
                        numDDDFax,
                        numTelefoneFax,
                        codFilial,
                        numeroPontoVenda,
                        codCategoriaPontoVenda,
                        indPropostaEmissor,
                        codCanal,
                        descCanal,
                        codCelula,
                        descCelula,
                        codPesoTarget,
                        indProntaInstalacao,
                        textoScript,
                        indMatrizRisco
                        );
                    client.Close();

                    return retorno[0];
                }
            }
            catch (FaultException<WMOcorrencia.ModelosErroServicos> fe)
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
        /// Atualiza Ocorrencia Proposta
        /// </summary>
        private void AtualizaOcorrenciaProposta()
        {
            ServicoPortalWFPropostaClient client = new ServicoPortalWFPropostaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Atualiza Ocorrencia Proposta"))
                {
                    Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int64 numCNPJ = 0;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                    Int32 numSeqProp = (Int32)Credenciamento.NumSequencia;
                    Int32 numOcorrencia = (Int32)Credenciamento.NumSolicitacao;
                    DateTime? dataAberturaOcorrencia = DateTime.Now;
                    String usuarioUltimaAtualizacao = SessaoAtual.LoginUsuario;

                    WFProposta.RetornoErro[] retorno = client.AtualizaOcorrenciaProposta(
                        codTipoPessoa,
                        numCNPJ,
                        numSeqProp,
                        numOcorrencia,
                        dataAberturaOcorrencia,
                        usuarioUltimaAtualizacao);

                    client.Close();
                }
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Confirmação Dados", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodigoErro);
            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Confirmação Dados", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Confirmação Dados", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Confirmação Dados", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Calculo do score de risco
        /// </summary>
        private void CalculoScoreRisco()
        {
            ServicoPortalWFScoreRiscoClient client = new ServicoPortalWFScoreRiscoClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Calculo do Score de Risco"))
                {
                    Int64 numCNPJ = 0;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                    Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int32 numOcorrencia = (Int32)Credenciamento.NumSolicitacao;
                    DateTime dataFundacao = String.Compare(Credenciamento.TipoPessoa, "J") == 0 ? Credenciamento.DataFundacao : Credenciamento.DataNascimento;
                    String usuario = SessaoAtual.LoginUsuario;
                    Int32 codGrupoRamo = Credenciamento.GrupoRamo;
                    Int32 codRamoAtivididade = Credenciamento.RamoAtividade;
                    String codCEP = Credenciamento.CEP.Substring(0, 5);
                    Int32 codTipoEstabelecimento = (Int32)Credenciamento.CodTipoEstabelecimento;
                    Int32 codCanal = Credenciamento.Canal;
                    String codTipoEquipamento = Credenciamento.TipoEquipamento;
                    Int32 codBanco = Credenciamento.CodBancoCredito;
                    Int32 codServico = 0;

                    var retorno = client.CalculoScoreRisco(
                        numCNPJ,
                        codTipoPessoa,
                        numOcorrencia,
                        dataFundacao,
                        usuario,
                        codGrupoRamo,
                        codRamoAtivididade,
                        codCEP,
                        codTipoEstabelecimento,
                        codCanal,
                        codTipoEquipamento,
                        codBanco,
                        codServico);
                    client.Close();

                    Credenciamento.DataScoreRisco = retorno[0].DataSituacaoScore;
                }
            }
            catch (FaultException<WFScoreRisco.ModelosErroServicos> fe)
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
        /// Atualiza Ocorrencia Score Risco
        /// </summary>
        /// <returns></returns>
        private WFScoreRisco.RetornoErro AtualizaOcorrenciaScoreRisco()
        {
            ServicoPortalWFScoreRiscoClient client = new ServicoPortalWFScoreRiscoClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Atualiza Ocorrencia Score Risco"))
                {
                    Int64 numCNPJ = 0;
                    if (String.Compare(Credenciamento.TipoPessoa, "J") == 0)
                        Int64.TryParse(Credenciamento.CNPJ.Replace("/", "").Replace(".", "").Replace("-", ""), out numCNPJ);
                    else
                        Int64.TryParse(Credenciamento.CPF.Replace(".", "").Replace("-", ""), out numCNPJ);

                    Char codTipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                    Int32 numOcorrencia = (Int32)Credenciamento.NumSolicitacao;
                    DateTime dataSituacaoScore = (DateTime)Credenciamento.DataScoreRisco;

                    WFScoreRisco.RetornoErro[] retorno = client.AtualizaOcorrenciaScoreRisco(numCNPJ, codTipoPessoa, numOcorrencia, dataSituacaoScore);
                    client.Close();

                    return retorno[0];
                }
            }
            catch (FaultException<WFScoreRisco.ModelosErroServicos> fe)
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
        /// Retorna descricao da celula
        /// </summary>
        /// <returns></returns>
        private String GetDescricaoCelula()
        {
            ServicoPortalGECelulasClient client = new ServicoPortalGECelulasClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Atualiza situação da proposta"))
                {
                    CelulasoListaDadosCadastraisPorCanal[] retorno = client.ListaDadosCadastraisPorCanal(Credenciamento.Canal, Credenciamento.Celula, null);
                    client.Close();

                    return retorno[0].NomeCelula;
                }
            }
            catch (FaultException<GECelulas.ModelosErroServicos> fe)
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
        /// Retorna a descrição do canal
        /// </summary>
        /// <returns></returns>
        private String GetDescricaoCanal()
        {
            ServicoPortalGECanaisClient client = new ServicoPortalGECanaisClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Atualiza situação da proposta"))
                {
                    CanaisListaDadosCadastrais[] retorno = client.ListaDadosCadastrais(null, Credenciamento.Canal, "=");
                    client.Close();

                    return retorno[0].NomeCanal;
                }
            }
            catch (FaultException<GECanais.ModelosErroServicos> fe)
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
        /// Retorna a descrição do ramo atividade
        /// </summary>
        /// <returns></returns>
        private String GetDescricaoRamoAtividade()
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Atualiza situação da proposta"))
                {
                    RamosAtividadesListaDadosCadastraisRamosAtividades[] retorno = client.ListaDadosCadastraisRamosAtividades(Credenciamento.GrupoRamo, Credenciamento.RamoAtividade);
                    client.Close();

                    return retorno[0].DescrRamoAtividade;
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
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
        /// Retorna a descrição do Grupo de Atividade
        /// </summary>
        /// <returns></returns>
        private String GetDescricaoGrupoRamo(Int32 codGrupoRamo)
        {
            ServicoPortalGERamosAtividadesClient client = new ServicoPortalGERamosAtividadesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Atualiza situação da proposta"))
                {
                    var retorno = client.ListaDadosCadastraisGruposRamosAtividades();
                    client.Close();

                    var grupoRamo = retorno.FirstOrDefault(g => g.CodGrupoRamoAtividade == codGrupoRamo);

                    if(grupoRamo != null)
                        return grupoRamo.DescrRamoAtividade;

                    return codGrupoRamo.ToString();
                }
            }
            catch (FaultException<GERamosAtd.ModelosErroServicos> fe)
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
        /// Cancela Ocorrência Credenciamento
        /// </summary>
        private void CancelaOcorrenciaCredenciamento()
        {
            ServicoPortalWMOcorrenciaClient client = new ServicoPortalWMOcorrenciaClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Cancela Ocorrencia Credenciamento"))
                {
                    String usuarioOcorrencia = SessaoAtual.LoginUsuario;
                    Int32 numSolicitacao = (Int32)Credenciamento.NumSolicitacao;
                    Int32 codCasoOcorrencia = 1;
                    String motivoCancelamento = "Solicitação de executar novamente Matriz de Risco via portal";
                    String obsCancelamento = "CREDENCIAMENTO PORTAL";

                    client.CancelaOcorrenciaCredenciamento(
                        usuarioOcorrencia, 
                        numSolicitacao, 
                        codCasoOcorrencia, 
                        motivoCancelamento, 
                        obsCancelamento);
                    client.Close();
                }
            }
            catch (FaultException<WMOcorrencia.ModelosErroServicos> fe)
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
