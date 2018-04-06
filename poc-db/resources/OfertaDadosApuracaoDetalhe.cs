/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Redecard.PN.OutrosServicos.Modelo.PlanoContas
{
    /// <summary>
    /// Japão - Detalhes dos dados da apuração da oferta
    /// </summary>
    public class OfertaDadosApuracaoDetalhe
    {
        /// <summary>
        /// DDD do celular
        /// </summary>
        public Int16 DddCelular { get; set; }

        /// <summary>
        /// Número do celular
        /// </summary>
        public Decimal NumeroCelular { get; set; }

        /// <summary>
        /// Data do fim da apuração
        /// </summary>
        public DateTime? DataFimApuracao { get; set; }

        /// <summary>
        /// Valor do bônus creditado
        /// </summary>
        public Decimal ValorBonusCreditado { get; set; }

        /// <summary>
        /// Data de previsão do crédito
        /// </summary>
        public DateTime? DataPrevisaoCredito { get; set; }

        /// <summary>
        /// Data do crédito
        /// </summary>
        public DateTime? DataCredito { get; set; }

        /// <summary>
        /// Observação
        /// </summary>
        public String Observacao { get; set; }
    }
}