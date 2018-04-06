using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA1112 - Relatório de créditos suspensos, retidos e penhorados - Créditos penhorados.
    /// </summary>
    [DataContract]
    public class ConsultarPenhoraEnvio
    {
        [DataMember]
        public DateTime DataInicial { get; set; }
        [DataMember]
        public DateTime DataFinal { get; set; }
        [DataMember]
        public short CodigoBandeira { get; set; }
        [DataMember]
        public List<int> Estabelecimentos { get; set; }
    }
}
