using System;
using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Servicos
{
    [DataContract]
    /// <summary>
    /// Totalizador de Pré-Pagamentos do Emissor
    /// </summary>
    public class SaldoPrePagamento
    {
        /// <summary>
        /// Data de Vencimento
        /// </summary>
        [DataMember]
        public DateTime Vencimento { get; set; }

        /// <summary>
        /// Total de Saldo a pagar
        /// </summary>
        [DataMember]
        public decimal SaldoPagar { get; set; }

        /// <summary>
        /// Total de Valor já antecidado
        /// </summary>
        [DataMember]
        public decimal ValorAntecipado { get; set; }

        /// <summary>
        /// Total de Valor Líquido
        /// </summary>
        [DataMember]
        public decimal SaldoLiquido { get; set; }

    }
}