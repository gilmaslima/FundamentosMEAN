using System;

namespace Redecard.PN.OutrosServicos.Modelo
{
    /// <summary>
    /// Modelo para o histórico de apuração das metas da oferta maquininha conta certa
    /// </summary>
    public class MaquininhaHistoricoApuracao
    {
        /// <summary>
        /// Data/mês de apuração
        /// </summary>
        public DateTime DataMesApuracao { get; set; }

        /// <summary>
        /// Data/mês de referência
        /// </summary>
        public DateTime DataMesReferencia { get; set; }

        /// <summary>
        /// Valor do aluguel
        /// </summary>
        public Decimal ValorAluguelMaquininha { get; set; }
        
        /// <summary>
        /// Valor do faturamento apurado
        /// </summary>
        public Decimal ValorFaturamentoApurado { get; set; }

        /// <summary>
        /// Valor de desconto faixa final
        /// </summary>
        public Decimal ValorFinalFaixaDesconto { get; set; }

        /// <summary>
        /// Valor de desconto faixa inicial
        /// </summary>
        public Decimal ValorInicialFaixaDesconto { get; set; }

        /// <summary>
        /// Valor do pacote básico
        /// </summary>
        public Decimal ValorPacoteBasico { get; set; }

        /// <summary>
        /// Valor total do pacote
        /// </summary>
        public Decimal ValorTotalPacote { get; set; }
    }
}
