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
    /// Classe Modelo VendaDigitadaTerminal
    /// </summary>
    [DataContract]
    public class VendaDigitadaTerminal : VendaDigitada
    {
        /// <summary>
        /// CVC2 Obrigatório
        /// </summary>
        [DataMember]
        public Boolean? Cvc2Obrigatorio { get; set; }
    }
}
