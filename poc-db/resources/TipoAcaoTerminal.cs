/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Enumeração TipoAcaoTerminal
    /// </summary>
    [DataContract]
    public enum TipoAcaoTerminal
    {
        /// <summary>
        /// Instalação
        /// </summary>
        [EnumMember]
        INSTALACAO,

        /// <summary>
        /// Desinstalação
        /// </summary>
        [EnumMember]
        DESINSTALACAO
    }
}
