/*
© Copyright 2016 Rede S.A.
Autor : EMA
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Rede.PN.AtendimentoDigital.SharePoint.Repository
{
    /// <summary>
    /// A class DuvidaFrequente é uma entidade da lista Dúvida frequente.
    /// </summary>
    public class DuvidaFrequente : BaseEntity
    {
        /// <summary>
        /// Gets or Sets a IDPerguntaResposta da DuvidaFrequente.
        /// </summary>
        public Int32 IDPerguntaResposta { get; set; }

        /// <summary>
        /// Gets or Sets a Ordem da DuvidaFrequente.
        /// </summary>
        public Int32 Ordem { get; set; }
    }
}
