/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.Terminal
{
    /// <summary>
    /// Classe Modelo TipoTerminalProprietario
    /// </summary>
    [DataContract]
    public enum TipoTerminalProprietario
    {
        /// <summary>
        /// Redecard
        /// </summary>
        [EnumMember]
        REDECARD,

        /// <summary>
        /// Integrador
        /// </summary>
        [EnumMember]
        INTEGRADOR,

        /// <summary>
        /// Fabricante
        /// </summary>
        [EnumMember]
        FABRICANTE,

        /// <summary>
        /// Cliente
        /// </summary>
        [EnumMember]
        CLIENTE,

        /// <summary>
        /// Cielo
        /// </summary>
        [EnumMember]
        CIELO,

        /// <summary>
        /// Amex
        /// </summary>
        [EnumMember]
        AMEX,

        /// <summary>
        /// Outros
        /// </summary>
        [EnumMember]
        OUTROS,

        /// <summary>
        /// Não se aplica
        /// </summary>
        [EnumMember]
        NAOSEAPLICA
    }
}
