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
    /// Classe Modelo OSAtendimentoBase
    /// </summary>
    [DataContract]
    public class OSAtendimentoBase
    {
        /// <summary>
        /// Número OS
        /// </summary>
        [DataMember]
        public String NumeroOs { get; set; }

        /// <summary>
        /// Data atendimento
        /// </summary>
        [DataMember]
        public DateTime? DataAtendimento { get; set; }
    }
}
