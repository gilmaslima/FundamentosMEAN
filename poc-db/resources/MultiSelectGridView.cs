using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;

namespace Redecard.PN.Comum.Web
{    
    /// <summary>
    /// GridView com CheckBox multi-seleção.
    /// Suporta apenas Grids com chave simples (não composta).
    /// </summary>
    public class MultiSelectGridView : GridView, IPostBackEventHandler
    {
        #region [ Propriedades ]

        /// <summary>
        /// Lista de Chaves dos itens marcados
        /// </summary>
        public List<Object> Marcados
        {
            get
            {
                if (ViewState["Marcados"] == null)
                    ViewState["Marcados"] = new List<Object>();
                return (List<Object>)ViewState["Marcados"];
            }
            set
            {
                ViewState["Marcados"] = value;
            }
        }

        /// <summary>
        /// Lista de todas as Chaves dos itens
        /// </summary>
        private List<Object> Items
        {
            get
            {
                if (ViewState["Items"] == null)
                    ViewState["Items"] = new List<Object>();
                return (List<Object>)ViewState["Items"];
            }
            set
            {
                ViewState["Items"] = value;
            }
        }

        /// <summary>
        /// Se deve exibir o checkbox selecionar todos
        /// </summary>
        public Boolean ExibirSelecionarTodos
        {
            get
            {
                if (ViewState["ExibirSelecionarTodos"] == null)
                    ViewState["ExibirSelecionarTodos"] = true;
                return (Boolean)ViewState["ExibirSelecionarTodos"];
            }
            set { ViewState["ExibirSelecionarTodos"] = value; }
        }

        /// <summary>
        /// Função javascript customizada que pode ser chamada após
        /// marcação/desmarcação de checkbox de um item.
        /// </summary>
        public String OnClientCheckedChanged
        {
            get { return (String)ViewState["OnClientCheckedChanged"]; }
            set { ViewState["OnClientCheckedChanged"] = value; }
        }

        /// <summary>
        /// CSS que é aplicado à coluna de CheckBox
        /// </summary>
        public String CheckBoxColumnCssClass
        {
            get { return (String)ViewState["CheckBoxColumnCssClass"]; }
            set { ViewState["CheckBoxColumnCssClass"] = value; }
        }

        /// <summary>
        /// Server ID do CheckBox "Todos"
        /// </summary>
        public String CheckBoxTodosID { get { return "chkTodos"; } }

        /// <summary>
        /// Server ID do checkbox de cada item
        /// </summary>
        public String CheckBoxItemID { get { return "chkSelecionado"; } }

        /// <summary>
        /// Delegate para obtenção das chaves dos registros.
        /// </summary>
        public delegate List<Object> GetItemsKeyValueEventHandler(Object sender, List<Object> items);

        /// <summary>
        /// Evento customizado para obtenção das chaves dos registros.
        /// Caso não informado, utiliza a propriedade DataKeyNames[0] para recuperação da chave do item.
        /// </summary>
        public event GetItemsKeyValueEventHandler GetItemsKeyValue;

        #endregion

        #region [ Construtor ]
        /// <summary>
        /// Construtor padrão
        /// </summary>
        public MultiSelectGridView() : base()
        {
            //Bind dos eventos
            this.PageIndexChanging += new GridViewPageEventHandler(MultiSelectGridView_PageIndexChanging);
            this.Init += new EventHandler(MultiSelectGridView_Init);
            this.RowCreated += new GridViewRowEventHandler(MultiSelectGridView_RowCreated);
            this.Load += new EventHandler(MultiSelectGridView_Load);
            this.DataBinding+=new EventHandler(MultiSelectGridView_DataBinding);
        }

        #endregion

        #region [ Eventos do Controle ]

        /// <summary>
        /// Evento Init
        /// </summary>
        private void MultiSelectGridView_Init(object sender, EventArgs e)
        {
            //Criação da coluna para os CheckBoxes
            this.Columns.Insert(0, new TemplateField());
            this.Columns[0].HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            this.Columns[0].ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            this.Columns[0].ItemStyle.CssClass = this.CheckBoxColumnCssClass;
        }

