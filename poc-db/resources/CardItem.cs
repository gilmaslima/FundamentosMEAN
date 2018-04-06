/*
© Copyright 2017 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content
{
    /// <summary>
    /// Item do tipo card
    /// </summary>
    public class CardItem : ContentItem
    {
        /// <summary>
        /// Tipo do conteúdo
        /// </summary>
        public override String ItemType { get { return "card"; } }

        /// <summary>
        /// Tipo do card
        /// </summary>
        public String CardType { get; set; }

        /// <summary>
        /// Subcards do card
        /// </summary>
        public List<String> Subcards { get; set; }

        /// <summary>
        /// Prioridade do card
        /// </summary>
        public Int32 Priority { get; set; }

        #region [ Construtor ]

        /// <summary>
        /// Construtor default
        /// </summary>
        public CardItem()
        {
            this.Subcards = new List<String>();
        }

        #endregion
    }
}