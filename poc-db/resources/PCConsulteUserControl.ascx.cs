using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.OutrasEntidades.SharePoint.ServicoWFPropostas;

namespace Redecard.PN.OutrasEntidades.SharePoint.WebParts.FMS.PCConsulte
{
    public partial class PCConsulteUserControl : UserControlBase
    {
        private Services services;
        public Services Services
        {
            get
            {
                if (services == null)
                    services = new Services(this);
                return services;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (SessaoAtual != null && SessaoAtual.GrupoEntidade == 3)
                    lblTitulo.Text = string.Format("{0} - {1}", lblTitulo.Text, "Emissores");
                else
                    lblTitulo.Text = string.Format("{0} - {1}", lblTitulo.Text, "Parceiros");
            }
        }

        /// <summary>
        /// Evento de mudança da view
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void mvwPropCredConsulte_ActiveViewChanged(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Outras Entidades - PCConsulte - Active View Change"))
            {
                try
                {
                    var activeView = mvwPropCredConsulte.GetActiveView();

                    if (activeView == vwPreencherFiltrosConsulte)
                        CarregaDadosFiltros();
                    else if (activeView == vwRetornoDadosConsulte)
                        CarregaDadosRetornoConsulta();
                }
                catch (PortalRedecardException ex)
                {
                    Logger.GravarErro("Outras Entidades - PCConsulte - Active View Change", ex);
                    SharePointUlsLog.LogErro(ex);
                    ExibirPainelExcecao(ex.Fonte, ex.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Outras Entidades - PCConsulte - Active View Change", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }

        }

        /// <summary>
        /// Execução do método ao ativar a View vwRetornoDadosConsulte
        /// </summary>
        private void CarregaDadosRetornoConsulta()
        {

            using (var log = Logger.IniciarLog("Outras Entidades - PCConsulte - Active View Change"))
            {
                //Variáveis
                string strAgencia = string.Empty;
                Char cTipoPessoa = rdbTipoPessoa.SelectedValue.ToCharArray()[0];
                Int64 intCcpjCpf = txtCnpjCpf.Text.ToInt64();

                //Nome da agência para emissores
                if (SessaoAtual != null && SessaoAtual.GrupoEntidade == 3) //Emissores
                    strAgencia = Services.ConsultarNomeAgencia(txtAgencia.Text.ToInt32(), SessaoAtual.CodigoEntidade);

                PropostaPorCNPJCPF objProposta = Services.ConsultaPropostaPorCNPJCPF(cTipoPessoa, intCcpjCpf, 1);

                //Preencher dados na tela
                lblTipoPessoaValorRetorno.Text = rdbTipoPessoa.SelectedValue == "F" ? "Física" : "Jurídica";
                lblAgenciaValorRetorno.Text = string.Format("{0} {1}", txtAgencia.Text.Trim(), strAgencia.Trim());
                lblCnpjCpfValorRetorno.Text = txtCnpjCpf.Text;

                lblRazaoSocialValorRetorno.Text = objProposta.NomeRazaoSocial;
                lblDataValorRetorno.Text = objProposta.Data;
                lblHoraValorRetorno.Text = objProposta.Hora;
                lblDescricaoValorRetorno.Text = objProposta.DescricaoRetorno;
            }

        }

        /// <summary>
        /// Execução do método ao ativar a View vwPreencherFiltrosConsulte
        /// </summary>
        private void CarregaDadosFiltros()
        {

        }

        /// <summary>
        /// Muda a view exibida
        /// </summary>
        /// <param name="view">Vizualição que deve ser exibida</param>
        private void ChangeView(View view)
        {
            mvwPropCredConsulte.SetActiveView(view);
        }

        /// <summary>
        /// Valida CNPJ/CPF
        /// </summary>
        /// <param name="source">Objeto fonte do evento</param>
        /// <param name="args">Argumentos</param>
        protected void cvCnpjCpf_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            if (txtCnpjCpf.Text.Length == 14)
                args.IsValid = txtCnpjCpf.Text.Replace(".", "").Replace("-", "").IsValidCPF();
            else if (txtCnpjCpf.Text.Length == 18)
                args.IsValid = txtCnpjCpf.Text.Replace(".", "").Replace("-", "").Replace("/", "").IsValidCNPJ();

        }

        /// <summary>
        /// Botão Continuar
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                ChangeView(vwRetornoDadosConsulte);
            }
        }

        /// <summary>
        /// Botão voltar para menu de opções
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("pn_PropostaCredenciamento.aspx");
        }

        /// <summary>
        /// Botão voltar para tela de filtros
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnVoltarRetorno_Click(object sender, EventArgs e)
        {
            ChangeView(vwPreencherFiltrosConsulte);
        }

    }
}
