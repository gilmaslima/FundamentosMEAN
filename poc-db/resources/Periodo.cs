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
    /// Classe Modelo Periodo
    /// </summary>
    [DataContract]
    public class Periodo
    {
        /// <summary>
        /// Início
        /// </summary>
        [DataMember]
        public DateTime Inicio { get; set; }

        /// <summary>
        /// Término
        /// </summary>
        [DataMember]
        public DateTime Termino { get; set; }
    }
}
