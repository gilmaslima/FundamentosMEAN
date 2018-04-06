using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.ServiceModel;
using Redecard.PN.Request.SharePoint.XBChargebackServico;

namespace Redecard.PN.Request.SharePoint.WebParts.RequestResumoVendas
{
    public partial class RequestResumoVendasUserControl : UserControlBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["dados"] != null)
                {
                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                    CarregarResumo(queryString);
                }
                else
                {
                    SetarAviso("Erro ao recuperar as informações passadas via QueryString.");
                }
            }
            else { pnlErro.Visible = false; }
        }

        #region Negócio e Carga de Dados
        private void CarregarResumo(QueryStringSegura queryString)
        {
            using (Logger Log = Logger.IniciarLog("Resumo de vendas"))
            {
                try
                {
                    //parametros                
                    Int32 codEstabelecimento = base.SessaoAtual.CodigoEntidade;
                    Decimal codProcesso = queryString["NumProcesso"].ToDecimal();
                    String filler = String.Empty;
                    Int64 codOcorrencia = 0;
                    Int32 codRetorno = 0;

                    //variáveis de servico
                    using (HISServicoXBChargebackClient client = new HISServicoXBChargebackClient())
                    {
                        Log.GravarLog(EventoLog.ChamadaServico, new { codEstabelecimento, codProcesso, origem = "IS", filler, codOcorrencia });

                        //chamada ao serviço
                        ComposicaoRV request = client.ComposicaoRV(codEstabelecimento, codProcesso, "IS", ref filler, ref codOcorrencia, out codRetorno);

                        Log.GravarLog(EventoLog.RetornoServico, new { filler, codOcorrencia, codRetorno, request });

                        //considerar que 10 ou 53 não é erro: DADOS NAO ENCONTRADOS NA TABELA
                        if (codRetorno > 0 && codRetorno != 10 && codRetorno != 53)
                            base.ExibirPainelExcecao("XBChargebackServico.ComposicaoRV", codRetorno);
                        else
                        {
                            lblEstabelecimento.Text = queryString["PV"];
                            lblProcesso.Text = queryString["NumProcesso"];

                            rptResumo.DataSource = request.Parcelas;

                            rptResumo.DataBind();
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Paginacao", "pageResultTable('tbParcelas', 1, 12, 5);", true);

                            lblValorVenda.Text = request.ValorVenda.ToString("N2");
                            lblValorCancelamento.Text = request.ValorCancelamento.ToString("N2");
                            lblQuantParcelas.Text = request.QuantidadeParcelas.ToString();
                            lblParcelaQuitada.Text = request.QuantidadeParcelasQuitadas.ToString();
                            lblParcelaVencer.Text = request.QuantidadeParcelasAVencer.ToString();
                        }
                    }
                }
                catch (FaultException<XBChargebackServico.GeneralFault> ex)
                {
                    Log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    Log.GravarErro(ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }
        #endregion

        #region Handlers

        protected void rptResumo_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Recupera objetos da linha
                var lblValorRpt = (Label)e.Item.FindControl("lblValorRpt");
                var lblParcelaRpt = (Label)e.Item.FindControl("lblParcelaRpt");

                //converte a linha num objeto request
                XBChargebackServico.ParcelaRV request = (XBChargebackServico.ParcelaRV)e.Item.DataItem;

                lblValorRpt.Text = request.ValorLiquido.ToString("N2");
                lblParcelaRpt.Text = request.DataParcela.ToString("dd/MM/yyyy");
            }
        }
        
        #endregion
        /// <summary>Mostra o painel de exceções</summary>        
        private void SetarAviso(String aviso)
        {
            lblMensagem.Text = aviso;
            lblMensagem.Visible = true;
            pnlErro.Visible = true;
            //pnlConteudo.Visible = false;
        }

        /// <summary>Handler do botão voltar, redireciona para a tela de Comprovantes Pendentes</summary>        
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect(base.web.ServerRelativeUrl + "/Paginas/pn_ComprovacaoVendas.aspx");
        }
    }
}