        /// <summary>
        /// Evento Load
        /// </summary>
        private void MultiSelectGridView_Load(object sender, EventArgs e)
        {
            //Atualiza a lista de itens marcados/desmarcados da página atual carregada da grid
            foreach (GridViewRow row in this.Rows)
            {
                var chkbox = row.FindControl(this.CheckBoxItemID) as CheckBox;
                if (chkbox != null)
                {
                    Object chave = (Object)this.DataKeys[row.RowIndex].Value;
                    if (chkbox.Checked)
                    {
                        if (!this.Marcados.Contains(chave))
                            this.Marcados.Add(chave);
                    }
                    else
                    {
                        if (this.Marcados.Contains(chave))
                            this.Marcados.Remove(chave);
                    }
                }
            }

            //Atualiza exibição do checkBox "Selecionar Todos" conforme configuração do controle
            if (this.HeaderRow != null)
            {
                var chkTodos = this.HeaderRow.FindControl(this.CheckBoxTodosID) as CheckBox;
                chkTodos.Visible = this.ExibirSelecionarTodos;
            }

            //Atualiza contagem dos itens marcados e total de itens
            this.Attributes["total"] = this.Items.Count.ToString();
            this.Attributes["totalchecked"] = this.Marcados.Count.ToString();
        }

        /// <summary>
        /// Evento DataBinding
        /// </summary>
        private void MultiSelectGridView_DataBinding(object sender, EventArgs e)
        {
            //Propriedade dos itens que armazena a chave
            String chave = this.DataKeyNames[0];

            //Transforma o DataSource em uma lista de Objects
            var collection = this.DataSource as ICollection;
            Array array = Array.CreateInstance(typeof(Object), collection.Count);
            collection.CopyTo(array, 0);
            List<Object> items = array.Cast<Object>().ToList();

            //Obtém as chaves dos itens
            if (GetItemsKeyValue != null)
                this.Items = GetItemsKeyValue(this, items);
            else if (items.Count > 0)
            {
                //Por Reflection, recupera a lista de chaves dos itens
                var property = items[0].GetType().GetProperty(chave);
                this.Items = items.Select(
                    item => property.GetValue(item, null)).ToList();
            }
            else
                this.Items = new List<Object>();
        }

        /// <summary>
        /// Evento RowCreated
        /// </summary>
        private void MultiSelectGridView_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                //Cria o checkbox "Selecionar" todos
                var chkTodos = new CheckBox();
                chkTodos.Visible = this.ExibirSelecionarTodos;
                chkTodos.ID = this.CheckBoxTodosID;
                chkTodos.Checked = this.Marcados.Count > 0 && this.Marcados.Count == this.Items.Count;
                this.Attributes["totalchecked"] = this.Marcados.Count.ToString();
                this.Attributes["total"] = this.Items.Count.ToString();
                chkTodos.Attributes["onclick"] = this.Page.ClientScript.GetPostBackEventReference(this, this.CheckBoxTodosID);
                e.Row.Cells[0].Controls.Add(chkTodos);
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Cria o checkbox do item
                var chkbox = new CheckBox();
                chkbox.ID = this.CheckBoxItemID;

                String nomeFuncao = this.RegistrarScript();
                chkbox.Attributes["onclick"] = String.Format("{0}(this);", nomeFuncao);
                e.Row.Cells[0].Controls.Add(chkbox);

