/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.WAExtratoAntecipacaoRAVServico;
using System;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.AntecipacaoRAV
{
    public partial class RelatorioRAV : BaseUserControl, IRelatorioHandler
    {
        public string IdControl { get { return "RelatorioRAV"; } }
        public String NomeOperacao { get { return "Relatório - Antecipação RAV"; } }
        protected Paginacao objPaginacao;
        protected Totalizadores rcTotalizadores;
        protected TableSize ddlRegistrosPorPagina;
        private WAExtratoAntecipacaoRAVServico.RAVTotalizador Totalizador { 
            get { return (WAExtratoAntecipacaoRAVServico.RAVTotalizador) ViewState["Totalizador"]; } 
            set { ViewState["Totalizador"] = value; }
        }
        private WAExtratoAntecipacaoRAVServico.RAV[] Registros {
            get { return (WAExtratoAntecipacaoRAVServico.RAV[]) ViewState["Registros"]; }
            set { ViewState["Registros"] = value; }
        }

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

            if (exibirTotalizadores)
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

                    using (var contexto = new ContextoWCF<HISServicoWA_Extrato_AntecipacaoRAVClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, pagina, tamanhoPagina, paginasVirtuais, registroInicial, qtdRegistrosVirtuais, GuidPesquisa = GuidPesquisa(), GuidTotalizador = GuidPesquisaTotalizador() });

                        StatusRetorno statusRelatorio, statusTotalizador;
                        WAExtratoAntecipacaoRAVServico.RAV[] registros;
                        WAExtratoAntecipacaoRAVServico.RAVTotalizador totalizador;

                        contexto.Cliente.ConsultarRelatorio(
                            GuidPesquisaTotalizador(),
                            GuidPesquisa(),                            
                            dadosBusca.CodigoBandeira,
                            dadosBusca.DataInicial,
                            dadosBusca.DataFinal,
                            dadosBusca.Estabelecimentos,
                            registroInicial,
                            tamanhoPagina,
                            ref qtdRegistrosVirtuais,
                            out totalizador,
                            out registros,
                            out statusTotalizador,
                            out statusRelatorio);

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
                rcTotalizadores.ValorLiquido = Totalizador.Totais.TotalValorLiquido;

            if (Totalizador.Valores != null)
                rcTotalizadores.Bandeiras = Totalizador.Valores.Select(bandeira =>
                    new Totalizadores.Bandeira(bandeira.TipoBandeira, bandeira.ValorLiquido)).ToList();

            rcTotalizadores.Atualizar();
        }

        protected void rptDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                WAExtratoAntecipacaoRAVServico.RAV dataItem = e.Item.DataItem as WAExtratoAntecipacaoRAVServico.RAV;

                Literal lblDataAntecipacao = e.Item.FindControl("lblDataAntecipacao") as Literal;
                Literal lblBandeira = e.Item.FindControl("lblBandeira") as Literal;
                Literal lblValorLiquido = e.Item.FindControl("lblValorLiquido") as Literal;
                LinkButton btnDetalhe = e.Item.FindControl("btnDetalhe") as LinkButton;

                lblDataAntecipacao.Text = dataItem.DataAntecipacao.ToString("dd/MM/yy");
                lblBandeira.Text = dataItem.TipoBandeira;
                lblValorLiquido.Text = dataItem.ValorAntecipacao.ToString("N2");

                btnDetalhe.CommandArgument = e.Item.ItemIndex.ToString();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                Literal lblTotalValorLiquido = e.Item.FindControl("lblTotalValorLiquido") as Literal;
                lblTotalValorLiquido.Text = Totalizador.Totais.TotalValorLiquido.ToString("N2");                                                                
            }
        }

        protected void btnDetalhe_Click(object sender, EventArgs e)
        {
            LinkButton btnDetalhe = sender as LinkButton;
            Int32? index = btnDetalhe.CommandArgument.ToInt32Null();
            if (index.HasValue && Registros.Length > index.Value)
            {
                WAExtratoAntecipacaoRAVServico.RAV dataItem = Registros[index.Value];

                QueryStringSegura qsDetalhe = new QueryStringSegura();
                qsDetalhe["DataAntecipacao"] = dataItem.DataAntecipacao.ToString("ddMMyyyyHHmmssfff");
                qsDetalhe["CodigoBandeira"] = dataItem.CodigoBandeira.ToString();
                qsDetalhe["TipoConsulta"] = "detalhe";

                SPUtility.Redirect(base.ObterUrlLinkDetalhe(qsDetalhe), SPRedirectFlags.CheckUrl, this.Context);
            }
        }

        protected void btnVerTodos_Click(object sender, EventArgs e)
        {
            QueryStringSegura qsVerTodos = new QueryStringSegura();
            qsVerTodos["TipoConsulta"] = "verTodos";

            String[] codigoBandeiras = Totalizador.Valores.Select(totalizadorBandeira => totalizadorBandeira.CodigoBandeira.ToString()).ToArray();
            qsVerTodos["CodigoBandeiras"] = String.Join(";", codigoBandeiras);

            SPUtility.Redirect(base.ObterUrlLinkDetalhe(qsVerTodos), SPRedirectFlags.CheckUrl, this.Context);
        }
    }
}
