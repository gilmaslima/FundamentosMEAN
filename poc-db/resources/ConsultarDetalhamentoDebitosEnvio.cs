using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarDetalhamentoDebitosEnvio
    {
        [DataMember]
        public DateTime DataInicial { get; set; }
        [DataMember]
        public DateTime DataFinal { get; set; }
        [DataMember]
        public string TipoPesquisa { get; set; }
        [DataMember]
        public short CodigoBandeira { get; set; }
        [DataMember]
        public List<int> Estabelecimentos { get; set; }
        [DataMember]
        public string ChavePesquisa { get; set; }
        [DataMember]
        public VersaoDebitoDesagendamento? Versao { get; set; }
    }
}