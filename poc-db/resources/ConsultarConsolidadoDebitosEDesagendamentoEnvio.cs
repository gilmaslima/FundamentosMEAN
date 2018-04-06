using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarConsolidadoDebitosEDesagendamentoEnvio
    {
        [DataMember]
        public DateTime DataInicial { get; set; }
        [DataMember]
        public DateTime DataFinal { get; set; }
        [DataMember]
        public int[] Estabelecimentos { get; set; }
        [DataMember]
        public VersaoDebitoDesagendamento? Versao { get; set; }
    }
}