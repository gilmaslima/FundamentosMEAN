/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo.HomePage
{
    /// <summary>
    /// Resumo por Data de Recebimento
    /// </summary>
    [DataContract]
    public class Resumo
    {
        /// <summary>Data de recebimento</summary>
        [DataMember]
        public DateTime DataRecebimento { get; set; }

        /// <summary>Valor bruto</summary>
        [DataMember]
        public Decimal ValorBruto { get; set; }

        /// <summary>Valor líquido</summary>
        [DataMember]
        public Decimal ValorLiquido { get; set; }
    }
}