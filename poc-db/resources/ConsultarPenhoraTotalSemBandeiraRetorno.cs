using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarPenhoraTotalSemBandeiraRetorno : BasicContract
    {
        [DataMember]
        public DateTime DataProcesso { get; set; }
        [DataMember]
        public DateTime DataApresentacao { get; set; }
        [DataMember]
        public DateTime DataVencimento { get; set; }
        [DataMember]
        public int NumeroEstabelecimento { get; set; }
        [DataMember]
        public int NumeroRV { get; set; }
        [DataMember]
        public string TipoBandeira { get; set; }
        [DataMember]
        public int QuantidadeTransacoes { get; set; }
        [DataMember]
        public string DescricaoResumo { get; set; }
        [DataMember]
        public decimal ValorPenhorado { get; set; }
    }
}