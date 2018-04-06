using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.SharePoint.Utilities;
using Rede.PN.Cancelamento.Sharepoint.BlacklistValidations;
using Rede.PN.Cancelamento.Sharepoint.CancelamentoServico;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Administration;

namespace Rede.PN.Cancelamento.Sharepoint.WebParts.SolicitarCancelamento
{
    /// <summary>
    /// Controle de solicitar cancelamentos
    /// </summary>
    public partial class SolicitarCancelamentoUserControl : UserControlBase
    {
        #region [ Propriedades ]

        /// <summary>
        /// querystring dados.
        /// </summary>
        protected String DadosQueryString
        {
            get
            {
                return Request.QueryString["dados"];
            }
        }


        /// <summary>
        /// Lista de Solicitações de Cancelamento que fica na ViewState
        /// </summary>
        private List<SolicitacaoCancelamento> SolicitacoesCancelamento
        {
            get
            {
                if (ViewState["SolicitacoesCancelamento"] == null)
                    ViewState["SolicitacoesCancelamento"] = new List<SolicitacaoCancelamento>();

                return (List<SolicitacaoCancelamento>)ViewState["SolicitacoesCancelamento"];
            }

            set
            {
                ViewState["SolicitacoesCancelamento"] = value;
            }
        }

        /// <summary>
        /// Lista de validações das solicitações em lote
        /// </summary>
        private List<Validacao> ValidacoesEmLote
        {
            get
            {
                if (ViewState["Validacoes"] == null)
                    ViewState["Validacoes"] = new List<Validacao>();

                return (List<Validacao>)ViewState["Validacoes"];
            }

            set
            {
                ViewState["Validacoes"] = value;
            }
        }

        /// <summary>
        /// Lista de linhas preenchidas
        /// </summary>
        private List<Int32> LinhasPreenchidas
        {
            get
            {
                if (ViewState["LinhasPreenchidas"] == null)
                    ViewState["LinhasPreenchidas"] = new List<Int32>();

                return (List<Int32>)ViewState["LinhasPreenchidas"];
            }

            set
            {
                ViewState["LinhasPreenchidas"] = value;
            }
        }

        /// <summary>
        /// Flag para substituir validacao de cancelamento em lote
        /// </summary>
        private Boolean CancelamentoEmLote
        {
            get
            {
                if (ViewState["CancelamentoEmLote"] == null)
                    ViewState["CancelamentoEmLote"] = false;

                return (Boolean)ViewState["CancelamentoEmLote"];
            }

            set
            {
                ViewState["CancelamentoEmLote"] = value;
            }
        }

        #endregion

