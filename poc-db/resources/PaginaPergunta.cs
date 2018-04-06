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
    /// A class PaginaPergunta é uma entidade da lista Página Pergunta    
    /// </summary>
    public class PaginaPergunta : BaseEntity
    {
        /// <summary>
        /// Gets or Sets a TituloPrincipal da PaginaPergunta.
        /// </summary>
        public String TituloPrincipal { get; set; }

        /// <summary>
        /// Gets or Sets a TextoPrincipal da PaginaPergunta.
        /// </summary>
        public String TextoPrincipal { get; set; }

        /// <summary>
        /// Gets or Sets a Pergunta da PaginaPergunta.
        /// </summary>
        public SPFieldLookupValue Pergunta { get; set; }

        /// <summary>
        /// Gets or Sets a TituloSecundario da PaginaPergunta.
        /// </summary>
        public String TituloSecundario { get; set; }

        /// <summary>
        /// Gets or Sets a TextoSecundario da PaginaPergunta.
        /// </summary>
        public String TextoSecundario { get; set; }

        /// <summary>
        /// Gets or Sets a UrlPagina da PaginaPergunta.
        /// </summary>
        public String UrlPagina { get; set; }

        /// <summary>
        /// Gets or Sets a Ordem da PaginaPergunta.
        /// </summary>
        public Int32 Ordem { get; set; }
    }
}
