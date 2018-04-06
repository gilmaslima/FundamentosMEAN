using Rede.PN.ManinhaContaCerta.Core.Web.Controles.Portal;
using Redecard.PN.Comum;
using System;
using System.ServiceModel;
using MaquininhaServico = Rede.PN.MaquininhaContaCerta.SharePoint.MaquininhaServico;

namespace Rede.PN.MaquininhaContaCerta.Sharepoint.WebParts.DetalheOferta
{
    /// <summary>
    /// User control principal da página Maquininha Conta Certa
    /// </summary>
    public partial class DetalheOfertaUserControl : UserControlBase
    {
        /// <summary>
        /// Carregamento da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger log = Logger.IniciarLog("Maquininha Conta Certa - Resumo"))
            {
                try
                {
                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["q"]);
                    Int16 codSitContrato = queryString["codSitContrato"].ToInt16Null(0).Value;
                    DateTime dataFimVigencia = queryString["dataFimVigencia"].ToDateTimeNull() ?? DateTime.MinValue;


                    // consulta os dados do resumo
                    using (var context = new ContextoWCF<MaquininhaServico.MaquininhaContaCertaClient>())
                    {
                        var contrato = context.Cliente.ConsultaContrato(
                            SessaoAtual.CodigoEntidade, 
                            dataFimVigencia,
                            codSitContrato);

                        Boolean contratoCancelado = String.Compare(contrato.CodigoSituacaoContrato, "C", true) == 0;

                        // conteúdo do bigheader do bloco do resumo
                        if (!contratoCancelado)
                        {
                            ltrValorAluguelPrimeiroTerminal.Text = contrato.ValorTerminais.ToString("n");
                            ltrValorAluguelDemaisTerminais.Text = contrato.ValorPacoteBasico.ToString("n");
                            ltrQuantidadeTerminais.Text = contrato.QtdTerminaisDoPacote.ToString();
                        }
                        else
                        {
                            ltrValorAluguelPrimeiroTerminal.Text = "-";
                            ltrValorAluguelPrimeiroTerminalMoeda.Visible = false;
                            ltrValorAluguelDemaisTerminais.Text = "-";
                            ltrValorAluguelDemaisTerminaisMoeda.Visible = false;
                            ltrQuantidadeTerminais.Text = "-";
                        }

                        // demais informações do resumo
                        ltrDataContratacao.Text = contrato.DataContrato.ToString("dd/MM/yyyy");
                        ltrPeriodoVigencia.Text = String.Format("{0} a {1}",
                            contrato.DataInicioVigencia.ToString("dd/MM/yyyy"),
                            contrato.DataFimVigencia.Year == 9999 ? "indeterminada" : contrato.DataFimVigencia.ToString("dd/MM/yyyy"));
                        ltrTipoTecnologia.Text = contrato.CodigoTecnologia;
                        ltrDescricaoCanal.Text = contrato.DescricaoCanalItau;
                        ltrCodigoOferta.Text = contrato.CodigoOfertaPacote;
                        ltrNumeroCnpj.Text = contrato.Cnpj;
                    }
                }
                catch (CommunicationException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);
                    ExibirExcecao(FONTE, CODIGO_ERRO);
                }
                catch (TimeoutException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);
                    ExibirExcecao(FONTE, CODIGO_ERRO);
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    log.GravarErro(ex);
                    ExibirExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Exibição de erro via QuadroAviso
        /// </summary>
        /// <param name="fonte"></param>
        /// <param name="codigoErro"></param>
        private void ExibirExcecao(String fonte, Int32 codigoErro)
        {
            pnlMain.Visible = false;

            // base.ExibirPainelExcecao(fonte, codigoErro);
            String msg = base.RetornarMensagemErro(fonte, codigoErro);

            quadroAviso.Mensagem = msg;
            quadroAviso.TipoQuadro = TipoQuadroAviso.Erro;
            quadroAviso.Visible = true;
        }
    }
}
