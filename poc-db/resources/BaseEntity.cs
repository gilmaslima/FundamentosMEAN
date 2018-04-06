/*
© Copyright 2016 Rede S.A.
Autor : EMA
Empresa : Iteris Consultoria e Software
*/

using System;
namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// BaseEntity
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Representa o campo Id 
        /// </summary>
        public Int32 Id { get; set; }

        /// <summary>
        /// Representa o campo Title 
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Representa uma Entity que gerou esta entidade.
        /// </summary>
        public BaseEntity Parent { get; set; }

    }
}
