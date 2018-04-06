using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.OutrosServicos.SharePoint.ZPContaCertaServico;
using System;
using System.ServiceModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.OutrosServicos.SharePoint.ControlTemplates.OutrosServicos.Mdr
{
    public partial class DetalheOfertaContaCerta : UserControlBase
    {
        /// <summary>
        /// Valor complementar privado do CodSitContrato
        /// </summary>
        private Int16? codSitContrato = null;

        /// <summary>
        /// Código do contrato para filtro junto à camada de WCF
        /// </summary>
        public Int16? CodSitContrato
        {
            get
            {
                if (!codSitContrato.HasValue)
                {
                    if (String.IsNullOrWhiteSpace(Request.QueryString["q"]))
                        return null;

                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["q"]);
                    if (queryString == null)
                        return null;

                    Int16 codTemp = 0;
                    if (!Int16.TryParse(queryString["codSitContrato"], out codTemp))
                        return null;

                    codSitContrato = codTemp;
                }

                return codSitContrato;
            }
        }

        /// <summary>
        /// Evento de carregamento da página
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // dados do usuário no header do bloco de impressão
            if (this.SessaoAtual != null)
                this.ltrHeaderImpressaoUsuario.Text = string.Concat(SessaoAtual.CodigoEntidade, " / ", SessaoAtual.LoginUsuario);

            DefinirDescricaoOferta();
            VerificarHistorico();
        }

        /// <summary>
        /// Define se a descrição da oferta será a de oferta ativa ou cancelada de acordo com o status da oferta enviado por querystring
        /// </summary>
        private void DefinirDescricaoOferta()
        {
            if (!this.CodSitContrato.HasValue)
                return;

            if (this.CodSitContrato.Value > 0)
                ucOfertaAtiva.Visible = false;
            else
                ucOfertaCancelada.Visible = false;
        }

        /// <summary>
        /// Verifica se a oferta contém histórico. Se conter, exibe o painel de histórico
        /// </summary>
        private void VerificarHistorico()
        {
            using (Logger log = Logger.IniciarLog("Consulta o histórico da oferta"))
            {
                phHistoricoOferta.Visible = false;
                if (Sessao.Contem())
                {
                    if (!this.CodSitContrato.HasValue)
                    {
                        qdAviso.Visible = true;
                        phDetalheOferta.Visible = false;
                        return;
                    }

                    mnuAcoes.Visible = true;

                    try
                    {
                        using (var client = new ContextoWCF<ZPContaCertaServico.HISServicoZPContaCertaClient>())
                        {
                            Int16 codRetorno = default(Int16);
                            phHistoricoOferta.Visible = client.Cliente.ContemHistorico(
                                SessaoAtual.CodigoEntidade,
                                this.CodSitContrato.Value,
                                out codRetorno);
                        }
                    }
                    catch (FaultException<OfertaServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (FaultException<ZPContaCertaServico.GeneralFault> ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(ex.Detail.Fonte, ex.Detail.Codigo);
                    }
                    catch (Exception ex)
                    {
                        log.GravarErro(ex);
                        base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                    }
                }
            }
        }
    }
}