using Rede.PN.ManinhaContaCerta.Core.Web.Controles.Portal;
using Rede.PN.MaquininhaContaCerta.SharePoint.MaquininhaServico;
using Redecard.PN.Comum;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Rede.PN.MaquininhaContaCerta.SharePoint.CONTROLTEMPLATES
{
    /// <summary>
    /// Controle para exibição dos dados de benefício da Maquininha Conta Certa
    /// </summary>
    public partial class Beneficios : UserControlBase
    {
        /// <summary>
        /// Valor máximo para a faixa
        /// </summary>
        private const Decimal faixaMaxValue = (Decimal)9999999999999.99;

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
            using (Logger log = Logger.IniciarLog("Maquininha Conta Certa - Benefícios"))
            {
                try
                {
                    using (var context = new ContextoWCF<MaquininhaServico.MaquininhaContaCertaClient>())
                    {
                        var metas = context.Cliente.ConsultaMetas(SessaoAtual.CodigoEntidade);
                        if (metas != null && metas.Count > 0)
                        {
                            rpBeneficios.DataSource = metas;
                            rpBeneficios.DataBind();
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
        protected void rpBeneficios_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var meta = e.Item.DataItem as MaquininhaServico.MaquininhaMetas;

                Literal ltrFaixa = e.Item.FindControl("ltrFaixa") as Literal;
                Literal ltrFaixaInicial = e.Item.FindControl("ltrFaixaInicial") as Literal;
                Literal ltrFaixaFinal = e.Item.FindControl("ltrFaixaFinal") as Literal;
                Literal ltrTotalItau = e.Item.FindControl("ltrTotalItau") as Literal;
                Literal ltrTotalRede = e.Item.FindControl("ltrTotalRede") as Literal;
                Literal ltrValorCobrado = e.Item.FindControl("ltrValorCobrado") as Literal;

                ltrFaixa.Text = String.Format("Faixa {0}", e.Item.ItemIndex + 1);
                ltrFaixaInicial.Text = meta.ValorMetaInicial.ToString("n");
                ltrFaixaFinal.Text = Decimal.Compare(meta.ValorMetaFinal, faixaMaxValue) < 0 ? meta.ValorMetaFinal.ToString("n") : "-";
                ltrTotalItau.Text = meta.ValorComboPacote.ToString("n");
                ltrTotalRede.Text = meta.ValorComboMaquininha.ToString("n");
                ltrValorCobrado.Text = meta.ValorDescontoPacote.ToString("n");
            }
        }
    }
}
