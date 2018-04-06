/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Enumeração TipoDia
    /// </summary>
    [DataContract]
    public enum TipoDia
    {
        /// <summary>
        /// Domingo
        /// </summary>
        [EnumMember]
        DOMINGO,

        /// <summary>
        /// Segunda
        /// </summary>
        [EnumMember]
        SEGUNDA,

        /// <summary>
        /// Terça
        /// </summary>
        [EnumMember]
        TERCA,

        /// <summary>
        /// Quarta
        /// </summary>
        [EnumMember]
        QUARTA,

        /// <summary>
        /// quinta
        /// </summary>
        [EnumMember]
        QUINTA,

        /// <summary>
        /// Sexta
        /// </summary>
        [EnumMember]
        SEXTA,

        /// <summary>
        /// Sábado
        /// </summary>
        [EnumMember]
        SABADO
    }
}