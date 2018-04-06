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
    /// A class DuvidaPerguntaResposta e uma entidade da lista Dúvida Pergunta Resposta.
    /// </summary>
    public class DuvidaPerguntaResposta : BaseEntity
    {
        /// <summary>
        /// Gets or Sets a TituloReduzido da DuvidaPerguntaResposta.
        /// </summary>
        public String TituloReduzido { get; set; }

        /// <summary>
        /// Gets or Sets a Resposta da DuvidaPerguntaResposta.
        /// </summary>
        public String Resposta { get; set; }

        /// <summary>
        /// Gets or Sets a Resposta da DuvidaPerguntaResposta.
        /// </summary>
        public String RespostaTexto { get; set; }

        /// <summary>
        /// Gets or Sets a Ordem da DuvidaPerguntaResposta.
        /// </summary>
        public Int32 Ordem { get; set; }

        /// <summary>
        /// Gets or Sets a SubCategoriaID da DuvidaPerguntaResposta.
        /// </summary>
        public SPFieldLookupValue SubCategoria { get; set; }

        /// <summary>
        /// Gets or Sets a URLVideo da DuvidaPerguntaResposta.
        /// </summary>
        public String URLVideo { get; set; }
    }

}
