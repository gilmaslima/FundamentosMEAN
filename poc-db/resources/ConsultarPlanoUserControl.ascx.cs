using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using System.ServiceModel;
using System.Collections.Generic;
using Redecard.PN.Outro.Core.Web.Controles.Portal;

namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.ConsultarPlano
{

    public partial class ConsultarPlanoUserControl : UserControlBase
    {
        /// <summary>
        /// Inicialização da webpart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!CarregarFranquia())
                    ExibirPainelConfirmacao();
            }
        }

        /// <summary>
        /// Exibi painel com mensagem
        /// </summary>
        private void ExibirPainelConfirmacao()
        {
            using (Logger Log = Logger.IniciarLog("Exibindo painel de confirmação"))
            {
                try
                {
                    pnlConsultaPlano.Visible = false;
                    pnlSemPlano.Visible = true;


                    qdAviso.TipoQuadro = TipoQuadroAviso.Aviso;
                    qdAviso.Mensagem = "Nenhuma franquia contratada.";
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }



        /// <summary>
        /// Carrega Franquias
        /// </summary>
        /// <returns>TRUE/FALSE se existem franquias carregadas</returns>
        private Boolean CarregarFranquia()
        {
            using (Logger Log = Logger.IniciarLog("Carregando franquias"))
            {
                try
                {
                    ZPOutrosServicos.RegimeFranquia regime = null;
                    Int16 codigoRetorno = 0;
                    using (var servicoCliente = new ZPOutrosServicos.HISServicoZP_OutrosServicosClient())
                    {
                        //Código de regime fixo em 1
                        //Int16 codigoRegime = 1;
                        regime = servicoCliente.ConsultarRegime(out codigoRetorno, SessaoAtual.CodigoEntidade);

                        if (codigoRetorno != 0)
                        {
                            this.ExibirPainelConfirmacao();
                            base.ExibirPainelExcecao("ZPOutrosServicos.ConsultarRegime", codigoRetorno);
                            return false;
                        }
                        else
                        {
                            if (regime.QuantidadeConsulta > 0)
                            {
                                lblQtdeConsulta.Text = regime.QuantidadeConsulta.ToString();
                                lblValorConsulta.Text = String.Format("{0:C2}", regime.ValorConsulta);
                                lblValorFranquia.Text = String.Format("{0:C2}", regime.ValorFranquia);
                                return true;
                            }
                            else
                                return false;
                        }
                    }
                }
                catch (FaultException<ZPOutrosServicos.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    return false;
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    return false;
                }
            }
        }
    }
}
