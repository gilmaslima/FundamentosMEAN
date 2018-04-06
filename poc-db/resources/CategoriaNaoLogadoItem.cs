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
    /// Item de categoria da área não logada
    /// </summary>
    public class CategoriaNaoLogadoItem: ContentItem
    {
        /// <summary>
        /// Id da categoria
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Ícone da categoria
        /// </summary>
        public String Icone { get; set; }

        /// <summary>
        /// Subcategorias filhas da categoria
        /// </summary>
        public List<Int32> Subcategorias { get; set; }

        /// <summary>
        /// ItemType
        /// </summary>
        public override string ItemType { get { return "categoria"; } }
    }
}
