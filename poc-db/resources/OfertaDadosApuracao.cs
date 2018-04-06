/*
© Copyright 2014 Rede S.A.
Autor : Alexandre Shiroma
Empresa : Iteris Consultoria e Software
*/

using System;

namespace Redecard.PN.OutrosServicos.Modelo.PlanoContas
{
    /// <summary>
    /// Japão - Dados da apuração da oferta
    /// </summary>
    public class OfertaDadosApuracao
    {
        /// <summary>
        /// Mês de referência da apuração
        /// </summary>
        public DateTime MesReferencia { get; set; }

        /// <summary>
        /// Data do início da apuração
        /// </summary>
        public DateTime? DataInicioApuracao { get; set; }
       
        /// <summary>
        /// Data do fum da apuração
        /// </summary>
        public DateTime? DataFimApuracao { get; set; }

        /// <summary>
        /// Código da meta atingida
        /// </summary>
        public Int16 CodigoMeta { get; set; }

        /// <summary>
        /// Valor inicial da meta atingida
        /// </summary>
        public Decimal ValorInicial { get; set; }

        /// <summary>
        /// Valor final da meta atingida
        /// </summary>
        public Decimal ValorFinal { get; set; }

        /// <summary>
        /// Valor faturamento realizado
        /// </summary>
        public Decimal ValorRealizado { get; set; }

        /// <summary>
        /// Quantidade de terminais do estabelecimento
        /// </summary>
        public Int16 QuantidadeTerminais { get; set; }

        /// <summary>
        /// Valor total do aluguel
        /// </summary>
        public Decimal ValorTotalAluguel { get; set; }

        /// <summary>
        /// Valor compensado até o dia 10
        /// </summary>
        public Decimal ValorCompensado { get; set; }

        /// <summary>
        /// Valor do bônus creditado
        /// </summary>
        public Decimal ValorBonusCreditado { get; set; }

        /// <summary>
        /// Indicador da meta
        /// </summary>
        public Boolean IndicadorMeta { get; set; }

        /// <summary>
        /// Indicador do pagamento
        /// </summary>
        public Boolean IndicadorPagamento { get; set; }
    }
}