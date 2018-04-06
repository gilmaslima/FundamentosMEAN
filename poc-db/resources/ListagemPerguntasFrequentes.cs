using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using Redecard.Portal.Helper.Paginacao;

namespace Redecard.Portal.Aberto.WebParts.ListagemPerguntasFrequentes
{
    [ToolboxItemAttribute(false)]
    public class ListagemPerguntasFrequentes : System.Web.UI.WebControls.WebParts.WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/ListagemPerguntasFrequentes/ListagemPerguntasFrequentesUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        private int quantidadeItensPorPagina = ParametrosGeraisPaginacao.QuantidadeItensPorPagina;
        private int quantidadeItensPorPaginaModoVerTodas = ParametrosGeraisPaginacao.QuantidadeItensPorPaginaModoVerTodas;
        private bool paginacaoHabilitada = ParametrosGeraisPaginacao.PaginacaoHabilitada;

        /// <summary>
        /// Quantidade de itens por página
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Quantidade de itens por página")]
        [Description("Define a quantidade de itens que deverão aparecer por página")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Quantidade de itens por página")]
        public int QuantidadeItensPorPagina
        {
            get { return this.quantidadeItensPorPagina; }
            set { this.quantidadeItensPorPagina = value; }
        }

        /// <summary>
        /// Quantidade de itens por página em modo ver todas
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Quantidade de itens por página - Modo de visualização Ver Todas")]
        [Description("Define a quantidade de itens que deverão aparecer por página para quando o botão Ver Todas é acionado")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Quantidade de itens por página - Modo de visualização Ver Todas")]
        public int QuantidadeItensPorPaginaModoVerTodas
        {
            get { return this.quantidadeItensPorPaginaModoVerTodas; }
            set { this.quantidadeItensPorPaginaModoVerTodas = value; }
        }

        /// <summary>
        /// Paginação habilitada
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Paginação de Perguntas Freqüentes habilitada")]
        [Description("Indica se haverá paginação para a WebPart")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Paginação de Perguntas Freqüentes habilitada")]
        public bool PaginacaoHabilitada
        {
            get { return this.paginacaoHabilitada; }
            set { this.paginacaoHabilitada = value; }
        }
    }
}