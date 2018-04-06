using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Redecard.PN.DataCash.Modelo;
using System.Globalization;
using Redecard.PN.Comum;
namespace Redecard.PN.DataCash.controles
{
    public partial class DadosIATA : System.Web.UI.UserControl
    {
        public Int32 QuantidadePassageiros { get { return ListaPassageiros.Count; } }
        public Decimal ValorPassagem
        {
            get
            {
                Decimal valorPassagem = 0;
                if (!string.IsNullOrEmpty(txtValorPassagem.Text))
                {
                    valorPassagem = Convert.ToDecimal(txtValorPassagem.Text);
                }
                return valorPassagem;
            }
        }
        public Decimal TotalPassagem { get { return ValorPassagem * QuantidadePassageiros; } }
        //public bool Visible { get { return tbIATA.Visible; } set { tbIATA.Visible = value; } }
        public List<Passageiro> ListaPassageiros
        {
            get
            {
                if (object.ReferenceEquals(ViewState["ListaPassageiros"], null))
                    ViewState["ListaPassageiros"] = new List<Passageiro>();

                return (ViewState["ListaPassageiros"] as List<Passageiro>);
            }
            set { ViewState["ListaPassageiros"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ListaPassageiros.Count == 0)
                    CarregaQtdPassageiros(1);
            }
        }

        protected void ddlQtdPassageiros_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int16 qtd = ddlQtdPassageiros.SelectedValue.ToInt16(0);
            MontaListaPassageiros(qtd);
        }

        protected void CarregaQtdPassageiros(Int32 qtdPassageiros)
        {
            for (int i = 0; i < 50; i++)
            {
                ddlQtdPassageiros.Items.Add(new ListItem((i + 1).ToString(), (i + 1).ToString()));
            }

            if (qtdPassageiros > 0)
                MontaListaPassageiros(qtdPassageiros);
        }

        protected void MontaListaPassageiros(Int32 qtdPassageiros)
        {
            if (qtdPassageiros > ListaPassageiros.Count)
            {
                while (qtdPassageiros > ListaPassageiros.Count)
                {
                    Passageiro passageiro = new Passageiro();
                    passageiro.Id = ListaPassageiros.Count + 1;
                    ListaPassageiros.Add(passageiro);
                }
            }
            else if (qtdPassageiros < ListaPassageiros.Count)
            {
                if (ListaPassageiros.Count > 0)
                {
                    while (ListaPassageiros.Count > qtdPassageiros)
                    {
                        ListaPassageiros.RemoveAt(ListaPassageiros.Count - 1);
                    }
                }
            }

            rptPassageiros.DataSource = ListaPassageiros;
            rptPassageiros.DataBind();
        }

        public VendaPagamentoIATA ObterDadosIata(enFormaPagamento formaPagamento)
        {
            VendaPagamentoIATA vendaPagamento;

            CarregarListaPassageiros();

            if (formaPagamento.Equals(enFormaPagamento.Avista))
            {
                vendaPagamento = new VendaPagamentoIATAAvista();                
            }
            else
            {
                vendaPagamento = new VendaPagamentoIATAParceladoEstabelecimento();
            }
            vendaPagamento.NomeAgenciaViagem = txtNomeAgenciaViagens.Text;
            vendaPagamento.Codigo = txtCodigoIATA.Text;
            vendaPagamento.ValorPassagem = txtValorPassagem.Text.ToDecimalNull(0).Value;
            vendaPagamento.Passageiros = ListaPassageiros;
            vendaPagamento.CodigoCompanhia = txtCodigoCompahia.Text;
            vendaPagamento.Classe = ddlClasse.SelectedValue;
            vendaPagamento.ClasseDescricao = ddlClasse.SelectedItem.Text;
            vendaPagamento.CodigoAeroportoPartida = txtCodigoAeroportoPartida.Text;
            vendaPagamento.CodigoAeroportoDestino = txtCodigoAeroportoDestino.Text;
            vendaPagamento.DataPartida = txtDataPartida.Text; 
            vendaPagamento.FusoHorarioPartida = ddlFusoHorario.SelectedValue;
            vendaPagamento.FusoHorarioPartidaDescricao = ddlFusoHorario.SelectedItem.Text;
            vendaPagamento.TaxaEmbarque = txtTaxaEmbarque.Text == String.Empty ? 0 : txtTaxaEmbarque.Text.ToDecimalNull(0).Value;

            return vendaPagamento;
        }

        private void CarregarListaPassageiros()
        {
            Passageiro passageiro;

            for (int i = 0; i < rptPassageiros.Items.Count; i++)
            {
                RepeaterItem item = rptPassageiros.Items[i];
                passageiro = new Passageiro();
                TextBox txtNomePassageiro = item.FindControl("txtNomePassageiro") as TextBox;
                TextBox txtCodigoReferenciaPassageiro = item.FindControl("txtCodigoReferenciaPassageiro") as TextBox;
                TextBox txtNumeroBilhete = item.FindControl("txtNumeroBilhete") as TextBox;
                passageiro.Nome = txtNomePassageiro.Text;
                passageiro.CodigoReferencia = txtCodigoReferenciaPassageiro.Text;
                passageiro.NumeroBilhete = txtNumeroBilhete.Text;

                ListaPassageiros[i] = passageiro;
            }
        }

        public void CarregarIata(VendaPagamentoIATA venda)
        {
            txtNomeAgenciaViagens.Text = venda.NomeAgenciaViagem;
            txtCodigoIATA.Text = Convert.ToString(venda.Codigo);
            txtValorPassagem.Text = venda.ValorPassagem.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR"));

            for (int i = 0; i < 50; i++)
            {
                ddlQtdPassageiros.Items.Add(new ListItem((i + 1).ToString(), (i + 1).ToString()));
            }

            if(venda.Passageiros != null)
                ddlQtdPassageiros.SelectedValue = venda.Passageiros.Count.ToString();

            ListaPassageiros = venda.Passageiros;
            MontaListaPassageiros(ListaPassageiros.Count);
            
            txtCodigoCompahia.Text = venda.CodigoCompanhia;
            ddlClasse.SelectedValue = venda.Classe;
            txtCodigoAeroportoPartida.Text = venda.CodigoAeroportoPartida;
            txtCodigoAeroportoDestino.Text = venda.CodigoAeroportoDestino;
            txtDataPartida.Text = venda.DataPartida;
            ddlFusoHorario.SelectedValue = venda.FusoHorarioPartida;
            txtTaxaEmbarque.Text = venda.TaxaEmbarque.ToString("N", CultureInfo.CreateSpecificCulture("pt-BR"));
        }

        protected void rptPassageiros_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item)
                return;

            TextBox txtNomePassageiro = e.Item.FindControl("txtNomePassageiro") as TextBox;
            TextBox txtCodigoReferenciaPassageiro = e.Item.FindControl("txtCodigoReferenciaPassageiro") as TextBox;
            TextBox txtNumeroBilhete = e.Item.FindControl("txtNumeroBilhete") as TextBox;
            txtNomePassageiro.Attributes.Add("autocomplete", "off");
            txtCodigoReferenciaPassageiro.Attributes.Add("autocomplete", "off");
            txtNumeroBilhete.Attributes.Add("autocomplete", "off");
        }
    }
}