using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using System.Collections.Generic;
using System.ServiceModel;

namespace Redecard.PN.DataCash.SharePoint.WebParts.ConfiguracaoAVS
{
    public partial class ConfiguracaoAVSUserControl : UserControlBaseDataCash
    {
        #region [ Propriedades e Constantes ]

        private static String[] OpcoesAVS = new String[] { "CPF", "Endereço", "CEP" };

        #endregion

        #region [ Página ]

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    mvwConfiguracaoAVS.SetActiveView(vwDadosAVS);
                }
            }
            catch (FaultException<DataCashService.GeneralFault> fe)
            {
                Logger.GravarErro("Configuração AVS", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Configuração AVS", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ Botões ]

        protected void btnContinuar_Click(object sender, EventArgs e)
        {
            try
            {
                mvwConfiguracaoAVS.SetActiveView(vwConfirmacao);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Configuração AVS", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnContinuar2_Click(object sender, EventArgs e)
        {
            try
            {
                SalvarRegraAVS();
                mvwConfiguracaoAVS.SetActiveView(vwEfetivacao);
            }
            catch (FaultException<DataCashService.GeneralFault> fe)
            {
                Logger.GravarErro("Configuração AVS", fe);
                SharePointUlsLog.LogErro(fe);
                base.ExibirPainelExcecao(fe.Detail.Fonte, fe.Detail.Codigo);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Configuração AVS", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            try
            {
                mvwConfiguracaoAVS.SetActiveView(vwDadosAVS);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Configuração AVS", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        protected void btnVoltar2_Click(object sender, EventArgs e)
        {
            try
            {
                mvwConfiguracaoAVS.SetActiveView(vwDadosAVS);
            }
            catch (Exception ex)
            {
                Logger.GravarErro("Configuração AVS", ex);
                SharePointUlsLog.LogErro(ex);
                base.ExibirPainelExcecao(FONTE, CODIGO_ERRO);
            }
        }

        #endregion

        #region [ MultiView ]

        protected void mvwConfiguracaoAVS_ActiveViewChanged(object sender, EventArgs e)
        {
            var activeView = mvwConfiguracaoAVS.GetActiveView();

            if (activeView == vwDadosAVS && String.IsNullOrEmpty(rbNivel.SelectedValue))
            {
                String regra = ConsultaRegraAVS();
                SetRegraAVS(regra);
            }

            if (activeView == vwConfirmacao)
            {
                SetRegraAVSConfirmacao();
            }

            if (activeView == vwEfetivacao)
            {
                SetRegraAVSEfetivacao();
            }

            AtualizaCabecalhoPassoAtual();
        }

        private void AtualizaCabecalhoPassoAtual()
        {
            var activeView = mvwConfiguracaoAVS.GetActiveView();
            
            if (activeView == vwDadosAVS)
                assistente.AtivarPasso(0);
            else if (activeView == vwConfirmacao)
                assistente.AtivarPasso(1);
            else if (activeView == vwEfetivacao)
                assistente.AtivarPasso(2);
        }

        private void SetRegraAVS(String regra)
        {
            if (!String.IsNullOrEmpty(regra))
                rbNivel.SelectedValue = regra;
        }

        private void SetRegraAVSConfirmacao()
        {
            String regra = rbNivel.SelectedValue;

            imgNivel1.Visible = false;
            imgNivel2.Visible = false;
            imgNivel3.Visible = false;
            imgNivel4.Visible = false;
            imgNivel5.Visible = false;
            imgNivel6.Visible = false;
            imgNivel7.Visible = false;
            imgRejeita1.Visible = true;
            imgRejeita2.Visible = true;
            imgRejeita3.Visible = true;
            imgRejeita4.Visible = true;
            imgRejeita5.Visible = true;
            imgAceita1.Visible = true;
            imgAceita2.Visible = true;
            imgAceita3.Visible = true;
            imgAceita4.Visible = true;
            imgAceita5.Visible = true;

            switch (regra)
            {
                case "0": imgNivel1.Visible = true;
                    imgRejeita1.Visible = false;
                    imgRejeita2.Visible = false;
                    imgRejeita3.Visible = false;
                    imgRejeita4.Visible = false;
                    imgRejeita5.Visible = false;
                    break;
                case "1": imgNivel4.Visible = true;
                    imgRejeita1.Visible = false;
                    imgAceita2.Visible = false;
                    imgRejeita3.Visible = false;
                    imgAceita4.Visible = false;
                    imgAceita5.Visible = false;
                    break;
                case "2": imgNivel5.Visible = true;
                    imgRejeita1.Visible = false;
                    imgRejeita2.Visible = false;
                    imgAceita3.Visible = false;
                    imgAceita4.Visible = false;
                    imgAceita5.Visible = false;
                    break;
                case "3": imgNivel7.Visible = true;
                    imgRejeita1.Visible = false;
                    imgAceita2.Visible = false;
                    imgAceita3.Visible = false;
                    imgAceita4.Visible = false;
                    imgAceita5.Visible = false;
                    break;
                case "5": imgNivel2.Visible = true;
                    imgRejeita1.Visible = false;
                    imgAceita2.Visible = false;
                    imgRejeita3.Visible = false;
                    imgRejeita4.Visible = false;
                    imgAceita5.Visible = false;
                    break;
                case "6": imgNivel3.Visible = true;
                    imgRejeita1.Visible = false;
                    imgRejeita2.Visible = false;
                    imgAceita3.Visible = false;
                    imgRejeita4.Visible = false;
                    imgAceita5.Visible = false;
                    break;
                case "7": imgNivel6.Visible = true;
                    imgRejeita1.Visible = false;
                    imgAceita2.Visible = false;
                    imgAceita3.Visible = false;
                    imgRejeita4.Visible = false;
                    imgAceita5.Visible = false;
                    break;
                default: break;
            }
        }

        private void SetRegraAVSEfetivacao()
        {
            String regra = rbNivel.SelectedValue;

            imgNivelEfetivacao1.Visible = false;
            imgNivelEfetivacao2.Visible = false;
            imgNivelEfetivacao3.Visible = false;
            imgNivelEfetivacao4.Visible = false;
            imgNivelEfetivacao5.Visible = false;
            imgNivelEfetivacao6.Visible = false;
            imgNivelEfetivacao7.Visible = false;
            imgRejeitaEfetivacao1.Visible = true;
            imgRejeitaEfetivacao2.Visible = true;
            imgRejeitaEfetivacao3.Visible = true;
            imgRejeitaEfetivacao4.Visible = true;
            imgRejeitaEfetivacao5.Visible = true;
            imgAceitaEfetivacao1.Visible = true;
            imgAceitaEfetivacao2.Visible = true;
            imgAceitaEfetivacao3.Visible = true;
            imgAceitaEfetivacao4.Visible = true;
            imgAceitaEfetivacao5.Visible = true;

            switch (regra)
            {
                case "0": imgNivelEfetivacao1.Visible = true;
                    imgRejeitaEfetivacao1.Visible = false;
                    imgRejeitaEfetivacao2.Visible = false;
                    imgRejeitaEfetivacao3.Visible = false;
                    imgRejeitaEfetivacao4.Visible = false;
                    imgRejeitaEfetivacao5.Visible = false;
                    break;
                case "1": imgNivelEfetivacao4.Visible = true;
                    imgRejeitaEfetivacao1.Visible = false;
                    imgAceitaEfetivacao2.Visible = false;
                    imgRejeitaEfetivacao3.Visible = false;
                    imgAceitaEfetivacao4.Visible = false;
                    imgAceitaEfetivacao5.Visible = false;
                    break;
                case "2": imgNivelEfetivacao5.Visible = true;
                    imgRejeitaEfetivacao1.Visible = false;
                    imgRejeitaEfetivacao2.Visible = false;
                    imgAceitaEfetivacao3.Visible = false;
                    imgAceitaEfetivacao4.Visible = false;
                    imgAceitaEfetivacao5.Visible = false;
                    break;
                case "3": imgNivelEfetivacao7.Visible = true;
                    imgRejeitaEfetivacao1.Visible = false;
                    imgAceitaEfetivacao2.Visible = false;
                    imgAceitaEfetivacao3.Visible = false;
                    imgAceitaEfetivacao4.Visible = false;
                    imgAceitaEfetivacao5.Visible = false;
                    break;
                case "5": imgNivelEfetivacao2.Visible = true;
                    imgRejeitaEfetivacao1.Visible = false;
                    imgAceitaEfetivacao2.Visible = false;
                    imgRejeitaEfetivacao3.Visible = false;
                    imgRejeitaEfetivacao4.Visible = false;
                    imgAceitaEfetivacao5.Visible = false;
                    break;
                case "6": imgNivelEfetivacao3.Visible = true;
                    imgRejeitaEfetivacao1.Visible = false;
                    imgRejeitaEfetivacao2.Visible = false;
                    imgAceitaEfetivacao3.Visible = false;
                    imgRejeitaEfetivacao4.Visible = false;
                    imgAceitaEfetivacao5.Visible = false;
                    break;
                case "7": imgNivelEfetivacao6.Visible = true;
                    imgRejeitaEfetivacao1.Visible = false;
                    imgAceitaEfetivacao2.Visible = false;
                    imgAceitaEfetivacao3.Visible = false;
                    imgRejeitaEfetivacao4.Visible = false;
                    imgAceitaEfetivacao5.Visible = false;
                    break;
                default: break;
            }
        }

        #endregion

        #region [ Operações ]

        private String ConsultaRegraAVS()
        {
            String retorno = String.Empty;

            using (var log = Logger.IniciarLog("Consulta Regra AVS"))
            {
                Int32 pv = base.SessaoAtual.CodigoEntidade;
                DataCashService.MensagemErro mensagemErro;

                log.GravarLog(EventoLog.ChamadaServico, new { pv });

                using (var contexto = new ContextoWCF<DataCashService.DataCashServiceClient>())
                {
                    retorno = contexto.Cliente.ConsultarRegraAVS(out mensagemErro, pv);
                }

                log.GravarLog(EventoLog.RetornoServico, new { retorno, mensagemErro });
            }

            return retorno;
        }

        private Boolean SalvarRegraAVS()
        {
            Boolean retorno = false;

            using (var log = Logger.IniciarLog("Salva Regra AVS"))
            {
                Int32 pv = base.SessaoAtual.CodigoEntidade;
                DataCashService.MensagemErro mensagemErro;
                Char avs = Char.Parse(rbNivel.SelectedValue);

                log.GravarLog(EventoLog.ChamadaServico, new { pv, avs });

                using (var contexto = new ContextoWCF<DataCashService.DataCashServiceClient>())
                {
                    retorno = contexto.Cliente.GerenciarRegraAVS(out mensagemErro, pv, avs);

                    if (mensagemErro.CodigoRetorno != 0)
                        base.ExibirPainelExcecao(mensagemErro.MensagemRetorno, mensagemErro.CodigoRetorno.ToString());
                }

                log.GravarLog(EventoLog.RetornoServico, new { retorno, mensagemErro });
            }

            return retorno;
        }

        #endregion
    }
}
