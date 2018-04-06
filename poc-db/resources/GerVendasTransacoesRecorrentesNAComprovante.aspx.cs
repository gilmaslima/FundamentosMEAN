using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.DataCash.BasePage;
using Redecard.PN.DataCash.Modelos;

namespace Redecard.PN.DataCash
{
    public partial class GerVendasTransacoesRecorrentesNAComprovante : PageBaseDataCash
    {
        #region [ Variáveis de Sessão ]
    
        private List<Modelo.NovaAutorizacao> TransacoesValores
        {
            get { return Util.DeserializarDado<List<Modelo.NovaAutorizacao>>((byte[])Session["TransacoesValores"]); }
            set { Session["TransacoesValores"] = Util.SerializarDado(value); }
        }

        #endregion

        /// <summary>
        /// Carrega a página de comprovante. Chama o serviço de transação com o datacash para cada transação selecionada.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Page_Load - Faça sua Venda"))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        pnlHistoricRecurring.Visible = true;
                        rptHistoricRecurring.DataSource = this.TransacoesValores;
                        rptHistoricRecurring.DataBind();
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
        /// Preenche o repeater de transações do tipo Historic Recurring.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptHitoricRecurring_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var item = (Modelo.NovaAutorizacao)e.Item.DataItem;
                
                ((Literal)e.Item.FindControl("lblNroConta")).Text = item.Transacao.NumeroConta.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblTID")).Text = item.Transacao.TID.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblDataVenda")).Text = item.Transacao.DataVenda.ToString("dd/MM/yy");
                ((Literal)e.Item.FindControl("lblValorVenda")).Text = item.Transacao.ValorVenda.ToString("N2", ptBR);
                ((Literal)e.Item.FindControl("lblBandeira")).Text = item.Transacao.Bandeira.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblNroCartao")).Text = item.Transacao.NumeroCartao.IfNullOrEmpty("-").PadLeft(4, ' ').Right(4);
                ((Literal)e.Item.FindControl("lblNroPedido")).Text = item.NumeroPedido;
                ((Literal)e.Item.FindControl("lbltValorTransacao")).Text = item.ValorTransacao.ToString("N2", ptBR);
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
    }
}