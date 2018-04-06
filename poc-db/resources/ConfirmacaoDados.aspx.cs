using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Boston.Sharepoint.Base;
using System.Web.UI.WebControls;
using Redecard.PN.Comum;
using Redecard.PN.Boston.Sharepoint.Negocio;
using System.ServiceModel;

namespace Redecard.PN.Boston.Sharepoint.Layouts.Redecard.PN.Boston.Sharepoint
{
    public partial class ConfirmacaoDados : BostonBasePage
    {
        #region [ Eventos ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Confirmação Dados - Page Load"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        MudarLogoOrigemCredenciamento();
                        CarregarDadosConfirmacaoDaSessao();
                    }
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Boston - Confirmação", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Muda a imagem e background do logotipo da Masterpage caso a origem seja diferente
        /// </summary>
        private void MudarLogoOrigemCredenciamento()
        {
            HiddenField hdnJsOrigem = (HiddenField)this.Master.FindControl("hdnJsOrigem");
            hdnJsOrigem.Value = String.Format("{0}-{1}", DadosCredenciamento.Canal, DadosCredenciamento.Celula);
        }

        /// <summary>
        /// Evento que popula o repeater de proprietários
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptProprietarios_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Literal ltlTituloProprietario = (Literal)e.Item.FindControl("ltlProprietarioLabel");
                Literal ltlProprietario = (Literal)e.Item.FindControl("ltlProprietario");

                ltlTituloProprietario.Visible = (e.Item.ItemIndex == 0);

                String proprietario = String.Format("{0} - {1}% de participação", ((Proprietario)e.Item.DataItem).NomeProprietario, ((Proprietario)e.Item.DataItem).PartAcionaria, "%");
                ltlProprietario.Text = proprietario;
            }
        }

        /// <summary>
        /// Evento do clique no botão de continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (var log = Logger.IniciarLog("Confirmação Dados - Continuar"))
            {
                try
                {
                    Page.Validate();
                    if (Page.IsValid)
                    {
                        String descRetorno = String.Empty;
                        Int32 numPdv = 0;
                        Int32 numSolicitacao = DadosCredenciamento.NumOcorrencia ?? 0;

                        Int32 codRetorno = 0;
                        // FinalizaCrendenciamento(out descRetorno, out numPdv, ref numSolicitacao);
                        //CarregarDadosConfirmacaoParaSessao(numPdv, numSolicitacao);

                        if (codRetorno == 0)
                        {    
                            Response.Redirect("EscolhaEquipamento.aspx", false);
                        }
                        else
                        {
                            Logger.GravarErro("Boston - Confirmação", new Exception(descRetorno));
                            SharePointUlsLog.LogErro(new Exception(descRetorno));
                            if (codRetorno == 699 || codRetorno == 698)
                            {
                                DadosCredenciamento.CodigoErro = codRetorno.ToString();
                                Response.Redirect("ImpossivelContinuar.aspx", false);
                            }
                            else
                                base.ExibirPainelExcecao(MENSAGEM, codRetorno);
                        }
                    }
                }
                catch (FaultException<PNTransicoes.GeneralFault> fe)
                {
                    Logger.GravarErro("Boston - Confirmação", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(MENSAGEM, fe.Detail.Codigo);
                }
                catch (TimeoutException te)
                {
                    Logger.GravarErro("Boston - Confirmação", te);
                    SharePointUlsLog.LogErro(te);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (CommunicationException ce)
                {
                    Logger.GravarErro("Boston - Confirmação", ce);
                    SharePointUlsLog.LogErro(ce);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Boston - Confirmação", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento do clique no botão de voltar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("DadosBancarios.aspx");
        }

        /// <summary>
        /// Validação do checkbox de "Concordo"
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void cvConcordo_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = chkConcordo.Checked;
        }

        #endregion

        #region [ Métodos ]

        /// <summary>
        /// Carrega dados da sessão para os controles da página
        /// </summary>
        private void CarregarDadosConfirmacaoDaSessao()
        {
            hlContratoAdesao.NavigateUrl = String.Format("/_layouts/download.aspx?SourceUrl={0}/Documents/PDF's/MobileRede.pdf", SPContext.Current.Web.Url);

            if (DadosCredenciamento.TipoPessoa == 'F')
            {
                ltlCNPJCPFLabel.Text = "CPF:";
                ltlRazaoSocialLabel.Text = "Nome Completo:";
                ltlEnderecoComercialLabel.Text = "Endereço:";
                ltlTelefone2Label.Text = "Celular:";
                pnlProprietarios.Visible = false;
            }

            ltlCNPJCPF.Text = DadosCredenciamento.CPF_CNPJ;
            ltlRazaoSocial.Text = DadosCredenciamento.RazaoSocial;
            ltlNomeFantasia.Text = DadosCredenciamento.NomeFantasia;

            ltlLogradouro.Text = DadosCredenciamento.EnderecoComercial.Logradouro;
            ltlNumero.Text = DadosCredenciamento.EnderecoComercial.Numero;
            ltlBairro.Text = DadosCredenciamento.EnderecoComercial.Bairro;
            ltlCidade.Text = DadosCredenciamento.EnderecoComercial.Cidade;
            ltlEstado.Text = DadosCredenciamento.EnderecoComercial.Estado;
            ltlCEP.Text = DadosCredenciamento.EnderecoComercial.CEP;
            ltlEmail.Text = DadosCredenciamento.Email;
            ltlTelefone1.Text = String.Format("{0} {1} {2}", DadosCredenciamento.DDDTelefone1, DadosCredenciamento.NumeroTelefone1, DadosCredenciamento.RamalTelefone1);
            ltlTelefone2.Text = String.Format("{0} {1} {2}", DadosCredenciamento.DDDTelefone2, DadosCredenciamento.NumeroTelefone2, DadosCredenciamento.RamalTelefone2);

            rptProprietarios.DataSource = DadosCredenciamento.Proprietarios;
            rptProprietarios.DataBind();

            chkConcordo.Checked = DadosCredenciamento.ConcordoContratoAdesao;
        }

        /// <summary>
        /// Carrega os dados dos controles da página para a sessão
        /// </summary>
        [Obsolete("Não usar esse método", true)]
        private void CarregarDadosConfirmacaoParaSessao(Int32 numPdv, Int32 numSolicitacao)
        {
            DadosCredenciamento.ConcordoContratoAdesao = chkConcordo.Checked;
            DadosCredenciamento.NumPdv = numPdv;
            DadosCredenciamento.NumOcorrencia = numSolicitacao;
        }

        /// <summary>
        /// Envia os dados para finalizar o crendenciamento
        /// </summary>
        /// <returns></returns>
        [Obsolete("Não usar esse método", true)]
        private Int32 FinalizaCrendenciamento(out String descRetorno, out Int32 numPdv, ref Int32 numSolicitacao)
        {
            return Servicos.TransicaoPasso4(DadosCredenciamento, out descRetorno, out numPdv, ref numSolicitacao);
        }

        #endregion
    }
}
