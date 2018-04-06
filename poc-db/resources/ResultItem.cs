/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Result
{
    /// <summary>
    /// Classe representando um item de resultado
    /// </summary>
    public class ResultItem
    {
        /// <summary>
        /// Ocorrências do termo encontradas no título
        /// </summary>
        public Hit TitleHits { get; set; }

        /// <summary>
        /// Ocorrências do termo encontradas na descrição
        /// </summary>
        public Hit DescriptionHits { get; set; }

        /// <summary>
        /// Conteúdo associado ao resultado
        /// </summary>
        public ContentItem Content { get; set; }

        /// <summary>
        /// Rank do conteúdo
        /// </summary>
        public Int32 Rank { get; set; }

        /// <summary>
        /// Prioridade do conteúdo (utilizado atualmente para os cards)
        /// </summary>
        public Int32 Priority { get; set; }
    }
}