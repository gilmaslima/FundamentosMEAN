using System;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Boston.Sharepoint.Base;
using Redecard.PN.Boston.Sharepoint.Negocio;
using Redecard.PN.Comum;
using System.Linq;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.Boston.Sharepoint.Layouts.Redecard.PN.Boston.Sharepoint
{
    public partial class DadosBancarios : BostonBasePage
    {
        #region [ Eventos ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Bancarios - Page Load"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        MudarLogoOrigemCredenciamento();
                        CarregarControles();
                        CarregarDadosBancariosDaSessao();
                    }
                }
                catch (FaultException<GEBancos.ModelosErroServicos> fe)
                {
                    Logger.GravarErro("Boston - Dados Bancários", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodErro ?? 600);
                }
                catch (TimeoutException te)
                {
                    Logger.GravarErro("Boston - Dados Bancários", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (CommunicationException ce)
                {
                    Logger.GravarErro("Boston - Dados Bancários", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Boston - Dados Bancários", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento do clique no botão de continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Bancarios - Continuar"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(ltlNomeAgencia.Text))
                    {
                        Page.Validate();
                        if (Page.IsValid)
                        {
                            CarregarDadosBancariosParaSessao();
                            Int32 codRetorno = SalvarDados();

                            String descRetorno = String.Empty;
                            Int32 numPdv = 0;
                            Int32 numSolicitacao = DadosCredenciamento.NumOcorrencia ?? 0;

                            codRetorno = FinalizaCrendenciamento(out descRetorno, out numPdv, ref numSolicitacao);
                            CarregarDadosConfirmacaoParaSessao(numPdv, numSolicitacao);

                            if (codRetorno == 0)
                                Response.Redirect("EscolhaEquipamento.aspx", false);
                            else
                                base.ExibirPainelExcecao(MENSAGEM, codRetorno);
                        }
                    }
                    else
                    {
                        base.ExibirPainelExcecao("A agência informada está incorreta. Por favor, insira a agência correta antes de seguir para o próximo passo", CODIGO_ERRO);
                    }
                }
                catch (FaultException<PNTransicoes.GeneralFault> fe)
                {
                    Logger.GravarErro("Boston - Dados Bancários", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(MENSAGEM, fe.Detail.Codigo);
                }
                catch (FaultException<GEContaCorr.ModelosErroServicos> fe)
                {
                    Logger.GravarErro("Boston - Dados Bancários", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodErro ?? 600);
                }
                catch (TimeoutException te)
                {
                    Logger.GravarErro("Boston - Dados Bancários", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (CommunicationException ce)
                {
                    Logger.GravarErro("Boston - Dados Bancários", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Boston - Dados Bancários", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Envia os dados para finalizar o crendenciamento
        /// </summary>
        /// <returns></returns>
        private Int32 FinalizaCrendenciamento(out String descRetorno, out Int32 numPdv, ref Int32 numSolicitacao) {
            return Servicos.TransicaoPasso4(DadosCredenciamento, out descRetorno, out numPdv, ref numSolicitacao);
        }

        /// <summary>
        /// Carrega os dados dos controles da página para a sessão
        /// </summary>
        private void CarregarDadosConfirmacaoParaSessao(Int32 numPdv, Int32 numSolicitacao) {
            DadosCredenciamento.ConcordoContratoAdesao = true;
            DadosCredenciamento.NumPdv = numPdv;
            DadosCredenciamento.NumOcorrencia = numSolicitacao;
        }

        /// <summary>
        /// Evento do clique no botão de voltar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("DadosCliente.aspx");
        }

        /// <summary>
        /// Evento de troca do valor selecionado para combo box banco
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlBanco_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (Request.Form.Get("__EVENTTARGET") == ddlBanco.UniqueID)
                {
                    txtAgencia.Text = String.Empty;
                    txtContaCorrente.Text = String.Empty;
                    ltlNomeAgencia.Text = String.Empty;

                    ltlTextoAuxiliar.Text = ddlBanco.SelectedValue.Equals("104")
                        ? "Favor incluir os dígitos do código de operação, seguido de <br />quantos zeros forem necessários, antes do número da conta <br />e completando o total de 10 números. Exemplo: 1000099999. <br />Caso dígito seja letra, substituir por 0."
                        : "Caso dígito seja letra, substituir por 0.";
                }
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Boston - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecaoAsync(MENSAGEM, CODIGO_ERRO, upAgencia);
            }
        }

        /// <summary>
        /// Evento que popula o nome da Agência ao trocar o texto do campo Agência
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtAgencia_TextChanged(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Dados Bancarios - txtAgencia Changed"))
            {
                try
                {
                    if (!String.IsNullOrEmpty(txtAgencia.Text))
                    {
                        Int32 codigoAgencia = txtAgencia.Text.ToInt32();
                        Int32 codigoBanco = ddlBanco.SelectedValue.ToInt32();
                        String mensagemRetorno;
                        Int32 codigoRetorno;

                        if (Servicos.ValidaAgencia(codigoBanco, codigoAgencia, out mensagemRetorno, out codigoRetorno))
                            ltlNomeAgencia.Text = Servicos.GetNomeAgencia(codigoBanco, codigoAgencia);
                        else
                            ltlNomeAgencia.Text = String.Empty;
                    }
                    else
                        ltlNomeAgencia.Text = String.Empty;
                }
                catch (FaultException<GEAgencias.ModelosErroServicos> fe)
                {
                    Logger.GravarErro("Boston - Dados Bancários", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecaoAsync(MENSAGEM, fe.Detail.CodErro ?? 600, upAgencia);
                }
                catch (TimeoutException te)
                {
                    Logger.GravarErro("Boston - Dados Bancários", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecaoAsync(MENSAGEM, CODIGO_ERRO, upAgencia);
                }
                catch (CommunicationException ce)
                {
                    Logger.GravarErro("Boston - Dados Bancários", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecaoAsync(MENSAGEM, CODIGO_ERRO, upAgencia);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Boston - Dados Bancários", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecaoAsync(MENSAGEM, CODIGO_ERRO, upAgencia);
                }
            }
        }

        /// <summary>
        /// Validação Server Side do número de Conta Corrente
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvContaCorrente_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                args.IsValid = false;

                Int64 cpfCnpj = DadosCredenciamento.CPF_CNPJ.CpfCnpjToLong();
                Char tipoPessoa = DadosCredenciamento.TipoPessoa;
                Int32 codigoBanco = ddlBanco.SelectedValue.ToInt32();
                Int32 codigoAgencia = txtAgencia.Text.ToInt32();
                Int64 contaCorrente = txtContaCorrente.Text.ToInt64();

                if(Servicos.ValidaContaCorrente(codigoBanco, codigoAgencia, contaCorrente))
                    if (Servicos.ValidaDomicilioBancarioDuplicado(cpfCnpj, codigoBanco, codigoAgencia, contaCorrente, tipoPessoa))
                        args.IsValid = true;
                    else
                        base.ExibirPainelExcecao("Dados Bancários já existentes na Rede para outro CNPJ/CPF. Por favor, confirmar se os dados estão corretos.", 600);
            }
            catch (FaultException<GEContaCorr.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Boston - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodErro ?? 600);
            }
            catch (FaultException<GEDomBancario.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Boston - Dados Bancários", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodErro ?? 600);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Boston - Dados Bancários", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Boston - Dados Bancários", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Boston - Dados Bancários", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ Métodos ]

        /// <summary>
        /// Muda a imagem e background do logotipo da Masterpage caso a origem seja diferente
        /// </summary>
        private void MudarLogoOrigemCredenciamento()
        {
            HiddenField hdnJsOrigem = (HiddenField)this.Master.FindControl("hdnJsOrigem");
            hdnJsOrigem.Value = String.Format("{0}-{1}", DadosCredenciamento.Canal, DadosCredenciamento.Celula);
        }

        /// <summary>
        /// Carrega dados da sessão para os controles da página
        /// </summary>
        private void CarregarDadosBancariosDaSessao()
        {
            ddlBanco.SelectedValue = DadosCredenciamento.CodigoBanco;
            txtAgencia.Text = DadosCredenciamento.CodigoAgencia;
            txtContaCorrente.Text = DadosCredenciamento.ContaCorrente;
        }

        /// <summary>
        /// Carrega dados dos controles da página para sessão
        /// </summary>
        private void CarregarDadosBancariosParaSessao()
        {
            DadosCredenciamento.CodigoBanco = ddlBanco.SelectedItem.Value;
            DadosCredenciamento.DescricaoBanco = ddlBanco.SelectedItem.Text;
            DadosCredenciamento.CodigoAgencia = txtAgencia.Text;
            DadosCredenciamento.DescricaoAgencia = ltlNomeAgencia.Text;
            DadosCredenciamento.ContaCorrente = txtContaCorrente.Text;
        }

        /// <summary>
        /// Carrega os valores dos controles da página
        /// </summary>
        private void CarregarControles()
        {
            CarregarBancos();
            CarregarNomeAgencia();
        }

        /// <summary>
        /// Carrega o nome da agência
        /// </summary>
        private void CarregarNomeAgencia()
        {
            if (!String.IsNullOrEmpty(DadosCredenciamento.CodigoAgencia) && !String.IsNullOrEmpty(DadosCredenciamento.CodigoBanco))
            {
                Int32 codigoAgencia = DadosCredenciamento.CodigoAgencia.ToInt32();
                Int32 codigoBanco = DadosCredenciamento.CodigoBanco.ToInt32();

                ltlNomeAgencia.Text = Servicos.GetNomeAgencia(codigoBanco, codigoAgencia);
            }
        }

        /// <summary>
        /// Popula os valores da combobox de Bancos
        /// </summary>
        private void CarregarBancos()
        {
            ddlBanco.Items.Clear();

            var bancos = Servicos.GetBancos('C');
            ddlBanco.Items.AddRange(bancos.ToArray());
        }

        /// <summary>
        /// Persiste os dados no BD
        /// </summary>
        private Int32 SalvarDados()
        {
            return Servicos.TransicaoPasso3(DadosCredenciamento);
        }

        #endregion
    }
}
