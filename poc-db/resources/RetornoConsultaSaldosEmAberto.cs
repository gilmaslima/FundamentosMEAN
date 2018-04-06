using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class RetornoConsultaSaldosEmAberto
    {
        [DataMember]
        public List<BaseDetalhe> Detalhe { get; set; }
        [DataMember]
        public TotaisPorBandeiraSaldosEmAberto TotalBandeiras { get; set; }
        [DataMember]
        public int QuantidadeTotalRegistros { get; set; }
    }
}