using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

using Redecard.PN.Comum;

namespace Redecard.PN.Request.SharePoint.Layouts.Request
{
    public partial class CartaEnvelope : ApplicationPageBaseAnonima
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["dados"] != null)
                {
                    //decodificação da query string com os dados do chargeback
                    QueryStringSegura queryString = new QueryStringSegura(Request.QueryString["dados"]);
                    if (queryString["Modo"].Equals("Carta"))
                    {
                        CarregarCarta(queryString, false);
                    }

                    if (queryString["Modo"].Equals("Envelope"))
                    {
                        CarregarEnvelope();
                    }

                    if (queryString["Modo"].Equals("CartaEnvelope"))
                    {
                        CarregarCarta(queryString, true);
                    }                    
                }
            }

            //Abrir tela de impressão
            ClientScript.RegisterStartupScript(GetType(), "print", "window.print();", true);
        }

        

        /// <summary>
        /// Override que também exibe o envelope, além da carta
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="mostrarEnvelope"></param>
        private void CarregarCarta(QueryStringSegura queryString, bool mostrarEnvelope)
        {
            lblProcesso.Text = queryString["Processo"];
            lblDataVenda.Text = queryString["DataVenda"];
            lblResumoVendas.Text = queryString["ResumoVendas"];                        
            lblCartaoNSU.Text = queryString["FlagNSU"] == "C" ? "Número do Cartão" : "Número do CV";                
            lblNumeroCV.Text = queryString["NumCartao"];
            lblValorVenda.Text = queryString["ValorVenda"];
            lblPontoVenda.Text = queryString["NumPV"];

            pnlCarta.Visible = true;

            if (mostrarEnvelope)
                CarregarEnvelope();
        }

        /// <summary>
        /// Exibe a imagem do envelope
        /// </summary>
        private void CarregarEnvelope()
        {
            pnlEnvelope.Visible = true;
        }
    }
}
