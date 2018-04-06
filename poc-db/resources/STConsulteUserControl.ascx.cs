using System;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.OutrasEntidades.SharePoint.ServicoPortalEmissores;

namespace Redecard.PN.OutrasEntidades.SharePoint.WebParts.FMS.STConsulte
{
    public partial class STConsulteUserControl : UserControlBase
    {
        #region .Page_Load.
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessaoAtual != null && SessaoAtual.GrupoEntidade == 12)
            {
                ViewState["GrupoEntidade"] = "PARCEIROS";
            }
            else
            {
                ViewState["GrupoEntidade"] = "EMISSORES";
            }

            lblTitulo.Text = string.Format("SOLICITAÇÃO DE TECNOLOGIA - {0}", ViewState["GrupoEntidade"]);
        } 
        #endregion

        #region .mvwPropCredConsulte_ActiveViewChanged.
        /// <summary>
        /// Evento de mudança da view
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void mvwPropCredConsulte_ActiveViewChanged(object sender, EventArgs e)
        {
            var activeView = mvwPropCredConsulte.GetActiveView();

            if (activeView == vwRetornoDadosConsulte)
            {
                lblTitulo2.Text = string.Format("SOLICITAÇÃO DE TECNOLOGIA - {0}", ViewState["GrupoEntidade"]);
            }
        } 
        #endregion

        #region .btnContinuar_Click.
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            Services servicos = new Services(this);

            String razao = string.Empty;

            Redecard.PN.Comum.SharePoint.EntidadeServico.Entidade entidade = servicos.ConsultarDadosCompletos(int.Parse(txtPv.Text.Trim()));

            razao = entidade.RazaoSocial;

            if (!string.IsNullOrEmpty(razao))
            {
                lblPVConsultado.Text = txtPv.Text.Trim();
                lblRazaoSocial.Text = razao;

                DadosSolicitacaoTecnologia[] dadosTecnologia = servicos.ConsultarTecnologia(int.Parse(txtPv.Text.Trim()));

                if (dadosTecnologia != null && dadosTecnologia.Length > 0)
                {
                    grdTecnologia.DataSource = dadosTecnologia;
                    grdTecnologia.DataBind();
                }

                ChangeView(vwRetornoDadosConsulte);
            }
            else
            {
                MostraQuadroAviso("PV não encontrado");
            }
        } 
        #endregion

        #region .btnVoltarDados_Click.
        protected void btnVoltarDados_Click(object sender, EventArgs e)
        {
            ChangeView(vwPreencherFiltrosConsulte);
        } 
        #endregion

        #region .ChangeView.
        /// <summary>
        /// Altera a visualização na pagina
        /// </summary>
        /// <param name="view"></param>
        private void ChangeView(View view)
        {
            mvwPropCredConsulte.SetActiveView(view);
        } 
        #endregion

        #region .grdTecnologia_RowDataBound.
        protected void grdTecnologia_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Literal ltlData = e.Row.FindControl("ltlData") as Literal;
                Literal ltlNrSolicitacao = e.Row.FindControl("ltlNrSolicitacao") as Literal;
                Literal ltlTipoEquipamento = e.Row.FindControl("ltlTipoEquipamento") as Literal;
                Literal ltlSituacao = e.Row.FindControl("ltlSituacao") as Literal;
                Literal ltlDataInstalacao = e.Row.FindControl("ltlDataInstalacao") as Literal;

                DadosSolicitacaoTecnologia item = e.Row.DataItem as DadosSolicitacaoTecnologia;

                ltlData.Text = item.Data.ToString("dd/MM/yyyy");
                ltlNrSolicitacao.Text = item.NumeroSolicitacao.ToString();
                ltlTipoEquipamento.Text = item.TipoEquipamento;
                ltlSituacao.Text = item.Status;
                ltlDataInstalacao.Text = item.DataInstalacao.GetValueOrDefault(DateTime.MinValue).ToString("dd/MM/yyyy");
            }
        } 
        #endregion

        #region .MostraQuadroAviso.
        /// <summary>
        /// Exibe o quadro de aviso
        /// </summary>
        private void MostraQuadroAviso(string Mensagem)
        {
            ((QuadroAviso)quadroAviso).ExibirPainelMensagem(Mensagem);
        } 
        #endregion
    }
}
