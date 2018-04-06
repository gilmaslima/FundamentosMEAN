using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.Servico.PA;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI.WebControls;
using Paginacao = Redecard.PN.Extrato.SharePoint.ControlTemplates.Paginacao;

namespace Redecard.PN.Extrato.SharePoint.PagamentosAjustados.PagamentosAjustados
{
    public partial class PagamentosAjustadosUserControl : BaseUserControl, IRelatorioHandler
    {
        #region Atributos e Propriedades
        protected Paginacao objPaginacao;
        List<SPListaPadrao> objListBanco;
        
        /// <summary>Registros</summary>
        private Servico.PA.BasicContract[] Registros
        {
            get { return (Servico.PA.BasicContract[])ViewState["Registros"]; }
            set { ViewState["Registros"] = value; }
        }
        #endregion Atributos e Propriedades

        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            objPaginacao.onPaginacaoChanged += new Paginacao.PaginacaoChanged(objPaginacao_onPaginacaoChanged);
            objPaginacao.RegistrosPorPagina = ObterQuantidadeRegistrosPagina();
        }

        private Int32 ObterQuantidadeRegistrosPagina()
        {
            return ddlRegistroPorPagina.SelectedSize;
        }

        protected void objPaginacao_onPaginacaoChanged(int pagina, EventArgs e)
        {
            Consultar(ObterBuscarDados(), pagina, ObterQuantidadeRegistrosPagina(), false);
        }

        protected String objBuscar_onObterConteudoHTMLDownload(BuscarDados buscarDados, Boolean recomporTela)
        {
            Int32 savePaginaAtual = objPaginacao.PaginaAtual;
            Int32 saveTotalPaginas = objPaginacao.TotalPagina;
            Int32 saveQtdRegistrosPagina = ObterQuantidadeRegistrosPagina();

            BuscarDados objEnvio = buscarDados;

            Consultar(objEnvio, 1, MAX_LINHAS_DOWNLOAD, true);

            String result = ObterHTMLControle(this.grvDados); // outTabela

            if (recomporTela)
            {
                Consultar(objEnvio, savePaginaAtual, saveQtdRegistrosPagina, true);
            }
            return result;
        }

        protected void objBuscar_onBuscar(BuscarDados Dados, EventArgs e)
        {
            Consultar(Dados, 1, ObterQuantidadeRegistrosPagina(), false);
        }

        protected void ddlRegistroPorPagina_SelectedIndexChanged(Object sender, Int32 selectedSize)
        {
            //esconde os resultas
            MostrarResultadoRelatorio(false);

            Consultar(ObterBuscarDados(), 1, selectedSize, false);
        }

        protected void grvDados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Servico.PA.ConsultarOrdensCreditoEnviadosAoBancoDetalheRetorno objItem = e.Row.DataItem as Servico.PA.ConsultarOrdensCreditoEnviadosAoBancoDetalheRetorno;
                
                SPListaPadrao objBanco = objListBanco.Find(base.SPListaPadraoPorValor(objItem.BancoCredito.ToString()));
                e.Row.Cells[6].Text = (objBanco == null ? objItem.BancoCredito.ToString() : objBanco.Titulo.PadLeft(10, ' ').Substring(0, 10).TrimStart(' ')) + "/" + objItem.AgenciaCredito.ToString() + "/" + objItem.ContaCorrente;

                //var lblEstabelecimento = e.Row.FindControl("lblEstabelecimento") as Literal;
                //lblEstabelecimento.Text = objItem.NumeroEstabelecimento.ToString();

