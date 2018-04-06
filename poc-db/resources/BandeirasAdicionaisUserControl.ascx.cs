using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Microsoft.SharePoint;
using System.ServiceModel;
using Redecard.PN.DataCash.SharePoint.DataCashService;

namespace Redecard.PN.DataCash.SharePoint.WebParts.BandeirasAdicionais
{
    public partial class BandeirasAdicionaisUserControl : UserControlBaseDataCash
    {
        #region [ Eventos ]

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Page Load - Bandeiras Adicionais"))
            {
                try
                {
                    if (!Page.IsPostBack)
                    {
                        CarregarDadosBandeirasAdicionais();
                        mvwBandeirasAdicionais.SetActiveView(vwDadosBandeira);
                    }
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Bandeiras Adicionais - Page Load", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Bandeiras Adicionais - Page Load", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Ação do botão Continuar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Continuar - Bandeiras Adicionais"))
            {
                try
                {
                    Page.Validate();
                    if (Page.IsValid)
                    {
                        var mensagemErro = SalvarBandeirasAdicionais();
                        
                        if (mensagemErro.CodigoRetorno == 0)
                            mvwBandeirasAdicionais.SetActiveView(vwEfetivacao);
                        else
                            base.ExibirPainelExcecao(mensagemErro.MensagemRetorno, mensagemErro.CodigoRetorno.ToString());
                        
                    }
                }
                catch (FaultException<GeneralFault> fe)
                {
                    Logger.GravarErro("Bandeiras Adicionais - Continuar", fe);
                    SharePointUlsLog.LogErro(fe);
                    base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Bandeiras Adicionais - Continuar", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento do botão voltar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            using (Logger Log = Logger.IniciarLog("Voltar - Bandeiras Adicionais"))
            {
                try
                {
                    mvwBandeirasAdicionais.SetActiveView(vwDadosBandeira);
                }
                catch (Exception ex)
                {
                    Logger.GravarErro("Bandeiras Adicionais - Voltar", ex);
                    SharePointUlsLog.LogErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        /// <summary>
        /// Evento de mudança da view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void mvwBandeirasAdicionais_ActiveViewChanged(object sender, EventArgs e)
        {
            var activeView = mvwBandeirasAdicionais.GetActiveView();

            if (activeView == vwEfetivacao)
            {
                (qdAviso as QuadroAviso).Mensagem = BuscarMensagemEfetivacao();
                (qdAviso as QuadroAviso).CarregarMensagem();
            }

            AtualizaCabecalhoPassoAtual();
        }

        #endregion
          
        #region [ Métodos ]

        /// <summary>
        /// Atualiza o passo atual
        /// </summary>
        private void AtualizaCabecalhoPassoAtual()
        {
            var activeView = mvwBandeirasAdicionais.GetActiveView();

            if (activeView == vwDadosBandeira)
                assistente.AtivarPasso(0);
            else if (activeView == vwEfetivacao)
                assistente.AtivarPasso(1);
        }

        /// <summary>
        /// Busca mensagem de efetivação na lista de configurações
        /// </summary>
        /// <returns></returns>
        private String BuscarMensagemEfetivacao()
        {
            SPList listaConfiguracoes = SPContext.Current.Web.Lists.TryGetList("ListaConfiguracoes");
            SPQuery query = new SPQuery();
            query.Query = @"<Where><Eq><FieldRef Name='Chave'/><Value Type='Text'>MensagemEfetivacao</Value></Eq></Where>";

            SPListItemCollection items = listaConfiguracoes.GetItems(query);
            SPListItem item = null;

            if (items != null && items.Count > 0)
                item = items[0];

            return item["Valor"].ToString();
        }

        /// <summary>
        /// Chama serviço de consulta bandeiras adicionais
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        private RetornoBandeirasAdicionais ConsultaBandeirasAdicionais(Int32 pv, out MensagemErro mensagemErro)
        {
            var retorno = new RetornoBandeirasAdicionais();

            using (var log = Logger.IniciarLog("Consulta Bandeiras Adicionais"))
            {
                log.GravarLog(EventoLog.ChamadaServico, pv);

                using(var contexto = new ContextoWCF<DataCashServiceClient>())
                {
                    retorno = contexto.Cliente.ConsultaBandeirasAdicionais(out mensagemErro, pv);
                }

                log.GravarLog(EventoLog.RetornoServico, new {
                    retorno,
                    mensagemErro
                });
            }

            return retorno;
        }

        /// <summary>
        /// Chamada do serviço de gravar atualizar bandeiras adicionais
        /// </summary>
        /// <param name="pv"></param>
        /// <param name="mensagemErro"></param>
        /// <param name="numeroAfiliacaoPdv"></param>
        /// <param name="chaveConfiguracaoPdv"></param>
        /// <returns></returns>
        private Boolean GravarAtualizarBandeirasAdicionais(Int32 pv, out MensagemErro mensagemErro, String numeroAfiliacaoPdv, String chaveConfiguracaoPdv )
        {
            Boolean retorno = false;

            using (var log = Logger.IniciarLog("Gravar Atualizar Bandeiras Adicionais"))
            {
                log.GravarLog(EventoLog.ChamadaServico, new {
                    pv, 
                    numeroAfiliacaoPdv, 
                    chaveConfiguracaoPdv
                });

                using (var contexto = new ContextoWCF<DataCashServiceClient>())
                {
                    retorno = contexto.Cliente.GravarAtualizarBandeirasAdicionais(out mensagemErro, pv, numeroAfiliacaoPdv, chaveConfiguracaoPdv);
                }

                log.GravarLog(EventoLog.RetornoServico, new {
                    retorno,
                    mensagemErro
                });
            }

            return retorno;
        }

        /// <summary>
        /// Carrega os dados da bandeira adicional caso haja
        /// </summary>
        private void CarregarDadosBandeirasAdicionais()
        {
            MensagemErro mensagemErro = new MensagemErro();
            
            var dadosBandeirasAdicionais = ConsultaBandeirasAdicionais(SessaoAtual.CodigoEntidade, out mensagemErro);

            if (mensagemErro.CodigoRetorno == 0)
            {
                if (dadosBandeirasAdicionais.BandeirasAdicionais != null && dadosBandeirasAdicionais.BandeirasAdicionais.Count > 0)
                {
                    txtChaveConfiguracao.Text = dadosBandeirasAdicionais.BandeirasAdicionais[0].ChaveConfiguracao;
                    txtNumeroAfiliacao.Text = dadosBandeirasAdicionais.BandeirasAdicionais[0].NumeroAfiliacao;
                }
            }
            else
            {
                base.ExibirPainelExcecao(mensagemErro.MensagemRetorno, mensagemErro.CodigoRetorno.ToString());
            }
        }

        /// <summary>
        /// Salva dados das bandeiras adicionais
        /// </summary>
        private MensagemErro SalvarBandeirasAdicionais()
        {
            Int32 pv = SessaoAtual.CodigoEntidade;
            MensagemErro mensagemErro = new MensagemErro();
            String numeroAfiliacaoPdv = txtNumeroAfiliacao.Text;
            String chaveConfiguracaoPdv = txtChaveConfiguracao.Text;

            GravarAtualizarBandeirasAdicionais(pv, out mensagemErro, numeroAfiliacaoPdv, chaveConfiguracaoPdv);

            return mensagemErro;
        }

        #endregion
    }
}
