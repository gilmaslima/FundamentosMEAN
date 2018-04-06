using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarDetalhamentoDebitosTotaisRetorno
    {
        [DataMember]
        public decimal TotalValorDevido { get; set; }
        [DataMember]
        public decimal TotalValorCompensado { get; set; }
    }
}