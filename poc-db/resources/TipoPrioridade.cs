/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Enumeração TipoPrioridade
    /// </summary>
    [DataContract]
    public enum TipoPrioridade
    {
        /// <summary>
        /// Normal
        /// </summary>
        [EnumMember]
        NORMAL,

        /// <summary>
        /// Ouvidoria
        /// </summary>
        [EnumMember]
        OUVIDORIA,

        /// <summary>
        /// Relacionamento
        /// </summary>
        [EnumMember]
        RELACIONAMENTO
    }
}
