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
    /// Classe Modelo EventoOS
    /// </summary>
    [DataContract]
    public class EventoOS
    {
        /// <summary>
        /// Código
        /// </summary>
        [DataMember]
        public String Codigo { get; set; }

        /// <summary>
        /// Início
        /// </summary>
        [DataMember]
        public DateTime? Inicio { get; set; }

        /// <summary>
        /// Término
        /// </summary>
        [DataMember]
        public DateTime? Termino { get; set; }        
    }
}
