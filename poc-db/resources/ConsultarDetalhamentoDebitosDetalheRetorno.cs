using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarDetalhamentoDebitosDetalheRetorno
    {
        [DataMember]
        public string Bandeira { get; set; }
        [DataMember]
        public DateTime DataInclusao { get; set; }
        [DataMember]
        public DateTime DataPagamento { get; set; }
        [DataMember]
        public int EstabelecimentoOrigem { get; set; }
        [DataMember]
        public string IndicadorDesagendamento { get; set; }
        [DataMember]
        public string MotivoDebito { get; set; }
        [DataMember]
        public string ProcessoReferente { get; set; }
        [DataMember]
        public string Resumo { get; set; }
        [DataMember]
        public string IndicadorDebitoCredito { get; set; }
        [DataMember]
        public string Timestamp { get; set; }
        [DataMember]
        public string TipoRegistro { get; set; }
        [DataMember]
        public decimal ValorCompensado { get; set; }
        [DataMember]
        public decimal ValorDebito { get; set; }
        [DataMember]
        public decimal ValorPendente { get; set; }
        [DataMember]
        public decimal NumeroDebito { get; set; }
    }
}