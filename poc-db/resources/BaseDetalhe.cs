using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [KnownType(typeof(TotalBandeiraMesSaldosEmAberto))]
    [KnownType(typeof(DetalheMesSaldosEmAberto))]
    [KnownType(typeof(ItemDetalheSaldosEmAberto))]
    [DataContract]
    public class BaseDetalhe
    {
        [DataMember]
        public String Tipo { get; set; }
    }
}