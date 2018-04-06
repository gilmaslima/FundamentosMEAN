/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content
{
    /// <summary>
    /// Item de conteúdo: Categoria
    /// </summary>
    public class CategoriaItem : ContentItem
    {
        /// <summary>
        /// Id da categoria
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Tipo do item
        /// </summary>
        public override String ItemType { get { return "categoria"; } }
    }
}