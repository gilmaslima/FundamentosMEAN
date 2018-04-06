using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redecard.PN.Extrato.Modelo
{
    /// <summary>
    /// WACA799 - Resumo de vendas - CDC.
    /// </summary>
    public class ConsultarTransacaoDebitoRetornoDTO
    {
        public int NumeroEstabelecimentoAnterior { get; set; }
        public int NumeroResumoVendaAnterior { get; set; }
        public DateTime DataApresentacaoAnterior { get; set; }
        public short QuantidadeOcorrencias { get; set; }
        public int QuantidadeComprovanteVenda { get; set; }
        public decimal ValorApresentado { get; set; }
        public decimal ValorLiquido { get; set; }
        public decimal ValorDesconto { get; set; }
        public DateTime DataVencimento { get; set; }
        public string IndicadorPreDatado { get; set; }
        public short CodigoTransacao { get; set; }
        public short SubCodigoTransacao { get; set; }
        public string DescricaoTransacao { get; set; }
        public decimal ValorDescontoTaxaCrediario { get; set; }
        public decimal ValorSaque { get; set; }
        public decimal ValorTotalCpmf { get; set; }
        public decimal ValorIof { get; set; }
        public string TipoResposta { get; set; }
        public string CodigoBandeira { get; set; }
    }
}