                if (objItem.IndicadorRecarga)
                {
                    //Link de detalhes de Recarga de Celular
                    var btnDetalhes = e.Row.FindControl("btnDetalhes") as LinkButton;
                    btnDetalhes.Visible = objItem.IndicadorRecarga;
                    btnDetalhes.CommandArgument = e.Row.RowIndex.ToString();
                }
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {                
                //rodapé
                Servico.PA.ConsultarOrdensCreditoEnviadosAoBancoTotaisRetorno objTotais = (Servico.PA.ConsultarOrdensCreditoEnviadosAoBancoTotaisRetorno)ViewState["Totais"];
                //valor bruto
                e.Row.Cells[0].ColumnSpan = 5;
                e.Row.Cells[5].Text = objTotais.TotalValorCredito.ToString("N2");
                e.Row.Cells.RemoveAt(4);
                e.Row.Cells.RemoveAt(3);
                e.Row.Cells.RemoveAt(2);
                e.Row.Cells.RemoveAt(1);
            }
        }

        protected void grvDados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (String.Compare("detalhar", e.CommandName, true) == 0)
            {
                Int32? index = Convert.ToString(e.CommandArgument).ToInt32Null();
                if (index.HasValue && Registros.Length > index.Value)
                {
                    var dataItem = this.Registros[index.Value] as ConsultarOrdensCreditoEnviadosAoBancoDetalheRetorno;
                    var filtro = ObterBuscarDados();

                    var qsDetalhe = new QueryStringSegura();
                    qsDetalhe["NumeroPv"] = dataItem.NumeroEstabelecimento.ToString();
                    qsDetalhe["NumeroRv"] = dataItem.NumeroResumoVenda.ToString();
                    qsDetalhe["DataPagamento"] = dataItem.DataVencimento.ToString("ddMMyyyyHHmmssfff");

                    String urlLinkDetalhe =
                        base.ObterUrlLinkDetalhe(TipoRelatorio.RecargaCelular, TipoVenda.Credito, qsDetalhe);
                    SPUtility.Redirect(urlLinkDetalhe, SPRedirectFlags.CheckUrl, this.Context);
                }
            }
        }
        
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("pn_default.aspx");
        }
        #endregion Eventos

        #region Metodos
        /// <summary>
        /// Realiza toda a consulta do relatorio de vendas
        /// </summary>
        private void Consultar(BuscarDados buscarDados, int pagina, Int32 qtdRegistrosPorPagina, bool lancarExcecao)
        {

            Servico.PA.StatusRetorno objStatusRetorno;
            Servico.PA.ConsultarOrdensCreditoEnviadosAoBancoRetorno objRetorno = new Servico.PA.ConsultarOrdensCreditoEnviadosAoBancoRetorno();
            try
            {
                using (var contexto = new Comum.ContextoWCF<Servico.PA.RelatorioPagamentosAjustadosClient>())
                {
                    GravarBuscarDados(buscarDados);
                    objRetorno = contexto.Cliente.ConsultarOrdensCreditoEnviadosAoBancoPesquisa(out objStatusRetorno,
                                                                        TradudorEnvioSPParaServico(buscarDados),
                                                                        pagina, qtdRegistrosPorPagina,
                                                                        GuidPesquisa(), GuidUsuario());

                     //trata retorno do serviço
                    if (objStatusRetorno.CodigoRetorno != 0)
                    {
                        if (lancarExcecao)
                            throw new Redecard.PN.Comum.PortalRedecardException(objStatusRetorno.CodigoRetorno, objStatusRetorno.Fonte);
                        
                        base.ExibirPainelExcecao(objStatusRetorno.Fonte, objStatusRetorno.CodigoRetorno);

                        return;
                    }
                    this.Registros = objRetorno.Registros;

                    objPaginacao.QuantidadeTotalRegistros = objRetorno.QuantidadeTotalRegistros;
                    objPaginacao.PaginaAtual = pagina;
                    ViewState["Totais"] = objRetorno.Totais;

                }

                //mostra os resultados
                MostrarResultadoRelatorio(true);

                //preenche a lista de bancos
                objListBanco = base.GetListaSP(Constantes.Extrato_Lista_Banco);

                grvDados.DataSource = this.Registros;
                grvDados.DataBind();

                this.qiValoresConsolidados.QuadroInformacaoItems.Clear();
                this.qiValoresConsolidados.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total pagamentos ajustados no período",
                    Valor = objRetorno.Totais.TotalValorCredito.ToString("C")
                });

                //Verifica os controles que devem estar visíveis
                base.VerificaControlesVisiveis(this.Registros.Length, null, null);
            }
            catch (FaultException<Servico.PA.GeneralFault> ex)
            {
                if (lancarExcecao)
                {
                    throw new Redecard.PN.Comum.PortalRedecardException(ex.Detail.Codigo, ex.Detail.Fonte, ex);
                }
                Redecard.PN.Comum.SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString());
            }
            catch (Exception ex)
            {
                if (lancarExcecao)
                {
                    throw ex;
                }
                Redecard.PN.Comum.SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Traduz o objeto BuscarDados para o objeto esperado pelo serviço
        /// </summary>
        /// <param name="BuscarDados">Modelo.BuscarDados</param>
        /// <returns>O tipo esperado pelo Serviço</returns>
        private Servico.PA.ConsultarOrdensCreditoEnviadosAoBancoEnvio TradudorEnvioSPParaServico(BuscarDados BuscarDados)
        {
            Servico.PA.ConsultarOrdensCreditoEnviadosAoBancoEnvio objEnvio = new Servico.PA.ConsultarOrdensCreditoEnviadosAoBancoEnvio();
            objEnvio.CodigoBandeira = BuscarDados.CodigoBandeira;
            objEnvio.DataFinal = BuscarDados.DataFinal;
            objEnvio.DataInicial = BuscarDados.DataInicial;
            objEnvio.Estabelecimentos = BuscarDados.Estabelecimentos;
            return objEnvio;
        }

        /// <summary>
        /// Mostra os resultados do relatorio
        /// </summary>
        /// <param name="FlagMostrar">True = Mostrar, False = Não Mostrar</param>
        private void MostrarResultadoRelatorio(bool FlagMostrar)
        {
            if (objPaginacao != null)
            {
                objPaginacao.Visible = FlagMostrar;
            }
            divRelatorioValores.Visible = FlagMostrar;
            grvDados.Visible = FlagMostrar;
        }
        #endregion Metodos

        #region [ Implementações ]
        public void Pesquisar(BuscarDados dados)
        {
            objBuscar_onBuscar(dados, new EventArgs());
        }

        public string IdControl
        {
            get { return "PagamentosAjustadosUserControl_ascx"; }
        }

        /// <summary>
        /// Retorna uma tabela HTML com os dados 
        /// </summary>
        public String ObterTabelaExcel(BuscarDados dados, Int32 quantidadeRegistros, Boolean incluirTotalizadores)
        {
            // Chamar método de consulta
            Consultar(dados, 1, quantidadeRegistros, false);

            if(incluirTotalizadores)
                return base.RenderizarControles(true, divRelatorioValores, grvDados);
            else
                return base.RenderizarControles(true, grvDados);
        }
        #endregion
    }
}
