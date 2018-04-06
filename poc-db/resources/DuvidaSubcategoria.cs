/*
© Copyright 2016 Rede S.A.
Autor : EMA
Empresa : Iteris Consultoria e Software
*/

using Microsoft.SharePoint;
using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// A class DuvidaSubcategoria e uma entidade da lista Dúvida subcategoria.
    /// </summary>
    public class DuvidaSubcategoria : BaseEntity
    {
        /// <summary>
        /// Gets or Sets a Categoria da DuvidaSubcategoria.
        /// </summary>
        public SPFieldLookupValue Categoria { get; set; }

        /// <summary>
        /// Gets or Sets a Ordem da DuvidaSubcategoria.
        /// </summary>
        public Int32 Ordem { get; set; }

        /// <summary>
        /// Gets or Sets a ExibeMenu da DuvidaSubcategoria.
        /// </summary>
        public Boolean ExibeMenu { get; set; }

        /// <summary>
        /// Gets or Sets a Resumo da DuvidaSubcategoria.
        /// </summary>
        public String Resumo { get; set; }
    }
}
