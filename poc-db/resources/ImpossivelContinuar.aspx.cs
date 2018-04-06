using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.PN.Boston.Sharepoint.Base;
using Redecard.PN.Comum;
using Redecard.PN.Boston.Sharepoint.Negocio;
using System.ServiceModel;
using System.Web.UI.WebControls;

namespace Redecard.PN.Boston.Sharepoint.Layouts.Boston
{
    public partial class ImpossivelContinuar : BostonBasePage
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

                    ltlCodigoErro.Text = DadosCredenciamento.CodigoErro;

                    if (String.Compare(DadosCredenciamento.CodigoErro, "699") == 0 || String.Compare(DadosCredenciamento.CodigoErro, "698") == 0)
                    {
                        String mensagemRetorno = String.Empty;
                        Char indSituacaoProposta = DadosCredenciamento.CodigoErro.Equals("699") ? 'X' : 'E';

                        Int32 codRetorno = Servicos.AtualizaSituacaoProposta(DadosCredenciamento.TipoPessoa, DadosCredenciamento.CPF_CNPJ.CpfCnpjToLong(),
                            DadosCredenciamento.NumeroSequencia, indSituacaoProposta, DadosCredenciamento.Usuario, null, DadosCredenciamento.NumPdv, 1, out mensagemRetorno);

                        if(codRetorno == 0 && DadosCredenciamento.NumOcorrencia != null)
                            codRetorno = Servicos.CancelaOcorrenciaCredenciamento("PORTAL", (Int32)DadosCredenciamento.NumOcorrencia, 1, "Impossível gerar o PV para a tecnologia MPOS", "CREDENCIAMENTO PORTAL", out mensagemRetorno);

                        if (codRetorno != 0)
                        {
                            Logger.GravarErro("Boston - Impossível Continuar", new Exception(mensagemRetorno));
                            SharePointUlsLog.LogErro(new Exception(mensagemRetorno));
                            base.ExibirPainelExcecao(MENSAGEM, codRetorno);
                        }
                    }
                    else if (String.Compare(DadosCredenciamento.CodigoErro, "655") == 0)
                    {
                        pnlMensagemPadrao.Visible = false;
                        pnlMensagemProprietario.Visible = true;
                    }
                }
            }
            catch (FaultException<WMOcorrencia.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Boston - Impossível Continuar", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodErro ?? 600);
            }
            catch (FaultException<WFProposta.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Boston - Impossível Continuar", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(MENSAGEM, fe.Detail.CodigoErro ?? 600);
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Boston - Impossível Continuar", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Boston - Impossível Continuar", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(MENSAGEM, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Boston - Impossível Continuar", ex);
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
        /// Evento do Clique no botão continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            if (DadosCredenciamento.Canal == 26)
                Response.Redirect("DadosIniciais.aspx?dados=W2kQvAu6568p5ijcECP4ScyjJcmdboFjjucHoI1oc5FIDNDMA3yhlJw1a59hJwzo");
            else
                Response.Redirect("DadosIniciais.aspx");
        }
    }
}
