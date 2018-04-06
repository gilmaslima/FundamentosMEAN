/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Enumeração TipoOrigem
    /// </summary>
    [DataContract]
    public enum TipoOrigem
    {
        /// <summary>
        /// TG
        /// </summary>
        [EnumMember]
        TG,

        /// <summary>
        /// TGA
        /// </summary>
        [EnumMember]
        TGA,
    }
}
