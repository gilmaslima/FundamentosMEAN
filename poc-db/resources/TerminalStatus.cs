/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.Terminal
{
    /// <summary>
    /// Classe Modelo TerminalStatus
    /// </summary>
    [DataContract]
    public class TerminalStatus : TerminalBase
    {
        /// <summary>
        /// Status
        /// </summary>
        [DataMember]
        public TipoTerminalStatus Status { get; set; }
    }
}
