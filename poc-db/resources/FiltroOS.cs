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
    /// Classe Modelo FiltroOS
    /// </summary>
    [DataContract]
    public class FiltroOS
    {
        /// <summary>
        /// Número OS
        /// </summary>
        [DataMember]
        public String NumeroOs { get; set; }

        /// <summary>
        /// Ponto Venda
        /// </summary>
        [DataMember]
        public String PontoVenda { get; set; }

        /// <summary>
        /// Data abertura
        /// </summary>
        [DataMember]
        public Periodo DataAbertura { get; set; }
    }
}
