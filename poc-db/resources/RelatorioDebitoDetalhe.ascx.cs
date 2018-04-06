/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.WAExtratoValoresPagosServico;
using System;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ValoresPagos
{
    public partial class RelatorioDebitoDetalhe : BaseUserControl, IRelatorioHandler
    {
        public string IdControl { get { return "RelatorioDebitoDetalhe"; } }
        public String NomeOperacao { get { return "Relatório - Valores Pagos - Débito - Detalhe"; } }
        protected Paginacao objPaginacao;
        protected Totalizadores rcTotalizadores;
        protected TableSize ddlRegistrosPorPagina;        
        private WAExtratoValoresPagosServico.DebitoDetalheTotalizador Totalizador;
        private WAExtratoValoresPagosServico.DebitoDetalhe[] Registros;

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

                    DateTime dataPagamento = QS["DataPagamento"].ToDate("ddMMyyyyHHmmssfff");
                    Int32 numeroPV = QS["NumeroPV"].ToInt32(0);

                    using (var contexto = new ContextoWCF<HISServicoWA_Extrato_ValoresPagosClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, pagina, tamanhoPagina, paginasVirtuais, registroInicial, qtdRegistrosVirtuais, GuidPesquisa = GuidPesquisa(), GuidTotalizador = GuidPesquisaTotalizador() });

                        StatusRetorno statusRelatorio, statusTotalizador;
                        WAExtratoValoresPagosServico.DebitoDetalhe[] registros;
                        WAExtratoValoresPagosServico.DebitoDetalheTotalizador totalizador;

                        contexto.Cliente.ConsultarRelatorioDebitoDetalhe(
                            GuidPesquisaTotalizador(),
                            GuidPesquisa(),
                            dataPagamento,
                            numeroPV,                            
                            registroInicial,
                            tamanhoPagina,
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
            if (Totalizador.Totais != null)
            {
                rcTotalizadores.ValorLiquido = Totalizador.Totais.TotalValorLiquido;
                rcTotalizadores.ValorBruto = Totalizador.Totais.TotalValorBrutoVenda;
            }

            if (Totalizador.Valores != null)
                rcTotalizadores.Bandeiras = Totalizador.Valores.Select(bandeira =>
                    new Totalizadores.Bandeira(bandeira.TipoBandeira, bandeira.ValorLiquido)).ToList();

            rcTotalizadores.Atualizar();
        }

        protected void rptDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var dataItem = (WAExtratoValoresPagosServico.DebitoDetalhe)e.Item.DataItem;

                Literal lblDataApresentacao = (Literal)e.Item.FindControl("lblDataApresentacao");
                PlaceHolder phResumoVendas = (PlaceHolder)e.Item.FindControl("phResumoVendas");
                Literal lblBandeira = (Literal)e.Item.FindControl("lblBandeira");
                Literal lblQtdVendas = (Literal)e.Item.FindControl("lblQtdVendas");
                Literal lblValorBruto = (Literal)e.Item.FindControl("lblValorBruto");
                Literal lblValorLiquido = (Literal)e.Item.FindControl("lblValorLiquido");

                lblDataApresentacao.Text = dataItem.DataVenda.ToString("dd/MM/yy");
                phResumoVendas.Controls.Add(base.ObterHyperLinkResumoVenda("D", dataItem.NumeroRV, dataItem.NumeroPV, dataItem.DataRV));
                lblQtdVendas.Text = dataItem.QuantidadeTransacao.ToString();
                lblBandeira.Text = dataItem.TipoBandeira;
                lblValorBruto.Text = dataItem.ValorVenda.ToString("N2");
                lblValorLiquido.Text = dataItem.ValorLiquido.ToString("N2");
            }
        }
    }
}
