using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using Redecard.PN.DataCash.Modelo.Util;
using Redecard.PN.DataCash.BasePage;
using System.Globalization;
using Redecard.PN.DataCash.controles;
using Redecard.PN.Comum;

namespace Redecard.PN.DataCash
{
    public partial class ConsultaXML : PageBaseDataCash
    {
        /// <summary>
        /// Carregamento da página
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        protected void btnConsultar_Click(object sender, EventArgs e)
        {
            try
            {
                Negocio.Vendas transacao = new Negocio.Vendas();

                String xml = Server.HtmlEncode(txtXML.Text);

                String retornoXML = transacao.ExecutaTransacaoXML(Server.HtmlDecode(xml));

                txtXMLRetorno.Text = retornoXML;
            }
            catch (Exception ex)
            {
                txtXMLRetorno.Text = ex.Message;
            }
        }

    }
}
