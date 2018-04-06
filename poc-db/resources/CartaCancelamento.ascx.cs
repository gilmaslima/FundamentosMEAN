using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.Helper;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Extrato.SharePoint.ConsultaPorTransacao;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.ConsultaPorTransacao
{
    public partial class CartaCancelamento : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void VisualizarCarta(VisualizarCartaFiltro filtro)
        {
            using (Logger Log = Logger.IniciarLog("Carta Cancelamento - Visualizar Carta"))
            {
                try
                {                    
                    StatusRetorno statusRetorno;

                    ConsultarCartasEnvio envio = new ConsultarCartasEnvio();
                    envio.NumeroProcesso = filtro.NumeroProcesso;
                    envio.SistemaDados = filtro.SistemaDados;
                    envio.TimestampTransacao = filtro.TimestampTransacao;

                    using (var contexto = new ContextoWCF<ConsultaPorTransacaoClient>())
                    {
                        ConsultarCartasRetorno[] retornoArray = contexto.Cliente.ConsultarCartas(out statusRetorno, envio);

                        ConsultarCartasRetorno retorno = retornoArray[0];

                        DateTime now = DateTime.Now;

                        lblDia.Text = now.Day.ToString();

                        switch (now.Month)
                        {
                            case 1: lblMes.Text = "Janeiro"; break;
                            case 2: lblMes.Text = "Fevereiro"; break;
                            case 3: lblMes.Text = "Março"; break;
                            case 4: lblMes.Text = "Abril"; break;
                            case 5: lblMes.Text = "Maio"; break;
                            case 6: lblMes.Text = "Junho"; break;
                            case 7: lblMes.Text = "Julho"; break;
                            case 8: lblMes.Text = "Agosto"; break;
                            case 9: lblMes.Text = "Setembro"; break;
                            case 10: lblMes.Text = "Outubro"; break;
                            case 11: lblMes.Text = "Novembro"; break;
                            case 12: lblMes.Text = "Dezembro"; break;
                        }

                        lblAno.Text = now.Year.ToString();

                        lblEstabelecimento.Text = retorno.NumeroEstabelecimento.ToString();
                        lblNumeroCartao.Text = retorno.NumeroCartao;
                        lblNSU.Text = retorno.NumeroNsu.ToString();
                        lblDataVenda.Text = retorno.DataVenda.ToString(Constantes.Formatador.FormatoDataPadrao);
                        lblValorTransacao.Text = retorno.ValorTransacao.ToString("N2");
                        lblValorCancelamento.Text = retorno.ValorCancelamento.ToString("N2");
                        lblDataCancelamento.Text = retorno.DataCancelamento.ToString(Constantes.Formatador.FormatoDataPadrao);
                    }
                }
                catch (Exception e)
                {
                    Log.GravarErro(e);
                    SharePointUlsLog.LogErro(e);
                }
            }
        }
    }
}
