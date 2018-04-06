using System;
using System.ServiceModel;
using System.Web.UI;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.Extrato.SharePoint.Helper;

namespace Redecard.PN.Extrato.SharePoint.WebParts.HomeFidelidade
{
    public partial class HomeFidelidadeUserControl : BaseUserControl
    {
        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    CarregarLinksAtalhos();
                    CarregarAvisoSenha();
                    ConfigurarVersaoExtratos();
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Home", ex);
                SharePointUlsLog.LogErro(ex);
                SharePointUlsLog.LogMensagem("@@@HOME: " + ex.StackTrace);
                base.ExibirPainelExcecao(ex.Fonte, ex.Codigo);

            }
            catch (Exception ex)
            {
                Logger.GravarErro("Home", ex);
                SharePointUlsLog.LogErro(ex);
                SharePointUlsLog.LogMensagem("@@@HOME: " + ex.StackTrace);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Carrega os links de atalho e Rav
        /// </summary>
        private void CarregarLinksAtalhos()
        {
            try
            {
                //RAV
                lnkContinuarRav.NavigateUrl = String.Format("{0}/{1}", base.web.Site.ServerRelativeUrl, "servicos/Paginas/pn_Rav.aspx");

                //Atalhos
                lnkAtalhoCancVendas.NavigateUrl = String.Format("{0}/{1}", base.web.Site.ServerRelativeUrl, "servicos/Paginas/pn_cancelamentovendas.aspx");
                lnkAtalhoChargeback.NavigateUrl = String.Format("{0}/{1}", base.web.Site.ServerRelativeUrl, "servicos/Paginas/pn_ComprovantesPendentes.aspx");
                lnkAtalhoRAV.NavigateUrl = String.Format("{0}/{1}", base.web.Site.ServerRelativeUrl, "servicos/Paginas/pn_Rav.aspx");
                lnkAtalhoRelVendas.NavigateUrl = String.Format("{0}/{1}", base.web.Site.ServerRelativeUrl, "extrato/Paginas/pn_Relatorios.aspx?tipo=0");
                lnkAtalhoValPagos.NavigateUrl = String.Format("{0}/{1}", base.web.Site.ServerRelativeUrl, "extrato/Paginas/pn_Relatorios.aspx?tipo=1");

                //ver outros períodos                
                lnkUltimasVendasCredito.NavigateUrl = String.Format("{0}/{1}", base.web.Site.ServerRelativeUrl, "extrato/Paginas/pn_Relatorios.aspx?tipo=0&tipoVenda=0");
                lnkUltimasVendasDebito.NavigateUrl = String.Format("{0}/{1}", base.web.Site.ServerRelativeUrl, "extrato/Paginas/pn_Relatorios.aspx?tipo=0&tipoVenda=1");
                lnkLancamentosFuturosCredito.NavigateUrl = String.Format("{0}/{1}", base.web.Site.ServerRelativeUrl, "extrato/Paginas/pn_Relatorios.aspx?tipo=4&tipoVenda=0");
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Home", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Home", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        /// <summary>
        /// Verifica quando a senha do usuário irá expirar. Se a quantidade de dias for menor ou igual a 15, exibe o aviso
        /// </summary>
        private void CarregarAvisoSenha()
        {
            using (Logger log = Logger.IniciarLog("Início Verificação de quando a senha do usuário irá expirar."))
            {
                try
                {
                    qdAvisoSenha.Visible = false;
                    if (SessaoAtual.PossuiKomerci && !SessaoAtual.AcessoFilial && !SessaoAtual.UsuarioAtendimento)
                    {
                        using (var usuarioServico = new UsuarioServico.UsuarioServicoClient())
                        {
                            Int32 codRetorno = 0;
                            UsuarioServico.Usuario usuario = usuarioServico.ConsultarDadosUsuario(out codRetorno,
                                                                                            SessaoAtual.GrupoEntidade,
                                                                                            SessaoAtual.CodigoEntidade,
                                                                                            SessaoAtual.LoginUsuario);

                            if (!object.ReferenceEquals(usuario, null))
                            {
                                if (!object.ReferenceEquals(usuario.DataExpiracaoSenha, null))
                                {
                                    TimeSpan qtdDias = (usuario.DataExpiracaoSenha - DateTime.Today);
                                    Double diasExpiracao = Math.Truncate(qtdDias.TotalDays);

                                    if (diasExpiracao <= 15)
                                    {
                                        String link = String.Format("{0}/{1}", base.web.Site.ServerRelativeUrl, "minhaconta/Paginas/pn_SenhaAcessoPortal.aspx");
                                        log.GravarMensagem(link);
                                        String mensagem = "";
                                        if (diasExpiracao > 0)
                                        {
                                            mensagem = String.Format("Sua senha vai expirar dentro de <b>{0} dia{1}</b>. Para seu maior conforto, <a href='{2}'>antecipe a troca</a>.",
                                                                                diasExpiracao,
                                                                                diasExpiracao > 1 ? "s" : "",
                                                                                link);
                                        }
                                        else
                                        {
                                            mensagem = String.Format("Sua senha expirou. Por favor, <a href='{0}'>efetue a troca</a>.",
                                                                                link);
                                        }

                                        log.GravarMensagem(mensagem);
                                        QuadroAvisoHome qdAviso = (QuadroAvisoHome)qdAvisoSenha;
                                        qdAviso.CarregarMensagem("Atenção!", mensagem, QuadroAvisoHome.Icone.Aviso);
                                        qdAviso.Visible = true;
                                        imgLentidaoHome.Visible = false;
                                        imgLentidaoHomeOK.Visible = false;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (FaultException ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    SharePointUlsLog.LogMensagem("@@@HOME: " + ex.StackTrace);
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(ex.Message, ex.Code.ToString().ToInt32());
                }
                catch (Exception ex)
                {
                    SharePointUlsLog.LogErro(ex);
                    SharePointUlsLog.LogMensagem("@@@HOME: " + ex.StackTrace);
                    log.GravarErro(ex);
                    base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
                }
            }
        }

        #endregion Eventos

        /// <summary>
        /// Não verificar permissões na página inicial do Portal Redecard Estabelecimento
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.ValidarPermissao = false;
            base.OnLoad(e);
        }

        /// <summary>
        /// Configura a versão do Extrato que será utilizada:<br/>
        /// QueryString: .aspx?grandesconsultas=[v]
        /// </summary>
        private void ConfigurarVersaoExtratos()
        {
            ConfiguracaoVersao.VersaoGrandesConsultas(Request);
        }
    }
}