/*
(c) Copyright [2012] Redecard S.A.
Autor : [Tiago Barbosa dos Santos]
Empresa : [BRQ IT Solutions]
Histórico:
- 2012/10/10 - Tiago Barbosa dos Santos - Versão Inicial
*/
using System;
using Redecard.PN.Comum;
using Redecard.PN.Emissores.Sharepoint.ServicoEmissores;
using System.ServiceModel;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace Redecard.PN.Emissores.Sharepoint.WebParts.ConsultaSolicitacao
{
    public partial class ConsultaSolicitacaoUserControl : UserControlBase
    {
        private static String razaoSocial = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnBuscar_Click(object sender, EventArgs e)
        {
            Logger.IniciarLog("Início evento BtnBuscar_Click ");
            try
            {
                using (var context = new ContextoWCF<ServicoPortalEmissoresClient>())
                {
                    Int32 codigoRetorno = 0;
                    Logger.GravarLog("Chamada ao método ObtemPV ", new { NumEstalecimento.Text });
                    PontoVenda pv = context.Cliente.ObtemPV(out codigoRetorno, NumEstalecimento.Text.ToInt32());
                    Logger.GravarLog("Retorno chamada ao método ObtemPV ", new { pv, codigoRetorno });

                    if (codigoRetorno != 0 || object.Equals(pv, null))
                    {
                        List<Panel> lstPaineis = new List<Panel>();
                        lstPaineis.Add(pnlTudo);
                        ExibirPainelConfirmacaoAcao("Emissores", "Não há dados para este Nº de Estabelecimento", Request.Url.AbsolutePath, lstPaineis.ToArray(), "icone-aviso");
                        return;
                    }

                    razaoSocial = pv.RazaoSocial;

                    List<DadosSolicitacaoTecnologia> list = context.Cliente.ConsultarTecnologia(Convert.ToInt32(NumEstalecimento.Text));

                    Logger.GravarLog("Retorno do método ", new { codigoRetorno, list });

                    pnlBusca.Visible = false;
                    pnlTudo.Visible = true;
                    tableListRpt.Visible = true;

                    rptDados.DataSource = list;
                    rptDados.DataBind();
                }
            }
            catch (FaultException<PortalRedecardException> ex)
            {
                Logger.GravarErro("BtnBuscar_Click - ", ex);
                ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                SharePointUlsLog.LogErro(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("BtnBuscar_Click - ", ex);
                SharePointUlsLog.LogErro(ex.Message);
                ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return;
            }
        }

        protected void OnclickVoltar(object sender, EventArgs e)
        {
            NumEstalecimento.Text = "";
            tableListRpt.Visible = false;
            pnlBusca.Visible = true;
        }

        protected void rptDados_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                Label lblRazaoSocial = (Label)e.Item.FindControl("lblRazaoSocial");
                Label lblPV = (Label)e.Item.FindControl("lblPV");
                lblPV.Text = NumEstalecimento.Text;
                lblRazaoSocial.Text = razaoSocial;
            }
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                Literal ltlDataFormatada = (Literal)e.Item.FindControl("ltlDataFormatada");
                Literal ltlNumeroSolicitacao = (Literal)e.Item.FindControl("ltlNumeroSolicitacao");
                Literal ltlTipoEquipamento = (Literal)e.Item.FindControl("ltlTipoEquipamento");
                Literal ltlStatus = (Literal)e.Item.FindControl("ltlStatus");
                Literal ltlDataInstalacaoRpt = (Literal)e.Item.FindControl("ltlDataInstalacaoRpt");

                DadosSolicitacaoTecnologia item = (DadosSolicitacaoTecnologia)e.Item.DataItem;
                ltlDataFormatada.Text = item.DataFormatada;
                ltlNumeroSolicitacao.Text = item.NumeroSolicitacao.ToString();
                ltlTipoEquipamento.Text = item.TipoEquipamento;
                ltlStatus.Text = item.Status;
                ltlDataInstalacaoRpt.Text = !object.Equals(item.DataInstalacao, null) ? item.DataInstalacao.Value.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;


            }
        }
    }
}
