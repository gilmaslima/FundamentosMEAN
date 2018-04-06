using System;

namespace Redecard.PN.OutrosServicos.Servicos
{
    /// <summary>
    /// Modelo para mas faixas de faturamento de meta da oferta maquininha conta certa
    /// </summary>
    public class MaquininhaMetas
    {
        /// <summary>
        /// Código do estabelecimento
        /// </summary>
        public Int32 NumeroPdv { get; set; }

        /// <summary>
        /// Valor combo da oferta
        /// </summary>
        public Decimal ValorComboMaquininha { get; set; }

        /// <summary>
        /// Valor combo do pacote
        /// </summary>
        public Decimal ValorComboPacote { get; set; }

        /// <summary>
        /// Valor do desconto
        /// </summary>
        public Decimal ValorDescontoPacote { get; set; }

        /// <summary>
        /// Valor da meta final
        /// </summary>
        public Decimal ValorMetaFinal { get; set; }

        /// <summary>
        /// Valor da meta inicial
        /// </summary>
        public Decimal ValorMetaInicial { get; set; }
    }
}
