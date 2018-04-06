using System;

namespace Redecard.PN.DadosCadastrais.Modelo.InformacaoComercial
{
    /// <summary>
    /// Define os valores de resposta para o serviço 
    /// de ValoresCobrancaServicos.
    /// </summary>
    public class Feature
    {
        /// <summary>
        /// Define um Código de CCA.
        /// </summary>
        public Int16 CodigoCCA { get; set; }

        /// <summary>
        /// Define um Código de Feature.
        /// </summary>
        public Int16 CodigoFeature { get; set; }

        /// <summary>
        /// Define um Código de CCA Flex.
        /// </summary>
        public Int16 CodigoCCAFlex { get; set; }

        /// <summary>
        /// Define um Código de Feature Flex.
        /// </summary>
        public Int16 CodigoFeatureFlex { get; set; }

        /// <summary>
        /// Define um indicador de Produto Flex.
        /// </summary>
        public String IndicadorProdutoFlex { get; set; }

        /// <summary>
        /// Define o percentual da Taxa 1.
        /// </summary>
        public Decimal PercentualTaxa1 { get; set; }

        /// <summary>
        /// Define o percentual da Taxa 2.
        /// </summary>
        public Decimal PercentualTaxa2 { get; set; }

        /// <summary>
        /// Define o percentual da Taxa 1.
        /// </summary>
        public Int16 QuantidadeDiasPrazo { get; set; }

        /// <summary>
        /// Define a quantidade de parcelas inicial.
        /// </summary>
        public Int16 QuantidadeParcelasInicial { get; set; }

        /// <summary>
        /// Define a quantidade de parcelas final.
        /// </summary>
        public Int16 QuantidadeParcelasFinal { get; set; }
    }
}