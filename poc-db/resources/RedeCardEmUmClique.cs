using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Web.UI;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;

namespace Redecard.Portal.Aberto.WebParts.RedeCardEmUmClique {

    /// <summary>
    /// Web part &quot;RedeCard em um Clique&quot; exibe uma série de links rápidos para
    /// o usuário em um controle de seleção
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class RedeCardEmUmClique : Microsoft.SharePoint.WebPartPages.WebPart {

        /* EXEMPLO DE RENDERIZAÇÃO DE HTML */
        //<div class="redecardClick">
        //    <fieldset>
        //        <label for="slcPerformance">Redecard em um Clique</label>
        //        <select name="" id="slcPerformance" title="Selecione sua atuação">
        //            <option value="">Selecione sua atuação</option>
        //            <option value="Opção 1">Opção 1</option>
        //            <option value="Opção 2">Opção 2</option>
        //        </select>
        //    </fieldset>
        //</div>

        /// <summary>
        /// Chave para identificar a inclusão do código javascript na página
        /// </summary>
        const string _jScriptKey = "_jscript_rc_emumclique_webpart";

        /// <summary>
        /// URL do código javascript da web part
        /// </summary>
        const string _jScriptUrl = " _layouts/RedeCard/WebParts/RedeCardEmUmClique/script.js";

        /// <summary>
        /// Variável que armazena o xml (origem) dos dados de links rápidos
        /// </summary>
        string _xmlItems = string.Empty;

        /// <summary>
        /// Identifica o carregamento dos dados de XML nas definições globais
        /// </summary>
        bool _isXmlDataLoaded = false;

        /// <summary>
        /// Armazena o XML (origem) que define os items de links rápidos exibidos no controle
        /// </summary>
        public string Source {
            get {
                return _xmlItems;
            }
            set {
                _xmlItems = value;
            }
        }

        /// <summary>
        /// Carregar dados das definições globais
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(System.EventArgs e) {
            if (!_isXmlDataLoaded) {
                // carrega os dados
                string sCurrentSiteUrl = SPContext.Current.Web.Url;
                SPFile globalXMlData = SPContext.Current.Web.GetFile(sCurrentSiteUrl + "/RedeCardEmUmClique.xml");
                if (globalXMlData.Exists) {
                    byte[] binaryData = globalXMlData.OpenBinary();
                    MemoryStream msData = new MemoryStream(binaryData);
                    if (msData.Length > 0) {
                        XmlDocument document = new XmlDocument();
                        document.Load(msData);
                        this.Source = document.OuterXml;
                        _isXmlDataLoaded = true;
                    }
                }
            }
            base.OnLoad(e);
        }

        /// <summary>
        /// Vincular código javascript de edição da webpart com a página
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(System.EventArgs e) {
            ClientScriptManager scriptMng = this.Page.ClientScript;
            if (!object.ReferenceEquals(scriptMng, null)) {
                // verificar se já esta registrado
                if (!scriptMng.IsClientScriptIncludeRegistered(_jScriptKey)) {
                    // registrar script
                    scriptMng.RegisterClientScriptInclude(this.GetType(), _jScriptKey, _jScriptUrl);
                }
            }
        }

        /// <summary>
        /// Renderização do código HTML do controle
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderBeginTag(HtmlTextWriter writer) {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "redecardClick");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.Fieldset);
            writer.AddAttribute(HtmlTextWriterAttribute.For, "slcPerformance");
            writer.RenderBeginTag(HtmlTextWriterTag.Label);
            writer.Write("Redecard em um Clique");
            writer.RenderEndTag();
            // renderizar select de opções
            this.RenderSelect(writer);
        }

        /// <summary>
        /// Renderiza as opções de seleção do usuário
        /// </summary>
        /// <param name="writer"></param>
        void RenderSelect(HtmlTextWriter writer) {
            writer.AddAttribute(HtmlTextWriterAttribute.Name, "");
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "slcPerformance");
            writer.AddAttribute(HtmlTextWriterAttribute.Title, "Selecione sua atuação");
            writer.AddAttribute(HtmlTextWriterAttribute.Onchange, "javascript:rc_emumclique_changeOption(this);");
            writer.RenderBeginTag(HtmlTextWriterTag.Select);
            // renderizar opção padrão
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "#");
            writer.RenderBeginTag(HtmlTextWriterTag.Option);
            writer.Write("Selecione sua atuação");
            writer.RenderEndTag();
            // inicia a renderização das opções
            this.RenderOptions(writer);
        }

        /// <summary>
        /// Renderiza as opções do controle de seleção
        /// </summary>
        /// <param name="writer"></param>
        void RenderOptions(HtmlTextWriter writer) {
            if (!string.IsNullOrEmpty(this.Source)) {
                RedecardEmUmCliqueContainer container = RedecardEmUmCliqueContainer.Deserialize(this.Source);
                List<RedecardEmUmCliqueItem> items = new List<RedecardEmUmCliqueItem>();
                items.AddRange(container.Items);
                // executa o comando de ordenaçaõ
                items.Sort();
                for (int i = 0; i < items.Count; i++) {
                    RedecardEmUmCliqueItem item = items[i];
                    // renderiza um item
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, item.Endereco);
                    writer.RenderBeginTag(HtmlTextWriterTag.Option);
                    writer.Write(item.Titulo);
                    writer.RenderEndTag();
                }
            }
        }

        /// <summary>
        /// Final da renderização
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderEndTag(HtmlTextWriter writer) {
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        /// <summary>
        /// Recupera o formulário customizado para edição do XML de dados da Web Part
        /// </summary>
        /// <returns></returns>
        public override Microsoft.SharePoint.WebPartPages.ToolPart[] GetToolParts() {
            ToolPart[] allToolParts = new ToolPart[3];
            WebPartToolPart standardToolParts = new WebPartToolPart();
            CustomPropertyToolPart customToolParts = new CustomPropertyToolPart();
            RedeCardEmUmCliqueToolPart redecardToolParts = new RedeCardEmUmCliqueToolPart();
            redecardToolParts.Title = "Editor";
            allToolParts[0] = standardToolParts;
            allToolParts[1] = customToolParts;
            allToolParts[2] = redecardToolParts;
            return allToolParts;
        }
    }
}