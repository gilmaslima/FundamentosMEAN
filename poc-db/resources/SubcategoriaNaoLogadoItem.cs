/*
© Copyright 2017 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content.Public
{
    /// <summary>
    /// Item da subcategoria
    /// </summary>
    public class SubcategoriaNaoLogadoItem: ContentItem
    {
        /// <summary>
        /// Id da subcategoria
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Id da categoria pai da subcategoria
        /// </summary>
        public Int32 Categoria { get; set; }

        /// <summary>
        /// Perguntas associadas À subcategoria
        /// </summary>
        public List<Int32> Perguntas { get; set; }

        /// <summary>
        /// ItemType
        /// </summary>
        public override string ItemType
        {
            get { return "subcategoria"; }
        }
    }
}
