﻿using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Redecard.Portal.Helper.Paginacao;
using Microsoft.SharePoint.WebPartPages;

namespace Redecard.Portal.Aberto.WebParts.ListagemCartoes
{
    [ToolboxItemAttribute(false)]
    public class ListagemCartoes : System.Web.UI.WebControls.WebParts.WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/Redecard.Portal.Aberto.WebParts/ListagemCartoes/ListagemCartoesUserControl.ascx";

        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        private int quantidadeItensPorPagina = ParametrosGeraisPaginacao.QuantidadeItensPorPagina;
        private Redecard.Portal.Aberto.Model.TipoDoCartão tipoDoCartao = Model.TipoDoCartão.None;

        /// <summary>
        /// Dicas por página
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
        /// Tipo de Cartão
        /// </summary>
        [WebBrowsable(true)]
        [DisplayName("Tipo de Cartão")]
        [Description("Define o Tipo de Cartão")]
        [Personalizable(PersonalizationScope.Shared)]
        [Category("Configurações")]
        [FriendlyName("Tipo de Cartão")]
        public Redecard.Portal.Aberto.Model.TipoDoCartão TipoDoCartao
        {
            get { return this.tipoDoCartao; }
            set { this.tipoDoCartao = value; }
        }
    }
}
