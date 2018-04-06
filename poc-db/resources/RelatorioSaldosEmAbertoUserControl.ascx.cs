using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.SaldosEmAberto;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;


namespace Redecard.PN.Extrato.SharePoint.RelatorioSaldosEmAberto
{
    public partial class RelatorioSaldosEmAbertoUserControl : BaseUserControl, IRelatorioHandler
    {
        #region Atributos
        List<SPListaPadrao> objListBanco;
        public Boolean AdicionaAlttr { get { return ViewState["AdicionaAlttr"] == null ? false : ((Boolean)ViewState["AdicionaAlttr"]) ; } set { ViewState["AdicionaAlttr"] = value; } }
        protected Redecard.PN.Extrato.SharePoint.ControlTemplates.Paginacao objPaginacao;
        protected ucOutrasOpcoes objOutrasOpcoes;
        public string IdControl
        {
            get
            {
                return "RelatorioSaldosEmAbertoUserControl_ascx";
            }
        }
        #endregion
        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            //recupera o controle de Paginação e seta seus eventos e variaveis
            objPaginacao.onPaginacaoChanged += new Redecard.PN.Extrato.SharePoint.ControlTemplates.Paginacao.PaginacaoChanged(objPaginacao_onPaginacaoChanged);
            objPaginacao.RegistrosPorPagina = Convert.ToInt32(ddlRegistroPorPagina.SelectedValue);

        }
        public void Pesquisar(BuscarDados dados)
        {
            objBuscar_onBuscar(dados, new EventArgs());
        }

        protected void objPaginacao_onPaginacaoChanged(int pagina, EventArgs e)
        {
            Consultar(ObterBuscarDados(), pagina, ObterQuantidadeRegistrosPagina(), false);
        }

        protected void objBuscar_onBuscar(BuscarDados Dados, EventArgs e)
        {
            Consultar(Dados, 1, ObterQuantidadeRegistrosPagina(), false);
        }

        protected String objBuscar_onObterConteudoHTMLDownload(BuscarDados buscarDados, Boolean recomporTela)
        {
            Int32 savePaginaAtual = objPaginacao.PaginaAtual;
            Int32 saveTotalPaginas = objPaginacao.TotalPagina;
            Int32 saveQtdRegistrosPagina = ObterQuantidadeRegistrosPagina();

            BuscarDados objEnvio = buscarDados;
            Consultar(objEnvio, 1, MAX_LINHAS_DOWNLOAD, true);

            String result = ObterHTMLControle(this.rptSaldos); // outTabela

            if (recomporTela)
            {
                Consultar(objEnvio, savePaginaAtual, saveQtdRegistrosPagina, true);
            }
            return result;
        }

        private Int32 ObterQuantidadeRegistrosPagina()
        {
            return Convert.ToInt32(ddlRegistroPorPagina.SelectedValue);
        }
        protected void ddlRegistroPorPagina_SelectedIndexChanged(object sender, EventArgs e)
        {
            //esconde os resultas
            MostrarResultadoRelatorio(false);

            Consultar(ObterBuscarDados(), 1, ObterQuantidadeRegistrosPagina(), false);
        }


        protected void rptSaldos_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Repeater rptDetalhe = e.Item.FindControl("rptDetalhe") as Repeater;
                SaldosEmAberto.BaseDetalhe detalhe = (e.Item.DataItem as SaldosEmAberto.BaseDetalhe);
                //List<SaldosEmAberto.TotalBandeiraMesSaldosEmAberto> itensMes = detalhe.TotalBandeiraMes;
                HtmlTableRow trDetalhe = e.Item.FindControl("trDetalhe") as HtmlTableRow;
                HtmlTableRow trTotalBandeiraMes = e.Item.FindControl("trTotalBandeiraMes") as HtmlTableRow;
                HtmlTableRow trTotalMes = e.Item.FindControl("trTotalMes") as HtmlTableRow;

