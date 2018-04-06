using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class DetalheMesSaldosEmAberto : BaseDetalhe
    {
        //[DataMember]
        //public List<TotalBandeiraMesSaldosEmAberto> TotalBandeiraMes { get; set; }
        [DataMember]
        public DateTime DataReferencia { get; set; }
        [DataMember]
        public Decimal ValorLiquido { get; set; }

    }
}