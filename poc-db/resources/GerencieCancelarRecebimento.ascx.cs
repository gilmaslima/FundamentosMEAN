using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.ComponentModel;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates
{
    public partial class GerencieCancelarRecebimento : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public delegate void ItemSelecionado(string tela, EventArgs e);

        [Browsable(true)]
        public event ItemSelecionado onItemSelecionado;
    }
}
