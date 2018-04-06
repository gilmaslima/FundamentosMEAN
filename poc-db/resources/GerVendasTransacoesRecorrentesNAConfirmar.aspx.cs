using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.DataCash.Modelos;
using Redecard.PN.DataCash.BasePage;

namespace Redecard.PN.DataCash
{
    public partial class GerVendasTransacoesRecorrentesNAConfirmar : PageBaseDataCash
    {
        #region [ Variáveis de Sessão ]

        private List<RegistroTransacaoHistoricRecurring> TransacoesHistoricoSelecionadas
        {
            get { return Util.DeserializarDado<List<RegistroTransacaoHistoricRecurring>>((byte[])Session["TransacoesSelecionadas"]); }
            set { Session["TransacoesSelecionadas"] = Util.SerializarDado(value); }
        }

        private List<Modelo.NovaAutorizacao> TransacoesValores
        {
            get { return Util.DeserializarDado<List<Modelo.NovaAutorizacao>>((byte[])Session["TransacoesValores"]); }
            set { Session["TransacoesValores"] = Util.SerializarDado(value); }
        }

        #endregion

        /// <summary>
        /// Carrega a página.
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
                        pnlHistoricRecurring.Visible = true;
                        rptHistoricRecurring.DataSource = this.TransacoesHistoricoSelecionadas;
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
                var item = (RegistroTransacaoHistoricRecurring)e.Item.DataItem;

                ((Literal)e.Item.FindControl("lblNroConta")).Text = item.NumeroConta.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblTID")).Text = item.TID.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblDataVenda")).Text = item.DataVenda.ToString("dd/MM/yy");
                ((Literal)e.Item.FindControl("lblValorVenda")).Text = item.ValorVenda.ToString("N2", ptBR);
                ((Literal)e.Item.FindControl("lblBandeira")).Text = item.Bandeira.IfNullOrEmpty("-");
                ((Literal)e.Item.FindControl("lblNroCartao")).Text = item.NumeroCartao.IfNullOrEmpty("-").PadLeft(4, ' ').Right(4);
            }
        }

        /// <summary>
        /// Evento do botão confirmar.
        /// </summary>
        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Transações Recorrentes - Nova Autorização - Confirmar"))
            {
                var transacoesValores = new List<Modelo.NovaAutorizacao>();

                //Recupera os valores digitados pelo usuário
                foreach (RepeaterItem item in rptHistoricRecurring.Items)
                {
                    TextBox txtNroPedido = (TextBox)item.FindControl("txtNroPedido");
                    TextBox txtValorTransacao = (TextBox)item.FindControl("txtValorTransacao");
                    Literal lblNroConta = (Literal)item.FindControl("lblNroConta");

                    transacoesValores.Add(new Modelo.NovaAutorizacao
                    {
                        NumeroPedido = txtNroPedido.Text,                           
                        ValorTransacao = txtValorTransacao.Text.ToDecimal(),
                        Transacao = this.TransacoesHistoricoSelecionadas.ElementAtOrDefault(item.ItemIndex)
                    });                        
                }

                this.TransacoesValores = transacoesValores;

                Boolean sucesso = this.EfetuarNovaAutorizacao(this.TransacoesValores);
                if(sucesso)
                    base.Redirecionar("GerVendasTransacoesRecorrentesNAComprovante.aspx");
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

        /// <summary>
        /// Efetua numa nova autorização
        /// </summary>
        /// <param name="transacoes">Transações</param>
        private Boolean EfetuarNovaAutorizacao(List<Modelo.NovaAutorizacao> transacoes)
        {
            using (Logger Log = Logger.IniciarLog("Transações Recorrentes - Nova Autorização - Comprovante"))
            {
                try
                {
                    var gerenciamento = new Negocio.Gerenciamento();
                    foreach (Modelo.NovaAutorizacao transacao in transacoes)
                    {
                        Modelo.RetornoTransacaoXML retorno = gerenciamento.ExecutarNovaAutorizacao(this.SessaoAtual.CodigoEntidade,
                            transacao.Transacao.NumeroConta, transacao.ValorTransacao, transacao.NumeroPedido);
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