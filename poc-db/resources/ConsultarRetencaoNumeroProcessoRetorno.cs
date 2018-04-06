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
    public class ConsultarRetencaoNumeroProcessoRetorno : BasicContract
    {
        [DataMember]
        public string NumeroProcesso { get; set; }
        [DataMember]
        public decimal ValorTotalProcesso { get; set; }
        [DataMember]
        public int QuantidadeDetalheProcesso { get; set; }
    }
}
