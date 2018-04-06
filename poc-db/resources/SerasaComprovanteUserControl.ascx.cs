using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Redecard.PN.Comum;
using System.ServiceModel;

namespace Redecard.PN.OutrosServicos.SharePoint.WebParts.SerasaComprovante
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SerasaComprovanteUserControl : UserControlBase
    {
        /// <summary>
        /// Inicialização da Tela
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);

                    if (!String.IsNullOrEmpty(queryString["CodigoRegime"]))
                    {
                        Int16 codigoRegime = (Int16)queryString["CodigoRegime"].ToString().ToInt16Null(0);
                        CarregarComprovante(codigoRegime);
                    }
                    else
                        base.ExibirPainelExcecao("300", "Dados do contrato inválidos.");

                        //dvPainel.Controls.Add(base.RetornarPainelExcecao("Dados do contrato inválidos."));
                }
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                //dvPainel.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
            }
        }

        /// <summary>
        /// Carrega o comprovante na tela
        /// </summary>
        /// <param name="codigoRegime">Código do Regime contratado</param>
        private void CarregarComprovante(Int16 codigoRegime)
        {
            using (Logger Log = Logger.IniciarLog("Carregando comprovante"))
            {
                try
                {
                    Int16 codigoRetorno = 0;
                    ZPOutrosServicos.RegimeFranquia regime = null;

                    using (var servicoCliente = new ZPOutrosServicos.HISServicoZP_OutrosServicosClient())
                    {
                        regime = servicoCliente.ConsultarRegime(out codigoRetorno, codigoRegime);

                        if (regime != null)
                        {
                            lblCodigoEntidade.Text = SessaoAtual.CodigoEntidade.ToString();
                            lblNomeEstabelecimento.Text = SessaoAtual.NomeEntidade;
                            lblUsuario.Text = SessaoAtual.LoginUsuario;

                            lblDataContratacao.Text = DateTime.Now.ToString();
                            lblCodigoRegime.Text = regime.CodigoRegime.ToString();
                            lblQuantidadeConsulta.Text = regime.QuantidadeConsulta.ToString();
                            lblValorConsulta.Text = String.Format("{0:C2}", regime.ValorConsulta);
                            lblValorFranquia.Text = String.Format("{0:C2}", regime.ValorFranquia);

                            mnuAcoes.Visible = true;
                        }
                    }
                }
                catch (FaultException<ZPOutrosServicos.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                    //dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    //dvPainel.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
                }
            }
        }
    }
}
