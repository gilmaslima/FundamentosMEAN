using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Redecard.Portal.Aberto.Model;
using Redecard.Portal.Aberto.Model.Repository;
using Redecard.Portal.Aberto.Model.Repository.DTOs;
using Redecard.Portal.Helper;
using Microsoft.SharePoint;

namespace Redecard.Portal.Aberto.WebParts.RedecardClick {

    /// <summary>
    /// Web part &quot;Redecard em um Clique&quot;. Exibe uma lista de links rápidos do Portal Redecard
    /// </summary>
    [ToolboxItemAttribute(false)]
    public class RedecardClick : WebPart {

        /// <summary>
        /// Chave para verificar se o código javascript já está renderizado na tela
        /// </summary>
        string s_JScriptKey = "__redecardClickChangedKey";

        /// <summary>
        /// Código JScript para redirecionar o usuário para o item selecionado na web part &quot;Redecard em um Clique&quot;
        /// </summary>
        string s_JScriptCode = "function __redecardClickChanged(sender) { var sUrlSelected = sender.options[sender.selectedIndex].value; window.location = sUrlSelected; }";

        // Exemplo de renderização da web part
        // ------------------------------------------
        //<div class="redecardClick">
        //    <div>
        //        <fieldset>
        //            <label for="slcPerformance">Redecard em um Clique</label>
        //            <select name="" id="slcPerformance" title="Selecione sua atuação">
        //                <option value="">Selecione sua atuação</option>
        //                <option value="Opção 1">Opção 1</option>
        //                <option value="Opção 2">Opção 2</option>
        //            </select>
        //        </fieldset>
        //    </div>
        //</div>

        /// <summary>
        /// Início da renderização, neste bloco decem ser renderizados scripts/estilos adicionais do controle
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(System.EventArgs e) {
            ClientScriptManager csManager = this.Page.ClientScript;
            if (!object.ReferenceEquals(csManager, null)) {
                // verificar se o código javascript já esta renderizado na tela
                if (!csManager.IsClientScriptBlockRegistered(s_JScriptKey)) {
                    csManager.RegisterClientScriptBlock(this.GetType(), s_JScriptKey, s_JScriptCode, true);
                }
            }
        }

        /// <summary>
        /// Inicia renderização da web part
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderBeginTag(HtmlTextWriter writer) {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "redecardClick");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.Fieldset);
            // renderizar o label
            this.RenderLabel(writer);
            // renderizar controle e items de links rápidos
            this.RenderSelect(writer);
        }

        /// <summary>
        /// Renderização do controle e das opções de links rápidos
        /// </summary>
        /// <param name="writer"></param>
        private void RenderSelect(HtmlTextWriter writer) {
            writer.AddAttribute(HtmlTextWriterAttribute.Name, string.Empty);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "slcPerformance");
            writer.AddAttribute(HtmlTextWriterAttribute.Onchange, "javascript: __redecardClickChanged(this);");
            writer.AddAttribute(HtmlTextWriterAttribute.Title, RedecardHelper.ObterResource("oquevoceprocura"));
            writer.RenderBeginTag(HtmlTextWriterTag.Select);
            // renderizar opção padrão
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "#");
            writer.AddAttribute(HtmlTextWriterAttribute.Selected, "TRUE");
            writer.RenderBeginTag(HtmlTextWriterTag.Option);
            writer.Write(RedecardHelper.ObterResource("oquevoceprocura"));
            writer.RenderEndTag();
            // renderizar opções
            this.RenderLinks(writer);
        }

        /// <summary>
        /// Renderizar opções do controle de links rápidos
        /// </summary>
        /// <param name="writer"></param>
        private void RenderLinks(HtmlTextWriter writer) {
            IList<DTORedecardClique> items = this.GetLinks();
            if (!object.ReferenceEquals(items, null) && items.Count > 0) {
                foreach (DTORedecardClique cliqueItem in items) {
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, cliqueItem.Url);
                    writer.RenderBeginTag(HtmlTextWriterTag.Option);
                    writer.Write(cliqueItem.Titulo);
                    writer.RenderEndTag();
                }
            }
        }

        /// <summary>
        /// Renderização do label da web part
        /// </summary>
        /// <param name="writer"></param>
        private void RenderLabel(HtmlTextWriter writer) {
            writer.AddAttribute(HtmlTextWriterAttribute.For, "slcPerformance");
            writer.RenderBeginTag(HtmlTextWriterTag.Label);
            writer.Write(RedecardHelper.ObterResource("redecardemumclique"));
            writer.RenderEndTag();
        }

        /// <summary>
        /// Finaliza a renderização do controle, fecha as tags do contexto
        /// </summary>
        /// <param name="writer"></param>
        public override void RenderEndTag(HtmlTextWriter writer) {
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
            writer.RenderEndTag();
        }

        /// <summary>
        /// Retorna os links configurados na lista &quot;Redecard em um Clique&quot;
        /// </summary>
        protected IList<DTORedecardClique> GetLinks() {
            IList<DTORedecardClique> items = null;
            using (var repository = SharePointServiceLocator.GetCurrent().GetInstance<IRepository<DTORedecardClique, RedecardEmUmCliqueItem>>()) {
                AnonymousContextSwitch.RunWithElevatedPrivilegesAndContextSwitch(
                delegate {
                    items = (from item in repository.GetAllItems()
                             select item).OrderBy(g => g.Titulo).Distinct().ToList();
                });
            }
            // verificar se houve retorno da função
            if (!object.ReferenceEquals(items, null)) {
                return items;
            }
            return null;
        }
    }
}
