using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.ServiceModel;
using Redecard.PN.Comum;
using Redecard.PN.Boston.Sharepoint.Base;
using Redecard.PN.Boston.Sharepoint.Negocio;
using System.Web.UI.WebControls;

namespace Redecard.PN.Boston.Sharepoint.Layouts.Boston
{
    public partial class TransacaoNaoAprovada : BostonBasePage
    {
        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    MudarLogoOrigemCredenciamento();
                    String mensagemRetorno = String.Empty;
                    Int32 codigoRetorno = 0;

                    codigoRetorno = AtualizaTaxaAtivacaoPropostaMPOS(out mensagemRetorno);

                    if (codigoRetorno != 0)
                    {
                        Logger.GravarErro("Boston - Escolha Equipamento", new Exception(mensagemRetorno));
                        SharePointUlsLog.LogErro(new Exception(mensagemRetorno));
                        base.ExibirPainelExcecao(MENSAGEM, codigoRetorno);
                    }
                }
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Boston - Escolha Equipamento", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodigoErro ?? 600);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Boston - Escolha Equipamento", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Boston - Escolha Equipamento", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Boston - Escolha Equipamento", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
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
        /// Atualiza a fase de filiação da proposta
        /// </summary>
        /// <param name="mensagemRetorno"></param>
        /// <returns></returns>
        private Int32 AtualizaTaxaAtivacaoPropostaMPOS(out String mensagemRetorno)
        {
            return Servicos.AtualizaTaxaAtivacaoPropostaMPOS(DadosCredenciamento.TipoPessoa, DadosCredenciamento.CPF_CNPJ.CpfCnpjToLong(), DadosCredenciamento.NumeroSequencia,
                    DadosCredenciamento.TaxaAtivacao, 7, DadosCredenciamento.Usuario, out mensagemRetorno);
        }

        /// <summary>
        /// Evento do Clique no botão continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("EscolhaEquipamento.aspx");
        }
    }
}