                switch (detalhe.Tipo)
                {
                    case "DT":
                        {

                            SaldosEmAberto.ItemDetalheSaldosEmAberto item = (detalhe as SaldosEmAberto.ItemDetalheSaldosEmAberto);
                            SPListaPadrao objBanco = objListBanco.Find(base.SPListaPadraoPorValor(item.CodigoBanco.ToString()));

                            Label lblMesAnoDetalhe = e.Item.FindControl("lblMesAnoDetalhe") as Label;
                            Label lblBanco = e.Item.FindControl("lblBanco") as Label;
                            Label lblEstabelecimento = e.Item.FindControl("lblEstabelecimento") as Label;
                            Label lblValorBruto = e.Item.FindControl("lblValorBruto") as Label;
                            Label lblValorLiquido = e.Item.FindControl("lblValorLiquido") as Label;
                            HtmlTableCell tdAnoMes = trDetalhe.FindControl("tdAnoMes") as HtmlTableCell;

                            //class='<%# (Container.ItemType == ListItemType.AlternatingItem) ? "alttr" : "" %>'
                            if (item.QuantidadeDetalhe > 0)
                            {
                                tdAnoMes.RowSpan = item.QuantidadeDetalhe;
                                lblMesAnoDetalhe.Text = item.DataReferencia.ToString("MM/yyyy");
                                //tdAnoMes.Attributes.Add("style", "background-color:#fff;");
                                AdicionaAlttr = true;
                            }
                            else
                            {
                                if (AdicionaAlttr)
                                {
                                    trDetalhe.Attributes.Add("class", "alttr");
                                    AdicionaAlttr = false;
                                }
                                else
                                {
                                    AdicionaAlttr = true;
                                }
                                tdAnoMes.Visible = false;
                            }
                            lblBanco.Text = (objBanco == null ? item.CodigoBanco.ToString() : objBanco.Titulo) + "/" + item.CodigoAgencia.ToString() + "/" + item.ContaCorrente;
                            lblEstabelecimento.Text = item.CodigoEstabelecimento.ToString();
                            lblValorBruto.Text = item.ValorBruto.ToString("N2");
                            lblValorLiquido.Text = item.ValorLiquido.ToString("N2");
                            trDetalhe.Visible = true;
                            trTotalBandeiraMes.Visible = false;
                            trTotalMes.Visible = false;
                            break;

                        }
                    case "T1":
                        {
                            SaldosEmAberto.TotalBandeiraMesSaldosEmAberto totalBandeira = (detalhe as SaldosEmAberto.TotalBandeiraMesSaldosEmAberto);

                            Literal ltBandeiraMes = e.Item.FindControl("ltBandeiraMes") as Literal;
                            Label lblTotalMesCartao = e.Item.FindControl("lblTotalMesCartao") as Label;
                            lblTotalMesCartao.Text = totalBandeira.ValorLiquido.ToString("N2");
                            ltBandeiraMes.Text = totalBandeira.DataReferencia.ToString("MM/yyyy") + " " + totalBandeira.DescricaoBandeira;

                            trDetalhe.Visible = false;
                            trTotalBandeiraMes.Visible = true;
                            trTotalMes.Visible = false;
                            break;
                        }
                    case "T2":
                        {
                            SaldosEmAberto.DetalheMesSaldosEmAberto totalMes = (detalhe as SaldosEmAberto.DetalheMesSaldosEmAberto);

                            Label lblTotalMes = e.Item.FindControl("lblTotalMes") as Label;
                            Literal ltMes = e.Item.FindControl("ltMes") as Literal;

                            ltMes.Text = totalMes.DataReferencia.ToString("MM/yyyy");
                            lblTotalMes.Text = totalMes.ValorLiquido.ToString("N2");


                            trDetalhe.Visible = false;
                            trTotalBandeiraMes.Visible = false;
                            trTotalMes.Visible = true;
                            break;
                        }
                }









            }
        }

        /*
        protected void rptDetalhe_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Label lblTotalMesCartao = e.Item.FindControl("lblTotalMesCartao") as Label;
                Literal ltlAnoMes = e.Item.FindControl("ltlAnoMes") as Literal;
                Repeater rptItem = e.Item.FindControl("rptItem") as Repeater;
                SaldosEmAberto.TotalBandeiraMesSaldosEmAberto total = e.Item.DataItem as SaldosEmAberto.TotalBandeiraMesSaldosEmAberto;
                if (!object.ReferenceEquals(total, null))
                {
                    rptItem.DataSource = total.Detalhes;
                    rptItem.DataBind();

                    lblTotalMesCartao.Text = total.ValorLiquido.ToString("N2");
                    ltlAnoMes.Text = total.DataReferencia.ToString("MM/yyyy") + " " + total.DescricaoBandeira;
                }

            }
        }
        protected void rptItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Label lblMesAno = e.Item.FindControl("lblMesAno") as Label;
                Label lblBanco = e.Item.FindControl("lblBanco") as Label;
                Label lblEstabelecimento = e.Item.FindControl("lblEstabelecimento") as Label;
                Label lblValorBruto = e.Item.FindControl("lblValorBruto") as Label;
                Label lblValorLiquido = e.Item.FindControl("lblValorLiquido") as Label;
                HtmlTableCell tdAnoMes = e.Item.FindControl("tdAnoMes") as HtmlTableCell;

                SaldosEmAberto.ItemDetalheSaldosEmAberto item = e.Item.DataItem as SaldosEmAberto.ItemDetalheSaldosEmAberto;
                SPListaPadrao objBanco = objListBanco.Find(base.SPListaPadraoPorValor(item.CodigoBanco.ToString()));


                if (!object.ReferenceEquals(item, null))
                {
                    if (e.Item.ItemIndex == 0)
                    {
                        tdAnoMes.RowSpan = (((sender as Repeater).DataSource) as IList).Count;
                        lblMesAno.Text = item.DataReferencia.ToString("MM/yyyy");
                    }
                    lblBanco.Text = (objBanco == null ? item.CodigoBanco.ToString() : objBanco.Titulo) + "/" + item.CodigoAgencia.ToString() + "/" + item.ContaCorrente;
                    lblEstabelecimento.Text = item.CodigoEstabelecimento.ToString();
                    lblValorBruto.Text = item.ValorBruto.ToString("N2");
                    lblValorLiquido.Text = item.ValorLiquido.ToString("N2");
                }

            }
        }
        
         */

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
        }
        #endregion
        #region Métodos
        /// <summary>
        /// Traduz o objeto de Bandeira para o objeto padrão de Bandeira
        /// </summary>
        /// <param name="Bandeira">Tipo de Bandeira que vem do Serviço</param>
        /// <returns>Lista de Modelo.BuscaPadrao</returns>
        private List<BandeiraPadrao> TradutorBandeiraServicoParaBandeiraPadrao(SaldosEmAberto.TotaisPorBandeiraSaldosEmAberto bandeiras)
        {
            List<BandeiraPadrao> objListBandeira = bandeiras.TotaisBandeiras.Select(item => new BandeiraPadrao() { Bandeira = item.DescricaoBandeira, Valor = item.TotalBandeira.ToString("N2") }).ToList();

            return objListBandeira;
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
            rptSaldos.Visible = FlagMostrar;
        }

        /// <summary>
        /// Realiza toda a consulta do relatorio de saldos em aberto
        /// </summary>
        private void Consultar(BuscarDados buscarDados, int pagina, Int32 qtdRegistrosPorPagina, bool lancarExcecao)
        {
            using (var contexto = new ContextoWCF<SaldosEmAberto.RelatorioSaldosEmAbertoClient>())
            {
                try
                {

                    SaldosEmAberto.StatusRetorno status = new SaldosEmAberto.StatusRetorno();
                    SaldosEmAberto.RetornoConsultaSaldosEmAberto retornoConsulta=null;
                    SaldosEmAberto.DadosConsultaSaldosEmAberto dadosBusca = new SaldosEmAberto.DadosConsultaSaldosEmAberto();
                    dadosBusca.DataInicial = buscarDados.DataInicial;
                    dadosBusca.DataFinal = buscarDados.DataFinal;
                    dadosBusca.Estabelecimentos = buscarDados.Estabelecimentos.ToList();

                    dadosBusca.CodigoSolicitacao = buscarDados.CodigoSolicitacao;

                    Int32 difMes = ((buscarDados.DataFinal.Month - buscarDados.DataInicial.Month) + 12 * (buscarDados.DataFinal.Year - buscarDados.DataInicial.Year)) + 1;
                    GravarBuscarDados(buscarDados);
                    if (difMes > 0)
                    {
                        if (difMes <= 12 && string.IsNullOrEmpty(dadosBusca.CodigoSolicitacao))
                        {
                            retornoConsulta = contexto.Cliente.ConsultarSaldosEmAbertoOnline(out status, dadosBusca, pagina, qtdRegistrosPorPagina, new Guid(), new Guid());
                            if (status.CodigoRetorno != 0 && status.CodigoRetorno != 60)
                            {

                                base.ExibirPainelExcecao("Redecard.PN.Extrato.Servicos.HIS.Retorno", status.CodigoRetorno);
                                return;
                            }

                            spnTotalValorLiquido.InnerText = retornoConsulta.TotalBandeiras.TotalLiquido.ToString("N2");
                            base.MontaBandeira(TradutorBandeiraServicoParaBandeiraPadrao(retornoConsulta.TotalBandeiras), tblBandeira);

                            //preenche a lista de bancos
                            objListBanco = base.GetListaSP(Constantes.Extrato_Lista_Banco);


                            objPaginacao.QuantidadeTotalRegistros = retornoConsulta.QuantidadeTotalRegistros;
                            objPaginacao.PaginaAtual = pagina;

                            //mostra os resultados
                            MostrarResultadoRelatorio(true);

                            rptSaldos.DataSource = retornoConsulta.Detalhe;
                            rptSaldos.DataBind();

                            pnlDados.Visible = true;
                            pnlTotais.Visible = true;
                            //Verifica os controles que devem estar visíveis
                            base.VerificaControlesVisiveis(retornoConsulta.Detalhe.Count, null, null);

                        }
                        else
                        {

                            if (!string.IsNullOrEmpty(dadosBusca.CodigoSolicitacao))
                            {
                                retornoConsulta = contexto.Cliente.ConsultarSaldosEmAbertoVSAM(out status, dadosBusca, pagina, qtdRegistrosPorPagina, new Guid(), new Guid());

                                if (status.CodigoRetorno != 0 && status.CodigoRetorno != 60)
                                {

                                    base.ExibirPainelExcecao("Redecard.PN.Extrato.SaldosEmAberto", status.CodigoRetorno);
                                    return;
                                }

                                spnTotalValorLiquido.InnerText = retornoConsulta.TotalBandeiras.TotalLiquido.ToString("N2");
                                base.MontaBandeira(TradutorBandeiraServicoParaBandeiraPadrao(retornoConsulta.TotalBandeiras), tblBandeira);

                                //preenche a lista de bancos
                                objListBanco = base.GetListaSP(Constantes.Extrato_Lista_Banco);


                                objPaginacao.QuantidadeTotalRegistros = retornoConsulta.QuantidadeTotalRegistros;
                                objPaginacao.PaginaAtual = pagina;

                                //mostra os resultados
                                MostrarResultadoRelatorio(true);

                                rptSaldos.DataSource = retornoConsulta.Detalhe;
                                rptSaldos.DataBind();

                                pnlDados.Visible = true;
                                pnlTotais.Visible = true;
                                //Verifica os controles que devem estar visíveis
                                base.VerificaControlesVisiveis(retornoConsulta.Detalhe.Count, null, null);

                            }
                            else
                            {
                                pnlDados.Visible = false;
                                pnlTotais.Visible = false;

                                int codRetorno = contexto.Cliente.IncluirSolicitacao(out status, dadosBusca);
                                if (codRetorno != 0)
                                {
                                    base.ExibirPainelExcecao("Redecard.PN.Extrato.SaldosEmAberto", status.CodigoRetorno);
                                    return;
                                }

                                //base.ExibirPainelMensagem("Periodo maior que 1(hum) ano. Solicitação enviada com sucesso. O período solicitado estará disponível em até 3 dias úteis.");
                                base.VerificaControlesVisiveis(0, null, "O período solicitado estará disponível em até 3 dias úteis.");

                            }
                        }
                    }
                    if (!object.ReferenceEquals(retornoConsulta, null))
                    {
                        objOutrasOpcoes.CodigoEntidade = buscarDados.Estabelecimentos[0];
                        objOutrasOpcoes.ValorCarta = retornoConsulta.TotalBandeiras.TotalLiquido;
                        objOutrasOpcoes.DataSolicitacao = ((Filtro)this.Parent.FindControl("filtroControl")).RecuperarBuscarDadosDTO().DataInicial;
                        objOutrasOpcoes.DadosBusca = dadosBusca;
                        objOutrasOpcoes.Carregar();
                    }
                }
                catch (FaultException<SaldosEmAberto.GeneralFault> ex)
                {
                    Redecard.PN.Comum.SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }

            }


        }
        public String ObterTabelaExcel(BuscarDados dados, Int32 quantidadeRegistros, Boolean incluirTotalizadores)
        {
            // Chamar método de consulta
            Consultar(dados, 1, quantidadeRegistros, false);

            if(incluirTotalizadores)
                return base.RenderizarControles(true, tblQuadroTotais, rptSaldos);
            else
                return base.RenderizarControles(true, rptSaldos);
        }
        #endregion

    }
}
