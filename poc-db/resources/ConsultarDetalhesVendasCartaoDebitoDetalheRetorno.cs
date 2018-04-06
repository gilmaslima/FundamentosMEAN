using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA668 - Resumo de vendas - Cartões de débito - CV's aceitos.
    /// </summary>
    [DataContract]
    public class ConsultarDetalhesVendasCartaoDebitoDetalheRetorno
    {
        [DataMember]
        public DateTime DataComprovanteVenda { get; set; }
        [DataMember]
        public DateTime DataVencimento { get; set; }
        [DataMember]
        public string Descricao { get; set; }
        [DataMember]
        public int HorasComprovanteVenda { get; set; }
        [DataMember]
        public string NumeroCartao { get; set; }
        [DataMember]
        public decimal NumeroTransacao { get; set; }
        [DataMember]
        public short QuantidadeParcelas { get; set; }
        [DataMember]
        public decimal ValorComplementar { get; set; }
        [DataMember]
        public decimal ValorComprovanteRenda { get; set; }
        [DataMember]
        public decimal ValorLiquido { get; set; }
        [DataMember]
        public decimal ValorSaque { get; set; }
        [DataMember]
        public string VendaCancelada { get; set; }
    }
}