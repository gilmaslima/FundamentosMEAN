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

namespace Rede.PN.AtendimentoDigital.SharePoint.Core.Search.Model
{
    /// <summary>
    /// Classe de Paginação dos resultados
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// Quantidade de itens retornados
        /// </summary>
        public Int32? RowsPerPage { get; set; }

        /// <summary>
        /// Índice do item inicial
        /// </summary>
        public Int32? StartRow { get; set; }
        
        /// <summary>
        /// Construtor padrão
        /// </summary>
        public Pagination()
        {
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="rowsPerPage">Items por página</param>
        /// <param name="startRow">Índice do item inicial</param>
        public Pagination(Int32 rowsPerPage, Int32 startRow) : this()
        {
            this.RowsPerPage = rowsPerPage;
            this.StartRow = startRow;
        }        
    }
}