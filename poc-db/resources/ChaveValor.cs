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
    /// Classe Modelo ChaveValor
    /// </summary>
    [DataContract]
    public class ChaveValor
    {
        /// <summary>
        /// Chave
        /// </summary>
        [DataMember]
        public String Chave { get; set; }

        /// <summary>
        /// Valor
        /// </summary>
        [DataMember]
        public String Valor { get; set; }
    }
}
