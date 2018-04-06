using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.ConsultaPorTransacao;
using Redecard.PN.Extrato.SharePoint.Modelo;
using Redecard.PN.Comum;
using Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.SaldosEmAberto;

namespace Redecard.PN.Extrato.SharePoint.Layouts.Redecard.PN.Extrato.SharePoint
{
    public partial class VisualizarCarta : ApplicationPageBaseAutenticada
    {
        CartaCancelamento cartaCancelamento;
        CartaChargeback cartaChargeback;
        CartadeCircularizacao cartaCircularizacao;

        private enum ViewCarta
        {
            NenhumRegistro = 0,
            Cancelamento = 1,
            Chargeback = 2,
            Circularizacao = 3
        }

        private void AlterarViewAtual(ViewCarta view)
        {
            ViewState["ViewAtual"] = view;

            MultiViewCarta.ActiveViewIndex = (int)view;
        }

        private ViewCarta ObterViewAtual()
        {
            ViewCarta view;

            if (ViewState["ViewAtual"] == null)
                view = ViewCarta.NenhumRegistro;
            else
                view = (ViewCarta)ViewState["ViewAtual"];

            return view;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger.IniciarLog("Visualizar Carta"))
            {
                string dados = Request.QueryString["dados"];

                if (!string.IsNullOrEmpty(dados))
                {
                    QueryStringSegura queryString = new QueryStringSegura(dados);
                    if (queryString["circularizacao"].EmptyToNull() == null)
                    {
                        decimal numeroProcesso = decimal.Parse(queryString["numeroProcesso"]);
                        short sistemaDados = short.Parse(queryString["sistemaDados"]);
                        string timestampTransacao = queryString["timestampTransacao"];
                        VisualizarCartaFiltro filtro = new VisualizarCartaFiltro();
                        filtro.NumeroProcesso = numeroProcesso;
                        filtro.SistemaDados = sistemaDados;
                        filtro.TimestampTransacao = timestampTransacao;

                        if (sistemaDados == 1 || sistemaDados == 23)
                        {
                            cartaChargeback = (CartaChargeback)this.FindControl("CartaChargeback1");
                            cartaChargeback.VisualizarCarta(filtro);
                            AlterarViewAtual(ViewCarta.Chargeback);
                        }
                        else
                        {
                            cartaCancelamento = (CartaCancelamento)this.FindControl("CartaCancelamento1");
                            cartaCancelamento.VisualizarCarta(filtro);
                            AlterarViewAtual(ViewCarta.Cancelamento);
                        }
                    }
                    else
                    {
                        Int32 codigoEntidade = int.Parse(queryString["codigoEntidade"]);
                        DateTime dataSolicitacao = DateTime.Parse(queryString["dataSolicitacao"]);
                        Decimal valorCarta = Decimal.Parse(queryString["valorCarta"]);
                        cartaCircularizacao = (CartadeCircularizacao)this.FindControl("CartadeCircularizacao1");
                        cartaCircularizacao.VisualizarCarta(codigoEntidade, dataSolicitacao, valorCarta);
                        AlterarViewAtual(ViewCarta.Circularizacao);
                    }
                }
                else
                {
                    AlterarViewAtual(ViewCarta.NenhumRegistro);
                }
            }
        }
    }
}
