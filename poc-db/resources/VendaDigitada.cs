/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Classe Modelo VendaDigitada
    /// </summary>
    [DataContract]
    public class VendaDigitada
    {
        /// <summary>
        /// Habilitada
        /// </summary>
        [DataMember]
        public Boolean Habilitada { get; set; }

        /// <summary>
        /// Habilitada receptivo
        /// </summary>
        [DataMember]
        public Boolean? HabilitadaReceptivo { get; set; }
    }
}
