using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Emissores.Servicos
{
    [DataContract]
    public class InformacaoCobranca
    {
        [DataMember]
        public String Tipo { get; set; }
        [DataMember]
        public decimal PercentualInicialMarketShare { get; set; }
        [DataMember]
        public decimal PercentualFinalMarketShare { get; set; }
        [DataMember]
        public decimal PrecoMedioReferencia { get; set; }
        [DataMember]
        public decimal ValorTotalFaturamento { get; set; }
        [DataMember]
        public decimal ValorTotalCobrado { get; set; }
        [DataMember]
        public decimal PercentualMarketShare { get; set; }
    }
}