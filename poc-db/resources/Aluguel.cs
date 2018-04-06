/*
© Copyright 2013 Redecard S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Redecard.PN.Maximo.Modelo.OrdemServico
{
    /// <summary>
    /// Classe Modelo Aluguel
    /// </summary>
    [DataContract]
    public class Aluguel
    {
        /// <summary>
        /// Valor unitário
        /// </summary>
        [DataMember]
        public Double ValorUnitario { get; set; }

        /// <summary>
        /// Isento
        /// </summary>
        [DataMember]
        public Boolean Isento { get; set; }

        /// <summary>
        /// Data início cobrança
        /// </summary>
        [DataMember]
        public DateTime? DataInicioCobranca { get; set; }

        /// <summary>
        /// Escalonamento
        /// </summary>
        [DataMember]
        public List<MesValor> Escalonamento { get; set; }

        /// <summary>
        /// Sazonalidade
        /// </summary>
        [DataMember]
        public List<MesValor> Sazonalidade { get; set; }
    }
}
