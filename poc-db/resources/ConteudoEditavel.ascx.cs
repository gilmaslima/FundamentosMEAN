/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using WebPart = Microsoft.SharePoint.WebPartPages.WebPart;

namespace Redecard.PN.Extrato.SharePoint.ControlTemplates.HomePage
{
    /// <summary>li
    /// </summary>
    [ParseChildren(true, "ConteudoInicial")]
    public partial class ConteudoEditavel : BaseUserControl
    {
        #region [ Controles ]

        /// <summary>
        /// pnlDefaultContent control.
        /// </summary>
        private Panel pnlDefaultContent;

        /// <summary>
        /// pnlDefaultContent control.
        /// </summary>
        public Panel PnlDefaultContent
        {
            get 
            {
                if (pnlDefaultContent == null)
                    pnlDefaultContent = new Panel();
                return pnlDefaultContent;
            }
        }

        /// <summary>
        /// editableRegion control.
        /// </summary>
        private HtmlGenericControl editableRegion;
        
        /// <summary>
        /// editableRegion control.
        /// </summary>
        private HtmlGenericControl EditableRegion
        {
            get
            {
                if (editableRegion == null)
                    editableRegion = new HtmlGenericControl();
                return editableRegion;
            }
        }

        #endregion

        #region [ Delegates - Definição ]

        /// <summary>
        /// Delegate para recuperação do conteúdo do controle.
        /// </summary>
        /// <param name="webPart">WebPart</param>
        /// <returns>String do conteúdo</returns>
        public delegate String GetWebPartPropertyContentEventHandler(WebPart webPart);

        /// <summary>
        /// Delegate para atribuição do conteúdo do controle.
        /// </summary>
        /// <param name="webPart">WebPart</param>
        /// <param name="content">Conteúdo</param>
        public delegate void SetWebPartPropertyContentEventHandler(WebPart webPart, String content);

        #endregion

        #region [ Propriedades ]

        /// <summary>
        /// Evento para obtenção do conteúdo editável do controle.
        /// Deve ser utilizado em conjunto com o evento SetContent.
        /// A propriedade WebPartProperty possui prioridade.
        /// </summary>
        public event GetWebPartPropertyContentEventHandler GetContent;        

        /// <summary>
        /// Evento para atribuição do conteúdo editável do controle.
        /// Deve ser utilizado em conjunto com o evento GetContent.
        /// A propriedade WebPartProperty possui prioridade.
        /// </summary>
        public event SetWebPartPropertyContentEventHandler SetContent;

        /// <summary>
        /// Propriedade da webpart que será utilizada para recuperar/salvar o conteúdo do controle.
        /// A utilização desta propriedade inutiliza os eventos GetContent e SetContent.
        /// </summary>
        public String WebPartProperty { get; set; }
        
        /// <summary>
        /// Conteúdo do controle. Verifica a propriedade WebPartProperty e,
        /// caso não esteja definida, invoca os métodos GetContent e SetContent.
        /// </summary>
        public String Conteudo
        {
            get 
            {
                String conteudo = default(String);

                //Lê o conteúdo da propriedade da WebPart, se propriedade WebPartProperty foi definida
                if (!String.IsNullOrEmpty(this.WebPartProperty))
                {
                    Type type = this.WP.GetType();
                    PropertyInfo property = type.GetProperty(this.WebPartProperty);
                    if (property != null)
                        conteudo = (String)property.GetValue(this.WP, null);
                    else
                    {
                        var msg = "A propriedade \"{0}\" especificada em \"WebPartProperty\" não foi encontrada na WebPart.";
                        throw new NotImplementedException(String.Format(msg, this.WebPartProperty));
                    }
                }
                //Lê o conteúdo do método de obtenção de conteúdo, se método GetContent foi implementado
                else if (GetContent != null)
                    conteudo = GetContent(this.WP);
                else
                {
                    var msg = new StringBuilder()
                        .Append("Deve ser especificada a propriedade \"WebPartProperty\" ou ")
                        .Append("o método \"GetContent\" deve ser implementado.");
                    throw new NotImplementedException(msg.ToString());
                }

                //Se não existe conteúdo definido, recupera o conteúdo default
                if (String.IsNullOrEmpty(conteudo) && this.UtilizarConteudoInicial)
                {
                    using (var stringWrite = new StringWriter())
                    {
                        using (var htmlWrite = new HtmlTextWriter(stringWrite))
                            foreach(Control controle in PnlDefaultContent.Controls)
                                controle.RenderControl(htmlWrite);
                        conteudo = stringWrite.ToString();
                    }                    
                }

                return conteudo;
            }
            set 
            {
                if (!String.IsNullOrEmpty(this.WebPartProperty))
                {
                    Type type = this.WP.GetType();
                    type.GetProperty(this.WebPartProperty).SetValue(this.WP, value, null);
                }
                else if (SetContent != null)
                    SetContent(this.WP, value);
                else
                {
                    var msg = new StringBuilder()
                        .Append("Deve ser especificada a propriedade \"WebPartProperty\" ou ")
                        .Append("o método \"SetContent\" deve ser implementado.");
                    throw new NotImplementedException(msg.ToString());
                }
            }
        }
                
