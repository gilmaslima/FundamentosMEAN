using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.ServiceModel;
using Redecard.PN.Comum;


namespace Redecard.PN.OutrosServicos.SharePoint.Layouts.OutrosServicos
{
    public partial class SerasaComprovante : ApplicationPageBaseAutenticada
    {
        /// <summary>
        /// Sessão do usuário
        /// </summary>
        private Sessao _sessao = null;

        /// <summary>
        /// Sessão atual do usuário
        /// </summary>
        private Sessao SessaoAtual
        {
            get
            {
                if (_sessao != null && Sessao.Contem())
                    return _sessao;
                else
                {
                    if (Sessao.Contem())
                    {
                        _sessao = Sessao.Obtem();
                    }
                    return _sessao;
                }
            }
        }
        
        /// <summary>
        /// Inicialização da página
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
                        dvPainel.Controls.Add(base.RetornarPainelExcecao("Dados do contrato inválidos."));
                }
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                dvPainel.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
            }
        }

        /// <summary>
        /// Carrega o comprovante na tela
        /// </summary>
        /// <param name="codigoRegime">Código do Regime contratado</param>
        private void CarregarComprovante(Int16 codigoRegime)
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
                    }
                }
            }
            catch (FaultException<ZPOutrosServicos.GeneralFault> ex)
            {
                SharePointUlsLog.LogErro(ex);
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                dvPainel.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
            }
        }
    }
}
