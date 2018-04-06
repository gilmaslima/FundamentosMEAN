using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace Redecard.PN.DataCash.controles
{
    public partial class ControleDadosPagamento : System.Web.UI.UserControl
    {
        public Modelo.DadosPagamento ObterDadosPagamento()
        {
            Modelo.DadosPagamento DadosPagamento = new Modelo.DadosPagamento()
            {
                Valor = Decimal.Parse(txtValor.Text),
                DataVencimento = txtDataVencimento.Text,
                MultaAtraso = Double.Parse(txtMultaAtraso.Text),
                JurosDia = Double.Parse(txtJurosDia.Text.Trim() == "" ? "0" : txtJurosDia.Text),
                NumeroPedido = txtNPedidoBoleto.Text,
                Nota = txtNota.Text,
            };

            return DadosPagamento;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetarValidationGroup();
            }
        }

        private void SetarValidationGroup()
        {
            String validationGroup = string.Empty;
            String classeCss = string.Empty;
            validationGroup = "vlgBoleto";
            classeCss = " vldBoleto";

            rfvValor.ValidationGroup = validationGroup;
            rfvDataVencimento.ValidationGroup = validationGroup;
            rfvMultaAtraso.ValidationGroup = validationGroup;
            //rfvJurosDia.ValidationGroup = validationGroup;
            cvDataVencimento.ValidationGroup = validationGroup;

            rfvValor.CssClass += classeCss;
            rfvDataVencimento.CssClass += classeCss;
            rfvMultaAtraso.CssClass += classeCss;
            //rfvJurosDia.CssClass += classeCss;
            cvDataVencimento.CssClass += classeCss;
        }

        public void CarregarPagamento(Modelo.DadosPagamento dadosPagamento)
        {
            if (dadosPagamento != null)
            {
                txtValor.Text = dadosPagamento.Valor.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR"));
                txtDataVencimento.Text = dadosPagamento.DataVencimento;
                txtMultaAtraso.Text = dadosPagamento.MultaAtraso.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR"));
                txtJurosDia.Text = dadosPagamento.JurosDia.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR"));
                txtNPedidoBoleto.Text = dadosPagamento.NumeroPedido;
                txtNota.Text = dadosPagamento.Nota;
            }
        }
    }
}