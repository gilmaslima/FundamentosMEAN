using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class DadosConsultaSaldosEmAberto
    {
        [DataMember]
        public DateTime DataInicial { get; set; }
        [DataMember]
        public DateTime DataFinal { get; set; }
        [DataMember]
        public List<int> Estabelecimentos { get; set; }
        [DataMember]
        public String CodigoSolicitacao { get; set; }
    }
}