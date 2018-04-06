/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.WAExtratoVendasServico;
using System;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.Vendas
{
    public partial class RelatorioConstrucard : BaseUserControl, IRelatorioHandler
    {
        public string IdControl { get { return "RelatorioConstrucard"; } }
        public String NomeOperacao { get { return "Relatório - Vendas - Construcard"; } }
        protected Paginacao objPaginacao;
        protected Totalizadores rcTotalizadores;
        protected TableSize ddlRegistrosPorPagina;        
        private WAExtratoVendasServico.ConstrucardTotalizador Totalizador;
        private WAExtratoVendasServico.Construcard[] Registros;

        protected void Page_Load(object sender, EventArgs e)
        {            
            objPaginacao.RegistrosPorPagina = ddlRegistrosPorPagina.SelectedSize;            
        }

        protected void objPaginacao_CacheTodosRegistros()
        {
            Consultar(ObterBuscarDados(), 1, 0, Int32.MaxValue);
        }

        protected void ddlRegistrosPorPagina_TamanhoPaginaChanged(Object sender, Int32 tamanhoPagina)        
        {
            Consultar(ObterBuscarDados(), 1, tamanhoPagina, objPaginacao.PaginasVirtuais);
        }

        protected void objPaginacao_onPaginacaoChanged(int pagina, EventArgs e)
        {
            Consultar(ObterBuscarDados(), pagina, ddlRegistrosPorPagina.SelectedSize, objPaginacao.PaginasVirtuais);
        }

        public void Pesquisar(BuscarDados dados)
        {
            Consultar(dados, 1, ddlRegistrosPorPagina.SelectedSize, objPaginacao.PaginasVirtuais);
        }

        public string ObterTabelaExcel(BuscarDados dados, Int32 quantidadeRegistros, Boolean exibirTotalizadores)
        {
            Consultar(dados, 1, quantidadeRegistros, Int32.MaxValue);

            if(exibirTotalizadores)
                return base.RenderizarControles(true, rcTotalizadores, rptDados);
            else
                return base.RenderizarControles(true, rptDados);
        }

        private void Consultar(BuscarDados dadosBusca, Int32 pagina, Int32 tamanhoPagina, Int32 paginasVirtuais)
        {
            using (Logger Log = Logger.IniciarLog(NomeOperacao))
            {
                try
                {
                    objPaginacao.PaginaAtual = pagina;
                    GravarBuscarDados(dadosBusca);

                    Int32 registroInicial = (pagina - 1) * tamanhoPagina;
                    Int32 qtdRegistrosVirtuais = (paginasVirtuais == Int32.MaxValue) ? Int32.MaxValue : paginasVirtuais * tamanhoPagina;

                    using (var contexto = new ContextoWCF<HISServicoWA_Extrato_VendasClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, pagina, tamanhoPagina, paginasVirtuais, registroInicial, qtdRegistrosVirtuais, GuidPesquisa = GuidPesquisa(), GuidTotalizador = GuidPesquisaTotalizador() });

                        StatusRetorno statusRelatorio, statusTotalizador;
                        WAExtratoVendasServico.Construcard[] registros;
                        WAExtratoVendasServico.ConstrucardTotalizador totalizador;

                        contexto.Cliente.ConsultarRelatorioConstrucard(
                            GuidPesquisaTotalizador(),
                            GuidPesquisa(),                            
                            dadosBusca.CodigoBandeira,
                            dadosBusca.DataInicial,
                            dadosBusca.DataFinal,
                            dadosBusca.Estabelecimentos,
                            registroInicial,
                            tamanhoPagina,
                            ConstrucardTipoRegistro.Todos,
                            ref qtdRegistrosVirtuais,
                            out totalizador,
                            out registros,
                            out statusTotalizador,
                            out statusRelatorio);

                        Log.GravarLog(EventoLog.RetornoServico, new { totalizador, registros, statusTotalizador, statusRelatorio, qtdRegistrosVirtuais });

                        if (statusRelatorio.CodigoRetorno != 0)
                        {
                            base.ExibirPainelExcecao(statusRelatorio.Fonte, statusRelatorio.CodigoRetorno);
                            return;
                        }

                        if (statusTotalizador.CodigoRetorno != 0)
                        {
                            base.ExibirPainelExcecao(statusTotalizador.Fonte, statusTotalizador.CodigoRetorno);
                            return;
                        }

                        objPaginacao.QuantidadeTotalRegistros = qtdRegistrosVirtuais;

                        Registros = registros;
                        Totalizador = totalizador;

                        CarregarDadosRelatorio();
                        CarregarTotalizadores();
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        private void CarregarDadosRelatorio()
        {
            rptDados.DataSource = Registros;
            rptDados.DataBind();

            //Verifica os controles que devem estar visíveis
            base.VerificaControlesVisiveis(Registros.Count(), null, null);
        }

        private void CarregarTotalizadores()
        {
            rcTotalizadores.ValorLiquido = Totalizador.ValorLiquido;
            rcTotalizadores.ValorBruto = Totalizador.ValorBruto;
            rcTotalizadores.ExibirTotaisPorBandeiras = false;
            
            rcTotalizadores.Atualizar();
        }

        protected void rptDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                WAExtratoVendasServico.Construcard item = e.Item.DataItem as WAExtratoVendasServico.Construcard;

                HtmlTableRow trDetalhe = e.Item.FindControl("trDetalhe") as HtmlTableRow;
                HtmlTableRow trAjusteComValor = e.Item.FindControl("trAjusteComValor") as HtmlTableRow;
                HtmlTableRow trAjusteSemValor = e.Item.FindControl("trAjusteSemValor") as HtmlTableRow;

                trDetalhe.Visible = item is WAExtratoVendasServico.ConstrucardDT;
                trAjusteComValor.Visible = item is WAExtratoVendasServico.ConstrucardA1;
                trAjusteSemValor.Visible = item is WAExtratoVendasServico.ConstrucardA2;

                if (item is WAExtratoVendasServico.ConstrucardDT)
                {
                    WAExtratoVendasServico.ConstrucardDT dataItem = item as WAExtratoVendasServico.ConstrucardDT;

                    Literal lblNumeroPV = trDetalhe.FindControl("lblNumeroPV") as Literal;
                    Literal lblDataVenda = trDetalhe.FindControl("lblDataVenda") as Literal;
                    Literal lblDataVencimento = trDetalhe.FindControl("lblDataVencimento") as Literal;
                    PlaceHolder phResumoVenda = trDetalhe.FindControl("phResumoVenda") as PlaceHolder;
                    Literal lblQtdVendas = trDetalhe.FindControl("lblQtdVendas") as Literal;
                    Literal lblTipoVenda = trDetalhe.FindControl("lblTipoVenda") as Literal;
                    Literal lblValorBruto = trDetalhe.FindControl("lblValorBruto") as Literal;
                    Literal lblValorDescontado = trDetalhe.FindControl("lblValorDescontado") as Literal;
                    Literal lblValorLiquido = trDetalhe.FindControl("lblValorLiquido") as Literal;
                    Literal lblBanco = trDetalhe.FindControl("lblBanco") as Literal;
                    Literal lblAgencia = trDetalhe.FindControl("lblAgencia") as Literal;
                    Literal lblConta = trDetalhe.FindControl("lblConta") as Literal;

                    lblNumeroPV.Text = dataItem.NumeroPV.ToString();
                    lblDataVenda.Text = dataItem.DataVenda.ToString("dd/MM/yy");
                    lblDataVencimento.Text = dataItem.DataVencimento.ToString("dd/MM/yy");
                    phResumoVenda.Controls.Add(base.ObterHyperLinkResumoVenda("CDC", dataItem.NumeroResumo, dataItem.NumeroPV, dataItem.DataVenda));
                    lblQtdVendas.Text = dataItem.QuantidadeTransacoesRV.ToString();
                    lblTipoVenda.Text = dataItem.DescricaoResumo;
                    lblValorBruto.Text = dataItem.ValorApresentado.ToString("N2");
                    lblValorDescontado.Text = dataItem.ValorDesconto.ToString("N2");
                    lblValorLiquido.Text = dataItem.ValorLiquido.ToString("N2");
                    lblBanco.Text = base.ObterNomeBanco(dataItem.BancoCredito);
                    lblAgencia.Text = dataItem.AgenciaCredito.ToString();
                    lblConta.Text = dataItem.ContaCredito;
                }
                else if (item is WAExtratoVendasServico.ConstrucardA1)
                {
                    var dataItem = item as WAExtratoVendasServico.ConstrucardA1;

                    Literal lblDescricaoAjuste = trAjusteComValor.FindControl("lblDescricaoAjuste") as Literal;
                    Literal lblValorBrutoAjuste = trAjusteComValor.FindControl("lblValorBrutoAjuste") as Literal;

                    lblDescricaoAjuste.Text = dataItem.Descricao;
                    lblValorBrutoAjuste.Text = dataItem.ValorApresentado.ToString("N2");
                }
                else if (item is WAExtratoVendasServico.ConstrucardA2)
                {
                    var dataItem = item as WAExtratoVendasServico.ConstrucardA2;

                    Literal lblDescricaoAjusteSemValor = trAjusteComValor.FindControl("lblDescricaoAjusteSemValor") as Literal;

                    lblDescricaoAjusteSemValor.Text = dataItem.Descricao;
                }
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                Literal lblTotalValorBruto = e.Item.FindControl("lblTotalValorBruto") as Literal;
                Literal lblTotalValorDescontado = e.Item.FindControl("lblTotalValorDescontado") as Literal;
                Literal lblTotalValorLiquido = e.Item.FindControl("lblTotalValorLiquido") as Literal;

                lblTotalValorBruto.Text = Totalizador.ValorBruto.ToString("N2");
                lblTotalValorLiquido.Text = Totalizador.ValorLiquido.ToString("N2");
                lblTotalValorDescontado.Text = Totalizador.ValorDescontado.ToString("N2");
            }
        }
    }
}
