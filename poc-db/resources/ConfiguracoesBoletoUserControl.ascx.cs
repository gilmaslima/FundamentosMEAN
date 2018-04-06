using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.ServiceModel;
using Redecard.PN.Comum;
using System.Collections.Generic;

namespace Redecard.PN.DataCash.SharePoint.WebParts.ConfiguracoesBoleto
{
    public partial class ConfiguracoesBoletoUserControl : UserControlBaseDataCash
    {
        #region [ Constantes e Propriedades ]

        private static Dictionary<String, ListItem[]> TiposBoleto { get; set; }

        private ListItem[] Bancos
        {
            get
            {
                return new ListItem[] {
                    new ListItem(String.Empty),
                    new ListItem("001 – Banco do Brasil", "001"),
                    new ListItem("033 - Santander", "033"),
                    new ListItem("104 – Caixa Econômica Federal", "104"),
                    new ListItem("237 - Bradesco", "237"),
                    new ListItem("341 - Itaú", "341"),
                    new ListItem("399 - HSBC", "399")
                };
            }
        }

        #endregion

        #region [ Página ]

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Page.Title = "Gerenciamento de Vendas";

                if (!Page.IsPostBack)
                {
                    CarregarControles();
                    mvwConfiguracoesBoleto.SetActiveView(vwDadosBoleto);
                }
            }
            catch (FaultException<DataCashService.GeneralFault> fe)
            {
                Logger.GravarErro("Configurações Boleto", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Configurações Boleto", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        private void CarregarControles()
        {
            CarregarBancos();
            CarregarDicionarioTipoBoleto();
        }

        private void CarregarDicionarioTipoBoleto()
        {
            TiposBoleto = new Dictionary<String, ListItem[]>();
            TiposBoleto.Add("001", new ListItem[] { new ListItem(String.Empty), new ListItem("16"), new ListItem("17"), new ListItem("18") });
            TiposBoleto.Add("033", new ListItem[] { new ListItem(String.Empty), new ListItem("101"), new ListItem("102"), new ListItem("201") });
            TiposBoleto.Add("104", new ListItem[] { new ListItem(String.Empty), new ListItem("SR") });
            TiposBoleto.Add("237", new ListItem[] { new ListItem(String.Empty), new ListItem("6") });
            TiposBoleto.Add("341", new ListItem[] { new ListItem(String.Empty), new ListItem("175") });
            TiposBoleto.Add("399", new ListItem[] { new ListItem(String.Empty), new ListItem("CNR") });
        }

        private void CarregarBancos()
        {
            ddlBanco.Items.Clear();
            ddlBanco.Items.AddRange(Bancos);
        }

        private void CarregarTiposBoleto()
        {
            if (!String.IsNullOrEmpty(ddlBanco.SelectedValue))
            {
                ListItem[] tiposBoleto;
                TiposBoleto.TryGetValue(ddlBanco.SelectedValue, out tiposBoleto);

                ddlTipoBoleto.Items.Clear();
                ddlTipoBoleto.Items.AddRange(tiposBoleto);
            }
            else
                ddlTipoBoleto.Items.Clear();
        }

        protected void ddlBanco_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CarregarTiposBoleto();
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Configurações Boleto", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ Botões ]

        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            try
            {
                Page.Validate();

                if (Page.IsValid)
                {
                    mvwConfiguracoesBoleto.SetActiveView(vwConfirmacao);
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Configurações Boleto", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnContinuar2_Click(object sender, EventArgs e)
        {
            try
            {
                if (SalvarBoleto())
                    mvwConfiguracoesBoleto.SetActiveView(vwEfetivacao);

            }
            catch (FaultException<DataCashService.GeneralFault> fe)
            {
                Logger.GravarErro("Configurações Boleto", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Configurações Boleto", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            try
            {
                mvwConfiguracoesBoleto.SetActiveView(vwDadosBoleto);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Configurações Boleto", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnVoltar2_Click(object sender, EventArgs e)
        {
            try
            {
                mvwConfiguracoesBoleto.SetActiveView(vwDadosBoleto);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Configurações Boleto", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnLimpar_Click(object sender, EventArgs e)
        {
            try
            {
                ddlBanco.SelectedValue = String.Empty;
                CarregarTiposBoleto();
                txtAgencia.Text = String.Empty;
                txtConta.Text = String.Empty;
                txtContaDigito.Text = String.Empty;
                txtDigitosIniciais.Text = String.Empty;
                txtNroContrato.Text = String.Empty;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Configurações Boleto", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ MultView ]

        protected void mvwConfiguracoesBoleto_ActiveViewChanged(object sender, EventArgs e)
        {
            var activeView = mvwConfiguracoesBoleto.GetActiveView();

            if (activeView == vwDadosBoleto)
            {
                DataCashService.Boleto boleto = ConsultaBoleto();
                SetBoleto(boleto);
            }

            if (activeView == vwConfirmacao)
            {
                SetBoletoConfirmacao();
            }

            if (activeView == vwEfetivacao)
            {
                SetBoletoEfetivacao();
            }

            AtualizaCabecalhoPassoAtual();
        }

        private void SetBoletoEfetivacao()
        {
            ltlBancoEfetivacao.Text = ddlBanco.SelectedItem != null ? ddlBanco.SelectedItem.Text : String.Empty;
            ltlTipoBoletoEfetivacao.Text = ddlTipoBoleto.SelectedItem != null ? ddlTipoBoleto.SelectedItem.Text : String.Empty;
            ltlAgenciaEfetivacao.Text = txtAgencia.Text;
            ltlContaEfetivacao.Text = String.Format(@"{0}-{1}", txtConta.Text, txtContaDigito.Text);
            ltlDigitosIniciaisEfetivacao.Text = txtDigitosIniciais.Text;
            ltlNroContratoEfetivacao.Text = txtNroContrato.Text;
        }

        private void SetBoletoConfirmacao()
        {
            ltlBancoConfirmacao.Text = ddlBanco.SelectedItem != null ? ddlBanco.SelectedItem.Text : String.Empty;
            ltlTipoBoletoConfirmacao.Text = ddlTipoBoleto.SelectedItem != null ? ddlTipoBoleto.SelectedItem.Text : String.Empty;
            ltlAgenciaConfirmacao.Text = txtAgencia.Text;
            ltlContaConfirmacao.Text = String.Format(@"{0}-{1}", txtConta.Text, txtContaDigito.Text);
            ltlDigitosIniciaisConfirmacao.Text = txtDigitosIniciais.Text;
            ltlNroContratoConfirmacao.Text = txtNroContrato.Text;
        }

        private void SetBoleto(DataCashService.Boleto boleto)
        {
            if (boleto != null)
            {
                ddlBanco.SelectedValue = boleto.Banco.ToString();
                CarregarTiposBoleto();
                ddlTipoBoleto.SelectedValue = boleto.BoletoType;
                txtAgencia.Text = boleto.Agencia.Trim();
                txtConta.Text = boleto.Conta.Trim();
                txtContaDigito.Text = boleto.Digito.Trim();
                txtDigitosIniciais.Text = boleto.IniciaisBoleto.ToString();
                txtNroContrato.Text = boleto.ContractID.Trim();
            }
        }

        private void AtualizaCabecalhoPassoAtual()
        {
            var activeView = mvwConfiguracoesBoleto.GetActiveView();

            if (activeView == vwDadosBoleto)
                assistente.AtivarPasso(0);
            else if (activeView == vwConfirmacao)
                assistente.AtivarPasso(1);
            else if (activeView == vwEfetivacao)
                assistente.AtivarPasso(2);
        }

        #endregion

        #region [ Operações ]

        private DataCashService.Boleto ConsultaBoleto()
        {
            DataCashService.Boleto retorno = new DataCashService.Boleto();
            DataCashService.MensagemErro mensagemErro;

            using (var log = Logger.IniciarLog("Consulta Boleto"))
            {
                Int32 pv = base.SessaoAtual.CodigoEntidade;


                log.GravarLog(EventoLog.ChamadaServico, new { pv });

                using (var contexto = new ContextoWCF<DataCashService.DataCashServiceClient>())
                {
                    retorno = contexto.Cliente.ConsultarBoleto(out mensagemErro, pv);
                }

                log.GravarLog(EventoLog.RetornoServico, new { retorno, mensagemErro });
            }

            if (mensagemErro.CodigoRetorno != 0)
                return null;

            return retorno;
        }

        private Boolean SalvarBoleto()
        {
            Boolean retorno = false;

            using (var log = Logger.IniciarLog("Salva Boleto"))
            {
                Int32 pv = base.SessaoAtual.CodigoEntidade;
                DataCashService.MensagemErro mensagemErro;
                DataCashService.Boleto boleto = new DataCashService.Boleto();
                boleto.Banco = ddlBanco.SelectedValue.ToInt32();
                boleto.BoletoType = ddlTipoBoleto.SelectedValue;
                boleto.Agencia = txtAgencia.Text;
                boleto.Conta = txtConta.Text;
                boleto.Digito = txtContaDigito.Text;
                boleto.IniciaisBoleto = txtDigitosIniciais.Text.ToInt32();
                boleto.ContractID = txtNroContrato.Text;

                log.GravarLog(EventoLog.ChamadaServico, new { pv, boleto });

                using (var contexto = new ContextoWCF<DataCashService.DataCashServiceClient>())
                {
                    retorno = contexto.Cliente.GerenciarBoleto(out mensagemErro, pv, boleto);

                    if (mensagemErro.CodigoRetorno != 0)
                        base.ExibirPainelExcecao(mensagemErro.MensagemRetorno, mensagemErro.CodigoRetorno.ToString());

                }

                log.GravarLog(EventoLog.RetornoServico, new { retorno, mensagemErro });
            }

            return retorno;
        }

        #endregion
    }
}
