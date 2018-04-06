using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Credenciamento.Sharepoint.GECelulas;
using System.ServiceModel;

namespace Redecard.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Redecard.PN.Credenciamento
{
    public partial class ResumoProposta : UserControlCredenciamentoBase
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
                ltlCnpjCpf.Text = Credenciamento.TipoPessoa.Equals("J") ? Credenciamento.CNPJ : Credenciamento.CPF;
                ltlRazaoSocial.Text = Credenciamento.TipoPessoa.Equals("J") ? Credenciamento.RazaoSocial : Credenciamento.NomeCompleto;
                ltlCelula.Text = String.Format(@"{0} - {1}", Credenciamento.Celula, GetDescricaoCelula());
                ltlTipoComercializacao.Text = Credenciamento.TipoComercializacaoDescricao;
            }
            catch (FaultException<GECelulas.ModelosErroServicos> fe)
            {
                Logger.GravarErro("Credenciamento - Dados Iniciais", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.CodErro.ToString());
            }
            catch (TimeoutException te)
            {
                Logger.GravarErro("Credenciamento - Dados Iniciais", te);
                SharePointUlsLog.LogErro(te);
                base.ExibirPainelExcecao(te.Message, CODIGO_ERRO);
            }
            catch (CommunicationException ce)
            {
                Logger.GravarErro("Credenciamento - Dados Iniciais", ce);
                SharePointUlsLog.LogErro(ce);
                base.ExibirPainelExcecao(ce.Message, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Credenciamento - Dados Iniciais", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Retorna descricao da celula
        /// </summary>
        /// <returns></returns>
        private String GetDescricaoCelula()
        {
            CelulasoListaDadosCadastraisPorCanal[] retorno;

            using (Logger log = Logger.IniciarLog("Atualiza situação da proposta"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new { 
                    Credenciamento.Canal,
                    Credenciamento.Celula
                });

                using (var contexto = new ContextoWCF<ServicoPortalGECelulasClient>())
                {
                    retorno = contexto.Cliente.ListaDadosCadastraisPorCanal(Credenciamento.Canal, Credenciamento.Celula, null);
                }

                log.GravarLog(EventoLog.RetornoServico, new { 
                    retorno
                });
            }

            return retorno[0].NomeCelula;
        }
    }
}
