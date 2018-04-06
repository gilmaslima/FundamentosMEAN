/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Content
{
    /// <summary>
    /// Item de conteúdo: Serviço
    /// </summary>
    public class ServicoItem : ContentItem
    {
        /// <summary>
        /// Url do serviço
        /// </summary>
        public String Url { get; set; }

        /// <summary>
        /// Tipo do conteúdo
        /// </summary>
        public override String ItemType { get { return "servico"; } }

        /// <summary>
        /// Código do menu
        /// </summary>
        public Int32 Codigo { get; set; }

        /// <summary>
        /// Hierarquia de Menus
        /// </summary>
        public List<String> NiveisMenu  { get; set; }

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ServicoItem()
        {
            this.NiveisMenu = new List<String>();
        }
    }
}