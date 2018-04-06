/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model
{
    /// <summary>
    /// Armazena metadados do termo que será pesquisado
    /// </summary>
    public class ProcessedTerm
    {
        /// <summary>
        /// Regex para encontrar a ocorrência da frase completa/exata
        /// </summary>
        [ScriptIgnoreAttribute]
        public Regex RegexExactMatch { get; set; }
        
        /// <summary>
        /// Regex para encontrar a ocorrência de palavras completas/exatas
        /// </summary>
        [ScriptIgnoreAttribute]
        public Regex RegexWordMatch { get; set; }

        /// <summary>
        /// Regex para encontrar a ocorrência parcial em palavras
        /// </summary>
        [ScriptIgnoreAttribute]
        public Regex RegexPartialMatch { get; set; }

        /// <summary>
        /// Termo formatado, sem espaços extras, lowercase
        /// </summary>
        public String Formatted { get; set; }

        /// <summary>
        /// Termo original pesquisado
        /// </summary>
        public String Original { get; set; }

        /// <summary>
        /// Termo corrigido
        /// </summary>
        public String Corrected { get; set; }

        /// <summary>
        /// Termo processado
        /// </summary>
        public String Processed { get; set; }

        /// <summary>
        /// Se termo é válido e deve ser utilizado para efetuar a pesquisa nos itens
        /// </summary>
        public Boolean IsValid { get; set; } 

        /// <summary>
        /// Palavras consideradas
        /// </summary>
        public List<String> Words { get; set; }

        /// <summary>
        /// Palavras ignoradas
        /// </summary>
        public List<String> IgnoredWords { get; set; }

        /// <summary>
        /// Correções aplicadas
        /// </summary>
        public Dictionary<String, String> Corrections { get; set; }

        /// <summary>
        /// Sinônimos encontrados
        /// </summary>
        public Dictionary<String, List<String>> Synonyms { get; set; }
    }
}