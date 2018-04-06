using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA617 - Resumo de vendas - Cartões de débito.
    /// </summary>
    [DataContract]
    public class ConsultarPreDatadosRetorno
    {
        [DataMember]
        public short CodigoSubTransacao { get; set; }
        [DataMember]
        public short CodigoTransacao { get; set; }
        [DataMember]
        public DateTime DataVencimento { get; set; }
        [DataMember]
        public decimal DescontoTaxaCredito { get; set; }
        [DataMember]
        public string Descricao { get; set; }
        [DataMember]
        public string DescricaoBandeira { get; set; }
        [DataMember]
        public short NumeroPeca { get; set; }
        [DataMember]
        public short QuantidadeComprovantesVenda { get; set; }
        [DataMember]
        public short QuantidadePecas { get; set; }
        [DataMember]
        public string ReservaDados { get; set; }
        [DataMember]
        public string TipoVenda { get; set; }
        [DataMember]
        public decimal ValorApresentado { get; set; }
        [DataMember]
        public decimal ValorDesconto { get; set; }
        [DataMember]
        public decimal ValorLiquido { get; set; }
        [DataMember]
        public decimal ValorSaque { get; set; }
        [DataMember]
        public decimal ValorTotalCpmf { get; set; }
        [DataMember]
        public decimal ValorTotalIof { get; set; }
    }
}