/*
© Copyright 2014 Rede S.A.
Autor : Valdinei Ribeiro
Empresa : Iteris Consultoria e Software
*/

using Microsoft.SharePoint.Utilities;
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
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ValoresConsolidados
{
    /// <summary>
    /// Turquia - Relatório Vendas Crédito.
    /// </summary>
    public partial class RelatorioVendasDebito : BaseUserControl, IRelatorioHandler
    {
        #region [Propriedades]

        /// <summary>CultureInfo pt-BR</summary>
        private readonly CultureInfo ptBr = CultureInfo.CreateSpecificCulture("pt-BR");

        /// <summary>
        /// Nome da operação.
        /// </summary>
        private String NomeOperacao { get { return "Relatório - Valores de Consolidados de vendas - Crédito"; } }

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
        private TotalVendasDebitoPorPeriodoBandeira Totalizador
        {
            get
            {
                return ViewState.Count > 0 && ViewState["Totalizador"] != null ?
                    (TotalVendasDebitoPorPeriodoBandeira)ViewState["Totalizador"] : new TotalVendasDebitoPorPeriodoBandeira();
            }
            set { ViewState["Totalizador"] = value; }
        }

        /// <summary>
        /// Lista de registros de vendas.
        /// </summary>
        private List<VendasDebitoPorDiaPv> Registros
        {
            get
            {
                return ViewState.Count > 0 && ViewState["Registros"] != null ?
                    (List<VendasDebitoPorDiaPv>)ViewState["Registros"] : new List<VendasDebitoPorDiaPv>();
            }
            set { ViewState["Registros"] = value; }
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
                VendasDebitoPorDiaPv dataItem = e.Item.DataItem as VendasDebitoPorDiaPv;

                Literal lblNumEstabelecimento = e.Item.FindControl("lblNumEstabelecimento") as Literal;
                Literal lblDataVenda = e.Item.FindControl("lblDataVenda") as Literal;
                Literal lblValorBruto = e.Item.FindControl("lblValorBruto") as Literal;
                Literal lblValorLiquido = e.Item.FindControl("lblValorLiquido") as Literal;
                LinkButton btnDetalhe = e.Item.FindControl("btnDetalhe") as LinkButton;

                lblNumEstabelecimento.Text = dataItem.NumeroEstabelecimanto.ToString();
                lblDataVenda.Text = dataItem.DataVenda.Value.ToString("dd/MM/yyyy", ptBr);
                lblValorBruto.Text = dataItem.TotalBruto.ToString("N2", ptBr);
                lblValorLiquido.Text = dataItem.TotalLiquido.ToString("N2", ptBr);
                btnDetalhe.CommandArgument = e.Item.ItemIndex.ToString();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                Literal lblTotalValorLiquido = e.Item.FindControl("lblTotalValorLiquido") as Literal;
                Literal lblTotalValorBruto = e.Item.FindControl("lblTotalValorBruto") as Literal;

                lblTotalValorLiquido.Text = Totalizador.TotalLiquido.ToString("N2");
                lblTotalValorBruto.Text = Totalizador.TotalBruto.ToString("N2");
            }
        }

        /// <summary>
        /// Evento clique o Link Button Detalhe
        /// </summary>
        /// <param name="sender">Objeto btnDetalhe</param>
        /// <param name="e">Objeto com argumentos do evento.</param>
        protected void btnDetalhe_Click(object sender, EventArgs e)
        {
            //Altera os dados da busca para exibir o relatório de débito - detalhe.
            BuscarDados dados = ObterBuscarDados();
            dados.IDTipoVenda = Convert.ToInt32(TipoVenda.Debito);
            GravarBuscarDados(dados);

            LinkButton btnDetalhe = sender as LinkButton;
            Int32? index = btnDetalhe.CommandArgument.ToInt32Null();

            if (index.HasValue && Registros.Count > index.Value)
            {
                VendasDebitoPorDiaPv dataItem = Registros.ToArray()[index.Value];

                QueryStringSegura qsDetalhe = new QueryStringSegura();
                qsDetalhe["DataVenda"] = dataItem.DataVenda.Value.ToString();
                qsDetalhe["NumeroEstabelecimanto"] = dataItem.NumeroEstabelecimanto.ToString();
                qsDetalhe["TipoConsulta"] = "detalhe";

                SPUtility.Redirect(base.ObterUrlLinkDetalhe(qsDetalhe), SPRedirectFlags.CheckUrl, this.Context);
            }
        }

        /// <summary>
        /// Evento clique o Link Button Ver Todos.
        /// </summary>
        /// <param name="sender">Objeto btnVerTodos.</param>
        /// <param name="e">Objeto com argumentos do evento.</param>
        protected void btnVerTodos_Click(object sender, EventArgs e)
        {
            BuscarDados dados = ObterBuscarDados();            
            QueryStringSegura qsVerTodos = new QueryStringSegura();

            qsVerTodos["TipoConsulta"] = "verTodos";
            qsVerTodos["DataInicial"] = dados.DataInicial.ToString();
            qsVerTodos["DataFinal"] = dados.DataFinal.ToString();

            String[] estabelecimentos = dados.Estabelecimentos.Select(estabelecimento => estabelecimento.ToString()).ToArray();
            qsVerTodos["Estabelecimentos"] = String.Join(";", estabelecimentos);            

            SPUtility.Redirect(base.ObterUrlLinkDetalhe(qsVerTodos), SPRedirectFlags.CheckUrl, this.Context);
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
            Consultar(dados, 1, quantidadeRegistros, Int32.MaxValue);

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
            get { return "RelatorioVendasDebito"; }
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

                    using (var contexto = new ContextoWCF< HisServicoWaExtratoValoresConsolidadosVendasClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, pagina, tamanhoPagina, paginasVirtuais, registroInicial, qtdRegistrosVirtuais, GuidPesquisa = GuidPesquisa(), GuidTotalizador = GuidPesquisaTotalizador() });

                        StatusRetorno statusRelatorio, statusTotalizador;
                        List<VendasDebitoPorDiaPv> registros;
                        TotalVendasDebitoPorPeriodoBandeira totalizador;

                        contexto.Cliente.ConsultarRelatorioVendasDebitoPorPeriodo(
                            GuidPesquisaTotalizador(),
                            GuidPesquisa(),
                            dadosBusca.DataInicial,
                            dadosBusca.DataFinal,                            
                            dadosBusca.Estabelecimentos.ToList(),
                            registroInicial,
                            tamanhoPagina,
                            ref qtdRegistrosVirtuais,
                            out totalizador,
                            out registros,
                            out statusTotalizador,
                            out statusRelatorio);

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

            if (Totalizador.ListaTotalVendasDebitoPorBandeira != null)
                RcTotalizadores.Bandeiras = Totalizador.ListaTotalVendasDebitoPorBandeira.Select(bandeira =>
                    new Totalizadores.Bandeira(bandeira.DescricaoBandeira, bandeira.TotalLiquido)).ToList();

            RcTotalizadores.Atualizar();
        }
        
        #endregion
    }
}
