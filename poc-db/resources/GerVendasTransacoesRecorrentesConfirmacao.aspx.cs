using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.DataCash.BasePage;
using Redecard.PN.Comum;
using Redecard.PN.DataCash.Modelos;
using TipoTransacao = Redecard.PN.DataCash.Modelo.TipoTransacaoRecorrente;

namespace Redecard.PN.DataCash
{
    public partial class GerVendasTransacoesRecorrentesConfirmacao : PageBaseDataCash
    {
        #region [ Variáveis de Sessão ]

        private List<RegistroTransacaoFireAndForget> TransacoesAgendadoSelecionadas
        {
            get { return Util.DeserializarDado<List<RegistroTransacaoFireAndForget>>((byte[])Session["TransacoesSelecionadas"]); }
            set { Session["TransacoesSelecionadas"] = Util.SerializarDado(value); }
        }

        private List<RegistroTransacaoHistoricRecurring> TransacoesHistoricoSelecionadas
        {
            get { return Util.DeserializarDado<List<RegistroTransacaoHistoricRecurring>>((byte[])Session["TransacoesSelecionadas"]); }
            set { Session["TransacoesSelecionadas"] = Util.SerializarDado(value); }
        }

        private TipoTransacao? TipoTransacao
        {
            get { return Util.DeserializarDado<TipoTransacao?>((byte[])Session["TipoTransacao"]); }
            set { Session["TipoTransacao"] = Util.SerializarDado(value); }
        }

        #endregion

        /// <summary>
        /// Carrega a página de confirmação.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Page_Load - Faça sua Venda"))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        if (this.TipoTransacao == Modelo.TipoTransacaoRecorrente.Agendado)
                        {
                            pnlFireForget.Visible = true;
                            rptFireForget.DataSource = this.TransacoesAgendadoSelecionadas;
                            rptFireForget.DataBind();
                        }
                        else if (this.TipoTransacao == Modelo.TipoTransacaoRecorrente.Historico)
                        {
                            pnlHistoricRecurring.Visible = true;
                            rptHistoricRecurring.DataSource = this.TransacoesHistoricoSelecionadas;
                            rptHistoricRecurring.DataBind();
                        }
                    }
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Preenche o repeater de transações do tipo Fire And Forget.
        /// </summary>
        protected void rptFireForget_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (RegistroTransacaoFireAndForget)e.Item.DataItem;
                ((Literal)e.Item.FindControl("lblTID")).Text = item.TID.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblDataVenda")).Text = item.DataVenda.ToString("dd/MM/yy");
                ((Literal)e.Item.FindControl("lblValorRecorrencia")).Text = item.ValorRecorente.ToString("N2", ptBR);
                ((Literal)e.Item.FindControl("lblFrequencia")).Text = item.Frequencia.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblDataInicio")).Text = item.DataInicio.ToString("dd/MM/yy");
                ((Literal)e.Item.FindControl("lblQnteRecorrencias")).Text = item.QuantidadeRecorrencias.ToString();
                ((Literal)e.Item.FindControl("lblValorUltimaCobranca")).Text = item.ValorUltimaCobranca.ToString("N2", ptBR);
                if (item.DataUltimaCobranca.ToString("dd/MM/yyyy").CompareTo("01/01/1900") == 0)
                    ((Literal)e.Item.FindControl("lblDataUltimaCobranca")).Text = "-";
                else
                    ((Literal)e.Item.FindControl("lblDataUltimaCobranca")).Text = item.DataUltimaCobranca.ToString("dd/MM/yy");
                ((Literal)e.Item.FindControl("lblNroCartao")).Text = item.NumeroCartao.IfNullOrEmpty("-").PadLeft(4, ' ').Right(4);
                ((Literal)e.Item.FindControl("lblRecorrRest")).Text = item.RecorrenciasRestantes.ToString();
            }
        }

        /// <summary>
        /// Preenche o repeater de transações do tipo Historic Recurring.
        /// </summary>
        protected void rptHitoricRecurring_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (RegistroTransacaoHistoricRecurring)e.Item.DataItem;

                ((Literal)e.Item.FindControl("lblNroConta")).Text = item.NumeroConta.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblTID")).Text = item.TID.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblDataVenda")).Text = item.DataVenda.ToString("dd/MM/yy");
                ((Literal)e.Item.FindControl("lblValorVenda")).Text = item.ValorVenda.ToString("N2", ptBR);
                ((Literal)e.Item.FindControl("lblBandeira")).Text = item.Bandeira.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblNroCartao")).Text = item.NumeroCartao.IfNullOrEmpty("-").PadLeft(4, ' ').Right(4);
                ((Literal)e.Item.FindControl("lblNroPedido")).Text = item.NumeroPedido.IfNullOrEmpty("-");
            }
        }

        /// <summary>
        /// Evento do botão confirmar.
        /// </summary>
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("btnConfirmar_Click - Faça sua Venda"))
            {
                try
                {
                    Boolean sucesso = false;
                    if (this.TipoTransacao == Modelo.TipoTransacaoRecorrente.Agendado)
                        sucesso = this.CancelarAgendamentos(this.TransacoesAgendadoSelecionadas);
                    else if (this.TipoTransacao == Modelo.TipoTransacaoRecorrente.Historico)
                        sucesso = this.CancelarContas(this.TransacoesHistoricoSelecionadas);

                    if (sucesso)
                        base.Redirecionar("GerVendasTransacoesRecorrentesComprovante.aspx");
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento do botão voltar
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            //Redireciona para a tela inicial de Relatório de Transações Recorrentes, 
            //já trazendo a última consulta paginada
            GerVendasTransacoesRecorrentes.RedirecionarParaRelatorio(HttpContext.Current);
        }

        private Boolean CancelarAgendamentos(List<Modelos.RegistroTransacaoFireAndForget> transacoes)
        {
            using (Logger Log = Logger.IniciarLog("Transações Recorrentes - Cancelamento de Agendamentos"))
            {
                try
                {
                    var gerenciamento = new Negocio.Gerenciamento();
                    foreach (Modelos.RegistroTransacaoFireAndForget transacao in transacoes)
                    {
                        //Executa chamada do DataCash
                        Modelo.RetornoTransacaoXML retorno =
                            gerenciamento.ExecutarCancelamentoAgendamento(SessaoAtual.CodigoEntidade, transacao.TID);
                    }
                    
                    return true;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                    return false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
            }
        }

        private Boolean CancelarContas(List<Modelos.RegistroTransacaoHistoricRecurring> transacoes)
        {
            using (Logger Log = Logger.IniciarLog("Transações Recorrentes - Cancelamento de Contas"))
            {
                try
                {
                    var gerenciamento = new Negocio.Gerenciamento();
                    foreach (Modelos.RegistroTransacaoHistoricRecurring transacao in transacoes)
                    {
                        //Executa chamada do DataCash
                        Modelo.RetornoTransacaoXML retorno =
                            gerenciamento.ExecutarCancelamentoConta(SessaoAtual.CodigoEntidade, transacao.NumeroConta);
                    }
                    
                    return true;
                }
                catch (PortalRedecardException ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex);
                    return false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
            }
        }
    }
}