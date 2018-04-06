/*
© Copyright 2015 Rede S.A.
Autor : Agnaldo Costa
Empresa : Iteris Consultoria e Software
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.OutrosServicos.Modelo.PlanoContas
{
    /// <summary>
    /// Classe representando uma Meta de Oferta do Plano de Contas
    /// </summary>
    public class MetaOferta
    {
        /// <summary>
        /// Valor do Faturamento Inicial da Meta
        /// </summary>
        public Decimal ValorInicial { get; set; }

        /// <summary>
        /// Valor do Faturamento Final da Meta
        /// </summary>
        public Decimal ValorFinal { get; set; }

        /// <summary>
        /// Valor da Oferta da Meta
        /// </summary>
        public Decimal ValorOferta { get; set; }

        /// <summary>
        /// Percentual da Oferta da Meta
        /// </summary>
        public Decimal PercentualOferta { get; set; }

        /// <summary>
        /// Quantidade Terminais Percentual Oferta
        /// </summary>
        public Int16 QuantidadeTerminais { get; set; }
    }
}
