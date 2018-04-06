using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    /// <summary>
    /// WACA1107 - Home - Últimas Vendas.
    /// </summary>
    [DataContract]
    public class ConsultarTransacoesCreditoDebitoEnvio
    {
        [DataMember]
        public DateTime DataInicial { get; set; }
        [DataMember]
        public DateTime DataFinal { get; set; }
        [DataMember]
        public List<int> Estabelecimentos { get; set; }
    }
}