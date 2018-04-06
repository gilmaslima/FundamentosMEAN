/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos.PlanoContas
{
    /// <summary>
    /// Tipo da oferta
    /// </summary>
    [DataContract]
    public enum TipoOferta : short
    {
        /// <summary>
        /// Sem oferta cadastrada
        /// </summary>
        [EnumMember]
        SemOferta           = 0,

        /// <summary>
        /// Oferta Plano de Contas (diferente de Japão e Turquia)
        /// </summary>
        [EnumMember]
        OutrasOfertas   = 1,

        /// <summary>
        /// Oferta Japão
        /// </summary>
        [EnumMember]
        OfertaJapao         = 2,

        /// <summary>
        /// Oferta Turquia
        /// </summary>
        [EnumMember]
        OfertaTurquia       = 3
    }
}