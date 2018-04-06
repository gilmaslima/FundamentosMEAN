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
    public partial class Credito : BaseUserControl
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

        //Dados da consulta
        private TransacaoDadosConsultaDTO DadosConsulta
        {
            get { return (TransacaoDadosConsultaDTO)ViewState["DadosConsulta"]; }
            set { ViewState["DadosConsulta"] = value; }
        }

        public void ConsultarTransacao(TransacaoDadosConsultaDTO dadosConsultaDTO)
        {
            this.sumario.Items.Clear();
            this.sumario.Visible = false;

            //Armazena dados da consulta na viewstate
            this.DadosConsulta = dadosConsultaDTO;

            using (Logger log = Logger.IniciarLog("Consulta por Transação - Crédito"))
            {
                log.GravarMensagem("Consulta por Transação - Crédito", new { dadosConsultaDTO });

                try
                {
                    StatusRetorno status;
                    List<Object> dados = null;

                    using (var contexto = new ContextoWCF<HISServicoME_Extrato_ConsultaTransacaoClient>())
                    {
                        if (dadosConsultaDTO.TID.EmptyToNull() != null)
                        {
                            var dadosRetorno = contexto.Cliente.ConsultarCreditoTID(
                                out status,
                                dadosConsultaDTO.TID,
                                dadosConsultaDTO.NumeroEstabelecimento);
                            dados = PrepararDados(dadosRetorno);
                        }
                        else
                        {
                            var dadosRetorno = contexto.Cliente.ConsultarCredito(
                                out status,
                                dadosConsultaDTO.NumeroEstabelecimento,
                                dadosConsultaDTO.DataInicial,
                                dadosConsultaDTO.DataFinal,
                                dadosConsultaDTO.NumeroCartao,
                                Convert.ToInt64(dadosConsultaDTO.Nsu));
                            dados = PrepararDados(dadosRetorno);
                        }
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
                                new ConsultarVendasSumarioItem("tipo de venda", "crédito"),
                                new ConsultarVendasSumarioItem("período", string.Format(
                                    "{0} a {1}",
                                    dadosConsultaDTO.DataInicial.ToString("dd/MM/yyyy"),
                                    dadosConsultaDTO.DataFinal.ToString("dd/MM/yyyy")))
                            });
                        }

                        rpDados.DataSource = dados;
                        rpDados.DataBind();
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

        private List<Object> PrepararDados(MEExtratoConsultaTransacaoServico.Credito[] itens)
        {
            List<Object> dados = new List<Object>();

            if (itens != null)
            {
                foreach (var item in itens)
                {
                    if (item.Cancelamentos == null || item.Cancelamentos.Length == 0)
                    {
                        dados.Add(new
                        {
                            TID = false,
                            Item = item,
                            Cancelamento = (CancelamentoCredito)null
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

            return dados;
        }

        private List<Object> PrepararDados(MEExtratoConsultaTransacaoServico.CreditoTID item)
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
                        Cancelamento = (CancelamentoCreditoTID)null
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
            rpDados.Visible = possuiDados;
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                String html = hdnConteudoExportacao.Value.ToString();
                String nomeArquivo = "ExtratosRedecardCredito_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmssfff");
                Utils.DownloadCSV(html, nomeArquivo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante Download Consulta por Transação - Crédito", ex);
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

        protected void rpDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Label lblTID = e.Item.FindControl("lblTID") as Label;
                Label lblNumeroCartao = e.Item.FindControl("lblNumeroCartao") as Label;
                Label lblDataVenda = e.Item.FindControl("lblDataVenda") as Label;
                Label lblValorTransacao = e.Item.FindControl("lblValorTransacao") as Label;
                Label lblAutorizacaoVenda = e.Item.FindControl("lblAutorizacaoVenda") as Label;
                Label lblQuantidadeParcelas = e.Item.FindControl("lblQuantidadeParcelas") as Label;
                Label lblCodigoProdutoVenda = e.Item.FindControl("lblCodigoProdutoVenda") as Label;
                Label lblDescricaoBandeira = e.Item.FindControl("lblDescricaoBandeira") as Label;
                Label lblDataCancelamento = e.Item.FindControl("lblDataCancelamento") as Label;
                Label lblNumeroAvisoProcesso = e.Item.FindControl("lblNumeroAvisoProcesso") as Label;
                Label lblMotivoCancelamento = e.Item.FindControl("lblMotivoCancelamento") as Label;
                HyperLink btnGerarCarta = e.Item.FindControl("btnGerarCarta") as HyperLink;
                PlaceHolder phResumoVendas = e.Item.FindControl("phResumoVendas") as PlaceHolder;
                Label lblGerarCarta = e.Item.FindControl("lblGerarCarta") as Label;
                Label lblIndicadorTokenizacao = e.Item.FindControl("lblIndicadorTokenizacao") as Label;

                var TID = (Boolean)DataBinder.Eval(e.Item.DataItem, "TID");
                if (TID)
                {
                    var item = (CreditoTID)DataBinder.Eval(e.Item.DataItem, "Item");
                    var cancelamento = (CancelamentoCreditoTID)DataBinder.Eval(e.Item.DataItem, "Cancelamento");

                    //preenche dados da transação apenas para o primeiro registro de cancelamento 
                    //ou se não possuir cancelamentos
                    if (item.Cancelamentos == null || item.Cancelamentos.Length == 0
                        || cancelamento == null || cancelamento == item.Cancelamentos[0])
                    {

                        lblTID.Text = item.NumeroIdDataCash;
                        if (item.NumeroCartao != null)
                            lblNumeroCartao.Text = item.NumeroCartao;
                        lblIndicadorTokenizacao.Text = (String.Compare(item.IndicadorTokenizacao, "s", true) == 0) ? "Sim" : "Não";
                        lblDataVenda.Text = item.DataTransacao.ToString("dd/MM/yyyy");
                        lblValorTransacao.Text = item.ValorTransacao.ToString("N2");
                        lblAutorizacaoVenda.Text = item.AutorizacaoVenda;
                        phResumoVendas.Controls.Add(this.ObterHyperLinkResumoVenda("C", item.NumeroResumoVendas, this.DadosConsulta.NumeroEstabelecimento, item.DataResumo));
                        lblQuantidadeParcelas.Text = item.QuantidadeParcelas.ToString();
                        lblCodigoProdutoVenda.Text = item.DescricaoProdutoVenda.EmptyToNull() ?? item.CodigoProdutoVenda.ToString();
                        lblDescricaoBandeira.Text = item.Bandeira;
                    }

                    //se cancelamento é válido, exibe informações na grid
                    if (cancelamento != null)
                    {
                        lblDataCancelamento.Text = cancelamento.DataCancelamento.ToString("dd/MM/yyyy");
                        lblNumeroAvisoProcesso.Text = cancelamento.NumeroAvisoProcesso.ToString();
                        switch (cancelamento.MotivoCancelamento)
                        {
                            case CreditoTidMotivoCancelamento.Cancelamento: lblMotivoCancelamento.Text = "Cancelam."; break;
                            case CreditoTidMotivoCancelamento.ChargeBack: lblMotivoCancelamento.Text = "Chargeback"; break;
                        }

                        Redecard.PN.Comum.QueryStringSegura queryString = new Redecard.PN.Comum.QueryStringSegura();
                        queryString["numeroProcesso"] = cancelamento.NumeroAvisoProcesso.ToString();
                        queryString["sistemaDados"] = cancelamento.CodigoMotivoCancelamento.ToString();
                        queryString["timestampTransacao"] = item.TimestampTransacao;
                        btnGerarCarta.Attributes["data-carta-param"] = Server.HtmlEncode(queryString.ToString());
                    }
                    else
                    {
                        lblDataCancelamento.Text = "-";
                        lblNumeroAvisoProcesso.Text = "-";
                        lblMotivoCancelamento.Text = "-";
                        lblGerarCarta.Visible = true;
                        btnGerarCarta.Visible = false;
                    }
                }
                else
                {
                    var item = (MEExtratoConsultaTransacaoServico.Credito)DataBinder.Eval(e.Item.DataItem, "Item");
                    var cancelamento = (MEExtratoConsultaTransacaoServico.CancelamentoCredito)DataBinder.Eval(e.Item.DataItem, "Cancelamento");

                    //preenche dados da transação apenas para o primeiro registro de cancelamento 
                    //ou se não possuir cancelamentos
                    if (item.Cancelamentos == null || item.Cancelamentos.Length == 0
                        || cancelamento == null || cancelamento == item.Cancelamentos[0])
                    {
                        lblTID.Text = item.NumeroIdDatacash;
                        if (item.NumeroCartao != null)
                            lblNumeroCartao.Text = item.NumeroCartao;
                        lblIndicadorTokenizacao.Text = (String.Compare(item.IndicadorTokenizacao, "s", true) == 0) ? "Sim" : "Não";
                        lblDataVenda.Text = item.DataTransacao.ToString("dd/MM/yyyy");
                        lblValorTransacao.Text = item.ValorTransacao.ToString("N2");
                        lblAutorizacaoVenda.Text = item.AutorizacaoVenda;
                        phResumoVendas.Controls.Add(this.ObterHyperLinkResumoVenda("C", item.NumeroResumoVendas, this.DadosConsulta.NumeroEstabelecimento, item.DataResumo));
                        lblQuantidadeParcelas.Text = item.QuantidadeParcelas.ToString();
                        lblCodigoProdutoVenda.Text = item.DescricaoProdutoVenda.EmptyToNull() ?? item.CodigoProdutoVenda.ToString();
                        lblDescricaoBandeira.Text = item.DescricaoBandeira;
                    }

                    //se cancelamento é válido, exibe informações na grid
                    if (cancelamento != null)
                    {
                        lblDataCancelamento.Text = cancelamento.DataCancelamento.ToString("dd/MM/yyyy");
                        lblNumeroAvisoProcesso.Text = cancelamento.NumeroAvisoProcesso.ToString();
                        switch (cancelamento.MotivoCancelamento)
                        {
                            case CreditoMotivoCancelamento.Cancelamento: lblMotivoCancelamento.Text = "Cancelam."; break;
                            case CreditoMotivoCancelamento.ChargeBack: lblMotivoCancelamento.Text = "Chargeback"; break;
                        }

                        Redecard.PN.Comum.QueryStringSegura queryString = new Redecard.PN.Comum.QueryStringSegura();
                        queryString["numeroProcesso"] = cancelamento.NumeroAvisoProcesso.ToString();
                        queryString["sistemaDados"] = cancelamento.CodigoMotivoCancelamento.ToString();
                        queryString["timestampTransacao"] = item.TimeStampTransacao;
                        btnGerarCarta.Attributes["data-carta-param"] = Server.HtmlEncode(queryString.ToString());
                    }
                    else
                    {
                        lblDataCancelamento.Text = "-";
                        lblNumeroAvisoProcesso.Text = "-";
                        lblMotivoCancelamento.Text = "-";
                        lblGerarCarta.Visible = true;
                        btnGerarCarta.Visible = false;
                    }
                }
            }
        }
    }
}
