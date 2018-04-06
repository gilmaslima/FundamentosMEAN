/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.MEExtratoConsultaTransacaoServico;
using Redecard.PN.Extrato.SharePoint.Helper.ConsultarVendas;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ConsultaTransacao
{
    public partial class Debito : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verificar se é central de atendimento, caso positivo, exibir o link de enviar por e-mail
                if (this.SessaoAtual != null && this.SessaoAtual.UsuarioAtendimento)
                {                    
                    mnuAcoes.BotaoEmail = true;
                }
#if DEBUG
                //Se em DEBUG, sempre exibe botão de Envio de E-mail, para testes
                mnuAcoes.BotaoEmail = true;
#endif
            }
        }

        public void ConsultarTransacao(TransacaoDadosConsultaDTO dadosConsultaDTO)
        {
            this.sumario.Items.Clear();
            this.sumario.Visible = false;

            using (Logger log = Logger.IniciarLog("Consulta por Transação - Débito"))
            {
                log.GravarMensagem("Consulta por Transação - Débito", new { dadosConsultaDTO });

                try
                {
                    StatusRetorno status;
                    List<Object> dados = null;

                    using (var contexto = new ContextoWCF<HISServicoME_Extrato_ConsultaTransacaoClient>())
                    {
                        if (dadosConsultaDTO.TID.EmptyToNull() != null)
                        {
                            var dadosRetorno = contexto.Cliente.ConsultarDebitoTID(
                                out status,
                                dadosConsultaDTO.TID,
                                dadosConsultaDTO.NumeroEstabelecimento);
                            dados = PrepararDados(dadosRetorno);
                        }
                        else
                        {
                            var dadosRetorno = contexto.Cliente.ConsultarDebito(
                                out status,
                                dadosConsultaDTO.NumeroEstabelecimento,
                                dadosConsultaDTO.DataInicial,
                                dadosConsultaDTO.DataFinal,
                                dadosConsultaDTO.NumeroCartao,
                                Convert.ToInt64(dadosConsultaDTO.Nsu));
                            dados = PrepararDados(dadosRetorno);
                        }

                        if (status.CodigoRetorno != 0)
                        {
                            base.ExibirPainelExcecao(status.Fonte, status.CodigoRetorno);
                            ExibirDados(false);
                            return;
                        }

                        ExibirDados(dados != null && dados.Count > 0);
                        if (dados != null && dados.Count > 0)
                        {
                            this.sumario.Visible = true;
                            this.sumario.Items.AddRange(new ConsultarVendasSumarioItem[]
                            {
                                new ConsultarVendasSumarioItem("nome do estabelecimento", this.SessaoAtual.NomeEntidade),
                                new ConsultarVendasSumarioItem("número do estabelecimento", dadosConsultaDTO.NumeroEstabelecimento.ToString()),
                                new ConsultarVendasSumarioItem("data da consulta", DateTime.Now.ToString("dd/MM/yyyy"))
                            });

                            if (dadosConsultaDTO.TID.EmptyToNull() == null)
                            {
                                this.sumario.Items.AddRange(new ConsultarVendasSumarioItem[]
                                {
                                    new ConsultarVendasSumarioItem("tipo de venda", "débito"),
                                    new ConsultarVendasSumarioItem("período", string.Format(
                                        "{0} a {1}",
                                        dadosConsultaDTO.DataInicial.ToString("dd/MM/yyyy"),
                                        dadosConsultaDTO.DataFinal.ToString("dd/MM/yyyy")))
                                });
                            }

                            rptDados.DataSource = dados;
                            rptDados.DataBind();
                        }
                    }
                }
                catch (FaultException<GeneralFault> ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirDados(false);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirDados(false);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        private List<Object> PrepararDados(MEExtratoConsultaTransacaoServico.Debito[] itens)
        {
            List<Object> dados = new List<Object>();

            if (itens != null)
            {
                foreach (var item in itens)
                {
                    if (item != null)
                    {
                        if (item.Cancelamentos == null || item.Cancelamentos.Length == 0)
                        {
                            dados.Add(new
                            {
                                TID = false,
                                Item = item,
                                Cancelamento = (CancelamentoDebito)null
                            });
                        }
                        else
                        {
                            for (Int32 iCancelamento = 0; iCancelamento < item.Cancelamentos.Length; iCancelamento++)
                            {
                                dados.Add(new
                                {
                                    TID = false,
                                    Item = item,
                                    Cancelamento = item.Cancelamentos[iCancelamento]
                                });
                            }
                        }
                    }
                }
            }
            return dados;
        }

        private List<Object> PrepararDados(MEExtratoConsultaTransacaoServico.DebitoTID item)
        {
            List<Object> dados = new List<Object>();

            if (item != null)
            {
                if (item.Cancelamentos == null || item.Cancelamentos.Length == 0)
                {
                    dados.Add(new
                    {
                        TID = true,
                        Item = item,
                        Cancelamento = (CancelamentoDebitoTID)null
                    });
                }
                else
                {
                    for (Int32 iCancelamento = 0; iCancelamento < item.Cancelamentos.Length; iCancelamento++)
                    {
                        dados.Add(new
                        {
                            TID = true,
                            Item = item,
                            Cancelamento = item.Cancelamentos[iCancelamento]
                        });
                    }
                }
            }

            return dados;
        }

        private void ExibirDados(Boolean possuiDados)
        {
            pnlAvisoSemDados.Visible = !possuiDados;
            rptDados.Visible = possuiDados;
        }

        protected void rptDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var TID = (Boolean)DataBinder.Eval(e.Item.DataItem, "TID");

                Label lblTID = e.Item.FindControl("lblTID") as Label;
                PlaceHolder phResumoVendas = e.Item.FindControl("phResumoVendas") as PlaceHolder;
                Label lblNumeroCartao = e.Item.FindControl("lblNumeroCartao") as Label;
                Label lblNumeroEstabelecimento = e.Item.FindControl("lblNumeroEstabelecimento") as Label;
                Label lblDataTransacao = e.Item.FindControl("lblDataTransacao") as Label;
                Label lblNumeroParcelas = e.Item.FindControl("lblNumeroParcelas") as Label;
                Label lblValorTransacao = e.Item.FindControl("lblValorTransacao") as Label;
                Label lblNumeroAutorizacaoBanco = e.Item.FindControl("lblNumeroAutorizacaoBanco") as Label;
                Label lblCodigoProdutoVenda = e.Item.FindControl("lblCodigoProdutoVenda") as Label;
                Label lblBandeira = e.Item.FindControl("lblBandeira") as Label;
                Label lblNumeroAvisoProcesso = e.Item.FindControl("lblNumeroAvisoProcesso") as Label;
                Label lblDataCancelamento = e.Item.FindControl("lblDataCancelamento") as Label;
                HyperLink btnGerarCarta = e.Item.FindControl("btnGerarCarta") as HyperLink;
                Label lblGerarCarta = e.Item.FindControl("lblGerarCarta") as Label;
                Label lblIndicadorTokenizacao = e.Item.FindControl("lblIndicadorTokenizacao") as Label;

                if (TID)
                {
                    var item = (DebitoTID)DataBinder.Eval(e.Item.DataItem, "Item");
                    var cancelamento = (CancelamentoDebitoTID)DataBinder.Eval(e.Item.DataItem, "Cancelamento");

                    if (item.Cancelamentos == null || item.Cancelamentos.Length == 0
                        || cancelamento == null || cancelamento == item.Cancelamentos[0])
                    {
                        lblTID.Text = item.NumeroIdDataCash;
                        phResumoVendas.Controls.Add(this.ObterHyperLinkResumoVenda("D", item.NumeroResumoVendas, item.NumeroPV, item.DataResumo));
                        if (item.NumeroCartao != null)
                            lblNumeroCartao.Text = item.NumeroCartao;
                        lblIndicadorTokenizacao.Text = (String.Compare(item.IndicadorTokenizacao, "s", true) == 0) ? "Sim" : "Não";
                        lblNumeroEstabelecimento.Text = item.NumeroPV.ToString();
                        lblDataTransacao.Text = item.DataTransacao.ToString("dd/MM/yyyy");
                        lblNumeroParcelas.Text = item.NumeroParcelas.ToString();
                        lblValorTransacao.Text = item.ValorTransacao.ToString("N2");
                        lblNumeroAutorizacaoBanco.Text = item.NumeroAutorizacaoBanco;
                        lblCodigoProdutoVenda.Text = item.DescricaoProdutoVenda.EmptyToNull() ?? item.CodigoProdutoVenda.ToString();
                        lblBandeira.Text = item.Bandeira;
                        
                    }

                    if (cancelamento != null)
                    {
                        lblNumeroAvisoProcesso.Text = cancelamento.NumeroAvisoProcesso.ToString();
                        lblDataCancelamento.Text = cancelamento.DataCancelamento.ToString("dd/MM/yyyy");

                        Redecard.PN.Comum.QueryStringSegura queryString = new Redecard.PN.Comum.QueryStringSegura();
                        queryString["numeroProcesso"] = cancelamento.NumeroAvisoProcesso.ToString();
                        queryString["sistemaDados"] = cancelamento.IdOrigemDesagendamento;
                        queryString["timestampTransacao"] = item.TimestampTransacao;
                        btnGerarCarta.Attributes["data-carta-param"] = Server.HtmlEncode(queryString.ToString());
                    }
                    else
                    {
                        lblDataCancelamento.Text = "-";
                        lblNumeroAvisoProcesso.Text = "-";
                        lblGerarCarta.Visible = true;
                        btnGerarCarta.Visible = false;
                    }
                }
                else
                {
                    var item = (MEExtratoConsultaTransacaoServico.Debito)DataBinder.Eval(e.Item.DataItem, "Item");
                    var cancelamento = (MEExtratoConsultaTransacaoServico.CancelamentoDebito)DataBinder.Eval(e.Item.DataItem, "Cancelamento");

                    if (item.Cancelamentos == null || item.Cancelamentos.Length == 0
                        || cancelamento == null || cancelamento == item.Cancelamentos[0])
                    {
                        lblTID.Text = item.IdDatacash;
                        phResumoVendas.Controls.Add(this.ObterHyperLinkResumoVenda("D", item.ResumoVenda, item.NumeroPV, item.DataResumo));
                        if (item.NumeroCartao != null)
                            lblNumeroCartao.Text = item.NumeroCartao;
                        lblIndicadorTokenizacao.Text = (String.Compare(item.IndicadorTokenizacao, "s", true) == 0) ? "Sim" : "Não";
                        lblNumeroEstabelecimento.Text = item.NumeroPV.ToString();
                        lblDataTransacao.Text = item.DataTransacao.ToString("dd/MM/yyyy");
                        lblNumeroParcelas.Text = item.NumeroParcelas.ToString();
                        lblValorTransacao.Text = item.ValorTransacao.ToString("N2");
                        lblNumeroAutorizacaoBanco.Text = item.NumeroAutorizacaoBanco;
                        lblCodigoProdutoVenda.Text = item.DescricaoProdutoVenda.EmptyToNull() ?? item.CodigoProdutoVenda.ToString();
                        lblBandeira.Text = item.DescricaoBandeira;
                    }

                    if (cancelamento != null)
                    {
                        lblNumeroAvisoProcesso.Text = cancelamento.NumeroAvisoProcesso.ToString();
                        lblDataCancelamento.Text = cancelamento.DataCancelamento.ToString("dd/MM/yyyy");

                        Redecard.PN.Comum.QueryStringSegura queryString = new Redecard.PN.Comum.QueryStringSegura();
                        queryString["numeroProcesso"] = cancelamento.NumeroAvisoProcesso.ToString();
                        queryString["sistemaDados"] = cancelamento.IdOrigemDesagendamento;
                        queryString["timestampTransacao"] = item.TimeStampTransacao;
                        btnGerarCarta.Attributes["data-carta-param"] = Server.HtmlEncode(queryString.ToString());
                    }
                    else
                    {
                        lblDataCancelamento.Text = "-";
                        lblNumeroAvisoProcesso.Text = "-";
                        lblGerarCarta.Visible = true;
                        btnGerarCarta.Visible = false;
                    }
                }
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                String html = hdnConteudoExportacao.Value.ToString();
                String nomeArquivo = "ExtratosRedecardDebito_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmssfff");
                Utils.DownloadCSV(html, nomeArquivo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante download Consulta por Transação - Débito", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
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
    }
}