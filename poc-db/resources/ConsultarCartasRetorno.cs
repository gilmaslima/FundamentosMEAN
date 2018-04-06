using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA1116 - Consultar por transação - Carta.
    /// </summary>
    [DataContract]
    public class ConsultarCartasRetorno
    {
        [DataMember]
        public short CodigoMotivo { get; set; }
        [DataMember]
        public DateTime DataCancelamento { get; set; }
        [DataMember]
        public DateTime DataVenda { get; set; }
        [DataMember]
        public string DescricaoMotivo { get; set; }
        [DataMember]
        public string NumeroCartao { get; set; }
        [DataMember]
        public int NumeroEstabelecimento { get; set; }
        [DataMember]
        public decimal NumeroNsu { get; set; }
        [DataMember]
        public decimal NumeroProcesso { get; set; }
        [DataMember]
        public int NumeroResumo { get; set; }
        [DataMember]
        public decimal ValorAjuste { get; set; }
        [DataMember]
        public decimal ValorCancelamento { get; set; }
        [DataMember]
        public decimal ValorDebito { get; set; }
        [DataMember]
        public decimal ValorTransacao { get; set; }
    }
}