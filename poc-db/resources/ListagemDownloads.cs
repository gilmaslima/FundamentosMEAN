using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint.WebPartPages;
using Redecard.Portal.Helper.Paginacao;

namespace Redecard.Portal.Aberto.WebParts.ListagemDownloads
{
    [ToolboxItemAttribute(false)]
    public class ListagemDownloads : System.Web.UI.WebControls.WebParts.WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/ListagemDownloads/ListagemDownloadsUserControl.ascx";

        /// <summary>
        /// 
        /// </summary>
        private int quantidadeItensPorPagina = ParametrosGeraisPaginacao.QuantidadeItensPorPagina;

        private string mensagemNoItems = "Não foram encontrados items referentes a pesquisa.";

        private string mensagemHasItems = "Downloads disponíveis: [0]";

        /// <summary>
        /// Dicas por página
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Quantidade de itens por página")]
        [Description("Define a quantidade de itens que deverão aparecer por página")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Quantidade de itens por página")]
        public int QuantidadeItensPorPagina {
            get { return this.quantidadeItensPorPagina; }
            set { this.quantidadeItensPorPagina = value; }
        }

        /// <summary>
        /// Dicas por página
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Mensagem quando não existem items na lista")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Mensagem quando não existem items na lista")]
        public string MensagemNoitems {
            get { return this.mensagemNoItems; }
            set { this.mensagemNoItems = value; }
        }

        /// <summary>
        /// Dicas por página
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Mensagem apresentado quando a lista possui items")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Mensagem apresentado quando a lista possui items")]
        public string MensagemHasitems {
            get { return this.mensagemHasItems; }
            set { this.mensagemHasItems = value; }
        }

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
