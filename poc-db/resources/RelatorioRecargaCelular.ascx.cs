/*
© Copyright 2015 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.WAExtratoVendasServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.Vendas
{
    /// <summary>
    /// Relatório de Vendas - Recarga de Celular (PV Físico e PV Lógico)
    /// </summary>
    /// <remarks>
    /// Relatório de Vendas - Recarga de Celular - PV Físico - Totalizador	BKWA2610	WAC261	WAAF
    /// Relatório de Vendas - Recarga de Celular - PV Físico - Registros	BKWA2620	WAC262	WAAG
    /// Relatório de Vendas - Recarga de Celular - PV Lógico - Totalizador	BKWA2630	WAC263	WAAH
    /// Relatório de Vendas - Recarga de Celular - PV Lógico - Registros	BKWA2640	WAC264	WAAI
    /// </remarks>
    public partial class RelatorioRecargaCelular : BaseUserControl, IRelatorioHandler, IRelatorioCSV
    {
        #region [ Propriedades ]

        /// <summary>
        /// Identificador do controle
        /// </summary>
        public string IdControl { get { return "RelatorioVendasRecargaCelular"; } }

        /// <summary>
        /// Nome da operação (para geração de log)
        /// </summary>
        public String NomeOperacao { get { return "Relatório de Vendas - Recarga de Celular"; } }

        /// <summary>
        /// Controle de paginação
        /// </summary>
        public Paginacao ObjPaginacao { get { return (Paginacao)objPaginacao; } }

        /// <summary>
        /// Controle de totalizadores
        /// </summary>
        public Totalizadores RcTotalizadores { get { return (Totalizadores)rcTotalizadores; } }

        /// <summary>
        /// Controle de tamanho da página
        /// </summary>
        public TableSize DdlRegistrosPorPagina { get { return (TableSize)ddlRegistrosPorPagina; } }

        #endregion

        #region [ Variáveis ]

        /// <summary>
        /// Armazena, durante o carregamento da página, os totalizadores do relatório
        /// </summary>
        private WAExtratoVendasServico.RecargaCelularTotalizador totalizador;

        /// <summary>
        /// Armazena, durante o carregamento da página, os registros do relatório
        /// </summary>
        private WAExtratoVendasServico.RecargaCelular[] registros;

        /// <summary>
        /// Armazena, durante o carregamento da página, o status da consulta dos registros do relatório
        /// </summary>
        private StatusRetorno statusRelatorio;

        /// <summary>
        /// Armazena, durante o carregamento da página, o status da consulta dos totalizadores do relatório
        /// </summary>
        private StatusRetorno statusTotalizador;

        #endregion

        /// <summary>
        /// Carregamento da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Atrela a quantidade de registros selecionados, com o objeto de paginação
            ObjPaginacao.RegistrosPorPagina = DdlRegistrosPorPagina.SelectedSize;
        }

        /// <summary>
        /// Evento da paginação, para forçar a consulta completa dos registros do relatório
        /// </summary>
        protected void objPaginacao_CacheTodosRegistros()
        {
            Consultar(ObterBuscarDados(), 1, 0, Int32.MaxValue);
        }

        /// <summary>
        /// Evento de alteração de tamanho da página.
        /// </summary>
        protected void ddlRegistrosPorPagina_TamanhoPaginaChanged(Object sender, Int32 tamanhoPagina)
        {
            Consultar(ObterBuscarDados(), 1, tamanhoPagina, ObjPaginacao.PaginasVirtuais);
        }

        /// <summary>
        /// Evento da paginação: alteração da página sendo visualizada
        /// </summary>
        /// <param name="pagina">Número da página</param>
        protected void objPaginacao_onPaginacaoChanged(int pagina, EventArgs e)
        {
            Consultar(ObterBuscarDados(), pagina, DdlRegistrosPorPagina.SelectedSize, ObjPaginacao.PaginasVirtuais);
        }

        /// <summary>
        /// Evento de busca do relatório (clique no botão Buscar do filtro da página)
        /// </summary>
        /// <param name="dados">Dados preenchidos no filtro</param>
        public void Pesquisar(BuscarDados dados)
        {
            Consultar(dados, 1, DdlRegistrosPorPagina.SelectedSize, ObjPaginacao.PaginasVirtuais);
        }

        /// <summary>
        /// Geração de HTML para exportação Excel
        /// </summary>
        /// <param name="dados">Dados preenchidos no filtro</param>
        /// <param name="quantidadeRegistros">Quantidade de registros que serão gerados no Excel</param>
        /// <param name="exibirTotalizadores">Flag indicando se deve renderizar, no Excel, os totalizadores</param>
        /// <returns>HTML para exportação Excel</returns>
        public string ObterTabelaExcel(BuscarDados dados, Int32 quantidadeRegistros, Boolean exibirTotalizadores)
        {
            //consulta o relatório completo
            ConsultarCompleto(dados);

            //renderização dos controles para geração HTML
            if (exibirTotalizadores)
                return base.RenderizarControles(true, rcTotalizadores, repDados);
            else
                return base.RenderizarControles(true, repDados);
        }

        /// <summary>
        /// Consulta o relatório completo para download
        /// </summary>
        /// <param name="dadosBusca">Dados da busca informados no filtro</param>
        private void ConsultarCompleto(BuscarDados dadosBusca)
        {
            using (Logger log = Logger.IniciarLog(NomeOperacao))
            {
                try
                {
                    Int32 registroInicial = 0; //registro inicial
                    Int32 tamanhoPagina = 1000; //tamanho da página
                    var todosRegistros = new List<WAExtratoVendasServico.RecargaCelular>();
                    WAExtratoVendasServico.RecargaCelular[] registrosPagina;

                    log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca,
                        GuidPesquisa = GuidPesquisa(), GuidTotalizador = GuidPesquisaTotalizador() });

                    //Consulta o totalizador do relatório, chamando o serviço WCF do relatório
                    using (var contexto = new ContextoWCF<HISServicoWA_Extrato_VendasClient>())
                    {
                        //Se o PV for PV Físico
                        if (SessaoAtual.PVFisico)
                        {
                            this.totalizador = contexto.Cliente.ConsultarRecargaCelularPvFisicoTotalizadores(
                                out statusTotalizador,
                                GuidPesquisaTotalizador(),
                                dadosBusca.DataInicial,
                                dadosBusca.DataFinal,
                                dadosBusca.Estabelecimentos);
                        }
                        //Se o PV for PV Lógico
                        else if (SessaoAtual.PVLogico)
                        {
                            this.totalizador = contexto.Cliente.ConsultarRecargaCelularPvLogicoTotalizadores(
                                out statusTotalizador,
                                GuidPesquisaTotalizador(),
                                dadosBusca.DataInicial,
                                dadosBusca.DataFinal,
                                dadosBusca.Estabelecimentos);
                        }
                        else
                            return;
                    }

                    //Em caso de erro no totalizador, cancela consulta de registros
                    if (statusTotalizador.CodigoRetorno != 0)
                        return;

                    //Consulta completa, paginada no serviço do relatório
                    do
                    {
                        Int32 qtdTotalRegistros = 2000 + registroInicial;

                        //Consulta os registros do relatório
                        using (var contexto = new ContextoWCF<HISServicoWA_Extrato_VendasClient>())
                        {
                            //Se o PV for PV Físico
                            if (SessaoAtual.PVFisico)
                            {
                                registrosPagina = contexto.Cliente.ConsultarRecargaCelularPvFisico(
                                    GuidPesquisa(),
                                    registroInicial,
                                    tamanhoPagina,
                                    ref qtdTotalRegistros,
                                    out statusRelatorio,
                                    dadosBusca.DataInicial,
                                    dadosBusca.DataFinal,
                                    dadosBusca.Estabelecimentos);
                            }
                            //Se o PV for PV Lógico
                            else if (SessaoAtual.PVLogico)
                            {
                                registrosPagina = contexto.Cliente.ConsultarRecargaCelularPvLogico(
                                    GuidPesquisa(),
                                    registroInicial,
                                    tamanhoPagina,
                                    ref qtdTotalRegistros,
                                    out statusRelatorio,
                                    dadosBusca.DataInicial,
                                    dadosBusca.DataFinal,
                                    dadosBusca.Estabelecimentos);
                            }
                            else
                                return;
                        }

                        //Em caso de erro na consulta da página corrente do relatório, cancela toda a consulta de registros
                        if (this.statusRelatorio == null || this.statusRelatorio.CodigoRetorno != 0)
                            return;
                        else if (registrosPagina != null)
                        {
                            todosRegistros.AddRange(registrosPagina);
                            registroInicial += registrosPagina.Length;
                        }
                    } while (registrosPagina != null && registrosPagina.Length > 0);

                    this.registros = todosRegistros.ToArray();

                    //bind dos dados nos controles
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
        /// Consulta o relatório
        /// </summary>
        /// <param name="dadosBusca">Dados da busca informados no filtro</param>
        /// <param name="pagina">Número da página</param>
        /// <param name="tamanhoPagina">Tamanho da página</param>
        /// <param name="paginasVirtuais">Quantidade de páginas virtuais
        /// (corresponde à quantidade mínima de registros cuja existência deve ser verifiada no mainframe).
        /// Utilizada para renderização da quantidade de páginas</param>
        private void Consultar(BuscarDados dadosBusca, Int32 pagina, Int32 tamanhoPagina, Int32 paginasVirtuais)
        {
            using (Logger log = Logger.IniciarLog(NomeOperacao))
            {
                try
                {
                    ObjPaginacao.PaginaAtual = pagina;
                    GravarBuscarDados(dadosBusca);

                    //cálculo da quantidade de registros virtuais
                    Int32 registroInicial = (pagina - 1) * tamanhoPagina;
                    Int32 qtdRegistrosVirtuais = (paginasVirtuais == Int32.MaxValue) ? 
                        Int32.MaxValue : paginasVirtuais * tamanhoPagina;

                    log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, pagina, tamanhoPagina, 
                        paginasVirtuais, registroInicial, qtdRegistrosVirtuais, GuidPesquisa = GuidPesquisa(), 
                        GuidTotalizador = GuidPesquisaTotalizador() });

                    //Realiza chamada no serviço WCF do relatório
                    using (var contexto = new ContextoWCF<HISServicoWA_Extrato_VendasClient>())
                    {
                        //Se for PV Físico
                        if (SessaoAtual.PVFisico)
                        {
                            contexto.Cliente.ConsultarRelatorioRecargaCelularPvFisico(
                                GuidPesquisaTotalizador(),
                                GuidPesquisa(),
                                dadosBusca.DataInicial,
                                dadosBusca.DataFinal,
                                dadosBusca.Estabelecimentos,
                                registroInicial,
                                tamanhoPagina,
                                ref qtdRegistrosVirtuais,
                                out this.totalizador,
                                out this.registros,
                                out this.statusTotalizador,
                                out this.statusRelatorio);
                        }
                        //Se for PV Lógico
                        else if(SessaoAtual.PVLogico)
                        {
                            contexto.Cliente.ConsultarRelatorioRecargaCelularPvLogico(
                                GuidPesquisaTotalizador(),
                                GuidPesquisa(),
                                dadosBusca.DataInicial,
                                dadosBusca.DataFinal,
                                dadosBusca.Estabelecimentos,
                                registroInicial,
                                tamanhoPagina,
                                ref qtdRegistrosVirtuais,
                                out this.totalizador,
                                out this.registros,
                                out this.statusTotalizador,
                                out this.statusRelatorio);
                        }
                    }

                    log.GravarLog(EventoLog.RetornoServico, new { totalizador, registros, 
                        statusTotalizador, statusRelatorio, qtdRegistrosVirtuais });

                    //Em caso de erro na consulta dos registros do relatório
                    if (this.statusRelatorio.CodigoRetorno != 0)
                    {
                        base.ExibirPainelExcecao(statusRelatorio.Fonte, statusRelatorio.CodigoRetorno);
                        return;
                    }

                    //Em caso de erro na consulta dos totalizadores do relatório
                    if (statusTotalizador.CodigoRetorno != 0)
                    {
                        base.ExibirPainelExcecao(statusTotalizador.Fonte, statusTotalizador.CodigoRetorno);
                        return;
                    }

                    //Carrega os dados retornados
                    ObjPaginacao.QuantidadeTotalRegistros = qtdRegistrosVirtuais;
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
        /// Realiza o bind dos registros do relatório nos controles asp.net
        /// </summary>
        private void CarregarDadosRelatorio()
        {
            repDados.DataSource = this.registros;
            repDados.DataBind();

            //Verifica os controles que devem estar visíveis
            base.VerificaControlesVisiveis(registros.Count(), null, null);
        }

        /// <summary>
        /// Realiza o bind dos totalizadores no controle de totalizadores
        /// </summary>
        private void CarregarTotalizadores()
        {
            RcTotalizadores.ValorTotalSuperior = totalizador.TotalValorBrutoRecarga;

            //Para PV Lógico, não exibe valor da comissão
            if (SessaoAtual.PVLogico)
                RcTotalizadores.ValorTotalInferior = null;
            else if(SessaoAtual.PVFisico)
                RcTotalizadores.ValorTotalInferior = totalizador.TotalValorLiquidoComissao;

            RcTotalizadores.Atualizar();
        }

        /// <summary>
        /// Bind do repeater de registros.
        /// Utilizado tanto para PV Físico quanto PV Lógico
        /// </summary>
        protected void repDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                PlaceHolder phdHeaderPvLogico = e.Item.FindControl("phdHeaderPvLogico") as PlaceHolder;
                PlaceHolder phdHeaderPvFisico = e.Item.FindControl("phdHeaderPvFisico") as PlaceHolder;

                phdHeaderPvLogico.Visible = this.SessaoAtual.PVLogico;
                phdHeaderPvFisico.Visible = this.SessaoAtual.PVFisico;
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = e.Item.DataItem as WAExtratoVendasServico.RecargaCelular;

                if (item is RecargaCelularPvFisico)
                {
                    var dataItem = item as RecargaCelularPvFisico;

                    var phdItemPvFisico = e.Item.FindControl("phdItemPvFisico") as PlaceHolder;
                    var ltrPvFisicoNumeroPv = e.Item.FindControl("ltrPvFisicoNumeroPv") as Literal;
                    var ltrPvFisicoNsu = e.Item.FindControl("ltrPvFisicoNsu") as Literal;
                    var ltrPvFisicoDataRecarga = e.Item.FindControl("ltrPvFisicoDataRecarga") as Literal;
                    var ltrPvFisicoHoraRecarga = e.Item.FindControl("ltrPvFisicoHoraRecarga") as Literal;
                    var ltrPvFisicoResumoVenda = e.Item.FindControl("ltrPvFisicoResumoVenda") as Literal;
                    var ltrPvFisicoTipoVenda = e.Item.FindControl("ltrPvFisicoTipoVenda") as Literal;
                    var ltrPvFisicoOperadora = e.Item.FindControl("ltrPvFisicoOperadora") as Literal;
                    var ltrPvFisicoCelular = e.Item.FindControl("ltrPvFisicoCelular") as Literal;
                    var ltrPvFisicoValorRecarga = e.Item.FindControl("ltrPvFisicoValorRecarga") as Literal;
                    var ltrPvFisicoValorComissao = e.Item.FindControl("ltrPvFisicoValorComissao") as Literal;
                    var ltrPvFisicoStatusRecarga = e.Item.FindControl("ltrPvFisicoStatusRecarga") as Literal;

                    phdItemPvFisico.Visible = true;

                    ltrPvFisicoNumeroPv.Text = dataItem.NumeroEstabelecimento.ToString();
                    ltrPvFisicoNsu.Text = dataItem.NsuRecarga.ToString();
                    ltrPvFisicoDataRecarga.Text = dataItem.DataHoraRecarga.ToString("dd/MM/yy");
                    ltrPvFisicoHoraRecarga.Text = dataItem.DataHoraRecarga.ToString("HH:mm:ss");
                    ltrPvFisicoResumoVenda.Text = dataItem.NumeroRV;
                    ltrPvFisicoTipoVenda.Text = dataItem.TipoVenda;
                    ltrPvFisicoOperadora.Text = dataItem.NomeOperadora;
                    ltrPvFisicoCelular.Text = dataItem.NumeroCelular;
                    ltrPvFisicoValorRecarga.Text = dataItem.ValorBrutoRecarga.ToString("N2");
                    ltrPvFisicoValorComissao.Text = dataItem.ValorLiquidoComissao.ToString("N2");
                    ltrPvFisicoStatusRecarga.Text = dataItem.StatusComissao;
                }
                else if (item is RecargaCelularPvLogico)
                {
                    var dataItem = item as RecargaCelularPvLogico;

                    var phdItemPvLogico = e.Item.FindControl("phdItemPvLogico") as PlaceHolder;
                    var ltrPvLogicoNumeroPv = e.Item.FindControl("ltrPvLogicoNumeroPv") as Literal;
                    var ltrPvLogicoNsu = e.Item.FindControl("ltrPvLogicoNsu") as Literal;
                    var ltrPvLogicoDataRecarga = e.Item.FindControl("ltrPvLogicoDataRecarga") as Literal;
                    var ltrPvLogicoHoraRecarga = e.Item.FindControl("ltrPvLogicoHoraRecarga") as Literal;
                    var ltrPvLogicoResumoVenda = e.Item.FindControl("ltrPvLogicoResumoVenda") as Literal;
                    var ltrPvLogicoTipoVenda = e.Item.FindControl("ltrPvLogicoTipoVenda") as Literal;
                    var ltrPvLogicoOperadora = e.Item.FindControl("ltrPvLogicoOperadora") as Literal;
                    var ltrPvLogicoCelular = e.Item.FindControl("ltrPvLogicoCelular") as Literal;
                    var ltrPvLogicoValorRecarga = e.Item.FindControl("ltrPvLogicoValorRecarga") as Literal;
                    var ltrPvLogicoValorComissao = e.Item.FindControl("ltrPvLogicoValorComissao") as Literal;
                    var ltrPvLogicoStatusRecarga = e.Item.FindControl("ltrPvLogicoStatusRecarga") as Literal;

                    phdItemPvLogico.Visible = true;

                    ltrPvLogicoNumeroPv.Text = dataItem.NumeroEstabelecimento.ToString();
                    ltrPvLogicoNsu.Text = dataItem.NsuRecarga.ToString();
                    ltrPvLogicoDataRecarga.Text = dataItem.DataHoraRecarga.ToString("dd/MM/yy");
                    ltrPvLogicoHoraRecarga.Text = dataItem.DataHoraRecarga.ToString("HH:mm:ss");
                    ltrPvLogicoResumoVenda.Text = dataItem.NumeroRV.ToString();
                    ltrPvLogicoTipoVenda.Text = dataItem.TipoVenda;
                    ltrPvLogicoOperadora.Text = dataItem.NomeOperadora;
                    ltrPvLogicoCelular.Text = dataItem.NumeroCelular;
                    ltrPvLogicoValorRecarga.Text = dataItem.ValorBrutoRecarga.ToString("N2");
                    ltrPvLogicoValorComissao.Text = "-";
                    ltrPvLogicoStatusRecarga.Text = dataItem.StatusComissao;
                }
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                if (this.SessaoAtual.PVLogico)
                {
                    PlaceHolder phdFooterPvLogico = e.Item.FindControl("phdFooterPvLogico") as PlaceHolder;
                    Literal ltrPvLogicoTotalValorRecarga = e.Item.FindControl("ltrPvLogicoTotalValorRecarga") as Literal;
                    Literal ltrPvLogicoTotalValorComissao = e.Item.FindControl("ltrPvLogicoTotalValorComissao") as Literal;

                    phdFooterPvLogico.Visible = true;

                    ltrPvLogicoTotalValorRecarga.Text = totalizador.TotalValorBrutoRecarga.ToString("N2");
                    ltrPvLogicoTotalValorComissao.Text = "-";
                }
                else if (this.SessaoAtual.PVFisico)
                {
                    PlaceHolder phdFooterPvFisico = e.Item.FindControl("phdFooterPvFisico") as PlaceHolder;

                    Literal ltrPvFisicoTotalValorRecarga = e.Item.FindControl("ltrPvFisicoTotalValorRecarga") as Literal;
                    Literal ltrPvFisicoTotalValorComissao = e.Item.FindControl("ltrPvFisicoTotalValorComissao") as Literal;

                    phdFooterPvFisico.Visible = true;

                    ltrPvFisicoTotalValorRecarga.Text = totalizador.TotalValorBrutoRecarga.ToString("N2");
                    ltrPvFisicoTotalValorComissao.Text = totalizador.TotalValorLiquidoComissao.ToString("N2");
                }                
            }
        }

        /// <summary>
        /// Geração de conteúdo CSV do relatório.
        /// Realiza a busca por blocos. A cada bloco consultado, é gerada a saída CSV.
        /// Método criado para grandes extratos, para evitar timeout do front-end com o client.
        /// </summary>
        /// <param name="dadosBusca">Dados informados no filtro</param>
        /// <param name="funcaoOutput">Function que irá gerar a saída do relatório. 
        /// No caso, Response.Write do conteúdo CSV gerado.</param>
        public void GerarConteudoRelatorio(BuscarDados dadosBusca, Action<String> funcaoOutput)
        {
            using (Logger log = Logger.IniciarLog(NomeOperacao))
            {
                try
                {
                    log.GravarLog(EventoLog.ChamadaServico, dadosBusca);

                    //Consulta o totalizador do relatório
                    using (var contexto = new ContextoWCF<HISServicoWA_Extrato_VendasClient>())
                    {
                        //Se for PV Físico
                        if (SessaoAtual.PVFisico)
                        {
                            this.totalizador = contexto.Cliente.ConsultarRecargaCelularPvFisicoTotalizadores(
                                out statusTotalizador,
                                GuidPesquisaTotalizador(),
                                dadosBusca.DataInicial,
                                dadosBusca.DataFinal,
                                dadosBusca.Estabelecimentos);
                        }
                        //Se for PV Lógico
                        else if(SessaoAtual.PVLogico)
                        {
                            this.totalizador = contexto.Cliente.ConsultarRecargaCelularPvLogicoTotalizadores(
                                out statusTotalizador,
                                GuidPesquisaTotalizador(),
                                dadosBusca.DataInicial,
                                dadosBusca.DataFinal,
                                dadosBusca.Estabelecimentos);
                        }
                    }

                    log.GravarMensagem("Consultou totalizadores", new { this.totalizador, this.statusTotalizador });

                    //Em caso de erro no totalizador, cancela consulta de registros
                    if (statusTotalizador.CodigoRetorno != 0)
                        return;
                    else //Carrega os totalizadores
                        CarregarTotalizadores();

                    Int32 registroInicial = 0;
                    Int32 tamanhoPagina = 500;
                    String linhaFooter = String.Empty;
                    String linhaHeader = String.Empty;

                    //Consulta completa por blocos de 500 em 500 registros, utilizando o serviço do relatório
                    do
                    {
                        Int32 qtdTotalRegistros = 1000 + registroInicial;

                        log.GravarMensagem(String.Concat("Consultando registro inicial: ", registroInicial, " - ", qtdTotalRegistros));

                        //Consulta dos registros do relatório
                        using (var contexto = new ContextoWCF<HISServicoWA_Extrato_VendasClient>())
                        {
                            //Se for PV Físico
                            if (this.SessaoAtual.PVFisico)
                            {
                                this.registros = contexto.Cliente.ConsultarRecargaCelularPvFisico(
                                    GuidPesquisa(),
                                    registroInicial,
                                    tamanhoPagina,
                                    ref qtdTotalRegistros,
                                    out statusRelatorio,
                                    dadosBusca.DataInicial,
                                    dadosBusca.DataFinal,
                                    dadosBusca.Estabelecimentos);
                            }
                            //Se for PV Lógico
                            else if (this.SessaoAtual.PVLogico)
                            {
                                this.registros = contexto.Cliente.ConsultarRecargaCelularPvLogico(
                                    GuidPesquisa(),
                                    registroInicial,
                                    tamanhoPagina,
                                    ref qtdTotalRegistros,
                                    out statusRelatorio,
                                    dadosBusca.DataInicial,
                                    dadosBusca.DataFinal,
                                    dadosBusca.Estabelecimentos);
                            }
                        }

                        log.GravarMensagem(String.Concat("Consultou registro inicial: ", registroInicial));

                        //Em caso de erro na consulta da página, cancela consulta de registros
                        if (statusRelatorio == null || statusRelatorio.CodigoRetorno != 0)
                            return;
                        else if (this.registros != null)
                        {
                            CarregarDadosRelatorio();

                            if (registros.Length > 0)
                            {
                                //Gera HTML, e utiliza HTML gerado para gerar CSV do conteúdo da página
                                String html = base.RenderizarControles(true, repDados);
                                String csv = CSVExporter.GerarCSV(html, "\t");

                                List<String> linhas = csv.Split(new String[] { Environment.NewLine }, 
                                    StringSplitOptions.RemoveEmptyEntries).ToList();

                                //Prepara conteúdo CSV do bloco atual de registros retornados
                                if (linhas != null && linhas.Count > 0)
                                {
                                    //Separa linhas do header e footer
                                    linhaHeader = linhas[0];
                                    linhaFooter = linhas[linhas.Count - 1];

                                    //Remove linhas do header e do footer                                   
                                    linhas.RemoveAt(0);
                                    linhas.RemoveAt(linhas.Count - 1);

                                    csv = String.Concat(String.Join(Environment.NewLine, linhas.ToArray()), Environment.NewLine);
                                }

                                //Se for primeira iteração, escreve linha header
                                if (registroInicial == 0)
                                    funcaoOutput(String.Concat(linhaHeader, Environment.NewLine));

                                funcaoOutput(csv);
                            }
                            else
                            {
                                //Não existem mais registros, escreve footer na saída
                                funcaoOutput(linhaFooter);
                            }

                            registroInicial += this.registros.Length;
                        }

                    //Enquanto forem retornados registros pelo serviço WCF, continua processamento
                    } while (this.registros != null && this.registros.Length > 0);
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
    }
}