/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Servico.RV;
using Redecard.PN.Extrato.SharePoint.WAExtratoResumoVendasServico;
using Redecard.PN.Extrato.SharePoint.Helper.ConsultarVendas;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2.ResumoVendas
{
    public partial class Construcard : BaseUserControl
    {
        #region [ Eventos de Controles ]

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verificar se é central de atendimento, caso positivo, exibir o link de enviar por e-mail
                if (this.SessaoAtual != null && this.SessaoAtual.UsuarioAtendimento)
                {
                    this.mnuAcoes.BotaoEmail = true;
                }
#if DEBUG
                //Se em DEBUG, sempre exibe botão de Envio de E-mail, para testes
                this.mnuAcoes.BotaoEmail = true;
#endif
            }
        }

        #endregion

        #region [ Métodos de Apoio ]

        public void ConsultarResumoVenda(ResumoVendaDadosConsultaDTO dadosConsultaDTO)
        {
            using (Logger Log = Logger.IniciarLog("Consulta Resumo de Vendas - Construcard"))
            {
                List<Exception> excecoes = new List<Exception>();

                try
                {
                    Servico.RV.StatusRetorno objStatusRetorno;
                    ConsultarTransacaoDebitoRetorno resumoRetorno;

                    using (var contexto = new ContextoWCF<Servico.RV.ResumoVendasClient>())
                    {

                        #region [ ==========  RESUMO DE VENDA - WACA799 ========== ]

                        ConsultarTransacaoDebitoEnvio resumoEnvio = new ConsultarTransacaoDebitoEnvio();
                        resumoEnvio.DataApresentacao = dadosConsultaDTO.DataApresentacao;
                        resumoEnvio.NumeroEstabelecimento = dadosConsultaDTO.NumeroEstabelecimento;
                        resumoEnvio.NumeroResumoVenda = dadosConsultaDTO.NumeroResumoVenda;

                        resumoRetorno = contexto.Cliente.ConsultarTransacaoDebito(out objStatusRetorno, resumoEnvio);

                        if (objStatusRetorno.CodigoRetorno != 0)
                        {
                            base.ExibirPainelExcecao(objStatusRetorno.Fonte, objStatusRetorno.CodigoRetorno);
                            return;
                        }
                        else
                        {
                            this.MontaGridResumo(resumoRetorno);
                        }

                        #endregion
                    }

                    using (var contexto = new ContextoWCF<HISServicoWA_Extrato_ResumoVendasClient>())
                    {
                        WAExtratoResumoVendasServico.StatusRetorno status;

                        #region [ ==========  RESUMO DE VENDA - WACA797 ========== ]

                        try
                        {
                            List<ConstrucardCVsAceitos> cvsAceitos = contexto.Cliente.ConsultarConstrucardCVsAceitos(
                                                out status, dadosConsultaDTO.NumeroEstabelecimento,
                                                dadosConsultaDTO.NumeroResumoVenda, dadosConsultaDTO.DataApresentacao);

                            if (status.CodigoRetorno == 0)
                            {
                                divComprovantesVendasAceitos.Visible = true;
                                this.MontaGridVendasAceitas(cvsAceitos);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.GravarErro(ex);
                            excecoes.Add(ex);
                        }

                        #endregion

                        #region [ ==========  RESUMO DE VENDA - WACA748 ========== ]

                        try
                        {
                            List<DebitoCDCAjuste> ajustes = contexto.Cliente.ConsultarDebitoConstrucardAjustes(out status,
                                               dadosConsultaDTO.NumeroEstabelecimento,
                                               dadosConsultaDTO.NumeroResumoVenda,
                                               dadosConsultaDTO.DataApresentacao,
                                               resumoRetorno.TipoResposta);

                            if (ajustes != null)
                            {
                                // altera a descrição do motivo de débito para uma versão customizada em lista do Sharepoint
                                ajustes.ForEach(x =>
                                {
                                    x.DescricaoAjuste = this.GetTituloMotivoCreditoDebitoCustomizado(
                                        x.DescricaoAjuste,
                                        tituloDefault: x.DescricaoAjuste);
                                });
                            }

                            if (status.CodigoRetorno == 0)
                            {
                                divAjustesDebito.Visible = true;
                                this.MontaGridAjustesDebito(ajustes);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.GravarErro(ex);
                            excecoes.Add(ex);
                        }

                        #endregion
                    }
                }
                catch (FaultException<Servico.RV.GeneralFault> ex)
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

                if (excecoes.Count > 0)
                {
                    if (excecoes[0] is FaultException<WAExtratoResumoVendasServico.GeneralFault>)
                    {
                        var ex = excecoes[0] as FaultException<WAExtratoResumoVendasServico.GeneralFault>;
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    else if (excecoes[0] is FaultException<Servico.RV.GeneralFault>)
                    {
                        var ex = excecoes[0] as FaultException<Servico.RV.GeneralFault>;
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    else
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #region [ Tabela Resumo ]

        private void MontaGridResumo(ConsultarTransacaoDebitoRetorno objConsultarTransacaoDebitoRetorno)
        {
            this.sumario.Items.Clear();
            this.sumario.Items.AddRange(new ConsultarVendasSumarioItem[]
            {
                new ConsultarVendasSumarioItem("resumo de vendas", objConsultarTransacaoDebitoRetorno.NumeroResumoVendaAnterior.ToString()),
                new ConsultarVendasSumarioItem("qtde. de vendas", objConsultarTransacaoDebitoRetorno.QuantidadeComprovanteVenda.ToString()),
                new ConsultarVendasSumarioItem("data de apresentação", objConsultarTransacaoDebitoRetorno.DataApresentacaoAnterior.ToString(Helper.Constantes.Formatador.FormatoDataPadrao)),
                new ConsultarVendasSumarioItem("data de vencimento", objConsultarTransacaoDebitoRetorno.DataVencimento.ToString(Helper.Constantes.Formatador.FormatoDataPadrao)),
                new ConsultarVendasSumarioItem("valor apresentado", objConsultarTransacaoDebitoRetorno.ValorApresentado.ToString("N2")),
                new ConsultarVendasSumarioItem("valor líquido", objConsultarTransacaoDebitoRetorno.ValorLiquido.ToString("N2")),
                new ConsultarVendasSumarioItem("desconto", objConsultarTransacaoDebitoRetorno.ValorDesconto.ToString("N2")),
                new ConsultarVendasSumarioItem("tipo", "Construcard")
            });
        }

        #endregion

        #region [ Tabela Comprovantes de Vendas Aceitos ]

        private void MontaGridVendasAceitas(List<ConstrucardCVsAceitos> itens)
        {
            Boolean possuiDados = itens != null && itens.Count > 0;
            pnlVendasAceitasVazio.Visible = !possuiDados;
            rptVendasAceitas.Visible = possuiDados;

            if (possuiDados)
            {
                rptVendasAceitas.DataSource = itens;
                rptVendasAceitas.DataBind();
            }
        }

        protected void rptVendasAceitas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = e.Item.DataItem as ConstrucardCVsAceitos;

                //Recuperação dos controles da linha atual do repeater
                Label _lblTID = e.Item.FindControl("lblTID") as Label;
                Label _lblComprovanteVendaNSU = e.Item.FindControl("lblComprovanteVendaNSU") as Label;
                Label _lblNumeroCartao = e.Item.FindControl("lblNumeroCartao") as Label;
                Label _lblDataVenda = e.Item.FindControl("lblDataVenda") as Label;
                Label _lblValorVenda = e.Item.FindControl("lblValorVenda") as Label;
                Label _lblQtdeParcelas = e.Item.FindControl("lblQtdeParcelas") as Label;
                Label _lblHora = e.Item.FindControl("lblHora") as Label;
                Label _lblDescricao = e.Item.FindControl("lblDescricao") as Label;

                //Atribuição dos valores nos controles da linha do repeater
                _lblTID.Text = item.TID;
                _lblComprovanteVendaNSU.Text = item.NumeroCV.ToString();
                if (item.NumeroCartao != null)
                    _lblNumeroCartao.Text = item.NumeroCartao.GetLast(4);
                _lblDataVenda.Text = item.DataCV.ToString("dd/MM/yyyy");
                _lblValorVenda.Text = item.ValorCV.ToString("N2");
                _lblQtdeParcelas.Text = item.Plano.ToString();                
                _lblHora.Text = item.HorasCV.ToString("HH'h'mm'min'ss's'");
                _lblDescricao.Text = item.Descricao;
            }
        }

        #region [ Tabela Ajustes Débito ]

        private void MontaGridAjustesDebito(List<DebitoCDCAjuste> itens)
        {
            Boolean possuiDados = itens != null && itens.Count > 0;
            pnlAjustesDebitoVazio.Visible = !possuiDados;
            rptAjustesDebito.Visible = possuiDados;

            if (possuiDados)
            {
                rptAjustesDebito.DataSource = itens;
                rptAjustesDebito.DataBind();
            }
        }

        protected void rptAjustesDebito_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = e.Item.DataItem as DebitoCDCAjuste;

                //Recuperação dos controles da linha atual do repeater
                Label _lblCodigo = e.Item.FindControl("lblCodigo") as Label;
                Label _lblDescricao = e.Item.FindControl("lblDescricao") as Label;
                Label _lblReferencia = e.Item.FindControl("lblReferencia") as Label;
                Label _lblPVOrigem = e.Item.FindControl("lblPVOrigem") as Label;
                Label _lblValorDebito = e.Item.FindControl("lblValorDebito") as Label;
                Label _lblValor = e.Item.FindControl("lblValor") as Label;
                Label _lblData = e.Item.FindControl("lblData") as Label;
                Label _lblDebitoDesagendamento = e.Item.FindControl("lblDebitoDesagendamento") as Label;

                //Atribuição dos valores nos controles da linha do repeater
                _lblCodigo.Text = item.PVAjuste.ToString();
                _lblDescricao.Text = item.DescricaoAjuste;
                _lblReferencia.Text = item.DataReferencia2;
                _lblPVOrigem.Text = item.PVAjuste.ToString();
                _lblValorDebito.Text = item.ValorDebito.ToString("N2");
                _lblValor.Text = item.ValorAjuste.ToString("N2");
                _lblData.Text = item.DataReferencia.ToString("dd/MM/yyyy");

                String debitoDesagendamento = item.IndicadorDebitoDesagendamento.EmptyToNull();
                if (debitoDesagendamento != null)
                {
                    if (debitoDesagendamento.Equals("DES", StringComparison.InvariantCultureIgnoreCase))
                        debitoDesagendamento = "Desagendamento";
                    else if (debitoDesagendamento.Equals("NET", StringComparison.InvariantCultureIgnoreCase))
                        debitoDesagendamento = "Débito";
                }
                _lblDebitoDesagendamento.Text = debitoDesagendamento;
            }
        }

        #endregion

        #endregion

        #region [ Download ]

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            QuantidadePost++;

            try
            {
                String csvContent = hdnConteudoExportacao.Value.ToString();
                String nomeArquivo = "ResumoVendasConstrucard_" + DateTime.Now.ToString("yyyy-MM-dd_HHmmssfff");
                Utils.DownloadXLS(csvContent, nomeArquivo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Erro durante Download - Resumo de Vendas - Construcard", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ E-mail ]

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

        #endregion
    }
}
