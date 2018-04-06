/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.WAExtratoValoresConsolidadosVendas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ValoresConsolidados
{
    /// <summary>
    /// Turquia - Relatório Vendas Crédito - Detalhe.
    /// </summary>
    public partial class RelatorioVendasCreditoDetalhe : BaseUserControl, IRelatorioHandler
    {
        #region [Propriedades]

        /// <summary>CultureInfo pt-BR</summary>
        private readonly CultureInfo ptBr = CultureInfo.CreateSpecificCulture("pt-BR");

        /// <summary>
        /// Nome da operação.
        /// </summary>
        private String NomeOperacao { get { return "Relatório Vendas Crédito Detalhe"; } }

        /// <summary>
        /// User Control de paginação.
        /// </summary>
        protected Paginacao ObjPaginacao
        {
            get { return (Paginacao)objPaginacao; }
        }

        /// <summary>
        /// User Control Totalizadores.
        /// </summary>
        protected Totalizadores RcTotalizadores
        {
            get { return (Totalizadores)rcTotalizadores; }
        }

        /// <summary>
        /// User Control Tamanho da página.
        /// </summary>
        protected TableSize DdlRegistrosPorPagina
        {
            get { return (TableSize)ddlRegistrosPorPagina; }
        }

        /// <summary>
        /// Totalizador
        /// </summary>
        private TotalVendasCreditoPorDiaBandeira Totalizador
        {
            get
            {
                return ViewState.Count > 0 && ViewState["Totalizador"] != null ?
                    (TotalVendasCreditoPorDiaBandeira)ViewState["Totalizador"] : new TotalVendasCreditoPorDiaBandeira();
            }
            set { ViewState["Totalizador"] = value; }
        }

        /// <summary>
        /// Lista de registros de vendas.
        /// </summary>
        private List<ResumoVendasCreditoPorDia> Registros
        {
            get
            {
                return ViewState.Count > 0 && ViewState["Registros"] != null ?
                    (List<ResumoVendasCreditoPorDia>)ViewState["Registros"] : new List<ResumoVendasCreditoPorDia>();
            }
            set { ViewState["Registros"] = value; }
        }

        /// <summary>
        /// Identificador de pesquisa de PV,s e Data Venda.
        /// </summary>
        private Guid GuidPesquisaPvDataVenda
        {
            get
            {
                return ViewState.Count > 0 && ViewState["GuidPesquisaPvDataVenda"] != null ?
                    (Guid)ViewState["GuidPesquisaPvDataVenda"] : Guid.NewGuid();
            }
            set { ViewState["GuidPesquisaPvDataVenda"] = value; }
        }

        #endregion

        #region [Eventos]

        /// <summary>
        /// Page Load.
        /// </summary>
        /// <param name="sender">Objeto page.</param>
        /// <param name="e">Objeto com argumentos do evento</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            ObjPaginacao.RegistrosPorPagina = DdlRegistrosPorPagina.SelectedSize;
        }

        /// <summary>
        /// Evento de alteração da seleção da DropDown registros por página.
        /// </summary>
        /// <param name="sender">Objeto ddlRegistrosPorPagina</param>
        /// <param name="tamanhoPagina">Quantidade de registros por página.</param>
        protected void ddlRegistrosPorPagina_TamanhoPaginaChanged(Object sender, Int32 tamanhoPagina)
        {
            Consultar(ObterBuscarDados(), 1, tamanhoPagina, ObjPaginacao.PaginasVirtuais);
        }

        /// <summary>
        /// Evento Cache Todos os Registros do objeto objPaginacao.
        /// </summary>
        protected void objPaginacao_CacheTodosRegistros()
        {
            Consultar(ObterBuscarDados(), 1, 0, Int32.MaxValue);
        }

        /// <summary>
        /// Evento Paginação Change do objeto objPaginacao
        /// </summary>
        /// <param name="pagina">Número da página</param>
        /// <param name="e">Objeto com argumentos do evento.</param>
        protected void objPaginacao_onPaginacaoChanged(int pagina, EventArgs e)
        {
            Consultar(ObterBuscarDados(), pagina, DdlRegistrosPorPagina.SelectedSize, ObjPaginacao.PaginasVirtuais);
        }

        /// <summary>
        /// Evento Data Bound do repeater dos registros.
        /// </summary>
        /// <param name="sender">Objeto rptDados</param>
        /// <param name="e">Objeto com argumentos do evento.</param>
        protected void rptDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                HtmlTableRow trDetalhe = e.Item.FindControl("trDetalhe") as HtmlTableRow;
                ResumoVendasCreditoPorDia dataItem = e.Item.DataItem as ResumoVendasCreditoPorDia;

                Literal lblNumeroEstabelecimento = (Literal)trDetalhe.FindControl("lblNumeroEstabelecimento");
                Literal lblDataApresentacao = (Literal)trDetalhe.FindControl("lblDataApresentacao");
                Literal lblDataVencimento = (Literal)trDetalhe.FindControl("lblDataVencimento");
                Literal lblPrazoRecebimento = (Literal)trDetalhe.FindControl("lblPrazoRecebimento");
                PlaceHolder phdResumoVrendas = (PlaceHolder)trDetalhe.FindControl("phResumoVendas");
                Literal lblQtdVendas = (Literal)trDetalhe.FindControl("lblQtdVendas");
                Literal lblTipoVenda = (Literal)trDetalhe.FindControl("lblTipoVenda");
                Literal lblBandeira = (Literal)trDetalhe.FindControl("lblBandeira");
                Literal lblValorBruto = (Literal)trDetalhe.FindControl("lblValorBruto");
                Literal lblValorDescontado = (Literal)trDetalhe.FindControl("lblValorDescontado");
                Literal lblValorLiquido = (Literal)trDetalhe.FindControl("lblValorLiquido");

                lblNumeroEstabelecimento.Text = dataItem.NumeroEstabelecimento.ToString();
                lblDataApresentacao.Text = dataItem.DataVenda.Value.ToString("dd/MM/yyyy", ptBr);
                lblDataVencimento.Text = dataItem.DataPagamento.Value.ToString("dd/MM/yyyy", ptBr);
                lblPrazoRecebimento.Text = String.Concat(dataItem.PrazoRecebimento, " dias");
                phdResumoVrendas.Controls.Add(base.ObterHyperLinkResumoVenda("C", dataItem.NumeroResumoVenda, dataItem.NumeroEstabelecimento, dataItem.DataVenda.Value));
                lblQtdVendas.Text = dataItem.QuantidadeVendas.ToString();
                lblTipoVenda.Text = dataItem.TipoVenda;
                lblBandeira.Text = dataItem.DescricaoBandeira;
                lblValorBruto.Text = dataItem.TotalBruto.ToString("N2", ptBr);
                lblValorLiquido.Text = dataItem.TotalLiquido.ToString("N2", ptBr);
                
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                Literal lblTotalValorLiquido = e.Item.FindControl("lblTotalValorLiquido") as Literal;
                Literal lblTotalValorBruto = e.Item.FindControl("lblTotalValorBruto") as Literal;

                lblTotalValorLiquido.Text = Totalizador.TotalLiquido.ToString("N2", ptBr);
                lblTotalValorBruto.Text = Totalizador.TotalBruto.ToString("N2", ptBr);
            }
        }

        #endregion

        #region [Implementações]

        /// <summary>
        /// Efetua a consulta do relatório
        /// </summary>
        /// <param name="dados">Objeto com os parâmetros da consulta</param>
        public void Pesquisar(BuscarDados dados)
        {
            Consultar(dados, 1, DdlRegistrosPorPagina.SelectedSize, ObjPaginacao.PaginasVirtuais);
        }

        /// <summary>
        /// Geração de conteúdo HTML para exportação do relatório
        /// </summary>
        /// <param name="dados">Parâmetros da consulta</param>
        /// <param name="quantidadeRegistros">Quantidade de registros</param>
        /// <param name="incluirTotalizadores">Incluir totalizadores na exportação?</param>
        /// <returns>HTML do relatório</returns>
        public string ObterTabelaExcel(BuscarDados dados, int quantidadeRegistros, bool incluirTotalizadores)
        {
            ConsultarCompleto(dados);

            if (incluirTotalizadores)
                return base.RenderizarControles(true, RcTotalizadores, rptDados);
            else
                return base.RenderizarControles(true, rptDados);
        }

        /// <summary>
        /// ID do Controle
        /// </summary>
        public string IdControl
        {
            get { return "RelatorioVendasCredito"; }
        }

        #endregion

        #region [Métodos]

        /// <summary>
        /// Realiza a consulta do relatório.
        /// </summary>
        /// <param name="dadosBusca">Objeto com parâmetros para pesquisa.</param>
        /// <param name="pagina">Númera da página que se deseja consultar</param>
        /// <param name="tamanhoPagina">Quantidade de registros por página.</param>
        /// <param name="paginasVirtuais">Quantidade de páginas virtuais.</param>
        private void Consultar(BuscarDados dadosBusca, Int32 pagina, Int32 tamanhoPagina, Int32 paginasVirtuais)
        {
            using (Logger log = Logger.IniciarLog(NomeOperacao))
            {
                try
                {
                    ObjPaginacao.PaginaAtual = pagina;
                    GravarBuscarDados(dadosBusca);

                    Int32 registroInicial = (pagina - 1) * tamanhoPagina;
                    Int32 qtdRegistrosVirtuais = (paginasVirtuais == Int32.MaxValue) ? Int32.MaxValue : paginasVirtuais * tamanhoPagina;

                    String tipoConsulta = QS["TipoConsulta"];

                    using (var contexto = new ContextoWCF<HisServicoWaExtratoValoresConsolidadosVendasClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, pagina, tamanhoPagina, paginasVirtuais, registroInicial, qtdRegistrosVirtuais, GuidPesquisa = GuidPesquisa(), GuidTotalizador = GuidPesquisaTotalizador() });

                        var statusRelatorio = default(StatusRetorno);
                        var statusTotalizador =  default(StatusRetorno);
                        var registros = default(List<ResumoVendasCreditoPorDia>);
                        var totalizador= default(TotalVendasCreditoPorDiaBandeira);

                        if (tipoConsulta == "detalhe")
                        {
                            DateTime dataVenda = QS["DataVenda"].ToDate();
                            Int32 numeroEstabelecimento = QS["NumeroEstabelecimanto"].ToInt32();
                            
                            contexto.Cliente.ConsultarRelatorioVendasCreditoPorDia(
                                GuidPesquisaTotalizador(),
                                GuidPesquisa(),
                                dataVenda,                                
                                numeroEstabelecimento,
                                registroInicial,
                                tamanhoPagina,
                                ref qtdRegistrosVirtuais,
                                out totalizador,
                                out registros,
                                out statusTotalizador,
                                out statusRelatorio);
                        }
                        else if (tipoConsulta == "verTodos")
                        {
                            DateTime dataInicio = QS["DataInicial"].ToDate();
                            DateTime dataFim = QS["DataFinal"].ToDate();
                            List<Int32> estabelecimentos = QS["Estabelecimentos"].Split(';').Select(estabecimento => estabecimento.ToInt32()).ToList();
                 
                            contexto.Cliente.ConsultarRelatorioVendasCreditoPorDiaTodos(
                                GuidPesquisaTotalizador(),
                                GuidPesquisa(),
                                GuidPesquisaPvDataVenda,
                                dataInicio,
                                dataFim,
                                estabelecimentos,
                                registroInicial,
                                tamanhoPagina,
                                ref qtdRegistrosVirtuais,
                                out totalizador,
                                out registros,
                                out statusTotalizador,
                                out statusRelatorio);
                        }

                        log.GravarLog(EventoLog.RetornoServico, new { totalizador, registros, statusTotalizador, statusRelatorio, qtdRegistrosVirtuais });

                        if (statusRelatorio.CodigoRetorno != (Int16)Constantes.CodigoRetorno.Ok &&
                            statusRelatorio.CodigoRetorno != (Int16)Constantes.CodigoRetorno.NenhumArgumentoEncontrado)
                        {
                            base.ExibirPainelExcecao(statusRelatorio.Fonte, statusRelatorio.CodigoRetorno);
                            return;
                        }

                        if (statusTotalizador.CodigoRetorno != (Int16)Constantes.CodigoRetorno.Ok &&
                            statusRelatorio.CodigoRetorno != (Int16)Constantes.CodigoRetorno.NenhumArgumentoEncontrado)
                        {
                            base.ExibirPainelExcecao(statusTotalizador.Fonte, statusTotalizador.CodigoRetorno);
                            return;
                        }

                        ObjPaginacao.QuantidadeTotalRegistros = qtdRegistrosVirtuais;

                        Registros = registros;
                        Totalizador = totalizador;

                        CarregarDadosRelatorio();
                        CarregarTotalizadores();
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
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
        /// Consulta o relatório completo para download
        /// </summary>
        /// <param name="dadosBusca">Dados da busca</param>
        private void ConsultarCompleto(BuscarDados dadosBusca)
        {
            using (Logger log = Logger.IniciarLog(NomeOperacao))
            {
                try
                {
                    Int32 registroInicial = 0;
                    Int32 tamanhoPagina = 2000; //Tamanho que não gera time out do serviço.

                    String tipoConsulta = QS["TipoConsulta"];

                    log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, registroInicial, GuidPesquisa = GuidPesquisa(), GuidTotalizador = GuidPesquisaTotalizador() });

                    var statusRelatorio = default(StatusRetorno);
                    var statusTotalizador = default(StatusRetorno);
                    var registros = new List<ResumoVendasCreditoPorDia>();
                    var registrosPagina = default(List<ResumoVendasCreditoPorDia>);
                    var totalizador = default(TotalVendasCreditoPorDiaBandeira);

                    if (tipoConsulta.CompareTo("detalhe") == 0)
                    {
                        DateTime dataVenda = QS["DataVenda"].ToDate();
                        Int32 numeroEstabelecimento = QS["NumeroEstabelecimanto"].ToInt32();

                        using (var contexto = new ContextoWCF<HisServicoWaExtratoValoresConsolidadosVendasClient>())
                        {
                            //Consulta o totalizador do relatório
                            totalizador = contexto.Cliente.ConsultarTotalVendasCreditoPorDiaBandeira(
                                out statusTotalizador,
                                GuidPesquisaTotalizador(),
                                dataVenda,
                                numeroEstabelecimento);
                        }

                        log.GravarMensagem("Consultou totalizadores", new { totalizador });

                        //Em caso de erro no totalizador, cancela consulta de registros
                        if (statusTotalizador.CodigoRetorno != 0)
                            return;

                        //Consulta completa, paginada no serviço do relatório
                        do
                        {
                            Int32 qtdTotalRegistros = tamanhoPagina + registroInicial;

                            log.GravarMensagem("Consultando registro inicial: " + registroInicial + " - " + qtdTotalRegistros);

                            using (var contexto = new ContextoWCF<HisServicoWaExtratoValoresConsolidadosVendasClient>())
                            {
                                registrosPagina = contexto.Cliente.ConsultarResumoVendasCreditoPorDia(
                                    GuidPesquisa(),
                                    registroInicial,
                                    tamanhoPagina,
                                    ref qtdTotalRegistros,
                                    out statusRelatorio,
                                    dataVenda,
                                    numeroEstabelecimento);
                            }

                            log.GravarMensagem("Consultou registro inicial: " + registroInicial);

                            //Em caso de erro na consulta da página, cancela consulta de registros
                            if (statusRelatorio == null || statusRelatorio.CodigoRetorno != 0)
                                return;
                            else if (registrosPagina != null)
                            {
                                registros.AddRange(registrosPagina);
                                registroInicial += registrosPagina.Count;
                            }
                        } while (registrosPagina != null && registrosPagina.Count > 0);
                    }
                    else if (tipoConsulta.CompareTo("verTodos") == 0)
                    {
                        DateTime dataInicio = QS["DataInicial"].ToDate();
                        DateTime dataFim = QS["DataFinal"].ToDate();
                        List<Int32> estabelecimentos = QS["Estabelecimentos"].Split(';').Select(estabecimento => estabecimento.ToInt32()).ToList();

                        using (var contexto = new ContextoWCF<HisServicoWaExtratoValoresConsolidadosVendasClient>())
                        {
                            //Consulta o totalizador do relatório
                            totalizador = contexto.Cliente.ConsultarTotalVendasCreditoPorDiaBandeiraTodos(
                               out statusTotalizador,
                               GuidPesquisaTotalizador(),
                               GuidPesquisaPvDataVenda,
                               dataInicio,
                               dataFim,
                               estabelecimentos);
                        }

                        log.GravarMensagem("Consultou totalizadores", new { totalizador });

                        //Em caso de erro no totalizador, cancela consulta de registros
                        if (statusTotalizador.CodigoRetorno != 0)
                            return;

                        //Consulta completa, paginada no serviço do relatório
                        do
                        {
                            Int32 qtdTotalRegistros = tamanhoPagina + registroInicial;

                            log.GravarMensagem("Consultando registro inicial: " + registroInicial + " - " + qtdTotalRegistros);

                            using (var contexto = new ContextoWCF<HisServicoWaExtratoValoresConsolidadosVendasClient>())
                            {
                                registrosPagina = contexto.Cliente.ConsultarResumoVendasCreditoPorDiaTodos(
                                    GuidPesquisa(),
                                    GuidPesquisaPvDataVenda,
                                    registroInicial,
                                    tamanhoPagina,
                                    ref qtdTotalRegistros,
                                    out statusRelatorio,
                                    dataInicio,
                                    dataFim,
                                    estabelecimentos);
                            }

                            log.GravarMensagem("Consultou registro inicial: " + registroInicial);

                            //Em caso de erro na consulta da página, cancela consulta de registros
                            if (statusRelatorio == null || statusRelatorio.CodigoRetorno != 0)
                                return;
                            else if (registrosPagina != null)
                            {
                                registros.AddRange(registrosPagina);
                                registroInicial += registrosPagina.Count;
                            }
                        } while (registrosPagina != null && registrosPagina.Count > 0);
                    }

                    Registros = registros;
                    Totalizador = totalizador;

                    CarregarDadosRelatorio();
                    CarregarTotalizadores();
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
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
        /// Carregar dados do relatório no repeater.
        /// </summary>
        private void CarregarDadosRelatorio()
        {
            rptDados.DataSource = Registros;
            rptDados.DataBind();

            //Verifica os controles que devem estar visíveis
            base.VerificaControlesVisiveis(Registros.Count(), null, null);
        }

        /// <summary>
        /// Carrega informações de totalizadores no User Control.
        /// </summary>
        private void CarregarTotalizadores()
        {
            if (Totalizador != null)
            {
                RcTotalizadores.ValorLiquido = Totalizador.TotalLiquido;
                RcTotalizadores.ValorBruto = Totalizador.TotalBruto;
            }

            if (Totalizador.ListaTotalVendasCreditoPorBandeira != null)
                RcTotalizadores.Bandeiras = Totalizador.ListaTotalVendasCreditoPorBandeira.Select(bandeira =>
                    new Totalizadores.Bandeira(bandeira.DescricaoBandeira, bandeira.TotalLiquido)).ToList();

            RcTotalizadores.Atualizar();
        }

        #endregion
    }
}
