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
    public partial class RelatorioCredito : BaseUserControl, IRelatorioHandler
    {
        public string IdControl { get { return "RelatorioCredito"; } }
        public String NomeOperacao { get { return "Relatório - Valores Pagos - Crédito"; } }
        protected Paginacao objPaginacao;
        protected Totalizadores rcTotalizadores;
        protected TableSize ddlRegistrosPorPagina;
        protected MensagemOfertaTaxa msgOferta;
        private WAExtratoValoresPagosServico.CreditoTotalizador Totalizador
        {
            get { return (WAExtratoValoresPagosServico.CreditoTotalizador)ViewState["Totalizador"]; }
            set { ViewState["Totalizador"] = value; }
        }
        private WAExtratoValoresPagosServico.Credito[] Registros
        {
            get { return (WAExtratoValoresPagosServico.Credito[])ViewState["Registros"]; }
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
                        WAExtratoValoresPagosServico.Credito[] registros;
                        WAExtratoValoresPagosServico.CreditoTotalizador totalizador;

                        contexto.Cliente.ConsultarRelatorioCredito(
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
            rcTotalizadores.ValorLiquido = Totalizador.TotalValorLiquido;
            rcTotalizadores.ExibirTotaisPorBandeiras = false;

            rcTotalizadores.Atualizar();
        }

        protected void rptDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                WAExtratoValoresPagosServico.Credito dataItem = e.Item.DataItem as WAExtratoValoresPagosServico.Credito;

                Literal lblNumeroPV = e.Item.FindControl("lblNumeroPV") as Literal;
                Literal lblDataBaixa = e.Item.FindControl("lblDataBaixa") as Literal;
                Literal lblBanco = e.Item.FindControl("lblBanco") as Literal;
                Literal lblAgencia = e.Item.FindControl("lblAgencia") as Literal;
                Literal lblConta = e.Item.FindControl("lblConta") as Literal;
                Literal lblValorLiquido = e.Item.FindControl("lblValorLiquido") as Literal;
                LinkButton btnDetalhe = e.Item.FindControl("btnDetalhe") as LinkButton;

                lblNumeroPV.Text = dataItem.NumeroPV.ToString();
                lblDataBaixa.Text = dataItem.DataBaixa.ToString("dd/MM/yy");
                lblValorLiquido.Text = dataItem.ValorLiquido.ToString("N2");
                lblBanco.Text = base.ObterNomeBanco(dataItem.BancoCredito);
                lblAgencia.Text = dataItem.AgenciaCredito.ToString();
                lblConta.Text = dataItem.ContaCredito;
                
                btnDetalhe.CommandArgument = e.Item.ItemIndex.ToString();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                Literal lblTotalValorLiquido = e.Item.FindControl("lblTotalValorLiquido") as Literal;
                lblTotalValorLiquido.Text = Totalizador.TotalValorLiquido.ToString("N2");
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
                WAExtratoValoresPagosServico.Credito dataItem = Registros[index.Value];

                QueryStringSegura qsDetalhe = new QueryStringSegura();
                qsDetalhe["NumeroOcu"] = dataItem.NumeroOcu.ToString();
                qsDetalhe["DataBaixa"] = dataItem.DataBaixa.ToString("ddMMyyyyHHmmssfff");
                qsDetalhe["NumeroPV"] = dataItem.NumeroPV.ToString();

                SPUtility.Redirect(base.ObterUrlLinkDetalhe(qsDetalhe), SPRedirectFlags.CheckUrl, this.Context);
            }
        }
    }
}
