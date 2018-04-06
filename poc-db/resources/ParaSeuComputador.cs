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

namespace Redecard.Portal.Aberto.WebParts.ParaSeuComputador
{
    [ToolboxItemAttribute(false)]
    public class ParaSeuComputador : System.Web.UI.WebControls.WebParts.WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/ParaSeuComputador/ParaSeuComputadorUserControl.ascx";
        private const string bibliotecaPadrao = "Papel de Parede";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        private string nomeBibliotecaExibicao = ParaSeuComputador.bibliotecaPadrao;
        private int quantidadeItensPorPagina = 6;
        private bool paginacaoHabilitada = ParametrosGeraisPaginacao.PaginacaoHabilitada;
        private string urlPaginaVisualizacao = string.Empty;

        /// <summary>
        /// Biblioteca de exibição
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Nome da biblioteca de exibição")]
        [Description("Define o nome da bibiloteca cujos itens serão listados")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Nome da biblioteca de exibição")]
        public string NomeBibliotecaExibicao
        {
            get { return this.nomeBibliotecaExibicao; }
            set { this.nomeBibliotecaExibicao = value; }
        }

        /// <summary>
        /// Fotos por página
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Quantidade de itens por página")]
        [Description("Define a quantidade de itens que deverão aparecer por página")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Quantidade de itens por página")]
        [DefaultValue(6)]
        public int QuantidadeItensPorPagina
        {
            get { return this.quantidadeItensPorPagina; }
            set { this.quantidadeItensPorPagina = value; }
        }

        /// <summary>
        /// Paginação habilitada
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Paginação de Itens de biblioteca")]
        [Description("Indica se haverá paginação para a WebPart")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Paginação de Itens de biblioteca")]
        public bool PaginacaoHabilitada
        {
            get { return this.paginacaoHabilitada; }
            set { this.paginacaoHabilitada = value; }
        }

        /// <summary>
        /// Endereço da página que listará todos os itens de uma determinada biblioteca
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Endereço da página que listará todos os itens de uma determinada biblioteca")]
        [Description("Endereço da página que listará todos os itens de uma determinada biblioteca")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Endereço da página que listará todos os itens de uma determinada biblioteca")]
        public string URLPaginaVisualizacao
        {
            get { return this.urlPaginaVisualizacao; }
            set { this.urlPaginaVisualizacao = value; }
        }
    }
}