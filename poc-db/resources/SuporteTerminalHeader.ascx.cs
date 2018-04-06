using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Redecard.PN.Comum;
using Redecard.PN.DadosCadastrais.SharePoint.MaximoServico;

namespace Redecard.PN.DadosCadastrais.SharePoint.CONTROLTEMPLATES.DadosCadastrais
{
    public partial class SuporteTerminalHeader : UserControlBase
    {
        internal TerminalDetalhado Terminal 
        {
            get { return (TerminalDetalhado)ViewState["Terminal"]; }
            set { ViewState["Terminal"] = value; }
        }

        internal void CarregarDadosTerminal(TerminalDetalhado terminal)
        {
            this.Terminal = terminal;
            lblTipoEquipamento.Text = terminal.TipoEquipamento;
            lblNumeroTerminal.Text = terminal.NumeroLogico;
        }
    }
}
