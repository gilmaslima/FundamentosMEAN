/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Result
{
    /// <summary>
    /// Classe representando o resultado da pesquisa
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Dicionário de resultados, separado por escopo
        /// </summary>
        public Dictionary<String, ResultGroup> Results { get; set; }

        /// <summary>
        /// Termo processado
        /// </summary>
        public ProcessedTerm Term { get; set; }

        /// <summary>
        /// Quantidade total de resultados encontrados
        /// </summary>
        public Int32 TotalCount
        {
            get
            {
                Int32 totalCount = 0;

                if (this.Results != null)
                {
                    foreach (ResultGroup results in this.Results.Values)
                        totalCount += (results != null) ? results.TotalCount : 0;
                }

                return totalCount;
            }
        }
    }
}