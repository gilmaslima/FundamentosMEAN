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
    /// Classe Modelo MesValor
    /// </summary>
    [DataContract]
    public class MesValor
    {
        /// <summary>
        /// Mês
        /// </summary>
        [DataMember]
        public TipoMeses Mes { get; set; }

        /// <summary>
        /// Valor
        /// </summary>
        [DataMember]
        public Double Valor { get; set; }
    }
}
