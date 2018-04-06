using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.Portal.Helper.Paginacao;
using Microsoft.SharePoint.WebPartPages;

namespace Redecard.Portal.Aberto.WebParts.ListagemReleases
{
    [ToolboxItemAttribute(false)]
    public class ListagemReleases : System.Web.UI.WebControls.WebParts.WebPart
    {
        public enum Visao : int
        {
            Resumo = 0,
            Detalhe = 1,            
        }

        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/ListagemReleases/ListagemReleasesUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        private int quantidadeItensPorPagina = ParametrosGeraisPaginacao.QuantidadeItensPorPagina;

        /// <summary>
        /// Fotos por página
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


        [WebBrowsable(true)]
        [WebDisplayName("Tipo de visão")]
        [Description("Define o modo de exibição da lista de Releases")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [DefaultValue(Visao.Resumo)]
        public Visao tipoVisao
        {
            get;
            set;
        }
    }
}
