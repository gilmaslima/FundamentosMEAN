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
    public class ConsultarPenhoraNumeroProcessoRetorno : BasicContract
    {
        [DataMember]
        public string NumeroProcesso { get; set; }
        [DataMember]
        public decimal ValorTotalProcesso { get; set; }
        [DataMember]
        public int QuantidadeDetalheProcesso { get; set; }
    }
}
