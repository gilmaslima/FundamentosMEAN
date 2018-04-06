/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Redecard.PN.OutrosServicos.Servicos.PlanoContas
{
    /// <summary>
    /// Japão - Faixa da Oferta no Aceite
    /// </summary>
    [DataContract]
    public class FaixaOfertaNoAceite
    {
        /// <summary>
        /// Código da Meta
        /// </summary>
        [DataMember]
        public Int16 CodigoMeta { get; set; }

        /// <summary>
        /// Valor inicial da faixa
        /// </summary>
        [DataMember]
        public Decimal ValorInicial { get; set; }

        /// <summary>
        /// Valor final da faixa
        /// </summary>
        [DataMember]
        public Decimal ValorFinal { get; set; }

        /// <summary>
        /// Valor do bônus da faixa
        /// </summary>
        [DataMember]
        public Decimal ValorBonus { get; set; }

        /// <summary>
        /// Equipamentos
        /// </summary>
        [DataMember]
        public List<FaixaOfertaNoAceiteEquipamento> Equipamentos { get; set; }
    }
}