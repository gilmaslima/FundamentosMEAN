using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.GEServicoInformacaoComercial;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.ZPServicoTerminalContratado;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.Layouts.Redecard.PN.Extrato.SharePoint
{
    public partial class DownloadAceiteCondicoesComerciais : ApplicationPageBaseAnonima
    {
        /// <summary>
        /// Verificar ser o controle está sendo renderizado dentro de uma
        /// tag runat=server. Este método foi sobrescito para gerar o HTML do controle sem
        /// essa verificação
        /// </summary>
        public override void VerifyRenderingInServerForm(Control control)
        {
        }
        public override bool EnableEventValidation
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Pré renderização da página
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Carregamento das informações do aceite de condições comerciais."))
            {
                try
                {
                    QueryStringSegura query = new QueryStringSegura(Request.QueryString["num_pdv"]);
                    Int32 numeroPDV = Int32.Parse(query["NUM_PDV"]);
                    Boolean reimpressao =  query.AllKeys.Contains("ModoReimpressao");

                    CarregarAceiteCondicoesComerciais(numeroPDV, reimpressao);
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogMensagem("PortalRedecard Exception - DownloadAceiteCondicoesComerciais.aspx.cs");
                    SharePointUlsLog.LogErro(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogMensagem("Exception - DownloadAceiteCondicoesComerciais.aspx.cs");
                    SharePointUlsLog.LogErro(ex);
                }
            }
        }

        /// <summary>
        /// Evento para carregar informação de tipo de Endereços no lightbox de Aceite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptEnderecos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var endereco = (EnderecoResponse)e.Item.DataItem as EnderecoResponse;
            var literalTipoEndereco = (Literal)e.Item.FindControl("ltrTipoEndereco");
            literalTipoEndereco.Text = endereco.TipoEndereco.HasValue ? endereco.TipoEndereco.Value == 1 ? "Endereço do Estabelecimento" : "Endereço de Correspondência" : String.Empty;
        }

        /// <summary>
        /// Evento para carregar informações de Taxas de domicílios no lightbox de Aceite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptDomicilioBancario_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var domicilioBancario = (DomicilioBancarioResponse)e.Item.DataItem as DomicilioBancarioResponse;
            var grvTaxas = (GridView)e.Item.FindControl("grvTaxas");
            grvTaxas.DataSource = domicilioBancario.Taxas;
            grvTaxas.DataBind();
        }

        /// <summary>
        /// Verifica e carrega informações para aceite de condições comerciais.
        /// </summary>
        private void CarregarAceiteCondicoesComerciais(Int32 numeroPDV, Boolean reimpressao)
        {
            using (var contextoGE = new ContextoWCF<ServicoInformacaoComercialClient>())
            {
                // Obtem informações de aceite
                InformacaoComercialResponse aceite = null;

                if(reimpressao) // para reimpressao é utilizado o metodo que retorna os dados do PV mesmo apos o aceite
                    aceite = contextoGE.Cliente.Recuperar(numeroPDV);
                else
                    aceite = contextoGE.Cliente.Consultar(new InformacaoComercialRequest() { NumeroPDV = numeroPDV });

                if (aceite.NumeroPDV != null && aceite.NumeroPDV != default(Decimal?))
                {
                    // Preenche informações no lightbox de aceite
                    ltrRazaoSocial.Text = aceite.RazaoSocial;
                    ltrRamoAtividade.Text = String.Format("{0}-{1}", aceite.CodigoRamoAtividade, aceite.DescricaoRamoAtividade);
                    ltrResponsavel.Text = aceite.Responsavel;

                    // Tratamento de preenchimento de telefones
                    String telefone1 = String.Empty;
                    String telefone2 = String.Empty;
                    if (aceite.Telefone1.GetValueOrDefault() > 0 && aceite.Telefone1.ToString().Length == 8)
                        telefone1 = String.Format("{0}-{1}", aceite.Telefone1.ToString().Substring(0, 4), aceite.Telefone1.ToString().Substring(4, 4));
                    else if (aceite.Telefone1.GetValueOrDefault() > 0 && aceite.Telefone1.ToString().Length == 9)
                        telefone1 = String.Format("{0}-{1}", aceite.Telefone1.ToString().Substring(0, 5), aceite.Telefone1.ToString().Substring(5, 4));
                    if (aceite.Telefone2.GetValueOrDefault() > 0 && aceite.Telefone2.ToString().Length == 8)
                        telefone2 = String.Format("{0}-{1}", aceite.Telefone2.ToString().Substring(0, 4), aceite.Telefone2.ToString().Substring(4, 4));
                    else if (aceite.Telefone2.GetValueOrDefault() > 0 && aceite.Telefone2.ToString().Length == 9)
                        telefone2 = String.Format("{0}-{1}", aceite.Telefone2.ToString().Substring(0, 5), aceite.Telefone2.ToString().Substring(5, 4));

                    StringBuilder telefones = new StringBuilder();
                    if (!String.IsNullOrWhiteSpace(telefone1))
                        telefones.Append(String.Format("({0}) {1}", aceite.DDD1, telefone1, aceite.Ramal1));
                    if (aceite.Ramal1.GetValueOrDefault() > 0)
                        telefones.Append(String.Format(" R: {0}", aceite.Ramal1));
                    if (!String.IsNullOrWhiteSpace(telefone2))
                        telefones.Append(String.Format(" / ({0}) {1}", aceite.DDD2, telefone2));
                    if (aceite.Ramal2.GetValueOrDefault() > 0)
                        telefones.Append(String.Format(" R: {0}", aceite.Ramal2));
                    ltrTelefones.Text = telefones.ToString();


                    grvSocios.DataSource = aceite.Socios;
                    grvSocios.DataBind();

                    rptEnderecos.DataSource = aceite.Enderecos;
                    rptEnderecos.DataBind();

                    rptDomicilioBancario.DataSource = aceite.DomiciliosBancarios;
                    rptDomicilioBancario.DataBind();

                    grvServicosContratados.DataSource = aceite.ServicosContratados;
                    grvServicosContratados.DataBind();

                    ltrValorTaxaAdesao.Text = String.Format("{0:C2}", aceite.ValorTaxaAdesao);
                    // Obtem informação complementar de aceite obtidos através do HIS
                    using (var contextoZP = new ContextoWCF<ServicoTerminalContratadoClient>())
                    {
                        ltrValorTaxaAdesaoMensal.Text = String.Format("{0:C2}", contextoZP.Cliente.ObterServico(new ValoresCobrancaServicosRequest() { CodigoServico = 302 }).ValorServico);

                        ListaTerminalContratadoResponse terminais = contextoZP.Cliente.ConsultarLista(new TerminalContratadoRequest()
                        {
                            NumeroPDV = numeroPDV
                        });

                        // Preenche informações de terminais no lightbox de aceite
                        grvTerminaisContratados.DataSource = terminais.Itens;
                        grvTerminaisContratados.DataBind();

                        // Chamada ZPL84800 para obtenção e preenchimento de dados de Oferta Preço Único
                        DadosPrecoUnicoPvResponse dadosPrecoUnico = contextoZP.Cliente.ObterDadosPrecoUnicoPv(new DadosPrecoUnicoPvRequest()
                        {
                            NumeroPDV = numeroPDV
                        });

                        if (dadosPrecoUnico.CodigoOferta == 0)
                            ControleVisibilidadePaineisPrecoUnico(false, false);
                        else
                        {
                            if (String.Compare(dadosPrecoUnico.Features.FirstOrDefault().IndicadorProdutoFlex, "S", true) == 0)
                            {
                                if (dadosPrecoUnico.Terminais != null && dadosPrecoUnico.Terminais.Count > 0)
                                {
                                    ControleVisibilidadePaineisPrecoUnico(true, false);

                                    List<PrecoUnico> listaPrecoUnico = new List<PrecoUnico>();

                                    dadosPrecoUnico.Terminais.ForEach(t => listaPrecoUnico.Add(new PrecoUnico()
                                    {
                                        ValorFaturamentoContrato = dadosPrecoUnico.ValorFaturamentoContrato,
                                        ValorPrecoUnicoComFlex = dadosPrecoUnico.ValorPrecoUnicoComFlex,
                                        ValorPrecoUnicoSemFlex = dadosPrecoUnico.ValorPrecoUnicoSemFlex,
                                        QuantidadeEquipamento = t.QuantidadeEquipamento.ToString(),
                                        TipoEquipamento = t.TipoEquipamento
                                    }));

                                    grvCondicaoComercialTecnologiaFlex.DataSource = listaPrecoUnico;
                                    grvCondicaoComercialTecnologiaFlex.DataBind();

                                    this.PreencheCamposFlex(dadosPrecoUnico.Features.FirstOrDefault().PercentualTaxa1,
                                                            dadosPrecoUnico.Features.FirstOrDefault().PercentualTaxa1,
                                                            dadosPrecoUnico.Features.FirstOrDefault().PercentualTaxa2);
                                }
                            }
                            else
                            {
                                if (dadosPrecoUnico.Terminais != null && dadosPrecoUnico.Terminais.Count > 0)
                                {
                                    ControleVisibilidadePaineisPrecoUnico(false, true);

                                    List<PrecoUnico> listaPrecoUnico = new List<PrecoUnico>();

                                    dadosPrecoUnico.Terminais.ForEach(t => listaPrecoUnico.Add(new PrecoUnico()
                                    {
                                        ValorFaturamentoContrato = dadosPrecoUnico.ValorFaturamentoContrato,
                                        ValorPrecoUnicoComFlex = dadosPrecoUnico.ValorPrecoUnicoComFlex,
                                        ValorPrecoUnicoSemFlex = dadosPrecoUnico.ValorPrecoUnicoSemFlex,
                                        QuantidadeEquipamento = t.QuantidadeEquipamento.ToString(),
                                        TipoEquipamento = t.TipoEquipamento
                                    }));

                                    grvCondicaoComercialTecnologia.DataSource = listaPrecoUnico;
                                    grvCondicaoComercialTecnologia.DataBind();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Controle de visibilidade dos painéis de Condições Comerciais da Oferta
        /// </summary>
        /// <param name="flex">Indicador de controle sobre paineis Flex</param>
        /// <param name="nonFlex">Indicador de controle sobre painel não Flex</param>
        private void ControleVisibilidadePaineisPrecoUnico(Boolean flex, Boolean nonFlex)
        {
            phdCondicaoComercialFaturamentoFlex.Visible = flex;
            phdCondicaoComercialFlex.Visible = flex;
            phdCondicaoComercialFaturamento.Visible = nonFlex;
        }

        /// <summary>
        /// Preenche os campos de dados Flex
        /// </summary>
        /// <param name="vendaVista">Descrição de venda a vista</param>
        /// <param name="parcelaPrimeira">Descrição da primeira parcela</param>
        /// <param name="parcelaAdicional">Descrição das parcelas adicionais</param>
        private void PreencheCamposFlex(Decimal vendaVista, Decimal parcelaPrimeira, Decimal parcelaAdicional)
        {
            ltrVendaVista.Text = vendaVista > 0 ? (vendaVista / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : vendaVista.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrParcelaPrimeira.Text = parcelaPrimeira > 0 ? (parcelaPrimeira / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : parcelaPrimeira.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrParcelaAdicional.Text = parcelaAdicional > 0 ? (parcelaAdicional / 100).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR")) : parcelaAdicional.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
        }

        /// <summary>
        /// Preenche os campos de dados Flex sem valores
        /// </summary>
        private void PreencheCamposFlex()
        {
            ltrVendaVista.Text = default(Decimal).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrParcelaPrimeira.Text = default(Decimal).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
            ltrParcelaAdicional.Text = default(Decimal).ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
        }

        /// <summary>
        /// Preenchimento de informações de Tecnlogias.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void grvCondicaoComercialTecnologia_DataBound(object sender, EventArgs e)
        {
            GridView condicaoComercial = sender as GridView;

            for (int i = condicaoComercial.Rows.Count - 1; i > 0; i--)
            {
                GridViewRow row = condicaoComercial.Rows[i];
                GridViewRow previousRow = condicaoComercial.Rows[i - 1];
                for (int j = 0; j < row.Cells.Count; j++)
                    if (row.Cells[j].Text == previousRow.Cells[j].Text)
                        if (previousRow.Cells[j].RowSpan == 0)
                        {
                            if (row.Cells[j].RowSpan == 0)
                                previousRow.Cells[j].RowSpan += 2;
                            else
                                previousRow.Cells[j].RowSpan = row.Cells[j].RowSpan + 1;
                            row.Cells[j].Visible = false;
                        }
            }
        }

        /// <summary>
        /// Converte string para TitleCase
        /// </summary>
        /// <param name="texto">Texto a ser convertido</param>
        /// <returns></returns>
        private string ToTitleCase(string texto)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(texto.ToLower());
        }
    }
}
