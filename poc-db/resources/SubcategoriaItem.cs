/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content
{
    /// <summary>
    /// Item de conteúdo: Subcategoria
    /// </summary>
    public class SubcategoriaItem : ContentItem
    {
        /// <summary>
        /// Id da subcategoria
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Tipo do conteúdo
        /// </summary>
        public override String ItemType { get { return "subcategoria"; } }
    }
}