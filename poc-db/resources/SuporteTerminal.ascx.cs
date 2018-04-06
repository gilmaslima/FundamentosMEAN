using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.Comum.SharePoint.CONTROLTEMPLATES.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.MaximoServico;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    public partial class SuporteTerminal : UserControlBase
    {
        #region [ Controles ]
        
        /// <summary>
        /// rcMotivo control.
        /// </summary>        
        protected SuporteTerminalMotivo rcMotivo;

        /// <summary>
        /// rcTrocaTerminal control.
        /// </summary>
        protected SuporteTerminalTroca rcTrocaTerminal;

        /// <summary>
        /// rcAtendimentoEmail control.
        /// </summary>        
        protected SuporteTerminalEmail rcAtendimentoEmail;

        #endregion

        #region [ Atributos/Variáveis ]

        /// <summary>
        /// Terminal selecionado
        /// </summary>
        private TerminalDetalhado Terminal
        {
            get { return (TerminalDetalhado)ViewState["Terminal"]; }
            set { ViewState["Terminal"] = value; }
        }

        #endregion

        /// <summary>
        /// Carrega os dados do terminal para suporte.
        /// Passo inicial.
        /// </summary>
        /// <param name="terminal">Terminal</param>
        internal void CarregarTerminal(TerminalDetalhado terminal)
        {
            this.Terminal = terminal;                        
            rcMotivo.CarregarDadosMotivos(this.Terminal);
        }

        /// <summary>
        /// Carrega os controles para Atendimento por E-mail
        /// </summary>
        public void CarregarAtendimentoEmail(String descricaoMotivo)
        {
            mvwSuporteTerminal.SetActiveView(vwAtendimentoEmail);
            rcAtendimentoEmail.Carregar(this.Terminal, descricaoMotivo);
        }

        /// <summary>
        /// Carrega os controles para Solicitação de Troca de Terminal
        /// </summary>
        public void CarregarTrocaTerminal(Int32 idMotivo, String descricaoMotivo)
        {
            mvwSuporteTerminal.SetActiveView(vwTrocaTerminal);
            rcTrocaTerminal.Carregar(this.Terminal, idMotivo, descricaoMotivo);
        }
    }
}
