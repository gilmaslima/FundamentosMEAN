using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.DadosCadastrais.Modelo
{
    /// <summary>
    /// Produto Flex
    /// </summary>
    public class ProdutoFlex
    {
        /// <summary>
        /// Código da CCA
        /// </summary>
        public Int32 CodigoCCA { get; set; }

        /// <summary>
        /// Descrição da CCA
        /// </summary>
        public String DescricaoCCA { get; set; }

        /// <summary>
        /// Código da Feature
        /// </summary>
        public Int32 CodigoFeature { get; set; }

        /// <summary>
        /// Descrição da Feature
        /// </summary>
        public String DescricaoFeature { get; set; }

        /// <summary>
        /// Código do Patamar Início
        /// </summary>
        public Int32 CodigoPatamarInicio { get; set; }

        /// <summary>
        /// Código do Patamar Fim
        /// </summary>
        public Int32 CodigoPatamarFim { get; set; }

        /// <summary>
        /// Indicador da Situação do Registro
        /// </summary>
        public String IndicadorSituacaoRegistro { get; set; }

        /// <summary>
        /// Código Precificação Produto Redecard
        /// </summary>
        public Int32 CodigoPrecificacaoProdutoRedecard { get; set; }

        /// <summary>
        /// Valor Preço Variante 1
        /// </summary>
        public Decimal ValorPrecoVariante1 { get; set; }

        /// <summary>
        /// Valor Preço Variante 2
        /// </summary>
        public Decimal ValorPrecoVariante2 { get; set; }

        /// <summary>
        /// Quantidade Prazo Produto
        /// </summary>
        public Int32 QuantidadePrazoProduto { get; set; }

        /// <summary>
        /// Data Hora Aprovação
        /// </summary>
        public DateTime DataHoraAprovacao { get; set; }
        
        /// <summary>
        /// Data Hora Última Atualização
        /// </summary>
        public DateTime DataHoraUltimaAtualizacao { get; set; }

        /// <summary>
        /// Código OP ID Última Atualização
        /// </summary>
        public String CodigoOpIDUltimaAtualizacao { get; set; }
    }
}