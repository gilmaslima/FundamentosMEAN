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
    /// Classe Modelo Chip
    /// </summary>
    [DataContract]
    public class Chip
    {
        /// <summary>
        /// ICCID
        /// </summary>
        [DataMember]
        public String Iccid { get; set; }

        /// <summary>
        /// Operadora
        /// </summary>
        [DataMember]
        public String Operadora { get; set; }
    }
}
