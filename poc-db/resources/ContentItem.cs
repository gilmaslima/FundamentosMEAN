/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content
{
    /// <summary>
    /// Item de conteúdo base
    /// </summary>
    [Serializable]
    public abstract class ContentItem
    {
        /// <summary>
        /// Título do conteúdo
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Descrição do conteúdo
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Tipo do item: deve ser sobrescrito pelas classes filhas
        /// </summary>
        public abstract String ItemType { get; }
    }
}