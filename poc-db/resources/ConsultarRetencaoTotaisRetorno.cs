using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA1113 - Relatório de créditos suspensos, retidos e penhorados - Créditos retidos.
    /// </summary>
    [DataContract]
    public class ConsultarRetencaoTotaisRetorno
    {
        [DataMember]
        public int TotalTransacoes { get; set; }
        [DataMember]
        public int TotalProcessos { get; set; }
        [DataMember]
        public decimal TotalValorProcesso { get; set; }
        [DataMember]
        public decimal TotalValorRetencao { get; set; }
    }
}