                Object chave = (Object)this.DataKeys[e.Row.RowIndex].Value;
                if (chave != null)
                    chkbox.Checked = this.Marcados.Contains(chave);
            }
        }
        
        /// <summary>
        /// Evento PageIndexChanging
        /// </summary>
        private void MultiSelectGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //Atualiza a lista de itens marcados/desmarcados
            foreach (GridViewRow row in this.Rows)
            {
                var chkbox = row.FindControl(this.CheckBoxItemID) as CheckBox;
                if (chkbox != null)
                {
                    Object chave = (Object)this.DataKeys[row.RowIndex].Value;
                    if (chkbox.Checked)
                    {
                        if (!this.Marcados.Contains(chave))
                            this.Marcados.Add(chave);
                    }
                    else
                    {
                        if (this.Marcados.Contains(chave))
                            this.Marcados.Remove(chave);
                    }
                }
            }
        }
        
        #endregion

        #region [ Implementações IPostBackEventHandler ]

        /// <summary>
        /// Implementação da interface IPostBackEventHandler
        /// </summary>
        void IPostBackEventHandler.RaisePostBackEvent(String eventArgument)
        {
            //Se postback foi causado pelo checkbox "Selecionar Todos"
            if (String.Compare(eventArgument, this.CheckBoxTodosID, true) == 0)
            {
                this.Marcados.Clear();

                var chkTodos = this.HeaderRow.FindControl(this.CheckBoxTodosID) as CheckBox;
                var selecionarTodos = chkTodos.Checked;

                //Marca todos os itens
                if (selecionarTodos)
                {
                    this.Marcados.Clear();
                    this.Marcados.AddRange(this.Items);
                }

                //Atualiza contagem de itens marcados
                chkTodos.Visible = this.ExibirSelecionarTodos;
                this.Attributes["totalchecked"] = this.Marcados.Count.ToString();
                this.Attributes["total"] = this.Items.Count.ToString();

                //Atualiza marcação nos checkboxes
                foreach (GridViewRow row in this.Rows)
                {
                    var chkbox = row.FindControl(this.CheckBoxItemID) as CheckBox;
                    if (chkbox != null)
                        chkbox.Checked = selecionarTodos;
                }
            }
            else
            {
                base.RaisePostBackEvent(eventArgument);
            }
        }

        #endregion

        #region [ Métodos Públicos ]

        /// <summary>
        /// Marca todos os itens da grid
        /// </summary>
        public void MarcarTodos()
        {
            MarcarTodos(true);
        }

        /// <summary>
        /// Desmarca todos os itens da grid
        /// </summary>
        public void DesmarcarTodos()
        {
            MarcarTodos(false);
        }

        /// <summary>
        /// Marca/Desmarca todos os itens da grid
        /// </summary>
        /// <param name="marcar">Marcar/Desmarcar</param>
        public void MarcarTodos(Boolean marcar)
        {
            var chkTodos = this.HeaderRow.FindControl(this.CheckBoxTodosID) as CheckBox;
            chkTodos.Checked = marcar;
            this.RaisePostBackEvent(this.CheckBoxTodosID);
        }

        #endregion

        #region [ Métodos Privados ]

        /// <summary>
        /// Registra javascripts 
        /// </summary>
        private String RegistrarScript()
        {
            String nomeFuncao = String.Format("{0}_chkboxClicked", this.ClientID);

            StringBuilder script = new StringBuilder()
                .Append("function ").Append(nomeFuncao).Append("(sender) {")
                .AppendLine("   var grid = $(sender).closest('table');")
                .AppendLine("   var chkTodos = grid.children('tbody').find('tr th input:checkbox');")
                .AppendLine("   var total = parseInt(grid.attr('total'), 10);")
                .AppendLine("   var totalChecked = parseInt(grid.attr('totalchecked'), 10);")
                .AppendLine("   if(!$(sender).is(':checked')) {")
                .AppendLine("       chkTodos.prop('checked', false);")
                .AppendLine("       totalChecked--;")
                .AppendLine("   } else {")
                .AppendLine("       totalChecked++;")
                .AppendLine("       chkTodos.prop('checked', totalChecked == total);")
                .AppendLine("   }")
                .AppendLine("   grid.attr('totalchecked', totalChecked);");

            if (!String.IsNullOrEmpty(this.OnClientCheckedChanged))
                script.Append(this.OnClientCheckedChanged).AppendLine("(total, totalChecked);");

            script.AppendLine("}");

            if (!Page.ClientScript.IsClientScriptBlockRegistered(nomeFuncao))
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), nomeFuncao, script.ToString(), true);

            return nomeFuncao;
        }

        #endregion
    }
}