using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Redecard.PN.Extrato.Servicos.Modelo
{
    [DataContract]
    public class ConsultarOrdensCreditoEnviadosAoBancoRetorno
    {
        /// <summary>
        /// D1 = ConsultarOrdensCreditoEnviadosAoBancoDetalheRetorno, D2 = ConsultarOrdensCreditoEnviadosAoBancoAjusteRetorno
        /// </summary>
        [DataMember]
        public List<BasicContract> Registros { get; set; }
        [DataMember]
        public ConsultarOrdensCreditoEnviadosAoBancoTotaisRetorno Totais { get; set; }
        [DataMember]
        public int QuantidadeTotalRegistros { get; set; }
    }
}