        #region [ Eventos ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Solicitar Cancelamento - Page Load"))
            {
                try
                {
                    Page.Title = "Solicitar Cancelamento";

                    if (!Page.IsPostBack)
                    {
                        // O usuario do tipo atendimento tem permissao apenas para visualizar a pagina
                        if (SessaoAtual != null && SessaoAtual.UsuarioAtendimento)
                        {
                            btnConfirmar.Visible = false;
                        }

                        // valida IP do cliente junto à blacklist
                        BlacklistIPs blacklistIps = new BlacklistIPs();
                        if (!blacklistIps.ValidarIP(BlacklistIPs.GetClientIP()))
                        {
                            MostrarViewBloqueioBlacklist();
                            Historico.BloqueioIPBlackListCancelamento(this.SessaoAtual);
                            return;
                        }

                        // valida PV do cliente junto à blacklist
                        BlacklistPVs blacklistPvs = new BlacklistPVs();
                        if (!blacklistPvs.ValidarPv(SessaoAtual.CodigoEntidade))
                        {
                            MostrarViewBloqueioBlacklist();
                            Historico.BloqueioPVBlackListCancelamento(this.SessaoAtual);
                            return;
                        }

                        if (base.VerificarConfirmacaoPositia())
                        {
                            RedirecionarConfirmacaoPositiva();
                            return;
                        }

                        Boolean pvEstaBloqueadoPorFraude = VerificaPvBloqueadoPorFraude(SessaoAtual.CodigoEntidade, SessaoAtual.CodigoRamoAtividade);
                        if (pvEstaBloqueadoPorFraude)
                        {
                            MostraViewPvBloqueadoPorFraude();
                            return;
                        }

                        CarregaControles();

                        Boolean cancelamentoFeitoNaHome = !String.IsNullOrEmpty(this.DadosQueryString);
                        QueryStringSegura queryString = new QueryStringSegura();
                        if (cancelamentoFeitoNaHome)
                        {
                            queryString = new QueryStringSegura(this.DadosQueryString);
                            String origem = queryString["Origem"];

                            if (origem != "PageSolicitacaoCancelamento")
                            {
                                ExecutarCancelamentoHome(origem, queryString);
                            }
                        }

                        if (String.IsNullOrEmpty(queryString["CodigoEntidade"]))
                        {
#if DEBUG
                            queryString.Add("CodigoEntidade", "12121212");
#else
                            queryString.Add("CodigoEntidade", this.SessaoAtual.CodigoEntidade.ToString());
#endif
                            String url = Request.Url.ToString().Split('?')[0] + "?dados=" + queryString.ToString();
                            Response.Redirect(url, false);
                        }

                    }
                }
                catch (FaultException<GEPontoVen.ModelosErroServicos> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Page Load", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.CodErro ?? 600);
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Page Load", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (PortalRedecardException pe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Page Load", pe);
                    SharePointUlsLog.LogErro(pe);
                    base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Page Load", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento do botão voltar Passo 1
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnVoltar1_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Solicitar Cancelamento - Voltar Passo 1"))
            {
                try
                {
                    // Volta para Home de Cancelamento
                    Response.Redirect("pn_cancelamento.aspx");
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Voltar Passo 1", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Voltar Passo 1", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento do botão voltar Passo 2
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnVoltar2_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Solicitar Cancelamento - Voltar Passo 2"))
            {
                try
                {
                    // Muda a view para o primeiro passo
                    ChangeView(vwDadosCancelamento);
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Voltar Passo 2", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Voltar Passo 2", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento do botão voltar Passo 2
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnVoltar3_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Solicitar Cancelamento - Voltar Passo 2 Em Lote"))
            {
                try
                {
                    // Muda a view para o primeiro passo
                    ChangeView(vwDadosCancelamento);
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Voltar Passo 2 Em Lote", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Voltar Passo 2 Em Lote", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento do botão continuar
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Solicitar Cancelamento - Continuar"))
            {
                try
                {
                    AvancarParaPasso2();
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Continuar", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Continuar", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento do botão confirmar
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Chamada ao serviço ConsultarCancelamentosPn"))
            {
                try
                {
                    String tipoTransacao = String.Empty;
                    List<CancelamentoPn> listaPopUp = new List<CancelamentoPn>();

                    CancelamentoPn cancelamento = new CancelamentoPn();

                    List<CancelamentoPn> cancelamentosDia = new List<CancelamentoPn>();

                    cancelamentosDia = Services.ConsultarCancelamentosPn(SessaoAtual.CodigoEntidade);

                    if (cancelamentosDia != null && cancelamentosDia.Count > 0)
                    {
                        foreach (var item in SolicitacoesCancelamento)
                        {
                            tipoTransacao = item.TipoVenda == TipoVenda.Credito ? "C" : "D";

                            cancelamento = cancelamentosDia.Where(x => (x.TipoTransacao.ToString().ToUpper() == tipoTransacao) &&
                                                                       (x.Nsu == item.NSU) &&
                                                                       (x.ValorCancelamento == item.ValorCancelamento)).FirstOrDefault();

                            if (cancelamento != null && cancelamento != default(CancelamentoPn))
                            {
                                listaPopUp.Add(cancelamento);
                            }
                        }

                        if (listaPopUp != null && listaPopUp.Count > 0)
                        {
                            rptPopUp.DataSource = listaPopUp;
                            rptPopUp.DataBind();
                            //log.GravarMensagem("Após o DataBind");

                            String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { cancelamentoOpenModal('#lbxModalDesbloqueio'); }, 'SP.UI.Dialog.js');";
                            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AbrirModalDialog", javaScript, true);
                        }
                        else
                        {
                            Cancelar();
                        }
                    }
                    else
                    {
                        Cancelar();
                    }
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Confirmar", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Confirmar", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Remove uma solicitação de cancelamento da tabela de confirmação
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnRemover_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Solicitar Cancelamento - Botão Remover"))
            {
                try
                {
                    // Busca o número da linha que deve ser removida
                    var itemIndex = ((RepeaterItem)((Control)sender).NamingContainer).ItemIndex;

                    RemoveSolicitacao(itemIndex);
                    CarregaConfirmacaoSolicitacoesCancelamento();
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Remover", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Remover", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
                finally
                {
                    // Script para desbloquear a tela
                    ScriptManager.RegisterStartupScript(upCancelamentoConfirmacao, upCancelamentoConfirmacao.GetType(), "UnblockUI", "unblockUI();", true);
                }
            }
        }

        /// <summary>
        /// Evento do botão de enviar arquivo para o cancelamento em lote
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnEnviarArquivo_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Solicitar Cancelamento - Botão Enviar Cancelamento em Lote"))
            {
                try
                {
                    if (fuCancelamentoLote.HasFile)
                    {
                        var extensao = fuCancelamentoLote.FileName.Substring(fuCancelamentoLote.FileName.LastIndexOf('.') + 1);

                        // Verifica se a extensão do arquivo é .txt
                        if (VerificaExtensaoArquivo(extensao))
                        {
                            LerArquivoCancelamentoEmLote(fuCancelamentoLote.FileContent);
                            RedirecionaProximoPassoEmLote();
                        }
                        else
                            //TODO Throw Portal Exception extensão de arquivo inválida
                            throw new PortalRedecardException(CODIGO_ERRO, FONTE);
                    }
                    else
                        //TODO Throw Portal Exception não foi selecionado nenhum arquivo
                        throw new PortalRedecardException(CODIGO_ERRO, FONTE);
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Enviar Cancelamento em Lote", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (PortalRedecardException pe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Enviar Cancelamento em Lote", pe);
                    SharePointUlsLog.LogErro(pe);
                    base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Botão Enviar Cancelamento em Lote", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento de mudança da view
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void mvwSolicitarCancelamento_ActiveViewChanged(object sender, EventArgs e)
        {
            var activeView = mvwSolicitarCancelamento.GetActiveView();

            if (activeView == vwDadosCancelamento)
                CarregaDadosCancelamento();
            else if (activeView == vwConfirmacao)
                CarregaDadosConfirmacao();
            else if (activeView == vwComprovante)
                CarregaDadosComprovante();
            else if (activeView == vwErroEmLote)
                CarregaDadosErrosEmLote();
            else if (activeView == vwPvBloqueadoPorFraude)
                CarregarQuadroAviso();
            else if (activeView == vwBloqueioBlacklist)
                CarregarQuadroAvisoBlacklist();

            AtualizaCabecalhoPassoAtual(activeView);
        }

        /// <summary>
        /// Evento de criação da tabela com os dados dos itens a serem cancelados
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptCancelamentosConfirmacao_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (SolicitacaoCancelamento)e.Item.DataItem;
                var ltlNumeroEstabelecimentoVenda = (Literal)e.Item.FindControl("ltlNumeroEstabelecimentoVenda");
                var ltlTipoVenda = (Literal)e.Item.FindControl("ltlTipoVenda");
                var ltlDataVenda = (Literal)e.Item.FindControl("ltlDataVenda");
                var ltlNSU = (Literal)e.Item.FindControl("ltlNSU");
                var ltlValorBrutoVenda = (Literal)e.Item.FindControl("ltlValorBrutoVenda");
                var ltlSaldoDisponivel = (Literal)e.Item.FindControl("ltlSaldoDisponivel");
                var ltlTipoCancelamento = (Literal)e.Item.FindControl("ltlTipoCancelamento");
                var ltlValorCancelamento = (Literal)e.Item.FindControl("ltlValorCancelamento");

                ltlNumeroEstabelecimentoVenda.Text = item.NumeroEstabelecimentoVenda.ToString();
                ltlTipoVenda.Text = item.TipoVendaDetalhado;
                ltlDataVenda.Text = item.DataVenda.ToString("dd/MM/yyyy");
                ltlNSU.Text = MascaraCartao(item.NSU);
                ltlValorBrutoVenda.Text = String.Format("{0:C}", item.ValorBruto);
                ltlSaldoDisponivel.Text = String.Format("{0:C}", item.SaldoDisponivel);
                ltlTipoCancelamento.Text = item.TipoCancelamento.GetDescription();
                ltlValorCancelamento.Text = String.Format("{0:C}", item.ValorCancelamento);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptPopUp_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            using (var log = Logger.IniciarLog("rptPopUp_ItemDataBound"))
            {
                try
                {
                    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                    {
                        var item = (CancelamentoPn)e.Item.DataItem;

                        var literalTipoVendaPopUp = (Literal)e.Item.FindControl("ltlTipoVendaPopUp");
                        var literalDataVendaPopUp = (Literal)e.Item.FindControl("ltlDataVendaPopUp");
                        var literalNSUPopUp = (Literal)e.Item.FindControl("ltlNSUPopUp");
                        var literalValorCancelamentoPopUp = (Literal)e.Item.FindControl("ltlValorCancelamentoPopUp");

                        //log.GravarMensagem("Passou pelos FindControl");

                        literalTipoVendaPopUp.Text = item.TipoTransacao.Equals('C') ? "Crédito" : "Débito";
                        literalDataVendaPopUp.Text = item.DataVenda.ToString("dd/MM/yyyy");
                        literalNSUPopUp.Text = MascaraCartao(item.Nsu);
                        literalValorCancelamentoPopUp.Text = String.Format("{0:C}", item.ValorCancelamento);
                    }
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Repeater DataBound", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Solicitar Cancelamentos - Repeater DataBound", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento de criação da tabela com os dados dos itens cancelados
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptCancelamentosComprovante_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (SolicitacaoCancelamento)e.Item.DataItem;

                var ltlNumeroEstabelecimentoCancelamento = (Literal)e.Item.FindControl("ltlNumeroEstabelecimentoCancelamento");
                var ltlNumeroEstabelecimentoVenda = (Literal)e.Item.FindControl("ltlNumeroEstabelecimentoVenda");
                var ltlTipoVenda = (Literal)e.Item.FindControl("ltlTipoVenda");
                var ltlDataVenda = (Literal)e.Item.FindControl("ltlDataVenda");
                var ltlNSU = (Literal)e.Item.FindControl("ltlNSU");
                var ltlValorBrutoVenda = (Literal)e.Item.FindControl("ltlValorBrutoVenda");
                var ltlSaldoDisponivel = (Literal)e.Item.FindControl("ltlSaldoDisponivel");
                var ltlTipoCancelamento = (Literal)e.Item.FindControl("ltlTipoCancelamento");
                var ltlValorCancelamento = (Literal)e.Item.FindControl("ltlValorCancelamento");
                var ltlNumeroAvisoCancelamento = (Literal)e.Item.FindControl("ltlNumeroAvisoCancelamento");

                ltlNumeroEstabelecimentoCancelamento.Text = SessaoAtual.CodigoEntidade.ToString();
                ltlNumeroEstabelecimentoVenda.Text = item.NumeroEstabelecimentoVenda.ToString();
                ltlTipoVenda.Text = item.TipoVendaDetalhado;
                ltlDataVenda.Text = item.DataVenda.ToString("dd/MM/yyyy");
                ltlNSU.Text = MascaraCartao(item.NSU);
                ltlValorBrutoVenda.Text = String.Format("{0:C}", item.ValorBruto);
                ltlSaldoDisponivel.Text = String.Format("{0:C}", item.SaldoDisponivel);
                ltlTipoCancelamento.Text = item.TipoCancelamento.GetDescription();
                ltlValorCancelamento.Text = String.Format("{0:C}", item.ValorCancelamento);
                if (item.NumeroAvisoCancelamento.Trim() == "0")
                    ltlNumeroAvisoCancelamento.Text = "Não Cancelado";
                else
                    ltlNumeroAvisoCancelamento.Text = item.NumeroAvisoCancelamento;

            }
        }

        /// <summary>
        /// Evento de criação da tabela de erros de validação das solicitações de cancelamento em lote
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void rptErroEmLote_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (Validacao)e.Item.DataItem;
                var linha = e.Item.ItemIndex + 1;

                var ltlLinha = (Literal)e.Item.FindControl("ltlLinha");
                var ltlMensagemErro = (Literal)e.Item.FindControl("ltlMensagemErro");

                ltlLinha.Text = linha.ToString();
                ltlMensagemErro.Text = !String.IsNullOrEmpty(item.Descricao) ? item.Descricao : "Venda com dados corretos.";

                if (item.Status == 1)
                    ((System.Web.UI.WebControls.Image)e.Item.FindControl("imgIcoAprovado")).Visible = true;
                else
                    ((System.Web.UI.WebControls.Image)e.Item.FindControl("imgIcoNaoAprovado")).Visible = true;
            }
        }

        /// <summary>
        /// Evento do botão Download Excel
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void lbtExcel_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Anular Cancelamentos - Gerar Excel"))
            {
                try
                {
                    Response.Clear();
                    Response.AddHeader("content-disposition", "attachment;filename=comprovanteCancelamento.xls");
                    Response.ContentEncoding = System.Text.Encoding.GetEncoding(1252);
                    Response.ContentType = "application/ms-excel";

                    System.IO.StringWriter stringWrite = new System.IO.StringWriter();
                    System.Web.UI.HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

                    MontaComprovanteExcel(htmlWrite);

                    Response.Write(stringWrite.ToString());
                    Response.End();
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Gerar Excel", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Anular Cancelamentos - Gerar Excel", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento do botão download do comprovante em pdf
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void lbtPDF_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Solicitar Cancelamento - Gerar PDF"))
            {
                try
                {
                    var ms = MontaComprovantePDF();

                    Byte[] file = ms.GetBuffer();
                    String nomeArquivo = String.Format("attachment; filename=Comprovante_{0}.pdf", DateTime.Now.ToString("ddMMyyyy_hhmmss"));

                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", nomeArquivo);
                    Response.BinaryWrite(file);

                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Gerar Excel", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Gerar Excel", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnProsseguir_Click(object sender, EventArgs e)
        {
            Cancelar();
        }
        #endregion

        #region [ Métodos ]


        /// <summary>
        /// Recebe valor do cartão ou NSU, verifica se é cartão e se for adiciona mascara de segurança de cartão
        /// </summary>
        /// <param name="strCartaoNsu">Valor do cartão ou NSU</param>
        /// <returns>Retorna estring de valor de cartão com mascara de segurança</returns>
        protected string MascaraCartao(string strCartaoNsu)
        {
            //Se caracteres > 12 é um cartão de c´redito, se não é NSU e não mascara
            if (strCartaoNsu.Count() > 12)
            {
                strCartaoNsu = String.Format("{0} - {1}", strCartaoNsu.Substring(0, 6), strCartaoNsu.Substring(strCartaoNsu.Length - 4, 4));
            }

            return strCartaoNsu;

        }

        /// <summary>
        /// Método que agrupa as funções executadas ao avançar para o passo 2
        /// </summary>
        private void AvancarParaPasso2()
        {
            PreencheSolicitacoesCancelamento();
            ChangeView(vwConfirmacao);
        }

        /// <summary>
        /// Muda a view exibida
        /// </summary>
        /// <param name="view">Visualização que deve ser exibida</param>
        private void ChangeView(View view)
        {
            mvwSolicitarCancelamento.SetActiveView(view);
        }

        /// <summary>
        /// Atualiza o cabeçalho para o passo escolhido
        /// </summary>
        /// <param name="activeView">Visualicação ativa</param>
        private void AtualizaCabecalhoPassoAtual(View activeView)
        {
            if (activeView == vwDadosCancelamento)
                assistentePassos.AtivarPasso(0);
            else if (activeView == vwConfirmacao)
                assistentePassos.AtivarPasso(1);
            else if (activeView == vwComprovante)
                assistentePassos.AtivarPasso(2);
        }

        /// <summary>
        /// Carrega os dados dos controles do passo 1
        /// </summary>
        private void CarregaDadosCancelamento()
        {
            //lblTitulo.Descricao = "Solicitar Cancelamento de Vendas";
            this.CancelamentoEmLote = false;
        }

        /// <summary>
        /// Carrega os dados dos controles do passo 2
        /// </summary>
        private void CarregaDadosConfirmacao()
        {
            CarregaConfirmacaoSolicitacoesCancelamento();
        }

        /// <summary>
        /// Carrega os dados dos controles do passo 3
        /// </summary>
        private void CarregaDadosComprovante()
        {
            //if (lblTitulo.Descricao.Equals("Solicitar Cancelamento de Vendas por Lote"))
            //    lblTitulo.Descricao = "Comprovante de Cancelamento de Vendas por Lote";
            //else
            //    lblTitulo.Descricao = "Comprovante de Cancelamento de Vendas";

            CarregaDadosImpressaoComprovante();
            CarregaComprovanteSolicitacoesCancelamento();
        }

        /// <summary>
        /// Carrega os dados dos controles que estão no comprovante para impressão
        /// </summary>
        private void CarregaDadosImpressaoComprovante()
        {
            ltlUsuarioImpressao.Text = SessaoAtual.LoginUsuario;

            ltlDataCancelamentoImpressao.Text = DateTime.Now.ToString("dd/MM/yyyy");
            ltlNomeEstabelecimentoImpressao.Text = SessaoAtual.NomeEntidade;
            ltlNumeroEstabelecimentoImpressao.Text = SessaoAtual.CodigoEntidade.ToString();
            ltlEnderecoImpressao.Text = Services.FormataEnderecoPV(SessaoAtual.CodigoEntidade, "E");

        }

        /// <summary>
        /// Carrega os dados dos controle do passo 2 em lote
        /// </summary>
        private void CarregaDadosErrosEmLote()
        {
            //lblTitulo.Descricao = "Solicitar Cancelamento de Vendas por Lote";
            this.CancelamentoEmLote = true;
            CarregaTabelaDeErrosEmLote(ValidacoesEmLote);
        }

        /// <summary>
        /// Carrega tabela de Erros no arquivo de solicitações em lote
        /// </summary>
        /// <param name="validacoesEmLote">Lista de validações do cancelamento em lote</param>
        private void CarregaTabelaDeErrosEmLote(List<Validacao> validacoesEmLote)
        {
            rptErroEmLote.DataSource = validacoesEmLote;
            rptErroEmLote.DataBind();
        }

        /// <summary>
        /// Carrega tabela de Solicitações de Cancelamento na tela de Confirmação
        /// </summary>
        private void CarregaConfirmacaoSolicitacoesCancelamento()
        {
            rptCancelamentosConfirmacao.DataSource = SolicitacoesCancelamento;
            rptCancelamentosConfirmacao.DataBind();
        }

        /// <summary>
        /// Carrega tabela de Solicitações de Cancelamento na tela de Comprovante
        /// </summary>
        private void CarregaComprovanteSolicitacoesCancelamento()
        {
            rptCancelamentosComprovante.DataSource = SolicitacoesCancelamento;
            rptCancelamentosComprovante.DataBind();

            // Mostra mensagem de atenção caso haja alguma solicitação de cancelamento de débito
            if (SolicitacoesCancelamento.Any(s => s.TipoVenda == TipoVenda.Debito))
                ltlMensagemAtencao.Visible = true;
        }

        /// <summary>
        /// Remove uma solicitação de cancelamento da lista em memória
        /// </summary>
        /// <param name="itemIndex">Posição do cancelamento que deve ser removido da lista de cancelamentos em memória</param>
        private void RemoveSolicitacao(Int32 itemIndex)
        {
            SolicitacoesCancelamento.RemoveAt(itemIndex);
        }

        /// <summary>
        /// Inclui as solicitações na ViewState de Solicitações de Cancelamento
        /// </summary>
        private void PreencheSolicitacoesCancelamento()
        {
            SolicitacoesCancelamento.Clear();
            LinhasPreenchidas.Clear();

            if (!String.IsNullOrEmpty(txtValorBrutoVenda1.Text) && !String.IsNullOrEmpty(txtSaldoDisponivel1.Text))
            {
                var solicitacao = PreencheSolicitacaoCancelamento(txtNumeroEstabelecimento1.Text.ToInt32(),
                    ddlTipoVenda1.SelectedValue.GetEnumValueFromDescription<TipoVenda>(),
                    txtDataVenda1.Text.ToDate("dd/MM/yyyy"),
                    txtNumeroCartaoNsuCv1.Text,
                    txtValorBrutoVenda1.Text.ToDecimal(),
                    txtSaldoDisponivel1.Text.ToDecimal(),
                    ddlTipoCancelamento1.SelectedValue.GetEnumValueFromDescription<TipoCancelamento>(),
                    txtValorCancelamento1.Text.ToDecimal(),
                    hdNumeroMes1.Value,
                    hdTimestampTransacao1.Value,
                    hdTipoTransacao1.Value,
                    hdTipoVendaDetalhado1.Value);

                SolicitacoesCancelamento.Add(solicitacao);
                LinhasPreenchidas.Add(0);
            }

            if (!String.IsNullOrEmpty(txtValorBrutoVenda2.Text) && !String.IsNullOrEmpty(txtSaldoDisponivel2.Text))
            {
                var solicitacao = PreencheSolicitacaoCancelamento(txtNumeroEstabelecimento2.Text.ToInt32(),
                    ddlTipoVenda2.SelectedValue.GetEnumValueFromDescription<TipoVenda>(),
                    txtDataVenda2.Text.ToDate("dd/MM/yyyy"),
                    txtNumeroCartaoNsuCv2.Text,
                    txtValorBrutoVenda2.Text.ToDecimal(),
                    txtSaldoDisponivel2.Text.ToDecimal(),
                    ddlTipoCancelamento2.SelectedValue.GetEnumValueFromDescription<TipoCancelamento>(),
                    txtValorCancelamento2.Text.ToDecimal(),
                    hdNumeroMes2.Value,
                    hdTimestampTransacao2.Value,
                    hdTipoTransacao2.Value,
                    hdTipoVendaDetalhado2.Value);

                SolicitacoesCancelamento.Add(solicitacao);
                LinhasPreenchidas.Add(1);
            }

            if (!String.IsNullOrEmpty(txtValorBrutoVenda3.Text) && !String.IsNullOrEmpty(txtSaldoDisponivel3.Text))
            {
                var solicitacao = PreencheSolicitacaoCancelamento(txtNumeroEstabelecimento3.Text.ToInt32(),
                    ddlTipoVenda3.SelectedValue.GetEnumValueFromDescription<TipoVenda>(),
                    txtDataVenda3.Text.ToDate("dd/MM/yyyy"),
                    txtNumeroCartaoNsuCv3.Text,
                    txtValorBrutoVenda3.Text.ToDecimal(),
                    txtSaldoDisponivel3.Text.ToDecimal(),
                    ddlTipoCancelamento3.SelectedValue.GetEnumValueFromDescription<TipoCancelamento>(),
                    txtValorCancelamento3.Text.ToDecimal(),
                    hdNumeroMes3.Value,
                    hdTimestampTransacao3.Value,
                    hdTipoTransacao3.Value,
                    hdTipoVendaDetalhado3.Value);

                SolicitacoesCancelamento.Add(solicitacao);
                LinhasPreenchidas.Add(2);
            }

            if (!String.IsNullOrEmpty(txtValorBrutoVenda4.Text) && !String.IsNullOrEmpty(txtSaldoDisponivel4.Text))
            {
                var solicitacao = PreencheSolicitacaoCancelamento(txtNumeroEstabelecimento4.Text.ToInt32(),
                    ddlTipoVenda4.SelectedValue.GetEnumValueFromDescription<TipoVenda>(),
                    txtDataVenda4.Text.ToDate("dd/MM/yyyy"),
                    txtNumeroCartaoNsuCv4.Text,
                    txtValorBrutoVenda4.Text.ToDecimal(),
                    txtSaldoDisponivel4.Text.ToDecimal(),
                    ddlTipoCancelamento4.SelectedValue.GetEnumValueFromDescription<TipoCancelamento>(),
                    txtValorCancelamento4.Text.ToDecimal(),
                    hdNumeroMes4.Value,
                    hdTimestampTransacao4.Value,
                    hdTipoTransacao4.Value,
                    hdTipoVendaDetalhado4.Value);

                SolicitacoesCancelamento.Add(solicitacao);
                LinhasPreenchidas.Add(3);
            }

            if (!String.IsNullOrEmpty(txtValorBrutoVenda5.Text) && !String.IsNullOrEmpty(txtSaldoDisponivel5.Text))
            {
                var solicitacao = PreencheSolicitacaoCancelamento(txtNumeroEstabelecimento5.Text.ToInt32(),
                    ddlTipoVenda5.SelectedValue.GetEnumValueFromDescription<TipoVenda>(),
                    txtDataVenda5.Text.ToDate("dd/MM/yyyy"),
                    txtNumeroCartaoNsuCv5.Text,
                    txtValorBrutoVenda5.Text.ToDecimal(),
                    txtSaldoDisponivel5.Text.ToDecimal(),
                    ddlTipoCancelamento5.SelectedValue.GetEnumValueFromDescription<TipoCancelamento>(),
                    txtValorCancelamento5.Text.ToDecimal(),
                    hdNumeroMes5.Value,
                    hdTimestampTransacao5.Value,
                    hdTipoTransacao5.Value,
                    hdTipoVendaDetalhado5.Value);

                SolicitacoesCancelamento.Add(solicitacao);
                LinhasPreenchidas.Add(4);
            }
        }

        /// <summary>
        /// Retorna um objeto do tipo SolicitacaoCancelamento com os dados preenchidos de acordo com os parâmetros passados
        /// </summary>
        /// <param name="numeroEstabelecimentoVenda">Número do estabelecimento da venda</param>
        /// <param name="tipoVenda">Tipo da venda</param>
        /// <param name="dataVenda">Data da venda</param>
        /// <param name="nsu">Número de sequência único</param>
        /// <param name="valorBruto">Valor bruto</param>
        /// <param name="saldoDisponivel">Saldo disponível</param>
        /// <param name="tipoCancelamento">Tipo de cancelamento</param>
        /// <param name="valorCancelamento">Valor do cancelamento</param>
        /// <returns></returns>
        private SolicitacaoCancelamento PreencheSolicitacaoCancelamento(Int32 numeroEstabelecimentoVenda, TipoVenda tipoVenda, DateTime dataVenda, String nsu, Decimal valorBruto, Decimal saldoDisponivel, TipoCancelamento tipoCancelamento, Decimal valorCancelamento, String numeroMes, String timestampTransacao, String tipoTransacao, String tipoVendaDetalhado)
        {
            return new SolicitacaoCancelamento
            {
                NumeroEstabelecimentoVenda = numeroEstabelecimentoVenda,
                TipoVenda = tipoVenda,
                DataVenda = dataVenda,
                NSU = nsu,
                ValorBruto = valorBruto,
                SaldoDisponivel = saldoDisponivel,
                TipoCancelamento = tipoCancelamento,
                ValorCancelamento = valorCancelamento,
                NumeroMes = !String.IsNullOrEmpty(numeroMes) ? numeroMes.ToInt16() : default(Int16),
                TimestampTransacao = timestampTransacao,
                TipoTransacao = (TipoTransacao)tipoTransacao.ToInt32(),
                CodigoRamoAtividade = SessaoAtual.CodigoRamoAtividade,
                TipoVendaDetalhado = tipoVendaDetalhado
            };
        }

        /// <summary>
        /// Valida as solicitações
        /// </summary>
        /// <param name="SolicitacoesCancelamento">Lista de solicitações de cancelamentos</param>
        /// <returns></returns>
        private List<Validacao> ValidaSolicitacoesCancelamento(List<SolicitacaoCancelamento> solicitacoesCancelamento)
        {
            String dados = this.DadosQueryString;
            return Services.ValidarCancelamentos('C', solicitacoesCancelamento, dados);
        }

        /// <summary>
        /// Cancela as solicitações enviadas
        /// </summary>
        /// <param name="SolicitacoesCancelamento">Lista de solicitações de cancelamentos</param>
        private void CancelaSolicitacoes(List<SolicitacaoCancelamento> solicitacoesCancelamento)
        {
            SolicitacoesCancelamento = Services.IncluirCancelamentos(SessaoAtual.LoginUsuario, solicitacoesCancelamento);

        }

        /// <summary>
        /// Inclui cancelamento no PN
        /// </summary>
        /// <param name="SolicitacoesCancelamento">Lista de solicitações de cancelamentos</param>
        private void IncluiCancelamentosPN(List<SolicitacaoCancelamento> SolicitacoesCancelamento)
        {
            try
            {

                String address = String.Empty;
                HttpRequest request = base.Request;
                address = request.ServerVariables["HTTP_TRUE_CLIENT_IP"];
                if (String.IsNullOrEmpty(address))
                    address = request.ServerVariables["REMOTE_ADDR"];

                Services.IncluirCancelamentosPn(SessaoAtual.LoginUsuario, address, SolicitacoesCancelamento);

            }
            catch (FaultException<CancelamentoServico.GeneralFault> ex)
            {
                Logger.GravarErro("Incluir Cancelamentos - Incluir Cancelamentos Selecionados", ex);
                SharePointUlsLog.LogErro(ex);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Incluir Cancelamentos - Incluir Cancelamentos Selecionados", ex);
                SharePointUlsLog.LogErro(ex);
            }
        }

        /// <summary>
        /// Carrega Controles da página
        /// </summary>
        private void CarregaControles()
        {
            CarregaDropDownListsTipoVenda();
            CarregaDropDownListsTipoCancelamento();
            CarregaCodigoRamoAtividade(SessaoAtual.CodigoRamoAtividade);
            DesabilitaCampos();
        }

        /// <summary>
        /// Carrega o código do ramo de atividade do estabelecimento em um controle hidden na página
        /// </summary>
        /// <param name="codigoRamoAtividade">Código do ramo de atividade</param>
        private void CarregaCodigoRamoAtividade(Int32 codigoRamoAtividade)
        {
            hdCodigoRamoAtividade.Value = codigoRamoAtividade.ToString();
        }

        /// <summary>
        /// Carrega o valor das drop down lists de tipo de cancelamento
        /// </summary>
        private void CarregaDropDownListsTipoCancelamento()
        {
            var lista = MontaListaDeItemsEnum<TipoCancelamento>();

            ddlTipoCancelamento1.DataSource = lista;
            ddlTipoCancelamento1.DataBind();
            ddlTipoCancelamento2.DataSource = lista;
            ddlTipoCancelamento2.DataBind();
            ddlTipoCancelamento3.DataSource = lista;
            ddlTipoCancelamento3.DataBind();
            ddlTipoCancelamento4.DataSource = lista;
            ddlTipoCancelamento4.DataBind();
            ddlTipoCancelamento5.DataSource = lista;
            ddlTipoCancelamento5.DataBind();
        }

        /// <summary>
        /// Carrega o valor das drop down lists de tipo de venda
        /// </summary>
        private void CarregaDropDownListsTipoVenda()
        {
            var lista = MontaListaDeItemsEnum<Modelos.TipoVenda>();

            ddlTipoVenda1.Items.AddRange(lista.ToArray());
            ddlTipoVenda2.Items.AddRange(lista.ToArray());
            ddlTipoVenda3.Items.AddRange(lista.ToArray());
            ddlTipoVenda4.Items.AddRange(lista.ToArray());
            ddlTipoVenda5.Items.AddRange(lista.ToArray());
        }

        /// <summary>
        /// Desabilita campos que não podem ser modificados de acordo com regras
        /// </summary>
        private void DesabilitaCampos()
        {
            Boolean isCentralizado = Services.IsCentralizado(SessaoAtual.CodigoEntidade);

            if (isCentralizado)
            {
                hdNumeroEstabelecimentoCentralizadora.Value = SessaoAtual.CodigoEntidade.ToString();
            }
            else
            {
                hdCentralizadora.Value = "false";

                txtNumeroEstabelecimento1.Text = SessaoAtual.CodigoEntidade.ToString();
                txtNumeroEstabelecimento1.Enabled = false;

                txtNumeroEstabelecimento2.Text = SessaoAtual.CodigoEntidade.ToString();
                txtNumeroEstabelecimento2.Enabled = false;

                txtNumeroEstabelecimento3.Text = SessaoAtual.CodigoEntidade.ToString();
                txtNumeroEstabelecimento3.Enabled = false;

                txtNumeroEstabelecimento4.Text = SessaoAtual.CodigoEntidade.ToString();
                txtNumeroEstabelecimento4.Enabled = false;

                txtNumeroEstabelecimento5.Text = SessaoAtual.CodigoEntidade.ToString();
                txtNumeroEstabelecimento5.Enabled = false;
            }
        }

        /// <summary>
        /// Dada uma enum, monta uma Lista de ListItem com o valor das descrições ou com o nome do item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private List<System.Web.UI.WebControls.ListItem> MontaListaDeItemsEnum<T>()
        {
            List<System.Web.UI.WebControls.ListItem> lista = new List<System.Web.UI.WebControls.ListItem>();
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();

            foreach (var field in type.GetFields())
            {
                if (field.FieldType == typeof(T))
                {
                    var attribute = Attribute.GetCustomAttribute(field,
                        typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attribute != null)
                    {
                        lista.Add(new System.Web.UI.WebControls.ListItem(attribute.Description, field.Name));
                    }
                    else
                    {
                        lista.Add(new System.Web.UI.WebControls.ListItem(field.Name));
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Busca o tipo de cancelamento para os dados enviados pelo arquivo em lote
        /// </summary>
        /// <param name="tipoCancelamento">Tipo de cancelamento</param>
        /// <returns></returns>
        private TipoCancelamento GetTipoCancelamentoPorSigla(String tipoCancelamento)
        {
            switch (tipoCancelamento)
            {
                case "T": return TipoCancelamento.Total;
                case "P": return TipoCancelamento.Parcial;
                default: return TipoCancelamento.Total;
            }
        }

        /// <summary>
        /// Busca o tipo de venda para os dados enviados pelo arquivo em lote
        /// </summary>
        /// <param name="tipoVenda">Tipo de venda</param>
        /// <returns></returns>
        private TipoVenda GetTipoVendaPorSigla(String tipoVenda)
        {
            switch (tipoVenda)
            {
                case "C": return TipoVenda.Credito;
                case "RO": return TipoVenda.Credito;
                case "PC": return TipoVenda.Credito;
                case "PS": return TipoVenda.Credito;
                case "DA": return TipoVenda.Debito;
                case "DP": return TipoVenda.Debito;
                case "D": return TipoVenda.Debito;
                default: return TipoVenda.Credito;
            }
        }

        /// <summary>
        /// Verifica se a extensão do arquivo é .txt
        /// </summary>
        /// <param name="extensao">Extensão do arquivo</param>
        /// <returns></returns>
        private Boolean VerificaExtensaoArquivo(String extensao)
        {
            if (extensao.ToLowerInvariant().Equals("txt"))
                return true;

            return false;
        }

        /// <summary>
        /// Lê conteúdo do arquivo linha a linha e põe dados na memória
        /// </summary>
        /// <param name="stream">Objeto Stream</param>
        private void LerArquivoCancelamentoEmLote(Stream stream)
        {
            // Limpa Solicitações anteriores da memória
            SolicitacoesCancelamento.Clear();
            ValidacoesEmLote.Clear();

            StreamReader reader = new StreamReader(stream);
            Int32 lineCounter = default(Int32);

            while (reader.Peek() != -1 && lineCounter < 50)
            {
                var solicitacao = new SolicitacaoCancelamento();
                var line = reader.ReadLine();
                lineCounter++;
                var valores = line.Trim().Split(';');

                Validacao validacao = new Validacao();
                if (valores.Length == 7 || (valores.Length == 8 && valores[7] == String.Empty))
                {
                    validacao = ValidaDados(valores);
                    if (validacao.Status == 1)
                    {
                        solicitacao.CodigoRamoAtividade = SessaoAtual.CodigoRamoAtividade;
                        solicitacao.NumeroEstabelecimentoVenda = valores[0].ToInt32();
                        solicitacao.NSU = valores[1];
                        solicitacao.DataVenda = valores[2].ToDate("ddMMyyyy");
                        solicitacao.ValorBruto = valores[3].ToDecimal();
                        solicitacao.TipoCancelamento = GetTipoCancelamentoPorSigla(valores[4]);
                        solicitacao.ValorCancelamento = valores[5].ToDecimal();
                        solicitacao.TipoVenda = GetTipoVendaPorSigla(valores[6]);

                        Boolean estabelecimentoEmCentralizadora = Services.VerificarEstabelecimentoEmCentralizadora(solicitacao.NumeroEstabelecimentoVenda, SessaoAtual.CodigoEntidade);

                        if (estabelecimentoEmCentralizadora)
                        {
                            var solicitacoesCancelamentos = Services.BuscarTransacaoDuplicadaParaCancelamento(solicitacao, this.DadosQueryString);
                            if (solicitacoesCancelamentos.Count == 0)
                            {
                                validacao.Status = 2;
                                validacao.Descricao = "Venda não encontrada, verifique os dados que foram digitados";
                            }
                            else if (solicitacoesCancelamentos.Count == 1)
                            {
                                var solicitacaoDuplicada = solicitacoesCancelamentos.FirstOrDefault();
                                solicitacao.NumeroMes = solicitacaoDuplicada.NumeroMes;
                                solicitacao.TimestampTransacao = solicitacaoDuplicada.TimestampTransacao;
                                solicitacao.TipoTransacao = solicitacaoDuplicada.TipoTransacao;
                                solicitacao.TipoVendaDetalhado = solicitacaoDuplicada.TipoVendaDetalhado;

                                var solicitacaoCancelamento = Services.BuscarTransacaoParaCancelamento(solicitacao, this.DadosQueryString);
                                solicitacao.SaldoDisponivel = solicitacaoCancelamento.SaldoDisponivel;

                                SolicitacoesCancelamento.Add(solicitacao);
                            }
                            else
                            {
                                validacao.Status = 2;
                                validacao.Descricao = "Venda duplicada. Não é possível realizar o cancelamento desta venda em lote, apenas digitando os dados da venda no passo anterior";
                            }
                        }
                        else
                        {
                            validacao.Status = 2;
                            validacao.Descricao = "Número do estabelecimento incorreto.";
                        }
                    }
                }
                else
                {
                    validacao.Status = 2;
                    validacao.Descricao = "Dados preenchidos incorretamente, por favor verifique.";
                }

                ValidacoesEmLote.Add(validacao);
            }
        }

        /// <summary>
        /// Valida as solicitações em Lote e redireciona para a página correta
        /// </summary>
        private void RedirecionaProximoPassoEmLote()
        {
            if (ValidacoesEmLote.All(v => v.Status == 1))
                ValidacoesEmLote = ValidaSolicitacoesCancelamento(SolicitacoesCancelamento);

            if (ValidacoesEmLote.All(v => v.Status == 1))
                this.CancelamentoEmLote = true;
            //lblTitulo.Descricao = "Solicitar Cancelamento de Vendas por Lote";

            if (ValidacoesEmLote.All(v => v.Status == 1))
                ChangeView(vwConfirmacao);
            else
                ChangeView(vwErroEmLote);
        }

        /// <summary>
        /// Monta Comprovante Excel
        /// </summary>
        /// <param name="htmlWrite">Objeto Html Writer</param>
        private void MontaComprovanteExcel(HtmlTextWriter htmlWrite)
        {
            htmlWrite.Write("<div>");
            htmlWrite.Write("<strong>COMPROVANTE DE CANCELAMENTO DE VENDAS</strong>");
            htmlWrite.Write("<br /><br />");
            htmlWrite.Write(String.Format(@"Data da Consulta: {0}", DateTime.Now.ToString("dd/MM/yyyy HH:mm")));
            htmlWrite.Write("<br />");
            htmlWrite.Write(String.Format(@"Usuário: {0}", SessaoAtual.LoginUsuario));
            htmlWrite.Write("<br /><br />");
            htmlWrite.Write(String.Format(@"Nome do Estabelecimento: {0}", SessaoAtual.NomeEntidade));
            htmlWrite.Write("<br />");
            htmlWrite.Write(String.Format(@"Endereço: {0}", Services.FormataEnderecoPV(SessaoAtual.CodigoEntidade, "E")));
            htmlWrite.Write("<br />");
            htmlWrite.Write(String.Format(@"Nº do Estabelcimento: {0}", SessaoAtual.CodigoEntidade));
            htmlWrite.Write("<br />");
            htmlWrite.Write(String.Format(@"Data do Cancelamento: {0}", DateTime.Now.ToString("dd/MM/yyyy")));
            htmlWrite.Write("</div>");
            htmlWrite.Write("<br />");

            rptCancelamentosComprovante.RenderControl(htmlWrite);
        }

        /// <summary>
        /// Monta comprovante em PDF
        /// </summary>
        private MemoryStream MontaComprovantePDF()
        {
            Document doc = new Document();
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

            doc.Open();

            // Adiciona o Header
            String headerPath = SPUtility.GetGenericSetupPath(@"Template\Layouts\Rede.PN.Cancelamento\Images\Header_Canc_Vendas.jpg");
            byte[] arrayHeader = File.ReadAllBytes(headerPath);
            iTextSharp.text.Image headerImage = iTextSharp.text.Image.GetInstance(arrayHeader);
            doc.Add(headerImage);

            // Header da direita
            PdfPTable userInfoTable = new PdfPTable(2);
            userInfoTable.SpacingBefore = 10f;
            userInfoTable.HeaderRows = 0;
            userInfoTable.HorizontalAlignment = 2;
            userInfoTable.WidthPercentage = 100f;
            float[] userInfoTableWidths = new float[] { 80f, 20f };
            userInfoTable.SetWidths(userInfoTableWidths);

            userInfoTable.AddCell((new PdfPCell(new Phrase(new Phrase("Data da consulta:",
                new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, new BaseColor(84, 84, 84)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(255, 255, 255),
                HorizontalAlignment = PdfPCell.ALIGN_RIGHT,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                Border = 0
            }));

            userInfoTable.AddCell((new PdfPCell(new Phrase(new Phrase(String.Format("{0} às {1}", DateTime.Now.ToString("dd/MM/yyyy"), DateTime.Now.ToString("HH:mm")),
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(255, 255, 255),
                HorizontalAlignment = PdfPCell.ALIGN_LEFT,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                Border = 0
            }));

            userInfoTable.AddCell((new PdfPCell(new Phrase(new Phrase("Usuário:",
                new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, new BaseColor(84, 84, 84)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(255, 255, 255),
                HorizontalAlignment = PdfPCell.ALIGN_RIGHT,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                Border = 0
            }));

            userInfoTable.AddCell((new PdfPCell(new Phrase(new Phrase(SessaoAtual.LoginUsuario,
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(255, 255, 255),
                HorizontalAlignment = PdfPCell.ALIGN_LEFT,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                Border = 0
            }));

            doc.Add(userInfoTable);

            // Tabela Principal
            PdfPTable mainTable = new PdfPTable(1);
            mainTable.HeaderRows = 1;
            mainTable.WidthPercentage = 98f;
            mainTable.SpacingAfter = 10f;

            mainTable.AddCell((new PdfPCell(new Phrase(new Phrase("COMPROVANTE DE CANCELAMENTO DE VENDAS",
                new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, new BaseColor(255, 255, 255)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(102, 102, 102),
                HorizontalAlignment = PdfPCell.ALIGN_LEFT,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            }));

            PdfPTable cancelamentosTable = new PdfPTable(10);

            cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase("Nº do estabelec. do Cancel.",
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(0, 0, 0)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(224, 224, 224),
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            }));

            cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase("Nº do estabelec. da Venda",
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(0, 0, 0)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(224, 224, 224),
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            }));

            cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase("Tipo de Venda",
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(0, 0, 0)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(224, 224, 224),
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            }));

            cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase("Data da Venda",
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(0, 0, 0)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(224, 224, 224),
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            }));

            cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase("Nº do Cartão ou NSU/CV",
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(0, 0, 0)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(224, 224, 224),
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            }));

            cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase("Valor Bruto da Venda",
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(0, 0, 0)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(224, 224, 224),
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            }));

            cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase("Saldo Disponível",
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(0, 0, 0)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(224, 224, 224),
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            }));

            cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase("Tipo de Cancel.",
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(0, 0, 0)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(224, 224, 224),
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            }));

            cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase("Valor de Cancel.",
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(0, 0, 0)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(224, 224, 224),
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthRight = 0,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            }));

            cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase("Nº do Aviso de Cancelamento",
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(0, 0, 0)))))
            {
                Padding = 5,
                BackgroundColor = new BaseColor(224, 224, 224),
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                BorderWidthLeft = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            }));

            var rowCount = 0;
            foreach (var item in SolicitacoesCancelamento)
            {
                cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase(SessaoAtual.CodigoEntidade.ToString(),
                new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84)))))
                {
                    Padding = 5,
                    BackgroundColor = rowCount % 2 == 1 ? new BaseColor(221, 221, 221) : new BaseColor(255, 255, 255),
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                    VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = rowCount == SolicitacoesCancelamento.Count - 1 ? 0.5f : 0,
                    BorderColor = new BaseColor(163, 163, 163)
                }));

                cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase(item.NumeroEstabelecimentoVenda.ToString(),
                    new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84)))))
                {
                    Padding = 5,
                    BackgroundColor = rowCount % 2 == 1 ? new BaseColor(221, 221, 221) : new BaseColor(255, 255, 255),
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                    VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                    BorderWidthRight = 0,
                    BorderWidthLeft = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = rowCount == SolicitacoesCancelamento.Count - 1 ? 0.5f : 0,
                    BorderColor = new BaseColor(163, 163, 163)
                }));

                cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase(item.TipoVendaDetalhado,
                    new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84)))))
                {
                    Padding = 5,
                    BackgroundColor = rowCount % 2 == 1 ? new BaseColor(221, 221, 221) : new BaseColor(255, 255, 255),
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                    VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                    BorderWidthRight = 0,
                    BorderWidthLeft = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = rowCount == SolicitacoesCancelamento.Count - 1 ? 0.5f : 0,
                    BorderColor = new BaseColor(163, 163, 163)
                }));

                cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase(item.DataVenda.ToString("dd/MM/yyyy"),
                    new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84)))))
                {
                    Padding = 5,
                    BackgroundColor = rowCount % 2 == 1 ? new BaseColor(221, 221, 221) : new BaseColor(255, 255, 255),
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                    VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                    BorderWidthRight = 0,
                    BorderWidthLeft = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = rowCount == SolicitacoesCancelamento.Count - 1 ? 0.5f : 0,
                    BorderColor = new BaseColor(163, 163, 163)
                }));

                cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase(MascaraCartao(item.NSU),
                    new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84)))))
                {
                    Padding = 5,
                    BackgroundColor = rowCount % 2 == 1 ? new BaseColor(221, 221, 221) : new BaseColor(255, 255, 255),
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                    VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                    BorderWidthRight = 0,
                    BorderWidthLeft = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = rowCount == SolicitacoesCancelamento.Count - 1 ? 0.5f : 0,
                    BorderColor = new BaseColor(163, 163, 163)
                }));

                cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase(String.Format(@"{0:C}", item.ValorBruto),
                    new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84)))))
                {
                    Padding = 5,
                    BackgroundColor = rowCount % 2 == 1 ? new BaseColor(221, 221, 221) : new BaseColor(255, 255, 255),
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                    VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                    BorderWidthRight = 0,
                    BorderWidthLeft = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = rowCount == SolicitacoesCancelamento.Count - 1 ? 0.5f : 0,
                    BorderColor = new BaseColor(163, 163, 163)
                }));

                cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase(String.Format(@"{0:C}", item.SaldoDisponivel),
                    new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84)))))
                {
                    Padding = 5,
                    BackgroundColor = rowCount % 2 == 1 ? new BaseColor(221, 221, 221) : new BaseColor(255, 255, 255),
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                    VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                    BorderWidthRight = 0,
                    BorderWidthLeft = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = rowCount == SolicitacoesCancelamento.Count - 1 ? 0.5f : 0,
                    BorderColor = new BaseColor(163, 163, 163)
                }));

                cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase(item.TipoCancelamento.GetDescription(),
                    new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84)))))
                {
                    Padding = 5,
                    BackgroundColor = rowCount % 2 == 1 ? new BaseColor(221, 221, 221) : new BaseColor(255, 255, 255),
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                    VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                    BorderWidthRight = 0,
                    BorderWidthLeft = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = rowCount == SolicitacoesCancelamento.Count - 1 ? 0.5f : 0,
                    BorderColor = new BaseColor(163, 163, 163)
                }));

                cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase(String.Format(@"{0:C}", item.ValorCancelamento),
                    new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84)))))
                {
                    Padding = 5,
                    BackgroundColor = rowCount % 2 == 1 ? new BaseColor(221, 221, 221) : new BaseColor(255, 255, 255),
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                    VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                    BorderWidthRight = 0,
                    BorderWidthLeft = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = rowCount == SolicitacoesCancelamento.Count - 1 ? 0.5f : 0,
                    BorderColor = new BaseColor(163, 163, 163)
                }));

                string strNumeroAviso = item.NumeroAvisoCancelamento.Trim() == "0" ? "Não Cancelado" : item.NumeroAvisoCancelamento;

                cancelamentosTable.AddCell((new PdfPCell(new Phrase(new Phrase(strNumeroAviso,
                    new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84)))))
                {
                    Padding = 5,
                    BackgroundColor = rowCount % 2 == 1 ? new BaseColor(221, 221, 221) : new BaseColor(255, 255, 255),
                    HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                    VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                    BorderWidthLeft = 0,
                    BorderWidthTop = 0,
                    BorderWidthBottom = rowCount == SolicitacoesCancelamento.Count - 1 ? 0.5f : 0,
                    BorderColor = new BaseColor(163, 163, 163)
                }));

                rowCount++;
            }

            var nomeEstabelecimentoPhrase = new Phrase();
            nomeEstabelecimentoPhrase.Add(new Chunk("Nome do Estabelecimento: ", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, new BaseColor(84, 84, 84))));
            nomeEstabelecimentoPhrase.Add(new Chunk(SessaoAtual.NomeEntidade, new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84))));
            mainTable.AddCell(new PdfPCell(nomeEstabelecimentoPhrase)
            {
                Padding = 5,
                BorderWidthTop = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            });

            var enderecoPhrase = new Phrase();
            enderecoPhrase.Add(new Chunk("Endereço: ", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, new BaseColor(84, 84, 84))));
            enderecoPhrase.Add(new Chunk(Services.FormataEnderecoPV(SessaoAtual.CodigoEntidade, "E"), new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84))));
            mainTable.AddCell(new PdfPCell(new Phrase(enderecoPhrase))
            {
                Padding = 5,
                BorderWidthTop = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            });

            var numeroEstabelecimentoPhrase = new Phrase();
            numeroEstabelecimentoPhrase.Add(new Chunk("Nº do Estabelecimento: ", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, new BaseColor(84, 84, 84))));
            numeroEstabelecimentoPhrase.Add(new Chunk(SessaoAtual.CodigoEntidade.ToString(), new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84))));
            mainTable.AddCell(new PdfPCell(numeroEstabelecimentoPhrase)
            {
                Padding = 5,
                BorderWidthTop = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            });

            var dataCancelamentoPhrase = new Phrase();
            dataCancelamentoPhrase.Add(new Chunk("Data do Cancelamento: ", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, new BaseColor(84, 84, 84))));
            dataCancelamentoPhrase.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, new BaseColor(84, 84, 84))));
            mainTable.AddCell(new PdfPCell(dataCancelamentoPhrase)
            {
                Padding = 5,
                BorderWidthTop = 0,
                BorderWidthBottom = 0,
                BorderColor = new BaseColor(163, 163, 163)
            });

            mainTable.AddCell(new PdfPCell(cancelamentosTable)
            {
                Padding = 5,
                BorderWidthTop = 0,
                BorderWidthBottom = SolicitacoesCancelamento.Any(s => s.TipoVenda == TipoVenda.Debito) ? 0 : 0.5f,
                BorderColor = new BaseColor(163, 163, 163)
            });

            if (SolicitacoesCancelamento.Any(s => s.TipoVenda == TipoVenda.Debito))
            {
                mainTable.AddCell(new PdfPCell(new Phrase("Atenção: O cancelamento de vendas com cartão de débito Maestro está sujeito a análise da bandeira em até 5 dias úteis. Visualize o status deste tipo de cancelamento no serviço Cancelamento de Vendas > Consultar Cancelamentos."
                , new Font(Font.FontFamily.HELVETICA, 11, Font.NORMAL, new BaseColor(84, 84, 84))))
                {
                    Padding = 5,
                    BorderWidthTop = 0,
                    BorderColor = new BaseColor(163, 163, 163)
                });
            }

            doc.Add(mainTable);

            // Adiciona o Footer
            String footerPath = SPUtility.GetGenericSetupPath(@"Template\Layouts\Rede.PN.Cancelamento\Images\Footer.jpg");
            byte[] arrayFooter = File.ReadAllBytes(footerPath);
            iTextSharp.text.Image footerImage = iTextSharp.text.Image.GetInstance(arrayFooter);
            doc.Add(footerImage);

            doc.Close();

            // Resize Page Height
            Document doc2 = new Document(new Rectangle(879, headerImage.Height + footerImage.Height + userInfoTable.TotalHeight + mainTable.TotalHeight), 0, 0, 0, 0);
            MemoryStream ms2 = new MemoryStream();
            PdfWriter writer2 = PdfWriter.GetInstance(doc2, ms2);

            doc2.Open();
            doc2.Add(headerImage);
            doc2.Add(userInfoTable);
            doc2.Add(mainTable);
            doc2.Add(footerImage);
            doc2.Close();

            return ms2;
        }

        /// <summary>
        /// Executa o cancelamento quando a entrada dele é a Home
        /// </summary>
        private void ExecutarCancelamentoHome(String origem, QueryStringSegura queryString)
        {
            var isCancelamentoHomeEmLote = String.Compare("HomePageCancelamentoLote", origem, true) == 0;
            var isCancelamentoHomeUnico = String.Compare("HomePageCancelamento", origem, true) == 0;

            //HomePage EMP/IBBA - Cancelamento em Lote
            if (isCancelamentoHomeEmLote) ExecutarCancelamentoHomeEmLote(queryString);

            //HomePage EMP/IBBA - Cancelamento de Venda
            if (isCancelamentoHomeUnico) ExecutarCancelamentoHomeUnico(queryString);
        }

        /// <summary>
        /// Executa Cancelamento de uma única transação vinda da Home
        /// </summary>
        /// <param name="queryString">Query string criptografada</param>
        private void ExecutarCancelamentoHomeUnico(QueryStringSegura queryString)
        {
            if (hdCentralizadora.Value != "false")
                txtNumeroEstabelecimento1.Text = queryString["NumeroPV"];

            ddlTipoVenda1.SelectedIndex = ddlTipoVenda1.Items.IndexOf(ddlTipoVenda1.Items.FindByValue(queryString["TipoVenda"]));
            txtNumeroCartaoNsuCv1.Text = queryString["Numero"];
            txtDataVenda1.Text = queryString["DataVenda"];

            String javaScript = "ExecuteOrDelayUntilScriptLoaded(function () { window.solictarCancelamentoObj.buscarTransacoesDuplicadas.call($('[id*=txtNumeroEstabelecimento1]')); }, 'SP.UI.Dialog.js');";
            ScriptManager.RegisterStartupScript(upCancelamentoConfirmacao, upCancelamentoConfirmacao.GetType(), "solicitarCancelamento", javaScript, true);

            //SolicitacaoCancelamento cancelamento = new SolicitacaoCancelamento();

            //txtNumeroEstabelecimento1.Text = queryString["NumeroPV"];
            //ddlTipoVenda1.SelectedIndex = ddlTipoVenda1.Items.IndexOf(ddlTipoVenda1.Items.FindByValue(queryString["TipoVenda"]));
            //txtNumeroCartaoNsuCv1.Text = queryString["Numero"];
            //txtDataVenda1.Text = queryString["DataVenda"];
            //txtValorBrutoVenda1.Text = String.Format(@"{0:C}", queryString["ValorVenda"].ToDecimal()).Replace("R$", "");

            //cancelamento.NumeroEstabelecimentoVenda = txtNumeroEstabelecimento1.Text.ToInt32();
            //cancelamento.TipoVenda = ddlTipoVenda1.SelectedValue.GetEnumValueFromDescription<TipoVenda>();
            //cancelamento.NSU = txtNumeroCartaoNsuCv1.Text;
            //cancelamento.DataVenda = txtDataVenda1.Text.ToDate("dd/MM/yyyy");
            //cancelamento.ValorBruto = txtValorBrutoVenda1.Text.ToDecimal();

            //var transacao = Services.BuscarTransacaoParaCancelamento(cancelamento);
            //txtSaldoDisponivel1.Text = txtValorCancelamento1.Text = String.Format(@"{0:C}", transacao.SaldoDisponivel).Replace("R$", "");
        }

        /// <summary>
        /// Executa o cancelamento do arquivo enviado na Home
        /// </summary>
        /// <param name="queryString">Query string criptografada</param>
        private void ExecutarCancelamentoHomeEmLote(QueryStringSegura queryString)
        {
            Guid? guidLote = queryString["GuidLote"].ToGuidNull();
            String nomeArquivo = queryString["NomeArquivo"];
            var extensao = nomeArquivo.Substring(nomeArquivo.LastIndexOf('.') + 1);

            if (VerificaExtensaoArquivo(extensao))
            {
                Byte[] conteudo = default(Byte[]);
                if (guidLote.HasValue)
                {
                    conteudo = (Byte[])Session[guidLote.Value.ToString("N")];
                    Session.Remove(guidLote.Value.ToString("N"));

                    Stream doc = new MemoryStream(conteudo);
                    LerArquivoCancelamentoEmLote(doc);
                    RedirecionaProximoPassoEmLote();
                }
            }
        }

        /// <summary>
        /// Exibe view de pv bloqueado por fraude
        /// </summary>
        private void MostraViewPvBloqueadoPorFraude()
        {
            mvwSolicitarCancelamento.SetActiveView(vwPvBloqueadoPorFraude);
        }

        /// <summary>
        /// Exibe a view se o estabelecimento consta na blacklist
        /// </summary>
        private void MostrarViewBloqueioBlacklist()
        {
            mvwSolicitarCancelamento.SetActiveView(vwBloqueioBlacklist);
        }

        /// <summary>
        /// Verifica se o PV está bloqueado por fraude
        /// </summary>
        /// <param name="numeroPontoVenda">Número do ponto de venda</param>
        private Boolean VerificaPvBloqueadoPorFraude(Int32 numeroPontoVenda, Int32 codigoRamoAtividade)
        {
            return Services.VerificaPVBloqueadoPorFraude(numeroPontoVenda, codigoRamoAtividade);
        }

        /// <summary>
        /// Carrega Quadro Aviso
        /// </summary>
        private void CarregarQuadroAviso()
        {
            assistentePassos.Visible = false;
            //((QuadroAviso)quadroAviso).CarregarMensagem();
        }

        /// <summary>
        /// Carrega Quadro Aviso blacklist
        /// </summary>
        private void CarregarQuadroAvisoBlacklist()
        {
            assistentePassos.Visible = false;
            //((QuadroAviso)quadroAvisoBlacklist).CarregarMensagem();
        }

        /// <summary>
        /// Regras de validação do arquivo de cancelamento em lote
        /// </summary>
        /// <param name="valores">Valores enviados</param>
        /// <returns>Validação</returns>
        private Validacao ValidaDados(string[] valores)
        {
            var validacao = new Validacao { Status = 1 };

            // Valida número do estabelecimento
            if (valores[0].Length > 9)
            {
                validacao.Status = 2;
                validacao.Descricao = "Número do estabelecimento inválido";
                return validacao;
            }

            // Valida número do NSU ou cartão
            if (valores[1].Length < 1 || valores[1].Length > 19 || valores[1].ToInt64() == 0)
            {
                validacao.Status = 2;
                validacao.Descricao = "O NSU/CV ou Nº do cartão está preenchido incorretamente.";
                return validacao;
            }


            // Valida data da venda
            if (valores[2].ToDate("ddMMyyyy") == DateTime.MinValue)
            {
                validacao.Status = 2;
                validacao.Descricao = "A data da venda está preenchida incorretamente.";
                validacao.Status = 2;
                return validacao;
            }

            // Valida valor da venda
            Decimal valorVenda = 0;
            if (!Decimal.TryParse(valores[3], out valorVenda))
            {
                validacao.Status = 2;
                validacao.Descricao = "O valor da venda está preenchido incorretamente.";
                return validacao;
            }

            // Valida tipo de cancelamento
            if (!valores[4].Equals("T") && !valores[4].Equals("P"))
            {
                validacao.Status = 2;
                validacao.Descricao = "O tipo de cancelamento está preenchido incorretamente.";
                return validacao;
            }

            // Valida valor do cancelamento
            Decimal valorCancelamento = 0;
            if (!Decimal.TryParse(valores[5], out valorCancelamento))
            {
                validacao.Status = 2;
                validacao.Descricao = "O valor do cancelamento está preenchido incorretamente.";
                return validacao;
            }

            // Valida tipo da venda
            if (!valores[6].Equals("C") && !valores[6].Equals("RO") && !valores[6].Equals("PC") && !valores[6].Equals("PS") &&
                !valores[6].Equals("DA") && !valores[6].Equals("DP") && !valores[6].Equals("D"))
            {
                validacao.Status = 2;
                validacao.Descricao = "O tipo de venda está preenchido incorretamente.";
                return validacao;
            }

            //Valida transações repetidas em um mesmo documento
            foreach (SolicitacaoCancelamento objCancelamento in SolicitacoesCancelamento)
            {
                //Se campos principais iguais aos já preenchidos
                if (
                        objCancelamento.NumeroEstabelecimentoVenda == valores[0].ToInt32() &&
                        objCancelamento.NSU.ToDecimal() == valores[1].ToDecimal() &&
                        objCancelamento.DataVenda == valores[2].ToDate("ddMMyyyy") &&
                        objCancelamento.ValorBruto == valores[3].ToDecimal() &&
                        objCancelamento.TipoVenda == GetTipoVendaPorSigla(valores[6])
                    )
                {
                    validacao.Status = 2;
                    validacao.Descricao = "Transação de venda repetida em item anterior.";
                    return validacao;
                }
            }

            return validacao;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Cancelar()
        {
            using (var log = Logger.IniciarLog("Solicitar Cancelamento - Confirmar"))
            {
                try
                {
                    //Validação do PV informado
                    Boolean ehEntidadeValida = false;
                    foreach (var solicitacao in SolicitacoesCancelamento)
                    {
                        ehEntidadeValida = Services.VerificarEstabelecimentoEmCentralizadora(solicitacao.NumeroEstabelecimentoVenda, this.SessaoAtual.CodigoEntidade);
                        if (!ehEntidadeValida)
                            break;
                    }

                    if (ehEntidadeValida)
                    {
                        CancelaSolicitacoes(SolicitacoesCancelamento);

                        //Incluir Cancelamento com IP na tabela PN
                        IncluiCancelamentosPN(SolicitacoesCancelamento);

                        //Verifica se é cancelamento lote ou individual
                        //Boolean cancelamentoLote = lblTitulo.Descricao.Equals("Solicitar Cancelamento de Vendas por Lote");
                        Boolean cancelamentoLote = this.CancelamentoEmLote;

                        //Grava no histórico de atividades
                        Historico.RealizacaoServico(this.SessaoAtual,
                            cancelamentoLote ? "Cancelamento de Vendas por Lote" : "Cancelamento de Venda");

                        //Muda a view para o segundo passo
                        ChangeView(vwComprovante);
                    }
                    else
                    {
                        PortalRedecardException ex = new PortalRedecardException(CODIGO_ERRO, "Dados Inválidos");
                        Logger.GravarErro("Dados de cancelamento inconsistentes", ex, SolicitacoesCancelamento);
                        throw ex;
                    }

                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Confirmar", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Solicitar Cancelamento - Confirmar", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }
        #endregion
    }
}
