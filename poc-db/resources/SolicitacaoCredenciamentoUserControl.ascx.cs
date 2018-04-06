using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;

namespace Rede.PN.Credenciamento.Sharepoint.WebParts.SolicitacaoCredenciamento
{
    /// <summary>
    /// Web Part de Solicitacao de Credenciamento
    /// </summary>
    public partial class SolicitacaoCredenciamentoUserControl : UserControlCredenciamentoBase
    {
        #region [ Comum ]

        /// <summary>
        /// Evento Page Load
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Page.Title = "Solicitação de Credenciamento";

                if (!Page.IsPostBack)
                {
                    mvCredenciamento.SetActiveView(vwDadosIniciais);
                }
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Evento chamado ao mudar a view ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void mvCredenciamento_ActiveViewChanged(object sender, EventArgs e)
        {
            try
            {
                var activeView = mvCredenciamento.GetActiveView();

                AtualizaCabecalhoPassoAtual(activeView);
            }
            catch (PortalRedecardException ex)
            {
                Logger.GravarErro("Credenciamento", ex);
                SharePointUlsLog.LogErro(ex);
                ExibirPainelExcecao(ex.Fonte, ex.Codigo);
            }
            catch (Exception ex)
            {
                ex.HandleException(this);
            }
        }

        /// <summary>
        /// Atualiza o cabeçalho para o passo escolhido
        /// </summary>
        /// <param name="activeView">View Ativa</param>
        private void AtualizaCabecalhoPassoAtual(View activeView)
        {
            if (activeView == vwDadosIniciais)
                assistentePassos.AtivarPasso(0);

            if (activeView == vwCondicaoComercial)
                assistentePassos.AtivarPasso(1);

            if (activeView == vwDadosBancarios)
                assistentePassos.AtivarPasso(2);

            if (activeView == vwDadosContato)
                assistentePassos.AtivarPasso(3);

            if (activeView == vwDadosOperacionais)
                assistentePassos.AtivarPasso(4);

            if (activeView == vwConfirmacao)
                assistentePassos.AtivarPasso(5);

            MostraEscondePassos(activeView);
        }

        /// <summary>
        /// Mostra ou esconde o cabeçalho de passos de acordo com a view;
        /// </summary>
        /// <param name="activeView">View Ativa</param>
        private void MostraEscondePassos(View activeView)
        {
            bool viewTemCabecalho = activeView == vwDadosIniciais ||
                    activeView == vwCondicaoComercial ||
                    activeView == vwDadosBancarios ||
                    activeView == vwDadosContato ||
                    activeView == vwDadosOperacionais ||
                    activeView == vwConfirmacao;

            if (viewTemCabecalho)
                assistentePassos.Visible = true;
            else
                assistentePassos.Visible = false;
        }

        #endregion

        #region [ Dados Inciais ]

        /// <summary>
        /// Seta a view vwCondicaoComercial como view Ativa 
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlDadosIniciais_Continuar(object sender, EventArgs e)
        {
            mvCredenciamento.SetActiveView(vwCondicaoComercial);
            assistentePassos.PassoMaximo = 1;
            ((Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento.CondicaoComercial)ctlCondicaoComercial).CarregarCamposIniciais();
        }

        /// <summary>
        /// Seta a view vwRecuperacaoProposta como view Ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlDadosIniciais_ContinuarRecuperarProposta(object sender, EventArgs e)
        {
            mvCredenciamento.SetActiveView(vwRecuperacaoProposta);
            ((Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento.RecuperacaoProposta)ctlRecuperacaoProposta).CarregarDadosIniciais();
        }

        #endregion

        #region [ Condição Comercial ]

        /// <summary>
        /// Seta a view vwDadosBancarios como view Ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlCondicaoComercial_Continuar(object sender, EventArgs e)
        {
            mvCredenciamento.SetActiveView(vwDadosBancarios);
            assistentePassos.PassoMaximo = 2;
            ((Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento.DadosBancarios)ctlDadosBancarios).CarregarCamposIniciais();
        }

        /// <summary>
        /// Seta a view vwDadosIniciais como view Ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlCondicaoComercial_Voltar(object sender, EventArgs e)
        {
            mvCredenciamento.SetActiveView(vwDadosIniciais);
        }

        #endregion

        #region [ Dados Bancários ]

        /// <summary>
        /// Seta a view vwDadosContato como view Ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlDadosBancarios_Continuar(object sender, EventArgs e)
        {
            mvCredenciamento.SetActiveView(vwDadosContato);
            assistentePassos.PassoMaximo = 3;
            ((Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento.DadosContato)ctlDadosContato).CarregarCamposIniciais();
        }

        /// <summary>
        /// Seta a view vwCondicaoComercial como view Ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlDadosBancarios_Voltar(object sender, EventArgs e)
        {
            mvCredenciamento.SetActiveView(vwCondicaoComercial);
        }

