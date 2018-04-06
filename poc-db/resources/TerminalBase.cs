/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.Terminal
{
    /// <summary>
    /// Classe Modelo TerminalBase
    /// </summary>
    [DataContract]
    public class TerminalBase
    {
        /// <summary>
        /// Número lógico
        /// </summary>
        [DataMember]
        public String NumeroLogico { get; set; }

        /// <summary>
        /// Número série
        /// </summary>
        [DataMember]
        public String NumeroSerie { get; set; }
    }
}
