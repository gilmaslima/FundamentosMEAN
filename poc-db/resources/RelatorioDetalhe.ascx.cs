/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.WAExtratoRecargaCelularServico;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.RecargaCelular
{
    /// <summary>
    /// Relatório de Detalhamento de Recarga de Celular
    /// </summary>
    public partial class RelatorioDetalhe : BaseUserControl, IRelatorioHandler
    {
        #region [ Controles ]

        /// <summary>
        /// objPaginacao control.
        /// </summary>
        protected Paginacao ObjPaginacao { get { return (Paginacao)objPaginacao; } }

        /// <summary>
        /// rcTotalizadores control.
        /// </summary>
        protected Totalizadores RcTotalizadores { get { return (Totalizadores)rcTotalizadores; } }

        /// <summary>
        /// ddlRegistrosPorPagina control.
        /// </summary>
        protected TableSize DdlRegistrosPorPagina { get { return (TableSize)ddlRegistrosPorPagina; } }

        #endregion

        #region [ Propriedades ]

        /// <summary>
        /// Implementação interface: ID do Controle
        /// </summary>
        public string IdControl { get { return "RecargaCelularDetalhe"; } }

        /// <summary>
        /// Identificação para log
        /// </summary>
        public String NomeOperacao { get { return "Relatório - Recarga de Celular - Detalhe"; } }
        
        /// <summary>
        /// QueryString - Número PV
        /// </summary>
        public Int32 NumeroPv { get { return QS["NumeroPv"].ToInt32(0); } }
        /// <summary>
        /// QueryString - Número RV
        /// </summary>
        public Int32 NumeroRv { get { return QS["NumeroRv"].ToInt32(0); } }
        /// <summary>
        /// QueryString - Data Pagamento
        /// </summary>
        public DateTime DataPagamento { get { return QS["DataPagamento"].ToDate("ddMMyyyyHHmmssfff"); } }
        
        /// <summary>
        /// Registros do relatório
        /// </summary>
        private List<RecargaCelularDetalhe> Registros;

        /// <summary>
        /// Registro dos totalizadores
        /// </summary>
        private WAExtratoResumoVendasServico.RecargaCelularResumo Totalizadores
        {
            get { return (WAExtratoResumoVendasServico.RecargaCelularResumo)ViewState["Totalizadores"]; }
            set { ViewState["Totalizadores"] = value; }
        }

        #endregion

        #region [ Eventos da Página ]

        /// <summary>
        /// Page load
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            ObjPaginacao.RegistrosPorPagina = DdlRegistrosPorPagina.SelectedSize;
        }

        #endregion

        #region [ Triggers da Consulta ]

        /// <summary>
        /// Pesquisa inicial do relatório (Buscar)
        /// </summary>
        public void Pesquisar(BuscarDados dados)
        {
            Consultar(1, DdlRegistrosPorPagina.SelectedSize, ObjPaginacao.PaginasVirtuais);
        }

        /// <summary>
        /// Alteração de tamanho da página
        /// </summary>
        protected void ddlRegistrosPorPagina_TamanhoPaginaChanged(Object sender, Int32 tamanhoPagina)
        {
            Consultar(1, tamanhoPagina, ObjPaginacao.PaginasVirtuais);
        }

        /// <summary>
        /// Mudança de página do relatório
        /// </summary>
        /// <param name="pagina">Página solicitada</param>
        protected void objPaginacao_onPaginacaoChanged(Int32 pagina, EventArgs e)
        {
            Consultar(pagina, DdlRegistrosPorPagina.SelectedSize, ObjPaginacao.PaginasVirtuais);
        }

        /// <summary>
        /// Método chamada para forçar cache de todos os registros do relatório
        /// </summary>
        protected void objPaginacao_CacheTodosRegistros()
        {
            Consultar(1, 0, Int32.MaxValue);
        }

        /// <summary>
        /// Geração de conteúdo HTML para exportação do relatório
        /// </summary>
        /// <param name="dados">Parâmetros da consulta</param>
        /// <param name="quantidadeRegistros">Quantidade de registros</param>
        /// <param name="incluirTotalizadores">Incluir totalizadores na exportação?</param>
        /// <returns>HTML do relatório</returns>
        public String ObterTabelaExcel(BuscarDados dados, Int32 quantidadeRegistros, Boolean incluirTotalizadores)
        {
            Consultar(1, quantidadeRegistros, Int32.MaxValue);
            if (incluirTotalizadores)
                return base.RenderizarControles(true, rcTotalizadores, rptDados);
            else
                return base.RenderizarControles(true, rptDados);
        }

        #endregion

        #region [ Consultas ]

        /// <summary>
        /// Consulta dos dados do relatório através de serviço
        /// </summary>
        /// <param name="pagina">Número da página</param>
        /// <param name="tamanhoPagina">Quantidade de registros por página</param>
        /// <param name="paginasVirtuais">Quantidade de páginas virtuais</param>
        private void Consultar(Int32 pagina, Int32 tamanhoPagina, Int32 paginasVirtuais)
        {
            using (Logger log = Logger.IniciarLog(NomeOperacao))
            {
                try
                {
                    this.ConsultarTotalizadores();

                    ObjPaginacao.PaginaAtual = pagina;

                    Int32 registroInicial = (pagina - 1) * tamanhoPagina;
                    Int32 qtdRegistrosVirtuais = (paginasVirtuais == Int32.MaxValue) ? Int32.MaxValue : paginasVirtuais * tamanhoPagina;
                    var status = default(StatusRetorno);

                    using (var ctx = new ContextoWCF<HISServicoWA_Extrato_RecargaCelularClient>())
                    {
                        this.Registros = ctx.Cliente.ConsultarRecargaCelularDetalhePorResumo(
                            GuidPesquisa(),
                            registroInicial,
                            tamanhoPagina,
                            ref qtdRegistrosVirtuais,
                            out status,
                            this.DataPagamento,
                            this.NumeroPv,
                            this.NumeroRv);
                    }

                    if (status.CodigoRetorno != 0)
                    {
                        base.ExibirPainelExcecao(status.Fonte, status.CodigoRetorno);
                        return;
                    }

                    ObjPaginacao.QuantidadeTotalRegistros = qtdRegistrosVirtuais;

                    CarregarDadosRelatorio();                    
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
        /// Consulta os dados para preenchimento dos totalizadores
        /// </summary>
        private void ConsultarTotalizadores()
        {
            using (Logger log = Logger.IniciarLog(NomeOperacao))
            {
                try
                {
                    var status = default(WAExtratoResumoVendasServico.StatusRetorno);
                    var origem = WAExtratoResumoVendasServico.RecargaCelularResumoOrigem.PagamentosAjustados;

                    using (var ctx = new ContextoWCF<WAExtratoResumoVendasServico.HISServicoWA_Extrato_ResumoVendasClient>())
                        this.Totalizadores = ctx.Cliente.ConsultarRecargaCelularResumo(
                            out status, this.NumeroPv, this.NumeroRv, this.DataPagamento, origem);

                    if (status.CodigoRetorno != 0)
                    {
                        base.ExibirPainelExcecao(status.Fonte, status.CodigoRetorno);
                        return;
                    }

                    CarregarTotalizadores();
                }
                catch (FaultException<WAExtratoResumoVendasServico.GeneralFault> ex)
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
        /// Bind dos dados no relatório
        /// </summary>
        private void CarregarDadosRelatorio()
        {
            rptDados.DataSource = this.Registros;
            rptDados.DataBind();

            //Verifica os controles que devem estar visíveis
            base.VerificaControlesVisiveis(this.Registros.Count, null, null);
        }

        /// <summary>
        /// Bind dos dados nos totalizadores
        /// </summary>
        private void CarregarTotalizadores()
        {
            RcTotalizadores.ValorTotalSuperior = this.Totalizadores.ValorTotalTransacao;
            RcTotalizadores.ValorTotalInferior = this.Totalizadores.ValorTotalComissao;
            RcTotalizadores.Atualizar();
        }

        #endregion

        #region [ Eventos de Controles ]

        /// <summary>
        /// Bind dos dados do relatório
        /// </summary>
        protected void rptDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblNsu = (Literal)e.Item.FindControl("lblNsu");
                var lblDataRecarga = (Literal)e.Item.FindControl("lblDataRecarga");
                var lblHora = (Literal)e.Item.FindControl("lblHora");
                var lblOperadora = (Literal)e.Item.FindControl("lblOperadora");
                var lblCelular = (Literal)e.Item.FindControl("lblCelular");
                var lblValorRecarga = (Literal)e.Item.FindControl("lblValorRecarga");
                var lblValorComissao = (Literal)e.Item.FindControl("lblValorComissao");
                var lblStatus = (Literal)e.Item.FindControl("lblStatus");
                var item = e.Item.DataItem as WAExtratoRecargaCelularServico.RecargaCelularDetalhe;

                lblNsu.Text = item.NumeroNsu.ToString();
                lblDataRecarga.Text = item.DataHoraTransacao.ToString("dd/MM/yyyy");
                lblHora.Text = item.DataHoraTransacao.ToString("HH:mm:ss");
                lblOperadora.Text = item.NomeOperadora;
                lblCelular.Text = item.NumeroCelular;
                lblValorRecarga.Text = item.ValorTransacao.ToString("N2", PtBR);
                lblValorComissao.Text = item.ValorComissao == 0 ? "-" : item.ValorComissao.ToString("N2", PtBR);
                lblStatus.Text = item.StatusTransacao;
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                var lblValorRecarga = (Literal)e.Item.FindControl("lblValorRecarga");
                var lblValorComissao = (Literal)e.Item.FindControl("lblValorComissao");

                lblValorRecarga.Text = this.Totalizadores.ValorTotalTransacao.ToString("N2", PtBR);
                lblValorComissao.Text = this.Totalizadores.ValorTotalComissao.ToString("N2", PtBR);
            }
        }

        #endregion
    }
}
