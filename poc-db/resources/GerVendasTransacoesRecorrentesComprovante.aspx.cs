using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.DataCash.BasePage;
using Redecard.PN.DataCash.Modelos;
using TipoTransacao = Redecard.PN.DataCash.Modelo.TipoTransacaoRecorrente;

namespace Redecard.PN.DataCash
{
    public partial class GerVendasTransacoesRecorrentesComprovante : PageBaseDataCash
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
        /// Carrega a página de comprovante. Chama o serviço de transação com o DataCash para cada transação selecionada.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// Botão Voltar
        /// </summary>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            //Redireciona para a tela inicial de Relatório de Transações Recorrentes, 
            //já trazendo a última consulta paginada
            GerVendasTransacoesRecorrentes.RedirecionarParaRelatorio(HttpContext.Current);
        }

        /// <summary>
        /// Preenche o repeater de transações do tipo Fire And Forget.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
    }
}