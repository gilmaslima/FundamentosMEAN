using System;

namespace Redecard.PN.DadosCadastrais.Modelo.InformacaoComercial
{
    /// <summary>
    /// Define os valores de resposta para o serviço 
    /// de Informação Comercial.
    /// </summary>
    public class ServicoContratado
    {
        /// <summary>
        /// Define uma Descrição do Serviço.
        /// </summary>
        public String Descricao { get; set; }

        /// <summary>
        /// Define uma Quantidade Inicial.
        /// </summary>
        public Decimal? QuantidadeInicial { get; set; }

        /// <summary>
        /// Define uma Quantidade Final.
        /// </summary>
        public Decimal? QuantidadeFinal { get; set; }

        /// <summary>
        /// Define uma Descrição de Quantidade.
        /// </summary>
        public String DescricaoQuantidade { get; set; }

        /// <summary>
        /// Define o Valor.
        /// </summary>
        public Decimal? Valor { get; set; }
    }
}