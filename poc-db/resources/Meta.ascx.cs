using Rede.PN.ManinhaContaCerta.Core.Web.Controles.Portal;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Rede.PN.MaquininhaContaCerta.SharePoint.CONTROLTEMPLATES
{
    /// <summary>
    /// Controle para exibição dos dados do histórico de enquadramento da meta
    /// na tela da Maquininha Conta Certa
    /// </summary>
    public partial class Meta : UserControlBase
    {
        /// <summary>
        /// Evento de carregamento do controle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            this.ValidarPermissao = false;
            base.OnLoad(e);
        }

        /// <summary>
        /// Evento de carregamento da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Maquininha Conta Certa - Meta"))
            {
                try
                {
                    using (var context = new ContextoWCF<MaquininhaServico.MaquininhaContaCertaClient>())
                    {
                        var historico = context.Cliente.ConsultaHistoricoApuracao(SessaoAtual.CodigoEntidade);
                        if (historico != null && historico.Count > 0)
                        {
                            rpHistorico.DataSource = historico;
                            rpHistorico.DataBind();
                        }
                        else
                            pnlMain.Visible = false;
                    }
                }
                catch (CommunicationException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);
                    pnlMain.Visible = false;
                }
                catch (TimeoutException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);
                    pnlMain.Visible = false;
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);
                    pnlMain.Visible = false;
                }
            }
        }

        /// <summary>
        /// Databound do repeater de histórico
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rpHistorico_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var historico = e.Item.DataItem as MaquininhaServico.MaquininhaHistoricoApuracao;

                Literal ltrMesApuracao = e.Item.FindControl("ltrMesApuracao") as Literal;
                Literal ltrMesReferencia = e.Item.FindControl("ltrMesReferencia") as Literal;
                Literal ltrValorFaturamentoApurado = e.Item.FindControl("ltrValorFaturamentoApurado") as Literal;
                Literal ltrValorPacoteBasico = e.Item.FindControl("ltrValorPacoteBasico") as Literal;
                Literal ltrValorAluguelMaquininha = e.Item.FindControl("ltrValorAluguelMaquininha") as Literal;
                Literal ltrValorTotalPacote = e.Item.FindControl("ltrValorTotalPacote") as Literal;

                ltrMesApuracao.Text = historico.DataMesApuracao.ToString("MMMM - yyyy");
                ltrMesReferencia.Text = historico.DataMesReferencia.ToString("MMMM - yyyy");
                ltrValorFaturamentoApurado.Text = historico.ValorFaturamentoApurado.ToString("n");
                ltrValorPacoteBasico.Text = historico.ValorPacoteBasico.ToString("n");
                ltrValorAluguelMaquininha.Text = historico.ValorAluguelMaquininha.ToString("n");
                ltrValorTotalPacote.Text = historico.ValorTotalPacote.ToString("n");
            }
        }
    }
}