        #endregion

        #region [ Dados Contato ]

        /// <summary>
        /// Seta a view vwDadosOperacionais como view Ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlDadosContato_Continuar(object sender, EventArgs e)
        {

            mvCredenciamento.SetActiveView(vwDadosOperacionais);
            assistentePassos.PassoMaximo = 4;
            ((Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento.DadosOperacionais)ctlDadosOperacionais).CarregarCamposIniciais();
        }

        /// <summary>
        /// Seta a view vwDadosBancarios como view Ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlDadosContato_Voltar(object sender, EventArgs e)
        {
            mvCredenciamento.SetActiveView(vwDadosBancarios);
        }

        #endregion

        #region [ Dados Operacionais ]

        /// <summary>
        /// Seta a view vwConfirmacao como view Ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlDadosOperacionais_Continuar(object sender, EventArgs e)
        {
            mvCredenciamento.SetActiveView(vwConfirmacao);
            assistentePassos.PassoMaximo = 5;
            ((Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento.Confirmacao)ctlConfirmacao).CarregarCamposIniciais();
        }

        /// <summary>
        /// Seta a view vwDadosContato como view Ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlDadosOperacionais_Voltar(object sender, EventArgs e)
        {
            mvCredenciamento.SetActiveView(vwDadosContato);
        }

        #endregion

        #region [ Confirmação ]

        /// <summary>
        /// Seta a view vwConclusao como view Ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlConfirmacao_Confirmar(object sender, EventArgs e)
        {
            mvCredenciamento.SetActiveView(vwConclusao);
            assistentePassos.PassoMaximo = 6;
            ((Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento.Conclusao)ctlConclusao).CarregarControles();
        }

        /// <summary>
        /// Seta a view vwDadosOperacionais como view Ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlConfirmacao_Voltar(object sender, EventArgs e)
        {
            mvCredenciamento.SetActiveView(vwDadosOperacionais);
        }

        #endregion

        #region [ Conclusão ]

        /// <summary>
        /// Redireciona tela para o preenchimento de uma nova proposta
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlConclusao_NovaProposta(object sender, EventArgs e)
        {
            Response.Redirect("SolicitacaoCredenciamento.aspx");
        }

        #endregion

        #region [ Recuperação Proposta ]

        /// <summary>
        /// Seta a view vwDadosIniciais como view Ativa
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void ctlRecuperarProposta_Continuar(object sender, EventArgs e)
        {
            mvCredenciamento.SetActiveView(vwDadosIniciais);

            ((Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento.DadosIniciais)ctlDadosIniciais).CarregarDadosIniciais();
        }

        #endregion

        #region [ Links dos Menus ]

        /// <summary>
        /// Menu rede:AssistentePassos pula para View vwDadosIniciais
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnMenu_DadosIniciais(object sender, EventArgs e)
        {
            UserControl objControl = ((Rede.PN.Credenciamento.Sharepoint.CONTROLTEMPLATES.Rede.Credenciamento.DadosIniciais)ctlDadosIniciais);

            //assistentePassos.PassoMaximo = 0;
            mvCredenciamento.SetActiveView(vwDadosIniciais);
        }

        /// <summary>
        /// Menu rede:AssistentePassos pula para View vwCondicaoComercial
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnMenu_CondicaoComercial(object sender, EventArgs e)
        {
            //assistentePassos.PassoMaximo = 1;
            mvCredenciamento.SetActiveView(vwCondicaoComercial);
        }

        /// <summary>
        /// Menu rede:AssistentePassos pula para View vwDadosBancarios
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnMenu_DadosBancarios(object sender, EventArgs e)
        {
            //assistentePassos.PassoMaximo = 2;
            mvCredenciamento.SetActiveView(vwDadosBancarios);
        }

        /// <summary>
        /// Menu rede:AssistentePassos pula para View vwDadosContato
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnMenu_DadosContato(object sender, EventArgs e)
        {
            //assistentePassos.PassoMaximo = 3;
            mvCredenciamento.SetActiveView(vwDadosContato);
        }

        /// <summary>
        /// Menu rede:AssistentePassos pula para View vwDadosOperacionais
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnMenu_DadosOperacionais(object sender, EventArgs e)
        {
            //assistentePassos.PassoMaximo = 4;
            mvCredenciamento.SetActiveView(vwDadosOperacionais);
        }

        /// <summary>
        /// Menu rede:AssistentePassos pula para View vwConfirmacao
        /// </summary>
        /// <param name="sender">Objeto que disparou o evento</param>
        /// <param name="e">Objeto que contém dados do evento</param>
        protected void btnMenu_ConfirmacaoDados(object sender, EventArgs e)
        {
            //assistentePassos.PassoMaximo = 5;
            mvCredenciamento.SetActiveView(vwConfirmacao);
        }
        #endregion
    }
}
