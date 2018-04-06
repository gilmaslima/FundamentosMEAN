using System;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace Redecard.Portal.Aberto.WebParts.RedeCardEmUmClique {

    /// <summary>
    /// Formulário de edição customizado o XML de dados da web part &quot;RedeCard em um Clique&quot;
    /// </summary>
    public class RedeCardEmUmCliqueToolPart : ToolPart {

        /// <summary>
        /// Link de chamada do editor do XML
        /// </summary>
        protected HyperLink _callEditorLink;

        /// <summary>
        /// Hidden field que armazena o XML de retorno do controle de edição
        /// </summary>
        protected HiddenField _hdfLink;

        /// <summary>
        /// Container do link e do hidden field de edição
        /// </summary>
        protected Panel _pnlContainer;

        /// <summary>
        /// Código javascript de chamada do editor de XML da web part
        /// </summary>
        const string _jScriptCallEditor = "javascript:rc_emumclique_callEditor('{0}');";

        /// <summary>
        /// Adiciona os controles de chamada da página de edição
        /// </summary>
        protected override void CreateChildControls() {
            // limpar controles
            this.Controls.Clear();
            // adicionar container
            _pnlContainer = new Panel();
            _pnlContainer.Style.Add(System.Web.UI.HtmlTextWriterStyle.PaddingBottom, "10px");
            this.Controls.Add(_pnlContainer);
            this.AddControls();
        }

        /// <summary>
        /// Adicionar controles para edição do XML
        /// </summary>
        private void AddControls() {
            // adicionar controles para chamada do formulário de edição
            _hdfLink = new HiddenField();
            _hdfLink.ID = Guid.NewGuid().ToString("N"); // gerar id dinâmico
            _pnlContainer.Controls.Add(_hdfLink);
            _callEditorLink = new HyperLink();
            _callEditorLink.Text = "Clique aqui para editar";
            _callEditorLink.NavigateUrl = String.Format(_jScriptCallEditor, _hdfLink.ClientID);
            _pnlContainer.Controls.Add(_callEditorLink);
        }

        /// <summary>
        /// Atualiza o XML na web part
        /// </summary>
        public override void ApplyChanges() {
            ((RedeCardEmUmClique)this.ParentToolPane.SelectedWebPart).Source = _hdfLink.Value;
        }
    }
}
