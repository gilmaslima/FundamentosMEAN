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
using System.Globalization;
using System.Linq;
using System.ServiceModel;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ValoresConsolidados
{
    /// <summary>
    /// Turquia - Relatório Valores Consolidados - Totais.
    /// </summary>
    public partial class RelatorioValoresConsolidados : BaseUserControl, IRelatorioHandler
    {
        #region [Propriedades]

        /// <summary>CultureInfo pt-BR</summary>
        private readonly CultureInfo ptBr = CultureInfo.CreateSpecificCulture("pt-BR");

        /// <summary>
        /// Nome da operação.
        /// </summary>
        private String NomeOperacao { get { return "Relatório - Valores de Consolidados de vendas - Geral"; } }

        /// <summary>
        /// Registro de total de vendas por período consolidado.
        /// </summary>
        private TotalVendasPorPeriodoConsolidado RegistroTotalVendas
        {
            get
            {
                return ViewState.Count > 0 && ViewState["Registros"] != null ?
                    (TotalVendasPorPeriodoConsolidado)ViewState["Registros"] : new TotalVendasPorPeriodoConsolidado();
            }
            set { ViewState["Registros"] = value; }
        }

        #endregion

        #region [Eventos]

        /// <summary>
        ///  Page Load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Evento clique o Link Button Detalhar Crédito
        /// </summary>
        /// <param name="sender">Objeto btnDetalharCredito</param>
        /// <param name="e">Objeto com argumentos do evento.</param>
        protected void btnDetalharCredito_Click(object sender, EventArgs e)
        {
            BuscarDados dadosBusca = ObterBuscarDados();
            this.RedirecionarRelatorio(TipoRelatorio.ValoresConsolidadosVendas, TipoVenda.Credito, dadosBusca.DataInicial, dadosBusca.DataFinal, true);
        }


        /// <summary>
        /// Evento clique o Link Button Detalhar Crédito
        /// </summary>
        /// <param name="sender">Objeto btnDetalharCredito</param>
        /// <param name="e">Objeto com argumentos do evento.</param>
        protected void btnDetalharDebito_Click(object sender, EventArgs e)
        {
            BuscarDados dadosBusca = ObterBuscarDados();
            this.RedirecionarRelatorio(TipoRelatorio.ValoresConsolidadosVendas, TipoVenda.Debito, dadosBusca.DataInicial, dadosBusca.DataFinal, true);
        }

        #endregion

        #region [Implementações]

        /// <summary>
        /// Efetua a consulta do relatório
        /// </summary>
        /// <param name="dados">Objeto com os parâmetros da consulta</param>
        public void Pesquisar(BuscarDados dados)
        {
            Consultar(dados);
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
            Consultar(dados);
            String cssClass = divTotais.Attributes["class"];
            divTotais.Attributes["class"] = String.Concat(cssClass, " tabelaDados dados");
            String html = base.RenderizarControles(true, divTotais);
            divTotais.Attributes["class"] = cssClass;
            return html;
        }

        /// <summary>
        /// ID do Controle
        /// </summary>
        public string IdControl
        {
            get { return "RelatorioValoresConsolidados"; }
        }

        #endregion

        #region [Métodos]

        /// <summary>
        /// Efetua a consulta do relatório
        /// </summary>
        /// <param name="dados">Objeto com os parâmetros da consulta</param>
        private void Consultar(BuscarDados dadosBusca)
        {
            using (Logger log = Logger.IniciarLog(NomeOperacao))
            {                
                try
                {                   
                    GravarBuscarDados(dadosBusca);

                    using (var contexto = new ContextoWCF<HisServicoWaExtratoValoresConsolidadosVendasClient>())
                    {
                        log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, GuidPesquisa = GuidPesquisa() });

                        StatusRetorno statusRelatorio;

                        RegistroTotalVendas = contexto.Cliente.ConsultarTotalVendasPorPeriodoConsolidado(
                            out statusRelatorio,
                            dadosBusca.DataInicial,
                            dadosBusca.DataFinal,
                            dadosBusca.Estabelecimentos.ToList());

                        log.GravarLog(EventoLog.RetornoServico, new { RegistroTotalVendas, statusRelatorio });

                        if (statusRelatorio.CodigoRetorno == (Int16)Constantes.CodigoRetorno.NenhumArgumentoEncontrado)
                        {
                            ExibirAviso("Não há movimento para o período informado!", true);
                            return;
                        }
                        if (statusRelatorio.CodigoRetorno != (Int16)Constantes.CodigoRetorno.Ok)
                        {
                            base.ExibirPainelExcecao(statusRelatorio.Fonte, statusRelatorio.CodigoRetorno);
                            return;
                        }

                        CarregarDadosRelatorio();
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
        /// Carrega na tela os dados do relatório.
        /// </summary>
        private void CarregarDadosRelatorio()
        {
            ltrTotalVendasCredito.Text = RegistroTotalVendas.TotalVendasCredito.ToString("C2", ptBr);
            ltrTotalVendasDebito.Text = RegistroTotalVendas.TotalVendasDebito.ToString("C2", ptBr);
            ltrTotalVendasPeriodo.Text = RegistroTotalVendas.TotalVendasPeriodo.ToString("C2", ptBr);

            btnDetalharCredito.Visible = RegistroTotalVendas.TotalVendasCredito > 0;
            btnDetalharDebito.Visible = RegistroTotalVendas.TotalVendasDebito > 0;
        }

        /// <summary>
        /// Exibe a mensagem do quadro de aviso
        /// </summary>
        /// <param name="titulo">Título do aviso.</param>
        /// <param name="mensagem">Mensagem do corpo do aviso.</param>
        /// <param name="esconderRelatorio">Deve esconder a table de relatório?</param>
        private void ExibirAviso(string mensagem, bool esconderRelatorio)
        {
            //Define a mensagem do Quadro de aviso
            QuadroAviso ucQdoAviso = qdoAviso as QuadroAviso;
            if (ucQdoAviso != null)
            {
                ucQdoAviso.Visible = true;
                ucQdoAviso.Mensagem = mensagem;
            }

            divConteudo.Visible = !esconderRelatorio;
            pnlQuadroAviso.Visible = true;
        }

        #endregion
    }
}
