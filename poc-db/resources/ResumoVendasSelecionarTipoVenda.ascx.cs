using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.Extrato.ResumoVendas
{
    public partial class ResumoVendasSelecionarTipoVenda : BaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Exibe opção "Recarga de Celular" apenas para PVs Físicos
            //que possuem o serviço recarga (cód. serviço 500, domínio Ativo ou Reativo)
            if (this.SessaoAtual != null)
                trRecargaCelular.Visible = SessaoAtual.PVFisico && SessaoAtual.PossuiRecarga;
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            if (rdDetalheCredito.Checked)
                onItemSelecionado("C", e);
            else if (rdDetalheDebito.Checked)
                onItemSelecionado("D", e);
            else if (rdDetalheCDC.Checked)
                onItemSelecionado("CDC", e);
            else if (rdDetalheRecargaCelular.Checked)
                onItemSelecionado("RC", e);
        }

        public delegate void ItemSelecionado(string tipoRelatorio, EventArgs e);

        [Browsable(true)]
        public event ItemSelecionado onItemSelecionado;

        protected void btnLimpar_Click(object sender, EventArgs e)
        {
            //TODO:RESOURCE
        }
    }
}
