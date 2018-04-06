using System;

namespace Redecard.PN.DadosCadastrais.Modelo.InformacaoComercial
{
    /// <summary>
    /// Define os valores de resposta para o serviço 
    /// de DadosPrecoUnicoPv.
    /// </summary>
    public class DadosPrecoUnicoPv
    {
        /// <summary>
        /// Define um Valor do Serviço.
        /// </summary>
        public Decimal ValorServico { get; set; }

        /// <summary>
        /// Define a Razão Social
        /// </summary>
        public String RazaoSocial { get; set; }

        /// <summary>
        /// Define o código da Oferta.
        /// </summary>
        public Int32 CodigoOferta { get; set; }

        /// <summary>
        /// Define o número da proposta.
        /// </summary>
        public Int32 NumeroProposta { get; set; }

        /// <summary>
        /// Define a situação da proposta.
        /// 'A' Ativa ou 'I' Inativa
        /// </summary>
        public String SituacaoProposta { get; set; }

        /// <summary>
        /// Define uma Data Início de Vigência da Proposta.
        /// </summary>
        public DateTime DataInicioVigenciaProposta { get; set; }

        /// <summary>
        /// Define uma Data Fim de Vigência da Proposta.
        /// </summary>
        public DateTime DataFimVigenciaProposta { get; set; }

        /// <summary>
        /// Define um Indicador de Renovação Automática.
        /// </summary>
        public String IndicadorRenovacaoAutomatica { get; set; }

        /// <summary>
        /// Define uma Quantidade de Renovação Automática.
        /// </summary>
        public Int16 QuantidadeRenovacaoAutomatica { get; set; }

        /// <summary>
        /// Define um Valor de Faturamento do Contrato.
        /// </summary>
        public Decimal ValorFaturamentoContrato { get; set; }

        /// <summary>
        /// Define um Valor de Preço Único sem Flex.
        /// </summary>
        public Decimal ValorPrecoUnicoSemFlex { get; set; }

        /// <summary>
        /// Define um Valor de Preço Único com Flex.
        /// </summary>
        public Decimal ValorPrecoUnicoComFlex { get; set; }

        /// <summary>
        /// Define uma lista de Terminais.
        /// </summary>
        public TerminalContratado[] Terminais { get; set; }

        /// <summary>
        /// Define uma lista de Features.
        /// </summary>
        public Feature[] Features { get; set; }
    }
}