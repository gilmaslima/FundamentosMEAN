/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Microsoft.SharePoint.Utilities;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.WAExtratoValoresPagosServico;
using System;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ValoresPagos
{
    public partial class RelatorioDebito : BaseUserControl, IRelatorioHandler
    {
        public string IdControl { get { return "RelatorioDebito"; } }
        public String NomeOperacao { get { return "Relatório - Valores Pagos - Débito"; } }
        protected Paginacao objPaginacao;
        protected Totalizadores rcTotalizadores;
        protected TableSize ddlRegistrosPorPagina;
        protected MensagemOfertaTaxa msgOferta;
        private WAExtratoValoresPagosServico.DebitoTotalizador Totalizador
        {
            get { return (WAExtratoValoresPagosServico.DebitoTotalizador)ViewState["Totalizador"]; }
            set { ViewState["Totalizador"] = value; }
        }
        private WAExtratoValoresPagosServico.Debito[] Registros
        {
            get { return (WAExtratoValoresPagosServico.Debito[])ViewState["Registros"]; }
            set { ViewState["Registros"] = value; }
        }

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

                    using (var contexto = new ContextoWCF<HISServicoWA_Extrato_ValoresPagosClient>())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { dadosBusca, pagina, tamanhoPagina, paginasVirtuais, registroInicial, qtdRegistrosVirtuais, GuidPesquisa = GuidPesquisa(), GuidTotalizador = GuidPesquisaTotalizador() });

                        StatusRetorno statusRelatorio, statusTotalizador;
                        WAExtratoValoresPagosServico.Debito[] registros;
                        WAExtratoValoresPagosServico.DebitoTotalizador totalizador;

                        contexto.Cliente.ConsultarRelatorioDebito(
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

                        msgOferta.CarregarMensagem();

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

            rcTotalizadores.ExibirTotaisPorBandeiras = false;
            rcTotalizadores.Atualizar();
        }

        protected void rptDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                WAExtratoValoresPagosServico.Debito dataItem = e.Item.DataItem as WAExtratoValoresPagosServico.Debito;

                Literal lblNumeroPV = e.Item.FindControl("lblNumeroPV") as Literal;
                Literal lblDataPagamento = e.Item.FindControl("lblDataPagamento") as Literal;
                Literal lblValorLiquido = e.Item.FindControl("lblValorLiquido") as Literal;
                Literal lblBandeira = e.Item.FindControl("lblBandeira") as Literal;
                Literal lblBanco = e.Item.FindControl("lblBanco") as Literal;
                Literal lblAgencia = e.Item.FindControl("lblAgencia") as Literal;
                Literal lblConta = e.Item.FindControl("lblConta") as Literal;
                LinkButton btnDetalhe = e.Item.FindControl("btnDetalhe") as LinkButton;

                lblNumeroPV.Text = dataItem.NumeroPV.ToString();
                lblDataPagamento.Text = dataItem.DataPagamento.ToString("dd/MM/yy");
                lblValorLiquido.Text = dataItem.ValorLiquido.ToString("N2");
                lblBandeira.Text = dataItem.TipoBandeira;
                lblBanco.Text = base.ObterNomeBanco(dataItem.BancoCredito);
                lblAgencia.Text = dataItem.AgenciaCredito.ToString();
                lblConta.Text = dataItem.ContaCredito;
                
                btnDetalhe.CommandArgument = e.Item.ItemIndex.ToString();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                Literal lblTotalValorLiquido = e.Item.FindControl("lblTotalValorLiquido") as Literal;
                lblTotalValorLiquido.Text = Totalizador.Totais.TotalValorLiquido.ToString("N2");
            }
            else if (e.Item.ItemType == ListItemType.Header)
            {
                Literal ltrObservacaoOferta = e.Item.FindControl("ltrObservacaoOferta") as Literal;

                if (!Object.ReferenceEquals(ltrObservacaoOferta, null))
                {
                    ltrObservacaoOferta.Visible = this.msgOferta.PossuiOferta;
                }
            }
        }

        protected void btnDetalhe_Click(object sender, EventArgs e)
        {
            LinkButton btnDetalhe = sender as LinkButton;
            Int32? index = btnDetalhe.CommandArgument.ToInt32Null();
            if (index.HasValue && Registros.Length > index.Value)
            {
                WAExtratoValoresPagosServico.Debito dataItem = Registros[index.Value];

                QueryStringSegura qsDetalhe = new QueryStringSegura();
                qsDetalhe["DataPagamento"] = dataItem.DataPagamento.ToString("ddMMyyyyHHmmssfff");
                qsDetalhe["NumeroPV"] = dataItem.NumeroPV.ToString();

                SPUtility.Redirect(base.ObterUrlLinkDetalhe(qsDetalhe), SPRedirectFlags.CheckUrl, this.Context);
            }
        }        
    }
}
