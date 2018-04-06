/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.Terminal
{
    /// <summary>
    /// Classe Modelo TipoTerminalTipoConexao
    /// </summary>
    [DataContract]
    public enum TipoTerminalTipoConexao
    {
        /// <summary>
        /// NAC
        /// </summary>
        [EnumMember]
        NAC,

        /// <summary>
        /// X25 dedicado
        /// </summary>
        [EnumMember]
        X25DEDICADO,

        /// <summary>
        /// X28
        /// </summary>
        [EnumMember]
        X28,
    }
}
