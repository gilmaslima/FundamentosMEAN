/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos.PlanoContas
{
    /// <summary>
    /// Japão - Celulares de recebimento da oferta
    /// </summary>
    [DataContract]
    public class OfertaCelular
    {
        /// <summary>
        /// Código da operadora
        /// </summary>
        [DataMember]
        public Int32 CodigoOperadora { get; set; }

        /// <summary>
        /// Nome da operadora
        /// </summary>
        [DataMember]
        public String NomeOperadora { get; set; }

        /// <summary>
        /// DDD do celular
        /// </summary>
        [DataMember]
        public Int16 DddCelular { get; set; }

        /// <summary>
        /// Número do celular
        /// </summary>
        [DataMember]
        public Decimal NumeroCelular { get; set; }

        /// <summary>
        /// Valor do bônus da faixa
        /// </summary>
        [DataMember]
        public Decimal ValorBonus { get; set; }

        /// <summary>
        /// Percentual do bônus do celular
        /// </summary>
        [DataMember]
        public Decimal PercentualBonus { get; set; }
    }
}