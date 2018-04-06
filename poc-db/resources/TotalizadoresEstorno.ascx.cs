/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using Redecard.PN.Extrato.Core.Web.Controles.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2
{
    public partial class TotalizadoresEstorno : BaseUserControl
    {
        public class Bandeira
        {
            public String NomeBandeira { get; set; }
            public Decimal Valor { get; set; }
            public String TipoVenda { get; set; }

            public Bandeira(String nomeBandeira, Decimal valor)
            {
                this.NomeBandeira = nomeBandeira;
                this.Valor = valor;
                this.TipoVenda = String.Empty;
            }

            public Bandeira(String nomeBandeira, Decimal valor, String tipoVenda)
            {
                this.NomeBandeira = nomeBandeira;
                this.Valor = valor;
                this.TipoVenda = tipoVenda;
            }
        }

        /// <summary>Valor total no período</summary>
        public Decimal ValorTotal { get; set; }

        private List<Bandeira> bandeiras;
        /// <summary>Totais por Bandeiras</summary>        
        public List<Bandeira> Bandeiras
        {
            get { return bandeiras ?? (bandeiras = new List<Bandeira>()); }
            set { bandeiras = value; }
        }

        private Boolean exibirTotaisPorBandeiras = true;
        /// <summary>Exibe box de totais por bandeira</summary>
        public Boolean ExibirTotaisPorBandeiras
        {
            get { return exibirTotaisPorBandeiras; }
            set { exibirTotaisPorBandeiras = value; }
        }

        private static JavaScriptSerializer JS = new JavaScriptSerializer();
        public Object JsonTotalizadores
        {
            set { hdnTotalizadoresEstornoAsync.Value = JS.Serialize(value); }
        }

        protected void repBandeiras_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Image imgBandeira = e.Item.FindControl("imgBandeira") as Image;
                Literal lblNomeBandeira = e.Item.FindControl("lblNomeBandeira") as Literal;
                Literal lblValorBandeira = e.Item.FindControl("lblValorBandeira") as Literal;
                Bandeira bandeira = e.Item.DataItem as Bandeira;

                //Esconde imagem caso sejam outras bandeiras
                imgBandeira.Visible = !bandeira.NomeBandeira.Trim().ToUpper().Contains("OUTRAS");
                //Monta endereço da imagem, nome e valor da bandeira
                imgBandeira.ImageUrl = String.Format("/_layouts/Redecard.PN.Extrato.SharePoint/Styles/ico_{0}.jpg", bandeira.NomeBandeira.Trim());
                lblNomeBandeira.Text = bandeira.NomeBandeira;
                lblValorBandeira.Text = bandeira.Valor.ToString("C", PtBR);
            }
        }

        /// <summary>
        /// Atualizar os valores no controle
        /// </summary>
        public void Atualizar()
        {
            ExibirTotaisPorBandeiras = Bandeiras != null && Bandeiras.Count >= 0;

            //Carrega valores
            CarregarValores();

            //Carrega totais
            CarregarTotaisBandeiras();

            //Força exibição
            this.Visible = true;
        }

        /// <summary>
        /// CarregarValores
        /// </summary>
        private void CarregarValores()
        {
            ValorTotal = Bandeiras.Sum(bc => bc.Valor);

            this.qiValoresConsolidados.QuadroInformacaoItems.Clear();
            this.qiValoresConsolidados.QuadroInformacaoItems.Add(new QuadroInformacaoItem
            {
                Descricao = "total de estornos realizados",
                Valor = ValorTotal.ToString("C", PtBR)
            });
        }

        /// <summary>
        /// CarregarTotaisBandeiras
        /// </summary>
        private void CarregarTotaisBandeiras()
        {
            //Carrega bandeiras
            pnlTotaisBandeira.Visible = ExibirTotaisPorBandeiras;
            if (ExibirTotaisPorBandeiras)
            {
                //Agrupa bandeiras por tipo e soma total, para garantir que não exista repetição de bandeira
                var bandeiras = Bandeiras
                    .GroupBy(bandeira => bandeira.NomeBandeira)
                    .Select(grupoBandeira => new Bandeira(grupoBandeira.Key, grupoBandeira.Sum(bandeira => bandeira.Valor)))
                    .ToList();

                //Bandeiras que serão renderizadas
                rptBandeira.DataSource = bandeiras.ToList();
                rptBandeira.DataBind();
            }
        }
    }
}