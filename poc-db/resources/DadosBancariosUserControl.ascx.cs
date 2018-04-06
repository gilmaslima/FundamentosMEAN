using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Sharepoint.GEBancos;
using Redecard.PN.Credenciamento.Sharepoint.GEAgencias;
using Redecard.PN.Credenciamento.Sharepoint.WFDomBancario;
using System.ServiceModel;
using Redecard.PN.Credenciamento.Sharepoint.WFProposta;
using Redecard.PN.Credenciamento.Sharepoint.GEContaCorr;
using Redecard.PN.Credenciamento.Sharepoint.PNTransicoesServico;
using System.Collections.Generic;
using Redecard.PN.Credenciamento.Sharepoint.GEDomBancario;

namespace Redecard.PN.Credenciamento.Sharepoint.WebParts.DadosBancarios
{
    public partial class DadosBancariosUserControl : UserControlCredenciamentoBase
    {
        #region [ Propriedades ]

        #endregion

        #region [ Eventos de Página ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Credenciamento.Fase < 5)
                    Credenciamento.Fase = 5;

                Page.MaintainScrollPositionOnPostBack = true;
                Page.Title = "Dados Bancários";

                if (!Page.IsPostBack)
                {
                    CarregarBancosCredito();
                    CarregarBancosDebito();
                    CarregarBancosConstrucard();

                    // Carregar dados
                    ddlBancoCredito.SelectedValue = Credenciamento.CodBancoCredito != 0 ? Credenciamento.CodBancoCredito.ToString() : ddlBancoCredito.SelectedValue;
                    txtAgenciaCredito.Text = Credenciamento.AgenciaCredito != 0 ? Credenciamento.AgenciaCredito.ToString() : txtAgenciaCredito.Text;
                    txtContaCredito.Text = !String.IsNullOrEmpty(Credenciamento.ContaCredito) ? Credenciamento.ContaCredito : txtContaCredito.Text;

                    if (Credenciamento.CodTipoEstabelecimento == 1 && Credenciamento.LocalPagamento == 2)
                    {
                        ddlBancoCredito.Enabled = false;
                        txtAgenciaCredito.Enabled = false;
                        txtContaCredito.Enabled = false;
                    }

                    if ((Credenciamento.CodBancoDebito != 0 && Credenciamento.CodBancoDebito != Credenciamento.CodBancoCredito) ||
                        (Credenciamento.AgenciaDebito != 0 && Credenciamento.AgenciaDebito != Credenciamento.AgenciaCredito) ||
                        (!String.IsNullOrEmpty(Credenciamento.ContaDebito) && Credenciamento.ContaDebito.Trim() != Credenciamento.ContaCredito.Trim()))
                    {
                        pnlDebito.Visible = true;
                        chkDebitoCredito.Checked = false;
                        ddlBancoDebito.SelectedValue = Credenciamento.CodBancoDebito != 0 ? Credenciamento.CodBancoDebito.ToString() : ddlBancoDebito.SelectedValue;
                        txtAgenciaDebito.Text = Credenciamento.AgenciaDebito != 0 ? Credenciamento.AgenciaDebito.ToString() : txtAgenciaDebito.Text;
                        txtContaDebito.Text = !String.IsNullOrEmpty(Credenciamento.ContaDebito) ? Credenciamento.ContaDebito : txtContaDebito.Text;
                    }

                    if (Credenciamento.ProdutosCredito == null || Credenciamento.ProdutosCredito.Count == 0)
                    {
                        chkDebitoCredito.Visible = false;
                        pnlCredito.Visible = false;

                        pnlDebito.Visible = true;
                    }

                    if (Credenciamento.ProdutosDebito == null || Credenciamento.ProdutosDebito.Count == 0)
                    {
                        chkDebitoCredito.Visible = false;
                        pnlDebito.Visible = false;
                    }


                    if (Credenciamento.ProdutosConstrucard != null && Credenciamento.ProdutosConstrucard.Count > 0)
                    {
                        pnlConstrucard.Visible = true;
                        ddlBancoConstrucard.SelectedValue = Credenciamento.CodBancoConstrucard != 0 ? Credenciamento.CodBancoConstrucard.ToString() : ddlBancoConstrucard.SelectedValue;
                        txtAgenciaConstrucard.Text = Credenciamento.AgenciaConstrucard != 0 ? Credenciamento.AgenciaConstrucard.ToString() : txtAgenciaConstrucard.Text;
                        txtContaConstrucard.Text = !String.IsNullOrEmpty(Credenciamento.ContaConstrucard) ? Credenciamento.ContaConstrucard : txtContaConstrucard.Text;
                    }

                    if (!String.IsNullOrEmpty(txtAgenciaCredito.Text))
                        lblAgenciaCredito.Text = CarregarAgencia(ddlBancoCredito.SelectedValue.ToInt32(), txtAgenciaCredito.Text.ToInt32()).NomeAgencia;

                    if (!String.IsNullOrEmpty(txtAgenciaDebito.Text))
                        lblAgenciaDebito.Text = CarregarAgencia(ddlBancoDebito.SelectedValue.ToInt32(), txtAgenciaDebito.Text.ToInt32()).NomeAgencia;

                    if (!String.IsNullOrEmpty(txtAgenciaConstrucard.Text))
                        lblAgenciaConstrucard.Text = CarregarAgencia(ddlBancoConstrucard.SelectedValue.ToInt32(), txtAgenciaConstrucard.Text.ToInt32()).NomeAgencia;
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ Eventos de Controles ]

