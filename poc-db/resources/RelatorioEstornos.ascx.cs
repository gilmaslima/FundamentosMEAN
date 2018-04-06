/*
© Copyright 2015 Rede S.A.
Autor : Dhouglas Lombello
Empresa : Iteris Consultoria e Software
Histórico : (DD/MM/AAAA - Comentário)
- 20/07/2015 - Início do projeto Relatório de Estorno.
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.WAExtratoEstornosServico;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.Estorno
{
    /// <summary>
    /// Relatório de Estornos.
    /// </summary>
    public partial class RelatorioEstorno : BaseUserControl, IRelatorioHandler
    {
        #region [Propriedades]

        /// <summary>CultureInfo pt-BR</summary>
        private readonly CultureInfo ptBr = CultureInfo.CreateSpecificCulture("pt-BR");

        /// <summary>
        /// Nome da operação.
        /// </summary>
        private String NomeOperacao { get { return "Relatório de Estorno"; } }

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
        protected TotalizadoresEstorno RcTotalizadores
        {
            get { return (TotalizadoresEstorno)rcTotalizadores; }
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
        private EstornoTotalizador Totalizador
        {
            get
            {
                return ViewState.Count > 0 && ViewState["EstornoTotalizador"] != null ?
                    (EstornoTotalizador)ViewState["EstornoTotalizador"] : new EstornoTotalizador();
            }
            set { ViewState["EstornoTotalizador"] = value; }
        }

        /// <summary>
        /// Lista de registros de estornos.
        /// </summary>
        private List<WAExtratoEstornosServico.Estorno> Registros
        {
            get
            {
                return ViewState.Count > 0 && ViewState["RegistrosEstorno"] != null ?
                    (List<WAExtratoEstornosServico.Estorno>)ViewState["RegistrosEstorno"] : new List<WAExtratoEstornosServico.Estorno>();
            }
            set { ViewState["RegistrosEstorno"] = value; }
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
        protected void RegistrosPorPaginaTamanhoPaginaChanged(Object sender, Int32 tamanhoPagina)
        {
            Consultar(ObterBuscarDados(), 1, tamanhoPagina, ObjPaginacao.PaginasVirtuais);
        }

        /// <summary>
        /// Evento Cache Todos os Registros do objeto objPaginacao.
        /// </summary>
        protected void PaginacaoCacheTodosRegistros()
        {
            Consultar(ObterBuscarDados(), 1, 0, Int32.MaxValue);
        }

        /// <summary>
        /// Evento Paginação Change do objeto objPaginacao
        /// </summary>
        /// <param name="pagina">Número da página</param>
        /// <param name="e">Objeto com argumentos do evento.</param>
        protected void PaginacaoOnPaginacaoChanged(int pagina, EventArgs e)
        {
            Consultar(ObterBuscarDados(), pagina, DdlRegistrosPorPagina.SelectedSize, ObjPaginacao.PaginasVirtuais);
        }

        /// <summary>
        /// Evento Data Bound do repeater dos registros.
        /// </summary>
        /// <param name="sender">Objeto rptDados</param>
        /// <param name="e">Objeto com argumentos do evento.</param>
        protected void Dados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                EstornoD dataItem = e.Item.DataItem as EstornoD;

                Literal lblNumEstabelecimento = e.Item.FindControl("lblNumEstabelecimento") as Literal;
                Literal lblDataVenda = e.Item.FindControl("lblDataVenda") as Literal;
                Literal lblHoraEstorno = e.Item.FindControl("lblHoraEstorno") as Literal;
                Literal lblTipoVenda = e.Item.FindControl("lblTipoVenda") as Literal;
                Literal lblModalidade = e.Item.FindControl("lblModalidade") as Literal;
                Literal lblBandeira = e.Item.FindControl("lblBandeira") as Literal;
                Literal lblCvNsuTid = e.Item.FindControl("lblCvNsuTid") as Literal;
                Literal lblNumeroCartao = e.Item.FindControl("lblNumeroCartao") as Literal;
                Literal lblValor = e.Item.FindControl("lblValor") as Literal;
                Literal lblTerminal = e.Item.FindControl("lblTerminal") as Literal;
                Literal lblIndicadorTokenizacao = e.Item.FindControl("lblIndicadorTokenizacao") as Literal;


                lblNumEstabelecimento.Text = dataItem.NumeroPV.ToString();
                lblDataVenda.Text = dataItem.DataHoraVenda.ToString("dd/MM/yyyy", ptBr);
                lblHoraEstorno.Text = dataItem.DataHoraEstorno.ToString("HH:mm:ss", ptBr);
                lblTipoVenda.Text = dataItem.DescricaoTipoConta;
                lblModalidade.Text = dataItem.DescricaoModalidadeVenda;
                lblBandeira.Text = dataItem.DescricaoBandeira.ToString();
                lblCvNsuTid.Text = dataItem.NSU.ToString();
                lblNumeroCartao.Text = dataItem.NumeroCartao;
                lblIndicadorTokenizacao.Text = (String.Compare(dataItem.IndicadorTokenizacao, "s", true) == 0) ? "Sim" : "Não";
                lblValor.Text = dataItem.ValorVenda.ToString("N2", ptBr);
                lblTerminal.Text = dataItem.CodigoTerminal;       
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                Literal lblValorTotal = e.Item.FindControl("lblValorTotal") as Literal;
                lblValorTotal.Text = Totalizador.ValorTotalEstornos.ToString("N2", ptBr);
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
            get { return "RelatorioEstorno"; }
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

                    using (var contexto = new ContextoWCF<WAExtratoEstornosServico.HISServicoWAExtratoEstornosClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, pagina, tamanhoPagina, paginasVirtuais, registroInicial, qtdRegistrosVirtuais, GuidPesquisa = GuidPesquisa(), GuidTotalizador = GuidPesquisaTotalizador() });

                        StatusRetorno statusRelatorio, statusTotalizador;
                        List<WAExtratoEstornosServico.Estorno> registros;

                        Int16 codigoTipoVenda;
                        // Valores do filtro de busca para Tipo de venda são direfentes dos Programas Mainframe do Estorno.
                        // Aqui faço o de-para nos valores.
                        switch (dadosBusca.IDTipoVenda)
                        {
                            case 0: codigoTipoVenda = 1; break; // Crédito
                            case 1: codigoTipoVenda = 2; break; // Débito
                            default: codigoTipoVenda = 1; break; // Crédito
                        }

                        Logger.GravarLog("Consultar() - Gravando objeto totalizador pré chamada WCF", Totalizador);
                        Totalizador = contexto.Cliente.ConsultarRelatorioEstorno(
                            GuidPesquisaTotalizador(),
                            GuidPesquisa(),
                            dadosBusca.DataInicial,
                            dadosBusca.DataFinal,
                            (Int16)dadosBusca.IDModalidade,
                            (Int16)codigoTipoVenda,
                            dadosBusca.Estabelecimentos.ToList(),
                            registroInicial,
                            tamanhoPagina,
                            ref qtdRegistrosVirtuais,
                            out registros,
                            out statusTotalizador,
                            out statusRelatorio);

                        log.GravarLog(EventoLog.RetornoServico, new { registros, statusTotalizador, statusRelatorio, qtdRegistrosVirtuais });

                        if (statusRelatorio != null
                         && statusRelatorio.CodigoRetorno != (Int16)Constantes.CodigoRetorno.Ok
                         && statusRelatorio.CodigoRetorno != (Int16)Constantes.CodigoRetorno.NenhumArgumentoEncontrado)
                        {
                            base.ExibirPainelExcecao(statusRelatorio.Fonte, statusRelatorio.CodigoRetorno);
                            return;
                        }

                        if (statusTotalizador != null
                         && statusTotalizador.CodigoRetorno != (Int16)Constantes.CodigoRetorno.Ok
                         && statusRelatorio.CodigoRetorno != (Int16)Constantes.CodigoRetorno.NenhumArgumentoEncontrado)
                        {
                            base.ExibirPainelExcecao(statusTotalizador.Fonte, statusTotalizador.CodigoRetorno);
                            return;
                        }

                        ObjPaginacao.QuantidadeTotalRegistros = qtdRegistrosVirtuais;
                        Registros = registros;

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
            if (this.Totalizador != null && this.Totalizador.Bandeiras != null && this.Totalizador.Bandeiras.Count > 0)
            {
                this.RcTotalizadores.ValorTotal = this.Totalizador.ValorTotalEstornos;
                this.RcTotalizadores.Bandeiras = this.Totalizador.Bandeiras.Select(b => new TotalizadoresEstorno.Bandeira(b.DescricaoBandeira, b.ValorBandeira)).ToList();
            }
            this.RcTotalizadores.Atualizar();
        }

        #endregion
    }
}

