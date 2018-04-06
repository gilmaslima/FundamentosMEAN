using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA1116 - Consultar por transação - Carta.
    /// </summary>
    [DataContract]
    public class ConsultarCartasEnvio
    {
        [DataMember]
        public decimal NumeroProcesso { get; set; }
        [DataMember]
        public string TimestampTransacao { get; set; }
        [DataMember]
        public short SistemaDados { get; set; }
    }
}