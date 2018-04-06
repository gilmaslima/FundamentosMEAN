using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ItemDetalheSaldosEmAberto : BaseDetalhe
    {
        [DataMember]
        public DateTime DataReferencia { get; set; }
        [DataMember]
        public Int16 CodigoBandeira { get; set; }
        [DataMember]
        public Int16 CodigoBanco { get; set; }
        [DataMember]
        public Int16 CodigoAgencia { get; set; }
        [DataMember]
        public String ContaCorrente { get; set; }
        [DataMember]
        public Int32 CodigoEstabelecimento { get; set; }
        [DataMember]
        public Decimal ValorBruto { get; set; }
        [DataMember]
        public Decimal ValorLiquido { get; set; }
        [DataMember]
        public Int32 QuantidadeDetalhe { get; set; }

    }
}