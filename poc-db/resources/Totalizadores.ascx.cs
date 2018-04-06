/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using System.Web.UI;
using Redecard.PN.Extrato.Core.Web.Controles.Portal;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.ExtratoV2
{
    public partial class Totalizadores : BaseUserControl
    {
        public class Bandeira
        {
            public String NomeBandeira { get; set; }
            public Decimal Valor { get; set; }

            public Bandeira(String nomeBandeira, Decimal valor)
            {
                this.NomeBandeira = nomeBandeira;
                this.Valor = valor;
            }
        }

        /// <summary>Total Valor Líquido no período</summary>
        public Decimal? ValorLiquido { get; set; }

        /// <summary>Total Valor Bruto no período</summary>
        public Decimal? ValorBruto { get; set; }

        /// <summary>Total exibido no totalizador superior</summary>
        public Decimal? ValorTotalSuperior { get; set; }

        /// <summary>Total exibido no totalizador inferior</summary>
        public Decimal? ValorTotalInferior { get; set; }

        private List<Bandeira> _bandeiras;
        /// <summary>Totais por Bandeiras</summary>        
        public List<Bandeira> Bandeiras
        {
            get { return _bandeiras ?? (_bandeiras = new List<Bandeira>()); }
            set { _bandeiras = value; }
        }

        private Boolean _exibirTotaisPorBandeiras = true;
        /// <summary>Exibe box de totais por bandeira</summary>
        public Boolean ExibirTotaisPorBandeiras
        {
            get { return _exibirTotaisPorBandeiras; }
            set { _exibirTotaisPorBandeiras = value; }
        }

        /// <summary>
        /// Atribui um título customizado para o totalizador
        /// </summary>
        public String Titulo
        {
            set
            {
                qiValoresConsolidados.Titulo = value;
            }
        }

        /// <summary>
        /// Texto do subtítulo inferior
        /// </summary>
        public String SubtituloInferior
        {
            set
            {
                ViewState["SubtituloInferior"] = value;
            }
            get
            {
                return Convert.ToString(ViewState["SubtituloInferior"]);
            }
        }

        /// <summary>
        /// Texto do subtítulo superior
        /// </summary>
        public String SubtituloSuperior
        {
            set
            {
                ViewState["SubtituloSuperior"] = value;
            }
            get
            {
                return Convert.ToString(ViewState["SubtituloSuperior"]);
            }
        }

        private static JavaScriptSerializer JS = new JavaScriptSerializer();
        public Object JsonTotalizadores
        {
            set { hdnTotalizadoresAsync.Value = JS.Serialize(value); }
        }

        protected void rptBandeira_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Image imgBandeira = e.Item.FindControl("imgBandeira") as Image;
                Literal lblNomeBandeira = e.Item.FindControl("lblNomeBandeira") as Literal;
                Literal lblValorBandeira = e.Item.FindControl("lblValorBandeira") as Literal;
                Bandeira bandeira = e.Item.DataItem as Bandeira;

                //Monta endereço da imagem, nome e valor da bandeira
                imgBandeira.ImageUrl = String.Format("/_layouts/Redecard.PN.Extrato.SharePoint/Styles/ico_{0}.jpg", bandeira.NomeBandeira.Trim());
                lblNomeBandeira.Text = bandeira.NomeBandeira;
                lblValorBandeira.Text = bandeira.Valor.ToString("C", PtBR);
            }
        }

        public void Atualizar()
        {
            //Carrega valores consolidados
            CarregarValoresConsolidados();

            //Carrega totais por bandeira
            CarregarTotaisPorBandeira();

            //Força exibição
            this.Visible = true;
        }

        private void CarregarValoresConsolidados()
        {
            this.qiValoresConsolidados.QuadroInformacaoItems.Clear();

            //Carrega valor líquido e valor bruto, se definidos
            if (ValorLiquido.HasValue && ValorBruto.HasValue)
            {
                this.qiValoresConsolidados.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total no período valor bruto",
                    Valor = ValorBruto.Value.ToString("C", PtBR)
                });

                this.qiValoresConsolidados.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total no período valor líquido",
                    Valor = ValorLiquido.Value.ToString("C", PtBR)
                });
            }
            //ou carrega apenas valor líquido, se definido
            else if (ValorLiquido.HasValue)
            {
                this.qiValoresConsolidados.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                {
                    Descricao = "total no período valor líquido",
                    Valor = ValorLiquido.Value.ToString("C", PtBR)
                });
            }
            else if (ValorTotalSuperior.HasValue || ValorTotalInferior.HasValue)
            {
                if (ValorTotalSuperior.HasValue)
                {
                    this.qiValoresConsolidados.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                    {
                        Descricao = this.SubtituloSuperior,
                        Valor = ValorTotalSuperior.Value.ToString("C", PtBR)
                    });
                }

                if (ValorTotalInferior.HasValue)
                {
                    this.qiValoresConsolidados.QuadroInformacaoItems.Add(new QuadroInformacaoItem
                    {
                        Descricao = this.SubtituloInferior,
                        Valor = ValorTotalInferior.Value.ToString("C", PtBR)
                    });
                }
            }
        }

        private void CarregarTotaisPorBandeira()
        {
            //Carrega bandeiras
            pnlTotaisBandeira.Visible = Bandeiras != null && Bandeiras.Count > 0 && ExibirTotaisPorBandeiras;
            if (Bandeiras != null && Bandeiras.Count > 0)
            {
                //Agrupa bandeiras por tipo e soma total, para garantir que não exista repetição de bandeira
                var bandeiras = Bandeiras
                    .GroupBy(bandeira => bandeira.NomeBandeira)
                    .Select(grupoBandeira => new Bandeira(grupoBandeira.Key, grupoBandeira.Sum(bandeira => bandeira.Valor)))
                    .ToList();

                //Bandeiras que serão renderizadas na primeira linha
                rptBandeira.DataSource = bandeiras.ToList();
                rptBandeira.DataBind();
            }
        }
    }
}