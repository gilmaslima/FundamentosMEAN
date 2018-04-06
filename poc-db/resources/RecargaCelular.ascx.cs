using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.WAExtratoResumoVendasServico;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato;
using Redecard.PN.Extrato.SharePoint.Helper.ConsultarVendas;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ResumoVendas
{
    public partial class RecargaCelular : BaseUserControl
    {
        #region [ Controles ]

        /// <summary>
        /// objPaginacaoComprovantes control.
        /// </summary>
        protected Paginacao ObjPaginacaoComprovantes { get { return (Paginacao)objPaginacaoComprovantes; } }

        #endregion

        #region [ Variáveis/Propriedades ]

        /// <summary>
        /// Parâmetros da consulta utilizada para carregar este Resumo
        /// </summary>
        private ResumoVendaDadosConsultaDTO DadosConsulta
        {
            get { return (ResumoVendaDadosConsultaDTO)ViewState["DadosConsulta"]; }
            set { ViewState["DadosConsulta"] = value; }
        }

        #endregion  

        /// <summary>
        /// Eventos de Página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            ObjPaginacaoComprovantes.RegistrosPorPagina = this.ddlTamanhoPaginaComprovantes.SelectedSize;

#if DEBUG
            //Se em DEBUG, sempre exibe botão de Envio de E-mail, para testes
            this.mnuAcoes.BotaoEmail = true;
#else
            // Verificar se é central de atendimento, caso positivo, exibir o link de enviar por e-mail
            this.mnuAcoes.BotaoEmail = this.SessaoAtual != null && this.SessaoAtual.UsuarioAtendimento;
#endif
        }

        #region [ Eventos dos Botões ]

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            base.QuantidadePost++;

            try
            {
                String html = base.RenderizarControles(
                    true, 
                    sumario.GetTableDesign(),
                    rptVencimento, 
                    rptAjustesDebito, 
                    rptAjustesCredito, 
                    rptComprovantes);

                String csv = CSVExporter.GerarCSV(html, "\t");
                String nomeArquivo = String.Format("RecargaCelular_{0}_{1}",
                    this.DadosConsulta.NumeroEstabelecimento, this.DadosConsulta.NumeroResumoVenda);
                Utils.DownloadXLS(csv, nomeArquivo);
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Erro durante download - Resumo de Vendas - Recarga de Celular", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante download - Resumo de Vendas - Recarga de Celular", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            base.RetornarPaginaAnterior();
        }

        protected void PopupEmailPrepararEmail()
        {
            // corpo do e-mail
            string html = "<html><head><style>.tbl_TR_Resumo_a{background-color:#FFFFFF;height:24px;background-image: url(../img/minus.gif);background-repeat:no-repeat;background-position:1% center;font-size:10px;font-family:Verdana;color:#5E5E5E;cursor:hand;}.tbl_RES2{BORDER-RIGHT: black 1pt solid;BORDER-TOP: white 0pt solid;BORDER-LEFT: black 1pt solid;BORDER-BOTTOM: black 1pt solid}.tbl_TD_Linha{font-family:Verdana;font-size:10px;color:#5E5E5E;text-decoration:none;}.tbl_RES1{BORDER-RIGHT: black 1pt solid;BORDER-TOP: black 1pt solid;BORDER-LEFT: black 1pt solid;BORDER-BOTTOM: Black 1pt solid}.frm_INS{font-size:10px;font-family:Verdana;color:#5E5E5E;}.tbl_TR_Resumo_f{background-color:#EDEDED;height:24px;background-image: url(../img/plus.gif);background-repeat:no-repeat;background-position:1% center;font-size:10px;font-family:Verdana;color:#5E5E5E;}</style></head><body>";
            string corpoEmail = html + hdnConteudoExportacao.Value.ToString();

            var emailController = (PopupEmail)this.popupEmail;
            emailController.EnviarEmail(corpoEmail);
        }

        protected void btnEnviarEmail_Click(object sender, EventArgs e)
        {
            QuantidadePost++;
            this.PopupEmailPrepararEmail();
        }

        #endregion

        #region [ Consultas/Carregamento de Dados ]

        /// <summary>Consulta e carrega os dados do Resumo de Vendas de Recarga de Celular</summary>
        /// <param name="dadosConsultaDTO">Dados da consulta</param>
        public void ConsultarResumoVenda(ResumoVendaDadosConsultaDTO dadosConsultaDTO)
        {
            QuantidadePost--;

            //Armazena dados da consulta na viewstate
            this.DadosConsulta = dadosConsultaDTO;
            
            //Carrega os boxes do Resumo
            this.CarregarCabecalho();
            this.CarregarVencimento();
            this.CarregarAjustesCredito();
            this.CarregarAjustesDebito();
            this.CarregarComprovantes(1, this.ddlTamanhoPaginaComprovantes.SelectedSize);
        }

        /// <summary>Carregamento do box do cabeçalho</summary>
        private void CarregarCabecalho()
        {            
            //Variáveis para chamada e retorno do serviço de Recarga Celular
            var resumo = default(RecargaCelularResumo);
            var status = default(StatusRetorno);
            Int32 numeroPv = this.DadosConsulta.NumeroEstabelecimento;
            Int32 numeroRv = this.DadosConsulta.NumeroResumoVenda;
            DateTime dataPagamento = this.DadosConsulta.DataApresentacao;
            var origem = RecargaCelularResumoOrigem.ResumoVendas;

            using (Logger log = Logger.IniciarLog("Resumo Vendas - Recarga Celular - Resumo"))
            {
                try
                {
                    //Chamada de serviço
                    using (var ctx = new ContextoWCF<HISServicoWA_Extrato_ResumoVendasClient>())
                        resumo = ctx.Cliente.ConsultarRecargaCelularResumo(
                            out status, numeroPv, numeroRv, dataPagamento, origem);

                    //Tratamento de erro caso código retorno inválido
                    if (status.CodigoRetorno != 0)
                    {
                        this.sumario.Visible = false;
                        base.ExibirPainelExcecao(status.Fonte, status.CodigoRetorno);
                    }
                    else
                    {
                        this.sumario.Items.Clear();
                        this.sumario.Items.AddRange(new ConsultarVendasSumarioItem[]
                        {
                            new ConsultarVendasSumarioItem("mês/ano referência", resumo.DataReferencia.ToString("MM/yyyy")),
                            new ConsultarVendasSumarioItem("data do processamento", resumo.DataProcessamento.ToString("dd/MM/yyyy")),
                            new ConsultarVendasSumarioItem("resumo vendas", numeroRv.ToString()),
                            new ConsultarVendasSumarioItem("qtde. de recargas", resumo.QuantidadeTransacao.ToString()),
                            new ConsultarVendasSumarioItem("valor total recargas", resumo.ValorTotalTransacao.ToString("C2", PtBR)),
                            new ConsultarVendasSumarioItem("valor total descontos", resumo.ValorTotalDesconto == 0 ? "-" : resumo.ValorTotalDesconto.ToString("C2", PtBR)),
                            new ConsultarVendasSumarioItem("valor total comissões", resumo.ValorTotalComissao.ToString("C2", PtBR))
                        });
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

        /// <summary>Carregamento do box de Vencimentos</summary>
        private void CarregarVencimento()
        {
            //Variáveis para chamada e retorno do serviço de Recarga Celular
            var vencimento = default(RecargaCelularVencimento);
            var status = default(StatusRetorno);
            Int32 numeroPv = this.DadosConsulta.NumeroEstabelecimento;
            Int32 numeroRv = this.DadosConsulta.NumeroResumoVenda;
            DateTime dataPagamento = this.DadosConsulta.DataApresentacao;

            using (Logger log = Logger.IniciarLog("Resumo Vendas - Recarga Celular - Vencimentos"))
            {
                try
                {
                    //Chamada de serviço
                    using (var ctx = new ContextoWCF<HISServicoWA_Extrato_ResumoVendasClient>())
                        vencimento = ctx.Cliente.ConsultarRecargaCelularVencimentos(
                            out status, numeroPv, numeroRv, dataPagamento);

                    //Tratamento de erro caso código retorno inválido
                    if (status.CodigoRetorno != 0)
                    {
                        base.ExibirPainelExcecao(status.Fonte, status.CodigoRetorno);
                    }                    
                    else
                    {
                        pnlVencimentos.Visible = true;
                        rptVencimento.DataSource = new [] { vencimento };
                        rptVencimento.DataBind();
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

        /// <summary>Carregamento do box de Ajustes a Crédito</summary>
        private void CarregarAjustesCredito()
        {
            //Variáveis para chamada e retorno do serviço de Recarga Celular
            var ajustesCredito = default(List<RecargaCelularAjusteCredito>);
            var status = default(StatusRetorno);
            Int32 numeroPv = this.DadosConsulta.NumeroEstabelecimento;
            Int32 numeroRv = this.DadosConsulta.NumeroResumoVenda;
            DateTime dataPagamento = this.DadosConsulta.DataApresentacao;

            using (Logger log = Logger.IniciarLog("Resumo Vendas - Recarga Celular - Ajustes a Crédito"))
            {
                try
                {
                    //Chamada de serviço
                    using (var ctx = new ContextoWCF<HISServicoWA_Extrato_ResumoVendasClient>())
                        ajustesCredito = ctx.Cliente.ConsultarRecargaCelularAjustesCredito(
                            out status, numeroPv, numeroRv, dataPagamento);

                    //Tratamento de erro caso código retorno inválido
                    if (status.CodigoRetorno != 0)
                    {
                        pnlAjustesCredito.Visible = false;
                        base.ExibirPainelExcecao(status.Fonte, status.CodigoRetorno);
                    }
                    else if (ajustesCredito != null && ajustesCredito.Count > 0)
                    {
                        pnlAjustesCredito.Visible = true;
                        rptAjustesCredito.Visible = true;
                        pnlAjustesCreditoVazio.Visible = false;
                        rptAjustesCredito.DataSource = ajustesCredito;
                        rptAjustesCredito.DataBind();
                    }
                    else
                    {
                        pnlAjustesCredito.Visible = true;
                        rptAjustesCredito.Visible = false;
                        pnlAjustesCreditoVazio.Visible = true;
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

        /// <summary>Carregamento do box de Ajustes a Débito</summary>
        private void CarregarAjustesDebito()
        {
            //Variáveis para chamada e retorno do serviço de Recarga Celular
            var ajustesDebito = default(List<RecargaCelularAjusteDebito>);
            var status = default(StatusRetorno);
            Int32 numeroPv = this.DadosConsulta.NumeroEstabelecimento;
            Int32 numeroRv = this.DadosConsulta.NumeroResumoVenda;
            DateTime dataPagamento = this.DadosConsulta.DataApresentacao;

            using (Logger log = Logger.IniciarLog("Resumo Vendas - Recarga Celular - Ajustes a Débito"))
            {
                try
                {
                    //Chamada de serviço
                    using (var ctx = new ContextoWCF<HISServicoWA_Extrato_ResumoVendasClient>())
                        ajustesDebito = ctx.Cliente.ConsultarRecargaCelularAjustesDebito(
                            out status, numeroPv, numeroRv, dataPagamento);

                    //Tratamento de erro caso código retorno inválido
                    if (status.CodigoRetorno != 0)
                    {
                        pnlAjustesDebito.Visible = false;
                        base.ExibirPainelExcecao(status.Fonte, status.CodigoRetorno);
                    }
                    else if (ajustesDebito != null && ajustesDebito.Count > 0)
                    {
                        pnlAjustesDebito.Visible = true;
                        rptAjustesDebito.Visible = true;
                        pnlAjustesDebitoVazio.Visible = false;
                        rptAjustesDebito.DataSource = ajustesDebito;
                        rptAjustesDebito.DataBind();
                    }
                    else
                    {
                        pnlAjustesDebito.Visible = true;                        
                        rptAjustesDebito.Visible = false;
                        pnlAjustesDebitoVazio.Visible = true;
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

        /// <summary>Carregamento do box de Comprovantes</summary>
        private void CarregarComprovantes(Int32 pagina, Int32 tamanhoPagina)
        {
            //Variáveis para chamada e retorno do serviço de Recarga Celular
            var comprovantes = default(List<RecargaCelularComprovante>);
            var status = default(StatusRetorno);
            Int32 numeroPv = this.DadosConsulta.NumeroEstabelecimento;
            Int32 numeroRv = this.DadosConsulta.NumeroResumoVenda;
            DateTime dataPagamento = this.DadosConsulta.DataApresentacao;

            using (Logger log = Logger.IniciarLog("Resumo Vendas - Recarga Celular - Comprovantes"))
            {
                try
                {
                    //Chamada de serviço
                    using (var ctx = new ContextoWCF<HISServicoWA_Extrato_ResumoVendasClient>())
                        comprovantes = ctx.Cliente.ConsultarRecargaCelularComprovantes(
                            out status, numeroPv, numeroRv, dataPagamento);

                    //Tratamento de erro caso código retorno inválido
                    if (status.CodigoRetorno != 0)
                    {
                        pnlComprovantes.Visible = false;                        
                        base.ExibirPainelExcecao(status.Fonte, status.CodigoRetorno);
                    }
                    else if (comprovantes != null && comprovantes.Count > 0)
                    {                        
                        rptComprovantes.Visible = true;
                        rptComprovantes.DataSource = comprovantes;
                        rptComprovantes.DataBind();

                        grvComprovantes.Visible = true;
                        grvComprovantes.DataSource = comprovantes.Skip((pagina-1) * tamanhoPagina).Take(tamanhoPagina).ToList();
                        grvComprovantes.DataBind();
                        ObjPaginacaoComprovantes.QuantidadeTotalRegistros = comprovantes.Count;
                        ObjPaginacaoComprovantes.PaginaAtual = pagina;

                        pnlComprovantes.Visible = true;
                        pnlComprovantesVazio.Visible = false;
                    }
                    else
                    {
                        pnlComprovantes.Visible = true;
                        rptComprovantes.Visible = false;
                        grvComprovantes.Visible = false;
                        pnlComprovantesVazio.Visible = true;
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

        #endregion

        #region [ DataBind das Tabelas ]

        /// <summary>DataBind do box de Vencimentos</summary>
        protected void rptVencimento_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblDataRecebimento = (Label)e.Item.FindControl("lblDataRecebimento");
                var lblNumeroEstabelecimento = (Label)e.Item.FindControl("lblNumeroEstabelecimento");
                var lblOrdemCredito = (Label)e.Item.FindControl("lblOrdemCredito");
                var lblStatusResumo = (Label)e.Item.FindControl("lblStatusResumo");
                var lblDataAntecipacao = (Label)e.Item.FindControl("lblDataAntecipacao");
                var lblValorLiquido = (Label)e.Item.FindControl("lblValorLiquido");
                var item = (RecargaCelularVencimento) e.Item.DataItem;

                lblDataRecebimento.Text = item.DataPagamento.ToString("dd/MM/yyyy");
                lblNumeroEstabelecimento.Text = this.DadosConsulta.NumeroEstabelecimento.ToString();
                lblOrdemCredito.Text = item.NumeroOc.ToString();
                lblStatusResumo.Text = item.StatusComissao;
                lblDataAntecipacao.Text = item.DataAntecipacao.ToString("dd/MM/yyyy", "00/00/0000");
                lblValorLiquido.Text = item.ValorLiquido.ToString("N2", PtBR);
            }
        }

        /// <summary>DataBind do box de Ajustes a Débito</summary>
        protected void rptAjustesDebito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblDataRecebimento = (Label)e.Item.FindControl("lblDataRecebimento");
                var lblNumeroEstabelecimento = (Label)e.Item.FindControl("lblNumeroEstabelecimento");
                var lblDescricao = (Label)e.Item.FindControl("lblDescricao");
                var lblReferencia = (Label)e.Item.FindControl("lblReferencia");
                var lblValorDevido = (Label)e.Item.FindControl("lblValorDevido");
                var lblValorDebitado = (Label)e.Item.FindControl("lblValorDebitado");
                var lblDebitoDesagendamento = (Label)e.Item.FindControl("lblDebitoDesagendamento");
                var item = (RecargaCelularAjusteDebito)e.Item.DataItem;

                lblDataRecebimento.Text = item.DataRecebimento.ToString("dd/MM/yyyy");
                lblNumeroEstabelecimento.Text = item.NumeroPvAjuste.ToString();
                lblDescricao.Text = item.DescricaoOrigemAjuste;
                lblReferencia.Text = item.DataReferencia.ToString("MM/yyyy");
                lblValorDevido.Text = item.ValorAjuste.ToString("N2", PtBR);
                lblValorDebitado.Text = item.ValorVenda.ToString("N2", PtBR);
                lblDebitoDesagendamento.Text = item.DescricaoAjuste;
            }
        }

        /// <summary>DataBind do box de Ajustes a Crédito</summary>
        protected void rptAjustesCredito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblDataRecebimento = (Label)e.Item.FindControl("lblDataRecebimento");
                var lblNumeroEstabelecimento = (Label)e.Item.FindControl("lblNumeroEstabelecimento");
                var lblDescricao = (Label)e.Item.FindControl("lblDescricao");
                var lblReferencia = (Label)e.Item.FindControl("lblReferencia");
                var lblValorCreditado = (Label)e.Item.FindControl("lblValorCreditado");
                var item = (RecargaCelularAjusteCredito)e.Item.DataItem;

                lblDataRecebimento.Text = item.DataRecebimento.ToString("dd/MM/yyyy");
                lblNumeroEstabelecimento.Text = item.NumeroPvAjuste.ToString();
                lblDescricao.Text = item.DescricaoAjuste;
                lblReferencia.Text = item.DataReferencia.ToString("MM/yyyy");
                lblValorCreditado.Text = item.ValorAjuste.ToString("N2", PtBR);
            }
        }

        /// <summary>DataBind do box de Comprovantes</summary>
        protected void rptComprovantes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var lblNSU = (Label)e.Item.FindControl("lblNSU");
                var lblDataRecarga = (Label)e.Item.FindControl("lblDataRecarga");
                var lblHora = (Label)e.Item.FindControl("lblHora");
                var lblOperadora = (Label)e.Item.FindControl("lblOperadora");
                var lblNumeroCelular = (Label)e.Item.FindControl("lblNumeroCelular");
                var lblValorRecarga = (Label)e.Item.FindControl("lblValorRecarga");
                var lblValorComissao = (Label)e.Item.FindControl("lblValorComissao");
                var lblStatusRecarga = (Label)e.Item.FindControl("lblStatusRecarga");
                var item = (RecargaCelularComprovante)e.Item.DataItem;

                lblNSU.Text = item.NumeroTransacao.ToString();
                lblDataRecarga.Text = item.DataHoraTransacao.ToString("dd/MM/yyyy");
                lblHora.Text = item.DataHoraTransacao.ToString("HH:mm:ss");
                lblOperadora.Text = item.NomeOperadora;
                lblNumeroCelular.Text = item.NumeroCelular;
                lblValorRecarga .Text = item.ValorTransacao.ToString("N2", PtBR);
                lblValorComissao.Text = item.ValorComissao.ToString("N2", PtBR);
                lblStatusRecarga.Text = item.StatusTransacao;
            }
        }

        /// <summary>DataBind do box de Comprovantes</summary>
        protected void grvComprovantes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = (RecargaCelularComprovante)e.Row.DataItem;

                var ltrValorTransacao = (Literal) e.Row.FindControl("ltrValorTransacao");
                var ltrValorComissao = (Literal) e.Row.FindControl("ltrValorComissao");

                ltrValorTransacao.Text = item.ValorTransacao.ToString("N2", PtBR);
                ltrValorComissao.Text = item.ValorComissao.ToString("N2", PtBR);
            }
        }

        #endregion

        #region [ Eventos Paginação ]

        /// <summary>
        /// Alteração no tamanho da página do box de Comprovantes
        /// </summary>
        protected void ddlTamanhoPaginaComprovantes_TamanhoPaginaChanged(Object sender, Int32 tamanhoPagina)
        {
            this.CarregarComprovantes(ObjPaginacaoComprovantes.PaginaAtual, tamanhoPagina);
        }

        /// <summary>
        /// Alteração na página do box de Comprovantes
        /// </summary>
        /// <param name="pagina"></param>
        /// <param name="e"></param>
        protected void objPaginacaoComprovantes_PaginacaoChanged(Int32 pagina, EventArgs e)
        {
            this.CarregarComprovantes(pagina, this.ddlTamanhoPaginaComprovantes.SelectedSize);
        }

        #endregion
    }
}