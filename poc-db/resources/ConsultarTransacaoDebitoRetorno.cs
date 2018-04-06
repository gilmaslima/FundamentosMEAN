using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA799 - Resumo de vendas - CDC.
    /// </summary>
    [DataContract]
    public class ConsultarTransacaoDebitoRetorno
    {
        [DataMember]
        public int NumeroEstabelecimentoAnterior { get; set; }
        [DataMember]
        public int NumeroResumoVendaAnterior { get; set; }
        [DataMember]
        public DateTime DataApresentacaoAnterior { get; set; }
        [DataMember]
        public short QuantidadeOcorrencias { get; set; }
        [DataMember]
        public int QuantidadeComprovanteVenda { get; set; }
        [DataMember]
        public decimal ValorApresentado { get; set; }
        [DataMember]
        public decimal ValorLiquido { get; set; }
        [DataMember]
        public decimal ValorDesconto { get; set; }
        [DataMember]
        public DateTime DataVencimento { get; set; }
        [DataMember]
        public string IndicadorPreDatado { get; set; }
        [DataMember]
        public short CodigoTransacao { get; set; }
        [DataMember]
        public short SubCodigoTransacao { get; set; }
        [DataMember]
        public string DescricaoTransacao { get; set; }
        [DataMember]
        public decimal ValorDescontoTaxaCrediario { get; set; }
        [DataMember]
        public decimal ValorSaque { get; set; }
        [DataMember]
        public decimal ValorTotalCpmf { get; set; }
        [DataMember]
        public decimal ValorIof { get; set; }
        [DataMember]
        public string TipoResposta { get; set; }
        [DataMember]
        public string CodigoBandeira { get; set; }
    }
}