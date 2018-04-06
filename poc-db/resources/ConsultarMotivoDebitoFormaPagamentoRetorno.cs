using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarMotivoDebitoFormaPagamentoRetorno
    {
        [DataMember]
        public string BandeiraResumoVendasDebitado { get; set; }
        [DataMember]
        public string DataPagamento { get; set; }
        [DataMember]
        public string EstabelecimentoDebito { get; set; }
        [DataMember]
        public string MeioPagamento { get; set; }
        [DataMember]
        public string ResumoVendaDebito { get; set; }
        [DataMember]
        public string SinalDebitoCredito { get; set; }
        [DataMember]
        public decimal ValorComplementar { get; set; }
        [DataMember]
        public decimal ValorDebito { get; set; }
        [DataMember]
        public decimal ValorPendente { get; set; }
    }
}