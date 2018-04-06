using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarOrdensCreditoEnviadosAoBancoAjusteRetorno : BasicContract
    {
        [DataMember]
        public string TipoBandeira { get; set; }
        [DataMember]
        public string DescricaoAjuste { get; set; }
        [DataMember]
        public int TotalTransacoes { get; set; }
        [DataMember]
        public decimal TotalValorCredito { get; set; }
    }
}