        /// <summary>
        /// Recupera a WebPart em que o controle está inserido.
        /// </summary>
        private WebPart WP
        {
            get
            {
                Control parent = this.Parent;
                while (parent != null && !(parent is WebPart))
                    parent = parent.Parent;
                return parent as WebPart;
            }
        }

        /// <summary>
        /// Verifica se a WebPart está em modo de edição.
        /// </summary>
        public Boolean IsInEditMode
        {
            get
            {
                var currentWebPartManager = (SPWebPartManager)WebPartManager.GetCurrentWebPartManager(this.Page);
                return currentWebPartManager != null && currentWebPartManager.GetDisplayMode().AllowPageDesign;
            }
        }

        /// <summary>
        /// Container para o conteúdo default
        /// </summary>
        public Panel ConteudoInicial
        {
            get
            {
                return PnlDefaultContent;
            }
        }

        /// <summary>
        /// Flag indicando se deve utilizar ou não o conteúdo default
        /// </summary>
        public Boolean UtilizarConteudoInicial { get; set; }

        #endregion

        #region [ Métodos Sobrescritos ]

        /// <summary>
        /// OnInit
        /// </summary>
        protected override void OnInit(EventArgs e)
        {            
            base.OnInit(e);
            if (this.IsInEditMode)
            {
                //Habilita ribbons de edição
                SPRibbon current = SPRibbon.GetCurrent(this.Page);
                if (current != null)
                {
                    current.MakeTabAvailable("Ribbon.EditingTools.CPEditTab");
                    current.MakeTabAvailable("Ribbon.Image.Image");
                    current.MakeTabAvailable("Ribbon.EditingTools.CPInsert");
                    current.MakeTabAvailable("Ribbon.Link.Link");
                    current.MakeTabAvailable("Ribbon.Table.Layout");
                    current.MakeTabAvailable("Ribbon.Table.Design");
                    if (!(this.Page is WikiEditPage))
                    {
                        current.TrimById("Ribbon.EditingTools.CPEditTab.Layout");
                        current.TrimById("Ribbon.EditingTools.CPEditTab.EditAndCheckout");
                    }
                }
            }
        }

        /// <summary>
        /// OnLoad.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            //Não precisa validar userControl
            ValidarPermissao = false;

            ConteudoInicial.Visible = true;

            //Adiciona controle de conteúdo
            this.pnlContentDisplay.Controls.Add(new LiteralControl(this.Conteudo));
            
            if (this.IsInEditMode)
            {
                //Adiciona controles de conteúdo editável
                this.pnlContentEdit.Controls.Add(this.EditableRegion);
                
                String contentId = String.Concat(this.ClientID, "content");
                String strUpdatedContent = this.Page.Request.Form[contentId];
                if ((strUpdatedContent != null) && (String.Compare(this.Conteudo, strUpdatedContent, false) != 0))
                {
                    this.Conteudo = strUpdatedContent;
                    var currentWebPartManager = (SPWebPartManager)WebPartManager.GetCurrentWebPartManager(this.Page);
                    Guid storageKey = currentWebPartManager.GetStorageKey(this.WP);
                    currentWebPartManager.SaveChanges(storageKey);
                }

                if (String.IsNullOrEmpty(this.Conteudo))
                    this.Page.ClientScript.RegisterHiddenField(contentId, "[Digite o texto aqui]");
                else
                    this.Page.ClientScript.RegisterHiddenField(contentId, this.Conteudo);

                this.pnlContentDisplay.Visible = false;

                base.Attributes["RteRedirect"] = this.EditableRegion.ClientID;
                ScriptLink.RegisterScriptAfterUI(this.Page, "SP.UI.Rte.js", false);
                ScriptLink.RegisterScriptAfterUI(this.Page, "SP.js", false);
                ScriptLink.RegisterScriptAfterUI(this.Page, "SP.Runtime.js", false);
                this.EditableRegion.TagName = "DIV";
                this.EditableRegion.Style.Add(HtmlTextWriterStyle.BorderStyle, "dashed");
                this.EditableRegion.Style.Add(HtmlTextWriterStyle.BorderWidth, "1px");
                this.EditableRegion.Style.Add(HtmlTextWriterStyle.BorderColor, "lightblue");
                this.EditableRegion.Style.Add("min-height", "14px");
                this.EditableRegion.InnerHtml = this.Conteudo;
                this.EditableRegion.Attributes["class"] = "ms-rtestate-write ms-rtestate-field";
                this.EditableRegion.Attributes["contentEditable"] = "true";
                this.EditableRegion.Attributes["InputFieldId"] = this.ClientID + "content";
                this.EditableRegion.Attributes["ContentEditor"] = "True";
                this.EditableRegion.Attributes["AllowScripts"] = "True";
                this.EditableRegion.Attributes["AllowWebParts"] = "False";
                String script = String.Format("RTE.RichTextEditor.transferContentsToInputField('{0}');",
                    SPHttpUtility.EcmaScriptStringLiteralEncode(this.EditableRegion.ClientID));
                String scriptId = String.Concat("transfer", this.EditableRegion.ClientID);
                this.Page.ClientScript.RegisterOnSubmitStatement(base.GetType(), scriptId, script);
            }
        }

        #endregion       
    }
}