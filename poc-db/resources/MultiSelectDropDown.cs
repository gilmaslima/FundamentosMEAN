/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Web.UI;
using Redecard.PN.Comum;
using System.Web.UI.WebControls;
using System.Linq;
using System.ComponentModel;
using System.Text;

[assembly: TagPrefix("Redecard.PN.Comum.Web", "Redecard")]
namespace Redecard.PN.Comum.Web
{
    /// <summary>
    /// MultiSelect DropDownList
    /// </summary>
    [ToolboxData(@"<{0}:MultiSelectDropDown ID="""" runat=""server"">                    
                    <Items>
                      <asp:ListItem Value="" Text="" Selected="" />
                      <asp:ListItem Value="" Text="" Selected="" />
                    </Items>
                  </{0}:MultiSelectDropDown>")]
    public class MultiSelectDropDown : CheckBoxList, IPostBackEventHandler
    {
        #region [ Propriedades não implementadas ]

        public override int SelectedIndex { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public override ListItem SelectedItem { get { throw new NotImplementedException(); } }
        public override string SelectedValue { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        #endregion

        #region [ Propriedades ]

        /// <summary>
        /// Flag indicando se todos os itens foram selecionado.
        /// </summary>
        public Boolean TodosItensSelecionados
        {
            get { return this.SelectedItems.ToArray().Length == Items.Count; }
        }

        /// <summary>
        /// Itens selecionados
        /// </summary>
        public new IEnumerable<ListItem> SelectedItems
        {
            get { return this.Items.Cast<ListItem>().Where(item => item.Selected).ToArray(); }
        }

        /// <summary>
        /// Índices selecionados
        /// </summary>
        public new IEnumerable<Int32> SelectedIndexes
        {
            get { return this.SelectedItems.ToArray().Select(item => this.Items.IndexOf(item)); }
            set
            {
                foreach (ListItem item in this.Items)
                    item.Selected = value.Contains(this.Items.IndexOf(item));
            }
        }

        /// <summary>
        /// Getter/Setter dos valores selecionados
        /// </summary>
        public new IEnumerable<String> SelectedValues
        {
            get { return this.SelectedItems.ToArray().Select(item => item.Value); }
            set
            {
                foreach (ListItem item in this.Items)
                    item.Selected = value.Contains(item.Value);
            }
        }

        /// <summary>
        /// Flag configurando se o controle realizará um PostBack ao perder o foco.<br/>
        /// Valor padrão: false
        /// </summary>
        public new Boolean AutoPostBack
        {
            get
            {
                if (ViewState["AutoPostBack"] == null)
                    ViewState["AutoPostBack"] = false;
                return (Boolean)ViewState["AutoPostBack"];
            }
            set { ViewState["AutoPostBack"] = value; }
        }

        /// <summary>
        /// Configura a exibição da opção Selecionar Todos.<br/>        
        /// Valor padrão: false
        /// </summary>
        public Boolean ExibirSelecionarTodos
        {
            get
            {
                if (ViewState["ExibirSelecionarTodos"] == null)
                    ViewState["ExibirSelecionarTodos"] = false;
                return (Boolean)ViewState["ExibirSelecionarTodos"];
            }
            set { ViewState["ExibirSelecionarTodos"] = value; }
        }

        /// <summary>
        /// Configura o texto da opção Selecionar Todos.<br/>
        /// Valor padrão: Todos
        /// </summary>
        public String TextoSelecionarTodos
        {
            get 
            {
                if (ViewState["TextoSelecionarTodos"] == null)
                    ViewState["TextoSelecionarTodos"] = "Todos";
                return (String)ViewState["TextoSelecionarTodos"]; 
            }
            set { ViewState["TextoSelecionarTodos"] = value; }
        }

        /// <summary>
        /// Configura o texto que é exibido no textbox quando nenhuma opção foi selecionada.<br/>
        /// Valor padrão: Selecione...
        /// </summary>
        public String TextoVazio
        {
            get 
            {
                if (ViewState["TextoVazio"] == null)
                    ViewState["TextoVazio"] = "Selecione...";
                return (String)ViewState["TextoVazio"]; 
            }
            set { ViewState["TextoVazio"] = value; }
        }

        /// <summary>
        /// Função Javascript chamada para customização do descritivo exibido no TextBox após a seleção das bandeiras.<br/>
        /// Exemplo: function atualizarDescricao(arrItems, arrSelectedItems, strEmptyText, strSelectAllText);<br/>
        /// Onde:<br/>
        /// - arrItems: Todos as opções do controle, array de tamanho N com a estrutura [{ Descricao: '', Valor: ''}, ..., n]<br/>
        /// - arrSelectedItems: Opções selecionadas, array de tamanho N com a estrutura [{ Descricao: '', Valor: ''}, ..., n]<br/>
        /// - strEmptyText: texto configurado para ser exibido quando não existem itens selecionados. <br/>
        ///                 Corresponde à propriedade TextoVazio.
        /// - strSelectAllText: texto configurado para ser exibido quando todos os itens estão selecionados.<br/>
        ///                     Corresponde à propriedade TextoSelecionarTodos.
        /// </summary>
        public String AtualizarDescricaoFuncaoJs
        {
            get { return (String)ViewState["AtualizarDescricaoFuncaoJs"]; }
            set { ViewState["AtualizarDescricaoFuncaoJs"] = value; }
        }

        /// <summary>
        /// Função Javascript chamada para customização da tooltip exibida no TextBox após a seleção das bandeiras.<br/>
        /// Exemplo: function atualizarDescricao(arrItems, arrSelectedItems, strEmptyText, strSelectAllText);<br/>
        /// Onde:<br/>
        /// - arrItems: Todos as opções do controle, array de tamanho N com a estrutura [{ Descricao: '', Valor: ''}, ..., n]<br/>
        /// - arrSelectedItems: Opções selecionadas, array de tamanho N com a estrutura [{ Descricao: '', Valor: ''}, ..., n]<br/>
        /// - strEmptyText: texto configurado para ser exibido quando não existem itens selecionados. <br/>
        ///                 Corresponde à propriedade TextoVazio.
        /// - strSelectAllText: texto configurado para ser exibido quando todos os itens estão selecionados.<br/>
        ///                     Corresponde à propriedade TextoSelecionarTodos.
        /// </summary>
        public String AtualizarTituloFuncaoJs
        {
            get { return (String)ViewState["AtualizarTituloFuncaoJs"]; }
            set { ViewState["AtualizarTituloFuncaoJs"] = value; }
        }

        /// <summary>
        /// Customização de CSS dos controles internos.
        /// </summary>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public new CssStyle Css
        {
            get
            {
                if (ViewState["Css"] == null)
                    ViewState["Css"] = new CssStyle();
                return (CssStyle)ViewState["Css"];
            }
            set { ViewState["Css"] = value; }
        }

        /// <summary>
        /// Tipo da imagem que será carregada no controle
        /// </summary>
        public CssImageType? Tipo 
        {
            get
            {
                return (CssImageType?)ViewState["Tipo"];
            }
            set { ViewState["Tipo"] = value; }
        }

        /// <summary>
        /// Getter/Setter para flag que indica se o botão/imagem será exibido.
        /// </summary>
        public Boolean ExibirImagem
        {
            get
            {
                if (ViewState["ExibirImagem"] == null)
                    ViewState["ExibirImagem"] = true;
                return (Boolean)ViewState["ExibirImagem"];
            }
            set { ViewState["ExibirImagem"] = value; }
        }

        /// <summary>
        /// Evento chamado quando a propriedade AutoPostBack é setada para true.
        /// </summary>
        public event EventHandler SelectedValuesChanged;

        #endregion

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public MultiSelectDropDown()
        {
            base.RepeatDirection = RepeatDirection.Vertical;
            base.RepeatLayout = RepeatLayout.Flow;
        }

        /// <summary>
        /// Sobrescreve o método para customizar a prérenderização do controle,
        /// com inclusão e registro de funções javascript.
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            //Registra o script do controle
            if (!Page.ClientScript.IsClientScriptIncludeRegistered("MultiSelectDropDownScript"))
            {
                Page.ClientScript.RegisterClientScriptInclude(this.GetType(),
                    "MultiSelectDropDownScript",
                    Page.ClientScript.GetWebResourceUrl(this.GetType(), "Redecard.PN.Comum.MultiSelectDropDown.MultiSelectDropDown.js"));
            }

            //Cria objeto javascript referente ao controle
            String initScript = "var {0} = MultiSelectDropDown.get('#{0}'); {0}.Init('{1}');";
            String postBackReference = (this.AutoPostBack && this.SelectedValuesChanged != null) ?
                this.Page.ClientScript.GetPostBackEventReference(this, String.Empty) : String.Empty;

            Page.ClientScript.RegisterStartupScript(this.GetType(),
                String.Concat("MultiSelectDropDownInit", this.ClientID),
                String.Format(initScript.ToString(), this.ClientID, postBackReference.Replace("'", "\"")), true);
        }

        /// <summary>
        /// Sobrescreve o método, para customizar a renderização dos items.
        /// </summary>
        protected override void RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo, HtmlTextWriter writer)
        {           
            writer.RenderBeginTag(HtmlTextWriterTag.Span);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, 
                String.Concat("item ", this.Css.Item.CssClass));
            writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "-1");
            writer.AddAttribute(HtmlTextWriterAttribute.Onchange, 
                String.Concat(this.ClientID, ".ItemChange(this, event);"));
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick, 
                String.Concat(this.ClientID, ".ItemClick(this, event);"));
            writer.AddAttribute("oldchecked", this.Items[repeatIndex].Selected ? "true" : "false");
            
            base.RenderItem(itemType, repeatIndex, repeatInfo, writer);

            writer.RenderEndTag();
        }

        /// <summary>
        /// Sobrescreve o método, customizando a renderização do controle
        /// </summary>
        protected override void Render(HtmlTextWriter writer)
        {
            //Renderização do container do controle
            writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, 
                String.Concat("multiSelectDropDown ", this.CssClass));
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            {
                //Renderização do TextBox e Imagem do controle
                writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
                writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
                writer.RenderBeginTag(HtmlTextWriterTag.Table);
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Td);
                        {
                            var cssClass = new StringBuilder("txtDescription caixaTexto ").Append(this.Css.TextBox.CssClass);
                            if (!this.Enabled) cssClass.Append(" disabled");
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass.ToString());
                            writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "-1");
                            writer.AddAttribute(HtmlTextWriterAttribute.ReadOnly, "readonly");
                            if (this.Enabled)
                                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, 
                                    String.Concat(this.ClientID, ".TxtDescriptionClick(this, event);"));
                            if(!this.Enabled)
                                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
                            writer.RenderBeginTag(HtmlTextWriterTag.Input);
                            writer.RenderEndTag();
                        } writer.RenderEndTag();

                        if (this.ExibirImagem)
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Td);
                            {
                                var imageClass = new StringBuilder("imgDropDown ").Append(this.Css.Image.CssClass);
                                if (this.Tipo.HasValue)
                                    imageClass.Append(" ").Append(this.Tipo.Value.GetDescription());
                                if (!this.Enabled) imageClass.Append(" disabled");
                                writer.AddAttribute(HtmlTextWriterAttribute.Class, imageClass.ToString());
                                if(this.Enabled)
                                    writer.AddAttribute(HtmlTextWriterAttribute.Onclick, 
                                        String.Concat(this.ClientID, ".ImageDropDownClick(this, event);"));
                                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                                writer.RenderEndTag();
                            } writer.RenderEndTag();
                        }
                    } writer.RenderEndTag();                            
                } writer.RenderEndTag();

                //Renderização do Box contendo as opções
                writer.AddAttribute(HtmlTextWriterAttribute.Class, String.Concat("divItems ", this.Css.ListBox.CssClass));
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                {
                    if (ExibirSelecionarTodos)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Span);
                        {
                            writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "-1");
                            writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
                            writer.AddAttribute(HtmlTextWriterAttribute.Onchange, String.Concat(this.ClientID, ".SelectAllChange(this, event);"));
                            writer.AddAttribute(HtmlTextWriterAttribute.Onclick, String.Concat(this.ClientID, ".SelectAllClick(this, event);"));
                            writer.AddAttribute(HtmlTextWriterAttribute.Id, String.Concat(this.ClientID, "_chkToggleAll"));
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, String.Concat("item chkToggleAll ", this.Css.Item.CssClass));
                            writer.RenderBeginTag(HtmlTextWriterTag.Input);
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.For, String.Concat(this.ClientID, "_chkToggleAll"));
                                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                                writer.Write(TextoSelecionarTodos);
                                writer.RenderEndTag();
                            }
                            writer.RenderEndTag();
                        } writer.RenderEndTag();
                        writer.Write("<br/>");
                    }

                    base.Render(writer);
                } writer.RenderEndTag();

                //Renderização dos Hiddens contendo as configurações do controle
                if (this.AtualizarDescricaoFuncaoJs.EmptyToNull() != null)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "hddUpdateDescriptionFunction");
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, this.AtualizarDescricaoFuncaoJs);
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                }

                if (this.AtualizarTituloFuncaoJs.EmptyToNull() != null)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "hddUpdateTitleFunction");
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, this.AtualizarTituloFuncaoJs);
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                }

                writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "hddEmptyText");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, this.TextoVazio);
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();

            } writer.RenderEndTag();
        }

        /// <summary>
        /// Implementação de IPostBackEventHandler
        /// </summary>
        public void RaisePostBackEvent(String eventArgument)
        {
            if (this.AutoPostBack && this.SelectedValuesChanged != null)
                this.SelectedValuesChanged(this, new EventArgs());
        }


        #region [ Classes/Enums/Structs Internas ]

        /// <summary>Classe para customização do CSS dos controles internos</summary>
        [Serializable]
        public struct CssStyle
        {
            /// <summary>Classe CSS do TextBox</summary>
            [PersistenceMode(PersistenceMode.InnerProperty)]
            public CssValue TextBox { get; set; }

            /// <summary>Classe CSS da Imagem</summary>
            [PersistenceMode(PersistenceMode.InnerProperty)]
            public CssValue Image { get; set; }

            /// <summary>Classe CSS do Box de opções</summary>
            [PersistenceMode(PersistenceMode.InnerProperty)]
            public CssValue ListBox { get; set; }

            /// <summary>Classe CSS de um Item/Opção</summary>
            [PersistenceMode(PersistenceMode.InnerProperty)]
            public CssValue Item { get; set; }
        }

        /// <summary>Classe auxiliar para configuração do CSS da imagem</summary>
        [Serializable]
        public struct CssImage
        {
            /// <summary>Tipo da imagem</summary>
            public CssImageType? Type { get; set; }

            /// <summary>Nome da classe CSS</summary>
            public String CssClass { get; set; }
        }

        /// <summary>Classe auxiliar para configuração do CSS de um controle interno</summary>
        [Serializable]
        public struct CssValue
        {
            /// <summary>
            /// Nome da classe
            /// </summary>
            public String CssClass { get; set; }
        }

        /// <summary>Tipo da Imagem que será utilizada</summary>
        [Serializable]
        public enum CssImageType
        {            
            /// <summary>Bandeiras</summary>
            [Description("bandeiras")]
            Bandeiras,

            /// <summary>Calendário</summary>
            [Description("calendario")]
            Calendario
        }

        #endregion
    }
}