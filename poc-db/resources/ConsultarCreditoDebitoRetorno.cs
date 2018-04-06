using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA1108 - Home - Lançamentos futuros.
    /// </summary>
    [DataContract]
    public class ConsultarCreditoDebitoRetorno
    {
        [DataMember]
        public DateTime Data { get; set; }
        [DataMember]
        public decimal ValorCredito { get; set; }
        [DataMember]
        public decimal ValorDebito { get; set; }
    }
}