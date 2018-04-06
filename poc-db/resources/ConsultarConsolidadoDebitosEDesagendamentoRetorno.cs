using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarConsolidadoDebitosEDesagendamentoRetorno
    {
        [DataMember]
        public string ChavePesquisa { get; set; }
        [DataMember]
        public decimal ValorPendenteDebito { get; set; }
        [DataMember]
        public decimal ValorPendenteLiquido { get; set; }
        [DataMember]
        public decimal ValorPendente { get; set; }
        [DataMember]
        public decimal ValorLiquidadoDebito { get; set; }
        [DataMember]
        public decimal ValorLiquidadoLiquido { get; set; }
    }
}