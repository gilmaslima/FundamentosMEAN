using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Redecard.PN.Extrato.SharePoint.Helper.ConsultarVendas
{
    [ParseChildren(true, "Items"),
    DefaultProperty("Items"),
    ToolboxData(@"  <{0}:ConsultarVendasSumario ID="""" runat=""server"">
                        <Item Descricao="""" Valor="""" />
                    </{0}:ConsultarVendasSumario>")]
    public class ConsultarVendasSumario : WebControl
    {
        #region Propriedades públicas

        /// <summary>
        /// Quantidade de colunas para disposição do conteúdo
        /// - Default: 3 colunas
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(3),
        Browsable(true)]
        public int Columns
        {
            get
            {
                return ((int?)ViewState["Columns"]).GetValueOrDefault(3);
            }
            set
            {
                ViewState["Columns"] = value;
            }
        }

        /// <summary>
        /// Items a serem listados na tela
        /// </summary>
        public List<ConsultarVendasSumarioItem> Items
        {
            get
            {
                if (ViewState["ConsultarVendasSumarioItems"] == null)
                    ViewState["ConsultarVendasSumarioItems"] = new List<ConsultarVendasSumarioItem>();

                return (List<ConsultarVendasSumarioItem>)ViewState["ConsultarVendasSumarioItems"];
            }
            set
            {
                ViewState["ConsultarVendasSumarioItems"] = value;
            }
        }

        /// <summary>
        /// Determina se deve renderizar table adicional com o conteúdo para impressão
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(false),
        Browsable(true)]
        public Boolean RenderPrintTable
        {
            get
            {
                return ((bool?)ViewState["RenderPrintTable"]).GetValueOrDefault(false);
            }
            set
            {
                ViewState["RenderPrintTable"] = value;
            }
        }

        #endregion

        /// <summary>
        /// Renderiza o controle na tela
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            // trata o número mínimo de colunas
            if (this.Columns <= 0)
                this.Columns = 1;

            // controladores
            int initial = 0;
            int maxItems = (int)(this.Items.Count / this.Columns);
            if (this.Items.Count % this.Columns > 0 || maxItems == 0)
                maxItems++;

            List<List<ConsultarVendasSumarioItem>> listItems = new List<List<ConsultarVendasSumarioItem>>();
            for (int i = 0; i < this.Columns; i++)
            {
                // valida o últim item da listagem (evitando exception)
                if (maxItems + initial > this.Items.Count)
                    maxItems = this.Items.Count - initial;

                // registra quebra de listagem
                listItems.Add(this.Items.GetRange(initial, maxItems));

                // define o item inicial para a próxima listagem
                initial += maxItems;
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Class, string.Format("{0} white-bg", this.CssClass));
            AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                string classCols = string.Concat("col-", this.Columns.ToString());
                if (this.Columns <= 1)
                    classCols = "";

                writer.AddAttribute(HtmlTextWriterAttribute.Class, string.Format("{0} info-display-rede", classCols));
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                {
                    foreach (var itemList in listItems)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, "col");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, "money-values");
                            writer.RenderBeginTag(HtmlTextWriterTag.Div);
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, "value-box");
                                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                                {
                                    foreach (var item in itemList)
                                    {
                                        this.RenderItems(writer, item);
                                    }
                                }
                                writer.RenderEndTag();
                            }
                            writer.RenderEndTag();
                        }
                        writer.RenderEndTag();
                    }
                }
                writer.RenderEndTag();

                if (this.RenderPrintTable)
                {
                    // renderiza table para impressão com display='none'
                    var table = this.GetTableDesign();
                    table.Style.Add("display", "none");
                    table.RenderControl(writer);
                }
            }
            writer.RenderEndTag();
        }

        /// <summary>
        /// Renderiza os itens agrupados pelo número de colunas
        /// </summary>
        /// <param name="writer"></param>
        protected void RenderItems(HtmlTextWriter writer, ConsultarVendasSumarioItem item)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "value-rede");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                // descrição
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "commom-text-bold");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(item.Descricao);
                writer.RenderEndTag();

                // valor
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "value-number");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(item.Valor);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        /// <summary>
        /// Retorna o conteúdo renderizado em modo tabela
        /// </summary>
        /// <returns></returns>
        public Table GetTableDesign(string title = "")
        {
            Table table = new Table()
            {
                CssClass = "tabelaDados dados rede-table-impressao"
            };

            if (!string.IsNullOrWhiteSpace(title))
            {
                TableCell cellTitle = new TableCell() { Text = title };
                
                TableRow rowTitle = new TableRow();
                rowTitle.Cells.Add(cellTitle);

                table.Rows.Add(rowTitle);
            }

            foreach (var item in this.Items)
            {
                TableRow row = new TableRow();

                row.Cells.Add(new TableCell()
                {
                    Text = item.Descricao,
                    CssClass = "commom-text-bold"
                });

                row.Cells.Add(new TableCell()
                {
                    Text = item.Valor
                });

                table.Rows.Add(row);
            }

            return table;
        }
    }
}
