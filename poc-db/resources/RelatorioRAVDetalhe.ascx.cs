/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.WAExtratoAntecipacaoRAVServico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.AntecipacaoRAV
{
    public partial class RelatorioRAVDetalhe : BaseUserControl, IRelatorioHandler, IRelatorioCSV
    {
        public string IdControl { get { return "RelatorioRAVDetalhe"; } }
        public String NomeOperacao { get { return "Relatório - Antecipação RAV - Detalhe"; } }
        protected Paginacao objPaginacao;
        protected Totalizadores rcTotalizadores;
        protected TableSize ddlRegistrosPorPagina;        
        private WAExtratoAntecipacaoRAVServico.RAVDetalheTotalizador Totalizador;
        private WAExtratoAntecipacaoRAVServico.RAVDetalhe[] Registros;

        protected void Page_Load(object sender, EventArgs e)
        {            
            objPaginacao.RegistrosPorPagina = ddlRegistrosPorPagina.SelectedSize;            
        }

        protected void ddlRegistrosPorPagina_TamanhoPaginaChanged(Object sender, Int32 tamanhoPagina)
        {
            Consultar(ObterBuscarDados(), 1, tamanhoPagina, objPaginacao.PaginasVirtuais);   
        }

        protected void objPaginacao_CacheTodosRegistros()
        {
            Consultar(ObterBuscarDados(), 1, 0, Int32.MaxValue);
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

                    String tipoConsulta = QS["TipoConsulta"];
                    
                    using (var contexto = new ContextoWCF<HISServicoWA_Extrato_AntecipacaoRAVClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, pagina, tamanhoPagina, paginasVirtuais, registroInicial, qtdRegistrosVirtuais, GuidPesquisa = GuidPesquisa(), GuidTotalizador = GuidPesquisaTotalizador() });

                        StatusRetorno statusRelatorio = null, statusTotalizador = null;
                        WAExtratoAntecipacaoRAVServico.RAVDetalhe[] registros = null;
                        WAExtratoAntecipacaoRAVServico.RAVDetalheTotalizador totalizador = null;

                        if (tipoConsulta == "detalhe")
                        {
                            DateTime dataAntecipacao = QS["DataAntecipacao"].ToDate("ddMMyyyyHHmmssfff");
                            Int32 bandeira = QS["CodigoBandeira"].ToInt32(0);

                            contexto.Cliente.ConsultarRelatorioDetalhe(
                                GuidPesquisaTotalizador(),
                                GuidPesquisa(),                                
                                bandeira,
                                dataAntecipacao,
                                dadosBusca.Estabelecimentos,
                                registroInicial,
                                tamanhoPagina,
                                RAVDetalheTipoRegistro.Todos,
                                ref qtdRegistrosVirtuais,
                                out totalizador,
                                out registros,
                                out statusTotalizador,
                                out statusRelatorio);
                        }
                        else if (tipoConsulta == "verTodos")
                        {
                            Int32[] codigoBandeiras = QS["CodigoBandeiras"].Split(';').Select(bandeira => bandeira.ToInt32(0)).Distinct().ToArray();

                            contexto.Cliente.ConsultarRelatorioDetalheTodos(
                                GuidPesquisaTotalizador(),
                                GuidPesquisa(),                                
                                codigoBandeiras,
                                dadosBusca.DataInicial,
                                dadosBusca.DataFinal,
                                dadosBusca.Estabelecimentos,
                                registroInicial,
                                tamanhoPagina,
                                RAVDetalheTipoRegistro.Todos,
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
                rcTotalizadores.ValorBruto = Totalizador.Totais.TotalValorBruto;
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
                WAExtratoAntecipacaoRAVServico.RAVDetalhe item = e.Item.DataItem as WAExtratoAntecipacaoRAVServico.RAVDetalhe;
                HtmlTableRow trDetalhe = e.Item.FindControl("trDetalhe") as HtmlTableRow;
                HtmlTableRow trAjusteComValor = e.Item.FindControl("trAjusteComValor") as HtmlTableRow;
                HtmlTableRow trAjusteSemValor = e.Item.FindControl("trAjusteSemValor") as HtmlTableRow;
                trDetalhe.Visible = item is WAExtratoAntecipacaoRAVServico.RAVDetalheDT;
                trAjusteComValor.Visible = item is WAExtratoAntecipacaoRAVServico.RAVDetalheA1;
                trAjusteSemValor.Visible = item is WAExtratoAntecipacaoRAVServico.RAVDetalheA2;

                if (item is WAExtratoAntecipacaoRAVServico.RAVDetalheDT)
                {
                    var dataItem = item as WAExtratoAntecipacaoRAVServico.RAVDetalheDT;
                    Literal lblNumeroPV = e.Item.FindControl("lblNumeroPV") as Literal;
                    Literal lblDataVenda = e.Item.FindControl("lblDataVenda") as Literal;
                    Literal lblPrevisaoRecebimento = e.Item.FindControl("lblPrevisaoRecebimento") as Literal;
                    Literal lblDataAntecipacao = e.Item.FindControl("lblDataAntecipacao") as Literal;
                    PlaceHolder phResumoVenda = e.Item.FindControl("phResumoVenda") as PlaceHolder;
                    Literal lblTipoVenda = e.Item.FindControl("lblTipoVenda") as Literal;
                    Literal lblBandeira = e.Item.FindControl("lblBandeira") as Literal;
                    Literal lblQtdVendas = e.Item.FindControl("lblQtdVendas") as Literal;
                    Literal lblValorDisponivel = e.Item.FindControl("lblValorDisponivel") as Literal;
                    Literal lblValorLiquido = e.Item.FindControl("lblValorLiquido") as Literal;
                    Literal lblBanco = e.Item.FindControl("lblBanco") as Literal;
                    Literal lblAgencia = e.Item.FindControl("lblAgencia") as Literal;
                    Literal lblConta = e.Item.FindControl("lblConta") as Literal;
                    Literal lblTipoAntecipacao = e.Item.FindControl("lblTipoAntecipacao") as Literal;

                    lblTipoAntecipacao.Text = this.RetornaAliasProdutoAntecipacao(dataItem.DescricaoCessaoCredito);

                    lblNumeroPV.Text = dataItem.NumeroPV.ToString();
                    lblDataVenda.Text = dataItem.DataApresentacao.ToString("dd/MM/yy");
                    lblPrevisaoRecebimento.Text = dataItem.DataVencimento.ToString("dd/MM/yy");
                    lblDataAntecipacao.Text = dataItem.DataAntecipacao.ToString("dd/MM/yy");
                    phResumoVenda.Controls.Add(base.ObterHyperLinkResumoVenda("C", dataItem.NumeroResumo, dataItem.NumeroPV, dataItem.DataApresentacao));
                    lblTipoVenda.Text = dataItem.DescricaoResumo;
                    lblBandeira.Text = dataItem.Bandeira;
                    lblQtdVendas.Text = dataItem.QuantidadeTransacoesRV.ToString();
                    lblValorDisponivel.Text = dataItem.ValorDisponivelRAV.ToString("N2");
                    lblValorLiquido.Text = dataItem.ValorLiquidoAntecipado.ToString("N2");
                    lblBanco.Text = base.ObterNomeBanco(dataItem.BancoCredito);
                    lblAgencia.Text = dataItem.AgenciaCredito.ToString();
                    lblConta.Text = dataItem.ContaCredito;
                }
                else if (item is WAExtratoAntecipacaoRAVServico.RAVDetalheA1)
                {
                    var dataItem = item as WAExtratoAntecipacaoRAVServico.RAVDetalheA1;

                    Literal lblDescricaoDiarioBandeiraComValor = e.Item.FindControl("lblDescricaoDiarioBandeiraComValor") as Literal;
                    Literal lblTotalValorLiquido = e.Item.FindControl("lblTotalValorLiquido") as Literal;

                    lblDescricaoDiarioBandeiraComValor.Text = dataItem.DescricaoTotalDiarioBandeira;
                    lblTotalValorLiquido.Text = dataItem.TotalValorLiquidoAntecipado.ToString("N2");
                }
                else if (item is WAExtratoAntecipacaoRAVServico.RAVDetalheA2)
                {
                    var dataItem = item as WAExtratoAntecipacaoRAVServico.RAVDetalheA2;

                    Literal lblDescricaoDiarioBandeiraSemValor = e.Item.FindControl("lblDescricaoDiarioBandeiraSemValor") as Literal;
                    lblDescricaoDiarioBandeiraSemValor.Text = dataItem.DescricaoTotalDiarioBandeira;
                }
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                Literal lblTotalValorDisponivel = e.Item.FindControl("lblTotalValorDisponivel") as Literal;
                Literal lblTotalValorLiquido = e.Item.FindControl("lblTotalValorLiquido") as Literal;

                lblTotalValorLiquido.Text = Totalizador.Totais.TotalValorLiquido.ToString("N2");
                lblTotalValorDisponivel.Text = Totalizador.Totais.TotalValorDisponivel.ToString("N2");
            }
        }

        /// <summary>
        /// Retorna alias do campo Produto Antecipação do projeto cessão de crédito
        /// </summary>
        /// <param name="nome">Nome original(mainframe)</param>
        /// <returns>Nome formatado</returns>
        private String RetornaAliasProdutoAntecipacao(String nome)
        {
            if (!String.IsNullOrEmpty(nome))
            {
                switch (nome)
                {
                    case "CESSAO DE CREDITO":
                        return "Cess&atilde;o de Cr&eacute;dito";

                    default:
                        return nome;
                }
            }
            else
                return String.Empty;
        }

        /// <summary>
        /// Gera conteúdo CSV da consulta completa do relatório
        /// </summary>
        /// <param name="dadosBusca">Dados da busca</param>
        /// <param name="funcaoOutput">Função responsável por processar a saída do CSV gerado</param>
        public void GerarConteudoRelatorio(BuscarDados dadosBusca, Action<String> funcaoOutput)
        {
            using (Logger Log = Logger.IniciarLog(NomeOperacao))
            {
                try
                {
                    Log.GravarLog(EventoLog.ChamadaServico, dadosBusca);

                    var statusRelatorio = new StatusRetorno();
                    var statusTotalizador = new StatusRetorno();

                    String tipoConsulta = QS["TipoConsulta"];

                    using (var contexto = new ContextoWCF<HISServicoWA_Extrato_AntecipacaoRAVClient>())
                    {
                        if (String.Compare(tipoConsulta, "detalhe") == 0)
                        {
                            DateTime dataAntecipacao = QS["DataAntecipacao"].ToDate("ddMMyyyyHHmmssfff");
                            Int32 bandeira = QS["CodigoBandeira"].ToInt32(0);

                            //Consulta o totalizador do relatório
                            this.Totalizador = contexto.Cliente.ConsultarDetalheTotalizadores(
                                out statusTotalizador,
                                GuidPesquisaTotalizador(),
                                bandeira,
                                dataAntecipacao,
                                dadosBusca.Estabelecimentos);
                        }
                        else if (String.Compare(tipoConsulta, "verTodos") == 0)
                        {
                            Int32[] codigoBandeiras = QS["CodigoBandeiras"].Split(';')
                                    .Select(bandeira => bandeira.ToInt32(0)).Distinct().ToArray();

                            this.Totalizador = contexto.Cliente.ConsultarDetalheTodosTotalizadores(
                                out statusTotalizador,
                                GuidPesquisaTotalizador(),
                                codigoBandeiras,
                                dadosBusca.DataInicial,
                                dadosBusca.DataFinal,
                                dadosBusca.Estabelecimentos);
                        }
                    }

                    Log.GravarMensagem("Consultou totalizadores", new { tipoConsulta, this.Totalizador });

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

                        using (var contexto = new ContextoWCF<HISServicoWA_Extrato_AntecipacaoRAVClient>())
                        {
                            if (String.Compare(tipoConsulta, "detalhe") == 0)
                            {
                                DateTime dataAntecipacao = QS["DataAntecipacao"].ToDate("ddMMyyyyHHmmssfff");
                                Int32 bandeira = QS["CodigoBandeira"].ToInt32(0);

                                this.Registros = contexto.Cliente.ConsultarDetalhe(
                                    GuidPesquisa(),
                                    registroInicial,
                                    tamanhoPagina,
                                    ref qtdTotalRegistros,
                                    out statusRelatorio,
                                    RAVDetalheTipoRegistro.Todos,
                                    bandeira,
                                    dataAntecipacao,
                                    dadosBusca.Estabelecimentos);
                            }
                            else if (String.Compare(tipoConsulta, "verTodos") == 0)
                            {
                                Int32[] codigoBandeiras = QS["CodigoBandeiras"].Split(';')
                                    .Select(bandeira => bandeira.ToInt32(0)).Distinct().ToArray();

                                this.Registros = contexto.Cliente.ConsultarDetalheTodos(
                                    GuidPesquisa(),
                                    registroInicial,
                                    tamanhoPagina,
                                    ref qtdTotalRegistros,
                                    out statusRelatorio,
                                    RAVDetalheTipoRegistro.Todos,
                                    codigoBandeiras,
                                    dadosBusca.DataInicial,
                                    dadosBusca.DataFinal,
                                    dadosBusca.Estabelecimentos);
                            }
                        }

                        Log.GravarMensagem("Consultou registro inicial: " + registroInicial);

                        //Em caso de erro na consulta da página, cancela consulta de registros
                        if (statusRelatorio == null || statusRelatorio.CodigoRetorno != 0)
                            return;
                        else if (this.Registros != null)
                        {
                            CarregarDadosRelatorio();

                            if (Registros.Length > 0)
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