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
    /// A class AtendimentoDigital e uma entidade de atendimento digital.
    /// </summary>
    public class AtendimentoDigital : BaseEntity
    {
        /// <summary>
        /// Gets or Sets a Name da AtendimentoDigital.
        /// </summary>
        public String FileName { get; set; }

        /// <summary>
        /// Gets or Sets a File da AtendimentoDigital.
        /// </summary>
        public SPFile File { get; set; }

        /// <summary>
        /// Gets or Sets a ReadyByteFile da AtendimentoDigital.
        /// </summary>
        public Byte[] ReadyByteFile { get; set; }

    }
}
