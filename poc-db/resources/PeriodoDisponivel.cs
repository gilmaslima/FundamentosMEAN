using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class PeriodoDisponivel
    {
        [DataMember]
        public DateTime DataPeriodoInicial { get; set; }
        [DataMember]
        public DateTime DataPeriodoFinal { get; set; }
        [DataMember]
        public String CodigoSolicitacao { get; set; }

    }
}