/*
© Copyright 2016 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model.Result
{
    /// <summary>
    /// Classe representando um agrupamento do resultado da pesquisa
    /// </summary>
    public class ResultGroup
    {
        /// <summary>
        /// Items
        /// </summary>
        public List<ResultItem> Items { get; set; }

        /// <summary>
        /// TotalCount
        /// </summary>
        public Int32 TotalCount { get; set; }
    }
}