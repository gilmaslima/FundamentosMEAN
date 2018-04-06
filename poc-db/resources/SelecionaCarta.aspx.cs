using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.ServiceModel;
using System.Collections.Generic;
using Redecard.PN.Comum;

namespace Redecard.PN.OutrosServicos.SharePoint.Layouts.OutrosServicos
{
    public partial class SelecionaCarta : ApplicationPageBaseAutenticada
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    CarregarCarta();
                }
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                dvPainel.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
            }
        }

        private void CarregarCarta()
        {
            try
            {
                if (Request.QueryString["dados"] != null)
                {
                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);

                    if (queryString["NumeroSolicitacao"] != null)
                    {
                        using (var cartaSolicitacaoServico = new SolicitacaoServico.SolicitacaoServicoClient())
                        {

                            Int32 numeroSolicicao = 0;
                            Int32.TryParse(queryString["NumeroSolicitacao"].ToString(), out numeroSolicicao);

                            //Código do caso fixo em 1 na consulta de status
                            SolicitacaoServico.CartaSolicitacao statusCarta = cartaSolicitacaoServico.ConsultarStatusCarta(numeroSolicicao, 1);
                            if (!object.ReferenceEquals(statusCarta, null))
                            {
                                String codigoCarta = String.Concat(statusCarta.CodigoSistema, String.Concat("000", statusCarta.CodigoTextoCarta).Right(3));
                                String destinoCarta = statusCarta.DestinatarioCarta;
                                Int32 formaEnvio = statusCarta.CodigoFormaEnvio;

                                SolicitacaoServico.CartaSolicitacao[] formaRespostaCarta = cartaSolicitacaoServico.ConsultarFormaRespostaCarta(numeroSolicicao);
                                if (formaRespostaCarta.Length > 0)
                                {
                                    destinoCarta = formaRespostaCarta[0].DestinatarioCarta;
                                    formaEnvio = formaRespostaCarta[0].CodigoFormaEnvio;
                                }

                                ExibirCarta(numeroSolicicao);

                                txtDestinatario.Text = destinoCarta;
                            }
                        }
                    }
                    else
                        dvPainel.Controls.Add(base.RetornarPainelExcecao("Dados inválidos."));
                }
            }
            catch (FaultException<SolicitacaoServico.GeneralFault> ex)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
            catch (FaultException<WMOutrosServicos.GeneralFault> ex)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
            catch (Exception ex)
            {
                SharePointUlsLog.LogErro(ex);
                dvPainel.Controls.Add(base.RetornarPainelExcecao(FONTE, CODIGO_ERRO));
            }
        }

        private void ExibirCarta(Int32 numeroSolicitacao)
        {
            try
            {
                using (var COMTIServico = new WMOutrosServicos.HISServicoWM_OutrosServicosClient())
                {
                    WMOutrosServicos.CartaSolicitacao cartaSolicitacao = null;
                    Int16 quantidadeLinha = 0;
                    txtCorpoEmail.Text = "";

                    Int16 codigoRetorno = COMTIServico.Manutencao(out cartaSolicitacao, out quantidadeLinha, numeroSolicitacao, "SOL");
                    if (codigoRetorno > 0)
                        dvPainel.Controls.Add(base.RetornarPainelExcecao("WMOutrosServicos.Manutencao", codigoRetorno));
                    else
                    {
                        foreach (String linhaCarta in cartaSolicitacao.LinhasCarta)
                        {
                            txtCorpoEmail.Text += linhaCarta + "\r\n";
                        }
                    }
                }
            }
            catch (FaultException<SolicitacaoServico.GeneralFault> ex)
            {
                dvPainel.Controls.Add(base.RetornarPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32()));
            }
            catch (FaultException<WMOutrosServicos.GeneralFault> ex)
            {
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
