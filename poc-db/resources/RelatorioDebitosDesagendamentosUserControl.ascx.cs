using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.ControlTemplates;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;
using System;
using System.Linq;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.DebitosDesagendamentos.RelatorioDebitosDesagendamentos
{
    public partial class RelatorioDebitosDesagendamentosUserControl : BaseUserControl, IRelatorioHandler
    {
        #region Atributos e Propriedades
        protected Paginacao objPaginacao;

        #endregion

        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            //recupera o controle de Paginação e seta seus eventos e variaveis
            objPaginacao.onPaginacaoChanged += new Paginacao.PaginacaoChanged(objPaginacao_onPaginacaoChanged);
            objPaginacao.RegistrosPorPagina = ObterQuantidadeRegistrosPagina();
        }

        private Int32 ObterQuantidadeRegistrosPagina()
        {
            return ddlRegistroPorPagina.SelectedSize;
        }

        protected void objPaginacao_onPaginacaoChanged(int pagina, EventArgs e)
        {
            String tipoPesquisa = ViewState["tipoPesquisa"] as String;
            String chavePesquisa = ViewState["chavePesquisa"] as String;
            ConsultarDetalhe(ObterBuscarDados(), pagina, ObterQuantidadeRegistrosPagina(), false, tipoPesquisa, chavePesquisa);
        }

        protected void objBuscar_onBuscar(BuscarDados Dados, EventArgs e)
        {
            ViewState["tipoPesquisa"] = null;
            Consultar(Dados, 1, ObterQuantidadeRegistrosPagina(), false);
            Session.Remove("DEBITOS_DESAGENDAMENTO_TIPOPESQUISA");
        }

        protected String objBuscar_onObterConteudoHTMLDownload(BuscarDados buscarDados, Boolean recomporTela)
        {
            if (ViewState["tipoPesquisa"] != null)
            {
                Int32 savePaginaAtual = objPaginacao.PaginaAtual;
                Int32 saveTotalPaginas = objPaginacao.TotalPagina;
                Int32 saveQtdRegistrosPagina = ObterQuantidadeRegistrosPagina();
                String tipoPesquisa = ViewState["tipoPesquisa"] as String;
                String chavePesquisa = ViewState["tipoPesquisa"] as String;

                ConsultarDetalhe(buscarDados, 1, MAX_LINHAS_DOWNLOAD, true, tipoPesquisa, chavePesquisa);

                String result = ObterHTMLControle(this.grvDados);

                if (recomporTela)
                {
                    Consultar(buscarDados, savePaginaAtual, saveQtdRegistrosPagina, true);
                }
                return result;
            }
            else
            {
                return ObterHTMLControle(null);
            }
        }

        protected void ddlRegistroPorPagina_SelectedIndexChanged(Object sender, Int32 selectedSize)
        {
            String tipoPesquisa = ViewState["tipoPesquisa"] as String;
            String chavePesquisa = ViewState["chavePesquisa"] as String;
            ConsultarDetalhe(ObterBuscarDados(), 1, selectedSize, false, tipoPesquisa, chavePesquisa);
        }

        protected void grvDados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Servico.DD.ConsultarDetalhamentoDebitosDetalheRetorno item = (Servico.DD.ConsultarDetalhamentoDebitosDetalheRetorno)e.Row.DataItem;

                Redecard.PN.Comum.QueryStringSegura queryString = new Redecard.PN.Comum.QueryStringSegura();
                queryString["numeroEstabelecimento"] = item.EstabelecimentoOrigem.ToString();
                queryString["dataPesquisa"] = item.DataInclusao.ToString("dd/MM/yyyy");
                queryString["timestamp"] = item.Timestamp.ToString();
                queryString["numeroDebito"] = item.NumeroDebito.ToString();
                queryString["tipoPesquisa"] = (string)ViewState["tipoPesquisa"];

                string motivoDebito = this.GetTituloMotivoCreditoDebitoCustomizado(item.MotivoDebito, tituloDefault: item.MotivoDebito);

                if (item.IndicadorDesagendamento == "S")
                {
                    motivoDebito += "*";
                }

                HtmlAnchor objLink = new HtmlAnchor();
                //objLink.Attributes.Add("onclick", "exibirMotivoDebito(\'" + Server.HtmlEncode(queryString.ToString()) + "\');return false;");
                objLink.InnerText = motivoDebito;
                objLink.Attributes["title"] = this.GetDescritivoMotivoCreditoDebitoCustomizado(item.MotivoDebito, descritivoDefault: motivoDebito);
                objLink.HRef = LinkMotivo(queryString.ToString());
                objLink.Attributes.Add("onclick", "blockUI();");

                e.Row.Cells[2].Controls.Add(objLink);

                if (item.DataPagamento == DateTime.MinValue)
                {
                    e.Row.Cells[5].Text = "-";
                }
            }

            if (e.Row.RowType == DataControlRowType.Footer)
            {
                // total no período
                e.Row.Cells[0].Text = "total no período";

                Servico.DD.ConsultarDetalhamentoDebitosTotaisRetorno totais = (Servico.DD.ConsultarDetalhamentoDebitosTotaisRetorno)ViewState["Totais"];

                // valor devido
                e.Row.Cells[6].Text = totais.TotalValorDevido.ToString("N2");
                // valor compensado
                e.Row.Cells[7].Text = totais.TotalValorCompensado.ToString("N2");

                e.Row.Cells[0].ColumnSpan = 6;
                e.Row.Cells.RemoveAt(5);
                e.Row.Cells.RemoveAt(4);
                e.Row.Cells.RemoveAt(3);
                e.Row.Cells.RemoveAt(2);
                e.Row.Cells.RemoveAt(1);
            }
        }

        /// <summary>
        /// Gera o link de Detalhamento de Motivo
        /// </summary>
        private String LinkMotivo(String urlDados)
        {
            try
            {
                if (SPContext.Current == null)
                {
                    String url = Request.Url.ToString();
                    url = url.Substring(0, url.LastIndexOf("/"));

                    using (SPSite site = new SPSite(url))
                    using (SPWeb web = site.OpenWeb())
                    {
                        return web.Url + string.Format("/Paginas/pn_MotivoDebitoDetalhe.aspx?dados={0}", urlDados);
                    }
                }
                else
                    return SPContext.Current.Web.Url +
                        string.Format("/Paginas/pn_MotivoDebitoDetalhe.aspx?dados={0}", urlDados);
            }
            catch (Exception ex)
            {
                Redecard.PN.Comum.SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return "#";
            }
        }


        protected void btnDetalharCompensado_Click(object sender, EventArgs e)
        {
            String tipoPesquisa = "L";
            ViewState["tipoPesquisa"] = tipoPesquisa;
            ViewState["chavePesquisa"] = ((Button)sender).CommandArgument;
            //btnVoltarTotalizador.Visible = false;
            //btnVoltar.Visible = true;
            informativoMotivoDebito.Visible = true;
            ConsultarDetalhe(ObterBuscarDados(), 1, ObterQuantidadeRegistrosPagina(), false, tipoPesquisa, ((Button)sender).CommandArgument);
            ddlRegistroPorPagina.Visible = true;
        }

        protected void btnDetalharPendentes_Click(object sender, EventArgs e)
        {
            String tipoPesquisa = "P";
            ViewState["tipoPesquisa"] = tipoPesquisa;
            ViewState["chavePesquisa"] = ((Button)sender).CommandArgument;
            //btnVoltarTotalizador.Visible = false;
            //btnVoltar.Visible = true;
            informativoMotivoDebito.Visible = true;
            ConsultarDetalhe(ObterBuscarDados(), 1, ObterQuantidadeRegistrosPagina(), false, tipoPesquisa, ((Button)sender).CommandArgument);
            ddlRegistroPorPagina.Visible = true;
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            /*
            PaginaVisitada objPagina = base.GetPaginaVisitadaRedirecionar();
            if (objPagina.Nome == string.Empty)
            {
                return;
            }
            SPUtility.Redirect(objPagina.Nome, SPRedirectFlags.CheckUrl, this.Context);*/

            //Se origem emissores redireciona para a págian default do subsite
            if (OrigemEmissores.Equals("S"))
                SPUtility.Redirect(SPContext.Current.Web.Url, SPRedirectFlags.CheckUrl, System.Web.HttpContext.Current);
            else
                Response.Redirect("pn_default.aspx");

        }
        #endregion

        #region Métodos
        /// <summary>
        /// Realiza toda a consulta do relatorio de vendas
        /// </summary>
        private void Consultar(BuscarDados buscarDados, int pagina, Int32 qtdRegistrosPorPagina, bool lancarExcecao)
        {
            Servico.DD.StatusRetorno objStatusRetorno;
            Servico.DD.ConsultarConsolidadoDebitosEDesagendamentoRetorno objRetorno;

            try
            {
                using (var contexto = new Comum.ContextoWCF<Servico.DD.RelatorioDebitosDesagendamentosClient>())
                {
                    GravarBuscarDados(buscarDados);
                    objRetorno = contexto.Cliente.ConsultarConsolidadoDebitosEDesagendamentoPesquisa(out objStatusRetorno, TradudorEnvioSPParaServico(buscarDados));

                    if (objStatusRetorno.CodigoRetorno != 0)
                    {
                        if (lancarExcecao)
                        {
                            throw new Redecard.PN.Comum.PortalRedecardException(objStatusRetorno.CodigoRetorno, objStatusRetorno.Fonte);
                        }

                        base.ExibirPainelExcecao(objStatusRetorno.Fonte, objStatusRetorno.CodigoRetorno);

                        return;
                    }
                }

                //mostra os resultados
                divRelatorioValores.Visible = true;

                // Valores Consolidados Pendentes
                this.qiValoresConsolidadosPendentes.QuadroInformacaoItems.Clear();
                this.qiValoresConsolidadosPendentes.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total de valores devidos",
                    Valor = objRetorno.ValorPendenteDebito.ToString("C")
                });
                this.qiValoresConsolidadosPendentes.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total de valores compensados",
                    Valor = objRetorno.ValorPendenteLiquido.ToString("C")
                });
                this.qiValoresConsolidadosPendentes.CommandArgument = objRetorno.ChavePesquisa;

                // Valores Consolidados Compensados
                this.qiValoresConsolidadosCompensados.QuadroInformacaoItems.Clear();
                this.qiValoresConsolidadosCompensados.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total de valores devidos",
                    Valor = objRetorno.ValorLiquidadoDebito.ToString("C")
                });
                this.qiValoresConsolidadosCompensados.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total de valores compensados",
                    Valor = objRetorno.ValorLiquidadoLiquido.ToString("C")
                });
                this.qiValoresConsolidadosCompensados.CommandArgument = objRetorno.ChavePesquisa;
            }
            catch (FaultException<Servico.DD.GeneralFault> ex)
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
        /// Realiza a busca dos detlahes para prrencher o grid
        /// </summary>
        /// <param name="BuscarDados"></param>
        private void ConsultarDetalhe(BuscarDados buscarDados, int pagina, Int32 qtdRegistrosPorPagina, bool lancarExcecao, String tipoPesquisa, String chavePesquisa)
        {
            //utilizado na exportação/download. 
            //TODO: rever lógica de montagem da tela para não ser necessária a utilização de Sessão
            Session["DEBITOS_DESAGENDAMENTO_TIPOPESQUISA"] = tipoPesquisa;
            Session["DEBITOS_DESAGENDAMENTO_CHAVEPESQUISA"] = chavePesquisa;

            MostrarResultadoRelatorio(false);

            if (string.IsNullOrEmpty(tipoPesquisa))
            {
                return;
            }

            Servico.DD.StatusRetorno objStatusRetorno;

            Servico.DD.ConsultarDetalhamentoDebitosEnvio objEnvio = new Servico.DD.ConsultarDetalhamentoDebitosEnvio();
            objEnvio.DataInicial = buscarDados.DataInicial;
            objEnvio.DataFinal = buscarDados.DataFinal;
            objEnvio.TipoPesquisa = tipoPesquisa;
            objEnvio.ChavePesquisa = chavePesquisa;
            objEnvio.CodigoBandeira = buscarDados.CodigoBandeira;
            objEnvio.Estabelecimentos = buscarDados.Estabelecimentos;

            //Configura qual versão mainframe será chamada para consulta do relatório
            //ISFx: novos programas - Grandes Consultas; ISDx: programas antigos
            if (ConfiguracaoVersao.VersaoGrandesConsultas() == 1)
                objEnvio.Versao = Servico.DD.VersaoDebitoDesagendamento.ISD;
            else
                objEnvio.Versao = Servico.DD.VersaoDebitoDesagendamento.ISF;

            Servico.DD.ConsultarDetalhamentoDebitosRetorno objRetorno;

            try
            {
                Servico.DD.ConsultarDetalhamentoDebitosDetalheRetorno[] objDetalheRetorno;

                using (var contexto = new Comum.ContextoWCF<Servico.DD.RelatorioDebitosDesagendamentosClient>())
                {
                    GravarBuscarDados(buscarDados);

                    objRetorno = contexto.Cliente.ConsultarDetalhamentoDebitosPesquisa(out objStatusRetorno,
                                                                        objEnvio,
                                                                        pagina, qtdRegistrosPorPagina,
                                                                        GuidPesquisa(), GuidUsuario());

                    if (objStatusRetorno.CodigoRetorno != 0)
                    {
                        if (lancarExcecao)
                        {
                            throw new Redecard.PN.Comum.PortalRedecardException(objStatusRetorno.CodigoRetorno, objStatusRetorno.Fonte);
                        }
                        base.ExibirPainelExcecao(objStatusRetorno.Fonte, objStatusRetorno.CodigoRetorno);
                        return;
                    }

                    objDetalheRetorno = objRetorno.Registros;

                    objPaginacao.QuantidadeTotalRegistros = objRetorno.QuantidadeTotalRegistros;
                    objPaginacao.PaginaAtual = pagina;

                }

                ViewState["Totais"] = objRetorno.Totais;

                //mostra os resultados
                MostrarResultadoRelatorio(true);

                grvDados.DataSource = objDetalheRetorno;
                grvDados.DataBind();

                //Define quando mostrar os controles
                ddlRegistroPorPagina.Visible = objDetalheRetorno.Length > 0;
                objPaginacao.Visible = objDetalheRetorno.Length > 0;

                //Verifica os controles que devem estar visíveis
                base.VerificaControlesVisiveis(objDetalheRetorno.Length, null, null);
            }
            catch (FaultException<Servico.DD.GeneralFault> ex)
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
        private Servico.DD.ConsultarConsolidadoDebitosEDesagendamentoEnvio TradudorEnvioSPParaServico(BuscarDados BuscarDados)
        {
            Servico.DD.ConsultarConsolidadoDebitosEDesagendamentoEnvio objEnvio = new Servico.DD.ConsultarConsolidadoDebitosEDesagendamentoEnvio();
            objEnvio.DataInicial = BuscarDados.DataInicial;
            objEnvio.DataFinal = BuscarDados.DataFinal;
            objEnvio.Estabelecimentos = BuscarDados.Estabelecimentos;

            //Configura qual versão mainframe será chamada para consulta do relatório
            //ISFx: novos programas - Grandes Consultas; ISDx: programas antigos
            if (ConfiguracaoVersao.VersaoGrandesConsultas() == 1)
                objEnvio.Versao = Servico.DD.VersaoDebitoDesagendamento.ISD;
            else
                objEnvio.Versao = Servico.DD.VersaoDebitoDesagendamento.ISF;

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
            ddlRegistroPorPagina.Visible = FlagMostrar;
        }
        #endregion Metodos

        #region [ Implementações ]
        public void Pesquisar(BuscarDados dados)
        {
            objBuscar_onBuscar(dados, new EventArgs());
        }

        public string IdControl
        {
            get { return "RelatorioDebitosDesagendamentosUserControl_ascx"; }
        }
        #endregion

        /// <summary>
        /// Retorna uma tabela HTML com os dados 
        /// </summary>        
        public String ObterTabelaExcel(BuscarDados dados, Int32 quantidadeRegistros, Boolean incluirTotalizadores)
        {
            // Chamar método de consulta
            Consultar(dados, 1, quantidadeRegistros, false);

            String tipoPesquisa = Session["DEBITOS_DESAGENDAMENTO_TIPOPESQUISA"] as String;
            String chavePesquisa = Session["DEBITOS_DESAGENDAMENTO_CHAVEPESQUISA"] as String;
            Session.Remove("DEBITOS_DESAGENDAMENTO_TIPOPESQUISA");
            Session.Remove("DEBITOS_DESAGENDAMENTO_CHAVEPESQUISA");
            if (!String.IsNullOrEmpty(tipoPesquisa))
                ConsultarDetalhe(dados, 1, quantidadeRegistros, false, tipoPesquisa, chavePesquisa);

            //oculta botões e controles cuja renderização é desnecessária
            var controles = new Control[] {
                Utils.FindControlRecursive(this.qiValoresConsolidadosPendentes, "btnAcao"),
                Utils.FindControlRecursive(this.qiValoresConsolidadosCompensados, "btnAcao")
            }.Where(controle => controle != null).ToList();
            controles.ForEach(controle => controle.Visible = false);

            if(incluirTotalizadores)
                return base.RenderizarControles(true, divRelatorioValores, grvDados);                
            else
                return base.RenderizarControles(true, grvDados);
        }

    }
}
