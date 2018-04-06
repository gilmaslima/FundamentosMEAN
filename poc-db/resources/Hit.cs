/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model
{
    /// <summary>
    /// Classe para agrupar os diversos tipo de ocorrências encontradas em um conteúdo
    /// para determinado termo pesquisado
    /// </summary>
    public class Hit
    {
        /// <summary>
        /// Ocorrências do termo/frase completo/exato pesquisado
        /// </summary>
        public Int32? ExactMatch { get; set; }

        /// <summary>
        /// Ocorrências de palavras exatas do termo pesquisado
        /// </summary>
        public Int32? Match { get; set; }

        /// <summary>
        /// Ocorrências do termo pesquisado encontrada parcialmente nas palavras
        /// </summary>
        public Int32? PartialMatch { get; set; }

        /// <summary>
        /// Não ocorrência das palavras
        /// </summary>
        public Int32? NoMatch { get; set; }
    }
}