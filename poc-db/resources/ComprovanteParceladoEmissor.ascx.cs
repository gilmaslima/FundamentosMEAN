using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.DataCash.controles.comprovantes
{
    public partial class ComprovanteParceladoEmissor : System.Web.UI.UserControl
    {
        public Decimal ValorParcela { set { ltValorParcela.Text = value.ToString("N2"); } }
        public Decimal Encargos { set { ltEncargos.Text = value.ToString("N2"); } }
        public Decimal ValorTotalPagar { set { ltValorTotalPagar.Text = value.ToString("N2"); } }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}