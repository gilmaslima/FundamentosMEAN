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
    /// Japão - Compensações de Débitos de Aluguel
    /// </summary>
    [DataContract]
    public class CompensacaoDebitoAluguel
    {
        /// <summary>
        /// Indicador Net:<br/>
        /// - (C) Compensado com Resumo de Crédito<br/>
        /// - (D) Compensado com Resumo de Débito<br/>
        /// - (O) Outras Compensações
        /// </summary>
        [DataMember]
        public String IndicadorNet { get; set; }

        /// <summary>
        /// Número do Resumo da OC Netada
        /// </summary>
        [DataMember]
        public Decimal NumeroResumoVenda { get; set; }

        /// <summary>
        /// Descrição do tipo de pagamento do débito
        /// </summary>
        [DataMember]
        public String DescricaoTipoPagamento { get; set; }

        /// <summary>
        /// Data de compensação do débito
        /// </summary>
        [DataMember]
        public DateTime? DataCompensacao { get; set; }

        /// <summary>
        /// Valor compensado
        /// </summary>
        [DataMember]
        public Decimal ValorCompensado { get; set; }
    }
}