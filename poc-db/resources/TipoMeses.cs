/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Enumeração TipoMeses
    /// </summary>
    [DataContract]
    public enum TipoMeses
    {
        /// <summary>
        /// Janeiro
        /// </summary>
        [EnumMember]
        JANEIRO,

        /// <summary>
        /// Fevereiro
        /// </summary>
        [EnumMember]
        FEVEREIRO,

        /// <summary>
        /// Março
        /// </summary>
        [EnumMember]
        MARCO,

        /// <summary>
        /// Abril
        /// </summary>
        [EnumMember]
        ABRIL,

        /// <summary>
        /// Maio
        /// </summary>
        [EnumMember]
        MAIO,

        /// <summary>
        /// Junho
        /// </summary>
        [EnumMember]
        JUNHO,

        /// <summary>
        /// Julho
        /// </summary>
        [EnumMember]
        JULHO,

        /// <summary>
        /// Agosto
        /// </summary>
        [EnumMember]
        AGOSTO,

        /// <summary>
        /// Setembro
        /// </summary>
        [EnumMember]
        SETEMBRO,

        /// <summary>
        /// Outubro
        /// </summary>
        [EnumMember]
        OUTUBRO,

        /// <summary>
        /// Novembro
        /// </summary>
        [EnumMember]
        NOVEMBRO,

        /// <summary>
        /// Dezembro
        /// </summary>
        [EnumMember]
        DEZEMBRO
    }
}