        /// <summary>
        /// Evento do botão continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            Int32 codRetorno = SalvarDados();
            if (codRetorno == 0)
                Response.Redirect("pn_escolhatecnologia.aspx", false);
            else if (codRetorno != 399)
                base.ExibirPainelExcecao("Redecard.PN.Credenciamento.Servicos", codRetorno);
        }

        /// <summary>
        /// Evento do botão Parar e Salvar Proposta
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            SalvarDados();

            Credenciamento = new Modelo.Credenciamento();
            Response.Redirect("pn_dadosiniciais.aspx", false);
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("pn_dadosoperacionais.aspx", false);
        }

        /// <summary>
        /// Mostra e esconde a informações de débito ao mudar o valor do checkbox chkDebitoCredito
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void chkDebitoCredito_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                pnlDebito.Visible = !pnlDebito.Visible;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Valida número da agência de Crédito
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtAgenciaCredito_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Int32 codBanco = ddlBancoCredito.SelectedValue.ToInt32();
                Int32 codAgencia = txtAgenciaCredito.Text.ToInt32();

                if (ValidaAgencia(codBanco, codAgencia))
                {
                    ConsultaDetalheAgencia agencia = CarregarAgencia(codBanco, codAgencia);
                    if (agencia != null)
                        lblAgenciaCredito.Text = agencia.NomeAgencia;
                }
                else
                {
                    lblAgenciaCredito.Text = String.Empty;
                }
            }
            catch (FaultException<GEAgencias.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Valida número da agência de Débito
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtAgenciaDebito_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Int32 codBanco = ddlBancoDebito.SelectedValue.ToInt32();
                Int32 codAgencia = txtAgenciaDebito.Text.ToInt32();

                if (ValidaAgencia(codBanco, codAgencia))
                {
                    ConsultaDetalheAgencia agencia = CarregarAgencia(codBanco, codAgencia);
                    if (agencia != null)
                        lblAgenciaDebito.Text = agencia.NomeAgencia;
                }
                else
                {
                    lblAgenciaDebito.Text = String.Empty;
                }
            }
            catch (FaultException<GEAgencias.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (FaultException<GEBancos.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);

            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Valida número da Agência Construcard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtAgenciaConstrucard_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Int32 codBanco = ddlBancoConstrucard.SelectedValue.ToInt32();
                Int32 codAgencia = txtAgenciaConstrucard.Text.ToInt32();

                if (ValidaAgencia(codBanco, codAgencia))
                {
                    ConsultaDetalheAgencia agencia = CarregarAgencia(codBanco, codAgencia);
                    if (agencia != null)
                        lblAgenciaConstrucard.Text = agencia.NomeAgencia;
                }
                else
                {
                    lblAgenciaConstrucard.Text = String.Empty;
                }
            }
            catch (FaultException<GEBancos.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);

            }
            catch (FaultException<GEAgencias.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Validação do número da Agência Crédito
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvAgenciaCredito_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;

                Int32 codBanco = ddlBancoCredito.SelectedValue.ToInt32();
                Int32 codAgencia = txtAgenciaCredito.Text.ToInt32();

                if (ValidaAgencia(codBanco, codAgencia))
                    args.IsValid = true;
            }
            catch (FaultException<GEAgencias.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Validação do número da Agência Débito
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvAgenciaDebito_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;

                Int32 codBanco = ddlBancoDebito.SelectedValue.ToInt32();
                Int32 codAgencia = txtAgenciaDebito.Text.ToInt32();

                if (ValidaAgencia(codBanco, codAgencia))
                    args.IsValid = true;
            }
            catch (FaultException<GEAgencias.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Validação do número da Agência Construcard
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvAgenciaConstrucard_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;

                Int32 codBanco = ddlBancoConstrucard.SelectedValue.ToInt32();
                Int32 codAgencia = txtAgenciaConstrucard.Text.ToInt32();

                if (ValidaAgencia(codBanco, codAgencia))
                    args.IsValid = true;
            }
            catch (FaultException<GEAgencias.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Validação Server Side do número de Conta Corrente Crédito
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvContaCorrenteCredito_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;

                Int32 codBanco = ddlBancoCredito.SelectedValue.ToInt32();
                Int32 codAgencia = txtAgenciaCredito.Text.ToInt32();
                Int64 cc = 0;
                Int64.TryParse(txtContaCredito.Text, out cc);
                Char tipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                Int64 cpfCnpj = tipoPessoa == 'J' ? Credenciamento.CNPJ.CpfCnpjToLong() : Credenciamento.CPF.CpfCnpjToLong();

                if (ValidaContaCorrente(codBanco, codAgencia, cc))
                    if (ValidaDomicilioBancarioDuplicado(cpfCnpj, codBanco, codAgencia, cc, tipoPessoa))
                        args.IsValid = true;
                    else
                        base.ExibirPainelExcecao("Dados Bancários já existentes na Rede para outro CNPJ/CPF. Por favor, confirmar se os dados estão corretos.", "600");
                //else
                //    base.ExibirPainelExcecao("Número da conta corrente inválido.", "300");
            }
            catch (FaultException<GEContaCorr.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Validação Server Side do número de Conta Corrente Débito
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvContaCorrenteDebito_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;

                Int32 codBanco = ddlBancoDebito.SelectedValue.ToInt32();
                Int32 codAgencia = txtAgenciaDebito.Text.ToInt32();
                Int64 cc = 0;
                Int64.TryParse(txtContaDebito.Text, out cc);
                Char tipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                Int64 cpfCnpj = tipoPessoa == 'J' ? Credenciamento.CNPJ.CpfCnpjToLong() : Credenciamento.CPF.CpfCnpjToLong();

                if (ValidaContaCorrente(codBanco, codAgencia, cc))
                    if (ValidaDomicilioBancarioDuplicado(cpfCnpj, codBanco, codAgencia, cc, tipoPessoa))
                        args.IsValid = true;
                    else
                        base.ExibirPainelExcecao("Dados Bancários já existentes na Rede para outro CNPJ/CPF. Por favor, confirmar se os dados estão corretos.", "600");
                //else
                //    base.ExibirPainelExcecao("Número da conta corrente inválido.", "300");
            }
            catch (FaultException<GEContaCorr.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Validação Server Side do número de Conta Corrente Construcard
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvContaCorrenteConstrucard_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;

                Int32 codBanco = ddlBancoConstrucard.SelectedValue.ToInt32();
                Int32 codAgencia = txtAgenciaConstrucard.Text.ToInt32();
                Int64 cc = 0;
                Int64.TryParse(txtContaConstrucard.Text, out cc);
                Char tipoPessoa = Credenciamento.TipoPessoa.ToCharArray()[0];
                Int64 cpfCnpj = tipoPessoa == 'J' ? Credenciamento.CNPJ.CpfCnpjToLong() : Credenciamento.CPF.CpfCnpjToLong();

                if (ValidaContaCorrente(codBanco, codAgencia, cc))
                    if(ValidaDomicilioBancarioDuplicado(cpfCnpj, codBanco, codAgencia, cc, tipoPessoa))
                        args.IsValid = true;
                    else
                        base.ExibirPainelExcecao("Dados Bancários já existentes na Rede para outro CNPJ/CPF. Por favor, confirmar se os dados estão corretos.", "600");
                //else
                //    base.ExibirPainelExcecao("Número da conta corrente inválido.", "300");
            }
            catch (FaultException<GEContaCorr.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);
            }
            catch (PortalRedecardException pe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", pe);
                SharePointUlsLog.LogErro(pe);
                base.ExibirPainelExcecao(pe.Fonte, pe.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Evento de troca do valor selecionado para combo box banco crédito
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlBancoCredito_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Request.Form.Get("__EVENTTARGET") == ddlBancoCredito.UniqueID)
            {
                txtAgenciaCredito.Text = String.Empty;
                txtContaCredito.Text = String.Empty;
                lblAgenciaCredito.Text = String.Empty;
            }
        }

        /// <summary>
        /// Evento de troca do valor selecionado para combo box banco débito
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlBancoDebito_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Request.Form.Get("__EVENTTARGET") == ddlBancoDebito.UniqueID)
            {
                txtAgenciaDebito.Text = String.Empty;
                txtContaDebito.Text = String.Empty;
                lblAgenciaDebito.Text = String.Empty;
            }
        }

        #endregion

        #region [ Métodos Auxiliares ]

        /// <summary>
        /// Busca lista de bancos e carrega dropdowns de banco crédito
        /// </summary>
        private void CarregarBancosCredito()
        {
            ServicoPortalGEBancoClient client = new ServicoPortalGEBancoClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Lista de Bancos"))
                {

                    ddlBancoCredito.Items.Clear();

                    BancosListaDadosCadastraisReduzidos[] bancos = client.ListaDadosCadastraisReduzidos('C');
                    client.Close();

                    foreach (BancosListaDadosCadastraisReduzidos banco in bancos)
                    {
                        ListItem item = new ListItem(String.Format("{0} - {1}", banco.CodBancoCompensacao, banco.NomeBanco), banco.CodBancoCompensacao.ToString());
                        ddlBancoCredito.Items.Add(item);
                    }

                }
            }
            catch (FaultException<GEBancos.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);

            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Bancários", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Bancários", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de bancos e carrega dropdowns de banco débito
        /// </summary>
        private void CarregarBancosDebito()
        {
            ServicoPortalGEBancoClient client = new ServicoPortalGEBancoClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Lista de Bancos"))
                {
                    ddlBancoDebito.Items.Clear();

                    BancosListaDadosCadastraisReduzidos[] bancos = client.ListaDadosCadastraisReduzidos('D');
                    client.Close();

                    foreach (BancosListaDadosCadastraisReduzidos banco in bancos)
                    {
                        ListItem item = new ListItem(String.Format("{0} - {1}", banco.CodBancoCompensacao, banco.NomeBanco), banco.CodBancoCompensacao.ToString());
                        ddlBancoDebito.Items.Add(item);
                    }

                }
            }
            catch (FaultException<GEBancos.ModelosErroServicos> fe)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, (Int32)fe.Detail.CodErro);

            }
            catch (TimeoutException te)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Bancários", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Bancários", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
            }
            catch (Exception ex)
            {
                client.Abort();
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Busca lista de bancos e carrega dropdowns de banco
        /// </summary>
        private void CarregarBancosConstrucard()
        {
            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Lista de Bancos"))
                {
                    ddlBancoConstrucard.Items.Clear();
                    ddlBancoConstrucard.Enabled = false;

                    ListItem item = new ListItem("104 - Caixa Economica Federal", "104");
                    ddlBancoConstrucard.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Valida número da agência 
        /// </summary>
        private Boolean ValidaAgencia(Int32 codBanco, Int32 codAgencia)
        {
            ServicoPortalGEAgenciasClient client = new ServicoPortalGEAgenciasClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Valida número da Agência"))
                {
                    GEAgencias.CodErroDescricaoErro[] erro = client.ValidaAgencias(codBanco, codAgencia);
                    client.Close();

                    if (erro[0].CodErro != 0)
                        return false;

                    return true;
                }
            }
            catch (FaultException<GEAgencias.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Método de validação de conta corrente
        /// </summary>
        /// <param name="codBanco"></param>
        /// <param name="codAgencia"></param>
        /// <param name="cc"></param>
        /// <returns></returns>
        private Boolean ValidaContaCorrente(Int32 codBanco, Int32 codAgencia, Int64 cc)
        {
            ServicoPortalGEContaCorrenteClient client = new ServicoPortalGEContaCorrenteClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Valida número da Conta Corrente"))
                {

                    Logger.GravarLog(String.Format("Banco: {0}, Agencia: {1}, CC: {2}", codBanco, codAgencia, cc));
                    Boolean retorno = client.ChecaDigito(codBanco, codAgencia, cc);
                    client.Close();

                    return retorno;

                }
            }
            catch (FaultException<GEContaCorr.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Carrega dados da Agencia
        /// </summary>
        private ConsultaDetalheAgencia CarregarAgencia(Int32 codBanco, Int32 codAgencia)
        {
            ServicoPortalGEAgenciasClient client = new ServicoPortalGEAgenciasClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Carregar Dados da Agencia"))
                {
                    ConsultaDetalheAgencia[] agencias = client.ConsultaDetalheAgencia(codBanco, codAgencia);
                    client.Close();

                    if (agencias.Length > 0)
                        return agencias[0];

                    return null;
                }
            }
            catch (FaultException<GEAgencias.ModelosErroServicos> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Grava ou atualiza dados da sexta tela
        /// </summary>
        /// <returns></returns>
        private Int32 GravarAtualizarPasso6()
        {
            TransicoesClient client = new TransicoesClient();

            try
            {
                using (Logger log = Logger.IniciarLog("Gravar Dados - Dados Bancários"))
                {
                    PNTransicoesServico.Proposta proposta = PreencheProposta();

                    PNTransicoesServico.DomicilioBancario domBancarioCredito = null;
                    if (Credenciamento.ProdutosCredito != null && Credenciamento.ProdutosCredito.Count > 0)
                        domBancarioCredito = PreencheDomicilioBancario(1, Credenciamento.CodBancoCredito,
                            Credenciamento.NomeBancoCredito, Credenciamento.AgenciaCredito, Credenciamento.ContaCredito);

                    PNTransicoesServico.DomicilioBancario domBancarioDebito = null;
                    if (Credenciamento.ProdutosDebito != null && Credenciamento.ProdutosDebito.Count > 0)
                        domBancarioDebito = PreencheDomicilioBancario(3, Credenciamento.CodBancoDebito,
                            Credenciamento.NomeBancoDebito, Credenciamento.AgenciaDebito, Credenciamento.ContaDebito);

                    PNTransicoesServico.DomicilioBancario domBancarioConstrucard = null;
                    if (Credenciamento.ProdutosConstrucard != null && Credenciamento.ProdutosConstrucard.Count > 0)
                        domBancarioConstrucard = PreencheDomicilioBancario(4, Credenciamento.CodBancoConstrucard,
                            Credenciamento.NomeBancoConstrucard, Credenciamento.AgenciaConstrucard, Credenciamento.ContaConstrucard);

                    Int32 retorno = client.GravarAtualizarPasso6(proposta, domBancarioCredito, domBancarioDebito, domBancarioConstrucard);
                    client.Close();

                    return retorno;
                }
            }
            catch (FaultException<PNTransicoesServico.GeneralFault> fe)
            {
                client.Abort();
                throw fe;
            }
            catch (TimeoutException te)
            {
                client.Abort();
                throw te;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        /// <summary>
        /// Salva dados da tela
        /// </summary>
        /// <returns></returns>
        private Int32 SalvarDados()
        {
            try
            {
                Page.Validate();
                if (Page.IsValid)
                {
                    // Grava dados Domicilio Bancário Crédito
                    if (pnlCredito.Visible)
                    {
                        Credenciamento.CodBancoCredito = ddlBancoCredito.SelectedValue.ToInt32();
                        Credenciamento.NomeBancoCredito = ddlBancoCredito.SelectedItem.Text;
                        Credenciamento.AgenciaCredito = txtAgenciaCredito.Text.ToInt32();
                        Credenciamento.ContaCredito = txtContaCredito.Text;
                    }

                    //Grava dados Domicílio Bancário Débito
                    if (pnlDebito.Visible)
                    {
                        Credenciamento.CodBancoDebito = ddlBancoDebito.SelectedValue.ToInt32();
                        Credenciamento.NomeBancoDebito = ddlBancoDebito.SelectedItem.Text;
                        Credenciamento.AgenciaDebito = txtAgenciaDebito.Text.ToInt32();
                        Credenciamento.ContaDebito = txtContaDebito.Text;
                    }
                    else
                    {
                        Credenciamento.CodBancoDebito = ddlBancoCredito.SelectedValue.ToInt32();
                        Credenciamento.NomeBancoDebito = ddlBancoCredito.SelectedItem.Text;
                        Credenciamento.AgenciaDebito = txtAgenciaCredito.Text.ToInt32();
                        Credenciamento.ContaDebito = txtContaCredito.Text;
                    }

                    //Grava dados Domicílio Bancário Construcard
                    if (Credenciamento.ProdutosConstrucard != null && Credenciamento.ProdutosConstrucard.Count > 0)
                    {
                        Credenciamento.CodBancoConstrucard = ddlBancoConstrucard.SelectedValue.ToInt32();
                        Credenciamento.NomeBancoConstrucard = ddlBancoConstrucard.SelectedItem.Text;
                        Credenciamento.AgenciaConstrucard = txtAgenciaConstrucard.Text.ToInt32();
                        Credenciamento.ContaConstrucard = txtContaConstrucard.Text;
                    }

                    return GravarAtualizarPasso6();
                }
                return 399;
            }
            catch (FaultException<PNTransicoesServico.GeneralFault> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                return fe.Detail.Codigo;
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, 300);
                return 300;
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, 300);
                return 300;
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                return CODIGO_ERRO;
            }
        }

        /// <summary>
        /// Valida se o domicílio bancário está duplicado
        /// </summary>
        private Boolean ValidaDomicilioBancarioDuplicado(Int64 cpfCnpj, Int32 codigoBanco, Int32 codigoAgencia, Int64 numeroConta, Char codigoTipoPessoa)
        {
            CodigoDescricaoPvsDuplicados retorno;

            using (var log = Logger.IniciarLog("Valida Domicílio Bancário Duplicado"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new {
                    cpfCnpj,
                    codigoBanco,
                    codigoAgencia,
                    numeroConta,
                    codigoTipoPessoa
                });

                using(var contexto = new ContextoWCF<ServicoPortalGEDomicilioBancarioClient>())
                {
                    retorno = contexto.Cliente.ValidaDomicilioDuplicado(cpfCnpj, codigoBanco, codigoAgencia, numeroConta, codigoTipoPessoa);
                }

                log.GravarLog(EventoLog.RetornoServico, new
                {
                    retorno
                });
            }

            if (retorno.CodErro != null && retorno.CodErro != 0)
                return false;

            return true;
        }

        #endregion
    }
}
