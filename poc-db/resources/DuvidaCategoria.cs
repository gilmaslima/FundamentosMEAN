/*
© Copyright 2016 Rede S.A.
Autor : EMA
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// A class DuvidaCategoria é uma entidade da lista Dúvida de catetoria.
    /// </summary>
    public class DuvidaCategoria : BaseEntity
    {
        /// <summary>
        /// Gets or Sets a Ordem da DuvidaCategoria.
        /// </summary>
        public Int32 Ordem { get; set; }

        /// <summary>
        /// Gets or Sets a Icone da DuvidaCategoria.
        /// </summary>
        public String Icone { get; set; }
    }
}
