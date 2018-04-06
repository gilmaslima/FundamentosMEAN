using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.DataCash.controles
{
    public partial class ControleDadosCliente : System.Web.UI.UserControl
    {
        public Modelo.DadosCliente ObterDadosCliente()
        {
            Modelo.DadosCliente dadosCliente = new Modelo.DadosCliente()
            {
                Titulo = ddlTitulo.SelectedValue,
                Nome = txtNomeCliente.Text,
                Sobrenome = txtSobreNomeCliente.Text,
                Email = txtEmailCliente.Text,
                DDD = txtDDDFoneCliente.Text,
                Telefone = txtFoneCliente.Text,
            };

            return dadosCliente;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetarValidationGroup();

                ddlTitulo.SelectedIndex = -1;
            }
        }

        private void SetarValidationGroup()
        {
            String validationGroup = string.Empty;
            String classeCss = string.Empty;
            validationGroup = "vlgBoleto";
            classeCss = " vldBoleto";

            rfvTitulo.ValidationGroup = validationGroup;
            rfvNomeCliente.ValidationGroup = validationGroup;
            rfvSobreNomeCliente.ValidationGroup = validationGroup;
            rfvEmailCliente.ValidationGroup = validationGroup;
            revEmail.ValidationGroup = validationGroup;
            rfvDDDFoneCliente.ValidationGroup = validationGroup;
            rfvFoneCliente.ValidationGroup = validationGroup;

            rfvTitulo.CssClass += classeCss;
            rfvNomeCliente.CssClass += classeCss;
            rfvSobreNomeCliente.CssClass += classeCss;
            rfvEmailCliente.CssClass += classeCss;
            revEmail.CssClass += classeCss;
            rfvDDDFoneCliente.CssClass += classeCss;
            rfvFoneCliente.CssClass += classeCss;
        }

        public void CarregarDados(Modelo.DadosCliente dadosCliente)
        {
            if (dadosCliente != null)
            {
                ddlTitulo.SelectedValue = dadosCliente.Titulo;
                txtNomeCliente.Text = dadosCliente.Nome;
                txtSobreNomeCliente.Text = dadosCliente.Sobrenome;
                txtEmailCliente.Text = dadosCliente.Email;
                txtDDDFoneCliente.Text = dadosCliente.DDD;
                txtFoneCliente.Text = dadosCliente.Telefone;
            }
        }
    }
}