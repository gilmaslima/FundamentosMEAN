/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.WAExtratoLancamentosFuturosServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.LancamentosFuturos
{
    public partial class RelatorioCreditoDetalhe : BaseUserControl, IRelatorioHandler, IRelatorioCSV
    {
        public string IdControl { get { return "RelatorioCreditoDetalhe"; } }
        public String NomeOperacao { get { return "Relatório - Lançamentos Futuros - Crédito - Detalhe"; } }
        protected Paginacao objPaginacao;
        protected Totalizadores rcTotalizadores;
        protected TableSize ddlRegistrosPorPagina;        
        private CreditoDetalheTotalizador Totalizador;
        private CreditoDetalhe[] Registros;

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
            ConsultarCompleto(dados);

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

                    String tipoConsulta = QS["TipoConsulta"];
                    
                    using (var contexto = new ContextoWCF<HISServicoWA_Extrato_LancamentosFuturosClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, pagina, tamanhoPagina, paginasVirtuais, registroInicial, qtdRegistrosVirtuais, GuidPesquisa = GuidPesquisa(), GuidTotalizador = GuidPesquisaTotalizador() });

                        StatusRetorno statusRelatorio = null, statusTotalizador = null;
                        CreditoDetalhe[] registros = null;
                        CreditoDetalheTotalizador totalizador = null;

                        if (tipoConsulta == "detalhe")
                        {
                            DateTime dataVencimento = QS["DataVencimento"].ToDate("ddMMyyyyHHmmssfff");
                            Int32 codigoBandeira = QS["CodigoBandeira"].ToInt32(0);

                            contexto.Cliente.ConsultarRelatorioCreditoDetalhe(
                                GuidPesquisaTotalizador(),
                                GuidPesquisa(),                                
                                codigoBandeira,
                                dataVencimento,
                                dadosBusca.Estabelecimentos,
                                registroInicial,
                                tamanhoPagina,
                                CreditoDetalheTipoRegistro.Todos,
                                ref qtdRegistrosVirtuais,
                                out totalizador,
                                out registros,
                                out statusTotalizador,
                                out statusRelatorio);
                        }
                        else if (tipoConsulta == "verTodos")
                        {
                            Int32[] codigoBandeiras = QS["CodigoBandeiras"].Split(';').Select(bandeira => bandeira.ToInt32(0)).Distinct().ToArray();

                            contexto.Cliente.ConsultarRelatorioCreditoDetalheTodos(
                                GuidPesquisaTotalizador(),
                                GuidPesquisa(),                                
                                codigoBandeiras,
                                dadosBusca.DataInicial,
                                dadosBusca.DataFinal,
                                dadosBusca.Estabelecimentos,
                                registroInicial,
                                tamanhoPagina,
                                CreditoDetalheTipoRegistro.Todos,
                                ref qtdRegistrosVirtuais,
                                out totalizador,
                                out registros,
                                out statusTotalizador,
                                out statusRelatorio);
                        }

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

        /// <summary>
        /// Consulta o relatório completo para download
        /// </summary>
        /// <param name="dadosBusca">Dados da busca</param>
        private void ConsultarCompleto(BuscarDados dadosBusca)
        {
            using (Logger Log = Logger.IniciarLog(NomeOperacao))
            {
                try
                {
                    Int32 registroInicial = 0;                    
                    Int32 tamanhoPagina = 1000;

                    String tipoConsulta = QS["TipoConsulta"];
                    
                    Log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, registroInicial, GuidPesquisa = GuidPesquisa(), GuidTotalizador = GuidPesquisaTotalizador() });

                    StatusRetorno statusRelatorio = null, statusTotalizador = null;
                    List<CreditoDetalhe> registros = new List<CreditoDetalhe>();
                    CreditoDetalhe[] registrosPagina = null;
                    CreditoDetalheTotalizador totalizador = null;

                    if (tipoConsulta.CompareTo("detalhe") == 0)
                    {
                        DateTime dataVencimento = QS["DataVencimento"].ToDate("ddMMyyyyHHmmssfff");
                        Int32 codigoBandeira = QS["CodigoBandeira"].ToInt32(0);
                        
                        using (var contexto = new ContextoWCF<HISServicoWA_Extrato_LancamentosFuturosClient>())
                        {
                            //Consulta o totalizador do relatório
                            totalizador = contexto.Cliente.ConsultarCreditoDetalheTotalizadores(
                                out statusTotalizador,
                                GuidPesquisaTotalizador(),
                                codigoBandeira,
                                dataVencimento,
                                dadosBusca.Estabelecimentos);
                        }

                        Log.GravarMensagem("Consultou totalizadores", new { totalizador });

                        //Em caso de erro no totalizador, cancela consulta de registros
                        if (statusTotalizador.CodigoRetorno != 0)
                            return;

                        //Consulta completa, paginada no serviço do relatório
                        do
                        {
                            Int32 qtdTotalRegistros = 2000 + registroInicial;

                            Log.GravarMensagem("Consultando registro inicial: " + registroInicial + " - " + qtdTotalRegistros);

                            using (var contexto = new ContextoWCF<HISServicoWA_Extrato_LancamentosFuturosClient>())
                            {
                                registrosPagina = contexto.Cliente.ConsultarCreditoDetalhe(
                                    GuidPesquisa(),
                                    registroInicial,
                                    tamanhoPagina,
                                    ref qtdTotalRegistros,
                                    out statusRelatorio,
                                    CreditoDetalheTipoRegistro.Todos,
                                    codigoBandeira,
                                    dataVencimento,
                                    dadosBusca.Estabelecimentos);
                            }

                            Log.GravarMensagem("Consultou registro inicial: " + registroInicial);
                                
                            //Em caso de erro na consulta da página, cancela consulta de registros
                            if (statusRelatorio == null || statusRelatorio.CodigoRetorno != 0)
                                return;
                            else if(registrosPagina != null)
                            {
                                registros.AddRange(registrosPagina);
                                registroInicial += registrosPagina.Length;
                            }
                        } while (registrosPagina != null && registrosPagina.Length > 0);

                        Log.GravarLog(EventoLog.RetornoServico, new { totalizador, registros, statusTotalizador, statusRelatorio });

                        Registros = registros.ToArray();
                        Totalizador = totalizador;

                        CarregarDadosRelatorio();
                        CarregarTotalizadores();
                    }
                    else if (tipoConsulta.CompareTo("verTodos") == 0)
                    {
                        Int32[] codigoBandeiras = QS["CodigoBandeiras"].Split(';').Select(bandeira => bandeira.ToInt32(0)).Distinct().ToArray();
                            
                        using (var contexto = new ContextoWCF<HISServicoWA_Extrato_LancamentosFuturosClient>())
                        {
                            //Consulta o totalizador do relatório
                            totalizador = contexto.Cliente.ConsultarCreditoDetalheTodosTotalizadores(
                                out statusTotalizador,
                                GuidPesquisaTotalizador(),
                                codigoBandeiras,
                                dadosBusca.Estabelecimentos,
                                dadosBusca.DataInicial,
                                dadosBusca.DataFinal);
                        }

                        Log.GravarMensagem("Consultou totalizadores", new { totalizador });

                        //Em caso de erro no totalizador, cancela consulta de registros
                        if (statusTotalizador.CodigoRetorno != 0)
                            return;

                        //Consulta completa, paginada no serviço do relatório
                        do
                        {
                            Int32 qtdTotalRegistros = 2000 + registroInicial;

                            Log.GravarMensagem("Consultando registro inicial: " + registroInicial + " - " + qtdTotalRegistros);

                            using (var contexto = new ContextoWCF<HISServicoWA_Extrato_LancamentosFuturosClient>())
                            {
                                registrosPagina = contexto.Cliente.ConsultarCreditoDetalheTodos(
                                    GuidPesquisa(),
                                    registroInicial,
                                    tamanhoPagina,
                                    ref qtdTotalRegistros,
                                    out statusRelatorio,
                                    CreditoDetalheTipoRegistro.Todos,
                                    codigoBandeiras,
                                    dadosBusca.DataInicial,
                                    dadosBusca.DataFinal,
                                    dadosBusca.Estabelecimentos);
                            }

                            Log.GravarMensagem("Consultou registro inicial: " + registroInicial);

                            //Em caso de erro na consulta da página, cancela consulta de registros
                            if (statusRelatorio == null || statusRelatorio.CodigoRetorno != 0)
                                return;
                            else if (registrosPagina != null)
                            {
                                registros.AddRange(registrosPagina);
                                registroInicial += registrosPagina.Length;
                            }
                        } while (registrosPagina != null && registrosPagina.Length > 0);

                        Log.GravarLog(EventoLog.RetornoServico, new { totalizador, registros, statusTotalizador, statusRelatorio });

                        Registros = registros.ToArray();
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
                var item = (CreditoDetalhe)e.Item.DataItem;
                HtmlTableRow trDetalhe = e.Item.FindControl("trDetalhe") as HtmlTableRow;
                HtmlTableRow trAjusteComValor = e.Item.FindControl("trAjusteComValor") as HtmlTableRow;
                HtmlTableRow trAjusteSemValor = e.Item.FindControl("trAjusteSemValor") as HtmlTableRow;
                trDetalhe.Visible = item is CreditoDetalheDT;
                trAjusteComValor.Visible = item is CreditoDetalheA1;
                trAjusteSemValor.Visible = item is CreditoDetalheA2;

                if (item is CreditoDetalheDT)
                {
                    var dataItem = item as CreditoDetalheDT;

                    Literal lblNumeroEstabelecimento = (Literal)trDetalhe.FindControl("lblNumeroEstabelecimento");
                    Literal lblDataApresentacao = (Literal)trDetalhe.FindControl("lblDataApresentacao");
                    Literal lblDataVencimento = (Literal)trDetalhe.FindControl("lblDataVencimento");
                    Literal lblPrazoRecebimento = (Literal)trDetalhe.FindControl("lblPrazoRecebimento");
                    PlaceHolder phResumoVendas = (PlaceHolder)trDetalhe.FindControl("phResumoVendas");
                    Literal lblQtdVendas = (Literal)trDetalhe.FindControl("lblQtdVendas");
                    Literal lblStatusResumo = (Literal)trDetalhe.FindControl("lblStatusResumo");
                    Literal lblTipoVenda = (Literal)trDetalhe.FindControl("lblTipoVenda");
                    Literal lblBandeira = (Literal)trDetalhe.FindControl("lblBandeira");
                    Literal lblValorBruto = (Literal)trDetalhe.FindControl("lblValorBruto");
                    Literal lblValorDescontado = (Literal)trDetalhe.FindControl("lblValorDescontado");
                    Literal lblValorLiquido = (Literal)trDetalhe.FindControl("lblValorLiquido");

                    lblNumeroEstabelecimento.Text = dataItem.NumeroPV.ToString();
                    lblDataApresentacao.Text = dataItem.DataVenda.ToString("dd/MM/yy");
                    lblDataVencimento.Text = dataItem.DataVencimento.ToString("dd/MM/yy");
                    lblPrazoRecebimento.Text = dataItem.PrazoRecebimento + " dias";
                    phResumoVendas.Controls.Add(base.ObterHyperLinkResumoVenda("C", dataItem.NumeroResumo, dataItem.NumeroPV, dataItem.DataVenda));
                    lblQtdVendas.Text = dataItem.QuantidadeTransacoesRV.ToString();
                    lblStatusResumo.Text = dataItem.StatusOc;
                    lblTipoVenda.Text = dataItem.DescricaoResumo;
                    lblBandeira.Text = dataItem.Bandeira;
                    lblValorBruto.Text = dataItem.ValorApresentacaoBruto.ToString("N2");
                    lblValorLiquido.Text = dataItem.ValorLiquido.ToString("N2");
                    lblValorDescontado.Text = dataItem.ValorDesconto.ToString("N2");
                }
                else if (item is CreditoDetalheA1)
                {
                    var dataItem = item as CreditoDetalheA1;

                    Literal lblDescricaoComValor = (Literal)trAjusteComValor.FindControl("lblDescricaoComValor");
                    Literal lblTotalValorLiquido = (Literal)trAjusteComValor.FindControl("lblTotalValorLiquido");

                    lblDescricaoComValor.Text = dataItem.DescricaoCompensacao;
                    lblTotalValorLiquido.Text = dataItem.TotalValorLiquido.ToString("N2");
                }
                else if (item is CreditoDetalheA2)
                {
                    var dataItem = item as CreditoDetalheA2;

                    Literal lblDescricaoSemValor = (Literal)trAjusteSemValor.FindControl("lblDescricaoSemValor");

                    lblDescricaoSemValor.Text = dataItem.DescricaoCompensacao;
                }
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                Literal lblTotalValorBruto = e.Item.FindControl("lblTotalValorBruto") as Literal;
                Literal lblTotalValorDesconto = e.Item.FindControl("lblTotalValorDesconto") as Literal;
                Literal lblTotalValorLiquido = e.Item.FindControl("lblTotalValorLiquido") as Literal;

                lblTotalValorBruto.Text = Totalizador.Totais.TotalValorBrutoVenda.ToString("N2");
                lblTotalValorDesconto.Text = Totalizador.Totais.TotalValorDescontado.ToString("N2");
                lblTotalValorLiquido.Text = Totalizador.Totais.TotalValorLiquido.ToString("N2");                
            }
        }

        public void GerarConteudoRelatorio(BuscarDados dadosBusca, Action<String> funcaoOutput)
        {
            using (Logger Log = Logger.IniciarLog(NomeOperacao))
            {
                try
                {                    
                    String tipoConsulta = QS["TipoConsulta"];

                    Log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca });

                    StatusRetorno statusRelatorio = null, statusTotalizador = null;

                    if (tipoConsulta.CompareTo("detalhe") == 0)
                    {
                        DateTime dataVencimento = QS["DataVencimento"].ToDate("ddMMyyyyHHmmssfff");
                        Int32 codigoBandeira = QS["CodigoBandeira"].ToInt32(0);

                        using (var contexto = new ContextoWCF<HISServicoWA_Extrato_LancamentosFuturosClient>())
                        {
                            //Consulta o totalizador do relatório
                            this.Totalizador = contexto.Cliente.ConsultarCreditoDetalheTotalizadores(
                                out statusTotalizador,
                                GuidPesquisaTotalizador(),
                                codigoBandeira,
                                dataVencimento,
                                dadosBusca.Estabelecimentos);
                        }

                        Log.GravarMensagem("Consultou totalizadores", new { this.Totalizador });

                        //Em caso de erro no totalizador, cancela consulta de registros
                        if (statusTotalizador.CodigoRetorno != 0)
                            return;
                        else //Carrega totalizadores
                            CarregarTotalizadores();

                        Int32 registroInicial = 0;
                        Int32 tamanhoPagina = 500;
                        String linhaFooter = String.Empty;
                        String linhaHeader = String.Empty;

                        //Consulta completa, paginada no serviço do relatório
                        do
                        {
                            Int32 qtdTotalRegistros = 1000 + registroInicial;

                            Log.GravarMensagem("Consultando registro inicial: " + registroInicial + " - " + qtdTotalRegistros);

                            using (var contexto = new ContextoWCF<HISServicoWA_Extrato_LancamentosFuturosClient>())
                            {
                                this.Registros = contexto.Cliente.ConsultarCreditoDetalhe(
                                    GuidPesquisa(),
                                    registroInicial,
                                    tamanhoPagina,
                                    ref qtdTotalRegistros,
                                    out statusRelatorio,
                                    CreditoDetalheTipoRegistro.Todos,
                                    codigoBandeira,
                                    dataVencimento,
                                    dadosBusca.Estabelecimentos);
                            }

                            Log.GravarMensagem("Consultou registro inicial: " + registroInicial);

                            //Em caso de erro na consulta da página, cancela consulta de registros
                            if (statusRelatorio == null || statusRelatorio.CodigoRetorno != 0)
                                return;
                            else if (this.Registros != null)
                            {
                                CarregarDadosRelatorio();

                                if (this.Registros.Length > 0)
                                {
                                    //Gera HTML e CSV do conteúdo da página
                                    String html = base.RenderizarControles(true, rptDados);
                                    String csv = CSVExporter.GerarCSV(html, "\t");

                                    List<String> linhas = csv.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
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

                                registroInicial += this.Registros.Length;
                            }
                        } while (this.Registros != null && this.Registros.Length > 0);
                    }
                    else if (tipoConsulta.CompareTo("verTodos") == 0)
                    {
                        Int32[] codigoBandeiras = QS["CodigoBandeiras"].Split(';').Select(bandeira => bandeira.ToInt32(0)).Distinct().ToArray();

                        using (var contexto = new ContextoWCF<HISServicoWA_Extrato_LancamentosFuturosClient>())
                        {
                            //Consulta o totalizador do relatório
                            this.Totalizador = contexto.Cliente.ConsultarCreditoDetalheTodosTotalizadores(
                                out statusTotalizador,
                                GuidPesquisaTotalizador(),
                                codigoBandeiras,
                                dadosBusca.Estabelecimentos,
                                dadosBusca.DataInicial,
                                dadosBusca.DataFinal);
                        }

                        Log.GravarMensagem("Consultou totalizadores", new { this.Totalizador });

                        //Em caso de erro no totalizador, cancela consulta de registros
                        if (statusTotalizador.CodigoRetorno != 0)
                            return;
                        else //Carrega os totalizadores
                            CarregarTotalizadores();

                        Int32 registroInicial = 0;
                        Int32 tamanhoPagina = 500;
                        String linhaFooter = String.Empty;
                        String linhaHeader = String.Empty;

                        //Consulta completa, paginada no serviço do relatório
                        do
                        {
                            Int32 qtdTotalRegistros = 1000 + registroInicial;

                            Log.GravarMensagem("Consultando registro inicial: " + registroInicial + " - " + qtdTotalRegistros);

                            using (var contexto = new ContextoWCF<HISServicoWA_Extrato_LancamentosFuturosClient>())
                            {
                                this.Registros = contexto.Cliente.ConsultarCreditoDetalheTodos(
                                    GuidPesquisa(),
                                    registroInicial,
                                    tamanhoPagina,
                                    ref qtdTotalRegistros,
                                    out statusRelatorio,
                                    CreditoDetalheTipoRegistro.Todos,
                                    codigoBandeiras,
                                    dadosBusca.DataInicial,
                                    dadosBusca.DataFinal,
                                    dadosBusca.Estabelecimentos);
                            }

                            Log.GravarMensagem("Consultou registro inicial: " + registroInicial);

                            //Em caso de erro na consulta da página, cancela consulta de registros
                            if (statusRelatorio == null || statusRelatorio.CodigoRetorno != 0)
                                return;
                            else if (this.Registros != null)
                            {
                                CarregarDadosRelatorio();

                                if (this.Registros.Length > 0)
                                {
                                    //Gera HTML e CSV do conteúdo da página
                                    String html = base.RenderizarControles(true, rptDados);
                                    String csv = CSVExporter.GerarCSV(html, "\t");

                                    List<String> linhas = csv.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
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

                                registroInicial += this.Registros.Length;
                            }
                        } while (this.Registros != null && this.Registros.Length > 0);                        
                    }

                    Log.GravarLog(EventoLog.RetornoServico, new { statusTotalizador, statusRelatorio });
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
    }
}